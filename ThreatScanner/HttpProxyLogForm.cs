// HttpProxyLogForm.cs
using Microsoft.Playwright;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreatScanner.Helpers;

namespace ThreatScanner
{
    public partial class HttpProxyLogForm : Form
    {
        // ── CDP / Playwright state ──────────────────────────────────────────────
        private CdpHelper _cdp;
        private IBrowserContext _context;
        private IPage _interceptPage;          // page we attach RouteAsync to
        private bool _capturing = false;

        private readonly ConcurrentDictionary<string, ProxyEntry> _entriesById =
            new ConcurrentDictionary<string, ProxyEntry>();
        private readonly ConcurrentDictionary<string, int> _rowIndexByRequestId =
            new ConcurrentDictionary<string, int>();
        private readonly ConcurrentDictionary<string, DateTime> _requestStartById =
            new ConcurrentDictionary<string, DateTime>();

        private long _totalCaptured = 0;
        private ProxyEntry _selectedEntry = null;

        private CodeViewerHelper.CodeViewerControl _reqBodyViewer;
        private CodeViewerHelper.CodeViewerControl _respBodyViewer;

        private static readonly HttpClient _replayClient = new HttpClient();

        private readonly System.Windows.Forms.Timer _filterDebounce =
            new System.Windows.Forms.Timer { Interval = 200 };

        // ── INTERCEPT state ────────────────────────────────────────────────────
        // Pending intercepts: requestId → TaskCompletionSource that the route
        // handler blocks on.  Resolving it with true = forward, false = drop.
        private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pendingIntercepts =
            new ConcurrentDictionary<string, TaskCompletionSource<bool>>();

        // The actual IRoute for each pending request (needed to call ContinueAsync / AbortAsync)
        private readonly ConcurrentDictionary<string, IRoute> _pendingRoutes =
            new ConcurrentDictionary<string, IRoute>();

        // Which resource types are being intercepted (driven by the checkboxes)
        private readonly HashSet<string> _interceptedTypes =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // ── ctor ───────────────────────────────────────────────────────────────

        public HttpProxyLogForm()
        {
            InitializeComponent();
            ApplyOutputTheme();
            ScanHelpers.EnableRowDeletion(dataGridView_Requests);

            _reqBodyViewer = CodeViewerHelper.Create();
            _reqBodyViewer.Panel.Dock = DockStyle.Fill;
            tabPage_ReqBody.Controls.Add(_reqBodyViewer.Panel);

            _respBodyViewer = CodeViewerHelper.Create();
            _respBodyViewer.Panel.Dock = DockStyle.Fill;
            tabPage_RespBody.Controls.Add(_respBodyViewer.Panel);

            _cdp = new CdpHelper((msg, isError) =>
                Log(msg, isError ? Color.OrangeRed : (Color?)null));

            _filterDebounce.Tick += (s, e) =>
            {
                _filterDebounce.Stop();
                ReapplyAllRowFilters();
            };

            // Wire intercept checkboxes
            chk_Intercept_XHR.CheckedChanged += OnInterceptCheckChanged;
            chk_Intercept_Fetch.CheckedChanged += OnInterceptCheckChanged;
            chk_Intercept_Document.CheckedChanged += OnInterceptCheckChanged;
            chk_Intercept_Script.CheckedChanged += OnInterceptCheckChanged;
            chk_Intercept_Image.CheckedChanged += OnInterceptCheckChanged;
            chk_Intercept_Font.CheckedChanged += OnInterceptCheckChanged;
            chk_Intercept_CSS.CheckedChanged += OnInterceptCheckChanged;
            chk_Intercept_Other.CheckedChanged += OnInterceptCheckChanged;

            // Wire intercept queue buttons
            btn_Intercept_Forward.Click += Btn_Intercept_Forward_Click;
            btn_Intercept_Drop.Click += Btn_Intercept_Drop_Click;
            btn_Intercept_ForwardAll.Click += Btn_Intercept_ForwardAll_Click;
            btn_Intercept_DropAll.Click += Btn_Intercept_DropAll_Click;

            this.FormClosing += HttpProxyLogForm_FormClosing;
            SetCapturing(false);
            ShowDetail(null);
            UpdateInterceptQueueUI();
        }

        // ── START / STOP ─────────────────────────────────────────────────────────

        private async void button_StartCapture_Click(object sender, EventArgs e)
        {
            if (_capturing) return;
            SetCapturing(true);
            Log("[Proxy] Connecting to browser…");

            try
            {
                IPage page = await _cdp.GetOrCreateActivePageAsync();
                _context = page.Context;
                _interceptPage = page;

                _context.Request += OnRequest;
                _context.Response += OnResponse;
                _context.RequestFailed += OnRequestFailed;

                // Register a catch-all route; we decide per-request whether to intercept
                await page.RouteAsync("**/*", HandleRouteAsync);

                Log("[Proxy] Capturing started. Browse normally — every request the browser makes will be logged here.", Color.LimeGreen);
                if (_interceptedTypes.Count > 0)
                    Log("[Intercept] Active for: " + string.Join(", ", _interceptedTypes), Color.Cyan);
            }
            catch (Exception ex)
            {
                Log("[Proxy] Failed to attach: " + ex.Message, Color.OrangeRed);
                SetCapturing(false);
            }
        }

        private void button_StopCapture_Click(object sender, EventArgs e) => StopCapture();

        private void StopCapture()
        {
            if (!_capturing) return;

            // Release all pending intercepts so their handler threads don't leak
            foreach (var kv in _pendingIntercepts)
                kv.Value.TrySetResult(true);   // forward anything still held

            _pendingIntercepts.Clear();
            _pendingRoutes.Clear();

            if (_context != null)
            {
                try
                {
                    _context.Request -= OnRequest;
                    _context.Response -= OnResponse;
                    _context.RequestFailed -= OnRequestFailed;
                }
                catch { }
            }

            if (_interceptPage != null)
            {
                try { _ = _interceptPage.UnrouteAllAsync(); } catch { }
                _interceptPage = null;
            }

            SetCapturing(false);
            Log("[Proxy] Capturing stopped.", Color.Orange);
            RunOnUi(UpdateInterceptQueueUI);
        }

        private void HttpProxyLogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCapture();
            try { _cdp?.Dispose(); } catch { }
            try { _filterDebounce.Stop(); _filterDebounce.Dispose(); } catch { }
        }

        // ── ROUTE HANDLER (intercept gate) ────────────────────────────────────
        // Called by Playwright on its own thread for every request that matches "**/*".
        // If interception is enabled for this resource type, we pause here until the
        // user clicks Forward or Drop.  Otherwise we immediately continue.

        private async Task HandleRouteAsync(IRoute route)
        {
            IRequest req = route.Request;
            string type = req.ResourceType ?? "other";
            string id = RequestKey(req);

            bool shouldIntercept;
            lock (_interceptedTypes)
                shouldIntercept = _interceptedTypes.Contains(type) ||
                                  (_interceptedTypes.Contains("other") &&
                                   !new[] { "xhr", "fetch", "document", "script", "image", "font", "stylesheet" }.Contains(type));

            if (!shouldIntercept)
            {
                await route.ContinueAsync();
                return;
            }

            // Park the request — add to the intercept queue UI, then block
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pendingIntercepts[id] = tcs;
            _pendingRoutes[id] = route;

            RunOnUi(() => AddToInterceptQueue(id, req.Method, type, req.Url));

            bool forward = await tcs.Task;   // blocks until user decides

            _pendingIntercepts.TryRemove(id, out _);
            _pendingRoutes.TryRemove(id, out _);

            try
            {
                if (forward) await route.ContinueAsync();
                else await route.AbortAsync();
            }
            catch { /* page may have navigated away */ }
        }

        // ── REQUEST / RESPONSE EVENTS ─────────────────────────────────────────

        private async void OnRequest(object sender, IRequest request)
        {
            string filter = GetFilterTextThreadSafe();
            if (!string.IsNullOrEmpty(filter) &&
                request.Url.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                return;

            string id = RequestKey(request);
            _requestStartById[id] = DateTime.Now;

            List<KeyValuePair<string, string>> headers;
            try { headers = (await request.AllHeadersAsync())?.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value)).ToList(); }
            catch { headers = new List<KeyValuePair<string, string>>(); }
            string body;
            try { body = request.PostData; } catch { body = null; }

            var entry = new ProxyEntry
            {
                Time = DateTime.Now,
                Method = request.Method,
                Url = request.Url,
                ResourceType = request.ResourceType,
                RequestHeaders = headers ?? new List<KeyValuePair<string, string>>(),
                RequestBody = body ?? "",
            };
            entry.CorrelationId = CorrelationHeaders.Extract(entry.RequestHeaders);
            _entriesById[id] = entry;

            string time = entry.Time.ToString("HH:mm:ss.fff");

            RunOnUi(() =>
            {
                int rowIndex = dataGridView_Requests.Rows.Add(
                    time, request.Method, "…", request.ResourceType, "", entry.CorrelationId ?? "", request.Url);
                var row = dataGridView_Requests.Rows[rowIndex];
                row.Tag = entry;
                entry.GridRow = row;
                _rowIndexByRequestId[id] = rowIndex;
                _totalCaptured++;
                UpdateCounter();
                ApplyRowFilterVisibility(row, entry);
            });
        }

        private async void OnResponse(object sender, IResponse response)
        {
            string id = RequestKey(response.Request);
            int size = 0;
            string respBody = null;
            try
            {
                var bodyBytes = await response.BodyAsync();
                size = bodyBytes?.Length ?? 0;
                if (bodyBytes != null)
                {
                    try { respBody = Encoding.UTF8.GetString(bodyBytes); }
                    catch { respBody = null; }
                }
            }
            catch { }

            List<KeyValuePair<string, string>> respHeaders;
            try { respHeaders = (await response.AllHeadersAsync())?.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value)).ToList(); }
            catch { respHeaders = new List<KeyValuePair<string, string>>(); }

            double? durationMs = null;
            if (_requestStartById.TryRemove(id, out var startedAt))
                durationMs = (DateTime.Now - startedAt).TotalMilliseconds;

            ProxyEntry entry = null;
            if (_entriesById.TryGetValue(id, out entry))
            {
                entry.Status = response.Status;
                entry.StatusText = response.StatusText;
                entry.ResponseHeaders = respHeaders ?? new List<KeyValuePair<string, string>>();
                entry.ResponseBody = respBody ?? "";
                entry.SizeBytes = size;
                entry.DurationMs = durationMs;
                entry.ResponseContentType = entry.ResponseHeaders
                    .FirstOrDefault(h => h.Key.Equals("content-type", StringComparison.OrdinalIgnoreCase)).Value;
                if (entry.CorrelationId == null)
                    entry.CorrelationId = CorrelationHeaders.Extract(entry.ResponseHeaders);
                try { entry.Timing = response.Request.Timing; } catch { }
            }

            RunOnUi(() =>
            {
                if (_rowIndexByRequestId.TryGetValue(id, out int rowIndex) &&
                    rowIndex < dataGridView_Requests.Rows.Count)
                {
                    var row = dataGridView_Requests.Rows[rowIndex];
                    row.Cells["col_Status"].Value = response.Status.ToString();
                    row.Cells["col_Size"].Value = FormatSize(size);
                    if (entry != null && !string.IsNullOrEmpty(entry.CorrelationId))
                        row.Cells["col_Correlation"].Value = entry.CorrelationId;

                    row.DefaultCellStyle.ForeColor = response.Status >= 400
                        ? Color.Firebrick
                        : (response.Status >= 300 ? Color.DarkOrange : dataGridView_Requests.DefaultCellStyle.ForeColor);

                    if (entry != null) ApplyRowFilterVisibility(row, entry);
                    if (entry != null && entry == _selectedEntry) ShowDetail(entry);
                }
            });
        }

        private void OnRequestFailed(object sender, IRequest request)
        {
            string id = RequestKey(request);
            _requestStartById.TryRemove(id, out _);

            ProxyEntry entry = null;
            if (_entriesById.TryGetValue(id, out entry))
                entry.Error = "Request failed";

            RunOnUi(() =>
            {
                if (_rowIndexByRequestId.TryGetValue(id, out int rowIndex) &&
                    rowIndex < dataGridView_Requests.Rows.Count)
                {
                    var row = dataGridView_Requests.Rows[rowIndex];
                    row.Cells["col_Status"].Value = "FAILED";
                    row.DefaultCellStyle.ForeColor = Color.Firebrick;
                    if (entry != null) ApplyRowFilterVisibility(row, entry);
                }
            });
        }

        private static string RequestKey(IRequest request) =>
            request.GetHashCode().ToString();

        // ── INTERCEPT QUEUE UI ────────────────────────────────────────────────

        private void OnInterceptCheckChanged(object sender, EventArgs e)
        {
            lock (_interceptedTypes)
            {
                _interceptedTypes.Clear();
                if (chk_Intercept_XHR.Checked) _interceptedTypes.Add("xhr");
                if (chk_Intercept_Fetch.Checked) _interceptedTypes.Add("fetch");
                if (chk_Intercept_Document.Checked) _interceptedTypes.Add("document");
                if (chk_Intercept_Script.Checked) _interceptedTypes.Add("script");
                if (chk_Intercept_Image.Checked) _interceptedTypes.Add("image");
                if (chk_Intercept_Font.Checked) _interceptedTypes.Add("font");
                if (chk_Intercept_CSS.Checked) _interceptedTypes.Add("stylesheet");
                if (chk_Intercept_Other.Checked) _interceptedTypes.Add("other");
            }

            string active = _interceptedTypes.Count > 0
                ? "Intercepting: " + string.Join(", ", _interceptedTypes)
                : "Interception off";
            label_InterceptStatus.Text = active;
            label_InterceptStatus.ForeColor = _interceptedTypes.Count > 0
                ? Color.FromArgb(234, 88, 12)   // orange-600
                : Color.FromArgb(100, 116, 139); // slate-500

            if (_interceptedTypes.Count > 0 && _capturing)
                Log("[Intercept] Now intercepting: " + string.Join(", ", _interceptedTypes), Color.Cyan);
        }

        // Add a row to the intercept queue listview
        private void AddToInterceptQueue(string id, string method, string type, string url)
        {
            var item = new ListViewItem(method);
            item.SubItems.Add(type);
            item.SubItems.Add(url);
            item.Tag = id;
            item.ForeColor = Color.FromArgb(234, 88, 12);
            listView_InterceptQueue.Items.Add(item);
            UpdateInterceptQueueUI();
            Log($"[Intercept] ⏸ {method} [{type}] {url}", Color.FromArgb(234, 179, 8));
        }

        private void RemoveFromInterceptQueue(string id)
        {
            foreach (ListViewItem item in listView_InterceptQueue.Items)
            {
                if (item.Tag as string == id)
                {
                    listView_InterceptQueue.Items.Remove(item);
                    break;
                }
            }
            UpdateInterceptQueueUI();
        }

        private void UpdateInterceptQueueUI()
        {
            int count = listView_InterceptQueue.Items.Count;
            label_InterceptQueueCount.Text = count == 0
                ? "No requests held"
                : $"{count} request{(count == 1 ? "" : "s")} held";
            label_InterceptQueueCount.ForeColor = count > 0
                ? Color.FromArgb(234, 88, 12)
                : Color.FromArgb(100, 116, 139);

            bool hasSelected = listView_InterceptQueue.SelectedItems.Count > 0;
            btn_Intercept_Forward.Enabled = hasSelected;
            btn_Intercept_Drop.Enabled = hasSelected;
            btn_Intercept_ForwardAll.Enabled = count > 0;
            btn_Intercept_DropAll.Enabled = count > 0;
        }

        private void listView_InterceptQueue_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInterceptQueueUI();
        }

        // Forward selected
        private void Btn_Intercept_Forward_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView_InterceptQueue.SelectedItems)
                ResolveIntercept(item.Tag as string, forward: true);
        }

        // Drop selected
        private void Btn_Intercept_Drop_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView_InterceptQueue.SelectedItems)
                ResolveIntercept(item.Tag as string, forward: false);
        }

        // Forward all
        private void Btn_Intercept_ForwardAll_Click(object sender, EventArgs e)
        {
            var ids = listView_InterceptQueue.Items.Cast<ListViewItem>()
                      .Select(i => i.Tag as string).ToList();
            foreach (var id in ids) ResolveIntercept(id, forward: true);
        }

        // Drop all
        private void Btn_Intercept_DropAll_Click(object sender, EventArgs e)
        {
            var ids = listView_InterceptQueue.Items.Cast<ListViewItem>()
                      .Select(i => i.Tag as string).ToList();
            foreach (var id in ids) ResolveIntercept(id, forward: false);
        }

        private void ResolveIntercept(string id, bool forward)
        {
            if (id == null) return;
            if (_pendingIntercepts.TryGetValue(id, out var tcs))
            {
                tcs.TrySetResult(forward);
                Log(forward ? $"[Intercept] ▶ Forwarded" : $"[Intercept] ✖ Dropped",
                    forward ? Color.LimeGreen : Color.OrangeRed);
            }
            RemoveFromInterceptQueue(id);
        }

        // ── DETAIL PANE ───────────────────────────────────────────────────────

        private void dataGridView_Requests_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView_Requests.SelectedRows.Count == 0) { ShowDetail(null); return; }
            var entry = dataGridView_Requests.SelectedRows[0].Tag as ProxyEntry;
            ShowDetail(entry);
        }

        private void ShowDetail(ProxyEntry entry)
        {
            _selectedEntry = entry;
            bool has = entry != null;
            button_Replay.Enabled = has;
            button_CopyCurl.Enabled = has;
            button_CopyHttpClient.Enabled = has;

            label_CorrelationValue.Text = has && !string.IsNullOrEmpty(entry.CorrelationId) ? entry.CorrelationId : "—";

            listView_ReqHeaders.BeginUpdate();
            listView_RespHeaders.BeginUpdate();
            try
            {
                listView_ReqHeaders.Items.Clear();
                listView_RespHeaders.Items.Clear();
                if (has)
                {
                    foreach (var h in entry.RequestHeaders)
                        listView_ReqHeaders.Items.Add(new ListViewItem(new[] { h.Key, h.Value }));
                    foreach (var h in entry.ResponseHeaders)
                        listView_RespHeaders.Items.Add(new ListViewItem(new[] { h.Key, h.Value }));
                }
            }
            finally
            {
                listView_ReqHeaders.EndUpdate();
                listView_RespHeaders.EndUpdate();
            }

            var reqCt = entry?.RequestHeaders.FirstOrDefault(h => h.Key.Equals("content-type", StringComparison.OrdinalIgnoreCase)).Value;
            _reqBodyViewer.SetContent(has ? entry.RequestBody : "", CodeViewerHelper.DetectLanguage(reqCt));
            _respBodyViewer.SetContent(has ? entry.ResponseBody : "", CodeViewerHelper.DetectLanguage(entry?.ResponseContentType));
            textBox_Timing.Text = has ? BuildTimingText(entry) : "Select a request above to see timing details.";
        }

        private static string BuildTimingText(ProxyEntry entry)
        {
            var sb = new StringBuilder();
            sb.AppendLine("URL:          " + entry.Url);
            sb.AppendLine("Method:       " + entry.Method);
            sb.AppendLine("Status:       " + (entry.Status > 0 ? entry.Status + " " + entry.StatusText : (entry.Error ?? "pending")));
            sb.AppendLine("Size:         " + FormatSize((int)entry.SizeBytes));
            sb.AppendLine("Total time:   " + (entry.DurationMs.HasValue ? Math.Round(entry.DurationMs.Value, 1) + " ms" : "n/a"));
            sb.AppendLine();
            var t = entry.Timing;
            if (t != null)
            {
                sb.AppendLine("Timing breakdown (ms, relative to request start):");
                void Line(string label, double? v) => sb.AppendLine(string.Format("  {0,-16}{1}", label, v.HasValue && v >= 0 ? v.Value.ToString("0.#") : "n/a"));
                Line("DNS lookup:", (t.DomainLookupEnd >= 0 && t.DomainLookupStart >= 0) ? t.DomainLookupEnd - t.DomainLookupStart : (double?)null);
                Line("Connect:", (t.ConnectEnd >= 0 && t.ConnectStart >= 0) ? t.ConnectEnd - t.ConnectStart : (double?)null);
                Line("TLS handshake:", (t.SecureConnectionStart >= 0 && t.ConnectEnd >= 0) ? t.ConnectEnd - t.SecureConnectionStart : (double?)null);
                Line("Request sent:", t.RequestStart >= 0 ? t.RequestStart : (double?)null);
                Line("Waiting (TTFB):", (t.ResponseStart >= 0 && t.RequestStart >= 0) ? t.ResponseStart - t.RequestStart : (double?)null);
                Line("Content download:", (t.ResponseEnd >= 0 && t.ResponseStart >= 0) ? t.ResponseEnd - t.ResponseStart : (double?)null);
            }
            else sb.AppendLine("(Detailed CDP timing not available for this request.)");
            return sb.ToString();
        }

        // ── REPLAY / EXPORT ──────────────────────────────────────────────────

        private async void button_Replay_Click(object sender, EventArgs e)
        {
            if (_selectedEntry == null) return;
            var entry = _selectedEntry;
            button_Replay.Enabled = false;
            Log("[Replay] " + entry.Method + " " + entry.Url);
            try
            {
                using (var req = new HttpRequestMessage(new HttpMethod(entry.Method), entry.Url))
                {
                    var skip = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                        { "host", "content-length", "connection", "accept-encoding" };
                    string contentType = "application/json";
                    foreach (var h in entry.RequestHeaders)
                    {
                        if (skip.Contains(h.Key) || h.Key.StartsWith(":")) continue;
                        if (h.Key.Equals("content-type", StringComparison.OrdinalIgnoreCase)) { contentType = h.Value; continue; }
                        req.Headers.TryAddWithoutValidation(h.Key, h.Value);
                    }
                    if (!string.IsNullOrEmpty(entry.RequestBody))
                        req.Content = new StringContent(entry.RequestBody, Encoding.UTF8, contentType.Split(';')[0]);

                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    using (var resp = await _replayClient.SendAsync(req))
                    {
                        sw.Stop();
                        string body = await resp.Content.ReadAsStringAsync();
                        Log(string.Format("[Replay] → {0} ({1} bytes, {2} ms)", (int)resp.StatusCode, body.Length, sw.ElapsedMilliseconds),
                            (int)resp.StatusCode >= 400 ? Color.OrangeRed : Color.LimeGreen);
                    }
                }
            }
            catch (Exception ex) { Log("[Replay] Failed: " + ex.Message, Color.OrangeRed); }
            finally { button_Replay.Enabled = true; }
        }

        private void button_CopyCurl_Click(object sender, EventArgs e)
        {
            if (_selectedEntry == null) return;
            Clipboard.SetText(RequestExporter.ToCurl(_selectedEntry));
            Log("[Proxy] cURL command copied to clipboard.", Color.LimeGreen);
        }

        private void button_CopyHttpClient_Click(object sender, EventArgs e)
        {
            if (_selectedEntry == null) return;
            Clipboard.SetText(RequestExporter.ToHttpClientSnippet(_selectedEntry));
            Log("[Proxy] HttpClient snippet copied to clipboard.", Color.LimeGreen);
        }

        // ── FILTERING ─────────────────────────────────────────────────────────

        private void textBox_Filter_TextChanged(object sender, EventArgs e)
        {
            _filterDebounce.Stop();
            _filterDebounce.Start();
        }

        private void comboBox_Filters_SelectedIndexChanged(object sender, EventArgs e) => ReapplyAllRowFilters();

        private void ReapplyAllRowFilters()
        {
            dataGridView_Requests.SuspendLayout();
            try
            {
                foreach (DataGridViewRow row in dataGridView_Requests.Rows)
                {
                    var entry = row.Tag as ProxyEntry;
                    if (entry != null) ApplyRowFilterVisibility(row, entry);
                }
            }
            finally { dataGridView_Requests.ResumeLayout(); }
        }

        private void ApplyRowFilterVisibility(DataGridViewRow row, ProxyEntry entry)
        {
            bool visible = true;

            string methodSel = comboBox_MethodFilter.SelectedItem as string;
            if (!string.IsNullOrEmpty(methodSel) && methodSel != "All Methods" &&
                !string.Equals(entry.Method, methodSel, StringComparison.OrdinalIgnoreCase))
                visible = false;

            string statusSel = comboBox_StatusFilter.SelectedItem as string;
            if (visible && !string.IsNullOrEmpty(statusSel) && statusSel != "All Status")
            {
                switch (statusSel)
                {
                    case "2xx": visible = entry.Status >= 200 && entry.Status < 300; break;
                    case "3xx": visible = entry.Status >= 300 && entry.Status < 400; break;
                    case "4xx": visible = entry.Status >= 400 && entry.Status < 500; break;
                    case "5xx": visible = entry.Status >= 500 && entry.Status < 600; break;
                    case "Failed": visible = entry.Error != null; break;
                    case "Pending": visible = entry.IsPending; break;
                }
            }

            row.Visible = visible;
        }

        // ── UI helpers ────────────────────────────────────────────────────────

        private void RunOnUi(Action action)
        {
            if (IsDisposed) return;
            try { if (InvokeRequired) BeginInvoke(action); else action(); }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        private string GetFilterTextThreadSafe()
        {
            string result = "";
            RunOnUiBlocking(() => result = textBox_Filter.Text.Trim());
            return result;
        }

        private void RunOnUiBlocking(Action action)
        {
            if (!IsHandleCreated) { action(); return; }
            if (InvokeRequired) Invoke(action);
            else action();
        }

        private void UpdateCounter()
        {
            label_Counter.Text = string.Format("{0} request{1} captured",
                _totalCaptured, _totalCaptured == 1 ? "" : "s");
        }

        private static string FormatSize(int bytes)
        {
            if (bytes <= 0) return "—";
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024.0).ToString("0.#") + " KB";
            return (bytes / 1024.0 / 1024.0).ToString("0.#") + " MB";
        }

        private void SetCapturing(bool capturing)
        {
            _capturing = capturing;
            progressBar_Capture.Style = capturing ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            button_StartCapture.Enabled = !capturing;
            button_StopCapture.Enabled = capturing;
            textBox_Filter.Enabled = true;
        }

        private void button_ClearLog_Click(object sender, EventArgs e)
        {
            dataGridView_Requests.Rows.Clear();
            _rowIndexByRequestId.Clear();
            _entriesById.Clear();
            _requestStartById.Clear();
            _totalCaptured = 0;
            UpdateCounter();
            ShowDetail(null);
            ScanHelpers.ClearOutputRtb(richTextBox_Output);
        }

        private void button_SaveLog_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "http_proxy_log_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv"
            })
            {
                if (sfd.ShowDialog(this) != DialogResult.OK) return;
                var sb = new StringBuilder();
                sb.AppendLine("Time,Method,Status,Type,Size,CorrelationId,Url");
                foreach (DataGridViewRow row in dataGridView_Requests.Rows)
                {
                    if (row.IsNewRow) continue;
                    sb.AppendLine(string.Join(",", new[]
                    {
                        CsvEscape(row.Cells["col_Time"].Value),
                        CsvEscape(row.Cells["col_Method"].Value),
                        CsvEscape(row.Cells["col_Status"].Value),
                        CsvEscape(row.Cells["col_Type"].Value),
                        CsvEscape(row.Cells["col_Size"].Value),
                        CsvEscape(row.Cells["col_Correlation"].Value),
                        CsvEscape(row.Cells["col_Url"].Value)
                    }));
                }
                File.WriteAllText(sfd.FileName, sb.ToString());
                Log("[Proxy] Saved log to " + sfd.FileName, Color.LimeGreen);
            }
        }

        private static string CsvEscape(object value)
        {
            string s = value?.ToString() ?? "";
            if (s.IndexOfAny(new[] { ',', '"', '\n' }) >= 0)
                s = "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        private void Log(string message, Color? color = null) =>
            ScanHelpers.LogRtb(richTextBox_Output, color == Color.OrangeRed ? "❌" : (color == Color.LimeGreen ? "✅" : "ℹ️"), message);

        private void ApplyOutputTheme()
        {
            dataGridView_Requests.BackgroundColor = Color.White;
            richTextBox_Output.BackColor = Color.FromArgb(15, 23, 42);
            richTextBox_Output.ForeColor = Color.FromArgb(212, 212, 212);
        }
    }
}
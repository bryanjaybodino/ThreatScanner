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
using System.Windows.Forms;
using ThreatScanner.Helpers;

namespace ThreatScanner
{
    public partial class HttpProxyLogForm : Form
    {
        // ── CDP / Playwright state ──────────────────────────────────────────────
        private CdpHelper _cdp;
        private IBrowserContext _context;
        private bool _capturing = false;

        // Request id (object hash of the Playwright IRequest) -> captured entry.
        // The grid row itself carries the same entry via DataGridViewRow.Tag,
        // so either the id or the row can be used to look an entry back up.
        private readonly ConcurrentDictionary<string, ProxyEntry> _entriesById =
            new ConcurrentDictionary<string, ProxyEntry>();
        private readonly ConcurrentDictionary<string, int> _rowIndexByRequestId =
            new ConcurrentDictionary<string, int>();
        private readonly ConcurrentDictionary<string, DateTime> _requestStartById =
            new ConcurrentDictionary<string, DateTime>();

        private long _totalCaptured = 0;
        private ProxyEntry _selectedEntry = null;

        // Code viewer panels for the Request/Response body tabs — built once
        // and re-filled via SetContent() whenever the grid selection changes.
        private CodeViewerHelper.CodeViewerControl _reqBodyViewer;
        private CodeViewerHelper.CodeViewerControl _respBodyViewer;

        private static readonly HttpClient _replayClient = new HttpClient();

        // Debounces textBox_Filter so typing doesn't re-scan every row in the
        // grid on every single keystroke — only after the user pauses.
        private readonly System.Windows.Forms.Timer _filterDebounce = new System.Windows.Forms.Timer { Interval = 200 };


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

            this.FormClosing += HttpProxyLogForm_FormClosing;
            SetCapturing(false);
            ShowDetail(null);
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

                _context.Request += OnRequest;
                _context.Response += OnResponse;
                _context.RequestFailed += OnRequestFailed;

                Log("[Proxy] Capturing started. Browse normally — every request the browser makes will be logged here.", Color.LimeGreen);
            }
            catch (Exception ex)
            {
                Log("[Proxy] Failed to attach: " + ex.Message, Color.OrangeRed);
                SetCapturing(false);
            }
        }

        private void button_StopCapture_Click(object sender, EventArgs e)
        {
            StopCapture();
        }

        private void StopCapture()
        {
            if (!_capturing) return;

            if (_context != null)
            {
                try
                {
                    _context.Request -= OnRequest;
                    _context.Response -= OnResponse;
                    _context.RequestFailed -= OnRequestFailed;
                }
                catch { /* context may already be gone */ }
            }

            SetCapturing(false);
            Log("[Proxy] Capturing stopped.", Color.Orange);
        }

        private void HttpProxyLogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCapture();
            try { _cdp?.Dispose(); } catch { }
            try { _filterDebounce.Stop(); _filterDebounce.Dispose(); } catch { }
        }

        // ── EVENT HANDLERS (these fire on Playwright's own thread) ──────────────

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
                    catch { respBody = null; /* binary payload, e.g. images/fonts */ }
                }
            }
            catch { /* some responses (e.g. redirects, opaque) have no body */ }

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
                    if (entry != null && entry == _selectedEntry) ShowDetail(entry); // refresh open detail live
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

        // Playwright's IRequest doesn't expose the raw CDP requestId, but each
        // IRequest instance is stable for the lifetime of that request, so the
        // object's identity hash is a perfectly good correlation key here.
        private static string RequestKey(IRequest request) =>
            request.GetHashCode().ToString();

        // ── DETAIL PANE ───────────────────────────────────────────────────────

        private void dataGridView_Requests_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView_Requests.SelectedRows.Count == 0)
            {
                ShowDetail(null);
                return;
            }
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
            else
            {
                sb.AppendLine("(Detailed CDP timing not available for this request.)");
            }
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
            catch (Exception ex)
            {
                Log("[Replay] Failed: " + ex.Message, Color.OrangeRed);
            }
            finally
            {
                button_Replay.Enabled = true;
            }
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
            // Restart the debounce timer instead of filtering immediately —
            // typing a 6-character search term used to mean 6 full passes
            // over every row in the grid, each one re-triggering DataGridView
            // layout. Now it's one pass, 200ms after the user stops typing.
            _filterDebounce.Stop();
            _filterDebounce.Start();
        }

        private void comboBox_Filters_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReapplyAllRowFilters();
        }

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
            finally
            {
                dataGridView_Requests.ResumeLayout();
            }
        }

        /// <summary>
        /// The substring filter only governs which *new* requests get logged
        /// (cheaper than discarding afterwards), but the method/status combo
        /// filters apply retroactively to whatever's already in the grid.
        /// </summary>
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
            try
            {
                if (InvokeRequired) BeginInvoke(action);
                else action();
            }
            catch (ObjectDisposedException) { /* form closed mid-flight */ }
            catch (InvalidOperationException) { /* handle not yet created */ }
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
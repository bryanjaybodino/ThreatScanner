using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreatScanner.Helpers;

namespace ThreatScanner
{
    /// <summary>
    ///   Subdomain scanner:
    ///   - Passive discovery via Certificate Transparency logs (crt.sh)
    ///   - Active discovery via DNS wordlist brute-force (200+ common names)
    ///   - Wildcard-DNS detection so brute-force doesn't return false positives
    ///   - Live HTTP/HTTPS probing of every discovered subdomain (status + title)
    ///   - DataGridView output with Name / Status / Response columns (copyable)
    ///
    ///   PERFORMANCE NOTE:
    ///   Row additions and progress updates no longer marshal to the UI thread
    ///   one-at-a-time via BeginInvoke. Worker threads enqueue results into
    ///   thread-safe queues, and a single UI Timer drains them in batches every
    ///   100ms. This avoids flooding the UI message queue during high-throughput
    ///   phases (DNS brute-force, HTTP probing), which previously caused the
    ///   grid to become unresponsive / appear frozen.
    /// </summary>
    public partial class SubdomainScannerForm : Form
    {
        private readonly HttpClient _http = ScanHelpers.BuildDefaultClient();

        // ── Scan state ───────────────────────────────────────────────────────────
        private readonly HashSet<string> _found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private CancellationTokenSource _cts;

        // Wildcard DNS fingerprint — if the registrar/host resolves *.domain.com to
        // the same IP(s) for ANY random label, brute-force results are unreliable
        // unless we detect and account for it.
        private HashSet<string> _wildcardIps = null;

        // How many subdomains to probe over HTTP concurrently.
        private const int ProbeConcurrency = 20;

        // ── Progress tracking ───────────────────────────────────────────────────
        private int _totalWork;
        private int _doneWork;

        // ── Batched UI update plumbing ──────────────────────────────────────────
        // Worker threads never touch the UI directly. They enqueue rows / set the
        // pending percentage, and a single Timer tick drains everything in one
        // shot. This keeps cross-thread marshalling to ~10 calls/sec regardless
        // of how fast the scan is producing results.
        private readonly ConcurrentQueue<(string name, string status, string response)> _pendingRows
            = new ConcurrentQueue<(string name, string status, string response)>();
        private System.Windows.Forms.Timer _uiFlushTimer;
        private volatile int _pendingPct = -1; // -1 = no pending progress update

        public SubdomainScannerForm()
        {
            InitializeComponent();
            InitUiFlushTimer();
            EnableDoubleBuffering(dataGridView_Output);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  UI BATCHING
        // ════════════════════════════════════════════════════════════════════════

        private void InitUiFlushTimer()
        {
            _uiFlushTimer = new System.Windows.Forms.Timer { Interval = 100 };
            _uiFlushTimer.Tick += (s, e) => FlushPendingUi();
            _uiFlushTimer.Start();
        }

        /// <summary>Runs on the UI thread every 100ms. Drains whatever rows have
        /// piled up since the last tick and applies them in one batch, plus the
        /// latest pending progress percentage (last-write-wins — intermediate
        /// values are intentionally dropped, we only care about the latest).</summary>
        private void FlushPendingUi()
        {
            if (!_pendingRows.IsEmpty)
            {
                dataGridView_Output.SuspendLayout();

                bool wasAtBottom = dataGridView_Output.Rows.Count == 0
                    || (dataGridView_Output.FirstDisplayedScrollingRowIndex >= 0
                        && dataGridView_Output.FirstDisplayedScrollingRowIndex
                           + dataGridView_Output.DisplayedRowCount(false)
                           >= dataGridView_Output.Rows.Count - 2);

                int lastIdx = -1;
                while (_pendingRows.TryDequeue(out var item))
                {
                    int idx = dataGridView_Output.Rows.Add(item.name, item.status, item.response);
                    var row = dataGridView_Output.Rows[idx];

                    Color fore = Color.FromArgb(226, 232, 240);
                    Color back = Color.FromArgb(15, 23, 42);

                    if (item.status.StartsWith("🚨") || item.status.StartsWith("❌"))
                        fore = Color.FromArgb(248, 113, 113);
                    else if (item.status.StartsWith("⚠️"))
                        fore = Color.FromArgb(251, 191, 36);
                    else if (item.status.StartsWith("✅"))
                        fore = Color.FromArgb(52, 211, 153);
                    else if (item.status.StartsWith("ℹ️"))
                        fore = Color.FromArgb(96, 165, 250);

                    row.Cells["colStatus"].Style.ForeColor = fore;
                    row.Cells["colStatus"].Style.BackColor = back;

                    lastIdx = idx;
                }

                if (wasAtBottom && lastIdx >= 0)
                    dataGridView_Output.FirstDisplayedScrollingRowIndex = lastIdx;

                dataGridView_Output.ResumeLayout();
            }

            int pct = _pendingPct;
            if (pct >= 0)
            {
                progressBar_Scan.Value = Math.Min(progressBar_Scan.Maximum, Math.Max(progressBar_Scan.Minimum, pct));
                label_ScanInfo.Text = _totalWork > 0
                    ? $"Progress: {_doneWork}/{_totalWork} ({pct}%)"
                    : "Progress: idle";
                _pendingPct = -1;
            }
        }

        /// <summary>DataGridView.DoubleBuffered is protected; flip it via reflection.
        /// Cuts down on flicker/redraw cost during heavy batch inserts.</summary>
        private static void EnableDoubleBuffering(DataGridView dgv)
        {
            typeof(DataGridView)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(dgv, true, null);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _uiFlushTimer?.Stop();
            _uiFlushTimer?.Dispose();
            base.OnFormClosed(e);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  HELPERS
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>Strips scheme/path/port from user input, returns bare root domain.</summary>
        private string ExtractRootDomain(string raw)
        {
            string s = raw.Trim();
            if (s.IndexOf("://", StringComparison.Ordinal) < 0) s = "http://" + s;

            string host;
            try { host = new Uri(s).Host; }
            catch { host = raw.Trim(); }

            host = host.Trim().TrimEnd('.');
            if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                host = host.Substring(4);

            return host.ToLowerInvariant();
        }

        private void SetProgress(bool running)
        {
            // While scanning: fixed column widths (no per-row layout recalculation)
            // and a real percentage bar instead of an indeterminate marquee.
            var mode = running ? DataGridViewAutoSizeColumnMode.None : DataGridViewAutoSizeColumnMode.Fill;
            colName.AutoSizeMode = mode;
            colStatus.AutoSizeMode = mode;
            colResponse.AutoSizeMode = mode;

            if (running)
            {
                colName.Width = 320;
                colStatus.Width = 140;
                colResponse.Width = Math.Max(300, dataGridView_Output.ClientSize.Width - 320 - 140);
            }

            progressBar_Scan.Style = ProgressBarStyle.Blocks;
            progressBar_Scan.Minimum = 0;
            progressBar_Scan.Maximum = 100;
            progressBar_Scan.Value = 0;

            _doneWork = 0;
            _totalWork = 0;
            _pendingPct = -1;
            label_ScanInfo.Text = "Progress: idle";
        }

        /// <summary>Call once you know how many discrete steps the scan will perform
        /// (wordlist size + number of hosts to probe), before those phases start.</summary>
        private void SetTotalWork(int total)
        {
            _totalWork = Math.Max(1, total);
            Interlocked.Exchange(ref _doneWork, 0);
            _pendingPct = 0; // picked up by the next timer tick
        }

        /// <summary>Increment completed work by one unit. Thread-safe, non-blocking —
        /// just updates a counter and stashes the latest percentage for the UI
        /// timer to pick up on its next tick. No per-call cross-thread marshalling.</summary>
        private void BumpProgress()
        {
            int done = Interlocked.Increment(ref _doneWork);
            int pct = _totalWork > 0 ? Math.Min(100, (int)(done * 100L / _totalWork)) : 0;
            _pendingPct = pct; // last-write-wins; intermediate values are fine to drop
        }

        /// <summary>Queue a row for the next UI flush tick (thread-safe, non-blocking,
        /// never touches the UI thread directly).</summary>
        private void AddRow(string name, string status, string response)
        {
            _pendingRows.Enqueue((name, status, response));
        }

        private void AddSep(string section)
            => AddRow($"── {section} ──", "────", "────────────────────────────────────────");

        // ════════════════════════════════════════════════════════════════════════
        //  TAB: SUBDOMAIN SCAN
        // ════════════════════════════════════════════════════════════════════════

        private async void button_Scan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_Url.Text))
            {
                MessageBox.Show("Please enter a domain (e.g. example.com).", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dataGridView_Output.Rows.Clear();
            while (_pendingRows.TryDequeue(out _)) { /* drop any stale queued rows */ }
            _found.Clear();
            _wildcardIps = null;
            _cts = new CancellationTokenSource();

            string domain = ExtractRootDomain(textBox_Url.Text);
            button_Scan.Enabled = false;
            button_StopScan.Enabled = true;
            SetProgress(true);

            AddRow("TARGET", "🔍 Scanning", domain);

            try
            {
                await Task.Run(async () =>
                {
                    var ct = _cts.Token;

                    // 1. Wildcard-DNS baseline — must run before brute-force so we
                    //    can filter out catch-all false positives.
                    await DetectWildcardDns(domain, ct);

                    // 2. Passive: Certificate Transparency logs — two independent
                    //    sources queried in parallel so one going down (rate-limit,
                    //    outage, or just being slow) doesn't cost us coverage.
                    //    NOTE: QueryCrtSh / QueryCertSpotter now swallow their own
                    //    per-source failures (including HttpClient request
                    //    timeouts) instead of rethrowing, so a slow/unavailable
                    //    source can no longer take the other one down with it via
                    //    Task.WhenAll's fail-fast behaviour.
                    var crtShTask = QueryCrtSh(domain, ct);
                    var certSpotterTask = QueryCertSpotter(domain, ct);
                    await Task.WhenAll(crtShTask, certSpotterTask);
                    var ctNames = crtShTask.Result;
                    var certSpotterNames = certSpotterTask.Result;

                    // 3. Active: DNS wordlist brute-force
                    SetTotalWork(SubdomainWordlist.Length); // brute-force phase counted first
                    var bruteNames = await BruteForceDns(domain, ct);

                    // 4. Merge + dedupe across all sources
                    var allNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var n in ctNames) allNames.Add(n);
                    foreach (var n in certSpotterNames) allNames.Add(n);
                    foreach (var n in bruteNames) allNames.Add(n);

                    if (checkBox_IncludeRoot.Checked) allNames.Add(domain);

                    AddSep($"Resolved Subdomains ({allNames.Count} unique across all sources)");

                    // 5. Live HTTP/HTTPS probing of every resolved subdomain
                    if (checkBox_ProbeHttp.Checked)
                    {
                        var probeList = allNames.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();
                        SetTotalWork(probeList.Count); // probe phase has its own counter
                        await ProbeAllAsync(probeList, ct);
                    }
                    else
                    {
                        foreach (var n in allNames.OrderBy(n => n, StringComparer.OrdinalIgnoreCase))
                            AddRow(n, "ℹ️ Found", "Subdomain discovered (HTTP probe disabled)");
                    }

                }, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                AddRow("SCAN", "⚠️ Cancelled", "Scan was stopped by user.");
            }
            catch (Exception ex)
            {
                AddRow("ERROR", "❌ Exception", ex.Message);
            }
            finally
            {
                AddRow("DONE", "✅ Complete", $"Total subdomains found: {_found.Count}");
                button_Scan.Enabled = true;
                button_StopScan.Enabled = false;
                SetProgress(false);
            }
        }

        private void button_StopScan_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            button_StopScan.Enabled = false;
        }

        // ════════════════════════════════════════════════════════════════════════
        //  WILDCARD DNS DETECTION
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Resolves a few random, near-certainly-nonexistent labels under the
        /// target domain. If they all resolve to the same IP(s), the domain has
        /// wildcard DNS — every brute-forced guess will "resolve" whether or not
        /// it's a real subdomain, so we record the wildcard IP set to filter
        /// those false positives out later.
        /// </summary>
        private async Task DetectWildcardDns(string domain, CancellationToken ct)
        {
            AddSep("Wildcard DNS Check");
            try
            {
                var seenIps = new HashSet<string>();
                bool anyResolved = false;

                for (int i = 0; i < 2; i++)
                {
                    string canary = "ts-canary-" + Guid.NewGuid().ToString("N").Substring(0, 12) + "." + domain;
                    try
                    {
                        var addrs = await Dns.GetHostAddressesAsync(canary);
                        if (addrs.Length > 0)
                        {
                            anyResolved = true;
                            foreach (var a in addrs) seenIps.Add(a.ToString());
                        }
                    }
                    catch { /* NXDOMAIN expected — good, no wildcard */ }
                }

                if (anyResolved && seenIps.Count > 0)
                {
                    _wildcardIps = seenIps;
                    AddRow("Wildcard DNS", "⚠️ Detected", "Domain catch-all resolves all subdomains to: "
                        + string.Join(", ", seenIps) + " — brute-force results filtered against this");
                }
                else
                {
                    AddRow("Wildcard DNS", "✅ Not detected", "Random labels did not resolve — brute-force results are reliable");
                }
            }
            catch (Exception ex) { AddRow("Wildcard DNS", "❌ Error", ex.Message); }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  PASSIVE: CERTIFICATE TRANSPARENCY — TWO INDEPENDENT SOURCES
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// crt.sh exposes a public JSON search over Certificate Transparency logs.
        /// Any subdomain that ever had a TLS cert issued for it (Let's Encrypt,
        /// etc.) shows up here — including ones DNS brute-force would never guess.
        ///
        /// Only a *genuine* cancellation (ct.IsCancellationRequested == true)
        /// propagates upward. A timeout or any other failure on this source alone
        /// is logged and returns an empty list, letting the other source's
        /// results survive.
        /// </summary>
        private async Task<List<string>> QueryCrtSh(string domain, CancellationToken ct)
        {
            AddSep("Certificate Transparency — crt.sh");
            var names = new List<string>();
            try
            {
                string url = $"https://crt.sh/?q=%25.{Uri.EscapeDataString(domain)}&output=json";
                var req = new HttpRequestMessage(HttpMethod.Get, url);

                var cts2 = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts2.CancelAfter(TimeSpan.FromSeconds(30)); // crt.sh is slow — give it real room

                var resp = await _http.SendAsync(req, cts2.Token);

                if (!resp.IsSuccessStatusCode)
                {
                    AddRow("crt.sh", "⚠️ Unavailable", $"HTTP {(int)resp.StatusCode} — falling back to CertSpotter + brute-force");
                    return names;
                }

                string body = await resp.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(body))
                {
                    AddRow("crt.sh", "ℹ️ No data", "No certificates found for this domain");
                    return names;
                }

                var doc = JsonDocument.Parse(body);
                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var entry in doc.RootElement.EnumerateArray())
                {
                    if (!entry.TryGetProperty("name_value", out var nameValEl)) continue;
                    string nameVal = nameValEl.GetString() ?? "";

                    // name_value can contain multiple newline-separated SANs
                    foreach (var raw in nameVal.Split('\n'))
                    {
                        string clean = raw.Trim().ToLowerInvariant().TrimEnd('.');
                        if (string.IsNullOrWhiteSpace(clean)) continue;
                        if (clean.StartsWith("*.")) clean = clean.Substring(2);

                        if (!clean.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase)
                            && !clean.Equals(domain, StringComparison.OrdinalIgnoreCase))
                            continue; // not actually under our target domain

                        if (seen.Add(clean)) names.Add(clean);
                    }
                }

                AddRow("crt.sh", "✅ Complete", $"{names.Count} unique name(s) found in CT logs");
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                // Genuine user-initiated cancel (Stop button) — propagate so the
                // outer scan loop unwinds cleanly instead of limping along.
                throw;
            }
            catch (OperationCanceledException)
            {
                // cts2 fired its own 30s CancelAfter — this is a per-source timeout,
                // NOT a real cancel (ct itself was never triggered). Log and move on.
                AddRow("crt.sh", "⚠️ Timeout", "No response within 30s — falling back to CertSpotter + brute-force");
            }
            catch (Exception ex)
            {
                // JSON parse failures, DNS errors, etc.
                AddRow("crt.sh", "❌ Error", ex.Message + " — falling back to CertSpotter + brute-force");
            }
            return names;
        }

        /// <summary>
        /// CertSpotter (SSLMate) is a second, independently-operated CT-log search
        /// index. It doesn't always see exactly the same certificates as crt.sh —
        /// different ingestion timing/log coverage — so querying both catches more
        /// names than either alone, and keeps results flowing if one source is
        /// down or rate-limited. Unauthenticated requests are allowed at a modest
        /// rate for personal/evaluation use; no API key needed.
        ///
        /// Same cancellation handling as QueryCrtSh — only rethrow on a genuine
        /// cancel (ct.IsCancellationRequested), not on a plain request timeout or
        /// other per-source failure, so this source can't take the other one
        /// down via Task.WhenAll.
        /// </summary>
        private async Task<List<string>> QueryCertSpotter(string domain, CancellationToken ct)
        {
            AddSep("Certificate Transparency — CertSpotter");
            var names = new List<string>();
            try
            {
                string url = "https://api.certspotter.com/v1/issuances"
                    + $"?domain={Uri.EscapeDataString(domain)}"
                    + "&include_subdomains=true"
                    + "&expand=dns_names";

                var req = new HttpRequestMessage(HttpMethod.Get, url);
                var resp = await _http.SendAsync(req, ct);

                if (resp.StatusCode == System.Net.HttpStatusCode.RequestEntityTooLarge)
                {
                    AddRow("CertSpotter", "⚠️ Rate-limited", "Hourly unauthenticated quota reached — skipping this source for now");
                    return names;
                }
                if (!resp.IsSuccessStatusCode)
                {
                    AddRow("CertSpotter", "⚠️ Unavailable", $"HTTP {(int)resp.StatusCode} — continuing with other sources");
                    return names;
                }

                string body = await resp.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(body) || body.Trim() == "[]")
                {
                    AddRow("CertSpotter", "ℹ️ No data", "No certificates found for this domain");
                    return names;
                }

                var doc = JsonDocument.Parse(body);
                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var issuance in doc.RootElement.EnumerateArray())
                {
                    if (!issuance.TryGetProperty("dns_names", out var dnsNamesEl)) continue;

                    foreach (var nameEl in dnsNamesEl.EnumerateArray())
                    {
                        string raw = nameEl.GetString() ?? "";
                        string clean = raw.Trim().ToLowerInvariant().TrimEnd('.');
                        if (string.IsNullOrWhiteSpace(clean)) continue;
                        if (clean.StartsWith("*.")) clean = clean.Substring(2);

                        if (!clean.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase)
                            && !clean.Equals(domain, StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (seen.Add(clean)) names.Add(clean);
                    }
                }

                AddRow("CertSpotter", "✅ Complete", $"{names.Count} unique name(s) found in CT logs");
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                AddRow("CertSpotter", "❌ Error", ex.Message + " — continuing with other sources");
            }
            return names;
        }

        // ════════════════════════════════════════════════════════════════════════
        //  ACTIVE: DNS WORDLIST BRUTE-FORCE
        // ════════════════════════════════════════════════════════════════════════

        private async Task<List<string>> BruteForceDns(string domain, CancellationToken ct)
        {
            AddSep($"DNS Brute-Force ({SubdomainWordlist.Length} labels)");
            var hits = new List<string>();
            int checkedCount = 0;
            int totalCount = SubdomainWordlist.Length;

            var throttle = new SemaphoreSlim(40);
            var tasks = new List<Task>();
            var hitsLock = new object();

            foreach (string label in SubdomainWordlist)
            {
                ct.ThrowIfCancellationRequested();
                await throttle.WaitAsync(ct);

                string candidate = label + "." + domain;
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var addrs = await Dns.GetHostAddressesAsync(candidate);
                        bool isWildcardHit = _wildcardIps != null
                            && addrs.Length > 0
                            && addrs.All(a => _wildcardIps.Contains(a.ToString()));

                        if (addrs.Length > 0 && !isWildcardHit)
                        {
                            lock (hitsLock) hits.Add(candidate);
                        }
                    }
                    catch { /* NXDOMAIN — not a subdomain, expected for most guesses */ }
                    finally
                    {
                        Interlocked.Increment(ref checkedCount);
                        BumpProgress();
                        throttle.Release();
                    }
                }, ct));
            }

            await Task.WhenAll(tasks);

            AddRow("DNS Brute-Force", "✅ Complete",
                $"{checkedCount}/{totalCount} labels tried — {hits.Count} resolved");

            return hits;
        }

        // ════════════════════════════════════════════════════════════════════════
        //  LIVE HTTP/HTTPS PROBING
        // ════════════════════════════════════════════════════════════════════════

        private async Task ProbeAllAsync(List<string> names, CancellationToken ct)
        {
            AddSep($"Live HTTP/HTTPS Probe ({names.Count} hosts)");

            var throttle = new SemaphoreSlim(ProbeConcurrency);
            var tasks = names.Select(async name =>
            {
                await throttle.WaitAsync(ct);
                try { await ProbeOneAsync(name, ct); }
                finally
                {
                    BumpProgress();
                    throttle.Release();
                }
            });

            await Task.WhenAll(tasks);
        }

        private async Task ProbeOneAsync(string name, CancellationToken ct)
        {
            // Try HTTPS first, fall back to HTTP.
            foreach (var scheme in new[] { "https://", "http://" })
            {
                try
                {
                    string url = scheme + name;
                    var req = new HttpRequestMessage(HttpMethod.Get, url);
                    var cts2 = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    cts2.CancelAfter(TimeSpan.FromSeconds(8));

                    var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts2.Token);
                    int code = (int)resp.StatusCode;

                    string title = "";
                    try
                    {
                        string body = await resp.Content.ReadAsStringAsync();
                        var m = Regex.Match(body, @"<title[^>]*>(.*?)</title>",
                            RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        if (m.Success)
                            title = " — \"" + Regex.Replace(m.Groups[1].Value.Trim(), @"\s+", " ") + "\"";
                        if (title.Length > 80) title = title.Substring(0, 80) + "…\"";
                    }
                    catch { /* body read failed — status code alone is still useful */ }

                    lock (_found) _found.Add(name);

                    string icon = code >= 200 && code < 300 ? "✅"
                                : code >= 300 && code < 400 ? "↪️"
                                : code >= 400 && code < 500 ? "⚠️"
                                : code >= 500 ? "🚨"
                                : "ℹ️";

                    AddRow(name, $"{icon} HTTP {code}", $"{url}{title}");
                    return; // got a response on this scheme — don't also try the other
                }
                catch (TaskCanceledException) when (!ct.IsCancellationRequested)
                {
                    // this scheme timed out — fall through and try the next scheme
                }
                catch (OperationCanceledException) { throw; }
                catch { /* connection refused / TLS failure / DNS fail on this scheme — try next */ }
            }

            // Neither HTTPS nor HTTP responded, but it DID resolve in DNS —
            // still worth reporting as a discovered (but dead web) subdomain.
            lock (_found) _found.Add(name);
            AddRow(name, "🔒 No HTTP", "Resolved in DNS but no web server responded on 80/443");
        }

        // ════════════════════════════════════════════════════════════════════════
        //  SAVE / CLEAR / COPY
        // ════════════════════════════════════════════════════════════════════════

        private void button_SaveReport_Click(object sender, EventArgs e)
        {
            if (dataGridView_Output.Rows.Count == 0)
            {
                MessageBox.Show("No scan results to save.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var dlg = new SaveFileDialog
            {
                Filter = "CSV File|*.csv|Text File|*.txt",
                FileName = $"ThreatScanner_Subdomains_{DateTime.Now:yyyyMMdd_HHmmss}"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Name,Status,Response");
                foreach (DataGridViewRow row in dataGridView_Output.Rows)
                {
                    if (row.IsNewRow) continue;
                    string name = row.Cells["colName"].Value?.ToString()?.Replace(",", ";") ?? "";
                    string status = row.Cells["colStatus"].Value?.ToString()?.Replace(",", ";") ?? "";
                    string response = row.Cells["colResponse"].Value?.ToString()?.Replace(",", ";") ?? "";
                    sb.AppendLine($"{name},{status},{response}");
                }
                File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"Report saved: {dlg.FileName}", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button_ClearOutput_Click(object sender, EventArgs e)
        {
            dataGridView_Output.Rows.Clear();
            while (_pendingRows.TryDequeue(out _)) { /* drop any stale queued rows */ }
            _found.Clear();
        }

        /// <summary>Copy selected rows (or all rows) to clipboard as TSV.</summary>
        private void button_CopyOutput_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Name\tStatus\tResponse");

            var rows = dataGridView_Output.SelectedRows.Count > 0
                ? dataGridView_Output.SelectedRows.Cast<DataGridViewRow>().ToList()
                : dataGridView_Output.Rows.Cast<DataGridViewRow>().ToList();

            foreach (var row in rows.Where(r => !r.IsNewRow))
            {
                string name = row.Cells["colName"].Value?.ToString() ?? "";
                string status = row.Cells["colStatus"].Value?.ToString() ?? "";
                string response = row.Cells["colResponse"].Value?.ToString() ?? "";
                sb.AppendLine($"{name}\t{status}\t{response}");
            }
            Clipboard.SetText(sb.ToString());
            MessageBox.Show("Copied to clipboard!", "ThreatScanner",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ── Right-click context menu on the grid ─────────────────────────────────
        private void dataGridView_Output_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            var hit = dataGridView_Output.HitTest(e.X, e.Y);
            if (hit.RowIndex >= 0)
            {
                dataGridView_Output.ClearSelection();
                dataGridView_Output.Rows[hit.RowIndex].Selected = true;
                dataGridView_Output.CurrentCell = dataGridView_Output.Rows[hit.RowIndex].Cells[0];
            }

            // Build the URL for "Open in Browser" from the clicked row's subdomain name
            string rowUrl = null;
            if (hit.RowIndex >= 0)
            {
                string nameCell = dataGridView_Output.Rows[hit.RowIndex]
                                      .Cells["colName"].Value?.ToString() ?? "";
                if (nameCell.StartsWith("http://") || nameCell.StartsWith("https://"))
                    rowUrl = nameCell;
                else if (!nameCell.StartsWith("─") && nameCell.Contains("."))
                    rowUrl = "https://" + nameCell;
            }

            var menu = new ContextMenuStrip();

            var openItem = new System.Windows.Forms.ToolStripMenuItem(
                "🌐  Open in Browser", null,
                (s, ev) =>
                {
                    if (!string.IsNullOrEmpty(rowUrl))
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = rowUrl,
                            UseShellExecute = true
                        });
                    else
                        MessageBox.Show("No valid URL found for this row.", "ThreatScanner",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
            openItem.Enabled = !string.IsNullOrEmpty(rowUrl);
            openItem.Font = new System.Drawing.Font(menu.Font, System.Drawing.FontStyle.Bold);
            menu.Items.Add(openItem);

            var copyUrlItem = new System.Windows.Forms.ToolStripMenuItem(
                "📋  Copy URL", null,
                (s, ev) =>
                {
                    if (!string.IsNullOrEmpty(rowUrl))
                        Clipboard.SetText(rowUrl);
                });
            copyUrlItem.Enabled = !string.IsNullOrEmpty(rowUrl);
            menu.Items.Add(copyUrlItem);

            menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

            menu.Items.Add("📄  Copy Selected Rows", null,
                (s, ev) => button_CopyOutput_Click(s, ev));
            menu.Items.Add("📄  Copy All Rows", null,
                (s, ev) =>
                {
                    dataGridView_Output.ClearSelection();
                    button_CopyOutput_Click(s, ev);
                });

            menu.Show(dataGridView_Output, e.Location);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  SUBDOMAIN WORDLIST (common labels)
        // ════════════════════════════════════════════════════════════════════════

        private static readonly string[] SubdomainWordlist =
      {
            // Core / common
            "www", "mail", "webmail", "email", "ftp", "sftp", "ssh", "vpn", "remote",
            "ns1", "ns2", "ns3", "ns4", "dns", "mx", "owa", "autodiscover", "smtp",

            // Environments
            "dev", "develop", "development", "staging", "stage", "stg", "test", "testing", "qa",
            "uat", "preprod", "pre-prod", "prod", "production", "sandbox", "demo", "beta", "alpha",
            "canary", "preview", "local", "integration", "sit", "perf", "performance", "loadtest", "dr",
            "failover",

            // Admin / internal
            "admin", "administrator", "panel", "cpanel", "whm", "portal", "console", "dashboard", "manage",
            "management", "control", "backend", "internal", "intranet", "private", "secure", "system", "sys",
            "root", "master", "ops", "operations", "noc", "soc", "helpdesk-admin",

            // Web tiers
            "app", "apps", "web", "site", "static", "assets", "cdn", "media", "img",
            "images", "pic", "pics", "video", "videos", "files", "file", "download", "downloads",
            "upload", "uploads", "content", "data", "front", "frontend", "landing", "home", "go",

            // API / services
            "api", "apidocs", "rest", "graphql", "ws", "websocket", "socket", "service", "services",
            "gateway", "microservice", "rpc", "grpc", "soap", "webhook", "webhooks", "sdk", "developer",
            "developers",

            // Auth / identity
            "auth", "sso", "login", "signin", "signup", "register", "oauth", "identity", "idp",
            "accounts", "account", "id", "iam", "passport", "session", "myaccount",

            // Databases / infra
            "db", "database", "sql", "mysql", "postgres", "postgresql", "redis", "mongo", "mongodb",
            "elastic", "elasticsearch", "kibana", "grafana", "prometheus", "metrics", "logs", "logging", "log",
            "monitor", "monitoring", "status", "health", "stats", "uptime", "telemetry",

            // DevOps / CI
            "git", "gitlab", "github", "bitbucket", "svn", "ci", "cd", "cicd", "jenkins",
            "build", "builds", "deploy", "deployment", "registry", "docker", "k8s", "kubernetes", "helm",
            "artifactory", "nexus", "pipeline", "runner", "sonar", "sonarqube",

            // Cloud / networking
            "cloud", "aws", "azure", "gcp", "s3", "storage", "blob", "lb", "loadbalancer",
            "proxy", "reverse-proxy", "gw", "edge", "origin", "cache", "vip", "nat", "firewall",
            "router", "switch", "subnet", "network",

            // Communication
            "chat", "support", "help", "helpdesk", "ticket", "tickets", "forum", "forums", "community",
            "blog", "news", "newsletter", "social", "feedback", "survey", "contact", "contactus", "live",
            "livechat",

            // Business / commerce
            "shop", "store", "checkout", "payment", "payments", "billing", "pay", "cart", "orders",
            "order", "invoice", "invoices", "crm", "erp", "hr", "hris", "payroll", "partner",
            "partners", "vendor", "vendors", "client", "clients", "customer", "customers", "client-portal", "extranet",
            "b2b", "b2c", "pos",

            // Finance / banking
            "bank", "banking", "finance", "financial", "treasury", "ledger", "accounting", "tax", "loans",
            "loan", "credit", "card", "cards", "wallet", "wire", "transfer", "statements", "reports",
            "audit",

            // Regions / geography
            "us", "eu", "uk", "asia", "apac", "emea", "latam", "east", "west",
            "north", "south", "us-east", "us-west", "eu-west", "eu-central", "ap-south", "global", "regional",

            // Mobile / client apps
            "m", "mobile", "wap", "ios", "android", "tv", "watch", "tablet",

            // Misc tech / protocols
            "ns", "pop", "pop3", "imap", "relay", "ntp", "time", "ldap", "radius",
            "ad", "dc", "vault", "secrets", "kms", "pki", "ca", "ocsp", "crl",
            "dhcp", "syslog", "ntp1", "ntp2",

            // Old / legacy / versioned
            "old", "legacy", "archive", "backup", "backups", "bak", "tmp", "temp", "new",
            "v1", "v2", "v3", "next", "classic", "retired",

            // Docs / knowledge
            "wiki", "docs", "documentation", "kb", "knowledgebase", "training", "lms", "elearning", "jobs",
            "careers", "recruiting", "faq", "manual", "guide", "handbook",

            // Security / compliance
            "security", "infosec", "compliance", "privacy", "gdpr", "trust", "vault-secure", "pentest", "scanner",
            "waf", "ids", "ips", "siem", "phish", "abuse",

            // Marketing / growth
            "marketing", "promo", "promotions", "campaign", "campaigns", "ads", "advertising", "affiliate", "affiliates",
            "referral", "tracking", "analytics", "pixel", "events",

            // Education / public sector
            "school", "university", "college", "edu", "student", "students", "alumni", "library", "registrar",
            "admissions", "gov", "government", "city", "county",

            // IoT / hardware
            "iot", "device", "devices", "sensor", "sensors", "firmware", "gateway-iot", "camera", "cameras",
            "printer", "scanner-device", "kiosk", "terminal",

            // Gaming / media streaming
            "game", "games", "play", "gaming", "stream", "streaming", "live-stream", "video-stream", "broadcast",
            "podcast", "music", "radio",

            // Misc business functions
            "legal", "procurement", "logistics", "warehouse", "inventory", "shipping", "fulfillment", "scheduling", "calendar",
            "booking", "reservations", "events-booking",

            // Healthcare
            "healthcare", "patient", "patients", "clinic", "clinical", "hospital", "ehr", "emr", "pharmacy",
            "lab", "labs", "diagnostics", "telehealth", "appointments", "insurance", "claims", "provider", "providers",

            // Real estate / facilities
            "realestate", "property", "properties", "listings", "leasing", "rentals", "facilities", "maintenance", "tenant",
            "tenants", "building", "buildings", "campus",

            // Telecom
            "telecom", "voip", "sip", "pbx", "phone", "fax", "sms", "messaging", "carrier",
            "roaming", "billing-telecom", "network-ops",

            // Automotive / fleet
            "fleet", "vehicle", "vehicles", "dealer", "dealers", "service-center", "parts", "garage", "tracking-fleet",
            "telematics",

            // Travel / hospitality
            "travel", "booking-travel", "hotel", "hotels", "flights", "airline", "tickets-travel", "tours", "rewards",
            "loyalty", "checkin", "concierge",

            // Manufacturing / supply chain
            "manufacturing", "factory", "plant", "production-line", "supply", "supplychain", "procurement-mfg", "quality", "qc",
            "scada", "plc",

            // Nonprofit / community
            "donate", "donations", "fundraising", "volunteer", "volunteers", "outreach", "members", "membership", "foundation",
            "grants",

            // Sports / fitness
            "sports", "fitness", "gym", "league", "scores", "stats-sports", "team", "teams", "tickets-sports",
            "schedule",

            // Agriculture
            "farm", "farming", "agriculture", "crop", "crops", "livestock", "harvest", "irrigation",

            // Energy / utilities
            "energy", "utilities", "power", "grid", "solar", "meter", "metering", "outage", "billing-utility",
            "consumption",

            // Legal / extra
            "legalteam", "contracts", "compliance-legal", "litigation", "patents", "trademarks",

            // HR / extra
            "benefits", "onboarding", "offboarding", "timesheet", "timeoff", "performance-review",

            // Extra misc
            "search", "directory", "sitemap", "rss", "feed", "export", "import", "sync-service", "notify",
            "notifications", "alerts", "reminder", "scheduler", "queue-worker",

        };
    }
}
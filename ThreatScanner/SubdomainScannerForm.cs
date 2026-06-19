using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        public SubdomainScannerForm() => InitializeComponent();

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
            progressBar_Scan.Style = running ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            if (!running) progressBar_Scan.Value = 0;
        }

        /// <summary>Add a row to the DataGridView (thread-safe).</summary>
        private void AddRow(string name, string status, string response)
        {
            void Do()
            {
                int idx = dataGridView_Output.Rows.Add(name, status, response);
                var row = dataGridView_Output.Rows[idx];

                Color fore = Color.FromArgb(226, 232, 240);
                Color back = Color.FromArgb(15, 23, 42);

                if (status.StartsWith("🚨") || status.StartsWith("❌"))
                { fore = Color.FromArgb(248, 113, 113); }
                else if (status.StartsWith("⚠️"))
                { fore = Color.FromArgb(251, 191, 36); }
                else if (status.StartsWith("✅"))
                { fore = Color.FromArgb(52, 211, 153); }
                else if (status.StartsWith("ℹ️"))
                { fore = Color.FromArgb(96, 165, 250); }

                row.Cells["colStatus"].Style.ForeColor = fore;
                row.Cells["colStatus"].Style.BackColor = back;

                dataGridView_Output.FirstDisplayedScrollingRowIndex = idx;
            }

            if (dataGridView_Output.InvokeRequired)
                dataGridView_Output.Invoke((Action)Do);
            else
                Do();
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
                    //    outage) doesn't cost us coverage.
                    var crtShTask = QueryCrtSh(domain, ct);
                    var certSpotterTask = QueryCertSpotter(domain, ct);
                    await Task.WhenAll(crtShTask, certSpotterTask);
                    var ctNames = crtShTask.Result;
                    var certSpotterNames = certSpotterTask.Result;

                    // 3. Active: DNS wordlist brute-force
                    var bruteNames = await BruteForceDns(domain, ct);

                    // 4. Merge + dedupe across all sources
                    var allNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var n in ctNames) allNames.Add(n);
                    foreach (var n in certSpotterNames) allNames.Add(n);
                    foreach (var n in bruteNames) allNames.Add(n);

                    if (checkBox_IncludeRoot.Checked) allNames.Add(domain);

                    Invoke((Action)(() =>
                        AddSep($"Resolved Subdomains ({allNames.Count} unique across all sources)")));

                    // 5. Live HTTP/HTTPS probing of every resolved subdomain
                    if (checkBox_ProbeHttp.Checked)
                        await ProbeAllAsync(allNames.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList(), ct);
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
                Invoke((Action)(() =>
                {
                    AddRow("DONE", "✅ Complete", $"Total subdomains found: {_found.Count}");
                    button_Scan.Enabled = true;
                    button_StopScan.Enabled = false;
                    SetProgress(false);
                }));
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
        /// </summary>
        private async Task<List<string>> QueryCrtSh(string domain, CancellationToken ct)
        {
            AddSep("Certificate Transparency — crt.sh");
            var names = new List<string>();
            try
            {
                string url = $"https://crt.sh/?q=%25.{Uri.EscapeDataString(domain)}&output=json";
                var req = new HttpRequestMessage(HttpMethod.Get, url);
                var resp = await _http.SendAsync(req, ct);

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
            catch (TaskCanceledException) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
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
            catch (TaskCanceledException) { throw; }
            catch (OperationCanceledException) { throw; }
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
                finally { throttle.Release(); }
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
            "ns1", "ns2", "ns3", "ns4", "dns", "dns1", "dns2", "mx", "mx1", "mx2",

            // Environments
            "dev", "develop", "development", "staging", "stage", "test", "testing",
            "qa", "uat", "preprod", "pre-prod", "prod", "production", "sandbox",
            "demo", "beta", "alpha", "canary", "preview", "local",

            // Admin / internal
            "admin", "administrator", "panel", "cpanel", "whm", "portal", "console",
            "dashboard", "manage", "management", "control", "backend", "internal",
            "intranet", "private", "secure", "system", "sys", "root", "master",

            // Web tiers
            "app", "apps", "web", "web1", "web2", "site", "static", "assets",
            "cdn", "media", "img", "images", "video", "videos", "files", "file",
            "download", "downloads", "upload", "uploads", "content", "data",

            // API / services
            "api", "api1", "api2", "apiv1", "apiv2", "rest", "graphql", "ws",
            "websocket", "socket", "service", "services", "gateway", "microservice",
            "rpc", "grpc", "soap",

            // Auth / identity
            "auth", "sso", "login", "signin", "oauth", "identity", "idp", "accounts",
            "account", "id", "iam",

            // Databases / infra
            "db", "database", "sql", "mysql", "postgres", "redis", "mongo", "elastic",
            "elasticsearch", "kibana", "grafana", "prometheus", "metrics", "logs",
            "logging", "log", "monitor", "monitoring", "status", "health",

            // DevOps / CI
            "git", "gitlab", "github", "bitbucket", "svn", "ci", "cd", "cicd",
            "jenkins", "build", "builds", "deploy", "deployment", "registry",
            "docker", "k8s", "kubernetes", "helm", "artifactory", "nexus",

            // Cloud / networking
            "cloud", "aws", "azure", "gcp", "s3", "storage", "blob", "lb",
            "loadbalancer", "proxy", "reverse-proxy", "gw", "edge", "origin",
            "cache", "cdn1", "cdn2",

            // Communication
            "chat", "support", "help", "helpdesk", "ticket", "tickets", "forum",
            "forums", "community", "blog", "news", "newsletter", "social",

            // Business / misc
            "shop", "store", "checkout", "payment", "payments", "billing", "pay",
            "cart", "orders", "order", "invoice", "crm", "erp", "hr", "hris",
            "partner", "partners", "vendor", "vendors", "client", "clients",
            "customer", "customers", "client-portal", "extranet",

            // Regions / numbered
            "us", "eu", "uk", "asia", "apac", "east", "west", "north", "south",
            "us-east", "us-west", "eu-west", "eu-central",
            "server1", "server2", "node1", "node2", "host1", "host2",
            "vm1", "vm2", "app1", "app2", "web3", "api3",

            // Mobile / misc apps
            "m", "mobile", "wap", "ios", "android", "app-api",

            // Misc tech
            "ns", "smtp", "smtp1", "smtp2", "pop", "pop3", "imap", "relay",
            "ntp", "time", "ldap", "radius", "ad", "dc", "vault", "secrets",
            "kms", "pki", "ca", "ocsp", "crl",

            // Old / legacy
            "old", "legacy", "archive", "backup", "backups", "bak", "tmp", "temp",
            "new", "new-site", "v1", "v2", "v3",

            // Wildcard-y generic
            "git2", "wiki", "docs", "documentation", "kb", "knowledgebase",
            "training", "lms", "elearning", "jobs", "careers", "recruiting"
        };
    }
}
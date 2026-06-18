using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreatScanner.Helpers;

namespace ThreatScanner
{
    /// <summary>
    /// Full-scanner tool: HTTPS, security headers, cookies, SQLi hints,
    /// XSS hints, open ports, sensitive files, header dump, and DNS lookup.
    /// </summary>
    public partial class FullScannerForm : Form
    {
        private readonly HttpClient _http = ScanHelpers.BuildDefaultClient();

        public FullScannerForm() => InitializeComponent();

        // ─── HELPERS ─────────────────────────────────────────────────────────────

        private void Log(string icon, string msg) => ScanHelpers.Log(listBox_Output, icon, msg);
        private void LogSep() => ScanHelpers.LogSeparator(listBox_Output);
        private void ClearOut() => ScanHelpers.ClearOutput(listBox_Output);
        private string NormalizeUrl(string raw) => ScanHelpers.NormalizeUrl(raw);

        private void SetProgress(bool running)
        {
            progressBar_Scan.Style = running
                ? ProgressBarStyle.Marquee
                : ProgressBarStyle.Blocks;
            if (!running) progressBar_Scan.Value = 0;
        }

        // ─── TAB: FULL SCAN ──────────────────────────────────────────────────────

        private async void button_Scan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_Url.Text))
            {
                MessageBox.Show("Please enter a URL.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ClearOut();
            string url = NormalizeUrl(textBox_Url.Text);
            button_Scan.Enabled = false;
            SetProgress(true);

            Log("🔍", $"Scanning: {url}");
            LogSep();

            try
            {
                await Task.Run(async () =>
                {
                    await CheckHttps(url);
                    await CheckSecurityHeaders(url);
                    await CheckCookieFlags(url);
                    await CheckRedirects(url);
                    CheckLocalStorage(url);
                    await CheckSqlInjectionHints(url);
                    await CheckXssHints(url);
                    await CheckOpenPorts(url);
                    await CheckSensitiveFiles(url);
                });
            }
            finally
            {
                Invoke((Action)(() =>
                {
                    LogSep();
                    Log("✅", "Scan complete.");
                    button_Scan.Enabled = true;
                    SetProgress(false);
                }));
            }
        }

        private Task CheckHttps(string url)
        {
            Invoke((Action)(() =>
                Log(url.StartsWith("https://") ? "✅" : "⚠️",
                    url.StartsWith("https://")
                        ? "HTTPS: Secure connection detected"
                        : "HTTPS: Site is using plain HTTP — data is NOT encrypted")));
            return Task.CompletedTask;
        }

        private async Task CheckSecurityHeaders(string url)
        {
            try
            {
                var resp = await _http.GetAsync(url);
                var headers = resp.Headers;
                string[] important = {
                    "Content-Security-Policy", "X-Frame-Options",
                    "X-Content-Type-Options",  "Strict-Transport-Security",
                    "Referrer-Policy",         "Permissions-Policy"
                };
                Invoke((Action)(() => Log("🛡", "Security Headers:")));
                foreach (var h in important)
                {
                    bool found = headers.Contains(h);
                    string note = found ? string.Join(", ", headers.GetValues(h)) : "MISSING";
                    Invoke((Action)(() => Log(found ? "✅" : "⚠️", $"  {h}: {note}")));
                }
                if (headers.Contains("Server"))
                    Invoke((Action)(() => Log("⚠️",
                        $"  Server header exposed: {string.Join(", ", headers.GetValues("Server"))}")));
                else
                    Invoke((Action)(() => Log("✅", "  Server header: Not exposed")));
            }
            catch (Exception ex) { Invoke((Action)(() => Log("❌", "Security headers check failed: " + ex.Message))); }
        }

        private async Task CheckCookieFlags(string url)
        {
            try
            {
                var handler = new HttpClientHandler { UseCookies = true, CookieContainer = new CookieContainer() };
                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("User-Agent", "ThreatScanner/1.0");
                var resp = await client.GetAsync(url);
                var cookies = resp.Headers.Contains("Set-Cookie")
                    ? resp.Headers.GetValues("Set-Cookie").ToList()
                    : new System.Collections.Generic.List<string>();

                if (cookies.Count == 0) { Invoke((Action)(() => Log("ℹ️", "Cookies: None set by server"))); return; }

                Invoke((Action)(() => Log("🍪", $"Cookies: {cookies.Count} cookie(s) found")));
                foreach (var cookie in cookies)
                {
                    bool httpOnly = cookie.IndexOf("HttpOnly", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool secure = cookie.IndexOf("Secure", StringComparison.OrdinalIgnoreCase) >= 0;
                    string flags = (!httpOnly ? "⚠️ Missing HttpOnly " : "") + (!secure ? "⚠️ Missing Secure" : "");
                    Invoke((Action)(() =>
                        Log(httpOnly && secure ? "✅" : "⚠️",
                            $"  {cookie.Split(';')[0]} [{(httpOnly ? "HttpOnly" : "NO-HttpOnly")} | {(secure ? "Secure" : "NO-Secure")}]")));
                }
            }
            catch (Exception ex) { Invoke((Action)(() => Log("❌", "Cookie check failed: " + ex.Message))); }
        }

        private async Task CheckRedirects(string url)
        {
            try
            {
                var handler = new HttpClientHandler { AllowAutoRedirect = false };
                var client = new HttpClient(handler);
                var resp = await client.GetAsync(url);
                int code = (int)resp.StatusCode;
                if (code >= 300 && code < 400)
                {
                    string location = resp.Headers.Location?.ToString() ?? "Unknown";
                    Invoke((Action)(() => Log("↪️", $"Redirect: {code} → {location}")));
                }
                else
                    Invoke((Action)(() => Log("✅", $"No redirect — Status: {code}")));
            }
            catch (Exception ex) { Invoke((Action)(() => Log("❌", "Redirect check failed: " + ex.Message))); }
        }

        private void CheckLocalStorage(string url)
        {
            Invoke((Action)(() =>
            {
                webBrowser_Hidden.DocumentCompleted -= WebBrowser_DocumentCompleted;
                webBrowser_Hidden.DocumentCompleted += WebBrowser_DocumentCompleted;
                webBrowser_Hidden.Navigate(url);
                Log("🔄", "localStorage: Loading page for JS inspection...");
            }));
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != webBrowser_Hidden.Url) return;
            try
            {
                object result = webBrowser_Hidden.Document?.InvokeScript("eval", new object[] { "localStorage.length" });
                if (result == null) { Log("❌", "localStorage: Document was null"); return; }
                int count = Convert.ToInt32(result);
                Log(count > 0 ? "⚠️" : "✅",
                    count > 0
                        ? $"localStorage: {count} item(s) found — review for sensitive data"
                        : "localStorage: Empty — no stored items");
            }
            catch (Exception ex) { Log("❌", "localStorage check error: " + ex.Message); }
            finally { webBrowser_Hidden.DocumentCompleted -= WebBrowser_DocumentCompleted; }
        }

        private async Task CheckSqlInjectionHints(string url)
        {
            try
            {
                string testUrl = url.Contains("?") ? url + "&id=1'" : url + "?id=1'";
                string body = await (await _http.GetAsync(testUrl)).Content.ReadAsStringAsync();
                string[] sqlErrors = { "syntax error", "sql", "mysql", "ORA-", "Microsoft OLE DB", "SQLite", "pg_query", "Warning: pg_" };
                bool found = sqlErrors.Any(err => body.IndexOf(err, StringComparison.OrdinalIgnoreCase) >= 0);
                Invoke((Action)(() =>
                    Log(found ? "🚨" : "✅",
                        found ? "SQL Injection: Possible SQL error disclosure detected!"
                              : "SQL Injection: No obvious error disclosure from basic probe")));
            }
            catch (Exception ex) { Invoke((Action)(() => Log("❌", "SQLi check failed: " + ex.Message))); }
        }

        private async Task CheckXssHints(string url)
        {
            try
            {
                const string xssPayload = "<script>xss</script>";
                const string htmlEncoded = "&lt;script&gt;xss&lt;/script&gt;";
                string testUrl = url.Contains("?")
                    ? url + "&q=" + Uri.EscapeDataString(xssPayload)
                    : url + "?q=" + Uri.EscapeDataString(xssPayload);

                string body = await (await _http.GetAsync(testUrl)).Content.ReadAsStringAsync();
                Invoke((Action)(() =>
                {
                    if (body.Contains(xssPayload))
                        Log("🚨", "XSS: Payload reflected UNENCODED — possible reflected XSS!");
                    else if (body.Contains(htmlEncoded))
                        Log("⚠️", "XSS: Payload reflected HTML-encoded — likely safe but review");
                    else
                        Log("✅", "XSS: Payload not reflected (basic probe)");
                }));
            }
            catch (Exception ex) { Invoke((Action)(() => Log("❌", "XSS check failed: " + ex.Message))); }
        }

        private async Task CheckOpenPorts(string url)
        {
            try
            {
                string host = new Uri(url).Host;
                int[] ports = { 80, 443, 8080, 8443 };
                Invoke((Action)(() => Log("🔌", "Port scan (common web ports):")));
                foreach (int port in ports)
                {
                    try
                    {
                        var tcp = new System.Net.Sockets.TcpClient();
                        var conn = tcp.ConnectAsync(host, port);
                        bool open = await Task.WhenAny(conn, Task.Delay(2000)) == conn && tcp.Connected;
                        Invoke((Action)(() =>
                            Log(open ? "✅" : "🔒", $"  Port {port}: {(open ? "OPEN" : "closed/filtered")}")));
                    }
                    catch { Invoke((Action)(() => Log("🔒", $"  Port {port}: closed/filtered"))); }
                }
            }
            catch (Exception ex) { Invoke((Action)(() => Log("❌", "Port check failed: " + ex.Message))); }
        }

        private async Task CheckSensitiveFiles(string url)
        {
            string[] paths = { "/.env", "/robots.txt", "/.git/HEAD", "/admin", "/wp-login.php", "/phpinfo.php", "/.htaccess" };
            string authority = new Uri(url.TrimEnd('/')).GetLeftPart(UriPartial.Authority);
            Invoke((Action)(() => Log("📂", "Sensitive paths:")));
            foreach (string path in paths)
            {
                try
                {
                    var resp = await _http.GetAsync(authority + path);
                    int code = (int)resp.StatusCode;
                    Invoke((Action)(() =>
                        Log(code == 200 ? "🚨" : code == 403 ? "⚠️" : "✅", $"  {path} → HTTP {code}")));
                }
                catch { Invoke((Action)(() => Log("✅", $"  {path} → Not reachable"))); }
            }
        }

        // ─── TAB: HEADER ANALYZER ────────────────────────────────────────────────

        private async void button_AnalyzeHeaders_Click(object sender, EventArgs e)
        {
            ClearOut();
            string url = NormalizeUrl(textBox_Url.Text);
            button_AnalyzeHeaders.Enabled = false;
            SetProgress(true);
            Log("📋", $"Full Header Dump → {url}");
            LogSep();
            try
            {
                var resp = await _http.GetAsync(url);
                Log("ℹ️", $"Status: {(int)resp.StatusCode} {resp.StatusCode}");
                LogSep();
                foreach (var h in resp.Headers)
                    foreach (var v in h.Value) Log("→", $"{h.Key}: {v}");
                foreach (var h in resp.Content.Headers)
                    foreach (var v in h.Value) Log("→", $"{h.Key}: {v}");
            }
            catch (Exception ex) { Log("❌", "Header analysis failed: " + ex.Message); }
            finally
            {
                LogSep();
                Log("✅", "Header analysis complete.");
                button_AnalyzeHeaders.Enabled = true;
                SetProgress(false);
            }
        }

        // ─── TAB: DNS LOOKUP ─────────────────────────────────────────────────────

        private async void button_DnsLookup_Click(object sender, EventArgs e)
        {
            ClearOut();
            string url = NormalizeUrl(textBox_Url.Text);
            button_DnsLookup.Enabled = false;
            SetProgress(true);
            try
            {
                string host = new Uri(url).Host;
                Log("🌐", $"DNS Lookup → {host}");
                LogSep();
                var addresses = await Dns.GetHostAddressesAsync(host);
                foreach (var addr in addresses) Log("📍", $"IP Address: {addr}");
                var entry = await Dns.GetHostEntryAsync(host);
                Log("🏷", $"Hostname: {entry.HostName}");
                foreach (var alias in entry.Aliases) Log("↪️", $"Alias: {alias}");
            }
            catch (Exception ex) { Log("❌", "DNS lookup failed: " + ex.Message); }
            finally
            {
                LogSep();
                Log("✅", "DNS lookup complete.");
                button_DnsLookup.Enabled = true;
                SetProgress(false);
            }
        }

        // ─── SAVE / CLEAR ─────────────────────────────────────────────────────────

        private void button_SaveReport_Click(object sender, EventArgs e)
        {
            if (listBox_Output.Items.Count == 0)
            {
                MessageBox.Show("No scan results to save.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var dlg = new SaveFileDialog
            {
                Filter = "Text File|*.txt",
                FileName = $"ThreatScanner_Report_{DateTime.Now:yyyyMMdd_HHmmss}"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(dlg.FileName,
                    listBox_Output.Items.Cast<string>().ToArray(), Encoding.UTF8);
                MessageBox.Show($"Report saved: {dlg.FileName}", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button_ClearOutput_Click(object sender, EventArgs e) => ClearOut();
    }
}
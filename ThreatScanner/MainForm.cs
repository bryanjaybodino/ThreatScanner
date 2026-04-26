using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreatScanner
{
    public partial class MainForm : Form
    {
        private readonly HttpClient _httpClient;

        public MainForm()
        {
            InitializeComponent();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ThreatScanner/1.0");
            _httpClient.Timeout = TimeSpan.FromSeconds(15);

            // ── Wire up body-type radio buttons to toggle input visibility
            radioButton_BodyNone.CheckedChanged += (s, e) => UpdateBodyInputVisibility();
            radioButton_BodyForm.CheckedChanged += (s, e) => UpdateBodyInputVisibility();
            radioButton_BodyJson.CheckedChanged += (s, e) => UpdateBodyInputVisibility();
            radioButton_BodyRaw.CheckedChanged += (s, e) => UpdateBodyInputVisibility();

            // Set initial state
            UpdateBodyInputVisibility();
        }

        // ─── BODY INPUT VISIBILITY ────────────────────────────────────────────────

        /// <summary>
        /// Shows the correct input control for the selected body type:
        ///   none  → nothing visible
        ///   form  → dataGridView_FormData (dynamic key/value rows)
        ///   JSON  → textBox_Body (code editor)
        ///   raw   → textBox_Body (plain text)
        /// </summary>
        private void UpdateBodyInputVisibility()
        {
            bool isNone = radioButton_BodyNone.Checked;
            bool isForm = radioButton_BodyForm.Checked;

            dataGridView_FormData.Visible = isForm;
            textBox_Body.Visible = !isForm && !isNone;
        }

        // ─── HELPERS ─────────────────────────────────────────────────────────────

        private void Log(string icon, string message)
        {
            if (listBox_Output.InvokeRequired)
                listBox_Output.Invoke((Action)(() => listBox_Output.Items.Add($"{icon}  {message}")));
            else
                listBox_Output.Items.Add($"{icon}  {message}");
        }

        private void HtmlLog(string icon, string message)
        {
            const int MaxLen = 300;
            string safe = message.Replace("\r", "").Replace("\n", " | ");
            if (safe.Length > MaxLen) safe = safe.Substring(0, MaxLen) + "…";
            Log(icon, safe);
        }

        private void LogSeparator()
        {
            Log("", "─────────────────────────────────────────────");
        }

        private string NormalizeUrl(string raw)
        {
            raw = raw.Trim();
            if (!raw.StartsWith("http://") && !raw.StartsWith("https://"))
                raw = "https://" + raw;
            return raw;
        }

        private void ClearOutput()
        {
            if (listBox_Output.InvokeRequired)
                listBox_Output.Invoke((Action)(() => listBox_Output.Items.Clear()));
            else
                listBox_Output.Items.Clear();
        }

        // ─── EXTRACT ASP.NET HIDDEN FIELDS ───────────────────────────────────────

        private string ExtractHiddenField(string html, string fieldName)
        {
            string pattern = $"id=\"{fieldName}\" value=\"";
            int start = html.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
            if (start < 0) return "";
            start += pattern.Length;
            int end = html.IndexOf("\"", start);
            return end < 0 ? "" : html.Substring(start, end - start);
        }

        // ─── FRAMEWORK DETECTION ─────────────────────────────────────────────────

        private enum LoginFramework { AspNetWebForms, PhpOrHtml, Generic }

        private LoginFramework DetectFramework(string html)
        {
            if (html.IndexOf("__VIEWSTATE", StringComparison.OrdinalIgnoreCase) >= 0)
                return LoginFramework.AspNetWebForms;
            if (html.IndexOf("<form", StringComparison.OrdinalIgnoreCase) >= 0)
                return LoginFramework.PhpOrHtml;
            return LoginFramework.Generic;
        }

        private Dictionary<string, string> ExtractAllHiddenFields(string html)
        {
            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string lower = html.ToLowerInvariant();
            int search = 0;
            while (true)
            {
                int inputPos = lower.IndexOf("<input", search);
                if (inputPos < 0) break;
                int closePos = lower.IndexOf(">", inputPos);
                if (closePos < 0) break;
                string tag = html.Substring(inputPos, closePos - inputPos + 1);
                if (tag.IndexOf("type=\"hidden\"", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    tag.IndexOf("type='hidden'", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    string name = ParseAttr(tag, "name");
                    string value = ParseAttr(tag, "value");
                    if (!string.IsNullOrEmpty(name))
                        fields[name] = value;
                }
                search = closePos + 1;
            }
            return fields;
        }

        private string DetectEventTarget(string html)
        {
            string lower = html.ToLowerInvariant();
            string marker = "__dopostback('";
            int search = 0;
            string firstTarget = "";
            string loginTarget = "";
            string submitTarget = "";

            while (true)
            {
                int pos = lower.IndexOf(marker, search);
                if (pos < 0) break;

                int start = pos + marker.Length;
                int end = html.IndexOf("'", start);
                if (end <= start) break;

                string target = html.Substring(start, end - start);

                if (string.IsNullOrEmpty(firstTarget))
                    firstTarget = target;

                string tLow = target.ToLowerInvariant();

                if (tLow.Contains("login") && string.IsNullOrEmpty(loginTarget))
                    loginTarget = target;

                if (tLow.Contains("submit") && string.IsNullOrEmpty(submitTarget))
                    submitTarget = target;

                search = end + 1;
            }

            if (!string.IsNullOrEmpty(loginTarget)) return loginTarget;
            if (!string.IsNullOrEmpty(submitTarget)) return submitTarget;

            int btnPos = html.IndexOf("type=\"submit\"", StringComparison.OrdinalIgnoreCase);
            if (btnPos < 0) btnPos = html.IndexOf("type='submit'", StringComparison.OrdinalIgnoreCase);
            if (btnPos >= 0)
            {
                int tagStart = html.LastIndexOf("<input", btnPos, StringComparison.OrdinalIgnoreCase);
                if (tagStart >= 0)
                {
                    int tagEnd = html.IndexOf(">", btnPos);
                    string tag = tagEnd > 0 ? html.Substring(tagStart, tagEnd - tagStart) : "";
                    string name = ParseAttr(tag, "name");
                    if (!string.IsNullOrEmpty(name)) return name;
                }
            }

            return firstTarget;
        }

        private string ParseAttr(string tag, string attr)
        {
            foreach (string quote in new[] { "\"", "'" })
            {
                string pattern = $"{attr}={quote}";
                int start = tag.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
                if (start >= 0)
                {
                    start += pattern.Length;
                    int end = tag.IndexOf(quote, start);
                    if (end >= 0) return tag.Substring(start, end - start);
                }
            }
            return "";
        }

        private bool IsLoginSuccess(string body, int statusCode, Uri loginUri,
            HttpResponseMessage response, LoginFramework framework)
        {
            string lower = body.ToLowerInvariant();

            if (response.Headers.Contains("Set-Cookie"))
            {
                var cookies = response.Headers.GetValues("Set-Cookie").ToList();
                int sessionCookieCount = cookies.Count(c =>
                    c.ToLower().Contains("expires=") || c.ToLower().Contains("max-age="));
                if (sessionCookieCount >= 2)
                    return true;
            }

            if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
            {
                string location = response.Headers.Location?.ToString() ?? "";
                if (!location.ToLower().Contains("login"))
                    return true;
            }

            if (lower.Contains("_direct(") ||
                lower.Contains("window.location") ||
                lower.Contains("location.href") ||
                lower.Contains("location.replace"))
            {
                if (!lower.Contains("login"))
                    return true;
            }

            string[] successKeywords = {
                "dashboard", "logout", "sign out", "signout",
                "welcome", "my account", "profile", "home page"
            };
            if (successKeywords.Any(k => lower.Contains(k)))
                return true;

            bool stillHasPasswordField =
                lower.Contains("type=\"password\"") ||
                lower.Contains("type='password'");
            if (!stillHasPasswordField)
                return true;

            return false;
        }

        private bool IsLoginFailure(string body, LoginFramework framework)
        {
            string lower = body.ToLowerInvariant();

            string[] failKeywords = {
                "invalid password", "invalid username", "invalid credentials",
                "incorrect password", "incorrect username",
                "wrong password", "wrong username",
                "login failed", "authentication failed",
                "bad credentials", "unauthorized",
                "try again", "not match",
                "invalid login"
            };

            return failKeywords.Any(k => lower.Contains(k));
        }

        // ─── TAB: SCANNER ────────────────────────────────────────────────────────

        private async void button_Scan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_Url.Text))
            {
                MessageBox.Show("Please enter a URL.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ClearOutput();
            string url = NormalizeUrl(textBox_Url.Text);
            button_Scan.Enabled = false;
            progressBar_Scan.Style = ProgressBarStyle.Marquee;

            Log("🔍", $"Scanning: {url}");
            LogSeparator();

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
                    LogSeparator();
                    Log("✅", "Scan complete.");
                    button_Scan.Enabled = true;
                    progressBar_Scan.Style = ProgressBarStyle.Blocks;
                    progressBar_Scan.Value = 0;
                }));
            }
        }

        private Task CheckHttps(string url)
        {
            Invoke((Action)(() =>
            {
                if (url.StartsWith("https://"))
                    Log("✅", "HTTPS: Secure connection detected");
                else
                    Log("⚠️", "HTTPS: Site is using plain HTTP — data is NOT encrypted");
            }));
            return Task.CompletedTask;
        }

        private async Task CheckSecurityHeaders(string url)
        {
            try
            {
                HttpResponseMessage resp = await _httpClient.GetAsync(url);
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
                    string icon = found ? "✅" : "⚠️";
                    string note = found ? string.Join(", ", headers.GetValues(h)) : "MISSING";
                    Invoke((Action)(() => Log(icon, $"  {h}: {note}")));
                }
                if (headers.Contains("Server"))
                    Invoke((Action)(() => Log("⚠️",
                        $"  Server header exposed: {string.Join(", ", headers.GetValues("Server"))} — consider hiding")));
                else
                    Invoke((Action)(() => Log("✅", "  Server header: Not exposed")));
            }
            catch (Exception ex)
            {
                Invoke((Action)(() => Log("❌", "Security headers check failed: " + ex.Message)));
            }
        }

        private async Task CheckCookieFlags(string url)
        {
            try
            {
                var handler = new HttpClientHandler
                { UseCookies = true, CookieContainer = new CookieContainer() };
                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("User-Agent", "ThreatScanner/1.0");
                HttpResponseMessage resp = await client.GetAsync(url);
                var cookies = resp.Headers.Contains("Set-Cookie")
                    ? resp.Headers.GetValues("Set-Cookie").ToList()
                    : new List<string>();

                if (cookies.Count == 0)
                { Invoke((Action)(() => Log("ℹ️", "Cookies: None set by server"))); return; }

                Invoke((Action)(() => Log("🍪", $"Cookies: {cookies.Count} cookie(s) found")));
                foreach (var cookie in cookies)
                {
                    bool hasHttpOnly = cookie.IndexOf("HttpOnly", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool hasSecure = cookie.IndexOf("Secure", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool hasSameSite = cookie.IndexOf("SameSite", StringComparison.OrdinalIgnoreCase) >= 0;
                    string name = cookie.Split('=')[0].Trim();
                    Invoke((Action)(() =>
                        Log(hasHttpOnly ? "✅" : "⚠️",
                            $"  {name} → HttpOnly:{(hasHttpOnly ? "Yes" : "NO")}  " +
                            $"Secure:{(hasSecure ? "Yes" : "NO")}  " +
                            $"SameSite:{(hasSameSite ? "Yes" : "NO")}")));
                }
            }
            catch (Exception ex)
            {
                Invoke((Action)(() => Log("❌", "Cookie check failed: " + ex.Message)));
            }
        }

        private async Task CheckRedirects(string url)
        {
            try
            {
                var handler = new HttpClientHandler { AllowAutoRedirect = false };
                var client = new HttpClient(handler);
                HttpResponseMessage resp = await client.GetAsync(url);
                int code = (int)resp.StatusCode;
                if (code >= 300 && code < 400)
                {
                    string location = resp.Headers.Location?.ToString() ?? "Unknown";
                    Invoke((Action)(() => Log("↪️", $"Redirect: {code} → {location}")));
                }
                else
                    Invoke((Action)(() => Log("✅", $"No redirect — Status: {code}")));
            }
            catch (Exception ex)
            {
                Invoke((Action)(() => Log("❌", "Redirect check failed: " + ex.Message)));
            }
        }

        private void CheckLocalStorage(string url)
        {
            Invoke((Action)(() =>
            {
                webBrowser_Hidden.DocumentCompleted -= WebBrowser_Hidden_DocumentCompleted;
                webBrowser_Hidden.DocumentCompleted += WebBrowser_Hidden_DocumentCompleted;
                webBrowser_Hidden.Navigate(url);
                Log("🔄", "localStorage: Loading page for JS inspection...");
            }));
        }

        private void WebBrowser_Hidden_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != webBrowser_Hidden.Url) return;
            try
            {
                object result = webBrowser_Hidden.Document?.InvokeScript(
                    "eval", new object[] { "localStorage.length" });
                if (result == null)
                { Log("❌", "localStorage: Document was null"); return; }
                int count = Convert.ToInt32(result);
                Log(count > 0 ? "⚠️" : "✅",
                    count > 0
                        ? $"localStorage: {count} item(s) found — review for sensitive data"
                        : "localStorage: Empty — no stored items");
            }
            catch (Exception ex)
            {
                Log("❌", "localStorage check error: " + ex.Message);
            }
            finally
            {
                webBrowser_Hidden.DocumentCompleted -= WebBrowser_Hidden_DocumentCompleted;
            }
        }

        private async Task CheckSqlInjectionHints(string url)
        {
            try
            {
                string testUrl = url.Contains("?") ? url + "&id=1'" : url + "?id=1'";
                string body = await (await _httpClient.GetAsync(testUrl)).Content.ReadAsStringAsync();
                string[] sqlErrors = {
                    "syntax error", "sql", "mysql", "ORA-",
                    "Microsoft OLE DB", "SQLite", "pg_query", "Warning: pg_"
                };
                bool found = sqlErrors.Any(err =>
                    body.IndexOf(err, StringComparison.OrdinalIgnoreCase) >= 0);
                Invoke((Action)(() =>
                    Log(found ? "🚨" : "✅",
                        found
                            ? "SQL Injection: Possible SQL error disclosure detected!"
                            : "SQL Injection: No obvious error disclosure from basic probe")));
            }
            catch (Exception ex)
            {
                Invoke((Action)(() => Log("❌", "SQLi check failed: " + ex.Message)));
            }
        }

        private async Task CheckXssHints(string url)
        {
            try
            {
                string xssPayload = "<script>xss</script>";
                string htmlEncoded = "&lt;script&gt;xss&lt;/script&gt;";
                string testUrl = url.Contains("?")
                    ? url + "&q=" + Uri.EscapeDataString(xssPayload)
                    : url + "?q=" + Uri.EscapeDataString(xssPayload);

                string body = await (await _httpClient.GetAsync(testUrl)).Content.ReadAsStringAsync();
                bool reflectedRaw = body.Contains(xssPayload);
                bool reflectedEncoded = body.Contains(htmlEncoded);

                Invoke((Action)(() =>
                {
                    if (reflectedRaw)
                        Log("🚨", "XSS: Payload reflected UNENCODED — possible reflected XSS!");
                    else if (reflectedEncoded)
                        Log("⚠️", "XSS: Payload reflected HTML-encoded — likely safe but review");
                    else
                        Log("✅", "XSS: Payload not reflected (basic probe)");
                }));
            }
            catch (Exception ex)
            {
                Invoke((Action)(() => Log("❌", "XSS check failed: " + ex.Message)));
            }
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
                            Log(open ? "✅" : "🔒",
                                $"  Port {port}: {(open ? "OPEN" : "closed/filtered")}")));
                    }
                    catch { Invoke((Action)(() => Log("🔒", $"  Port {port}: closed/filtered"))); }
                }
            }
            catch (Exception ex)
            {
                Invoke((Action)(() => Log("❌", "Port check failed: " + ex.Message)));
            }
        }

        private async Task CheckSensitiveFiles(string url)
        {
            string[] paths = {
                "/.env", "/robots.txt", "/.git/HEAD",
                "/admin", "/wp-login.php", "/phpinfo.php", "/.htaccess"
            };
            string authority = new Uri(url.TrimEnd('/')).GetLeftPart(UriPartial.Authority);
            Invoke((Action)(() => Log("📂", "Sensitive paths:")));
            foreach (string path in paths)
            {
                try
                {
                    var resp = await _httpClient.GetAsync(authority + path);
                    int code = (int)resp.StatusCode;
                    Invoke((Action)(() =>
                        Log(code == 200 ? "🚨" : code == 403 ? "⚠️" : "✅",
                            $"  {path} → HTTP {code}")));
                }
                catch { Invoke((Action)(() => Log("✅", $"  {path} → Not reachable"))); }
            }
        }

        // ─── TAB: BRUTE FORCE ────────────────────────────────────────────────────

        private (string userField, string passField) AutoDetectLoginFields(string html)
        {
            string detectedPass = "";
            string lastTextBeforePass = "";

            string lower = html.ToLowerInvariant();
            int search = 0;

            while (true)
            {
                int inputPos = lower.IndexOf("<input", search);
                if (inputPos < 0) break;

                int closePos = lower.IndexOf(">", inputPos);
                if (closePos < 0) break;

                string tag = html.Substring(inputPos, closePos - inputPos + 1);
                string typeVal = ParseAttr(tag, "type").ToLowerInvariant();
                string nameVal = ParseAttr(tag, "name");

                if (!string.IsNullOrEmpty(nameVal))
                {
                    if (typeVal == "password")
                    {
                        detectedPass = nameVal;
                        break;
                    }
                    else if (typeVal == "text" || typeVal == "email")
                    {
                        lastTextBeforePass = nameVal;
                    }
                }

                search = closePos + 1;
            }

            return (lastTextBeforePass, detectedPass);
        }

        private async void button_BruteForce_Click(object sender, EventArgs e)
        {
            ClearOutput();
            string url = NormalizeUrl(textBox_Url.Text);
            string username = textBox_Username.Text.Trim();
            string password = textBox_Password.Text;

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Enter URL and username.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            button_BruteForce.Enabled = false;
            progressBar_Scan.Style = ProgressBarStyle.Marquee;

            Log("🔐", $"Brute Force (Auto-Detect Framework) → {url}");
            Log("🔎", "Auto-detecting login field names from the page...");
            LogSeparator();

            List<string> passwords = new List<string>();
            if (!string.IsNullOrEmpty(textBox_WordlistPath.Text) &&
                File.Exists(textBox_WordlistPath.Text))
            {
                passwords = File.ReadAllLines(textBox_WordlistPath.Text).ToList();
                Log("📄", $"Wordlist: {passwords.Count} password(s)");
            }
            else
            {
                passwords.Add(password);
            }

            try
            {
                await Task.Run(async () =>
                {
                    var jar = new CookieContainer();
                    var handler = new HttpClientHandler
                    { CookieContainer = jar, UseCookies = true, AllowAutoRedirect = false };
                    var client = new HttpClient(handler)
                    { Timeout = TimeSpan.FromSeconds(15) };
                    client.DefaultRequestHeaders.Add("User-Agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ThreatScanner/1.0");

                    var probeResp = await client.GetAsync(url);
                    string probeHtml = await probeResp.Content.ReadAsStringAsync();
                    LoginFramework fw = DetectFramework(probeHtml);

                    var (usernameField, passwordField) = AutoDetectLoginFields(probeHtml);
                    Invoke((Action)(() =>
                    {
                        Log("🔎", $"Framework detected: {fw}");
                        Log("👤", $"Auto-detected → User: [{usernameField}]  Pass: [{passwordField}]");
                    }));

                    foreach (string pwd in passwords)
                    {
                        try
                        {
                            var getResp = await client.GetAsync(url);
                            string html = await getResp.Content.ReadAsStringAsync();
                            var fields = new List<KeyValuePair<string, string>>();

                            if (fw == LoginFramework.AspNetWebForms)
                            {
                                var hidden = ExtractAllHiddenFields(html);
                                foreach (var kv in hidden)
                                    fields.Add(new KeyValuePair<string, string>(kv.Key, kv.Value));

                                string et = DetectEventTarget(html);

                                fields.RemoveAll(f =>
                                    f.Key == "__EVENTTARGET" ||
                                    f.Key == "__EVENTARGUMENT" ||
                                    f.Key == "__SCROLLPOSITIONX" ||
                                    f.Key == "__SCROLLPOSITIONY");

                                fields.Add(new KeyValuePair<string, string>("__EVENTTARGET", et));
                                fields.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", ""));
                                fields.Add(new KeyValuePair<string, string>("__SCROLLPOSITIONX", "0"));
                                fields.Add(new KeyValuePair<string, string>("__SCROLLPOSITIONY", "0"));
                                int vsLen = hidden.ContainsKey("__VIEWSTATE") ? hidden["__VIEWSTATE"].Length : 0;
                                Invoke((Action)(() => Log("🔄",
                                    $"  ASP.NET → EVENTTARGET: {et}  VIEWSTATE len: {vsLen}")));
                            }
                            else if (fw == LoginFramework.PhpOrHtml)
                            {
                                var hidden = ExtractAllHiddenFields(html);
                                foreach (var kv in hidden)
                                    fields.Add(new KeyValuePair<string, string>(kv.Key, kv.Value));
                                Invoke((Action)(() => Log("🔄",
                                    $"  PHP/HTML → {hidden.Count} hidden field(s) harvested")));

                                string submitName = "";
                                int btnPos = html.IndexOf("type=\"submit\"", StringComparison.OrdinalIgnoreCase);
                                if (btnPos >= 0)
                                {
                                    int tagStart = html.LastIndexOf("<button", btnPos, StringComparison.OrdinalIgnoreCase);
                                    if (tagStart < 0)
                                        tagStart = html.LastIndexOf("<input", btnPos, StringComparison.OrdinalIgnoreCase);

                                    if (tagStart >= 0)
                                    {
                                        int tagEnd = html.IndexOf(">", btnPos);
                                        string tag = html.Substring(tagStart, tagEnd - tagStart);
                                        submitName = ParseAttr(tag, "name");
                                    }
                                }

                                if (!string.IsNullOrEmpty(submitName))
                                    fields.Add(new KeyValuePair<string, string>(submitName, "SUBMIT"));
                            }
                            else
                            {
                                Invoke((Action)(() => Log("🔄", "  Generic → username + password only")));
                            }

                            var (curUser, curPass) = AutoDetectLoginFields(html);
                            if (string.IsNullOrEmpty(curUser)) curUser = usernameField;
                            if (string.IsNullOrEmpty(curPass)) curPass = passwordField;

                            fields.RemoveAll(f =>
                                f.Key.Equals(curUser, StringComparison.OrdinalIgnoreCase) ||
                                f.Key.Equals(curPass, StringComparison.OrdinalIgnoreCase));
                            fields.Add(new KeyValuePair<string, string>(curUser, username));
                            fields.Add(new KeyValuePair<string, string>(curPass, pwd));

                            client.DefaultRequestHeaders.Remove("Referer");
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Referer", url);
                            if (!client.DefaultRequestHeaders.Contains("X-Requested-With"))
                                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                            if (!client.DefaultRequestHeaders.Contains("Accept"))
                                client.DefaultRequestHeaders.Add("Accept", "*/*");

                            string postUrl = url;
                            if (fw == LoginFramework.PhpOrHtml)
                            {
                                string action = ExtractFormAction(html);
                                if (string.IsNullOrEmpty(action))
                                {
                                    if (html.Contains("login_process.php"))
                                        action = "login_process.php";
                                }
                                postUrl = BuildPostUrl(url, action);
                            }

                            Invoke((Action)(() =>
                            {
                                Log("🌐", $"POST URL: {postUrl}");
                                Log("📦", "FIELDS:");
                                foreach (var f in fields)
                                    Log("   ", $"{f.Key} = {f.Value}");
                            }));

                            var postResp = await client.PostAsync(postUrl, new FormUrlEncodedContent(fields));
                            string body = await postResp.Content.ReadAsStringAsync();

                            int code = (int)postResp.StatusCode;
                            Uri loginUri = new Uri(url);
                            bool failure = IsLoginFailure(body, fw);
                            bool success = !failure && IsLoginSuccess(body, code, loginUri, postResp, fw);

                            string icon, label;
                            if (success)
                            {
                                icon = "🚨";
                                label = "SUCCESS — credentials accepted!";
                                Invoke((Action)(() =>
                                {
                                    Log(icon, $"  [{pwd}] HTTP {code}  {label}");
                                    HtmlLog("   ", $"  response: {body}");
                                }));
                                break;
                            }
                            else
                            {
                                icon = "🔒";
                                label = "Failed — wrong credentials";
                                Invoke((Action)(() =>
                                {
                                    Log(icon, $"  [{pwd}] HTTP {code}  {label}");
                                }));
                            }

                            if (success && !failure) break;
                        }
                        catch (Exception ex)
                        {
                            Invoke((Action)(() => Log("❌", "Request error: " + ex.Message)));
                            break;
                        }
                    }
                });
            }
            finally
            {
                Invoke((Action)(() =>
                {
                    LogSeparator();
                    Log("✅", "Brute force test complete.");
                    button_BruteForce.Enabled = true;
                    progressBar_Scan.Style = ProgressBarStyle.Blocks;
                    progressBar_Scan.Value = 0;
                }));
            }
        }

        private string ExtractFormAction(string html)
        {
            int formPos = html.IndexOf("<form", StringComparison.OrdinalIgnoreCase);
            if (formPos < 0) return "";

            int actionPos = html.IndexOf("action=", formPos, StringComparison.OrdinalIgnoreCase);
            if (actionPos < 0) return "";

            foreach (string quote in new[] { "\"", "'" })
            {
                string pattern = $"action={quote}";
                int start = html.IndexOf(pattern, formPos, StringComparison.OrdinalIgnoreCase);
                if (start >= 0)
                {
                    start += pattern.Length;
                    int end = html.IndexOf(quote, start);
                    if (end > start)
                        return html.Substring(start, end - start);
                }
            }

            return "";
        }

        private string BuildPostUrl(string baseUrl, string action)
        {
            if (string.IsNullOrEmpty(action))
                return baseUrl;

            try
            {
                Uri baseUri = new Uri(baseUrl);
                Uri fullUri = new Uri(baseUri, action);
                return fullUri.ToString();
            }
            catch
            {
                return baseUrl;
            }
        }

        // ─── TAB: API TESTER (Postman-style) ─────────────────────────────────────

        private async void button_ApiForce_Click(object sender, EventArgs e)
        {
            ClearOutput();

            // ── Resolve URL
            string rawUrl = string.IsNullOrWhiteSpace(textBox_ApiEndpoint.Text)
                ? textBox_Url.Text
                : textBox_ApiEndpoint.Text;
            if (string.IsNullOrWhiteSpace(rawUrl))
            {
                MessageBox.Show("Enter an endpoint URL.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string url = NormalizeUrl(rawUrl);
            string method = comboBox_Method.SelectedItem?.ToString() ?? "GET";

            button_ApiForce.Enabled = false;
            progressBar_Scan.Style = ProgressBarStyle.Marquee;

            Log("🛰", $"{method} → {url}");
            LogSeparator();

            // ── Snapshot UI on UI thread before Task.Run
            var queryParams = GetEnabledGridRows(dataGridView_Params, "col_ParamKey", "col_ParamValue");
            var extraHeaders = GetEnabledGridRows(dataGridView_Headers, "col_HdrKey", "col_HdrValue");

            // ── Read form-data grid rows (new dynamic grid)
            var formDataRows = GetEnabledGridRows(dataGridView_FormData, "col_FormKey", "col_FormValue");

            string bodyText = textBox_Body.Text;
            bool bodyIsJson = radioButton_BodyJson.Checked;
            bool bodyIsForm = radioButton_BodyForm.Checked;
            bool bodyIsRaw = radioButton_BodyRaw.Checked;
            bool bodyIsNone = radioButton_BodyNone.Checked;

            string authType = comboBox_AuthType.SelectedItem?.ToString() ?? "No Auth";
            string authKey = textBox_HeaderKey.Text.Trim();
            string authValue = textBox_HeaderValue.Text.Trim();

            // Wordlist attack options
            bool useWordlist = !string.IsNullOrEmpty(textBox_WordlistPath.Text) &&
                                File.Exists(textBox_WordlistPath.Text);
            bool useQuery = checkBox_UseQuery.Checked;
            bool forceGet = checkBox_IsGaetMethod.Checked;
            bool jsonPayload = checkBox_Json.Checked;
            string wlTarget = textBox_ApiWlTarget.Text.Trim();

            List<string> wordlist = new List<string> { "" };
            if (useWordlist)
            {
                wordlist = File.ReadAllLines(textBox_WordlistPath.Text).ToList();
                Log("📄", $"Wordlist: {wordlist.Count} item(s)");
            }

            try
            {
                await Task.Run(async () =>
                {
                    foreach (string wlValue in wordlist)
                    {
                        try
                        {
                            // ── Build query string
                            var qParams = new Dictionary<string, string>(queryParams);
                            if (!string.IsNullOrEmpty(wlTarget))
                                InjectWordlistValue(qParams, wlTarget, wlValue);

                            string finalUrl = url;
                            if (qParams.Count > 0)
                            {
                                string qs = BuildQueryString(qParams);
                                finalUrl = finalUrl.Contains("?")
                                    ? $"{finalUrl}&{qs}" : $"{finalUrl}?{qs}";
                            }

                            // ── Build HttpRequestMessage
                            string effectiveMethod = forceGet ? "GET" : method;
                            var req = new HttpRequestMessage(
                                new HttpMethod(effectiveMethod), finalUrl);

                            // ── Apply auth header
                            ApplyAuth(req, authType, authKey, authValue);

                            // ── Apply extra headers
                            foreach (var kv in extraHeaders)
                                if (!string.IsNullOrEmpty(kv.Key))
                                    req.Headers.TryAddWithoutValidation(kv.Key, kv.Value);

                            // ── Apply body
                            if (!bodyIsNone && effectiveMethod != "GET" && effectiveMethod != "HEAD")
                            {
                                var bodyParams = new Dictionary<string, string>();
                                if (!string.IsNullOrEmpty(wlTarget) && !useQuery)
                                    InjectWordlistValue(bodyParams, wlTarget, wlValue);

                                if (bodyIsJson || jsonPayload)
                                {
                                    string json = bodyText;
                                    if (bodyParams.Count > 0 && string.IsNullOrWhiteSpace(json))
                                        json = JsonSerializer.Serialize(bodyParams);
                                    req.Content = new StringContent(json, Encoding.UTF8, "application/json");
                                }
                                else if (bodyIsForm)
                                {
                                    // ── Use the dynamic DataGridView rows (not parsed textbox)
                                    var formData = new Dictionary<string, string>(formDataRows);
                                    foreach (var kv in bodyParams) formData[kv.Key] = kv.Value;
                                    req.Content = new FormUrlEncodedContent(formData);
                                }
                                else if (bodyIsRaw)
                                {
                                    req.Content = new StringContent(bodyText, Encoding.UTF8, "text/plain");
                                }
                            }

                            // ── Send
                            var iterClient = BuildClient(extraHeaders);
                            var resp = await iterClient.SendAsync(req);
                            string body = await resp.Content.ReadAsStringAsync();
                            int code = (int)resp.StatusCode;

                            Invoke((Action)(() =>
                            {
                                string icon = code >= 200 && code < 300 ? "✅"
                                            : code >= 400 && code < 500 ? "⚠️"
                                            : code >= 500 ? "🚨" : "ℹ️";

                                Log(icon, $"HTTP {code}  {resp.ReasonPhrase}" +
                                    (useWordlist ? $"  [val: {wlValue}]" : ""));

                                // ── Response headers
                                foreach (var h in resp.Headers)
                                    foreach (var v in h.Value)
                                        Log("→", $"  {h.Key}: {v}");
                                foreach (var h in resp.Content.Headers)
                                    foreach (var v in h.Value)
                                        Log("→", $"  {h.Key}: {v}");

                                LogSeparator();

                                // ── Response body — multi-line, pretty printed ──────────────
                                if (string.IsNullOrWhiteSpace(body))
                                {
                                    Log("📄", "(empty response body)");
                                }
                                else
                                {
                                    // Try to pretty-print JSON
                                    string displayBody = body;
                                    try
                                    {
                                         var doc = JsonDocument.Parse(body);
                                        displayBody = JsonSerializer.Serialize(
                                            doc.RootElement,
                                            new JsonSerializerOptions { WriteIndented = true });
                                    }
                                    catch { /* not JSON — display as-is */ }

                                    string[] bodyLines = displayBody
                                        .Replace("\r\n", "\n")
                                        .Replace("\r", "\n")
                                        .Split('\n');

                                    int lineCount = 0;
                                    foreach (string line in bodyLines)
                                    {
                                        if (string.IsNullOrWhiteSpace(line)) continue;
                                        string trimmed = line.Length > 300
                                            ? line.Substring(0, 300) + "…"
                                            : line;
                                        Log("📄", trimmed);
                                        if (++lineCount >= 200)
                                        {
                                            Log("…", $"(output truncated — {bodyLines.Length} total lines)");
                                            break;
                                        }
                                    }
                                }
                                // ───────────────────────────────────────────────────────────

                                LogSeparator();
                            }));
                        }
                        catch (Exception ex)
                        {
                            Invoke((Action)(() => Log("❌", "Request error: " + ex.Message)));
                            break;
                        }
                    }
                });
            }
            finally
            {
                Invoke((Action)(() =>
                {
                    LogSeparator();
                    Log("✅", "API test complete.");
                    button_ApiForce.Enabled = true;
                    progressBar_Scan.Style = ProgressBarStyle.Blocks;
                    progressBar_Scan.Value = 0;
                }));
            }
        }

        // ── Helpers for API tester ────────────────────────────────────────────────

        private HttpClient BuildClient(Dictionary<string, string> _ignoredHeaders)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(20);
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ThreatScanner/1.0");
            return client;
        }

        private void ApplyAuth(HttpRequestMessage req, string authType, string key, string value)
        {
            switch (authType)
            {
                case "Bearer Token":
                    req.Headers.TryAddWithoutValidation("Authorization",
                        value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                            ? value : "Bearer " + value);
                    break;
                case "API Key":
                    string headerName = string.IsNullOrEmpty(key) ? "X-Api-Key" : key;
                    req.Headers.TryAddWithoutValidation(headerName, value);
                    break;
                case "Basic Auth":
                    string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{key}:{value}"));
                    req.Headers.TryAddWithoutValidation("Authorization", "Basic " + encoded);
                    break;
                case "Custom Header":
                    if (!string.IsNullOrEmpty(key))
                        req.Headers.TryAddWithoutValidation(key, value);
                    break;
                    // "No Auth" → nothing
            }
        }

        /// <summary>Reads enabled rows from a DataGridView with named columns.</summary>
        private Dictionary<string, string> GetEnabledGridRows(
            DataGridView grid, string keyCol, string valueCol)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                var enabledCell = row.Cells.OfType<DataGridViewCheckBoxCell>().FirstOrDefault();
                if (enabledCell != null && enabledCell.Value is bool b && !b) continue;

                string k = row.Cells[keyCol]?.Value?.ToString()?.Trim() ?? "";
                string v = row.Cells[valueCol]?.Value?.ToString()?.Trim() ?? "";
                if (!string.IsNullOrEmpty(k))
                    result[k] = v;
            }
            return result;
        }

        /// <summary>Injects a wordlist value into params dict.</summary>
        private void InjectWordlistValue(Dictionary<string, string> dict, string template, string wlValue)
        {
            foreach (string part in template.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!part.Contains("=")) continue;
                var kv = part.Split(new[] { '=' }, 2);
                dict[kv[0].Trim()] = kv[1].Trim().Replace("{value}", wlValue);
            }
        }

        /// <summary>Parses "key=value" lines from a multiline text box into a dict.</summary>
        private Dictionary<string, string> ParseKeyValueLines(string text)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string line in text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int eq = line.IndexOf('=');
                if (eq <= 0) continue;
                dict[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
            }
            return dict;
        }

        private string BuildQueryString(Dictionary<string, string> parameters)
        {
            return string.Join("&", parameters.Select(kv =>
                $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        }

        // ─── TAB: HEADER ANALYZER ────────────────────────────────────────────────

        private async void button_AnalyzeHeaders_Click(object sender, EventArgs e)
        {
            ClearOutput();
            string url = NormalizeUrl(textBox_Url.Text);
            button_AnalyzeHeaders.Enabled = false;
            progressBar_Scan.Style = ProgressBarStyle.Marquee;

            Log("📋", $"Full Header Dump → {url}");
            LogSeparator();

            try
            {
                HttpResponseMessage resp = await _httpClient.GetAsync(url);
                Log("ℹ️", $"Status: {(int)resp.StatusCode} {resp.StatusCode}");
                LogSeparator();
                foreach (var h in resp.Headers)
                    foreach (var val in h.Value)
                        Log("→", $"{h.Key}: {val}");
                foreach (var h in resp.Content.Headers)
                    foreach (var val in h.Value)
                        Log("→", $"{h.Key}: {val}");
            }
            catch (Exception ex) { Log("❌", "Header analysis failed: " + ex.Message); }
            finally
            {
                LogSeparator();
                Log("✅", "Header analysis complete.");
                button_AnalyzeHeaders.Enabled = true;
                progressBar_Scan.Style = ProgressBarStyle.Blocks;
                progressBar_Scan.Value = 0;
            }
        }

        // ─── TAB: DNS LOOKUP ─────────────────────────────────────────────────────

        private async void button_DnsLookup_Click(object sender, EventArgs e)
        {
            ClearOutput();
            string url = NormalizeUrl(textBox_Url.Text);
            button_DnsLookup.Enabled = false;
            progressBar_Scan.Style = ProgressBarStyle.Marquee;

            try
            {
                string host = new Uri(url).Host;
                Log("🌐", $"DNS Lookup → {host}");
                LogSeparator();
                var addresses = await System.Net.Dns.GetHostAddressesAsync(host);
                foreach (var addr in addresses) Log("📍", $"IP Address: {addr}");
                var entry = await System.Net.Dns.GetHostEntryAsync(host);
                Log("🏷", $"Hostname: {entry.HostName}");
                foreach (var alias in entry.Aliases) Log("↪️", $"Alias: {alias}");
            }
            catch (Exception ex) { Log("❌", "DNS lookup failed: " + ex.Message); }
            finally
            {
                LogSeparator();
                Log("✅", "DNS lookup complete.");
                button_DnsLookup.Enabled = true;
                progressBar_Scan.Style = ProgressBarStyle.Blocks;
            }
        }

        // ─── SAVE REPORT ─────────────────────────────────────────────────────────

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

        // ─── WORDLIST BROWSE ─────────────────────────────────────────────────────

        private void button_BrowseWordlist_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Text Files|*.txt|All Files|*.*",
                Title = "Select Password Wordlist"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
                textBox_WordlistPath.Text = dlg.FileName;
        }

        // ─── CLEAR ───────────────────────────────────────────────────────────────

        private void button_ClearOutput_Click(object sender, EventArgs e) => ClearOutput();
    }
}
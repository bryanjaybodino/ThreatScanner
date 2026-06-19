using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreatScanner.Helpers;

namespace ThreatScanner
{
    public partial class CsrfTesterForm : Form
    {

        // ── Constants ─────────────────────────────────────────────────────────
        private const string CDP_ENDPOINT = "http://localhost:65535";
        private const string EDGE_PATH_X86 = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
        private const string EDGE_PATH_X64 = @"C:\Program Files\Microsoft\Edge\Application\msedge.exe";
        private const string EDGE_SESSION = @"C:\EdgeSession";

        // Persistent browser session variables
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _activePage;



        private static readonly HttpClient CdpHttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        private HttpClient _client;

        // Stores the last auto-detected form fields (including hidden/token fields)
        private List<KeyValuePair<string, string>> _autoFields = new List<KeyValuePair<string, string>>();
        private string _detectedFramework = "";
        private string _detectedAction = "";

        public CsrfTesterForm()
        {
            InitializeComponent();
            ResetClient();
            ScanHelpers.EnableRowDeletion(dataGridView_Headers);
            if (comboBox_Method.SelectedIndex < 0)
                comboBox_Method.SelectedIndex = 0;
        }

        // ─── HTTP CLIENT ──────────────────────────────────────────────────────────

        // Add this class-level field to track the cookie container
        private CookieContainer _cookieJar;

        private void ResetClient()
        {
            _client?.Dispose();
            _cookieJar = new CookieContainer(); // Store reference to update dynamically

            var handler = new HttpClientHandler
            {
                CookieContainer = _cookieJar,
                UseCookies = true,
                AllowAutoRedirect = false
            };

            _client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(20) };
            _client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ThreatScanner/1.0");
        }
        private async Task SyncClientCookiesFromCdpAsync(string url)
        {
            try
            {
                // Get or connect to the active Playwright/CDP page
                IPage page = await GetOrCreateActivePageAsync();

                // Fetch cookies from the current browser context for the specified domain
                var contextCookies = await page.Context.CookiesAsync(new[] { url });

                if (contextCookies != null && _cookieJar != null)
                {
                    var uri = new Uri(url);
                    foreach (var c in contextCookies)
                    {
                        var cookie = new System.Net.Cookie(c.Name, c.Value)
                        {
                            Domain = string.IsNullOrEmpty(c.Domain) ? uri.Host : c.Domain,
                            Path = string.IsNullOrEmpty(c.Path) ? "/" : c.Path,
                            Secure = c.Secure,
                            HttpOnly = c.HttpOnly
                        };

                        _cookieJar.Add(uri, cookie);
                    }
                    Log("🍪", $"Synchronized {contextCookies.Count} cookie(s) from CDP to HttpClient.");
                }
            }
            catch (Exception ex)
            {
                Log("❌", "Failed to sync cookies from CDP: " + ex.Message);
            }
        }
        // ─── LOGGING ─────────────────────────────────────────────────────────────

        private void Log(string icon, string msg) => ScanHelpers.LogRtb(richTextBox_Output, icon, msg);

        private void Log(string message, Color? color = null)
        {
            if (richTextBox_Output.InvokeRequired)
            {
                richTextBox_Output.Invoke(new Action<string, Color?>(Log), message, color);
                return;
            }

            richTextBox_Output.SelectionStart = richTextBox_Output.TextLength;
            richTextBox_Output.SelectionLength = 0;
            richTextBox_Output.SelectionColor = color.HasValue ? color.Value : Color.FromArgb(212, 212, 212);
            richTextBox_Output.AppendText(string.Format("[{0:HH:mm:ss}]  {1}\r\n", DateTime.Now, message));
            richTextBox_Output.ScrollToCaret();
        }

        private void LogSep() => ScanHelpers.LogSeparatorRtb(richTextBox_Output);
        private void ClearOut() => ScanHelpers.ClearOutputRtb(richTextBox_Output);

        private void SetProgress(bool running)
        {
            progressBar_Scan.Style = running ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            if (!running) progressBar_Scan.Value = 0;
        }

        // =========================================================================
        // AUTO-FILL FROM URL
        // Fetches the page, detects framework, parses all form fields,
        // and populates the Forge tab body automatically.
        // =========================================================================

        private async void button_AutoFill_Click(object sender, EventArgs e)
        {
            string rawUrl = textBox_ForgeUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(rawUrl))
            {
                MessageBox.Show("Enter an Endpoint URL first, then click Auto-Fill.",
                    "ThreatScanner", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string url = ScanHelpers.NormalizeUrl(rawUrl);
            ResetClient(); // Clears the client and sets up a fresh, empty cookie container

            button_AutoFill.Enabled = false;
            SetProgress(true);
            ClearOut();
            Log("🔄", $"Auto-Fill → fetching {url}");
            LogSep();

            try
            {
                await Task.Run(async () =>
                {
                    string html = null;
                    try
                    {
                        // ── ADD THIS LINE HERE ───────────────────────────────────────
                        // Pull active session cookies from the live browser via CDP
                        // and inject them into the freshly reset HttpClient handler.
                        await SyncClientCookiesFromCdpAsync(url);
                        // ─────────────────────────────────────────────────────────────

                        var resp = await _client.GetAsync(url);
                        html = await resp.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex)
                    {
                        Invoke((Action)(() => Log("❌", "Fetch failed: " + ex.Message)));
                        return;
                    }

                    if (string.IsNullOrEmpty(html))
                    {
                        Invoke((Action)(() => Log("❌", "Empty response — nothing to parse.")));
                        return;
                    }

                    // ── Detect framework ─────────────────────────────────────────
                    bool isAspNetWebForms = html.IndexOf("__VIEWSTATE", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool isAspNetMvc = html.IndexOf("__RequestVerificationToken", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool isPhp = html.IndexOf(".php", StringComparison.OrdinalIgnoreCase) >= 0
                                 || url.IndexOf(".php", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool isLaravel = html.IndexOf("_token", StringComparison.OrdinalIgnoreCase) >= 0 && isPhp;
                    bool isDjango = html.IndexOf("csrfmiddlewaretoken", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool isRails = html.IndexOf("authenticity_token", StringComparison.OrdinalIgnoreCase) >= 0;

                    string fw;
                    if (isAspNetWebForms) fw = "ASP.NET WebForms";
                    else if (isAspNetMvc) fw = "ASP.NET MVC";
                    else if (isLaravel) fw = "PHP Laravel";
                    else if (isDjango) fw = "Django";
                    else if (isRails) fw = "Ruby on Rails";
                    else if (isPhp) fw = "PHP (plain)";
                    else fw = "Plain HTML";

                    _detectedFramework = fw;

                    // ── Find the first POST form ──────────────────────────────────
                    var formMatch = FindBestForm(html);
                    if (formMatch == null)
                    {
                        Invoke((Action)(() =>
                        {
                            Log("⚠️", "No POST form found on this page.");
                            Log("ℹ️", "Framework detected: " + fw);
                        }));
                        return;
                    }

                    string formHtml = formMatch.Value;

                    // Extract form action
                    string action = ExtractAttr(formHtml, "action");
                    _detectedAction = string.IsNullOrEmpty(action) ? url : BuildAbsoluteUrl(url, action);

                    // ── Parse all input fields ────────────────────────────────────
                    var fields = new List<KeyValuePair<string, string>>();

                    // All <input> tags
                    foreach (Match m in Regex.Matches(formHtml, @"<input[^>]*>", RegexOptions.IgnoreCase))
                    {
                        string tag = m.Value;
                        string type = ExtractAttr(tag, "type").ToLowerInvariant();
                        string name = ExtractAttr(tag, "name");
                        string val = ExtractAttr(tag, "value");

                        if (string.IsNullOrEmpty(name)) continue;
                        if (type == "submit" || type == "image" || type == "button" || type == "reset") continue;

                        fields.Add(new KeyValuePair<string, string>(name, val));
                    }

                    // All <select> tags
                    foreach (Match m in Regex.Matches(formHtml, @"<select[^>]*>[\s\S]*?</select>", RegexOptions.IgnoreCase))
                    {
                        string tag = m.Value;
                        string name = ExtractAttr(tag, "name");
                        if (string.IsNullOrEmpty(name)) continue;
                        var optMatch = Regex.Match(tag, @"<option[^>]*value=[""']?([^""'\s>]*)", RegexOptions.IgnoreCase);
                        string optVal = optMatch.Success ? optMatch.Groups[1].Value : "";
                        fields.Add(new KeyValuePair<string, string>(name, optVal));
                    }

                    // All <textarea> tags
                    foreach (Match m in Regex.Matches(formHtml, @"<textarea[^>]*>([\s\S]*?)</textarea>", RegexOptions.IgnoreCase))
                    {
                        string tag = m.Value;
                        string name = ExtractAttr(tag, "name");
                        if (string.IsNullOrEmpty(name)) continue;
                        string content = Regex.Match(tag, @">([\s\S]*?)</textarea>", RegexOptions.IgnoreCase).Groups[1].Value.Trim();
                        fields.Add(new KeyValuePair<string, string>(name, content));
                    }

                    _autoFields = fields;

                    // ── Identify which fields are CSRF tokens ─────────────────────
                    var csrfTokenNames = new[] {
                "__requestverificationtoken", "csrfmiddlewaretoken", "authenticity_token",
                "_token", "csrf_token", "csrf", "xsrf", "__eventvalidation"
            };

                    // ── Build the body text for the textbox ───────────────────────
                    var bodyLines = new List<string>();
                    var tokenFields = new List<string>();
                    var userFields = new List<string>();

                    foreach (var kv in fields)
                    {
                        bool isToken = csrfTokenNames.Any(t =>
                            kv.Key.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0);
                        bool isHidden = kv.Key.StartsWith("__", StringComparison.Ordinal);

                        if (isToken || isHidden)
                            tokenFields.Add($"{kv.Key}={kv.Value}");
                        else
                            userFields.Add($"{kv.Key}=");
                    }

                    bodyLines.Add("# ── Hidden / Framework fields (auto-filled) ──");
                    bodyLines.AddRange(tokenFields);
                    bodyLines.Add("");
                    bodyLines.Add("# ── User input fields (fill these in) ──");
                    bodyLines.AddRange(userFields);

                    string bodyText = string.Join("\r\n", bodyLines);

                    Invoke((Action)(() =>
                    {
                        textBox_ForgeBody.Text = bodyText;
                        textBox_ForgeUrl.Text = _detectedAction;
                        comboBox_Method.SelectedItem = "POST";

                        Log("✅", $"Framework detected: {fw}");
                        Log("✅", $"Form action: {_detectedAction}");
                        Log("✅", $"Fields found: {fields.Count}");
                        LogSep();

                        foreach (var kv in fields)
                        {
                            bool isToken = csrfTokenNames.Any(t =>
                                kv.Key.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0);

                            if (isToken)
                                Log("🔑", $"  CSRF token field: [{kv.Key}]  ({(kv.Value.Length > 0 ? $"value len={kv.Value.Length}" : "empty")})");
                            else if (kv.Key.StartsWith("__"))
                                Log("⚙️", $"  Framework field: [{kv.Key}]  len={kv.Value.Length}");
                            else
                                Log("✏️", $"  User field: [{kv.Key}]");
                        }

                        LogSep();
                        Log("ℹ️", "Body auto-filled in the Forge tab.");
                        Log("ℹ️", "Fill in the user fields (username/password/etc.) then click ⚡ Send Forged Request.");

                        tabControl_Main.SelectedTab = tabPage_Forge;
                    }));
                });
            }
            catch (Exception ex)
            {
                Log("❌", "Auto-Fill error: " + ex.Message);
            }
            finally
            {
                Invoke((Action)(() =>
                {
                    button_AutoFill.Enabled = true;
                    SetProgress(false);
                }));
            }
        }

        // ─── FIND BEST FORM ───────────────────────────────────────────────────────
        // Prefer login/auth POST forms, fall back to first POST form

        private Match FindBestForm(string html)
        {
            var allForms = Regex.Matches(html, @"<form[\s\S]*?</form>",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            Match firstPost = null;
            foreach (Match m in allForms)
            {
                string method = ExtractAttr(m.Value, "method");
                bool isPost = string.IsNullOrEmpty(method) ||
                              method.Equals("post", StringComparison.OrdinalIgnoreCase);
                if (!isPost) continue;

                if (firstPost == null) firstPost = m;

                string lower = m.Value.ToLowerInvariant();
                if (lower.Contains("login") || lower.Contains("password") ||
                    lower.Contains("signin") || lower.Contains("username") ||
                    lower.Contains("email"))
                    return m;   // Best match — login form
            }
            return firstPost;
        }

        // ─── MAIN SCAN ────────────────────────────────────────────────────────────

        private async void button_Scan_Click(object sender, EventArgs e)
        {
            ClearOut();
            string rawUrl = textBox_Url.Text.Trim();
            if (string.IsNullOrWhiteSpace(rawUrl))
            {
                MessageBox.Show("Enter a target URL.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string url = ScanHelpers.NormalizeUrl(rawUrl);
            ResetClient();
            button_Scan.Enabled = false;
            SetProgress(true);
            Log("🛡", $"CSRF Scan → {url}");
            LogSep();

            try
            {
                await Task.Run(async () =>
                {
                    string html = await FetchAndCheckCookies(url);
                    if (!string.IsNullOrEmpty(html))
                        CheckForms(html, url);
                    await CheckCors(url);
                    await CheckRefererEnforcement(url);
                });
            }
            catch (Exception ex) { Log("❌", "Scan error: " + ex.Message); }
            finally
            {
                Invoke((Action)(() =>
                {
                    LogSep();
                    Log("✅", "CSRF scan complete.");
                    button_Scan.Enabled = true;
                    SetProgress(false);
                }));
            }
        }

        // ─── STEP 1: FETCH + COOKIE FLAGS ────────────────────────────────────────

        // ─── STEP 1: FETCH VIA CDP + COOKIE FLAGS ────────────────────────────────
        private async Task<string> FetchAndCheckCookies(string url)
        {
            Invoke((Action)(() => Log("🍪", "── Cookie / SameSite Analysis (CDP) ──────────────────")));
            try
            {
                // Connect to browser instance and navigate
                IPage page = await GetOrCreateActivePageAsync();

                Invoke((Action)(() => Log("🔄", $"Navigating to {url} via CDP...")));
                await page.GotoAsync(url, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                    Timeout = 60000
                });
                await page.WaitForLoadStateAsync(LoadState.Load, new PageWaitForLoadStateOptions { Timeout = 30000 });

                // Get page HTML structure
                string html = await page.ContentAsync();

                // Fetch cookies via the Playwright Browser Context API
                var contextCookies = await page.Context.CookiesAsync(new[] { url });

                if (contextCookies == null || contextCookies.Count == 0)
                {
                    Invoke((Action)(() => Log("ℹ️", "No cookies found on response payload via CDP session.")));
                }
                else
                {
                    foreach (var cookie in contextCookies)
                    {
                        string cookieName = cookie.Name;
                        bool hasHttpOnly = cookie.HttpOnly;
                        bool hasSecure = cookie.Secure;

                        // Checking Playwright SameSite status mappings
                        string sameSiteStr = cookie.SameSite.ToString();
                        bool hasSameSiteStrict = sameSiteStr.Equals("Strict", StringComparison.OrdinalIgnoreCase);
                        bool hasSameSiteLax = sameSiteStr.Equals("Lax", StringComparison.OrdinalIgnoreCase);
                        bool hasSameSiteNone = sameSiteStr.Equals("None", StringComparison.OrdinalIgnoreCase);

                        Invoke((Action)(() =>
                        {
                            Log("🍪", $"Cookie: {cookieName}");
                            Log(hasHttpOnly ? "✅" : "⚠️",
                                $"  HttpOnly: {(hasHttpOnly ? "Yes" : "MISSING — accessible via JavaScript")}");
                            Log(hasSecure ? "✅" : "⚠️",
                                $"  Secure:   {(hasSecure ? "Yes" : "MISSING — sent over plain HTTP")}");
                            if (hasSameSiteStrict)
                                Log("✅", "  SameSite: Strict — best CSRF protection");
                            else if (hasSameSiteLax)
                                Log("⚠️", "  SameSite: Lax — partial protection");
                            else if (hasSameSiteNone)
                                Log("🚨", "  SameSite: None — cookie sent on ALL cross-site requests (CSRF risk)");
                            else
                                Log("🚨", "  SameSite: NOT SET — explicit value required");
                            LogSep();
                        }));
                    }
                }
                return html;
            }
            catch (Exception ex)
            {
                Invoke((Action)(() => Log("❌", "Cookie check via CDP failed: " + ex.Message)));
                return null;
            }
        }

        // ─── STEP 2: FORM CSRF TOKEN CHECK ───────────────────────────────────────

        private void CheckForms(string html, string baseUrl)
        {
            Invoke((Action)(() => Log("📋", "── Form CSRF Token Analysis ────────────────────────")));

            var formMatches = Regex.Matches(html, @"<form[\s\S]*?</form>",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if (formMatches.Count == 0)
            {
                Invoke((Action)(() => Log("ℹ️", "No HTML forms found on this page.")));
                return;
            }

            Invoke((Action)(() => Log("ℹ️", $"Found {formMatches.Count} form(s).")));

            var csrfPatterns = new[]
            {
                "csrf", "xsrf", "_token", "authenticity_token", "__requestverificationtoken",
                "csrfmiddlewaretoken", "csrf_token", "anti_csrf", "nonce", "_wpnonce", "__eventvalidation"
            };

            int formIndex = 0;
            foreach (Match formMatch in formMatches)
            {
                formIndex++;
                string formHtml = formMatch.Value;
                string action = ExtractAttr(formHtml, "action");
                string method = ExtractAttr(formHtml, "method");
                if (string.IsNullOrEmpty(method)) method = "GET";
                string actionDisplay = string.IsNullOrEmpty(action) ? "(same page)" : action;

                var fieldNames = Regex.Matches(formHtml, @"<input[^>]*>", RegexOptions.IgnoreCase)
                    .Cast<Match>()
                    .Select(m => ExtractAttr(m.Value, "name").ToLowerInvariant())
                    .Where(n => !string.IsNullOrEmpty(n))
                    .ToList();

                bool hasCsrfToken = fieldNames.Any(n => csrfPatterns.Any(p => n.Contains(p)));
                string foundTokenField = hasCsrfToken
                    ? fieldNames.FirstOrDefault(n => csrfPatterns.Any(p => n.Contains(p)))
                    : null;
                bool isPost = method.Equals("POST", StringComparison.OrdinalIgnoreCase);

                Invoke((Action)(() =>
                {
                    Log("📋", $"Form #{formIndex} — Action: {actionDisplay}  Method: {method.ToUpper()}");
                    if (!isPost)
                        Log("ℹ️", "  GET form — typically not a CSRF risk.");
                    else if (hasCsrfToken)
                    {
                        Log("✅", $"  CSRF token found: [{foundTokenField}]");
                        Log("⚠️", "  Verify it is unique per session and validated server-side.");
                    }
                    else
                    {
                        Log("🚨", "  NO CSRF TOKEN detected in this POST form — potentially vulnerable!");
                        Log("🚨", $"  Fields: {string.Join(", ", fieldNames.Take(10))}");
                    }
                    LogSep();
                }));
            }
        }

        // ─── STEP 3: CORS CHECK ───────────────────────────────────────────────────

        private async Task CheckCors(string url)
        {
            Invoke((Action)(() => Log("🌐", "── CORS Origin Reflection Check ────────────────────")));
            try
            {
                string attackerOrigin = "https://evil-attacker.com";
                var req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Headers.TryAddWithoutValidation("Origin", attackerOrigin);
                var resp = await _client.SendAsync(req);

                string acao = resp.Headers.Contains("Access-Control-Allow-Origin")
                    ? string.Join(", ", resp.Headers.GetValues("Access-Control-Allow-Origin")) : "";
                string acac = resp.Headers.Contains("Access-Control-Allow-Credentials")
                    ? string.Join(", ", resp.Headers.GetValues("Access-Control-Allow-Credentials")) : "";

                Invoke((Action)(() =>
                {
                    if (string.IsNullOrEmpty(acao))
                        Log("✅", "Access-Control-Allow-Origin: not present.");
                    else if (acao == "*")
                        Log("⚠️", "Access-Control-Allow-Origin: * (wildcard)");
                    else if (acao.Equals(attackerOrigin, StringComparison.OrdinalIgnoreCase))
                    {
                        Log("🚨", $"ACAO REFLECTS attacker origin: {acao}");
                        if (acac.Equals("true", StringComparison.OrdinalIgnoreCase))
                            Log("🚨", "  + Credentials: true — CRITICAL! CSRF via CORS possible.");
                        else
                            Log("⚠️", "  Arbitrary origin reflected — review CORS policy.");
                    }
                    else
                        Log("✅", $"Access-Control-Allow-Origin: {acao} (attacker NOT reflected)");

                    if (!string.IsNullOrEmpty(acac))
                        Log("ℹ️", $"Access-Control-Allow-Credentials: {acac}");
                    LogSep();
                }));
            }
            catch (Exception ex) { Invoke((Action)(() => Log("❌", "CORS check failed: " + ex.Message))); }
        }

        // ─── STEP 4: REFERER / ORIGIN ENFORCEMENT ────────────────────────────────

        private async Task CheckRefererEnforcement(string url)
        {
            Invoke((Action)(() => Log("🔗", "── Referer / Origin Header Enforcement ─────────────")));
            try
            {
                var req1 = new HttpRequestMessage(HttpMethod.Get, url);
                req1.Headers.TryAddWithoutValidation("Origin", "https://evil-attacker.com");
                var resp1 = await _client.SendAsync(req1);

                var req2 = new HttpRequestMessage(HttpMethod.Get, url);
                req2.Headers.TryAddWithoutValidation("Referer", "https://evil-attacker.com/csrf.html");
                var resp2 = await _client.SendAsync(req2);

                Invoke((Action)(() =>
                {
                    int c1 = (int)resp1.StatusCode, c2 = (int)resp2.StatusCode;
                    Log("ℹ️", $"Request with attacker Origin:  HTTP {c1} {resp1.ReasonPhrase}");
                    Log("ℹ️", $"Request with attacker Referer: HTTP {c2} {resp2.ReasonPhrase}");
                    if (c1 == 403 || c1 == 401 || c2 == 403 || c2 == 401)
                        Log("✅", "Server appears to reject forged Origin/Referer.");
                    else
                    {
                        Log("⚠️", "Server did NOT reject forged Origin/Referer.");
                        Log("ℹ️", "Combine with form token and SameSite findings for full assessment.");
                    }
                    LogSep();
                }));
            }
            catch (Exception ex) { Invoke((Action)(() => Log("❌", "Referer check failed: " + ex.Message))); }
        }

        // ─── FORGE REQUEST ────────────────────────────────────────────────────────

        private async void button_ForgeRequest_Click(object sender, EventArgs e)
        {
            ClearOut();
            string rawUrl = textBox_ForgeUrl.Text.Trim();
            if (string.IsNullOrWhiteSpace(rawUrl))
            {
                MessageBox.Show("Enter an endpoint URL for the forge test.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string url = ScanHelpers.NormalizeUrl(rawUrl);
            string method = comboBox_Method.SelectedItem?.ToString() ?? "POST";
            string origin = textBox_ForgeOrigin.Text.Trim();
            bool omitToken = checkBox_OmitToken.Checked;
            var extraHeaders = ScanHelpers.GetEnabledGridRows(dataGridView_Headers, "col_CsrfHdrKey", "col_CsrfHdrValue");

            // Parse body — skip comment lines (starting with #)
            string bodyText = textBox_ForgeBody.Text.Trim();
            var pairs = bodyText
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !l.TrimStart().StartsWith("#"))
                .Select(p => p.Split(new[] { '=' }, 2))
                .Where(p => p.Length == 2 && !string.IsNullOrWhiteSpace(p[0]))
                .Select(p => new KeyValuePair<string, string>(p[0].Trim(), p[1].Trim()))
                .ToList();

            // Remove CSRF token fields if checkbox is ticked
            if (omitToken)
            {
                var csrfNames = new[] {
                    "__requestverificationtoken", "csrfmiddlewaretoken", "authenticity_token",
                    "_token", "csrf_token", "csrf", "xsrf", "__eventvalidation"
                };
                pairs = pairs.Where(kv =>
                    !csrfNames.Any(t => kv.Key.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }

            button_ForgeRequest.Enabled = false;
            SetProgress(true);

            Log("⚡", $"Forged {method} → {url}");
            if (!string.IsNullOrEmpty(origin))
                Log("ℹ️", $"Simulating cross-site request from: {origin}");
            if (omitToken) Log("🚨", "CSRF token deliberately OMITTED.");
            Log("📦", $"Sending {pairs.Count} field(s):");
            foreach (var kv in pairs)
                Log("   ", $"  {kv.Key} = {(kv.Value.Length > 60 ? kv.Value.Substring(0, 60) + "…" : kv.Value)}");
            LogSep();

            try
            {
                await Task.Run(async () =>
                {
                    ResetClient();
                    await SyncClientCookiesFromCdpAsync(url);
                    var req = new HttpRequestMessage(new HttpMethod(method), url);

                    if (!string.IsNullOrEmpty(origin))
                    {
                        req.Headers.TryAddWithoutValidation("Origin", origin);
                        req.Headers.TryAddWithoutValidation("Referer", origin + "/csrf-poc.html");
                    }

                    foreach (var kv in extraHeaders)
                        if (!string.IsNullOrEmpty(kv.Key))
                            req.Headers.TryAddWithoutValidation(kv.Key, kv.Value);

                    if (method != "GET" && method != "HEAD" && pairs.Any())
                        req.Content = new FormUrlEncodedContent(pairs);

                    var resp = await _client.SendAsync(req);
                    string respBody = await resp.Content.ReadAsStringAsync();
                    int code = (int)resp.StatusCode;

                    Invoke((Action)(() =>
                    {
                        string icon = code >= 200 && code < 300 ? "✅"
                                    : code >= 300 && code < 400 ? "ℹ️"
                                    : code >= 400 && code < 500 ? "⚠️" : "🚨";
                        Log(icon, $"HTTP {code}  {resp.ReasonPhrase}");

                        if (code == 403 || code == 401)
                            Log("✅", "Server REJECTED the forged request — CSRF protection appears active.");
                        else if (code == 500)
                            Log("⚠️", "Server returned 500 — likely __EVENTVALIDATION / ViewState mismatch (ASP.NET WebForms protection).");
                        else if (code >= 200 && code < 300)
                            Log("🚨", "Server ACCEPTED the forged cross-site request — may be CSRF vulnerable!");
                        else if (code >= 300 && code < 400)
                        {
                            string loc = resp.Headers.Location?.ToString() ?? "";
                            if (loc.ToLower().Contains("login"))
                                Log("ℹ️", $"Redirected to login — session required. Location: {loc}");
                            else
                                Log("⚠️", $"Redirect to: {loc} — verify manually.");
                        }

                        LogSep();
                        Log("→", "Response headers:");
                        foreach (var h in resp.Headers)
                            foreach (var v in h.Value)
                                Log("→", $"  {h.Key}: {v}");
                        LogSep();

                        if (!string.IsNullOrWhiteSpace(respBody))
                        {
                            string[] lines = respBody.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
                            int lc = 0;
                            foreach (string line in lines)
                            {
                                if (string.IsNullOrWhiteSpace(line)) continue;
                                Log("", line.Length > 300 ? line.Substring(0, 300) + "…" : line);
                                if (++lc >= 100) { Log("…", $"(truncated — {lines.Length} total lines)"); break; }
                            }
                        }
                        else Log("📄", "(empty response body)");

                        LogSep();
                    }));
                });
            }
            catch (Exception ex) { Log("❌", "Request error: " + ex.Message); }
            finally
            {
                Invoke((Action)(() =>
                {
                    Log("✅", "Forge test complete.");
                    button_ForgeRequest.Enabled = true;
                    SetProgress(false);
                }));
            }
        }

        // ─── GENERATE PoC HTML ────────────────────────────────────────────────────

        private void button_GeneratePoc_Click(object sender, EventArgs e)
        {
            string targetUrl = ScanHelpers.NormalizeUrl(textBox_ForgeUrl.Text.Trim());
            if (string.IsNullOrWhiteSpace(textBox_ForgeUrl.Text))
            {
                MessageBox.Show("Enter a target URL in the Forge tab first.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string method = comboBox_Method.SelectedItem?.ToString() ?? "POST";
            var fields = textBox_ForgeBody.Text
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !l.TrimStart().StartsWith("#"))
                .Select(p => p.Split(new[] { '=' }, 2))
                .Where(p => p.Length == 2)
                .Select(p => (Key: Uri.UnescapeDataString(p[0].Trim()), Value: Uri.UnescapeDataString(p[1].Trim())))
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head><title>CSRF PoC — ThreatScanner</title></head>");
            sb.AppendLine("<body>");
            sb.AppendLine("  <h1>CSRF Proof-of-Concept</h1>");
            sb.AppendLine($"  <p>Target: <b>{System.Net.WebUtility.HtmlEncode(targetUrl)}</b></p>");
            sb.AppendLine($"  <form action=\"{System.Net.WebUtility.HtmlEncode(targetUrl)}\" method=\"{method}\" id=\"csrfForm\">");
            foreach (var (Key, Value) in fields)
                sb.AppendLine($"    <input type=\"hidden\" name=\"{System.Net.WebUtility.HtmlEncode(Key)}\" value=\"{System.Net.WebUtility.HtmlEncode(Value)}\" />");
            sb.AppendLine("    <input type=\"submit\" value=\"Click me\" />");
            sb.AppendLine("  </form>");
            sb.AppendLine("  <script>");
            sb.AppendLine("    // Uncomment to auto-submit silently:");
            sb.AppendLine("    // document.getElementById('csrfForm').submit();");
            sb.AppendLine("  </script>");
            sb.AppendLine("</body></html>");

            string poc = sb.ToString();
            ClearOut();
            Log("⚡", "Generated CSRF Proof-of-Concept HTML:");
            LogSep();
            foreach (string line in poc.Split(new[] { '\n' }, StringSplitOptions.None))
                Log("", line.TrimEnd());
            LogSep();

            var dlg = new SaveFileDialog
            {
                Filter = "HTML File|*.html",
                FileName = $"CSRF_PoC_{DateTime.Now:yyyyMMdd_HHmmss}.html"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dlg.FileName, poc, Encoding.UTF8);
                MessageBox.Show($"PoC saved:\n{dlg.FileName}", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ─── HELPERS ─────────────────────────────────────────────────────────────

        private static string ExtractAttr(string tag, string attr)
        {
            foreach (string q in new[] { "\"", "'" })
            {
                string pattern = $"{attr}={q}";
                int start = tag.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
                if (start >= 0)
                {
                    start += pattern.Length;
                    int end = tag.IndexOf(q, start);
                    if (end >= 0) return tag.Substring(start, end - start);
                }
            }
            var m = Regex.Match(tag, $@"{attr}=([^\s>""']+)", RegexOptions.IgnoreCase);
            return m.Success ? m.Groups[1].Value : "";
        }

        private static string BuildAbsoluteUrl(string baseUrl, string action)
        {
            if (string.IsNullOrEmpty(action)) return baseUrl;
            try { return new Uri(new Uri(baseUrl), action).ToString(); }
            catch { return baseUrl; }
        }

        // ─── SAVE / CLEAR ─────────────────────────────────────────────────────────

        private void button_SaveReport_Click(object sender, EventArgs e)
        {
            if (richTextBox_Output.TextLength == 0) return;
            var dlg = new SaveFileDialog
            {
                Filter = "Text File|*.txt",
                FileName = $"CSRF_Report_{DateTime.Now:yyyyMMdd_HHmmss}"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
                File.WriteAllText(dlg.FileName, richTextBox_Output.Text, Encoding.UTF8);
        }

        private void button_ClearOutput_Click(object sender, EventArgs e) => ClearOut();



        private void CsrfTesterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Disconnect (but do not close) the CDP-attached Edge
            try { _browser?.CloseAsync(); } catch { }
            try { _playwright?.Dispose(); } catch { }
        }

        /// <summary>
        /// Returns the live page to operate on. Connects to the
        /// already-running Edge (via CDP) on first use and reuses the same page.
        /// </summary>
        private async Task<IPage> GetOrCreateActivePageAsync()
        {
            if (_activePage != null && !_activePage.IsClosed)
                return _activePage;

            await EnsureEdgeRunningAsync();

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.ConnectOverCDPAsync(CDP_ENDPOINT);
            IBrowserContext context = _browser.Contexts[0];

            IPage page = null;
            foreach (IPage p in context.Pages)
            {
                if (p.Url.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    page = p;
                    break;
                }
            }
            if (page == null)
                page = context.Pages.Count > 0 ? context.Pages[0] : await context.NewPageAsync();

            _activePage = page;
            return _activePage;
        }

        private async Task EnsureEdgeRunningAsync()
        {
            if (await IsCdpReady()) { Log("[Edge] Already connected via CDP."); return; }

            string edgePath = GetEdgePath();
            if (edgePath == null)
            {
                Log("[Edge] msedge.exe not found. Launch Edge manually with --remote-debugging-port=65535", Color.OrangeRed);
                return;
            }

            Log("[Edge] Closing existing Edge instances ...");
            foreach (Process proc in Process.GetProcessesByName("msedge"))
            {
                try { proc.Kill(); proc.WaitForExit(2000); } catch { }
            }
            await Task.Delay(2500);

            Log("[Edge] Launching Edge with remote debugging on port 65535 ...");
            Process.Start(new ProcessStartInfo
            {
                FileName = edgePath,
                Arguments = "--remote-debugging-port=65535 --user-data-dir=\"" + EDGE_SESSION + "\" --no-first-run --no-default-browser-check",
                UseShellExecute = true
            });

            Log("[Edge] Waiting for Edge to be ready ...", Color.Yellow);
            for (int i = 0; i < 20; i++)
            {
                await Task.Delay(1000);
                if (await IsCdpReady()) { Log("[Edge] Ready!", Color.LightGreen); return; }
            }
            Log("[Edge] Edge did not respond in time - trying anyway.", Color.OrangeRed);
        }
        private static async Task<bool> IsCdpReady()
        {
            try
            {
                using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    HttpResponseMessage res = await CdpHttpClient.GetAsync(CDP_ENDPOINT + "/json/version", cts.Token);
                    return res.IsSuccessStatusCode;
                }
            }
            catch { return false; }
        }
        private static string GetEdgePath()
        {
            if (File.Exists(EDGE_PATH_X86)) return EDGE_PATH_X86;
            if (File.Exists(EDGE_PATH_X64)) return EDGE_PATH_X64;
            return null;
        }
    }
}
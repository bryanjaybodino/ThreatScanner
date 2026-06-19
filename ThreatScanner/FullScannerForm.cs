using System;
using System.Collections.Generic;
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
    /// <summary>
    ///   full scanner:
    ///   - Sensitive file / admin panel discovery (extended wordlist)
    ///   - Hyperlink + JS link crawling (configurable depth)
    ///   - Security headers, cookies, SQLi hints, XSS hints, open ports, DNS
    ///   - DataGridView output with Name / Status / Response columns (copyable)
    /// </summary>
    public partial class FullScannerForm : Form
    {
        private readonly HttpClient _http = ScanHelpers.BuildDefaultClient();

        // ── Crawl state ──────────────────────────────────────────────────────────
        private readonly HashSet<string> _visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private CancellationTokenSource _cts;

        // ── Soft-404 baseline (fingerprint of the server's custom 404 page) ─────
        // We fetch a guaranteed-nonexistent URL first, record its body length and
        // a 256-char snippet so we can detect exact soft-404 duplicates.
        private long _soft404Size = -1;
        private string _soft404Snippet = null;  // first 256 chars of the baseline body

        public FullScannerForm() => InitializeComponent();

        // ════════════════════════════════════════════════════════════════════════
        //  HELPERS
        // ════════════════════════════════════════════════════════════════════════

        private string NormalizeUrl(string raw) => ScanHelpers.NormalizeUrl(raw);

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

                // Colour the Status cell
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

                // Auto-scroll
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
        //  TAB: FULL SCAN
        // ════════════════════════════════════════════════════════════════════════

        private async void button_Scan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_Url.Text))
            {
                MessageBox.Show("Please enter a URL.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dataGridView_Output.Rows.Clear();
            _visited.Clear();
            _soft404Size = -1;
            _soft404Snippet = null;
            _cts = new CancellationTokenSource();

            string url = NormalizeUrl(textBox_Url.Text.Trim());
            button_Scan.Enabled = false;
            button_StopScan.Enabled = true;
            SetProgress(true);

            AddRow("TARGET", "🔍 Scanning", url);

            try
            {
                await Task.Run(async () =>
                {
                    var ct = _cts.Token;

                    // 1. Basic checks on root
                    await CheckHttps(url);
                    await CheckSecurityHeaders(url);
                    await CheckCookieFlags(url);
                    await CheckRedirects(url);
                    await CheckSqlInjectionHints(url);

                    // SQLi probe against any <form> fields on the page
                    await ProbeForms(url, ct);

                    await CheckXssHints(url);
                    await CheckOpenPorts(url);

                    // Establish soft-404 baseline BEFORE sensitive file scan
                    await BuildSoft404Baseline(url);

                    // 2. Sensitive file / admin panel discovery
                    await CheckSensitiveFiles(url, ct);

                    // 3. Crawl hyperlinks + JS-embedded links (depth ≤ 2)
                    await CrawlAndScan(url, depth: 0, maxDepth: 2, ct: ct);

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
                    AddRow("DONE", "✅ Complete", $"Total URLs scanned: {_visited.Count}");
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
        //  INDIVIDUAL CHECKS
        // ════════════════════════════════════════════════════════════════════════

        private Task CheckHttps(string url)
        {
            bool secure = url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
            AddRow("HTTPS Check", secure ? "✅ Secure" : "⚠️ Insecure",
                secure ? "Connection uses HTTPS" : "Site is using plain HTTP — data is NOT encrypted");
            return Task.CompletedTask;
        }

        private async Task CheckSecurityHeaders(string url)
        {
            AddSep("Security Headers");
            try
            {
                var resp = await _http.GetAsync(url);
                var headers = resp.Headers;

                var checks = new[]
                {
                    ("Content-Security-Policy",  "Prevents XSS / data injection"),
                    ("X-Frame-Options",           "Prevents clickjacking"),
                    ("X-Content-Type-Options",    "Prevents MIME sniffing"),
                    ("Strict-Transport-Security", "Forces HTTPS"),
                    ("Referrer-Policy",           "Controls referrer leakage"),
                    ("Permissions-Policy",        "Restricts browser features"),
                    ("Cross-Origin-Opener-Policy","Isolates browsing context"),
                    ("Cross-Origin-Resource-Policy","Restricts cross-origin reads"),
                };

                foreach (var (header, desc) in checks)
                {
                    bool found = headers.Contains(header);
                    string val = found ? string.Join(", ", headers.GetValues(header)) : "MISSING";
                    AddRow(header,
                        found ? "✅ Present" : "⚠️ Missing",
                        found ? val : $"{desc} — header not set");
                }

                // Server header leakage
                if (headers.Contains("Server"))
                    AddRow("Server Header", "⚠️ Exposed",
                        string.Join(", ", headers.GetValues("Server")));
                else
                    AddRow("Server Header", "✅ Hidden", "Not disclosed");

                // X-Powered-By leakage
                if (headers.Contains("X-Powered-By"))
                    AddRow("X-Powered-By", "⚠️ Exposed",
                        string.Join(", ", headers.GetValues("X-Powered-By")));
            }
            catch (Exception ex) { AddRow("Security Headers", "❌ Error", ex.Message); }
        }

        private async Task CheckCookieFlags(string url)
        {
            AddSep("Cookie Flags");
            try
            {
                var handler = new HttpClientHandler { UseCookies = true, CookieContainer = new CookieContainer() };
                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("User-Agent", "ThreatScanner/1.0");
                var resp = await client.GetAsync(url);

                if (!resp.Headers.Contains("Set-Cookie"))
                { AddRow("Cookies", "ℹ️ None", "No cookies set by server"); return; }

                foreach (var cookie in resp.Headers.GetValues("Set-Cookie"))
                {
                    bool httpOnly = cookie.IndexOf("HttpOnly", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool secure = cookie.IndexOf("Secure", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool sameSite = cookie.IndexOf("SameSite", StringComparison.OrdinalIgnoreCase) >= 0;
                    string name = cookie.Split(';')[0];
                    string flags = $"HttpOnly:{(httpOnly ? "✔" : "✖")}  Secure:{(secure ? "✔" : "✖")}  SameSite:{(sameSite ? "✔" : "✖")}";
                    string icon = (httpOnly && secure) ? "✅ OK" : "⚠️ Weak flags";
                    AddRow(name, icon, flags);
                }
            }
            catch (Exception ex) { AddRow("Cookies", "❌ Error", ex.Message); }
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
                    string loc = resp.Headers.Location?.ToString() ?? "Unknown";
                    AddRow("Redirect", $"⚠️ HTTP {code}", $"→ {loc}");
                }
                else
                    AddRow("Redirect", "✅ None", $"HTTP {code} — no redirect");
            }
            catch (Exception ex) { AddRow("Redirect", "❌ Error", ex.Message); }
        }

        private async Task CheckSqlInjectionHints(string url)
        {
            try
            {
                string testUrl = url.Contains("?") ? url + "&id=1'" : url + "?id=1'";
                string body = await (await _http.GetAsync(testUrl)).Content.ReadAsStringAsync();
                string[] sqlErrors = { "syntax error", "sql syntax", "mysql_fetch", "ORA-", "Microsoft OLE DB",
                                        "SQLite", "pg_query", "Warning: pg_", "Unclosed quotation", "SQLSTATE" };
                bool found = sqlErrors.Any(err => body.IndexOf(err, StringComparison.OrdinalIgnoreCase) >= 0);
                AddRow("SQLi Probe", found ? "🚨 Possible SQLi" : "✅ No disclosure",
                    found ? "SQL error string detected in response body" : "No SQL error patterns found");
            }
            catch (Exception ex) { AddRow("SQLi Probe", "❌ Error", ex.Message); }
        }

        private async Task CheckXssHints(string url)
        {
            try
            {
                const string payload = "<script>xss</script>";
                const string encoded = "&lt;script&gt;xss&lt;/script&gt;";
                string testUrl = (url.Contains("?") ? url + "&q=" : url + "?q=") + Uri.EscapeDataString(payload);
                string body = await (await _http.GetAsync(testUrl)).Content.ReadAsStringAsync();

                if (body.Contains(payload))
                    AddRow("XSS Probe", "🚨 Reflected (raw)", "Payload reflected UNENCODED — possible reflected XSS");
                else if (body.Contains(encoded))
                    AddRow("XSS Probe", "⚠️ Reflected (encoded)", "Payload HTML-encoded — likely safe but review CSP");
                else
                    AddRow("XSS Probe", "✅ Not reflected", "Basic probe not reflected");
            }
            catch (Exception ex) { AddRow("XSS Probe", "❌ Error", ex.Message); }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  SQLI PROBE: HTML FORMS ON THE PAGE
        //
        //  Same passive, single-request error-disclosure check as
        //  CheckSqlInjectionHints above, but applied to each <form> discovered
        //  on the page. Injects a single quote into one text-like field at a
        //  time (keeping every other field, including CSRF tokens, at its
        //  normal auto-filled value) and submits via the form's real method
        //  (GET/POST) and action URL. One request per field — no payload
        //  chaining, no blind/time-based confirmation, no data extraction.
        // ════════════════════════════════════════════════════════════════════════

        private static readonly string[] SqlErrorSignatures = {
            "sql syntax", "mysql_fetch", "mysqli_", "you have an error in your sql syntax",
            "microsoft ole db", "odbc sql server driver", "unclosed quotation mark",
            "sqlstate", "incorrect syntax near",
            "ora-00933", "ora-01756", "ora-",
            "pg_query", "warning: pg_", "postgresql query failed",
            "sqlite3.operationalerror", "sqlite_error", "sqlite",
            "syntax error", "unterminated string", "quoted string not properly terminated"
        };

        private async Task ProbeForms(string url, CancellationToken ct)
        {
            AddSep("Form SQLi Probe");
            AddRow("Form Probe", "🔍 Starting", $"Fetching {url} to discover forms…");

            string html;
            try
            {
                var resp = await _http.GetAsync(url, ct);
                html = await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                AddRow("Form Fetch", "❌ Error", ex.Message);
                return;
            }

            var forms = FormParsingHelper.ParseForms(html, url);
            if (forms.Count == 0)
            {
                AddRow("Forms", "ℹ️ None found", "No <form> elements detected on this page.");
                AddRow("Form Probe", "✅ Complete", "No forms to test.");
                return;
            }

            AddRow("Forms", "ℹ️ Found", $"{forms.Count} form(s) detected — testing text-like fields one at a time.");

            int fieldsTested = 0;
            foreach (var form in forms)
            {
                if (ct.IsCancellationRequested) break;

                AddRow($"Form: {(string.IsNullOrWhiteSpace(form.Name) ? $"#{form.Index + 1}" : form.Name)}",
                    "ℹ️ Info",
                    $"{form.Method.ToUpper()} {form.Action} ({form.Fields.Count} fields, framework: {form.Framework})");

                var textFields = form.Fields
                    .Where(f => !f.IsCsrfToken &&
                                (f.Type == "text" || f.Type == "email" || f.Type == "search" ||
                                 f.Type == "password" || f.Type == "textarea" || f.Type == "tel" ||
                                 f.Type == "url" || string.IsNullOrEmpty(f.Type)))
                    .ToList();

                if (textFields.Count == 0)
                {
                    AddRow(form.ToString(), "ℹ️ Skipped", "No text-like fields to test.");
                    continue;
                }

                foreach (var field in textFields)
                {
                    if (ct.IsCancellationRequested) break;
                    await ProbeFormField(form, field, ct);
                    fieldsTested++;
                }
            }

            AddRow("Form Probe", "✅ Complete", $"Tested {fieldsTested} field(s) across {forms.Count} form(s).");
        }

        private async Task ProbeFormField(
            FormParsingHelper.FormInfo form, FormParsingHelper.FormField targetField, CancellationToken ct)
        {
            // Inject the probe payload into the target field only; keep every other
            // field's auto-filled / existing value (incl. CSRF tokens) so the request
            // looks like a normal form submission.
            var originalValue = targetField.Value;
            targetField.Value = (originalValue ?? "") + "'";
            string label = $"Form field: {targetField.Name}";

            try
            {
                 var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                 var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, cts.Token);

                HttpResponseMessage resp;
                if (form.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                {
                    string qs = FormParsingHelper.BuildRequestBody(form, includeCsrfTokens: true);
                    string getUrl = form.Action.Contains("?") ? $"{form.Action}&{qs}" : $"{form.Action}?{qs}";
                    resp = await _http.GetAsync(getUrl, linked.Token);
                }
                else
                {
                    string body = FormParsingHelper.BuildRequestBody(form, includeCsrfTokens: true);
                    var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                    resp = await _http.PostAsync(form.Action, content, linked.Token);
                }

                string respBody = await resp.Content.ReadAsStringAsync();
                var matched = SqlErrorSignatures.FirstOrDefault(err =>
                    respBody.IndexOf(err, StringComparison.OrdinalIgnoreCase) >= 0);

                if (matched != null)
                {
                    AddRow(label, "🚨 Possible SQLi",
                        $"DB error signature \"{matched}\" found. Likely unsanitized input reaching a SQL query. " +
                        "Fix: use parameterized queries / prepared statements, never concatenate user input into SQL; " +
                        "disable detailed error pages in production.");
                }
                else
                {
                    AddRow(label, "✅ No disclosure",
                        "No known DB error patterns found. Passive check only — does not rule out blind/second-order injection.");
                }
            }
            catch (OperationCanceledException)
            {
                AddRow(label, "⚠️ Timeout", $"Request to {form.Action} timed out.");
            }
            catch (Exception ex)
            {
                AddRow(label, "❌ Error", ex.Message);
            }
            finally
            {
                targetField.Value = originalValue; // restore for next field's probe
            }
        }

        private async Task CheckOpenPorts(string url)
        {
            AddSep("Port Scan");
            try
            {
                string host = new Uri(url).Host;
                int[] ports = {
                    // Web servers / HTTP(S)
                    80,    // HTTP
                    443,   // HTTPS
                    3000,  // Common dev server (React, Node/Express, etc.)
                    3001,  // Alternate dev server
                    4200,  // Angular CLI default
                    5173,  // Vite default
                    8080,  // Alternate HTTP / Tomcat / common proxy
                    8443,  // Alternate HTTPS
                    8888,  // Jupyter Notebook / alternate HTTP
                    9000,  // PHP-FPM / common app port

                    // Databases
                    1433,  // Microsoft SQL Server
                    1521,  // Oracle DB
                    3306,  // MySQL / MariaDB
                    5432,  // PostgreSQL
                    6379,  // Redis
                    7000,  // Cassandra (inter-node)
                    7199,  // Cassandra (JMX)
                    9042,  // Cassandra (CQL)
                    27017, // MongoDB
                    27018, // MongoDB shard
                    27019, // MongoDB config server

                    // Search / messaging / streaming
                    2181,  // Zookeeper
                    5672,  // RabbitMQ
                    9092,  // Kafka
                    9200,  // Elasticsearch (HTTP)
                    9300,  // Elasticsearch (transport)
                    15672, // RabbitMQ management UI

                    // Remote access / file transfer
                    21,    // FTP
                    22,    // SSH / SFTP
                    23,    // Telnet
                    25,    // SMTP
                    110,   // POP3
                    143,   // IMAP

                    // Containers / orchestration
                    2375,  // Docker (unencrypted API)
                    2376,  // Docker (TLS API)
                    6443,  // Kubernetes API server
                    10250, // Kubelet API

                    // Misc dev tools
                    389,   // LDAP
                    5000,  // Flask default / Docker registry
                    5601,  // Kibana
                    8081,  // Alternate HTTP / Nexus / Kafka REST proxy
                    9090,  // Prometheus
                    3000   // Grafana (duplicate of above note, often reused)
                };
                foreach (int port in ports)
                {
                    try
                    {
                        var tcp = new System.Net.Sockets.TcpClient();
                        var conn = tcp.ConnectAsync(host, port);
                        bool open = await Task.WhenAny(conn, Task.Delay(1500)) == conn && tcp.Connected;
                        AddRow($"Port {port}", open ? "✅ Open" : "🔒 Closed",
                            open ? $"{host}:{port} is reachable" : "Closed / filtered");
                    }
                    catch { AddRow($"Port {port}", "🔒 Closed", "Closed / filtered"); }
                }
            }
            catch (Exception ex) { AddRow("Port Scan", "❌ Error", ex.Message); }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  SENSITIVE FILE DISCOVERY (extended wordlist)
        // ════════════════════════════════════════════════════════════════════════

        private static readonly string[] SensitivePaths =
        {
            // ════════════════════════════════════════════════════════════════════
            // ENVIRONMENT & SECRETS
            // ════════════════════════════════════════════════════════════════════
            "/.env", "/.env.local", "/.env.dev", "/.env.development", "/.env.staging", "/.env.production", "/.env.prod", "/.env.test", "/.env.testing", "/.env.backup", "/.env.bak", "/.env.old", "/.env.example", "/.env.sample", "/.env.copy", "/.env.save", "/.env.orig", "/.env.0", "/.env.1", "/.env~", "/env", "/env.js", "/env.json", "/env.yml", "/env.yaml", "/environment", "/environment.rb", "/environments/development.rb", "/environments/production.rb",

            // ════════════════════════════════════════════════════════════════════
            // CONFIG FILES (Generic)
            // ════════════════════════════════════════════════════════════════════
            "/config", "/config.php", "/config.php.bak", "/config.php~", "/config.inc.php", "/config.inc", "/config.js", "/config.json", "/config.yml", "/config.yaml", "/config.xml", "/config.ini", "/config.toml", "/config.cfg", "/config.conf", "/config.properties", "/config.py", "/config.rb", "/config.bak", "/config.old", "/config.orig", "/config.save", "/config.tmp", "/config.dist", "/config.example", "/config/config.php", "/config/database.php", "/config/app.php", "/config/auth.php", "/config/mail.php", "/config/secrets.yml", "/configuration.php", "/configuration.yml", "/configuration.xml", "/app.config", "/app.json", "/app.yml", "/app.yaml", "/app.ini", "/application.properties", "/application.yml", "/application.yaml", "/application.xml", "/application.ini", "/application.conf", "/application-dev.properties", "/application-prod.properties", "/application-staging.properties", "/local.properties", "/local.yml", "/local.json", "/settings.php", "/settings.py", "/settings.yml", "/settings.yaml", "/settings.json", "/settings.ini", "/settings.cfg", "/settings.xml", "/settings/base.py", "/settings/local.py", "/settings/production.py", "/settings/development.py", "/local_settings.py", "/local_settings.php", "/params.php", "/params.yml", "/parameters.php", "/parameters.yml", "/parameters.yaml",

            // ════════════════════════════════════════════════════════════════════
            // DATABASE CONFIG
            // ════════════════════════════════════════════════════════════════════
            "/database.php", "/database.yml", "/database.yaml", "/database.json", "/database.ini", "/database.xml", "/database.conf", "/database.config.php", "/db.php", "/db.yml", "/db.json", "/db.ini", "/db.conf", "/db_config.php", "/db_connect.php", "/db_connection.php", "/dbconfig.php", "/dbconfig.yml", "/dbconnect.php", "/connection.php", "/connect.php", "/mysqli.php", "/pdo.php", "/mysql.php", "/postgres.php", "/mongodb.php",

            // ════════════════════════════════════════════════════════════════════
            // CREDENTIALS & SECRETS
            // ════════════════════════════════════════════════════════════════════
            "/secrets.json", "/secrets.yml", "/secrets.yaml", "/secrets.xml", "/secrets.php", "/secret.json", "/secret.yml", "/secret.key", "/credentials", "/credentials.json", "/credentials.yml", "/credentials.xml", "/credentials.ini", "/credentials.php", "/credential.json", "/.credentials", "/auth.json", "/auth.yml", "/auth.php", "/authentication.php", "/key.json", "/key.pem", "/key.p12", "/private.key", "/private.pem", "/id_rsa", "/.ssh/id_rsa", "/.ssh/id_rsa.pub", "/.ssh/authorized_keys", "/.ssh/known_hosts", "/.ssh/config", "/keys.json", "/keys.yml", "/apikeys.json", "/api_keys.json", "/api_key.txt", "/service-account.json", "/serviceaccount.json", "/google-credentials.json", "/firebase-adminsdk.json", "/aws-credentials", "/.aws/credentials", "/.aws/config", "/azure-credentials.json",

            // ════════════════════════════════════════════════════════════════════
            // WORDPRESS
            // ════════════════════════════════════════════════════════════════════
            "/wp-config.php", "/wp-config.php.bak", "/wp-config.php.old", "/wp-config.php.orig", "/wp-config.php.save", "/wp-config.php~", "/wp-config.txt", "/wp-config.inc", "/wp-admin", "/wp-admin/", "/wp-admin/admin.php", "/wp-admin/login.php", "/wp-admin/admin-ajax.php", "/wp-login.php", "/wp-login.php?action=register", "/wp-json", "/wp-json/wp/v2/users", "/wp-json/wp/v2/posts", "/wp-content/debug.log", "/wp-content/uploads/", "/wp-includes/", "/wordpress/wp-config.php", "/wordpress/wp-login.php", "/xmlrpc.php",

            // ════════════════════════════════════════════════════════════════════
            // ADMIN PANELS (Generic)
            // ════════════════════════════════════════════════════════════════════
            "/admin", "/admin/", "/admin.php", "/admin.html", "/admin.asp", "/admin.aspx", "/admin.jsp", "/admin/index.php", "/admin/index.html", "/admin/login", "/admin/login.php", "/admin/login.html", "/admin/login.asp", "/admin/login.aspx", "/admin/dashboard", "/admin/dashboard.php", "/admin/panel", "/admin/panel.php", "/admin/cp", "/admin/cp.php", "/admin/config.php", "/admin/admin.php", "/admin/home", "/admin/manage", "/admin/users", "/admin/user", "/admin/settings", "/admin/setup", "/admin/install", "/admin/account", "/admin/console", "/admin/portal", "/admin/backend", "/admin/control", "/admin/controlpanel", "/admin_area", "/admin_area/", "/admin_area/admin.php", "/adminpanel", "/adminpanel/", "/administrator", "/administrator/", "/administrator/index.php", "/administrator/admin.php", "/administrator/login.php", "/administration", "/administration/", "/backend", "/backend/", "/backend/admin", "/backend/login", "/cp", "/cp/", "/controlpanel", "/controlpanel/", "/control", "/dashboard", "/dashboard/", "/manage", "/manage/", "/management", "/management/", "/manager", "/manager/", "/panel", "/panel/", "/portal", "/portal/", "/moderation", "/moderator", "/sysadmin", "/superadmin", "/superadmin/", "/root", "/webmaster", "/webadmin", "/masteradmin", "/adm", "/adm/",

            // ════════════════════════════════════════════════════════════════════
            // LOGIN / AUTHENTICATION PAGES
            // ════════════════════════════════════════════════════════════════════
            "/login", "/login.php", "/login.html", "/login.asp", "/login.aspx", "/login.jsp", "/login.do", "/login.cfm", "/log-in", "/log_in", "/logon", "/logon.php", "/signin", "/signin.php", "/sign-in", "/sign_in", "/auth", "/auth/login", "/auth/sign_in", "/auth/signin", "/authenticate", "/authentication", "/user/login", "/user/sign_in", "/users/login", "/users/sign_in", "/account/login", "/accounts/login", "/session", "/sessions/new", "/secure/login", "/members/login", "/member/login", "/staff/login", "/employee/login", "/operator/login",

            // ════════════════════════════════════════════════════════════════════
            // PHPMYADMIN / DB TOOLS
            // ════════════════════════════════════════════════════════════════════
            "/phpmyadmin", "/phpmyadmin/", "/phpmyadmin/index.php", "/phpMyAdmin", "/phpMyAdmin/", "/phpMyAdmin/index.php", "/pma", "/pma/", "/mysql", "/mysql/", "/mysqladmin", "/myadmin", "/myadmin/", "/dbadmin", "/dbadmin/", "/adminer", "/adminer.php", "/adminer/", "/pgadmin", "/pgadmin4", "/pgadmin4/", "/sqlitemanager", "/sqlite", "/sql", "/sql/", "/db", "/db/",

            // ════════════════════════════════════════════════════════════════════
            // JAVA / TOMCAT / JBOSS / JENKINS
            // ════════════════════════════════════════════════════════════════════
            "/manager/html", "/manager/text", "/manager/status", "/host-manager/html", "/host-manager", "/console", "/console/", "/jmx-console", "/jmx-console/", "/web-console", "/web-console/", "/invoker/JMXInvokerServlet", "/invoker/EJBInvokerServlet", "/WEB-INF/web.xml", "/WEB-INF/classes/", "/WEB-INF/lib/", "/META-INF/MANIFEST.MF", "/META-INF/context.xml", "/jenkins", "/jenkins/", "/jenkins/script", "/jenkins/login", "/jenkins/api/json", "/hudson", "/hudson/", "/hudson/script",

            // ════════════════════════════════════════════════════════════════════
            // ASP.NET
            // ════════════════════════════════════════════════════════════════════
            "/Elmah.axd", "/elmah.axd", "/trace.axd", "/Trace.axd", "/WebResource.axd", "/ScriptResource.axd", "/web.config", "/web.config.bak", "/web.config.old", "/web.config.orig", "/web.config~", "/Web.config", "/web.Debug.config", "/web.Release.config", "/global.asax", "/Global.asax", "/app.config", "/App.config", "/packages.config", "/applicationHost.config", "/machine.config", "/_mvc", "/_api",

            // ════════════════════════════════════════════════════════════════════
            // SPRING BOOT / ACTUATOR
            // ════════════════════════════════════════════════════════════════════
            "/actuator", "/actuator/", "/actuator/health", "/actuator/info", "/actuator/env", "/actuator/beans", "/actuator/mappings", "/actuator/metrics", "/actuator/threaddump", "/actuator/heapdump", "/actuator/logfile", "/actuator/configprops", "/actuator/auditevents", "/actuator/httptrace", "/actuator/sessions", "/actuator/shutdown", "/actuator/refresh", "/actuator/flyway", "/actuator/liquibase", "/actuator/caches", "/actuator/scheduledtasks", "/actuator/integrationgraph", "/health", "/health/", "/info", "/metrics", "/env", "/beans", "/trace", "/dump", "/heapdump", "/logfile", "/autoconfig", "/configprops", "/mappings",

            // ════════════════════════════════════════════════════════════════════
            // GIT / SVN / VERSION CONTROL LEAKS
            // ════════════════════════════════════════════════════════════════════
            "/.git", "/.git/", "/.git/HEAD", "/.git/config", "/.git/index", "/.git/COMMIT_EDITMSG", "/.git/packed-refs", "/.git/refs/heads/master", "/.git/refs/heads/main", "/.git/logs/HEAD", "/.git/objects/info/packs", "/.gitignore", "/.gitmodules", "/.gitattributes", "/.svn", "/.svn/", "/.svn/entries", "/.svn/wc.db", "/.svn/format", "/.hg", "/.hg/", "/.hg/hgrc", "/.bzr", "/.bzr/", "/.bzr/branch/branch.conf", "/CVS", "/CVS/Root", "/CVS/Entries", "/.cvs",

            // ════════════════════════════════════════════════════════════════════
            // SERVER / APACHE / NGINX CONFIG
            // ════════════════════════════════════════════════════════════════════
            "/.htaccess", "/.htpasswd", "/.htgroups", "/.htdigest", "/server-status", "/server-info", "/nginx.conf", "/nginx/nginx.conf", "/etc/nginx/nginx.conf", "/apache2.conf", "/httpd.conf", "/lighttpd.conf", "/php.ini", "/php-fpm.conf", "/php-fpm.d/www.conf", "/error_log", "/access_log", "/error.log", "/access.log",

            // ════════════════════════════════════════════════════════════════════
            // LOG FILES
            // ════════════════════════════════════════════════════════════════════
            "/logs", "/logs/", "/log", "/log/", "/log.txt", "/log.log", "/logs/error.log", "/logs/access.log", "/logs/debug.log", "/logs/app.log", "/logs/application.log", "/logs/system.log", "/logs/server.log", "/logs/sql.log", "/logs/db.log", "/logs/auth.log", "/storage/logs/laravel.log", "/var/log/apache2/error.log", "/var/log/nginx/error.log", "/debug.log", "/debug.txt", "/error.log", "/error.txt", "/app.log", "/application.log", "/laravel.log", "/django.log", "/rails.log", "/development.log", "/production.log", "/test.log",

            // ════════════════════════════════════════════════════════════════════
            // INFO DISCLOSURE
            // ════════════════════════════════════════════════════════════════════
            "/robots.txt", "/sitemap.xml", "/sitemap.xml.gz", "/sitemap_index.xml", "/crossdomain.xml", "/clientaccesspolicy.xml", "/humans.txt", "/security.txt", "/.well-known/security.txt", "/.well-known/", "/.well-known/openid-configuration", "/.well-known/oauth-authorization-server", "/.well-known/jwks.json", "/phpinfo.php", "/info.php", "/php_info.php", "/test.php", "/testing.php", "/debug.php", "/check.php", "/status.php", "/version.php", "/version.txt", "/version.json", "/VERSION", "/CHANGELOG", "/CHANGELOG.md", "/CHANGELOG.txt", "/CHANGES", "/CHANGES.md", "/CHANGES.txt", "/RELEASE", "/RELEASE.md", "/RELEASE.txt", "/RELEASE-NOTES", "/RELEASE-NOTES.txt", "/INSTALL", "/INSTALL.md", "/INSTALL.txt", "/README", "/README.md", "/README.txt", "/TODO", "/TODO.md", "/TODO.txt", "/NOTES", "/NOTES.txt", "/LICENSE", "/LICENSE.txt", "/LICENSE.md",

            // ════════════════════════════════════════════════════════════════════
            // API / SWAGGER / GRAPHQL
            // ════════════════════════════════════════════════════════════════════
            "/api", "/api/", "/api/v1", "/api/v1/", "/api/v2", "/api/v2/", "/api/v3", "/api/v3/", "/api/v4", "/api/docs", "/api/docs/", "/api/swagger", "/api/swagger/", "/api/swagger/index.html", "/api/swagger-ui.html", "/api/swagger.json", "/api/swagger.yaml", "/api/v1/swagger.json", "/api/v2/swagger.json", "/api/openapi.json", "/api/openapi.yaml", "/api/spec", "/api/spec.json", "/api/users", "/api/user", "/api/admin", "/api/login", "/api/logout", "/api/register", "/api/signup", "/api/auth", "/api/auth/login", "/api/auth/token", "/api/health", "/api/healthz", "/api/status", "/api/version", "/api/info", "/api/config", "/api/configuration", "/api/settings", "/api/debug", "/api/test", "/api/metrics", "/api/keys", "/api/apikey", "/api/api-key", "/api/token", "/api/tokens", "/api/secret", "/api/secrets", "/api/internal", "/api/private", "/api/console", "/api/actuator", "/api-docs", "/api-docs/", "/api-docs/swagger.json",
            "/swagger", "/swagger/", "/swagger.json", "/swagger.yaml", "/swagger.yml", "/swagger/index.html", "/swagger/ui", "/swagger/ui/index.html", "/swagger/docs", "/swagger/docs/v1", "/swagger/v1", "/swagger/v1/", "/swagger/v1/swagger.json", "/swagger/v2/swagger.json", "/swagger/v3/swagger.json", "/swagger/v1/swagger.yaml", "/swagger/users", "/swagger/account", "/swagger/api-docs", "/swagger-ui", "/swagger-ui/", "/swagger-ui.html", "/swagger-ui/index.html", "/swagger-ui/swagger-ui.html", "/swagger-ui-bundle.js", "/swagger-resources", "/swagger-resources/", "/swagger-resources/configuration/ui", "/swagger-resources/configuration/security",
            "/openapi.json", "/openapi.yaml", "/openapi.yml", "/openapi/v1.json", "/openapi/v1", "/openapi/v2", "/openapi/v3", "/.well-known/openapi.json", "/v1/api-docs", "/v2/api-docs", "/v2/api-docs/", "/v3/api-docs", "/v3/api-docs/", "/v3/api-docs.yaml", "/redoc", "/redoc.html", "/docs", "/docs/", "/documentation", "/help", "/explorer",
            "/app/swagger/", "/admin/swagger/", "/internal/swagger/", "/backend/swagger/", "/service/swagger/", "/gateway/swagger/", "/portal/swagger/", "/dev/swagger/", "/development/swagger/", "/staging/swagger/", "/stage/swagger/", "/test/swagger/", "/qa/swagger/", "/uat/swagger/", "/prod/swagger/", "/v1/swagger/", "/v2/swagger/", "/v3/swagger/", "/auth/swagger/", "/identity/swagger/", "/account/swagger/", "/accounts/swagger/", "/users/swagger/", "/user/swagger/", "/orders/swagger/", "/order/swagger/", "/payments/swagger/", "/payment/swagger/", "/billing/swagger/", "/notifications/swagger/", "/notification/swagger/", "/public/swagger/", "/private/swagger/", "/secure/swagger/", "/management/swagger/", "/manage/swagger/", "/core/swagger/", "/api-gateway/swagger/", "/webapi/swagger/", "/rest/swagger/", "/services/swagger/", "/microservice/swagger/", "/inventory/swagger/", "/product/swagger/", "/products/swagger/", "/catalog/swagger/", "/cart/swagger/", "/checkout/swagger/", "/search/swagger/", "/reporting/swagger/", "/reports/swagger/", "/admin-api/swagger/", "/external/swagger/", "/integration/swagger/", "/files/swagger/", "/upload/swagger/", "/media/swagger/", "/customer/swagger/", "/customers/swagger/", "/employee/swagger/", "/employees/swagger/", "/hr/swagger/", "/finance/swagger/", "/sso/swagger/", "/oauth/swagger/", "/booking/swagger/", "/bookings/swagger/", "/shipping/swagger/", "/delivery/swagger/", "/logistics/swagger/", "/warehouse/swagger/", "/crm/swagger/", "/erp/swagger/", "/cms/swagger/", "/data/swagger/", "/analytics/swagger/", "/dashboard/swagger/", "/monitoring/swagger/", "/scheduler/swagger/", "/jobs/swagger/", "/queue/swagger/", "/messaging/swagger/", "/email/swagger/", "/sms/swagger/", "/chat/swagger/", "/support/swagger/", "/ticket/swagger/", "/tickets/swagger/", "/contract/swagger/", "/contracts/swagger/", "/document/swagger/", "/documents/swagger/", "/legal/swagger/", "/compliance/swagger/", "/audit/swagger/", "/risk/swagger/", "/fraud/swagger/", "/security/swagger/", "/license/swagger/", "/subscription/swagger/", "/subscriptions/swagger/", "/loyalty/swagger/", "/rewards/swagger/", "/promo/swagger/", "/promotion/swagger/", "/coupon/swagger/", "/pricing/swagger/", "/tax/swagger/", "/currency/swagger/", "/exchange/swagger/", "/wallet/swagger/", "/transaction/swagger/", "/transactions/swagger/", "/ledger/swagger/", "/accounting/swagger/", "/hrms/swagger/", "/payroll/swagger/","/hris/swagger/","/pms/swagger/","/sse/swagger/","/warehouse/swagger/","/warehouses/swagger/", "/timesheet/swagger/", "/leave/swagger/", "/attendance/swagger/", "/recruitment/swagger/", "/onboarding/swagger/", "/training/swagger/", "/lms/swagger/", "/asset/swagger/", "/assets/swagger/", "/fleet/swagger/", "/maintenance/swagger/", "/iot/swagger/", "/device/swagger/", "/devices/swagger/", "/sensor/swagger/", "/telemetry/swagger/", "/location/swagger/", "/geo/swagger/", "/map/swagger/", "/weather/swagger/",
            "/graphql", "/graphql/", "/graphiql", "/graphql/console", "/graphql/playground", "/playground", "/altair", "/voyager", "/graphql-playground",
            "/actuator", "/actuator/health", "/actuator/info", "/actuator/env", "/actuator/mappings", "/actuator/beans", "/management", "/management/", "/health", "/healthcheck", "/ping", "/.env", "/.git/config", "/server-status",

            // ════════════════════════════════════════════════════════════════════
            // BACKUP & ARCHIVE FILES
            // ════════════════════════════════════════════════════════════════════
            "/backup", "/backup/", "/backups", "/backups/", "/backup.sql", "/backup.php", "/backup.zip", "/backup.tar", "/backup.tar.gz", "/backup.tgz", "/backup.7z", "/backup.rar", "/backup.bak", "/backup.old", "/backup.txt", "/dump.sql", "/dump.zip", "/db_backup.sql", "/database_backup.sql", "/db.sql", "/db.zip", "/db.tar.gz", "/data.sql", "/data.zip", "/data.tar.gz", "/full_backup.zip", "/full_backup.tar.gz", "/site.zip", "/site.tar.gz", "/www.zip", "/www.tar.gz", "/html.zip", "/html.tar.gz", "/web.zip", "/web.tar.gz", "/index.php.bak", "/index.bak", "/index.old", "/index.html.bak", "/index.html.old", "/index~", "/index.php~", "/.backup", "/.bak",

            // ════════════════════════════════════════════════════════════════════
            // PHP COMMON FILES
            // ════════════════════════════════════════════════════════════════════
            "/install.php", "/install", "/install/", "/setup.php", "/setup", "/setup/", "/update.php", "/upgrade.php", "/migrate.php", "/migration.php", "/shell.php", "/cmd.php", "/exec.php", "/eval.php", "/upload.php", "/uploader.php", "/file_upload.php", "/fileupload.php", "/files.php", "/file.php", "/download.php", "/downloader.php", "/register.php", "/signup.php", "/forgot.php", "/reset.php", "/password.php", "/change_password.php", "/checkout.php", "/payment.php", "/cron.php", "/crons.php", "/cronjob.php", "/cron_job.php", "/mail.php", "/mailer.php", "/send.php", "/sendmail.php", "/email.php", "/contact.php", "/form.php", "/process.php", "/ajax.php", "/ajax_handler.php", "/api.php", "/endpoint.php", "/proxy.php", "/redirect.php", "/include.php", "/includes.php", "/common.php", "/functions.php", "/utils.php", "/helper.php", "/helpers.php", "/class.php", "/lib.php", "/libs.php",

            // ════════════════════════════════════════════════════════════════════
            // DEPENDENCY / BUILD / PACKAGE FILES
            // ════════════════════════════════════════════════════════════════════
            "/composer.json", "/composer.lock", "/composer.phar", "/package.json", "/package-lock.json", "/yarn.lock", "/npm-shrinkwrap.json", "/bower.json", "/Gemfile", "/Gemfile.lock", "/requirements.txt", "/requirements.pip", "/Pipfile", "/Pipfile.lock", "/poetry.lock", "/pyproject.toml", "/setup.py", "/setup.cfg", "/pom.xml", "/build.gradle", "/build.xml", "/Makefile", "/makefile", "/Rakefile", "/Gruntfile.js", "/Gruntfile.coffee", "/Gulpfile.js", "/gulpfile.js", "/webpack.config.js", "/webpack.config.ts", "/vite.config.js", "/vite.config.ts", "/rollup.config.js", "/tsconfig.json", "/jsconfig.json", "/.babelrc", "/.eslintrc", "/.eslintrc.json", "/.eslintrc.js", "/.prettierrc", "/.stylelintrc", "/next.config.js", "/nuxt.config.js",

            // ════════════════════════════════════════════════════════════════════
            // DOCKER / CI / DEVOPS
            // ════════════════════════════════════════════════════════════════════
            "/Dockerfile", "/dockerfile", "/docker-compose.yml", "/docker-compose.yaml", "/docker-compose.dev.yml", "/docker-compose.prod.yml", "/docker-compose.override.yml", "/.dockerignore", "/Vagrantfile", "/.travis.yml", "/.circleci/config.yml", "/.github/workflows/", "/.gitlab-ci.yml", "/bitbucket-pipelines.yml", "/Jenkinsfile", "/azure-pipelines.yml", "/.helm", "/helm/", "/kubernetes/", "/k8s/", "/deploy/", "/deployment/", "/deployments/", "/infrastructure/", "/infra/", "/terraform/", "/ansible/", "/playbook.yml", "/playbooks/", "/inventory", "/inventory.yml", "/hosts", "/Procfile", "/app.yaml", "/app.yml", "/serverless.yml", "/serverless.yaml", "/sam.yaml", "/sam.yml", "/cloudformation.yml", "/cloudformation.yaml", "/.elasticbeanstalk/config.yml",

            // ════════════════════════════════════════════════════════════════════
            // OPERATING SYSTEM ARTIFACTS
            // ════════════════════════════════════════════════════════════════════
            "/.DS_Store", "/.ds_store", "/Thumbs.db", "/thumbs.db", "/desktop.ini", "/Desktop.ini", "/.bashrc", "/.bash_profile", "/.bash_history", "/.profile", "/.zshrc", "/.zsh_history", "/.viminfo", "/.npmrc", "/.yarnrc", "/.pypirc", "/.netrc", "/.pgpass", "/.my.cnf", "/.myclirc", "/.rediscli_history", "/.lesshst", "/.mysql_history",

            // ════════════════════════════════════════════════════════════════════
            // CMS — DRUPAL
            // ════════════════════════════════════════════════════════════════════
            "/sites/default/settings.php", "/sites/default/default.settings.php", "/sites/default/settings.local.php", "/sites/default/files/", "/user", "/user/login", "/user/register", "/user/password", "/admin/config", "/admin/structure", "/admin/modules", "/admin/reports", "/admin/reports/dblog", "/update.php", "/CHANGELOG.txt", "/core/INSTALL.txt", "/install.php",

            // ════════════════════════════════════════════════════════════════════
            // CMS — JOOMLA
            // ════════════════════════════════════════════════════════════════════
            "/administrator", "/administrator/", "/administrator/index.php", "/administrator/manifests/files/joomla.xml", "/configuration.php", "/configuration.php.bak", "/htaccess.txt", "/web.config.txt", "/joomla.xml", "/README.txt", "/libraries/", "/components/", "/modules/", "/plugins/", "/templates/", "/cache/", "/tmp/", "/logs/error.php",

            // ════════════════════════════════════════════════════════════════════
            // CMS — MAGENTO
            // ════════════════════════════════════════════════════════════════════
            "/app/etc/local.xml", "/app/etc/env.php", "/app/etc/config.php", "/downloader", "/downloader/", "/var/log/system.log", "/var/log/exception.log", "/var/export/", "/var/backups/", "/media/downloadable/",

            // ════════════════════════════════════════════════════════════════════
            // CMS — LARAVEL
            // ════════════════════════════════════════════════════════════════════
            "/storage/logs/laravel.log", "/storage/logs/", "/storage/framework/sessions/", "/storage/app/", "/bootstrap/cache/config.php", "/bootstrap/cache/routes.php", "/.env", "/artisan",

            // ════════════════════════════════════════════════════════════════════
            // CMS — DJANGO / FLASK / PYTHON
            // ════════════════════════════════════════════════════════════════════
            "/manage.py", "/settings.py", "/urls.py", "/wsgi.py", "/asgi.py", "/requirements.txt", "/app.py", "/run.py", "/main.py", "/server.py", "/__debug__/", "/admin/login/?next=/admin/",

            // ════════════════════════════════════════════════════════════════════
            // CLOUD & METADATA (SSRF bait — only flags if accessible from server)
            // These are useful to confirm SSRF if the scanner is run inside the
            // same network as the target, but harmless otherwise.
            // ════════════════════════════════════════════════════════════════════
            "/.aws/credentials","/.gcloud/credentials.db",

            // ════════════════════════════════════════════════════════════════════
            // MISC SENSITIVE PATHS
            // ════════════════════════════════════════════════════════════════════
            "/upload", "/upload/", "/uploads", "/uploads/", "/files", "/files/", "/file", "/media", "/media/", "/static", "/static/", "/assets", "/assets/", "/private", "/private/", "/secret", "/secret/", "/hidden", "/hidden/", "/internal", "/internal/", "/restricted", "/restricted/", "/secure", "/secure/", "/old", "/old/", "/temp", "/temp/", "/tmp", "/tmp/", "/test", "/test/", "/tests", "/tests/", "/dev", "/dev/", "/development", "/development/", "/staging", "/staging/", "/beta", "/beta/", "/alpha", "/alpha/", "/demo", "/demo/", "/preview", "/preview/", "/sandbox", "/sandbox/", "/debug", "/debug/", "/dump", "/dump/", "/export", "/export/", "/reports", "/reports/", "/report", "/report/", "/stats", "/stats/", "/statistics", "/statistics/", "/metrics", "/monitor", "/monitoring", "/monitoring/", "/tools", "/tools/", "/util", "/util/", "/utils", "/utils/", "/scripts", "/scripts/", "/shell", "/shell/", "/cmd", "/exec", "/run", "/cgi-bin", "/cgi-bin/", "/cgi-bin/printenv", "/cgi-bin/test-cgi", "/cgi-bin/admin.cgi", "/cgi-bin/php.cgi", "/cgi-bin/php5.cgi", "/.well-known/change-password",
        };

        // ── Soft-404 baseline builder ────────────────────────────────────────────
        /// <summary>
        /// Fetches a guaranteed-nonexistent random path to capture the server's
        /// custom 404 page fingerprint (body size + first-512-char snippet).
        /// Any subsequent 200 that matches this fingerprint is a soft-404 false alarm.
        /// </summary>
        private async Task BuildSoft404Baseline(string baseUrl)
        {
            try
            {
                string authority = new Uri(baseUrl.TrimEnd('/')).GetLeftPart(UriPartial.Authority);
                // Random path that will never exist on any server
                string canary = authority + "/ts_canary_" + Guid.NewGuid().ToString("N");
                var resp = await _http.GetAsync(canary);
                string body = await resp.Content.ReadAsStringAsync();
                _soft404Size = body.Length;
                _soft404Snippet = body.Length >= 256 ? body.Substring(0, 256) : body;
                AddRow("Soft-404 Baseline",
                    $"ℹ️ HTTP {(int)resp.StatusCode}",
                    $"Custom 404 fingerprint recorded — {body.Length} chars. False alarms will be suppressed.");
            }
            catch
            {
                // If baseline fails, we still run the scan — just without soft-404 filtering
                AddRow("Soft-404 Baseline", "⚠️ Skipped", "Could not fetch baseline — soft-404 filter disabled");
            }
        }

        /// <summary>
        /// Returns true ONLY when a 200 response is essentially byte-for-byte
        /// identical to the server's custom 404 page.
        /// Uses two strict checks (never fuzzy size alone):
        ///   1. Exact body-length match AND first-256-char snippet match.
        ///   2. Full body string equality (for small pages).
        /// 403 / 401 / 500 are NEVER suppressed — they are always shown.
        /// </summary>
        private bool IsSoft404(string body, long bodyLength)
        {
            if (_soft404Snippet == null) return false;   // no baseline — don't suppress anything

            // Must be the same size (exact, no tolerance)
            if (bodyLength != _soft404Size) return false;

            // Additionally the first 256 chars must match exactly
            // (catches servers that inject a tiny dynamic nonce into the page
            //  without changing the length — extremely rare in practice)
            string snippet256 = body.Length >= 256 ? body.Substring(0, 256) : body;
            string baseline256 = _soft404Snippet.Length >= 256
                ? _soft404Snippet.Substring(0, 256)
                : _soft404Snippet;

            return string.Equals(snippet256, baseline256, StringComparison.Ordinal);
        }

        private async Task CheckSensitiveFiles(string baseUrl, CancellationToken ct)
        {
            AddSep("Sensitive File / Admin Panel Discovery");

            List<string> scanBases = await GetScanBasesAsync(baseUrl, ct);

            foreach (var b in scanBases)
                AddRow("Scan Base", "ℹ️ Target", b);

            var triedTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string authority in scanBases)
            {
                if (ct.IsCancellationRequested) break;

                foreach (string path in SensitivePaths)
                {
                    if (ct.IsCancellationRequested) break;

                    string target = authority + path;
                    if (!triedTargets.Add(target)) continue;

                    try
                    {
                        var resp = await _http.GetAsync(target, ct);
                        int code = (int)resp.StatusCode;
                        string body = await resp.Content.ReadAsStringAsync();
                        long bodyLen = body.Length;
                        string detail = $"HTTP {code}  [{bodyLen} bytes]";

                        if (code == 200 && IsSoft404(body, bodyLen))
                            continue;

                        string status;
                        if (code == 200) status = "🚨 Found (200)";
                        else if (code == 403) status = "⚠️ Forbidden (403)";
                        else if (code == 401) status = "⚠️ Auth Required (401)";
                        else if (code == 301 || code == 302) status = $"↪️ Redirect ({code})";
                        else if (code == 500) status = "⚠️ Server Error (500)";
                        else if (code == 404) status = "✅ Not found (404)";
                        else status = $"ℹ️ HTTP {code}";

                        AddRow(target, status, detail);
                    }
                    catch (OperationCanceledException) { break; }
                    catch (Exception ex)
                    {
                        AddRow(target, "✅ Not reachable", ex.Message);
                    }
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  CRAWLER: HTML hyperlinks + JS-embedded URLs
        // ════════════════════════════════════════════════════════════════════════

        private async Task CrawlAndScan(string url, int depth, int maxDepth, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return;
            if (_visited.Contains(url)) return;
            if (depth > maxDepth) return;

            _visited.Add(url);

            string html;
            try
            {
                var resp = await _http.GetAsync(url, ct);
                int code = (int)resp.StatusCode;
                string ctype = resp.Content.Headers.ContentType?.MediaType ?? "";

                // Only crawl HTML pages
                if (!ctype.Contains("html") && depth > 0) return;

                html = await resp.Content.ReadAsStringAsync();

                if (depth == 0)
                {
                    AddSep("Crawled Pages");
                }

                AddRow(url, $"ℹ️ HTTP {code}", $"Crawl depth {depth} — {html.Length} chars");
            }
            catch (OperationCanceledException) { return; }
            catch (Exception ex)
            {
                AddRow(url, "❌ Error", ex.Message);
                return;
            }

            // ── Extract links from HTML (<a href>, <link href>, <script src>, <img src>, <form action>)
            var links = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            ExtractHtmlLinks(html, url, links);
            ExtractJsLinks(html, url, links);

            string baseAuthority = new Uri(url).GetLeftPart(UriPartial.Authority);

            foreach (string link in links)
            {
                if (ct.IsCancellationRequested) break;
                if (_visited.Contains(link)) continue;

                // Only follow same-origin links for deeper scanning
                bool sameOrigin = link.StartsWith(baseAuthority, StringComparison.OrdinalIgnoreCase);

                if (sameOrigin && depth < maxDepth)
                    await CrawlAndScan(link, depth + 1, maxDepth, ct);
                else if (!_visited.Contains(link))
                {
                    // Just record external / JS-extracted links without crawling further
                    _visited.Add(link);
                    try
                    {
                        var resp = await _http.GetAsync(link, ct);
                        int code = (int)resp.StatusCode;
                        AddRow(link,
                            code == 200 ? "✅ Reachable" : $"⚠️ HTTP {code}",
                            $"External / JS link — HTTP {code}");
                    }
                    catch (OperationCanceledException) { break; }
                    catch (Exception ex) { AddRow(link, "❌ Error", ex.Message); }
                }
            }
        }

        // ── Extract <a href>, <link href>, <script src>, <form action> ──────────
        private static readonly Regex _htmlLinkRx = new Regex(
            @"(?:href|src|action)\s*=\s*[""']([^""'#>]+)[""']",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static void ExtractHtmlLinks(string html, string baseUrl, HashSet<string> links)
        {
            foreach (Match m in _htmlLinkRx.Matches(html))
            {
                string href = m.Groups[1].Value.Trim();
                if (TryResolve(href, baseUrl, out string abs))
                    links.Add(abs);
            }
        }

        // ── Extract URLs from inline / external JS (strings that look like paths) ─
        private static readonly Regex _jsUrlRx = new Regex(
            @"[""'`](/[a-zA-Z0-9_/\-\.]+)[""'`]",
            RegexOptions.Compiled);

        private static void ExtractJsLinks(string html, string baseUrl, HashSet<string> links)
        {
            // Also look inside <script> blocks
            var scriptBlocks = Regex.Matches(html, @"<script[^>]*>(.*?)</script>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match sb in scriptBlocks)
            {
                string jsCode = sb.Groups[1].Value;
                foreach (Match m in _jsUrlRx.Matches(jsCode))
                {
                    string path = m.Groups[1].Value;
                    if (TryResolve(path, baseUrl, out string abs))
                        links.Add(abs);
                }
            }

            // Also scan <script src="..."> external JS files referenced in html
            var scriptSrc = Regex.Matches(html, @"<script[^>]+src\s*=\s*[""']([^""']+)[""']",
                RegexOptions.IgnoreCase);
            foreach (Match m in scriptSrc)
            {
                if (TryResolve(m.Groups[1].Value.Trim(), baseUrl, out string abs))
                    links.Add(abs);
            }
        }

        private static bool TryResolve(string href, string baseUrl, out string abs)
        {
            abs = null;
            if (string.IsNullOrWhiteSpace(href)) return false;
            if (href.StartsWith("mailto:") || href.StartsWith("javascript:") ||
                href.StartsWith("tel:") || href.StartsWith("#")) return false;
            try
            {
                abs = new Uri(new Uri(baseUrl), href).ToString();
                // Remove fragment
                int frag = abs.IndexOf('#');
                if (frag >= 0) abs = abs.Substring(0, frag);
                return abs.StartsWith("http://") || abs.StartsWith("https://");
            }
            catch { return false; }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  TAB: HEADER ANALYZER
        // ════════════════════════════════════════════════════════════════════════

        private async void button_AnalyzeHeaders_Click(object sender, EventArgs e)
        {
            dataGridView_Output.Rows.Clear();
            string url = NormalizeUrl(textBox_Url.Text);
            button_AnalyzeHeaders.Enabled = false;
            SetProgress(true);
            AddRow("TARGET", "📋 Header Dump", url);
            try
            {
                var resp = await _http.GetAsync(url);
                AddRow("Status", $"ℹ️ HTTP {(int)resp.StatusCode}", resp.StatusCode.ToString());
                foreach (var h in resp.Headers)
                    foreach (var v in h.Value) AddRow(h.Key, "→ Response Header", v);
                foreach (var h in resp.Content.Headers)
                    foreach (var v in h.Value) AddRow(h.Key, "→ Content Header", v);
            }
            catch (Exception ex) { AddRow("Error", "❌ Failed", ex.Message); }
            finally
            {
                AddRow("DONE", "✅ Complete", "Header analysis finished");
                button_AnalyzeHeaders.Enabled = true;
                SetProgress(false);
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  TAB: DNS LOOKUP
        // ════════════════════════════════════════════════════════════════════════

        private async void button_DnsLookup_Click(object sender, EventArgs e)
        {
            dataGridView_Output.Rows.Clear();
            string url = NormalizeUrl(textBox_Url.Text);
            button_DnsLookup.Enabled = false;
            SetProgress(true);
            try
            {
                string host = new Uri(url).Host;
                AddRow("DNS Target", "🌐 Resolving", host);
                var addresses = await Dns.GetHostAddressesAsync(host);
                foreach (var addr in addresses) AddRow("IP Address", "📍 Resolved", addr.ToString());
                var entry = await Dns.GetHostEntryAsync(host);
                AddRow("Hostname", "🏷 PTR", entry.HostName);
                foreach (var alias in entry.Aliases) AddRow("Alias", "↪️ CNAME", alias);
            }
            catch (Exception ex) { AddRow("DNS", "❌ Error", ex.Message); }
            finally
            {
                AddRow("DONE", "✅ Complete", "DNS lookup finished");
                button_DnsLookup.Enabled = true;
                SetProgress(false);
            }
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
                FileName = $"ThreatScanner_Report_{DateTime.Now:yyyyMMdd_HHmmss}"
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
            _visited.Clear();
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

            // Select the row under the cursor so the user gets visual feedback
            var hit = dataGridView_Output.HitTest(e.X, e.Y);
            if (hit.RowIndex >= 0)
            {
                dataGridView_Output.ClearSelection();
                dataGridView_Output.Rows[hit.RowIndex].Selected = true;
                dataGridView_Output.CurrentCell = dataGridView_Output.Rows[hit.RowIndex].Cells[0];
            }

            // Build the URL for "Open in Browser" from the clicked row
            string rowUrl = null;
            if (hit.RowIndex >= 0)
            {
                string nameCell = dataGridView_Output.Rows[hit.RowIndex]
                                      .Cells["colName"].Value?.ToString() ?? "";
                // The Name column may be a full URL (crawl rows) or a path (/admin)
                if (nameCell.StartsWith("http://") || nameCell.StartsWith("https://"))
                    rowUrl = nameCell;
                else if (nameCell.StartsWith("/"))
                {
                    // Reconstruct absolute URL using the current target
                    string rawTarget = NormalizeUrl(textBox_Url.Text.Trim());
                    try
                    {
                        string authority = new Uri(rawTarget).GetLeftPart(UriPartial.Authority);
                        rowUrl = authority + nameCell;
                    }
                    catch { rowUrl = null; }
                }
            }

            var menu = new ContextMenuStrip();

            // ── Open in Browser ───────────────────────────────────────────────
            var openItem = new System.Windows.Forms.ToolStripMenuItem(
                "🌐  Open in Browser", null,
                (s, ev) =>
                {
                    if (!string.IsNullOrEmpty(rowUrl))
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = rowUrl,
                            UseShellExecute = true   // opens the default browser
                        });
                    else
                        MessageBox.Show("No valid URL found for this row.", "ThreatScanner",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
            openItem.Enabled = !string.IsNullOrEmpty(rowUrl);
            openItem.Font = new System.Drawing.Font(menu.Font, System.Drawing.FontStyle.Bold);
            menu.Items.Add(openItem);

            // ── Copy URL ──────────────────────────────────────────────────────
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

            // ── Copy row(s) ───────────────────────────────────────────────────
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



        /// <summary>
        /// Given a target URL, returns every "base" URL that sensitive-file paths
        /// should be appended to: the root authority AND every parent folder
        /// walking up from the full path.
        ///
        /// e.g. http://localhost:81/ERP/Seller-Login.aspx
        ///   -> http://localhost:81
        ///   -> http://localhost:81/ERP
        ///
        /// e.g. http://localhost:81/ERP/Sub/Page.aspx
        ///   -> http://localhost:81
        ///   -> http://localhost:81/ERP
        ///   -> http://localhost:81/ERP/Sub
        /// </summary>
        private async Task<List<string>> GetScanBasesAsync(string url, CancellationToken ct)
        {
            var bases = new List<string>();
            var uri = new Uri(url);

            string authority = uri.GetLeftPart(UriPartial.Authority);
            bases.Add(authority);

            string path = uri.AbsolutePath;
            var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();
            for (int i = 0; i < segments.Length; i++)
            {
                string seg = segments[i];

                // Explicit extension anywhere in the segment => definitely a file.
                // Stop walking deeper — nothing after a file can be a real folder.
                if (seg.Contains("."))
                    break;

                sb.Append('/').Append(seg);
                string candidateUrl = authority + sb.ToString();

                // Ambiguous extensionless segment — ask the server which it is.
                bool isFile = await IsActuallyAFileAsync(candidateUrl, ct);
                if (isFile)
                {
                    AddRow("Path Check", "ℹ️ Detected as FILE",
                        $"{candidateUrl} — extensionless file route, not scanning as folder");
                    break; // stop walking — nothing under a file is a real subfolder
                }

                bases.Add(candidateUrl);
            }

            return bases;
        }
        /// <summary>
        /// Probes whether a given path segment is actually a FILE (e.g. an
        /// extensionless ASP.NET route) as opposed to a real FOLDER.
        ///
        /// IMPORTANT: a soft-404 mismatch alone is NOT reliable evidence of
        /// "file" — different folders on the same server can legitimately
        /// produce different 404 styles (IIS static handler vs custom error
        /// page) depending on routing config. We only call something a FILE
        /// when we get a strong, unambiguous signal: a 500-class error, since
        /// that's what ASP.NET typically throws when you try to route "under"
        /// a compiled page/handler that isn't a directory.
        /// Default assumption is FOLDER.
        /// </summary>
        private async Task<bool> IsActuallyAFileAsync(string segmentUrl, CancellationToken ct)
        {
            try
            {
                string canaryChild = segmentUrl.TrimEnd('/') + "/__ts_canary_" + Guid.NewGuid().ToString("N");
                var resp = await _http.GetAsync(canaryChild, ct);
                int code = (int)resp.StatusCode;

                // Only a server-error response under the segment is strong
                // enough evidence that it's a file masquerading as a folder.
                // 404s of any flavor are NOT evidence — too many legitimate
                // reasons a folder's 404 can differ from the root's.
                return code == 500;
            }
            catch
            {
                return false; // probe failed — default to folder, don't lose coverage
            }
        }
    }
}
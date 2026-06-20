using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreatScanner.Helpers;
using static ThreatScanner.Helpers.HtmlHelpers;

namespace ThreatScanner
{
    /// <summary>
    /// Brute-force login form: auto-detects ASP.NET / PHP / HTML frameworks
    /// and replays credentials from a wordlist.
    /// </summary>
    public partial class BruteForceForm : Form
    {
        // ── UI batching ──────────────────────────────────────────────────────
        // Worker threads enqueue log entries; the UI timer drains them all in
        // one batch via ScanHelpers' scroll-preserving append, mirroring the
        // pattern AutoFillForm uses for LogRtbColor.
        private readonly ConcurrentQueue<PendingRow> _pendingRows = new ConcurrentQueue<PendingRow>();
        private readonly System.Windows.Forms.Timer _flushTimer = new System.Windows.Forms.Timer { Interval = 75 };
        private const int MaxRows = 2000;

        private readonly struct PendingRow
        {
            public readonly string Icon, Message;
            public PendingRow(string icon, string message) { Icon = icon; Message = message; }
        }

        public BruteForceForm()
        {
            InitializeComponent();

            _flushTimer.Tick += (s, e) => FlushPendingRows();
            _flushTimer.Start();
            this.FormClosed += (s, e) => _flushTimer.Stop();
        }

        // ─── QUEUE-BASED LOGGING ─────────────────────────────────────────────────

        /// <summary>
        /// Queues one log line. Safe to call from any thread — never marshals
        /// to the UI thread directly; the flush timer handles that in bulk.
        /// </summary>
        private void Log(string icon, string msg)
            => _pendingRows.Enqueue(new PendingRow(icon, msg));

        /// <summary>
        /// Queues an HTML-stripped log line. Safe to call from any thread.
        /// Stripping is delegated to ScanHelpers.StripHtmlTags so the logic
        /// stays in one place.
        /// </summary>
        private void HtmlLog(string icon, string msg)
            => _pendingRows.Enqueue(new PendingRow(icon, ScanHelpers.StripHtmlTags(msg)));

        private void LogSep() => Log("", new string('─', 60));

        /// <summary>
        /// Drains the pending-row queue and appends all entries to the
        /// RichTextBox in one batch via ScanHelpers' scroll-preserving append —
        /// the same mechanism AutoFillForm uses, so the user can scroll up and
        /// stay there even while rows keep arriving. Runs on the UI thread only
        /// (called from the Timer.Tick event).
        /// </summary>
        private void FlushPendingRows()
        {
            if (_pendingRows.IsEmpty) return;

            var sb = new StringBuilder();
            while (_pendingRows.TryDequeue(out var pr))
                sb.AppendLine(ScanHelpers.FormatQueueRow(pr.Icon, pr.Message));

            ScanHelpers.AppendBatchPreservingScroll(richTextBox_Output, sb.ToString());

            // Cap box size so a long fuzz run can't bloat memory/repaint cost.
            ScanHelpers.TrimToLastLinesPreservingScroll(richTextBox_Output, MaxRows);
        }

        private void ClearOut()
        {
            // Drain the queue first so stale rows don't reappear after a clear.
            while (_pendingRows.TryDequeue(out _)) { }
            richTextBox_Output.Clear();
        }

        private void SetProgress(bool running)
        {
            progressBar_Scan.Style = running ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            if (!running) progressBar_Scan.Value = 0;
        }

        // ─── BROWSE WORDLIST ─────────────────────────────────────────────────────

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

        // ─── BRUTE FORCE ─────────────────────────────────────────────────────────

        private async void button_BruteForce_Click(object sender, EventArgs e)
        {
            ClearOut();
            string url = ScanHelpers.NormalizeUrl(textBox_Url.Text);
            string username = textBox_Username.Text.Trim();
            string password = textBox_Password.Text;

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Enter URL and username.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            button_BruteForce.Enabled = false;
            SetProgress(true);

            Log("🔐", $"Brute Force (Auto-Detect Framework) → {url}");
            Log("🔎", "Auto-detecting login field names from the page...");
            LogSep();

            var passwords = new List<string>();
            if (!string.IsNullOrEmpty(textBox_WordlistPath.Text) && File.Exists(textBox_WordlistPath.Text))
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
                    var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(15) };
                    client.DefaultRequestHeaders.Add("User-Agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ThreatScanner/1.0");

                    var probeResp = await client.GetAsync(url);
                    string probeHtml = await probeResp.Content.ReadAsStringAsync();
                    LoginFramework fw = DetectFramework(probeHtml);

                    var (usernameField, passwordField) = AutoDetectLoginFields(probeHtml);

                    // All Log() calls below are thread-safe — no Invoke required.
                    Log("🔎", $"Framework detected: {fw}");
                    Log("👤", $"Auto-detected → User: [{usernameField}]  Pass: [{passwordField}]");

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
                                    f.Key == "__EVENTTARGET" || f.Key == "__EVENTARGUMENT" ||
                                    f.Key == "__SCROLLPOSITIONX" || f.Key == "__SCROLLPOSITIONY");
                                fields.Add(new KeyValuePair<string, string>("__EVENTTARGET", et));
                                fields.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", ""));
                                fields.Add(new KeyValuePair<string, string>("__SCROLLPOSITIONX", "0"));
                                fields.Add(new KeyValuePair<string, string>("__SCROLLPOSITIONY", "0"));
                                int vsLen = hidden.ContainsKey("__VIEWSTATE") ? hidden["__VIEWSTATE"].Length : 0;
                                Log("🔄", $"  ASP.NET → EVENTTARGET: {et}  VIEWSTATE len: {vsLen}");
                            }
                            else if (fw == LoginFramework.PhpOrHtml)
                            {
                                var hidden = ExtractAllHiddenFields(html);
                                foreach (var kv in hidden)
                                    fields.Add(new KeyValuePair<string, string>(kv.Key, kv.Value));
                                Log("🔄", $"  PHP/HTML → {hidden.Count} hidden field(s) harvested");

                                int btnPos = html.IndexOf("type=\"submit\"", StringComparison.OrdinalIgnoreCase);
                                if (btnPos >= 0)
                                {
                                    int tagStart = html.LastIndexOf("<button", btnPos, StringComparison.OrdinalIgnoreCase);
                                    if (tagStart < 0) tagStart = html.LastIndexOf("<input", btnPos, StringComparison.OrdinalIgnoreCase);
                                    if (tagStart >= 0)
                                    {
                                        int tagEnd = html.IndexOf(">", btnPos);
                                        string tag = html.Substring(tagStart, tagEnd - tagStart);
                                        string submitName = ParseAttr(tag, "name");
                                        if (!string.IsNullOrEmpty(submitName))
                                            fields.Add(new KeyValuePair<string, string>(submitName, "SUBMIT"));
                                    }
                                }
                            }
                            else
                            {
                                Log("🔄", "  Generic → username + password only");
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
                                if (string.IsNullOrEmpty(action) && html.Contains("login_process.php"))
                                    action = "login_process.php";
                                postUrl = BuildPostUrl(url, action);
                            }

                            Log("🌐", $"POST URL: {postUrl}");
                            Log("📦", "FIELDS:");
                            foreach (var f in fields) Log("   ", $"{f.Key} = {f.Value}");

                            var postResp = await client.PostAsync(postUrl, new FormUrlEncodedContent(fields));
                            string body = await postResp.Content.ReadAsStringAsync();
                            int code = (int)postResp.StatusCode;
                            Uri loginUri = new Uri(url);

                            bool failure = IsLoginFailure(body, fw);
                            bool success = !failure && IsLoginSuccess(body, code, loginUri, postResp, fw);

                            if (success)
                            {
                                Log("🚨", $"  [{pwd}] HTTP {code}  SUCCESS — credentials accepted!");
                                HtmlLog("   ", $"  response: {body}");
                                break;
                            }
                            else
                            {
                                Log("🔒", $"  [{pwd}] HTTP {code}  Failed — wrong credentials");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log("❌", "Request error: " + ex.Message);
                            break;
                        }
                    }
                });
            }
            finally
            {
                // Button/progress updates must still happen on the UI thread.
                // Use Invoke here (only once, at the very end) rather than
                // wrapping every log call.
                Invoke((Action)(() =>
                {
                    LogSep();
                    Log("✅", "Brute force test complete.");
                    button_BruteForce.Enabled = true;
                    SetProgress(false);
                }));
            }
        }

        // ─── SAVE / CLEAR ─────────────────────────────────────────────────────────

        private void button_SaveReport_Click(object sender, EventArgs e)
        {
            if (richTextBox_Output.TextLength == 0)
            {
                MessageBox.Show("No results to save.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var dlg = new SaveFileDialog
            {
                Filter = "Text File|*.txt",
                FileName = $"BruteForce_Report_{DateTime.Now:yyyyMMdd_HHmmss}"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
                File.WriteAllText(dlg.FileName, richTextBox_Output.Text, Encoding.UTF8);
        }

        private void button_ClearOutput_Click(object sender, EventArgs e) => ClearOut();
    }
}
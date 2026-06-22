// FileUploadSecurityForm.cs
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreatScanner.Helpers;

namespace ThreatScanner
{
    public partial class FileUploadSecurityForm : Form
    {
        // Harmless marker embedded in every test payload — lets you confirm a
        // file was actually stored/served without it containing anything
        // that does something if executed.
        private const string Marker = "THREATSCANNER-UPLOAD-TEST-MARKER";

        private HttpClient _httpClient;

        private bool _running = false;

        // Browser-driven detection/verification. Separate from the raw HTTP
        // fuzzing above — this drives the *actual* page in the user's browser
        // via CDP, so it sees whatever the real <form> looks like (relative
        // action URLs, hidden CSRF fields, JS-rendered inputs, etc.).
        private readonly CdpHelper _cdp = new CdpHelper();

        // Detected from the last Auto-Detect run — used so Verify can target
        // the exact <input type="file"> the user confirmed, not just "any".
        private string _detectedFieldName;
        private string _detectedAction;
        private string _detectedMethod;

        public FileUploadSecurityForm()
        {
            InitializeComponent();
            ScanHelpers.EnableRowDeletion(dataGridView_Results);
            dataGridView_Results.CellDoubleClick += (s, e) => ShowSnippetForRow(e.RowIndex);
            this.FormClosing += (s, e) => { try { _cdp.Dispose(); } catch { } try { _httpClient?.Dispose(); } catch { } };
        }

        // ── RUN ────────────────────────────────────────────────────────────────

        private async void button_RunTests_Click(object sender, EventArgs e)
        {
            if (_running) return;

            string url = textBox_PageUrl.Text.Trim();
            string field = string.IsNullOrWhiteSpace(textBox_FieldName.Text) ? "file" : textBox_FieldName.Text.Trim();

            if (string.IsNullOrEmpty(url) || !Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                Log("[!] Enter a valid upload endpoint URL first.", Color.OrangeRed);
                return;
            }

            var cases = BuildTestCases();
            if (cases.Count == 0)
            {
                Log("[!] Select at least one test case below.", Color.OrangeRed);
                return;
            }

            SetRunning(true);
            dataGridView_Results.Rows.Clear();
            Log($"[*] Running {cases.Count} test case(s) against {url} …", Color.DeepSkyBlue);

            _httpClient?.Dispose();
            _httpClient = BuildHttpClient(checkBox_IgnoreSslErrors.Checked);

            foreach (var test in cases)
            {
                if (!_running) break; // Stop pressed
                await RunOneTest(url, field, test);
            }

            Log("[*] Done.", Color.LimeGreen);
            SetRunning(false);
        }

        /// <summary>
        /// A self-signed or otherwise untrusted certificate on a local/test
        /// target is a very common reason every single request comes back
        /// as "ERR" with no useful status — the TLS handshake fails before
        /// any HTTP traffic happens at all. The checkbox opt-in keeps that
        /// bypass explicit and off by default.
        /// </summary>
        private static HttpClient BuildHttpClient(bool ignoreSslErrors)
        {
            var handler = new HttpClientHandler();
            if (ignoreSslErrors)
                handler.ServerCertificateCustomValidationCallback =
                    (request, cert, chain, errors) => true;

            return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            _running = false;
        }

        private void SetRunning(bool running)
        {
            _running = running;
            button_RunTests.Enabled = !running;
            button_Stop.Enabled = running;
            progressBar.Style = running ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
        }

        // ── Auto-detect upload form from the live browser tab (via CDP) ─────────

        private async void button_AutoDetect_Click(object sender, EventArgs e)
        {
            // ── CHANGE 1 ──────────────────────────────────────────────────────
            // Read the page URL from textBox_PageUrl (Step 1) instead of
            // assuming the user has already navigated the browser there.
            string pageUrl = textBox_PageUrl.Text.Trim();
            if (string.IsNullOrEmpty(pageUrl) || !Uri.TryCreate(pageUrl, UriKind.Absolute, out _))
            {
                Log("[!] Enter a valid page URL in Step 1 first.", Color.OrangeRed);
                return;
            }
            // ─────────────────────────────────────────────────────────────────

            button_AutoDetect.Enabled = false;
            label_DetectStatus.Text = "Connecting to browser…";
            Log("[*] Auto-detect: connecting to browser via CDP…", Color.DeepSkyBlue);

            try
            {
                IPage page = await _cdp.GetOrCreateActivePageAsync();

                // ── CHANGE 1 (continued) ──────────────────────────────────────
                // Navigate the CDP-controlled page to the URL the user entered
                // so we always scan the right page, not whatever tab was open.
                Log("[*] Navigating browser to " + pageUrl, Color.DeepSkyBlue);
                await page.GotoAsync(pageUrl, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                    Timeout = 15000
                });
                // ─────────────────────────────────────────────────────────────

                var info = await DetectFileUploadAsync(page);

                if (info == null)
                {
                    label_DetectStatus.Text = "No <input type=\"file\"> found on the current page.";
                    Log("[!] No file input found on the active page. Navigate to the upload form first.", Color.OrangeRed);
                    return;
                }

                _detectedFieldName = info.Value.fieldName;
                _detectedAction = info.Value.action;
                _detectedMethod = info.Value.method;

                textBox_FieldName.Text = _detectedFieldName;
                textBox_PageUrl.Text = _detectedAction;   // POST endpoint only — pageUrl is kept in textBox_PageUrl

                string multipartNote = info.Value.hasFile ? "" : "  ⚠ form is not multipart/form-data";
                label_DetectStatus.Text = $"Found: name=\"{_detectedFieldName}\", method={_detectedMethod.ToUpperInvariant()}, action={_detectedAction}{multipartNote}";
                Log($"[✓] Detected file input \"{_detectedFieldName}\" → {_detectedMethod.ToUpperInvariant()} {_detectedAction}", Color.SeaGreen);

                if (!info.Value.hasFile)
                    Log("[!] The enclosing <form> doesn't declare enctype=\"multipart/form-data\" — the browser may not actually send the file. Verify before trusting raw-HTTP fuzz results.", Color.DarkOrange);
            }
            catch (Exception ex)
            {
                label_DetectStatus.Text = "Detection failed — see log.";
                Log("[!] Auto-detect failed: " + ex.Message, Color.OrangeRed);
            }
            finally
            {
                button_AutoDetect.Enabled = true;
            }
        }

        /// <summary>
        /// Looks for the first &lt;input type="file"&gt; on the page and walks up
        /// to its enclosing &lt;form&gt; (if any) to read the field name, the
        /// resolved absolute action URL, the HTTP method, and whether the form
        /// is actually set up to carry file content (multipart/form-data).
        /// </summary>
        private static async Task<(string fieldName, string action, string method, bool hasFile)?> DetectFileUploadAsync(IPage page)
        {
            const string js = @"() => {
                const input = document.querySelector('input[type=file]');
                if (!input) return null;
                const form = input.closest('form');
                return {
                    fieldName: input.name || input.id || 'file',
                    action: form ? new URL(form.getAttribute('action') || '', location.href).href : location.href,
                    method: (form && form.method ? form.method : 'post').toLowerCase(),
                    hasFile: !form || (form.enctype || '').toLowerCase().includes('multipart')
                };
            }";

            var result = await page.EvaluateAsync<System.Text.Json.JsonElement?>(js);
            if (result == null || result.Value.ValueKind == System.Text.Json.JsonValueKind.Null)
                return null;

            var obj = result.Value;
            return (
                fieldName: obj.GetProperty("fieldName").GetString(),
                action: obj.GetProperty("action").GetString(),
                method: obj.GetProperty("method").GetString(),
                hasFile: obj.GetProperty("hasFile").GetBoolean()
            );
        }

        // ── Verify the upload is actually submittable, end-to-end, in the real browser ──

        private async void button_VerifySubmit_Click(object sender, EventArgs e)
        {
            button_VerifySubmit.Enabled = false;
            Log("[*] Verify: submitting a harmless control file through the real page…", Color.DeepSkyBlue);

            string tempFile = null;
            try
            {
                IPage page = await _cdp.GetOrCreateActivePageAsync();

                // ── CHANGE 2 ──────────────────────────────────────────────────
                // Navigate to the page URL (textBox_PageUrl) before re-detecting,
                // for the same reason as Auto-Detect: the user should not have to
                // manually navigate the browser before clicking Verify.
                string pageUrl = textBox_PageUrl.Text.Trim();
                if (!string.IsNullOrEmpty(pageUrl) && Uri.TryCreate(pageUrl, UriKind.Absolute, out _))
                {
                    Log("[*] Navigating browser to " + pageUrl, Color.DeepSkyBlue);
                    await page.GotoAsync(pageUrl, new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.DOMContentLoaded,
                        Timeout = 15000
                    });
                }
                // ─────────────────────────────────────────────────────────────

                // Re-detect every time — the user may have navigated since the
                // last Auto-Detect, and submitting against a stale selector
                // would silently no-op.
                var info = await DetectFileUploadAsync(page);
                if (info == null)
                {
                    Log("[!] No file input found on the active page. Run Auto-Detect first.", Color.OrangeRed);
                    return;
                }

                byte[] jpegMagic = { 0xFF, 0xD8, 0xFF, 0xE0 };
                string marker = $"{Marker}-{Guid.NewGuid():N}";
                byte[] controlBytes = jpegMagic.Concat(Encoding.UTF8.GetBytes(marker)).ToArray();

                tempFile = Path.Combine(Path.GetTempPath(), "ts_upload_control_" + Guid.NewGuid().ToString("N") + ".jpg");
                File.WriteAllBytes(tempFile, controlBytes);

                await page.SetInputFilesAsync("input[type=file]", tempFile);
                Log("[*] File attached to the input. Submitting form…", Color.DeepSkyBlue);

                var responseTask = page.WaitForResponseAsync(
                    r => r.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase)
                         || r.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase),
                    new PageWaitForResponseOptions { Timeout = 15000 });

                const string submitJs = @"() => {
                    const input = document.querySelector('input[type=file]');
                    const form = input ? input.closest('form') : null;
                    if (form) {
                        if (form.requestSubmit) { form.requestSubmit(); return 'form.requestSubmit()'; }
                        const btn = form.querySelector('button[type=submit], input[type=submit], button:not([type])');
                        if (btn) { btn.click(); return 'submit button click'; }
                        form.submit(); return 'form.submit()';
                    }
                    return 'no form found — could not submit';
                }";
                string submitMethod = await page.EvaluateAsync<string>(submitJs);
                Log("[*] Submit triggered via: " + submitMethod, Color.DeepSkyBlue);

                IResponse response;
                try
                {
                    response = await responseTask;
                }
                catch (TimeoutException)
                {
                    Log("[!] No POST/PUT request was observed within 15s. The form may require manual interaction (e.g. a CAPTCHA), or didn't actually submit.", Color.OrangeRed);
                    return;
                }

                string body;
                try { body = await response.TextAsync(); } catch { body = ""; }

                bool ok = response.Ok;
                Color color = ok ? Color.SeaGreen : Color.Firebrick;
                Log($"[{(int)response.Status}] Verify submit → {response.Url} ({(ok ? "accepted" : "rejected/error")}, {body.Length} bytes)", color);

                if (!string.IsNullOrEmpty(body))
                {
                    string snippet = body.Length > 500 ? body.Substring(0, 500) + "…" : body;
                    int rowIndex = dataGridView_Results.Rows.Add(
                        "Browser verify (control.jpg)", "control.jpg", "image/jpeg",
                        ((int)response.Status).ToString(), FormatSize(Encoding.UTF8.GetByteCount(body)), "—",
                        ok ? "✔ Submitted via real browser" : "⚠ Browser submit returned error");
                    var row = dataGridView_Results.Rows[rowIndex];
                    row.Tag = snippet;
                    row.DefaultCellStyle.ForeColor = color;
                }
            }
            catch (Exception ex)
            {
                Log("[!] Verify failed: " + ex.Message, Color.OrangeRed);
            }
            finally
            {
                button_VerifySubmit.Enabled = true;
                if (tempFile != null)
                {
                    try { File.Delete(tempFile); } catch { /* best effort cleanup */ }
                }
            }
        }

        // ── Test case generation ────────────────────────────────────────────────

        private class TestCase
        {
            public string Name;
            public string FileName;
            public string ContentType;
            public byte[] Content;
        }

        private List<TestCase> BuildTestCases()
        {
            var list = new List<TestCase>();
            string marker = $"{Marker}-{Guid.NewGuid():N}";
            byte[] textPayload = Encoding.UTF8.GetBytes($"<?php /* {marker} */ ?>");
            byte[] jpegMagic = { 0xFF, 0xD8, 0xFF, 0xE0 };

            if (checkBox_DoubleExtension.Checked)
                list.Add(new TestCase { Name = "Double extension", FileName = "shell.php.jpg", ContentType = "image/jpeg", Content = textPayload });

            if (checkBox_NullByte.Checked)
                list.Add(new TestCase { Name = "Null byte injection", FileName = "shell.php\0.jpg", ContentType = "image/jpeg", Content = textPayload });

            if (checkBox_CaseVariation.Checked)
                list.Add(new TestCase { Name = "Case variation", FileName = "shell.PhP", ContentType = "application/octet-stream", Content = textPayload });

            if (checkBox_MimeSpoof.Checked)
                list.Add(new TestCase { Name = "MIME type spoof", FileName = "shell.php", ContentType = "image/png", Content = textPayload });

            if (checkBox_MagicByteMismatch.Checked)
                list.Add(new TestCase { Name = "Magic-byte/polyglot mismatch", FileName = "shell.php", ContentType = "image/jpeg", Content = jpegMagic.Concat(textPayload).ToArray() });

            if (checkBox_PathTraversal.Checked)
                list.Add(new TestCase { Name = "Path traversal filename", FileName = "../../../tmp/shell.php", ContentType = "application/octet-stream", Content = textPayload });

            if (checkBox_NoExtension.Checked)
                list.Add(new TestCase { Name = "No extension", FileName = "shell", ContentType = "application/octet-stream", Content = textPayload });

            if (checkBox_TrailingDot.Checked)
                list.Add(new TestCase { Name = "Trailing dot/space (Windows alias)", FileName = "shell.php.", ContentType = "image/jpeg", Content = textPayload });

            if (checkBox_AlternateExtension.Checked)
                foreach (var ext in new[] { "phtml", "php5", "pht", "asp", "aspx", "jsp" })
                    list.Add(new TestCase { Name = $"Alternate executable ext (.{ext})", FileName = $"shell.{ext}", ContentType = "application/octet-stream", Content = textPayload });

            if (checkBox_Oversized.Checked)
            {
                long sizeBytes = (long)numericUpDown_OversizeMb.Value * 1024 * 1024;
                list.Add(new TestCase { Name = $"Oversized file ({numericUpDown_OversizeMb.Value} MB)", FileName = "big.jpg", ContentType = "image/jpeg", Content = BuildFiller(sizeBytes, jpegMagic) });
            }

            if (checkBox_ControlBaseline.Checked)
                list.Add(new TestCase { Name = "Control (legit .jpg, should be accepted)", FileName = "control.jpg", ContentType = "image/jpeg", Content = jpegMagic.Concat(Encoding.UTF8.GetBytes(marker)).ToArray() });

            return list;
        }

        private static byte[] BuildFiller(long size, byte[] header)
        {
            if (size < header.Length) size = header.Length;
            var buf = new byte[size];
            Array.Copy(header, buf, header.Length);
            return buf;
        }

        // ── Execute one test ────────────────────────────────────────────────────

        private async Task RunOneTest(string url, string field, TestCase test)
        {
            string status = "ERR";
            long respSize = 0;
            string verdict = "?";
            Color verdictColor = Color.Gray;
            string snippet = "";
            double ms = 0;

            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                using (var content = new MultipartFormDataContent())
                using (var fileContent = new ByteArrayContent(test.Content))
                {
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.TryParse(test.ContentType, out var mt)
                        ? mt : new MediaTypeHeaderValue("application/octet-stream");

                    // Content-Disposition is set via TryAddWithoutValidation rather
                    // than Add — several of these test cases (null byte, path
                    // traversal, trailing dot) deliberately use filenames that
                    // .NET's strict header validation would otherwise reject
                    // with a FormatException before a single byte hits the wire.
                    content.Add(fileContent);
                    fileContent.Headers.TryAddWithoutValidation("Content-Disposition",
                        $"form-data; name=\"{field}\"; filename=\"{test.FileName.Replace("\"", "")}\"");

                    using (var response = await _httpClient.PostAsync(url, content))
                    {
                        sw.Stop();
                        ms = sw.Elapsed.TotalMilliseconds;
                        status = ((int)response.StatusCode).ToString();
                        string body = await response.Content.ReadAsStringAsync();
                        respSize = Encoding.UTF8.GetByteCount(body);
                        snippet = body.Length > 500 ? body.Substring(0, 500) + "…" : body;

                        bool looksRejected = response.StatusCode == System.Net.HttpStatusCode.BadRequest
                            || response.StatusCode == System.Net.HttpStatusCode.UnsupportedMediaType
                            || response.StatusCode == System.Net.HttpStatusCode.Forbidden
                            || ContainsRejectionKeyword(body);

                        bool isControl = test.Name.StartsWith("Control");

                        if (isControl)
                        {
                            verdict = response.IsSuccessStatusCode ? "✔ Baseline OK" : "⚠ Baseline rejected?!";
                            verdictColor = response.IsSuccessStatusCode ? Color.SeaGreen : Color.DarkOrange;
                        }
                        else if (response.IsSuccessStatusCode && !looksRejected)
                        {
                            verdict = "⚠ Possibly accepted";
                            verdictColor = Color.Firebrick;
                        }
                        else
                        {
                            verdict = "✔ Rejected";
                            verdictColor = Color.SeaGreen;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                ms = sw.Elapsed.TotalMilliseconds;
                status = "ERR";
                // ex.Message alone is often a generic "An error occurred while
                // sending the request." for HttpRequestException — the actual
                // cause (TLS failure, DNS, connection refused, etc.) lives in
                // the inner exception(s), so walk the whole chain for the snippet.
                snippet = DescribeExceptionChain(ex);
                verdict = "✖ Request failed";
                verdictColor = Color.Gray;
            }

            int rowIndex = dataGridView_Results.Rows.Add(
                test.Name, test.FileName, test.ContentType, status,
                FormatSize(respSize), $"{ms:0} ms", verdict);
            var row = dataGridView_Results.Rows[rowIndex];
            row.Tag = snippet;
            row.DefaultCellStyle.ForeColor = verdictColor;

            Log($"[{status}] {test.Name} → {verdict}", verdictColor);
        }

        private static string DescribeExceptionChain(Exception ex)
        {
            var parts = new List<string>();
            for (var cur = ex; cur != null; cur = cur.InnerException)
                parts.Add($"{cur.GetType().Name}: {cur.Message}");
            return string.Join("  →  ", parts);
        }

        private static bool ContainsRejectionKeyword(string body)
        {
            if (string.IsNullOrEmpty(body)) return false;
            string b = body.ToLowerInvariant();
            string[] keywords = { "invalid", "not allowed", "rejected", "unsupported", "forbidden", "denied", "error" };
            return keywords.Any(k => b.Contains(k));
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024.0).ToString("0.#") + " KB";
            return (bytes / 1024.0 / 1024.0).ToString("0.#") + " MB";
        }

        private void ShowSnippetForRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dataGridView_Results.Rows.Count) return;
            string snippet = dataGridView_Results.Rows[rowIndex].Tag as string;
            if (string.IsNullOrEmpty(snippet)) snippet = "(empty response body)";
            MessageBox.Show(this, snippet, "Response snippet", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_SelectAll_Click(object sender, EventArgs e) => SetAllChecks(true);
        private void button_SelectNone_Click(object sender, EventArgs e) => SetAllChecks(false);

        private void button_ClearLog_Click(object sender, EventArgs e)
        {
            richTextBox_Output.Clear();
            dataGridView_Results.Rows.Clear();
        }

        private void button_SaveResults_Click(object sender, EventArgs e)
        {
            if (dataGridView_Results.Rows.Count == 0)
            {
                Log("[!] Nothing to save — run some tests first.", Color.OrangeRed);
                return;
            }

            using (var sfd = new SaveFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "file_upload_security_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv"
            })
            {
                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                var sb = new StringBuilder();
                sb.AppendLine("Test,FileName,ContentType,Status,RespSize,Time,Verdict,Snippet");
                foreach (DataGridViewRow row in dataGridView_Results.Rows)
                {
                    if (row.IsNewRow) continue;
                    sb.AppendLine(string.Join(",", new[]
                    {
                        CsvEscape(row.Cells["col_TestName"].Value),
                        CsvEscape(row.Cells["col_FileName"].Value),
                        CsvEscape(row.Cells["col_ContentType"].Value),
                        CsvEscape(row.Cells["col_HttpStatus"].Value),
                        CsvEscape(row.Cells["col_RespSize"].Value),
                        CsvEscape(row.Cells["col_Time"].Value),
                        CsvEscape(row.Cells["col_Verdict"].Value),
                        CsvEscape(row.Tag as string)
                    }));
                }

                File.WriteAllText(sfd.FileName, sb.ToString());
                Log("[*] Saved results to " + sfd.FileName, Color.LimeGreen);
            }
        }

        private static string CsvEscape(object value)
        {
            string s = value?.ToString() ?? "";
            if (s.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0)
                s = "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        private void SetAllChecks(bool value)
        {
            foreach (Control c in panel_Tests.Controls)
                if (c is CheckBox cb) cb.Checked = value;
        }

        private void Log(string message, Color? color = null) =>
         ScanHelpers.LogRtb(richTextBox_Output, color == Color.OrangeRed || color == Color.Firebrick || color == Color.Gray ? "⛔" : color == Color.SeaGreen || color == Color.LimeGreen ? "✅" : color == Color.DarkOrange ? "⚠️" : "ℹ️", message);
    }
}
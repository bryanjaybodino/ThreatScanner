using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreatScanner.Helpers;

namespace ThreatScanner
{
    /// <summary>
    /// Postman-style HTTP API tester with params, headers, body types, and auth.
    /// </summary>
    public partial class ApiTesterForm : Form
    {
        // The actual RichTextBox inside the JsonEditorHelper panel (JSON + Raw body input)
        private RichTextBox _jsonEditor;

        public ApiTesterForm()
        {
            InitializeComponent();

            // ── Inject the modern JSON editor into panel_JsonEditor ────────────
            var editorPanel = JsonEditorHelper.Create(out _jsonEditor);
            editorPanel.Dock = DockStyle.Fill;
            panel_JsonEditor.Controls.Add(editorPanel);

            // ── Wire body-type radio buttons ──────────────────────────────────
            radioButton_BodyNone.CheckedChanged += (s, e) => UpdateBodyVisibility();
            radioButton_BodyForm.CheckedChanged += (s, e) => UpdateBodyVisibility();
            radioButton_BodyJson.CheckedChanged += (s, e) => UpdateBodyVisibility();
            radioButton_BodyRaw.CheckedChanged += (s, e) => UpdateBodyVisibility();
            UpdateBodyVisibility();

            // ── Enable Delete-key row deletion on all three grids ─────────────
            ScanHelpers.EnableRowDeletion(dataGridView_Params);
            ScanHelpers.EnableRowDeletion(dataGridView_Headers);
            ScanHelpers.EnableRowDeletion(dataGridView_FormData);

            // ── Default method selection ───────────────────────────────────────
            if (comboBox_Method.SelectedIndex < 0)
                comboBox_Method.SelectedIndex = 0;

            if (comboBox_AuthType.SelectedIndex < 0)
                comboBox_AuthType.SelectedIndex = 0;
        }

        // ─── HELPERS ─────────────────────────────────────────────────────────────

        private void Log(string icon, string msg) => ScanHelpers.LogRtb(richTextBox_Output, icon, msg);
        private void LogSep() => ScanHelpers.LogSeparatorRtb(richTextBox_Output);
        private void ClearOut() => ScanHelpers.ClearOutputRtb(richTextBox_Output);

        private void SetProgress(bool running)
        {
            progressBar_Scan.Style = running ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            if (!running) progressBar_Scan.Value = 0;
        }

        private void UpdateBodyVisibility()
        {
            bool isForm = radioButton_BodyForm.Checked;
            bool isJsonOrRaw = radioButton_BodyJson.Checked || radioButton_BodyRaw.Checked;

            dataGridView_FormData.Visible = isForm;
            panel_JsonEditor.Visible = isJsonOrRaw;
        }

        // ─── SEND ─────────────────────────────────────────────────────────────────

        private async void button_ApiForce_Click(object sender, EventArgs e)
        {
            ClearOut();

            string rawUrl = textBox_ApiEndpoint.Text;
            if (string.IsNullOrWhiteSpace(rawUrl))
            {
                MessageBox.Show("Enter an endpoint URL.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string url = ScanHelpers.NormalizeUrl(rawUrl);
            string method = comboBox_Method.SelectedItem?.ToString() ?? "GET";

            button_ApiForce.Enabled = false;
            SetProgress(true);

            Log("🛰", $"{method} → {url}");
            LogSep();

            // ── Snapshot UI state before Task.Run ─────────────────────────────
            var queryParams = ScanHelpers.GetEnabledGridRows(dataGridView_Params, "col_ParamKey", "col_ParamValue");
            var extraHeaders = ScanHelpers.GetEnabledGridRows(dataGridView_Headers, "col_HdrKey", "col_HdrValue");
            var formDataRows = ScanHelpers.GetEnabledGridRows(dataGridView_FormData, "col_FormKey", "col_FormValue");

            string bodyText = _jsonEditor.Text;          // read from the injected editor
            bool bodyIsJson = radioButton_BodyJson.Checked;
            bool bodyIsForm = radioButton_BodyForm.Checked;
            bool bodyIsRaw = radioButton_BodyRaw.Checked;
            bool bodyIsNone = radioButton_BodyNone.Checked;

            string authType = comboBox_AuthType.SelectedItem?.ToString() ?? "No Auth";
            string authKey = textBox_HeaderKey.Text.Trim();
            string authValue = textBox_HeaderValue.Text.Trim();

            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        var qParams = new Dictionary<string, string>(queryParams);
                        string finalUrl = url;
                        if (qParams.Count > 0)
                        {
                            string qs = ScanHelpers.BuildQueryString(qParams);
                            finalUrl = finalUrl.Contains("?") ? $"{finalUrl}&{qs}" : $"{finalUrl}?{qs}";
                        }

                        var req = new HttpRequestMessage(new HttpMethod(method), finalUrl);
                        ScanHelpers.ApplyAuth(req, authType, authKey, authValue);

                        foreach (var kv in extraHeaders)
                            if (!string.IsNullOrEmpty(kv.Key))
                                req.Headers.TryAddWithoutValidation(kv.Key, kv.Value);

                        if (!bodyIsNone && method != "GET" && method != "HEAD")
                        {
                            if (bodyIsJson)
                                req.Content = new StringContent(bodyText, Encoding.UTF8, "application/json");
                            else if (bodyIsForm)
                                req.Content = new FormUrlEncodedContent(formDataRows);
                            else if (bodyIsRaw)
                                req.Content = new StringContent(bodyText, Encoding.UTF8, "text/plain");
                        }

                        var iterClient = ScanHelpers.BuildClient();
                        var resp = await iterClient.SendAsync(req);
                        string body = await resp.Content.ReadAsStringAsync();
                        int code = (int)resp.StatusCode;

                        Invoke((Action)(() =>
                        {
                            string icon = code >= 200 && code < 300 ? "✅"
                                        : code >= 400 && code < 500 ? "⚠️"
                                        : code >= 500 ? "🚨" : "ℹ️";

                            Log(icon, $"HTTP {code}  {resp.ReasonPhrase}");

                            foreach (var h in resp.Headers)
                                foreach (var v in h.Value)
                                    Log("→", $"  {h.Key}: {v}");
                            foreach (var h in resp.Content.Headers)
                                foreach (var v in h.Value)
                                    Log("→", $"  {h.Key}: {v}");

                            LogSep();

                            if (string.IsNullOrWhiteSpace(body))
                            {
                                Log("📄", "(empty response body)");
                            }
                            else
                            {
                                string displayBody = body;
                                try
                                {
                                    var doc = JsonDocument.Parse(body);
                                    displayBody = JsonSerializer.Serialize(
                                        doc.RootElement,
                                        new JsonSerializerOptions { WriteIndented = true });
                                }
                                catch { /* not JSON – display as-is */ }

                                string[] lines = displayBody
                                    .Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
                                int lineCount = 0;
                                foreach (string line in lines)
                                {
                                    if (string.IsNullOrWhiteSpace(line)) continue;
                                    string trimmed = line.Length > 300 ? line.Substring(0, 300) + "…" : line;
                                    Log("", trimmed);
                                    if (++lineCount >= 200)
                                    {
                                        Log("…", $"(output truncated — {lines.Length} total lines)");
                                        break;
                                    }
                                }
                            }

                            LogSep();
                        }));
                    }
                    catch (Exception ex)
                    {
                        Invoke((Action)(() => Log("❌", "Request error: " + ex.Message)));
                    }
                });
            }
            finally
            {
                Invoke((Action)(() =>
                {
                    LogSep();
                    Log("✅", "API test complete.");
                    button_ApiForce.Enabled = true;
                    SetProgress(false);
                }));
            }
        }

        // ─── SAVE / CLEAR ─────────────────────────────────────────────────────────

        private void button_SaveReport_Click(object sender, EventArgs e)
        {
            if (richTextBox_Output.TextLength == 0) return;
            var dlg = new SaveFileDialog
            {
                Filter = "Text File|*.txt",
                FileName = $"ApiTest_Report_{DateTime.Now:yyyyMMdd_HHmmss}"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
                File.WriteAllText(dlg.FileName, richTextBox_Output.Text, Encoding.UTF8);
        }

        private void button_ClearOutput_Click(object sender, EventArgs e) => ClearOut();
    }
}
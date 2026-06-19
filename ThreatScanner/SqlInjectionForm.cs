using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreatScanner.Helpers;

namespace ThreatScanner
{
    /// <summary>
    /// SQL Injection — passive error-disclosure checker.
    ///
    /// For the target URL itself: appends a single quote to a query param.
    /// For each &lt;form&gt; discovered on the page: builds a normal POST/GET
    /// request from the form's real fields (via FormParsingHelper) and injects
    /// a single quote into each text-like field, one field at a time.
    ///
    /// Each probe is a single request that checks whether the response leaks a
    /// database error message. This is a passive disclosure check, not an
    /// exploitation tool — it does not chain payloads, confirm exploitability,
    /// or extract data.
    /// </summary>
    public partial class SqlInjectionForm : Form
    {
        private readonly HttpClient _http = ScanHelpers.BuildDefaultClient();
        private CancellationTokenSource _cts;

        private static readonly string[] SqlErrorSignatures = {
            "sql syntax", "mysql_fetch", "mysqli_", "you have an error in your sql syntax",
            "microsoft ole db", "odbc sql server driver", "unclosed quotation mark",
            "sqlstate", "incorrect syntax near",
            "ora-00933", "ora-01756", "ora-",
            "pg_query", "warning: pg_", "postgresql query failed",
            "sqlite3.operationalerror", "sqlite_error", "sqlite",
            "syntax error", "unterminated string", "quoted string not properly terminated"
        };

        public SqlInjectionForm() => InitializeComponent();

        // ════════════════════════════════════════════════════════════════════
        //  GRID HELPERS (same pattern as FullScannerForm)
        // ════════════════════════════════════════════════════════════════════

        private void SetProgress(bool running)
        {
            progressBar_Scan.Style = running ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            if (!running) progressBar_Scan.Value = 0;
        }

        private void AddRow(string name, string status, string response)
        {
            void Do()
            {
                int idx = dataGridView_Output.Rows.Add(name, status, response);
                var row = dataGridView_Output.Rows[idx];

                Color fore = Color.FromArgb(226, 232, 240);
                Color back = Color.FromArgb(15, 23, 42);

                if (status.StartsWith("🚨") || status.StartsWith("❌")) fore = Color.FromArgb(248, 113, 113);
                else if (status.StartsWith("⚠️")) fore = Color.FromArgb(251, 191, 36);
                else if (status.StartsWith("✅")) fore = Color.FromArgb(52, 211, 153);
                else if (status.StartsWith("ℹ️")) fore = Color.FromArgb(96, 165, 250);

                row.Cells["colStatus"].Style.ForeColor = fore;
                row.Cells["colStatus"].Style.BackColor = back;
                dataGridView_Output.FirstDisplayedScrollingRowIndex = idx;
            }

            if (dataGridView_Output.InvokeRequired) dataGridView_Output.Invoke((Action)Do);
            else Do();
        }

        private void AddSep(string section)
            => AddRow($"── {section} ──", "────", "────────────────────────────────────────");

        // ════════════════════════════════════════════════════════════════════
        //  MAIN SCAN
        // ════════════════════════════════════════════════════════════════════

        private async void button_Scan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_Url.Text))
            {
                MessageBox.Show("Please enter a URL.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dataGridView_Output.Rows.Clear();
            _cts = new CancellationTokenSource();
            string url = ScanHelpers.NormalizeUrl(textBox_Url.Text.Trim());

            button_Scan.Enabled = false;
            button_Stop.Enabled = true;
            SetProgress(true);
            AddRow("TARGET", "🔍 Scanning", url);

            try
            {
                var ct = _cts.Token;
                await Task.Run(async () =>
                {
                    // 1. Probe the URL's own query parameter(s)
                    await ProbeUrlParam(url, ct);

                    // 2. Discover and probe HTML forms on the page
                    await ProbeForms(url, ct);

                }, ct);
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
                    AddRow("DONE", "✅ Complete", "Probe finished.");
                    button_Scan.Enabled = true;
                    button_Stop.Enabled = false;
                    SetProgress(false);
                }));
            }
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            button_Stop.Enabled = false;
        }

        // ════════════════════════════════════════════════════════════════════
        //  PROBE 1: URL query parameter
        // ════════════════════════════════════════════════════════════════════

        private async Task ProbeUrlParam(string url, CancellationToken ct)
        {
            AddSep("URL Parameter Probe");

            string param = "id";
            string testUrl = url.Contains("?")
                ? $"{url}&{param}={Uri.EscapeDataString("1'")}"
                : $"{url}?{param}={Uri.EscapeDataString("1'")}";

            await RunSingleProbe($"URL param ({param})", testUrl, async () =>
            {
                var resp = await _http.GetAsync(testUrl, ct);
                return await resp.Content.ReadAsStringAsync();
            });
        }

        // ════════════════════════════════════════════════════════════════════
        //  PROBE 2: HTML forms on the page
        // ════════════════════════════════════════════════════════════════════

        private async Task ProbeForms(string url, CancellationToken ct)
        {
            AddSep("Form Discovery");

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
                return;
            }

            AddRow("Forms", "ℹ️ Found", $"{forms.Count} form(s) detected — testing text-like fields one at a time.");

            foreach (var form in forms)
            {
                ct.ThrowIfCancellationRequested();
                AddSep($"Form: {(string.IsNullOrWhiteSpace(form.Name) ? $"#{form.Index + 1}" : form.Name)}");
                AddRow("Form Target", "ℹ️ Info", $"{form.Method.ToUpper()} {form.Action} ({form.Fields.Count} fields, framework: {form.Framework})");

                // Only inject into fields a user could actually type into.
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
                    ct.ThrowIfCancellationRequested();
                    await ProbeFormField(form, field, ct);
                }
            }
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
                await RunSingleProbe(label, form.Action, async () =>
                {
                    HttpResponseMessage resp;
                    if (form.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    {
                        string qs = FormParsingHelper.BuildRequestBody(form, includeCsrfTokens: true);
                        string getUrl = form.Action.Contains("?") ? $"{form.Action}&{qs}" : $"{form.Action}?{qs}";
                        resp = await _http.GetAsync(getUrl, ct);
                    }
                    else
                    {
                        string body = FormParsingHelper.BuildRequestBody(form, includeCsrfTokens: true);
                        var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                        resp = await _http.PostAsync(form.Action, content, ct);
                    }
                    return await resp.Content.ReadAsStringAsync();
                });
            }
            finally
            {
                targetField.Value = originalValue; // restore for next field's probe
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  SHARED SINGLE-PROBE LOGIC
        // ════════════════════════════════════════════════════════════════════

        private async Task RunSingleProbe(string label, string requestDescription, Func<Task<string>> sendRequest)
        {
            try
            {
                string body = await sendRequest();
                var matched = SqlErrorSignatures.FirstOrDefault(err =>
                    body.IndexOf(err, StringComparison.OrdinalIgnoreCase) >= 0);

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
            catch (TaskCanceledException)
            {
                AddRow(label, "⚠️ Timeout", $"Request to {requestDescription} timed out.");
            }
            catch (Exception ex)
            {
                AddRow(label, "❌ Error", ex.Message);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  COPY / CONTEXT MENU (same pattern as FullScannerForm)
        // ════════════════════════════════════════════════════════════════════

        private void button_ClearOutput_Click(object sender, EventArgs e)
        {
            dataGridView_Output.Rows.Clear();
        }

        private void button_SaveReport_Click(object sender, EventArgs e)
        {
            if (dataGridView_Output.Rows.Count == 0)
            {
                MessageBox.Show("No probe results to save.", "ThreatScanner",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var dlg = new SaveFileDialog
            {
                Filter = "CSV File|*.csv|Text File|*.txt",
                FileName = $"ThreatScanner_SqlInjection_{DateTime.Now:yyyyMMdd_HHmmss}"
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
                System.IO.File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"Report saved: {dlg.FileName}", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

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

            var menu = new ContextMenuStrip();
            menu.Items.Add("📄  Copy Selected Rows", null, (s, ev) => button_CopyOutput_Click(s, ev));
            menu.Items.Add("📄  Copy All Rows", null, (s, ev) =>
            {
                dataGridView_Output.ClearSelection();
                button_CopyOutput_Click(s, ev);
            });
            menu.Show(dataGridView_Output, e.Location);
        }
    }
}
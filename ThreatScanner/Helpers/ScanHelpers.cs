using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace ThreatScanner.Helpers
{
    /// <summary>
    /// Static utility methods shared across all tool forms.
    /// </summary>
    public static class ScanHelpers
    {
        // ─── URL ─────────────────────────────────────────────────────────────────

        public static string NormalizeUrl(string raw)
        {
            raw = raw.Trim();
            if (!raw.StartsWith("http://") && !raw.StartsWith("https://"))
                raw = "https://" + raw;
            return raw;
        }

        // ─── OUTPUT LOGGING (ListBox) ─────────────────────────────────────────────

        public static void Log(ListBox output, string icon, string message)
        {
            string line = $"{icon}  {message}";
            if (output.InvokeRequired)
                output.Invoke((Action)(() => output.Items.Add(line)));
            else
                output.Items.Add(line);
        }

        public static void HtmlLog(ListBox output, string icon, string message)
        {
            const int MaxLen = 300;
            string safe = message.Replace("\r", "").Replace("\n", " | ");
            if (safe.Length > MaxLen) safe = safe.Substring(0, MaxLen) + "…";
            Log(output, icon, safe);
        }

        public static void LogSeparator(ListBox output)
            => Log(output, "", "─────────────────────────────────────────────");

        public static void ClearOutput(ListBox output)
        {
            if (output.InvokeRequired)
                output.Invoke((Action)(() => output.Items.Clear()));
            else
                output.Items.Clear();
        }

        // ─── HTTP CLIENT FACTORY ─────────────────────────────────────────────────

        public static HttpClient BuildDefaultClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ThreatScanner/1.0");
            return client;
        }

        public static HttpClient BuildClient()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ThreatScanner/1.0");
            return client;
        }

        // ─── AUTH ─────────────────────────────────────────────────────────────────

        public static void ApplyAuth(HttpRequestMessage req, string authType, string key, string value)
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
            }
        }

        // ─── GRID HELPERS ─────────────────────────────────────────────────────────

        public static Dictionary<string, string> GetEnabledGridRows(
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

        // ─── MISC ─────────────────────────────────────────────────────────────────

        public static void InjectWordlistValue(Dictionary<string, string> dict, string template, string wlValue)
        {
            foreach (string part in template.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!part.Contains("=")) continue;
                var kv = part.Split(new[] { '=' }, 2);
                dict[kv[0].Trim()] = kv[1].Trim().Replace("{value}", wlValue);
            }
        }

        public static Dictionary<string, string> ParseKeyValueLines(string text)
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

        public static string BuildQueryString(Dictionary<string, string> parameters)
            => string.Join("&", parameters.Select(kv =>
                $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

        // ─── GRID ROW DELETION ────────────────────────────────────────────────────

        /// <summary>
        /// Wires the Delete key on a DataGridView so selected rows are removed.
        /// Call once per grid in the form constructor.
        /// </summary>
        public static void EnableRowDeletion(DataGridView grid)
        {
            grid.KeyDown += (s, e) =>
            {
                if (e.KeyCode != System.Windows.Forms.Keys.Delete) return;
                var toRemove = new System.Collections.Generic.List<int>();
                foreach (DataGridViewRow row in grid.SelectedRows)
                    if (!row.IsNewRow) toRemove.Add(row.Index);
                if (toRemove.Count == 0)
                {
                    var rowSet = new System.Collections.Generic.HashSet<int>();
                    foreach (DataGridViewCell cell in grid.SelectedCells)
                        if (!grid.Rows[cell.RowIndex].IsNewRow) rowSet.Add(cell.RowIndex);
                    toRemove.AddRange(rowSet);
                }
                toRemove.Sort((a, b) => b.CompareTo(a));
                foreach (int idx in toRemove) grid.Rows.RemoveAt(idx);
                e.Handled = true;
            };
        }

        // ─── RICHTEXTBOX COLORS ───────────────────────────────────────────────────

        private static readonly System.Drawing.Color ColOk = System.Drawing.Color.FromArgb(78, 201, 176);
        private static readonly System.Drawing.Color ColWarn = System.Drawing.Color.FromArgb(255, 198, 64);
        private static readonly System.Drawing.Color ColError = System.Drawing.Color.FromArgb(240, 100, 100);
        private static readonly System.Drawing.Color ColInfo = System.Drawing.Color.FromArgb(134, 187, 243);
        private static readonly System.Drawing.Color ColDefault = System.Drawing.Color.FromArgb(212, 212, 212);
        private static readonly System.Drawing.Color ColSep = System.Drawing.Color.FromArgb(255, 255, 255);

        private static System.Drawing.Color IconToColor(string icon) =>
            icon == "✅" ? ColOk :
            icon == "⚠️" ? ColWarn :
            icon == "🚨" || icon == "❌" ? ColError :
            icon == "→" ? ColInfo :
            icon == "" ? ColSep :
                                       ColDefault;

        // ─── RICHTEXTBOX OUTPUT (icon-based) ─────────────────────────────────────

        /// <summary>
        /// Appends a coloured line to a RichTextBox using icon-derived colour.
        /// Thread-safe: marshals to the UI thread via Invoke when needed.
        /// </summary>
        public static void LogRtb(System.Windows.Forms.RichTextBox rtb, string icon, string message)
        {
            void Append()
            {
                rtb.SelectionStart = rtb.TextLength;
                rtb.SelectionLength = 0;
                rtb.SelectionColor = IconToColor(icon);
                string line = string.IsNullOrEmpty(icon) ? message : $"{icon}  {message}";
                rtb.AppendText(line + Environment.NewLine);
                rtb.SelectionColor = ColDefault;
                rtb.ScrollToCaret();
            }

            if (rtb.InvokeRequired) rtb.Invoke((Action)Append);
            else Append();
        }

        /// <summary>Appends a separator line to a RichTextBox output panel.</summary>
        public static void LogSeparatorRtb(System.Windows.Forms.RichTextBox rtb)
            => LogRtb(rtb, "", "─────────────────────────────────────────────");

        /// <summary>Clears the RichTextBox output panel (thread-safe).</summary>
        public static void ClearOutputRtb(System.Windows.Forms.RichTextBox rtb)
        {
            if (rtb.InvokeRequired) rtb.Invoke((Action)(() => rtb.Clear()));
            else rtb.Clear();
        }

        // ─── RICHTEXTBOX OUTPUT (explicit colour) ─────────────────────────────────

        /// <summary>
        /// Appends a timestamped line to a RichTextBox with an explicit colour.
        /// Used by forms (AutoFillForm, CsrfTesterForm) that write
        ///   "[HH:mm:ss]  message"  with a caller-supplied highlight colour.
        /// Passing <c>null</c> for <paramref name="color"/> falls back to the
        /// default text colour (RGB 212, 212, 212).
        /// Thread-safe via Invoke.
        /// </summary>
        public static void LogRtbColor(
            System.Windows.Forms.RichTextBox rtb,
            string message,
            System.Drawing.Color? color = null)
        {
            void Append()
            {
                rtb.SelectionStart = rtb.TextLength;
                rtb.SelectionLength = 0;
                rtb.SelectionColor = color ?? ColDefault;
                rtb.AppendText($"[{DateTime.Now:HH:mm:ss}]  {message}{Environment.NewLine}");
                rtb.SelectionColor = ColDefault;
                rtb.ScrollToCaret();
            }

            if (rtb.InvokeRequired) rtb.Invoke((Action)Append);
            else Append();
        }

        // ─── HTML STRIP HELPER (used by queue-based forms) ────────────────────────

        /// <summary>
        /// Strips HTML tags from <paramref name="message"/> and returns the
        /// plain-text result.  Used by BruteForceForm (and any future queue-based
        /// form) so the stripping logic lives in one place.
        /// </summary>
        public static string StripHtmlTags(string message)
            => System.Text.RegularExpressions.Regex.Replace(message, "<.*?>", string.Empty);

        /// <summary>
        /// Formats a queue-entry string the same way BruteForceForm's
        /// FlushPendingRows writes to the ListBox: "{icon}  {message}" (trimmed).
        /// Centralises the format so every queue-based form uses identical output.
        /// </summary>
        public static string FormatQueueRow(string icon, string message)
            => $"{icon}  {message}".TrimStart();
    }
}
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
                AppendPreservingScroll(rtb, () =>
                {
                    rtb.SelectionStart = rtb.TextLength;
                    rtb.SelectionLength = 0;
                    rtb.SelectionColor = IconToColor(icon);
                    string line = string.IsNullOrEmpty(icon) ? message : $"{icon}  {message}";
                    rtb.AppendText(line + Environment.NewLine);
                    rtb.SelectionColor = ColDefault;
                });
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
                AppendPreservingScroll(rtb, () =>
                {
                    rtb.SelectionStart = rtb.TextLength;
                    rtb.SelectionLength = 0;
                    rtb.SelectionColor = color ?? ColDefault;
                    rtb.AppendText($"[{DateTime.Now:HH:mm:ss}]  {message}{Environment.NewLine}");
                    rtb.SelectionColor = ColDefault;
                });
            }

            if (rtb.InvokeRequired) rtb.Invoke((Action)Append);
            else Append();
        }

        /// <summary>
        /// Public entry point for queue/batch-based forms (e.g. BruteForceForm's
        /// FlushPendingRows) that build up multiple lines and append them in one
        /// shot. Routes through the same scroll-preserving logic as LogRtb /
        /// LogRtbColor so all forms behave identically — the user can scroll up
        /// and stay there even while a scan is continuously logging in the
        /// background. Must be called on the UI thread.
        /// </summary>
        public static void AppendBatchPreservingScroll(System.Windows.Forms.RichTextBox rtb, string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            AppendPreservingScroll(rtb, () => rtb.AppendText(text));
        }

        /// <summary>
        /// Trims a RichTextBox down to its last <paramref name="maxLines"/> lines,
        /// preserving scroll position the same way appends do. Used to cap memory
        /// growth on long-running scans without yanking the user's scroll position
        /// if they're mid-read.
        /// </summary>
        public static void TrimToLastLinesPreservingScroll(System.Windows.Forms.RichTextBox rtb, int maxLines)
        {
            string[] lines = rtb.Lines;
            int overflow = lines.Length - maxLines;
            if (overflow <= 0) return;

            AppendPreservingScroll(rtb, () =>
            {
                rtb.Lines = lines.Skip(overflow).ToArray();
            });
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




        // ─── AUTO-SCROLL-AWARE RICHTEXTBOX LOGGING ────────────────────────────────

        // Win32 interop for true viewport scroll-position save/restore.
        // RichTextBox doesn't expose its scroll offset as a public property —
        // SelectionStart/ScrollToCaret only move the (invisible, since these
        // boxes are ReadOnly) caret, which is NOT the same thing as the
        // viewport position and is unreliable for "don't yank my scroll"
        // behaviour during rapid AppendText calls. EM_GETSCROLLPOS /
        // EM_SETSCROLLPOS read and write the actual scroll offset directly,
        // which is the only fully reliable way to do this.
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref System.Drawing.Point lParam);

        // Separate overload for messages like WM_SETREDRAW whose lParam is a
        // plain IntPtr, not a POINT struct — using the Point-typed overload
        // above for these would marshal the wrong data.
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int EM_GETSCROLLPOS = 0x0400 + 221;
        private const int EM_SETSCROLLPOS = 0x0400 + 222;
        private const int WM_SETREDRAW = 0x000B;

        /// <summary>
        /// Returns true if the RichTextBox's vertical scroll position is at (or very
        /// near) the bottom — i.e. the user hasn't scrolled up to read earlier text.
        /// Used to decide whether appending new text should also auto-scroll.
        /// </summary>
        private static bool IsScrolledToBottom(System.Windows.Forms.RichTextBox rtb)
        {
            if (rtb.TextLength == 0) return true;

            // Character index at the bottom-left corner of the visible client area.
            var bottomPoint = new System.Drawing.Point(0, rtb.ClientSize.Height - 1);
            int charIndexAtBottom = rtb.GetCharIndexFromPosition(bottomPoint);

            // If the char at the bottom of the visible area is within a small
            // tolerance of the end of the text, treat the view as "at the bottom".
            // Tolerance covers the partially-clipped last line.
            const int tolerance = 2;
            return charIndexAtBottom >= rtb.TextLength - tolerance - 1;
        }

        /// <summary>
        /// Appends text to a RichTextBox without disturbing the user's scroll
        /// position if they've scrolled up to read earlier lines. Only auto-scrolls
        /// to the new bottom if the view was already at (or near) the bottom.
        /// Holds the real native scroll offset (not just the caret) so the
        /// viewport genuinely stays put even under rapid, continuous appends —
        /// this is what lets the user scroll up and stay there while logging
        /// keeps running in the background. Must be called on the UI thread.
        /// </summary>
        private static void AppendPreservingScroll(System.Windows.Forms.RichTextBox rtb, Action appendAction)
        {
            bool wasAtBottom = IsScrolledToBottom(rtb);

            // Remember the caret/selection the user may have set (e.g. text they
            // selected to copy) so we don't clobber it for no reason.
            int savedSelStart = rtb.SelectionStart;
            int savedSelLength = rtb.SelectionLength;

            // Snapshot the real scroll offset before appending.
            var scrollPos = new System.Drawing.Point();
            SendMessage(rtb.Handle, EM_GETSCROLLPOS, IntPtr.Zero, ref scrollPos);

            // Suspend repainting for the duration of the append so the
            // intermediate (pre-restore) scroll jump never flashes on screen.
            SendMessage(rtb.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

            try
            {
                appendAction();

                if (wasAtBottom)
                {
                    rtb.SelectionStart = rtb.TextLength;
                    rtb.SelectionLength = 0;
                    rtb.ScrollToCaret();
                }
                else
                {
                    // Restore prior selection/caret so the append doesn't move it.
                    if (savedSelStart <= rtb.TextLength)
                    {
                        rtb.SelectionStart = savedSelStart;
                        rtb.SelectionLength = savedSelLength;
                    }

                    // Restore the real viewport position the user was reading —
                    // this is the actual fix: AppendText silently scrolls the
                    // view in some cases even when the caret doesn't move, and
                    // only EM_SETSCROLLPOS reliably undoes that.
                    SendMessage(rtb.Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref scrollPos);
                }
            }
            finally
            {
                SendMessage(rtb.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
                rtb.Invalidate();
            }
        }
    }
}
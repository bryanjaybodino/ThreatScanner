using System;
using System.Drawing;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace ThreatScanner.Helpers
{
    /// <summary>
    /// Reusable JSON editor panel: dark-themed RichTextBox with Format / Validate / Copy toolbar.
    /// Call <see cref="Create"/> to get a Panel you can drop into any form or tab page.
    /// </summary>
    public static class JsonEditorHelper
    {
        // ── Colours (VS Code Dark+ palette) ────────────────────────────────────
        private static readonly Color BgColor = Color.FromArgb(30, 30, 30);
        private static readonly Color ToolbarBg = Color.FromArgb(37, 37, 38);
        private static readonly Color KeyColor = Color.FromArgb(134, 187, 243);   // blue   – keys
        private static readonly Color StringColor = Color.FromArgb(206, 145, 120);   // orange – string values
        private static readonly Color NumberColor = Color.FromArgb(181, 206, 168);   // green  – numbers
        private static readonly Color BoolNullColor = Color.FromArgb(86, 156, 214);   // bright-blue – true/false/null
        private static readonly Color BraceColor = Color.FromArgb(255, 215, 0);     // gold   – braces / brackets
        private static readonly Color ColonColor = Color.FromArgb(212, 212, 212);   // grey   – colon / comma
        private static readonly Color DefaultColor = Color.FromArgb(212, 212, 212);   // light-grey
        private static readonly Color BorderAccent = Color.FromArgb(0, 122, 204);   // VS Code blue border

        // ── Button colours ─────────────────────────────────────────────────────
        private static readonly Color BtnBg = Color.FromArgb(50, 50, 50);
        private static readonly Color BtnBorder = Color.FromArgb(70, 70, 70);
        private static readonly Color BtnHover = Color.FromArgb(65, 65, 65);
        private static readonly Color StatusOk = Color.FromArgb(78, 201, 176);  // teal
        private static readonly Color StatusErr = Color.FromArgb(240, 100, 100);  // red
        private static readonly Color StatusInfo = Color.FromArgb(150, 150, 150);  // grey

        // ── Public factory ─────────────────────────────────────────────────────

        /// <summary>
        /// Creates a self-contained JSON editor Panel.
        /// </summary>
        /// <param name="outEditor">The inner RichTextBox — read/write .Text to get/set JSON.</param>
        public static Panel Create(out RichTextBox outEditor)
        {
            // ── Toolbar ────────────────────────────────────────────────────────
            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = ToolbarBg,
                Padding = new Padding(6, 4, 6, 0)
            };

            // Draw a thin blue accent line at the bottom of the toolbar
            toolbar.Paint += (s, e) =>
            {
                var pen = new System.Drawing.Pen(BorderAccent, 1);
                e.Graphics.DrawLine(pen, 0, toolbar.Height - 1, toolbar.Width, toolbar.Height - 1);
            };

            Button MakeBtn(string text, int x) => new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(200, 200, 200),
                BackColor = BtnBg,
                Cursor = Cursors.Hand,
                AutoSize = true,
                Location = new Point(x, 5),
                Height = 26,
                Padding = new Padding(6, 0, 6, 0)
            };

            var btnFormat = MakeBtn("{ } Format", 4);
            var btnValidate = MakeBtn("✔ Validate", 90);
            var btnClear = MakeBtn("✕ Clear", 172);
            var btnCopy = MakeBtn("⧉ Copy", 232);

            foreach (var btn in new[] { btnFormat, btnValidate, btnClear, btnCopy })
            {
                btn.FlatAppearance.BorderColor = BtnBorder;
                btn.FlatAppearance.MouseOverBackColor = BtnHover;
                btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(80, 80, 80);
                toolbar.Controls.Add(btn);
            }

            var lblStatus = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = StatusInfo,
                Location = new Point(300, 10),
                Text = "ready"
            };
            toolbar.Controls.Add(lblStatus);

            void SetStatus(string msg, Color color)
            {
                lblStatus.Text = msg;
                lblStatus.ForeColor = color;
            }

            // ── Editor ─────────────────────────────────────────────────────────
            var rtb = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Cascadia Code", 10f) is var f && f.Name == "Cascadia Code"
                              ? f : new Font("Consolas", 10f),
                BackColor = BgColor,
                ForeColor = DefaultColor,
                BorderStyle = BorderStyle.None,
                AcceptsTab = true,
                ScrollBars = RichTextBoxScrollBars.Both,
                WordWrap = false,
                DetectUrls = false,
                Margin = new Padding(0)
            };

            // ── Button handlers ────────────────────────────────────────────────
            btnFormat.Click += (s, e) =>
            {
                string result = FormatAndHighlight(rtb, out string error);
                SetStatus(result, string.IsNullOrEmpty(error) ? StatusOk : StatusErr);
            };

            btnValidate.Click += (s, e) =>
            {
                string text = rtb.Text.Trim();
                if (string.IsNullOrEmpty(text)) { SetStatus("(empty)", StatusInfo); return; }
                try { JsonDocument.Parse(text); SetStatus("✔ Valid JSON", StatusOk); }
                catch (JsonException ex) { SetStatus($"✖ {ex.Message}", StatusErr); }
            };

            btnClear.Click += (s, e) =>
            {
                rtb.Clear();
                SetStatus("cleared", StatusInfo);
            };

            btnCopy.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(rtb.Text))
                    Clipboard.SetText(rtb.Text);
                SetStatus("copied ✔", StatusOk);
            };

            // ── Live syntax highlight (debounced via flag) ──────────────────────
            bool highlighting = false;
            rtb.TextChanged += (s, e) =>
            {
                if (highlighting) return;
                highlighting = true;
                try { ApplySyntaxHighlight(rtb); }
                finally { highlighting = false; }
            };

            // ── Line-number gutter (lightweight) ───────────────────────────────
            var gutter = new Panel
            {
                Width = 42,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(37, 37, 38),
                Padding = new Padding(0)
            };

            rtb.VScroll += (s, e) => gutter.Invalidate();
            rtb.TextChanged += (s, e) => gutter.Invalidate();

            gutter.Paint += (s, e) =>
            {
                e.Graphics.Clear(Color.FromArgb(37, 37, 38));
                var brush = new SolidBrush(Color.FromArgb(90, 90, 90));
                var font = new Font("Consolas", 9f);
                int lineH = rtb.Font.Height + 1;
                // First visible line via GetCharIndexFromPosition
                int firstChar = rtb.GetCharIndexFromPosition(new Point(0, 0));
                int firstLine = rtb.GetLineFromCharIndex(firstChar);
                int totalLines = rtb.Lines.Length == 0 ? 1 : rtb.Lines.Length;
                int gutterW = gutter.Width;

                for (int i = 0; i < gutter.Height / lineH + 2; i++)
                {
                    int lineNum = firstLine + i + 1;
                    if (lineNum > totalLines) break;
                    string num = lineNum.ToString();
                    var sz = e.Graphics.MeasureString(num, font);
                    e.Graphics.DrawString(num, font, brush,
                        gutterW - sz.Width - 4,
                        i * lineH + 2);
                }
            };

            // ── Outer container ────────────────────────────────────────────────
            var editorRow = new Panel { Dock = DockStyle.Fill, BackColor = BgColor };
            editorRow.Controls.Add(rtb);
            editorRow.Controls.Add(gutter);

            var outer = new Panel { Dock = DockStyle.Fill, BackColor = BgColor };
            outer.Controls.Add(editorRow);
            outer.Controls.Add(toolbar);

            outEditor = rtb;
            return outer;
        }

        // ── Public helpers ─────────────────────────────────────────────────────

        /// <summary>Pretty-prints JSON. Returns original string on failure.</summary>
        public static string PrettyPrint(string json)
        {
            try
            {
                var doc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(doc.RootElement,
                    new JsonSerializerOptions { WriteIndented = true });
            }
            catch { return json; }
        }

        /// <summary>
        /// Formats the RichTextBox content as indented JSON and re-highlights it.
        /// Returns a short status string; sets <paramref name="error"/> on failure.
        /// </summary>
        public static string FormatAndHighlight(RichTextBox rtb, out string error)
        {
            error = null;
            string text = rtb.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                ApplySyntaxHighlight(rtb);
                return "";
            }

            try
            {
                var doc = JsonDocument.Parse(text);
                string pretty = JsonSerializer.Serialize(doc.RootElement,
                    new JsonSerializerOptions { WriteIndented = true });

                int caret = Math.Min(rtb.SelectionStart, pretty.Length);
                rtb.Text = pretty;
                rtb.SelectionStart = caret;
                ApplySyntaxHighlight(rtb);
                return "✔ Valid JSON";
            }
            catch (JsonException ex)
            {
                error = ex.Message;
                return $"✖ {ex.Message}";
            }
        }

        // ── Syntax highlighting ────────────────────────────────────────────────

        /// <summary>
        /// Applies VS Code Dark+ syntax highlighting to the entire RichTextBox.
        /// Preserves scroll position. Safe to call from TextChanged.
        /// </summary>
        public static void ApplySyntaxHighlight(RichTextBox rtb)
        {
            string text = rtb.Text;
            if (string.IsNullOrEmpty(text)) return;

            rtb.SuspendLayout();
            int savedStart = rtb.SelectionStart;
            int savedLength = rtb.SelectionLength;

            // Default: whole document light-grey
            rtb.SelectAll();
            rtb.SelectionColor = DefaultColor;

            int i = 0;
            while (i < text.Length)
            {
                char c = text[i];

                // ── String (key or value) ──────────────────────────────────────
                if (c == '"')
                {
                    int start = i++;
                    while (i < text.Length)
                    {
                        if (text[i] == '\\') { i += 2; continue; }
                        if (text[i] == '"') { i++; break; }
                        i++;
                    }
                    // Is it a key? Peek past whitespace for ':'
                    int peek = i;
                    while (peek < text.Length && (text[peek] == ' ' || text[peek] == '\t')) peek++;
                    bool isKey = peek < text.Length && text[peek] == ':';
                    Colorize(rtb, start, i - start, isKey ? KeyColor : StringColor);
                    continue;
                }

                // ── Number ─────────────────────────────────────────────────────
                if (char.IsDigit(c) || (c == '-' && i + 1 < text.Length && char.IsDigit(text[i + 1])))
                {
                    int start = i;
                    if (c == '-') i++;
                    while (i < text.Length && (char.IsDigit(text[i]) || text[i] == '.' ||
                           text[i] == 'e' || text[i] == 'E' || text[i] == '+' || text[i] == '-'))
                        i++;
                    Colorize(rtb, start, i - start, NumberColor);
                    continue;
                }

                // ── true / false / null ────────────────────────────────────────
                if (Matches(text, i, "true")) { Colorize(rtb, i, 4, BoolNullColor); i += 4; continue; }
                if (Matches(text, i, "false")) { Colorize(rtb, i, 5, BoolNullColor); i += 5; continue; }
                if (Matches(text, i, "null")) { Colorize(rtb, i, 4, BoolNullColor); i += 4; continue; }

                // ── Braces & brackets ──────────────────────────────────────────
                if (c == '{' || c == '}' || c == '[' || c == ']')
                { Colorize(rtb, i, 1, BraceColor); i++; continue; }

                // ── Colon & comma ──────────────────────────────────────────────
                if (c == ':' || c == ',')
                { Colorize(rtb, i, 1, ColonColor); i++; continue; }

                i++;
            }

            rtb.SelectionStart = savedStart;
            rtb.SelectionLength = savedLength;
            rtb.ResumeLayout();
        }

        // ── Private utils ──────────────────────────────────────────────────────

        private static void Colorize(RichTextBox rtb, int start, int length, Color color)
        {
            rtb.SelectionStart = start;
            rtb.SelectionLength = length;
            rtb.SelectionColor = color;
        }

        private static bool Matches(string text, int pos, string word)
        {
            if (pos + word.Length > text.Length) return false;
            for (int k = 0; k < word.Length; k++)
                if (text[pos + k] != word[k]) return false;
            return true;
        }
    }
}
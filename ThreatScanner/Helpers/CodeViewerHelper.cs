// CodeViewerHelper.cs
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ThreatScanner.Helpers
{
    /// <summary>
    /// SuspendLayout/ResumeLayout do NOT stop a RichTextBox from repainting
    /// on every SelectionColor change — that repaint is the #1 cause of the
    /// "click a row and the form freezes" symptom when a body has thousands
    /// of highlight tokens. WM_SETREDRAW actually tells Win32 to stop
    /// painting the control until we say otherwise, which turns thousands
    /// of tiny repaints into a single one.
    /// </summary>
    internal static class NativeRedraw
    {
        private const int WM_SETREDRAW = 0x000B;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, bool wParam, IntPtr lParam);

        public static IDisposable Suspend(Control c) => new Scope(c);

        private sealed class Scope : IDisposable
        {
            private readonly Control _c;
            private bool _disposed;
            public Scope(Control c)
            {
                _c = c;
                if (_c.IsHandleCreated)
                    SendMessage(_c.Handle, WM_SETREDRAW, false, IntPtr.Zero);
            }
            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                if (_c.IsHandleCreated)
                {
                    SendMessage(_c.Handle, WM_SETREDRAW, true, IntPtr.Zero);
                    _c.Invalidate();
                    _c.Update();
                }
            }
        }
    }

    public enum CodeLanguage { Json, Html, Css, JavaScript, Text }

    /// <summary>
    /// Self-contained "View / Format" panel for a body of code (JSON, HTML,
    /// CSS, JS, or plain text). Used for the Request/Response body tabs in
    /// the HTTP Proxy Log form, but generic enough to drop in anywhere.
    /// </summary>
    public static class CodeViewerHelper
    {
        // VS Code Dark+ -ish palette, matching JsonEditorHelper for consistency.
        // Internal (not private) so SyntaxHighlighter can tag spans with the
        // right color while doing its work on a background thread, without
        // needing a RichTextBox or any UI access at all.
        internal static readonly Color BgColor = Color.FromArgb(30, 30, 30);
        internal static readonly Color ToolbarBg = Color.FromArgb(37, 37, 38);
        internal static readonly Color DefaultColor = Color.FromArgb(212, 212, 212);
        internal static readonly Color KeyColor = Color.FromArgb(134, 187, 243);   // JSON keys / HTML tag names
        internal static readonly Color StringColor = Color.FromArgb(206, 145, 120);   // strings / attribute values
        internal static readonly Color NumberColor = Color.FromArgb(181, 206, 168);
        internal static readonly Color BoolNullColor = Color.FromArgb(86, 156, 214);
        internal static readonly Color BraceColor = Color.FromArgb(255, 215, 0);
        internal static readonly Color CommentColor = Color.FromArgb(106, 153, 85);
        internal static readonly Color AttrNameColor = Color.FromArgb(156, 220, 254);
        internal static readonly Color KeywordColor = Color.FromArgb(197, 134, 192);
        private static readonly Color BtnBg = Color.FromArgb(50, 50, 50);
        private static readonly Color BtnBorder = Color.FromArgb(70, 70, 70);
        private static readonly Color BtnHover = Color.FromArgb(65, 65, 65);
        private static readonly Color StatusOk = Color.FromArgb(78, 201, 176);
        private static readonly Color StatusErr = Color.FromArgb(240, 100, 100);
        private static readonly Color StatusInfo = Color.FromArgb(150, 150, 150);

        private static readonly string[] JsKeywords =
        {
            "function","return","var","let","const","if","else","for","while","new",
            "this","null","true","false","typeof","class","extends","import","export",
            "default","async","await","try","catch","finally","throw","switch","case","break"
        };

        /// <summary>Guesses a language from an HTTP Content-Type header value.</summary>
        public static CodeLanguage DetectLanguage(string contentType)
        {
            contentType = (contentType ?? "").ToLowerInvariant();
            if (contentType.Contains("json")) return CodeLanguage.Json;
            if (contentType.Contains("html")) return CodeLanguage.Html;
            if (contentType.Contains("css")) return CodeLanguage.Css;
            if (contentType.Contains("javascript") || contentType.Contains("ecmascript")) return CodeLanguage.JavaScript;
            return CodeLanguage.Text;
        }

        /// <summary>
        /// Creates the viewer panel. Call <see cref="SetContent"/> on the
        /// returned controller to load a new body + language into it.
        /// </summary>
        /// <param name="editable">
        /// False (default) gives the original read-only response/request
        /// *viewer* behavior used by HttpProxyLogForm. True turns the inner
        /// RichTextBox into a typeable editor — for forms like ApiTesterForm
        /// that need the user to compose a JSON/raw body, not just inspect
        /// one. In editable mode, syntax highlighting runs on Format-click
        /// and on SetContent, but NOT on every keystroke — re-highlighting
        /// while someone is actively typing would either lag or visibly
        /// fight the caret, so typed text stays plain until the user hits
        /// Format (or the form calls SetContent itself, e.g. loading a
        /// saved request).
        /// </param>
        public static CodeViewerControl Create(bool editable = false)
        {
            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 34,
                BackColor = ToolbarBg,
                Padding = new Padding(4, 4, 4, 0),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
            };

            Button MakeBtn(string text) => new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(200, 200, 200),
                BackColor = BtnBg,
                Cursor = Cursors.Hand,
                Size = new Size(90, 24),
                Margin = new Padding(2, 0, 2, 0),
            };

            var lblLang = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(86, 156, 214),
                Text = "TEXT",
                Margin = new Padding(2, 6, 8, 0),
            };
            var btnFormat = MakeBtn("{ } Format");
            var btnWrap = MakeBtn("↵ Wrap");
            var btnCopy = MakeBtn("⧉ Copy");
            var lblStatus = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = StatusInfo,
                Text = "",
                Margin = new Padding(8, 6, 0, 0),
            };

            foreach (var btn in new[] { btnFormat, btnWrap, btnCopy })
            {
                btn.FlatAppearance.BorderColor = BtnBorder;
                btn.FlatAppearance.MouseOverBackColor = BtnHover;
                toolbar.Controls.Add(btn);
            }
            toolbar.Controls.Add(lblLang);
            toolbar.Controls.Add(lblStatus);

            var rtb = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10f),
                BackColor = BgColor,
                ForeColor = DefaultColor,
                BorderStyle = BorderStyle.None,
                ReadOnly = !editable,
                ScrollBars = RichTextBoxScrollBars.Both,
                WordWrap = false,
                DetectUrls = false,
                AcceptsTab = editable,
            };

            var outer = new Panel { Dock = DockStyle.Fill, BackColor = BgColor };
            outer.Controls.Add(rtb);
            outer.Controls.Add(toolbar);

            var control = new CodeViewerControl(outer, rtb, lblLang, lblStatus, editable);

            btnFormat.Click += (s, e) => control.Prettify();
            btnWrap.Click += (s, e) =>
            {
                rtb.WordWrap = !rtb.WordWrap;
                rtb.ScrollBars = rtb.WordWrap ? RichTextBoxScrollBars.Vertical : RichTextBoxScrollBars.Both;
            };
            btnCopy.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(rtb.Text)) Clipboard.SetText(rtb.Text);
                control.SetStatus("copied ✔", StatusOk);
            };

            // lblLang starts at "TEXT"/default language for a fresh editor —
            // SetContent() is what a viewer caller uses to pick a language;
            // an editor caller can call SetLanguage() directly instead since
            // there's no response body driving the choice.
            if (editable) control.SetLanguage(CodeLanguage.Json);

            return control;
        }

        // ── Controller object handed back to the form ────────────────────────────
        public class CodeViewerControl
        {
            // Bodies larger than this are still shown in full (so you can read
            // and copy them) but are NOT syntax-highlighted — coloring a
            // few-hundred-KB blob token-by-token is exactly what was freezing
            // the form, since the UI thread doesn't return until every token's
            // SelectionColor has been applied. Above this size we just show
            // plain text instead, which is instant.
            private const int MaxHighlightChars = 200_000;

            public Panel Panel { get; }

            /// <summary>
            /// The raw RichTextBox, exposed for editable instances so a form
            /// can bind to TextChanged, read .Text live while the user types
            /// (e.g. ApiTesterForm reading the request body at send-time),
            /// or pass it anywhere a RichTextBox is expected — the same
            /// calling convention JsonEditorHelper.Create(out var box) used.
            /// Null-safe to use even on a read-only viewer, but typing into
            /// it will be rejected by Win32 since ReadOnly stays true there.
            /// </summary>
            public RichTextBox Editor => _rtb;

            /// <summary>Live text in the box — same value as Editor.Text, for callers that don't want to hold a RichTextBox reference.</summary>
            public string Text
            {
                get => _rtb.Text;
                set => SetContent(value, _language);
            }

            private readonly RichTextBox _rtb;
            private readonly Label _lblLang;
            private readonly Label _lblStatus;
            private readonly bool _editable;
            private string _rawText = "";
            private CodeLanguage _language = CodeLanguage.Text;

            // Bumped on every SetContent call; a background highlight pass
            // checks this before touching the control, so clicking quickly
            // through several grid rows never leaves a stale highlight job
            // racing to paint over the newer one (and never piles up work).
            private int _generation = 0;

            internal CodeViewerControl(Panel panel, RichTextBox rtb, Label lblLang, Label lblStatus, bool editable = false)
            {
                Panel = panel; _rtb = rtb; _lblLang = lblLang; _lblStatus = lblStatus; _editable = editable;
            }

            /// <summary>
            /// Sets the language label/highlighting mode without touching the
            /// text — for an editable instance where there's no response
            /// Content-Type to drive DetectLanguage, so the form (or the
            /// body-type radio buttons) decides the language directly.
            /// </summary>
            public void SetLanguage(CodeLanguage language)
            {
                _language = language;
                _lblLang.Text = language.ToString().ToUpperInvariant();
            }

            public void SetContent(string text, CodeLanguage language)
            {
                int myGeneration = Interlocked.Increment(ref _generation);

                _rawText = text ?? "";
                _language = language;
                _lblLang.Text = language.ToString().ToUpperInvariant();

                // Setting .Text on a RichTextBox is itself cheap (single op);
                // it's only per-token SelectionColor calls that are expensive.
                _rtb.Text = _rawText;

                if (_rawText.Length == 0)
                {
                    SetStatus("", StatusInfo);
                    return;
                }

                if (_rawText.Length > MaxHighlightChars)
                {
                    _rtb.SelectAll();
                    _rtb.SelectionColor = DefaultColor;
                    _rtb.DeselectAll();
                    SetStatus(string.Format("body too large to highlight ({0:N0} chars) — showing plain text", _rawText.Length), StatusInfo);
                    return;
                }

                SetStatus("", StatusInfo);
                HighlightAsync(myGeneration);
            }

            public void SetStatus(string msg, Color color)
            {
                _lblStatus.Text = msg;
                _lblStatus.ForeColor = color;
            }

            public void Prettify()
            {
                try
                {
                    string pretty = CodeFormatter.Format(_rtb.Text, _language);
                    _rtb.Text = pretty;
                    _rawText = pretty; // keep in sync — HighlightAsync reads _rawText, not _rtb.Text

                    if (pretty.Length > MaxHighlightChars)
                    {
                        SetStatus("✔ formatted (too large to re-highlight)", StatusOk);
                        return;
                    }
                    int myGeneration = Interlocked.Increment(ref _generation);
                    HighlightAsync(myGeneration);
                    SetStatus("✔ formatted", StatusOk);
                }
                catch (Exception ex)
                {
                    SetStatus("✖ " + ex.Message, StatusErr);
                }
            }

            /// <summary>
            /// Computes highlight spans off the UI thread (pure regex/string
            /// work over a local string copy — no control access), then hops
            /// back to paint them in one redraw-suspended batch. This is what
            /// keeps the grid responsive: row selection returns immediately,
            /// and the body tab fills in a beat later instead of blocking.
            /// </summary>
            private void HighlightAsync(int myGeneration)
            {
                string text = _rawText;
                CodeLanguage language = _language;

                Task.Run(() =>
                {
                    List<SyntaxHighlighter.Span> spans;
                    try { spans = SyntaxHighlighter.ComputeSpans(text, language); }
                    catch { spans = new List<SyntaxHighlighter.Span>(); }

                    if (myGeneration != _generation) return; // superseded by a newer selection

                    try
                    {
                        if (!_rtb.IsHandleCreated || _rtb.IsDisposed) return;
                        _rtb.BeginInvoke((Action)(() =>
                        {
                            if (myGeneration != _generation) return; // double-check on the UI thread too
                            if (_rtb.IsDisposed) return;
                            Paint(spans);
                        }));
                    }
                    catch (ObjectDisposedException) { /* form closed mid-flight */ }
                    catch (InvalidOperationException) { /* handle torn down mid-flight */ }
                });
            }

            private void Paint(List<SyntaxHighlighter.Span> spans)
            {
                using (NativeRedraw.Suspend(_rtb))
                {
                    int savedStart = _rtb.SelectionStart;

                    _rtb.SelectAll();
                    _rtb.SelectionColor = DefaultColor;

                    foreach (var s in spans)
                    {
                        _rtb.SelectionStart = s.Start;
                        _rtb.SelectionLength = s.Length;
                        _rtb.SelectionColor = s.Color;
                    }

                    _rtb.SelectionStart = Math.Min(savedStart, _rtb.TextLength);
                    _rtb.SelectionLength = 0;
                }
            }
        }
    }

    /// <summary>Best-effort, parser-free pretty-printers. Good enough for eyeballing a payload, not a replacement for a real formatter.</summary>
    internal static class CodeFormatter
    {
        public static string Format(string text, CodeLanguage language)
        {
            switch (language)
            {
                case CodeLanguage.Json: return FormatJson(text);
                case CodeLanguage.Html: return FormatMarkupOrCss(text, isCss: false);
                case CodeLanguage.Css: return FormatMarkupOrCss(text, isCss: true);
                case CodeLanguage.JavaScript: return FormatJs(text);
                default: return text;
            }
        }

        private static string FormatJson(string text)
        {
            var doc = JsonDocument.Parse(text);
            return JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = true });
        }

        // Simple tag/brace-depth indenter — not a real parser, but handles
        // typical API/HTML responses well enough to read at a glance.
        private static string FormatMarkupOrCss(string text, bool isCss)
        {
            text = Regex.Replace(text.Trim(), @">\s*<", ">\n<");          // HTML: break between tags
            if (isCss)
            {
                text = text.Replace("{", "{\n").Replace("}", "\n}\n").Replace(";", ";\n");
            }

            string[] lines = text.Split('\n');
            var sb = new StringBuilder();
            int depth = 0;
            foreach (var raw in lines)
            {
                string line = raw.Trim();
                if (line.Length == 0) continue;

                bool dedentFirst = isCss ? line.StartsWith("}") : Regex.IsMatch(line, @"^</");
                if (dedentFirst && depth > 0) depth--;

                sb.Append(new string(' ', depth * 2)).Append(line).Append('\n');

                if (isCss)
                {
                    if (line.EndsWith("{")) depth++;
                }
                else
                {
                    bool opens = Regex.IsMatch(line, @"^<[a-zA-Z][^>]*[^/]>$") && !Regex.IsMatch(line, @"^<(br|img|input|hr|meta|link)\b", RegexOptions.IgnoreCase);
                    bool closesSelf = line.EndsWith("/>") || Regex.IsMatch(line, @"^<[a-zA-Z][^>]*>.*</[a-zA-Z]+>$");
                    if (opens && !closesSelf && !dedentFirst) depth++;
                }
            }
            return sb.ToString();
        }

        // No real JS parser here — just normalizes statement breaks so a
        // minified blob is at least scrollable/readable. Use a real
        // formatter (e.g. browser devtools) for anything you need exact.
        private static string FormatJs(string text)
        {
            text = Regex.Replace(text, @";(?!\n)", ";\n");
            text = Regex.Replace(text, @"\{(?!\n)", "{\n");
            text = Regex.Replace(text, @"\}(?!\n)", "\n}\n");

            string[] lines = text.Split('\n');
            var sb = new StringBuilder();
            int depth = 0;
            foreach (var raw in lines)
            {
                string line = raw.Trim();
                if (line.Length == 0) continue;
                if (line.StartsWith("}") && depth > 0) depth--;
                sb.Append(new string(' ', depth * 2)).Append(line).Append('\n');
                if (line.EndsWith("{")) depth++;
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Regex/scan-based highlighters — fast and good enough for a log viewer,
    /// not a full lexer.
    ///
    /// IMPORTANT: this class no longer touches a RichTextBox at all. It only
    /// computes (start, length, color) spans over a plain string, which is
    /// safe to run on a background thread. Painting those spans onto the
    /// control is the caller's job (see CodeViewerControl.Paint), done in one
    /// redraw-suspended batch on the UI thread. Splitting it this way is what
    /// stops a large response body from freezing the form: the expensive
    /// regex scanning happens off the UI thread, and the part that must run
    /// on the UI thread (applying SelectionColor) is now the only "thousands
    /// of Win32 calls" cost left — and WM_SETREDRAW collapses the repaints
    /// from thousands down to one.
    /// </summary>
    internal static class SyntaxHighlighter
    {
        public readonly struct Span
        {
            public readonly int Start, Length;
            public readonly Color Color;
            public Span(int start, int length, Color color) { Start = start; Length = length; Color = color; }
        }

        // Colors are resolved by the caller's palette; this method just needs
        // *a* palette to tag spans with, so it accepts one explicitly rather
        // than reaching into CodeViewerHelper's private statics.
        public static List<Span> ComputeSpans(string text, CodeLanguage language)
        {
            switch (language)
            {
                case CodeLanguage.Json: return ComputeJson(text);
                case CodeLanguage.Html: return ComputeHtml(text);
                case CodeLanguage.Css: return ComputeCss(text);
                case CodeLanguage.JavaScript: return ComputeJs(text);
                default: return new List<Span>();
            }
        }

        private static List<Span> ComputeJson(string text)
        {
            var spans = new List<Span>();
            if (string.IsNullOrEmpty(text)) return spans;

            int i = 0;
            while (i < text.Length)
            {
                char c = text[i];
                if (c == '"')
                {
                    int start = i++;
                    while (i < text.Length) { if (text[i] == '\\') { i += 2; continue; } if (text[i] == '"') { i++; break; } i++; }
                    int peek = i; while (peek < text.Length && (text[peek] == ' ' || text[peek] == '\t')) peek++;
                    bool isKey = peek < text.Length && text[peek] == ':';
                    spans.Add(new Span(start, i - start, isKey ? CodeViewerHelper.KeyColor : CodeViewerHelper.StringColor));
                    continue;
                }
                if (char.IsDigit(c) || (c == '-' && i + 1 < text.Length && char.IsDigit(text[i + 1])))
                {
                    int start = i; if (c == '-') i++;
                    while (i < text.Length && (char.IsDigit(text[i]) || text[i] == '.' || text[i] == 'e' || text[i] == 'E' || text[i] == '+' || text[i] == '-')) i++;
                    spans.Add(new Span(start, i - start, CodeViewerHelper.NumberColor));
                    continue;
                }
                if (Matches(text, i, "true")) { spans.Add(new Span(i, 4, CodeViewerHelper.BoolNullColor)); i += 4; continue; }
                if (Matches(text, i, "false")) { spans.Add(new Span(i, 5, CodeViewerHelper.BoolNullColor)); i += 5; continue; }
                if (Matches(text, i, "null")) { spans.Add(new Span(i, 4, CodeViewerHelper.BoolNullColor)); i += 4; continue; }
                if (c == '{' || c == '}' || c == '[' || c == ']') { spans.Add(new Span(i, 1, CodeViewerHelper.BraceColor)); i++; continue; }
                i++;
            }
            return spans;
        }

        private static List<Span> ComputeHtml(string text)
        {
            var spans = new List<Span>();
            if (string.IsNullOrEmpty(text)) return spans;

            foreach (Match m in Regex.Matches(text, @"<!--[\s\S]*?-->"))
                spans.Add(new Span(m.Index, m.Length, CodeViewerHelper.CommentColor));

            foreach (Match m in Regex.Matches(text, @"</?[a-zA-Z][a-zA-Z0-9:-]*"))
                spans.Add(new Span(m.Index, m.Length, CodeViewerHelper.KeyColor));

            foreach (Match m in Regex.Matches(text, "\"[^\"]*\"|'[^']*'"))
                spans.Add(new Span(m.Index, m.Length, CodeViewerHelper.StringColor));

            foreach (Match m in Regex.Matches(text, @"([a-zA-Z-]+)(?==)"))
                spans.Add(new Span(m.Index, m.Length, CodeViewerHelper.AttrNameColor));

            return spans;
        }

        private static List<Span> ComputeCss(string text)
        {
            var spans = new List<Span>();
            if (string.IsNullOrEmpty(text)) return spans;

            foreach (Match m in Regex.Matches(text, @"[.#]?[a-zA-Z][a-zA-Z0-9_-]*(?=\s*\{)"))
                spans.Add(new Span(m.Index, m.Length, CodeViewerHelper.KeyColor));

            foreach (Match m in Regex.Matches(text, "\"[^\"]*\"|'[^']*'"))
                spans.Add(new Span(m.Index, m.Length, CodeViewerHelper.StringColor));

            foreach (Match m in Regex.Matches(text, @"\b\d+(\.\d+)?(px|em|rem|%|s|ms)?\b"))
                spans.Add(new Span(m.Index, m.Length, CodeViewerHelper.NumberColor));

            foreach (Match m in Regex.Matches(text, @"[{}]"))
                spans.Add(new Span(m.Index, m.Length, CodeViewerHelper.BraceColor));

            return spans;
        }

        private static List<Span> ComputeJs(string text)
        {
            var spans = new List<Span>();
            if (string.IsNullOrEmpty(text)) return spans;

            foreach (Match m in Regex.Matches(text, @"//[^\n]*|/\*[\s\S]*?\*/"))
                spans.Add(new Span(m.Index, m.Length, CodeViewerHelper.CommentColor));

            foreach (Match m in Regex.Matches(text, "\"[^\"]*\"|'[^']*'|`[^`]*`"))
                spans.Add(new Span(m.Index, m.Length, CodeViewerHelper.StringColor));

            foreach (Match m in Regex.Matches(text, @"\b\d+(\.\d+)?\b"))
                spans.Add(new Span(m.Index, m.Length, CodeViewerHelper.NumberColor));

            string kwPattern = @"\b(function|return|var|let|const|if|else|for|while|new|this|null|true|false|typeof|class|extends|import|export|default|async|await|try|catch|finally|throw|switch|case|break)\b";
            foreach (Match m in Regex.Matches(text, kwPattern))
                spans.Add(new Span(m.Index, m.Length, CodeViewerHelper.KeywordColor));

            return spans;
        }

        private static bool Matches(string text, int pos, string word)
        {
            if (pos + word.Length > text.Length) return false;
            for (int k = 0; k < word.Length; k++) if (text[pos + k] != word[k]) return false;
            return true;
        }
    }
}
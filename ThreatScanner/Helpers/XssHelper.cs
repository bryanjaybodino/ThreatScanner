using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreatScanner.Helpers
{
    /// <summary>
    /// Shared, passive reflected-XSS probe.
    ///
    /// Both SqlInjectionForm and FullScannerForm can call into this helper so
    /// the detection logic (payload list, encoding check, single-request
    /// probing of URL params and discovered &lt;form&gt; fields) lives in
    /// exactly one place.
    ///
    /// This is a passive reflection check only — for each payload it injects
    /// a marker and looks at whether it comes back raw (unencoded) in the
    /// response. It does not bypass active WAFs/filters with chained
    /// mutation, confirm DOM execution in a real browser, or test
    /// stored/DOM-based/second-order XSS. Different payload *vectors* are
    /// tried (not just encoding tricks on one vector) because an app might
    /// strip "&lt;script&gt;" specifically while still reflecting an
    /// "onerror=" attribute payload raw — relying on a single payload string
    /// (as the old version did) would silently miss that.
    /// </summary>
    public static class XssHelper
    {
        // Kept for backward compatibility with any external callers that
        // still reference the single default payload directly.
        public const string Payload = "<script>xss</script>";
        public const string PayloadEncoded = "&lt;script&gt;xss&lt;/script&gt;";

        // ════════════════════════════════════════════════════════════════════
        //  XSS PAYLOAD LIST (reflected, passive-reflection-check only)
        //  Each payload carries a unique marker token (xssNNNN) so a hit can
        //  be traced back to exactly which vector reflected — useful when
        //  several payloads are tried against the same field and only one
        //  comes back raw. Comments explain what each entry targets/evades.
        // ════════════════════════════════════════════════════════════════════
        public static readonly (string Payload, string Description)[] XssPayloads = {
            // ── Basic tag-based vectors ──────────────────────────────────────
            ("<script>/*xss0001*/</script>",                  "plain script tag — baseline, the most commonly filtered vector"),
            ("<SCRIPT>/*xss0002*/</SCRIPT>",                   "uppercase tag, evades case-sensitive '<script>' string filters"),
            ("<ScRiPt>/*xss0003*/</sCrIpT>",                    "mixed-case tag, evades naive case-sensitive matching"),
            ("<script >/*xss0004*/</script>",                  "extra space before '>', evades exact-string '<script>' filters"),
            ("<script\n>/*xss0005*/</script>",                  "newline inside opening tag, evades single-line regex filters"),
            ("<script/xss0006>",                                "self-closing-style malformed tag, some parsers still execute it"),

            // ── Event-handler vectors (no <script> tag at all) ──────────────
            ("<img src=x onerror=\"/*xss0007*/\">",             "img onerror — fires without any <script> tag, bypasses script-tag-only filters"),
            ("<svg onload=\"/*xss0008*/\">",                    "svg onload — alternate tag entirely, bypasses img/script-specific filters"),
            ("<body onload=\"/*xss0009*/\">",                   "body onload — fires on page load if reflected near <body>"),
            ("<input autofocus onfocus=\"/*xss0010*/\">",       "autofocus+onfocus — fires without user interaction once rendered"),
            ("<select autofocus onfocus=\"/*xss0011*/\">",      "select autofocus+onfocus variant"),
            ("<textarea autofocus onfocus=\"/*xss0012*/\">",    "textarea autofocus+onfocus variant"),
            ("<video src=x onerror=\"/*xss0013*/\">",           "video onerror vector"),
            ("<audio src=x onerror=\"/*xss0014*/\">",           "audio onerror vector"),
            ("<iframe src=\"javascript:/*xss0015*/\">",         "iframe javascript: URI vector"),
            ("<details open ontoggle=\"/*xss0016*/\">",         "details ontoggle — fires automatically when 'open' is present"),
            ("<marquee onstart=\"/*xss0017*/\">",                "marquee onstart — legacy tag, still parsed by many engines"),
            ("<a onmouseover=\"/*xss0018*/\">x</a>",             "anchor onmouseover — requires interaction but commonly missed by filters scanning only script/img"),

            // ── javascript: / data: URI vectors ──────────────────────────────
            ("javascript:/*xss0019*/",                           "bare javascript: URI — for href/src attribute injection contexts"),
            ("JaVaScRiPt:/*xss0020*/",                           "mixed-case javascript: URI, evades case-sensitive scheme filters"),
            ("data:text/html,<script>/*xss0021*/</script>",     "data: URI wrapping a script tag, for iframe/object src contexts"),
            ("&#106;avascript:/*xss0022*/",                      "HTML-entity-encoded leading character of 'javascript:', evades literal scheme-string filters"),

            // ── Attribute-breakout vectors (when input lands inside an attribute) ─
            ("\" onmouseover=\"/*xss0023*/",                    "double-quote attribute breakout into a new event handler"),
            ("' onmouseover='/*xss0024*/",                      "single-quote attribute breakout into a new event handler"),
            ("\"><script>/*xss0025*/</script>",                 "double-quote breakout that closes the tag, then injects a script tag"),
            ("'><script>/*xss0026*/</script>",                  "single-quote breakout that closes the tag, then injects a script tag"),
            ("\" autofocus onfocus=\"/*xss0027*/",              "attribute breakout combined with autofocus, fires without a closing tag"),
            ("` onmouseover=`/*xss0028*/",                      "backtick attribute breakout, for templates/frameworks using backtick-quoted attrs"),

            // ── Encoding / filter-evasion variants of the basic script tag ───
            ("%3Cscript%3E/*xss0029*/%3C/script%3E",            "URL-encoded angle brackets, evades filters expecting literal '<' '>'"),
            ("%253Cscript%253E/*xss0030*/%253C/script%253E",    "double-URL-encoded angle brackets, evades single-decode filters"),
            ("&lt;script&gt;/*xss0031*/&lt;/script&gt;",         "HTML-entity-encoded angle brackets — should NOT execute; included to verify the scanner's encoded-reflection branch is reachable"),
            ("<scr<script>ipt>/*xss0032*/</scr</script>ipt>",   "nested-tag split, defeats naive single-pass '<script>' string-stripping filters"),
            ("<scr\u0000ipt>/*xss0033*/</scr\u0000ipt>",         "embedded null byte inside the tag name, some legacy parsers strip nulls and reconstruct the tag"),
            ("<<script>script>/*xss0034*/<</script>/script>",   "doubled-tag obfuscation, another single-pass-stripping bypass"),

            // ── Polyglot-style vectors (work across multiple injection contexts) ─
            ("'\"><img src=x onerror=\"/*xss0035*/\">",          "combined quote-breakout + img onerror, works whether input lands in single or double quotes"),
            ("javascript:/*-/*`/*\\`/*'/*\"/**/(/* */oNcliCk=/*xss0036*/ )//",
                                                                  "classic XSS polyglot — designed to execute across HTML/attribute/JS-string contexts simultaneously"),

            // ── CSS / style-context vectors ───────────────────────────────────
            ("<style>@import 'javascript:/*xss0037*/';</style>", "CSS @import javascript: URI, for style-context injection"),
            ("<div style=\"background:url(javascript:/*xss0038*/)\">", "inline style background-url javascript: URI"),

            // ── SVG-specific extra vectors ────────────────────────────────────
            ("<svg><script>/*xss0039*/</script></svg>",          "script nested inside svg, bypasses filters that only scan top-level <script>"),
            ("<svg><animate onbegin=\"/*xss0040*/\" attributeName=\"x\" dur=\"1s\">", "svg animate onbegin — fires automatically on render"),

            // ── Meta-refresh / form-action vectors ────────────────────────────
            ("<meta http-equiv=\"refresh\" content=\"0;url=javascript:/*xss0041*/\">", "meta-refresh javascript: redirect vector"),
            ("<form action=\"javascript:/*xss0042*/\"><input type=submit></form>",     "form action javascript: URI vector"),

            // ── Object/embed vectors ──────────────────────────────────────────
            ("<object data=\"javascript:/*xss0043*/\">",         "object data javascript: URI vector"),
            ("<embed src=\"javascript:/*xss0044*/\">",            "embed src javascript: URI vector"),
        };

        /// <summary>How many payloads to probe concurrently per param/field.
        /// Mirrors the SQLi boolean-probe throttle so a long payload list
        /// doesn't burst the target with one request per payload simultaneously.</summary>
        private const int XssProbeConcurrency = 8;

        // ════════════════════════════════════════════════════════════════════
        //  PROBE 1: URL query parameter
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Tries every payload in <see cref="XssPayloads"/> against the given
        /// query param (e.g. "q"), one request per payload, throttled. Each
        /// payload reports its own row via <paramref name="addRow"/>, so a
        /// log line always shows exactly which vector reflected and the
        /// literal value to paste back into a browser to reproduce it.
        /// </summary>
        public static async Task ProbeUrlParamAsync(
            HttpClient http, string url, string param,
            Action<string, string, string> addRow,
            CancellationToken ct = default)
        {
            var throttle = new SemaphoreSlim(XssProbeConcurrency);
            var tasks = XssPayloads.Select(async entry =>
            {
                if (ct.IsCancellationRequested) return;
                await throttle.WaitAsync(ct);
                try
                {
                    string testUrl = (url.Contains("?") ? $"{url}&{param}=" : $"{url}?{param}=")
                        + Uri.EscapeDataString(entry.Payload);

                    string label = $"URL param ({param}) [{entry.Description}]";
                    try
                    {
                        var resp = await http.GetAsync(testUrl, ct);
                        string body = await resp.Content.ReadAsStringAsync();
                        ReportReflection(label, entry.Payload, body, $"{param}={entry.Payload}", addRow);
                    }
                    catch (OperationCanceledException)
                    {
                        addRow(label, "⚠️ Timeout", $"Request to {testUrl} timed out.{Environment.NewLine}  Payload sent: {entry.Payload}");
                    }
                    catch (Exception ex)
                    {
                        addRow(label, "❌ Error", $"{ex.Message}{Environment.NewLine}  Payload sent: {entry.Payload}");
                    }
                }
                finally { throttle.Release(); }
            });

            await Task.WhenAll(tasks);
        }

        // ════════════════════════════════════════════════════════════════════
        //  PROBE 2: HTML forms on the page
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Fetches <paramref name="url"/>, discovers every &lt;form&gt; on the
        /// page, and probes each text-like field against every payload in
        /// <see cref="XssPayloads"/> (keeping all other fields, including
        /// CSRF tokens, at their normal value). Returns the number of
        /// field/payload combinations tested.
        /// </summary>
        public static async Task<int> ProbeFormsAsync(
            HttpClient http, string url,
            Action<string, string, string> addRow,
            Action<string> addSep,
            CancellationToken ct = default)
        {
            string html;
            try
            {
                var resp = await http.GetAsync(url, ct);
                html = await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                addRow("Form Fetch", "❌ Error", ex.Message);
                return 0;
            }

            var forms = FormParsingHelper.ParseForms(html, url);
            if (forms.Count == 0)
            {
                addRow("Forms", "ℹ️ None found", "No <form> elements detected on this page.");
                return 0;
            }

            addRow("Forms", "ℹ️ Found", $"{forms.Count} form(s) detected — testing text-like fields against {XssPayloads.Length} payload vectors each.");

            int fieldsTested = 0;
            foreach (var form in forms)
            {
                if (ct.IsCancellationRequested) break;

                addSep?.Invoke($"Form: {(string.IsNullOrWhiteSpace(form.Name) ? $"#{form.Index + 1}" : form.Name)} (XSS)");

                var textFields = form.Fields
                    .Where(f => !f.IsCsrfToken &&
                                (f.Type == "text" || f.Type == "email" || f.Type == "search" ||
                                 f.Type == "password" || f.Type == "textarea" || f.Type == "tel" ||
                                 f.Type == "url" || string.IsNullOrEmpty(f.Type)))
                    .ToList();

                if (textFields.Count == 0)
                {
                    addRow(form.ToString(), "ℹ️ Skipped", "No text-like fields to test.");
                    continue;
                }

                foreach (var field in textFields)
                {
                    if (ct.IsCancellationRequested) break;
                    await ProbeFormFieldAsync(http, form, field, addRow, ct);
                    fieldsTested += XssPayloads.Length;
                }
            }

            return fieldsTested;
        }

        /// <summary>
        /// Tries every payload in <see cref="XssPayloads"/> against
        /// <paramref name="targetField"/> only, submitting the form via its
        /// real method/action each time, with all other fields held at their
        /// normal value. Reports one row per payload.
        ///
        /// Each request body is built up-front, one payload at a time (cheap,
        /// no I/O) by mutating <paramref name="targetField"/>.Value and
        /// immediately snapshotting + restoring it before any await — the
        /// same mutate→snapshot→restore pattern used in
        /// <c>SqlInjectionHelper.ProbeFormFieldBooleanAsync</c> — so the
        /// actual HTTP requests can then safely fire concurrently afterward.
        /// </summary>
        public static async Task ProbeFormFieldAsync(
            HttpClient http,
            FormParsingHelper.FormInfo form, FormParsingHelper.FormField targetField,
            Action<string, string, string> addRow,
            CancellationToken ct = default)
        {
            var originalValue = targetField.Value;
            bool isGet = form.Method.Equals("GET", StringComparison.OrdinalIgnoreCase);

            async Task<HttpResponseMessage> SendBodyAsync(string fieldValue)
            {
                targetField.Value = (originalValue ?? "") + fieldValue;
                string body = FormParsingHelper.BuildRequestBody(form, includeCsrfTokens: true);
                targetField.Value = originalValue; // restore immediately — single-threaded section only

                if (isGet)
                {
                    string getUrl = form.Action.Contains("?") ? $"{form.Action}&{body}" : $"{form.Action}?{body}";
                    return await http.GetAsync(getUrl, ct);
                }
                else
                {
                    var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                    return await http.PostAsync(form.Action, content, ct);
                }
            }

            var throttle = new SemaphoreSlim(XssProbeConcurrency);
            var tasks = XssPayloads.Select(async entry =>
            {
                if (ct.IsCancellationRequested) return;
                await throttle.WaitAsync(ct);
                string label = $"Form field (XSS): {targetField.Name} [{entry.Description}]";
                try
                {
                    var resp = await SendBodyAsync(entry.Payload);
                    string respBody = await resp.Content.ReadAsStringAsync();
                    ReportReflection(label, entry.Payload, respBody, $"{targetField.Name}={entry.Payload}", addRow);
                }
                catch (OperationCanceledException)
                {
                    addRow(label, "⚠️ Timeout", $"Request to {form.Action} timed out.{Environment.NewLine}  Payload sent: {entry.Payload}");
                }
                catch (Exception ex)
                {
                    addRow(label, "❌ Error", $"{ex.Message}{Environment.NewLine}  Payload sent: {entry.Payload}");
                }
                finally { throttle.Release(); }
            });

            await Task.WhenAll(tasks);
        }

        // ════════════════════════════════════════════════════════════════════
        //  SHARED REFLECTION CHECK
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Checks whether <paramref name="payload"/> comes back raw (likely
        /// exploitable), HTML-encoded (likely safe), or not reflected at all.
        /// The reported row always includes the exact payload string and how
        /// it was sent, so the finding can be reproduced by hand — e.g. by
        /// pasting the same query string/form submission into a browser —
        /// rather than just stating "vulnerable" with no repro steps.
        /// </summary>
        private static void ReportReflection(
            string label, string payload, string body, string sentAs,
            Action<string, string, string> addRow)
        {
            string encodedForm = WebUtility.HtmlEncode(payload);

            if (!string.IsNullOrEmpty(body) && body.Contains(payload))
            {
                addRow(label, "🚨 Reflected (raw)",
                    $"Payload reflected UNENCODED — likely reflected XSS.{Environment.NewLine}" +
                    $"  Payload sent: {sentAs}{Environment.NewLine}" +
                    "  To reproduce: send the same request (same param/field, same value) and view the response — " +
                    "the payload appears verbatim in the HTML/attribute/script context it landed in." + Environment.NewLine +
                    "Fix: HTML-encode all user input on output, set a Content-Security-Policy, " +
                    "and use framework auto-encoding (e.g. Html.Encode / Razor @ syntax) instead of raw writes.");
            }
            else if (!string.IsNullOrEmpty(body) && body.Contains(encodedForm))
            {
                addRow(label, "⚠️ Reflected (encoded)",
                    $"Payload reflected HTML-encoded — likely safe for this vector/context.{Environment.NewLine}" +
                    $"  Payload sent: {sentAs}{Environment.NewLine}" +
                    "Confirm encoding is applied consistently across all output contexts (attributes, inline JS, URLs) — " +
                    "encoding that's correct in one context can still be exploitable in another (e.g. HTML-encoded but unescaped inside a <script> block).");
            }
            else
            {
                addRow(label, "✅ No disclosure",
                    $"Payload not reflected for this vector.{Environment.NewLine}  Payload sent: {sentAs}{Environment.NewLine}" +
                    "Passive check only — does not rule out stored, DOM-based, second-order, or context-specific XSS that this vector didn't happen to trigger.");
            }
        }
    }
}
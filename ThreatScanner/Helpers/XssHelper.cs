using System;
using System.Linq;
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
    /// the detection logic (payload, encoding check, single-request probing
    /// of URL params and discovered &lt;form&gt; fields) lives in exactly one
    /// place.
    ///
    /// This is a passive reflection check only — it injects a simple
    /// &lt;script&gt; marker and looks at whether it comes back raw
    /// (unencoded) in the response. It does not bypass filters/WAFs, chain
    /// payloads, or confirm DOM execution.
    /// </summary>
    public static class XssHelper
    {
        public const string Payload = "<script>xss</script>";
        public const string PayloadEncoded = "&lt;script&gt;xss&lt;/script&gt;";

        // ════════════════════════════════════════════════════════════════════
        //  PROBE 1: URL query parameter
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Appends the XSS marker to the given query param (e.g. "q") and
        /// checks the response for raw vs. encoded reflection. Reports a
        /// single row via <paramref name="addRow"/>: (name, status, response).
        /// </summary>
        public static async Task ProbeUrlParamAsync(
            HttpClient http, string url, string param,
            Action<string, string, string> addRow,
            CancellationToken ct = default)
        {
            string testUrl = (url.Contains("?") ? $"{url}&{param}=" : $"{url}?{param}=") + Uri.EscapeDataString(Payload);

            try
            {
                var resp = await http.GetAsync(testUrl, ct);
                string body = await resp.Content.ReadAsStringAsync();
                ReportReflection($"URL param ({param})", body, addRow);
            }
            catch (OperationCanceledException)
            {
                addRow($"URL param ({param})", "⚠️ Timeout", $"Request to {testUrl} timed out.");
            }
            catch (Exception ex)
            {
                addRow($"URL param ({param})", "❌ Error", ex.Message);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  PROBE 2: HTML forms on the page
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Fetches <paramref name="url"/>, discovers every &lt;form&gt; on the
        /// page, and probes each text-like field one at a time (keeping all
        /// other fields, including CSRF tokens, at their normal value).
        /// Returns the number of fields tested.
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
                    fieldsTested++;
                }
            }

            return fieldsTested;
        }

        /// <summary>
        /// Injects the XSS marker into <paramref name="targetField"/> only,
        /// submits the form via its real method/action, and reports a single
        /// row via <paramref name="addRow"/>.
        /// </summary>
        public static async Task ProbeFormFieldAsync(
            HttpClient http,
            FormParsingHelper.FormInfo form, FormParsingHelper.FormField targetField,
            Action<string, string, string> addRow,
            CancellationToken ct = default)
        {
            var originalValue = targetField.Value;
            targetField.Value = (originalValue ?? "") + Payload;
            string label = $"Form field (XSS): {targetField.Name}";

            try
            {
                HttpResponseMessage resp;
                if (form.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                {
                    string qs = FormParsingHelper.BuildRequestBody(form, includeCsrfTokens: true);
                    string getUrl = form.Action.Contains("?") ? $"{form.Action}&{qs}" : $"{form.Action}?{qs}";
                    resp = await http.GetAsync(getUrl, ct);
                }
                else
                {
                    string body = FormParsingHelper.BuildRequestBody(form, includeCsrfTokens: true);
                    var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                    resp = await http.PostAsync(form.Action, content, ct);
                }

                string respBody = await resp.Content.ReadAsStringAsync();
                ReportReflection(label, respBody, addRow);
            }
            catch (OperationCanceledException)
            {
                addRow(label, "⚠️ Timeout", $"Request to {form.Action} timed out.");
            }
            catch (Exception ex)
            {
                addRow(label, "❌ Error", ex.Message);
            }
            finally
            {
                targetField.Value = originalValue; // restore for next field's probe
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  SHARED REFLECTION CHECK
        // ════════════════════════════════════════════════════════════════════

        private static void ReportReflection(string label, string body, Action<string, string, string> addRow)
        {
            if (!string.IsNullOrEmpty(body) && body.Contains(Payload))
            {
                addRow(label, "🚨 Reflected (raw)",
                    "Payload reflected UNENCODED — likely reflected XSS. " +
                    "Fix: HTML-encode all user input on output, set a Content-Security-Policy, " +
                    "and use framework auto-encoding (e.g. Html.Encode / Razor @ syntax) instead of raw writes.");
            }
            else if (!string.IsNullOrEmpty(body) && body.Contains(PayloadEncoded))
            {
                addRow(label, "⚠️ Reflected (encoded)",
                    "Payload reflected HTML-encoded — likely safe, but confirm encoding is applied consistently across all output contexts (attributes, JS, URLs).");
            }
            else
            {
                addRow(label, "✅ No disclosure",
                    "Payload not reflected. Passive check only — does not rule out stored, DOM-based, or context-specific (attribute/JS) XSS.");
            }
        }
    }
}
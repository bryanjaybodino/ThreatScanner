using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreatScanner.Helpers
{
    /// <summary>
    /// Shared, passive SQL-injection error-disclosure probe.
    ///
    /// Both SqlInjectionForm and FullScannerForm call into this helper so the
    /// detection logic (error signatures, single-request probing of URL params
    /// and discovered &lt;form&gt; fields) lives in exactly one place.
    ///
    /// This is a passive disclosure check only — it appends a single quote to
    /// a parameter/field and looks for a DB error signature in the response.
    /// It does not chain payloads, confirm exploitability, or extract data.
    /// </summary>
    public static class SqlInjectionHelper
    {
        public static readonly string[] ErrorSignatures = {
            "sql syntax", "mysql_fetch", "mysqli_", "you have an error in your sql syntax",
            "microsoft ole db", "odbc sql server driver", "unclosed quotation mark",
            "sqlstate", "incorrect syntax near",
            "ora-00933", "ora-01756", "ora-",
            "pg_query", "warning: pg_", "postgresql query failed",
            "sqlite3.operationalerror", "sqlite_error", "sqlite",
            "syntax error", "unterminated string", "quoted string not properly terminated"
        };

        /// <summary>The default payload appended to params/fields ('). </summary>
        public const string Payload = "'";

        /// <summary>Returns the matched error signature, or null if the body looks clean.</summary>
        public static string FindErrorSignature(string body)
        {
            if (string.IsNullOrEmpty(body)) return null;
            return ErrorSignatures.FirstOrDefault(err =>
                body.IndexOf(err, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        // ════════════════════════════════════════════════════════════════════
        //  PROBE 1: URL query parameter
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Appends a single quote to the given query param (e.g. "id") and
        /// checks the response for a DB error signature. Reports a single
        /// row via <paramref name="addRow"/>: (name, status, response).
        /// </summary>
        public static async Task ProbeUrlParamAsync(
            HttpClient http, string url, string param,
            Action<string, string, string> addRow,
            CancellationToken ct = default)
        {
            string testUrl = url.Contains("?")
                ? $"{url}&{param}={Uri.EscapeDataString("1" + Payload)}"
                : $"{url}?{param}={Uri.EscapeDataString("1" + Payload)}";

            await RunSingleProbeAsync($"URL param ({param})", testUrl, async () =>
            {
                var resp = await http.GetAsync(testUrl, ct);
                return await resp.Content.ReadAsStringAsync();
            }, addRow);
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

            addRow("Forms", "ℹ️ Found", $"{forms.Count} form(s) detected — testing text-like fields one at a time.");

            int fieldsTested = 0;
            foreach (var form in forms)
            {
                if (ct.IsCancellationRequested) break;

                addSep?.Invoke($"Form: {(string.IsNullOrWhiteSpace(form.Name) ? $"#{form.Index + 1}" : form.Name)}");
                addRow("Form Target", "ℹ️ Info",
                    $"{form.Method.ToUpper()} {form.Action} ({form.Fields.Count} fields, framework: {form.Framework})");

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
        /// Injects the probe payload into <paramref name="targetField"/> only,
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
            string label = $"Form field: {targetField.Name}";

            try
            {
                await RunSingleProbeAsync(label, form.Action, async () =>
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
                    return await resp.Content.ReadAsStringAsync();
                }, addRow);
            }
            finally
            {
                targetField.Value = originalValue; // restore for next field's probe
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  SHARED SINGLE-PROBE LOGIC
        // ════════════════════════════════════════════════════════════════════

        private static async Task RunSingleProbeAsync(
            string label, string requestDescription, Func<Task<string>> sendRequest,
            Action<string, string, string> addRow)
        {
            try
            {
                string body = await sendRequest();
                var matched = FindErrorSignature(body);

                if (matched != null)
                {
                    addRow(label, "🚨 Possible SQLi",
                        $"DB error signature \"{matched}\" found. Likely unsanitized input reaching a SQL query. " +
                        "Fix: use parameterized queries / prepared statements, never concatenate user input into SQL; " +
                        "disable detailed error pages in production.");
                }
                else
                {
                    addRow(label, "✅ No disclosure",
                        "No known DB error patterns found. Passive check only — does not rule out blind/second-order injection.");
                }
            }
            catch (OperationCanceledException)
            {
                addRow(label, "⚠️ Timeout", $"Request to {requestDescription} timed out.");
            }
            catch (Exception ex)
            {
                addRow(label, "❌ Error", ex.Message);
            }
        }
    }
}
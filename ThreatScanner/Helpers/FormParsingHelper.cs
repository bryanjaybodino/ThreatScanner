using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ThreatScanner.Helpers
{
    /// <summary>
    /// FormParsingHelper - Automatically parse HTML forms and generate request bodies
    /// Supports: Plain HTML, ASP.NET MVC, ASP.NET WebForms, PHP
    /// </summary>
    public static class FormParsingHelper
    {
        public class FormInfo
        {
            public int Index { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string Action { get; set; }
            public string Method { get; set; } = "POST";
            public List<FormField> Fields { get; set; } = new List<FormField>();
            public string Framework { get; set; } = "Unknown";
            public bool HasCsrfToken { get; set; }
            public string CsrfTokenFieldName { get; set; }

            public override string ToString()
            {
                return $"{(string.IsNullOrWhiteSpace(Name) ? "(unnamed)" : Name)} - {Method.ToUpper()} {Action} ({Fields.Count} fields)";
            }
        }

        public class FormField
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
            public List<string> Options { get; set; } = new List<string>();
            public bool Required { get; set; }
            public bool IsCsrfToken { get; set; }

            public override string ToString()
            {
                return $"{Name} ({Type}) = {Value}";
            }
        }

        /// <summary>
        /// Parse HTML and extract all forms with their fields
        /// </summary>
        public static List<FormInfo> ParseForms(string html, string baseUrl = "")
        {
            var forms = new List<FormInfo>();

            if (string.IsNullOrWhiteSpace(html))
                return forms;

            string framework = DetectFramework(html);
            var formMatches = Regex.Matches(html, @"<form\s+([^>]*)>(.*?)</form>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            int formIndex = 0;
            foreach (Match formMatch in formMatches)
            {
                try
                {
                    string formAttrs = formMatch.Groups[1].Value;
                    string formContent = formMatch.Groups[2].Value;

                    var form = new FormInfo
                    {
                        Index = formIndex,
                        Framework = framework,
                        Action = ExtractAttr(formAttrs, "action"),
                        Method = ExtractAttr(formAttrs, "method") ?? "POST",
                        Name = ExtractAttr(formAttrs, "name"),
                        Id = ExtractAttr(formAttrs, "id")
                    };

                    // Make action absolute URL if needed
                    if (!string.IsNullOrWhiteSpace(form.Action) && !form.Action.StartsWith("http"))
                    {
                        var baseUri = new Uri(baseUrl ?? "http://localhost");
                        form.Action = new Uri(baseUri, form.Action).ToString();
                    }
                    else if (string.IsNullOrWhiteSpace(form.Action))
                    {
                        form.Action = baseUrl;
                    }

                    // Parse fields
                    ExtractInputFields(formContent, form);
                    ExtractSelectFields(formContent, form);
                    ExtractTextareaFields(formContent, form);

                    // Auto-fill values
                    foreach (var field in form.Fields)
                    {
                        field.Value = GenerateAutoFilledValue(field, framework);

                        // Check if CSRF token
                        if (field.Name.IndexOf("csrf", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            field.Name.IndexOf("token", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            field.Name.IndexOf("nonce", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            field.Name.IndexOf("__requestverificationtoken", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            field.IsCsrfToken = true;
                            form.HasCsrfToken = true;
                            form.CsrfTokenFieldName = field.Name;
                        }
                    }

                    forms.Add(form);
                    formIndex++;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error parsing form: {ex.Message}");
                }
            }

            return forms;
        }

        /// <summary>
        /// Extract input fields from form HTML
        /// </summary>
        private static void ExtractInputFields(string formContent, FormInfo form)
        {
            var inputMatches = Regex.Matches(formContent, @"<input\s+([^>]*)>", RegexOptions.IgnoreCase);
            foreach (Match inputMatch in inputMatches)
            {
                string inputAttrs = inputMatch.Groups[1].Value;
                var field = new FormField
                {
                    Name = ExtractAttr(inputAttrs, "name"),
                    Type = ExtractAttr(inputAttrs, "type") ?? "text",
                    Value = ExtractAttr(inputAttrs, "value"),
                    Required = inputAttrs.IndexOf("required", StringComparison.OrdinalIgnoreCase) >= 0
                };

                if (!string.IsNullOrWhiteSpace(field.Name))
                    form.Fields.Add(field);
            }
        }

        /// <summary>
        /// Extract select fields from form HTML
        /// </summary>
        private static void ExtractSelectFields(string formContent, FormInfo form)
        {
            var selectMatches = Regex.Matches(formContent, @"<select\s+([^>]*)>(.*?)</select>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match selectMatch in selectMatches)
            {
                string selectAttrs = selectMatch.Groups[1].Value;
                string selectContent = selectMatch.Groups[2].Value;
                var field = new FormField
                {
                    Name = ExtractAttr(selectAttrs, "name"),
                    Type = "select",
                    Required = selectAttrs.IndexOf("required", StringComparison.OrdinalIgnoreCase) >= 0
                };

                var optionMatches = Regex.Matches(selectContent, @"<option\s+([^>]*)>(.*?)</option>", RegexOptions.IgnoreCase);
                foreach (Match optionMatch in optionMatches)
                {
                    string optionAttrs = optionMatch.Groups[1].Value;
                    string optionText = optionMatch.Groups[2].Value.Trim();
                    string optionValue = ExtractAttr(optionAttrs, "value") ?? optionText;
                    field.Options.Add(optionValue);
                }

                if (!string.IsNullOrWhiteSpace(field.Name) && field.Options.Count > 0)
                {
                    field.Value = field.Options[0];
                    form.Fields.Add(field);
                }
            }
        }

        /// <summary>
        /// Extract textarea fields from form HTML
        /// </summary>
        private static void ExtractTextareaFields(string formContent, FormInfo form)
        {
            var textareaMatches = Regex.Matches(formContent, @"<textarea\s+([^>]*)>(.*?)</textarea>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match textareaMatch in textareaMatches)
            {
                string textareaAttrs = textareaMatch.Groups[1].Value;
                string textareaValue = textareaMatch.Groups[2].Value;
                var field = new FormField
                {
                    Name = ExtractAttr(textareaAttrs, "name"),
                    Type = "textarea",
                    Value = textareaValue.Trim(),
                    Required = textareaAttrs.IndexOf("required", StringComparison.OrdinalIgnoreCase) >= 0
                };

                if (!string.IsNullOrWhiteSpace(field.Name))
                    form.Fields.Add(field);
            }
        }

        /// <summary>
        /// Detect web framework from HTML
        /// </summary>
        public static string DetectFramework(string html)
        {
            if (html.IndexOf("__RequestVerificationToken", StringComparison.OrdinalIgnoreCase) >= 0 ||
                html.IndexOf("__ViewBag", StringComparison.OrdinalIgnoreCase) >= 0 ||
                html.IndexOf("__ViewData", StringComparison.OrdinalIgnoreCase) >= 0)
                return "ASP.NET MVC/Core";

            if (html.IndexOf("__VIEWSTATE", StringComparison.OrdinalIgnoreCase) >= 0 ||
                html.IndexOf("__EVENTVALIDATION", StringComparison.OrdinalIgnoreCase) >= 0)
                return "ASP.NET WebForms";

            if (html.IndexOf("<?php", StringComparison.OrdinalIgnoreCase) >= 0 ||
                html.IndexOf("_SESSION", StringComparison.OrdinalIgnoreCase) >= 0)
                return "PHP";

            return "Plain HTML";
        }

        /// <summary>
        /// Generate auto-filled test value for a form field
        /// </summary>
        public static string GenerateAutoFilledValue(FormField field, string framework)
        {
            if (!string.IsNullOrWhiteSpace(field.Value) && !field.Value.StartsWith("["))
                return field.Value;

            string nameUpper = field.Name.ToUpper();

            // CSRF/Token fields
            if (nameUpper.Contains("CSRF") || nameUpper.Contains("TOKEN") ||
                nameUpper.Contains("NONCE") || nameUpper.Contains("VERIFICATION") ||
                nameUpper.Contains("ANTIFORGERY"))
                return "[CSRF_TOKEN_HERE]";

            if (nameUpper.Contains("__VIEWSTATE"))
                return "[VIEWSTATE_HERE]";

            if (nameUpper.Contains("__EVENTVALIDATION"))
                return "[EVENTVALIDATION_HERE]";

            // User/Auth
            if (nameUpper.Contains("USER") || nameUpper.Contains("LOGIN") ||
                nameUpper.Contains("USERNAME") || nameUpper.Contains("UNAME"))
                return "testuser";

            if (nameUpper.Contains("EMAIL"))
                return "test@example.com";

            // Password
            if (nameUpper.Contains("PASS") || nameUpper.Contains("PWD"))
                return "TestPass123!";

            // Dropdown/Select - first option
            if (field.Type == "select" && field.Options.Count > 0)
                return field.Options[0];

            // Hidden
            if (field.Type == "hidden")
                return field.Value ?? "";

            // Checkbox/Radio
            if (field.Type == "checkbox" || field.Type == "radio")
                return "on";

            // Name
            if (nameUpper.Contains("FNAME") || nameUpper.Contains("FIRST"))
                return "John";

            if (nameUpper.Contains("LNAME") || nameUpper.Contains("LAST"))
                return "Doe";

            if (nameUpper.Contains("NAME") || nameUpper.Contains("FULLNAME") ||
                nameUpper.Contains("TITLE"))
                return "John Doe";

            // Phone
            if (nameUpper.Contains("PHONE") || nameUpper.Contains("MOBILE") ||
                nameUpper.Contains("TEL"))
                return "+1234567890";

            // Address
            if (nameUpper.Contains("ADDRESS") || nameUpper.Contains("STREET"))
                return "123 Test Street";

            if (nameUpper.Contains("CITY") || nameUpper.Contains("TOWN"))
                return "TestCity";

            if (nameUpper.Contains("COUNTRY") || nameUpper.Contains("STATE") ||
                nameUpper.Contains("PROVINCE"))
                return "US";

            if (nameUpper.Contains("ZIP") || nameUpper.Contains("POSTAL") ||
                nameUpper.Contains("CODE"))
                return "12345";

            // Financial
            if (nameUpper.Contains("AMOUNT") || nameUpper.Contains("PRICE") ||
                nameUpper.Contains("VALUE") || nameUpper.Contains("TOTAL"))
                return "100.00";

            // Text content
            if (nameUpper.Contains("DESCRIPTION") || nameUpper.Contains("COMMENT") ||
                nameUpper.Contains("NOTES") || nameUpper.Contains("MESSAGE"))
                return "Test comment";

            // URL
            if (nameUpper.Contains("URL") || nameUpper.Contains("LINK") ||
                nameUpper.Contains("WEBSITE"))
                return "https://example.com";

            // Default
            return "test";
        }

        /// <summary>
        /// Build URL-encoded form body from fields
        /// </summary>
        public static string BuildRequestBody(FormInfo form, bool includeCsrfTokens = false)
        {
            var bodyParts = new List<string>();

            foreach (var field in form.Fields)
            {
                // Skip CSRF tokens unless explicitly included
                if (!includeCsrfTokens && field.IsCsrfToken)
                    continue;

                if (string.IsNullOrWhiteSpace(field.Value))
                    bodyParts.Add($"{Uri.EscapeDataString(field.Name)}=");
                else
                    bodyParts.Add($"{Uri.EscapeDataString(field.Name)}={Uri.EscapeDataString(field.Value)}");
            }

            return string.Join("&", bodyParts);
        }

        /// <summary>
        /// Build JSON body for API endpoints
        /// </summary>
        public static string BuildJsonBody(FormInfo form, bool includeCsrfTokens = false)
        {
            var dict = new Dictionary<string, object>();

            foreach (var field in form.Fields)
            {
                if (!includeCsrfTokens && field.IsCsrfToken)
                    continue;

                dict[field.Name] = field.Value ?? "";
            }

            return System.Text.Json.JsonSerializer.Serialize(dict);
        }

        /// <summary>
        /// Extract attribute value from HTML tag
        /// </summary>
        private static string ExtractAttr(string tag, string attr)
        {
            foreach (string quote in new[] { "\"", "'" })
            {
                string pattern = $"{attr}={quote}";
                int start = tag.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
                if (start >= 0)
                {
                    start += pattern.Length;
                    int end = tag.IndexOf(quote, start);
                    if (end >= 0) return tag.Substring(start, end - start);
                }
            }

            var m = Regex.Match(tag, $@"{attr}=([^\s>]+)", RegexOptions.IgnoreCase);
            return m.Success ? m.Groups[1].Value : "";
        }

        /// <summary>
        /// Get summary of detected forms
        /// </summary>
        public static string GetFormsSummary(List<FormInfo> forms)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Found {forms.Count} form(s):\n");

            foreach (var form in forms)
            {
                sb.AppendLine($"Form #{form.Index + 1}: {form}");
                sb.AppendLine($"  Framework: {form.Framework}");
                sb.AppendLine($"  CSRF Token: {(form.HasCsrfToken ? "✅ YES (" + form.CsrfTokenFieldName + ")" : "❌ NO")}");

                foreach (var field in form.Fields.Take(5))
                {
                    sb.AppendLine($"    • {field}");
                }

                if (form.Fields.Count > 5)
                    sb.AppendLine($"    ... and {form.Fields.Count - 5} more fields");

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
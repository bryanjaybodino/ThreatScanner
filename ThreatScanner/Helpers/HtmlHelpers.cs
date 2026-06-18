using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace ThreatScanner.Helpers
{
    /// <summary>
    /// HTML parsing and login-detection helpers shared by BruteForceForm.
    /// </summary>
    public static class HtmlHelpers
    {
        // ─── FRAMEWORK DETECTION ─────────────────────────────────────────────────

        public enum LoginFramework { AspNetWebForms, PhpOrHtml, Generic }

        public static LoginFramework DetectFramework(string html)
        {
            if (html.IndexOf("__VIEWSTATE", StringComparison.OrdinalIgnoreCase) >= 0)
                return LoginFramework.AspNetWebForms;
            if (html.IndexOf("<form", StringComparison.OrdinalIgnoreCase) >= 0)
                return LoginFramework.PhpOrHtml;
            return LoginFramework.Generic;
        }

        // ─── FIELD PARSING ────────────────────────────────────────────────────────

        public static string ParseAttr(string tag, string attr)
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
            return "";
        }

        public static string ExtractHiddenField(string html, string fieldName)
        {
            string pattern = $"id=\"{fieldName}\" value=\"";
            int start = html.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
            if (start < 0) return "";
            start += pattern.Length;
            int end = html.IndexOf("\"", start);
            return end < 0 ? "" : html.Substring(start, end - start);
        }

        public static Dictionary<string, string> ExtractAllHiddenFields(string html)
        {
            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string lower = html.ToLowerInvariant();
            int search = 0;
            while (true)
            {
                int inputPos = lower.IndexOf("<input", search);
                if (inputPos < 0) break;
                int closePos = lower.IndexOf(">", inputPos);
                if (closePos < 0) break;
                string tag = html.Substring(inputPos, closePos - inputPos + 1);
                if (tag.IndexOf("type=\"hidden\"", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    tag.IndexOf("type='hidden'", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    string name = ParseAttr(tag, "name");
                    string value = ParseAttr(tag, "value");
                    if (!string.IsNullOrEmpty(name))
                        fields[name] = value;
                }
                search = closePos + 1;
            }
            return fields;
        }

        public static string DetectEventTarget(string html)
        {
            string lower = html.ToLowerInvariant();
            string marker = "__dopostback('";
            int search = 0;
            string firstTarget = "", loginTarget = "", submitTarget = "";

            while (true)
            {
                int pos = lower.IndexOf(marker, search);
                if (pos < 0) break;
                int start = pos + marker.Length;
                int end = html.IndexOf("'", start);
                if (end <= start) break;
                string target = html.Substring(start, end - start);

                if (string.IsNullOrEmpty(firstTarget)) firstTarget = target;
                string tLow = target.ToLowerInvariant();
                if (tLow.Contains("login") && string.IsNullOrEmpty(loginTarget)) loginTarget = target;
                if (tLow.Contains("submit") && string.IsNullOrEmpty(submitTarget)) submitTarget = target;
                search = end + 1;
            }

            if (!string.IsNullOrEmpty(loginTarget)) return loginTarget;
            if (!string.IsNullOrEmpty(submitTarget)) return submitTarget;

            int btnPos = html.IndexOf("type=\"submit\"", StringComparison.OrdinalIgnoreCase);
            if (btnPos < 0) btnPos = html.IndexOf("type='submit'", StringComparison.OrdinalIgnoreCase);
            if (btnPos >= 0)
            {
                int tagStart = html.LastIndexOf("<input", btnPos, StringComparison.OrdinalIgnoreCase);
                if (tagStart >= 0)
                {
                    int tagEnd = html.IndexOf(">", btnPos);
                    string tag = tagEnd > 0 ? html.Substring(tagStart, tagEnd - tagStart) : "";
                    string name = ParseAttr(tag, "name");
                    if (!string.IsNullOrEmpty(name)) return name;
                }
            }

            return firstTarget;
        }

        public static (string userField, string passField) AutoDetectLoginFields(string html)
        {
            string lower = html.ToLowerInvariant();
            int search = 0;
            string detectedPass = "";
            string lastText = "";

            while (true)
            {
                int inputPos = lower.IndexOf("<input", search);
                if (inputPos < 0) break;
                int closePos = lower.IndexOf(">", inputPos);
                if (closePos < 0) break;

                string tag = html.Substring(inputPos, closePos - inputPos + 1);
                string typeVal = ParseAttr(tag, "type").ToLowerInvariant();
                string nameVal = ParseAttr(tag, "name");

                if (!string.IsNullOrEmpty(nameVal))
                {
                    if (typeVal == "password") { detectedPass = nameVal; break; }
                    else if (typeVal == "text" || typeVal == "email") lastText = nameVal;
                }
                search = closePos + 1;
            }

            return (lastText, detectedPass);
        }

        public static string ExtractFormAction(string html)
        {
            int formPos = html.IndexOf("<form", StringComparison.OrdinalIgnoreCase);
            if (formPos < 0) return "";

            foreach (string quote in new[] { "\"", "'" })
            {
                string pattern = $"action={quote}";
                int start = html.IndexOf(pattern, formPos, StringComparison.OrdinalIgnoreCase);
                if (start >= 0)
                {
                    start += pattern.Length;
                    int end = html.IndexOf(quote, start);
                    if (end > start)
                        return html.Substring(start, end - start);
                }
            }
            return "";
        }

        public static string BuildPostUrl(string baseUrl, string action)
        {
            if (string.IsNullOrEmpty(action)) return baseUrl;
            try { return new Uri(new Uri(baseUrl), action).ToString(); }
            catch { return baseUrl; }
        }

        // ─── LOGIN RESULT DETECTION ───────────────────────────────────────────────

        public static bool IsLoginSuccess(string body, int statusCode, Uri loginUri,
            HttpResponseMessage response, LoginFramework framework)
        {
            string lower = body.ToLowerInvariant();

            if (response.Headers.Contains("Set-Cookie"))
            {
                var cookies = response.Headers.GetValues("Set-Cookie").ToList();
                if (cookies.Count(c => c.ToLower().Contains("expires=") || c.ToLower().Contains("max-age=")) >= 2)
                    return true;
            }

            if ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
            {
                string location = response.Headers.Location?.ToString() ?? "";
                if (!location.ToLower().Contains("login")) return true;
            }

            if ((lower.Contains("_direct(") || lower.Contains("window.location") ||
                 lower.Contains("location.href") || lower.Contains("location.replace")) &&
                !lower.Contains("login"))
                return true;

            string[] successKeywords = { "dashboard", "logout", "sign out", "signout",
                                         "welcome", "my account", "profile", "home page" };
            if (successKeywords.Any(k => lower.Contains(k))) return true;

            bool stillHasPasswordField = lower.Contains("type=\"password\"") ||
                                         lower.Contains("type='password'");
            return !stillHasPasswordField;
        }

        public static bool IsLoginFailure(string body, LoginFramework framework)
        {
            string lower = body.ToLowerInvariant();
            string[] failKeywords = {
                "invalid password", "invalid username", "invalid credentials",
                "incorrect password", "incorrect username", "wrong password", "wrong username",
                "login failed", "authentication failed", "bad credentials", "unauthorized",
                "try again", "not match", "invalid login"
            };
            return failKeywords.Any(k => lower.Contains(k));
        }
    }
}
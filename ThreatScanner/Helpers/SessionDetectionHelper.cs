using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace ThreatScanner.Helpers
{
    /// <summary>
    /// SessionDetectionHelper
    /// Auto-detects session/CSRF cookie types across common frameworks
    /// (PHP, ASP.NET WebForms, ASP.NET MVC/Core, Django, Rails, Express/Node,
    /// Laravel, generic JWT-in-cookie setups) from either:
    ///   - a raw "Cookie:" header string (pasted from browser DevTools), or
    ///   - Set-Cookie response headers captured by HttpClient itself.
    /// No manual cookie-name guessing required — paste once, classify automatically.
    /// </summary>
    public static class SessionDetectionHelper
    {
        public class DetectedCookie
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public string Kind { get; set; }       // "Session" | "CSRF" | "Auth" | "Other"
            public string Framework { get; set; }   // best-guess framework this cookie belongs to
        }

        // Known session-cookie name patterns -> framework
        private static readonly (string Pattern, string Framework, string Kind)[] KnownCookies = new[]
        {
            ("PHPSESSID",                     "PHP",                 "Session"),
            ("ASP.NET_SessionId",             "ASP.NET WebForms",    "Session"),
            (".AspNet.ApplicationCookie",     "ASP.NET MVC (Legacy)","Auth"),
            (".AspNetCore.Identity.Application","ASP.NET Core",      "Auth"),
            (".AspNetCore.Session",           "ASP.NET Core",        "Session"),
            ("__RequestVerificationToken",    "ASP.NET MVC/Core",    "CSRF"),
            ("XSRF-TOKEN",                     "ASP.NET Core / Angular","CSRF"),
            ("sessionid",                      "Django",             "Session"),
            ("csrftoken",                      "Django",             "CSRF"),
            ("_session_id",                    "Ruby on Rails",       "Session"),
            ("_csrf",                          "Express/Node",        "CSRF"),
            ("connect.sid",                    "Express/Node",        "Session"),
            ("laravel_session",                "PHP Laravel",         "Session"),
            ("XSRF-TOKEN",                      "PHP Laravel",        "CSRF"),
            ("JSESSIONID",                      "Java (Spring/JSP)",  "Session"),
            ("rack.session",                   "Ruby (Rack)",         "Session"),
        };

        /// <summary>
        /// Parse a raw "Cookie:" header string (e.g. copy-pasted from DevTools)
        /// into classified cookies, auto-detecting framework and kind per cookie.
        /// </summary>
        public static List<DetectedCookie> ParseCookieHeader(string rawHeader)
        {
            var results = new List<DetectedCookie>();
            if (string.IsNullOrWhiteSpace(rawHeader)) return results;

            // Strip a leading "Cookie:" if the user pasted the whole header line
            rawHeader = Regex.Replace(rawHeader.Trim(), @"^Cookie:\s*", "", RegexOptions.IgnoreCase);

            foreach (var part in rawHeader.Split(';'))
            {
                var kv = part.Trim().Split(new[] { '=' }, 2);
                if (kv.Length != 2 || string.IsNullOrWhiteSpace(kv[0])) continue;

                string name = kv[0].Trim();
                string value = kv[1].Trim();

                results.Add(Classify(name, value));
            }

            return results;
        }

        /// <summary>
        /// Classify cookies straight from an HttpResponseMessage's Set-Cookie headers
        /// (used right after a login POST the tool performs itself).
        /// </summary>
        public static List<DetectedCookie> ParseSetCookieHeaders(IEnumerable<string> setCookieHeaders)
        {
            var results = new List<DetectedCookie>();
            if (setCookieHeaders == null) return results;

            foreach (var header in setCookieHeaders)
            {
                // Set-Cookie: name=value; Path=/; HttpOnly; ...
                string firstPair = header.Split(';')[0];
                var kv = firstPair.Split(new[] { '=' }, 2);
                if (kv.Length != 2) continue;

                results.Add(Classify(kv[0].Trim(), kv[1].Trim()));
            }

            return results;
        }

        private static DetectedCookie Classify(string name, string value)
        {
            foreach (var known in KnownCookies)
            {
                if (name.Equals(known.Pattern, StringComparison.OrdinalIgnoreCase) ||
                    name.IndexOf(known.Pattern, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return new DetectedCookie
                    {
                        Name = name,
                        Value = value,
                        Framework = known.Framework,
                        Kind = known.Kind
                    };
                }
            }

            // Heuristic fallback for unknown cookie names
            string kind = "Other";
            string lower = name.ToLowerInvariant();
            if (lower.Contains("sess"))
                kind = "Session";
            else if (lower.Contains("csrf") || lower.Contains("xsrf") || lower.Contains("token"))
                kind = "CSRF";
            else if (lower.Contains("auth") || lower.Contains("login") || lower.Contains("identity"))
                kind = "Auth";

            // JWT cookies are recognizable by structure: header.payload.signature (base64url x3)
            if (Regex.IsMatch(value, @"^[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+$"))
                kind = "Auth (JWT)";

            return new DetectedCookie
            {
                Name = name,
                Value = value,
                Framework = "Unknown",
                Kind = kind
            };
        }

        /// <summary>
        /// Inject all detected cookies into a CookieContainer for the given target URL.
        /// </summary>
        public static void ApplyToJar(CookieContainer jar, Uri targetUri, List<DetectedCookie> cookies)
        {
            foreach (var c in cookies)
            {
                try { jar.Add(targetUri, new Cookie(c.Name, c.Value)); }
                catch { /* skip malformed cookie, don't abort the whole batch */ }
            }
        }

        /// <summary>
        /// Auto-detect the username/password field names on a login form already
        /// parsed by FindBestForm/ParseForms, so the caller doesn't have to know
        /// the field names in advance (they differ per framework/app).
        /// </summary>
        public static bool TryGuessLoginFieldNames(IEnumerable<string> fieldNames,
            out string userField, out string passField)
        {
            userField = null;
            passField = null;

            var names = fieldNames.ToList();

            var userCandidates = new[] { "username", "user", "email", "login", "uname", "userid" };
            var passCandidates = new[] { "password", "pass", "pwd", "passwd" };

            foreach (var cand in userCandidates)
            {
                userField = names.FirstOrDefault(n => n.IndexOf(cand, StringComparison.OrdinalIgnoreCase) >= 0);
                if (userField != null) break;
            }

            foreach (var cand in passCandidates)
            {
                passField = names.FirstOrDefault(n => n.IndexOf(cand, StringComparison.OrdinalIgnoreCase) >= 0);
                if (passField != null) break;
            }

            return userField != null && passField != null;
        }

        /// <summary>
        /// Human-readable summary for the output log.
        /// </summary>
        public static string Summarize(List<DetectedCookie> cookies)
        {
            if (cookies.Count == 0) return "No cookies detected.";

            var lines = new List<string> { $"Detected {cookies.Count} cookie(s):" };
            foreach (var c in cookies)
            {
                string icon;
                switch (c.Kind)
                {
                    case "Session": icon = "🔐"; break;
                    case "CSRF": icon = "🔑"; break;
                    case "Auth": icon = "🪪"; break;
                    case "Auth (JWT)": icon = "🪪"; break;
                    default: icon = "🍪"; break;
                }
                lines.Add($"  {icon} {c.Name}  →  {c.Kind} ({c.Framework})");
            }
            return string.Join("\r\n", lines);
        }
    }
}
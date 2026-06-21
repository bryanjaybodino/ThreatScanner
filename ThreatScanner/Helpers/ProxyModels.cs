// ProxyModels.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ThreatScanner.Helpers
{
    /// <summary>
    /// Header names commonly used by apps/tracing frameworks to correlate a
    /// request across services. We scan for these (case-insensitive) so the
    /// proxy log can surface a "Correlation Id" column without the user
    /// having to know which header their stack happens to use.
    /// </summary>
    public static class CorrelationHeaders
    {
        public static readonly string[] KnownNames =
        {
            "x-correlation-id", "x-request-id", "x-trace-id", "correlation-id",
            "request-id", "trace-id", "traceparent", "x-amzn-trace-id"
        };

        public static string Extract(IEnumerable<KeyValuePair<string, string>> headers)
        {
            if (headers == null) return null;
            foreach (var h in headers)
                if (Array.IndexOf(KnownNames, h.Key.ToLowerInvariant()) >= 0)
                    return h.Value;
            return null;
        }
    }

    /// <summary>
    /// One captured request/response pair, plus everything the proxy log
    /// view needs to render it (grid row, detail tabs, replay, exports).
    /// </summary>
    public class ProxyEntry
    {
        // ── Identity ──────────────────────────────────────────────────────────
        public string Id { get; } = Guid.NewGuid().ToString("N");
        public DateTime Time { get; set; } = DateTime.Now;

        // ── Request ───────────────────────────────────────────────────────────
        public string Method { get; set; } = "";
        public string Url { get; set; } = "";
        public string ResourceType { get; set; } = "";
        public List<KeyValuePair<string, string>> RequestHeaders { get; set; } = new List<KeyValuePair<string, string>>();
        public string RequestBody { get; set; } = "";

        // ── Response ──────────────────────────────────────────────────────────
        public int Status { get; set; } = 0;          // 0 = pending
        public string StatusText { get; set; } = "";
        public string ResponseContentType { get; set; } = "";
        public List<KeyValuePair<string, string>> ResponseHeaders { get; set; } = new List<KeyValuePair<string, string>>();
        public string ResponseBody { get; set; } = "";
        public long SizeBytes { get; set; } = 0;

        // ── Meta ──────────────────────────────────────────────────────────────
        public string CorrelationId { get; set; }
        public string Error { get; set; }              // set when the request failed entirely
        public double? DurationMs { get; set; }
        public Microsoft.Playwright.RequestTimingResult Timing { get; set; }

        // ── UI wiring (set by the form once the row exists) ─────────────────────
        public DataGridViewRow GridRow { get; set; }

        public bool IsError => Error != null || Status >= 400;
        public bool IsPending => Status == 0 && Error == null;

        /// <summary>Cheap full-text search target: url + headers + bodies.</summary>
        public bool MatchesSearch(string term)
        {
            if (string.IsNullOrEmpty(term)) return true;
            term = term.ToLowerInvariant();

            if (Url.ToLowerInvariant().Contains(term)) return true;
            if (!string.IsNullOrEmpty(RequestBody) && RequestBody.ToLowerInvariant().Contains(term)) return true;
            if (!string.IsNullOrEmpty(ResponseBody) && ResponseBody.ToLowerInvariant().Contains(term)) return true;
            if (RequestHeaders.Any(h => h.Key.ToLowerInvariant().Contains(term) || h.Value.ToLowerInvariant().Contains(term))) return true;
            if (ResponseHeaders.Any(h => h.Key.ToLowerInvariant().Contains(term) || h.Value.ToLowerInvariant().Contains(term))) return true;
            return false;
        }
    }

    /// <summary>
    /// Turns a captured <see cref="ProxyEntry"/> back into copy-pasteable
    /// reproduction commands, for when a developer wants to re-run a request
    /// outside the browser.
    /// </summary>
    public static class RequestExporter
    {
        // Headers a browser sets automatically and that curl/HttpClient will
        // set themselves (or that are illegal to set manually) — including
        // them verbatim just adds noise/errors to the generated snippet.
        private static readonly HashSet<string> SkipHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "host", "content-length", "connection", "accept-encoding", ":method", ":path", ":scheme", ":authority"
        };

        public static string ToCurl(ProxyEntry entry)
        {
            var sb = new StringBuilder();
            sb.Append("curl -X ").Append(entry.Method);
            sb.Append(" \"").Append(entry.Url).Append("\"");

            foreach (var h in entry.RequestHeaders)
            {
                if (SkipHeaders.Contains(h.Key) || h.Key.StartsWith(":")) continue;
                sb.Append(" \\\n  -H \"").Append(h.Key).Append(": ").Append(EscapeForShell(h.Value)).Append("\"");
            }

            if (!string.IsNullOrEmpty(entry.RequestBody))
                sb.Append(" \\\n  --data-raw \"").Append(EscapeForShell(entry.RequestBody)).Append("\"");

            return sb.ToString();
        }

        public static string ToHttpClientSnippet(ProxyEntry entry)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using var client = new HttpClient();");
            sb.AppendLine($"using var request = new HttpRequestMessage(HttpMethod.{Capitalize(entry.Method)}, \"{entry.Url}\");");

            bool hasContentHeaders = false;
            var contentHeaderNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "content-type", "content-length" };

            foreach (var h in entry.RequestHeaders)
            {
                if (SkipHeaders.Contains(h.Key) || h.Key.StartsWith(":")) continue;
                if (contentHeaderNames.Contains(h.Key)) { hasContentHeaders = true; continue; } // applied to Content below
                sb.AppendLine($"request.Headers.TryAddWithoutValidation(\"{h.Key}\", \"{EscapeForCSharp(h.Value)}\");");
            }

            if (!string.IsNullOrEmpty(entry.RequestBody))
            {
                string contentType = entry.RequestHeaders
                    .FirstOrDefault(h => h.Key.Equals("content-type", StringComparison.OrdinalIgnoreCase)).Value
                    ?? "application/json";

                sb.AppendLine($"request.Content = new StringContent(\"{EscapeForCSharp(entry.RequestBody)}\", System.Text.Encoding.UTF8, \"{contentType}\");");
            }

            sb.AppendLine("using var response = await client.SendAsync(request);");
            sb.AppendLine("string body = await response.Content.ReadAsStringAsync();");
            sb.AppendLine("Console.WriteLine($\"{(int)response.StatusCode} {body}\");");
            return sb.ToString();
        }

        private static string Capitalize(string method)
        {
            if (string.IsNullOrEmpty(method)) return "Get";
            return char.ToUpperInvariant(method[0]) + method.Substring(1).ToLowerInvariant();
        }

        private static string EscapeForShell(string s) =>
            (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");

        private static string EscapeForCSharp(string s) =>
            (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "\\n");
    }
}
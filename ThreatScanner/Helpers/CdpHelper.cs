using Microsoft.Playwright;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ThreatScanner.Helpers
{
    /// <summary>
    /// Centralizes all Chrome DevTools Protocol (CDP) session management.
    /// Attach to the running Edge instance once; reuse the same IPage across
    /// every tool (AutoFillForm, CsrfTesterForm, …) for the lifetime of the app.
    /// </summary>
    public sealed class CdpHelper : IDisposable
    {
        // ── Constants ─────────────────────────────────────────────────────────
        public const string CDP_ENDPOINT = "http://localhost:65535";
        public const string EDGE_PATH_X86 = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
        public const string EDGE_PATH_X64 = @"C:\Program Files\Microsoft\Edge\Application\msedge.exe";
        public const string EDGE_SESSION = @"C:\EdgeSession";

        // ── Shared HTTP client used only for CDP /json/version health-checks ──
        private static readonly HttpClient _cdpHttpClient =
            new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        // ── State ─────────────────────────────────────────────────────────────
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _activePage;

        // Optional logger delegate — callers supply their own Log method.
        // Signature: (message, isError) where isError drives colour choice.
        private readonly Action<string, bool> _log;

        // ── Constructor ───────────────────────────────────────────────────────
        /// <param name="log">
        ///   Optional logging callback.  First arg = message text; second = true
        ///   when the message should be highlighted as a warning/error.
        /// </param>
        public CdpHelper(Action<string, bool> log = null)
        {
            _log = log ?? ((msg, _) => System.Diagnostics.Debug.WriteLine(msg));
        }

        // =========================================================================
        //  PUBLIC API
        // =========================================================================

        /// <summary>
        /// Returns the live <see cref="IPage"/> attached to the running Edge
        /// instance.  On first call it launches (or connects to) Edge via CDP
        /// and picks the best matching tab.  Subsequent calls reuse the cached
        /// page unless it has been closed.
        /// </summary>
        public async Task<IPage> GetOrCreateActivePageAsync()
        {
            if (_activePage != null && !_activePage.IsClosed)
                return _activePage;

            await EnsureEdgeRunningAsync();

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.ConnectOverCDPAsync(CDP_ENDPOINT);

            IBrowserContext context = _browser.Contexts[0];

            // Prefer a tab already pointing at localhost (the target app).
            IPage page = null;
            foreach (IPage p in context.Pages)
            {
                if (p.Url.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    page = p;
                    break;
                }
            }
            if (page == null)
                page = context.Pages.Count > 0
                    ? context.Pages[0]
                    : await context.NewPageAsync();

            _activePage = page;
            return _activePage;
        }

        /// <summary>
        /// Returns <c>true</c> when Edge is already accepting CDP connections.
        /// </summary>
        public static async Task<bool> IsCdpReadyAsync()
        {
            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    HttpResponseMessage res = await _cdpHttpClient
                        .GetAsync(CDP_ENDPOINT + "/json/version", cts.Token);
                    return res.IsSuccessStatusCode;
                }
            }
            catch { return false; }
        }

        /// <summary>
        /// Ensures Edge is running with remote-debugging enabled on port 65535.
        /// If Edge is already listening the method returns immediately.
        /// Otherwise it kills any existing msedge.exe processes and relaunches.
        /// </summary>
        public async Task EnsureEdgeRunningAsync()
        {
            if (await IsCdpReadyAsync())
            {
                Log("[CDP] Already connected to Edge via CDP.");
                return;
            }

            string edgePath = GetEdgePath();
            if (edgePath == null)
            {
                Log("[CDP] msedge.exe not found. Launch Edge manually with --remote-debugging-port=65535", isError: true);
                return;
            }

            Log("[CDP] Closing any existing Edge instances …");
            foreach (Process proc in Process.GetProcessesByName("msedge"))
            {
                try { proc.Kill(); proc.WaitForExit(2000); } catch { }
            }
            await Task.Delay(2500);

            Log("[CDP] Launching Edge with remote debugging on port 65535 …");
            Process.Start(new ProcessStartInfo
            {
                FileName = edgePath,
                Arguments = $"--remote-debugging-port=65535 --user-data-dir=\"{EDGE_SESSION}\" --no-first-run --no-default-browser-check",
                UseShellExecute = true
            });

            Log("[CDP] Waiting for Edge to become ready …", isError: false);
            for (int i = 0; i < 20; i++)
            {
                await Task.Delay(1000);
                if (await IsCdpReadyAsync())
                {
                    Log("[CDP] Edge is ready!");
                    return;
                }
            }
            Log("[CDP] Edge did not respond in time — trying anyway.", isError: true);
        }

        /// <summary>
        /// Returns the path to msedge.exe, or <c>null</c> if not found.
        /// </summary>
        public static string GetEdgePath()
        {
            if (File.Exists(EDGE_PATH_X86)) return EDGE_PATH_X86;
            if (File.Exists(EDGE_PATH_X64)) return EDGE_PATH_X64;
            return null;
        }

        // =========================================================================
        //  DISPOSE — disconnect without closing the user's Edge window
        // =========================================================================

        public void Dispose()
        {
            try { _browser?.CloseAsync().GetAwaiter().GetResult(); } catch { }
            try { _playwright?.Dispose(); } catch { }
            _activePage = null;
            _browser = null;
            _playwright = null;
        }

        // ── Private helpers ───────────────────────────────────────────────────
        private void Log(string message, bool isError = false) =>
            _log?.Invoke(message, isError);
    }
}
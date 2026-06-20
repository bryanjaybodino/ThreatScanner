using Microsoft.Playwright;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ThreatScanner.Helpers
{
    public enum BrowserKind { Edge, Chrome, Brave, Firefox, Unknown }

    public sealed class CdpHelper : IDisposable
    {
        // ── Constants ──────────────────────────────────────────────────────────
        public const string CDP_ENDPOINT = "http://localhost:65535";
        public const string EDGE_SESSION = @"C:\EdgeSession";
        public const string CDP_PORT_ARG = "--remote-debugging-port=65535";

        // ── Browser executable candidate paths ────────────────────────────────
        private static readonly Dictionary<BrowserKind, string[]> BrowserPaths =
            new Dictionary<BrowserKind, string[]>
            {
                {
                    BrowserKind.Edge, new[]
                    {
                        @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                        @"C:\Program Files\Microsoft\Edge\Application\msedge.exe"
                    }
                },
                {
                    BrowserKind.Chrome, new[]
                    {
                        @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                        @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            @"Google\Chrome\Application\chrome.exe")
                    }
                },
                {
                    BrowserKind.Brave, new[]
                    {
                        @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe",
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            @"BraveSoftware\Brave-Browser\Application\brave.exe")
                    }
                },
                {
                    BrowserKind.Firefox, new[]
                    {
                        @"C:\Program Files\Mozilla Firefox\firefox.exe",
                        @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe"
                    }
                }
            };

        // ── Shared HTTP client for CDP health-checks ──────────────────────────
        private static readonly HttpClient _http =
            new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

        // ── Instance state ────────────────────────────────────────────────────
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _activePage;

        private readonly Action<string, bool> _log;

        public CdpHelper(Action<string, bool> log = null)
        {
            _log = log ?? delegate (string msg, bool _) { Debug.WriteLine(msg); };
        }

        // =========================================================================
        //  PUBLIC API
        // =========================================================================

        public async Task<IPage> GetOrCreateActivePageAsync()
        {
            if (_activePage != null && !_activePage.IsClosed)
                return _activePage;

            BrowserKind kind = DetectDefaultBrowser();
            Log(string.Format("[CDP] Detected default browser: {0}", kind));

            await EnsureBrowserRunningAsync(kind);

            _playwright = await Playwright.CreateAsync();

            // Firefox 86+ exposes CDP on the same port; Playwright uses its own
            // Firefox driver to connect. All Chromium-based browsers use Chromium CDP.
            if (kind == BrowserKind.Firefox)
                _browser = await _playwright.Firefox.ConnectOverCDPAsync(CDP_ENDPOINT);
            else
                _browser = await _playwright.Chromium.ConnectOverCDPAsync(CDP_ENDPOINT);

            IBrowserContext context = _browser.Contexts[0];

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

        // ─── Browser detection ─────────────────────────────────────────────────

        /// <summary>
        /// Reads the Windows registry UserChoice key to identify the default browser.
        /// Returns <see cref="BrowserKind.Unknown"/> if detection fails.
        /// </summary>
        public static BrowserKind DetectDefaultBrowser()
        {
            try
            {
                const string keyPath =
                    @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\https\UserChoice";

                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(keyPath))
                {
                    if (regKey == null) return BrowserKind.Unknown;

                    object value = regKey.GetValue("ProgId");
                    if (value == null) return BrowserKind.Unknown;

                    string progId = value.ToString();

                    // ProgId examples: MSEdgeHTM, ChromeHTML, BraveHTML, FirefoxURL-xxxxx
                    if (progId.IndexOf("Edge", StringComparison.OrdinalIgnoreCase) >= 0) return BrowserKind.Edge;
                    if (progId.IndexOf("Chrome", StringComparison.OrdinalIgnoreCase) >= 0) return BrowserKind.Chrome;
                    if (progId.IndexOf("Brave", StringComparison.OrdinalIgnoreCase) >= 0) return BrowserKind.Brave;
                    if (progId.IndexOf("Firefox", StringComparison.OrdinalIgnoreCase) >= 0) return BrowserKind.Firefox;
                }
            }
            catch { /* registry unavailable — fall through */ }

            return BrowserKind.Unknown;
        }

        // ─── Launch / connect ──────────────────────────────────────────────────

        public async Task EnsureBrowserRunningAsync(BrowserKind kind)
        {
            // ── Already listening? Just connect — don't touch the browser at all. ──
            if (await IsCdpReadyAsync())
            {
                Log("[CDP] Browser already has CDP open. Connecting to existing session.");
                return;
            }

            // ── Nothing listening — only NOW do we launch a new instance. ──────────
            if (kind == BrowserKind.Unknown)
            {
                Log("[CDP] Default browser unknown — falling back to Edge.", isError: true);
                kind = BrowserKind.Edge;
            }

            string exePath = ResolveBrowserPath(kind);
            if (exePath == null)
            {
                Log(string.Format(
                    "[CDP] Could not find {0}. Launch it manually with {1}.",
                    kind, CDP_PORT_ARG), isError: true);
                return;
            }

            // No Kill() call here — we only reach this branch when nothing is running.
            string args = BuildLaunchArgs(kind);
            Log(string.Format("[CDP] No browser detected. Launching {0}…", kind));
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = args,
                UseShellExecute = true
            });

            Log("[CDP] Waiting for browser to become ready…");
            for (int i = 0; i < 20; i++)
            {
                await Task.Delay(1000);
                if (await IsCdpReadyAsync())
                {
                    Log("[CDP] Browser ready!");
                    return;
                }
            }
            Log("[CDP] Browser did not respond in time — trying anyway.", isError: true);
        }

        // ─── Helpers ───────────────────────────────────────────────────────────

        public static string ResolveBrowserPath(BrowserKind kind)
        {
            string[] candidates;
            if (!BrowserPaths.TryGetValue(kind, out candidates))
                return null;

            foreach (string path in candidates)
                if (File.Exists(path)) return path;

            return null;
        }

        private static string BuildLaunchArgs(BrowserKind kind)
        {
            // Firefox ignores Chromium-specific switches like --no-first-run.
            if (kind == BrowserKind.Firefox)
                return CDP_PORT_ARG;

            return string.Format(
                "{0} --no-first-run --no-default-browser-check --user-data-dir=\"{1}\"",
                CDP_PORT_ARG, EDGE_SESSION);
        }

        public static async Task<bool> IsCdpReadyAsync()
        {
            try
            {
                using (CancellationTokenSource cts =
                    new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    HttpResponseMessage res = await _http
                        .GetAsync(CDP_ENDPOINT + "/json/version", cts.Token)
                        .ConfigureAwait(false);
                    return res.IsSuccessStatusCode;
                }
            }
            catch { return false; }
        }

        // ── Dispose ────────────────────────────────────────────────────────────

        public void Dispose()
        {
            try { if (_browser != null) _browser.CloseAsync().GetAwaiter().GetResult(); }
            catch { }
            try { if (_playwright != null) _playwright.Dispose(); }
            catch { }
            _activePage = null;
            _browser = null;
            _playwright = null;
        }

        private void Log(string message, bool isError = false)
        {
            if (_log != null) _log(message, isError);
        }
    }
}
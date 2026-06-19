using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ThreatScanner
{
    internal static class Program
    {
        // ── Per-Monitor V2 DPI awareness (requires Windows 10 1703+) ────────────
        // On .NET Framework 4.8 we must P/Invoke shcore.dll directly.
        // The app.manifest already declares PerMonitorV2, but calling this at
        // runtime as well ensures the process DPI context is set before any
        // window handles are created, which is the safest approach on 4.8.

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(int awareness);
        // awareness values:
        //   0 = PROCESS_DPI_UNAWARE
        //   1 = PROCESS_SYSTEM_DPI_AWARE
        //   2 = PROCESS_PER_MONITOR_DPI_AWARE

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        private static void EnableDpiAwareness()
        {
            try
            {
                // PROCESS_PER_MONITOR_DPI_AWARE (shcore, Win 8.1+)
                SetProcessDpiAwareness(2);
            }
            catch
            {
                // Fallback for Windows 7 / older: system-DPI aware
                try { SetProcessDPIAware(); } catch { /* ignore */ }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                EnableDpiAwareness();                          // must be called FIRST
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch { }
        }
    }
}
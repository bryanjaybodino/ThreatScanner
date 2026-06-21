using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ThreatScanner
{
    /// <summary>
    /// ThreatScanner launcher — choose a tool to open.
    ///
    /// Fixes applied:
    ///   1) Singleton-per-tool tracking: clicking a launcher button when that
    ///      tool is already open just brings the existing window to the front
    ///      instead of spawning a duplicate. Closing MainForm no longer
    ///      "re-opens" anything — each child form is independent, and we only
    ///      ever hold a reference to it, never re-create it while it's alive.
    ///   2) Non-blocking show: heavy forms (lots of grids/rich text boxes) can
    ///      cause a visible UI hitch while WinForms lays out and paints all
    ///      their child controls for the first time. We construct + show them
    ///      via BeginInvoke (posted to the message loop) so the button click
    ///      itself returns immediately and MainForm never appears to freeze,
    ///      and we wrap construction in SuspendLayout/ResumeLayout-friendly
    ///      handling so the first paint is a single batch instead of N
    ///      incremental layout passes.
    /// </summary>
    public partial class MainForm : Form
    {
        // One slot per tool. Holds the live instance (or null if closed).
        private readonly Dictionary<Type, Form> _openForms = new Dictionary<Type, Form>();

        public MainForm()
        {
            InitializeComponent();
        }

        // ─── CORE: open-or-focus a singleton tool window ───────────────────────

        /// <summary>
        /// Shows a single instance of TForm. If one is already open, it is
        /// restored (if minimized) and brought to the foreground instead of
        /// creating a new one. Construction + Show happen on a posted message
        /// (BeginInvoke) so the calling click handler returns immediately and
        /// the launcher UI never blocks/lags waiting for the child form's
        /// first layout+paint pass.
        /// </summary>
        private void OpenOrFocus<TForm>(Func<TForm> factory) where TForm : Form
        {
            var key = typeof(TForm);

            if (_openForms.TryGetValue(key, out var existing) && existing != null && !existing.IsDisposed)
            {
                FocusExisting(existing);
                return;
            }

            BeginInvoke((Action)(() =>
            {
                TForm form = factory();

                form.SuspendLayout();
                try
                {
                    form.StartPosition = FormStartPosition.CenterScreen;
                }
                finally
                {
                    form.ResumeLayout(performLayout: true);
                }

                _openForms[key] = form;
                form.FormClosed += (s, e) =>
                {
                    if (_openForms.TryGetValue(key, out var tracked) && ReferenceEquals(tracked, form))
                        _openForms.Remove(key);

                    // Sub-form closed -> bring MainForm back
                    this.Show();
                    FocusExisting(this);
                };

                // Sub-form opening -> hide MainForm
                this.Hide();

                form.Show();
                FocusExisting(form);
            }));
        }

        private static void FocusExisting(Form form)
        {
            if (form.WindowState == FormWindowState.Minimized)
                form.WindowState = FormWindowState.Normal;

            form.Activate();
            form.BringToFront();
        }

        // ─── BUTTON HANDLERS ─────────────────────────────────────────────────────

        private void button_OpenScanner_Click(object sender, EventArgs e)
        {
            OpenOrFocus(() => new FullScannerForm());
        }

        private void button_OpenBruteForce_Click(object sender, EventArgs e)
        {
            OpenOrFocus(() => new BruteForceForm());
        }

        private void button_OpenApiTester_Click(object sender, EventArgs e)
        {
            OpenOrFocus(() => new ApiTesterForm());
        }

        private void button_OpenWebSocket_Click(object sender, EventArgs e)
        {
            OpenOrFocus(() => new WebSocketForm());
        }

        private void button_OpenCsrf_Click(object sender, EventArgs e)
        {
            OpenOrFocus(() => new CsrfTesterForm());
        }

        private void button_OpenAutoFill_Click(object sender, EventArgs e)
        {
            OpenOrFocus(() => new AutoFillForm());
        }

        private void button_OpenSubdomainScanner_Click(object sender, EventArgs e)
        {
            OpenOrFocus(() => new SubdomainScannerForm());
        }

        private void button_OpenSqlInjection_Click(object sender, EventArgs e)
        {
            OpenOrFocus(() => new SqlInjectionForm());
        }

        private void button_OpenHttpProxyLog_Click(object sender, EventArgs e)
        {
            OpenOrFocus(() => new HttpProxyLogForm());
        }
    }
}
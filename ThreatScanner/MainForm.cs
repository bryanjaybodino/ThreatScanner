using System;
using System.Drawing;
using System.Windows.Forms;

namespace ThreatScanner
{
    /// <summary>
    /// ThreatScanner launcher — choose a tool to open.
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // ─── BUTTON HANDLERS ─────────────────────────────────────────────────────

        private void button_OpenScanner_Click(object sender, EventArgs e)
        {
            new FullScannerForm().Show();
        }

        private void button_OpenBruteForce_Click(object sender, EventArgs e)
        {
            new BruteForceForm().Show();
        }

        private void button_OpenApiTester_Click(object sender, EventArgs e)
        {
            new ApiTesterForm().Show();
        }

        private void button_OpenWebSocket_Click(object sender, EventArgs e)
        {
            new WebSocketForm().Show();
        }
        private void button_OpenCsrf_Click(object sender, EventArgs e)
        {
            new CsrfTesterForm().Show();
        }
        private void button_OpenAutoFill_Click(object sender, EventArgs e)
        {
            new AutoFillForm().Show();
        }

        private void button_OpenSubdomainScanner_Click(object sender, EventArgs e)
        {
            new SubdomainScannerForm().Show();
        }
    }
}
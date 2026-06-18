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
    }
}
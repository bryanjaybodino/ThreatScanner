namespace ThreatScanner
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panel_TopBar = new System.Windows.Forms.Panel();
            this.label_Title = new System.Windows.Forms.Label();
            this.label_Subtitle = new System.Windows.Forms.Label();
            this.panel_Cards = new System.Windows.Forms.Panel();
            this.button_OpenScanner = new System.Windows.Forms.Button();
            this.button_OpenBruteForce = new System.Windows.Forms.Button();
            this.button_OpenApiTester = new System.Windows.Forms.Button();
            this.label_ScannerDesc = new System.Windows.Forms.Label();
            this.label_BruteDesc = new System.Windows.Forms.Label();
            this.label_ApiDesc = new System.Windows.Forms.Label();
            this.label_Disclaimer = new System.Windows.Forms.Label();

            this.panel_TopBar.SuspendLayout();
            this.panel_Cards.SuspendLayout();
            this.SuspendLayout();

            // ── panel_TopBar ────────────────────────────────────────────────────
            this.panel_TopBar.BackColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.panel_TopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_TopBar.Height = 80;
            this.panel_TopBar.Controls.Add(this.label_Title);
            this.panel_TopBar.Controls.Add(this.label_Subtitle);

            this.label_Title.AutoSize = true;
            this.label_Title.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.label_Title.ForeColor = System.Drawing.Color.White;
            this.label_Title.Location = new System.Drawing.Point(20, 10);
            this.label_Title.Text = "⚡ ThreatScanner";

            this.label_Subtitle.AutoSize = true;
            this.label_Subtitle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_Subtitle.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.label_Subtitle.Location = new System.Drawing.Point(22, 48);
            this.label_Subtitle.Text = "Web Vulnerability Testing Toolkit  •  For authorized testing only";

            // ── panel_Cards ─────────────────────────────────────────────────────
            this.panel_Cards.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.panel_Cards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Cards.Controls.Add(this.button_OpenScanner);
            this.panel_Cards.Controls.Add(this.label_ScannerDesc);
            this.panel_Cards.Controls.Add(this.button_OpenBruteForce);
            this.panel_Cards.Controls.Add(this.label_BruteDesc);
            this.panel_Cards.Controls.Add(this.button_OpenApiTester);
            this.panel_Cards.Controls.Add(this.label_ApiDesc);
            this.panel_Cards.Controls.Add(this.label_Disclaimer);

            // ── Card 1 · Full Scanner ───────────────────────────────────────────
            this.button_OpenScanner.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.button_OpenScanner.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_OpenScanner.FlatAppearance.BorderSize = 0;
            this.button_OpenScanner.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.button_OpenScanner.ForeColor = System.Drawing.Color.White;
            this.button_OpenScanner.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_OpenScanner.Location = new System.Drawing.Point(50, 60);
            this.button_OpenScanner.Size = new System.Drawing.Size(300, 100);
            this.button_OpenScanner.Text = "🔍  Full Scanner";
            this.button_OpenScanner.Click += new System.EventHandler(this.button_OpenScanner_Click);

            this.label_ScannerDesc.AutoSize = false;
            this.label_ScannerDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_ScannerDesc.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.label_ScannerDesc.Location = new System.Drawing.Point(50, 170);
            this.label_ScannerDesc.Size = new System.Drawing.Size(300, 50);
            this.label_ScannerDesc.Text = "HTTPS, security headers, cookies,\nSQL injection hints, XSS, ports, DNS";
            this.label_ScannerDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── Card 2 · Brute Force ────────────────────────────────────────────
            this.button_OpenBruteForce.BackColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.button_OpenBruteForce.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_OpenBruteForce.FlatAppearance.BorderSize = 0;
            this.button_OpenBruteForce.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.button_OpenBruteForce.ForeColor = System.Drawing.Color.White;
            this.button_OpenBruteForce.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_OpenBruteForce.Location = new System.Drawing.Point(400, 60);
            this.button_OpenBruteForce.Size = new System.Drawing.Size(300, 100);
            this.button_OpenBruteForce.Text = "🔐  Brute Force";
            this.button_OpenBruteForce.Click += new System.EventHandler(this.button_OpenBruteForce_Click);

            this.label_BruteDesc.AutoSize = false;
            this.label_BruteDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_BruteDesc.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.label_BruteDesc.Location = new System.Drawing.Point(400, 170);
            this.label_BruteDesc.Size = new System.Drawing.Size(300, 50);
            this.label_BruteDesc.Text = "Auto-detect ASP.NET / PHP / HTML forms\nand attack with a wordlist";
            this.label_BruteDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── Card 3 · API Tester ─────────────────────────────────────────────
            this.button_OpenApiTester.BackColor = System.Drawing.Color.FromArgb(5, 150, 105);
            this.button_OpenApiTester.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_OpenApiTester.FlatAppearance.BorderSize = 0;
            this.button_OpenApiTester.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.button_OpenApiTester.ForeColor = System.Drawing.Color.White;
            this.button_OpenApiTester.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_OpenApiTester.Location = new System.Drawing.Point(750, 60);
            this.button_OpenApiTester.Size = new System.Drawing.Size(300, 100);
            this.button_OpenApiTester.Text = "🛰  API Tester";
            this.button_OpenApiTester.Click += new System.EventHandler(this.button_OpenApiTester_Click);

            this.label_ApiDesc.AutoSize = false;
            this.label_ApiDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_ApiDesc.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.label_ApiDesc.Location = new System.Drawing.Point(750, 170);
            this.label_ApiDesc.Size = new System.Drawing.Size(300, 50);
            this.label_ApiDesc.Text = "Postman-style HTTP tester with auth,\nheaders, params, body & wordlist fuzzing";
            this.label_ApiDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── Disclaimer ──────────────────────────────────────────────────────
            this.label_Disclaimer.AutoSize = false;
            this.label_Disclaimer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label_Disclaimer.Height = 28;
            this.label_Disclaimer.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
            this.label_Disclaimer.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.label_Disclaimer.Text = "⚠️  For authorized penetration testing only. Unauthorized use is illegal.";
            this.label_Disclaimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── MainForm ────────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.ClientSize = new System.Drawing.Size(1100, 310);
            this.Controls.Add(this.panel_Cards);
            this.Controls.Add(this.panel_TopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 350);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThreatScanner — Select Tool";

            this.panel_TopBar.ResumeLayout(false);
            this.panel_TopBar.PerformLayout();
            this.panel_Cards.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel_TopBar;
        private System.Windows.Forms.Label label_Title;
        private System.Windows.Forms.Label label_Subtitle;
        private System.Windows.Forms.Panel panel_Cards;
        private System.Windows.Forms.Button button_OpenScanner;
        private System.Windows.Forms.Button button_OpenBruteForce;
        private System.Windows.Forms.Button button_OpenApiTester;
        private System.Windows.Forms.Label label_ScannerDesc;
        private System.Windows.Forms.Label label_BruteDesc;
        private System.Windows.Forms.Label label_ApiDesc;
        private System.Windows.Forms.Label label_Disclaimer;
    }
}
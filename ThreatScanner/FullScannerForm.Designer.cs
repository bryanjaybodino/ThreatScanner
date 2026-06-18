namespace ThreatScanner
{
    partial class FullScannerForm
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
            this.label_AppTitle = new System.Windows.Forms.Label();
            this.label_AppSubtitle = new System.Windows.Forms.Label();
            this.button_SaveReport = new System.Windows.Forms.Button();
            this.button_ClearOutput = new System.Windows.Forms.Button();
            this.panel_UrlBar = new System.Windows.Forms.Panel();
            this.label_UrlTitle = new System.Windows.Forms.Label();
            this.textBox_Url = new System.Windows.Forms.TextBox();
            this.tabControl_Main = new System.Windows.Forms.TabControl();
            this.tabPage_Scanner = new System.Windows.Forms.TabPage();
            this.button_Scan = new System.Windows.Forms.Button();
            this.label_ScanInfo = new System.Windows.Forms.Label();
            this.tabPage_Headers = new System.Windows.Forms.TabPage();
            this.button_AnalyzeHeaders = new System.Windows.Forms.Button();
            this.label_HeaderInfo = new System.Windows.Forms.Label();
            this.tabPage_Dns = new System.Windows.Forms.TabPage();
            this.button_DnsLookup = new System.Windows.Forms.Button();
            this.label_DnsInfo = new System.Windows.Forms.Label();
            this.panel_Output = new System.Windows.Forms.Panel();
            this.listBox_Output = new System.Windows.Forms.ListBox();
            this.label_Output = new System.Windows.Forms.Label();
            this.progressBar_Scan = new System.Windows.Forms.ProgressBar();
            this.webBrowser_Hidden = new System.Windows.Forms.WebBrowser();
            this.panel_TopBar.SuspendLayout();
            this.panel_UrlBar.SuspendLayout();
            this.tabControl_Main.SuspendLayout();
            this.tabPage_Scanner.SuspendLayout();
            this.tabPage_Headers.SuspendLayout();
            this.tabPage_Dns.SuspendLayout();
            this.panel_Output.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_TopBar
            // 
            this.panel_TopBar.BackColor = System.Drawing.Color.White;
            this.panel_TopBar.Controls.Add(this.label_AppTitle);
            this.panel_TopBar.Controls.Add(this.label_AppSubtitle);
            this.panel_TopBar.Controls.Add(this.button_SaveReport);
            this.panel_TopBar.Controls.Add(this.button_ClearOutput);
            this.panel_TopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_TopBar.Location = new System.Drawing.Point(0, 0);
            this.panel_TopBar.Name = "panel_TopBar";
            this.panel_TopBar.Size = new System.Drawing.Size(1100, 64);
            this.panel_TopBar.TabIndex = 3;
            // 
            // label_AppTitle
            // 
            this.label_AppTitle.AutoSize = true;
            this.label_AppTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label_AppTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.label_AppTitle.Location = new System.Drawing.Point(16, 12);
            this.label_AppTitle.Name = "label_AppTitle";
            this.label_AppTitle.Size = new System.Drawing.Size(201, 32);
            this.label_AppTitle.TabIndex = 0;
            this.label_AppTitle.Text = "🔍  Full Scanner";
            // 
            // label_AppSubtitle
            // 
            this.label_AppSubtitle.AutoSize = true;
            this.label_AppSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_AppSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AppSubtitle.Location = new System.Drawing.Point(18, 40);
            this.label_AppSubtitle.Name = "label_AppSubtitle";
            this.label_AppSubtitle.Size = new System.Drawing.Size(500, 20);
            this.label_AppSubtitle.TabIndex = 1;
            this.label_AppSubtitle.Text = "HTTPS · Security Headers · Cookies · SQLi · XSS · Ports · Sensitive Files · DNS";
            // 
            // button_SaveReport
            // 
            this.button_SaveReport.BackColor = System.Drawing.Color.White;
            this.button_SaveReport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_SaveReport.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_SaveReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SaveReport.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_SaveReport.Location = new System.Drawing.Point(960, 16);
            this.button_SaveReport.Name = "button_SaveReport";
            this.button_SaveReport.Size = new System.Drawing.Size(120, 32);
            this.button_SaveReport.TabIndex = 2;
            this.button_SaveReport.Text = "💾  Save Report";
            this.button_SaveReport.UseVisualStyleBackColor = false;
            this.button_SaveReport.Click += new System.EventHandler(this.button_SaveReport_Click);
            // 
            // button_ClearOutput
            // 
            this.button_ClearOutput.BackColor = System.Drawing.Color.White;
            this.button_ClearOutput.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_ClearOutput.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_ClearOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_ClearOutput.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_ClearOutput.Location = new System.Drawing.Point(850, 16);
            this.button_ClearOutput.Name = "button_ClearOutput";
            this.button_ClearOutput.Size = new System.Drawing.Size(100, 32);
            this.button_ClearOutput.TabIndex = 3;
            this.button_ClearOutput.Text = "🗑  Clear";
            this.button_ClearOutput.UseVisualStyleBackColor = false;
            this.button_ClearOutput.Click += new System.EventHandler(this.button_ClearOutput_Click);
            // 
            // panel_UrlBar
            // 
            this.panel_UrlBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel_UrlBar.Controls.Add(this.label_UrlTitle);
            this.panel_UrlBar.Controls.Add(this.textBox_Url);
            this.panel_UrlBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_UrlBar.Location = new System.Drawing.Point(0, 64);
            this.panel_UrlBar.Name = "panel_UrlBar";
            this.panel_UrlBar.Size = new System.Drawing.Size(1100, 52);
            this.panel_UrlBar.TabIndex = 2;
            // 
            // label_UrlTitle
            // 
            this.label_UrlTitle.AutoSize = true;
            this.label_UrlTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_UrlTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_UrlTitle.Location = new System.Drawing.Point(16, 16);
            this.label_UrlTitle.Name = "label_UrlTitle";
            this.label_UrlTitle.Size = new System.Drawing.Size(99, 20);
            this.label_UrlTitle.TabIndex = 0;
            this.label_UrlTitle.Text = "TARGET URL";
            // 
            // textBox_Url
            // 
            this.textBox_Url.BackColor = System.Drawing.Color.White;
            this.textBox_Url.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Url.Font = new System.Drawing.Font("Consolas", 10F);
            this.textBox_Url.Location = new System.Drawing.Point(120, 12);
            this.textBox_Url.Name = "textBox_Url";
            this.textBox_Url.Size = new System.Drawing.Size(960, 27);
            this.textBox_Url.TabIndex = 1;
            // 
            // tabControl_Main
            // 
            this.tabControl_Main.Controls.Add(this.tabPage_Scanner);
            this.tabControl_Main.Controls.Add(this.tabPage_Headers);
            this.tabControl_Main.Controls.Add(this.tabPage_Dns);
            this.tabControl_Main.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl_Main.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.tabControl_Main.Location = new System.Drawing.Point(0, 116);
            this.tabControl_Main.Name = "tabControl_Main";
            this.tabControl_Main.SelectedIndex = 0;
            this.tabControl_Main.Size = new System.Drawing.Size(1100, 80);
            this.tabControl_Main.TabIndex = 1;
            // 
            // tabPage_Scanner
            // 
            this.tabPage_Scanner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Scanner.Controls.Add(this.button_Scan);
            this.tabPage_Scanner.Controls.Add(this.label_ScanInfo);
            this.tabPage_Scanner.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Scanner.Name = "tabPage_Scanner";
            this.tabPage_Scanner.Size = new System.Drawing.Size(1092, 46);
            this.tabPage_Scanner.TabIndex = 0;
            this.tabPage_Scanner.Text = "  Full Scan";
            // 
            // button_Scan
            // 
            this.button_Scan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.button_Scan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Scan.FlatAppearance.BorderSize = 0;
            this.button_Scan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Scan.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_Scan.ForeColor = System.Drawing.Color.White;
            this.button_Scan.Location = new System.Drawing.Point(12, 8);
            this.button_Scan.Name = "button_Scan";
            this.button_Scan.Size = new System.Drawing.Size(130, 36);
            this.button_Scan.TabIndex = 0;
            this.button_Scan.Text = "▶  Start Scan";
            this.button_Scan.UseVisualStyleBackColor = false;
            this.button_Scan.Click += new System.EventHandler(this.button_Scan_Click);
            // 
            // label_ScanInfo
            // 
            this.label_ScanInfo.AutoSize = true;
            this.label_ScanInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_ScanInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_ScanInfo.Location = new System.Drawing.Point(155, 16);
            this.label_ScanInfo.Name = "label_ScanInfo";
            this.label_ScanInfo.Size = new System.Drawing.Size(541, 20);
            this.label_ScanInfo.TabIndex = 1;
            this.label_ScanInfo.Text = "Runs all checks: HTTPS, headers, cookies, redirects, SQLi, XSS, ports, sensitive " +
    "files";
            // 
            // tabPage_Headers
            // 
            this.tabPage_Headers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Headers.Controls.Add(this.button_AnalyzeHeaders);
            this.tabPage_Headers.Controls.Add(this.label_HeaderInfo);
            this.tabPage_Headers.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Headers.Name = "tabPage_Headers";
            this.tabPage_Headers.Size = new System.Drawing.Size(1092, 46);
            this.tabPage_Headers.TabIndex = 1;
            this.tabPage_Headers.Text = "  Header Analyzer";
            // 
            // button_AnalyzeHeaders
            // 
            this.button_AnalyzeHeaders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.button_AnalyzeHeaders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_AnalyzeHeaders.FlatAppearance.BorderSize = 0;
            this.button_AnalyzeHeaders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_AnalyzeHeaders.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_AnalyzeHeaders.ForeColor = System.Drawing.Color.White;
            this.button_AnalyzeHeaders.Location = new System.Drawing.Point(12, 8);
            this.button_AnalyzeHeaders.Name = "button_AnalyzeHeaders";
            this.button_AnalyzeHeaders.Size = new System.Drawing.Size(150, 36);
            this.button_AnalyzeHeaders.TabIndex = 0;
            this.button_AnalyzeHeaders.Text = "📋  Dump Headers";
            this.button_AnalyzeHeaders.UseVisualStyleBackColor = false;
            this.button_AnalyzeHeaders.Click += new System.EventHandler(this.button_AnalyzeHeaders_Click);
            // 
            // label_HeaderInfo
            // 
            this.label_HeaderInfo.AutoSize = true;
            this.label_HeaderInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_HeaderInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_HeaderInfo.Location = new System.Drawing.Point(175, 16);
            this.label_HeaderInfo.Name = "label_HeaderInfo";
            this.label_HeaderInfo.Size = new System.Drawing.Size(367, 20);
            this.label_HeaderInfo.TabIndex = 1;
            this.label_HeaderInfo.Text = "Dumps all response headers and the HTTP status code";
            // 
            // tabPage_Dns
            // 
            this.tabPage_Dns.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Dns.Controls.Add(this.button_DnsLookup);
            this.tabPage_Dns.Controls.Add(this.label_DnsInfo);
            this.tabPage_Dns.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Dns.Name = "tabPage_Dns";
            this.tabPage_Dns.Size = new System.Drawing.Size(1092, 46);
            this.tabPage_Dns.TabIndex = 2;
            this.tabPage_Dns.Text = "  DNS Lookup";
            // 
            // button_DnsLookup
            // 
            this.button_DnsLookup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.button_DnsLookup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_DnsLookup.FlatAppearance.BorderSize = 0;
            this.button_DnsLookup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_DnsLookup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_DnsLookup.ForeColor = System.Drawing.Color.White;
            this.button_DnsLookup.Location = new System.Drawing.Point(12, 8);
            this.button_DnsLookup.Name = "button_DnsLookup";
            this.button_DnsLookup.Size = new System.Drawing.Size(140, 36);
            this.button_DnsLookup.TabIndex = 0;
            this.button_DnsLookup.Text = "🌐  DNS Lookup";
            this.button_DnsLookup.UseVisualStyleBackColor = false;
            this.button_DnsLookup.Click += new System.EventHandler(this.button_DnsLookup_Click);
            // 
            // label_DnsInfo
            // 
            this.label_DnsInfo.AutoSize = true;
            this.label_DnsInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_DnsInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_DnsInfo.Location = new System.Drawing.Point(165, 16);
            this.label_DnsInfo.Name = "label_DnsInfo";
            this.label_DnsInfo.Size = new System.Drawing.Size(392, 20);
            this.label_DnsInfo.TabIndex = 1;
            this.label_DnsInfo.Text = "Resolves IP addresses, hostname and aliases for the target";
            // 
            // panel_Output
            // 
            this.panel_Output.BackColor = System.Drawing.Color.White;
            this.panel_Output.Controls.Add(this.listBox_Output);
            this.panel_Output.Controls.Add(this.label_Output);
            this.panel_Output.Controls.Add(this.progressBar_Scan);
            this.panel_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Output.Location = new System.Drawing.Point(0, 196);
            this.panel_Output.Name = "panel_Output";
            this.panel_Output.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.panel_Output.Size = new System.Drawing.Size(1100, 504);
            this.panel_Output.TabIndex = 0;
            // 
            // listBox_Output
            // 
            this.listBox_Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.listBox_Output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Output.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.listBox_Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.listBox_Output.HorizontalScrollbar = true;
            this.listBox_Output.ItemHeight = 19;
            this.listBox_Output.Location = new System.Drawing.Point(12, 28);
            this.listBox_Output.Name = "listBox_Output";
            this.listBox_Output.Size = new System.Drawing.Size(1076, 464);
            this.listBox_Output.TabIndex = 0;
            // 
            // label_Output
            // 
            this.label_Output.AutoSize = true;
            this.label_Output.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_Output.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_Output.Location = new System.Drawing.Point(12, 8);
            this.label_Output.Name = "label_Output";
            this.label_Output.Size = new System.Drawing.Size(69, 20);
            this.label_Output.TabIndex = 1;
            this.label_Output.Text = "OUTPUT";
            // 
            // progressBar_Scan
            // 
            this.progressBar_Scan.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar_Scan.Location = new System.Drawing.Point(12, 492);
            this.progressBar_Scan.MarqueeAnimationSpeed = 30;
            this.progressBar_Scan.Name = "progressBar_Scan";
            this.progressBar_Scan.Size = new System.Drawing.Size(1076, 4);
            this.progressBar_Scan.TabIndex = 2;
            // 
            // webBrowser_Hidden
            // 
            this.webBrowser_Hidden.Location = new System.Drawing.Point(0, 0);
            this.webBrowser_Hidden.Name = "webBrowser_Hidden";
            this.webBrowser_Hidden.ScriptErrorsSuppressed = true;
            this.webBrowser_Hidden.Size = new System.Drawing.Size(1, 1);
            this.webBrowser_Hidden.TabIndex = 4;
            this.webBrowser_Hidden.Visible = false;
            // 
            // FullScannerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.panel_Output);
            this.Controls.Add(this.tabControl_Main);
            this.Controls.Add(this.panel_UrlBar);
            this.Controls.Add(this.panel_TopBar);
            this.Controls.Add(this.webBrowser_Hidden);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "FullScannerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThreatScanner — Full Scanner";
            this.panel_TopBar.ResumeLayout(false);
            this.panel_TopBar.PerformLayout();
            this.panel_UrlBar.ResumeLayout(false);
            this.panel_UrlBar.PerformLayout();
            this.tabControl_Main.ResumeLayout(false);
            this.tabPage_Scanner.ResumeLayout(false);
            this.tabPage_Scanner.PerformLayout();
            this.tabPage_Headers.ResumeLayout(false);
            this.tabPage_Headers.PerformLayout();
            this.tabPage_Dns.ResumeLayout(false);
            this.tabPage_Dns.PerformLayout();
            this.panel_Output.ResumeLayout(false);
            this.panel_Output.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_TopBar;
        private System.Windows.Forms.Label label_AppTitle;
        private System.Windows.Forms.Label label_AppSubtitle;
        private System.Windows.Forms.Button button_SaveReport;
        private System.Windows.Forms.Button button_ClearOutput;
        private System.Windows.Forms.Panel panel_UrlBar;
        private System.Windows.Forms.Label label_UrlTitle;
        private System.Windows.Forms.TextBox textBox_Url;
        private System.Windows.Forms.TabControl tabControl_Main;
        private System.Windows.Forms.TabPage tabPage_Scanner;
        private System.Windows.Forms.Button button_Scan;
        private System.Windows.Forms.Label label_ScanInfo;
        private System.Windows.Forms.TabPage tabPage_Headers;
        private System.Windows.Forms.Button button_AnalyzeHeaders;
        private System.Windows.Forms.Label label_HeaderInfo;
        private System.Windows.Forms.TabPage tabPage_Dns;
        private System.Windows.Forms.Button button_DnsLookup;
        private System.Windows.Forms.Label label_DnsInfo;
        private System.Windows.Forms.Panel panel_Output;
        private System.Windows.Forms.Label label_Output;
        private System.Windows.Forms.ListBox listBox_Output;
        private System.Windows.Forms.ProgressBar progressBar_Scan;
        private System.Windows.Forms.WebBrowser webBrowser_Hidden;
    }
}
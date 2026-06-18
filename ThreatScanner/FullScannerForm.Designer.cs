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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel_TopBar = new System.Windows.Forms.Panel();
            this.label_AppTitle = new System.Windows.Forms.Label();
            this.label_AppSubtitle = new System.Windows.Forms.Label();
            this.button_CopyOutput = new System.Windows.Forms.Button();
            this.button_SaveReport = new System.Windows.Forms.Button();
            this.button_ClearOutput = new System.Windows.Forms.Button();
            this.panel_UrlBar = new System.Windows.Forms.Panel();
            this.label_UrlTitle = new System.Windows.Forms.Label();
            this.textBox_Url = new System.Windows.Forms.TextBox();
            this.tabControl_Main = new System.Windows.Forms.TabControl();
            this.tabPage_Scanner = new System.Windows.Forms.TabPage();
            this.button_Scan = new System.Windows.Forms.Button();
            this.button_StopScan = new System.Windows.Forms.Button();
            this.label_ScanInfo = new System.Windows.Forms.Label();
            this.tabPage_Headers = new System.Windows.Forms.TabPage();
            this.button_AnalyzeHeaders = new System.Windows.Forms.Button();
            this.label_HeaderInfo = new System.Windows.Forms.Label();
            this.tabPage_Dns = new System.Windows.Forms.TabPage();
            this.button_DnsLookup = new System.Windows.Forms.Button();
            this.label_DnsInfo = new System.Windows.Forms.Label();
            this.panel_Output = new System.Windows.Forms.Panel();
            this.dataGridView_Output = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colResponse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label_Output = new System.Windows.Forms.Label();
            this.progressBar_Scan = new System.Windows.Forms.ProgressBar();
            this.panel_TopBar.SuspendLayout();
            this.panel_UrlBar.SuspendLayout();
            this.tabControl_Main.SuspendLayout();
            this.tabPage_Scanner.SuspendLayout();
            this.tabPage_Headers.SuspendLayout();
            this.tabPage_Dns.SuspendLayout();
            this.panel_Output.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Output)).BeginInit();
            this.SuspendLayout();
            // 
            // panel_TopBar
            // 
            this.panel_TopBar.BackColor = System.Drawing.Color.White;
            this.panel_TopBar.Controls.Add(this.label_AppTitle);
            this.panel_TopBar.Controls.Add(this.label_AppSubtitle);
            this.panel_TopBar.Controls.Add(this.button_CopyOutput);
            this.panel_TopBar.Controls.Add(this.button_SaveReport);
            this.panel_TopBar.Controls.Add(this.button_ClearOutput);
            this.panel_TopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_TopBar.Location = new System.Drawing.Point(0, 0);
            this.panel_TopBar.Name = "panel_TopBar";
            this.panel_TopBar.Size = new System.Drawing.Size(1200, 64);
            this.panel_TopBar.TabIndex = 3;
            // 
            // label_AppTitle
            // 
            this.label_AppTitle.AutoSize = true;
            this.label_AppTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label_AppTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.label_AppTitle.Location = new System.Drawing.Point(16, 12);
            this.label_AppTitle.Name = "label_AppTitle";
            this.label_AppTitle.Size = new System.Drawing.Size(365, 32);
            this.label_AppTitle.TabIndex = 0;
            this.label_AppTitle.Text = "🔍  ThreatScanner — Full Scan";
            // 
            // label_AppSubtitle
            // 
            this.label_AppSubtitle.AutoSize = true;
            this.label_AppSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_AppSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AppSubtitle.Location = new System.Drawing.Point(18, 40);
            this.label_AppSubtitle.Name = "label_AppSubtitle";
            this.label_AppSubtitle.Size = new System.Drawing.Size(608, 20);
            this.label_AppSubtitle.TabIndex = 1;
            this.label_AppSubtitle.Text = "HTTPS · Security Headers · Cookies · SQLi · XSS · Ports · Sensitive Files · Crawl" +
    " · JS Links · DNS";
            // 
            // button_CopyOutput
            // 
            this.button_CopyOutput.BackColor = System.Drawing.Color.White;
            this.button_CopyOutput.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_CopyOutput.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_CopyOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_CopyOutput.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_CopyOutput.Location = new System.Drawing.Point(750, 16);
            this.button_CopyOutput.Name = "button_CopyOutput";
            this.button_CopyOutput.Size = new System.Drawing.Size(110, 32);
            this.button_CopyOutput.TabIndex = 5;
            this.button_CopyOutput.Text = "📋  Copy All";
            this.button_CopyOutput.UseVisualStyleBackColor = false;
            this.button_CopyOutput.Click += new System.EventHandler(this.button_CopyOutput_Click);
            // 
            // button_SaveReport
            // 
            this.button_SaveReport.BackColor = System.Drawing.Color.White;
            this.button_SaveReport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_SaveReport.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_SaveReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SaveReport.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_SaveReport.Location = new System.Drawing.Point(875, 16);
            this.button_SaveReport.Name = "button_SaveReport";
            this.button_SaveReport.Size = new System.Drawing.Size(130, 32);
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
            this.button_ClearOutput.Location = new System.Drawing.Point(1020, 16);
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
            this.panel_UrlBar.Size = new System.Drawing.Size(1200, 52);
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
            this.textBox_Url.Size = new System.Drawing.Size(1060, 27);
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
            this.tabControl_Main.Size = new System.Drawing.Size(1200, 80);
            this.tabControl_Main.TabIndex = 1;
            // 
            // tabPage_Scanner
            // 
            this.tabPage_Scanner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Scanner.Controls.Add(this.button_Scan);
            this.tabPage_Scanner.Controls.Add(this.button_StopScan);
            this.tabPage_Scanner.Controls.Add(this.label_ScanInfo);
            this.tabPage_Scanner.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Scanner.Name = "tabPage_Scanner";
            this.tabPage_Scanner.Size = new System.Drawing.Size(1192, 46);
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
            this.button_Scan.Location = new System.Drawing.Point(12, 6);
            this.button_Scan.Name = "button_Scan";
            this.button_Scan.Size = new System.Drawing.Size(130, 34);
            this.button_Scan.TabIndex = 0;
            this.button_Scan.Text = "▶  Start Scan";
            this.button_Scan.UseVisualStyleBackColor = false;
            this.button_Scan.Click += new System.EventHandler(this.button_Scan_Click);
            // 
            // button_StopScan
            // 
            this.button_StopScan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.button_StopScan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_StopScan.Enabled = false;
            this.button_StopScan.FlatAppearance.BorderSize = 0;
            this.button_StopScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_StopScan.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_StopScan.ForeColor = System.Drawing.Color.White;
            this.button_StopScan.Location = new System.Drawing.Point(155, 6);
            this.button_StopScan.Name = "button_StopScan";
            this.button_StopScan.Size = new System.Drawing.Size(110, 34);
            this.button_StopScan.TabIndex = 1;
            this.button_StopScan.Text = "■  Stop";
            this.button_StopScan.UseVisualStyleBackColor = false;
            this.button_StopScan.Click += new System.EventHandler(this.button_StopScan_Click);
            // 
            // label_ScanInfo
            // 
            this.label_ScanInfo.AutoSize = true;
            this.label_ScanInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_ScanInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_ScanInfo.Location = new System.Drawing.Point(280, 13);
            this.label_ScanInfo.Name = "label_ScanInfo";
            this.label_ScanInfo.Size = new System.Drawing.Size(672, 20);
            this.label_ScanInfo.TabIndex = 2;
            this.label_ScanInfo.Text = "HTTPS · Headers · Cookies · SQLi · XSS · Ports · 60+ Sensitive Paths · Crawl (dep" +
    "th 2) · JS link extraction";
            // 
            // tabPage_Headers
            // 
            this.tabPage_Headers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Headers.Controls.Add(this.button_AnalyzeHeaders);
            this.tabPage_Headers.Controls.Add(this.label_HeaderInfo);
            this.tabPage_Headers.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Headers.Name = "tabPage_Headers";
            this.tabPage_Headers.Size = new System.Drawing.Size(1192, 46);
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
            this.button_AnalyzeHeaders.Location = new System.Drawing.Point(12, 6);
            this.button_AnalyzeHeaders.Name = "button_AnalyzeHeaders";
            this.button_AnalyzeHeaders.Size = new System.Drawing.Size(150, 34);
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
            this.label_HeaderInfo.Location = new System.Drawing.Point(175, 13);
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
            this.tabPage_Dns.Size = new System.Drawing.Size(1192, 46);
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
            this.button_DnsLookup.Location = new System.Drawing.Point(12, 6);
            this.button_DnsLookup.Name = "button_DnsLookup";
            this.button_DnsLookup.Size = new System.Drawing.Size(140, 34);
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
            this.label_DnsInfo.Location = new System.Drawing.Point(165, 13);
            this.label_DnsInfo.Name = "label_DnsInfo";
            this.label_DnsInfo.Size = new System.Drawing.Size(392, 20);
            this.label_DnsInfo.TabIndex = 1;
            this.label_DnsInfo.Text = "Resolves IP addresses, hostname and aliases for the target";
            // 
            // panel_Output
            // 
            this.panel_Output.BackColor = System.Drawing.Color.White;
            this.panel_Output.Controls.Add(this.dataGridView_Output);
            this.panel_Output.Controls.Add(this.label_Output);
            this.panel_Output.Controls.Add(this.progressBar_Scan);
            this.panel_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Output.Location = new System.Drawing.Point(0, 196);
            this.panel_Output.Name = "panel_Output";
            this.panel_Output.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.panel_Output.Size = new System.Drawing.Size(1200, 504);
            this.panel_Output.TabIndex = 0;
            // 
            // dataGridView_Output
            // 
            this.dataGridView_Output.AllowUserToAddRows = false;
            this.dataGridView_Output.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(48)))));
            this.dataGridView_Output.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_Output.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.dataGridView_Output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_Output.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGridView_Output.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView_Output.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.dataGridView_Output.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_Output.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Output.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colStatus,
            this.colResponse});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_Output.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Output.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.dataGridView_Output.Location = new System.Drawing.Point(12, 28);
            this.dataGridView_Output.Name = "dataGridView_Output";
            this.dataGridView_Output.ReadOnly = true;
            this.dataGridView_Output.RowHeadersVisible = false;
            this.dataGridView_Output.RowHeadersWidth = 51;
            this.dataGridView_Output.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_Output.Size = new System.Drawing.Size(1176, 464);
            this.dataGridView_Output.TabIndex = 0;
            this.dataGridView_Output.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView_Output_MouseDown);
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colName.HeaderText = "Name / Path";
            this.colName.MinimumWidth = 200;
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 320;
            // 
            // colStatus
            // 
            this.colStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colStatus.HeaderText = "Status";
            this.colStatus.MinimumWidth = 140;
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 200;
            // 
            // colResponse
            // 
            this.colResponse.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colResponse.HeaderText = "Response / Detail";
            this.colResponse.MinimumWidth = 300;
            this.colResponse.Name = "colResponse";
            this.colResponse.ReadOnly = true;
            // 
            // label_Output
            // 
            this.label_Output.AutoSize = true;
            this.label_Output.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_Output.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_Output.Location = new System.Drawing.Point(12, 8);
            this.label_Output.Name = "label_Output";
            this.label_Output.Size = new System.Drawing.Size(296, 20);
            this.label_Output.TabIndex = 1;
            this.label_Output.Text = "SCAN OUTPUT  (right-click to copy rows)";
            // 
            // progressBar_Scan
            // 
            this.progressBar_Scan.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar_Scan.Location = new System.Drawing.Point(12, 492);
            this.progressBar_Scan.MarqueeAnimationSpeed = 30;
            this.progressBar_Scan.Name = "progressBar_Scan";
            this.progressBar_Scan.Size = new System.Drawing.Size(1176, 4);
            this.progressBar_Scan.TabIndex = 2;
            // 
            // FullScannerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.panel_Output);
            this.Controls.Add(this.tabControl_Main);
            this.Controls.Add(this.panel_UrlBar);
            this.Controls.Add(this.panel_TopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1000, 640);
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Output)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        // ── Field declarations ────────────────────────────────────────────────────
        private System.Windows.Forms.Panel panel_TopBar;
        private System.Windows.Forms.Label label_AppTitle;
        private System.Windows.Forms.Label label_AppSubtitle;
        private System.Windows.Forms.Button button_SaveReport;
        private System.Windows.Forms.Button button_ClearOutput;
        private System.Windows.Forms.Button button_CopyOutput;
        private System.Windows.Forms.Panel panel_UrlBar;
        private System.Windows.Forms.Label label_UrlTitle;
        private System.Windows.Forms.TextBox textBox_Url;
        private System.Windows.Forms.TabControl tabControl_Main;
        private System.Windows.Forms.TabPage tabPage_Scanner;
        private System.Windows.Forms.Button button_Scan;
        private System.Windows.Forms.Button button_StopScan;
        private System.Windows.Forms.Label label_ScanInfo;
        private System.Windows.Forms.TabPage tabPage_Headers;
        private System.Windows.Forms.Button button_AnalyzeHeaders;
        private System.Windows.Forms.Label label_HeaderInfo;
        private System.Windows.Forms.TabPage tabPage_Dns;
        private System.Windows.Forms.Button button_DnsLookup;
        private System.Windows.Forms.Label label_DnsInfo;
        private System.Windows.Forms.Panel panel_Output;
        private System.Windows.Forms.Label label_Output;
        private System.Windows.Forms.DataGridView dataGridView_Output;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colResponse;
        private System.Windows.Forms.ProgressBar progressBar_Scan;
    }
}
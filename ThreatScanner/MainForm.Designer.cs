namespace ThreatScanner
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
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
            this.tabPage_BruteForce = new System.Windows.Forms.TabPage();
            this.button_BruteForce = new System.Windows.Forms.Button();
            this.label_BruteSection1 = new System.Windows.Forms.Label();
            this.label_Username = new System.Windows.Forms.Label();
            this.textBox_Username = new System.Windows.Forms.TextBox();
            this.label_Password = new System.Windows.Forms.Label();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.label_Wordlist = new System.Windows.Forms.Label();
            this.textBox_WordlistPath = new System.Windows.Forms.TextBox();
            this.button_BrowseWordlist = new System.Windows.Forms.Button();
            this.label_BruteInfo = new System.Windows.Forms.Label();
            this.tabPage_Api = new System.Windows.Forms.TabPage();
            this.label_ApiMethod = new System.Windows.Forms.Label();
            this.comboBox_Method = new System.Windows.Forms.ComboBox();
            this.label_ApiEndpoint = new System.Windows.Forms.Label();
            this.textBox_ApiEndpoint = new System.Windows.Forms.TextBox();
            this.button_ApiForce = new System.Windows.Forms.Button();
            this.tabControl_ApiDetail = new System.Windows.Forms.TabControl();
            this.tabPage_ApiParams = new System.Windows.Forms.TabPage();
            this.dataGridView_Params = new System.Windows.Forms.DataGridView();
            this.col_ParamEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.col_ParamKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ParamValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ParamDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage_ApiHeaders = new System.Windows.Forms.TabPage();
            this.dataGridView_Headers = new System.Windows.Forms.DataGridView();
            this.col_HdrEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.col_HdrKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_HdrValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage_ApiBody = new System.Windows.Forms.TabPage();
            this.dataGridView_FormData = new System.Windows.Forms.DataGridView();
            this.col_FormKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_FormValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox_Body = new System.Windows.Forms.TextBox();
            this.panel_BodyType = new System.Windows.Forms.Panel();
            this.radioButton_BodyNone = new System.Windows.Forms.RadioButton();
            this.radioButton_BodyForm = new System.Windows.Forms.RadioButton();
            this.radioButton_BodyJson = new System.Windows.Forms.RadioButton();
            this.radioButton_BodyRaw = new System.Windows.Forms.RadioButton();
            this.tabPage_ApiAuth = new System.Windows.Forms.TabPage();
            this.label_AuthType = new System.Windows.Forms.Label();
            this.comboBox_AuthType = new System.Windows.Forms.ComboBox();
            this.label_AuthKey = new System.Windows.Forms.Label();
            this.textBox_HeaderKey = new System.Windows.Forms.TextBox();
            this.label_AuthValue = new System.Windows.Forms.Label();
            this.textBox_HeaderValue = new System.Windows.Forms.TextBox();
            this.tabPage_ApiWordlist = new System.Windows.Forms.TabPage();
            this.label_ApiWordlistInfo = new System.Windows.Forms.Label();
            this.label_ApiWlTarget = new System.Windows.Forms.Label();
            this.textBox_ApiWlTarget = new System.Windows.Forms.TextBox();
            this.checkBox_UseQuery = new System.Windows.Forms.CheckBox();
            this.checkBox_IsGaetMethod = new System.Windows.Forms.CheckBox();
            this.checkBox_Json = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
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
            this.tabPage_BruteForce.SuspendLayout();
            this.tabPage_Api.SuspendLayout();
            this.tabControl_ApiDetail.SuspendLayout();
            this.tabPage_ApiParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Params)).BeginInit();
            this.tabPage_ApiHeaders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Headers)).BeginInit();
            this.tabPage_ApiBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_FormData)).BeginInit();
            this.panel_BodyType.SuspendLayout();
            this.tabPage_ApiAuth.SuspendLayout();
            this.tabPage_ApiWordlist.SuspendLayout();
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
            this.panel_TopBar.Size = new System.Drawing.Size(1280, 64);
            this.panel_TopBar.TabIndex = 0;
            // 
            // label_AppTitle
            // 
            this.label_AppTitle.AutoSize = true;
            this.label_AppTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label_AppTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.label_AppTitle.Location = new System.Drawing.Point(16, 12);
            this.label_AppTitle.Name = "label_AppTitle";
            this.label_AppTitle.Size = new System.Drawing.Size(219, 32);
            this.label_AppTitle.TabIndex = 0;
            this.label_AppTitle.Text = "⚡ ThreatScanner";
            // 
            // label_AppSubtitle
            // 
            this.label_AppSubtitle.AutoSize = true;
            this.label_AppSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_AppSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AppSubtitle.Location = new System.Drawing.Point(18, 40);
            this.label_AppSubtitle.Name = "label_AppSubtitle";
            this.label_AppSubtitle.Size = new System.Drawing.Size(425, 20);
            this.label_AppSubtitle.TabIndex = 1;
            this.label_AppSubtitle.Text = "Web Vulnerability Testing Toolkit  •  For authorized testing only";
            // 
            // button_SaveReport
            // 
            this.button_SaveReport.BackColor = System.Drawing.Color.White;
            this.button_SaveReport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_SaveReport.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_SaveReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SaveReport.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_SaveReport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.button_SaveReport.Location = new System.Drawing.Point(1140, 16);
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
            this.button_ClearOutput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.button_ClearOutput.Location = new System.Drawing.Point(1020, 16);
            this.button_ClearOutput.Name = "button_ClearOutput";
            this.button_ClearOutput.Size = new System.Drawing.Size(110, 32);
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
            this.panel_UrlBar.Size = new System.Drawing.Size(1280, 52);
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
            this.textBox_Url.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.textBox_Url.Location = new System.Drawing.Point(120, 12);
            this.textBox_Url.Name = "textBox_Url";
            this.textBox_Url.Size = new System.Drawing.Size(1140, 27);
            this.textBox_Url.TabIndex = 1;
            // 
            // tabControl_Main
            // 
            this.tabControl_Main.Controls.Add(this.tabPage_Scanner);
            this.tabControl_Main.Controls.Add(this.tabPage_BruteForce);
            this.tabControl_Main.Controls.Add(this.tabPage_Api);
            this.tabControl_Main.Controls.Add(this.tabPage_Headers);
            this.tabControl_Main.Controls.Add(this.tabPage_Dns);
            this.tabControl_Main.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl_Main.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.tabControl_Main.Location = new System.Drawing.Point(0, 116);
            this.tabControl_Main.Name = "tabControl_Main";
            this.tabControl_Main.SelectedIndex = 0;
            this.tabControl_Main.Size = new System.Drawing.Size(1280, 263);
            this.tabControl_Main.TabIndex = 1;
            // 
            // tabPage_Scanner
            // 
            this.tabPage_Scanner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Scanner.Controls.Add(this.button_Scan);
            this.tabPage_Scanner.Controls.Add(this.label_ScanInfo);
            this.tabPage_Scanner.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Scanner.Name = "tabPage_Scanner";
            this.tabPage_Scanner.Padding = new System.Windows.Forms.Padding(16);
            this.tabPage_Scanner.Size = new System.Drawing.Size(1272, 229);
            this.tabPage_Scanner.TabIndex = 0;
            this.tabPage_Scanner.Text = " 🔍  Full Scanner";
            // 
            // button_Scan
            // 
            this.button_Scan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.button_Scan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Scan.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.button_Scan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Scan.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.button_Scan.ForeColor = System.Drawing.Color.White;
            this.button_Scan.Location = new System.Drawing.Point(16, 16);
            this.button_Scan.Name = "button_Scan";
            this.button_Scan.Size = new System.Drawing.Size(180, 44);
            this.button_Scan.TabIndex = 0;
            this.button_Scan.Text = "▶  Run Full Scan";
            this.button_Scan.UseVisualStyleBackColor = false;
            this.button_Scan.Click += new System.EventHandler(this.button_Scan_Click);
            // 
            // label_ScanInfo
            // 
            this.label_ScanInfo.AutoSize = true;
            this.label_ScanInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_ScanInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_ScanInfo.Location = new System.Drawing.Point(210, 26);
            this.label_ScanInfo.Name = "label_ScanInfo";
            this.label_ScanInfo.Size = new System.Drawing.Size(936, 20);
            this.label_ScanInfo.TabIndex = 1;
            this.label_ScanInfo.Text = "Checks: HTTPS  •  Security Headers  •  Cookie Flags  •  Redirects  •  localStorag" +
    "e  •  SQLi Hints  •  XSS Reflection  •  Port Scan  •  Sensitive Files";
            // 
            // tabPage_BruteForce
            // 
            this.tabPage_BruteForce.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_BruteForce.Controls.Add(this.button_BruteForce);
            this.tabPage_BruteForce.Controls.Add(this.label_BruteSection1);
            this.tabPage_BruteForce.Controls.Add(this.label_Username);
            this.tabPage_BruteForce.Controls.Add(this.textBox_Username);
            this.tabPage_BruteForce.Controls.Add(this.label_Password);
            this.tabPage_BruteForce.Controls.Add(this.textBox_Password);
            this.tabPage_BruteForce.Controls.Add(this.label_Wordlist);
            this.tabPage_BruteForce.Controls.Add(this.textBox_WordlistPath);
            this.tabPage_BruteForce.Controls.Add(this.button_BrowseWordlist);
            this.tabPage_BruteForce.Controls.Add(this.label_BruteInfo);
            this.tabPage_BruteForce.Location = new System.Drawing.Point(4, 30);
            this.tabPage_BruteForce.Name = "tabPage_BruteForce";
            this.tabPage_BruteForce.Padding = new System.Windows.Forms.Padding(16);
            this.tabPage_BruteForce.Size = new System.Drawing.Size(1272, 229);
            this.tabPage_BruteForce.TabIndex = 1;
            this.tabPage_BruteForce.Text = " 🔐  Brute Force";
            // 
            // button_BruteForce
            // 
            this.button_BruteForce.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.button_BruteForce.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_BruteForce.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.button_BruteForce.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_BruteForce.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_BruteForce.ForeColor = System.Drawing.Color.White;
            this.button_BruteForce.Location = new System.Drawing.Point(16, 172);
            this.button_BruteForce.Name = "button_BruteForce";
            this.button_BruteForce.Size = new System.Drawing.Size(180, 44);
            this.button_BruteForce.TabIndex = 0;
            this.button_BruteForce.Text = "🔓  Start Attack";
            this.button_BruteForce.UseVisualStyleBackColor = false;
            this.button_BruteForce.Click += new System.EventHandler(this.button_BruteForce_Click);
            // 
            // label_BruteSection1
            // 
            this.label_BruteSection1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.label_BruteSection1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_BruteSection1.Location = new System.Drawing.Point(16, 16);
            this.label_BruteSection1.Name = "label_BruteSection1";
            this.label_BruteSection1.Size = new System.Drawing.Size(600, 18);
            this.label_BruteSection1.TabIndex = 1;
            this.label_BruteSection1.Text = "CREDENTIALS";
            // 
            // label_Username
            // 
            this.label_Username.AutoSize = true;
            this.label_Username.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_Username.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_Username.Location = new System.Drawing.Point(16, 40);
            this.label_Username.Name = "label_Username";
            this.label_Username.Size = new System.Drawing.Size(86, 20);
            this.label_Username.TabIndex = 2;
            this.label_Username.Text = "USERNAME";
            // 
            // textBox_Username
            // 
            this.textBox_Username.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Username.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBox_Username.Location = new System.Drawing.Point(16, 60);
            this.textBox_Username.Name = "textBox_Username";
            this.textBox_Username.Size = new System.Drawing.Size(220, 30);
            this.textBox_Username.TabIndex = 3;
            // 
            // label_Password
            // 
            this.label_Password.AutoSize = true;
            this.label_Password.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_Password.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_Password.Location = new System.Drawing.Point(252, 40);
            this.label_Password.Name = "label_Password";
            this.label_Password.Size = new System.Drawing.Size(87, 20);
            this.label_Password.TabIndex = 4;
            this.label_Password.Text = "PASSWORD";
            // 
            // textBox_Password
            // 
            this.textBox_Password.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Password.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBox_Password.Location = new System.Drawing.Point(252, 60);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.PasswordChar = '●';
            this.textBox_Password.Size = new System.Drawing.Size(220, 30);
            this.textBox_Password.TabIndex = 5;
            // 
            // label_Wordlist
            // 
            this.label_Wordlist.AutoSize = true;
            this.label_Wordlist.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_Wordlist.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_Wordlist.Location = new System.Drawing.Point(16, 106);
            this.label_Wordlist.Name = "label_Wordlist";
            this.label_Wordlist.Size = new System.Drawing.Size(398, 20);
            this.label_Wordlist.TabIndex = 10;
            this.label_Wordlist.Text = "WORDLIST  (leave blank to use the single password above)";
            // 
            // textBox_WordlistPath
            // 
            this.textBox_WordlistPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_WordlistPath.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.textBox_WordlistPath.Location = new System.Drawing.Point(16, 126);
            this.textBox_WordlistPath.Name = "textBox_WordlistPath";
            this.textBox_WordlistPath.Size = new System.Drawing.Size(520, 26);
            this.textBox_WordlistPath.TabIndex = 11;
            // 
            // button_BrowseWordlist
            // 
            this.button_BrowseWordlist.BackColor = System.Drawing.Color.White;
            this.button_BrowseWordlist.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_BrowseWordlist.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_BrowseWordlist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_BrowseWordlist.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_BrowseWordlist.Location = new System.Drawing.Point(544, 124);
            this.button_BrowseWordlist.Name = "button_BrowseWordlist";
            this.button_BrowseWordlist.Size = new System.Drawing.Size(90, 28);
            this.button_BrowseWordlist.TabIndex = 12;
            this.button_BrowseWordlist.Text = "Browse…";
            this.button_BrowseWordlist.UseVisualStyleBackColor = false;
            this.button_BrowseWordlist.Click += new System.EventHandler(this.button_BrowseWordlist_Click);
            // 
            // label_BruteInfo
            // 
            this.label_BruteInfo.AutoSize = true;
            this.label_BruteInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_BruteInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(100)))), ((int)(((byte)(0)))));
            this.label_BruteInfo.Location = new System.Drawing.Point(210, 183);
            this.label_BruteInfo.Name = "label_BruteInfo";
            this.label_BruteInfo.Size = new System.Drawing.Size(439, 20);
            this.label_BruteInfo.TabIndex = 13;
            this.label_BruteInfo.Text = "⚠  Only test systems you own or have written permission to test.";
            // 
            // tabPage_Api
            // 
            this.tabPage_Api.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Api.Controls.Add(this.label_ApiMethod);
            this.tabPage_Api.Controls.Add(this.comboBox_Method);
            this.tabPage_Api.Controls.Add(this.label_ApiEndpoint);
            this.tabPage_Api.Controls.Add(this.textBox_ApiEndpoint);
            this.tabPage_Api.Controls.Add(this.button_ApiForce);
            this.tabPage_Api.Controls.Add(this.tabControl_ApiDetail);
            this.tabPage_Api.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Api.Name = "tabPage_Api";
            this.tabPage_Api.Padding = new System.Windows.Forms.Padding(12);
            this.tabPage_Api.Size = new System.Drawing.Size(1272, 229);
            this.tabPage_Api.TabIndex = 2;
            this.tabPage_Api.Text = " 🛰  API Tester";
            // 
            // label_ApiMethod
            // 
            this.label_ApiMethod.AutoSize = true;
            this.label_ApiMethod.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_ApiMethod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_ApiMethod.Location = new System.Drawing.Point(12, 14);
            this.label_ApiMethod.Name = "label_ApiMethod";
            this.label_ApiMethod.Size = new System.Drawing.Size(73, 20);
            this.label_ApiMethod.TabIndex = 0;
            this.label_ApiMethod.Text = "METHOD";
            // 
            // comboBox_Method
            // 
            this.comboBox_Method.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Method.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.comboBox_Method.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.comboBox_Method.Items.AddRange(new object[] {
            "GET",
            "POST",
            "PUT",
            "PATCH",
            "DELETE",
            "HEAD",
            "OPTIONS"});
            this.comboBox_Method.Location = new System.Drawing.Point(12, 34);
            this.comboBox_Method.Name = "comboBox_Method";
            this.comboBox_Method.Size = new System.Drawing.Size(110, 31);
            this.comboBox_Method.TabIndex = 1;
            // 
            // label_ApiEndpoint
            // 
            this.label_ApiEndpoint.AutoSize = true;
            this.label_ApiEndpoint.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_ApiEndpoint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_ApiEndpoint.Location = new System.Drawing.Point(132, 14);
            this.label_ApiEndpoint.Name = "label_ApiEndpoint";
            this.label_ApiEndpoint.Size = new System.Drawing.Size(392, 20);
            this.label_ApiEndpoint.TabIndex = 2;
            this.label_ApiEndpoint.Text = "ENDPOINT  (overrides TARGET URL above when filled)";
            // 
            // textBox_ApiEndpoint
            // 
            this.textBox_ApiEndpoint.BackColor = System.Drawing.Color.White;
            this.textBox_ApiEndpoint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_ApiEndpoint.Font = new System.Drawing.Font("Consolas", 10F);
            this.textBox_ApiEndpoint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.textBox_ApiEndpoint.Location = new System.Drawing.Point(132, 34);
            this.textBox_ApiEndpoint.Name = "textBox_ApiEndpoint";
            this.textBox_ApiEndpoint.Size = new System.Drawing.Size(940, 27);
            this.textBox_ApiEndpoint.TabIndex = 3;
            // 
            // button_ApiForce
            // 
            this.button_ApiForce.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.button_ApiForce.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_ApiForce.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.button_ApiForce.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_ApiForce.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_ApiForce.ForeColor = System.Drawing.Color.White;
            this.button_ApiForce.Location = new System.Drawing.Point(1078, 26);
            this.button_ApiForce.Name = "button_ApiForce";
            this.button_ApiForce.Size = new System.Drawing.Size(127, 39);
            this.button_ApiForce.TabIndex = 4;
            this.button_ApiForce.Text = "▶  Send";
            this.button_ApiForce.UseVisualStyleBackColor = false;
            this.button_ApiForce.Click += new System.EventHandler(this.button_ApiForce_Click);
            // 
            // tabControl_ApiDetail
            // 
            this.tabControl_ApiDetail.Controls.Add(this.tabPage_ApiParams);
            this.tabControl_ApiDetail.Controls.Add(this.tabPage_ApiHeaders);
            this.tabControl_ApiDetail.Controls.Add(this.tabPage_ApiBody);
            this.tabControl_ApiDetail.Controls.Add(this.tabPage_ApiAuth);
            this.tabControl_ApiDetail.Controls.Add(this.tabPage_ApiWordlist);
            this.tabControl_ApiDetail.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabControl_ApiDetail.Location = new System.Drawing.Point(12, 72);
            this.tabControl_ApiDetail.Name = "tabControl_ApiDetail";
            this.tabControl_ApiDetail.SelectedIndex = 0;
            this.tabControl_ApiDetail.Size = new System.Drawing.Size(1248, 222);
            this.tabControl_ApiDetail.TabIndex = 10;
            // 
            // tabPage_ApiParams
            // 
            this.tabPage_ApiParams.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_ApiParams.Controls.Add(this.dataGridView_Params);
            this.tabPage_ApiParams.Location = new System.Drawing.Point(4, 29);
            this.tabPage_ApiParams.Name = "tabPage_ApiParams";
            this.tabPage_ApiParams.Size = new System.Drawing.Size(1240, 189);
            this.tabPage_ApiParams.TabIndex = 0;
            this.tabPage_ApiParams.Text = "  Params";
            // 
            // dataGridView_Params
            // 
            this.dataGridView_Params.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.dataGridView_Params.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_Params.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Params.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_ParamEnabled,
            this.col_ParamKey,
            this.col_ParamValue,
            this.col_ParamDesc});
            this.dataGridView_Params.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Params.Font = new System.Drawing.Font("Consolas", 9F);
            this.dataGridView_Params.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dataGridView_Params.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_Params.Name = "dataGridView_Params";
            this.dataGridView_Params.RowHeadersVisible = false;
            this.dataGridView_Params.RowHeadersWidth = 51;
            this.dataGridView_Params.RowTemplate.Height = 26;
            this.dataGridView_Params.Size = new System.Drawing.Size(1240, 189);
            this.dataGridView_Params.TabIndex = 0;
            // 
            // col_ParamEnabled
            // 
            this.col_ParamEnabled.FalseValue = false;
            this.col_ParamEnabled.HeaderText = "";
            this.col_ParamEnabled.MinimumWidth = 6;
            this.col_ParamEnabled.Name = "col_ParamEnabled";
            this.col_ParamEnabled.TrueValue = true;
            this.col_ParamEnabled.Width = 30;
            // 
            // col_ParamKey
            // 
            this.col_ParamKey.HeaderText = "Key";
            this.col_ParamKey.MinimumWidth = 6;
            this.col_ParamKey.Name = "col_ParamKey";
            this.col_ParamKey.Width = 280;
            // 
            // col_ParamValue
            // 
            this.col_ParamValue.HeaderText = "Value";
            this.col_ParamValue.MinimumWidth = 6;
            this.col_ParamValue.Name = "col_ParamValue";
            this.col_ParamValue.Width = 400;
            // 
            // col_ParamDesc
            // 
            this.col_ParamDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col_ParamDesc.HeaderText = "Description";
            this.col_ParamDesc.MinimumWidth = 6;
            this.col_ParamDesc.Name = "col_ParamDesc";
            // 
            // tabPage_ApiHeaders
            // 
            this.tabPage_ApiHeaders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_ApiHeaders.Controls.Add(this.dataGridView_Headers);
            this.tabPage_ApiHeaders.Location = new System.Drawing.Point(4, 29);
            this.tabPage_ApiHeaders.Name = "tabPage_ApiHeaders";
            this.tabPage_ApiHeaders.Size = new System.Drawing.Size(1240, 189);
            this.tabPage_ApiHeaders.TabIndex = 1;
            this.tabPage_ApiHeaders.Text = "  Headers";
            // 
            // dataGridView_Headers
            // 
            this.dataGridView_Headers.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.dataGridView_Headers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_Headers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Headers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_HdrEnabled,
            this.col_HdrKey,
            this.col_HdrValue});
            this.dataGridView_Headers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Headers.Font = new System.Drawing.Font("Consolas", 9F);
            this.dataGridView_Headers.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dataGridView_Headers.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_Headers.Name = "dataGridView_Headers";
            this.dataGridView_Headers.RowHeadersVisible = false;
            this.dataGridView_Headers.RowHeadersWidth = 51;
            this.dataGridView_Headers.RowTemplate.Height = 26;
            this.dataGridView_Headers.Size = new System.Drawing.Size(1240, 189);
            this.dataGridView_Headers.TabIndex = 0;
            // 
            // col_HdrEnabled
            // 
            this.col_HdrEnabled.HeaderText = "";
            this.col_HdrEnabled.MinimumWidth = 6;
            this.col_HdrEnabled.Name = "col_HdrEnabled";
            this.col_HdrEnabled.Width = 30;
            // 
            // col_HdrKey
            // 
            this.col_HdrKey.HeaderText = "Header";
            this.col_HdrKey.MinimumWidth = 6;
            this.col_HdrKey.Name = "col_HdrKey";
            this.col_HdrKey.Width = 320;
            // 
            // col_HdrValue
            // 
            this.col_HdrValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col_HdrValue.HeaderText = "Value";
            this.col_HdrValue.MinimumWidth = 6;
            this.col_HdrValue.Name = "col_HdrValue";
            // 
            // tabPage_ApiBody
            // 
            this.tabPage_ApiBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_ApiBody.Controls.Add(this.dataGridView_FormData);
            this.tabPage_ApiBody.Controls.Add(this.textBox_Body);
            this.tabPage_ApiBody.Controls.Add(this.panel_BodyType);
            this.tabPage_ApiBody.Location = new System.Drawing.Point(4, 29);
            this.tabPage_ApiBody.Name = "tabPage_ApiBody";
            this.tabPage_ApiBody.Size = new System.Drawing.Size(1240, 189);
            this.tabPage_ApiBody.TabIndex = 2;
            this.tabPage_ApiBody.Text = "  Body";
            // 
            // dataGridView_FormData
            // 
            this.dataGridView_FormData.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.dataGridView_FormData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_FormData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_FormData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_FormKey,
            this.col_FormValue});
            this.dataGridView_FormData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_FormData.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.dataGridView_FormData.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dataGridView_FormData.Location = new System.Drawing.Point(0, 32);
            this.dataGridView_FormData.Name = "dataGridView_FormData";
            this.dataGridView_FormData.RowHeadersVisible = false;
            this.dataGridView_FormData.RowHeadersWidth = 51;
            this.dataGridView_FormData.RowTemplate.Height = 26;
            this.dataGridView_FormData.Size = new System.Drawing.Size(1240, 157);
            this.dataGridView_FormData.TabIndex = 2;
            this.dataGridView_FormData.Visible = false;
            // 
            // col_FormKey
            // 
            this.col_FormKey.HeaderText = "Key";
            this.col_FormKey.MinimumWidth = 6;
            this.col_FormKey.Name = "col_FormKey";
            this.col_FormKey.Width = 320;
            // 
            // col_FormValue
            // 
            this.col_FormValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col_FormValue.HeaderText = "Value";
            this.col_FormValue.MinimumWidth = 6;
            this.col_FormValue.Name = "col_FormValue";
            // 
            // textBox_Body
            // 
            this.textBox_Body.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.textBox_Body.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_Body.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Body.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.textBox_Body.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(230)))), ((int)(((byte)(253)))));
            this.textBox_Body.Location = new System.Drawing.Point(0, 32);
            this.textBox_Body.Multiline = true;
            this.textBox_Body.Name = "textBox_Body";
            this.textBox_Body.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Body.Size = new System.Drawing.Size(1240, 157);
            this.textBox_Body.TabIndex = 0;
            this.textBox_Body.Visible = false;
            this.textBox_Body.WordWrap = false;
            // 
            // panel_BodyType
            // 
            this.panel_BodyType.Controls.Add(this.radioButton_BodyNone);
            this.panel_BodyType.Controls.Add(this.radioButton_BodyForm);
            this.panel_BodyType.Controls.Add(this.radioButton_BodyJson);
            this.panel_BodyType.Controls.Add(this.radioButton_BodyRaw);
            this.panel_BodyType.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_BodyType.Location = new System.Drawing.Point(0, 0);
            this.panel_BodyType.Name = "panel_BodyType";
            this.panel_BodyType.Size = new System.Drawing.Size(1240, 32);
            this.panel_BodyType.TabIndex = 1;
            // 
            // radioButton_BodyNone
            // 
            this.radioButton_BodyNone.AutoSize = true;
            this.radioButton_BodyNone.Checked = true;
            this.radioButton_BodyNone.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioButton_BodyNone.Location = new System.Drawing.Point(8, 6);
            this.radioButton_BodyNone.Name = "radioButton_BodyNone";
            this.radioButton_BodyNone.Size = new System.Drawing.Size(63, 24);
            this.radioButton_BodyNone.TabIndex = 0;
            this.radioButton_BodyNone.TabStop = true;
            this.radioButton_BodyNone.Text = "none";
            // 
            // radioButton_BodyForm
            // 
            this.radioButton_BodyForm.AutoSize = true;
            this.radioButton_BodyForm.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioButton_BodyForm.Location = new System.Drawing.Point(72, 6);
            this.radioButton_BodyForm.Name = "radioButton_BodyForm";
            this.radioButton_BodyForm.Size = new System.Drawing.Size(98, 24);
            this.radioButton_BodyForm.TabIndex = 1;
            this.radioButton_BodyForm.Text = "form-data";
            // 
            // radioButton_BodyJson
            // 
            this.radioButton_BodyJson.AutoSize = true;
            this.radioButton_BodyJson.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioButton_BodyJson.Location = new System.Drawing.Point(164, 6);
            this.radioButton_BodyJson.Name = "radioButton_BodyJson";
            this.radioButton_BodyJson.Size = new System.Drawing.Size(65, 24);
            this.radioButton_BodyJson.TabIndex = 2;
            this.radioButton_BodyJson.Text = "JSON";
            // 
            // radioButton_BodyRaw
            // 
            this.radioButton_BodyRaw.AutoSize = true;
            this.radioButton_BodyRaw.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioButton_BodyRaw.Location = new System.Drawing.Point(234, 6);
            this.radioButton_BodyRaw.Name = "radioButton_BodyRaw";
            this.radioButton_BodyRaw.Size = new System.Drawing.Size(54, 24);
            this.radioButton_BodyRaw.TabIndex = 3;
            this.radioButton_BodyRaw.Text = "raw";
            // 
            // tabPage_ApiAuth
            // 
            this.tabPage_ApiAuth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_ApiAuth.Controls.Add(this.label_AuthType);
            this.tabPage_ApiAuth.Controls.Add(this.comboBox_AuthType);
            this.tabPage_ApiAuth.Controls.Add(this.label_AuthKey);
            this.tabPage_ApiAuth.Controls.Add(this.textBox_HeaderKey);
            this.tabPage_ApiAuth.Controls.Add(this.label_AuthValue);
            this.tabPage_ApiAuth.Controls.Add(this.textBox_HeaderValue);
            this.tabPage_ApiAuth.Location = new System.Drawing.Point(4, 29);
            this.tabPage_ApiAuth.Name = "tabPage_ApiAuth";
            this.tabPage_ApiAuth.Size = new System.Drawing.Size(1240, 189);
            this.tabPage_ApiAuth.TabIndex = 3;
            this.tabPage_ApiAuth.Text = "  Auth";
            // 
            // label_AuthType
            // 
            this.label_AuthType.AutoSize = true;
            this.label_AuthType.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_AuthType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AuthType.Location = new System.Drawing.Point(16, 16);
            this.label_AuthType.Name = "label_AuthType";
            this.label_AuthType.Size = new System.Drawing.Size(90, 20);
            this.label_AuthType.TabIndex = 0;
            this.label_AuthType.Text = "AUTH TYPE";
            // 
            // comboBox_AuthType
            // 
            this.comboBox_AuthType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_AuthType.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.comboBox_AuthType.Items.AddRange(new object[] {
            "No Auth",
            "Bearer Token",
            "API Key",
            "Basic Auth",
            "Custom Header"});
            this.comboBox_AuthType.Location = new System.Drawing.Point(16, 36);
            this.comboBox_AuthType.Name = "comboBox_AuthType";
            this.comboBox_AuthType.Size = new System.Drawing.Size(180, 29);
            this.comboBox_AuthType.TabIndex = 1;
            // 
            // label_AuthKey
            // 
            this.label_AuthKey.AutoSize = true;
            this.label_AuthKey.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_AuthKey.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AuthKey.Location = new System.Drawing.Point(216, 16);
            this.label_AuthKey.Name = "label_AuthKey";
            this.label_AuthKey.Size = new System.Drawing.Size(159, 20);
            this.label_AuthKey.TabIndex = 2;
            this.label_AuthKey.Text = "HEADER / KEY NAME";
            // 
            // textBox_HeaderKey
            // 
            this.textBox_HeaderKey.BackColor = System.Drawing.Color.White;
            this.textBox_HeaderKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_HeaderKey.Font = new System.Drawing.Font("Consolas", 10F);
            this.textBox_HeaderKey.Location = new System.Drawing.Point(216, 36);
            this.textBox_HeaderKey.Name = "textBox_HeaderKey";
            this.textBox_HeaderKey.Size = new System.Drawing.Size(280, 27);
            this.textBox_HeaderKey.TabIndex = 3;
            // 
            // label_AuthValue
            // 
            this.label_AuthValue.AutoSize = true;
            this.label_AuthValue.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_AuthValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AuthValue.Location = new System.Drawing.Point(512, 16);
            this.label_AuthValue.Name = "label_AuthValue";
            this.label_AuthValue.Size = new System.Drawing.Size(120, 20);
            this.label_AuthValue.TabIndex = 4;
            this.label_AuthValue.Text = "VALUE / TOKEN";
            // 
            // textBox_HeaderValue
            // 
            this.textBox_HeaderValue.BackColor = System.Drawing.Color.White;
            this.textBox_HeaderValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_HeaderValue.Font = new System.Drawing.Font("Consolas", 10F);
            this.textBox_HeaderValue.Location = new System.Drawing.Point(512, 36);
            this.textBox_HeaderValue.Name = "textBox_HeaderValue";
            this.textBox_HeaderValue.Size = new System.Drawing.Size(500, 27);
            this.textBox_HeaderValue.TabIndex = 5;
            // 
            // tabPage_ApiWordlist
            // 
            this.tabPage_ApiWordlist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_ApiWordlist.Controls.Add(this.label_ApiWordlistInfo);
            this.tabPage_ApiWordlist.Controls.Add(this.label_ApiWlTarget);
            this.tabPage_ApiWordlist.Controls.Add(this.textBox_ApiWlTarget);
            this.tabPage_ApiWordlist.Controls.Add(this.checkBox_UseQuery);
            this.tabPage_ApiWordlist.Controls.Add(this.checkBox_IsGaetMethod);
            this.tabPage_ApiWordlist.Controls.Add(this.checkBox_Json);
            this.tabPage_ApiWordlist.Controls.Add(this.checkBox1);
            this.tabPage_ApiWordlist.Location = new System.Drawing.Point(4, 29);
            this.tabPage_ApiWordlist.Name = "tabPage_ApiWordlist";
            this.tabPage_ApiWordlist.Size = new System.Drawing.Size(1240, 189);
            this.tabPage_ApiWordlist.TabIndex = 4;
            this.tabPage_ApiWordlist.Text = "  Wordlist Attack";
            // 
            // label_ApiWordlistInfo
            // 
            this.label_ApiWordlistInfo.AutoSize = true;
            this.label_ApiWordlistInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_ApiWordlistInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_ApiWordlistInfo.Location = new System.Drawing.Point(16, 10);
            this.label_ApiWordlistInfo.Name = "label_ApiWordlistInfo";
            this.label_ApiWordlistInfo.Size = new System.Drawing.Size(941, 20);
            this.label_ApiWordlistInfo.TabIndex = 0;
            this.label_ApiWordlistInfo.Text = "Wordlist is shared with the Brute Force tab. Each line is tried as the value inje" +
    "cted into the field below. The Send button above fires the attack.";
            // 
            // label_ApiWlTarget
            // 
            this.label_ApiWlTarget.AutoSize = true;
            this.label_ApiWlTarget.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_ApiWlTarget.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_ApiWlTarget.Location = new System.Drawing.Point(16, 42);
            this.label_ApiWlTarget.Name = "label_ApiWlTarget";
            this.label_ApiWlTarget.Size = new System.Drawing.Size(439, 20);
            this.label_ApiWlTarget.TabIndex = 1;
            this.label_ApiWlTarget.Text = "INJECT INTO FIELD (key=value pairs, use {value} placeholder)";
            // 
            // textBox_ApiWlTarget
            // 
            this.textBox_ApiWlTarget.BackColor = System.Drawing.Color.White;
            this.textBox_ApiWlTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_ApiWlTarget.Font = new System.Drawing.Font("Consolas", 10F);
            this.textBox_ApiWlTarget.Location = new System.Drawing.Point(16, 62);
            this.textBox_ApiWlTarget.Name = "textBox_ApiWlTarget";
            this.textBox_ApiWlTarget.Size = new System.Drawing.Size(600, 27);
            this.textBox_ApiWlTarget.TabIndex = 2;
            // 
            // checkBox_UseQuery
            // 
            this.checkBox_UseQuery.AutoSize = true;
            this.checkBox_UseQuery.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.checkBox_UseQuery.Location = new System.Drawing.Point(16, 104);
            this.checkBox_UseQuery.Name = "checkBox_UseQuery";
            this.checkBox_UseQuery.Size = new System.Drawing.Size(290, 25);
            this.checkBox_UseQuery.TabIndex = 3;
            this.checkBox_UseQuery.Text = "Append as query string  (?key=value)";
            // 
            // checkBox_IsGaetMethod
            // 
            this.checkBox_IsGaetMethod.AutoSize = true;
            this.checkBox_IsGaetMethod.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.checkBox_IsGaetMethod.Location = new System.Drawing.Point(16, 132);
            this.checkBox_IsGaetMethod.Name = "checkBox_IsGaetMethod";
            this.checkBox_IsGaetMethod.Size = new System.Drawing.Size(266, 25);
            this.checkBox_IsGaetMethod.TabIndex = 4;
            this.checkBox_IsGaetMethod.Text = "Force GET method for all requests";
            // 
            // checkBox_Json
            // 
            this.checkBox_Json.AutoSize = true;
            this.checkBox_Json.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.checkBox_Json.Location = new System.Drawing.Point(16, 160);
            this.checkBox_Json.Name = "checkBox_Json";
            this.checkBox_Json.Size = new System.Drawing.Size(297, 25);
            this.checkBox_Json.TabIndex = 5;
            this.checkBox_Json.Text = "Send body as JSON  (application/json)";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(-500, -500);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(18, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Visible = false;
            // 
            // tabPage_Headers
            // 
            this.tabPage_Headers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Headers.Controls.Add(this.button_AnalyzeHeaders);
            this.tabPage_Headers.Controls.Add(this.label_HeaderInfo);
            this.tabPage_Headers.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Headers.Name = "tabPage_Headers";
            this.tabPage_Headers.Padding = new System.Windows.Forms.Padding(16);
            this.tabPage_Headers.Size = new System.Drawing.Size(1272, 229);
            this.tabPage_Headers.TabIndex = 3;
            this.tabPage_Headers.Text = " 📋  Headers";
            // 
            // button_AnalyzeHeaders
            // 
            this.button_AnalyzeHeaders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.button_AnalyzeHeaders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_AnalyzeHeaders.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.button_AnalyzeHeaders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_AnalyzeHeaders.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_AnalyzeHeaders.ForeColor = System.Drawing.Color.White;
            this.button_AnalyzeHeaders.Location = new System.Drawing.Point(16, 16);
            this.button_AnalyzeHeaders.Name = "button_AnalyzeHeaders";
            this.button_AnalyzeHeaders.Size = new System.Drawing.Size(236, 70);
            this.button_AnalyzeHeaders.TabIndex = 0;
            this.button_AnalyzeHeaders.Text = "📋  Dump All Headers";
            this.button_AnalyzeHeaders.UseVisualStyleBackColor = false;
            this.button_AnalyzeHeaders.Click += new System.EventHandler(this.button_AnalyzeHeaders_Click);
            // 
            // label_HeaderInfo
            // 
            this.label_HeaderInfo.AutoSize = true;
            this.label_HeaderInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_HeaderInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_HeaderInfo.Location = new System.Drawing.Point(258, 43);
            this.label_HeaderInfo.Name = "label_HeaderInfo";
            this.label_HeaderInfo.Size = new System.Drawing.Size(700, 20);
            this.label_HeaderInfo.TabIndex = 1;
            this.label_HeaderInfo.Text = "Dumps all response headers — useful for fingerprinting server tech, cache behavio" +
    "r, and security posture.";
            // 
            // tabPage_Dns
            // 
            this.tabPage_Dns.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Dns.Controls.Add(this.button_DnsLookup);
            this.tabPage_Dns.Controls.Add(this.label_DnsInfo);
            this.tabPage_Dns.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Dns.Name = "tabPage_Dns";
            this.tabPage_Dns.Padding = new System.Windows.Forms.Padding(16);
            this.tabPage_Dns.Size = new System.Drawing.Size(1272, 229);
            this.tabPage_Dns.TabIndex = 4;
            this.tabPage_Dns.Text = " 🌐  DNS Lookup";
            // 
            // button_DnsLookup
            // 
            this.button_DnsLookup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.button_DnsLookup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_DnsLookup.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.button_DnsLookup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_DnsLookup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_DnsLookup.ForeColor = System.Drawing.Color.White;
            this.button_DnsLookup.Location = new System.Drawing.Point(16, 16);
            this.button_DnsLookup.Name = "button_DnsLookup";
            this.button_DnsLookup.Size = new System.Drawing.Size(160, 44);
            this.button_DnsLookup.TabIndex = 0;
            this.button_DnsLookup.Text = "🌐  Lookup DNS";
            this.button_DnsLookup.UseVisualStyleBackColor = false;
            this.button_DnsLookup.Click += new System.EventHandler(this.button_DnsLookup_Click);
            // 
            // label_DnsInfo
            // 
            this.label_DnsInfo.AutoSize = true;
            this.label_DnsInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_DnsInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_DnsInfo.Location = new System.Drawing.Point(190, 26);
            this.label_DnsInfo.Name = "label_DnsInfo";
            this.label_DnsInfo.Size = new System.Drawing.Size(476, 20);
            this.label_DnsInfo.TabIndex = 1;
            this.label_DnsInfo.Text = "Resolves hostname to IP address(es), aliases, and canonical hostnames.";
            // 
            // panel_Output
            // 
            this.panel_Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel_Output.Controls.Add(this.listBox_Output);
            this.panel_Output.Controls.Add(this.label_Output);
            this.panel_Output.Controls.Add(this.progressBar_Scan);
            this.panel_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Output.Location = new System.Drawing.Point(0, 379);
            this.panel_Output.Name = "panel_Output";
            this.panel_Output.Padding = new System.Windows.Forms.Padding(16, 8, 16, 16);
            this.panel_Output.Size = new System.Drawing.Size(1280, 381);
            this.panel_Output.TabIndex = 0;
            // 
            // listBox_Output
            // 
            this.listBox_Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.listBox_Output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Output.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.listBox_Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(230)))), ((int)(((byte)(253)))));
            this.listBox_Output.IntegralHeight = false;
            this.listBox_Output.ItemHeight = 19;
            this.listBox_Output.Location = new System.Drawing.Point(16, 8);
            this.listBox_Output.Name = "listBox_Output";
            this.listBox_Output.Size = new System.Drawing.Size(1248, 357);
            this.listBox_Output.TabIndex = 0;
            // 
            // label_Output
            // 
            this.label_Output.AutoSize = true;
            this.label_Output.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_Output.Location = new System.Drawing.Point(16, 8);
            this.label_Output.Name = "label_Output";
            this.label_Output.Size = new System.Drawing.Size(113, 20);
            this.label_Output.TabIndex = 1;
            this.label_Output.Text = "SCAN OUTPUT";
            // 
            // progressBar_Scan
            // 
            this.progressBar_Scan.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar_Scan.Location = new System.Drawing.Point(16, 281);
            this.progressBar_Scan.Name = "progressBar_Scan";
            this.progressBar_Scan.Size = new System.Drawing.Size(2328, 6);
            this.progressBar_Scan.TabIndex = 2;
            // 
            // webBrowser_Hidden
            // 
            this.webBrowser_Hidden.Location = new System.Drawing.Point(-500, -500);
            this.webBrowser_Hidden.Name = "webBrowser_Hidden";
            this.webBrowser_Hidden.ScriptErrorsSuppressed = true;
            this.webBrowser_Hidden.Size = new System.Drawing.Size(10, 10);
            this.webBrowser_Hidden.TabIndex = 3;
            this.webBrowser_Hidden.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1280, 760);
            this.Controls.Add(this.panel_Output);
            this.Controls.Add(this.tabControl_Main);
            this.Controls.Add(this.panel_UrlBar);
            this.Controls.Add(this.panel_TopBar);
            this.Controls.Add(this.webBrowser_Hidden);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 680);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThreatScanner — Web Vulnerability Toolkit";
            this.panel_TopBar.ResumeLayout(false);
            this.panel_TopBar.PerformLayout();
            this.panel_UrlBar.ResumeLayout(false);
            this.panel_UrlBar.PerformLayout();
            this.tabControl_Main.ResumeLayout(false);
            this.tabPage_Scanner.ResumeLayout(false);
            this.tabPage_Scanner.PerformLayout();
            this.tabPage_BruteForce.ResumeLayout(false);
            this.tabPage_BruteForce.PerformLayout();
            this.tabPage_Api.ResumeLayout(false);
            this.tabPage_Api.PerformLayout();
            this.tabControl_ApiDetail.ResumeLayout(false);
            this.tabPage_ApiParams.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Params)).EndInit();
            this.tabPage_ApiHeaders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Headers)).EndInit();
            this.tabPage_ApiBody.ResumeLayout(false);
            this.tabPage_ApiBody.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_FormData)).EndInit();
            this.panel_BodyType.ResumeLayout(false);
            this.panel_BodyType.PerformLayout();
            this.tabPage_ApiAuth.ResumeLayout(false);
            this.tabPage_ApiAuth.PerformLayout();
            this.tabPage_ApiWordlist.ResumeLayout(false);
            this.tabPage_ApiWordlist.PerformLayout();
            this.tabPage_Headers.ResumeLayout(false);
            this.tabPage_Headers.PerformLayout();
            this.tabPage_Dns.ResumeLayout(false);
            this.tabPage_Dns.PerformLayout();
            this.panel_Output.ResumeLayout(false);
            this.panel_Output.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        // ─── Control declarations ───────────────────────────────────────────────

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

        private System.Windows.Forms.TabPage tabPage_BruteForce;
        private System.Windows.Forms.Label label_BruteSection1;
        private System.Windows.Forms.Label label_Username;
        private System.Windows.Forms.TextBox textBox_Username;
        private System.Windows.Forms.Label label_Password;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.Label label_Wordlist;
        private System.Windows.Forms.TextBox textBox_WordlistPath;
        private System.Windows.Forms.Button button_BrowseWordlist;
        private System.Windows.Forms.Label label_BruteInfo;
        private System.Windows.Forms.Button button_BruteForce;

        private System.Windows.Forms.TabPage tabPage_Api;
        private System.Windows.Forms.Label label_ApiMethod;
        private System.Windows.Forms.ComboBox comboBox_Method;
        private System.Windows.Forms.Label label_ApiEndpoint;
        private System.Windows.Forms.TextBox textBox_ApiEndpoint;
        private System.Windows.Forms.Button button_ApiForce;
        private System.Windows.Forms.TabControl tabControl_ApiDetail;
        private System.Windows.Forms.TabPage tabPage_ApiParams;
        private System.Windows.Forms.DataGridView dataGridView_Params;
        private System.Windows.Forms.TabPage tabPage_ApiHeaders;
        private System.Windows.Forms.DataGridView dataGridView_Headers;
        private System.Windows.Forms.TabPage tabPage_ApiBody;
        private System.Windows.Forms.DataGridView dataGridView_FormData;  // ← NEW
        private System.Windows.Forms.Panel panel_BodyType;
        private System.Windows.Forms.RadioButton radioButton_BodyNone;
        private System.Windows.Forms.RadioButton radioButton_BodyForm;
        private System.Windows.Forms.RadioButton radioButton_BodyJson;
        private System.Windows.Forms.RadioButton radioButton_BodyRaw;
        private System.Windows.Forms.TextBox textBox_Body;
        private System.Windows.Forms.TabPage tabPage_ApiAuth;
        private System.Windows.Forms.Label label_AuthType;
        private System.Windows.Forms.ComboBox comboBox_AuthType;
        private System.Windows.Forms.Label label_AuthKey;
        private System.Windows.Forms.TextBox textBox_HeaderKey;
        private System.Windows.Forms.Label label_AuthValue;
        private System.Windows.Forms.TextBox textBox_HeaderValue;
        private System.Windows.Forms.TabPage tabPage_ApiWordlist;
        private System.Windows.Forms.Label label_ApiWordlistInfo;
        private System.Windows.Forms.Label label_ApiWlTarget;
        private System.Windows.Forms.TextBox textBox_ApiWlTarget;
        private System.Windows.Forms.CheckBox checkBox_UseQuery;
        private System.Windows.Forms.CheckBox checkBox_Json;
        private System.Windows.Forms.CheckBox checkBox_IsGaetMethod;
        private System.Windows.Forms.CheckBox checkBox1;

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
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_ParamEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ParamKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ParamValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ParamDesc;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_HdrEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_HdrKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_HdrValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_FormKey;   // ← NEW
        private System.Windows.Forms.DataGridViewTextBoxColumn col_FormValue; // ← NEW
    }
}
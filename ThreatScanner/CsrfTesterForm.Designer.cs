namespace ThreatScanner
{
    partial class CsrfTesterForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CsrfTesterForm));
            this.panel_TopBar = new System.Windows.Forms.Panel();
            this.button_SaveReport = new System.Windows.Forms.Button();
            this.button_ClearOutput = new System.Windows.Forms.Button();
            this.label_Title = new System.Windows.Forms.Label();
            this.label_Subtitle = new System.Windows.Forms.Label();
            this.panel_Main = new System.Windows.Forms.Panel();
            this.panel_Output = new System.Windows.Forms.Panel();
            this.richTextBox_Output = new System.Windows.Forms.RichTextBox();
            this.progressBar_Scan = new System.Windows.Forms.ProgressBar();
            this.tabControl_Main = new System.Windows.Forms.TabControl();
            this.tabPage_Scan = new System.Windows.Forms.TabPage();
            this.label_ScanUrl = new System.Windows.Forms.Label();
            this.textBox_Url = new System.Windows.Forms.TextBox();
            this.button_Scan = new System.Windows.Forms.Button();
            this.label_ScanHints = new System.Windows.Forms.Label();
            this.tabPage_Forge = new System.Windows.Forms.TabPage();
            this.label_ForgeUrl = new System.Windows.Forms.Label();
            this.textBox_ForgeUrl = new System.Windows.Forms.TextBox();
            this.button_AutoFill = new System.Windows.Forms.Button();
            this.label_Method = new System.Windows.Forms.Label();
            this.comboBox_Method = new System.Windows.Forms.ComboBox();
            this.label_ForgeOrigin = new System.Windows.Forms.Label();
            this.textBox_ForgeOrigin = new System.Windows.Forms.TextBox();
            this.label_ForgeBody = new System.Windows.Forms.Label();
            this.textBox_ForgeBody = new System.Windows.Forms.TextBox();
            this.checkBox_OmitToken = new System.Windows.Forms.CheckBox();
            this.label_CustomHeaders = new System.Windows.Forms.Label();
            this.dataGridView_Headers = new System.Windows.Forms.DataGridView();
            this.col_CsrfHdrKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_CsrfHdrValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button_ForgeRequest = new System.Windows.Forms.Button();
            this.button_GeneratePoc = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel_TopBar.SuspendLayout();
            this.panel_Main.SuspendLayout();
            this.panel_Output.SuspendLayout();
            this.tabControl_Main.SuspendLayout();
            this.tabPage_Scan.SuspendLayout();
            this.tabPage_Forge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Headers)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_TopBar
            // 
            this.panel_TopBar.BackColor = System.Drawing.Color.White;
            this.panel_TopBar.Controls.Add(this.button_SaveReport);
            this.panel_TopBar.Controls.Add(this.button_ClearOutput);
            this.panel_TopBar.Controls.Add(this.label_Title);
            this.panel_TopBar.Controls.Add(this.label_Subtitle);
            this.panel_TopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_TopBar.Location = new System.Drawing.Point(0, 0);
            this.panel_TopBar.Name = "panel_TopBar";
            this.panel_TopBar.Size = new System.Drawing.Size(1100, 66);
            this.panel_TopBar.TabIndex = 0;
            // 
            // button_SaveReport
            // 
            this.button_SaveReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveReport.BackColor = System.Drawing.Color.White;
            this.button_SaveReport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_SaveReport.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_SaveReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SaveReport.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_SaveReport.Location = new System.Drawing.Point(968, 12);
            this.button_SaveReport.Name = "button_SaveReport";
            this.button_SaveReport.Size = new System.Drawing.Size(120, 32);
            this.button_SaveReport.TabIndex = 4;
            this.button_SaveReport.Text = "💾  Save Report";
            this.button_SaveReport.UseVisualStyleBackColor = false;
            this.button_SaveReport.Click += new System.EventHandler(this.button_SaveReport_Click);
            // 
            // button_ClearOutput
            // 
            this.button_ClearOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ClearOutput.BackColor = System.Drawing.Color.White;
            this.button_ClearOutput.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_ClearOutput.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_ClearOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_ClearOutput.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_ClearOutput.Location = new System.Drawing.Point(858, 12);
            this.button_ClearOutput.Name = "button_ClearOutput";
            this.button_ClearOutput.Size = new System.Drawing.Size(100, 32);
            this.button_ClearOutput.TabIndex = 5;
            this.button_ClearOutput.Text = "🗑  Clear";
            this.button_ClearOutput.UseVisualStyleBackColor = false;
            this.button_ClearOutput.Click += new System.EventHandler(this.button_ClearOutput_Click);
            // 
            // label_Title
            // 
            this.label_Title.AutoSize = true;
            this.label_Title.BackColor = System.Drawing.Color.Transparent;
            this.label_Title.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label_Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(40)))), ((int)(((byte)(217)))));
            this.label_Title.Location = new System.Drawing.Point(16, 8);
            this.label_Title.Name = "label_Title";
            this.label_Title.Size = new System.Drawing.Size(193, 32);
            this.label_Title.TabIndex = 0;
            this.label_Title.Text = "🛡  CSRF Tester";
            // 
            // label_Subtitle
            // 
            this.label_Subtitle.AutoSize = true;
            this.label_Subtitle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_Subtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.label_Subtitle.Location = new System.Drawing.Point(18, 40);
            this.label_Subtitle.Name = "label_Subtitle";
            this.label_Subtitle.Size = new System.Drawing.Size(619, 20);
            this.label_Subtitle.TabIndex = 1;
            this.label_Subtitle.Text = "Scan for missing CSRF tokens · SameSite cookies · CORS misconfig · Forge cross-si" +
    "te requests";
            // 
            // panel_Main
            // 
            this.panel_Main.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel_Main.Controls.Add(this.panel_Output);
            this.panel_Main.Controls.Add(this.tabControl_Main);
            this.panel_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Main.Location = new System.Drawing.Point(0, 66);
            this.panel_Main.Name = "panel_Main";
            this.panel_Main.Size = new System.Drawing.Size(1100, 598);
            this.panel_Main.TabIndex = 1;
            // 
            // panel_Output
            // 
            this.panel_Output.BackColor = System.Drawing.Color.Transparent;
            this.panel_Output.Controls.Add(this.richTextBox_Output);
            this.panel_Output.Controls.Add(this.progressBar_Scan);
            this.panel_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Output.Location = new System.Drawing.Point(535, 0);
            this.panel_Output.Name = "panel_Output";
            this.panel_Output.Padding = new System.Windows.Forms.Padding(8, 8, 8, 4);
            this.panel_Output.Size = new System.Drawing.Size(565, 598);
            this.panel_Output.TabIndex = 1;
            // 
            // richTextBox_Output
            // 
            this.richTextBox_Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.richTextBox_Output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_Output.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.richTextBox_Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.richTextBox_Output.Location = new System.Drawing.Point(8, 8);
            this.richTextBox_Output.Name = "richTextBox_Output";
            this.richTextBox_Output.ReadOnly = true;
            this.richTextBox_Output.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox_Output.Size = new System.Drawing.Size(549, 578);
            this.richTextBox_Output.TabIndex = 0;
            this.richTextBox_Output.Text = "";
            this.richTextBox_Output.WordWrap = false;
            // 
            // progressBar_Scan
            // 
            this.progressBar_Scan.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar_Scan.Location = new System.Drawing.Point(8, 586);
            this.progressBar_Scan.Name = "progressBar_Scan";
            this.progressBar_Scan.Size = new System.Drawing.Size(549, 8);
            this.progressBar_Scan.TabIndex = 1;
            // 
            // tabControl_Main
            // 
            this.tabControl_Main.Controls.Add(this.tabPage_Scan);
            this.tabControl_Main.Controls.Add(this.tabPage_Forge);
            this.tabControl_Main.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl_Main.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.tabControl_Main.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Main.Name = "tabControl_Main";
            this.tabControl_Main.SelectedIndex = 0;
            this.tabControl_Main.Size = new System.Drawing.Size(535, 598);
            this.tabControl_Main.TabIndex = 0;
            // 
            // tabPage_Scan
            // 
            this.tabPage_Scan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Scan.Controls.Add(this.label_ScanUrl);
            this.tabPage_Scan.Controls.Add(this.textBox_Url);
            this.tabPage_Scan.Controls.Add(this.button_Scan);
            this.tabPage_Scan.Controls.Add(this.label_ScanHints);
            this.tabPage_Scan.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Scan.Name = "tabPage_Scan";
            this.tabPage_Scan.Padding = new System.Windows.Forms.Padding(10);
            this.tabPage_Scan.Size = new System.Drawing.Size(527, 564);
            this.tabPage_Scan.TabIndex = 0;
            this.tabPage_Scan.Text = "🔍  Scan";
            // 
            // label_ScanUrl
            // 
            this.label_ScanUrl.AutoSize = true;
            this.label_ScanUrl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_ScanUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.label_ScanUrl.Location = new System.Drawing.Point(14, 16);
            this.label_ScanUrl.Name = "label_ScanUrl";
            this.label_ScanUrl.Size = new System.Drawing.Size(87, 20);
            this.label_ScanUrl.TabIndex = 0;
            this.label_ScanUrl.Text = "Target URL";
            // 
            // textBox_Url
            // 
            this.textBox_Url.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Url.Font = new System.Drawing.Font("Consolas", 10F);
            this.textBox_Url.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.textBox_Url.Location = new System.Drawing.Point(14, 36);
            this.textBox_Url.Name = "textBox_Url";
            this.textBox_Url.Size = new System.Drawing.Size(500, 27);
            this.textBox_Url.TabIndex = 1;
            // 
            // button_Scan
            // 
            this.button_Scan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(40)))), ((int)(((byte)(217)))));
            this.button_Scan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Scan.FlatAppearance.BorderSize = 0;
            this.button_Scan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Scan.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_Scan.ForeColor = System.Drawing.Color.White;
            this.button_Scan.Location = new System.Drawing.Point(14, 76);
            this.button_Scan.Name = "button_Scan";
            this.button_Scan.Size = new System.Drawing.Size(200, 38);
            this.button_Scan.TabIndex = 2;
            this.button_Scan.Text = "▶  Run CSRF Scan";
            this.button_Scan.UseVisualStyleBackColor = false;
            this.button_Scan.Click += new System.EventHandler(this.button_Scan_Click);
            // 
            // label_ScanHints
            // 
            this.label_ScanHints.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ScanHints.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label_ScanHints.Location = new System.Drawing.Point(14, 128);
            this.label_ScanHints.Name = "label_ScanHints";
            this.label_ScanHints.Size = new System.Drawing.Size(500, 440);
            this.label_ScanHints.TabIndex = 3;
            this.label_ScanHints.Text = resources.GetString("label_ScanHints.Text");
            // 
            // tabPage_Forge
            // 
            this.tabPage_Forge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Forge.Controls.Add(this.panel3);
            this.tabPage_Forge.Controls.Add(this.panel1);
            this.tabPage_Forge.Controls.Add(this.panel2);
            this.tabPage_Forge.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Forge.Name = "tabPage_Forge";
            this.tabPage_Forge.Padding = new System.Windows.Forms.Padding(10);
            this.tabPage_Forge.Size = new System.Drawing.Size(527, 564);
            this.tabPage_Forge.TabIndex = 1;
            this.tabPage_Forge.Text = "⚡  Forge Request";
            // 
            // label_ForgeUrl
            // 
            this.label_ForgeUrl.AutoSize = true;
            this.label_ForgeUrl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_ForgeUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.label_ForgeUrl.Location = new System.Drawing.Point(4, 6);
            this.label_ForgeUrl.Name = "label_ForgeUrl";
            this.label_ForgeUrl.Size = new System.Drawing.Size(105, 20);
            this.label_ForgeUrl.TabIndex = 0;
            this.label_ForgeUrl.Text = "Endpoint URL";
            // 
            // textBox_ForgeUrl
            // 
            this.textBox_ForgeUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_ForgeUrl.Font = new System.Drawing.Font("Consolas", 10F);
            this.textBox_ForgeUrl.Location = new System.Drawing.Point(4, 26);
            this.textBox_ForgeUrl.Name = "textBox_ForgeUrl";
            this.textBox_ForgeUrl.Size = new System.Drawing.Size(500, 27);
            this.textBox_ForgeUrl.TabIndex = 1;
            // 
            // button_AutoFill
            // 
            this.button_AutoFill.BackColor = System.Drawing.SystemColors.Control;
            this.button_AutoFill.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_AutoFill.FlatAppearance.BorderSize = 0;
            this.button_AutoFill.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.button_AutoFill.ForeColor = System.Drawing.Color.DimGray;
            this.button_AutoFill.Location = new System.Drawing.Point(4, 58);
            this.button_AutoFill.Name = "button_AutoFill";
            this.button_AutoFill.Size = new System.Drawing.Size(500, 28);
            this.button_AutoFill.TabIndex = 2;
            this.button_AutoFill.Text = "🔄  Auto-Fill from URL";
            this.button_AutoFill.UseVisualStyleBackColor = false;
            this.button_AutoFill.Click += new System.EventHandler(this.button_AutoFill_Click);
            // 
            // label_Method
            // 
            this.label_Method.AutoSize = true;
            this.label_Method.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_Method.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.label_Method.Location = new System.Drawing.Point(4, 96);
            this.label_Method.Name = "label_Method";
            this.label_Method.Size = new System.Drawing.Size(64, 20);
            this.label_Method.TabIndex = 3;
            this.label_Method.Text = "Method";
            // 
            // comboBox_Method
            // 
            this.comboBox_Method.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Method.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.comboBox_Method.Items.AddRange(new object[] {
            "POST",
            "GET",
            "PUT",
            "PATCH",
            "DELETE"});
            this.comboBox_Method.Location = new System.Drawing.Point(4, 116);
            this.comboBox_Method.Name = "comboBox_Method";
            this.comboBox_Method.Size = new System.Drawing.Size(120, 29);
            this.comboBox_Method.TabIndex = 4;
            // 
            // label_ForgeOrigin
            // 
            this.label_ForgeOrigin.AutoSize = true;
            this.label_ForgeOrigin.Font = new System.Drawing.Font("Segoe UI", 7.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ForgeOrigin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.label_ForgeOrigin.Location = new System.Drawing.Point(140, 96);
            this.label_ForgeOrigin.Name = "label_ForgeOrigin";
            this.label_ForgeOrigin.Size = new System.Drawing.Size(228, 15);
            this.label_ForgeOrigin.TabIndex = 5;
            this.label_ForgeOrigin.Text = "Attacker Origin (sent as Origin/Referer)";
            // 
            // textBox_ForgeOrigin
            // 
            this.textBox_ForgeOrigin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_ForgeOrigin.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.textBox_ForgeOrigin.Location = new System.Drawing.Point(140, 116);
            this.textBox_ForgeOrigin.Name = "textBox_ForgeOrigin";
            this.textBox_ForgeOrigin.Size = new System.Drawing.Size(364, 26);
            this.textBox_ForgeOrigin.TabIndex = 6;
            this.textBox_ForgeOrigin.Text = "https://evil-attacker.com";
            // 
            // label_ForgeBody
            // 
            this.label_ForgeBody.AutoSize = true;
            this.label_ForgeBody.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_ForgeBody.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_ForgeBody.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.label_ForgeBody.Location = new System.Drawing.Point(0, 0);
            this.label_ForgeBody.Name = "label_ForgeBody";
            this.label_ForgeBody.Size = new System.Drawing.Size(386, 20);
            this.label_ForgeBody.TabIndex = 7;
            this.label_ForgeBody.Text = "Request Body  (key=value, one per line or &-separated)";
            // 
            // textBox_ForgeBody
            // 
            this.textBox_ForgeBody.AcceptsReturn = true;
            this.textBox_ForgeBody.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_ForgeBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_ForgeBody.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.textBox_ForgeBody.Location = new System.Drawing.Point(0, 20);
            this.textBox_ForgeBody.Multiline = true;
            this.textBox_ForgeBody.Name = "textBox_ForgeBody";
            this.textBox_ForgeBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_ForgeBody.Size = new System.Drawing.Size(507, 160);
            this.textBox_ForgeBody.TabIndex = 8;
            // 
            // checkBox_OmitToken
            // 
            this.checkBox_OmitToken.AutoSize = true;
            this.checkBox_OmitToken.Checked = true;
            this.checkBox_OmitToken.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_OmitToken.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.checkBox_OmitToken.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkBox_OmitToken.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.checkBox_OmitToken.Location = new System.Drawing.Point(0, 180);
            this.checkBox_OmitToken.Name = "checkBox_OmitToken";
            this.checkBox_OmitToken.Size = new System.Drawing.Size(507, 24);
            this.checkBox_OmitToken.TabIndex = 9;
            this.checkBox_OmitToken.Text = "Omit CSRF token (simulate forged cross-site request)";
            // 
            // label_CustomHeaders
            // 
            this.label_CustomHeaders.AutoSize = true;
            this.label_CustomHeaders.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_CustomHeaders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.label_CustomHeaders.Location = new System.Drawing.Point(4, 12);
            this.label_CustomHeaders.Name = "label_CustomHeaders";
            this.label_CustomHeaders.Size = new System.Drawing.Size(184, 20);
            this.label_CustomHeaders.TabIndex = 10;
            this.label_CustomHeaders.Text = "Extra Headers  (optional)";
            // 
            // dataGridView_Headers
            // 
            this.dataGridView_Headers.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView_Headers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Headers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_CsrfHdrKey,
            this.col_CsrfHdrValue});
            this.dataGridView_Headers.Location = new System.Drawing.Point(4, 32);
            this.dataGridView_Headers.Name = "dataGridView_Headers";
            this.dataGridView_Headers.RowHeadersWidth = 24;
            this.dataGridView_Headers.Size = new System.Drawing.Size(500, 92);
            this.dataGridView_Headers.TabIndex = 11;
            // 
            // col_CsrfHdrKey
            // 
            this.col_CsrfHdrKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col_CsrfHdrKey.HeaderText = "Header Name";
            this.col_CsrfHdrKey.MinimumWidth = 6;
            this.col_CsrfHdrKey.Name = "col_CsrfHdrKey";
            // 
            // col_CsrfHdrValue
            // 
            this.col_CsrfHdrValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col_CsrfHdrValue.HeaderText = "Value";
            this.col_CsrfHdrValue.MinimumWidth = 6;
            this.col_CsrfHdrValue.Name = "col_CsrfHdrValue";
            // 
            // button_ForgeRequest
            // 
            this.button_ForgeRequest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(40)))), ((int)(((byte)(217)))));
            this.button_ForgeRequest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_ForgeRequest.FlatAppearance.BorderSize = 0;
            this.button_ForgeRequest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_ForgeRequest.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_ForgeRequest.ForeColor = System.Drawing.Color.White;
            this.button_ForgeRequest.Location = new System.Drawing.Point(4, 138);
            this.button_ForgeRequest.Name = "button_ForgeRequest";
            this.button_ForgeRequest.Size = new System.Drawing.Size(245, 38);
            this.button_ForgeRequest.TabIndex = 12;
            this.button_ForgeRequest.Text = "⚡  Send Forged Request";
            this.button_ForgeRequest.UseVisualStyleBackColor = false;
            this.button_ForgeRequest.Click += new System.EventHandler(this.button_ForgeRequest_Click);
            // 
            // button_GeneratePoc
            // 
            this.button_GeneratePoc.BackColor = System.Drawing.Color.RoyalBlue;
            this.button_GeneratePoc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_GeneratePoc.FlatAppearance.BorderSize = 0;
            this.button_GeneratePoc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_GeneratePoc.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_GeneratePoc.ForeColor = System.Drawing.Color.White;
            this.button_GeneratePoc.Location = new System.Drawing.Point(267, 138);
            this.button_GeneratePoc.Name = "button_GeneratePoc";
            this.button_GeneratePoc.Size = new System.Drawing.Size(237, 38);
            this.button_GeneratePoc.TabIndex = 13;
            this.button_GeneratePoc.Text = "📄  Generate PoC HTML";
            this.button_GeneratePoc.UseVisualStyleBackColor = false;
            this.button_GeneratePoc.Click += new System.EventHandler(this.button_GeneratePoc_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_ForgeUrl);
            this.panel1.Controls.Add(this.textBox_ForgeUrl);
            this.panel1.Controls.Add(this.textBox_ForgeOrigin);
            this.panel1.Controls.Add(this.button_AutoFill);
            this.panel1.Controls.Add(this.label_ForgeOrigin);
            this.panel1.Controls.Add(this.label_Method);
            this.panel1.Controls.Add(this.comboBox_Method);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(10, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(507, 149);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label_CustomHeaders);
            this.panel2.Controls.Add(this.button_GeneratePoc);
            this.panel2.Controls.Add(this.button_ForgeRequest);
            this.panel2.Controls.Add(this.dataGridView_Headers);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(10, 363);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(507, 191);
            this.panel2.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.textBox_ForgeBody);
            this.panel3.Controls.Add(this.label_ForgeBody);
            this.panel3.Controls.Add(this.checkBox_OmitToken);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(10, 159);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(507, 204);
            this.panel3.TabIndex = 2;
            // 
            // CsrfTesterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1100, 664);
            this.Controls.Add(this.panel_Main);
            this.Controls.Add(this.panel_TopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Name = "CsrfTesterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThreatScanner — CSRF Tester";
            this.panel_TopBar.ResumeLayout(false);
            this.panel_TopBar.PerformLayout();
            this.panel_Main.ResumeLayout(false);
            this.panel_Output.ResumeLayout(false);
            this.tabControl_Main.ResumeLayout(false);
            this.tabPage_Scan.ResumeLayout(false);
            this.tabPage_Scan.PerformLayout();
            this.tabPage_Forge.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Headers)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        // ── Controls ─────────────────────────────────────────────────────────────
        private System.Windows.Forms.Panel panel_TopBar;
        private System.Windows.Forms.Label label_Title;
        private System.Windows.Forms.Label label_Subtitle;
        private System.Windows.Forms.Panel panel_Main;
        private System.Windows.Forms.TabControl tabControl_Main;
        private System.Windows.Forms.TabPage tabPage_Scan;
        private System.Windows.Forms.Label label_ScanUrl;
        private System.Windows.Forms.TextBox textBox_Url;
        private System.Windows.Forms.Button button_Scan;
        private System.Windows.Forms.TabPage tabPage_Forge;
        private System.Windows.Forms.Label label_ForgeUrl;
        private System.Windows.Forms.TextBox textBox_ForgeUrl;
        private System.Windows.Forms.Button button_AutoFill;
        private System.Windows.Forms.Label label_Method;
        private System.Windows.Forms.ComboBox comboBox_Method;
        private System.Windows.Forms.Label label_ForgeOrigin;
        private System.Windows.Forms.TextBox textBox_ForgeOrigin;
        private System.Windows.Forms.Label label_ForgeBody;
        private System.Windows.Forms.TextBox textBox_ForgeBody;
        private System.Windows.Forms.CheckBox checkBox_OmitToken;
        private System.Windows.Forms.Label label_CustomHeaders;
        private System.Windows.Forms.DataGridView dataGridView_Headers;
        private System.Windows.Forms.Button button_ForgeRequest;
        private System.Windows.Forms.Button button_GeneratePoc;
        private System.Windows.Forms.Panel panel_Output;
        private System.Windows.Forms.RichTextBox richTextBox_Output;
        private System.Windows.Forms.ProgressBar progressBar_Scan;
        private System.Windows.Forms.Label label_ScanHints;
        private System.Windows.Forms.Button button_SaveReport;
        private System.Windows.Forms.Button button_ClearOutput;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_CsrfHdrKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_CsrfHdrValue;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
    }
}
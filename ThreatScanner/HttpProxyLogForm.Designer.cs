namespace ThreatScanner
{
    partial class HttpProxyLogForm
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
            this.button_SaveLog = new System.Windows.Forms.Button();
            this.button_ClearLog = new System.Windows.Forms.Button();
            this.panel_RequestBar = new System.Windows.Forms.Panel();
            this.label_Filter = new System.Windows.Forms.Label();
            this.textBox_Filter = new System.Windows.Forms.TextBox();
            this.button_StartCapture = new System.Windows.Forms.Button();
            this.button_StopCapture = new System.Windows.Forms.Button();
            this.comboBox_MethodFilter = new System.Windows.Forms.ComboBox();
            this.comboBox_StatusFilter = new System.Windows.Forms.ComboBox();
            this.label_Counter = new System.Windows.Forms.Label();
            this.progressBar_Capture = new System.Windows.Forms.ProgressBar();
            this.panel_Grid = new System.Windows.Forms.Panel();
            this.splitContainer_Main = new System.Windows.Forms.SplitContainer();
            this.dataGridView_Requests = new System.Windows.Forms.DataGridView();
            this.col_Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Method = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Correlation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Url = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel_Detail = new System.Windows.Forms.Panel();
            this.tabControl_Detail = new System.Windows.Forms.TabControl();
            this.tabPage_Headers = new System.Windows.Forms.TabPage();
            this.splitContainer_Headers = new System.Windows.Forms.SplitContainer();
            this.listView_ReqHeaders = new System.Windows.Forms.ListView();
            this.colHdrReqName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHdrReqValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label_ReqHeaders = new System.Windows.Forms.Label();
            this.listView_RespHeaders = new System.Windows.Forms.ListView();
            this.colHdrRespName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHdrRespValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label_RespHeaders = new System.Windows.Forms.Label();
            this.tabPage_ReqBody = new System.Windows.Forms.TabPage();
            this.tabPage_RespBody = new System.Windows.Forms.TabPage();
            this.tabPage_Timing = new System.Windows.Forms.TabPage();
            this.textBox_Timing = new System.Windows.Forms.TextBox();
            this.panel_DetailToolbar = new System.Windows.Forms.Panel();
            this.label_CorrelationCaption = new System.Windows.Forms.Label();
            this.label_CorrelationValue = new System.Windows.Forms.Label();
            this.button_CopyHttpClient = new System.Windows.Forms.Button();
            this.button_CopyCurl = new System.Windows.Forms.Button();
            this.button_Replay = new System.Windows.Forms.Button();
            this.panel_Output = new System.Windows.Forms.Panel();
            this.richTextBox_Output = new System.Windows.Forms.RichTextBox();
            this.label_Output = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel_TopBar.SuspendLayout();
            this.panel_RequestBar.SuspendLayout();
            this.panel_Grid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).BeginInit();
            this.splitContainer_Main.Panel1.SuspendLayout();
            this.splitContainer_Main.Panel2.SuspendLayout();
            this.splitContainer_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Requests)).BeginInit();
            this.panel_Detail.SuspendLayout();
            this.tabControl_Detail.SuspendLayout();
            this.tabPage_Headers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Headers)).BeginInit();
            this.splitContainer_Headers.Panel1.SuspendLayout();
            this.splitContainer_Headers.Panel2.SuspendLayout();
            this.splitContainer_Headers.SuspendLayout();
            this.tabPage_Timing.SuspendLayout();
            this.panel_DetailToolbar.SuspendLayout();
            this.panel_Output.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_TopBar
            // 
            this.panel_TopBar.BackColor = System.Drawing.Color.White;
            this.panel_TopBar.Controls.Add(this.label_AppTitle);
            this.panel_TopBar.Controls.Add(this.label_AppSubtitle);
            this.panel_TopBar.Controls.Add(this.button_SaveLog);
            this.panel_TopBar.Controls.Add(this.button_ClearLog);
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
            this.label_AppTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(145)))), ((int)(((byte)(178)))));
            this.label_AppTitle.Location = new System.Drawing.Point(16, 12);
            this.label_AppTitle.Name = "label_AppTitle";
            this.label_AppTitle.Size = new System.Drawing.Size(245, 32);
            this.label_AppTitle.TabIndex = 0;
            this.label_AppTitle.Text = "🌐  HTTP Proxy Log";
            // 
            // label_AppSubtitle
            // 
            this.label_AppSubtitle.AutoSize = true;
            this.label_AppSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_AppSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AppSubtitle.Location = new System.Drawing.Point(18, 40);
            this.label_AppSubtitle.Name = "label_AppSubtitle";
            this.label_AppSubtitle.Size = new System.Drawing.Size(414, 20);
            this.label_AppSubtitle.TabIndex = 1;
            this.label_AppSubtitle.Text = "Logs every HTTP request/response made by your browser tab";
            // 
            // button_SaveLog
            // 
            this.button_SaveLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveLog.BackColor = System.Drawing.Color.White;
            this.button_SaveLog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_SaveLog.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_SaveLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SaveLog.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_SaveLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.button_SaveLog.Location = new System.Drawing.Point(1040, 16);
            this.button_SaveLog.Name = "button_SaveLog";
            this.button_SaveLog.Size = new System.Drawing.Size(110, 32);
            this.button_SaveLog.TabIndex = 2;
            this.button_SaveLog.Text = "💾 Save Log";
            this.button_SaveLog.UseVisualStyleBackColor = false;
            this.button_SaveLog.Click += new System.EventHandler(this.button_SaveLog_Click);
            // 
            // button_ClearLog
            // 
            this.button_ClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_ClearLog.BackColor = System.Drawing.Color.White;
            this.button_ClearLog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_ClearLog.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_ClearLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_ClearLog.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_ClearLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.button_ClearLog.Location = new System.Drawing.Point(1160, 16);
            this.button_ClearLog.Name = "button_ClearLog";
            this.button_ClearLog.Size = new System.Drawing.Size(100, 32);
            this.button_ClearLog.TabIndex = 3;
            this.button_ClearLog.Text = "🧹 Clear";
            this.button_ClearLog.UseVisualStyleBackColor = false;
            this.button_ClearLog.Click += new System.EventHandler(this.button_ClearLog_Click);
            // 
            // panel_RequestBar
            // 
            this.panel_RequestBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel_RequestBar.Controls.Add(this.label2);
            this.panel_RequestBar.Controls.Add(this.label1);
            this.panel_RequestBar.Controls.Add(this.label_Filter);
            this.panel_RequestBar.Controls.Add(this.textBox_Filter);
            this.panel_RequestBar.Controls.Add(this.button_StartCapture);
            this.panel_RequestBar.Controls.Add(this.button_StopCapture);
            this.panel_RequestBar.Controls.Add(this.comboBox_MethodFilter);
            this.panel_RequestBar.Controls.Add(this.comboBox_StatusFilter);
            this.panel_RequestBar.Controls.Add(this.label_Counter);
            this.panel_RequestBar.Controls.Add(this.progressBar_Capture);
            this.panel_RequestBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_RequestBar.Location = new System.Drawing.Point(0, 64);
            this.panel_RequestBar.Name = "panel_RequestBar";
            this.panel_RequestBar.Size = new System.Drawing.Size(1280, 76);
            this.panel_RequestBar.TabIndex = 1;
            // 
            // label_Filter
            // 
            this.label_Filter.AutoSize = true;
            this.label_Filter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_Filter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.label_Filter.Location = new System.Drawing.Point(292, 16);
            this.label_Filter.Name = "label_Filter";
            this.label_Filter.Size = new System.Drawing.Size(275, 20);
            this.label_Filter.TabIndex = 0;
            this.label_Filter.Text = "Filter (optional, substring match on URL)";
            // 
            // textBox_Filter
            // 
            this.textBox_Filter.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBox_Filter.Location = new System.Drawing.Point(292, 40);
            this.textBox_Filter.Name = "textBox_Filter";
            this.textBox_Filter.Size = new System.Drawing.Size(452, 30);
            this.textBox_Filter.TabIndex = 1;
            this.textBox_Filter.TextChanged += new System.EventHandler(this.textBox_Filter_TextChanged);
            // 
            // button_StartCapture
            // 
            this.button_StartCapture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.button_StartCapture.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_StartCapture.FlatAppearance.BorderSize = 0;
            this.button_StartCapture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_StartCapture.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_StartCapture.ForeColor = System.Drawing.Color.White;
            this.button_StartCapture.Location = new System.Drawing.Point(16, 14);
            this.button_StartCapture.Name = "button_StartCapture";
            this.button_StartCapture.Size = new System.Drawing.Size(150, 31);
            this.button_StartCapture.TabIndex = 2;
            this.button_StartCapture.Text = "▶  Start Capture";
            this.button_StartCapture.UseVisualStyleBackColor = false;
            this.button_StartCapture.Click += new System.EventHandler(this.button_StartCapture_Click);
            // 
            // button_StopCapture
            // 
            this.button_StopCapture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.button_StopCapture.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_StopCapture.Enabled = false;
            this.button_StopCapture.FlatAppearance.BorderSize = 0;
            this.button_StopCapture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_StopCapture.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_StopCapture.ForeColor = System.Drawing.Color.White;
            this.button_StopCapture.Location = new System.Drawing.Point(176, 14);
            this.button_StopCapture.Name = "button_StopCapture";
            this.button_StopCapture.Size = new System.Drawing.Size(110, 31);
            this.button_StopCapture.TabIndex = 3;
            this.button_StopCapture.Text = "■  Stop";
            this.button_StopCapture.UseVisualStyleBackColor = false;
            this.button_StopCapture.Click += new System.EventHandler(this.button_StopCapture_Click);
            // 
            // comboBox_MethodFilter
            // 
            this.comboBox_MethodFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_MethodFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBox_MethodFilter.FormattingEnabled = true;
            this.comboBox_MethodFilter.Items.AddRange(new object[] {
            "All Methods",
            "GET",
            "POST",
            "PUT",
            "PATCH",
            "DELETE",
            "HEAD",
            "OPTIONS"});
            this.comboBox_MethodFilter.Location = new System.Drawing.Point(750, 40);
            this.comboBox_MethodFilter.Name = "comboBox_MethodFilter";
            this.comboBox_MethodFilter.Size = new System.Drawing.Size(115, 28);
            this.comboBox_MethodFilter.TabIndex = 6;
            this.comboBox_MethodFilter.SelectedIndexChanged += new System.EventHandler(this.comboBox_Filters_SelectedIndexChanged);
            // 
            // comboBox_StatusFilter
            // 
            this.comboBox_StatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_StatusFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBox_StatusFilter.FormattingEnabled = true;
            this.comboBox_StatusFilter.Items.AddRange(new object[] {
            "All Status",
            "2xx",
            "3xx",
            "4xx",
            "5xx",
            "Failed",
            "Pending"});
            this.comboBox_StatusFilter.Location = new System.Drawing.Point(875, 40);
            this.comboBox_StatusFilter.Name = "comboBox_StatusFilter";
            this.comboBox_StatusFilter.Size = new System.Drawing.Size(115, 28);
            this.comboBox_StatusFilter.TabIndex = 7;
            this.comboBox_StatusFilter.SelectedIndexChanged += new System.EventHandler(this.comboBox_Filters_SelectedIndexChanged);
            // 
            // label_Counter
            // 
            this.label_Counter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_Counter.AutoSize = true;
            this.label_Counter.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.label_Counter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(145)))), ((int)(((byte)(178)))));
            this.label_Counter.Location = new System.Drawing.Point(1080, 14);
            this.label_Counter.Name = "label_Counter";
            this.label_Counter.Size = new System.Drawing.Size(159, 21);
            this.label_Counter.TabIndex = 4;
            this.label_Counter.Text = "0 requests captured";
            // 
            // progressBar_Capture
            // 
            this.progressBar_Capture.Location = new System.Drawing.Point(18, 51);
            this.progressBar_Capture.Name = "progressBar_Capture";
            this.progressBar_Capture.Size = new System.Drawing.Size(268, 12);
            this.progressBar_Capture.TabIndex = 5;
            // 
            // panel_Grid
            // 
            this.panel_Grid.BackColor = System.Drawing.Color.White;
            this.panel_Grid.Controls.Add(this.splitContainer_Main);
            this.panel_Grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Grid.Location = new System.Drawing.Point(0, 140);
            this.panel_Grid.Name = "panel_Grid";
            this.panel_Grid.Padding = new System.Windows.Forms.Padding(12);
            this.panel_Grid.Size = new System.Drawing.Size(1280, 501);
            this.panel_Grid.TabIndex = 2;
            // 
            // splitContainer_Main
            // 
            this.splitContainer_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Main.Location = new System.Drawing.Point(12, 12);
            this.splitContainer_Main.Name = "splitContainer_Main";
            this.splitContainer_Main.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_Main.Panel1
            // 
            this.splitContainer_Main.Panel1.Controls.Add(this.dataGridView_Requests);
            this.splitContainer_Main.Panel1MinSize = 120;
            // 
            // splitContainer_Main.Panel2
            // 
            this.splitContainer_Main.Panel2.Controls.Add(this.panel_Detail);
            this.splitContainer_Main.Panel2MinSize = 160;
            this.splitContainer_Main.Size = new System.Drawing.Size(1256, 477);
            this.splitContainer_Main.SplitterDistance = 257;
            this.splitContainer_Main.SplitterWidth = 6;
            this.splitContainer_Main.TabIndex = 0;
            // 
            // dataGridView_Requests
            // 
            this.dataGridView_Requests.AllowUserToAddRows = false;
            this.dataGridView_Requests.AllowUserToDeleteRows = false;
            this.dataGridView_Requests.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView_Requests.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_Requests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Requests.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Time,
            this.col_Method,
            this.col_Status,
            this.col_Type,
            this.col_Size,
            this.col_Correlation,
            this.col_Url});
            this.dataGridView_Requests.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Requests.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_Requests.Name = "dataGridView_Requests";
            this.dataGridView_Requests.ReadOnly = true;
            this.dataGridView_Requests.RowHeadersVisible = false;
            this.dataGridView_Requests.RowHeadersWidth = 51;
            this.dataGridView_Requests.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_Requests.Size = new System.Drawing.Size(1256, 257);
            this.dataGridView_Requests.TabIndex = 0;
            this.dataGridView_Requests.SelectionChanged += new System.EventHandler(this.dataGridView_Requests_SelectionChanged);
            // 
            // col_Time
            // 
            this.col_Time.HeaderText = "Time";
            this.col_Time.MinimumWidth = 6;
            this.col_Time.Name = "col_Time";
            this.col_Time.ReadOnly = true;
            this.col_Time.Width = 95;
            // 
            // col_Method
            // 
            this.col_Method.HeaderText = "Method";
            this.col_Method.MinimumWidth = 6;
            this.col_Method.Name = "col_Method";
            this.col_Method.ReadOnly = true;
            this.col_Method.Width = 75;
            // 
            // col_Status
            // 
            this.col_Status.HeaderText = "Status";
            this.col_Status.MinimumWidth = 6;
            this.col_Status.Name = "col_Status";
            this.col_Status.ReadOnly = true;
            this.col_Status.Width = 70;
            // 
            // col_Type
            // 
            this.col_Type.HeaderText = "Type";
            this.col_Type.MinimumWidth = 6;
            this.col_Type.Name = "col_Type";
            this.col_Type.ReadOnly = true;
            this.col_Type.Width = 90;
            // 
            // col_Size
            // 
            this.col_Size.HeaderText = "Size";
            this.col_Size.MinimumWidth = 6;
            this.col_Size.Name = "col_Size";
            this.col_Size.ReadOnly = true;
            this.col_Size.Width = 80;
            // 
            // col_Correlation
            // 
            this.col_Correlation.HeaderText = "Correlation Id";
            this.col_Correlation.MinimumWidth = 6;
            this.col_Correlation.Name = "col_Correlation";
            this.col_Correlation.ReadOnly = true;
            this.col_Correlation.Width = 150;
            // 
            // col_Url
            // 
            this.col_Url.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col_Url.HeaderText = "URL";
            this.col_Url.MinimumWidth = 6;
            this.col_Url.Name = "col_Url";
            this.col_Url.ReadOnly = true;
            // 
            // panel_Detail
            // 
            this.panel_Detail.Controls.Add(this.tabControl_Detail);
            this.panel_Detail.Controls.Add(this.panel_DetailToolbar);
            this.panel_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Detail.Location = new System.Drawing.Point(0, 0);
            this.panel_Detail.Name = "panel_Detail";
            this.panel_Detail.Size = new System.Drawing.Size(1256, 214);
            this.panel_Detail.TabIndex = 0;
            // 
            // tabControl_Detail
            // 
            this.tabControl_Detail.Controls.Add(this.tabPage_Headers);
            this.tabControl_Detail.Controls.Add(this.tabPage_ReqBody);
            this.tabControl_Detail.Controls.Add(this.tabPage_RespBody);
            this.tabControl_Detail.Controls.Add(this.tabPage_Timing);
            this.tabControl_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Detail.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabControl_Detail.Location = new System.Drawing.Point(0, 36);
            this.tabControl_Detail.Name = "tabControl_Detail";
            this.tabControl_Detail.SelectedIndex = 0;
            this.tabControl_Detail.Size = new System.Drawing.Size(1256, 178);
            this.tabControl_Detail.TabIndex = 1;
            // 
            // tabPage_Headers
            // 
            this.tabPage_Headers.Controls.Add(this.splitContainer_Headers);
            this.tabPage_Headers.Location = new System.Drawing.Point(4, 29);
            this.tabPage_Headers.Name = "tabPage_Headers";
            this.tabPage_Headers.Padding = new System.Windows.Forms.Padding(6);
            this.tabPage_Headers.Size = new System.Drawing.Size(1248, 145);
            this.tabPage_Headers.TabIndex = 0;
            this.tabPage_Headers.Text = "Headers";
            this.tabPage_Headers.UseVisualStyleBackColor = true;
            // 
            // splitContainer_Headers
            // 
            this.splitContainer_Headers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Headers.Location = new System.Drawing.Point(6, 6);
            this.splitContainer_Headers.Name = "splitContainer_Headers";
            // 
            // splitContainer_Headers.Panel1
            // 
            this.splitContainer_Headers.Panel1.Controls.Add(this.listView_ReqHeaders);
            this.splitContainer_Headers.Panel1.Controls.Add(this.label_ReqHeaders);
            // 
            // splitContainer_Headers.Panel2
            // 
            this.splitContainer_Headers.Panel2.Controls.Add(this.listView_RespHeaders);
            this.splitContainer_Headers.Panel2.Controls.Add(this.label_RespHeaders);
            this.splitContainer_Headers.Size = new System.Drawing.Size(1236, 133);
            this.splitContainer_Headers.SplitterDistance = 615;
            this.splitContainer_Headers.TabIndex = 0;
            // 
            // listView_ReqHeaders
            // 
            this.listView_ReqHeaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colHdrReqName,
            this.colHdrReqValue});
            this.listView_ReqHeaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_ReqHeaders.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.listView_ReqHeaders.FullRowSelect = true;
            this.listView_ReqHeaders.GridLines = true;
            this.listView_ReqHeaders.HideSelection = false;
            this.listView_ReqHeaders.Location = new System.Drawing.Point(0, 18);
            this.listView_ReqHeaders.Name = "listView_ReqHeaders";
            this.listView_ReqHeaders.Size = new System.Drawing.Size(615, 115);
            this.listView_ReqHeaders.TabIndex = 1;
            this.listView_ReqHeaders.UseCompatibleStateImageBehavior = false;
            this.listView_ReqHeaders.View = System.Windows.Forms.View.Details;
            // 
            // colHdrReqName
            // 
            this.colHdrReqName.Text = "Name";
            this.colHdrReqName.Width = 220;
            // 
            // colHdrReqValue
            // 
            this.colHdrReqValue.Text = "Value";
            this.colHdrReqValue.Width = 380;
            // 
            // label_ReqHeaders
            // 
            this.label_ReqHeaders.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_ReqHeaders.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_ReqHeaders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(145)))), ((int)(((byte)(178)))));
            this.label_ReqHeaders.Location = new System.Drawing.Point(0, 0);
            this.label_ReqHeaders.Name = "label_ReqHeaders";
            this.label_ReqHeaders.Size = new System.Drawing.Size(615, 18);
            this.label_ReqHeaders.TabIndex = 0;
            this.label_ReqHeaders.Text = "REQUEST HEADERS";
            // 
            // listView_RespHeaders
            // 
            this.listView_RespHeaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colHdrRespName,
            this.colHdrRespValue});
            this.listView_RespHeaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_RespHeaders.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.listView_RespHeaders.FullRowSelect = true;
            this.listView_RespHeaders.GridLines = true;
            this.listView_RespHeaders.HideSelection = false;
            this.listView_RespHeaders.Location = new System.Drawing.Point(0, 18);
            this.listView_RespHeaders.Name = "listView_RespHeaders";
            this.listView_RespHeaders.Size = new System.Drawing.Size(617, 115);
            this.listView_RespHeaders.TabIndex = 1;
            this.listView_RespHeaders.UseCompatibleStateImageBehavior = false;
            this.listView_RespHeaders.View = System.Windows.Forms.View.Details;
            // 
            // colHdrRespName
            // 
            this.colHdrRespName.Text = "Name";
            this.colHdrRespName.Width = 220;
            // 
            // colHdrRespValue
            // 
            this.colHdrRespValue.Text = "Value";
            this.colHdrRespValue.Width = 380;
            // 
            // label_RespHeaders
            // 
            this.label_RespHeaders.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_RespHeaders.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_RespHeaders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(145)))), ((int)(((byte)(178)))));
            this.label_RespHeaders.Location = new System.Drawing.Point(0, 0);
            this.label_RespHeaders.Name = "label_RespHeaders";
            this.label_RespHeaders.Size = new System.Drawing.Size(617, 18);
            this.label_RespHeaders.TabIndex = 0;
            this.label_RespHeaders.Text = "RESPONSE HEADERS";
            // 
            // tabPage_ReqBody
            // 
            this.tabPage_ReqBody.Location = new System.Drawing.Point(4, 29);
            this.tabPage_ReqBody.Name = "tabPage_ReqBody";
            this.tabPage_ReqBody.Size = new System.Drawing.Size(1248, 145);
            this.tabPage_ReqBody.TabIndex = 1;
            this.tabPage_ReqBody.Text = "Request Body";
            this.tabPage_ReqBody.UseVisualStyleBackColor = true;
            // 
            // tabPage_RespBody
            // 
            this.tabPage_RespBody.Location = new System.Drawing.Point(4, 29);
            this.tabPage_RespBody.Name = "tabPage_RespBody";
            this.tabPage_RespBody.Size = new System.Drawing.Size(1248, 145);
            this.tabPage_RespBody.TabIndex = 2;
            this.tabPage_RespBody.Text = "Response Body";
            this.tabPage_RespBody.UseVisualStyleBackColor = true;
            // 
            // tabPage_Timing
            // 
            this.tabPage_Timing.Controls.Add(this.textBox_Timing);
            this.tabPage_Timing.Location = new System.Drawing.Point(4, 29);
            this.tabPage_Timing.Name = "tabPage_Timing";
            this.tabPage_Timing.Padding = new System.Windows.Forms.Padding(8);
            this.tabPage_Timing.Size = new System.Drawing.Size(1248, 145);
            this.tabPage_Timing.TabIndex = 3;
            this.tabPage_Timing.Text = "Timing";
            this.tabPage_Timing.UseVisualStyleBackColor = true;
            // 
            // textBox_Timing
            // 
            this.textBox_Timing.BackColor = System.Drawing.Color.White;
            this.textBox_Timing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Timing.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.textBox_Timing.Location = new System.Drawing.Point(8, 8);
            this.textBox_Timing.Multiline = true;
            this.textBox_Timing.Name = "textBox_Timing";
            this.textBox_Timing.ReadOnly = true;
            this.textBox_Timing.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Timing.Size = new System.Drawing.Size(1232, 129);
            this.textBox_Timing.TabIndex = 0;
            // 
            // panel_DetailToolbar
            // 
            this.panel_DetailToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel_DetailToolbar.Controls.Add(this.label_CorrelationCaption);
            this.panel_DetailToolbar.Controls.Add(this.label_CorrelationValue);
            this.panel_DetailToolbar.Controls.Add(this.button_CopyHttpClient);
            this.panel_DetailToolbar.Controls.Add(this.button_CopyCurl);
            this.panel_DetailToolbar.Controls.Add(this.button_Replay);
            this.panel_DetailToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_DetailToolbar.Location = new System.Drawing.Point(0, 0);
            this.panel_DetailToolbar.Name = "panel_DetailToolbar";
            this.panel_DetailToolbar.Size = new System.Drawing.Size(1256, 36);
            this.panel_DetailToolbar.TabIndex = 0;
            // 
            // label_CorrelationCaption
            // 
            this.label_CorrelationCaption.AutoSize = true;
            this.label_CorrelationCaption.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_CorrelationCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_CorrelationCaption.Location = new System.Drawing.Point(420, 11);
            this.label_CorrelationCaption.Name = "label_CorrelationCaption";
            this.label_CorrelationCaption.Size = new System.Drawing.Size(109, 20);
            this.label_CorrelationCaption.TabIndex = 3;
            this.label_CorrelationCaption.Text = "Correlation Id:";
            // 
            // label_CorrelationValue
            // 
            this.label_CorrelationValue.AutoSize = true;
            this.label_CorrelationValue.Font = new System.Drawing.Font("Consolas", 9F);
            this.label_CorrelationValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(145)))), ((int)(((byte)(178)))));
            this.label_CorrelationValue.Location = new System.Drawing.Point(518, 11);
            this.label_CorrelationValue.Name = "label_CorrelationValue";
            this.label_CorrelationValue.Size = new System.Drawing.Size(16, 18);
            this.label_CorrelationValue.TabIndex = 4;
            this.label_CorrelationValue.Text = "—";
            // 
            // button_CopyHttpClient
            // 
            this.button_CopyHttpClient.BackColor = System.Drawing.Color.White;
            this.button_CopyHttpClient.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_CopyHttpClient.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_CopyHttpClient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_CopyHttpClient.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_CopyHttpClient.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.button_CopyHttpClient.Location = new System.Drawing.Point(252, 4);
            this.button_CopyHttpClient.Name = "button_CopyHttpClient";
            this.button_CopyHttpClient.Size = new System.Drawing.Size(150, 28);
            this.button_CopyHttpClient.TabIndex = 2;
            this.button_CopyHttpClient.Text = "⧉ Copy as HttpClient";
            this.button_CopyHttpClient.UseVisualStyleBackColor = false;
            this.button_CopyHttpClient.Click += new System.EventHandler(this.button_CopyHttpClient_Click);
            // 
            // button_CopyCurl
            // 
            this.button_CopyCurl.BackColor = System.Drawing.Color.White;
            this.button_CopyCurl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_CopyCurl.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_CopyCurl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_CopyCurl.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_CopyCurl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.button_CopyCurl.Location = new System.Drawing.Point(126, 4);
            this.button_CopyCurl.Name = "button_CopyCurl";
            this.button_CopyCurl.Size = new System.Drawing.Size(120, 28);
            this.button_CopyCurl.TabIndex = 1;
            this.button_CopyCurl.Text = "⧉ Copy as cURL";
            this.button_CopyCurl.UseVisualStyleBackColor = false;
            this.button_CopyCurl.Click += new System.EventHandler(this.button_CopyCurl_Click);
            // 
            // button_Replay
            // 
            this.button_Replay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(145)))), ((int)(((byte)(178)))));
            this.button_Replay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Replay.FlatAppearance.BorderSize = 0;
            this.button_Replay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Replay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.button_Replay.ForeColor = System.Drawing.Color.White;
            this.button_Replay.Location = new System.Drawing.Point(8, 4);
            this.button_Replay.Name = "button_Replay";
            this.button_Replay.Size = new System.Drawing.Size(110, 28);
            this.button_Replay.TabIndex = 0;
            this.button_Replay.Text = "↻ Replay";
            this.button_Replay.UseVisualStyleBackColor = false;
            this.button_Replay.Click += new System.EventHandler(this.button_Replay_Click);
            // 
            // panel_Output
            // 
            this.panel_Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.panel_Output.Controls.Add(this.richTextBox_Output);
            this.panel_Output.Controls.Add(this.label_Output);
            this.panel_Output.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_Output.Location = new System.Drawing.Point(0, 641);
            this.panel_Output.Name = "panel_Output";
            this.panel_Output.Padding = new System.Windows.Forms.Padding(12, 8, 12, 12);
            this.panel_Output.Size = new System.Drawing.Size(1280, 99);
            this.panel_Output.TabIndex = 3;
            // 
            // richTextBox_Output
            // 
            this.richTextBox_Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.richTextBox_Output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_Output.DetectUrls = false;
            this.richTextBox_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_Output.Font = new System.Drawing.Font("Consolas", 9F);
            this.richTextBox_Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.richTextBox_Output.HideSelection = false;
            this.richTextBox_Output.Location = new System.Drawing.Point(12, 28);
            this.richTextBox_Output.Name = "richTextBox_Output";
            this.richTextBox_Output.ReadOnly = true;
            this.richTextBox_Output.Size = new System.Drawing.Size(1256, 59);
            this.richTextBox_Output.TabIndex = 1;
            this.richTextBox_Output.Text = "";
            // 
            // label_Output
            // 
            this.label_Output.AutoSize = true;
            this.label_Output.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_Output.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.label_Output.Location = new System.Drawing.Point(12, 8);
            this.label_Output.Name = "label_Output";
            this.label_Output.Size = new System.Drawing.Size(39, 20);
            this.label_Output.TabIndex = 0;
            this.label_Output.Text = "LOG";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.label1.Location = new System.Drawing.Point(746, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Method";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.label2.Location = new System.Drawing.Point(871, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Status";
            // 
            // HttpProxyLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1280, 740);
            this.Controls.Add(this.panel_Grid);
            this.Controls.Add(this.panel_Output);
            this.Controls.Add(this.panel_RequestBar);
            this.Controls.Add(this.panel_TopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1298, 787);
            this.Name = "HttpProxyLogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThreatScanner — HTTP Proxy Log";
            this.panel_TopBar.ResumeLayout(false);
            this.panel_TopBar.PerformLayout();
            this.panel_RequestBar.ResumeLayout(false);
            this.panel_RequestBar.PerformLayout();
            this.panel_Grid.ResumeLayout(false);
            this.splitContainer_Main.Panel1.ResumeLayout(false);
            this.splitContainer_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).EndInit();
            this.splitContainer_Main.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Requests)).EndInit();
            this.panel_Detail.ResumeLayout(false);
            this.tabControl_Detail.ResumeLayout(false);
            this.tabPage_Headers.ResumeLayout(false);
            this.splitContainer_Headers.Panel1.ResumeLayout(false);
            this.splitContainer_Headers.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Headers)).EndInit();
            this.splitContainer_Headers.ResumeLayout(false);
            this.tabPage_Timing.ResumeLayout(false);
            this.tabPage_Timing.PerformLayout();
            this.panel_DetailToolbar.ResumeLayout(false);
            this.panel_DetailToolbar.PerformLayout();
            this.panel_Output.ResumeLayout(false);
            this.panel_Output.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_TopBar;
        private System.Windows.Forms.Label label_AppTitle;
        private System.Windows.Forms.Label label_AppSubtitle;
        private System.Windows.Forms.Button button_SaveLog;
        private System.Windows.Forms.Button button_ClearLog;
        private System.Windows.Forms.Panel panel_RequestBar;
        private System.Windows.Forms.Label label_Filter;
        private System.Windows.Forms.TextBox textBox_Filter;
        private System.Windows.Forms.Button button_StartCapture;
        private System.Windows.Forms.Button button_StopCapture;
        private System.Windows.Forms.ComboBox comboBox_MethodFilter;
        private System.Windows.Forms.ComboBox comboBox_StatusFilter;
        private System.Windows.Forms.Label label_Counter;
        private System.Windows.Forms.ProgressBar progressBar_Capture;
        private System.Windows.Forms.Panel panel_Grid;
        private System.Windows.Forms.SplitContainer splitContainer_Main;
        private System.Windows.Forms.DataGridView dataGridView_Requests;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Method;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Size;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Correlation;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Url;
        private System.Windows.Forms.Panel panel_Detail;
        private System.Windows.Forms.Panel panel_DetailToolbar;
        private System.Windows.Forms.Button button_Replay;
        private System.Windows.Forms.Button button_CopyCurl;
        private System.Windows.Forms.Button button_CopyHttpClient;
        private System.Windows.Forms.Label label_CorrelationCaption;
        private System.Windows.Forms.Label label_CorrelationValue;
        private System.Windows.Forms.TabControl tabControl_Detail;
        private System.Windows.Forms.TabPage tabPage_Headers;
        private System.Windows.Forms.SplitContainer splitContainer_Headers;
        private System.Windows.Forms.Label label_ReqHeaders;
        private System.Windows.Forms.ListView listView_ReqHeaders;
        private System.Windows.Forms.ColumnHeader colHdrReqName;
        private System.Windows.Forms.ColumnHeader colHdrReqValue;
        private System.Windows.Forms.Label label_RespHeaders;
        private System.Windows.Forms.ListView listView_RespHeaders;
        private System.Windows.Forms.ColumnHeader colHdrRespName;
        private System.Windows.Forms.ColumnHeader colHdrRespValue;
        private System.Windows.Forms.TabPage tabPage_ReqBody;
        private System.Windows.Forms.TabPage tabPage_RespBody;
        private System.Windows.Forms.TabPage tabPage_Timing;
        private System.Windows.Forms.TextBox textBox_Timing;
        private System.Windows.Forms.Panel panel_Output;
        private System.Windows.Forms.RichTextBox richTextBox_Output;
        private System.Windows.Forms.Label label_Output;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
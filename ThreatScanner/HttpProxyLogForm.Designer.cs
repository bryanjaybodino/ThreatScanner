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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel_Intercept = new System.Windows.Forms.Panel();
            this.label_InterceptTitle = new System.Windows.Forms.Label();
            this.label_InterceptHint = new System.Windows.Forms.Label();
            this.label_InterceptStatus = new System.Windows.Forms.Label();
            this.chk_Intercept_XHR = new System.Windows.Forms.CheckBox();
            this.chk_Intercept_Fetch = new System.Windows.Forms.CheckBox();
            this.chk_Intercept_Document = new System.Windows.Forms.CheckBox();
            this.chk_Intercept_Script = new System.Windows.Forms.CheckBox();
            this.chk_Intercept_Image = new System.Windows.Forms.CheckBox();
            this.chk_Intercept_Font = new System.Windows.Forms.CheckBox();
            this.chk_Intercept_CSS = new System.Windows.Forms.CheckBox();
            this.chk_Intercept_Other = new System.Windows.Forms.CheckBox();
            this.panel_InterceptQueue = new System.Windows.Forms.Panel();
            this.label_InterceptQueueTitle = new System.Windows.Forms.Label();
            this.label_InterceptQueueCount = new System.Windows.Forms.Label();
            this.listView_InterceptQueue = new System.Windows.Forms.ListView();
            this.col_IQ_Method = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_IQ_Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.col_IQ_Url = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btn_Intercept_Forward = new System.Windows.Forms.Button();
            this.btn_Intercept_Drop = new System.Windows.Forms.Button();
            this.btn_Intercept_ForwardAll = new System.Windows.Forms.Button();
            this.btn_Intercept_DropAll = new System.Windows.Forms.Button();
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
            this.button_Replay = new System.Windows.Forms.Button();
            this.button_CopyCurl = new System.Windows.Forms.Button();
            this.button_CopyHttpClient = new System.Windows.Forms.Button();
            this.panel_Output = new System.Windows.Forms.Panel();
            this.richTextBox_Output = new System.Windows.Forms.RichTextBox();
            this.label_Output = new System.Windows.Forms.Label();
            this.panel_TopBar.SuspendLayout();
            this.panel_RequestBar.SuspendLayout();
            this.panel_Intercept.SuspendLayout();
            this.panel_InterceptQueue.SuspendLayout();
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
            this.panel_TopBar.Size = new System.Drawing.Size(1380, 64);
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
            this.button_SaveLog.Location = new System.Drawing.Point(1240, 18);
            this.button_SaveLog.Name = "button_SaveLog";
            this.button_SaveLog.Size = new System.Drawing.Size(120, 32);
            this.button_SaveLog.TabIndex = 2;
            this.button_SaveLog.Text = "💾  Save CSV";
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
            this.button_ClearLog.Location = new System.Drawing.Point(1112, 18);
            this.button_ClearLog.Name = "button_ClearLog";
            this.button_ClearLog.Size = new System.Drawing.Size(120, 32);
            this.button_ClearLog.TabIndex = 3;
            this.button_ClearLog.Text = "🗑  Clear Log";
            this.button_ClearLog.UseVisualStyleBackColor = false;
            this.button_ClearLog.Click += new System.EventHandler(this.button_ClearLog_Click);
            // 
            // panel_RequestBar
            // 
            this.panel_RequestBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.panel_RequestBar.Controls.Add(this.label_Filter);
            this.panel_RequestBar.Controls.Add(this.textBox_Filter);
            this.panel_RequestBar.Controls.Add(this.button_StartCapture);
            this.panel_RequestBar.Controls.Add(this.button_StopCapture);
            this.panel_RequestBar.Controls.Add(this.comboBox_MethodFilter);
            this.panel_RequestBar.Controls.Add(this.comboBox_StatusFilter);
            this.panel_RequestBar.Controls.Add(this.label_Counter);
            this.panel_RequestBar.Controls.Add(this.progressBar_Capture);
            this.panel_RequestBar.Controls.Add(this.label1);
            this.panel_RequestBar.Controls.Add(this.label2);
            this.panel_RequestBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_RequestBar.Location = new System.Drawing.Point(0, 64);
            this.panel_RequestBar.Name = "panel_RequestBar";
            this.panel_RequestBar.Size = new System.Drawing.Size(1380, 50);
            this.panel_RequestBar.TabIndex = 1;
            // 
            // label_Filter
            // 
            this.label_Filter.AutoSize = true;
            this.label_Filter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_Filter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.label_Filter.Location = new System.Drawing.Point(12, 16);
            this.label_Filter.Name = "label_Filter";
            this.label_Filter.Size = new System.Drawing.Size(75, 20);
            this.label_Filter.TabIndex = 0;
            this.label_Filter.Text = "Filter URL:";
            // 
            // textBox_Filter
            // 
            this.textBox_Filter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Filter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBox_Filter.Location = new System.Drawing.Point(93, 13);
            this.textBox_Filter.Name = "textBox_Filter";
            this.textBox_Filter.Size = new System.Drawing.Size(289, 27);
            this.textBox_Filter.TabIndex = 1;
            this.textBox_Filter.TextChanged += new System.EventHandler(this.textBox_Filter_TextChanged);
            // 
            // button_StartCapture
            // 
            this.button_StartCapture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(145)))), ((int)(((byte)(178)))));
            this.button_StartCapture.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_StartCapture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_StartCapture.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.button_StartCapture.ForeColor = System.Drawing.Color.White;
            this.button_StartCapture.Location = new System.Drawing.Point(400, 11);
            this.button_StartCapture.Name = "button_StartCapture";
            this.button_StartCapture.Size = new System.Drawing.Size(120, 28);
            this.button_StartCapture.TabIndex = 2;
            this.button_StartCapture.Text = "▶  Start Capture";
            this.button_StartCapture.UseVisualStyleBackColor = false;
            this.button_StartCapture.Click += new System.EventHandler(this.button_StartCapture_Click);
            // 
            // button_StopCapture
            // 
            this.button_StopCapture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.button_StopCapture.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_StopCapture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_StopCapture.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.button_StopCapture.ForeColor = System.Drawing.Color.White;
            this.button_StopCapture.Location = new System.Drawing.Point(530, 11);
            this.button_StopCapture.Name = "button_StopCapture";
            this.button_StopCapture.Size = new System.Drawing.Size(110, 28);
            this.button_StopCapture.TabIndex = 3;
            this.button_StopCapture.Text = "■  Stop";
            this.button_StopCapture.UseVisualStyleBackColor = false;
            this.button_StopCapture.Click += new System.EventHandler(this.button_StopCapture_Click);
            // 
            // comboBox_MethodFilter
            // 
            this.comboBox_MethodFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_MethodFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBox_MethodFilter.Items.AddRange(new object[] {
            "All Methods",
            "GET",
            "POST",
            "PUT",
            "DELETE",
            "PATCH",
            "OPTIONS",
            "HEAD"});
            this.comboBox_MethodFilter.Location = new System.Drawing.Point(720, 12);
            this.comboBox_MethodFilter.Name = "comboBox_MethodFilter";
            this.comboBox_MethodFilter.Size = new System.Drawing.Size(110, 28);
            this.comboBox_MethodFilter.TabIndex = 4;
            this.comboBox_MethodFilter.SelectedIndexChanged += new System.EventHandler(this.comboBox_Filters_SelectedIndexChanged);
            // 
            // comboBox_StatusFilter
            // 
            this.comboBox_StatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_StatusFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBox_StatusFilter.Items.AddRange(new object[] {
            "All Status",
            "2xx",
            "3xx",
            "4xx",
            "5xx",
            "Failed",
            "Pending"});
            this.comboBox_StatusFilter.Location = new System.Drawing.Point(900, 12);
            this.comboBox_StatusFilter.Name = "comboBox_StatusFilter";
            this.comboBox_StatusFilter.Size = new System.Drawing.Size(100, 28);
            this.comboBox_StatusFilter.TabIndex = 5;
            this.comboBox_StatusFilter.SelectedIndexChanged += new System.EventHandler(this.comboBox_Filters_SelectedIndexChanged);
            // 
            // label_Counter
            // 
            this.label_Counter.AutoSize = true;
            this.label_Counter.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_Counter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_Counter.Location = new System.Drawing.Point(1106, 16);
            this.label_Counter.Name = "label_Counter";
            this.label_Counter.Size = new System.Drawing.Size(139, 20);
            this.label_Counter.TabIndex = 7;
            this.label_Counter.Text = "0 requests captured";
            // 
            // progressBar_Capture
            // 
            this.progressBar_Capture.Location = new System.Drawing.Point(1018, 18);
            this.progressBar_Capture.Name = "progressBar_Capture";
            this.progressBar_Capture.Size = new System.Drawing.Size(80, 14);
            this.progressBar_Capture.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.label1.Location = new System.Drawing.Point(660, 16);
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
            this.label2.Location = new System.Drawing.Point(842, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Status";
            // 
            // panel_Intercept
            // 
            this.panel_Intercept.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panel_Intercept.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_Intercept.Controls.Add(this.label_InterceptTitle);
            this.panel_Intercept.Controls.Add(this.label_InterceptHint);
            this.panel_Intercept.Controls.Add(this.label_InterceptStatus);
            this.panel_Intercept.Controls.Add(this.chk_Intercept_XHR);
            this.panel_Intercept.Controls.Add(this.chk_Intercept_Fetch);
            this.panel_Intercept.Controls.Add(this.chk_Intercept_Document);
            this.panel_Intercept.Controls.Add(this.chk_Intercept_Script);
            this.panel_Intercept.Controls.Add(this.chk_Intercept_Image);
            this.panel_Intercept.Controls.Add(this.chk_Intercept_Font);
            this.panel_Intercept.Controls.Add(this.chk_Intercept_CSS);
            this.panel_Intercept.Controls.Add(this.chk_Intercept_Other);
            this.panel_Intercept.Controls.Add(this.panel_InterceptQueue);
            this.panel_Intercept.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Intercept.Location = new System.Drawing.Point(0, 114);
            this.panel_Intercept.Name = "panel_Intercept";
            this.panel_Intercept.Size = new System.Drawing.Size(1380, 130);
            this.panel_Intercept.TabIndex = 2;
            // 
            // label_InterceptTitle
            // 
            this.label_InterceptTitle.AutoSize = true;
            this.label_InterceptTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label_InterceptTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(52)))), ((int)(((byte)(18)))));
            this.label_InterceptTitle.Location = new System.Drawing.Point(14, 10);
            this.label_InterceptTitle.Name = "label_InterceptTitle";
            this.label_InterceptTitle.Size = new System.Drawing.Size(117, 23);
            this.label_InterceptTitle.TabIndex = 0;
            this.label_InterceptTitle.Text = "⏸  Intercept";
            // 
            // label_InterceptHint
            // 
            this.label_InterceptHint.AutoSize = true;
            this.label_InterceptHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.label_InterceptHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.label_InterceptHint.Location = new System.Drawing.Point(133, 14);
            this.label_InterceptHint.Name = "label_InterceptHint";
            this.label_InterceptHint.Size = new System.Drawing.Size(440, 19);
            this.label_InterceptHint.TabIndex = 1;
            this.label_InterceptHint.Text = "Check resource types to pause matching requests before they are sent";
            // 
            // label_InterceptStatus
            // 
            this.label_InterceptStatus.AutoSize = true;
            this.label_InterceptStatus.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.label_InterceptStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_InterceptStatus.Location = new System.Drawing.Point(14, 32);
            this.label_InterceptStatus.Name = "label_InterceptStatus";
            this.label_InterceptStatus.Size = new System.Drawing.Size(113, 19);
            this.label_InterceptStatus.TabIndex = 2;
            this.label_InterceptStatus.Text = "Interception off";
            // 
            // chk_Intercept_XHR
            // 
            this.chk_Intercept_XHR.AutoSize = true;
            this.chk_Intercept_XHR.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk_Intercept_XHR.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(53)))), ((int)(((byte)(15)))));
            this.chk_Intercept_XHR.Location = new System.Drawing.Point(14, 59);
            this.chk_Intercept_XHR.Name = "chk_Intercept_XHR";
            this.chk_Intercept_XHR.Size = new System.Drawing.Size(60, 24);
            this.chk_Intercept_XHR.TabIndex = 3;
            this.chk_Intercept_XHR.Text = "XHR";
            // 
            // chk_Intercept_Fetch
            // 
            this.chk_Intercept_Fetch.AutoSize = true;
            this.chk_Intercept_Fetch.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk_Intercept_Fetch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(53)))), ((int)(((byte)(15)))));
            this.chk_Intercept_Fetch.Location = new System.Drawing.Point(15, 89);
            this.chk_Intercept_Fetch.Name = "chk_Intercept_Fetch";
            this.chk_Intercept_Fetch.Size = new System.Drawing.Size(66, 24);
            this.chk_Intercept_Fetch.TabIndex = 4;
            this.chk_Intercept_Fetch.Text = "Fetch";
            // 
            // chk_Intercept_Document
            // 
            this.chk_Intercept_Document.AutoSize = true;
            this.chk_Intercept_Document.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk_Intercept_Document.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(53)))), ((int)(((byte)(15)))));
            this.chk_Intercept_Document.Location = new System.Drawing.Point(124, 59);
            this.chk_Intercept_Document.Name = "chk_Intercept_Document";
            this.chk_Intercept_Document.Size = new System.Drawing.Size(100, 24);
            this.chk_Intercept_Document.TabIndex = 5;
            this.chk_Intercept_Document.Text = "Document";
            // 
            // chk_Intercept_Script
            // 
            this.chk_Intercept_Script.AutoSize = true;
            this.chk_Intercept_Script.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk_Intercept_Script.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(53)))), ((int)(((byte)(15)))));
            this.chk_Intercept_Script.Location = new System.Drawing.Point(124, 89);
            this.chk_Intercept_Script.Name = "chk_Intercept_Script";
            this.chk_Intercept_Script.Size = new System.Drawing.Size(69, 24);
            this.chk_Intercept_Script.TabIndex = 6;
            this.chk_Intercept_Script.Text = "Script";
            // 
            // chk_Intercept_Image
            // 
            this.chk_Intercept_Image.AutoSize = true;
            this.chk_Intercept_Image.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk_Intercept_Image.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(53)))), ((int)(((byte)(15)))));
            this.chk_Intercept_Image.Location = new System.Drawing.Point(274, 59);
            this.chk_Intercept_Image.Name = "chk_Intercept_Image";
            this.chk_Intercept_Image.Size = new System.Drawing.Size(73, 24);
            this.chk_Intercept_Image.TabIndex = 7;
            this.chk_Intercept_Image.Text = "Image";
            // 
            // chk_Intercept_Font
            // 
            this.chk_Intercept_Font.AutoSize = true;
            this.chk_Intercept_Font.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk_Intercept_Font.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(53)))), ((int)(((byte)(15)))));
            this.chk_Intercept_Font.Location = new System.Drawing.Point(274, 89);
            this.chk_Intercept_Font.Name = "chk_Intercept_Font";
            this.chk_Intercept_Font.Size = new System.Drawing.Size(60, 24);
            this.chk_Intercept_Font.TabIndex = 8;
            this.chk_Intercept_Font.Text = "Font";
            // 
            // chk_Intercept_CSS
            // 
            this.chk_Intercept_CSS.AutoSize = true;
            this.chk_Intercept_CSS.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk_Intercept_CSS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(53)))), ((int)(((byte)(15)))));
            this.chk_Intercept_CSS.Location = new System.Drawing.Point(381, 59);
            this.chk_Intercept_CSS.Name = "chk_Intercept_CSS";
            this.chk_Intercept_CSS.Size = new System.Drawing.Size(56, 24);
            this.chk_Intercept_CSS.TabIndex = 9;
            this.chk_Intercept_CSS.Text = "CSS";
            // 
            // chk_Intercept_Other
            // 
            this.chk_Intercept_Other.AutoSize = true;
            this.chk_Intercept_Other.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.chk_Intercept_Other.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(53)))), ((int)(((byte)(15)))));
            this.chk_Intercept_Other.Location = new System.Drawing.Point(381, 89);
            this.chk_Intercept_Other.Name = "chk_Intercept_Other";
            this.chk_Intercept_Other.Size = new System.Drawing.Size(68, 24);
            this.chk_Intercept_Other.TabIndex = 10;
            this.chk_Intercept_Other.Text = "Other";
            // 
            // panel_InterceptQueue
            // 
            this.panel_InterceptQueue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.panel_InterceptQueue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_InterceptQueue.Controls.Add(this.label_InterceptQueueTitle);
            this.panel_InterceptQueue.Controls.Add(this.label_InterceptQueueCount);
            this.panel_InterceptQueue.Controls.Add(this.listView_InterceptQueue);
            this.panel_InterceptQueue.Controls.Add(this.btn_Intercept_Forward);
            this.panel_InterceptQueue.Controls.Add(this.btn_Intercept_Drop);
            this.panel_InterceptQueue.Controls.Add(this.btn_Intercept_ForwardAll);
            this.panel_InterceptQueue.Controls.Add(this.btn_Intercept_DropAll);
            this.panel_InterceptQueue.Location = new System.Drawing.Point(631, 6);
            this.panel_InterceptQueue.Name = "panel_InterceptQueue";
            this.panel_InterceptQueue.Size = new System.Drawing.Size(729, 116);
            this.panel_InterceptQueue.TabIndex = 11;
            // 
            // label_InterceptQueueTitle
            // 
            this.label_InterceptQueueTitle.AutoSize = true;
            this.label_InterceptQueueTitle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_InterceptQueueTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(53)))), ((int)(((byte)(15)))));
            this.label_InterceptQueueTitle.Location = new System.Drawing.Point(6, 5);
            this.label_InterceptQueueTitle.Name = "label_InterceptQueueTitle";
            this.label_InterceptQueueTitle.Size = new System.Drawing.Size(109, 20);
            this.label_InterceptQueueTitle.TabIndex = 0;
            this.label_InterceptQueueTitle.Text = "Held Requests";
            // 
            // label_InterceptQueueCount
            // 
            this.label_InterceptQueueCount.AutoSize = true;
            this.label_InterceptQueueCount.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.label_InterceptQueueCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_InterceptQueueCount.Location = new System.Drawing.Point(110, 6);
            this.label_InterceptQueueCount.Name = "label_InterceptQueueCount";
            this.label_InterceptQueueCount.Size = new System.Drawing.Size(113, 19);
            this.label_InterceptQueueCount.TabIndex = 1;
            this.label_InterceptQueueCount.Text = "No requests held";
            // 
            // listView_InterceptQueue
            // 
            this.listView_InterceptQueue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col_IQ_Method,
            this.col_IQ_Type,
            this.col_IQ_Url});
            this.listView_InterceptQueue.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.listView_InterceptQueue.FullRowSelect = true;
            this.listView_InterceptQueue.GridLines = true;
            this.listView_InterceptQueue.HideSelection = false;
            this.listView_InterceptQueue.Location = new System.Drawing.Point(6, 22);
            this.listView_InterceptQueue.Name = "listView_InterceptQueue";
            this.listView_InterceptQueue.Size = new System.Drawing.Size(584, 86);
            this.listView_InterceptQueue.TabIndex = 2;
            this.listView_InterceptQueue.UseCompatibleStateImageBehavior = false;
            this.listView_InterceptQueue.View = System.Windows.Forms.View.Details;
            this.listView_InterceptQueue.SelectedIndexChanged += new System.EventHandler(this.listView_InterceptQueue_SelectedIndexChanged);
            // 
            // col_IQ_Method
            // 
            this.col_IQ_Method.Text = "Method";
            this.col_IQ_Method.Width = 56;
            // 
            // col_IQ_Type
            // 
            this.col_IQ_Type.Text = "Type";
            this.col_IQ_Type.Width = 70;
            // 
            // col_IQ_Url
            // 
            this.col_IQ_Url.Text = "URL";
            this.col_IQ_Url.Width = 330;
            // 
            // btn_Intercept_Forward
            // 
            this.btn_Intercept_Forward.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btn_Intercept_Forward.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Intercept_Forward.Enabled = false;
            this.btn_Intercept_Forward.FlatAppearance.BorderSize = 0;
            this.btn_Intercept_Forward.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Intercept_Forward.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btn_Intercept_Forward.ForeColor = System.Drawing.Color.White;
            this.btn_Intercept_Forward.Location = new System.Drawing.Point(596, 5);
            this.btn_Intercept_Forward.Name = "btn_Intercept_Forward";
            this.btn_Intercept_Forward.Size = new System.Drawing.Size(118, 24);
            this.btn_Intercept_Forward.TabIndex = 3;
            this.btn_Intercept_Forward.Text = "▶ Forward";
            this.btn_Intercept_Forward.UseVisualStyleBackColor = false;
            // 
            // btn_Intercept_Drop
            // 
            this.btn_Intercept_Drop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.btn_Intercept_Drop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Intercept_Drop.Enabled = false;
            this.btn_Intercept_Drop.FlatAppearance.BorderSize = 0;
            this.btn_Intercept_Drop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Intercept_Drop.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btn_Intercept_Drop.ForeColor = System.Drawing.Color.White;
            this.btn_Intercept_Drop.Location = new System.Drawing.Point(596, 33);
            this.btn_Intercept_Drop.Name = "btn_Intercept_Drop";
            this.btn_Intercept_Drop.Size = new System.Drawing.Size(118, 24);
            this.btn_Intercept_Drop.TabIndex = 4;
            this.btn_Intercept_Drop.Text = "✖ Drop";
            this.btn_Intercept_Drop.UseVisualStyleBackColor = false;
            // 
            // btn_Intercept_ForwardAll
            // 
            this.btn_Intercept_ForwardAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.btn_Intercept_ForwardAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Intercept_ForwardAll.Enabled = false;
            this.btn_Intercept_ForwardAll.FlatAppearance.BorderSize = 0;
            this.btn_Intercept_ForwardAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Intercept_ForwardAll.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btn_Intercept_ForwardAll.ForeColor = System.Drawing.Color.White;
            this.btn_Intercept_ForwardAll.Location = new System.Drawing.Point(596, 61);
            this.btn_Intercept_ForwardAll.Name = "btn_Intercept_ForwardAll";
            this.btn_Intercept_ForwardAll.Size = new System.Drawing.Size(118, 24);
            this.btn_Intercept_ForwardAll.TabIndex = 5;
            this.btn_Intercept_ForwardAll.Text = "▶▶ Forward All";
            this.btn_Intercept_ForwardAll.UseVisualStyleBackColor = false;
            // 
            // btn_Intercept_DropAll
            // 
            this.btn_Intercept_DropAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.btn_Intercept_DropAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Intercept_DropAll.Enabled = false;
            this.btn_Intercept_DropAll.FlatAppearance.BorderSize = 0;
            this.btn_Intercept_DropAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Intercept_DropAll.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btn_Intercept_DropAll.ForeColor = System.Drawing.Color.White;
            this.btn_Intercept_DropAll.Location = new System.Drawing.Point(596, 89);
            this.btn_Intercept_DropAll.Name = "btn_Intercept_DropAll";
            this.btn_Intercept_DropAll.Size = new System.Drawing.Size(118, 24);
            this.btn_Intercept_DropAll.TabIndex = 6;
            this.btn_Intercept_DropAll.Text = "✖✖ Drop All";
            this.btn_Intercept_DropAll.UseVisualStyleBackColor = false;
            // 
            // panel_Grid
            // 
            this.panel_Grid.Controls.Add(this.splitContainer_Main);
            this.panel_Grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Grid.Location = new System.Drawing.Point(0, 244);
            this.panel_Grid.Name = "panel_Grid";
            this.panel_Grid.Size = new System.Drawing.Size(1380, 518);
            this.panel_Grid.TabIndex = 3;
            // 
            // splitContainer_Main
            // 
            this.splitContainer_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Main.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_Main.Name = "splitContainer_Main";
            this.splitContainer_Main.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_Main.Panel1
            // 
            this.splitContainer_Main.Panel1.Controls.Add(this.dataGridView_Requests);
            // 
            // splitContainer_Main.Panel2
            // 
            this.splitContainer_Main.Panel2.Controls.Add(this.panel_Detail);
            this.splitContainer_Main.Size = new System.Drawing.Size(1380, 518);
            this.splitContainer_Main.SplitterDistance = 367;
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
            this.dataGridView_Requests.Size = new System.Drawing.Size(1380, 367);
            this.dataGridView_Requests.TabIndex = 0;
            this.dataGridView_Requests.SelectionChanged += new System.EventHandler(this.dataGridView_Requests_SelectionChanged);
            // 
            // col_Time
            // 
            this.col_Time.HeaderText = "Time";
            this.col_Time.MinimumWidth = 6;
            this.col_Time.Name = "col_Time";
            this.col_Time.ReadOnly = true;
            this.col_Time.Width = 90;
            // 
            // col_Method
            // 
            this.col_Method.HeaderText = "Method";
            this.col_Method.MinimumWidth = 6;
            this.col_Method.Name = "col_Method";
            this.col_Method.ReadOnly = true;
            this.col_Method.Width = 70;
            // 
            // col_Status
            // 
            this.col_Status.HeaderText = "Status";
            this.col_Status.MinimumWidth = 6;
            this.col_Status.Name = "col_Status";
            this.col_Status.ReadOnly = true;
            this.col_Status.Width = 60;
            // 
            // col_Type
            // 
            this.col_Type.HeaderText = "Type";
            this.col_Type.MinimumWidth = 6;
            this.col_Type.Name = "col_Type";
            this.col_Type.ReadOnly = true;
            this.col_Type.Width = 80;
            // 
            // col_Size
            // 
            this.col_Size.HeaderText = "Size";
            this.col_Size.MinimumWidth = 6;
            this.col_Size.Name = "col_Size";
            this.col_Size.ReadOnly = true;
            this.col_Size.Width = 70;
            // 
            // col_Correlation
            // 
            this.col_Correlation.HeaderText = "Correlation";
            this.col_Correlation.MinimumWidth = 6;
            this.col_Correlation.Name = "col_Correlation";
            this.col_Correlation.ReadOnly = true;
            this.col_Correlation.Width = 130;
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
            this.panel_Detail.Size = new System.Drawing.Size(1380, 147);
            this.panel_Detail.TabIndex = 0;
            // 
            // tabControl_Detail
            // 
            this.tabControl_Detail.Controls.Add(this.tabPage_Headers);
            this.tabControl_Detail.Controls.Add(this.tabPage_ReqBody);
            this.tabControl_Detail.Controls.Add(this.tabPage_RespBody);
            this.tabControl_Detail.Controls.Add(this.tabPage_Timing);
            this.tabControl_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Detail.Location = new System.Drawing.Point(0, 40);
            this.tabControl_Detail.Name = "tabControl_Detail";
            this.tabControl_Detail.SelectedIndex = 0;
            this.tabControl_Detail.Size = new System.Drawing.Size(1380, 107);
            this.tabControl_Detail.TabIndex = 1;
            // 
            // tabPage_Headers
            // 
            this.tabPage_Headers.Controls.Add(this.splitContainer_Headers);
            this.tabPage_Headers.Location = new System.Drawing.Point(4, 29);
            this.tabPage_Headers.Name = "tabPage_Headers";
            this.tabPage_Headers.Size = new System.Drawing.Size(1372, 74);
            this.tabPage_Headers.TabIndex = 0;
            this.tabPage_Headers.Text = "Headers";
            // 
            // splitContainer_Headers
            // 
            this.splitContainer_Headers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Headers.Location = new System.Drawing.Point(0, 0);
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
            this.splitContainer_Headers.Size = new System.Drawing.Size(1372, 74);
            this.splitContainer_Headers.SplitterDistance = 457;
            this.splitContainer_Headers.TabIndex = 0;
            // 
            // listView_ReqHeaders
            // 
            this.listView_ReqHeaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colHdrReqName,
            this.colHdrReqValue});
            this.listView_ReqHeaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_ReqHeaders.FullRowSelect = true;
            this.listView_ReqHeaders.HideSelection = false;
            this.listView_ReqHeaders.Location = new System.Drawing.Point(0, 22);
            this.listView_ReqHeaders.Name = "listView_ReqHeaders";
            this.listView_ReqHeaders.Size = new System.Drawing.Size(457, 52);
            this.listView_ReqHeaders.TabIndex = 1;
            this.listView_ReqHeaders.UseCompatibleStateImageBehavior = false;
            this.listView_ReqHeaders.View = System.Windows.Forms.View.Details;
            // 
            // colHdrReqName
            // 
            this.colHdrReqName.Text = "Name";
            this.colHdrReqName.Width = 180;
            // 
            // colHdrReqValue
            // 
            this.colHdrReqValue.Text = "Value";
            this.colHdrReqValue.Width = 400;
            // 
            // label_ReqHeaders
            // 
            this.label_ReqHeaders.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_ReqHeaders.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_ReqHeaders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.label_ReqHeaders.Location = new System.Drawing.Point(0, 0);
            this.label_ReqHeaders.Name = "label_ReqHeaders";
            this.label_ReqHeaders.Size = new System.Drawing.Size(457, 22);
            this.label_ReqHeaders.TabIndex = 0;
            this.label_ReqHeaders.Text = "Request Headers";
            // 
            // listView_RespHeaders
            // 
            this.listView_RespHeaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colHdrRespName,
            this.colHdrRespValue});
            this.listView_RespHeaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_RespHeaders.FullRowSelect = true;
            this.listView_RespHeaders.HideSelection = false;
            this.listView_RespHeaders.Location = new System.Drawing.Point(0, 22);
            this.listView_RespHeaders.Name = "listView_RespHeaders";
            this.listView_RespHeaders.Size = new System.Drawing.Size(911, 52);
            this.listView_RespHeaders.TabIndex = 1;
            this.listView_RespHeaders.UseCompatibleStateImageBehavior = false;
            this.listView_RespHeaders.View = System.Windows.Forms.View.Details;
            // 
            // colHdrRespName
            // 
            this.colHdrRespName.Text = "Name";
            this.colHdrRespName.Width = 180;
            // 
            // colHdrRespValue
            // 
            this.colHdrRespValue.Text = "Value";
            this.colHdrRespValue.Width = 400;
            // 
            // label_RespHeaders
            // 
            this.label_RespHeaders.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_RespHeaders.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_RespHeaders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.label_RespHeaders.Location = new System.Drawing.Point(0, 0);
            this.label_RespHeaders.Name = "label_RespHeaders";
            this.label_RespHeaders.Size = new System.Drawing.Size(911, 22);
            this.label_RespHeaders.TabIndex = 0;
            this.label_RespHeaders.Text = "Response Headers";
            // 
            // tabPage_ReqBody
            // 
            this.tabPage_ReqBody.Location = new System.Drawing.Point(4, 25);
            this.tabPage_ReqBody.Name = "tabPage_ReqBody";
            this.tabPage_ReqBody.Size = new System.Drawing.Size(1372, 68);
            this.tabPage_ReqBody.TabIndex = 1;
            this.tabPage_ReqBody.Text = "Request Body";
            // 
            // tabPage_RespBody
            // 
            this.tabPage_RespBody.Location = new System.Drawing.Point(4, 25);
            this.tabPage_RespBody.Name = "tabPage_RespBody";
            this.tabPage_RespBody.Size = new System.Drawing.Size(1372, 68);
            this.tabPage_RespBody.TabIndex = 2;
            this.tabPage_RespBody.Text = "Response Body";
            // 
            // tabPage_Timing
            // 
            this.tabPage_Timing.Controls.Add(this.textBox_Timing);
            this.tabPage_Timing.Location = new System.Drawing.Point(4, 25);
            this.tabPage_Timing.Name = "tabPage_Timing";
            this.tabPage_Timing.Size = new System.Drawing.Size(1372, 68);
            this.tabPage_Timing.TabIndex = 3;
            this.tabPage_Timing.Text = "Timing";
            // 
            // textBox_Timing
            // 
            this.textBox_Timing.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.textBox_Timing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_Timing.Font = new System.Drawing.Font("Consolas", 9F);
            this.textBox_Timing.Location = new System.Drawing.Point(0, 0);
            this.textBox_Timing.Multiline = true;
            this.textBox_Timing.Name = "textBox_Timing";
            this.textBox_Timing.ReadOnly = true;
            this.textBox_Timing.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Timing.Size = new System.Drawing.Size(1372, 68);
            this.textBox_Timing.TabIndex = 0;
            // 
            // panel_DetailToolbar
            // 
            this.panel_DetailToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.panel_DetailToolbar.Controls.Add(this.label_CorrelationCaption);
            this.panel_DetailToolbar.Controls.Add(this.label_CorrelationValue);
            this.panel_DetailToolbar.Controls.Add(this.button_Replay);
            this.panel_DetailToolbar.Controls.Add(this.button_CopyCurl);
            this.panel_DetailToolbar.Controls.Add(this.button_CopyHttpClient);
            this.panel_DetailToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_DetailToolbar.Location = new System.Drawing.Point(0, 0);
            this.panel_DetailToolbar.Name = "panel_DetailToolbar";
            this.panel_DetailToolbar.Size = new System.Drawing.Size(1380, 40);
            this.panel_DetailToolbar.TabIndex = 0;
            // 
            // label_CorrelationCaption
            // 
            this.label_CorrelationCaption.AutoSize = true;
            this.label_CorrelationCaption.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_CorrelationCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_CorrelationCaption.Location = new System.Drawing.Point(8, 11);
            this.label_CorrelationCaption.Name = "label_CorrelationCaption";
            this.label_CorrelationCaption.Size = new System.Drawing.Size(105, 20);
            this.label_CorrelationCaption.TabIndex = 0;
            this.label_CorrelationCaption.Text = "Correlation ID:";
            // 
            // label_CorrelationValue
            // 
            this.label_CorrelationValue.AutoSize = true;
            this.label_CorrelationValue.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_CorrelationValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(145)))), ((int)(((byte)(178)))));
            this.label_CorrelationValue.Location = new System.Drawing.Point(96, 11);
            this.label_CorrelationValue.Name = "label_CorrelationValue";
            this.label_CorrelationValue.Size = new System.Drawing.Size(24, 20);
            this.label_CorrelationValue.TabIndex = 1;
            this.label_CorrelationValue.Text = "—";
            // 
            // button_Replay
            // 
            this.button_Replay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(145)))), ((int)(((byte)(178)))));
            this.button_Replay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Replay.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.button_Replay.ForeColor = System.Drawing.Color.White;
            this.button_Replay.Location = new System.Drawing.Point(300, 7);
            this.button_Replay.Name = "button_Replay";
            this.button_Replay.Size = new System.Drawing.Size(80, 26);
            this.button_Replay.TabIndex = 2;
            this.button_Replay.Text = "▶ Replay";
            this.button_Replay.UseVisualStyleBackColor = false;
            this.button_Replay.Click += new System.EventHandler(this.button_Replay_Click);
            // 
            // button_CopyCurl
            // 
            this.button_CopyCurl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_CopyCurl.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.button_CopyCurl.Location = new System.Drawing.Point(390, 7);
            this.button_CopyCurl.Name = "button_CopyCurl";
            this.button_CopyCurl.Size = new System.Drawing.Size(90, 26);
            this.button_CopyCurl.TabIndex = 3;
            this.button_CopyCurl.Text = "📋 cURL";
            this.button_CopyCurl.Click += new System.EventHandler(this.button_CopyCurl_Click);
            // 
            // button_CopyHttpClient
            // 
            this.button_CopyHttpClient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_CopyHttpClient.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.button_CopyHttpClient.Location = new System.Drawing.Point(490, 7);
            this.button_CopyHttpClient.Name = "button_CopyHttpClient";
            this.button_CopyHttpClient.Size = new System.Drawing.Size(110, 26);
            this.button_CopyHttpClient.TabIndex = 4;
            this.button_CopyHttpClient.Text = "📋 HttpClient";
            this.button_CopyHttpClient.Click += new System.EventHandler(this.button_CopyHttpClient_Click);
            // 
            // panel_Output
            // 
            this.panel_Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.panel_Output.Controls.Add(this.richTextBox_Output);
            this.panel_Output.Controls.Add(this.label_Output);
            this.panel_Output.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_Output.Location = new System.Drawing.Point(0, 762);
            this.panel_Output.Name = "panel_Output";
            this.panel_Output.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.panel_Output.Size = new System.Drawing.Size(1380, 98);
            this.panel_Output.TabIndex = 4;
            // 
            // richTextBox_Output
            // 
            this.richTextBox_Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.richTextBox_Output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_Output.Font = new System.Drawing.Font("Consolas", 9F);
            this.richTextBox_Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.richTextBox_Output.Location = new System.Drawing.Point(12, 30);
            this.richTextBox_Output.Name = "richTextBox_Output";
            this.richTextBox_Output.ReadOnly = true;
            this.richTextBox_Output.Size = new System.Drawing.Size(1356, 60);
            this.richTextBox_Output.TabIndex = 1;
            this.richTextBox_Output.Text = "";
            // 
            // label_Output
            // 
            this.label_Output.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_Output.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.label_Output.Location = new System.Drawing.Point(12, 8);
            this.label_Output.Name = "label_Output";
            this.label_Output.Size = new System.Drawing.Size(1356, 22);
            this.label_Output.TabIndex = 0;
            this.label_Output.Text = "LOG";
            // 
            // HttpProxyLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1380, 860);
            this.Controls.Add(this.panel_Grid);
            this.Controls.Add(this.panel_Output);
            this.Controls.Add(this.panel_Intercept);
            this.Controls.Add(this.panel_RequestBar);
            this.Controls.Add(this.panel_TopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Name = "HttpProxyLogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThreatScanner — HTTP Proxy Log";
            this.panel_TopBar.ResumeLayout(false);
            this.panel_TopBar.PerformLayout();
            this.panel_RequestBar.ResumeLayout(false);
            this.panel_RequestBar.PerformLayout();
            this.panel_Intercept.ResumeLayout(false);
            this.panel_Intercept.PerformLayout();
            this.panel_InterceptQueue.ResumeLayout(false);
            this.panel_InterceptQueue.PerformLayout();
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
            this.ResumeLayout(false);

        }

        #endregion

        // ── Field declarations ─────────────────────────────────────────────────
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel_Intercept;
        private System.Windows.Forms.Label label_InterceptTitle;
        private System.Windows.Forms.Label label_InterceptHint;
        private System.Windows.Forms.Label label_InterceptStatus;
        private System.Windows.Forms.CheckBox chk_Intercept_XHR;
        private System.Windows.Forms.CheckBox chk_Intercept_Fetch;
        private System.Windows.Forms.CheckBox chk_Intercept_Document;
        private System.Windows.Forms.CheckBox chk_Intercept_Script;
        private System.Windows.Forms.CheckBox chk_Intercept_Image;
        private System.Windows.Forms.CheckBox chk_Intercept_Font;
        private System.Windows.Forms.CheckBox chk_Intercept_CSS;
        private System.Windows.Forms.CheckBox chk_Intercept_Other;
        private System.Windows.Forms.Panel panel_InterceptQueue;
        private System.Windows.Forms.Label label_InterceptQueueTitle;
        private System.Windows.Forms.Label label_InterceptQueueCount;
        private System.Windows.Forms.ListView listView_InterceptQueue;
        private System.Windows.Forms.ColumnHeader col_IQ_Method;
        private System.Windows.Forms.ColumnHeader col_IQ_Type;
        private System.Windows.Forms.ColumnHeader col_IQ_Url;
        private System.Windows.Forms.Button btn_Intercept_Forward;
        private System.Windows.Forms.Button btn_Intercept_Drop;
        private System.Windows.Forms.Button btn_Intercept_ForwardAll;
        private System.Windows.Forms.Button btn_Intercept_DropAll;
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
    }
}
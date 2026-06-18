namespace ThreatScanner
{
    partial class ApiTesterForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel_TopBar = new System.Windows.Forms.Panel();
            this.label_AppTitle = new System.Windows.Forms.Label();
            this.label_AppSubtitle = new System.Windows.Forms.Label();
            this.button_SaveReport = new System.Windows.Forms.Button();
            this.button_ClearOutput = new System.Windows.Forms.Button();
            this.panel_RequestBar = new System.Windows.Forms.Panel();
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
            this.panel_JsonEditor = new System.Windows.Forms.Panel();
            this.dataGridView_FormData = new System.Windows.Forms.DataGridView();
            this.col_FormEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.col_FormKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_FormValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.panel_Output = new System.Windows.Forms.Panel();
            this.richTextBox_Output = new System.Windows.Forms.RichTextBox();
            this.label_Output = new System.Windows.Forms.Label();
            this.progressBar_Scan = new System.Windows.Forms.ProgressBar();
            this.panel_TopBar.SuspendLayout();
            this.panel_RequestBar.SuspendLayout();
            this.tabControl_ApiDetail.SuspendLayout();
            this.tabPage_ApiParams.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Params)).BeginInit();
            this.tabPage_ApiHeaders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Headers)).BeginInit();
            this.tabPage_ApiBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_FormData)).BeginInit();
            this.panel_BodyType.SuspendLayout();
            this.tabPage_ApiAuth.SuspendLayout();
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
            this.panel_TopBar.TabIndex = 3;
            // 
            // label_AppTitle
            // 
            this.label_AppTitle.AutoSize = true;
            this.label_AppTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label_AppTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.label_AppTitle.Location = new System.Drawing.Point(16, 12);
            this.label_AppTitle.Name = "label_AppTitle";
            this.label_AppTitle.Size = new System.Drawing.Size(177, 32);
            this.label_AppTitle.TabIndex = 0;
            this.label_AppTitle.Text = "🛰  API Tester";
            // 
            // label_AppSubtitle
            // 
            this.label_AppSubtitle.AutoSize = true;
            this.label_AppSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_AppSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AppSubtitle.Location = new System.Drawing.Point(18, 40);
            this.label_AppSubtitle.Name = "label_AppSubtitle";
            this.label_AppSubtitle.Size = new System.Drawing.Size(436, 20);
            this.label_AppSubtitle.TabIndex = 1;
            this.label_AppSubtitle.Text = "Postman-style HTTP tester  •  Auth  •  Params  •  Headers  •  Body";
            // 
            // button_SaveReport
            // 
            this.button_SaveReport.BackColor = System.Drawing.Color.White;
            this.button_SaveReport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_SaveReport.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_SaveReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SaveReport.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_SaveReport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.button_SaveReport.Location = new System.Drawing.Point(1140, 16);
            this.button_SaveReport.Name = "button_SaveReport";
            this.button_SaveReport.Size = new System.Drawing.Size(124, 32);
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
            this.button_ClearOutput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.button_ClearOutput.Location = new System.Drawing.Point(1030, 16);
            this.button_ClearOutput.Name = "button_ClearOutput";
            this.button_ClearOutput.Size = new System.Drawing.Size(100, 32);
            this.button_ClearOutput.TabIndex = 3;
            this.button_ClearOutput.Text = "🗑  Clear";
            this.button_ClearOutput.UseVisualStyleBackColor = false;
            this.button_ClearOutput.Click += new System.EventHandler(this.button_ClearOutput_Click);
            // 
            // panel_RequestBar
            // 
            this.panel_RequestBar.BackColor = System.Drawing.Color.White;
            this.panel_RequestBar.Controls.Add(this.label_ApiMethod);
            this.panel_RequestBar.Controls.Add(this.comboBox_Method);
            this.panel_RequestBar.Controls.Add(this.label_ApiEndpoint);
            this.panel_RequestBar.Controls.Add(this.textBox_ApiEndpoint);
            this.panel_RequestBar.Controls.Add(this.button_ApiForce);
            this.panel_RequestBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_RequestBar.Location = new System.Drawing.Point(0, 64);
            this.panel_RequestBar.Name = "panel_RequestBar";
            this.panel_RequestBar.Size = new System.Drawing.Size(1280, 56);
            this.panel_RequestBar.TabIndex = 2;
            // 
            // label_ApiMethod
            // 
            this.label_ApiMethod.AutoSize = true;
            this.label_ApiMethod.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_ApiMethod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_ApiMethod.Location = new System.Drawing.Point(12, 18);
            this.label_ApiMethod.Name = "label_ApiMethod";
            this.label_ApiMethod.Size = new System.Drawing.Size(73, 20);
            this.label_ApiMethod.TabIndex = 0;
            this.label_ApiMethod.Text = "METHOD";
            // 
            // comboBox_Method
            // 
            this.comboBox_Method.BackColor = System.Drawing.Color.White;
            this.comboBox_Method.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Method.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Bold);
            this.comboBox_Method.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.comboBox_Method.Items.AddRange(new object[] {
            "GET",
            "POST",
            "PUT",
            "PATCH",
            "DELETE",
            "HEAD",
            "OPTIONS"});
            this.comboBox_Method.Location = new System.Drawing.Point(91, 12);
            this.comboBox_Method.Name = "comboBox_Method";
            this.comboBox_Method.Size = new System.Drawing.Size(89, 27);
            this.comboBox_Method.TabIndex = 1;
            // 
            // label_ApiEndpoint
            // 
            this.label_ApiEndpoint.AutoSize = true;
            this.label_ApiEndpoint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_ApiEndpoint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_ApiEndpoint.Location = new System.Drawing.Point(196, 18);
            this.label_ApiEndpoint.Name = "label_ApiEndpoint";
            this.label_ApiEndpoint.Size = new System.Drawing.Size(86, 20);
            this.label_ApiEndpoint.TabIndex = 2;
            this.label_ApiEndpoint.Text = "ENDPOINT";
            // 
            // textBox_ApiEndpoint
            // 
            this.textBox_ApiEndpoint.BackColor = System.Drawing.Color.White;
            this.textBox_ApiEndpoint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_ApiEndpoint.Font = new System.Drawing.Font("Consolas", 10F);
            this.textBox_ApiEndpoint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.textBox_ApiEndpoint.Location = new System.Drawing.Point(288, 12);
            this.textBox_ApiEndpoint.Name = "textBox_ApiEndpoint";
            this.textBox_ApiEndpoint.Size = new System.Drawing.Size(770, 27);
            this.textBox_ApiEndpoint.TabIndex = 3;
            // 
            // button_ApiForce
            // 
            this.button_ApiForce.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.button_ApiForce.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_ApiForce.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.button_ApiForce.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(120)))), ((int)(((byte)(87)))));
            this.button_ApiForce.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_ApiForce.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_ApiForce.ForeColor = System.Drawing.Color.White;
            this.button_ApiForce.Location = new System.Drawing.Point(1074, 8);
            this.button_ApiForce.Name = "button_ApiForce";
            this.button_ApiForce.Size = new System.Drawing.Size(190, 38);
            this.button_ApiForce.TabIndex = 4;
            this.button_ApiForce.Text = "▶  Send Request";
            this.button_ApiForce.UseVisualStyleBackColor = false;
            this.button_ApiForce.Click += new System.EventHandler(this.button_ApiForce_Click);
            // 
            // tabControl_ApiDetail
            // 
            this.tabControl_ApiDetail.Controls.Add(this.tabPage_ApiParams);
            this.tabControl_ApiDetail.Controls.Add(this.tabPage_ApiHeaders);
            this.tabControl_ApiDetail.Controls.Add(this.tabPage_ApiBody);
            this.tabControl_ApiDetail.Controls.Add(this.tabPage_ApiAuth);
            this.tabControl_ApiDetail.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl_ApiDetail.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabControl_ApiDetail.Location = new System.Drawing.Point(0, 120);
            this.tabControl_ApiDetail.Name = "tabControl_ApiDetail";
            this.tabControl_ApiDetail.SelectedIndex = 0;
            this.tabControl_ApiDetail.Size = new System.Drawing.Size(1280, 232);
            this.tabControl_ApiDetail.TabIndex = 1;
            // 
            // tabPage_ApiParams
            // 
            this.tabPage_ApiParams.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_ApiParams.Controls.Add(this.dataGridView_Params);
            this.tabPage_ApiParams.Location = new System.Drawing.Point(4, 29);
            this.tabPage_ApiParams.Name = "tabPage_ApiParams";
            this.tabPage_ApiParams.Size = new System.Drawing.Size(1272, 199);
            this.tabPage_ApiParams.TabIndex = 0;
            this.tabPage_ApiParams.Text = "  Params";
            // 
            // dataGridView_Params
            // 
            this.dataGridView_Params.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.dataGridView_Params.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_Params.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Params.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView_Params.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Params.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_ParamEnabled,
            this.col_ParamKey,
            this.col_ParamValue,
            this.col_ParamDesc});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_Params.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView_Params.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Params.EnableHeadersVisualStyles = false;
            this.dataGridView_Params.Font = new System.Drawing.Font("Consolas", 9F);
            this.dataGridView_Params.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dataGridView_Params.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_Params.Name = "dataGridView_Params";
            this.dataGridView_Params.RowHeadersVisible = false;
            this.dataGridView_Params.RowHeadersWidth = 51;
            this.dataGridView_Params.RowTemplate.Height = 26;
            this.dataGridView_Params.Size = new System.Drawing.Size(1272, 199);
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
            this.tabPage_ApiHeaders.Size = new System.Drawing.Size(1272, 199);
            this.tabPage_ApiHeaders.TabIndex = 1;
            this.tabPage_ApiHeaders.Text = "  Headers";
            // 
            // dataGridView_Headers
            // 
            this.dataGridView_Headers.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.dataGridView_Headers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_Headers.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Headers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView_Headers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Headers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_HdrEnabled,
            this.col_HdrKey,
            this.col_HdrValue});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Consolas", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_Headers.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView_Headers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Headers.EnableHeadersVisualStyles = false;
            this.dataGridView_Headers.Font = new System.Drawing.Font("Consolas", 9F);
            this.dataGridView_Headers.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dataGridView_Headers.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_Headers.Name = "dataGridView_Headers";
            this.dataGridView_Headers.RowHeadersVisible = false;
            this.dataGridView_Headers.RowHeadersWidth = 51;
            this.dataGridView_Headers.RowTemplate.Height = 26;
            this.dataGridView_Headers.Size = new System.Drawing.Size(1272, 199);
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
            this.col_HdrKey.HeaderText = "Key";
            this.col_HdrKey.MinimumWidth = 6;
            this.col_HdrKey.Name = "col_HdrKey";
            this.col_HdrKey.Width = 280;
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
            this.tabPage_ApiBody.Controls.Add(this.panel_JsonEditor);
            this.tabPage_ApiBody.Controls.Add(this.dataGridView_FormData);
            this.tabPage_ApiBody.Controls.Add(this.panel_BodyType);
            this.tabPage_ApiBody.Location = new System.Drawing.Point(4, 29);
            this.tabPage_ApiBody.Name = "tabPage_ApiBody";
            this.tabPage_ApiBody.Size = new System.Drawing.Size(1272, 199);
            this.tabPage_ApiBody.TabIndex = 2;
            this.tabPage_ApiBody.Text = "  Body";
            // 
            // panel_JsonEditor
            // 
            this.panel_JsonEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_JsonEditor.Location = new System.Drawing.Point(0, 32);
            this.panel_JsonEditor.Name = "panel_JsonEditor";
            this.panel_JsonEditor.Size = new System.Drawing.Size(1272, 167);
            this.panel_JsonEditor.TabIndex = 0;
            this.panel_JsonEditor.Visible = false;
            // 
            // dataGridView_FormData
            // 
            this.dataGridView_FormData.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.dataGridView_FormData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_FormData.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Consolas", 9F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_FormData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView_FormData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_FormData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_FormEnabled,
            this.col_FormKey,
            this.col_FormValue});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Consolas", 9F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_FormData.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView_FormData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_FormData.EnableHeadersVisualStyles = false;
            this.dataGridView_FormData.Font = new System.Drawing.Font("Consolas", 9F);
            this.dataGridView_FormData.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.dataGridView_FormData.Location = new System.Drawing.Point(0, 32);
            this.dataGridView_FormData.Name = "dataGridView_FormData";
            this.dataGridView_FormData.RowHeadersVisible = false;
            this.dataGridView_FormData.RowHeadersWidth = 51;
            this.dataGridView_FormData.RowTemplate.Height = 26;
            this.dataGridView_FormData.Size = new System.Drawing.Size(1272, 167);
            this.dataGridView_FormData.TabIndex = 1;
            this.dataGridView_FormData.Visible = false;
            // 
            // col_FormEnabled
            // 
            this.col_FormEnabled.FalseValue = false;
            this.col_FormEnabled.HeaderText = "";
            this.col_FormEnabled.MinimumWidth = 6;
            this.col_FormEnabled.Name = "col_FormEnabled";
            this.col_FormEnabled.TrueValue = true;
            this.col_FormEnabled.Width = 30;
            // 
            // col_FormKey
            // 
            this.col_FormKey.HeaderText = "Key";
            this.col_FormKey.MinimumWidth = 6;
            this.col_FormKey.Name = "col_FormKey";
            this.col_FormKey.Width = 300;
            // 
            // col_FormValue
            // 
            this.col_FormValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col_FormValue.HeaderText = "Value";
            this.col_FormValue.MinimumWidth = 6;
            this.col_FormValue.Name = "col_FormValue";
            // 
            // panel_BodyType
            // 
            this.panel_BodyType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel_BodyType.Controls.Add(this.radioButton_BodyNone);
            this.panel_BodyType.Controls.Add(this.radioButton_BodyForm);
            this.panel_BodyType.Controls.Add(this.radioButton_BodyJson);
            this.panel_BodyType.Controls.Add(this.radioButton_BodyRaw);
            this.panel_BodyType.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_BodyType.Location = new System.Drawing.Point(0, 0);
            this.panel_BodyType.Name = "panel_BodyType";
            this.panel_BodyType.Size = new System.Drawing.Size(1272, 32);
            this.panel_BodyType.TabIndex = 2;
            // 
            // radioButton_BodyNone
            // 
            this.radioButton_BodyNone.AutoSize = true;
            this.radioButton_BodyNone.Checked = true;
            this.radioButton_BodyNone.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioButton_BodyNone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
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
            this.radioButton_BodyForm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.radioButton_BodyForm.Location = new System.Drawing.Point(80, 6);
            this.radioButton_BodyForm.Name = "radioButton_BodyForm";
            this.radioButton_BodyForm.Size = new System.Drawing.Size(98, 24);
            this.radioButton_BodyForm.TabIndex = 1;
            this.radioButton_BodyForm.Text = "form-data";
            // 
            // radioButton_BodyJson
            // 
            this.radioButton_BodyJson.AutoSize = true;
            this.radioButton_BodyJson.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioButton_BodyJson.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.radioButton_BodyJson.Location = new System.Drawing.Point(186, 6);
            this.radioButton_BodyJson.Name = "radioButton_BodyJson";
            this.radioButton_BodyJson.Size = new System.Drawing.Size(65, 24);
            this.radioButton_BodyJson.TabIndex = 2;
            this.radioButton_BodyJson.Text = "JSON";
            // 
            // radioButton_BodyRaw
            // 
            this.radioButton_BodyRaw.AutoSize = true;
            this.radioButton_BodyRaw.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioButton_BodyRaw.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.radioButton_BodyRaw.Location = new System.Drawing.Point(258, 6);
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
            this.tabPage_ApiAuth.Size = new System.Drawing.Size(1272, 199);
            this.tabPage_ApiAuth.TabIndex = 3;
            this.tabPage_ApiAuth.Text = "  Auth";
            // 
            // label_AuthType
            // 
            this.label_AuthType.AutoSize = true;
            this.label_AuthType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_AuthType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AuthType.Location = new System.Drawing.Point(12, 16);
            this.label_AuthType.Name = "label_AuthType";
            this.label_AuthType.Size = new System.Drawing.Size(90, 20);
            this.label_AuthType.TabIndex = 0;
            this.label_AuthType.Text = "AUTH TYPE";
            // 
            // comboBox_AuthType
            // 
            this.comboBox_AuthType.BackColor = System.Drawing.Color.White;
            this.comboBox_AuthType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_AuthType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.comboBox_AuthType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.comboBox_AuthType.Items.AddRange(new object[] {
            "No Auth",
            "Bearer Token",
            "API Key",
            "Basic Auth",
            "Custom Header"});
            this.comboBox_AuthType.Location = new System.Drawing.Point(110, 10);
            this.comboBox_AuthType.Name = "comboBox_AuthType";
            this.comboBox_AuthType.Size = new System.Drawing.Size(180, 28);
            this.comboBox_AuthType.TabIndex = 1;
            // 
            // label_AuthKey
            // 
            this.label_AuthKey.AutoSize = true;
            this.label_AuthKey.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_AuthKey.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AuthKey.Location = new System.Drawing.Point(310, 16);
            this.label_AuthKey.Name = "label_AuthKey";
            this.label_AuthKey.Size = new System.Drawing.Size(133, 20);
            this.label_AuthKey.TabIndex = 2;
            this.label_AuthKey.Text = "KEY / USERNAME";
            // 
            // textBox_HeaderKey
            // 
            this.textBox_HeaderKey.BackColor = System.Drawing.Color.White;
            this.textBox_HeaderKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_HeaderKey.Font = new System.Drawing.Font("Consolas", 9F);
            this.textBox_HeaderKey.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.textBox_HeaderKey.Location = new System.Drawing.Point(449, 10);
            this.textBox_HeaderKey.Name = "textBox_HeaderKey";
            this.textBox_HeaderKey.Size = new System.Drawing.Size(191, 25);
            this.textBox_HeaderKey.TabIndex = 3;
            // 
            // label_AuthValue
            // 
            this.label_AuthValue.AutoSize = true;
            this.label_AuthValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_AuthValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AuthValue.Location = new System.Drawing.Point(660, 16);
            this.label_AuthValue.Name = "label_AuthValue";
            this.label_AuthValue.Size = new System.Drawing.Size(155, 20);
            this.label_AuthValue.TabIndex = 4;
            this.label_AuthValue.Text = "TOKEN / PASSWORD";
            // 
            // textBox_HeaderValue
            // 
            this.textBox_HeaderValue.BackColor = System.Drawing.Color.White;
            this.textBox_HeaderValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_HeaderValue.Font = new System.Drawing.Font("Consolas", 9F);
            this.textBox_HeaderValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.textBox_HeaderValue.Location = new System.Drawing.Point(816, 10);
            this.textBox_HeaderValue.Name = "textBox_HeaderValue";
            this.textBox_HeaderValue.PasswordChar = '●';
            this.textBox_HeaderValue.Size = new System.Drawing.Size(344, 25);
            this.textBox_HeaderValue.TabIndex = 5;
            // 
            // panel_Output
            // 
            this.panel_Output.BackColor = System.Drawing.Color.White;
            this.panel_Output.Controls.Add(this.richTextBox_Output);
            this.panel_Output.Controls.Add(this.label_Output);
            this.panel_Output.Controls.Add(this.progressBar_Scan);
            this.panel_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Output.Location = new System.Drawing.Point(0, 352);
            this.panel_Output.Name = "panel_Output";
            this.panel_Output.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.panel_Output.Size = new System.Drawing.Size(1280, 448);
            this.panel_Output.TabIndex = 0;
            // 
            // richTextBox_Output
            // 
            this.richTextBox_Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.richTextBox_Output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_Output.DetectUrls = false;
            this.richTextBox_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_Output.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.richTextBox_Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.richTextBox_Output.Location = new System.Drawing.Point(12, 28);
            this.richTextBox_Output.Name = "richTextBox_Output";
            this.richTextBox_Output.ReadOnly = true;
            this.richTextBox_Output.Size = new System.Drawing.Size(1256, 408);
            this.richTextBox_Output.TabIndex = 0;
            this.richTextBox_Output.Text = "";
            this.richTextBox_Output.WordWrap = false;
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
            this.progressBar_Scan.Location = new System.Drawing.Point(12, 436);
            this.progressBar_Scan.MarqueeAnimationSpeed = 30;
            this.progressBar_Scan.Name = "progressBar_Scan";
            this.progressBar_Scan.Size = new System.Drawing.Size(1256, 4);
            this.progressBar_Scan.TabIndex = 2;
            // 
            // ApiTesterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Controls.Add(this.panel_Output);
            this.Controls.Add(this.tabControl_ApiDetail);
            this.Controls.Add(this.panel_RequestBar);
            this.Controls.Add(this.panel_TopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Name = "ApiTesterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThreatScanner — API Tester";
            this.panel_TopBar.ResumeLayout(false);
            this.panel_TopBar.PerformLayout();
            this.panel_RequestBar.ResumeLayout(false);
            this.panel_RequestBar.PerformLayout();
            this.tabControl_ApiDetail.ResumeLayout(false);
            this.tabPage_ApiParams.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Params)).EndInit();
            this.tabPage_ApiHeaders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Headers)).EndInit();
            this.tabPage_ApiBody.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_FormData)).EndInit();
            this.panel_BodyType.ResumeLayout(false);
            this.panel_BodyType.PerformLayout();
            this.tabPage_ApiAuth.ResumeLayout(false);
            this.tabPage_ApiAuth.PerformLayout();
            this.panel_Output.ResumeLayout(false);
            this.panel_Output.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        // TopBar
        private System.Windows.Forms.Panel panel_TopBar;
        private System.Windows.Forms.Label label_AppTitle;
        private System.Windows.Forms.Label label_AppSubtitle;
        private System.Windows.Forms.Button button_SaveReport;
        private System.Windows.Forms.Button button_ClearOutput;
        // RequestBar
        private System.Windows.Forms.Panel panel_RequestBar;
        private System.Windows.Forms.Label label_ApiMethod;
        private System.Windows.Forms.ComboBox comboBox_Method;
        private System.Windows.Forms.Label label_ApiEndpoint;
        private System.Windows.Forms.TextBox textBox_ApiEndpoint;
        private System.Windows.Forms.Button button_ApiForce;
        // Detail tabs
        private System.Windows.Forms.TabControl tabControl_ApiDetail;
        private System.Windows.Forms.TabPage tabPage_ApiParams;
        private System.Windows.Forms.DataGridView dataGridView_Params;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_ParamEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ParamKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ParamValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ParamDesc;
        private System.Windows.Forms.TabPage tabPage_ApiHeaders;
        private System.Windows.Forms.DataGridView dataGridView_Headers;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_HdrEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_HdrKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_HdrValue;
        private System.Windows.Forms.TabPage tabPage_ApiBody;
        private System.Windows.Forms.Panel panel_BodyType;
        private System.Windows.Forms.RadioButton radioButton_BodyNone;
        private System.Windows.Forms.RadioButton radioButton_BodyForm;
        private System.Windows.Forms.RadioButton radioButton_BodyJson;
        private System.Windows.Forms.RadioButton radioButton_BodyRaw;
        private System.Windows.Forms.Panel panel_JsonEditor;       // replaces richTextBox_Body
        private System.Windows.Forms.DataGridView dataGridView_FormData;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_FormEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_FormKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_FormValue;
        private System.Windows.Forms.TabPage tabPage_ApiAuth;
        private System.Windows.Forms.Label label_AuthType;
        private System.Windows.Forms.ComboBox comboBox_AuthType;
        private System.Windows.Forms.Label label_AuthKey;
        private System.Windows.Forms.TextBox textBox_HeaderKey;
        private System.Windows.Forms.Label label_AuthValue;
        private System.Windows.Forms.TextBox textBox_HeaderValue;
        // Output
        private System.Windows.Forms.Panel panel_Output;
        private System.Windows.Forms.Label label_Output;
        private System.Windows.Forms.RichTextBox richTextBox_Output;
        private System.Windows.Forms.ProgressBar progressBar_Scan;
    }
}
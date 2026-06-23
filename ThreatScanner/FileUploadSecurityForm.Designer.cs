namespace ThreatScanner
{
    partial class FileUploadSecurityForm
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
            this.button_SaveResults = new System.Windows.Forms.Button();
            this.button_ClearLog = new System.Windows.Forms.Button();
            this.panel_RequestBar = new System.Windows.Forms.Panel();
            this.label_TargetUrl = new System.Windows.Forms.Label();
            this.textBox_PageUrl = new System.Windows.Forms.TextBox();
            this.label_FieldName = new System.Windows.Forms.Label();
            this.textBox_FieldName = new System.Windows.Forms.TextBox();
            this.button_RunTests = new System.Windows.Forms.Button();
            this.button_Stop = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.button_AutoDetect = new System.Windows.Forms.Button();
            this.label_DetectStatus = new System.Windows.Forms.Label();
            this.panel_Tests = new System.Windows.Forms.Panel();
            this.label_TestsTitle = new System.Windows.Forms.Label();
            this.button_SelectAll = new System.Windows.Forms.Button();
            this.button_SelectNone = new System.Windows.Forms.Button();
            this.checkBox_DoubleExtension = new System.Windows.Forms.CheckBox();
            this.checkBox_NullByte = new System.Windows.Forms.CheckBox();
            this.checkBox_CaseVariation = new System.Windows.Forms.CheckBox();
            this.checkBox_MimeSpoof = new System.Windows.Forms.CheckBox();
            this.checkBox_MagicByteMismatch = new System.Windows.Forms.CheckBox();
            this.checkBox_PathTraversal = new System.Windows.Forms.CheckBox();
            this.checkBox_NoExtension = new System.Windows.Forms.CheckBox();
            this.checkBox_TrailingDot = new System.Windows.Forms.CheckBox();
            this.checkBox_AlternateExtension = new System.Windows.Forms.CheckBox();
            this.checkBox_Oversized = new System.Windows.Forms.CheckBox();
            this.numericUpDown_OversizeMb = new System.Windows.Forms.NumericUpDown();
            this.label_OversizeMb = new System.Windows.Forms.Label();
            this.checkBox_ControlBaseline = new System.Windows.Forms.CheckBox();
            this.checkBox_IgnoreSslErrors = new System.Windows.Forms.CheckBox();
            this.panel_Grid = new System.Windows.Forms.Panel();
            this.dataGridView_Results = new System.Windows.Forms.DataGridView();
            this.col_TestName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ContentType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_HttpStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_RespSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Verdict = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel_Output = new System.Windows.Forms.Panel();
            this.richTextBox_Output = new System.Windows.Forms.RichTextBox();
            this.label_Output = new System.Windows.Forms.Label();
            this.panel_TopBar.SuspendLayout();
            this.panel_RequestBar.SuspendLayout();
            this.panel_Tests.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_OversizeMb)).BeginInit();
            this.panel_Grid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Results)).BeginInit();
            this.panel_Output.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_TopBar
            // 
            this.panel_TopBar.BackColor = System.Drawing.Color.White;
            this.panel_TopBar.Controls.Add(this.label_AppTitle);
            this.panel_TopBar.Controls.Add(this.label_AppSubtitle);
            this.panel_TopBar.Controls.Add(this.button_SaveResults);
            this.panel_TopBar.Controls.Add(this.button_ClearLog);
            this.panel_TopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_TopBar.Location = new System.Drawing.Point(0, 0);
            this.panel_TopBar.Name = "panel_TopBar";
            this.panel_TopBar.Size = new System.Drawing.Size(1320, 64);
            this.panel_TopBar.TabIndex = 0;
            // 
            // label_AppTitle
            // 
            this.label_AppTitle.AutoSize = true;
            this.label_AppTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label_AppTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.label_AppTitle.Location = new System.Drawing.Point(16, 12);
            this.label_AppTitle.Name = "label_AppTitle";
            this.label_AppTitle.Size = new System.Drawing.Size(289, 32);
            this.label_AppTitle.TabIndex = 0;
            this.label_AppTitle.Text = "📁  File Upload Security";
            // 
            // label_AppSubtitle
            // 
            this.label_AppSubtitle.AutoSize = true;
            this.label_AppSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_AppSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AppSubtitle.Location = new System.Drawing.Point(18, 40);
            this.label_AppSubtitle.Name = "label_AppSubtitle";
            this.label_AppSubtitle.Size = new System.Drawing.Size(554, 20);
            this.label_AppSubtitle.TabIndex = 1;
            this.label_AppSubtitle.Text = "Probes an upload endpoint for extension / MIME / magic-byte validation bypasses";
            // 
            // button_SaveResults
            // 
            this.button_SaveResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SaveResults.BackColor = System.Drawing.Color.White;
            this.button_SaveResults.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_SaveResults.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_SaveResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SaveResults.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_SaveResults.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.button_SaveResults.Location = new System.Drawing.Point(1080, 16);
            this.button_SaveResults.Name = "button_SaveResults";
            this.button_SaveResults.Size = new System.Drawing.Size(120, 32);
            this.button_SaveResults.TabIndex = 2;
            this.button_SaveResults.Text = "💾 Save CSV";
            this.button_SaveResults.UseVisualStyleBackColor = false;
            this.button_SaveResults.Click += new System.EventHandler(this.button_SaveResults_Click);
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
            this.button_ClearLog.Location = new System.Drawing.Point(1210, 16);
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
            this.panel_RequestBar.Controls.Add(this.label_TargetUrl);
            this.panel_RequestBar.Controls.Add(this.textBox_PageUrl);
            this.panel_RequestBar.Controls.Add(this.label_FieldName);
            this.panel_RequestBar.Controls.Add(this.textBox_FieldName);
            this.panel_RequestBar.Controls.Add(this.button_RunTests);
            this.panel_RequestBar.Controls.Add(this.button_Stop);
            this.panel_RequestBar.Controls.Add(this.progressBar);
            this.panel_RequestBar.Controls.Add(this.button_AutoDetect);
            this.panel_RequestBar.Controls.Add(this.label_DetectStatus);
            this.panel_RequestBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_RequestBar.Location = new System.Drawing.Point(0, 64);
            this.panel_RequestBar.Name = "panel_RequestBar";
            this.panel_RequestBar.Size = new System.Drawing.Size(1320, 110);
            this.panel_RequestBar.TabIndex = 1;
            // 
            // label_TargetUrl
            // 
            this.label_TargetUrl.AutoSize = true;
            this.label_TargetUrl.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_TargetUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.label_TargetUrl.Location = new System.Drawing.Point(16, 10);
            this.label_TargetUrl.Name = "label_TargetUrl";
            this.label_TargetUrl.Size = new System.Drawing.Size(152, 20);
            this.label_TargetUrl.TabIndex = 0;
            this.label_TargetUrl.Text = "Upload endpoint URL";
            // 
            // textBox_PageUrl
            // 
            this.textBox_PageUrl.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBox_PageUrl.Location = new System.Drawing.Point(16, 32);
            this.textBox_PageUrl.Name = "textBox_PageUrl";
            this.textBox_PageUrl.Size = new System.Drawing.Size(560, 30);
            this.textBox_PageUrl.TabIndex = 1;
            // 
            // label_FieldName
            // 
            this.label_FieldName.AutoSize = true;
            this.label_FieldName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_FieldName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.label_FieldName.Location = new System.Drawing.Point(588, 10);
            this.label_FieldName.Name = "label_FieldName";
            this.label_FieldName.Size = new System.Drawing.Size(118, 20);
            this.label_FieldName.TabIndex = 2;
            this.label_FieldName.Text = "Form field name";
            // 
            // textBox_FieldName
            // 
            this.textBox_FieldName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBox_FieldName.Location = new System.Drawing.Point(588, 32);
            this.textBox_FieldName.Name = "textBox_FieldName";
            this.textBox_FieldName.Size = new System.Drawing.Size(130, 30);
            this.textBox_FieldName.TabIndex = 3;
            this.textBox_FieldName.Text = "file";
            // 
            // button_RunTests
            // 
            this.button_RunTests.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.button_RunTests.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_RunTests.FlatAppearance.BorderSize = 0;
            this.button_RunTests.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_RunTests.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_RunTests.ForeColor = System.Drawing.Color.White;
            this.button_RunTests.Location = new System.Drawing.Point(736, 30);
            this.button_RunTests.Name = "button_RunTests";
            this.button_RunTests.Size = new System.Drawing.Size(150, 31);
            this.button_RunTests.TabIndex = 4;
            this.button_RunTests.Text = "▶  Run Tests";
            this.button_RunTests.UseVisualStyleBackColor = false;
            this.button_RunTests.Click += new System.EventHandler(this.button_RunTests_Click);
            // 
            // button_Stop
            // 
            this.button_Stop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.button_Stop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Stop.Enabled = false;
            this.button_Stop.FlatAppearance.BorderSize = 0;
            this.button_Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Stop.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_Stop.ForeColor = System.Drawing.Color.White;
            this.button_Stop.Location = new System.Drawing.Point(896, 30);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(96, 31);
            this.button_Stop.TabIndex = 5;
            this.button_Stop.Text = "■  Stop";
            this.button_Stop.UseVisualStyleBackColor = false;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(1012, 34);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(292, 22);
            this.progressBar.TabIndex = 6;
            // 
            // button_AutoDetect
            // 
            this.button_AutoDetect.BackColor = System.Drawing.Color.White;
            this.button_AutoDetect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_AutoDetect.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.button_AutoDetect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_AutoDetect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.button_AutoDetect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.button_AutoDetect.Location = new System.Drawing.Point(16, 68);
            this.button_AutoDetect.Name = "button_AutoDetect";
            this.button_AutoDetect.Size = new System.Drawing.Size(220, 30);
            this.button_AutoDetect.TabIndex = 7;
            this.button_AutoDetect.Text = "🔍  Auto-Detect Form";
            this.button_AutoDetect.UseVisualStyleBackColor = false;
            this.button_AutoDetect.Click += new System.EventHandler(this.button_AutoDetect_Click);
            // 
            // label_DetectStatus
            // 
            this.label_DetectStatus.AutoSize = true;
            this.label_DetectStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
            this.label_DetectStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_DetectStatus.Location = new System.Drawing.Point(242, 78);
            this.label_DetectStatus.Name = "label_DetectStatus";
            this.label_DetectStatus.Size = new System.Drawing.Size(452, 20);
            this.label_DetectStatus.TabIndex = 9;
            this.label_DetectStatus.Text = "Make sure the page with the upload form is open in your browser first.";
            // 
            // panel_Tests
            // 
            this.panel_Tests.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(253)))), ((int)(((byte)(250)))));
            this.panel_Tests.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_Tests.Controls.Add(this.label_TestsTitle);
            this.panel_Tests.Controls.Add(this.button_SelectAll);
            this.panel_Tests.Controls.Add(this.button_SelectNone);
            this.panel_Tests.Controls.Add(this.checkBox_DoubleExtension);
            this.panel_Tests.Controls.Add(this.checkBox_NullByte);
            this.panel_Tests.Controls.Add(this.checkBox_CaseVariation);
            this.panel_Tests.Controls.Add(this.checkBox_MimeSpoof);
            this.panel_Tests.Controls.Add(this.checkBox_MagicByteMismatch);
            this.panel_Tests.Controls.Add(this.checkBox_PathTraversal);
            this.panel_Tests.Controls.Add(this.checkBox_NoExtension);
            this.panel_Tests.Controls.Add(this.checkBox_TrailingDot);
            this.panel_Tests.Controls.Add(this.checkBox_AlternateExtension);
            this.panel_Tests.Controls.Add(this.checkBox_Oversized);
            this.panel_Tests.Controls.Add(this.numericUpDown_OversizeMb);
            this.panel_Tests.Controls.Add(this.label_OversizeMb);
            this.panel_Tests.Controls.Add(this.checkBox_ControlBaseline);
            this.panel_Tests.Controls.Add(this.checkBox_IgnoreSslErrors);
            this.panel_Tests.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Tests.Location = new System.Drawing.Point(0, 174);
            this.panel_Tests.Name = "panel_Tests";
            this.panel_Tests.Size = new System.Drawing.Size(1320, 160);
            this.panel_Tests.TabIndex = 2;
            // 
            // label_TestsTitle
            // 
            this.label_TestsTitle.AutoSize = true;
            this.label_TestsTitle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.label_TestsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.label_TestsTitle.Location = new System.Drawing.Point(10, 6);
            this.label_TestsTitle.Name = "label_TestsTitle";
            this.label_TestsTitle.Size = new System.Drawing.Size(143, 20);
            this.label_TestsTitle.TabIndex = 0;
            this.label_TestsTitle.Text = "Bypass tests to run";
            // 
            // button_SelectAll
            // 
            this.button_SelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SelectAll.BackColor = System.Drawing.Color.White;
            this.button_SelectAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_SelectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SelectAll.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.button_SelectAll.Location = new System.Drawing.Point(1112, 4);
            this.button_SelectAll.Name = "button_SelectAll";
            this.button_SelectAll.Size = new System.Drawing.Size(90, 33);
            this.button_SelectAll.TabIndex = 1;
            this.button_SelectAll.Text = "Select all";
            this.button_SelectAll.UseVisualStyleBackColor = false;
            this.button_SelectAll.Click += new System.EventHandler(this.button_SelectAll_Click);
            // 
            // button_SelectNone
            // 
            this.button_SelectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SelectNone.BackColor = System.Drawing.Color.White;
            this.button_SelectNone.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_SelectNone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SelectNone.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.button_SelectNone.Location = new System.Drawing.Point(1208, 4);
            this.button_SelectNone.Name = "button_SelectNone";
            this.button_SelectNone.Size = new System.Drawing.Size(100, 33);
            this.button_SelectNone.TabIndex = 2;
            this.button_SelectNone.Text = "Select none";
            this.button_SelectNone.UseVisualStyleBackColor = false;
            this.button_SelectNone.Click += new System.EventHandler(this.button_SelectNone_Click);
            // 
            // checkBox_DoubleExtension
            // 
            this.checkBox_DoubleExtension.AutoSize = true;
            this.checkBox_DoubleExtension.Checked = true;
            this.checkBox_DoubleExtension.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_DoubleExtension.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.checkBox_DoubleExtension.Location = new System.Drawing.Point(12, 32);
            this.checkBox_DoubleExtension.Name = "checkBox_DoubleExtension";
            this.checkBox_DoubleExtension.Size = new System.Drawing.Size(147, 24);
            this.checkBox_DoubleExtension.TabIndex = 3;
            this.checkBox_DoubleExtension.Text = "Double extension";
            this.checkBox_DoubleExtension.UseVisualStyleBackColor = true;
            // 
            // checkBox_NullByte
            // 
            this.checkBox_NullByte.AutoSize = true;
            this.checkBox_NullByte.Checked = true;
            this.checkBox_NullByte.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_NullByte.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.checkBox_NullByte.Location = new System.Drawing.Point(12, 58);
            this.checkBox_NullByte.Name = "checkBox_NullByte";
            this.checkBox_NullByte.Size = new System.Drawing.Size(152, 24);
            this.checkBox_NullByte.TabIndex = 4;
            this.checkBox_NullByte.Text = "Null byte injection";
            this.checkBox_NullByte.UseVisualStyleBackColor = true;
            // 
            // checkBox_CaseVariation
            // 
            this.checkBox_CaseVariation.AutoSize = true;
            this.checkBox_CaseVariation.Checked = true;
            this.checkBox_CaseVariation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_CaseVariation.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.checkBox_CaseVariation.Location = new System.Drawing.Point(12, 84);
            this.checkBox_CaseVariation.Name = "checkBox_CaseVariation";
            this.checkBox_CaseVariation.Size = new System.Drawing.Size(165, 24);
            this.checkBox_CaseVariation.TabIndex = 5;
            this.checkBox_CaseVariation.Text = "Case variation (.PhP)";
            this.checkBox_CaseVariation.UseVisualStyleBackColor = true;
            // 
            // checkBox_MimeSpoof
            // 
            this.checkBox_MimeSpoof.AutoSize = true;
            this.checkBox_MimeSpoof.Checked = true;
            this.checkBox_MimeSpoof.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_MimeSpoof.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.checkBox_MimeSpoof.Location = new System.Drawing.Point(182, 32);
            this.checkBox_MimeSpoof.Name = "checkBox_MimeSpoof";
            this.checkBox_MimeSpoof.Size = new System.Drawing.Size(144, 24);
            this.checkBox_MimeSpoof.TabIndex = 6;
            this.checkBox_MimeSpoof.Text = "MIME type spoof";
            this.checkBox_MimeSpoof.UseVisualStyleBackColor = true;
            // 
            // checkBox_MagicByteMismatch
            // 
            this.checkBox_MagicByteMismatch.AutoSize = true;
            this.checkBox_MagicByteMismatch.Checked = true;
            this.checkBox_MagicByteMismatch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_MagicByteMismatch.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.checkBox_MagicByteMismatch.Location = new System.Drawing.Point(182, 58);
            this.checkBox_MagicByteMismatch.Name = "checkBox_MagicByteMismatch";
            this.checkBox_MagicByteMismatch.Size = new System.Drawing.Size(245, 24);
            this.checkBox_MagicByteMismatch.TabIndex = 7;
            this.checkBox_MagicByteMismatch.Text = "Magic-byte / polyglot mismatch";
            this.checkBox_MagicByteMismatch.UseVisualStyleBackColor = true;
            // 
            // checkBox_PathTraversal
            // 
            this.checkBox_PathTraversal.AutoSize = true;
            this.checkBox_PathTraversal.Checked = true;
            this.checkBox_PathTraversal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_PathTraversal.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.checkBox_PathTraversal.Location = new System.Drawing.Point(182, 84);
            this.checkBox_PathTraversal.Name = "checkBox_PathTraversal";
            this.checkBox_PathTraversal.Size = new System.Drawing.Size(181, 24);
            this.checkBox_PathTraversal.TabIndex = 8;
            this.checkBox_PathTraversal.Text = "Path traversal filename";
            this.checkBox_PathTraversal.UseVisualStyleBackColor = true;
            // 
            // checkBox_NoExtension
            // 
            this.checkBox_NoExtension.AutoSize = true;
            this.checkBox_NoExtension.Checked = true;
            this.checkBox_NoExtension.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_NoExtension.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.checkBox_NoExtension.Location = new System.Drawing.Point(412, 32);
            this.checkBox_NoExtension.Name = "checkBox_NoExtension";
            this.checkBox_NoExtension.Size = new System.Drawing.Size(118, 24);
            this.checkBox_NoExtension.TabIndex = 9;
            this.checkBox_NoExtension.Text = "No extension";
            this.checkBox_NoExtension.UseVisualStyleBackColor = true;
            // 
            // checkBox_TrailingDot
            // 
            this.checkBox_TrailingDot.AutoSize = true;
            this.checkBox_TrailingDot.Checked = true;
            this.checkBox_TrailingDot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_TrailingDot.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.checkBox_TrailingDot.Location = new System.Drawing.Point(412, 58);
            this.checkBox_TrailingDot.Name = "checkBox_TrailingDot";
            this.checkBox_TrailingDot.Size = new System.Drawing.Size(260, 24);
            this.checkBox_TrailingDot.TabIndex = 10;
            this.checkBox_TrailingDot.Text = "Trailing dot/space (Windows alias)";
            this.checkBox_TrailingDot.UseVisualStyleBackColor = true;
            // 
            // checkBox_AlternateExtension
            // 
            this.checkBox_AlternateExtension.AutoSize = true;
            this.checkBox_AlternateExtension.Checked = true;
            this.checkBox_AlternateExtension.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AlternateExtension.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.checkBox_AlternateExtension.Location = new System.Drawing.Point(412, 84);
            this.checkBox_AlternateExtension.Name = "checkBox_AlternateExtension";
            this.checkBox_AlternateExtension.Size = new System.Drawing.Size(319, 24);
            this.checkBox_AlternateExtension.TabIndex = 11;
            this.checkBox_AlternateExtension.Text = "Alternate executable ext (.phtml/.asp/.jsp…)";
            this.checkBox_AlternateExtension.UseVisualStyleBackColor = true;
            // 
            // checkBox_Oversized
            // 
            this.checkBox_Oversized.AutoSize = true;
            this.checkBox_Oversized.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.checkBox_Oversized.Location = new System.Drawing.Point(720, 32);
            this.checkBox_Oversized.Name = "checkBox_Oversized";
            this.checkBox_Oversized.Size = new System.Drawing.Size(124, 24);
            this.checkBox_Oversized.TabIndex = 12;
            this.checkBox_Oversized.Text = "Oversized file:";
            this.checkBox_Oversized.UseVisualStyleBackColor = true;
            // 
            // numericUpDown_OversizeMb
            // 
            this.numericUpDown_OversizeMb.Location = new System.Drawing.Point(850, 31);
            this.numericUpDown_OversizeMb.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDown_OversizeMb.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_OversizeMb.Name = "numericUpDown_OversizeMb";
            this.numericUpDown_OversizeMb.Size = new System.Drawing.Size(70, 27);
            this.numericUpDown_OversizeMb.TabIndex = 13;
            this.numericUpDown_OversizeMb.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // label_OversizeMb
            // 
            this.label_OversizeMb.AutoSize = true;
            this.label_OversizeMb.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.label_OversizeMb.Location = new System.Drawing.Point(924, 34);
            this.label_OversizeMb.Name = "label_OversizeMb";
            this.label_OversizeMb.Size = new System.Drawing.Size(31, 20);
            this.label_OversizeMb.TabIndex = 14;
            this.label_OversizeMb.Text = "MB";
            // 
            // checkBox_ControlBaseline
            // 
            this.checkBox_ControlBaseline.AutoSize = true;
            this.checkBox_ControlBaseline.Checked = true;
            this.checkBox_ControlBaseline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_ControlBaseline.Font = new System.Drawing.Font("Segoe UI", 8.75F, System.Drawing.FontStyle.Bold);
            this.checkBox_ControlBaseline.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(118)))), ((int)(((byte)(110)))));
            this.checkBox_ControlBaseline.Location = new System.Drawing.Point(720, 58);
            this.checkBox_ControlBaseline.Name = "checkBox_ControlBaseline";
            this.checkBox_ControlBaseline.Size = new System.Drawing.Size(221, 24);
            this.checkBox_ControlBaseline.TabIndex = 15;
            this.checkBox_ControlBaseline.Text = "Control baseline (legit .jpg)";
            this.checkBox_ControlBaseline.UseVisualStyleBackColor = true;
            // 
            // checkBox_IgnoreSslErrors
            // 
            this.checkBox_IgnoreSslErrors.AutoSize = true;
            this.checkBox_IgnoreSslErrors.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.checkBox_IgnoreSslErrors.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(83)))), ((int)(((byte)(9)))));
            this.checkBox_IgnoreSslErrors.Location = new System.Drawing.Point(11, 114);
            this.checkBox_IgnoreSslErrors.Name = "checkBox_IgnoreSslErrors";
            this.checkBox_IgnoreSslErrors.Size = new System.Drawing.Size(407, 24);
            this.checkBox_IgnoreSslErrors.TabIndex = 16;
            this.checkBox_IgnoreSslErrors.Text = "Ignore SSL/TLS certificate errors (self-signed test targets)";
            this.checkBox_IgnoreSslErrors.UseVisualStyleBackColor = true;
            // 
            // panel_Grid
            // 
            this.panel_Grid.BackColor = System.Drawing.Color.White;
            this.panel_Grid.Controls.Add(this.dataGridView_Results);
            this.panel_Grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Grid.Location = new System.Drawing.Point(0, 334);
            this.panel_Grid.Name = "panel_Grid";
            this.panel_Grid.Padding = new System.Windows.Forms.Padding(12);
            this.panel_Grid.Size = new System.Drawing.Size(1320, 256);
            this.panel_Grid.TabIndex = 3;
            // 
            // dataGridView_Results
            // 
            this.dataGridView_Results.AllowUserToAddRows = false;
            this.dataGridView_Results.AllowUserToDeleteRows = false;
            this.dataGridView_Results.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView_Results.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_Results.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Results.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_TestName,
            this.col_FileName,
            this.col_ContentType,
            this.col_HttpStatus,
            this.col_RespSize,
            this.col_Time,
            this.col_Verdict});
            this.dataGridView_Results.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Results.Location = new System.Drawing.Point(12, 12);
            this.dataGridView_Results.Name = "dataGridView_Results";
            this.dataGridView_Results.ReadOnly = true;
            this.dataGridView_Results.RowHeadersVisible = false;
            this.dataGridView_Results.RowHeadersWidth = 51;
            this.dataGridView_Results.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_Results.Size = new System.Drawing.Size(1296, 232);
            this.dataGridView_Results.TabIndex = 0;
            // 
            // col_TestName
            // 
            this.col_TestName.HeaderText = "Test";
            this.col_TestName.MinimumWidth = 6;
            this.col_TestName.Name = "col_TestName";
            this.col_TestName.ReadOnly = true;
            this.col_TestName.Width = 240;
            // 
            // col_FileName
            // 
            this.col_FileName.HeaderText = "Filename sent";
            this.col_FileName.MinimumWidth = 6;
            this.col_FileName.Name = "col_FileName";
            this.col_FileName.ReadOnly = true;
            this.col_FileName.Width = 200;
            // 
            // col_ContentType
            // 
            this.col_ContentType.HeaderText = "Content-Type";
            this.col_ContentType.MinimumWidth = 6;
            this.col_ContentType.Name = "col_ContentType";
            this.col_ContentType.ReadOnly = true;
            this.col_ContentType.Width = 120;
            // 
            // col_HttpStatus
            // 
            this.col_HttpStatus.HeaderText = "Status";
            this.col_HttpStatus.MinimumWidth = 6;
            this.col_HttpStatus.Name = "col_HttpStatus";
            this.col_HttpStatus.ReadOnly = true;
            this.col_HttpStatus.Width = 70;
            // 
            // col_RespSize
            // 
            this.col_RespSize.HeaderText = "Resp. Size";
            this.col_RespSize.MinimumWidth = 6;
            this.col_RespSize.Name = "col_RespSize";
            this.col_RespSize.ReadOnly = true;
            this.col_RespSize.Width = 90;
            // 
            // col_Time
            // 
            this.col_Time.HeaderText = "Time";
            this.col_Time.MinimumWidth = 6;
            this.col_Time.Name = "col_Time";
            this.col_Time.ReadOnly = true;
            this.col_Time.Width = 80;
            // 
            // col_Verdict
            // 
            this.col_Verdict.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.col_Verdict.HeaderText = "Verdict (double-click row for response snippet)";
            this.col_Verdict.MinimumWidth = 6;
            this.col_Verdict.Name = "col_Verdict";
            this.col_Verdict.ReadOnly = true;
            // 
            // panel_Output
            // 
            this.panel_Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.panel_Output.Controls.Add(this.richTextBox_Output);
            this.panel_Output.Controls.Add(this.label_Output);
            this.panel_Output.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_Output.Location = new System.Drawing.Point(0, 590);
            this.panel_Output.Name = "panel_Output";
            this.panel_Output.Padding = new System.Windows.Forms.Padding(12, 8, 12, 12);
            this.panel_Output.Size = new System.Drawing.Size(1320, 150);
            this.panel_Output.TabIndex = 4;
            // 
            // richTextBox_Output
            // 
            this.richTextBox_Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.richTextBox_Output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_Output.Font = new System.Drawing.Font("Consolas", 9F);
            this.richTextBox_Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(212)))), ((int)(((byte)(212)))));
            this.richTextBox_Output.Location = new System.Drawing.Point(12, 28);
            this.richTextBox_Output.Name = "richTextBox_Output";
            this.richTextBox_Output.ReadOnly = true;
            this.richTextBox_Output.Size = new System.Drawing.Size(1296, 110);
            this.richTextBox_Output.TabIndex = 2;
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
            // FileUploadSecurityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1320, 740);
            this.Controls.Add(this.panel_Grid);
            this.Controls.Add(this.panel_Output);
            this.Controls.Add(this.panel_Tests);
            this.Controls.Add(this.panel_RequestBar);
            this.Controls.Add(this.panel_TopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 650);
            this.Name = "FileUploadSecurityForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThreatScanner — File Upload Security";
            this.panel_TopBar.ResumeLayout(false);
            this.panel_TopBar.PerformLayout();
            this.panel_RequestBar.ResumeLayout(false);
            this.panel_RequestBar.PerformLayout();
            this.panel_Tests.ResumeLayout(false);
            this.panel_Tests.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_OversizeMb)).EndInit();
            this.panel_Grid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Results)).EndInit();
            this.panel_Output.ResumeLayout(false);
            this.panel_Output.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_TopBar;
        private System.Windows.Forms.Label label_AppTitle;
        private System.Windows.Forms.Label label_AppSubtitle;
        private System.Windows.Forms.Button button_SaveResults;
        private System.Windows.Forms.Button button_ClearLog;
        private System.Windows.Forms.Panel panel_RequestBar;
        private System.Windows.Forms.Label label_TargetUrl;
        private System.Windows.Forms.TextBox textBox_PageUrl;
        private System.Windows.Forms.Label label_FieldName;
        private System.Windows.Forms.TextBox textBox_FieldName;
        private System.Windows.Forms.Button button_RunTests;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button button_AutoDetect;
        private System.Windows.Forms.Label label_DetectStatus;
        private System.Windows.Forms.Panel panel_Tests;
        private System.Windows.Forms.Label label_TestsTitle;
        private System.Windows.Forms.Button button_SelectAll;
        private System.Windows.Forms.Button button_SelectNone;
        private System.Windows.Forms.CheckBox checkBox_DoubleExtension;
        private System.Windows.Forms.CheckBox checkBox_NullByte;
        private System.Windows.Forms.CheckBox checkBox_CaseVariation;
        private System.Windows.Forms.CheckBox checkBox_MimeSpoof;
        private System.Windows.Forms.CheckBox checkBox_MagicByteMismatch;
        private System.Windows.Forms.CheckBox checkBox_PathTraversal;
        private System.Windows.Forms.CheckBox checkBox_NoExtension;
        private System.Windows.Forms.CheckBox checkBox_TrailingDot;
        private System.Windows.Forms.CheckBox checkBox_AlternateExtension;
        private System.Windows.Forms.CheckBox checkBox_Oversized;
        private System.Windows.Forms.NumericUpDown numericUpDown_OversizeMb;
        private System.Windows.Forms.Label label_OversizeMb;
        private System.Windows.Forms.CheckBox checkBox_ControlBaseline;
        private System.Windows.Forms.CheckBox checkBox_IgnoreSslErrors;
        private System.Windows.Forms.Panel panel_Grid;
        private System.Windows.Forms.DataGridView dataGridView_Results;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_TestName;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_FileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ContentType;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_HttpStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_RespSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Verdict;
        private System.Windows.Forms.Panel panel_Output;
        private System.Windows.Forms.Label label_Output;
        private System.Windows.Forms.RichTextBox richTextBox_Output;
    }
}
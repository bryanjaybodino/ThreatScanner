namespace ThreatScanner
{
    partial class WebSocketForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle36 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle37 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle42 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle38 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle39 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle40 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle41 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel_TopBar = new System.Windows.Forms.Panel();
            this.label_Title = new System.Windows.Forms.Label();
            this.label_Subtitle = new System.Windows.Forms.Label();
            this.button_ClearOutput = new System.Windows.Forms.Button();
            this.splitContainer_Main = new System.Windows.Forms.SplitContainer();
            this.tableLayout_Top = new System.Windows.Forms.TableLayoutPanel();
            this.panel_UrlRow = new System.Windows.Forms.Panel();
            this.label_WsUrl = new System.Windows.Forms.Label();
            this.textBox_WsUrl = new System.Windows.Forms.TextBox();
            this.button_Connect = new System.Windows.Forms.Button();
            this.button_Disconnect = new System.Windows.Forms.Button();
            this.button_TestConnection = new System.Windows.Forms.Button();
            this.panel_SubRow = new System.Windows.Forms.Panel();
            this.textBox_SubProtocol = new System.Windows.Forms.TextBox();
            this.label_SubProto = new System.Windows.Forms.Label();
            this.tabControl_Left = new System.Windows.Forms.TabControl();
            this.tabPage_Message = new System.Windows.Forms.TabPage();
            this.tableLayout_Send = new System.Windows.Forms.TableLayoutPanel();
            this.label_MsgHint = new System.Windows.Forms.Label();
            this.richTextBox_Message = new System.Windows.Forms.RichTextBox();
            this.panel_SendBottom = new System.Windows.Forms.Panel();
            this.button_Send = new System.Windows.Forms.Button();
            this.checkBox_SendBinary = new System.Windows.Forms.CheckBox();
            this.tabPage_Fuzz = new System.Windows.Forms.TabPage();
            this.tableLayout_Fuzz = new System.Windows.Forms.TableLayoutPanel();
            this.label_FuzzHint = new System.Windows.Forms.Label();
            this.panel_FuzzWordlistRow = new System.Windows.Forms.Panel();
            this.label_FuzzWordlist = new System.Windows.Forms.Label();
            this.textBox_FuzzWordlist = new System.Windows.Forms.TextBox();
            this.button_BrowseFuzzWordlist = new System.Windows.Forms.Button();
            this.panel_FuzzDelayRow = new System.Windows.Forms.Panel();
            this.label_FuzzDelay = new System.Windows.Forms.Label();
            this.numericUpDown_FuzzDelay = new System.Windows.Forms.NumericUpDown();
            this.button_Fuzz = new System.Windows.Forms.Button();
            this.tabPage_Headers = new System.Windows.Forms.TabPage();
            this.dataGridView_Headers = new System.Windows.Forms.DataGridView();
            this.col_WsHdrEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.col_WsHdrKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_WsHdrValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayout_Bottom = new System.Windows.Forms.TableLayoutPanel();
            this.panel_OutputBar = new System.Windows.Forms.Panel();
            this.progressBar_Scan = new System.Windows.Forms.ProgressBar();
            this.label_OutputTitle = new System.Windows.Forms.Label();
            this.dataGridView_Output = new System.Windows.Forms.DataGridView();
            this.col_Num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Direction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Data = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button_SaveReport = new System.Windows.Forms.Button();
            this.panel_TopBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).BeginInit();
            this.splitContainer_Main.Panel1.SuspendLayout();
            this.splitContainer_Main.Panel2.SuspendLayout();
            this.splitContainer_Main.SuspendLayout();
            this.tableLayout_Top.SuspendLayout();
            this.panel_UrlRow.SuspendLayout();
            this.panel_SubRow.SuspendLayout();
            this.tabControl_Left.SuspendLayout();
            this.tabPage_Message.SuspendLayout();
            this.tableLayout_Send.SuspendLayout();
            this.panel_SendBottom.SuspendLayout();
            this.tabPage_Fuzz.SuspendLayout();
            this.tableLayout_Fuzz.SuspendLayout();
            this.panel_FuzzWordlistRow.SuspendLayout();
            this.panel_FuzzDelayRow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_FuzzDelay)).BeginInit();
            this.tabPage_Headers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Headers)).BeginInit();
            this.tableLayout_Bottom.SuspendLayout();
            this.panel_OutputBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Output)).BeginInit();
            this.SuspendLayout();
            // 
            // panel_TopBar
            // 
            this.panel_TopBar.BackColor = System.Drawing.Color.White;
            this.panel_TopBar.Controls.Add(this.button_SaveReport);
            this.panel_TopBar.Controls.Add(this.label_Title);
            this.panel_TopBar.Controls.Add(this.label_Subtitle);
            this.panel_TopBar.Controls.Add(this.button_ClearOutput);
            this.panel_TopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_TopBar.Location = new System.Drawing.Point(0, 0);
            this.panel_TopBar.Name = "panel_TopBar";
            this.panel_TopBar.Size = new System.Drawing.Size(1100, 64);
            this.panel_TopBar.TabIndex = 3;
            // 
            // label_Title
            // 
            this.label_Title.AutoSize = true;
            this.label_Title.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label_Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(88)))), ((int)(((byte)(12)))));
            this.label_Title.Location = new System.Drawing.Point(16, 12);
            this.label_Title.Name = "label_Title";
            this.label_Title.Size = new System.Drawing.Size(263, 32);
            this.label_Title.TabIndex = 0;
            this.label_Title.Text = "🟠  WebSocket Tester";
            // 
            // label_Subtitle
            // 
            this.label_Subtitle.AutoSize = true;
            this.label_Subtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_Subtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_Subtitle.Location = new System.Drawing.Point(18, 40);
            this.label_Subtitle.Name = "label_Subtitle";
            this.label_Subtitle.Size = new System.Drawing.Size(357, 20);
            this.label_Subtitle.TabIndex = 1;
            this.label_Subtitle.Text = "Connect · Send · Receive · Fuzz WebSocket endpoints";
            // 
            // button_ClearOutput
            // 
            this.button_ClearOutput.BackColor = System.Drawing.Color.White;
            this.button_ClearOutput.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_ClearOutput.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_ClearOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_ClearOutput.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_ClearOutput.Location = new System.Drawing.Point(853, 15);
            this.button_ClearOutput.Name = "button_ClearOutput";
            this.button_ClearOutput.Size = new System.Drawing.Size(100, 32);
            this.button_ClearOutput.TabIndex = 3;
            this.button_ClearOutput.Text = "🗑  Clear";
            this.button_ClearOutput.UseVisualStyleBackColor = false;
            this.button_ClearOutput.Click += new System.EventHandler(this.button_ClearOutput_Click);
            // 
            // splitContainer_Main
            // 
            this.splitContainer_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Main.Location = new System.Drawing.Point(0, 64);
            this.splitContainer_Main.Name = "splitContainer_Main";
            this.splitContainer_Main.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_Main.Panel1
            // 
            this.splitContainer_Main.Panel1.Controls.Add(this.tableLayout_Top);
            // 
            // splitContainer_Main.Panel2
            // 
            this.splitContainer_Main.Panel2.Controls.Add(this.tableLayout_Bottom);
            this.splitContainer_Main.Size = new System.Drawing.Size(1100, 696);
            this.splitContainer_Main.SplitterDistance = 494;
            this.splitContainer_Main.TabIndex = 0;
            // 
            // tableLayout_Top
            // 
            this.tableLayout_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tableLayout_Top.ColumnCount = 1;
            this.tableLayout_Top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout_Top.Controls.Add(this.panel_UrlRow, 0, 0);
            this.tableLayout_Top.Controls.Add(this.panel_SubRow, 0, 1);
            this.tableLayout_Top.Controls.Add(this.tabControl_Left, 0, 2);
            this.tableLayout_Top.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout_Top.Location = new System.Drawing.Point(0, 0);
            this.tableLayout_Top.Name = "tableLayout_Top";
            this.tableLayout_Top.Padding = new System.Windows.Forms.Padding(8, 8, 8, 4);
            this.tableLayout_Top.RowCount = 3;
            this.tableLayout_Top.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayout_Top.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tableLayout_Top.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout_Top.Size = new System.Drawing.Size(1100, 494);
            this.tableLayout_Top.TabIndex = 0;
            // 
            // panel_UrlRow
            // 
            this.panel_UrlRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel_UrlRow.Controls.Add(this.label_WsUrl);
            this.panel_UrlRow.Controls.Add(this.textBox_WsUrl);
            this.panel_UrlRow.Controls.Add(this.button_Connect);
            this.panel_UrlRow.Controls.Add(this.button_Disconnect);
            this.panel_UrlRow.Controls.Add(this.button_TestConnection);
            this.panel_UrlRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_UrlRow.Location = new System.Drawing.Point(11, 11);
            this.panel_UrlRow.Name = "panel_UrlRow";
            this.panel_UrlRow.Size = new System.Drawing.Size(1078, 104);
            this.panel_UrlRow.TabIndex = 0;
            // 
            // label_WsUrl
            // 
            this.label_WsUrl.AutoSize = true;
            this.label_WsUrl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_WsUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_WsUrl.Location = new System.Drawing.Point(0, 0);
            this.label_WsUrl.Name = "label_WsUrl";
            this.label_WsUrl.Size = new System.Drawing.Size(130, 20);
            this.label_WsUrl.TabIndex = 0;
            this.label_WsUrl.Text = "WEBSOCKET URL";
            // 
            // textBox_WsUrl
            // 
            this.textBox_WsUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_WsUrl.BackColor = System.Drawing.Color.White;
            this.textBox_WsUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_WsUrl.Font = new System.Drawing.Font("Consolas", 10F);
            this.textBox_WsUrl.Location = new System.Drawing.Point(0, 20);
            this.textBox_WsUrl.Name = "textBox_WsUrl";
            this.textBox_WsUrl.Size = new System.Drawing.Size(1065, 27);
            this.textBox_WsUrl.TabIndex = 1;
            // 
            // button_Connect
            // 
            this.button_Connect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(88)))), ((int)(((byte)(12)))));
            this.button_Connect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Connect.FlatAppearance.BorderSize = 0;
            this.button_Connect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Connect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.button_Connect.ForeColor = System.Drawing.Color.White;
            this.button_Connect.Location = new System.Drawing.Point(0, 58);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(90, 34);
            this.button_Connect.TabIndex = 2;
            this.button_Connect.Text = "▶ Connect";
            this.button_Connect.UseVisualStyleBackColor = false;
            this.button_Connect.Click += new System.EventHandler(this.button_Connect_Click);
            // 
            // button_Disconnect
            // 
            this.button_Disconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.button_Disconnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Disconnect.Enabled = false;
            this.button_Disconnect.FlatAppearance.BorderSize = 0;
            this.button_Disconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Disconnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.button_Disconnect.ForeColor = System.Drawing.Color.White;
            this.button_Disconnect.Location = new System.Drawing.Point(96, 58);
            this.button_Disconnect.Name = "button_Disconnect";
            this.button_Disconnect.Size = new System.Drawing.Size(106, 34);
            this.button_Disconnect.TabIndex = 3;
            this.button_Disconnect.Text = "■ Disconnect";
            this.button_Disconnect.UseVisualStyleBackColor = false;
            this.button_Disconnect.Click += new System.EventHandler(this.button_Disconnect_Click);
            // 
            // button_TestConnection
            // 
            this.button_TestConnection.BackColor = System.Drawing.Color.Goldenrod;
            this.button_TestConnection.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_TestConnection.FlatAppearance.BorderSize = 0;
            this.button_TestConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_TestConnection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.button_TestConnection.ForeColor = System.Drawing.Color.White;
            this.button_TestConnection.Location = new System.Drawing.Point(208, 58);
            this.button_TestConnection.Name = "button_TestConnection";
            this.button_TestConnection.Size = new System.Drawing.Size(134, 34);
            this.button_TestConnection.TabIndex = 4;
            this.button_TestConnection.Text = "🔎 Test Connection";
            this.button_TestConnection.UseVisualStyleBackColor = false;
            this.button_TestConnection.Click += new System.EventHandler(this.button_TestConnection_Click);
            // 
            // panel_SubRow
            // 
            this.panel_SubRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel_SubRow.Controls.Add(this.textBox_SubProtocol);
            this.panel_SubRow.Controls.Add(this.label_SubProto);
            this.panel_SubRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_SubRow.Location = new System.Drawing.Point(11, 121);
            this.panel_SubRow.Name = "panel_SubRow";
            this.panel_SubRow.Size = new System.Drawing.Size(1078, 50);
            this.panel_SubRow.TabIndex = 1;
            // 
            // textBox_SubProtocol
            // 
            this.textBox_SubProtocol.BackColor = System.Drawing.Color.White;
            this.textBox_SubProtocol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_SubProtocol.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.textBox_SubProtocol.Location = new System.Drawing.Point(0, 18);
            this.textBox_SubProtocol.Name = "textBox_SubProtocol";
            this.textBox_SubProtocol.Size = new System.Drawing.Size(260, 26);
            this.textBox_SubProtocol.TabIndex = 1;
            // 
            // label_SubProto
            // 
            this.label_SubProto.AutoSize = true;
            this.label_SubProto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_SubProto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_SubProto.Location = new System.Drawing.Point(2, -4);
            this.label_SubProto.Name = "label_SubProto";
            this.label_SubProto.Size = new System.Drawing.Size(206, 20);
            this.label_SubProto.TabIndex = 0;
            this.label_SubProto.Text = "SUBPROTOCOL (OPTIONAL)";
            // 
            // tabControl_Left
            // 
            this.tabControl_Left.Controls.Add(this.tabPage_Message);
            this.tabControl_Left.Controls.Add(this.tabPage_Fuzz);
            this.tabControl_Left.Controls.Add(this.tabPage_Headers);
            this.tabControl_Left.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_Left.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.tabControl_Left.Location = new System.Drawing.Point(11, 177);
            this.tabControl_Left.Name = "tabControl_Left";
            this.tabControl_Left.SelectedIndex = 0;
            this.tabControl_Left.Size = new System.Drawing.Size(1078, 310);
            this.tabControl_Left.TabIndex = 2;
            // 
            // tabPage_Message
            // 
            this.tabPage_Message.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Message.Controls.Add(this.tableLayout_Send);
            this.tabPage_Message.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Message.Name = "tabPage_Message";
            this.tabPage_Message.Padding = new System.Windows.Forms.Padding(6);
            this.tabPage_Message.Size = new System.Drawing.Size(1070, 276);
            this.tabPage_Message.TabIndex = 0;
            this.tabPage_Message.Text = "  📤  Send";
            // 
            // tableLayout_Send
            // 
            this.tableLayout_Send.ColumnCount = 1;
            this.tableLayout_Send.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout_Send.Controls.Add(this.label_MsgHint, 0, 0);
            this.tableLayout_Send.Controls.Add(this.richTextBox_Message, 0, 1);
            this.tableLayout_Send.Controls.Add(this.panel_SendBottom, 0, 2);
            this.tableLayout_Send.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout_Send.Location = new System.Drawing.Point(6, 6);
            this.tableLayout_Send.Name = "tableLayout_Send";
            this.tableLayout_Send.RowCount = 3;
            this.tableLayout_Send.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayout_Send.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout_Send.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayout_Send.Size = new System.Drawing.Size(1058, 264);
            this.tableLayout_Send.TabIndex = 0;
            // 
            // label_MsgHint
            // 
            this.label_MsgHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_MsgHint.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.label_MsgHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_MsgHint.Location = new System.Drawing.Point(3, 0);
            this.label_MsgHint.Name = "label_MsgHint";
            this.label_MsgHint.Size = new System.Drawing.Size(1052, 22);
            this.label_MsgHint.TabIndex = 0;
            this.label_MsgHint.Text = "Message body — text, JSON, or hex bytes (when Binary is checked)  •  Ctrl+Enter t" +
    "o send";
            // 
            // richTextBox_Message
            // 
            this.richTextBox_Message.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_Message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_Message.Font = new System.Drawing.Font("Consolas", 10F);
            this.richTextBox_Message.Location = new System.Drawing.Point(3, 25);
            this.richTextBox_Message.Name = "richTextBox_Message";
            this.richTextBox_Message.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox_Message.Size = new System.Drawing.Size(1052, 190);
            this.richTextBox_Message.TabIndex = 1;
            this.richTextBox_Message.Text = "";
            // 
            // panel_SendBottom
            // 
            this.panel_SendBottom.Controls.Add(this.button_Send);
            this.panel_SendBottom.Controls.Add(this.checkBox_SendBinary);
            this.panel_SendBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_SendBottom.Location = new System.Drawing.Point(3, 221);
            this.panel_SendBottom.Name = "panel_SendBottom";
            this.panel_SendBottom.Padding = new System.Windows.Forms.Padding(4);
            this.panel_SendBottom.Size = new System.Drawing.Size(1052, 40);
            this.panel_SendBottom.TabIndex = 2;
            // 
            // button_Send
            // 
            this.button_Send.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(88)))), ((int)(((byte)(12)))));
            this.button_Send.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Send.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_Send.Enabled = false;
            this.button_Send.FlatAppearance.BorderSize = 0;
            this.button_Send.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Send.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.button_Send.ForeColor = System.Drawing.Color.White;
            this.button_Send.Location = new System.Drawing.Point(888, 4);
            this.button_Send.Name = "button_Send";
            this.button_Send.Size = new System.Drawing.Size(160, 32);
            this.button_Send.TabIndex = 0;
            this.button_Send.Text = "📤  Send Message";
            this.button_Send.UseVisualStyleBackColor = false;
            this.button_Send.Click += new System.EventHandler(this.button_Send_Click);
            // 
            // checkBox_SendBinary
            // 
            this.checkBox_SendBinary.AutoSize = true;
            this.checkBox_SendBinary.Dock = System.Windows.Forms.DockStyle.Left;
            this.checkBox_SendBinary.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.checkBox_SendBinary.Location = new System.Drawing.Point(4, 4);
            this.checkBox_SendBinary.Name = "checkBox_SendBinary";
            this.checkBox_SendBinary.Size = new System.Drawing.Size(164, 32);
            this.checkBox_SendBinary.TabIndex = 1;
            this.checkBox_SendBinary.Text = "Send as Binary (hex)";
            // 
            // tabPage_Fuzz
            // 
            this.tabPage_Fuzz.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Fuzz.Controls.Add(this.tableLayout_Fuzz);
            this.tabPage_Fuzz.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Fuzz.Name = "tabPage_Fuzz";
            this.tabPage_Fuzz.Padding = new System.Windows.Forms.Padding(6);
            this.tabPage_Fuzz.Size = new System.Drawing.Size(1070, 276);
            this.tabPage_Fuzz.TabIndex = 1;
            this.tabPage_Fuzz.Text = "  🎯  Fuzz";
            // 
            // tableLayout_Fuzz
            // 
            this.tableLayout_Fuzz.ColumnCount = 1;
            this.tableLayout_Fuzz.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout_Fuzz.Controls.Add(this.label_FuzzHint, 0, 0);
            this.tableLayout_Fuzz.Controls.Add(this.panel_FuzzWordlistRow, 0, 1);
            this.tableLayout_Fuzz.Controls.Add(this.panel_FuzzDelayRow, 0, 2);
            this.tableLayout_Fuzz.Controls.Add(this.button_Fuzz, 0, 3);
            this.tableLayout_Fuzz.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout_Fuzz.Location = new System.Drawing.Point(6, 6);
            this.tableLayout_Fuzz.Name = "tableLayout_Fuzz";
            this.tableLayout_Fuzz.RowCount = 5;
            this.tableLayout_Fuzz.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayout_Fuzz.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayout_Fuzz.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayout_Fuzz.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayout_Fuzz.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout_Fuzz.Size = new System.Drawing.Size(1058, 264);
            this.tableLayout_Fuzz.TabIndex = 0;
            // 
            // label_FuzzHint
            // 
            this.label_FuzzHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_FuzzHint.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_FuzzHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_FuzzHint.Location = new System.Drawing.Point(3, 0);
            this.label_FuzzHint.Name = "label_FuzzHint";
            this.label_FuzzHint.Size = new System.Drawing.Size(1052, 46);
            this.label_FuzzHint.TabIndex = 0;
            this.label_FuzzHint.Text = "Write the template in the Send tab. Use §FUZZ§ as the injection marker.\nLeave wor" +
    "dlist blank to use built-in payloads (XSS, SQLi, path traversal…)";
            // 
            // panel_FuzzWordlistRow
            // 
            this.panel_FuzzWordlistRow.Controls.Add(this.label_FuzzWordlist);
            this.panel_FuzzWordlistRow.Controls.Add(this.textBox_FuzzWordlist);
            this.panel_FuzzWordlistRow.Controls.Add(this.button_BrowseFuzzWordlist);
            this.panel_FuzzWordlistRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_FuzzWordlistRow.Location = new System.Drawing.Point(3, 49);
            this.panel_FuzzWordlistRow.Name = "panel_FuzzWordlistRow";
            this.panel_FuzzWordlistRow.Size = new System.Drawing.Size(1052, 54);
            this.panel_FuzzWordlistRow.TabIndex = 1;
            // 
            // label_FuzzWordlist
            // 
            this.label_FuzzWordlist.AutoSize = true;
            this.label_FuzzWordlist.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_FuzzWordlist.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_FuzzWordlist.Location = new System.Drawing.Point(0, 0);
            this.label_FuzzWordlist.Name = "label_FuzzWordlist";
            this.label_FuzzWordlist.Size = new System.Drawing.Size(250, 20);
            this.label_FuzzWordlist.TabIndex = 0;
            this.label_FuzzWordlist.Text = "PAYLOAD WORDLIST (OPTIONAL)";
            // 
            // textBox_FuzzWordlist
            // 
            this.textBox_FuzzWordlist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_FuzzWordlist.BackColor = System.Drawing.Color.White;
            this.textBox_FuzzWordlist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_FuzzWordlist.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBox_FuzzWordlist.Location = new System.Drawing.Point(0, 20);
            this.textBox_FuzzWordlist.Name = "textBox_FuzzWordlist";
            this.textBox_FuzzWordlist.Size = new System.Drawing.Size(952, 27);
            this.textBox_FuzzWordlist.TabIndex = 1;
            // 
            // button_BrowseFuzzWordlist
            // 
            this.button_BrowseFuzzWordlist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_BrowseFuzzWordlist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(88)))), ((int)(((byte)(12)))));
            this.button_BrowseFuzzWordlist.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_BrowseFuzzWordlist.FlatAppearance.BorderSize = 0;
            this.button_BrowseFuzzWordlist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_BrowseFuzzWordlist.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.button_BrowseFuzzWordlist.ForeColor = System.Drawing.Color.White;
            this.button_BrowseFuzzWordlist.Location = new System.Drawing.Point(1192, 18);
            this.button_BrowseFuzzWordlist.Name = "button_BrowseFuzzWordlist";
            this.button_BrowseFuzzWordlist.Size = new System.Drawing.Size(80, 28);
            this.button_BrowseFuzzWordlist.TabIndex = 2;
            this.button_BrowseFuzzWordlist.Text = "Browse…";
            this.button_BrowseFuzzWordlist.UseVisualStyleBackColor = false;
            this.button_BrowseFuzzWordlist.Click += new System.EventHandler(this.button_BrowseFuzzWordlist_Click);
            // 
            // panel_FuzzDelayRow
            // 
            this.panel_FuzzDelayRow.Controls.Add(this.label_FuzzDelay);
            this.panel_FuzzDelayRow.Controls.Add(this.numericUpDown_FuzzDelay);
            this.panel_FuzzDelayRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_FuzzDelayRow.Location = new System.Drawing.Point(3, 109);
            this.panel_FuzzDelayRow.Name = "panel_FuzzDelayRow";
            this.panel_FuzzDelayRow.Size = new System.Drawing.Size(1052, 54);
            this.panel_FuzzDelayRow.TabIndex = 2;
            // 
            // label_FuzzDelay
            // 
            this.label_FuzzDelay.AutoSize = true;
            this.label_FuzzDelay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_FuzzDelay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_FuzzDelay.Location = new System.Drawing.Point(0, 0);
            this.label_FuzzDelay.Name = "label_FuzzDelay";
            this.label_FuzzDelay.Size = new System.Drawing.Size(247, 20);
            this.label_FuzzDelay.TabIndex = 0;
            this.label_FuzzDelay.Text = "DELAY BETWEEN PAYLOADS (MS)";
            // 
            // numericUpDown_FuzzDelay
            // 
            this.numericUpDown_FuzzDelay.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericUpDown_FuzzDelay.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDown_FuzzDelay.Location = new System.Drawing.Point(0, 20);
            this.numericUpDown_FuzzDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDown_FuzzDelay.Name = "numericUpDown_FuzzDelay";
            this.numericUpDown_FuzzDelay.Size = new System.Drawing.Size(110, 27);
            this.numericUpDown_FuzzDelay.TabIndex = 1;
            this.numericUpDown_FuzzDelay.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // button_Fuzz
            // 
            this.button_Fuzz.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(88)))), ((int)(((byte)(12)))));
            this.button_Fuzz.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Fuzz.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Fuzz.Enabled = false;
            this.button_Fuzz.FlatAppearance.BorderSize = 0;
            this.button_Fuzz.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Fuzz.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_Fuzz.ForeColor = System.Drawing.Color.White;
            this.button_Fuzz.Location = new System.Drawing.Point(3, 169);
            this.button_Fuzz.Name = "button_Fuzz";
            this.button_Fuzz.Size = new System.Drawing.Size(1052, 40);
            this.button_Fuzz.TabIndex = 3;
            this.button_Fuzz.Text = "🎯  Start Fuzz";
            this.button_Fuzz.UseVisualStyleBackColor = false;
            this.button_Fuzz.Click += new System.EventHandler(this.button_Fuzz_Click);
            // 
            // tabPage_Headers
            // 
            this.tabPage_Headers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tabPage_Headers.Controls.Add(this.dataGridView_Headers);
            this.tabPage_Headers.Location = new System.Drawing.Point(4, 30);
            this.tabPage_Headers.Name = "tabPage_Headers";
            this.tabPage_Headers.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage_Headers.Size = new System.Drawing.Size(1070, 276);
            this.tabPage_Headers.TabIndex = 2;
            this.tabPage_Headers.Text = "  🔑  Headers";
            // 
            // dataGridView_Headers
            // 
            this.dataGridView_Headers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_Headers.ColumnHeadersHeight = 29;
            this.dataGridView_Headers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_WsHdrEnabled,
            this.col_WsHdrKey,
            this.col_WsHdrValue});
            this.dataGridView_Headers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Headers.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dataGridView_Headers.Location = new System.Drawing.Point(4, 4);
            this.dataGridView_Headers.Name = "dataGridView_Headers";
            this.dataGridView_Headers.RowHeadersVisible = false;
            this.dataGridView_Headers.RowHeadersWidth = 51;
            this.dataGridView_Headers.Size = new System.Drawing.Size(1062, 268);
            this.dataGridView_Headers.TabIndex = 0;
            // 
            // col_WsHdrEnabled
            // 
            this.col_WsHdrEnabled.FalseValue = false;
            this.col_WsHdrEnabled.FillWeight = 8F;
            this.col_WsHdrEnabled.HeaderText = "✓";
            this.col_WsHdrEnabled.MinimumWidth = 28;
            this.col_WsHdrEnabled.Name = "col_WsHdrEnabled";
            this.col_WsHdrEnabled.TrueValue = true;
            // 
            // col_WsHdrKey
            // 
            this.col_WsHdrKey.FillWeight = 46F;
            this.col_WsHdrKey.HeaderText = "Header Name";
            this.col_WsHdrKey.MinimumWidth = 6;
            this.col_WsHdrKey.Name = "col_WsHdrKey";
            // 
            // col_WsHdrValue
            // 
            this.col_WsHdrValue.FillWeight = 46F;
            this.col_WsHdrValue.HeaderText = "Value";
            this.col_WsHdrValue.MinimumWidth = 6;
            this.col_WsHdrValue.Name = "col_WsHdrValue";
            // 
            // tableLayout_Bottom
            // 
            this.tableLayout_Bottom.ColumnCount = 1;
            this.tableLayout_Bottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout_Bottom.Controls.Add(this.panel_OutputBar, 0, 0);
            this.tableLayout_Bottom.Controls.Add(this.dataGridView_Output, 0, 1);
            this.tableLayout_Bottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout_Bottom.Location = new System.Drawing.Point(0, 0);
            this.tableLayout_Bottom.Name = "tableLayout_Bottom";
            this.tableLayout_Bottom.RowCount = 2;
            this.tableLayout_Bottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayout_Bottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout_Bottom.Size = new System.Drawing.Size(1100, 198);
            this.tableLayout_Bottom.TabIndex = 0;
            // 
            // panel_OutputBar
            // 
            this.panel_OutputBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.panel_OutputBar.Controls.Add(this.progressBar_Scan);
            this.panel_OutputBar.Controls.Add(this.label_OutputTitle);
            this.panel_OutputBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_OutputBar.Location = new System.Drawing.Point(3, 3);
            this.panel_OutputBar.Name = "panel_OutputBar";
            this.panel_OutputBar.Padding = new System.Windows.Forms.Padding(8, 4, 6, 4);
            this.panel_OutputBar.Size = new System.Drawing.Size(1094, 32);
            this.panel_OutputBar.TabIndex = 0;
            // 
            // progressBar_Scan
            // 
            this.progressBar_Scan.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar_Scan.Location = new System.Drawing.Point(297, 24);
            this.progressBar_Scan.MarqueeAnimationSpeed = 30;
            this.progressBar_Scan.Name = "progressBar_Scan";
            this.progressBar_Scan.Size = new System.Drawing.Size(791, 4);
            this.progressBar_Scan.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar_Scan.TabIndex = 0;
            // 
            // label_OutputTitle
            // 
            this.label_OutputTitle.AutoSize = true;
            this.label_OutputTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_OutputTitle.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_OutputTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.label_OutputTitle.Location = new System.Drawing.Point(8, 4);
            this.label_OutputTitle.Name = "label_OutputTitle";
            this.label_OutputTitle.Size = new System.Drawing.Size(289, 17);
            this.label_OutputTitle.TabIndex = 1;
            this.label_OutputTitle.Text = "  📋  MESSAGE LOG  (double-click to inspect)";
            this.label_OutputTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataGridView_Output
            // 
            this.dataGridView_Output.AllowUserToAddRows = false;
            this.dataGridView_Output.AllowUserToDeleteRows = false;
            dataGridViewCellStyle36.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(48)))));
            this.dataGridView_Output.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle36;
            this.dataGridView_Output.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.dataGridView_Output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_Output.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGridView_Output.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView_Output.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle37.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle37.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle37.Font = new System.Drawing.Font("Consolas", 9F);
            dataGridViewCellStyle37.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            dataGridViewCellStyle37.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            dataGridViewCellStyle37.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle37.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_Output.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle37;
            this.dataGridView_Output.ColumnHeadersHeight = 28;
            this.dataGridView_Output.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView_Output.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Num,
            this.col_Time,
            this.col_Direction,
            this.col_Type,
            this.col_Status,
            this.col_Size,
            this.col_Data});
            dataGridViewCellStyle42.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle42.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            dataGridViewCellStyle42.Font = new System.Drawing.Font("Consolas", 9F);
            dataGridViewCellStyle42.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle42.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            dataGridViewCellStyle42.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            dataGridViewCellStyle42.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle42.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView_Output.DefaultCellStyle = dataGridViewCellStyle42;
            this.dataGridView_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_Output.Font = new System.Drawing.Font("Consolas", 9F);
            this.dataGridView_Output.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.dataGridView_Output.Location = new System.Drawing.Point(3, 41);
            this.dataGridView_Output.MultiSelect = false;
            this.dataGridView_Output.Name = "dataGridView_Output";
            this.dataGridView_Output.ReadOnly = true;
            this.dataGridView_Output.RowHeadersVisible = false;
            this.dataGridView_Output.RowHeadersWidth = 51;
            this.dataGridView_Output.RowTemplate.Height = 24;
            this.dataGridView_Output.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_Output.Size = new System.Drawing.Size(1094, 154);
            this.dataGridView_Output.TabIndex = 1;
            this.dataGridView_Output.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Output_CellDoubleClick);
            // 
            // col_Num
            // 
            dataGridViewCellStyle38.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle38.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.col_Num.DefaultCellStyle = dataGridViewCellStyle38;
            this.col_Num.HeaderText = "#";
            this.col_Num.MinimumWidth = 40;
            this.col_Num.Name = "col_Num";
            this.col_Num.ReadOnly = true;
            this.col_Num.Width = 48;
            // 
            // col_Time
            // 
            dataGridViewCellStyle39.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.col_Time.DefaultCellStyle = dataGridViewCellStyle39;
            this.col_Time.HeaderText = "Time";
            this.col_Time.MinimumWidth = 90;
            this.col_Time.Name = "col_Time";
            this.col_Time.ReadOnly = true;
            this.col_Time.Width = 125;
            // 
            // col_Direction
            // 
            this.col_Direction.HeaderText = "Direction";
            this.col_Direction.MinimumWidth = 70;
            this.col_Direction.Name = "col_Direction";
            this.col_Direction.ReadOnly = true;
            this.col_Direction.Width = 90;
            // 
            // col_Type
            // 
            this.col_Type.HeaderText = "Type";
            this.col_Type.MinimumWidth = 58;
            this.col_Type.Name = "col_Type";
            this.col_Type.ReadOnly = true;
            this.col_Type.Width = 72;
            // 
            // col_Status
            // 
            this.col_Status.HeaderText = "Status";
            this.col_Status.MinimumWidth = 70;
            this.col_Status.Name = "col_Status";
            this.col_Status.ReadOnly = true;
            this.col_Status.Width = 90;
            // 
            // col_Size
            // 
            dataGridViewCellStyle40.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle40.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.col_Size.DefaultCellStyle = dataGridViewCellStyle40;
            this.col_Size.HeaderText = "Size";
            this.col_Size.MinimumWidth = 55;
            this.col_Size.Name = "col_Size";
            this.col_Size.ReadOnly = true;
            this.col_Size.Width = 72;
            // 
            // col_Data
            // 
            this.col_Data.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle41.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Data.DefaultCellStyle = dataGridViewCellStyle41;
            this.col_Data.HeaderText = "Data";
            this.col_Data.MinimumWidth = 120;
            this.col_Data.Name = "col_Data";
            this.col_Data.ReadOnly = true;
            // 
            // button_SaveReport
            // 
            this.button_SaveReport.BackColor = System.Drawing.Color.White;
            this.button_SaveReport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_SaveReport.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_SaveReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SaveReport.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_SaveReport.Location = new System.Drawing.Point(959, 15);
            this.button_SaveReport.Name = "button_SaveReport";
            this.button_SaveReport.Size = new System.Drawing.Size(120, 32);
            this.button_SaveReport.TabIndex = 4;
            this.button_SaveReport.Text = "💾  Save Report";
            this.button_SaveReport.UseVisualStyleBackColor = false;
            this.button_SaveReport.Click += new System.EventHandler(this.button_SaveReport_Click);
            // 
            // WebSocketForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1100, 760);
            this.Controls.Add(this.splitContainer_Main);
            this.Controls.Add(this.panel_TopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(820, 600);
            this.Name = "WebSocketForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThreatScanner — WebSocket Tester";
            this.panel_TopBar.ResumeLayout(false);
            this.panel_TopBar.PerformLayout();
            this.splitContainer_Main.Panel1.ResumeLayout(false);
            this.splitContainer_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).EndInit();
            this.splitContainer_Main.ResumeLayout(false);
            this.tableLayout_Top.ResumeLayout(false);
            this.panel_UrlRow.ResumeLayout(false);
            this.panel_UrlRow.PerformLayout();
            this.panel_SubRow.ResumeLayout(false);
            this.panel_SubRow.PerformLayout();
            this.tabControl_Left.ResumeLayout(false);
            this.tabPage_Message.ResumeLayout(false);
            this.tableLayout_Send.ResumeLayout(false);
            this.panel_SendBottom.ResumeLayout(false);
            this.panel_SendBottom.PerformLayout();
            this.tabPage_Fuzz.ResumeLayout(false);
            this.tableLayout_Fuzz.ResumeLayout(false);
            this.panel_FuzzWordlistRow.ResumeLayout(false);
            this.panel_FuzzWordlistRow.PerformLayout();
            this.panel_FuzzDelayRow.ResumeLayout(false);
            this.panel_FuzzDelayRow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_FuzzDelay)).EndInit();
            this.tabPage_Headers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Headers)).EndInit();
            this.tableLayout_Bottom.ResumeLayout(false);
            this.panel_OutputBar.ResumeLayout(false);
            this.panel_OutputBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Output)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        // ── Control declarations ──────────────────────────────────────────────────
        private System.Windows.Forms.Panel panel_TopBar;
        private System.Windows.Forms.Label label_Title;
        private System.Windows.Forms.Label label_Subtitle;
        private System.Windows.Forms.Button button_ClearOutput;
        private System.Windows.Forms.SplitContainer splitContainer_Main;
        // TOP
        private System.Windows.Forms.TableLayoutPanel tableLayout_Top;
        private System.Windows.Forms.Panel panel_UrlRow;
        private System.Windows.Forms.Label label_WsUrl;
        private System.Windows.Forms.TextBox textBox_WsUrl;
        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.Button button_Disconnect;
        private System.Windows.Forms.Button button_TestConnection;
        private System.Windows.Forms.Panel panel_SubRow;
        private System.Windows.Forms.Label label_SubProto;
        private System.Windows.Forms.TextBox textBox_SubProtocol;
        private System.Windows.Forms.TabControl tabControl_Left;
        private System.Windows.Forms.TabPage tabPage_Message;
        private System.Windows.Forms.TableLayoutPanel tableLayout_Send;
        private System.Windows.Forms.Label label_MsgHint;
        private System.Windows.Forms.RichTextBox richTextBox_Message;
        private System.Windows.Forms.Panel panel_SendBottom;
        private System.Windows.Forms.CheckBox checkBox_SendBinary;
        private System.Windows.Forms.Button button_Send;
        private System.Windows.Forms.TabPage tabPage_Fuzz;
        private System.Windows.Forms.TableLayoutPanel tableLayout_Fuzz;
        private System.Windows.Forms.Label label_FuzzHint;
        private System.Windows.Forms.Panel panel_FuzzWordlistRow;
        private System.Windows.Forms.Label label_FuzzWordlist;
        private System.Windows.Forms.TextBox textBox_FuzzWordlist;
        private System.Windows.Forms.Button button_BrowseFuzzWordlist;
        private System.Windows.Forms.Panel panel_FuzzDelayRow;
        private System.Windows.Forms.Label label_FuzzDelay;
        private System.Windows.Forms.NumericUpDown numericUpDown_FuzzDelay;
        private System.Windows.Forms.Button button_Fuzz;
        private System.Windows.Forms.TabPage tabPage_Headers;
        private System.Windows.Forms.DataGridView dataGridView_Headers;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_WsHdrEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_WsHdrKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_WsHdrValue;
        // BOTTOM
        private System.Windows.Forms.TableLayoutPanel tableLayout_Bottom;
        private System.Windows.Forms.Panel panel_OutputBar;
        private System.Windows.Forms.Label label_OutputTitle;
        private System.Windows.Forms.ProgressBar progressBar_Scan;
        private System.Windows.Forms.DataGridView dataGridView_Output;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Num;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Direction;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Size;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Data;
        private System.Windows.Forms.Button button_SaveReport;
    }
}
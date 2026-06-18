namespace ThreatScanner
{
    partial class BruteForceForm
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
            this.panel_Config = new System.Windows.Forms.Panel();
            this.label_Username = new System.Windows.Forms.Label();
            this.textBox_Username = new System.Windows.Forms.TextBox();
            this.label_Password = new System.Windows.Forms.Label();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.label_Wordlist = new System.Windows.Forms.Label();
            this.textBox_WordlistPath = new System.Windows.Forms.TextBox();
            this.button_BrowseWordlist = new System.Windows.Forms.Button();
            this.button_BruteForce = new System.Windows.Forms.Button();
            this.label_BruteInfo = new System.Windows.Forms.Label();
            this.panel_Output = new System.Windows.Forms.Panel();
            this.listBox_Output = new System.Windows.Forms.ListBox();
            this.label_Output = new System.Windows.Forms.Label();
            this.progressBar_Scan = new System.Windows.Forms.ProgressBar();
            this.panel_TopBar.SuspendLayout();
            this.panel_UrlBar.SuspendLayout();
            this.panel_Config.SuspendLayout();
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
            this.panel_TopBar.Size = new System.Drawing.Size(900, 64);
            this.panel_TopBar.TabIndex = 3;
            // 
            // label_AppTitle
            // 
            this.label_AppTitle.AutoSize = true;
            this.label_AppTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label_AppTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.label_AppTitle.Location = new System.Drawing.Point(16, 12);
            this.label_AppTitle.Name = "label_AppTitle";
            this.label_AppTitle.Size = new System.Drawing.Size(193, 32);
            this.label_AppTitle.TabIndex = 0;
            this.label_AppTitle.Text = "🔐  Brute Force";
            // 
            // label_AppSubtitle
            // 
            this.label_AppSubtitle.AutoSize = true;
            this.label_AppSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.label_AppSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_AppSubtitle.Location = new System.Drawing.Point(18, 40);
            this.label_AppSubtitle.Name = "label_AppSubtitle";
            this.label_AppSubtitle.Size = new System.Drawing.Size(485, 20);
            this.label_AppSubtitle.TabIndex = 1;
            this.label_AppSubtitle.Text = "Auto-detects ASP.NET / PHP / HTML login forms and replays credentials";
            // 
            // button_SaveReport
            // 
            this.button_SaveReport.BackColor = System.Drawing.Color.White;
            this.button_SaveReport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_SaveReport.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_SaveReport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_SaveReport.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_SaveReport.Location = new System.Drawing.Point(760, 16);
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
            this.button_ClearOutput.Location = new System.Drawing.Point(650, 16);
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
            this.panel_UrlBar.Size = new System.Drawing.Size(900, 52);
            this.panel_UrlBar.TabIndex = 2;
            // 
            // label_UrlTitle
            // 
            this.label_UrlTitle.AutoSize = true;
            this.label_UrlTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_UrlTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_UrlTitle.Location = new System.Drawing.Point(16, 16);
            this.label_UrlTitle.Name = "label_UrlTitle";
            this.label_UrlTitle.Size = new System.Drawing.Size(89, 20);
            this.label_UrlTitle.TabIndex = 0;
            this.label_UrlTitle.Text = "LOGIN URL";
            // 
            // textBox_Url
            // 
            this.textBox_Url.BackColor = System.Drawing.Color.White;
            this.textBox_Url.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Url.Font = new System.Drawing.Font("Consolas", 10F);
            this.textBox_Url.Location = new System.Drawing.Point(100, 12);
            this.textBox_Url.Name = "textBox_Url";
            this.textBox_Url.Size = new System.Drawing.Size(780, 27);
            this.textBox_Url.TabIndex = 1;
            // 
            // panel_Config
            // 
            this.panel_Config.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel_Config.Controls.Add(this.label_Username);
            this.panel_Config.Controls.Add(this.textBox_Username);
            this.panel_Config.Controls.Add(this.label_Password);
            this.panel_Config.Controls.Add(this.textBox_Password);
            this.panel_Config.Controls.Add(this.label_Wordlist);
            this.panel_Config.Controls.Add(this.textBox_WordlistPath);
            this.panel_Config.Controls.Add(this.button_BrowseWordlist);
            this.panel_Config.Controls.Add(this.button_BruteForce);
            this.panel_Config.Controls.Add(this.label_BruteInfo);
            this.panel_Config.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Config.Location = new System.Drawing.Point(0, 116);
            this.panel_Config.Name = "panel_Config";
            this.panel_Config.Size = new System.Drawing.Size(900, 100);
            this.panel_Config.TabIndex = 1;
            // 
            // label_Username
            // 
            this.label_Username.AutoSize = true;
            this.label_Username.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_Username.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_Username.Location = new System.Drawing.Point(16, 14);
            this.label_Username.Name = "label_Username";
            this.label_Username.Size = new System.Drawing.Size(91, 20);
            this.label_Username.TabIndex = 0;
            this.label_Username.Text = "USERNAME";
            // 
            // textBox_Username
            // 
            this.textBox_Username.BackColor = System.Drawing.Color.White;
            this.textBox_Username.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Username.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.textBox_Username.Location = new System.Drawing.Point(110, 10);
            this.textBox_Username.Name = "textBox_Username";
            this.textBox_Username.Size = new System.Drawing.Size(180, 26);
            this.textBox_Username.TabIndex = 1;
            // 
            // label_Password
            // 
            this.label_Password.AutoSize = true;
            this.label_Password.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_Password.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_Password.Location = new System.Drawing.Point(310, 14);
            this.label_Password.Name = "label_Password";
            this.label_Password.Size = new System.Drawing.Size(91, 20);
            this.label_Password.TabIndex = 2;
            this.label_Password.Text = "PASSWORD";
            // 
            // textBox_Password
            // 
            this.textBox_Password.BackColor = System.Drawing.Color.White;
            this.textBox_Password.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_Password.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.textBox_Password.Location = new System.Drawing.Point(405, 10);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.PasswordChar = '●';
            this.textBox_Password.Size = new System.Drawing.Size(150, 26);
            this.textBox_Password.TabIndex = 3;
            // 
            // label_Wordlist
            // 
            this.label_Wordlist.AutoSize = true;
            this.label_Wordlist.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label_Wordlist.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.label_Wordlist.Location = new System.Drawing.Point(16, 48);
            this.label_Wordlist.Name = "label_Wordlist";
            this.label_Wordlist.Size = new System.Drawing.Size(86, 20);
            this.label_Wordlist.TabIndex = 4;
            this.label_Wordlist.Text = "WORDLIST";
            // 
            // textBox_WordlistPath
            // 
            this.textBox_WordlistPath.BackColor = System.Drawing.Color.White;
            this.textBox_WordlistPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_WordlistPath.Font = new System.Drawing.Font("Consolas", 9F);
            this.textBox_WordlistPath.Location = new System.Drawing.Point(110, 44);
            this.textBox_WordlistPath.Name = "textBox_WordlistPath";
            this.textBox_WordlistPath.ReadOnly = true;
            this.textBox_WordlistPath.Size = new System.Drawing.Size(400, 25);
            this.textBox_WordlistPath.TabIndex = 5;
            // 
            // button_BrowseWordlist
            // 
            this.button_BrowseWordlist.BackColor = System.Drawing.Color.White;
            this.button_BrowseWordlist.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_BrowseWordlist.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.button_BrowseWordlist.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_BrowseWordlist.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.button_BrowseWordlist.Location = new System.Drawing.Point(520, 42);
            this.button_BrowseWordlist.Name = "button_BrowseWordlist";
            this.button_BrowseWordlist.Size = new System.Drawing.Size(80, 26);
            this.button_BrowseWordlist.TabIndex = 6;
            this.button_BrowseWordlist.Text = "Browse…";
            this.button_BrowseWordlist.UseVisualStyleBackColor = false;
            this.button_BrowseWordlist.Click += new System.EventHandler(this.button_BrowseWordlist_Click);
            // 
            // button_BruteForce
            // 
            this.button_BruteForce.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.button_BruteForce.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_BruteForce.FlatAppearance.BorderSize = 0;
            this.button_BruteForce.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_BruteForce.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button_BruteForce.ForeColor = System.Drawing.Color.White;
            this.button_BruteForce.Location = new System.Drawing.Point(750, 34);
            this.button_BruteForce.Name = "button_BruteForce";
            this.button_BruteForce.Size = new System.Drawing.Size(130, 36);
            this.button_BruteForce.TabIndex = 7;
            this.button_BruteForce.Text = "▶  Start Attack";
            this.button_BruteForce.UseVisualStyleBackColor = false;
            this.button_BruteForce.Click += new System.EventHandler(this.button_BruteForce_Click);
            // 
            // label_BruteInfo
            // 
            this.label_BruteInfo.AutoSize = true;
            this.label_BruteInfo.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.label_BruteInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.label_BruteInfo.Location = new System.Drawing.Point(16, 76);
            this.label_BruteInfo.Name = "label_BruteInfo";
            this.label_BruteInfo.Size = new System.Drawing.Size(600, 19);
            this.label_BruteInfo.TabIndex = 8;
            this.label_BruteInfo.Text = "Leave PASSWORD blank when using a wordlist. Framework and field names are auto-de" +
    "tected.";
            // 
            // panel_Output
            // 
            this.panel_Output.BackColor = System.Drawing.Color.White;
            this.panel_Output.Controls.Add(this.listBox_Output);
            this.panel_Output.Controls.Add(this.label_Output);
            this.panel_Output.Controls.Add(this.progressBar_Scan);
            this.panel_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Output.Location = new System.Drawing.Point(0, 216);
            this.panel_Output.Name = "panel_Output";
            this.panel_Output.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.panel_Output.Size = new System.Drawing.Size(900, 464);
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
            this.listBox_Output.Size = new System.Drawing.Size(876, 424);
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
            this.progressBar_Scan.Location = new System.Drawing.Point(12, 452);
            this.progressBar_Scan.MarqueeAnimationSpeed = 30;
            this.progressBar_Scan.Name = "progressBar_Scan";
            this.progressBar_Scan.Size = new System.Drawing.Size(876, 4);
            this.progressBar_Scan.TabIndex = 2;
            // 
            // BruteForceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(900, 680);
            this.Controls.Add(this.panel_Output);
            this.Controls.Add(this.panel_Config);
            this.Controls.Add(this.panel_UrlBar);
            this.Controls.Add(this.panel_TopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(800, 580);
            this.Name = "BruteForceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThreatScanner — Brute Force";
            this.panel_TopBar.ResumeLayout(false);
            this.panel_TopBar.PerformLayout();
            this.panel_UrlBar.ResumeLayout(false);
            this.panel_UrlBar.PerformLayout();
            this.panel_Config.ResumeLayout(false);
            this.panel_Config.PerformLayout();
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
        private System.Windows.Forms.Panel panel_Config;
        private System.Windows.Forms.Label label_Username;
        private System.Windows.Forms.TextBox textBox_Username;
        private System.Windows.Forms.Label label_Password;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.Label label_Wordlist;
        private System.Windows.Forms.TextBox textBox_WordlistPath;
        private System.Windows.Forms.Button button_BrowseWordlist;
        private System.Windows.Forms.Button button_BruteForce;
        private System.Windows.Forms.Label label_BruteInfo;
        private System.Windows.Forms.Panel panel_Output;
        private System.Windows.Forms.Label label_Output;
        private System.Windows.Forms.ListBox listBox_Output;
        private System.Windows.Forms.ProgressBar progressBar_Scan;
    }
}
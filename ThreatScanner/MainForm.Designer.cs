namespace ThreatScanner
{
    partial class MainForm
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
            this.label_Title = new System.Windows.Forms.Label();
            this.label_Subtitle = new System.Windows.Forms.Label();
            this.panel_Cards = new System.Windows.Forms.Panel();
            this.button_OpenScanner = new System.Windows.Forms.Button();
            this.label_ScannerDesc = new System.Windows.Forms.Label();
            this.button_OpenBruteForce = new System.Windows.Forms.Button();
            this.label_BruteDesc = new System.Windows.Forms.Label();
            this.button_OpenApiTester = new System.Windows.Forms.Button();
            this.label_ApiDesc = new System.Windows.Forms.Label();
            this.button_OpenWebSocket = new System.Windows.Forms.Button();
            this.label_WsDesc = new System.Windows.Forms.Label();
            this.button_OpenCsrf = new System.Windows.Forms.Button();
            this.label_CsrfDesc = new System.Windows.Forms.Label();
            this.label_Disclaimer = new System.Windows.Forms.Label();
            this.panel_TopBar.SuspendLayout();
            this.panel_Cards.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_TopBar
            // 
            this.panel_TopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.panel_TopBar.Controls.Add(this.label_Title);
            this.panel_TopBar.Controls.Add(this.label_Subtitle);
            this.panel_TopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_TopBar.Location = new System.Drawing.Point(0, 0);
            this.panel_TopBar.Name = "panel_TopBar";
            this.panel_TopBar.Size = new System.Drawing.Size(1840, 80);
            this.panel_TopBar.TabIndex = 1;
            // 
            // label_Title
            // 
            this.label_Title.AutoSize = true;
            this.label_Title.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.label_Title.ForeColor = System.Drawing.Color.White;
            this.label_Title.Location = new System.Drawing.Point(20, 10);
            this.label_Title.Name = "label_Title";
            this.label_Title.Size = new System.Drawing.Size(247, 37);
            this.label_Title.TabIndex = 0;
            this.label_Title.Text = "⚡ ThreatScanner";
            // 
            // label_Subtitle
            // 
            this.label_Subtitle.AutoSize = true;
            this.label_Subtitle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_Subtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.label_Subtitle.Location = new System.Drawing.Point(22, 48);
            this.label_Subtitle.Name = "label_Subtitle";
            this.label_Subtitle.Size = new System.Drawing.Size(425, 20);
            this.label_Subtitle.TabIndex = 1;
            this.label_Subtitle.Text = "Web Vulnerability Testing Toolkit  •  For authorized testing only";
            // 
            // panel_Cards
            // 
            this.panel_Cards.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panel_Cards.Controls.Add(this.button_OpenScanner);
            this.panel_Cards.Controls.Add(this.label_ScannerDesc);
            this.panel_Cards.Controls.Add(this.button_OpenBruteForce);
            this.panel_Cards.Controls.Add(this.label_BruteDesc);
            this.panel_Cards.Controls.Add(this.button_OpenApiTester);
            this.panel_Cards.Controls.Add(this.label_ApiDesc);
            this.panel_Cards.Controls.Add(this.button_OpenWebSocket);
            this.panel_Cards.Controls.Add(this.label_WsDesc);
            this.panel_Cards.Controls.Add(this.button_OpenCsrf);
            this.panel_Cards.Controls.Add(this.label_CsrfDesc);
            this.panel_Cards.Controls.Add(this.label_Disclaimer);
            this.panel_Cards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Cards.Location = new System.Drawing.Point(0, 80);
            this.panel_Cards.Name = "panel_Cards";
            this.panel_Cards.Size = new System.Drawing.Size(1840, 263);
            this.panel_Cards.TabIndex = 0;
            // 
            // button_OpenScanner
            // 
            this.button_OpenScanner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.button_OpenScanner.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_OpenScanner.FlatAppearance.BorderSize = 0;
            this.button_OpenScanner.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_OpenScanner.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.button_OpenScanner.ForeColor = System.Drawing.Color.White;
            this.button_OpenScanner.Location = new System.Drawing.Point(30, 60);
            this.button_OpenScanner.Name = "button_OpenScanner";
            this.button_OpenScanner.Size = new System.Drawing.Size(330, 100);
            this.button_OpenScanner.TabIndex = 0;
            this.button_OpenScanner.Text = "🔍  Full Scanner";
            this.button_OpenScanner.UseVisualStyleBackColor = false;
            this.button_OpenScanner.Click += new System.EventHandler(this.button_OpenScanner_Click);
            // 
            // label_ScannerDesc
            // 
            this.label_ScannerDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_ScannerDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label_ScannerDesc.Location = new System.Drawing.Point(30, 170);
            this.label_ScannerDesc.Name = "label_ScannerDesc";
            this.label_ScannerDesc.Size = new System.Drawing.Size(330, 50);
            this.label_ScannerDesc.TabIndex = 1;
            this.label_ScannerDesc.Text = "HTTPS, security headers, cookies,\nSQL injection hints, XSS, ports, DNS";
            this.label_ScannerDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_OpenBruteForce
            // 
            this.button_OpenBruteForce.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.button_OpenBruteForce.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_OpenBruteForce.FlatAppearance.BorderSize = 0;
            this.button_OpenBruteForce.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_OpenBruteForce.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.button_OpenBruteForce.ForeColor = System.Drawing.Color.White;
            this.button_OpenBruteForce.Location = new System.Drawing.Point(390, 60);
            this.button_OpenBruteForce.Name = "button_OpenBruteForce";
            this.button_OpenBruteForce.Size = new System.Drawing.Size(330, 100);
            this.button_OpenBruteForce.TabIndex = 2;
            this.button_OpenBruteForce.Text = "🔐  Brute Force";
            this.button_OpenBruteForce.UseVisualStyleBackColor = false;
            this.button_OpenBruteForce.Click += new System.EventHandler(this.button_OpenBruteForce_Click);
            // 
            // label_BruteDesc
            // 
            this.label_BruteDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_BruteDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label_BruteDesc.Location = new System.Drawing.Point(390, 170);
            this.label_BruteDesc.Name = "label_BruteDesc";
            this.label_BruteDesc.Size = new System.Drawing.Size(330, 50);
            this.label_BruteDesc.TabIndex = 3;
            this.label_BruteDesc.Text = "Auto-detect ASP.NET / PHP / HTML forms\nand attack with a wordlist";
            this.label_BruteDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_OpenApiTester
            // 
            this.button_OpenApiTester.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.button_OpenApiTester.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_OpenApiTester.FlatAppearance.BorderSize = 0;
            this.button_OpenApiTester.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_OpenApiTester.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.button_OpenApiTester.ForeColor = System.Drawing.Color.White;
            this.button_OpenApiTester.Location = new System.Drawing.Point(750, 60);
            this.button_OpenApiTester.Name = "button_OpenApiTester";
            this.button_OpenApiTester.Size = new System.Drawing.Size(330, 100);
            this.button_OpenApiTester.TabIndex = 4;
            this.button_OpenApiTester.Text = "🛰  API Tester";
            this.button_OpenApiTester.UseVisualStyleBackColor = false;
            this.button_OpenApiTester.Click += new System.EventHandler(this.button_OpenApiTester_Click);
            // 
            // label_ApiDesc
            // 
            this.label_ApiDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_ApiDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label_ApiDesc.Location = new System.Drawing.Point(750, 170);
            this.label_ApiDesc.Name = "label_ApiDesc";
            this.label_ApiDesc.Size = new System.Drawing.Size(330, 50);
            this.label_ApiDesc.TabIndex = 5;
            this.label_ApiDesc.Text = "Postman-style HTTP tester with auth,\r\nheaders, params, body";
            this.label_ApiDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_OpenWebSocket
            // 
            this.button_OpenWebSocket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(88)))), ((int)(((byte)(12)))));
            this.button_OpenWebSocket.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_OpenWebSocket.FlatAppearance.BorderSize = 0;
            this.button_OpenWebSocket.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_OpenWebSocket.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.button_OpenWebSocket.ForeColor = System.Drawing.Color.White;
            this.button_OpenWebSocket.Location = new System.Drawing.Point(1110, 60);
            this.button_OpenWebSocket.Name = "button_OpenWebSocket";
            this.button_OpenWebSocket.Size = new System.Drawing.Size(330, 100);
            this.button_OpenWebSocket.TabIndex = 6;
            this.button_OpenWebSocket.Text = "🟠  WebSocket Tester";
            this.button_OpenWebSocket.UseVisualStyleBackColor = false;
            this.button_OpenWebSocket.Click += new System.EventHandler(this.button_OpenWebSocket_Click);
            // 
            // label_WsDesc
            // 
            this.label_WsDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_WsDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label_WsDesc.Location = new System.Drawing.Point(1110, 170);
            this.label_WsDesc.Name = "label_WsDesc";
            this.label_WsDesc.Size = new System.Drawing.Size(330, 50);
            this.label_WsDesc.TabIndex = 7;
            this.label_WsDesc.Text = "Connect, send frames, receive live data,\nfuzz payloads over ws:// and wss://";
            this.label_WsDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_OpenCsrf
            // 
            this.button_OpenCsrf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(40)))), ((int)(((byte)(217)))));
            this.button_OpenCsrf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_OpenCsrf.FlatAppearance.BorderSize = 0;
            this.button_OpenCsrf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_OpenCsrf.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.button_OpenCsrf.ForeColor = System.Drawing.Color.White;
            this.button_OpenCsrf.Location = new System.Drawing.Point(1470, 60);
            this.button_OpenCsrf.Name = "button_OpenCsrf";
            this.button_OpenCsrf.Size = new System.Drawing.Size(330, 100);
            this.button_OpenCsrf.TabIndex = 8;
            this.button_OpenCsrf.Text = "🛡  CSRF Tester";
            this.button_OpenCsrf.UseVisualStyleBackColor = false;
            this.button_OpenCsrf.Click += new System.EventHandler(this.button_OpenCsrf_Click);
            // 
            // label_CsrfDesc
            // 
            this.label_CsrfDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label_CsrfDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.label_CsrfDesc.Location = new System.Drawing.Point(1470, 170);
            this.label_CsrfDesc.Name = "label_CsrfDesc";
            this.label_CsrfDesc.Size = new System.Drawing.Size(330, 50);
            this.label_CsrfDesc.TabIndex = 9;
            this.label_CsrfDesc.Text = "Detect missing tokens, SameSite cookies,\nCORS misconfig & forge cross-site reques" +
    "ts";
            this.label_CsrfDesc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_Disclaimer
            // 
            this.label_Disclaimer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label_Disclaimer.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic);
            this.label_Disclaimer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.label_Disclaimer.Location = new System.Drawing.Point(0, 235);
            this.label_Disclaimer.Name = "label_Disclaimer";
            this.label_Disclaimer.Size = new System.Drawing.Size(1840, 28);
            this.label_Disclaimer.TabIndex = 10;
            this.label_Disclaimer.Text = "⚠️  For authorized penetration testing only. Unauthorized use is illegal.";
            this.label_Disclaimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1840, 343);
            this.Controls.Add(this.panel_Cards);
            this.Controls.Add(this.panel_TopBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1840, 380);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThreatScanner — Select Tool";
            this.panel_TopBar.ResumeLayout(false);
            this.panel_TopBar.PerformLayout();
            this.panel_Cards.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_TopBar;
        private System.Windows.Forms.Label label_Title;
        private System.Windows.Forms.Label label_Subtitle;
        private System.Windows.Forms.Panel panel_Cards;
        private System.Windows.Forms.Button button_OpenScanner;
        private System.Windows.Forms.Button button_OpenBruteForce;
        private System.Windows.Forms.Button button_OpenApiTester;
        private System.Windows.Forms.Button button_OpenWebSocket;
        private System.Windows.Forms.Button button_OpenCsrf;
        private System.Windows.Forms.Label label_ScannerDesc;
        private System.Windows.Forms.Label label_BruteDesc;
        private System.Windows.Forms.Label label_ApiDesc;
        private System.Windows.Forms.Label label_WsDesc;
        private System.Windows.Forms.Label label_CsrfDesc;
        private System.Windows.Forms.Label label_Disclaimer;
    }
}
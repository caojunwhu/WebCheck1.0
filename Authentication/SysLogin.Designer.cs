namespace Authentication
{
    partial class SysLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_quit = new System.Windows.Forms.Button();
            this.btn_login = new System.Windows.Forms.Button();
            this.tb_password = new System.Windows.Forms.TextBox();
            this.tb_username = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabelAuthorizeApply = new System.Windows.Forms.LinkLabel();
            this.labelAuthorizedInfo = new System.Windows.Forms.Label();
            this.linkLabelHelp = new System.Windows.Forms.LinkLabel();
            this.linkLabelForgetPassword = new System.Windows.Forms.LinkLabel();
            this.linkLabelAbout = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btn_quit
            // 
            this.btn_quit.Location = new System.Drawing.Point(556, 399);
            this.btn_quit.Name = "btn_quit";
            this.btn_quit.Size = new System.Drawing.Size(45, 23);
            this.btn_quit.TabIndex = 7;
            this.btn_quit.Text = "退出";
            this.btn_quit.UseVisualStyleBackColor = true;
            this.btn_quit.Click += new System.EventHandler(this.btn_quit_Click);
            // 
            // btn_login
            // 
            this.btn_login.Location = new System.Drawing.Point(556, 369);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(45, 23);
            this.btn_login.TabIndex = 6;
            this.btn_login.Text = "进入";
            this.btn_login.UseVisualStyleBackColor = true;
            this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
            // 
            // tb_password
            // 
            this.tb_password.Location = new System.Drawing.Point(449, 401);
            this.tb_password.Name = "tb_password";
            this.tb_password.Size = new System.Drawing.Size(100, 21);
            this.tb_password.TabIndex = 5;
            this.tb_password.Text = "250922107";
            this.tb_password.UseSystemPasswordChar = true;
            this.tb_password.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_password_KeyPress);
            // 
            // tb_username
            // 
            this.tb_username.Location = new System.Drawing.Point(449, 370);
            this.tb_username.Name = "tb_username";
            this.tb_username.Size = new System.Drawing.Size(100, 21);
            this.tb_username.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft YaHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.DarkGreen;
            this.label5.Location = new System.Drawing.Point(12, 360);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(321, 80);
            this.label5.TabIndex = 13;
            this.label5.Text = "Version 1.1.8.4\r\n湖北省测绘质量监督检验站\r\n2015年7月";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(390, 404);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "密  码：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(390, 373);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "用户名：";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("STLiti", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(176, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(513, 80);
            this.label3.TabIndex = 13;
            this.label3.Text = "           湖北省地理国情普查——\r\n       成果检查与验收信息管理系统";
            // 
            // linkLabelAuthorizeApply
            // 
            this.linkLabelAuthorizeApply.AutoSize = true;
            this.linkLabelAuthorizeApply.BackColor = System.Drawing.Color.Transparent;
            this.linkLabelAuthorizeApply.Location = new System.Drawing.Point(636, 374);
            this.linkLabelAuthorizeApply.Name = "linkLabelAuthorizeApply";
            this.linkLabelAuthorizeApply.Size = new System.Drawing.Size(53, 12);
            this.linkLabelAuthorizeApply.TabIndex = 14;
            this.linkLabelAuthorizeApply.TabStop = true;
            this.linkLabelAuthorizeApply.Text = "申请授权";
            this.linkLabelAuthorizeApply.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAuthorizeApply_LinkClicked);
            // 
            // labelAuthorizedInfo
            // 
            this.labelAuthorizedInfo.AutoSize = true;
            this.labelAuthorizedInfo.BackColor = System.Drawing.Color.Transparent;
            this.labelAuthorizedInfo.Location = new System.Drawing.Point(284, 428);
            this.labelAuthorizedInfo.Name = "labelAuthorizedInfo";
            this.labelAuthorizedInfo.Size = new System.Drawing.Size(143, 12);
            this.labelAuthorizedInfo.TabIndex = 15;
            this.labelAuthorizedInfo.Text = "本机MAC地址，未授权版本";
            // 
            // linkLabelHelp
            // 
            this.linkLabelHelp.AutoSize = true;
            this.linkLabelHelp.BackColor = System.Drawing.Color.Transparent;
            this.linkLabelHelp.Location = new System.Drawing.Point(636, 428);
            this.linkLabelHelp.Name = "linkLabelHelp";
            this.linkLabelHelp.Size = new System.Drawing.Size(29, 12);
            this.linkLabelHelp.TabIndex = 16;
            this.linkLabelHelp.TabStop = true;
            this.linkLabelHelp.Text = "帮助";
            this.linkLabelHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelHelp_LinkClicked);
            // 
            // linkLabelForgetPassword
            // 
            this.linkLabelForgetPassword.AutoSize = true;
            this.linkLabelForgetPassword.BackColor = System.Drawing.Color.Transparent;
            this.linkLabelForgetPassword.Location = new System.Drawing.Point(636, 404);
            this.linkLabelForgetPassword.Name = "linkLabelForgetPassword";
            this.linkLabelForgetPassword.Size = new System.Drawing.Size(53, 12);
            this.linkLabelForgetPassword.TabIndex = 17;
            this.linkLabelForgetPassword.TabStop = true;
            this.linkLabelForgetPassword.Text = "忘记密码";
            this.linkLabelForgetPassword.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelForgetPassword_LinkClicked);
            // 
            // linkLabelAbout
            // 
            this.linkLabelAbout.AutoSize = true;
            this.linkLabelAbout.BackColor = System.Drawing.Color.Transparent;
            this.linkLabelAbout.Location = new System.Drawing.Point(671, 428);
            this.linkLabelAbout.Name = "linkLabelAbout";
            this.linkLabelAbout.Size = new System.Drawing.Size(29, 12);
            this.linkLabelAbout.TabIndex = 16;
            this.linkLabelAbout.TabStop = true;
            this.linkLabelAbout.Text = "关于";
            this.linkLabelAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAbout_LinkClicked);
            // 
            // SysLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 449);
            this.Controls.Add(this.linkLabelForgetPassword);
            this.Controls.Add(this.linkLabelAbout);
            this.Controls.Add(this.linkLabelHelp);
            this.Controls.Add(this.labelAuthorizedInfo);
            this.Controls.Add(this.linkLabelAuthorizeApply);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_quit);
            this.Controls.Add(this.btn_login);
            this.Controls.Add(this.tb_password);
            this.Controls.Add(this.tb_username);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SysLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SysLogin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SysLogin_FormClosing);
            this.Load += new System.EventHandler(this.SysLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_quit;
        private System.Windows.Forms.Button btn_login;
        private System.Windows.Forms.TextBox tb_password;
        private System.Windows.Forms.TextBox tb_username;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabelAuthorizeApply;
        private System.Windows.Forms.Label labelAuthorizedInfo;
        private System.Windows.Forms.LinkLabel linkLabelHelp;
        private System.Windows.Forms.LinkLabel linkLabelForgetPassword;
        private System.Windows.Forms.LinkLabel linkLabelAbout;
    }
}
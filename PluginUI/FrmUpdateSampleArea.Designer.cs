namespace PluginUI
{
    partial class FrmUpdateSampleArea
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUpdateSampleArea));
            this.btn_BroswerScaterFile = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_filepath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1_updatesamplearea = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_BroswerScaterFile
            // 
            this.btn_BroswerScaterFile.Location = new System.Drawing.Point(498, 26);
            this.btn_BroswerScaterFile.Name = "btn_BroswerScaterFile";
            this.btn_BroswerScaterFile.Size = new System.Drawing.Size(75, 23);
            this.btn_BroswerScaterFile.TabIndex = 11;
            this.btn_BroswerScaterFile.Text = "浏览";
            this.btn_BroswerScaterFile.UseVisualStyleBackColor = true;
            this.btn_BroswerScaterFile.Click += new System.EventHandler(this.btn_BroswerScaterFile_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_filepath);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(563, 43);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "文件浏览";
            // 
            // textBox_filepath
            // 
            this.textBox_filepath.Location = new System.Drawing.Point(79, 16);
            this.textBox_filepath.Name = "textBox_filepath";
            this.textBox_filepath.Size = new System.Drawing.Size(395, 21);
            this.textBox_filepath.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "文件路径：";
            // 
            // button1_updatesamplearea
            // 
            this.button1_updatesamplearea.Location = new System.Drawing.Point(457, 334);
            this.button1_updatesamplearea.Name = "button1_updatesamplearea";
            this.button1_updatesamplearea.Size = new System.Drawing.Size(116, 23);
            this.button1_updatesamplearea.TabIndex = 13;
            this.button1_updatesamplearea.Text = "导入更新抽样分区";
            this.button1_updatesamplearea.UseVisualStyleBackColor = true;
            this.button1_updatesamplearea.Click += new System.EventHandler(this.button1_updatesamplearea_Click);
            // 
            // FrmUpdateSampleArea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 369);
            this.Controls.Add(this.button1_updatesamplearea);
            this.Controls.Add(this.btn_BroswerScaterFile);
            this.Controls.Add(this.groupBox2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmUpdateSampleArea";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "导入项目抽样分区";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_BroswerScaterFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_filepath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1_updatesamplearea;
    }
}
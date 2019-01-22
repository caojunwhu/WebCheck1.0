namespace PluginUI
{
    partial class FrmUpdateMapbindingTable
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUpdateMapbindingTable));
            this.button1_updatemapbingding = new System.Windows.Forms.Button();
            this.btn_BroswerScaterFile = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_filepath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1_Mapnumber = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.superGridControl1 = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1_updatemapbingding
            // 
            this.button1_updatemapbingding.Location = new System.Drawing.Point(460, 324);
            this.button1_updatemapbingding.Name = "button1_updatemapbingding";
            this.button1_updatemapbingding.Size = new System.Drawing.Size(116, 23);
            this.button1_updatemapbingding.TabIndex = 16;
            this.button1_updatemapbingding.Text = "导入更新结合表";
            this.button1_updatemapbingding.UseVisualStyleBackColor = true;
            this.button1_updatemapbingding.Click += new System.EventHandler(this.button1_updatemapbingding_Click);
            // 
            // btn_BroswerScaterFile
            // 
            this.btn_BroswerScaterFile.Location = new System.Drawing.Point(501, 16);
            this.btn_BroswerScaterFile.Name = "btn_BroswerScaterFile";
            this.btn_BroswerScaterFile.Size = new System.Drawing.Size(75, 23);
            this.btn_BroswerScaterFile.TabIndex = 14;
            this.btn_BroswerScaterFile.Text = "浏览";
            this.btn_BroswerScaterFile.UseVisualStyleBackColor = true;
            this.btn_BroswerScaterFile.Click += new System.EventHandler(this.btn_BroswerMapbindingTableFile_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_filepath);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(15, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(563, 43);
            this.groupBox2.TabIndex = 15;
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1_Mapnumber);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(20, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(556, 68);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "图幅号映射";
            // 
            // comboBox1_Mapnumber
            // 
            this.comboBox1_Mapnumber.FormattingEnabled = true;
            this.comboBox1_Mapnumber.Location = new System.Drawing.Point(113, 22);
            this.comboBox1_Mapnumber.Name = "comboBox1_Mapnumber";
            this.comboBox1_Mapnumber.Size = new System.Drawing.Size(141, 20);
            this.comboBox1_Mapnumber.TabIndex = 1;
            this.comboBox1_Mapnumber.SelectedIndexChanged += new System.EventHandler(this.comboBox1_Mapnumber_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "图符号字段：";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.superGridControl1);
            this.groupBox3.Location = new System.Drawing.Point(20, 133);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(556, 185);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "预览";
            // 
            // superGridControl1
            // 
            this.superGridControl1.BackColor = System.Drawing.Color.White;
            this.superGridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superGridControl1.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.superGridControl1.ForeColor = System.Drawing.Color.Black;
            this.superGridControl1.Location = new System.Drawing.Point(3, 17);
            this.superGridControl1.Name = "superGridControl1";
            this.superGridControl1.Size = new System.Drawing.Size(550, 165);
            this.superGridControl1.TabIndex = 0;
            this.superGridControl1.Text = "superGridControl1";
            // 
            // FrmUpdateMapbindingTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 349);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1_updatemapbingding);
            this.Controls.Add(this.btn_BroswerScaterFile);
            this.Controls.Add(this.groupBox2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmUpdateMapbindingTable";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "导入项目样本结合表";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1_updatemapbingding;
        private System.Windows.Forms.Button btn_BroswerScaterFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_filepath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBox1_Mapnumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private DevComponents.DotNetBar.SuperGrid.SuperGridControl superGridControl1;
    }
}
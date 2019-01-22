namespace PluginUI.Frms
{
    partial class FrmUpdateScater
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUpdateScater));
            this.btn_BroswerScaterFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_filepath = new System.Windows.Forms.TextBox();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.实测点预览 = new System.Windows.Forms.GroupBox();
            this.btn_import = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.comboBox1_ptid = new System.Windows.Forms.ComboBox();
            this.label_ptid = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1_splitchar = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox5_sz = new System.Windows.Forms.ComboBox();
            this.comboBox4_sy = new System.Windows.Forms.ComboBox();
            this.comboBox2_sx = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.实测点预览.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_BroswerScaterFile
            // 
            this.btn_BroswerScaterFile.Location = new System.Drawing.Point(498, 19);
            this.btn_BroswerScaterFile.Name = "btn_BroswerScaterFile";
            this.btn_BroswerScaterFile.Size = new System.Drawing.Size(75, 23);
            this.btn_BroswerScaterFile.TabIndex = 0;
            this.btn_BroswerScaterFile.Text = "浏览";
            this.btn_BroswerScaterFile.UseVisualStyleBackColor = true;
            this.btn_BroswerScaterFile.Click += new System.EventHandler(this.btn_BroswerScaterFile_Click);
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
            // textBox_filepath
            // 
            this.textBox_filepath.Location = new System.Drawing.Point(79, 16);
            this.textBox_filepath.Name = "textBox_filepath";
            this.textBox_filepath.Size = new System.Drawing.Size(395, 21);
            this.textBox_filepath.TabIndex = 2;
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewX1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.RowTemplate.Height = 23;
            this.dataGridViewX1.Size = new System.Drawing.Size(562, 195);
            this.dataGridViewX1.TabIndex = 3;
            // 
            // 实测点预览
            // 
            this.实测点预览.Controls.Add(this.dataGridViewX1);
            this.实测点预览.Location = new System.Drawing.Point(12, 164);
            this.实测点预览.Name = "实测点预览";
            this.实测点预览.Size = new System.Drawing.Size(568, 215);
            this.实测点预览.TabIndex = 4;
            this.实测点预览.TabStop = false;
            this.实测点预览.Text = "实测点预览";
            // 
            // btn_import
            // 
            this.btn_import.Location = new System.Drawing.Point(502, 385);
            this.btn_import.Name = "btn_import";
            this.btn_import.Size = new System.Drawing.Size(75, 23);
            this.btn_import.TabIndex = 5;
            this.btn_import.Text = "导入项目";
            this.btn_import.UseVisualStyleBackColor = true;
            this.btn_import.Click += new System.EventHandler(this.btn_import_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(16, 19);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(84, 16);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "首行为表头";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // comboBox1_ptid
            // 
            this.comboBox1_ptid.FormattingEnabled = true;
            this.comboBox1_ptid.Location = new System.Drawing.Point(60, 45);
            this.comboBox1_ptid.Name = "comboBox1_ptid";
            this.comboBox1_ptid.Size = new System.Drawing.Size(121, 20);
            this.comboBox1_ptid.TabIndex = 7;
            this.comboBox1_ptid.SelectedIndexChanged += new System.EventHandler(this.comboBox1_ptid_SelectedIndexChanged);
            // 
            // label_ptid
            // 
            this.label_ptid.AutoSize = true;
            this.label_ptid.Location = new System.Drawing.Point(13, 48);
            this.label_ptid.Name = "label_ptid";
            this.label_ptid.Size = new System.Drawing.Size(41, 12);
            this.label_ptid.TabIndex = 8;
            this.label_ptid.Text = "ptid：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1_splitchar);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label_ptid);
            this.groupBox1.Controls.Add(this.comboBox5_sz);
            this.groupBox1.Controls.Add(this.comboBox4_sy);
            this.groupBox1.Controls.Add(this.comboBox2_sx);
            this.groupBox1.Controls.Add(this.comboBox1_ptid);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(564, 104);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "关系映射";
            // 
            // comboBox1_splitchar
            // 
            this.comboBox1_splitchar.FormattingEnabled = true;
            this.comboBox1_splitchar.Items.AddRange(new object[] {
            ",",
            ";",
            "\t"});
            this.comboBox1_splitchar.Location = new System.Drawing.Point(235, 19);
            this.comboBox1_splitchar.Name = "comboBox1_splitchar";
            this.comboBox1_splitchar.Size = new System.Drawing.Size(121, 20);
            this.comboBox1_splitchar.TabIndex = 10;
            this.comboBox1_splitchar.Text = ",";
            this.comboBox1_splitchar.SelectedIndexChanged += new System.EventHandler(this.comboBox1_splitchar_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(176, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "分割符：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(188, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "sz：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "sy：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(188, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "sx：";
            // 
            // comboBox5_sz
            // 
            this.comboBox5_sz.FormattingEnabled = true;
            this.comboBox5_sz.Location = new System.Drawing.Point(235, 78);
            this.comboBox5_sz.Name = "comboBox5_sz";
            this.comboBox5_sz.Size = new System.Drawing.Size(121, 20);
            this.comboBox5_sz.TabIndex = 7;
            this.comboBox5_sz.SelectedIndexChanged += new System.EventHandler(this.comboBox5_sz_SelectedIndexChanged);
            // 
            // comboBox4_sy
            // 
            this.comboBox4_sy.FormattingEnabled = true;
            this.comboBox4_sy.Location = new System.Drawing.Point(60, 78);
            this.comboBox4_sy.Name = "comboBox4_sy";
            this.comboBox4_sy.Size = new System.Drawing.Size(121, 20);
            this.comboBox4_sy.TabIndex = 7;
            this.comboBox4_sy.SelectedIndexChanged += new System.EventHandler(this.comboBox4_sy_SelectedIndexChanged);
            // 
            // comboBox2_sx
            // 
            this.comboBox2_sx.FormattingEnabled = true;
            this.comboBox2_sx.Location = new System.Drawing.Point(235, 45);
            this.comboBox2_sx.Name = "comboBox2_sx";
            this.comboBox2_sx.Size = new System.Drawing.Size(121, 20);
            this.comboBox2_sx.TabIndex = 7;
            this.comboBox2_sx.SelectedIndexChanged += new System.EventHandler(this.comboBox2_sx_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_filepath);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(563, 43);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "文件浏览";
            // 
            // FrmUpdateScater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 420);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_import);
            this.Controls.Add(this.实测点预览);
            this.Controls.Add(this.btn_BroswerScaterFile);
            this.Controls.Add(this.groupBox2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmUpdateScater";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "导入样本实测散点";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.实测点预览.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_BroswerScaterFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_filepath;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.GroupBox 实测点预览;
        private System.Windows.Forms.Button btn_import;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox comboBox1_ptid;
        private System.Windows.Forms.Label label_ptid;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox5_sz;
        private System.Windows.Forms.ComboBox comboBox4_sy;
        private System.Windows.Forms.ComboBox comboBox2_sx;
        private System.Windows.Forms.ComboBox comboBox1_splitchar;
        private System.Windows.Forms.Label label3;
    }
}
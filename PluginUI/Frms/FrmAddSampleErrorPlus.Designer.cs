namespace PluginUI.Frms
{
    partial class FrmAddSampleErrorPlus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAddSampleErrorPlus));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmb_checkitem = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btn_lookpreerror = new System.Windows.Forms.Button();
            this.cmb_preerror = new System.Windows.Forms.ComboBox();
            this.cmb_errorclass = new System.Windows.Forms.ComboBox();
            this.cmb_subqualityitem = new System.Windows.Forms.ComboBox();
            this.cmb_qualityitem = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_mapnumber = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.rtb_errorofsample = new System.Windows.Forms.RichTextBox();
            this.btn_OK = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.fucharichTextBox1 = new System.Windows.Forms.RichTextBox();
            this.xiugairichTextBox3 = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chulirichTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "样本号：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmb_checkitem);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.btn_lookpreerror);
            this.groupBox1.Controls.Add(this.cmb_preerror);
            this.groupBox1.Controls.Add(this.cmb_errorclass);
            this.groupBox1.Controls.Add(this.cmb_subqualityitem);
            this.groupBox1.Controls.Add(this.cmb_qualityitem);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tb_mapnumber);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(736, 105);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "错漏归类";
            // 
            // cmb_checkitem
            // 
            this.cmb_checkitem.FormattingEnabled = true;
            this.cmb_checkitem.Location = new System.Drawing.Point(365, 20);
            this.cmb_checkitem.Name = "cmb_checkitem";
            this.cmb_checkitem.Size = new System.Drawing.Size(277, 20);
            this.cmb_checkitem.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(306, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 7;
            this.label7.Text = "检查项：";
            // 
            // btn_lookpreerror
            // 
            this.btn_lookpreerror.Location = new System.Drawing.Point(649, 76);
            this.btn_lookpreerror.Name = "btn_lookpreerror";
            this.btn_lookpreerror.Size = new System.Drawing.Size(81, 22);
            this.btn_lookpreerror.TabIndex = 6;
            this.btn_lookpreerror.Text = "概览及多选";
            this.btn_lookpreerror.UseVisualStyleBackColor = true;
            this.btn_lookpreerror.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmb_preerror
            // 
            this.cmb_preerror.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmb_preerror.FormattingEnabled = true;
            this.cmb_preerror.ItemHeight = 14;
            this.cmb_preerror.Location = new System.Drawing.Point(365, 76);
            this.cmb_preerror.Name = "cmb_preerror";
            this.cmb_preerror.Size = new System.Drawing.Size(277, 22);
            this.cmb_preerror.TabIndex = 5;
            this.cmb_preerror.SelectedIndexChanged += new System.EventHandler(this.comboBox4_SelectedIndexChanged);
            // 
            // cmb_errorclass
            // 
            this.cmb_errorclass.FormattingEnabled = true;
            this.cmb_errorclass.Items.AddRange(new object[] {
            "A",
            "B",
            "C",
            "D"});
            this.cmb_errorclass.Location = new System.Drawing.Point(365, 48);
            this.cmb_errorclass.Name = "cmb_errorclass";
            this.cmb_errorclass.Size = new System.Drawing.Size(277, 20);
            this.cmb_errorclass.TabIndex = 4;
            this.cmb_errorclass.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // cmb_subqualityitem
            // 
            this.cmb_subqualityitem.FormattingEnabled = true;
            this.cmb_subqualityitem.Location = new System.Drawing.Point(97, 79);
            this.cmb_subqualityitem.Name = "cmb_subqualityitem";
            this.cmb_subqualityitem.Size = new System.Drawing.Size(132, 20);
            this.cmb_subqualityitem.TabIndex = 3;
            this.cmb_subqualityitem.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // cmb_qualityitem
            // 
            this.cmb_qualityitem.FormattingEnabled = true;
            this.cmb_qualityitem.Location = new System.Drawing.Point(97, 47);
            this.cmb_qualityitem.Name = "cmb_qualityitem";
            this.cmb_qualityitem.Size = new System.Drawing.Size(132, 20);
            this.cmb_qualityitem.TabIndex = 2;
            this.cmb_qualityitem.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(298, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "错漏类别：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(294, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "参考描述：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "质量子元素：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "质量元素：";
            // 
            // tb_mapnumber
            // 
            this.tb_mapnumber.Location = new System.Drawing.Point(97, 14);
            this.tb_mapnumber.Name = "tb_mapnumber";
            this.tb_mapnumber.ReadOnly = true;
            this.tb_mapnumber.Size = new System.Drawing.Size(132, 21);
            this.tb_mapnumber.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "错漏描述：";
            // 
            // rtb_errorofsample
            // 
            this.rtb_errorofsample.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtb_errorofsample.Location = new System.Drawing.Point(75, 20);
            this.rtb_errorofsample.Name = "rtb_errorofsample";
            this.rtb_errorofsample.Size = new System.Drawing.Size(205, 60);
            this.rtb_errorofsample.TabIndex = 6;
            this.rtb_errorofsample.Text = "";
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(314, 96);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(80, 23);
            this.btn_OK.TabIndex = 7;
            this.btn_OK.Text = "填好了";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(445, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "复查情况：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(590, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 2;
            this.label9.Text = "修改情况：";
            // 
            // fucharichTextBox1
            // 
            this.fucharichTextBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.fucharichTextBox1.Location = new System.Drawing.Point(511, 19);
            this.fucharichTextBox1.Name = "fucharichTextBox1";
            this.fucharichTextBox1.Size = new System.Drawing.Size(78, 60);
            this.fucharichTextBox1.TabIndex = 6;
            this.fucharichTextBox1.Text = "";
            // 
            // xiugairichTextBox3
            // 
            this.xiugairichTextBox3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.xiugairichTextBox3.Location = new System.Drawing.Point(656, 19);
            this.xiugairichTextBox3.Name = "xiugairichTextBox3";
            this.xiugairichTextBox3.Size = new System.Drawing.Size(75, 60);
            this.xiugairichTextBox3.TabIndex = 6;
            this.xiugairichTextBox3.Text = "";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chulirichTextBox1);
            this.groupBox3.Controls.Add(this.fucharichTextBox1);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.btn_OK);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.xiugairichTextBox3);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.rtb_errorofsample);
            this.groupBox3.Location = new System.Drawing.Point(12, 118);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(736, 125);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "错漏描述";
            // 
            // chulirichTextBox1
            // 
            this.chulirichTextBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chulirichTextBox1.Location = new System.Drawing.Point(365, 20);
            this.chulirichTextBox1.Name = "chulirichTextBox1";
            this.chulirichTextBox1.Size = new System.Drawing.Size(78, 60);
            this.chulirichTextBox1.TabIndex = 6;
            this.chulirichTextBox1.Text = "";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(298, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "处理意见：";
            // 
            // FrmAddSampleErrorPlus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 251);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAddSampleErrorPlus";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "添加样本错漏描述";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmAddSampleErrorPlus_FormClosed);
            this.Load += new System.EventHandler(this.FrmAddSampleError_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmb_subqualityitem;
        private System.Windows.Forms.ComboBox cmb_qualityitem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_mapnumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmb_errorclass;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox rtb_errorofsample;
        private System.Windows.Forms.ComboBox cmb_preerror;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RichTextBox fucharichTextBox1;
        private System.Windows.Forms.RichTextBox xiugairichTextBox3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_lookpreerror;
        private System.Windows.Forms.RichTextBox chulirichTextBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmb_checkitem;
        private System.Windows.Forms.Label label7;
    }
}
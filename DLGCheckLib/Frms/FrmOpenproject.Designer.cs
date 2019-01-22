namespace DLGCheckLib
{
    partial class FrmOpenproject
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonok = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.sharedlistView1 = new System.Windows.Forms.ListView();
            this.lastopentimetextBox1 = new System.Windows.Forms.TextBox();
            this.departmenttextBox2 = new System.Windows.Forms.TextBox();
            this.textBox_SampleFileType = new System.Windows.Forms.TextBox();
            this.ownertextBox1 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tb_SAMPLEAREA = new System.Windows.Forms.TextBox();
            this.tb_PROJECTID = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_CoordSys = new System.Windows.Forms.TextBox();
            this.tb_scale = new System.Windows.Forms.TextBox();
            this.tb_samplesize = new System.Windows.Forms.TextBox();
            this.tb_lotsize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_projectname = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(18, 20);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(338, 20);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(376, 54);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "选择已有质检任务";
            // 
            // buttonok
            // 
            this.buttonok.Location = new System.Drawing.Point(161, 556);
            this.buttonok.Name = "buttonok";
            this.buttonok.Size = new System.Drawing.Size(75, 23);
            this.buttonok.TabIndex = 1;
            this.buttonok.Text = "确定";
            this.buttonok.UseVisualStyleBackColor = true;
            this.buttonok.Click += new System.EventHandler(this.buttonok_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.richTextBox1);
            this.groupBox2.Controls.Add(this.sharedlistView1);
            this.groupBox2.Controls.Add(this.lastopentimetextBox1);
            this.groupBox2.Controls.Add(this.departmenttextBox2);
            this.groupBox2.Controls.Add(this.textBox_SampleFileType);
            this.groupBox2.Controls.Add(this.ownertextBox1);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.tb_SAMPLEAREA);
            this.groupBox2.Controls.Add(this.tb_PROJECTID);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.tb_CoordSys);
            this.groupBox2.Controls.Add(this.tb_scale);
            this.groupBox2.Controls.Add(this.tb_samplesize);
            this.groupBox2.Controls.Add(this.tb_lotsize);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tb_projectname);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 63);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(376, 487);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "项目属性";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox1.Location = new System.Drawing.Point(8, 404);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(336, 77);
            this.richTextBox1.TabIndex = 28;
            this.richTextBox1.Text = "备注：1、创建者拥有修改项目基本信息、更新结合表、抽样分区、检测点的权限，可以指定项目参与者；2、参与者拥有自动搜索、编辑检测线的权限。二者同时均可以查看项目基本" +
    "信息、抽样信息、样本质量信息，拥有打印各类报表的权限。";
            // 
            // sharedlistView1
            // 
            this.sharedlistView1.CheckBoxes = true;
            this.sharedlistView1.FullRowSelect = true;
            this.sharedlistView1.GridLines = true;
            this.sharedlistView1.Location = new System.Drawing.Point(149, 306);
            this.sharedlistView1.Name = "sharedlistView1";
            this.sharedlistView1.Size = new System.Drawing.Size(195, 62);
            this.sharedlistView1.TabIndex = 27;
            this.sharedlistView1.UseCompatibleStateImageBehavior = false;
            this.sharedlistView1.View = System.Windows.Forms.View.List;
            // 
            // lastopentimetextBox1
            // 
            this.lastopentimetextBox1.Location = new System.Drawing.Point(149, 374);
            this.lastopentimetextBox1.Name = "lastopentimetextBox1";
            this.lastopentimetextBox1.Size = new System.Drawing.Size(195, 21);
            this.lastopentimetextBox1.TabIndex = 25;
            // 
            // departmenttextBox2
            // 
            this.departmenttextBox2.Location = new System.Drawing.Point(149, 279);
            this.departmenttextBox2.Name = "departmenttextBox2";
            this.departmenttextBox2.Size = new System.Drawing.Size(195, 21);
            this.departmenttextBox2.TabIndex = 25;
            // 
            // textBox_SampleFileType
            // 
            this.textBox_SampleFileType.Location = new System.Drawing.Point(149, 225);
            this.textBox_SampleFileType.Name = "textBox_SampleFileType";
            this.textBox_SampleFileType.Size = new System.Drawing.Size(195, 21);
            this.textBox_SampleFileType.TabIndex = 26;
            // 
            // ownertextBox1
            // 
            this.ownertextBox1.Location = new System.Drawing.Point(149, 252);
            this.ownertextBox1.Name = "ownertextBox1";
            this.ownertextBox1.Size = new System.Drawing.Size(195, 21);
            this.ownertextBox1.TabIndex = 26;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 335);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 12);
            this.label10.TabIndex = 22;
            this.label10.Text = "项目参与者：";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 232);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(89, 12);
            this.label12.TabIndex = 24;
            this.label12.Text = "样本文件类型：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 382);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 12);
            this.label11.TabIndex = 23;
            this.label11.Text = "最近编辑时间：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 259);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 24;
            this.label5.Text = "创建者：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 287);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 23;
            this.label9.Text = "所属部门：";
            // 
            // tb_SAMPLEAREA
            // 
            this.tb_SAMPLEAREA.Location = new System.Drawing.Point(149, 105);
            this.tb_SAMPLEAREA.Name = "tb_SAMPLEAREA";
            this.tb_SAMPLEAREA.Size = new System.Drawing.Size(195, 21);
            this.tb_SAMPLEAREA.TabIndex = 21;
            // 
            // tb_PROJECTID
            // 
            this.tb_PROJECTID.Location = new System.Drawing.Point(149, 48);
            this.tb_PROJECTID.Name = "tb_PROJECTID";
            this.tb_PROJECTID.Size = new System.Drawing.Size(195, 21);
            this.tb_PROJECTID.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 19;
            this.label7.Text = "项目ID：";
            // 
            // tb_CoordSys
            // 
            this.tb_CoordSys.Location = new System.Drawing.Point(149, 195);
            this.tb_CoordSys.Name = "tb_CoordSys";
            this.tb_CoordSys.Size = new System.Drawing.Size(195, 21);
            this.tb_CoordSys.TabIndex = 14;
            // 
            // tb_scale
            // 
            this.tb_scale.Location = new System.Drawing.Point(149, 168);
            this.tb_scale.Name = "tb_scale";
            this.tb_scale.Size = new System.Drawing.Size(195, 21);
            this.tb_scale.TabIndex = 15;
            // 
            // tb_samplesize
            // 
            this.tb_samplesize.Location = new System.Drawing.Point(149, 135);
            this.tb_samplesize.Name = "tb_samplesize";
            this.tb_samplesize.Size = new System.Drawing.Size(195, 21);
            this.tb_samplesize.TabIndex = 16;
            // 
            // tb_lotsize
            // 
            this.tb_lotsize.Location = new System.Drawing.Point(149, 76);
            this.tb_lotsize.Name = "tb_lotsize";
            this.tb_lotsize.Size = new System.Drawing.Size(195, 21);
            this.tb_lotsize.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 200);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "坐标系统：";
            // 
            // tb_projectname
            // 
            this.tb_projectname.Location = new System.Drawing.Point(149, 21);
            this.tb_projectname.Name = "tb_projectname";
            this.tb_projectname.Size = new System.Drawing.Size(195, 21);
            this.tb_projectname.TabIndex = 18;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 173);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "比例尺（整数部分）：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 140);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "样本量（单位幅）：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 110);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 12;
            this.label8.Text = "抽样分区数：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "批量（单位幅）：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "项目名称（成果名称）：";
            // 
            // FrmOpenproject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 591);
            this.Controls.Add(this.buttonok);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmOpenproject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "打开项目";
            this.Load += new System.EventHandler(this.FrmOpenproject_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonok;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tb_SAMPLEAREA;
        private System.Windows.Forms.TextBox tb_PROJECTID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_CoordSys;
        private System.Windows.Forms.TextBox tb_scale;
        private System.Windows.Forms.TextBox tb_samplesize;
        private System.Windows.Forms.TextBox tb_lotsize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_projectname;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ListView sharedlistView1;
        private System.Windows.Forms.TextBox departmenttextBox2;
        private System.Windows.Forms.TextBox ownertextBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox lastopentimetextBox1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox_SampleFileType;
        private System.Windows.Forms.Label label12;
    }
}
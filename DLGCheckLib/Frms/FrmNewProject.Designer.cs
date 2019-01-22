namespace DLGCheckLib
{
    partial class FrmNewProject
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1_FileType = new System.Windows.Forms.ComboBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.sharedlistView1 = new System.Windows.Forms.ListView();
            this.tb_SAMPLEAREA = new System.Windows.Forms.TextBox();
            this.tb_PROJECTID = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lastopentimetextBox1 = new System.Windows.Forms.TextBox();
            this.departmenttextBox2 = new System.Windows.Forms.TextBox();
            this.ownertextBox1 = new System.Windows.Forms.TextBox();
            this.tb_CoordSys = new System.Windows.Forms.TextBox();
            this.tb_scale = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tb_samplesize = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_lotsize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_projectname = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bt_CoordSysSetting = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1_FileType);
            this.groupBox1.Controls.Add(this.richTextBox1);
            this.groupBox1.Controls.Add(this.sharedlistView1);
            this.groupBox1.Controls.Add(this.tb_SAMPLEAREA);
            this.groupBox1.Controls.Add(this.tb_PROJECTID);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.lastopentimetextBox1);
            this.groupBox1.Controls.Add(this.departmenttextBox2);
            this.groupBox1.Controls.Add(this.ownertextBox1);
            this.groupBox1.Controls.Add(this.tb_CoordSys);
            this.groupBox1.Controls.Add(this.tb_scale);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.tb_samplesize);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tb_lotsize);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tb_projectname);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 503);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "填写项目基本信息";
            // 
            // comboBox1_FileType
            // 
            this.comboBox1_FileType.FormattingEnabled = true;
            this.comboBox1_FileType.Items.AddRange(new object[] {
            "DWG(AutoCAD Files)",
            "MDB(Persanal Geodatabase)",
            "FGDB(File Geodatabase)",
            "DOM(Digtial Orth Image)"});
            this.comboBox1_FileType.Location = new System.Drawing.Point(168, 376);
            this.comboBox1_FileType.Name = "comboBox1_FileType";
            this.comboBox1_FileType.Size = new System.Drawing.Size(195, 20);
            this.comboBox1_FileType.TabIndex = 10;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox1.Location = new System.Drawing.Point(27, 420);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(336, 77);
            this.richTextBox1.TabIndex = 9;
            this.richTextBox1.Text = "备注：1、创建者拥有修改项目基本信息、更新结合表、抽样分区、检测点的权限，可以指定项目参与者；2、参与者拥有自动搜索、编辑检测线的权限。二者同时均可以查看项目基本" +
    "信息、抽样信息、样本质量信息，拥有打印各类报表的权限。";
            // 
            // sharedlistView1
            // 
            this.sharedlistView1.CheckBoxes = true;
            this.sharedlistView1.FullRowSelect = true;
            this.sharedlistView1.GridLines = true;
            this.sharedlistView1.Location = new System.Drawing.Point(168, 271);
            this.sharedlistView1.Name = "sharedlistView1";
            this.sharedlistView1.Size = new System.Drawing.Size(195, 62);
            this.sharedlistView1.TabIndex = 8;
            this.sharedlistView1.UseCompatibleStateImageBehavior = false;
            this.sharedlistView1.View = System.Windows.Forms.View.List;
            // 
            // tb_SAMPLEAREA
            // 
            this.tb_SAMPLEAREA.Location = new System.Drawing.Point(168, 103);
            this.tb_SAMPLEAREA.Name = "tb_SAMPLEAREA";
            this.tb_SAMPLEAREA.Size = new System.Drawing.Size(195, 21);
            this.tb_SAMPLEAREA.TabIndex = 7;
            // 
            // tb_PROJECTID
            // 
            this.tb_PROJECTID.Location = new System.Drawing.Point(168, 47);
            this.tb_PROJECTID.Name = "tb_PROJECTID";
            this.tb_PROJECTID.Size = new System.Drawing.Size(195, 21);
            this.tb_PROJECTID.TabIndex = 6;
            this.tb_PROJECTID.Text = "项目唯一标示符，格式：测区首字母+比例尺+成果类型+日期+检验次数，例如枝江市1:500地形图测绘“ZJS500DLG20160426”。";
            this.tb_PROJECTID.Click += new System.EventHandler(this.tb_PROJECTID_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "项目ID：";
            // 
            // lastopentimetextBox1
            // 
            this.lastopentimetextBox1.Location = new System.Drawing.Point(168, 340);
            this.lastopentimetextBox1.Name = "lastopentimetextBox1";
            this.lastopentimetextBox1.Size = new System.Drawing.Size(195, 21);
            this.lastopentimetextBox1.TabIndex = 2;
            // 
            // departmenttextBox2
            // 
            this.departmenttextBox2.Location = new System.Drawing.Point(168, 243);
            this.departmenttextBox2.Name = "departmenttextBox2";
            this.departmenttextBox2.Size = new System.Drawing.Size(195, 21);
            this.departmenttextBox2.TabIndex = 2;
            // 
            // ownertextBox1
            // 
            this.ownertextBox1.Location = new System.Drawing.Point(168, 215);
            this.ownertextBox1.Name = "ownertextBox1";
            this.ownertextBox1.Size = new System.Drawing.Size(195, 21);
            this.ownertextBox1.TabIndex = 2;
            // 
            // tb_CoordSys
            // 
            this.tb_CoordSys.Location = new System.Drawing.Point(168, 187);
            this.tb_CoordSys.Name = "tb_CoordSys";
            this.tb_CoordSys.Size = new System.Drawing.Size(195, 21);
            this.tb_CoordSys.TabIndex = 2;
            // 
            // tb_scale
            // 
            this.tb_scale.Location = new System.Drawing.Point(168, 159);
            this.tb_scale.Name = "tb_scale";
            this.tb_scale.Size = new System.Drawing.Size(195, 21);
            this.tb_scale.TabIndex = 2;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(25, 379);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(89, 12);
            this.label12.TabIndex = 1;
            this.label12.Text = "样本数据类型：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(25, 349);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 12);
            this.label11.TabIndex = 1;
            this.label11.Text = "最近修改时间：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(25, 285);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 12);
            this.label10.TabIndex = 1;
            this.label10.Text = "项目参与者：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(25, 248);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 1;
            this.label9.Text = "所属部门：";
            // 
            // tb_samplesize
            // 
            this.tb_samplesize.Location = new System.Drawing.Point(168, 131);
            this.tb_samplesize.Name = "tb_samplesize";
            this.tb_samplesize.Size = new System.Drawing.Size(195, 21);
            this.tb_samplesize.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 220);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "创建者：";
            // 
            // tb_lotsize
            // 
            this.tb_lotsize.Location = new System.Drawing.Point(168, 75);
            this.tb_lotsize.Name = "tb_lotsize";
            this.tb_lotsize.Size = new System.Drawing.Size(195, 21);
            this.tb_lotsize.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 192);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "坐标系统：";
            // 
            // tb_projectname
            // 
            this.tb_projectname.Location = new System.Drawing.Point(168, 19);
            this.tb_projectname.Name = "tb_projectname";
            this.tb_projectname.Size = new System.Drawing.Size(195, 21);
            this.tb_projectname.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 164);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "比例尺（整数部分）：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "样本量（单位幅）：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "抽样分区数：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "批量（单位幅）：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "项目名称（成果名称）：";
            // 
            // bt_CoordSysSetting
            // 
            this.bt_CoordSysSetting.Location = new System.Drawing.Point(69, 521);
            this.bt_CoordSysSetting.Name = "bt_CoordSysSetting";
            this.bt_CoordSysSetting.Size = new System.Drawing.Size(75, 23);
            this.bt_CoordSysSetting.TabIndex = 8;
            this.bt_CoordSysSetting.Text = "设置坐标系";
            this.bt_CoordSysSetting.UseVisualStyleBackColor = true;
            this.bt_CoordSysSetting.Click += new System.EventHandler(this.bt_CoordSysSetting_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(255, 521);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "点击新建";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FrmNewProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 556);
            this.Controls.Add(this.bt_CoordSysSetting);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Name = "FrmNewProject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "测绘成果检查项目";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox tb_samplesize;
        private System.Windows.Forms.TextBox tb_lotsize;
        private System.Windows.Forms.TextBox tb_projectname;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_scale;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_PROJECTID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_SAMPLEAREA;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tb_CoordSys;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bt_CoordSysSetting;
        private System.Windows.Forms.ListView sharedlistView1;
        private System.Windows.Forms.TextBox departmenttextBox2;
        private System.Windows.Forms.TextBox ownertextBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox lastopentimetextBox1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBox1_FileType;
        private System.Windows.Forms.Label label12;
    }
}
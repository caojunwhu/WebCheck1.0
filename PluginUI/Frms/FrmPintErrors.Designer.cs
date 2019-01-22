namespace PluginUI.Frms
{
    partial class FrmPintErrors
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPintErrors));
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.relativeErrortoolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.SampleAreaComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.SampleSerialComboBox2 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.MapNumberComboBox3 = new System.Windows.Forms.ToolStripComboBox();
            this.SearchCheckLineButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deletetoolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.SaveCheckLinestoolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel8 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButton1_editerror = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToOrderColumns = true;
            this.dataGridViewX1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
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
            this.dataGridViewX1.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.ReadOnly = true;
            this.dataGridViewX1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewX1.Size = new System.Drawing.Size(879, 155);
            this.dataGridViewX1.TabIndex = 0;
            this.dataGridViewX1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellClick);
            this.dataGridViewX1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellContentClick);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.toolStrip2);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.dataGridViewX1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(879, 155);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(879, 227);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.relativeErrortoolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 25);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(879, 22);
            this.statusStrip1.TabIndex = 0;
            // 
            // relativeErrortoolStripStatusLabel1
            // 
            this.relativeErrortoolStripStatusLabel1.Name = "relativeErrortoolStripStatusLabel1";
            this.relativeErrortoolStripStatusLabel1.Size = new System.Drawing.Size(68, 17);
            this.relativeErrortoolStripStatusLabel1.Text = "错漏统计：";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.SampleAreaComboBox1,
            this.toolStripLabel2,
            this.SampleSerialComboBox2,
            this.toolStripLabel3,
            this.MapNumberComboBox3,
            this.SearchCheckLineButton,
            this.toolStripSeparator1,
            this.toolStripButton1_editerror,
            this.toolStripSeparator2,
            this.deletetoolStripButton1,
            this.SaveCheckLinestoolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(726, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(68, 22);
            this.toolStripLabel1.Text = "抽样分区：";
            // 
            // SampleAreaComboBox1
            // 
            this.SampleAreaComboBox1.DropDownWidth = 75;
            this.SampleAreaComboBox1.Name = "SampleAreaComboBox1";
            this.SampleAreaComboBox1.Size = new System.Drawing.Size(75, 25);
            this.SampleAreaComboBox1.SelectedIndexChanged += new System.EventHandler(this.SampleAreaComboBox1_SelectedIndexChanged);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(56, 22);
            this.toolStripLabel2.Text = "流水号：";
            // 
            // SampleSerialComboBox2
            // 
            this.SampleSerialComboBox2.Name = "SampleSerialComboBox2";
            this.SampleSerialComboBox2.Size = new System.Drawing.Size(75, 25);
            this.SampleSerialComboBox2.SelectedIndexChanged += new System.EventHandler(this.SampleSerialComboBox2_SelectedIndexChanged);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(56, 22);
            this.toolStripLabel3.Text = "图幅号：";
            // 
            // MapNumberComboBox3
            // 
            this.MapNumberComboBox3.Name = "MapNumberComboBox3";
            this.MapNumberComboBox3.Size = new System.Drawing.Size(90, 25);
            // 
            // SearchCheckLineButton
            // 
            this.SearchCheckLineButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.SearchCheckLineButton.Image = ((System.Drawing.Image)(resources.GetObject("SearchCheckLineButton.Image")));
            this.SearchCheckLineButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SearchCheckLineButton.Name = "SearchCheckLineButton";
            this.SearchCheckLineButton.Size = new System.Drawing.Size(84, 22);
            this.SearchCheckLineButton.Text = "开始标注错漏";
            this.SearchCheckLineButton.Click += new System.EventHandler(this.PickCheckLineButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // deletetoolStripButton1
            // 
            this.deletetoolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.deletetoolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("deletetoolStripButton1.Image")));
            this.deletetoolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deletetoolStripButton1.Name = "deletetoolStripButton1";
            this.deletetoolStripButton1.Size = new System.Drawing.Size(60, 22);
            this.deletetoolStripButton1.Text = "删除错漏";
            this.deletetoolStripButton1.Click += new System.EventHandler(this.deletetoolStripButton1_Click);
            // 
            // SaveCheckLinestoolStripButton1
            // 
            this.SaveCheckLinestoolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.SaveCheckLinestoolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("SaveCheckLinestoolStripButton1.Image")));
            this.SaveCheckLinestoolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveCheckLinestoolStripButton1.Name = "SaveCheckLinestoolStripButton1";
            this.SaveCheckLinestoolStripButton1.Size = new System.Drawing.Size(72, 22);
            this.SaveCheckLinestoolStripButton1.Text = "提交数据库";
            this.SaveCheckLinestoolStripButton1.Click += new System.EventHandler(this.SaveCheckLinestoolStripButton1_Click);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel8,
            this.toolStripComboBox1});
            this.toolStrip2.Location = new System.Drawing.Point(3, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip2.Size = new System.Drawing.Size(200, 25);
            this.toolStrip2.TabIndex = 1;
            // 
            // toolStripLabel8
            // 
            this.toolStripLabel8.Name = "toolStripLabel8";
            this.toolStripLabel8.Size = new System.Drawing.Size(80, 22);
            this.toolStripLabel8.Text = "跳转比例尺：";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "1:50",
            "1:100",
            "1:200",
            "1:500",
            "1:1000",
            "1:2000",
            ""});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(106, 25);
            this.toolStripComboBox1.Text = "1:100";
            this.toolStripComboBox1.TextChanged += new System.EventHandler(this.toolStripComboBox1_TextChanged);
            // 
            // toolStripButton1_editerror
            // 
            this.toolStripButton1_editerror.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1_editerror.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1_editerror.Image")));
            this.toolStripButton1_editerror.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1_editerror.Name = "toolStripButton1_editerror";
            this.toolStripButton1_editerror.Size = new System.Drawing.Size(60, 22);
            this.toolStripButton1_editerror.Text = "编辑错漏";
            this.toolStripButton1_editerror.Click += new System.EventHandler(this.toolStripButton1_editerror_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // FrmPintErrors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 227);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmPintErrors";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "人机交互标注样本错漏";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmSearchCheckLines_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel relativeErrortoolStripStatusLabel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox SampleAreaComboBox1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox SampleSerialComboBox2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox MapNumberComboBox3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton SaveCheckLinestoolStripButton1;
        private System.Windows.Forms.ToolStripButton SearchCheckLineButton;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel8;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripButton deletetoolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton1_editerror;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}
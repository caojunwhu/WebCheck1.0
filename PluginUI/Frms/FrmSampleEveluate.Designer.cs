namespace PluginUI.Frms
{
    partial class FrmSampleEveluate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSampleEveluate));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.relativeErrortoolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.superGridControl1 = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.SampleAreaComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.SampleSerialComboBox2 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.MapNumberComboBox3 = new System.Windows.Forms.ToolStripComboBox();
            this.TSB_PrintQualityReportOfSample = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.superGridControl1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(879, 180);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
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
            // superGridControl1
            // 
            this.superGridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superGridControl1.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.superGridControl1.Location = new System.Drawing.Point(0, 0);
            this.superGridControl1.Name = "superGridControl1";
            // 
            // 
            // 
            this.superGridControl1.PrimaryGrid.EnableCellMerging = true;
            this.superGridControl1.Size = new System.Drawing.Size(879, 180);
            this.superGridControl1.TabIndex = 0;
            this.superGridControl1.Text = "superGridControl1";
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
            this.TSB_PrintQualityReportOfSample});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(637, 25);
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
            // TSB_PrintQualityReportOfSample
            // 
            this.TSB_PrintQualityReportOfSample.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TSB_PrintQualityReportOfSample.Image = ((System.Drawing.Image)(resources.GetObject("TSB_PrintQualityReportOfSample.Image")));
            this.TSB_PrintQualityReportOfSample.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSB_PrintQualityReportOfSample.Name = "TSB_PrintQualityReportOfSample";
            this.TSB_PrintQualityReportOfSample.Size = new System.Drawing.Size(168, 22);
            this.TSB_PrintQualityReportOfSample.Text = "打印样本质量统计表（归档）";
            this.TSB_PrintQualityReportOfSample.Click += new System.EventHandler(this.TSB_PrintQualityReportOfSample_Click);
            // 
            // FrmSampleEveluate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 227);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSampleEveluate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "样本质量评价：";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmSearchCheckLines_FormClosed);
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
            this.ResumeLayout(false);

        }

        #endregion
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
        private DevComponents.DotNetBar.SuperGrid.SuperGridControl superGridControl1;
        private System.Windows.Forms.ToolStripButton TSB_PrintQualityReportOfSample;
    }
}
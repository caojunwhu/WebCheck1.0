namespace PluginUI.Frms
{
    partial class FrmQualityErrorReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmQualityErrorReport));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_EvluationSample = new System.Windows.Forms.Button();
            this.btn_ExportErrorData = new System.Windows.Forms.Button();
            this.PrintProjectReportbutton2 = new System.Windows.Forms.Button();
            this.CheckercomboBox2 = new System.Windows.Forms.ComboBox();
            this.PrintSampleReportbutton1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SamplecomboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btn_PrintReportOfWeiTu = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridViewX1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(713, 220);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "记录表";
            // 
            // dataGridViewX1
            // 
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
            this.dataGridViewX1.Location = new System.Drawing.Point(3, 17);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.ReadOnly = true;
            this.dataGridViewX1.RowTemplate.Height = 23;
            this.dataGridViewX1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewX1.Size = new System.Drawing.Size(707, 200);
            this.dataGridViewX1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_PrintReportOfWeiTu);
            this.groupBox2.Controls.Add(this.btn_EvluationSample);
            this.groupBox2.Controls.Add(this.btn_ExportErrorData);
            this.groupBox2.Controls.Add(this.PrintProjectReportbutton2);
            this.groupBox2.Controls.Add(this.CheckercomboBox2);
            this.groupBox2.Controls.Add(this.PrintSampleReportbutton1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.SamplecomboBox1);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(3, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(698, 86);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "打印设置";
            // 
            // btn_EvluationSample
            // 
            this.btn_EvluationSample.Location = new System.Drawing.Point(419, 13);
            this.btn_EvluationSample.Name = "btn_EvluationSample";
            this.btn_EvluationSample.Size = new System.Drawing.Size(134, 23);
            this.btn_EvluationSample.TabIndex = 5;
            this.btn_EvluationSample.Text = "打印样本质量评分";
            this.btn_EvluationSample.UseVisualStyleBackColor = true;
            this.btn_EvluationSample.Click += new System.EventHandler(this.btn_EvluationSample_Click);
            // 
            // btn_ExportErrorData
            // 
            this.btn_ExportErrorData.Location = new System.Drawing.Point(265, 13);
            this.btn_ExportErrorData.Name = "btn_ExportErrorData";
            this.btn_ExportErrorData.Size = new System.Drawing.Size(133, 23);
            this.btn_ExportErrorData.TabIndex = 4;
            this.btn_ExportErrorData.Text = "导出错漏数据集";
            this.btn_ExportErrorData.UseVisualStyleBackColor = true;
            this.btn_ExportErrorData.Click += new System.EventHandler(this.btn_ExportErrorData_Click);
            // 
            // PrintProjectReportbutton2
            // 
            this.PrintProjectReportbutton2.Location = new System.Drawing.Point(419, 47);
            this.PrintProjectReportbutton2.Name = "PrintProjectReportbutton2";
            this.PrintProjectReportbutton2.Size = new System.Drawing.Size(134, 23);
            this.PrintProjectReportbutton2.TabIndex = 3;
            this.PrintProjectReportbutton2.Text = "打印该项目问题记录表";
            this.PrintProjectReportbutton2.UseVisualStyleBackColor = true;
            this.PrintProjectReportbutton2.Click += new System.EventHandler(this.PrintProjectReportbutton2_Click);
            // 
            // CheckercomboBox2
            // 
            this.CheckercomboBox2.FormattingEnabled = true;
            this.CheckercomboBox2.Location = new System.Drawing.Point(104, 50);
            this.CheckercomboBox2.Name = "CheckercomboBox2";
            this.CheckercomboBox2.Size = new System.Drawing.Size(121, 20);
            this.CheckercomboBox2.TabIndex = 1;
            this.CheckercomboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // PrintSampleReportbutton1
            // 
            this.PrintSampleReportbutton1.Location = new System.Drawing.Point(265, 47);
            this.PrintSampleReportbutton1.Name = "PrintSampleReportbutton1";
            this.PrintSampleReportbutton1.Size = new System.Drawing.Size(133, 23);
            this.PrintSampleReportbutton1.TabIndex = 2;
            this.PrintSampleReportbutton1.Text = "打印该样本问题记录表";
            this.PrintSampleReportbutton1.UseVisualStyleBackColor = true;
            this.PrintSampleReportbutton1.Click += new System.EventHandler(this.PrintSampleReportbutton1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "检查者：";
            // 
            // SamplecomboBox1
            // 
            this.SamplecomboBox1.FormattingEnabled = true;
            this.SamplecomboBox1.Location = new System.Drawing.Point(104, 15);
            this.SamplecomboBox1.Name = "SamplecomboBox1";
            this.SamplecomboBox1.Size = new System.Drawing.Size(121, 20);
            this.SamplecomboBox1.TabIndex = 1;
            this.SamplecomboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "样本号：";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(713, 336);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(713, 361);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(713, 336);
            this.splitContainer1.SplitterDistance = 112;
            this.splitContainer1.TabIndex = 0;
            // 
            // btn_PrintReportOfWeiTu
            // 
            this.btn_PrintReportOfWeiTu.Location = new System.Drawing.Point(581, 13);
            this.btn_PrintReportOfWeiTu.Name = "btn_PrintReportOfWeiTu";
            this.btn_PrintReportOfWeiTu.Size = new System.Drawing.Size(111, 23);
            this.btn_PrintReportOfWeiTu.TabIndex = 6;
            this.btn_PrintReportOfWeiTu.Text = "打印委托检验报告";
            this.btn_PrintReportOfWeiTu.UseVisualStyleBackColor = true;
            this.btn_PrintReportOfWeiTu.Click += new System.EventHandler(this.btn_PrintReportOfWeiTu_Click);
            // 
            // FrmQualityErrorReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 361);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmQualityErrorReport";
            this.Text = "样本质量问题记录表打印";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox SamplecomboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CheckercomboBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button PrintSampleReportbutton1;
        private System.Windows.Forms.Button PrintProjectReportbutton2;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btn_ExportErrorData;
        private System.Windows.Forms.Button btn_EvluationSample;
        private System.Windows.Forms.Button btn_PrintReportOfWeiTu;
    }
}
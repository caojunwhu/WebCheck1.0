namespace PluginUI.Frms
{
    partial class FrmSearchCheckLines
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSearchCheckLines));
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox2_Plainbufferlength = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel9 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox1_heigthdiff = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBox1_heigthbufferlength = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel8 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.planErrortoolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.heightErrortoolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.SampleAreaComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.SampleSerialComboBox2 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.MapNumberComboBox3 = new System.Windows.Forms.ToolStripComboBox();
            this.SearchCheckLineButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ReSetCheckPointToWaittoolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveCheckLinestoolStripButton1 = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToOrderColumns = true;
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
            this.dataGridViewX1.Size = new System.Drawing.Size(855, 197);
            this.dataGridViewX1.TabIndex = 0;
            this.dataGridViewX1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellContentClick);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.toolStrip2);
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.dataGridViewX1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(855, 197);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(855, 269);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel6,
            this.toolStripTextBox2_Plainbufferlength,
            this.toolStripSeparator6,
            this.toolStripLabel9,
            this.toolStripTextBox1_heigthdiff,
            this.toolStripSeparator3,
            this.toolStripLabel4,
            this.toolStripTextBox1_heigthbufferlength,
            this.toolStripSeparator4,
            this.toolStripLabel8,
            this.toolStripComboBox1});
            this.toolStrip2.Location = new System.Drawing.Point(3, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip2.Size = new System.Drawing.Size(784, 25);
            this.toolStrip2.TabIndex = 2;
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(139, 22);
            this.toolStripLabel6.Text = "平面点搜索半径（m）：";
            // 
            // toolStripTextBox2_Plainbufferlength
            // 
            this.toolStripTextBox2_Plainbufferlength.Name = "toolStripTextBox2_Plainbufferlength";
            this.toolStripTextBox2_Plainbufferlength.ReadOnly = true;
            this.toolStripTextBox2_Plainbufferlength.Size = new System.Drawing.Size(50, 25);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel9
            // 
            this.toolStripLabel9.Name = "toolStripLabel9";
            this.toolStripLabel9.Size = new System.Drawing.Size(151, 22);
            this.toolStripLabel9.Text = "高程注记搜索限差（m）：";
            // 
            // toolStripTextBox1_heigthdiff
            // 
            this.toolStripTextBox1_heigthdiff.Name = "toolStripTextBox1_heigthdiff";
            this.toolStripTextBox1_heigthdiff.ReadOnly = true;
            this.toolStripTextBox1_heigthdiff.Size = new System.Drawing.Size(50, 25);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(151, 22);
            this.toolStripLabel4.Text = "高程注记搜索半径（m）：";
            // 
            // toolStripTextBox1_heigthbufferlength
            // 
            this.toolStripTextBox1_heigthbufferlength.Name = "toolStripTextBox1_heigthbufferlength";
            this.toolStripTextBox1_heigthbufferlength.Size = new System.Drawing.Size(50, 25);
            this.toolStripTextBox1_heigthbufferlength.TextChanged += new System.EventHandler(this.toolStripTextBox1_heigthbufferlength_TextChanged);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
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
            this.toolStripComboBox1.Size = new System.Drawing.Size(75, 25);
            this.toolStripComboBox1.Text = "1:100";
            this.toolStripComboBox1.Click += new System.EventHandler(this.toolStripComboBox1_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.planErrortoolStripStatusLabel1,
            this.heightErrortoolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 25);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(855, 22);
            this.statusStrip1.TabIndex = 0;
            // 
            // planErrortoolStripStatusLabel1
            // 
            this.planErrortoolStripStatusLabel1.Name = "planErrortoolStripStatusLabel1";
            this.planErrortoolStripStatusLabel1.Size = new System.Drawing.Size(68, 17);
            this.planErrortoolStripStatusLabel1.Text = "平面精度：";
            // 
            // heightErrortoolStripStatusLabel2
            // 
            this.heightErrortoolStripStatusLabel2.Name = "heightErrortoolStripStatusLabel2";
            this.heightErrortoolStripStatusLabel2.Size = new System.Drawing.Size(56, 17);
            this.heightErrortoolStripStatusLabel2.Text = "高程精度";
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
            this.ReSetCheckPointToWaittoolStripButton1,
            this.toolStripSeparator2,
            this.SaveCheckLinestoolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(694, 25);
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
            this.SearchCheckLineButton.Image = ((System.Drawing.Image)(resources.GetObject("SearchCheckLineButton.Image")));
            this.SearchCheckLineButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SearchCheckLineButton.Name = "SearchCheckLineButton";
            this.SearchCheckLineButton.Size = new System.Drawing.Size(88, 22);
            this.SearchCheckLineButton.Text = "搜索检测线";
            this.SearchCheckLineButton.Click += new System.EventHandler(this.SearchCheckLineButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // ReSetCheckPointToWaittoolStripButton1
            // 
            this.ReSetCheckPointToWaittoolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ReSetCheckPointToWaittoolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("ReSetCheckPointToWaittoolStripButton1.Image")));
            this.ReSetCheckPointToWaittoolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ReSetCheckPointToWaittoolStripButton1.Name = "ReSetCheckPointToWaittoolStripButton1";
            this.ReSetCheckPointToWaittoolStripButton1.Size = new System.Drawing.Size(84, 22);
            this.ReSetCheckPointToWaittoolStripButton1.Text = "重设为待检点";
            this.ReSetCheckPointToWaittoolStripButton1.Click += new System.EventHandler(this.ReSetCheckPointToWaittoolStripButton1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
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
            // FrmSearchCheckLines
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 269);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSearchCheckLines";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "电脑辅助搜索检测线";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmSearchCheckLines_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel planErrortoolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel heightErrortoolStripStatusLabel2;
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
        private System.Windows.Forms.ToolStripButton ReSetCheckPointToWaittoolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2_Plainbufferlength;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel toolStripLabel8;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1_heigthbufferlength;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel9;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1_heigthdiff;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}
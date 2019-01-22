namespace PluginUI.Frms
{
    partial class FrmSampleCheckStation
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
            this.superGridControl1 = new DevComponents.DotNetBar.SuperGrid.SuperGridControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.AutoMatchScaterbutton1 = new System.Windows.Forms.Button();
            this.RefeshCheckStatebutton1 = new System.Windows.Forms.Button();
            this.JumpToRelativeCheckStationbutton2 = new System.Windows.Forms.Button();
            this.JumpToPHCheckStationbutton1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // superGridControl1
            // 
            this.superGridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superGridControl1.FilterExprColors.SysFunction = System.Drawing.Color.DarkRed;
            this.superGridControl1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.superGridControl1.Location = new System.Drawing.Point(0, 0);
            this.superGridControl1.Name = "superGridControl1";
            // 
            // 
            // 
            this.superGridControl1.PrimaryGrid.AllowEdit = false;
            this.superGridControl1.PrimaryGrid.ColumnAutoSizeMode = DevComponents.DotNetBar.SuperGrid.ColumnAutoSizeMode.AllCells;
            this.superGridControl1.PrimaryGrid.EnableFiltering = true;
            this.superGridControl1.PrimaryGrid.EnableRowFiltering = true;
            // 
            // 
            // 
            this.superGridControl1.PrimaryGrid.Filter.ShowPanelFilterExpr = true;
            // 
            // 
            // 
            this.superGridControl1.PrimaryGrid.Header.RowHeaderVisibility = DevComponents.DotNetBar.SuperGrid.RowHeaderVisibility.Always;
            this.superGridControl1.PrimaryGrid.MultiSelect = false;
            this.superGridControl1.PrimaryGrid.SelectionGranularity = DevComponents.DotNetBar.SuperGrid.SelectionGranularity.RowWithCellHighlight;
            this.superGridControl1.PrimaryGrid.ShowRowGridIndex = true;
            this.superGridControl1.PrimaryGrid.ShowTreeButtons = true;
            this.superGridControl1.PrimaryGrid.ShowTreeLines = true;
            this.superGridControl1.PrimaryGrid.UseAlternateColumnStyle = true;
            this.superGridControl1.PrimaryGrid.UseAlternateRowStyle = true;
            this.superGridControl1.Size = new System.Drawing.Size(905, 291);
            this.superGridControl1.TabIndex = 0;
            this.superGridControl1.Text = "superGridControl1";
            this.superGridControl1.DataBindingComplete += new System.EventHandler<DevComponents.DotNetBar.SuperGrid.GridDataBindingCompleteEventArgs>(this.superGridControl1_DataBindingComplete);
            this.superGridControl1.Click += new System.EventHandler(this.superGridControl1_Click);
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
            this.splitContainer1.Panel1.Controls.Add(this.AutoMatchScaterbutton1);
            this.splitContainer1.Panel1.Controls.Add(this.RefeshCheckStatebutton1);
            this.splitContainer1.Panel1.Controls.Add(this.JumpToRelativeCheckStationbutton2);
            this.splitContainer1.Panel1.Controls.Add(this.JumpToPHCheckStationbutton1);
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.superGridControl1);
            this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Size = new System.Drawing.Size(905, 329);
            this.splitContainer1.SplitterDistance = 34;
            this.splitContainer1.TabIndex = 1;
            // 
            // AutoMatchScaterbutton1
            // 
            this.AutoMatchScaterbutton1.Location = new System.Drawing.Point(685, 6);
            this.AutoMatchScaterbutton1.Name = "AutoMatchScaterbutton1";
            this.AutoMatchScaterbutton1.Size = new System.Drawing.Size(158, 23);
            this.AutoMatchScaterbutton1.TabIndex = 5;
            this.AutoMatchScaterbutton1.Text = "自动化批量匹配散点[New]";
            this.AutoMatchScaterbutton1.UseVisualStyleBackColor = true;
            this.AutoMatchScaterbutton1.Click += new System.EventHandler(this.AutoMatchScaterbutton1_Click);
            // 
            // RefeshCheckStatebutton1
            // 
            this.RefeshCheckStatebutton1.Location = new System.Drawing.Point(568, 6);
            this.RefeshCheckStatebutton1.Name = "RefeshCheckStatebutton1";
            this.RefeshCheckStatebutton1.Size = new System.Drawing.Size(95, 23);
            this.RefeshCheckStatebutton1.TabIndex = 4;
            this.RefeshCheckStatebutton1.Text = "刷新检测状态";
            this.RefeshCheckStatebutton1.UseVisualStyleBackColor = true;
            this.RefeshCheckStatebutton1.Click += new System.EventHandler(this.RefeshCheckStatebutton1_Click);
            // 
            // JumpToRelativeCheckStationbutton2
            // 
            this.JumpToRelativeCheckStationbutton2.Location = new System.Drawing.Point(398, 6);
            this.JumpToRelativeCheckStationbutton2.Name = "JumpToRelativeCheckStationbutton2";
            this.JumpToRelativeCheckStationbutton2.Size = new System.Drawing.Size(150, 23);
            this.JumpToRelativeCheckStationbutton2.TabIndex = 3;
            this.JumpToRelativeCheckStationbutton2.Text = "跳转到间距边长检测台";
            this.JumpToRelativeCheckStationbutton2.UseVisualStyleBackColor = true;
            this.JumpToRelativeCheckStationbutton2.Click += new System.EventHandler(this.JumpToRelativeCheckStationbutton2_Click);
            // 
            // JumpToPHCheckStationbutton1
            // 
            this.JumpToPHCheckStationbutton1.Location = new System.Drawing.Point(221, 6);
            this.JumpToPHCheckStationbutton1.Name = "JumpToPHCheckStationbutton1";
            this.JumpToPHCheckStationbutton1.Size = new System.Drawing.Size(157, 23);
            this.JumpToPHCheckStationbutton1.TabIndex = 2;
            this.JumpToPHCheckStationbutton1.Text = "跳转到平面高程检测台";
            this.JumpToPHCheckStationbutton1.UseVisualStyleBackColor = true;
            this.JumpToPHCheckStationbutton1.Click += new System.EventHandler(this.JumpToPHCheckStationbutton1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(98, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "当前图幅：";
            // 
            // FrmSampleCheckStation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 329);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmSampleCheckStation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "样本检测工作台";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.SuperGrid.SuperGridControl superGridControl1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button JumpToRelativeCheckStationbutton2;
        private System.Windows.Forms.Button JumpToPHCheckStationbutton1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button RefeshCheckStatebutton1;
        private System.Windows.Forms.Button AutoMatchScaterbutton1;
    }
}
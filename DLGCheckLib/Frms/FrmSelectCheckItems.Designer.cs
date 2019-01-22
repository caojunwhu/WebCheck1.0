namespace DLGCheckLib.Frms
{
    partial class FrmSelectCheckItems
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
            this.advTree1_selectcheckitems = new DevComponents.AdvTree.AdvTree();
            this.nodeConnector1 = new DevComponents.AdvTree.NodeConnector();
            this.elementStyle1 = new DevComponents.DotNetBar.ElementStyle();
            this.button1 = new System.Windows.Forms.Button();
            this.columnHeader1 = new DevComponents.AdvTree.ColumnHeader();
            this.columnHeader2 = new DevComponents.AdvTree.ColumnHeader();
            this.columnHeader5 = new DevComponents.AdvTree.ColumnHeader();
            this.columnHeader6 = new DevComponents.AdvTree.ColumnHeader();
            this.columnHeader7 = new DevComponents.AdvTree.ColumnHeader();
            this.elementStyle2 = new DevComponents.DotNetBar.ElementStyle();
            ((System.ComponentModel.ISupportInitialize)(this.advTree1_selectcheckitems)).BeginInit();
            this.SuspendLayout();
            // 
            // advTree1_selectcheckitems
            // 
            this.advTree1_selectcheckitems.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.advTree1_selectcheckitems.AllowDrop = true;
            this.advTree1_selectcheckitems.BackColor = System.Drawing.SystemColors.Window;
            // 
            // 
            // 
            this.advTree1_selectcheckitems.BackgroundStyle.Class = "TreeBorderKey";
            this.advTree1_selectcheckitems.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.advTree1_selectcheckitems.Columns.Add(this.columnHeader1);
            this.advTree1_selectcheckitems.Columns.Add(this.columnHeader2);
            this.advTree1_selectcheckitems.Columns.Add(this.columnHeader5);
            this.advTree1_selectcheckitems.Columns.Add(this.columnHeader6);
            this.advTree1_selectcheckitems.Columns.Add(this.columnHeader7);
            this.advTree1_selectcheckitems.ExpandButtonType = DevComponents.AdvTree.eExpandButtonType.Triangle;
            this.advTree1_selectcheckitems.GroupNodeStyle = this.elementStyle2;
            this.advTree1_selectcheckitems.HotTracking = true;
            this.advTree1_selectcheckitems.Location = new System.Drawing.Point(12, 12);
            this.advTree1_selectcheckitems.Name = "advTree1_selectcheckitems";
            this.advTree1_selectcheckitems.NodesConnector = this.nodeConnector1;
            this.advTree1_selectcheckitems.NodeStyle = this.elementStyle1;
            this.advTree1_selectcheckitems.PathSeparator = ";";
            this.advTree1_selectcheckitems.Size = new System.Drawing.Size(778, 358);
            this.advTree1_selectcheckitems.Styles.Add(this.elementStyle1);
            this.advTree1_selectcheckitems.Styles.Add(this.elementStyle2);
            this.advTree1_selectcheckitems.TabIndex = 0;
            this.advTree1_selectcheckitems.Text = "advTree1";
            this.advTree1_selectcheckitems.NodeClick += new DevComponents.AdvTree.TreeNodeMouseEventHandler(this.advTree1_selectcheckitems_NodeClick);
            // 
            // nodeConnector1
            // 
            this.nodeConnector1.LineColor = System.Drawing.SystemColors.ControlText;
            // 
            // elementStyle1
            // 
            this.elementStyle1.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.elementStyle1.Name = "elementStyle1";
            this.elementStyle1.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(362, 390);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "选好了";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Name = "columnHeader1";
            this.columnHeader1.Text = "项目类型";
            this.columnHeader1.Width.Absolute = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Name = "columnHeader2";
            this.columnHeader2.Text = "成果种类";
            this.columnHeader2.Width.Absolute = 150;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Name = "columnHeader5";
            this.columnHeader5.Text = "检查项";
            this.columnHeader5.Width.Absolute = 150;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Name = "columnHeader6";
            this.columnHeader6.Text = "质量元素权";
            this.columnHeader6.Width.Absolute = 150;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Name = "columnHeader7";
            this.columnHeader7.Text = "质量子元素权";
            this.columnHeader7.Width.Absolute = 150;
            // 
            // elementStyle2
            // 
            this.elementStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(230)))), ((int)(((byte)(247)))));
            this.elementStyle2.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(168)))), ((int)(((byte)(228)))));
            this.elementStyle2.BackColorGradientAngle = 90;
            this.elementStyle2.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle2.BorderBottomWidth = 1;
            this.elementStyle2.BorderColor = System.Drawing.Color.DarkGray;
            this.elementStyle2.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle2.BorderLeftWidth = 1;
            this.elementStyle2.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle2.BorderRightWidth = 1;
            this.elementStyle2.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.elementStyle2.BorderTopWidth = 1;
            this.elementStyle2.CornerDiameter = 4;
            this.elementStyle2.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.elementStyle2.Description = "Blue";
            this.elementStyle2.Name = "elementStyle2";
            this.elementStyle2.PaddingBottom = 1;
            this.elementStyle2.PaddingLeft = 1;
            this.elementStyle2.PaddingRight = 1;
            this.elementStyle2.PaddingTop = 1;
            this.elementStyle2.TextColor = System.Drawing.Color.Black;
            // 
            // FrmSelectCheckItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 425);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.advTree1_selectcheckitems);
            this.Name = "FrmSelectCheckItems";
            this.Text = "FrmSelectCheckItems";
            ((System.ComponentModel.ISupportInitialize)(this.advTree1_selectcheckitems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.AdvTree.AdvTree advTree1_selectcheckitems;
        private DevComponents.AdvTree.ColumnHeader columnHeader1;
        private DevComponents.AdvTree.ColumnHeader columnHeader2;
        private DevComponents.AdvTree.ColumnHeader columnHeader5;
        private DevComponents.AdvTree.ColumnHeader columnHeader6;
        private DevComponents.AdvTree.ColumnHeader columnHeader7;
        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle1;
        private System.Windows.Forms.Button button1;
        private DevComponents.DotNetBar.ElementStyle elementStyle2;
    }
}
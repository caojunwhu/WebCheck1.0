namespace DLGCheckLib
{
    partial class FrmImportShapeFile2PostGIS
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
            this.textBox_ShpCheckPointsPath = new System.Windows.Forms.TextBox();
            this.button_ShpCheckPoints = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button_Import = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_ShpCheckPointsPath
            // 
            this.textBox_ShpCheckPointsPath.Location = new System.Drawing.Point(104, 12);
            this.textBox_ShpCheckPointsPath.Name = "textBox_ShpCheckPointsPath";
            this.textBox_ShpCheckPointsPath.Size = new System.Drawing.Size(251, 21);
            this.textBox_ShpCheckPointsPath.TabIndex = 0;
            // 
            // button_ShpCheckPoints
            // 
            this.button_ShpCheckPoints.Location = new System.Drawing.Point(401, 12);
            this.button_ShpCheckPoints.Name = "button_ShpCheckPoints";
            this.button_ShpCheckPoints.Size = new System.Drawing.Size(107, 23);
            this.button_ShpCheckPoints.TabIndex = 1;
            this.button_ShpCheckPoints.Text = "选择路径";
            this.button_ShpCheckPoints.UseVisualStyleBackColor = true;
            this.button_ShpCheckPoints.Click += new System.EventHandler(this.button_ShpCheckPoints_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "ShapeFile路径：";
            // 
            // button_Import
            // 
            this.button_Import.Location = new System.Drawing.Point(204, 60);
            this.button_Import.Name = "button_Import";
            this.button_Import.Size = new System.Drawing.Size(75, 23);
            this.button_Import.TabIndex = 3;
            this.button_Import.Text = "好，导入";
            this.button_Import.UseVisualStyleBackColor = true;
            this.button_Import.Click += new System.EventHandler(this.button_Import_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(401, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "设置数据库连接";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(401, 70);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(107, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "高级";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FrmImportShapeFile2PostGIS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 95);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button_Import);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_ShpCheckPoints);
            this.Controls.Add(this.textBox_ShpCheckPointsPath);
            this.Name = "FrmImportShapeFile2PostGIS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "导入ShapeFile文件";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_ShpCheckPointsPath;
        private System.Windows.Forms.Button button_ShpCheckPoints;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_Import;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}
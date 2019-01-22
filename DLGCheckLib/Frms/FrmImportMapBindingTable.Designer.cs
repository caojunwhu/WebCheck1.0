namespace DLGCheckLib.Frms
{
    partial class FrmImportMapBindingTable
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
            this.label1 = new System.Windows.Forms.Label();
            this.tb_mapbindingtable = new System.Windows.Forms.TextBox();
            this.btn_open = new System.Windows.Forms.Button();
            this.tb_import = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "结合表路径：";
            // 
            // tb_mapbindingtable
            // 
            this.tb_mapbindingtable.Location = new System.Drawing.Point(122, 12);
            this.tb_mapbindingtable.Name = "tb_mapbindingtable";
            this.tb_mapbindingtable.Size = new System.Drawing.Size(286, 21);
            this.tb_mapbindingtable.TabIndex = 1;
            // 
            // btn_open
            // 
            this.btn_open.Location = new System.Drawing.Point(418, 10);
            this.btn_open.Name = "btn_open";
            this.btn_open.Size = new System.Drawing.Size(75, 23);
            this.btn_open.TabIndex = 2;
            this.btn_open.Text = "浏览";
            this.btn_open.UseVisualStyleBackColor = true;
            this.btn_open.Click += new System.EventHandler(this.btn_open_Click);
            // 
            // tb_import
            // 
            this.tb_import.Location = new System.Drawing.Point(221, 39);
            this.tb_import.Name = "tb_import";
            this.tb_import.Size = new System.Drawing.Size(75, 23);
            this.tb_import.TabIndex = 3;
            this.tb_import.Text = "导入";
            this.tb_import.UseVisualStyleBackColor = true;
            this.tb_import.Click += new System.EventHandler(this.tb_import_Click);
            // 
            // FrmImportMapBindingTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 73);
            this.Controls.Add(this.tb_import);
            this.Controls.Add(this.btn_open);
            this.Controls.Add(this.tb_mapbindingtable);
            this.Controls.Add(this.label1);
            this.Name = "FrmImportMapBindingTable";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmImportMapBindingTable";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_mapbindingtable;
        private System.Windows.Forms.Button btn_open;
        private System.Windows.Forms.Button tb_import;
    }
}
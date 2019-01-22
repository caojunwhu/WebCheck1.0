namespace DLGCheckLib.Frms
{
    partial class FrmProjectItemsManager
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1_delete = new System.Windows.Forms.Button();
            this.button2_export = new System.Windows.Forms.Button();
            this.button1_modify = new System.Windows.Forms.Button();
            this.checkBox2_Highlight = new System.Windows.Forms.CheckBox();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.button1_selectall = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "外业检测点",
            "抽样分区",
            "样本结合表"});
            this.comboBox1.Location = new System.Drawing.Point(124, 37);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.Text = "外业检测点";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "要素类型";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "检验项目名称";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(124, 11);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(318, 21);
            this.textBox1.TabIndex = 3;
            // 
            // button1_delete
            // 
            this.button1_delete.Location = new System.Drawing.Point(72, 349);
            this.button1_delete.Name = "button1_delete";
            this.button1_delete.Size = new System.Drawing.Size(75, 23);
            this.button1_delete.TabIndex = 5;
            this.button1_delete.Text = "删除";
            this.button1_delete.UseVisualStyleBackColor = true;
            this.button1_delete.Click += new System.EventHandler(this.button1_delete_Click);
            // 
            // button2_export
            // 
            this.button2_export.Location = new System.Drawing.Point(367, 349);
            this.button2_export.Name = "button2_export";
            this.button2_export.Size = new System.Drawing.Size(75, 23);
            this.button2_export.TabIndex = 6;
            this.button2_export.Text = "导出";
            this.button2_export.UseVisualStyleBackColor = true;
            this.button2_export.Click += new System.EventHandler(this.button2_export_Click);
            // 
            // button1_modify
            // 
            this.button1_modify.Location = new System.Drawing.Point(222, 349);
            this.button1_modify.Name = "button1_modify";
            this.button1_modify.Size = new System.Drawing.Size(75, 23);
            this.button1_modify.TabIndex = 7;
            this.button1_modify.Text = "修改";
            this.button1_modify.UseVisualStyleBackColor = true;
            this.button1_modify.Click += new System.EventHandler(this.button1_modify_Click);
            // 
            // checkBox2_Highlight
            // 
            this.checkBox2_Highlight.AutoSize = true;
            this.checkBox2_Highlight.Location = new System.Drawing.Point(16, 85);
            this.checkBox2_Highlight.Name = "checkBox2_Highlight";
            this.checkBox2_Highlight.Size = new System.Drawing.Size(96, 16);
            this.checkBox2_Highlight.TabIndex = 9;
            this.checkBox2_Highlight.Text = "高亮选中要素";
            this.checkBox2_Highlight.UseVisualStyleBackColor = true;
            this.checkBox2_Highlight.CheckedChanged += new System.EventHandler(this.checkBox2_Highlight_CheckedChanged);
            // 
            // dataGridViewX1
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewX1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewX1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewX1.EnableHeadersVisualStyles = false;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewX1.Name = "dataGridViewX1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewX1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewX1.RowTemplate.Height = 23;
            this.dataGridViewX1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewX1.Size = new System.Drawing.Size(511, 224);
            this.dataGridViewX1.TabIndex = 11;
            this.dataGridViewX1.SelectionChanged += new System.EventHandler(this.dataGridViewX1_SelectionChanged);
            // 
            // button1_selectall
            // 
            this.button1_selectall.Location = new System.Drawing.Point(182, 81);
            this.button1_selectall.Name = "button1_selectall";
            this.button1_selectall.Size = new System.Drawing.Size(75, 23);
            this.button1_selectall.TabIndex = 10;
            this.button1_selectall.Text = "全选";
            this.button1_selectall.UseVisualStyleBackColor = true;
            this.button1_selectall.Click += new System.EventHandler(this.button1_selectall_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGridViewX1);
            this.panel1.Location = new System.Drawing.Point(12, 119);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(511, 224);
            this.panel1.TabIndex = 12;
            // 
            // FrmProjectItemsManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 389);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1_selectall);
            this.Controls.Add(this.checkBox2_Highlight);
            this.Controls.Add(this.button1_modify);
            this.Controls.Add(this.button2_export);
            this.Controls.Add(this.button1_delete);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.MaximizeBox = false;
            this.Name = "FrmProjectItemsManager";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "检验项目要素管理";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmProjectItemsManager_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1_delete;
        private System.Windows.Forms.Button button2_export;
        private System.Windows.Forms.Button button1_modify;
        private System.Windows.Forms.CheckBox checkBox2_Highlight;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.Button button1_selectall;
        private System.Windows.Forms.Panel panel1;
    }
}
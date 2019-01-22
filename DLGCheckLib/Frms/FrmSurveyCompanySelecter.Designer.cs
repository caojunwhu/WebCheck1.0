namespace DLGCheckLib.Frms
{
    partial class FrmSurveyCompanySelecter
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
            this.cmb_province = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmb_city = new System.Windows.Forms.ComboBox();
            this.cklst_company = new System.Windows.Forms.CheckedListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmb_county = new System.Windows.Forms.ComboBox();
            this.rtb_companydetial = new System.Windows.Forms.RichTextBox();
            this.btn_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "省份：";
            // 
            // cmb_province
            // 
            this.cmb_province.FormattingEnabled = true;
            this.cmb_province.Location = new System.Drawing.Point(74, 20);
            this.cmb_province.Name = "cmb_province";
            this.cmb_province.Size = new System.Drawing.Size(112, 20);
            this.cmb_province.TabIndex = 1;
            this.cmb_province.SelectedIndexChanged += new System.EventHandler(this.cmb_province_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(220, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "地市：";
            // 
            // cmb_city
            // 
            this.cmb_city.FormattingEnabled = true;
            this.cmb_city.Location = new System.Drawing.Point(267, 20);
            this.cmb_city.Name = "cmb_city";
            this.cmb_city.Size = new System.Drawing.Size(115, 20);
            this.cmb_city.TabIndex = 3;
            this.cmb_city.SelectedIndexChanged += new System.EventHandler(this.cmb_city_SelectedIndexChanged);
            // 
            // cklst_company
            // 
            this.cklst_company.FormattingEnabled = true;
            this.cklst_company.Location = new System.Drawing.Point(12, 58);
            this.cklst_company.Name = "cklst_company";
            this.cklst_company.Size = new System.Drawing.Size(277, 308);
            this.cklst_company.TabIndex = 4;
            this.cklst_company.Click += new System.EventHandler(this.cklst_company_Click);
            this.cklst_company.SelectedIndexChanged += new System.EventHandler(this.cklst_company_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(413, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "区县：";
            // 
            // cmb_county
            // 
            this.cmb_county.FormattingEnabled = true;
            this.cmb_county.Location = new System.Drawing.Point(460, 23);
            this.cmb_county.Name = "cmb_county";
            this.cmb_county.Size = new System.Drawing.Size(119, 20);
            this.cmb_county.TabIndex = 3;
            this.cmb_county.SelectedIndexChanged += new System.EventHandler(this.cmb_county_SelectedIndexChanged);
            // 
            // rtb_companydetial
            // 
            this.rtb_companydetial.Location = new System.Drawing.Point(315, 58);
            this.rtb_companydetial.Name = "rtb_companydetial";
            this.rtb_companydetial.Size = new System.Drawing.Size(264, 308);
            this.rtb_companydetial.TabIndex = 5;
            this.rtb_companydetial.Text = "";
            // 
            // btn_ok
            // 
            this.btn_ok.Location = new System.Drawing.Point(267, 394);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(75, 23);
            this.btn_ok.TabIndex = 6;
            this.btn_ok.Text = "选好了";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // FrmSurveyCompanySelecter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 429);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.rtb_companydetial);
            this.Controls.Add(this.cklst_company);
            this.Controls.Add(this.cmb_county);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmb_city);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmb_province);
            this.Controls.Add(this.label1);
            this.Name = "FrmSurveyCompanySelecter";
            this.Text = "选择一个测绘单位";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmb_province;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_city;
        private System.Windows.Forms.CheckedListBox cklst_company;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmb_county;
        private System.Windows.Forms.RichTextBox rtb_companydetial;
        private System.Windows.Forms.Button btn_ok;
    }
}
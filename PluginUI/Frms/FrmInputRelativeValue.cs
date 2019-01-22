using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginUI.Frms
{
    public partial class FrmInputRelativeValue : Form
    {
        public FrmInputRelativeValue()
        {
            InitializeComponent();
        }

        string sRelativeValue;

        public string SRelativeValue
        {
            get
            {
                return sRelativeValue;
            }

            set
            {
                sRelativeValue = value;
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            SRelativeValue = textBox1.Text;
            if (SRelativeValue == "")
            {
                MessageBox.Show("请输入间距边长测量值！");
                return;
            }

            this.Close();
            this.DialogResult = DialogResult.OK;
                

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar==13)
            {
                SRelativeValue = textBox1.Text;
                if (SRelativeValue == "")
                {
                    MessageBox.Show("请输入间距边长测量值！");
                    return;
                }

                this.Close();
                this.DialogResult = DialogResult.OK;

            }
        }
    }
}

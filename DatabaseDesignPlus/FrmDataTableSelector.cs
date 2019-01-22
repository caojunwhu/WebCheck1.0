using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DatabaseDesignPlus
{
    public partial class FrmDataTableSelector : Form
    {
        public string SelectedTableName { set; get; }
        IDatabaseReaderWriter _reader = null;
        public FrmDataTableSelector(IDatabaseReaderWriter ireader)
        {
            InitializeComponent();
            _reader = ireader;
            List<string> TableNames = _reader.GetSchameDataTableNames();
            DatabaseReaderWriterFactory.FillCombox(TableNames, comboBox1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedTableName = comboBox1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(SelectedTableName==""|| SelectedTableName==null)
            {
                MessageBox.Show("请选择表!");
                    return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

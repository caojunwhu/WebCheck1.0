using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLGCheckLib.Frms
{
    public partial class FrmSpatialReferenceSetting : Form
    {
        string spatialreferencewkt;
        DLGCheckCoordinateSystem coordsys;
        public FrmSpatialReferenceSetting()
        {
            InitializeComponent();

            coordsys = new DLGCheckCoordinateSystem();
            dataGridViewX1.DataSource = coordsys.spatialreferences;
            foreach(string geogcs in coordsys.geogcsnames)
            {
                toolStripComboBox1.Items.Add(geogcs);
            }
            toolStripComboBox1.SelectedIndex = 0;
        }

        public string Spatialreferencewkt
        {
            get
            {
                return spatialreferencewkt;
            }

            set
            {
                spatialreferencewkt = value;
            }
        }

        //双击选择行，关闭窗口，返回坐标系统文本
        private void dataGridViewX1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Spatialreferencewkt = dataGridViewX1.CurrentRow.Cells[3].Value.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string geogcs = toolStripComboBox1.SelectedItem as string;
            List<DLGCheckSpatialReference> sparef = new List<DLGCheckSpatialReference>();
            foreach(DLGCheckSpatialReference srf in coordsys.spatialreferences)
            {
                if(srf.geogcs == geogcs)
                {
                    sparef.Add(srf);
                }
            }
            dataGridViewX1.DataSource = sparef;
        }
    }
}

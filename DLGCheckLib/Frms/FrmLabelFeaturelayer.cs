using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using PluginUI;
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
    public partial class FrmLabelFeaturelayer : Form
    {
        IFeatureLayer iFeatureLayer;
        public FrmLabelFeaturelayer()
        {
            InitializeComponent();
        }
        public FrmLabelFeaturelayer(IFeatureLayer pLabelLayer)
        {
            InitializeComponent();
            iFeatureLayer = pLabelLayer;
            if (iFeatureLayer != null)
            {
                IFeatureClass pFeatureClass = iFeatureLayer.FeatureClass;
                textBox1.Text = pFeatureClass.AliasName;
                IFields pFields = pFeatureClass.Fields;
                for (int j = pFields.FieldCount-1; j >=0 ; j--)
                {
                    //string layervalue = pFeatureClass.
                    comboBox1.Items.Add(pFields.Field[j].Name);
                }
                comboBox1.SelectedIndex = 0;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int size = Convert.ToInt32(textBox2.Text);
            string colorName = colorDropDownList1.Text;
            string field = comboBox1.Text;
            Color color = Color.FromName(colorName);

            if (field == "" || size <= 0) return;
            IRgbColor colorLabel = new RgbColorClass();
            colorLabel.Blue = color.B;
            colorLabel.Red = color.R;
            colorLabel.Green = color.G;

            ArcGISHelper.EnableFeatureLayerLabel(iFeatureLayer, field, colorLabel, size, null);
            this.DialogResult = DialogResult.OK;
        }
    }
}

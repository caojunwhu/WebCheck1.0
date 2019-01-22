using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace DLGCheckLib.Frms
{
    public partial class FrmElevationSetting : Form
    {
        public FrmElevationSetting()
        {
            InitializeComponent();
        }
        string layer = "";
        string layername = "";
        string elevationFeild = "";

        public string Layer
        {
            get
            {
                return layer;
            }

            set
            {
                layer = value;
            }
        }

        public string Layername
        {
            get
            {
                return layername;
            }

            set
            {
                layername = value;
            }
        }

        public string ElevationFeild
        {
            get
            {
                return elevationFeild;
            }

            set
            {
                elevationFeild = value;
            }
        }

        public FrmElevationSetting(ESRI.ArcGIS.Carto.IMap map,ElevationSearchSetting elevsetting)
        {
            InitializeComponent();

            //取值时不做检查，默认是正常的配置
            comboBox1.Text = elevsetting.LayerName;
            comboBox2.Text = elevsetting.Layer;
            comboBox3.Text = elevsetting.ElevationField;

            int nPointLayerindex = -1;
            int nLayerCount = map.LayerCount;
            for (int i = 0; i < nLayerCount; i++)
            {
                string LayerName = map.Layer[i].Name;

                IFeatureLayer pFeatureLayer = map.Layer[i] as IFeatureLayer;
                if(pFeatureLayer.Name.IndexOf("Point")>=0)
                {
                   comboBox1.Items.Add(pFeatureLayer.Name);
                    nPointLayerindex = i;
                }               
            }
            if(comboBox1.Items.Count<=0)
            {
                MessageBox.Show("无有效Point图层，请检查样本是否正确加载！");
                this.Close();
            }
            else
            {
                comboBox1.SelectedIndex = 0;
                IFeatureLayer pFeatureLayer = map.Layer[nPointLayerindex] as IFeatureLayer;
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                IFields pFields = pFeatureClass.Fields;

                string layerfield = "layer";
                int layerindex = pFields.FindField(layerfield);
                List<string> layerArray = new List<string>();

                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = "";
                IFeatureCursor cursor = pFeatureClass.Search(filter, false);
                IFeature feature = cursor.NextFeature();

                while(feature!=null)
                {
                    string layername = "";
                    layername = feature.Value[layerindex] as string;

                    if(layerArray.Contains(layername)==false)
                    {
                        layerArray.Add(layername);
                        comboBox2.Items.Add(layername);
                    }
                    feature = cursor.NextFeature();
                }


                for (int j=0;j<pFields.FieldCount;j++)
                {
                    //string layervalue = pFeatureClass.
                    comboBox3.Items.Add(pFields.Field[j].Name);
                }
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Layername = comboBox1.Text;
            Layer = comboBox2.Text;
            ElevationFeild = comboBox3.Text;
            if(Layer=="" || Layername=="" || ElevationFeild=="")
            {
                MessageBox.Show("参数未设置！");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

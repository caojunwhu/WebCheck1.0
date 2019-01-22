using Authentication.Class;
using Core;
using DatabaseDesignPlus;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PluginUI
{
    public partial class FrmOpenPGLayer : Form
    {
        private string pgconnectionstring;
        AxMapControl _Map;
        public FrmOpenPGLayer(Dictionary<string, string> configs, Dictionary<string, string> databases,  UserObject oLoginUser, AxMapControl map)
        {
            InitializeComponent();

            _Map = map;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "" ||
                textBox3.Text == "" ||
                textBox4.Text == "" ||
                textBox5.Text == "")
            {
                MessageBox.Show("填写相关信息");
                return;
            }
            //Server=localhost;Port=5432;Database=SurveryProductCheckDatabase;uid=postgres;password=123;
            pgconnectionstring = string.Format("host={0} port=5432 dbname={1} user={2} password={3} ", textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text);

            IFeatureClass featClass = ArcGISHelper.CreateMemoryFeatureClassFromPostGIS(pgconnectionstring, tb_LayerName.Text);

            IFeatureLayer featlayer = new FeatureLayerClass();
            featlayer.FeatureClass = featClass;
            featlayer.Name = tb_LayerName.Text;

            _Map.AddLayer(featlayer);


            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void tb_LayerName_MouseClick(object sender, MouseEventArgs e)
        {
            if (textBox2.Text == "" ||
                textBox3.Text == "" ||
                textBox4.Text == "" ||
                textBox5.Text == "")
            {
                MessageBox.Show("填写相关信息");
                return;
            }
            tb_LayerName.Items.Clear();

            string pgconstr = string.Format("server={0};port=5432;database={1};user={2};password={3}", textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text);
            DatabaseDesignPlus.ClsPostgreSql cpgsql = new DatabaseDesignPlus.ClsPostgreSql(pgconstr);
            List<string> tablenames = cpgsql.GetSchameDataTableNames(" where tablename like '%cgcs%'");
            foreach(string tablename in tablenames)
            {
                List<FieldDesign> fields = cpgsql.GetFieldDesign(tablename);
                foreach(FieldDesign field in fields)
                {
                    if(field.FieldName.IndexOf("geometry")>0)
                    {
                        tb_LayerName.Items.Add(tablename);
                    }
                }
            }

        }
    }
}

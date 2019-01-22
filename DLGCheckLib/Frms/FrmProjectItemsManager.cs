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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLGCheckLib.Frms
{
    public partial class FrmProjectItemsManager : Form
    {
        DLGCheckProjectClass localProject;
        string ItemType;
        IFeatureLayer iFeatureLayer;
        AxMapControl localMapControl;
        List<int> selectOIDs = new List<int>();
        string Layername;
        public FrmProjectItemsManager(DLGCheckProjectClass dlgcheckproject,AxMapControl axMapControl)
        {
            InitializeComponent();
            localProject = dlgcheckproject;
            textBox1.Text = localProject.ProjectName;
            ItemType = comboBox1.Text;
            localMapControl = axMapControl;

            LoadProjectItem(ItemType);
        }

        public void LoadProjectItem(string itemType)
        {
            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);
            DataTable datatable=null;
            switch (itemType)
            {
                case "外业检测点":
                    {
                        string sqlscater = string.Format("select  ogc_fid,ptid,sx,sy,sz from scater where projectid = '{0}' order by sx,sy ", localProject.ProjectID);
                        datatable = dbread.GetDataTableBySQL(sqlscater);
                        //superGridControl1.PrimaryGrid.DataSource = datatable;
                        Layername = "scater";
                        iFeatureLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(Layername, localMapControl.Map);

                    }
                    break;
                case "抽样分区":
                    {
                        string sqlsamplearea = string.Format("select ogc_fid,samplearea from samplearea where projectid = '{0}' order by samplearea ", localProject.ProjectID);
                        datatable = dbread.GetDataTableBySQL(sqlsamplearea);
                        //superGridControl1.PrimaryGrid.DataSource = null;
                        //superGridControl1.PrimaryGrid.DataSource = datatable;
                        Layername = "samplearea";
                        iFeatureLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(Layername, localMapControl.Map);


                    }
                    break;
                case "样本结合表":
                    {
                        string sqlmapbindingtable = string.Format("select ogc_fid, mapnumber from mapbindingtable where projectid = '{0}' order by mapnumber ", localProject.ProjectID);
                        datatable = dbread.GetDataTableBySQL(sqlmapbindingtable);
                        //superGridControl1.PrimaryGrid.DataSource = null;
                        //superGridControl1.PrimaryGrid.DataSource = datatable;
                        Layername = "mapbindingtable";
                        iFeatureLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(Layername, localMapControl.Map);


                    }
                    break;
            }
            dataGridViewX1.DataSource = datatable;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ItemType = comboBox1.Text;
            LoadProjectItem(ItemType);
        }

        private void button1_selectall_Click(object sender, EventArgs e)
        {
            dataGridViewX1.SelectAll();
        }

        void SelectItems()
        {
            GetOIDs();
            IFeatureLayer selfeatlayer = PluginUI.ArcGISHelper.GetFeatureLayerByName("SelectedFeatures",localMapControl.Map);

            if (checkBox2_Highlight.Checked==true)
            {
                if (selfeatlayer != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(selfeatlayer.FeatureClass);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(selfeatlayer);
                }
                selfeatlayer = null;

                string pgdatabase = System.Configuration.ConfigurationManager.AppSettings["PGDatabase"]; ;
                pgdatabase = DataBaseConfigs.RePlaceConfig(pgdatabase);
                selfeatlayer = new FeatureLayerClass();
                selfeatlayer.Name = "SelectedFeatures";
                selfeatlayer.FeatureClass = PluginUI.ArcGISHelper.CreateMemoryFeatureClassFromPostGIS(pgdatabase, Layername, "projectid='" + localProject.ProjectID + "'", localProject.SrText, selectOIDs);
                localMapControl.Map.AddLayer(selfeatlayer);
                IFeatureSelection pFeatureSelection = selfeatlayer as IFeatureSelection;

                for (int i=1;i<=selfeatlayer.FeatureClass.FeatureCount(null);i++)
                {
                    IFeature pFeature = iFeatureLayer.FeatureClass.GetFeature(i);
                    pFeatureSelection.Add(pFeature);
                }
            }
            else
            {
                if(selfeatlayer!=null)
                {
                    IFeatureSelection pFeatureSelection = selfeatlayer as IFeatureSelection;
                    pFeatureSelection.Clear();
                    localMapControl.Map.DeleteLayer(selfeatlayer);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(selfeatlayer.FeatureClass);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(selfeatlayer);

                }

            }
            localMapControl.Refresh();

        }
        private void checkBox2_Highlight_CheckedChanged(object sender, EventArgs e)
        {
            SelectItems();
        }
        private void GetOIDs()
        {
            selectOIDs.Clear();
            foreach(DataGridViewRow dr in dataGridViewX1.SelectedRows)
            {
                selectOIDs.Add(Convert.ToInt32(dr.Cells["ogc_fid"].Value));
            }       
        }

        private void FrmProjectItemsManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            IFeatureLayer selfeatlayer = PluginUI.ArcGISHelper.GetFeatureLayerByName("SelectedFeatures", localMapControl.Map);
            if (selfeatlayer != null)
            {
                IFeatureSelection pFeatureSelection = selfeatlayer as IFeatureSelection;
                pFeatureSelection.Clear();
                localMapControl.Map.DeleteLayer(selfeatlayer);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(selfeatlayer.FeatureClass);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(selfeatlayer);

            }
            localMapControl.Refresh();

        }

        private void dataGridViewX1_SelectionChanged(object sender, EventArgs e)
        {
            //SelectItems();
        }

        private void button1_delete_Click(object sender, EventArgs e)
        {
            GetOIDs();

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);
            foreach (int id in selectOIDs)
            {
                string sqlscater = string.Format("delete  from {0} where projectid = '{1}'  and ogc_fid={2}", dbread.GetTableName(Layername), localProject.ProjectID, id);
                dbread.ExecuteSQL(sqlscater);
            }

            foreach (DataGridViewRow dr in dataGridViewX1.SelectedRows)
            {
                dataGridViewX1.Rows.Remove(dr);
            }
            
            string message = string.Format("成功删除{0}{1}个！", Layername, selectOIDs.Count);
            MessageBox.Show(message);
            selectOIDs.Clear();
        }

        private void button2_export_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            //SaveFileDialog dlg = new SaveFileDialog();
            if (DialogResult.OK == dlg.ShowDialog())
            {
                //string file = dlg.FileName.Substring(0, dlg.FileName.LastIndexOf('\\'));
                string folder = dlg.SelectedPath;
                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                }
                try
                {
                    GetOIDs();
                    string pgdatabase = System.Configuration.ConfigurationManager.AppSettings["PGDatabase"]; ;
                    pgdatabase = DataBaseConfigs.RePlaceConfig(pgdatabase);
                    IFeatureLayer outputfeatlayer = new FeatureLayerClass();
                    outputfeatlayer.Name = Layername;
                    outputfeatlayer.FeatureClass = PluginUI.ArcGISHelper.CreateMemoryFeatureClassFromPostGIS(pgdatabase, Layername, "projectid='" + localProject.ProjectID + "'", localProject.SrText, selectOIDs);
                    PluginUI.ArcGISHelper.ExportFeature(outputfeatlayer.FeatureClass, folder);
                    string message = string.Format("成功导出{0}{1}个！", Layername, selectOIDs.Count);
                    MessageBox.Show(message);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("导出失败！" + ex.Message);
                }
            }
        }

        private void button1_modify_Click(object sender, EventArgs e)
        {
            MessageBox.Show("敬请期待……");
        }
    }
}

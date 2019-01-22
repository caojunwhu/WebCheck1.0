using DatabaseDesignPlus;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
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
    public partial class FrmSetMapBindSearchParameters : Form
    {
        string localprojectid = "";
        string localcurrentuser = "";
        IMap localMap = null;
        IFeatureSelection pFeatureSelection = null;
        string localszMapnumber = "";
        MapSampleItemSetting mapsampleitem;

        public MapSampleItemSetting Mapsampleitem
        {
            get
            {
                return mapsampleitem;
            }

            set
            {
                mapsampleitem = value;
            }
        }

        public FrmSetMapBindSearchParameters(ESRI.ArcGIS.Carto.IMap map, string projectid, string mapnumber, string curentuser)
        {
            InitializeComponent();
            localprojectid = projectid;
            localMap = map;
            localcurrentuser = curentuser;
            localszMapnumber = mapnumber;
            InitailizeForm(localszMapnumber);
        }

        private void InitailizeForm(string szMapnumber)
        {
            DLGCheckLib.DLGCheckProjectClass localproject = new DLGCheckProjectClass(localprojectid, localcurrentuser);
            localproject.ReadSampleSetting(localproject.ProjectID);

            MapSampleItemSetting mapsampleitem = localproject.GetMapSampleItemSetting(szMapnumber);

            if (mapsampleitem != null)
            {
                cmb_mapnumber.Text = mapsampleitem.MapNumber;
                tb_sampleareaid.Text = Convert.ToString(mapsampleitem.SampleAreaIndex);
                tb_sampleserial.Text = Convert.ToString(mapsampleitem.SampleSerial);
                cmb_mapsacle.Text = Convert.ToString(localproject.MapScale);
                cmb_terrian.Text = mapsampleitem.Terrian;
                cmb_checktype.Text = mapsampleitem.CheckType;
                tb_plainmaxerror.Text = Convert.ToString(mapsampleitem.PErrorMax);
                tb_relativemaxerror.Text = Convert.ToString(mapsampleitem.RErrorMax);
                tb_heightmaxerror.Text = Convert.ToString(mapsampleitem.HErrorMax);

            }
            // for mapbindingtable item

            string gisdatabase = DataBaseConfigs.GetGISDbDatasource(DataBaseConfigs.DatabaseEngineType);
            IFeatureLayer selfeatlayer = new FeatureLayerClass();
            selfeatlayer.Name = szMapnumber + "mapbound";
            string Layername = "mapbindingtable";
            string whereclause = string.Format("projectid='{0}' and mapnumber='{1}'", localproject.ProjectID, szMapnumber);
            selfeatlayer.FeatureClass = PluginUI.ArcGISHelper.CreateMemoryFeatureClassFromPostGIS(gisdatabase, Layername, whereclause, localproject.SrText);
            localMap.AddLayer(selfeatlayer);
            pFeatureSelection = selfeatlayer as IFeatureSelection;

            OSGeo.OGR.Feature pFeature = PluginUI.ArcGISHelper.GetFeatureFromPostGIS(gisdatabase, Layername, whereclause);
            //IFeature pFeature = PluginUI.ArcGISHelper.GetFeature(selfeatlayer, "mapnumber", szMapnumber);
            //string wkt = Utils.Converters.ConvertGeometryToWKT(pFeature.ShapeCopy); ;
            //tb_mapbindareacoord.Text = wkt;

            string wkt = "";
            if (pFeature != null) pFeature.GetGeometryRef().ExportToWkt(out wkt);
            tb_mapbindareacoord.Text = wkt;

            ////////////////////load mapnumber selection choices from the annotation layers and the value from the Refname field;
            //cmb_annolayer.SelectedIndex = 0;
            IFeatureLayer pFeatureLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName("Annotation", localMap) as IFeatureLayer;
            if (pFeatureLayer != null)
            {
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                IFields pFields = pFeatureClass.Fields;

                string layerfield = "layer";
                int layerindex = pFields.FindField(layerfield);
                List<string> layerArray = new List<string>();

                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = "";
                IFeatureCursor cursor = pFeatureClass.Search(filter, false);
                IFeature feature = cursor.NextFeature();

                while (feature != null)
                {
                    string layername = "";
                    layername = feature.Value[layerindex] as string;

                    if (layerArray.Contains(layername) == false)
                    {
                        layerArray.Add(layername);
                        cmb_annolayer.Items.Add(layername);
                    }
                    feature = cursor.NextFeature();
                }
            }

            //////////////////////load layer fom map where the sample is 

        }

        private void FrmSearchSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            string sMapbindLayerName = localszMapnumber + "mapbound";

            IFeatureLayer sampleMapbindLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(sMapbindLayerName, localMap);
            while ((sampleMapbindLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(sMapbindLayerName, localMap)) != null)
            {
                localMap.DeleteLayer(sampleMapbindLayer);

            }
            this.DialogResult = DialogResult.OK;
        }

        private void cmb_annolayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_mapnumber.Items.Clear();
            IFeatureLayer pFeatureLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName("Annotation", localMap) as IFeatureLayer;
            if (pFeatureLayer != null)
            {
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                IFields pFields = pFeatureClass.Fields;

                string RefNamefield = "RefName";
                int RefNameindex = pFields.FindField(RefNamefield);
                List<string> layerArray = new List<string>();

                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = string.Format("layer = '{0}'", cmb_annolayer.Text);
                IFeatureCursor cursor = pFeatureClass.Search(filter, false);
                IFeature feature = cursor.NextFeature();

                while (feature != null)
                {
                    string RefName = "";
                    RefName = feature.Value[RefNameindex] as string;

                    if (layerArray.Contains(RefName) == false)
                    {
                        layerArray.Add(RefName);
                        cmb_mapnumber.Items.Add(RefName);
                    }
                    feature = cursor.NextFeature();
                }
            }

        }

        private void cmb_featurelayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_featurelayerlayer.Items.Clear();
            int nLayerindex = cmb_featurelayer.SelectedIndex;
            int nLayerCount = localMap.LayerCount;
            string LayerName = cmb_featurelayer.Text;

            IFeatureLayer pFeatureLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(LayerName, localMap);
            if (pFeatureLayer != null)
            {
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                IFields pFields = pFeatureClass.Fields;

                string layerfield = "layer";
                int layerindex = pFields.FindField(layerfield);
                List<string> layerArray = new List<string>();

                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = "";
                IFeatureCursor cursor = pFeatureClass.Search(filter, false);
                IFeature feature = cursor.NextFeature();

                while (feature != null)
                {
                    string layername = "";
                    layername = feature.Value[layerindex] as string;

                    if (layerArray.Contains(layername) == false)
                    {
                        layerArray.Add(layername);
                        cmb_featurelayerlayer.Items.Add(layername);
                    }
                    feature = cursor.NextFeature();
                }
            }
        }
        // if selection change is valid ,this will triger the selection features into selection and compute a boundingbox for sample mapbindingtable 
        private void cmb_featurelayerlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                string LayerName = cmb_featurelayer.Text;
                IFeatureLayer pFeatureLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(LayerName, localMap) as IFeatureLayer;
                if (pFeatureLayer != null)
                {
                    IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                    IFields pFields = pFeatureClass.Fields;

                    string LayerNamefield = "layer";
                    int LayerNameindex = pFields.FindField(LayerNamefield);
                    List<string> layerArray = new List<string>();

                    IQueryFilter filter = new QueryFilterClass();
                    filter.WhereClause = string.Format("layer = '{0}'", cmb_featurelayerlayer.Text);
                    IFeatureCursor cursor = pFeatureClass.Search(filter, false);
                    IFeature feature = cursor.NextFeature();
                    IEnvelope eSampleEnvelop = new EnvelopeClass();
                    eSampleEnvelop = feature.Extent;
                    IFeatureSelection pFeatureSelection = pFeatureLayer as IFeatureSelection;
                    pFeatureSelection.Clear();
                    while (feature != null)
                    {
                        pFeatureSelection.Add(feature);
                        feature = cursor.NextFeature();
                        if (feature != null) eSampleEnvelop.Union(feature.Extent);
                    }
                    tb_mapbindareacoord.Text = string.Format("POLYGON(({0} {1},{2} {3},{4} {5},{6} {7},{8} {9}))",
                        eSampleEnvelop.LowerRight.X, eSampleEnvelop.LowerRight.Y,
                        eSampleEnvelop.LowerLeft.X, eSampleEnvelop.LowerLeft.Y,
                        eSampleEnvelop.UpperLeft.X, eSampleEnvelop.UpperLeft.Y,
                        eSampleEnvelop.UpperRight.X, eSampleEnvelop.UpperRight.Y,
                        eSampleEnvelop.LowerRight.X, eSampleEnvelop.LowerRight.Y
                        );

                    //POLYGON ((378750.0 3578499.99995878,378500.0 3578499.99995878,378500.0 3578749.99995878,378750.0 3578749.99995878,378750.0 3578499.99995878))
                    //POLYGON((378500,3578750.490866),(378750,3578500))
                    OSGeo.OGR.Geometry pGeometry = OSGeo.OGR.Geometry.CreateFromWkt(tb_mapbindareacoord.Text);
                    IGeometry pArcGeometry = Utils.Converters.ConvertWKTToGeometry(tb_mapbindareacoord.Text);

                    string sMapbindLayerName = localszMapnumber + "mapbound";
                    IFeatureClass mapbindfeautreclass = null;
                    IFeatureLayer mapbindlayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(sMapbindLayerName, localMap);
                    if (mapbindlayer == null)
                    {
                        DLGCheckLib.DLGCheckProjectClass localproject = new DLGCheckProjectClass(localprojectid, localcurrentuser);
                        mapbindfeautreclass = PluginUI.ArcGISHelper.CreateMemoryFeatureFromGeometry(sMapbindLayerName, localproject.SrText);
                        mapbindlayer = new FeatureLayerClass();
                        mapbindlayer.Name = mapbindfeautreclass.AliasName;
                        mapbindlayer.FeatureClass = mapbindfeautreclass;
                        IFeature pFeature = mapbindfeautreclass.CreateFeature();
                        pFeature.Shape = pArcGeometry;

                        localMap.AddLayer(mapbindlayer);
                    }
                    else
                    {
                        mapbindfeautreclass = mapbindlayer.FeatureClass;
                    }

                }

            }

        }

        private void reset_Click(object sender, EventArgs e)
        {
            string sMapbindLayerName = localszMapnumber + "mapbound";

            IFeatureLayer sampleMapbindLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(sMapbindLayerName, localMap);
            while ((sampleMapbindLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(sMapbindLayerName, localMap)) != null)
            {
                localMap.DeleteLayer(sampleMapbindLayer);

            }

            tb_mapbindareacoord.Text = "";
        }

        /// <summary>
        /// 确定生成了新的结合表后，替换原有的结合表并对检验参数进行更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void set_ok_Click(object sender, EventArgs e)
        {
            if (tb_mapbindareacoord.Text == "")
            {
                MessageBox.Show("请设置结合表范围！");
                return;
            }
            if (tb_sampleareaid.Text == ""||tb_sampleserial.Text==""
                ||tb_heightmaxerror.Text==""||tb_plainmaxerror.Text==""||tb_relativemaxerror.Text==""||
                cmb_mapnumber.Text==""||cmb_checktype.Text==""||cmb_mapsacle.Text==""||cmb_terrian.Text=="")
            {
                MessageBox.Show("单位成果抽样参数未填写！");
                return;
            }

            string sMapbindLayerName = localszMapnumber + "mapbound";
            string gisdatabase = DataBaseConfigs.GetGISDbDatasource(DataBaseConfigs.DatabaseEngineType);
            string Layername = "mapbindingtable";
            string whereclause = string.Format("projectid='{0}' and mapnumber='{1}'", localprojectid, localszMapnumber);

            //判断结合表是否存在
            OSGeo.OGR.Feature pFeature = PluginUI.ArcGISHelper.GetFeatureFromPostGIS(gisdatabase, Layername, whereclause);

            //1.删除原结合表后插入
            OSGeo.OGR.Feature pFeatureInsert = null;
            if (pFeature != null)
            {
                PluginUI.ArcGISHelper.DeleteOGRFeature(gisdatabase, Layername, pFeature);
                pFeatureInsert = pFeature.Clone();

            }
            else
            {
                pFeature = PluginUI.ArcGISHelper.GetFeatureFromPostGIS(gisdatabase, Layername);
                pFeatureInsert = pFeature.Clone();
            }
            //2.不存在的直接插入
            ////获取生成的结合表图形
            //使用OGR 驱动插入
            string dbconnection = System.Configuration.ConfigurationManager.AppSettings["Login"];
            dbconnection = DataBaseConfigs.RePlaceConfig(dbconnection);
            IDatabaseReaderWriter dbread = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", dbconnection);
            string maxIDsql = string.Format("select max(ogc_fid) from {0}", Layername);
            int maxID = Convert.ToInt32(dbread.GetScalar(maxIDsql));
            pFeatureInsert.SetGeometry(OSGeo.OGR.Geometry.CreateFromWkt(tb_mapbindareacoord.Text));
            pFeatureInsert.SetFID(maxID+1);
            pFeatureInsert.SetField("projectid", localprojectid);
            pFeatureInsert.SetField("mapnumber", localszMapnumber);
            PluginUI.ArcGISHelper.CreateOGRFeature(gisdatabase, Layername, pFeatureInsert);

            //更新单位成果检验参数

            mapsampleitem = new MapSampleItemSetting();
            mapsampleitem.SampleAreaIndex = Convert.ToInt32(tb_sampleareaid.Text);
            mapsampleitem.SampleSerial = Convert.ToInt32(tb_sampleserial.Text);
            mapsampleitem.MapNumber = cmb_mapnumber.Text;
            mapsampleitem.CheckType = cmb_checktype.Text;
            mapsampleitem.Terrian = cmb_terrian.Text;
            mapsampleitem.HErrorMax = Convert.ToDouble(tb_heightmaxerror.Text);
            mapsampleitem.PErrorMax = Convert.ToDouble(tb_plainmaxerror.Text);
            mapsampleitem.RErrorMax = Convert.ToDouble(tb_relativemaxerror.Text);

            DLGCheckLib.DLGCheckProjectClass localproject = new DLGCheckProjectClass(localprojectid, localcurrentuser);
            localproject.ReadSampleSetting(localproject.ProjectID);

            if(localproject.UpdateMapSampleItemSetting(mapsampleitem)==true)
            {
                MessageBox.Show("更新成功！需重新打开项目并计算以便参数生效！");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("更新失败！");
            }

        }
    }

}

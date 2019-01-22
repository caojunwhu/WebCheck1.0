using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseDesignPlus;
using DLGCheckLib;
using OSGeo.OGR;
using System.Runtime.InteropServices;
using OSGeo.GDAL;

namespace PluginUI
{
    public partial class FrmUpdateMapbindingTable : Form
    {
        [DllImport("gdal111.dll", EntryPoint = "OGR_F_GetFieldAsString")]
        public extern static System.IntPtr OGR_F_GetFieldAsString(HandleRef handle, int i);
        public DLGCheckProjectClass localCheckProject { set; get; }
        string localpgDatasourceConnectionID { set; get; }
        public FrmUpdateMapbindingTable(DLGCheckProjectClass GlobeCheckProject, string paras, string pgDatasourceConnectionID)
        {
            InitializeComponent();
            localCheckProject = GlobeCheckProject;
            localpgDatasourceConnectionID = pgDatasourceConnectionID;
        }

        private OSGeo.OGR.Feature MapbindingTableShapeToOGRFeature(Feature mapbinding)
        {
            OSGeo.OGR.FeatureDefn ofd = new OSGeo.OGR.FeatureDefn("");
            OSGeo.OGR.FieldDefn fd = null;

            fd = new OSGeo.OGR.FieldDefn("projectid", OSGeo.OGR.FieldType.OFTString);
            ofd.AddFieldDefn(fd);

            fd = new OSGeo.OGR.FieldDefn("mapnumber", OSGeo.OGR.FieldType.OFTString);
            ofd.AddFieldDefn(fd);

            OSGeo.OGR.Feature ogrFeature = new OSGeo.OGR.Feature(ofd);

            string mapnumberfield = comboBox1_Mapnumber.Text;
            string mapnumber = mapbinding.GetFieldAsString(mapnumberfield);
            //IntPtr pchar = OGR_F_GetFieldAsString(Feature.getCPtr(mapbinding), comboBox1_Mapnumber.SelectedIndex);
            //string mapnumber = Marshal.PtrToStringAnsi(pchar);

            ogrFeature.SetField(0, localCheckProject.ProjectID);
            ogrFeature.SetField(1, mapnumber);
            //此处要特别注意内存泄露，因为直接获取的是图形指针，创建新的图形是必须是拷贝一份
            ogrFeature.SetGeometryDirectly(mapbinding.GetGeometryRef().Clone());

            return ogrFeature;
        }
        private void button1_updatemapbingding_Click(object sender, EventArgs e)
        {
            ImportMapbindingTableByOgrDriver(textBox_filepath.Text);
        }
        //perfectly realize import shapefile to postgis ,this is used to import mapbindingtable,sampletable,etc.
        //the ogr driver import shapefile name is lower case,so the source shapefile filename is lower case is better.
        public void ImportMapbindingTableByOgrDriver(string sShapeFilePath)
        {
            if (sShapeFilePath == "" || sShapeFilePath == null)
                return;

            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);

            //截取文件路径
            string directory = Path.GetDirectoryName(sShapeFilePath);
            string sLayerName = Path.GetFileNameWithoutExtension(sShapeFilePath).ToLower();

            string ogrPgConnectionID = string.Format("PG:{0}", localpgDatasourceConnectionID);

            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
            //为了支持中文路径，请添加下面这句SHAPE_ENCODING;
            //UTF-8 to ISO-8859-1.
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "ASCII");

            OSGeo.OGR.Ogr.RegisterAll();
            //ShapeFile layer
            OSGeo.OGR.Driver shapefiledriver = OSGeo.OGR.Ogr.GetDriverByName("ESRI Shapefile");
            OSGeo.OGR.DataSource shapefileDatasource = shapefiledriver.Open(sShapeFilePath, 0);
            OSGeo.OGR.Layer shapefileLayer = shapefileDatasource.GetLayerByName(sLayerName);

            //PG Layer
            OSGeo.OGR.Driver pgdriver = OSGeo.OGR.Ogr.GetDriverByName("PostgreSQL");
            OSGeo.OGR.DataSource pgDatasource = pgdriver.Open(ogrPgConnectionID, 1);

            OSGeo.OGR.Layer pglayer;
            // this case is that target layer is exist,just get it 
            // warrning :ogr pgdriver support chinese is not good!
            string pgLayername = "mapbindingtable";
            pglayer = pgDatasource.GetLayerByName(pgLayername);

            if (pglayer.GetGeomType() != wkbGeometryType.wkbPolygon&& pglayer.GetGeomType() != wkbGeometryType.wkbMultiPolygon)
            {
                return;
            }

            //Import process
            Feature shapeFeature;
            shapefileLayer.ResetReading();
            int nCount = 0;
            try
            {
                shapeFeature = shapefileLayer.GetNextFeature();
                while (shapeFeature != null)
                {
                    Feature sampleareafeature = MapbindingTableShapeToOGRFeature(shapeFeature);

                    string wkt = "";
                    shapeFeature.GetGeometryRef().ExportToWkt(out wkt);

                    string select_feature = string.Format("select * from mapbindingtable where geometry_distance_centroid(geometry('{0}'),wkb_geometry)<=0.000001 and projectid = '{1}'", wkt, localCheckProject.ProjectID);
                    DataTable datatable = datareadwrite.GetDataTableBySQL(select_feature);

                    if (datatable.Rows.Count == 0)
                    {
                        string select_maxid = string.Format("select max(ogc_fid) from mapbindingtable ");
                        int maxid = Convert.ToInt32(datareadwrite.GetScalar(select_maxid));

                        int fid = maxid + 1;
                        sampleareafeature.SetFID(fid);
                        pglayer.CreateFeature(sampleareafeature);
                        nCount += 1;
                    }

                    shapeFeature = shapefileLayer.GetNextFeature();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show(string.Format("成功导入{0}个结合表！", nCount));
        }

        private void btn_BroswerMapbindingTableFile_Click(object sender, EventArgs e)
        {
            //获取当前路径和文件名
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "ESRI ShapeFile(*.shp)|*.shp|All Files(*.*)|*.*";
            dlg.Title = "Open ESRI ShapeFile";
            if(dlg.ShowDialog()==DialogResult.OK)
            {
                string strFullPath = dlg.FileName;
                if (strFullPath == "") return;
                textBox_filepath.Text = strFullPath;

                LoadFieldNames();
            }
        }

        void LoadFieldNames()
        {
            string sShapeFilePath = textBox_filepath.Text;
            if (sShapeFilePath == "")
                return;

            //截取文件路径
            string directory = Path.GetDirectoryName(sShapeFilePath);
            string sLayerName = Path.GetFileNameWithoutExtension(sShapeFilePath).ToLower();

            string ogrPgConnectionID = string.Format("PG:{0}", localpgDatasourceConnectionID);

            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
            //为了支持中文路径，请添加下面这句SHAPE_ENCODING;
            //UTF-8 to ISO-8859-1.
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");

            OSGeo.OGR.Ogr.RegisterAll();
            //ShapeFile layer
            OSGeo.OGR.Driver shapefiledriver = OSGeo.OGR.Ogr.GetDriverByName("ESRI Shapefile");
            OSGeo.OGR.DataSource shapefileDatasource = shapefiledriver.Open(sShapeFilePath, 0);
            OSGeo.OGR.Layer shapefileLayer = shapefileDatasource.GetLayerByName(sLayerName);

            if(shapefileLayer.GetGeomType()!=wkbGeometryType.wkbMultiPolygon&&shapefileLayer.GetGeomType() != wkbGeometryType.wkbPolygon&& 
                shapefileLayer.GetGeomType() != wkbGeometryType.wkbPolygon25D&& shapefileLayer.GetGeomType() != wkbGeometryType.wkbMultiPolygon25D)
            {
                MessageBox.Show("非多边形！");
                return;
            }

            FeatureDefn featurefdn = shapefileLayer.GetLayerDefn();
            if (comboBox1_Mapnumber.Items.Count > 0) comboBox1_Mapnumber.Items.Clear();

            for(int i=0; i< featurefdn.GetFieldCount();i++)
            {
                FieldDefn fdn = featurefdn.GetFieldDefn(i);
                comboBox1_Mapnumber.Items.Add(fdn.GetName());
            }

        }

        void MapbindingPreview()
        {
            string sShapeFilePath = textBox_filepath.Text;
            if (sShapeFilePath == "" || comboBox1_Mapnumber.Text=="")
                return;


            //截取文件路径
            string directory = Path.GetDirectoryName(sShapeFilePath);
            string sLayerName = Path.GetFileNameWithoutExtension(sShapeFilePath).ToLower();

            string ogrPgConnectionID = string.Format("PG:{0}", localpgDatasourceConnectionID);

            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
            //为了支持中文路径，请添加下面这句SHAPE_ENCODING;
            //UTF-8 to ISO-8859-1.
            //OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "gb2312");
            Gdal.SetConfigOption("SHAPE_ENCODING", "");

            // 为了支持中文路径，请添加下面这句代码
            //OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
            // 为了使属性表字段支持中文，请添加下面这句
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");

            OSGeo.OGR.Ogr.RegisterAll();
            //ShapeFile layer
            //OSGeo.OGR.Driver shapefiledriver = OSGeo.OGR.Ogr.GetDriverByName("ESRI Shapefile");
            //OSGeo.OGR.DataSource shapefileDatasource = shapefiledriver.Open(sShapeFilePath, 0);
            OSGeo.OGR.DataSource shapefileDatasource = Ogr.Open(sShapeFilePath, 0);
            OSGeo.OGR.Layer shapefileLayer = shapefileDatasource.GetLayerByName(sLayerName);

            string mapnumberfieldname = comboBox1_Mapnumber.Text;
            List<Mapbinding> MapBindingList = new List<Mapbinding>();
            Feature feature;
            while((feature=shapefileLayer.GetNextFeature())!=null)
            {
                Mapbinding mapbind = new Mapbinding();
                //mapbind.sMapnumber = feature.GetFieldAsString(mapnumberfieldname);
                mapbind.sMapnumber = feature.GetFieldAsString(mapnumberfieldname);
                //IntPtr pchar = OGR_F_GetFieldAsString(Feature.getCPtr(feature), comboBox1_Mapnumber.SelectedIndex);
                //mapbind.sMapnumber = Marshal.PtrToStringAnsi(pchar);
                //mapbind.sProjectid = localCheckProject.ProjectID;
                MapBindingList.Add(mapbind);
            }
            superGridControl1.PrimaryGrid.DataSource = MapBindingList;
        }

        private void comboBox1_Mapnumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            MapbindingPreview();
        }

    }

    class Mapbinding
    {
        public string sMapnumber { set; get; }
        //public string sProjectid { set; get; }
    }
}

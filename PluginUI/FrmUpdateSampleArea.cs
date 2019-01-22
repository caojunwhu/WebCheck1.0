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

namespace PluginUI
{

    public partial class FrmUpdateSampleArea : Form
    {
        public DLGCheckProjectClass localCheckProject { set; get; }
        string localpgDatasourceConnectionID { set; get; }
        public FrmUpdateSampleArea(DLGCheckProjectClass GlobeCheckProject, string paras, string pgDatasourceConnectionID)
        {
            InitializeComponent();
            localCheckProject = GlobeCheckProject;
            localpgDatasourceConnectionID = pgDatasourceConnectionID;

        }

        private void btn_BroswerScaterFile_Click(object sender, EventArgs e)
        {
            //获取当前路径和文件名
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "ESRI ShapeFile(*.shp)|*.shp|All Files(*.*)|*.*";
            dlg.Title = "Open ESRI ShapeFile";
            dlg.ShowDialog();
            string strFullPath = dlg.FileName;
            if (strFullPath == "") return;
            textBox_filepath.Text = strFullPath;


        }

        //perfectly realize import shapefile to postgis ,this is used to import mapbindingtable,sampletable,etc.
        //the ogr driver import shapefile name is lower case,so the source shapefile filename is lower case is better.
        public void ImportSampleAreaByOgrDriver(string sShapeFilePath)
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
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");

            OSGeo.OGR.Ogr.RegisterAll();
            //ShapeFile layer
            OSGeo.OGR.Driver shapefiledriver = OSGeo.OGR.Ogr.GetDriverByName("ESRI Shapefile");
            OSGeo.OGR.DataSource shapefileDatasource = shapefiledriver.Open(sShapeFilePath, 0);
            OSGeo.OGR.Layer shapefileLayer = shapefileDatasource.GetLayerByName(sLayerName);

            if (shapefileLayer.GetGeomType() != wkbGeometryType.wkbMultiPolygon && shapefileLayer.GetGeomType() != wkbGeometryType.wkbPolygon)
            {
                MessageBox.Show("非多边形！");
                return;
            }

            //PG Layer
            OSGeo.OGR.Driver pgdriver = OSGeo.OGR.Ogr.GetDriverByName("PostgreSQL");
            OSGeo.OGR.DataSource pgDatasource = pgdriver.Open(ogrPgConnectionID, 1);

            OSGeo.OGR.Layer pglayer;
            // this case is that target layer is exist,just get it 
            // warrning :ogr pgdriver support chinese is not good!
            pglayer = pgDatasource.GetLayerByName(sLayerName);

            if(pglayer.GetGeomType()!=wkbGeometryType.wkbPolygon)
            {
                return;
            }

            //Import process
            Feature feature;
            shapefileLayer.ResetReading();
            int nCount = 0;
            try
            {
                while ((feature = shapefileLayer.GetNextFeature()) != null)
                {
                    Feature sampleareafeature = SampleAreaShapeToOGRFeature(feature);

                    string wkt = "";
                    feature.GetGeometryRef().ExportToWkt(out wkt);

                    string select_feature = string.Format("select * from samplearea where geometry_distance_centroid(ST_Centroid(geometry('{0}')),ST_Centroid(wkb_geometry))<=0.000001 and projectid = '{1}'", wkt, localCheckProject.ProjectID);
                    DataTable datatable = datareadwrite.GetDataTableBySQL(select_feature);

                    if (datatable.Rows.Count == 0)
                    {
                        string select_maxid = string.Format("select max(ogc_fid) from samplearea ");
                        int maxid = Convert.ToInt32(datareadwrite.GetScalar(select_maxid));

                        int fid = maxid + 1;
                        sampleareafeature.SetFID(fid);
                        pglayer.CreateFeature(sampleareafeature);
                        nCount += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show(string.Format("成功导入{0}个抽样分区！",nCount));
        }

        private OSGeo.OGR.Feature SampleAreaShapeToOGRFeature(Feature samplearea)
        {
            OSGeo.OGR.FeatureDefn ofd = new OSGeo.OGR.FeatureDefn("");
            OSGeo.OGR.FieldDefn fd = null;

            fd = new OSGeo.OGR.FieldDefn("samplearea", OSGeo.OGR.FieldType.OFTInteger);
            ofd.AddFieldDefn(fd);

            fd = new OSGeo.OGR.FieldDefn("projectid", OSGeo.OGR.FieldType.OFTString);
            ofd.AddFieldDefn(fd);


            OSGeo.OGR.Feature ogrFeature = new OSGeo.OGR.Feature(ofd);
            ogrFeature.SetField(0, 1);
            ogrFeature.SetField(1, localCheckProject.ProjectID);
            
            ogrFeature.SetGeometryDirectly(samplearea.GetGeometryRef().Clone());

            return ogrFeature;
        }

        private void button1_updatesamplearea_Click(object sender, EventArgs e)
        {

            ImportSampleAreaByOgrDriver(textBox_filepath.Text);

        }
    }


}


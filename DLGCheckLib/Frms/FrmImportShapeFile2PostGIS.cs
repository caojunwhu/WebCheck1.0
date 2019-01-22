using System;
using System.Data;
using System.Windows.Forms;
using System.IO;
using OSGeo.OGR;
using SharpMap.Data;
using SharpMap.Data.Providers;

namespace DLGCheckLib
{
    public partial class FrmImportShapeFile2PostGIS : Form
    {
        string _shpFilePath = "";//PG:dbname=hbdlgqgeoinfodb user=hbdlgqinfouser password=87788106
        string _pgConnectionID = "Server=127.0.0.1;Port=5432;Database=hbdlgqgeoinfodb;uid=hbdlgqinfouser;password=87788106;";
        public FrmImportShapeFile2PostGIS()
        {
            InitializeComponent();
        }

        public string ShapeFilePath
        {
            get { return _shpFilePath; }
            set { _shpFilePath = value; textBox_ShpCheckPointsPath.Text = _shpFilePath; }
        }
        DatabaseDesignPlus.DataTableDesign theDataBaseDesign = null;

        SharpMap.Data.Providers.ShapeFileEx _shpEx;
        public  SharpMap.Data.Providers.ShapeFileEx ShpeFile
        {
            get { return _shpEx; }
        }

        public string PgConnectionID
        {
            get
            {
                return _pgConnectionID;
            }

            set
            {
                _pgConnectionID = value;
            }
        }

        private static OSGeo.OGR.Feature OgrFeatureToFeatureDataRow(FeatureDataRow fdr)
        {
            OSGeo.OGR.FeatureDefn ofd = new OSGeo.OGR.FeatureDefn("");
            OSGeo.OGR.FieldDefn fd = null;
            for (int iField = 0; iField < fdr.Table.Columns.Count; iField++)
            {
                switch (fdr.Table.Columns[iField].DataType.Name)
                {
                    case "String":
                        fd = new OSGeo.OGR.FieldDefn(fdr.Table.Columns[iField].ColumnName, OSGeo.OGR.FieldType.OFTString);
                        ofd.AddFieldDefn(fd);
                        break;
                    case "Int32":
                        fd = new OSGeo.OGR.FieldDefn(fdr.Table.Columns[iField].ColumnName, OSGeo.OGR.FieldType.OFTInteger);
                        ofd.AddFieldDefn(fd);
                        break;

                    case "Double":
                        fd = new OSGeo.OGR.FieldDefn(fdr.Table.Columns[iField].ColumnName, OSGeo.OGR.FieldType.OFTReal);
                        ofd.AddFieldDefn(fd);
                        break;
                }
            }

            OSGeo.OGR.Feature ogrFeature = new OSGeo.OGR.Feature(ofd);
            for (int iField = 0; iField < fdr.Table.Columns.Count; iField++)
            {
                ogrFeature.SetField(iField, Convert.ToString(fdr[iField]));
            }

            ogrFeature.SetGeometryDirectly(OSGeo.OGR.Geometry.CreateFromWkb(fdr.Geometry.AsBinary()));

            return ogrFeature;
        }
        public void Import(string sShapeFilePath)
        {
            ShapeFileEx shapeFileDataSource = new ShapeFileEx(sShapeFilePath);
            shapeFileDataSource.Open();
            string sLayerName = Path.GetFileNameWithoutExtension(sShapeFilePath);
            FeatureDataSet fds =new FeatureDataSet();
            shapeFileDataSource.ExecuteIntersectionQuery(shapeFileDataSource.GetExtents(), fds);
            FeatureDataTable fdt = fds.Tables[0];

            DataColumn dc = new DataColumn("wkb_geometry",typeof(byte[]));
            fdt.Columns.Add(dc);
            fdt.Columns.Add("ogc_fid");
            int index = 0;
            foreach (FeatureDataRow fdr in fdt.Rows)
            {
                //OSGeo.OGR.Feature feature = OgrFeatureToFeatureDataRow(fdr);
                //pglayer.CreateFeature(feature);
                byte[] buffer = fdr.Geometry.AsBinary(); ;
                //string hexstr = CommonUtil.ByteArrayToString(buffer);
                fdr["wkb_geometry"] = buffer;
                fdr["ogc_fid"] = index++;
            }
            shapeFileDataSource.Close();
            ///////////////////////////////////////////////////////
            DatabaseDesignPlus.IDatabaseReaderWriter dbWriter = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", PgConnectionID);

            bool bSuccess = false;
            if (theDataBaseDesign != null)
            {
                theDataBaseDesign.InitializeDataTableDesign("PostgreSQL", sLayerName);
                bSuccess = dbWriter.ImportDataTableRecords(sLayerName, fdt, theDataBaseDesign);
            }
            else
            {
                bSuccess = dbWriter.ImportDataTableRecords(sLayerName, fdt);
            }
            if (bSuccess)
            {
                MessageBox.Show("成功导入");
            }
            else
            {
                MessageBox.Show("导入失败");
            }
        }
        //perfectly realize import shapefile to postgis ,this is used to import mapbindingtable,sampletable,etc.
        //the ogr driver import shapefile name is lower case,so the source shapefile filename is lower case is better.
        public void ImportByOgrDriver(string sShapeFilePath)
        {
            if (sShapeFilePath == "" || sShapeFilePath == null)
                return;

            //截取文件路径
            string directory = Path.GetDirectoryName(sShapeFilePath);
            string sLayerName = Path.GetFileNameWithoutExtension(sShapeFilePath).ToLower();

            string ogrPgConnectionID = string.Format("PG:{0}",PgConnectionID);
            //Ogr ogr = new Ogr(ogrPgConnectionID);

            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
            //为了支持中文路径，请添加下面这句SHAPE_ENCODING;
            //UTF-8 to ISO-8859-1.
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", ""); 

            OSGeo.OGR.Ogr.RegisterAll();
            //ShapeFile layer
            OSGeo.OGR.Driver shapefiledriver = OSGeo.OGR.Ogr.GetDriverByName("ESRI Shapefile");
            OSGeo.OGR.DataSource shapefileDatasource = shapefiledriver.Open(sShapeFilePath, 0);
            OSGeo.OGR.Layer shapefileLayer = shapefileDatasource.GetLayerByName(sLayerName);

            //PG Layer
            OSGeo.OGR.Driver pgdriver = OSGeo.OGR.Ogr.GetDriverByName("PostgreSQL");
            OSGeo.OGR.DataSource pgDatasource = pgdriver.Open(ogrPgConnectionID, 1);
            
            string srswkt = "";
            OSGeo.OSR.SpatialReference srf=null;
            try
            {
                srf= shapefileLayer.GetSpatialRef();
                srf.ExportToWkt(out srswkt);
            }catch{ }

            string[] options = { "append=yes", "skipfailures=yes", "update=yes" };
            OSGeo.OGR.Layer pglayer;
            //OSGeo.OGR.Layer pglayer = pgDatasource.CreateLayer(sLayerName, srf, OSGeo.OGR.wkbGeometryType.wkbPolygon, options);
            //软件报错 AddGeometryColumn failed for layer xjxzjx, layer creation has failed.
            try
            {
                //this case is that the target layer is not exist.
                pglayer = pgDatasource.CreateLayer(sLayerName, srf, shapefileLayer.GetGeomType(), options);
                //add fields
                OSGeo.OGR.FeatureDefn ofd = new OSGeo.OGR.FeatureDefn("");
                OSGeo.OGR.FieldDefn fd = null;
                FeatureDefn fdef = shapefileLayer.GetLayerDefn();
                for (int iField = 0; iField < fdef.GetFieldCount(); iField++)
                {
                    fd = fdef.GetFieldDefn(iField);
                    string sFieldName = fd.GetName();
                    pglayer.CreateField(fd, 1);
                }
            }
            catch
            {
                // this case is that target layer is exist,just get it 
                // warrning :ogr pgdriver support chinese is not good!
                pglayer = pgDatasource.GetLayerByName(sLayerName);
            }

            //Import process
            Feature feature;
            shapefileLayer.ResetReading();

            try
            {
                while ((feature = shapefileLayer.GetNextFeature()) != null)
                {
                    int fid = pglayer.GetFeatureCount(1) + 1;
                    feature.SetFID(fid);
                    pglayer.CreateFeature(feature);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show("ShapeFile Import Success!");
         }

        private void button_ShpCheckPoints_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();

            fdlg.Filter = "ESRI Shapefile|*.shp";
            if (fdlg.ShowDialog() != DialogResult.OK)
                return;

            _shpFilePath = fdlg.FileName;
            textBox_ShpCheckPointsPath.Text = _shpFilePath;


        }

        private void button_Import_Click(object sender, EventArgs e)
        {
            //Import(_shpFilePath);
            ImportByOgrDriver(ShapeFilePath);

        }

        private void button1_Click(object sender, EventArgs e)
        {

            DatabaseDesignPlus.FrmPostgresSetting frm = new DatabaseDesignPlus.FrmPostgresSetting();
            if(frm.ShowDialog()==DialogResult.OK)
            {
                PgConnectionID = frm.PGConnectionstring;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DatabaseDesignPlus.FrmDatabaseDesignChoose frm = new DatabaseDesignPlus.FrmDatabaseDesignChoose();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                theDataBaseDesign = frm.TheDataTableDesign;

            }
        }
    }
}

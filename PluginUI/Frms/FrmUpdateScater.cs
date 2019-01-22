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

namespace PluginUI.Frms
{

    public partial class FrmUpdateScater : Form
    {
        public DLGCheckProjectClass localCheckProject { set; get; }
        List<Scater> LocalScaterList { set; get; }
        List<string> listLines;
        string localPGDatasourceConnectionID = "";
        public FrmUpdateScater(DLGCheckProjectClass GlobeCheckProject,string paras,string pgDatasourceConnectionID)
        {
            InitializeComponent();
            if (GlobeCheckProject == null)
            {
                MessageBox.Show("请先打开或新建一个项目！");
                return;
            }

            localCheckProject = GlobeCheckProject;
            localPGDatasourceConnectionID = pgDatasourceConnectionID;

        }
        void InitalizeControl()
        {
            string strFullPath = textBox_filepath.Text;
            listLines = new List<string>();
            using (StreamReader reader = new StreamReader(strFullPath))
            {
                string line = reader.ReadLine();
                while (line != "" && line != null)
                {
                    listLines.Add(line);
                    line = reader.ReadLine();
                }
                //循环完后，listLines 里面就放有第三行到第十行的数据了
            }

            //将第一行按分隔符进行分解后填入combbox中
            if (listLines.Count <= 0)
                return;
            if (comboBox1_splitchar.Text == "")
            {
                MessageBox.Show("请正确填写分割符");
                return;
            }
            string[] headers = listLines[0].Split(comboBox1_splitchar.Text[0]);

            if (headers.Length < 4)
            {
                MessageBox.Show("当前文件列数不够");
                return;
            }
            comboBox1_ptid.Items.Clear();
            comboBox2_sx.Items.Clear();
            comboBox4_sy.Items.Clear();
            comboBox5_sz.Items.Clear();

            foreach (string header in headers)
            {
                comboBox1_ptid.Items.Add(header);
                comboBox2_sx.Items.Add(header);
                comboBox4_sy.Items.Add(header);
                comboBox5_sz.Items.Add(header);
            }
                comboBox1_ptid.SelectedIndex = 0;
                comboBox2_sx.SelectedIndex = 1;
                comboBox4_sy.SelectedIndex = 2;
                comboBox5_sz.SelectedIndex = 3;



        }

        void RefreshLoadFile()
        {
            if (comboBox1_ptid.Text == "" || comboBox2_sx.Text == "" ||  comboBox4_sy.Text == "" || comboBox5_sz.Text == "")
            {
                return;
            }
            if (comboBox1_ptid.SelectedIndex <0 || comboBox2_sx.SelectedIndex < 0 || comboBox4_sy.SelectedIndex < 0 || comboBox5_sz.SelectedIndex < 0)
            {
                return;
            }

            List<Scater> stringfiledList = new List<Scater>();

            if (LocalScaterList == null)
            {
                LocalScaterList = new List<Scater>();
            }
            if (LocalScaterList.Count > 0)
            {
                LocalScaterList.Clear();
            }

            foreach (string line in listLines)
            {
                string[] fields = line.Split(comboBox1_splitchar.Text[0]);

                if (fields.Length < 4)
                    break;

                Scater scater = new Scater();
                scater.ptid = fields[comboBox1_ptid.SelectedIndex];
                scater.sx = fields[comboBox2_sx.SelectedIndex];
                scater.sy = fields[comboBox4_sy.SelectedIndex];
                scater.sz = fields[comboBox5_sz.SelectedIndex];
                stringfiledList.Add(scater);

                Scater scater2 = new Scater();
                scater2 = scater;
                LocalScaterList.Add(scater2);
            }

            if (checkBox1.Checked == true)
            {
                if(stringfiledList.Count>0)
                stringfiledList.RemoveAt(0);
            }

            dataGridViewX1.DataSource = stringfiledList;

        }

        private void btn_BroswerScaterFile_Click(object sender, EventArgs e)
        {
            //获取当前路径和文件名
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "DAT(*.dat)|*.dat|TEXT(*.txt)|*.txt|All Files(*.*)|*.*";
            dlg.Title = "Open DAT Data file";
            dlg.ShowDialog();
            string strFullPath = dlg.FileName;
            if (strFullPath == "") return;
            textBox_filepath.Text = strFullPath;

            InitalizeControl();
            //RefreshLoadFile();
        }

        private void comboBox1_splitchar_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshLoadFile();

        }

        private void comboBox2_sx_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshLoadFile();

        }

        private void comboBox1_ptid_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshLoadFile();

        }

        private void comboBox4_sy_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshLoadFile();

        }

        private void comboBox5_sz_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshLoadFile();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            RefreshLoadFile();

        }

        // 导入配置好的点
        private void btn_import_Click(object sender, EventArgs e)
        {

            if (LocalScaterList.Count <= 0)
                return;
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);

            string ogrPgConnectionID = string.Format("PG:{0}", localPGDatasourceConnectionID);
            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
            //为了支持中文路径，请添加下面这句SHAPE_ENCODING;
            //UTF-8 to ISO-8859-1.
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");

            OSGeo.OGR.Ogr.RegisterAll();
            //PG Layer
            OSGeo.OGR.Driver pgdriver = OSGeo.OGR.Ogr.GetDriverByName("PostgreSQL");
            OSGeo.OGR.DataSource pgDatasource = pgdriver.Open(ogrPgConnectionID, 1);
            string[] options = { "append=yes", "skipfailures=yes", "update=yes" };
            OSGeo.OGR.Layer pglayer;
            int nCount = 0;
            try
            {
                //this case is that the target layer is not exist.
                pglayer = pgDatasource.GetLayerByName("scater");

                foreach (Scater scater in LocalScaterList)
                {
                    try
                    {
                        OSGeo.OGR.Feature feature = ScaterToOGRFeature(scater);

                        string wkt = "";
                        feature.GetGeometryRef().ExportToWkt(out wkt);

                        //string select_feature = string.Format("select * from scater where sx={0} and sy={1} and sz={2} and projectid = '{3}'", scater.sx,scater.sy,scater.sz,localCheckProject.ProjectID);
                        string select_feature = string.Format("select * from scater where geometry_distance_centroid(geometry('{0}'),wkb_geometry)<=0.000001 and projectid = '{1}'", wkt, localCheckProject.ProjectID);
                        DataTable datatable =  datareadwrite.GetDataTableBySQL(select_feature);

                        if(datatable.Rows.Count ==0)
                        {
                            string select_maxid = string.Format("select max(ogc_fid) from scater ");
                            int maxid = Convert.ToInt32(datareadwrite.GetScalar(select_maxid));
                            //此处发现bug，新建散点id应为最大的ogc_fid+1，并非是数量+1
                            //int fid = pglayer.GetFeatureCount(1) + 1;
                            int fid = maxid + 1;
                            feature.SetFID(fid);
                            pglayer.CreateFeature(feature);
                            nCount += 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                } 
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            MessageBox.Show(string.Format("成功导入{0}个点。", nCount));

        }

        private OSGeo.OGR.Feature ScaterToOGRFeature(Scater scater)
        {
            OSGeo.OGR.FeatureDefn ofd = new OSGeo.OGR.FeatureDefn("");
            OSGeo.OGR.FieldDefn fd = null;

            fd = new OSGeo.OGR.FieldDefn("ptid", OSGeo.OGR.FieldType.OFTString);
            ofd.AddFieldDefn(fd);

            fd = new OSGeo.OGR.FieldDefn("sx", OSGeo.OGR.FieldType.OFTReal);
            ofd.AddFieldDefn(fd);

            fd = new OSGeo.OGR.FieldDefn("sy", OSGeo.OGR.FieldType.OFTReal);
            ofd.AddFieldDefn(fd);

            fd = new OSGeo.OGR.FieldDefn("sz", OSGeo.OGR.FieldType.OFTReal);
            ofd.AddFieldDefn(fd);

            fd = new OSGeo.OGR.FieldDefn("projectid", OSGeo.OGR.FieldType.OFTString);
            ofd.AddFieldDefn(fd);


            OSGeo.OGR.Feature ogrFeature = new OSGeo.OGR.Feature(ofd);
            ogrFeature.SetField(0, scater.ptid);
            ogrFeature.SetField(1, Convert.ToDouble(scater.sx));
            ogrFeature.SetField(2, Convert.ToDouble(scater.sy));
            ogrFeature.SetField(3, Convert.ToDouble(scater.sz));
            ogrFeature.SetField(4, localCheckProject.ProjectID);

            OSGeo.OGR.Geometry point = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPoint);
            point.AddPoint(Convert.ToDouble(scater.sx), Convert.ToDouble(scater.sy), Convert.ToDouble(scater.sz));
            ogrFeature.SetGeometryDirectly(point);

            return ogrFeature;
        }
    }

    class Scater
    {
        public string ptid { set; get; }
        public string sx { set; get; }
        public string sy { set; get; }
        public string sz { set; get; }
    }
}

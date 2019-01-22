using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Authentication.Class;
using OSGeo.OGR;
using DatabaseDesignPlus;

namespace DLGCheckLib
{
    public partial class FrmNewProject : Form
    {
        UserObject GlobeUser = null;
        public FrmNewProject(UserObject oUser)
        {
            InitializeComponent();

            GlobeUser = oUser;

            departmenttextBox2.Text = GlobeUser.company;
            departmenttextBox2.ReadOnly = true;
            ownertextBox1.Text = GlobeUser.username;
            ownertextBox1.ReadOnly = true;
            lastopentimetextBox1.Text = DateTime.Now.ToString();
            lastopentimetextBox1.ReadOnly = true;
            label11.Text = "当前时间：";

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);
            string sql_shared = string.Format("select * from 用户表 where company = '{0}' and username <> '{1}'", GlobeUser.company, GlobeUser.username);
            DataTable datatable= dbread.GetDataTableBySQL(sql_shared);

            if (datatable.Rows.Count <= 0)
                return;

            foreach(DataRow datarow in datatable.Rows)
            {
                string username = Convert.ToString(datarow["username"]);
                sharedlistView1.Items.Add(username);
            }
        }
        public FrmNewProject(DLGCheckLib.DLGCheckProjectClass oProject)
        {
            InitializeComponent();
            GlobeProject = oProject;
            tb_projectname.Text = GlobeProject.ProjectName;
            tb_PROJECTID.Text = GlobeProject.ProjectID;
            tb_lotsize.Text = Convert.ToString(GlobeProject.nLots);
            tb_SAMPLEAREA.Text = Convert.ToString(GlobeProject.nSampleAreaCount);
            tb_scale.Text = Convert.ToString(GlobeProject.MapScale);
            tb_samplesize.Text = Convert.ToString(GlobeProject.nSampleSize);
            tb_CoordSys.Text = GlobeProject.SrText;
            departmenttextBox2.Text = GlobeProject.department;
            ownertextBox1.Text = GlobeProject.owner;
            lastopentimetextBox1.Text = GlobeProject.lastOpentime;
            comboBox1_FileType.Text = GlobeProject.SampleFileFormat;

            string shared = GlobeProject.shared;
            string[] sharedUsernames = shared.Split(';');
            foreach (string sharedUsername in sharedUsernames)
            {
                if (sharedUsername == null || sharedUsername == "") continue;

                ListViewItem item = sharedlistView1.Items.Add(sharedUsername);
                item.Checked = true;
            }

            tb_projectname.ReadOnly = true;
            tb_PROJECTID.ReadOnly = true;
            tb_lotsize.ReadOnly = true;
            tb_SAMPLEAREA.ReadOnly = true;
            tb_scale.ReadOnly = true;
            tb_samplesize.ReadOnly = true;
            tb_CoordSys.ReadOnly = true;
            departmenttextBox2.ReadOnly = true;
            ownertextBox1.ReadOnly = true;
            lastopentimetextBox1.ReadOnly = true;
            comboBox1_FileType.Enabled = false;

            button2.Hide();
            bt_CoordSysSetting.Hide();
            
        }
        string sProjectName = "";
        DLGCheckProjectClass GlobeProject;

        string sDwgfilePath = "";
        int iSampleArea = 0;
        int iLotsize = 0;
        int iSamplesize = 0;
        int iScale = 0;
        string srtext = "";
        string department = "";
        string owner = "";
        string shared = "";
        string lastopentime = "";
        double dMaxMeanError = 0.0;
        string sSampleFileType = "";
        
        

        string sPostGISConnection = "";

        public string SPostGISConnection
        {
            get
            {
                return sPostGISConnection;
            }

            set
            {
                sPostGISConnection = value;
            }
        }

        string _sprojectid = "";
        public string SProjectID
        {
            get
            {
                return _sprojectid;
            }

            set
            {
                _sprojectid = value;
            }
        }


        //确定按钮
        private void button2_Click(object sender, EventArgs e)
        {
            
            sProjectName = tb_projectname.Text;
            SProjectID = tb_PROJECTID.Text;
            iSampleArea = Convert.ToInt32(tb_SAMPLEAREA.Text);
            iLotsize = Convert.ToInt32(tb_lotsize.Text);
            iSamplesize = Convert.ToInt32(tb_samplesize.Text);
            iScale = Convert.ToInt32(tb_scale.Text);
            srtext = tb_CoordSys.Text;
            department = departmenttextBox2.Text;
            departmenttextBox2.ReadOnly = true;
            ownertextBox1.ReadOnly = true;
            owner = ownertextBox1.Text;
            if (comboBox1_FileType.Text != "")
                sSampleFileType = comboBox1_FileType.Text.Substring(0, comboBox1_FileType.Text.IndexOf('(')) ;

            //处理选择的项目参与者
            shared = "";
            foreach(ListViewItem li in sharedlistView1.Items)
            {
                if(li.Checked==true)
                {
                    shared += li.Text + ";";
                }
            }
            //检查创建者是否添加自己
            if(shared.IndexOf(owner)<0)
            {
                shared += owner + ";";
            }
            // 管理员作为超级用户，对每个项目都有参与权力，
            if(shared.IndexOf("管理员")<0)
            {
                shared += "管理员";
            }

            lastopentime = DateTime.Now.ToString();
            //检查项目基本参数是否正确填写
            if (sProjectName == "" ||
                iLotsize == 0 || iSamplesize == 0 ||
                iScale == 0 || iSampleArea==0 || srtext =="" || sSampleFileType=="")
            {
                MessageBox.Show("请检查并输入项目基本信息！");
                return;
            }
            //检查projectid是否正确填写
            //1、检查时间
            string date = string.Format("{0:yyyyMMdd}",DateTime.Now.Date);
            if(SProjectID.IndexOf(date) <0)
            {
                MessageBox.Show("请检查项目ID是否含有今天的日期！格式：20160815");
                return;
            }
            //2、检查比例尺
            if (SProjectID.IndexOf("00") < 0)
            {
                MessageBox.Show("请检查项目ID是否正确输入比例尺！格式：ZJS500DLG2016081501");
                return;
            }
            //2、检查比例尺与项目ID中是否一致
            if (SProjectID.IndexOf(tb_scale.Text) < 0)
            {
                MessageBox.Show("请检查项目ID比例尺与项目比例尺是否一致！格式：ZJS500DLG2016081501");
                return;
            }
            //3、检查样本量与批量是否合法
            if(iSamplesize>iLotsize|| iLotsize<=0)
            {
                MessageBox.Show("请检查批量大小与样本量是否填写正确！");
                return;
            }
            //4、抽样分区数检查
            if(iSampleArea<0)
            {
                MessageBox.Show("请检查抽样分区数是否填写正确！");
                return;
            }
            //5、SRTEXT中双引号处理
            srtext = srtext.Replace("\"", "\\\"");

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            SPostGISConnection = pgdbconstr;
            DatabaseDesignPlus.DatabaseReaderWriter dbReaderWriter = new DatabaseDesignPlus.ClsPostgreSql(SPostGISConnection);
            string sProject = "DLGCHECKPROJECT";
            //1.存在项目表
            string sInsertProject = 
                string.Format("insert into DLGCHECKPROJECT (PROJECTNAME,PROJECTID,LOTSIZE,SAMPLESIZE,SAMPLEAREA,SCALE,SRTEXT,DEPARTMENT,OWNER,SHARED,LASTOPENTIME,SAMPLEFILETYPE)values('{0}','{1}',{2},{3},{4},{5},'{6}','{7}','{8}','{9}','{10}','{11}')",sProjectName,SProjectID,iLotsize,iSamplesize,iSampleArea,iScale, srtext,department,owner,shared,lastopentime,sSampleFileType);
            if (dbReaderWriter.GetSchameDataTableNames().IndexOf(sProject.ToLower()) > 0)
            {
                string sql_projecid = string.Format("select projectid from dlgcheckproject where projectid = '{0}'", tb_PROJECTID.Text);
                if(dbReaderWriter.GetScalar(sql_projecid)!=null)
                {
                    MessageBox.Show("项目ID已存在，请重新设定ID："+tb_PROJECTID.Text);
                    return;
                }
                else
                {
                    dbReaderWriter.ExecuteSQL(sInsertProject);
                }
            }
            else //第一次新建项目,自动插入后，表名称变更为小写
            {
                string sCreateProjectTable = string.Format("create table DLGCHECKPROJECT(PROJECTNAME text ,PROJECTID text ,LOTSIZE integer,SAMPLESIZE integer,SAMPLEAREA integer,SCALE integer,SRTEXT text,DEPARTMENT text, OWNER text ,SHARED text,LASTOPENTIME timestamp without time zone,SAMPLEFILETYPE text,CONSTRAINT dlgcheckproject_pkey PRIMARY KEY (PROJECTID))");
                dbReaderWriter.ExecuteSQL(sCreateProjectTable);

                dbReaderWriter.ExecuteSQL(sInsertProject);
            }

            //createprojectinfotables();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        //选择dwg文件路径
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fdlg = new FolderBrowserDialog();
            fdlg.SelectedPath = @"D:\GDAL\dwgfile";
            if (fdlg.ShowDialog() != DialogResult.OK)
                return;

            sDwgfilePath = fdlg.SelectedPath;

           
        }

        string ConverUnicode2Utf8(string sunicode)
        {
            string sUtf8;
            //byte[] bytes = System.Text.Encoding.Unicode.GetBytes(sunicode);
            //bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, bytes);
            //sUtf8 = Encoding.GetEncoding("UTF-8").GetString(bytes);
            byte[] bytes = Encoding.UTF8.GetBytes(sunicode);
            sUtf8 = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return sUtf8;
        }

        private void createprojectinfotables()
        {
            string dwgFolder = "";// tb_dwgfilepath.Text;
            OSGeo.OGR.Ogr.RegisterAll();
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");
            OSGeo.OGR.Driver aodriver = OSGeo.OGR.Ogr.GetDriverByName("ArcObjects");
            string dwgconnection = string.Format("AO:{0}", sDwgfilePath);
            OSGeo.OGR.DataSource aodatasource = aodriver.Open(dwgconnection, 1);

            string _sDbConnectionString = "";
            _sDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            //_sDbConnectionString = "host=localhost dbname=SurveryProductCheckDatabase user=postgres password='123'";
            _sDbConnectionString = DataBaseConfigs.RePlaceConfig(_sDbConnectionString);

            OSGeo.OGR.Driver pgdriver = OSGeo.OGR.Ogr.GetDriverByName("PostgreSQL");
            string ogconnection = string.Format("PG:{0}", _sDbConnectionString);
            OSGeo.OGR.DataSource pgdatasource = pgdriver.Open(ogconnection, 1);

            Dictionary<string, OSGeo.OGR.Layer> pglayers = new Dictionary<string, Layer>();

            string[] options = { "append=yes", "skipfailures=yes" ,"update=yes"};
            OSGeo.OGR.Layer pglayer;
            string sLayerName = "";
            // annotation
            try
            {
                
                //sLayerName = string.Format("{0}:annotation", sProjectName);
                sLayerName = "annotation";
                pglayer = pgdatasource.CreateLayer(sLayerName, null, wkbGeometryType.wkbPoint, options);
            }
            catch
            {
                pglayer = pgdatasource.GetLayerByName(sLayerName);
            }
            pglayers.Add(sLayerName, pglayer);

            //polygon
            try
            {
                //sLayerName = string.Format("{0}:polygon", sProjectName);
                sLayerName = "polygon";
                pglayer = pgdatasource.CreateLayer(sLayerName, null, wkbGeometryType.wkbPolygon, null);
            }
            catch
            {
                pglayer = pgdatasource.GetLayerByName(sLayerName);
            }
            pglayers.Add(sLayerName, pglayer);

            //polyline
            try
            {
                //sLayerName = string.Format("{0}:polyline", sProjectName);
                sLayerName = "polyline";
                pglayer = pgdatasource.CreateLayer(sLayerName, null, wkbGeometryType.wkbLinearRing, null);
            }
            catch
            {
                pglayer = pgdatasource.GetLayerByName(sLayerName);
            }
            pglayers.Add(sLayerName, pglayer);

            //point
            try
            {
                //sLayerName = string.Format("{0}:point", sProjectName);
                sLayerName = "point";
                pglayer = pgdatasource.CreateLayer(sLayerName, null, wkbGeometryType.wkbPoint, null);
            }
            catch
            {
                pglayer = pgdatasource.GetLayerByName(sLayerName);
            }
            pglayers.Add(sLayerName, pglayer);

            //将图形从文件导入到新建的矢量图层中，包括字段建立
            for (int i = 0; i < aodatasource.GetLayerCount(); i++)
            {
                OSGeo.OGR.Layer _aolayer = aodatasource.GetLayerByIndex(i);             
                OSGeo.OGR.Layer _pglayer = pglayers[_aolayer.GetName().ToLower()];

                if (_pglayer.GetLayerDefn().GetFieldCount() < 2)
                {
                    //如果是新建图层，处理图层属性字段
                }
                //此处图层导入postgis,
                // 添加项目表，项目信息表（用户交互填充），并将分区信息填入项目信息表，增加图形属性

                Feature feature;
                _aolayer.ResetReading();              
               
                try
                {
                    while ((feature = _aolayer.GetNextFeature()) != null)
                    {
                        int fid = _pglayer.GetFeatureCount(1) + 1;
                        feature.SetFID(fid);                        
                        _pglayer.CreateFeature(feature);                        
                    }
                }
                catch(Exception ex)
                {
                    //if (_aolayer.GetName() == "Annotation")
                    {
                        System.Console.Write(ex.Message);
                    }
                }

                _aolayer.Dispose();
            }
            aodatasource.Dispose();
            aodriver.Dispose();
        }

        private void bt_CoordSysSetting_Click(object sender, EventArgs e)
        {
            Frms.FrmSpatialReferenceSetting frmsrf = new Frms.FrmSpatialReferenceSetting();
            if(frmsrf.ShowDialog()==DialogResult.OK)
            {
                tb_CoordSys.Text = frmsrf.Spatialreferencewkt;
                srtext = tb_CoordSys.Text;
            }
        }

        string oldProjectidHelpString = "";
        private void tb_PROJECTID_Click(object sender, EventArgs e)
        {
            if(tb_PROJECTID.Text!="")
            {
                oldProjectidHelpString = tb_PROJECTID.Text;
                tb_PROJECTID.Text = "";
            }
            else
            {
                tb_PROJECTID.Text = oldProjectidHelpString;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DatabaseDesignPlus;
using System.IO;
using Authentication.Class;
using SharpMap.Data.Providers;
using System.Drawing;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;

namespace DLGCheckLib
{
    public partial class FrmOpenproject : Form
    {
        //string _LoginUserName;
        UserObject LoginUser;
        IMap GlobeMap;

        public FrmOpenproject(string sdbConnectionString, UserObject oLoginUser,IMap oMap)
        {
            //_LoginUserName = loginusername;
            LoginUser = oLoginUser;
            GlobeMap = oMap;
            SDbConnectionString = sdbConnectionString;

            InitializeComponent();

            datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string sqlfillcb1 = string.Format("select {0},{1} from {2} where position('{3}' in shared)>0 order by lastopentime desc ", "projectname", "projectid", "dlgcheckproject",LoginUser.username);
            List<string> projects = datareadwrite.GetSingleFieldValueList("projectname", sqlfillcb1);
            _projects = datareadwrite.GetKeyPairValueDictionary(sqlfillcb1);

            DatabaseReaderWriterFactory.FillCombox(projects, comboBox1);
            if (projects.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }
        string _projectname = "";
        public string ProjectName
        {
            get { return _projectname; }
        }

        private string SDbConnectionString
        {
            get
            {
                return _sDbConnectionString;
            }

            set
            {
                _sDbConnectionString = value;
            }
        }

        public IMap Map
        {
            get
            {
                return _Map;
            }

            set
            {
                _Map = value;
            }
        }

        public string Projectid
        {
            get
            {
                return _projectid;
            }

            set
            {
                _projectid = value;
            }
        }

        string _creater;
        string _projectid = "";
        IMap _Map = null;
        Dictionary<string, string> _projects=null;

        DatabaseDesignPlus.IDatabaseReaderWriter datareadwrite;

        string _sDbConnectionString = "";
        private DLGCheckProjectClass GlobeProject;

        private void FrmOpenproject_Load(object sender, EventArgs e)
        {

       }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _projectname = comboBox1.SelectedItem as string;
            //string sqlselectprojectid = string.Format("select {0},{1} from {2} where projectname='{3}' ", "projectid", "projectname", "dlgcheckproject", _projectname);
            // Projectid = datareadwrite.GetScalar(sqlselectprojectid) as string;
            Projectid = _projects[_projectname];

            string creatersql = string.Format("select {0} from {1} where 成果名称='{2}'", "任务创建者", "检测项目信息表", _projectname);
            //_creater = ClsPostgreSql.GetSaclar(_sDbConnectionString, creatersql) as string;
            //_creater = datareadwrite.GetScalar(creatersql) as string;
            GlobeProject = new DLGCheckProjectClass(Projectid, LoginUser.username);

            tb_projectname.Text = GlobeProject.ProjectName;
            tb_PROJECTID.Text = GlobeProject.ProjectID;
            tb_lotsize.Text = Convert.ToString(GlobeProject.nLots);
            tb_SAMPLEAREA.Text = Convert.ToString(GlobeProject.nSampleAreaCount);
            tb_scale.Text = Convert.ToString(GlobeProject.MapScale);
            tb_samplesize.Text = Convert.ToString(GlobeProject.nSampleSize);
            tb_CoordSys.Text = GlobeProject.SrText;
            textBox_SampleFileType.Text = GlobeProject.SampleFileFormat;

            departmenttextBox2.Text = GlobeProject.department;
            ownertextBox1.Text = GlobeProject.owner;
            lastopentimetextBox1.Text = GlobeProject.lastOpentime;
            string[] shareusernames = GlobeProject.shared.Split(';');
            sharedlistView1.Items.Clear();
            foreach(string shared in shareusernames)
            {
                if (shared == null || shared == "") continue;

                ListViewItem item = sharedlistView1.Items.Add(shared);
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
            textBox_SampleFileType.ReadOnly = true;

        }
        private void buttonok_Click(object sender, EventArgs e)
        {


            this.DialogResult = DialogResult.OK;
        }
    }
}

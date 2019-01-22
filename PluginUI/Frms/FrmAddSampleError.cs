using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseDesignPlus;
using ServiceRanking;

namespace PluginUI.Frms
{
    public partial class FrmAddSampleError : Form
    {
        DataTable _QualityErrorTable;
        DataRow oldQualityError;
        public FrmAddSampleError()
        {
            InitializeComponent();
        }

        string _sMapnumber;
        Authentication.Class.UserObject _loginuser;
        string _sprojectid;
        public FrmAddSampleError(string sMapnumber,Authentication.Class.UserObject loginuser,string projectid)
        {
            InitializeComponent();
            _sMapnumber = sMapnumber;
            _loginuser = loginuser;
            _sprojectid = projectid;
            string SampleerrorCollectionTableName = "检查意见记录表";
            _QualityErrorTable = new DataTable();
            _QualityErrorTable.Columns.Add("质量元素");
            _QualityErrorTable.Columns.Add("质量子元素");
            _QualityErrorTable.Columns.Add("错漏类别");
            _QualityErrorTable.Columns.Add("错漏描述");
            _QualityErrorTable.Columns.Add("处理意见");
            _QualityErrorTable.Columns.Add("复查情况");
            _QualityErrorTable.Columns.Add("修改情况");
            _QualityErrorTable.Columns.Add("检查者");
            _QualityErrorTable.Columns.Add("检查日期");


            textBox1.Text = _sMapnumber;

            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            List<string> tableNames = datareadwrite.GetSchameDataTableNames();

            string qualityelementTable = "质量元素编码";
            tableNames = datareadwrite.GetSchameDataTableNames();
            if (tableNames.Contains(qualityelementTable) == true)
            {
                string sql_qualityFClass = string.Format("select 质量元素 from {0} where length(质量元素编码)=2", qualityelementTable);
                DataTable datatable = datareadwrite.GetDataTableBySQL(sql_qualityFClass);
                List<string> qualityFClass = datareadwrite.GetSingleFieldValueList("质量元素", sql_qualityFClass);
                foreach(string str in qualityFClass)
                {
                    comboBox1.Items.Add(str);
                }
                if(qualityFClass.Count>0)
                { comboBox1.SelectedIndex = 0; }
            }


            if (tableNames.Contains(SampleerrorCollectionTableName) == true)
            {
                //逐条记录到数据表_QualityErrorTable
                string sql_queryitem = string.Format("select * from {0} where ProjectID = '{1}' and Mapnumber='{2}' ", SampleerrorCollectionTableName, _sprojectid, _sMapnumber);
                DataTable datatable0 = datareadwrite.GetDataTableBySQL(sql_queryitem);
                foreach (DataRow dr in datatable0.Rows)
                {
                    DataRow datarow = _QualityErrorTable.NewRow();
                    datarow["质量元素"] = dr["质量元素"];
                    datarow["质量子元素"] = dr["质量子元素"];
                    datarow["错漏类别"] = dr["错漏类别"];
                    datarow["错漏描述"] = dr["错漏描述"];
                    datarow["处理意见"] = dr["处理意见"];
                    datarow["复查情况"] = dr["复查情况"];
                    datarow["修改情况"] = dr["修改情况"];
                    datarow["检查者"] = dr["检查者"];
                    datarow["检查日期"] = dr["检查日期"];
                    _QualityErrorTable.Rows.Add(datarow);

                }
                dataGridViewX1.DataSource = _QualityErrorTable;
                dataGridViewX1.Refresh();

            }


        }
        //质量元素切换时导致质量子元素跟着切换
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string qualityelement = comboBox1.SelectedItem as string;
            string qualityelementTable = "质量元素编码";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            List<string> tableNames = datareadwrite.GetSchameDataTableNames();
            if (tableNames.Contains(qualityelementTable) == true)
            {
                string sql_qualityFClassCode = string.Format("select 质量元素编码 from {0} where length(质量元素编码)=2 and 质量元素 = '{1}'", qualityelementTable, qualityelement);
                string qualityFClassCode = datareadwrite.GetScalar(sql_qualityFClassCode) as string;

                string sql_qualitSClass = string.Format("select 质量元素 from {0} where length(质量元素编码)=4 and left(质量元素编码,2) = '{1}'", qualityelementTable, qualityFClassCode);
                List<string> qualitySClass = datareadwrite.GetSingleFieldValueList("质量元素", sql_qualitSClass);
                comboBox2.Text = "";
                comboBox2.Items.Clear();
                foreach (string str in qualitySClass)
                {
                    comboBox2.Items.Add(str);
                }
                if (qualitySClass.Count > 0)
                { comboBox2.SelectedIndex = 0; }
            }
        }
        //质量子元素切换时，查询对应
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Text = "";
            comboBox4.Text = "";
            comboBox4.Items.Clear();
            //richTextBox_Error.Text = "";
        }
        //错漏类型切换时，根据质量子元素编码，错漏类别，查询错漏描述列表，进行填充更新
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string errorclass = comboBox3.Text;
            if (errorclass == "") return;

            string qualityelement = comboBox2.SelectedItem as string;
            string qualityelementTable = "质量元素编码";
            string qualityerrorTable = "错漏分类编码";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            List<string> tableNames = datareadwrite.GetSchameDataTableNames();
            if (tableNames.Contains(qualityelementTable) == true)
            {
                string sql_qualitySClassCode = string.Format("select 质量元素编码 from {0} where length(质量元素编码)=4 and 质量元素 = '{1}'", qualityelementTable, qualityelement);
                string qualitySClassCode = datareadwrite.GetScalar(sql_qualitySClassCode) as string;

                string errorcode = qualitySClassCode + errorclass;

                string sql_qualitError = string.Format("select 错漏参考描述 from {0} where  left(错漏分类编码,5) = '{1}'", qualityerrorTable, errorcode);
                List<string> qualityError = datareadwrite.GetSingleFieldValueList("错漏参考描述", sql_qualitError);
                //更新时需要清空
                comboBox4.Items.Clear();
                comboBox4.Text = "";
                foreach (string str in qualityError)
                {
                    comboBox4.Items.Add(str);
                }
               // if (qualityError.Count > 0)
               // { comboBox4.SelectedIndex = 0; }
            }
        }
        //错漏参考描述列表切换时，对错漏描述框进行切换
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox_Error.Text = comboBox4.Text;
        }
        //将编辑的问题保存到Grid中
        private void AddToTablebutton1_Click(object sender, EventArgs e)
        {
            string qualityFClass = comboBox1.Text;
            string qualitySClass = comboBox2.Text;
            string qualityErrorClass = comboBox3.Text;
            string qualityError = richTextBox_Error.Text;
            string fucha = fucharichTextBox1.Text;
            string chuli = chulirichTextBox1.Text;
            string xiugai = xiugairichTextBox3.Text;

            if(qualityError ==""||qualityErrorClass ==""||
                qualitySClass == "" || qualityFClass=="")
            {
                MessageBox.Show("请检查错漏描述是否完整！");
                return;
            }


            //如果是选中了某行进行了修改,且质量元素、质量子元素、错漏类别、错漏描述没有变化则进行修改，否则是新增
            if (oldQualityError != null&&
                (oldQualityError["质量元素"]as string) == qualityFClass&&
                (oldQualityError["质量子元素"] as string) == qualitySClass&&
                (oldQualityError["错漏类别"] as string) == qualityErrorClass&&
                (oldQualityError["错漏描述"] as string) == qualityError)
            {

                oldQualityError["质量元素"] = qualityFClass;
                oldQualityError["质量子元素"] = qualitySClass;
                oldQualityError["错漏类别"] = qualityErrorClass;
                oldQualityError["错漏描述"] = qualityError;
                oldQualityError["处理意见"] = chuli;
                oldQualityError["复查情况"] = fucha;
                oldQualityError["修改情况"] = xiugai;

                oldQualityError["检查者"] = _loginuser.username;
                oldQualityError["检查日期"] = DateTime.Now.ToString();

                //_QualityErrorTable.Rows.Remove(oldQualityError);
                //oldQualityError = null;
            }
            else
            {
                DataRow dr = _QualityErrorTable.NewRow();
                dr["质量元素"] = qualityFClass;
                dr["质量子元素"] = qualitySClass;
                dr["错漏类别"] = qualityErrorClass;
                dr["错漏描述"] = qualityError;
                dr["处理意见"] = chuli;
                dr["复查情况"] = fucha;
                dr["修改情况"] = xiugai;

                dr["检查者"] = _loginuser.username;
                dr["检查日期"] = DateTime.Now.ToString();
                _QualityErrorTable.Rows.Add(dr);

            }

            dataGridViewX1.DataSource = _QualityErrorTable;
            dataGridViewX1.Refresh();
        }

        //对该样本的问题进行更新
        private void button2_Click(object sender, EventArgs e)
        {
            //if (_QualityErrorTable.Rows.Count == 0) return;

            //先将该样本中的问题记录进行清空
            ///////////////////////////////////////////////////////////////////////
            /////////////更新到数据库
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string SampleerrorCollectionTableName = "检查意见记录表";
            //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
            List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
            if (tableNames2.Contains(SampleerrorCollectionTableName) == false)
            {
                string sql_createTable = string.Format("create table {0}( ProjectID text,Mapnumber text,质量元素 text,质量子元素 text,错漏类别 text ,错漏描述 text ,处理意见 text, 修改情况 text, 复查情况 text,检查者 text,检查日期 text)", SampleerrorCollectionTableName);
                datareadwrite.ExecuteSQL(sql_createTable);
                //记录
            }
            //逐条记录到数据表SearchTargetSetting
            string sql_queryitem = string.Format("select Mapnumber from {0} where ProjectID = '{1}' and Mapnumber='{2}' ", SampleerrorCollectionTableName, _sprojectid, _sMapnumber);
            object o = datareadwrite.GetScalar(sql_queryitem);
            if (o == null)//insert
            {
            }
            else // delete
            {
                string sql_delete = string.Format("delete from {0} where  ProjectID = '{1}' and Mapnumber='{2}' ", SampleerrorCollectionTableName, _sprojectid, _sMapnumber);
                datareadwrite.ExecuteSQL(sql_delete);
            }


            //写入新的问题记录
            foreach(DataRow dr in _QualityErrorTable.Rows)
            {
                string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", SampleerrorCollectionTableName, _sprojectid, _sMapnumber, 
                    dr["质量元素"] as string , dr["质量子元素"] as string , dr["错漏类别"] as string, dr["错漏描述"] as string ,dr["处理意见"] as string, dr["修改情况"] as string, dr["复查情况"] as string, dr["检查者"] as string, dr["检查日期"] as string);
                datareadwrite.ExecuteSQL(sql_insert);
                //记录搜索到的目标
            }

            MessageBox.Show("提交数据库完毕！");
        }

        private void FrmAddSampleError_Load(object sender, EventArgs e)
        {

        }
        //选择的意见变化时，需要将该行的意见进行填充显示，并记录下上一条记录的情况，如果进行了修改，需要将原记录删除，上传新记录，出发操作在“提交到数据库”
        private void dataGridViewX1_SelectionChanged(object sender, EventArgs e)
        {

            int index = dataGridViewX1.CurrentRow.Index;
            if (index <0 || comboBox2.Items.Count<=0 ||
                comboBox3.Items.Count<=0||index >= _QualityErrorTable.Rows.Count)
                return;

            oldQualityError = null;

            try
            {
                DataRow dr = _QualityErrorTable.Rows[index];
                comboBox1.Text = dr["质量元素"] as string;
                comboBox2.Text = dr["质量子元素"] as string;
                comboBox3.Text = dr["错漏类别"] as string;
                richTextBox_Error.Text = dr["错漏描述"] as string;
                chulirichTextBox1.Text = dr["处理意见"] as string;
                fucharichTextBox1.Text = dr["复查情况"] as string;
                xiugairichTextBox3.Text = dr["修改情况"] as string;

                oldQualityError = dr;

            }
            catch
            {

            }

        }
        //打开多选项窗口
        private void button1_Click(object sender, EventArgs e)
        {
            FrmAddQualityErrorItems frmqitem = new FrmAddQualityErrorItems(comboBox2.Text);
            if (frmqitem.ShowDialog() == DialogResult.OK)
            {
                if (frmqitem.QualityElementCode.Count > 0)
                {
                    string qualityFClass = comboBox1.Text;
                    string qualitySClass = comboBox2.Text;
                    string qualityErrorClass = comboBox3.Text;
                    string qualityError = richTextBox_Error.Text;
                    string fucha = fucharichTextBox1.Text;
                    string chuli = chulirichTextBox1.Text;
                    string xiugai = xiugairichTextBox3.Text;

                    foreach (KeyValuePair<string, string> code in frmqitem.QualityElementCode)
                    {
                        string errorclass = code.Key.Substring(4, 1);
                        string error = code.Value;

                        DataRow dr = _QualityErrorTable.NewRow();
                        dr["质量元素"] = qualityFClass;
                        dr["质量子元素"] = qualitySClass;
                        dr["错漏类别"] = errorclass;
                        dr["错漏描述"] = error;
                        dr["处理意见"] = chuli;
                        dr["复查情况"] = fucha;
                        dr["修改情况"] = xiugai;

                        dr["检查者"] = _loginuser.username;
                        dr["检查日期"] = DateTime.Now.ToString();
                        _QualityErrorTable.Rows.Add(dr);
                    }
                    dataGridViewX1.Refresh();
                }
            }
        }

        //根据输入的错漏描述自动匹配错漏分类：所属质量元素、错漏类别等信息，提供自动填充；
        private void richTextBox_Error_TextChanged(object sender, EventArgs e)
        {
            //从错漏描述数据库中读取所有错漏描述，进行初始化
            DLGCheckLib.DLGCheckProjectClass localProject = new DLGCheckLib.DLGCheckProjectClass();
            localProject.ReadProject(_sprojectid);
            string maptype = localProject.productType;
            ////////////////////////////////////************************************///
            maptype = "大比例尺地形图";
            string sql = string.Format("select * from ah错漏分类表 where 成果种类='{0}'", maptype);

            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);
            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);

            DataTable dt = datareadwrite.GetDataTableBySQL(sql);
            string[] ErrorDoc = new string[dt.Rows.Count+1];
            int index = 0;
            foreach (DataRow dr in dt.Rows)
            {
                ErrorDoc[index++] = dr["错漏内容"] as string;
            }
            ErrorDoc[index] = richTextBox_Error.Text;
            StopWordsHandler s= new StopWordsHandler();
            TFIDFMeasure tfidf = new TFIDFMeasure(ErrorDoc);
            Dictionary<int,float> Similarity = new Dictionary<int, float>();
            //与自身以外的描述计算相似性
            for(int i=0;i<index-1;i++)
            {
                float si = tfidf.GetSimilarity(i, index);
                Similarity.Add(i,si);
            }
            //求取最大值
            List<KeyValuePair<int,float>>maxSimilarity = Similarity.OrderByDescending(ao => ao.Value).ToList();
            int id = maxSimilarity.ElementAt(0).Key;
            DataRow d = dt.Rows[id];

            string qualitysubitem = d["质量子元素"] as string;           
            string sqlqualityitem = string.Format("select 质量元素 from ahselecteditems where 成果种类='{0}' and 质量子元素='{1}'", maptype, qualitysubitem);
            string qualityitem = datareadwrite.GetScalar(sqlqualityitem) as string;

            comboBox1.Text = qualityitem;
            comboBox2.Text = qualitysubitem;
            comboBox3.Text = d["错漏类型"] as string;

            //将前5个匹配度较高的错漏填充到combobox4中
            //comboBox4.DroppedDown = false;
            comboBox4.Items.Clear();
            for(int j=0;j<5;j++)
            {
                id = maxSimilarity.ElementAt(j).Key;
                d = dt.Rows[id];

                string errorstandard = d["错漏内容"] as string;
                comboBox4.Items.Add(string.Format("{0}({1:P})", errorstandard, maxSimilarity.ElementAt(j).Value));
                comboBox4.DroppedDown = true;
            }

        }
    }
}

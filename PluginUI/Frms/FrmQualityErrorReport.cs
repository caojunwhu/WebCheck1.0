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
using Eipsoft.Common;
using Newtonsoft.Json;
using ReportPrinter;
using DLGCheckLib;

namespace PluginUI.Frms
{
    public partial class FrmQualityErrorReport : Form
    {

        DataTable _QualityErrorTable;
        string _sMapnumber;
        string _loginuser;
        string _sprojectid;
        DLGCheckLib.DLGCheckProjectClass _CheckProject;
        public FrmQualityErrorReport()
        {
            InitializeComponent();
        }
        public FrmQualityErrorReport(DLGCheckLib.DLGCheckProjectClass GlobeCheckProject,string paras)
        {
            InitializeComponent();
            _CheckProject = GlobeCheckProject;
            _loginuser = _CheckProject.currentuser;
            _sprojectid = _CheckProject.ProjectID;

            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string SampleerrorCollectionTableName = "检查意见记录表";
            //逐条记录到数据表SearchTargetSetting
            string sql_queryitem = string.Format("select * from {0} where ProjectID = '{1}'  ", SampleerrorCollectionTableName, _sprojectid);
            DataTable datatable0 = datareadwrite.GetDataTableBySQL(sql_queryitem);

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

            string sql_queryChecker = string.Format("select distinct 检查者 from {0} where ProjectID = '{1}' order by 检查者 asc ", SampleerrorCollectionTableName, _sprojectid);
            List<string> Checkers = datareadwrite.GetSingleFieldValueList("检查者", sql_queryChecker);
            foreach (string chker in Checkers)
            {
                CheckercomboBox2.Items.Add(chker);
            }


            string sql_querySampleNumber = string.Format("select distinct Mapnumber from {0} where ProjectID = '{1}' order by Mapnumber asc ", SampleerrorCollectionTableName, _sprojectid);
            List<string>mapnumbers = datareadwrite.GetSingleFieldValueList("Mapnumber",sql_querySampleNumber);
            foreach(string str in mapnumbers)
            {
                SamplecomboBox1.Items.Add(str);
            }
            //显示第一个样本的检查记录
            if(SamplecomboBox1.Items.Count>0)
            {
                SamplecomboBox1.SelectedIndex = 0;
            }

        }
        //切换检查者后，跟着填充该管理员检查的样本号，显示该样本记录
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckercomboBox2.Text == "")
                return;

            string SampleerrorCollectionTableName = "检查意见记录表";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string sql_querySampleNumber = string.Format("select distinct Mapnumber from {0} where ProjectID = '{1}' and 检查者='{2}' order by Mapnumber asc ", SampleerrorCollectionTableName, _sprojectid,CheckercomboBox2.Text);
            List<string> mapnumbers = datareadwrite.GetSingleFieldValueList("Mapnumber", sql_querySampleNumber);
            SamplecomboBox1.Text = "";
            SamplecomboBox1.Items.Clear();
            foreach (string str in mapnumbers)
            {
                SamplecomboBox1.Items.Add(str);
            }//默认状态下不选择某个样本，则显示该检查者的所有检查记录


            //逐条记录到数据表SearchTargetSetting
            string sql_queryitem = string.Format("select * from {0} where ProjectID = '{1}' and 检查者= '{2}' ", SampleerrorCollectionTableName, _sprojectid, CheckercomboBox2.Text);
            DataTable datatable0 = datareadwrite.GetDataTableBySQL(sql_queryitem);

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


        //切换样本号后，填充该样本内的检查记录。
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( SamplecomboBox1.Text == "")
                return;

            _QualityErrorTable.Rows.Clear();
            dataGridViewX1.Refresh();

            //根据检查者和样本号查询记录

            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string SampleerrorCollectionTableName = "检查意见记录表";
            //逐条记录到数据表SearchTargetSetting
            string sql_queryitem = "";
            if(CheckercomboBox2.Text!="")
            {
                sql_queryitem = string.Format("select * from {0} where ProjectID = '{1}' and 检查者= '{2}' and Mapnumber='{3}' ", SampleerrorCollectionTableName, _sprojectid, CheckercomboBox2.Text, SamplecomboBox1.Text);

            }else
            {
                sql_queryitem = string.Format("select * from {0} where ProjectID = '{1}' and  Mapnumber='{2}' ", SampleerrorCollectionTableName, _sprojectid,  SamplecomboBox1.Text);

            }
            DataTable datatable0 = datareadwrite.GetDataTableBySQL(sql_queryitem);

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
        //打印样本统计表
        private void PrintSampleReportbutton1_Click(object sender, EventArgs e)
        {
            string szMapnumber = SamplecomboBox1.Text;
            if (szMapnumber == "")
            {
                MessageBox.Show("请选择图幅号!");
                return;
            }

            //打印质量问题统计表
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(CallClass.Configs["QualityErrorStatisticPrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));
            //未进行问题记录较多的分页处理
            PrintCheckRecordReport pr = new PrintCheckRecordReport();
            pr.Configs = CallClass.Configs;
            pr.Databases = CallClass.Databases;
            pr.AppPath = CallClass.AppPath;

            pr.PrintInit(pPrintEnvironment);
            try
            {
                string word = pr.PrintWord(szMapnumber,_CheckProject.ProjectID);
                if (File.Exists(word))
                    System.Diagnostics.Process.Start(word);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        //打印整个项目的问题记录表,如果选择了检查者，则进行检查者过滤
        private void PrintProjectReportbutton2_Click(object sender, EventArgs e)
        {
            if (SamplecomboBox1.Items.Count <= 0)
                return;

            string sProjectName = _CheckProject.ProjectName;
            //打印质量问题统计表
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(CallClass.Configs["QualityErrorStatisticPrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));

            string[] wordList = new string[SamplecomboBox1.Items.Count];
            int wordindex = 0;

            foreach (string szMapnumber in SamplecomboBox1.Items)
            {
                string WordFilePath = "";

                //未进行问题记录较多的分页处理
                PrintCheckRecordReport pr = new PrintCheckRecordReport();
                pr.Configs = CallClass.Configs;
                pr.Databases = CallClass.Databases;
                pr.AppPath = CallClass.AppPath;

                pr.PrintInit(pPrintEnvironment);
                try
                {
                    WordFilePath = pr.PrintWord(szMapnumber, _CheckProject.ProjectID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                if (File.Exists(WordFilePath))
                {
                    wordList[wordindex++] = WordFilePath;
                }
            }

            //记录第一个 
            string finalWordPath = "";
            try
            {
                if (wordindex > 1)
                {
                    string wordOrg = wordList[0];
                    string[] wordToMerge = new string[wordindex - 1];
                    Array.Copy(wordList, 1, wordToMerge, 0, wordindex - 1);
                    if (sProjectName.IndexOf(':') >= 0) sProjectName = sProjectName.Replace(':', '-');
                    finalWordPath = string.Format("{0}\\{1}{2}.doc", CallClass.AppPath, sProjectName, CallClass.Configs["CheckPointsTableName"]);
                    WordDocumentMerger dm = new WordDocumentMerger();
                    //逆序插入
                    Array.Reverse(wordToMerge);
                    dm.InsertMerge(wordOrg, wordToMerge, finalWordPath);
                }
                else
                {
                    finalWordPath = wordList[0];
                }
                // 做好清理工作
                if (wordindex > 1)
                {
                    foreach (string word in wordList)
                    {
                        if (File.Exists(word))
                            File.Delete(word);
                    }
                }
                string sMsg = string.Format("质检项目 ({0}) 的检查记录表报告成功导出，是否打开查看！", sProjectName);
                if (MessageBox.Show(sMsg, "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    System.Diagnostics.Process.Start(finalWordPath);
                };
            }
            catch (Exception ex)
            {
                string sMsg = string.Format("合并项目{0}报告时失败：{1}", sProjectName, ex.Message);
                MessageBox.Show(sMsg);
            }


        }

        //导出GDB格式的错漏数据集，便于在ArcGIS等软件叠加原始样本进行错漏修改确认
        private void btn_ExportErrorData_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK)
                return;
            
            foreach (string mapnumber in SamplecomboBox1.Items)
            {
                PinErrorCollection pinErrorCollection = new PinErrorCollection();
                pinErrorCollection.Read(_CheckProject.ProjectID, mapnumber);
                pinErrorCollection.Export(fbd.SelectedPath, _CheckProject);

            }            
        }

        //根据样本检查意见记录表和项目信息登记表中数学精度打分/粗差率的情况
        private void btn_EvluationSample_Click(object sender, EventArgs e)
        {
            string szMapnumber = SamplecomboBox1.Text;
            if (szMapnumber == "")
            {
                MessageBox.Show("请选择图幅号!");
                return;
            }

            SampleScorer.EvluationOfSample(_CheckProject, szMapnumber, CallClass.Configs, CallClass.Databases);

            //打印质量问题统计表
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(CallClass.Configs["QualityErrorOfSamplePrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));
            //未进行问题记录较多的分页处理
            PrintCheckRecordReport pr = new PrintCheckRecordReport();
            pr.Configs = CallClass.Configs;
            pr.Databases = CallClass.Databases;
            pr.AppPath = CallClass.AppPath;

            pr.PrintInit(pPrintEnvironment);
            try
            {
                string word = pr.PrintWord(szMapnumber, _CheckProject.ProjectID);
                if (File.Exists(word))
                    System.Diagnostics.Process.Start(word);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void btn_PrintReportOfWeiTu_Click(object sender, EventArgs e)
        {
            string szMapnumber = SamplecomboBox1.Text;
            if (szMapnumber == "")
            {
                MessageBox.Show("请选择图幅号!");
                return;
            }//打印质量问题统计表
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(CallClass.Configs["CheckProjectPrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));
            //未进行问题记录较多的分页处理
            PrintCheckRecordReport pr = new PrintCheckRecordReport();
            pr.Configs = CallClass.Configs;
            pr.Databases = CallClass.Databases;
            pr.AppPath = CallClass.AppPath;

            pr.PrintInit(pPrintEnvironment);
            try
            {
                string word = pr.PrintWord(szMapnumber, _CheckProject.ProjectID);
                if (File.Exists(word))
                    System.Diagnostics.Process.Start(word);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}

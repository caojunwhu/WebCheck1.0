using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System;
using System.Collections;
using System.Data;
using DataBaseDesign;
using Newtonsoft.Json;
using Eipsoft.Common;
using WordAddinSample;
using Authentication.Class;
using ReportPrinter;
using DatabaseDesignPlus;

namespace PluginUI
{
    public partial class ReportPrintButtonForm : Form
    {

        bool UserExit = false;
        //string LoginUserName = "";
        UserObject GlobleLoginUser;
        DLGCheckLib.DLGCheckProjectClass GlobleProject;

        DataTableDesign datatabledesign = null;
        DataTableDesign TopographicMapLocationAccuracyCheckDataTableDesign = null;
        string sourcrExcelFilePath = "";
        DataTable sourceDatatable = null;
        string _PrintWordType = "";
        string _sProjectName = "";
        string _sProjectFieldCode = "";
        string _sDbConnectionString = "";

        public ReportPrintButtonForm(/*List<Function> functions, Root root, string sLoginUserName*/UserObject oLoginUser,DLGCheckLib.DLGCheckProjectClass oProject, string sPrintWordType)
        {
            InitializeComponent();
            _PrintWordType = sPrintWordType;
            _sDbConnectionString = CallClass.Databases["SurveryProductCheckDatabase"];

            //初始化数据表设计
            try
            {

             /*   datatabledesign = new DataTableDesign(CallClass.Databases["CheckPointMeanErrorStatisticDataTableDesign"],
                    CallClass.Configs["DataTableDesignName"], CallClass.Configs["DataTableDesignVersion"], CallClass.Configs["DataTableDesignFieldAttributesName"], CallClass.Configs["ImportDataTableName"]);

                TopographicMapLocationAccuracyCheckDataTableDesign = new DataTableDesign(CallClass.Databases["TopographicMapLocationAccuracyCheckDataTableDesign"],
                    CallClass.Configs["TopographicMapLocationAccuracyCheckDataTableDesignName"], CallClass.Configs["DataTableDesignVersion"], CallClass.Configs["DataTableDesignFieldAttributesName"], CallClass.Configs["TopographicMapLocationAccuracyCheckDataTableName"]);
            */
                //1、 初始化参数
                datatabledesign = new DataTableDesign(_sDbConnectionString,  
                    "PostgreSQL",
                    CallClass.Configs["DataTableDesignName"], 
                    CallClass.Configs["DataTableDesignVersion"],
                    CallClass.Configs["DataTableDesignFieldAttributesName"],
                    _sDbConnectionString,
                    "PostgreSQL",
                    CallClass.Configs["DatatableDataTypeRelationDataTableName"]);
                //2、初始化数据表设计
                datatabledesign.InitializeDataTableDesign("PostgreSQL", CallClass.Configs["ImportDataTableName"]);

                TopographicMapLocationAccuracyCheckDataTableDesign = new DataTableDesign(_sDbConnectionString,
                    "PostgreSQL",
                    CallClass.Configs["TopographicMapLocationAccuracyCheckDataTableDesignName"], 
                    CallClass.Configs["DataTableDesignVersion"],
                    CallClass.Configs["DataTableDesignFieldAttributesName"],
                    _sDbConnectionString,
                    "PostgreSQL",
                    CallClass.Configs["DatatableDataTypeRelationDataTableName"]);
                TopographicMapLocationAccuracyCheckDataTableDesign.InitializeDataTableDesign("PostgreSQL", CallClass.Configs["TopographicMapLocationAccuracyCheckDataTableName"]);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            //读取模块配置，取得界面元素尺寸
            this.FormClosed += new FormClosedEventHandler(ButtonForm_FormClosed);
            this.FormClosing += new FormClosingEventHandler(ButtonForm_FormClosing);
            try
            {
                btn_lrmargin = int.Parse(System.Configuration.ConfigurationManager.AppSettings["LRMargin"]);
            }
            catch { }
            try
            {
                btn_height = int.Parse(System.Configuration.ConfigurationManager.AppSettings["Height"]);
            }
            catch { }
            try
            {
                btn_upmargin = int.Parse(System.Configuration.ConfigurationManager.AppSettings["UPMargin"]);
            }
            catch { }

           // r = root;
            this.StartPosition = FormStartPosition.CenterScreen;
            try
            {
                //this.Text = System.Configuration.ConfigurationManager.AppSettings["SystemName"];
                this.Text = sPrintWordType;
            }
            catch { }


            this.Load += new EventHandler(ButtonForm_Load);

            //增加用户登录信息，写入配置字典中
            //LoginUserName = sLoginUserName;
            GlobleLoginUser = oLoginUser;
            GlobleProject = oProject;
            //加入菜单和标签、下拉框等。
            //InitMenu(functions);
            InitControls(datatabledesign);

            ResetLayout();

            this.SizeChanged += new EventHandler(ButtonForm_SizeChanged);
        }

        void ButtonForm_SizeChanged(object sender, EventArgs e)
        {
            ResetLayout();
        }

        void ButtonForm_Load(object sender, EventArgs e)
        {
           // throw new NotImplementedException();
        }

        void ButtonForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!UserExit)
            {
                e.Cancel = !(MessageBox.Show(string.Format("确定退出{0}？",this.Text), "退出系统", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK);
            }
        }

        void ButtonForm_FormClosed(object sender, FormClosedEventArgs e)
        {
           // Application.Exit();
           
        }

        void InitControls(DataTableDesign dtdesign)
        {
            GroupBox g = new GroupBox { Text = dtdesign.CreateTableName,Tag = 1};
            this.Controls.Add(g);
            g.BackColor = Color.Transparent;
            AddControls(g, TopographicMapLocationAccuracyCheckDataTableDesign);

        }

        int btn_lrmargin = 4;
        int btn_height = 12;
        int btn_upmargin = 1;
        ComboBox cb1 = null;
        ComboBox cb2 = null;

        void AddControls(GroupBox group, DataTableDesign lTopographicMapLocationAccuracyCheckDataTableDesign)
        {
            group.Controls.Clear();

            int index = 1;
            foreach (FieldDesign field in lTopographicMapLocationAccuracyCheckDataTableDesign.IndetifyFieldIndexTarget)
            {
                Label lbl = new Label { Text = string.Format("{0}{1}", field.FieldName, field.FieldIsNull == "是" ? "*" : ""), Tag = field, Height = btn_height, Left = btn_lrmargin, Top = (index) * (btn_upmargin + btn_height) };
                new ToolTip().SetToolTip(lbl, field.FieldRemarks);
                group.Controls.Add(lbl);

                ComboBox cb = new ComboBox { Tag = field, Height = btn_height, Left = btn_lrmargin + group.Width / 2, Top = (index) * (btn_upmargin + btn_height) };
                //new ToolTip().SetToolTip(cb, field.FieldRemarks);
                cb.SelectedIndexChanged += new System.EventHandler(cb_selectindexchange);
                group.Controls.Add(cb);
                if (field.FieldIndex == 1)
                {
                    try
                    {
                        string sqlfillcb1 = string.Format("select distinct {0} from {1} where {2}='{3}' ", field.FieldCode, TopographicMapLocationAccuracyCheckDataTableDesign.CreateTableName, field.FieldCode,GlobleProject.ProjectName);
                        // Clsmdb mdb = new Clsmdb();
                        // List<string> items = mdb.GetSingleFieldValueList(field.FieldCode, sqlfillcb1, CallClass.Databases["SPIDatabaseFilePath"]);
                        IDatabaseReaderWriter dbreader = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _sDbConnectionString);
                        List<string> items = dbreader.GetSingleFieldValueList(field.FieldCode, sqlfillcb1);
                        //List<string> items = ClsPostgreSql.GetSingleFieldValueList(field.FieldCode, sqlfillcb1, _sDbConnectionString);
                        // mdb.FillCombox(items, cb);
                        DatabaseReaderWriterFactory.FillCombox(items, cb);
                        //ClsPostgreSql.FillCombox(items, cb);
                        _sProjectName = items[0]; ;
                        _sProjectFieldCode = field.FieldCode;
                        cb.SelectedIndex = 0;
                        cb1 = cb;
                    }
                    catch { }

                }
                else
                {
                    try
                    {
                        string sqlfillcb2 = string.Format("select distinct {0} from {1} where {2} = '{3}'", field.FieldCode, TopographicMapLocationAccuracyCheckDataTableDesign.CreateTableName, _sProjectFieldCode, _sProjectName);
                        //Clsmdb mdb = new Clsmdb();
                        IDatabaseReaderWriter dbreader = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _sDbConnectionString);
                        List<string> items = dbreader.GetSingleFieldValueList(field.FieldCode, sqlfillcb2);
                        //List<string> items = ClsPostgreSql.GetSingleFieldValueList(field.FieldCode, sqlfillcb2, _sDbConnectionString);
                        //List<string> items = mdb.GetSingleFieldValueList(field.FieldCode, sqlfillcb2, CallClass.Databases["SPIDatabaseFilePath"]);                        
                        items.Sort();
                        DatabaseReaderWriterFactory.FillCombox(items, cb);
                        //mdb.FillCombox(items, cb);
                        cb2 = cb;                                              
                    }
                    catch
                    {
                    }

                }
                index++;
            }

            foreach (FieldDesign field in lTopographicMapLocationAccuracyCheckDataTableDesign.TargetFieldsList)
            {
                if (field.FieldImportIDCode == "是") continue;
                
                Label lbl = new Label { Text = string.Format("{0}{1}", field.FieldName, field.FieldIsNull == "是" ? "*" : ""), Tag = field, Height = btn_height, Left = btn_lrmargin, Top = (index) * (btn_upmargin + btn_height) };
                //new ToolTip().SetToolTip(lbl, field.FieldRemarks);
                group.Controls.Add(lbl);

                TextBox tb = new TextBox { Tag = field, Height = btn_height, Left = btn_lrmargin + group.Width / 2, Top = (index) * (btn_upmargin + btn_height) };
                //new ToolTip().SetToolTip(cb, field.FieldRemarks);
                group.Controls.Add(tb);

                index++;
              
            }

            if (_PrintWordType == "检测点精度统计表打印")
            {

                Button btnExportSingleReport = new Button { Text = "打印本幅检测点中误差统计表", Name = "Btn_Left", Height = btn_height, Left = btn_lrmargin, Top = group.Height - btn_upmargin - btn_height, Width = Text.Length * 3 };
                group.Controls.Add(btnExportSingleReport);
                btnExportSingleReport.Click += new System.EventHandler(Btn_ExportOneClick);

                Button btnExportAllReports = new Button { Text = "打印项目检测点误差统计表", Name = "Btn_ExportAll", Height = btn_height, Left = btn_lrmargin + group.Width / 2, Top = group.Height - btn_upmargin - btn_height, Width = Text.Length * 3 };
                group.Controls.Add(btnExportAllReports);
                btnExportAllReports.Click += new System.EventHandler(Btn_ExportALLClick);

            }
            else if (_PrintWordType == "间距边长误差统计表打印")
            {
                Button btnExportSingleReport = new Button { Text = "打印本幅间距边长误差统计表", Name = "Btn_Left", Height = btn_height, Left = btn_lrmargin, Top = group.Height - btn_upmargin - btn_height, Width = Text.Length * 3 };
                group.Controls.Add(btnExportSingleReport);
                btnExportSingleReport.Click += new System.EventHandler(Btn_ExportRelativeClick);

                Button btnExportAllReports = new Button { Text = "打印项目间距边长误差统计表", Name = "Btn_ExportRelativeAll", Height = btn_height, Left = btn_lrmargin + group.Width / 2, Top = group.Height - btn_upmargin - btn_height, Width = Text.Length * 3 };
                group.Controls.Add(btnExportAllReports);
                btnExportAllReports.Click += new System.EventHandler(Btn_ExportALLRelativeClick);
            }
            else if (_PrintWordType == "检测中误差及得分计算")
            {
                Button btn1 = new Button { Text = "计算项目平面高程中误差及得分", Name = "Btn_Left", Height = btn_height, Left = btn_lrmargin, Top = group.Height - btn_upmargin - btn_height, Width = Text.Length * 3 };
                group.Controls.Add(btn1);
                btn1.Click += new System.EventHandler(Btn_CalcPositionHeightClick);

                Button btn2 = new Button { Text = "计算项目间距中误差及得分", Name = "Btn_CalcRelative", Height = btn_height, Left = btn_lrmargin + group.Width / 2, Top = group.Height - btn_upmargin - btn_height, Width = Text.Length * 3 };
                group.Controls.Add(btn2);
                btn2.Click += new System.EventHandler(Btn_CalcRelativeClick);
            }
            else if (_PrintWordType == "样本图幅检测精度统计表打印")
            {
                Button btn2 = new Button { Text = "打印样本图幅检测精度统计表",  Height = btn_height, Left = btn_lrmargin + group.Width / 2, Top = group.Height - btn_upmargin - btn_height, Width = Text.Length * 3 };
                group.Controls.Add(btn2);
                btn2.Click += new System.EventHandler(Btn_PrintCheckItemsQulityReport);
            }

            cb2.SelectedIndex = 0;  

        }

        void ResetLayout()
        {
            int g_topMargin = 20;

            int g_bottomMargin = 10;
            try
            {
                g_bottomMargin = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["GroupBottomMargin"]);
                if (g_bottomMargin < 1) g_bottomMargin = 10;
            }
            catch { }
            int g_lrMargin = 20;
            try
            {
                g_lrMargin = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["GroupLRMargin"]);
                if (g_lrMargin < 1) g_lrMargin = 20;
            }
            catch { }
            int g_upMargin = 10;
            try
            {
                g_upMargin = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["GroupUPMargin"]);
                if (g_upMargin < 1) g_upMargin = 10;
            }
            catch { }


            Control group = null;
             foreach (Control c in this.Controls)
            {

                if (c is GroupBox){group = c;                }
             }

            int g_width = Convert.ToInt32((this.Width - 2 * g_lrMargin-10) );
            this.Height = 20 + menuStrip1.Height + g_topMargin + g_bottomMargin + 5 + (group as GroupBox).Controls.Count * (btn_height+btn_upmargin) /4+60;

            int g_height = Convert.ToInt32((this.Height-20- menuStrip1.Height - g_topMargin - g_bottomMargin - 5) / 1.0);


            int top1 = g_topMargin + menuStrip1.Height;
            int top2 = g_topMargin + menuStrip1.Height + g_height + g_upMargin;

            foreach (Control c in this.Controls)
            {
                c.Width = g_width;
                c.Height = g_height;
                if (c is GroupBox)
                {
                    int index = int.Parse(c.Tag.ToString());
                    switch (index)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            c.Left = g_lrMargin * index + (index - 1) * g_width;
                            c.Top = top1;
                            break;
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                            c.Left = g_lrMargin * (index - 4) + (index - 5) * g_width;
                            c.Top = top2;
                            break;
                    }
                    InitControlsInGroup(c as GroupBox);
                }
                else if (c is ProgressBar)
                {
                    c.Top = menuStrip1.Top;
                    c.Height = menuStrip1.Height;
                }
            }
        }

        void InitControlsInGroup(GroupBox g)
         {
            //四个一排
             int colitemcount = (int)Math.Ceiling(TopographicMapLocationAccuracyCheckDataTableDesign.FieldDesigns.Count / 2.0);
            
             foreach (Control c in g.Controls)
             {
                 //FieldDesign field = c.Tag as FieldDesign;
                 //int controlindex = (field.FieldIndex - colitemcount) < 0 ? field.FieldIndex : (field.FieldIndex - colitemcount);

                 int row = g.Controls.IndexOf(c)  / 4 ;
                 int col = g.Controls.IndexOf(c)  - row * 4 ;

                 if (c is Label)
                 {                     
                     c.Width = g.Width / 4 - btn_lrmargin ;
                     c.Top = row * (btn_upmargin + btn_height) + btn_upmargin + btn_height;
                     c.Left = col * g.Width / 4+ btn_lrmargin ;
                 }
                 else if (c is TextBox)
                 {
                     c.Width = g.Width / 4 - btn_lrmargin;
                     c.Left = col * g.Width / 4 + btn_lrmargin;
                     c.Top = row * (btn_upmargin + btn_height) + btn_upmargin + btn_height;
                 }
                 else if (c is ComboBox)
                 {
                     c.Width = g.Width / 4 - btn_lrmargin;
                     c.Left = col * g.Width / 4 + btn_lrmargin;
                     c.Top = row * (btn_upmargin + btn_height) + btn_upmargin + btn_height;
                 }
                 else if (c is Button)
                 {
                     c.Top = g.Height - btn_upmargin - btn_height;
                     c.Width = g.Width / 2 - btn_lrmargin * 4;
                     if (c.Name == "Btn_Left")
                         c.Left = btn_lrmargin;
                     else
                         c.Left = g.Width / 2 + btn_lrmargin;
                 }

             }

         }

        void cb_selectindexchange(object sender, System.EventArgs e)
        {

            ComboBox cb = sender as ComboBox;
            FieldDesign field = cb.Tag as FieldDesign;
            if (field.FieldIndex == 1&&cb.Items.Count>0)
            {
                //触动组合框1,填充组合框2
                try
                {
                    _sProjectName = cb.Text;
                    FieldDesign field2 = cb2.Tag as FieldDesign;                    
                    string sqlfillcb2 = string.Format("select distinct {0} from {1} where {2} = '{3}'", field2.FieldCode, TopographicMapLocationAccuracyCheckDataTableDesign.CreateTableName, _sProjectFieldCode, _sProjectName);
                    //Clsmdb mdb = new Clsmdb();
                    //List<string> items = mdb.GetSingleFieldValueList(field2.FieldCode, sqlfillcb2, _sDbConnectionString);
                    IDatabaseReaderWriter dbreader = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _sDbConnectionString);
                    List<string> items = dbreader.GetSingleFieldValueList(field2.FieldCode, sqlfillcb2);
                    //List<string> items=ClsPostgreSql.GetSingleFieldValueList(field2.FieldCode, sqlfillcb2, _sDbConnectionString);
                    cb2.Items.Clear();
                    //mdb.FillCombox(items, cb2);
                    //ClsPostgreSql.FillCombox(items, cb2);
                    items.Sort();
                    DatabaseReaderWriterFactory.FillCombox(items, cb2);
                    cb2.SelectedIndex = 0;
                }
                catch
                {
                }
            }
            else if(field.FieldIndex == 7)
            {
                try
                {
                    FieldDesign field2 = cb2.Tag as FieldDesign;
                    string sqlfillTextBoxs = string.Format("select * from {0} where {1} = '{2}' and {3} = '{4}'", TopographicMapLocationAccuracyCheckDataTableDesign.CreateTableName, _sProjectFieldCode, _sProjectName, field2.FieldCode, cb2.Text);
                    //Clsmdb mdb = new Clsmdb();
                    IDatabaseReaderWriter dbreader = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _sDbConnectionString);
                    Dictionary<string, string> MultiplyFieldValue = dbreader.GetMultiplyFieldValueDictonary(sqlfillTextBoxs);
                    //Dictionary<string,string>MultiplyFieldValue = mdb.GetMultiplyFieldValueDictonary(sqlfillTextBoxs,_sDbConnectionString);
                    //Dictionary<string, string> MultiplyFieldValue = ClsPostgreSql.GetMultiplyFieldValueDictonary(sqlfillTextBoxs, _sDbConnectionString);

                    foreach (Control g in this.Controls)
                    {
                        if (g is GroupBox)
                        {
                            foreach (Control t in g.Controls)
                            {
                                if (t is TextBox)
                                {
                                    string fieldcode = (t.Tag as FieldDesign).FieldCode;
                                    t.Text = MultiplyFieldValue[fieldcode];
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                }
            }
        }

        void Btn_PrintCheckItemsQulityReport(object sender, System.EventArgs e)
        {

            string sProjectName = cb1.Text;
            string sMapNumber = cb2.Text;

            //打印中误差统计表
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(CallClass.Configs["PrintCheckItemsQulityReportEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));
            PrintQulityReport pr = new PrintQulityReport();
            pr.Configs = CallClass.Configs;
            pr.Databases = CallClass.Databases;
            pr.AppPath = CallClass.AppPath;

            pr.PrintInit(pPrintEnvironment);
            try
            {
                string word = pr.PrintWord(sProjectName);
                if (File.Exists(word))
                    System.Diagnostics.Process.Start(word);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

        }

        void Btn_ExportALLClick(object sender, System.EventArgs e)
        {
            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);

            string sProjectName = cb1.Text;       
            string sMapNumber = cb2.Text;
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(CallClass.Configs["CheckPointsErrorStatisticPrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));
            
            DataTable dt = null;
            try
            {
                string sqlGetMapNumbers = string.Format("select 图幅号 from {0} where 成果名称='{1}' order by 抽样分区 desc,流水号 desc", CallClass.Configs["TopographicMapLocationAccuracyCheckDataTableName"], sProjectName);
                //Clsmdb mdb = new Clsmdb();
                //dt = mdb.GetMdbDataTableBySql(CallClass.Databases["SPIDatabaseFilePath"], sqlGetMapNumbers);
                IDatabaseReaderWriter dbread = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _sDbConnectionString);
                dt = dbread.GetDataTableBySQL( sqlGetMapNumbers);
            }
            catch (Exception ex)
            {
                string sMsg = string.Format("项目{0}中图幅数为零，请检查是否已经正确导入检测点成果表！系统报错：{1}。提示：批量生成报告需要按抽样分区和流水号排序，流水号应为数字，不可以使用字母。", sProjectName, ex.Message);
                MessageBox.Show(sMsg);
                return;
            }

            string[] wordList = new string[dt.Rows.Count];
            int wordindex = 0;
            int istep = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string WordFilePath = "";
                try
                {
                    sMapNumber = Convert.ToString(dr["图幅号"]);
                   /* //计算中误差
                    PositionMeanError pme = new PositionMeanError();
                    pme.QueryParameter(sMapNumber);
                    pme.Calc(sMapNumber);
                    pme.UpdateReslut(sMapNumber);

                    HeightMeanError hme = new HeightMeanError();
                    hme.QueryParameter(sMapNumber);
                    hme.Calc(sMapNumber);
                    hme.UpdateReslut(sMapNumber);*/

                    istep++;
                    Invoke(showProgress, new object[] { dt.Rows.Count, istep });

                    PrintReport pr = new PrintReport();
                    pr.Databases = CallClass.Databases;
                    pr.Configs = CallClass.Configs;
                    pr.AppPath = CallClass.AppPath;

                    pr.PrintInit(pPrintEnvironment);
                    WordFilePath = pr.PrintWord(sMapNumber);

                }
                catch (Exception ex)
                {
                    string sMsg = string.Format("导出项目{0}图幅{1}报告失败：{2}", sProjectName,sMapNumber, ex.Message);
                    //MessageBox.Show(sMsg);
                    //return;
                    LogOut.Info(sMsg);
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
                string sMsg = string.Format("质检项目 ({0}) 的检测报告成功导出，是否打开查看！", sProjectName);
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

        void Btn_ExportOneClick(object sender, System.EventArgs e)
        {

            string sProjectName = cb1.Text;
            string sMapNumber = cb2.Text;

            //计算中误差
          /*  PositionMeanError pme = new PositionMeanError();
            pme.QueryParameter(sMapNumber);
            pme.Calc(sMapNumber);
            pme.UpdateReslut(sMapNumber);

            HeightMeanError hme = new HeightMeanError();
            hme.QueryParameter(sMapNumber);
            hme.Calc(sMapNumber);
            hme.UpdateReslut(sMapNumber);*/

            //打印中误差统计表
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(CallClass.Configs["CheckPointsErrorStatisticPrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));
            PrintReport pr = new PrintReport();
            pr.Databases = CallClass.Databases;
            pr.Configs = CallClass.Configs;
            pr.AppPath = CallClass.AppPath;

            pr.PrintInit(pPrintEnvironment);
            try
            {
                string word = pr.PrintWord(sMapNumber);
                if (File.Exists(word))
                    System.Diagnostics.Process.Start(word);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
              
        }

        void Btn_ExportRelativeClick(object sender, System.EventArgs e)
        {

            string sProjectName = cb1.Text;
            string sMapNumber = cb2.Text;

            //计算误差
           /* RelativeMeanError rme = new RelativeMeanError();
            rme.QueryParameter(sMapNumber);
            rme.Calc(sMapNumber);
            rme.UpdateReslut(sMapNumber);*/

            //打印误差统计表
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(CallClass.Configs["RelativeErrorStatisticPrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));
            PrintReport pr = new PrintReport();
            pr.Configs = CallClass.Configs;
            pr.Databases = CallClass.Databases;
            pr.AppPath = CallClass.AppPath;

            pr.PrintInit(pPrintEnvironment);
            try
            {
                string word = pr.PrintWord(sMapNumber);
                if (File.Exists(word))
                    System.Diagnostics.Process.Start(word);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

        }

        void Btn_ExportALLRelativeClick(object sender, System.EventArgs e)
        {
            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);

            string sProjectName = cb1.Text;       
            string sMapNumber = cb2.Text;
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(CallClass.Configs["RelativeErrorStatisticPrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));

            DataTable dt = null;
            try
            {
                string sqlGetMapNumbers = string.Format("select 图幅号 from {0} where 成果名称='{1}' order by 抽样分区 desc,流水号 desc", CallClass.Configs["TopographicMapLocationAccuracyCheckDataTableName"], sProjectName);
                //Clsmdb mdb = new Clsmdb();
                //dt = mdb.GetMdbDataTableBySql(CallClass.Databases["SPIDatabaseFilePath"], sqlGetMapNumbers);
                IDatabaseReaderWriter dbreader = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _sDbConnectionString);
                dt = dbreader.GetDataTableBySQL(sqlGetMapNumbers);
                //dt = ClsPostgreSql.GetDataTableBySql(_sDbConnectionString, sqlGetMapNumbers);
            }
            catch (Exception ex)
            {
                string sMsg = string.Format("项目{0}中图幅数为零，请检查是否已经正确导入检测点成果表！系统报错：{1}。提示：批量生成报告需要按抽样分区和流水号排序，流水号应为数字，不可以使用字母。", sProjectName, ex.Message);
                MessageBox.Show(sMsg);
                return;
            }

            string[] wordList = new string[dt.Rows.Count];
            int wordindex = 0;
            int istep = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string WordFilePath = "";
                try
                {
                    sMapNumber = Convert.ToString(dr["图幅号"]);
                    //计算误差
                    /*RelativeMeanError rme = new RelativeMeanError();
                    rme.QueryParameter(sMapNumber);
                    rme.Calc(sMapNumber);
                    rme.UpdateReslut(sMapNumber);*/


                    istep++;
                    Invoke(showProgress, new object[] { dt.Rows.Count, istep });

                    PrintReport pr = new PrintReport();
                    pr.Configs = CallClass.Configs;
                    pr.Databases = CallClass.Databases;
                    pr.AppPath = CallClass.AppPath;

                    pr.PrintInit(pPrintEnvironment);
                    WordFilePath = pr.PrintWord(sMapNumber);

                }
                catch (Exception ex)
                {
                    string sMsg = string.Format("导出项目{0}图幅{1}报告失败：{2}", sProjectName,sMapNumber, ex.Message);
                    //MessageBox.Show(sMsg);
                    //return;
                    LogOut.Info(sMsg);
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
                    finalWordPath = string.Format("{0}\\{1}{2}.doc", CallClass.AppPath, sProjectName, CallClass.Configs["RelativeCheckPointsTableName"]);
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
                string sMsg = string.Format("质检项目 ({0}) 的检测报告成功导出，是否打开查看！", sProjectName);
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

        void Btn_CalcPositionHeightClick(object sender, System.EventArgs e)
        {
            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);

            string sProjectName = cb1.Text;
            string sMapNumber = cb2.Text;
            DataTable dt = null;
            try
            {
                string sqlGetMapNumbers = string.Format("select 图幅号 from {0} where 成果名称='{1}' order by 抽样分区 desc,流水号 desc", CallClass.Configs["TopographicMapLocationAccuracyCheckDataTableName"], sProjectName);
                //Clsmdb mdb = new Clsmdb();
                //dt = mdb.GetMdbDataTableBySql(CallClass.Databases["SPIDatabaseFilePath"], sqlGetMapNumbers);
                IDatabaseReaderWriter dbreader = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _sDbConnectionString);

                dt = dbreader.GetDataTableBySQL(sqlGetMapNumbers);
            }
            catch (Exception ex)
            {
                string sMsg = string.Format("项目{0}中图幅数为零，请检查是否已经正确导入检测点成果表！系统报错：{1}。提示：批量生成报告需要按抽样分区和流水号排序，流水号应为数字，不可以使用字母。", sProjectName, ex.Message);
                MessageBox.Show(sMsg);
                return;
            }

            int istep = 0;
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    sMapNumber = Convert.ToString(dr["图幅号"]);
                    //计算中误差
                    PositionMeanError pme = new PositionMeanError(CallClass.Configs,CallClass.Databases);
                    pme.QueryParameter(sMapNumber);
                    pme.Calc(sMapNumber);
                    pme.UpdateReslut(sMapNumber);

                    HeightMeanError hme = new HeightMeanError(CallClass.Configs, CallClass.Databases);
                    hme.QueryParameter(sMapNumber);
                    hme.Calc(sMapNumber);
                    hme.UpdateReslut(sMapNumber);

                    istep++; 
                    Invoke(showProgress, new object[] { dt.Rows.Count, istep });

                }
                catch (Exception ex)
                {
                    string sMsg = string.Format("导出项目{0}图幅{1}报告失败：{2}", sProjectName, sMapNumber, ex.Message);
                    LogOut.Info(sMsg);
                }
            }
            if (MessageBox.Show("计算完毕，是否关闭窗口！", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            };
        }

        void Btn_CalcRelativeClick(object sender, System.EventArgs e)
        {
            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);

            string sProjectName = cb1.Text;
            string sMapNumber = cb2.Text;

            DataTable dt = null;
            try
            {
                string sqlGetMapNumbers = string.Format("select 图幅号 from {0} where 成果名称='{1}' order by 抽样分区 desc,流水号 desc", CallClass.Configs["TopographicMapLocationAccuracyCheckDataTableName"], sProjectName);
                //Clsmdb mdb = new Clsmdb();
                //dt = mdb.GetMdbDataTableBySql(CallClass.Databases["SPIDatabaseFilePath"], sqlGetMapNumbers);
                IDatabaseReaderWriter dbreader = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _sDbConnectionString);
                dt = dbreader.GetDataTableBySQL(sqlGetMapNumbers);
                //dt = ClsPostgreSql.GetDataTableBySql(_sDbConnectionString, sqlGetMapNumbers);
            }
            catch (Exception ex)
            {
                string sMsg = string.Format("项目{0}中图幅数为零，请检查是否已经正确导入检测点成果表！系统报错：{1}。提示：批量生成报告需要按抽样分区和流水号排序，流水号应为数字，不可以使用字母。", sProjectName, ex.Message);
                MessageBox.Show(sMsg);
                return;
            }
            int istep = 0;
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    sMapNumber = Convert.ToString(dr["图幅号"]);
                    //计算误差
                    RelativeMeanError rme = new RelativeMeanError(CallClass.Configs, CallClass.Databases);
                    rme.QueryParameter(sMapNumber);
                    rme.Calc(sMapNumber);
                    rme.UpdateReslut(sMapNumber);

                    istep++;
                    Invoke(showProgress, new object[] { dt.Rows.Count, istep });
                }
                catch (Exception ex)
                {
                    string sMsg = string.Format("导出项目{0}图幅{1}报告失败：{2}", sProjectName, sMapNumber, ex.Message);
                    LogOut.Info(sMsg);
                }
            }
            //MessageBox.Show("计算完毕，请关闭窗口！");
            if (MessageBox.Show("计算完毕，是否关闭窗口！", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            };

        }
        void invoke_ApplicationClosed(object sender, System.EventArgs e)
        {
            this.Show();
        }

        // 显示进度条的委托声明
        delegate void ShowProgressDelegate(int totalStep, int currentStep);

        // 显示进度条
        void ShowProgress(int totalStep, int currentStep)
        {
            if (currentStep == 1)
            {
                progressBar1.Visible = true;
            }
            else if (currentStep == totalStep)
            {
                progressBar1.Visible = false;
                
            }
            progressBar1.Maximum = totalStep;
            progressBar1.Value = currentStep;
            
        }

    }
}

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System;
using System.Collections;
using System.Data;
using DatabaseDesignPlus;
using Authentication.Class;

namespace PluginUI
{
    public partial class DataTableImportButtonForm : Form
    {

        bool UserExit = false;
        string LoginUserName = "";
        string localProjectid = "";

        DataTableDesign datatabledesign = null;
        string sourcrExcelFilePath = "";
        DataTable sourceDatatable = null;
        UserObject LoginUser;
        DatabaseDesignPlus.IDatabaseReaderWriter datareadwrite;

        public DataTableImportButtonForm(/*List<Function> functions, Root root, */UserObject oLoginUser,string sTypes,string projectid)
        {
            InitializeComponent();
            LoginUser = oLoginUser;
            localProjectid = projectid;
            //初始化数据表设计
            try
            {
                /////////////////////////////////////////////////////////////////////////
                datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", CallClass.Databases["SurveryProductCheckDatabase"]);

                string sDataTableDesignFieldAttributesNameJson = CallClass.Configs["DataTableDesignVersion"] == "1.3" ? CallClass.Configs["DataTableDesignFieldAttributesNamePlus"] : CallClass.Configs["DataTableDesignFieldAttributesName"];

                datatabledesign = new DataTableDesign(
                    CallClass.Databases["SurveryProductCheckDatabase"],//sDatatbaseDesignConnectionID
                    "PostgreSQL",//sDatabaseDesignType
                    CallClass.Configs["DataTableDesignName"], //sDataTableDesignTableName
                    CallClass.Configs["DataTableDesignVersion"],//sDataTableDesignVersion
                    sDataTableDesignFieldAttributesNameJson,//sDataTableDesignFieldAttributesNameJson
                    CallClass.Databases["SurveryProductCheckDatabase"],//sDataTypeRelationDatatbaseConnectionID
                    "PostgreSQL",//sDataTypeRelationDatabaseType
                    "数据类型关系表"//sDatatableDataTypeRelationDataTableName

                    );
                datatabledesign.InitializeDataTableDesign("PostgreSQL", CallClass.Configs["ImportDataTableName"]);

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
                btn_lrmargin = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["LRMargin"]);
            }
            catch { }
            try
            {
                btn_height = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["Height"]);
            }
            catch { }
            try
            {
                btn_upmargin = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["UPMargin"]);
            }
            catch { }

           // r = root;
            this.StartPosition = FormStartPosition.CenterScreen;
            try
            {
                //this.Text = System.Configuration.ConfigurationSettings.AppSettings["SystemName"];
                this.Text = sTypes;
            }
            catch { }


            this.Load += new EventHandler(ButtonForm_Load);

            //加入菜单和标签、下拉框等。
            //InitMenu(functions);
            InitControls(datatabledesign);
            //增加用户登录信息，写入配置字典中
            //LoginUserName = sLoginUserName;

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
            AddControls(g, dtdesign.TargetOrginFieldList);

        }

        int btn_lrmargin = 4;
        int btn_height = 15;
        int btn_upmargin = 2;

        void AddControls(GroupBox group, List<FieldDesign> fieldsOrgin)
        {
            group.Controls.Clear();
            foreach (FieldDesign field in fieldsOrgin)
            {
                int index = field.FieldIndex;
                Label lbl = new Label { Text = string.Format("{0}{1}",field.FieldName,field.FieldIsNull=="是"?"*":""), Tag = field, Height = btn_height, Left = btn_lrmargin, Top = (index) * (btn_upmargin + btn_height) };
                new ToolTip().SetToolTip(lbl, field.FieldRemarks);
                group.Controls.Add(lbl); 
                
                ComboBox cb = new ComboBox { Tag = field, Height = btn_height, Left = btn_lrmargin  + group.Width/2, Top = (index) * (btn_upmargin + btn_height) };
                new ToolTip().SetToolTip(cb, field.FieldRemarks);

                group.Controls.Add(cb);
            }

            Button btnOpenExcelFile = new Button { Text = "打开要导入的数据表", Name = "Btn_Open", Height = btn_height, Left = btn_lrmargin, Top = group.Height - btn_upmargin - btn_height, Width = Text.Length * 3 };
            group.Controls.Add(btnOpenExcelFile);
            //new ToolTip().SetToolTip(btnOpenExcelFile, "从选中的Excel格式的检测项目信息表中向系统数据库导入项目名称、抽样分区、流水号、图幅号、比例尺、地形、点位图上中误差限差（mm）、高程中误差限差（m）、间距中误差限差（m）、检测类型、检查者、检查日期；初次导入时插入记录，有重复点记录导入时，先按图幅和点号清除该点记录，然后插入更新。*项必填，非*项由系统自动补录。");
             btnOpenExcelFile.Click += new System.EventHandler(Btn_OpenClick);

            Button btnImportExcelFile = new Button { Text = "开始导入", Name = "Btn_Import", Height = btn_height, Left = btn_lrmargin + group.Width / 2, Top = group.Height - btn_upmargin - btn_height, Width = Text.Length * 3 };
            group.Controls.Add(btnImportExcelFile);
            //new ToolTip().SetToolTip(btnImportExcelFile, "从选中的Excel格式的检测项目信息表中向系统数据库导入项目名称、抽样分区、流水号、图幅号、比例尺、地形、点位图上中误差限差（mm）、高程中误差限差（m）、间距中误差限差（m）、检测类型、检查者、检查日期；初次导入时插入记录，有重复点记录导入时，先按图幅和点号清除该点记录，然后插入更新。*项必填，非*项由系统自动补录。");
            btnImportExcelFile.Click += new System.EventHandler(Btn_ImportClick);

        }
     /*    */




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
            //this.Height = 20 + menuStrip1.Height + g_topMargin + g_bottomMargin + 5 + (group as GroupBox).Controls.Count * (btn_height+btn_upmargin) /2+30;
            //int g_height = Convert.ToInt32((this.Height-20- menuStrip1.Height - g_topMargin - g_bottomMargin - 5) / 1.0);
            //int top1 = g_topMargin + menuStrip1.Height;
            //int top2 = g_topMargin + menuStrip1.Height + g_height + g_upMargin;

            this.Height = 20 + g_topMargin + g_bottomMargin + 5 + (group as GroupBox).Controls.Count * (btn_height + btn_upmargin) / 2 + 30;
            int g_height = Convert.ToInt32((this.Height - 20 - g_topMargin - g_bottomMargin - 5) / 1.0);
            int top1 = g_topMargin ;
            int top2 = g_topMargin + g_height + g_upMargin;

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
            }
        }

          void InitControlsInGroup(GroupBox g)
         {
             foreach (Control c in g.Controls)
             {
                 if (c is Label)
                 {
                     int index = (g.Controls.IndexOf(c) - 1)/2;
                     c.Width = g.Width / 2 - btn_lrmargin * 2;
                 }
                 else if (c is ComboBox)
                 {
                 int index = (g.Controls.IndexOf(c)-1)/2;
                 c.Width = g.Width/2 - btn_lrmargin ;
                 c.Left = g.Width / 2 + btn_lrmargin;
                 }
                 else if (c is Button)
                 {
                    c.Top = g.Height - btn_upmargin - btn_height;
                    c.Width = g.Width / 2 - btn_lrmargin * 2;
                    if (c.Name == "Btn_Open")
                        c.Left = btn_lrmargin;
                    else
                        c.Left = g.Width / 2 + btn_lrmargin;
                 }
             }

         }

          void Btn_OpenClick(object sender, System.EventArgs e)
          {
              //打开文件对话框，选择单幅地图检查结果表
              OpenFileDialog openFileDialog = new OpenFileDialog();
              string sFileName = "检测项目信息表.xls";
              string sheetname = "检测项目信息表";
              openFileDialog.DefaultExt = "xls";
              openFileDialog.Filter = "EXCEL 2003文件(*.XLS) |*.xls";
              //默然路径是系统当前路径 
              openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
              if (openFileDialog.ShowDialog() == DialogResult.OK)
              {
                  sFileName = openFileDialog.FileName;
                  sourcrExcelFilePath = sFileName;

                  IDatabaseReaderWriter dbreader = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("Excel", sFileName);
                  FrmDataTableSelector fselector = new FrmDataTableSelector(dbreader);
                if(fselector.ShowDialog()==DialogResult.OK)
                {
                    sheetname = fselector.SelectedTableName;
                } 

                  sourceDatatable = dbreader.GetDataTable(sheetname);

                  int cbSelectIndex = 0;
                  foreach (Control c in this.Controls)
                  {
                      if (c is GroupBox)
                      {
                          foreach (Control cb in c.Controls)
                          {
                              if (cb is ComboBox)
                              {
                                  FieldDesign field = cb.Tag as FieldDesign;
                                  if (field.FieldImportType == "可选")
                                  {
                                      (cb as ComboBox).Items.Add("");
                                      foreach (DataColumn dc in sourceDatatable.Columns)
                                      {
                                          (cb as ComboBox).Items.Add(dc.ColumnName);
                                          cbSelectIndex = 0;
                                      }
                                  }
                                  else
                                  {
                                      foreach (DataColumn dc in sourceDatatable.Columns)
                                      {
                                          (cb as ComboBox).Items.Add(dc.ColumnName);
                                          cbSelectIndex = field.FieldIndex - 1;
                                      }
                                  }
                                  (cb as ComboBox).SelectedIndex = cbSelectIndex;
                              }
                              
                          }
                      }
                  }
              }
          }

          void Btn_ImportClick(object sender, System.EventArgs e)
          {
              try
              {

                  List<string> sourceFieldNameList = new List<string>();
                  List<string> sourceIndetifyFiedlnameList = new List<string>();
                  foreach (Control c in this.Controls)
                  {
                      if (c is GroupBox)
                      {
                          foreach (Control cb in c.Controls)
                          {
                              if (cb is ComboBox)
                              {
                                  sourceFieldNameList.Add(cb.Text);
                                  FieldDesign field = cb.Tag as FieldDesign;
                                  if (field.FieldImportIDCode == "是")
                                      sourceIndetifyFiedlnameList.Add(cb.Text);
                              }
                          }
                      }
                  }

                  foreach (string code in datatabledesign.TargetAddedFieldCodeList)
                  {
                      sourceFieldNameList.Add(code);
                  }
                foreach(DataRow dr in sourceDatatable.Rows)
                {
                    foreach(string code in datatabledesign.TargetAddedFieldCodeList)
                    {
                        if (code.ToLower().IndexOf("projectid")>=0)
                        {
                            if (sourceDatatable.Columns.IndexOf("projectid") < 0)
                                sourceDatatable.Columns.Add(new DataColumn("projectid"));
                            dr["projectid"] = localProjectid;
                        }
                    }
                }

                  IDatabaseReaderWriter dbWriter = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", CallClass.Databases["SurveryProductCheckDatabase"]);

                  //bool bsuccess = dbWriter.ImportDataTableRecords(datatabledesign.CreateTableName,
                  //    datatabledesign.TargetFieldsCodeList, datatabledesign.TargetAddedFieldCodeList, 
                  //    datatabledesign.IndetifyFieldCodeIndexTarget, sourceDatatable, sourceFieldNameList, sourceIndetifyFiedlnameList);
                  bool bsuccess = dbWriter.ImportDataTableRecords(datatabledesign.CreateTableName, sourceDatatable, datatabledesign, 1);

                  if (bsuccess == true)
                  {
                      this.Close();
                  }
              }
              catch (Exception ex)
              {
                  string faultmessage = string.Format("数据还未导入\"{0}\"；系统报错：{1}!", datatabledesign.CreateTableName, ex.Message);
                  MessageBox.Show(faultmessage); //                 throw new Exception(faultmessage);

              }


          }

        void invoke_ApplicationClosed(object sender, System.EventArgs e)
        {
            this.Show();
        }

        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show("确定退出该系统？", "退出系统", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                UserExit = true;
                Application.Exit();
            }
            else
            { return; }
        }
    }
}

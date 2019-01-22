using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System;
using System.Collections;
using System.Data;
using DatabaseDesignPlus;
using Core;
using Newtonsoft.Json;
using Authentication.Class;
using DLGCheckLib;

namespace PluginUI
{
    public partial class ButtonFormProjectImport : Form
    {

        bool UserExit = false;
        //string LoginUserName = "";
        UserObject GlobleLoginUser;
        DLGCheckProjectClass GlobleProject;

        DataTableDesign datatabledesign = null;
        string sourcrExcelFilePath = "";
        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        DataTable sourceDatatable = null;
        Root _root;
        Function _function = null;
        Dictionary<string, string> Configs = new Dictionary<string, string>();
        Dictionary<string, string> Databases = new Dictionary<string, string>();
        DatabaseDesignPlus.IDatabaseReaderWriter datareadwrite;

        public ButtonFormProjectImport(List<Function> functions, Root root, UserObject oLoginUser, DLGCheckProjectClass oProject)
        {
            InitializeComponent();

            GlobleLoginUser = oLoginUser;
            GlobleProject = oProject;
            //初始化数据表设计
            try
            {//////////////////////////////////////////////////////////////////////////////////////
                _root = root;
                foreach (Function func in functions[0].Functions)
                {
                    if (func.Tile == "新建检测项目信息")
                    {
                        _function = func;
                    }
                }
                ////////////////////////////////////////////////////////////////////////////////////
                foreach (Config c in _function.Configs)
                {
                    Configs.Add(c.Key, c.Value);
                }

                foreach (RefDatabase r in _function.RefDatabases)
                {
                    foreach (Database d in _root.Databases)
                    {
                        if (d.Key.ToUpper() == r.Key.ToUpper())
                        {
                            Databases.Add(d.Key, d.Value.Replace("{*}", Application.StartupPath));
                            continue;
                        }
                    }
                }

             /*  */               
                /////////////////////////////////////////////////////////////////////////
                datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", Databases["SurveryProductCheckDatabase"]);
                datatabledesign = new DataTableDesign(
                    Databases["SurveryProductCheckDatabase"],//sDatatbaseDesignConnectionID
                    "PostgreSQL",//sDatabaseDesignType
                    Configs["DataTableDesignName"], //sDataTableDesignTableName
                    Configs["DataTableDesignVersion"],//sDataTableDesignVersion
                    Configs["DataTableDesignFieldAttributesNamePlus"],//sDataTableDesignFieldAttributesNameJson
                    Databases["SurveryProductCheckDatabase"],//sDataTypeRelationDatatbaseConnectionID
                    "PostgreSQL",//sDataTypeRelationDatabaseType
                    "数据类型关系表"//sDatatableDataTypeRelationDataTableName
                    
                    );
                datatabledesign.InitializeDataTableDesign("PostgreSQL", Configs["ImportDataTableName"]);
                
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

            _root = root;
            this.StartPosition = FormStartPosition.CenterScreen;
            try
            {
                //this.Text = System.Configuration.ConfigurationSettings.AppSettings["SystemName"];
                //模块的标题从func中提取
                this.Text = _function.Tile;
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
            //1、检查“检测项目信息表”是否存在，不存在的情况需要创建
            List<string> tablenames = datareadwrite.GetSchameDataTableNames();
            string projecttablename = "检测项目信息表";
            if (tablenames.IndexOf(projecttablename) < 0)
            {
                string createslq = datatabledesign.GetCreateTableSQL(projecttablename);
                datareadwrite.ExecuteSQL(createslq);
            }

            string sql = "select distinct 属性类别 from 检测项目信息表设计 where 导入类型='必须'";
            //List<string> groupboxNames = ClsPostgreSql.GetSingleFieldValueList("属性类别", sql, dtdesign.DatatableDesignDbReader.DatatbaseConnectionID);
            List<string> groupboxNames = datareadwrite.GetSingleFieldValueList("属性类别", sql);
            int gindex = 0;
            foreach (string gname in groupboxNames)
            {
                GroupBox g = new GroupBox { Text = gname, Tag = ++gindex };
                this.Controls.Add(g);
                g.BackColor = Color.Transparent;

                string sqlinput = string.Format("select 名称,属性来源,输入框  from 检测项目信息表设计 where 属性类别='{0}'", gname);
                AddControls(g, dtdesign.TargetOrginFieldList);
            }

        }

        int btn_lrmargin = 4;
        int btn_height = 15;
        int btn_upmargin = 2;

        void AddControls(GroupBox group, List<FieldDesign> fieldsOrgin)
        {
            group.Controls.Clear();
            int gcindex = 0;
            foreach (FieldDesign field in fieldsOrgin)
            {
                int index = field.FieldIndex;
                Label lbl = new Label { Text = string.Format("{0}{1}", field.FieldName, field.FieldIsNull == "是" ? "*" : ""), Tag = field, Height = btn_height, Left = btn_lrmargin, Top =  (index) * (btn_upmargin + btn_height) };
                new ToolTip().SetToolTip(lbl, field.FieldRemarks);
                group.Controls.Add(lbl);

                Control ctrl = null;
                if (field.FieldInput == "CombBox")
                {
                    ComboBox cb = new ComboBox { Tag = field, Height = btn_height, Left = btn_lrmargin + group.Width / 2, Top = index * (btn_upmargin + btn_height) };
                    new ToolTip().SetToolTip(cb, field.FieldRemarks);
                    ctrl = cb;
                    group.Controls.Add(ctrl);
                    //////////            
                    JsonSerializer serializer = new JsonSerializer();
                    StringReader sr = null;
                    sr = new StringReader(field.FieldSource);
                    DataValueQuery dq = (DataValueQuery)serializer.Deserialize(new JsonTextReader(sr), typeof(DataValueQuery));
                    dq.DbFilePath = datatabledesign.DatatableDesignDbReader.DatatbaseConnectionID;
                    //ClsPostgreSql.FillCombox(dq.QueryValue as DataTable, field.FieldCode, cb);
                    DatabaseReaderWriterFactory.FillCombox(dq.QueryValue as DataTable, field.FieldCode, cb);
                    if(cb.Items.Count>0)
                        cb.SelectedIndex = 0;
                }
                else if (field.FieldInput == "TextBox")
                {
                    TextBox tb = new TextBox { Tag = field, Height = btn_height, Left = btn_lrmargin + group.Width / 2, Top = index * (btn_upmargin + btn_height) };
                    group.Controls.Add(tb);
                   
                }
                
            }

            Button btnImportExcelFile = new Button { Text = "点击按钮新建项目", Name = "Btn_Import", Height = btn_height, Left = btn_lrmargin + group.Width / 2, Top = group.Height - btn_upmargin - btn_height, Width = Text.Length * 3 };
            group.Controls.Add(btnImportExcelFile);
            btnImportExcelFile.Click += new System.EventHandler(Btn_NewProjectClick);

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

                if (c is GroupBox){group = c;}
             }

            int g_width = Convert.ToInt32((this.Width - 2 * g_lrMargin-10) );
            //this.Height = 20 + menuStrip1.Height + g_topMargin + g_bottomMargin + 5 + (group as GroupBox).Controls.Count * (btn_height+btn_upmargin) /2+30;

            //int g_height = Convert.ToInt32((this.Height-20- menuStrip1.Height - g_topMargin - g_bottomMargin - 5) / 1.0);


            //int top1 = g_topMargin + menuStrip1.Height;
            //int top2 = g_topMargin + menuStrip1.Height + g_height + g_upMargin;
            //考虑到增加了按钮，控件行数应增加1
            this.Height = 20 + g_topMargin + g_bottomMargin + 5 + (group as GroupBox).Controls.Count * ((btn_height + btn_upmargin) / 2+1)  ;

            int g_height = Convert.ToInt32((this.Height -20  - g_topMargin - g_bottomMargin - 5) / 1.0);


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
                            c.Left = g_lrMargin * index + (index - 1) * g_width;
                            c.Top = top1;
                            break;
                        case 3:
                        case 4:
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
                 else if (c is TextBox)
                 {
                     int index = (g.Controls.IndexOf(c) - 1) / 2;
                     c.Width = g.Width / 2 - btn_lrmargin;
                     c.Left = g.Width / 2 + btn_lrmargin;
                 }
                 else if (c is Button)
                 {
                     c.Top = g.Height - btn_upmargin - btn_height;
                     c.Width = g.Width / 2 - btn_lrmargin * 2;
                     if (c.Name == "Btn_Import")
                         c.Left = btn_lrmargin;
                     else
                         c.Left = g.Width / 2 + btn_lrmargin;
                 }
             }

         }        
          void Btn_NewProjectClick(object sender, System.EventArgs e)
          {
              try
              {
                  string projectitemdirectory = "";
                  string projectname = "";
                  Dictionary<FieldDesign, string> targetFieldValueDic = new Dictionary<FieldDesign, string>();
                  foreach (Control c in this.Controls)
                  {
                      if (c is GroupBox && c.Text == "抽样信息")
                      {
                          foreach (Control cb in c.Controls)
                          {
                              if (cb is ComboBox || cb is TextBox)
                              {
                                  targetFieldValueDic.Add(cb.Tag as FieldDesign, cb.Text);
                                  if ((cb.Tag as FieldDesign).FieldCode == "样本目录")
                                  {
                                      projectitemdirectory = cb.Text;
                                  }
                                  if((cb.Tag as FieldDesign).FieldCode == "成果名称")
                                  {
                                      projectname = cb.Text;
                                  }
                              }
                          }
                      }
                  }

                  ///////////////////////////////////////////////
                  // 使用ogr pg driver 打开图层创建要素
                /*  OSGeo.OGR.Ogr.RegisterAll();
                  OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
                  //为了使属性表字段支持中文，请添加下面这句SHAPE_ENCODING
                  //UTF-8 to ISO-8859-1.
                  OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", ""); 
                  
                  OSGeo.OGR.Driver driver = OSGeo.OGR.Ogr.GetDriverByName("PostgreSQL");

                  OSGeo.OGR.DataSource datasource = driver.Open(@"PG:dbname=SurveryProductCheckDatabase user=postgres password=123456",1);
                  //OSGeo.OGR.Layer layer = datasource.GetLayerByName("检测项目信息表");
                  OSGeo.OGR.Layer layer = datasource.GetLayerByName("polygon");

                  OSGeo.OGR.Feature feature = null;
                  layer.GetFeature(
                  layer.CreateFeature(feature);
                  */
                  //////////////////////////////////////////////////
                    string insertProjectSqlColum = "insert into 检测项目信息表(";
                    string insertProjectSqlValues = ")values(";
                  foreach (KeyValuePair<FieldDesign,string> code in targetFieldValueDic)
                  {
                      insertProjectSqlColum += code.Key.FieldCode+",";
                      switch (code.Key.FieldType)
                      {
                          case "varchar":
                              {
                                  insertProjectSqlValues += "'"+ code.Value + "',";
                              }
                              break;
                          case "text":
                              {
                                  insertProjectSqlValues += "'" + code.Value + "',";
                              }
                              break;
                          case "date":
                              {
                                  insertProjectSqlValues += "'" + code.Value + "',";
                              }
                              break;
                          case "integer":
                              {
                                  insertProjectSqlValues +=Convert.ToString(code.Value) + ",";
                              }
                              break;
                      }
                  }
                  foreach (string fd in datatabledesign.TargetAddedFieldCodeList)
                  {
                      switch (fd)
                      {
                          case "ogc_fid":
                              {
                                  DataValueQuery dq = new DataValueQuery();
                                  dq.QueryFields = "count(ogc_fid)+1";
                                  dq.QueryCondition = "from 检测项目信息表";
                                  dq.QueryValueType = "Integer";
                                  dq.DbFilePath = datatabledesign.DatatableDesignDbReader.DatatbaseConnectionID;
                                  insertProjectSqlColum += fd + ",";
                                  insertProjectSqlValues += Convert.ToString(dq.QueryValue) + ",";
                              }
                              break;
                          case "wkb_geometry":
                              {
                                  //2015年11月7日注释,进行代码整合测试
                            /*      ClsTK tk = new ClsTK(projectitemdirectory);
                                  //tk.GetTKUnion();
                                  insertProjectSqlColum += fd + ",";
                                  byte[] buffer = new byte[tk.TKUnion.WkbSize()];
                                  tk.TKUnion.ExportToWkb(buffer);
                                  string hexstr = CommonUtil.ByteArrayToString(buffer);
                                  insertProjectSqlValues += "'" + hexstr + "',";
                             * */
                              }
                              break;
                          case "任务创建时间":
                              {
                                  insertProjectSqlColum += fd + ",";
                                  insertProjectSqlValues +="'"+ DateTime.Now.Date.ToShortDateString() +"',";
                              }
                              break;
                          case "任务创建者":
                              {
                                  insertProjectSqlColum += fd + ",";
                                  insertProjectSqlValues += "'" + GlobleLoginUser.username + "',";
                              }
                              break;
                      }
                  }

                  string insertsql = insertProjectSqlColum.Remove(insertProjectSqlColum.Length - 1, 1) + insertProjectSqlValues.Remove(insertProjectSqlValues.Length - 1, 1)+")";

                  
                  if ( datareadwrite.ExecuteSQL(insertsql)== 1)
                  {
                      MessageBox.Show("成功建立质检项目！");
                  }

                /*  Clsmdb mdb = new Clsmdb();
                  bool bsuccess = mdb.ImportDataTableRecords(CallClass.Databases["SPIDatabaseFilePath"], datatabledesign.CreateTableName,
                      datatabledesign.TargetFieldsCodeList, datatabledesign.TargetAddedFieldCodeList, datatabledesign.CreateTableSQL,
                      datatabledesign.IndetifyFieldCodeIndexTarget, sourceDatatable, sourceFieldNameList, sourceIndetifyFiedlnameList);*/

                  /*
                  bool bsuccess = ClsPostgreSql.ImportDataTableRecords(CallClass.Databases["SurveryProductCheckDatabase"], datatabledesign.CreateTableName,
                      datatabledesign.TargetFieldsCodeList, datatabledesign.TargetAddedFieldCodeList, datatabledesign.CreateTableSQL,
                      datatabledesign.IndetifyFieldCodeIndexTarget, sourceDatatable, sourceFieldNameList, sourceIndetifyFiedlnameList);
                  
                  if (bsuccess == true)
                  {
                      this.Close();
                  }*/
              }
              catch (Exception ex)
              {
                  string faultmessage = string.Format("数据还未导入\"{0}\"；系统报错：{1}!", datatabledesign.DatatableDesignDbReader.DatatbaseConnectionID,ex.Message);
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

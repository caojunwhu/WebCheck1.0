using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Authentication.Class;
using DatabaseDesignPlus;
using System.Data;
using System.Drawing;
using System.IO;
using Core;

namespace WebMapCheck
{
    public partial class NewProject : System.Web.UI.Page
    {
        bool UserExit = false;
        string LoginUserName = "";

        DataTableDesign datatabledesign = null;
        string sourcrExcelFilePath = "";
        DataTable sourceDatatable = null;
        UserObject LoginUser;
        DatabaseDesignPlus.IDatabaseReaderWriter datareadwrite;
        internal static Dictionary<string, string> Configs { get; private set; }
        internal static Dictionary<string, string> Databases { get; private set; }
        FileUpload fileUpload;
        string webFilePath;
        Root root;
        string rootvirtualpath = "~/";
        string userid;
        string functiontype;
        protected void Page_Load(object sender, EventArgs e)
        {
            userid = Session["userid"] as string;
            if (userid == "" || userid == null)
            {
                //MessageBox.Show("您当前用户名在本机还未授权，请申请授权或等待管理员授权！");
                //ClientScript.RegisterStartupScript(this.GetType(), "温馨提示", "alert('提示：请您登录本系统后在查看页面！');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "opennewwindow", "alert('提示：请您登录本系统后在查看页面！');", true);
                //Response.Redirect("~/Default.aspx");
                return;
            }

            functiontype = Request["id"];
            root = Root.FromXML(Server.MapPath(rootvirtualpath) + "Functions.xml");

            Function printfunc = root.GetFunction(functiontype);
            Configs = new Dictionary<string, string>();
            foreach (Config cfig in printfunc.Configs)
            {
                Configs.Add(cfig.Key, cfig.Value);
            }
            Databases = new Dictionary<string, string>();
            foreach (Database rdbs in root.Databases)
            {
                Databases.Add(rdbs.Key, rdbs.Value.Replace(@"{*}\", Server.MapPath(rootvirtualpath)));
            }

            if (!Page.IsPostBack)
            {
                PrepareUI();
            }
            else
            {
                PrepareUI();
            }

        }

        void PrepareUI()
        {
            //Configs = new Dictionary<string, string>();
            //Databases = new Dictionary<string, string>();
            
            /*
            Databases["SurveryProductCheckDatabase"] = "Server=localhost;Port=5432;Database=SurveryProductCheckDatabase;uid=postgres;password=123456;";
            Configs["DataTableDesignVersion"] = "1.2";
            Configs["DataTableDesignFieldAttributesNamePlus"] = "{'FieldIndex':'序号','FieldName':'名称','FieldCode':'代码','FieldClass':'属性类别','FieldSource':'属性来源','FieldInput':'输入框','FieldType':'类型','FieldLength':'长度','FieldPrecision':'小数位数','FieldValue':'值域','FieldIsNull':'是否必填','FieldRemarks':'说明','FieldImportType':'导入类型','FieldImportIDCode':'导入识别码'}";
            Configs["DataTableDesignFieldAttributesName"] = "{'FieldIndex':'序号','FieldName':'名称','FieldCode':'代码','FieldType':'类型','FieldLength':'长度','FieldPrecision':'小数位数','FieldValue':'值域','FieldIsNull':'是否必填','FieldRemarks':'说明','FieldImportType':'导入类型','FieldImportIDCode':'导入识别码'}";
            Configs["DataTableDesignName"] = "位置精度检测项目信息表设计";
            Configs["ImportDataTableName"] = "位置精度检测项目信息表";
            */

            /////////////////////////////////////////////////////////////////////////
            datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", Databases["SurveryProductCheckDatabase"]);

            string sDataTableDesignFieldAttributesNameJson = Configs["DataTableDesignVersion"] == "1.3" ? Configs["DataTableDesignFieldAttributesNamePlus"] : Configs["DataTableDesignFieldAttributesName"];

            datatabledesign = new DataTableDesign(
                Databases["SurveryProductCheckDatabase"],//sDatatbaseDesignConnectionID
                "PostgreSQL",//sDatabaseDesignType
                Configs["DataTableDesignName"], //sDataTableDesignTableName
                Configs["DataTableDesignVersion"],//sDataTableDesignVersion
                sDataTableDesignFieldAttributesNameJson,//sDataTableDesignFieldAttributesNameJson
                Databases["SurveryProductCheckDatabase"],//sDataTypeRelationDatatbaseConnectionID
                "PostgreSQL",//sDataTypeRelationDatabaseType
                "数据类型关系表"//sDatatableDataTypeRelationDataTableName

                );
            datatabledesign.InitializeDataTableDesign("PostgreSQL", Configs["ImportDataTableName"]);

            InitControls(datatabledesign);
        }
        
        void InitControls(DataTableDesign dtdesign)
        {
            //GroupBox g = new GroupBox { Text = dtdesign.CreateTableName, Tag = 1 };
            //this.Controls.Add(g);
            //g.BackColor = Color.Transparent;
            AddControls(Panel1, dtdesign.TargetOrginFieldList);

        }
        int btn_lrmargin = 4;
        int btn_height = 30;
        int btn_upmargin = 2;
        void AddControls(Panel p, List<FieldDesign> fieldsOrgin)
        {
            p.Controls.Clear();
            foreach (FieldDesign field in fieldsOrgin)
            {
                int index = field.FieldIndex;
                Label lbl = new Label { Text = string.Format("{0}{1}", field.FieldName, field.FieldIsNull == "是" ? "*" : ""),Width=180,Height = btn_height-5  };
                //new ToolTip().SetToolTip(lbl, field.FieldRemarks);
                p.Controls.Add(lbl);

                DropDownList ddl = new DropDownList { Width = 200, ID = field.FieldName, Height = btn_height-5 };
                //new ToolTip().SetToolTip(cb, field.FieldRemarks);

                p.Controls.Add(ddl);

                //添加按钮后，界面换行再继续添加
                p.Controls.Add(new Literal() { Text = "<br />" });
            }

            //添加按钮前，再增加一行
            p.Controls.Add(new Literal() { Text = "<br />" });

            //添加上传工具
            fileUpload = new FileUpload { ID = "fileUpload1", Height = btn_height };
            p.Controls.Add(fileUpload);

            // 两个按钮间增加一个table空格
            p.Controls.Add(new Literal() { Text = "     " });

            Button btnOpenExcelFile = new Button { Text = "上传数据表", ID = "Btn_Open", Height = btn_height };
            p.Controls.Add(btnOpenExcelFile);
            //new ToolTip().SetToolTip(btnOpenExcelFile, "从选中的Excel格式的检测项目信息表中向系统数据库导入项目名称、抽样分区、流水号、图幅号、比例尺、地形、点位图上中误差限差（mm）、高程中误差限差（m）、间距中误差限差（m）、检测类型、检查者、检查日期；初次导入时插入记录，有重复点记录导入时，先按图幅和点号清除该点记录，然后插入更新。*项必填，非*项由系统自动补录。");
            btnOpenExcelFile.Click += new System.EventHandler(Btn_OpenClick);

            // 两个按钮间增加一个table空格
            p.Controls.Add(new Literal() { Text = "     " });

            Button btnImportExcelFile = new Button { Text = "开始导入", ID = "Btn_Import", Height = btn_height, };
            p.Controls.Add(btnImportExcelFile);
            //new ToolTip().SetToolTip(btnImportExcelFile, "从选中的Excel格式的检测项目信息表中向系统数据库导入项目名称、抽样分区、流水号、图幅号、比例尺、地形、点位图上中误差限差（mm）、高程中误差限差（m）、间距中误差限差（m）、检测类型、检查者、检查日期；初次导入时插入记录，有重复点记录导入时，先按图幅和点号清除该点记录，然后插入更新。*项必填，非*项由系统自动补录。");
            btnImportExcelFile.Click += new System.EventHandler(Btn_ImportClick);

        }

        void Btn_OpenClick(object sender, System.EventArgs e)
        {
            #region 单机版
            //打开文件对话框，选择单幅地图检查结果表
            /* OpenFileDialog openFileDialog = new OpenFileDialog();
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
             }*/
            #endregion
            #region 网络版电子表格上传，有文件类型、大小限制
            if (fileUpload.HasFile)
            {
                string upPath = "/up/";  //上传文件路径
                int upLength = 5;        //上传文件大小
                string upFileType = "|application/vnd.ms-excel|";//application/vnd.openxmlformats-officedocument.spreadsheetml.sheet|

                string fileContentType = fileUpload.PostedFile.ContentType;    //文件类型

                if (upFileType.IndexOf(fileContentType.ToLower()) > 0)
                {
                    string name = fileUpload.PostedFile.FileName;                  // 客户端文件路径

                    FileInfo file = new FileInfo(name);

                    string fileName = DateTime.Now.ToString("yyyyMMddhhmmssfff") + file.Extension; // 文件名称，当前时间（yyyyMMddhhmmssfff）
                    webFilePath = Server.MapPath(upPath) + fileName;        // 服务器端文件路径

                    string FilePath = upPath + fileName;   //页面中使用的路径

                    if (!File.Exists(webFilePath))
                    {
                        if ((fileUpload.FileBytes.Length / (1024 * 1024)) > upLength)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "upfileOK", "alert('大小超出 " + upLength + " M的限制，请处理后再上传！');", true);
                            return;
                        }

                        try
                        {
                            fileUpload.SaveAs(webFilePath);                                // 使用 SaveAs 方法保存文件

                            ClientScript.RegisterStartupScript(this.GetType(), "upfileOK", "alert('提示：文件上传成功');", true);
                        }
                        catch (Exception ex)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "upfileOK", "alert('提示：文件上传失败" + ex.Message + "');", true);
                            return;
                        }
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "upfileOK", "alert('提示：文件已经存在，请重命名后上传');", true);
                        return;
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "upfileOK", "alert('提示：文件类型不符" + fileContentType + "');", true);
                    return;
                }
            }
            #endregion 

            IDatabaseReaderWriter dbreader = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("Excel", webFilePath);

            //默认获取第一个表名称
            string sheetname = dbreader.GetSchameDataTableNames()[0];//"检测项目信息表"
            sourceDatatable = dbreader.GetDataTable(sheetname);

            //本次打开的excel表数据存储到session中
            Session["sourceDatatable"] = sourceDatatable;

            int cbSelectIndex = 0;
            foreach (Control cb in Panel1.Controls)
            {
                if (cb is DropDownList)
                {
                    FieldDesign field = DataTableDesign.FindFieldDesign(  datatabledesign.TargetOrginFieldList,(cb as DropDownList).ID);
                    if (field.FieldImportType == "可选")
                    {
                        (cb as DropDownList).Items.Add("");
                        foreach (DataColumn dc in sourceDatatable.Columns)
                        {
                            (cb as DropDownList).Items.Add(dc.ColumnName);
                            cbSelectIndex = 0;
                        }
                    }
                    else
                    {
                        foreach (DataColumn dc in sourceDatatable.Columns)
                        {
                            (cb as DropDownList).Items.Add(dc.ColumnName);
                            cbSelectIndex = field.FieldIndex - 1;
                        }
                    }
                    (cb as DropDownList).SelectedIndex = cbSelectIndex;
                }

            }

        }

        void Btn_ImportClick(object sender, System.EventArgs e)
        {
            #region //单机版
            /* try
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

                IDatabaseReaderWriter dbWriter = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", CallClass.Databases["SurveryProductCheckDatabase"]);

                bool bsuccess = dbWriter.ImportDataTableRecords(datatabledesign.CreateTableName,
                    datatabledesign.TargetFieldsCodeList, datatabledesign.TargetAddedFieldCodeList,
                    datatabledesign.IndetifyFieldCodeIndexTarget, sourceDatatable, sourceFieldNameList, sourceIndetifyFiedlnameList);

                if (bsuccess == true)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                string faultmessage = string.Format("数据还未导入\"{0}\"；系统报错：{1}!", datatabledesign.CreateTableName, ex.Message);
                MessageBox.Show(faultmessage); //                 throw new Exception(faultmessage);

            }*/
            #endregion
            try
            {

                List<string> sourceFieldNameList = new List<string>();
                List<string> sourceIndetifyFiedlnameList = new List<string>();

                foreach (Control cb in Panel1.Controls)
                {
                    if (cb is DropDownList)
                    {
                        sourceFieldNameList.Add((cb as DropDownList).Text);
                        FieldDesign field = DataTableDesign.FindFieldDesign(datatabledesign.TargetOrginFieldList, (cb as DropDownList).ID);
                        if (field.FieldImportIDCode == "是")
                            sourceIndetifyFiedlnameList.Add((cb as DropDownList).Text);
                    }
                }


                foreach (string code in datatabledesign.TargetAddedFieldCodeList)
                {
                    sourceFieldNameList.Add(code);
                }

                IDatabaseReaderWriter dbWriter = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", Databases["SurveryProductCheckDatabase"]);

                //从session中取出Session["sourceDatatable"] = sourceDatatable;
                sourceDatatable = Session["sourceDatatable"] as DataTable;

                //以追加的方式导入数据，并根据唯一标示码进行筛选，覆盖原有记录，保持记录为最新
                //暂时要求数据库中必须存在表模板，否则不予导入
                //使用追加的方式进行
                string sCreateTableSql = datatabledesign.GetCreateTableSQL(dbWriter.GetTableName(datatabledesign.CreateTableName));

                bool bsuccess = dbWriter.ImportDataTableRecords(datatabledesign.CreateTableName,
                    datatabledesign.TargetFieldsCodeList, datatabledesign.TargetAddedFieldCodeList,
                    datatabledesign.IndetifyFieldCodeIndexTarget, sourceDatatable, sourceFieldNameList, sourceIndetifyFiedlnameList,1);

                //bool bsuccess = dbWriter.ImportDataTableRecords(datatabledesign.CreateTableName,sourceDatatable, datatabledesign, 1);

                if (bsuccess == true)
                {
                    //create projects and samples 
                    CreateProjectAndSamples(functiontype, sourceDatatable);

                    //==this.Close();
                    ClientScript.RegisterStartupScript(this.GetType(), "updataOK", "alert('提示：数据导入成功!');", true);
                    //导入成功，跳转到我的项目页面
                    string redirecturl = string.Format("~/WebCheckProjects.aspx?userid={0}", userid);
                    //Response.Redirect("~/MyProjects.aspx");
                    Response.Redirect(redirecturl);
                }
            }
            catch (Exception ex)
            {
                string faultmessage = string.Format("数据还未导入\"{0}\"；系统报错：{1}!", datatabledesign.CreateTableName, ex.Message);
                //==MessageBox.Show(faultmessage); //                 throw new Exception(faultmessage);

            }

        }

        protected void CreateProjectAndSamples(string functiontype,DataTable data)
        {
            if(functiontype== "位置精度检测项目信息入库" && data!=null&&data.Rows.Count>0)
            {

                datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", Databases["SurveryProductCheckDatabase"]);
                LoginUser = UserAuthenticate.GetUserObject(datareadwrite, userid);


                string projectname = data.Rows[0]["成果名称"] as string;
                string projectid = System.Guid.NewGuid().ToString("D");
                string producer = "";
                string owner = LoginUser.username;
                string shared = string.Format("{0};{1}", owner, "管理员");
                string department = LoginUser.company;
                string lastupdatetime = DateTime.Now.ToString();
                string insert_sql = string.Format("insert into webcheckprojects values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", projectid, projectname, producer, owner, shared, department, lastupdatetime);
                datareadwrite.ExecuteSQL(insert_sql);

                DataRowCollection mapnumbers = data.DefaultView.ToTable(true,"图幅号").Rows;
                foreach(DataRow dr in mapnumbers)
                {
                    string mapid = System.Guid.NewGuid().ToString("D");
                    string mapnumber =Convert.ToString( dr[0] );
                    string qualitymodel = "{\\QualityName\\:\\大比例尺地形图\\,\\QualityItemList\\:[{\\QualityItemName\\:\\数学精度\\,\\SubQualitys\\:[{\\SubQualityItemName\\:\\高程精度\\,\\CheckItem\\:\\等高线高程中误差\\},{\\SubQualityItemName\\:\\高程精度\\,\\CheckItem\\:\\接边精度\\},{\\SubQualityItemName\\:\\高程精度\\,\\CheckItem\\:\\注记点高程中误差\\},{\\SubQualityItemName\\:\\平面精度\\,\\CheckItem\\:\\接边精度\\},{\\SubQualityItemName\\:\\平面精度\\,\\CheckItem\\:\\绝对位置中误差\\},{\\SubQualityItemName\\:\\平面精度\\,\\CheckItem\\:\\相对位置中误差\\},{\\SubQualityItemName\\:\\数学基础\\,\\CheckItem\\:\\控制点距离较差\\},{\\SubQualityItemName\\:\\数学基础\\,\\CheckItem\\:\\投影计算和使用参数\\},{\\SubQualityItemName\\:\\数学基础\\,\\CheckItem\\:\\图根控制测量精度\\},{\\SubQualityItemName\\:\\数学基础\\,\\CheckItem\\:\\图廓格网尺寸、格网尺寸\\},{\\SubQualityItemName\\:\\数学基础\\,\\CheckItem\\:\\坐标和高程系统\\}]},{\\QualityItemName\\:\\地理精度\\,\\SubQualitys\\:[{\\SubQualityItemName\\:\\地理精度\\,\\CheckItem\\:\\地理要素的协调性\\},{\\SubQualityItemName\\:\\地理精度\\,\\CheckItem\\:\\地理要素接边质量\\},{\\SubQualityItemName\\:\\地理精度\\,\\CheckItem\\:\\地理要素完整性和正确性\\},{\\SubQualityItemName\\:\\地理精度\\,\\CheckItem\\:\\注记和符号的正确性\\},{\\SubQualityItemName\\:\\地理精度\\,\\CheckItem\\:\\综合取舍的合理性\\}]},{\\QualityItemName\\:\\数据及结构正确性\\,\\SubQualitys\\:[{\\SubQualityItemName\\:\\数据及结构正确性\\,\\CheckItem\\:\\属性代码\\},{\\SubQualityItemName\\:\\数据及结构正确性\\,\\CheckItem\\:\\属性接边质量\\},{\\SubQualityItemName\\:\\数据及结构正确性\\,\\CheckItem\\:\\数据格式\\},{\\SubQualityItemName\\:\\数据及结构正确性\\,\\CheckItem\\:\\文件命名和数据组织\\},{\\SubQualityItemName\\:\\数据及结构正确性\\,\\CheckItem\\:\\要素分层\\}]},{\\QualityItemName\\:\\附件质量\\,\\SubQualitys\\:[{\\SubQualityItemName\\:\\附件质量\\,\\CheckItem\\:\\成果资料的齐全性\\},{\\SubQualityItemName\\:\\附件质量\\,\\CheckItem\\:\\各类报告、附图、附表的规整性\\},{\\SubQualityItemName\\:\\附件质量\\,\\CheckItem\\:\\检查报告和技术总结\\},{\\SubQualityItemName\\:\\附件质量\\,\\CheckItem\\:\\元数据文件\\},{\\SubQualityItemName\\:\\附件质量\\,\\CheckItem\\:\\资料装帧\\}]},{\\QualityItemName\\:\\整饰质量\\,\\SubQualitys\\:[{\\SubQualityItemName\\:\\整饰质量\\,\\CheckItem\\:\\符号线划和色彩质量\\},{\\SubQualityItemName\\:\\整饰质量\\,\\CheckItem\\:\\图廓内外整饰质量\\},{\\SubQualityItemName\\:\\整饰质量\\,\\CheckItem\\:\\图面要素协调性\\},{\\SubQualityItemName\\:\\整饰质量\\,\\CheckItem\\:\\注记质量\\}]}]}";
                    string score = "0.0";
                    string level = "未评定";
                    string maptype = "大比例尺地形图";
                    insert_sql = string.Format("insert into webchecksamples values('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}')",projectid,mapid,mapnumber,qualitymodel,score,level,lastupdatetime,maptype);
                    datareadwrite.ExecuteSQL(insert_sql);
                }

              

            }
        }
    }
}
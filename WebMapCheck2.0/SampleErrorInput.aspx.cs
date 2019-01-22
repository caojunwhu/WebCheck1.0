using Authentication.Class;
using DatabaseDesignPlus;
using DLGCheckLib;
using Ext.Net;
using ServiceRanking;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebMapCheck
{
    public partial class SampleErrorInput : System.Web.UI.Page
    {
        string maptype = "大比例尺地形图";
        string[] ErrorDoc = null;
        DataTable standarderrordata = null;
        IDatabaseReaderWriter datareadwrite=null;

        DataTable _QualityErrorTable;
        DataRow oldQualityError;

        string _sMapnumber;
        string _sMapid;
        Authentication.Class.UserObject _loginuser;
        string _sprojectid;
        QualityItems _qitem;
        PinErrorItem _pinerror;

        public PinErrorItem Pinerror
        {
            get
            {
                return _pinerror;
            }

            set
            {
                _pinerror = value;
            }
        }

        public void BindGridPanelData(DataTable data)
        {
            //Store1.DataSource = data;
            //Store1.DataBind();
            GridPanel1.Store[0].LoadData(data);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode) {
                _sMapnumber = HttpUtility.UrlDecode(Request["mapnumber"]);
                //_loginuser = loginuser;
                _sprojectid = HttpUtility.UrlDecode(Request["projectid"]);
                _sMapid = HttpUtility.UrlDecode(Request["mapid"]);
                string userid = Session["userid"] as string;
                userid = "83386B36-C591-46C6-B0E2-E2C3AE593312";
                if (userid == "" || userid == null)
                {
                    //MessageBox.Show("您当前用户名在本机还未授权，请申请授权或等待管理员授权！");
                    //ClientScript.RegisterStartupScript(this.GetType(), "温馨提示", "alert('提示：请您登录本系统后在查看页面！');", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "opennewwindow", "alert('提示：请您登录本系统后在查看页面！');", true);
                    //Response.Redirect("~/Default.aspx");
                    return;
                }
                ////////////////////////////////////************************************///
                string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
                SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);
                datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
                _loginuser = UserAuthenticate.GetUserObject(datareadwrite, userid);
                if (_loginuser.authorized != "1" || _loginuser.authorized == null)
                {
                    //MessageBox.Show("您当前用户名在本机还未授权，请申请授权或等待管理员授权！");
                    ClientScript.RegisterStartupScript(this.GetType(), "温馨提示", "alert('提示：您当前用户名在本机还未授权，请申请授权或等待管理员授权！');", true);

                    Response.Redirect("~/Default.aspx");
                    return;
                }

                string sql_QualityModel = string.Format("select qualitymodel from webchecksamples where mapid='{0}'", _sMapid);
                string sQualityModel = datareadwrite.GetScalar(sql_QualityModel) as string;
                _qitem = QualityItems.FromJson(sQualityModel.Replace('\\','"'));

                if(!IsPostBack )
                {
                    foreach (QualityItem qi in _qitem.QualityItemList)
                    {
                        tbFClass.Items.Add(qi.QualityItemName);

                        foreach (SubQualityItem si in qi.SubQualitys)
                        {
                            if (!tbSClass.Items.Contains(new System.Web.UI.WebControls.ListItem(si.SubQualityItemName)))
                                tbSClass.Items.Add(si.SubQualityItemName);

                            tbCheckItem.Items.Add(si.CheckItem);
                        }
                    }
                    //检查项增加默认值
                    tbCheckItem.Items.Add("");
                }

                //网页版与客户端版不同，每次变化都会向后台提交刷新，需要每次增加时将表格中的记录录入到后台数据库，刷新时重新读取
                List<string> tableNames = datareadwrite.GetSchameDataTableNames();
                tableNames = datareadwrite.GetSchameDataTableNames();

                _QualityErrorTable = new DataTable();
                _QualityErrorTable.Columns.Add("序号");
                _QualityErrorTable.Columns.Add("质量元素");
                _QualityErrorTable.Columns.Add("质量子元素");
                _QualityErrorTable.Columns.Add("检查项");
                _QualityErrorTable.Columns.Add("错漏类别");
                _QualityErrorTable.Columns.Add("错漏描述");
                _QualityErrorTable.Columns.Add("处理意见");
                _QualityErrorTable.Columns.Add("复查情况");
                _QualityErrorTable.Columns.Add("修改情况");
                _QualityErrorTable.Columns.Add("检查者");
                _QualityErrorTable.Columns.Add("检查日期");

                string SampleerrorCollectionTableName = "sampleerrorplus";
                if (tableNames.Contains(SampleerrorCollectionTableName) == true)
                {
                    //逐条记录到数据表_QualityErrorTable
                    string sql_queryitem = string.Format("select * from {0} where projectid = '{1}' and mapnumber='{2}' order by 检查日期 asc", SampleerrorCollectionTableName, _sprojectid,_sMapnumber);
                    DataTable datatable0 = datareadwrite.GetDataTableBySQL(sql_queryitem);
                    foreach (DataRow dr in datatable0.Rows)
                    {
                        DataRow datarow = _QualityErrorTable.NewRow();
                        datarow["序号"] = _QualityErrorTable.Rows.Count+1;
                        datarow["质量元素"] = dr["质量元素"];
                        datarow["质量子元素"] = dr["质量子元素"];
                        datarow["检查项"] = dr["检查项"];
                        datarow["错漏类别"] = dr["错漏类别"];
                        datarow["错漏描述"] = dr["错漏描述"];
                        datarow["处理意见"] = dr["处理意见"];
                        datarow["复查情况"] = dr["复查情况"];
                        datarow["修改情况"] = dr["修改情况"];
                        datarow["检查者"] = dr["检查者"];
                        datarow["检查日期"] = dr["检查日期"];
                        _QualityErrorTable.Rows.Add(datarow);

                    }
                    // GridView1.DataSource = _QualityErrorTable;
                    //GridView1.DataBind();
                    //Store store = this.FindControl("Store1") as Store;
                    Store1.DataSource = _QualityErrorTable;
                    Store1.DataBind();
                    GridPanel1.Store.Add(Store1);

                }
                if (IsPostBack)
                {
                    string sql = string.Format("select * from ah错漏分类表 where 成果种类='{0}'", maptype);
                    //string sql = string.Format("select * from ah错漏分类表  " );
                    standarderrordata = datareadwrite.GetDataTableBySQL(sql);
                    ErrorDoc = new string[standarderrordata.Rows.Count + 1];
                    int index = 0;
                    foreach (DataRow dr in standarderrordata.Rows)
                    {
                        
                        ErrorDoc[index++] = dr["错漏内容"] as string;
                    }

                }

                if (!X.IsAjaxRequest)
                {
                    
                }

            }

        }
        [DirectMethod]
        protected void Main_ReadData(object sender, StoreReadDataEventArgs e)
        {
            Store store = sender as Store;
            _sMapnumber = "12.00-71.00";
            //_loginuser = loginuser;
            _sprojectid = "SYS500DLG20161115";

            ////////////////////////////////////************************************///
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);
            datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);

            //网页版与客户端版不同，每次变化都会向后台提交刷新，需要每次增加时将表格中的记录录入到后台数据库，刷新时重新读取
            List<string> tableNames = datareadwrite.GetSchameDataTableNames();
            tableNames = datareadwrite.GetSchameDataTableNames();

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

            string SampleerrorCollectionTableName = "检查意见记录表";
            if (tableNames.Contains(SampleerrorCollectionTableName) == true)
            {
                //逐条记录到数据表_QualityErrorTable
                string sql_queryitem = string.Format("select * from {0} where mapid = '{1}'   ", SampleerrorCollectionTableName, _sMapid);
                DataTable datatable0 = datareadwrite.GetDataTableBySQL(sql_queryitem);
                foreach (DataRow dr in datatable0.Rows)
                {
                    DataRow datarow = _QualityErrorTable.NewRow();
                    datarow["质量元素"] = dr["质量元素"];
                    datarow["质量子元素"] = dr["质量子元素"];
                    datarow["检查项"] = dr["检查项"];
                    datarow["错漏类别"] = dr["错漏类别"];
                    datarow["错漏描述"] = dr["错漏描述"];
                    datarow["处理意见"] = dr["处理意见"];
                    datarow["复查情况"] = dr["复查情况"];
                    datarow["修改情况"] = dr["修改情况"];
                    datarow["检查者"] = dr["检查者"];
                    datarow["检查日期"] = dr["检查日期"];
                    _QualityErrorTable.Rows.Add(datarow);

                }
            }

                store.DataSource = _QualityErrorTable;
                store.DataBind();
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            this.TextBox1.Focus();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (TextBox1.Text.Trim().Length <= 3)
            {
                
                return;
            }
            ErrorDoc[ErrorDoc.Length - 1] = TextBox1.Text;
            StopWordsHandler s = new StopWordsHandler();
            TFIDFMeasure tfidf = new TFIDFMeasure(ErrorDoc);
            Dictionary<int, float> Similarity = new Dictionary<int, float>();
            //与自身以外的描述计算相似性
            for (int i = 0; i < ErrorDoc.Length - 2; i++)
            {
                float si = tfidf.GetSimilarity(i, ErrorDoc.Length - 1);
                Similarity.Add(i, si);
            }
            //求取最大值
            List<KeyValuePair<int, float>> maxSimilarity = Similarity.OrderByDescending(ao => ao.Value).ToList();
            int id = maxSimilarity.ElementAt(0).Key;
            if(maxSimilarity.ElementAt(0).Value<=0)
            {
                //tbFClass.Text = "未找到匹配项，请调整输入问题描述";
                tbErrorClass.Text = "";
                DropDownList3.Items.Clear();
                return;
            }

            DataRow d = standarderrordata.Rows[id];

            string qualitysubitem = d["质量子元素"] as string;
            string sqlqualityitem = string.Format("select 质量元素 from ahselecteditems where 成果种类='{0}' and 质量子元素='{1}'", maptype, qualitysubitem);
            string qualityitem = datareadwrite.GetScalar(sqlqualityitem) as string;

            tbFClass.Text=qualityitem;
            tbSClass.Text = qualitysubitem;
            tbErrorClass.Text = d["错漏类型"] as string;
            tbCheckItem.Text ="" ;

            //将 匹配度较由高到低的错漏填充到DropDownList3中
            //comboBox4.DroppedDown = false;
            DropDownList3.Items.Clear();
            for (int j = 0; j < maxSimilarity.Count; j++)
            {
                //排除为零的
                if (maxSimilarity.ElementAt(j).Value == 0)
                    continue;

                id = maxSimilarity.ElementAt(j).Key;
                d = standarderrordata.Rows[id];

                string errorstandard = d["错漏内容"] as string;
                DropDownList3.Items.Add(string.Format("{0}({1:P})", errorstandard, maxSimilarity.ElementAt(j).Value));
                //DropDownList3.DroppedDown = true;
            }
        }
        protected void MyButtonClickHandler(object sender, DirectEventArgs e)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            Dictionary<string, object> JsonData = (Dictionary<string, object>)s.DeserializeObject(e.ExtraParams["id"]);
            int recordid = Convert.ToInt32(JsonData["序号"]);
            string qualityitem = Convert.ToString(JsonData["质量元素"]) ;
            //DirectEventArgs e 参数中带了命令名称“删除”或者“修改”；以及行数据json格式，可以对原始表进行操作；
            // { "质量元素":"地理精度","质量子元素":"地理精度","错漏类别":"C","错漏描述":"其他一般错漏：多余消防栓1处","处理意见":"","复查情况":"","修改情况":"","检查者":"曹俊","检查日期":"2016/11/18 19:45:42","id":"ext-29-4"}
            if (e.ExtraParams["command"] == "Delete")
            {
                //数学精度为自动计算，不允许删除
                if (qualityitem == "数学精度")
                    return;
                DataRow deleteRow = _QualityErrorTable.Select(string.Format("序号='{0}'", recordid))[0];
                _QualityErrorTable.Rows.Remove(deleteRow);
                //重新编号并显示
                int indexer = 1;
                System.Data.DataView dataview = _QualityErrorTable.DefaultView;
                dataview.Sort = "检查日期 asc";
                _QualityErrorTable = dataview.ToTable();
                foreach (DataRow dr in _QualityErrorTable.Rows)
                {
                    dr["序号"] = indexer;
                    indexer = indexer + 1;
                }
                FlushToDatabase();

                BindGridPanelData(_QualityErrorTable);
            }
        }
        protected void AddCustomCheckState(object sender, DirectEventArgs e)
        {
            string qualityFClass = tbFClass.Text;
            string qualitySClass = tbSClass.Text;
            string qualityErrorClass = tbErrorClass.Text;
            string qualityError = TextBox1.Text;
            string qualityCheckItem = tbCheckItem.Text;
            //string fucha = fucharichTextBox1.Text;
            //string chuli = chulirichTextBox1.Text;
            // string xiugai = xiugairichTextBox3.Text;

            if (qualityError == "" || qualityErrorClass == "" ||
                qualitySClass == "" || qualityFClass == "")
            {
                //MessageBox.Show("请检查错漏描述是否完整！");
                Response.Write("<script>alert('请检查错漏描述是否完整！')</script>");
                return;
            }


            //如果是选中了某行进行了修改,且质量元素、质量子元素、错漏类别、错漏描述没有变化则进行修改，否则是新增
            if (oldQualityError != null &&
                (oldQualityError["质量元素"] as string) == qualityFClass &&
                (oldQualityError["质量子元素"] as string) == qualitySClass &&
                (oldQualityError["错漏类别"] as string) == qualityErrorClass &&
                (oldQualityError["错漏描述"] as string) == qualityError)
            {
                //datarow["序号"] = _QualityErrorTable.Rows.Count + 1;
                oldQualityError["质量元素"] = qualityFClass;
                oldQualityError["质量子元素"] = qualitySClass;
                oldQualityError["错漏类别"] = qualityErrorClass;
                oldQualityError["错漏描述"] = qualityError;
                oldQualityError["检查项"] = qualityCheckItem;
                oldQualityError["处理意见"] = "修改";
                //oldQualityError["复查情况"] = fucha;
                //oldQualityError["修改情况"] = xiugai;

                oldQualityError["检查者"] = _loginuser.username;
                oldQualityError["检查日期"] = DateTime.Now.ToString();

                //_QualityErrorTable.Rows.Remove(oldQualityError);
                //oldQualityError = null;
            }
            else
            {
                DataRow dr = _QualityErrorTable.NewRow();
                dr["序号"] = _QualityErrorTable.Rows.Count + 1;
                dr["质量元素"] = qualityFClass;
                dr["质量子元素"] = qualitySClass;
                dr["检查项"] = qualityCheckItem;
                dr["错漏类别"] = qualityErrorClass;
                dr["错漏描述"] = qualityError;
                dr["处理意见"] = "修改";
                //dr["复查情况"] = fucha;
                //dr["修改情况"] = xiugai;

                dr["检查者"] = _loginuser.username;
                dr["检查日期"] = DateTime.Now.ToString();
                _QualityErrorTable.Rows.Add(dr);

                System.Data.DataView dataview = _QualityErrorTable.DefaultView;
                dataview.Sort = "检查日期 asc";
                _QualityErrorTable = dataview.ToTable();

            }

            //GridView1.DataSource = _QualityErrorTable;
            //GridView1.DataBind();
            FlushToDatabase();

            BindGridPanelData(_QualityErrorTable);
        }
        protected void AddStandardCheckState(object sender, DirectEventArgs e)
        {
            string qualityFClass = tbFClass.Text;
            string qualitySClass = tbSClass.Text;
            string qualityErrorClass = tbErrorClass.Text;
            string qualityError = DropDownList3.Text;
            string qualityCheckItem = tbCheckItem.Text;
            //string fucha = fucharichTextBox1.Text;
            //string chuli = chulirichTextBox1.Text;
            // string xiugai = xiugairichTextBox3.Text;

            if (qualityError == "" || qualityErrorClass == "" ||
                qualitySClass == "" || qualityFClass == "")
            {
                //MessageBox.Show("请检查错漏描述是否完整！");
                Response.Write("<script>alert('请检查错漏描述是否完整！')</script>");
                return;
            }


            //如果是选中了某行进行了修改,且质量元素、质量子元素、错漏类别、错漏描述没有变化则进行修改，否则是新增
            if (oldQualityError != null &&
                (oldQualityError["质量元素"] as string) == qualityFClass &&
                (oldQualityError["质量子元素"] as string) == qualitySClass &&
                (oldQualityError["错漏类别"] as string) == qualityErrorClass &&
                (oldQualityError["错漏描述"] as string) == qualityError)
            {
                //datarow["序号"] = _QualityErrorTable.Rows.Count + 1;
                oldQualityError["质量元素"] = qualityFClass;
                oldQualityError["质量子元素"] = qualitySClass;
                oldQualityError["错漏类别"] = qualityErrorClass;
                oldQualityError["错漏描述"] = qualityError;
                oldQualityError["检查项"] = qualityCheckItem;
                oldQualityError["处理意见"] = "修改";
                //oldQualityError["复查情况"] = fucha;
                //oldQualityError["修改情况"] = xiugai;

                oldQualityError["检查者"] = _loginuser.username;
                oldQualityError["检查日期"] = DateTime.Now.ToString();

                //_QualityErrorTable.Rows.Remove(oldQualityError);
                //oldQualityError = null;
            }
            else
            {
                DataRow dr = _QualityErrorTable.NewRow();
                dr["序号"] = _QualityErrorTable.Rows.Count + 1;
                dr["质量元素"] = qualityFClass;
                dr["质量子元素"] = qualitySClass;
                dr["检查项"] = qualityCheckItem;
                dr["错漏类别"] = qualityErrorClass;
                dr["错漏描述"] = qualityError;
                dr["处理意见"] = "修改";
                //dr["复查情况"] = fucha;
                //dr["修改情况"] = xiugai;

                dr["检查者"] = _loginuser.username;
                dr["检查日期"] = DateTime.Now.ToString();
                _QualityErrorTable.Rows.Add(dr);

                System.Data.DataView dataview = _QualityErrorTable.DefaultView;
                dataview.Sort = "检查日期 asc";
                _QualityErrorTable = dataview.ToTable();

            }

            //GridView1.DataSource = _QualityErrorTable;
            //GridView1.DataBind();
            FlushToDatabase();

            BindGridPanelData(_QualityErrorTable);

        }

        protected void btnAddStandardCheckState_Click(object sender, EventArgs e)
        {

        }

        protected void FlushToDatabase()
        {
            //if (_QualityErrorTable.Rows.Count == 0) return;

            //先将该样本中的问题记录进行清空
            ///////////////////////////////////////////////////////////////////////
            /////////////更新到数据库
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string SampleerrorCollectionTableName = "sampleerrorplus";
            //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
            List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
            if (tableNames2.Contains(SampleerrorCollectionTableName) == false)
            {
                string sql_createTable = string.Format("create table {0}( ProjectID text,Mapnumber text,质量元素 text,质量子元素 text,检查项 text,错漏类别 text ,错漏描述 text ,处理意见 text, 修改情况 text, 复查情况 text,检查者 text,检查日期 text,备注 text,Shape text)", SampleerrorCollectionTableName);
                datareadwrite.ExecuteSQL(sql_createTable);
                //记录
            }
            //逐条记录到数据表SearchTargetSetting
            string sql_queryitem = string.Format("select Mapnumber from {0} where projectid = '{1}' and mapnumber='{2}' ", SampleerrorCollectionTableName,_sprojectid,_sMapnumber );
            object o = datareadwrite.GetScalar(sql_queryitem);
            if (o == null)//insert
            {
            }
            else // delete
            {
                //删除除数学精度以外的内容
                string sql_delete = string.Format("delete from {0} where  projectid = '{1}' and mapnumber='{2}' and 质量元素 != '数学精度'", SampleerrorCollectionTableName, _sprojectid, _sMapnumber);
                datareadwrite.ExecuteSQL(sql_delete);
            }


            //写入新的问题记录，写入除数学精度以外的内容
            DataRow[] datarows = _QualityErrorTable.Select("质量元素 <> '数学精度'");
            foreach (DataRow dr in datarows)
            {
                string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')", SampleerrorCollectionTableName, _sprojectid, _sMapnumber,
                    dr["质量元素"] as string, dr["质量子元素"] as string, dr["检查项"] as string, dr["错漏类别"] as string, dr["错漏描述"] as string, dr["处理意见"] as string, dr["修改情况"] as string, dr["复查情况"] as string, dr["检查者"] as string, dr["检查日期"] as string,"备注","");
                datareadwrite.ExecuteSQL(sql_insert);
                //记录搜索到的目标
            }
        }
        //standarderrordata selected change to update the listdowns
        protected void DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string select_erroritem = DropDownList3.Text;
            select_erroritem = select_erroritem.Substring(0,select_erroritem.IndexOf('('));
            DataRow dr = standarderrordata.Select(string.Format("错漏内容='{0}'", select_erroritem))[0];
            if(dr!=null)
            {
                tbErrorClass.Text = dr["错漏类型"] as string;
                tbSClass.Text = dr["质量子元素"] as string;
                //再根据质量子元素和成果类型查询质量元素
                string select_sql = string.Format("select 质量元素 from ahselecteditems where 成果种类='{0}' and 质量子元素='{1}'", maptype, tbSClass.Text);
               tbFClass.Text= datareadwrite.GetScalar(select_sql) as string;
            }
        }


        protected void tbFClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            //tbSClass.Items.Clear();
            tbCheckItem.Items.Clear();
            string qualityname = tbFClass.Text;
            //tbSClass.Text = "";
            //tbCheckItem.Text = "";
            foreach (QualityItem qi in _qitem.QualityItemList)
            {
                if (qualityname == qi.QualityItemName)
                {
                    //cmb_qualityitem.Items.Add(qi.QualityItemName);

                    foreach (SubQualityItem si in qi.SubQualitys)
                    {
                        if (tbSClass.Items.IndexOf(new System.Web.UI.WebControls.ListItem(si.SubQualityItemName)) < 0)
                            tbSClass.Items.Add(si.SubQualityItemName);

                        tbCheckItem.Items.Add(si.CheckItem);
                    }
                }

            }
        }

        protected void tbSClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbCheckItem.Items.Clear();
            string qualityname = tbFClass.Text;
            string subqualityname = tbSClass.Text;
            //tbCheckItem.Text = "";
            foreach (QualityItem qi in _qitem.QualityItemList)
            {
                if (qualityname == qi.QualityItemName)
                {
                    // cmb_qualityitem.Items.Add(qi.QualityItemName);

                    foreach (SubQualityItem si in qi.SubQualitys)
                    {
                        if (subqualityname == si.SubQualityItemName)
                        {
                            if (tbSClass.Items.IndexOf(new System.Web.UI.WebControls.ListItem(si.SubQualityItemName)) < 0)
                                tbSClass.Items.Add(si.SubQualityItemName);

                            tbCheckItem.Items.Add(si.CheckItem);
                        }
                    }
                }
            }

            tbErrorClass.Text = "";
            DropDownList3.Items.Clear();
            DropDownList3.Text = "";
            TextBox1.Text = "";
        }

        protected void tbErrorClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            string producttype = _qitem.QualityName;
            string subqualityname = tbSClass.Text;
            string errorclass = tbErrorClass.Text;

            string sql_preerror = string.Format("select 错漏内容 from ah错漏分类表 where 成果种类='{0}' and  质量子元素='{1}' and  错漏类型='{2}'", producttype, subqualityname, errorclass);
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);
            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);

            List<string> preerror = datareadwrite.GetSingleFieldValueList("错漏内容", sql_preerror);
            DropDownList3.Items.Clear();
            //DropDownList3.Text = "";

            foreach(string s in preerror)
            {
                DropDownList3.Items.Add(s);
            }

        }
    }
}
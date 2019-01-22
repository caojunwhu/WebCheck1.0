using Authentication.Class;
using DatabaseDesignPlus;
using Ext.Net;
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
    public partial class WebCheckProjects : System.Web.UI.Page
    {
        Authentication.Class.UserObject _loginuser;
        IDatabaseReaderWriter datareadwrite = null;
        string rooturl = "http://localhost:5152/";
        string rootvirtualpath = "~/";

        protected void Page_Load(object sender, EventArgs e)
        {
            string userid = Session["userid"] as string;
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

            this.Title = string.Format("欢迎来自{0}的{1}！", _loginuser.company, _loginuser.username);

            string webcheckprojects = "webcheckprojects";
            List<string> tables = datareadwrite.GetSchameDataTableNames();
            if(tables.IndexOf(webcheckprojects)<0)
            {
                string create_sql = string.Format("create table {0}{projectid text ,projectname text,producer text ,owner text,shared text,department text,lastupdatetime  timestamp without time zone, PRIMARY KEY(projectid)}", webcheckprojects);
            }


            string sql_select = string.Format("select * from {0} where owner='{1}' or position('{2}' in shared )>0 ",webcheckprojects,_loginuser.username, _loginuser.username);

            DataTable dt = datareadwrite.GetDataTableBySQL(sql_select);
            Store1.DataSource = dt;
            Store1.DataBind();
        }
        protected void Main_ReadData(object sender, StoreReadDataEventArgs e)
        {
        }
        protected void MyButtonClickHandler(object sender, DirectEventArgs e)
        {
            //DirectEventArgs e 参数中带了命令名称“删除”或者“修改”；以及行数据json格式，可以对原始表进行操作；
            // { "质量元素":"地理精度","质量子元素":"地理精度","错漏类别":"C","错漏描述":"其他一般错漏：多余消防栓1处","处理意见":"","复查情况":"","修改情况":"","检查者":"曹俊","检查日期":"2016/11/18 19:45:42","id":"ext-29-4"}
            JavaScriptSerializer s = new JavaScriptSerializer();
            Dictionary<string, object> JsonData = (Dictionary<string, object>)s.DeserializeObject(e.ExtraParams["id"]);
            string projectid = Convert.ToString(JsonData["projectid"]);
            string projectname = Convert.ToString(JsonData["projectname"]);
            if (e.ExtraParams["command"]== "Detail")
            {
                //Server.Transfer(returnURL, true);
                //A bit strange, the way I fixed this..
                //Earlier i was using Server.Transfer(..) to redirect the page then I started facing the same problem
                ///Then I searched a lot and in the end I changed that Server.Transfer(..) to Response.Redirect(..), And ta-da it worked perfectly..
                //Hope this helps you :-)
                string RedirectURL = string.Format("{0}/WebCheckSamples2.aspx?projectid={1}", rooturl, HttpUtility.UrlEncode(projectid));
                //Server.Transfer(returnURL, true);
                //Response.Redirect(returnURL);
                System.Diagnostics.Process.Start(RedirectURL);
                Session["projectid"] = projectid;
            }
            else if (e.ExtraParams["command"] == "MathPrecision")
            {
                Session["username"] = _loginuser.username;
                Session["projectid"] = projectid;
                string returnURL = string.Format("{0}/SampleDetail.aspx?project={1}", rooturl, HttpUtility.UrlEncode(projectname));
                System.Diagnostics.Process.Start(returnURL);

            }
            else if (e.ExtraParams["command"] == "Delete")
            {
                string owner = Convert.ToString(JsonData["owner"]);
                if(owner!=_loginuser.username)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "updataOK", string.Format("alert('提示：删除项目请联系创建者{0}!');",owner), true);
                    return;
                }
                //delete webcheckprojects and webchecksamples
                string delete_sql = string.Format("delete   from webcheckprojects where projectid='{0}'", projectid);
                datareadwrite.ExecuteSQL(delete_sql);
                delete_sql = string.Format("delete   from webchecksamples where projectid='{0}'", projectid);
                datareadwrite.ExecuteSQL(delete_sql);
                ClientScript.RegisterStartupScript(this.GetType(), "updataOK", string.Format("alert('提示：已删除项目，请刷新页面!');"), true);

            }
            //X.Msg.Alert("Hello", "HelloWorld!");
        }
    }
}
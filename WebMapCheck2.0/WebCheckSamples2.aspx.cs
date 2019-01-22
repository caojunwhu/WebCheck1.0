using Authentication.Class;
using DatabaseDesignPlus;
using DLGCheckLib;
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
    public partial class WebCheckSamples2 : System.Web.UI.Page
    {
        Authentication.Class.UserObject _loginuser;
        IDatabaseReaderWriter datareadwrite = null;
        string _sprojectid = "";
        string rooturl = "http://localhost:5152/";

        protected void Page_Load(object sender, EventArgs e)
        {
            string userid = Session["userid"] as string;
            _sprojectid = Session["projectid"] as string ;

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

            string select_projectname = string.Format("select projectname from webcheckprojects where projectid = '{0}'", _sprojectid);
            string projectname = datareadwrite.GetScalar(select_projectname) as string;
            this.Title = string.Format("当前项目为{0}", projectname);

            string WebcheckSamples = "webchecksamples";
            List<string> tables = datareadwrite.GetSchameDataTableNames();
            if (tables.IndexOf(WebcheckSamples) < 0)
            {
                //Create WebCheckSamples table;
                string create_sql = string.Format("create table {0}(projectid character varying,mapid  character varying,maptype  character varying,mapnumber character varying,qualitymodel character varying,score numeric(6,2),level character varying,lastupdatetime timestamp without time zone, PRIMARY KEY(mapid))", WebcheckSamples);
                datareadwrite.ExecuteSQL(create_sql);
            }

            string sql_select = string.Format("select * from {0} where projectid='{1}'  ", "webchecksamples", _sprojectid);

            DataTable dt = datareadwrite.GetDataTableBySQL(sql_select);
            dt.Columns.Add("checkqitems");
            foreach(DataRow r in dt.Rows)
            {
                string json = r["qualitymodel"] as string;
                QualityItems items  = QualityItems.FromJson(json.Replace('\\', '"'));
                r["checkqitems"] = items.QualityItemNameString;
            }

            Store1.DataSource = dt;
            Store1.DataBind();
        }
        protected void Main_ReadData(object sender, StoreReadDataEventArgs e)
        {
        }
        protected void MyButtonClickHandler(object sender, DirectEventArgs e)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            Dictionary<string, object> JsonData = (Dictionary<string, object>)s.DeserializeObject(e.ExtraParams["id"]);
            string mapid = Convert.ToString(JsonData["mapid"]);
            string mapnumber = Convert.ToString(JsonData["mapnumber"]);
            //DirectEventArgs e 参数中带了命令名称“删除”或者“修改”；以及行数据json格式，可以对原始表进行操作；
            // { "质量元素":"地理精度","质量子元素":"地理精度","错漏类别":"C","错漏描述":"其他一般错漏：多余消防栓1处","处理意见":"","复查情况":"","修改情况":"","检查者":"曹俊","检查日期":"2016/11/18 19:45:42","id":"ext-29-4"}
            if (e.ExtraParams["command"] == "CheckRecord")
            {
                //string RedirectURL = string.Format(@"~/SampleErrorInput.aspx?mapid={0}", HttpUtility.UrlEncode(mapid));
                //Server.Transfer(returnURL, true);
                //Response.Redirect(RedirectURL);
                string RedirectURL = string.Format("{0}/SampleErrorInput.aspx?projectid={1}&mapnumber={2}&mapid={3}", rooturl, HttpUtility.UrlEncode(_sprojectid), HttpUtility.UrlEncode(mapnumber), HttpUtility.UrlEncode(mapid));              
                System.Diagnostics.Process.Start(RedirectURL);
                //Response.Redirect(RedirectURL);
            }
            else if(e.ExtraParams["command"] == "QualityModel")
            {
                string RedirectURL = string.Format("{0}/SelectCheckItems.aspx?mapid={1}", rooturl, HttpUtility.UrlEncode(mapid));
                System.Diagnostics.Process.Start(RedirectURL);
            }else　if(e.ExtraParams["command"]=="Quality")
            {
                string RedirectURL = string.Format("{0}/WebCheckSampleQuality.aspx?mapid={1}", rooturl, HttpUtility.UrlEncode(mapid));
                System.Diagnostics.Process.Start(RedirectURL);

            }
            else if(e.ExtraParams["command"]== "MathPrecision")
            {
                string RedirectURL = string.Format("{0}/Checkrecord.aspx?mapnumber={1}", rooturl, HttpUtility.UrlEncode(mapnumber));
                System.Diagnostics.Process.Start(RedirectURL);

            }
            Session["mapid"] = mapid;

        }
    }
}
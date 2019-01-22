using Authentication.Class;
using DatabaseDesignPlus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebMapCheck
{
    public partial class WebCheckSampleQuality : System.Web.UI.Page
    {
        Authentication.Class.UserObject _loginuser;
        IDatabaseReaderWriter datareadwrite = null;
        string _sprojectid = "";
        string rooturl = "http://localhost:5152/";
        private string projectid;
        string _sMapid; protected void Page_Load(object sender, EventArgs e)
        {
            string userid = Session["userid"] as string;
            _sMapid = HttpUtility.UrlDecode(Request["mapid"]);

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);
            datareadwrite = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);

            _loginuser = UserAuthenticate.GetUserObject(datareadwrite, userid);
            if (_loginuser.authorized != "1" || _loginuser.authorized == null)
            {
                //MessageBox.Show("您当前用户名在本机还未授权，请申请授权或等待管理员授权！");
                ClientScript.RegisterStartupScript(this.GetType(), "温馨提示", "alert('提示：您当前用户名在本机还未授权，请申请授权或等待管理员授权！');", true);

                Response.Redirect("~/Default.aspx");
                return;
            }

            string WebcheckSampleQualities = "webchecksamplequalities";
            List<string> tables = datareadwrite.GetSchameDataTableNames();
            if (tables.IndexOf(WebcheckSampleQualities) < 0)
            {
                //  projectid text, mapnumber text,  score numeric(6, 2),  qualityitem text,  qitemweight numeric(6, 2),
                //  qitemscore numeric(6, 2),  subqualityitem text,  subqitemweight numeric(6, 2),  subqitemscore numeric(6, 2),  faulta integer,
                // faultb integer,  faultc integer,  faultd integer,  comment text
                //Create WebcheckSampleQualities table;
                string create_sql = string.Format("create table {0}(projectid text, mapid text,mapnumber text,  score numeric(6, 2),  qualityitem text,  qitemweight numeric(6, 2),qitemscore numeric(6, 2),  subqualityitem text,  subqitemweight numeric(6, 2),  subqitemscore numeric(6, 2),  faulta integer,faultb integer,  faultc integer,  faultd integer,  comment text)", WebcheckSampleQualities);
                datareadwrite.ExecuteSQL(create_sql);
            }
            string sql_select = string.Format("select * from {0} where mapid='{1}'  ", WebcheckSampleQualities, _sMapid);

            DataTable dt = datareadwrite.GetDataTableBySQL(sql_select);
            Store1.DataSource = dt;
            Store1.DataBind();

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseDesignPlus;
using System.Data;
using Authentication.Class;

namespace WebMapCheck
{
    public partial class MyProjects : System.Web.UI.Page
    {
        DatabaseDesignPlus.IDatabaseReaderWriter datareadwrite;
        string _sDbConnectionString = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                //string userid = HttpUtility.UrlDecode(Request["userid"]);
                string userid = Session["userid"] as string;
                if (userid == "" || userid == null)
                {
                    //MessageBox.Show("您当前用户名在本机还未授权，请申请授权或等待管理员授权！");
                    //ClientScript.RegisterStartupScript(this.GetType(), "温馨提示", "alert('提示：请您登录本系统后在查看页面！');", true);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "opennewwindow", "alert('提示：请您登录本系统后在查看页面！');", true);
                    //Response.Redirect("~/Default.aspx");
                    return;
                }

                _sDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];

                datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _sDbConnectionString);
                UserObject userobj = UserAuthenticate.GetUserObject(datareadwrite, userid);
                if (userobj.authorized != "1" || userobj.authorized == null)
                {
                    //MessageBox.Show("您当前用户名在本机还未授权，请申请授权或等待管理员授权！");
                    ClientScript.RegisterStartupScript(this.GetType(), "温馨提示", "alert('提示：您当前用户名在本机还未授权，请申请授权或等待管理员授权！');", true);

                    Response.Redirect("~/Default.aspx");
                    return;
                }

                string sqlfillcb1 = string.Format("select distinct 成果名称,批量,样本数量,批量单位,比例尺 from {0} order by 成果名称 asc ", "位置精度检测项目信息表");
                DataTable projects = datareadwrite.GetDataTableBySQL(sqlfillcb1);
                GridView1.DataSource = projects;
                //GridView1.AutoGenerateColumns = false;
                GridView1.DataBind();
            }
        }

    }
}
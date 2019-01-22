using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseDesignPlus;
using System.Data;

namespace WebMapCheck
{
    public partial class ProjectDetail : System.Web.UI.Page
    {
        DatabaseDesignPlus.IDatabaseReaderWriter datareadwrite;
        string _sDbConnectionString = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string projectname = HttpUtility.UrlDecode(Request["project"]);
                _sDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];

                datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _sDbConnectionString);
                string sqlfillcb1 = string.Format("select distinct 地形,平面精度限差（mm）,高程精度限差（m）,间距精度限差（mm）,检测精度类型 from {0} where 成果名称 = '{1}' ", "位置精度检测项目信息表", projectname);
                DataTable projects = datareadwrite.GetDataTableBySQL(sqlfillcb1);
                GridView1.DataSource = projects;
                //GridView1.AutoGenerateColumns = false;
                GridView1.DataBind();
            }
        }
    }
}
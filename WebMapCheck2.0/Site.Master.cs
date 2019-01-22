using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebMapCheck
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void NavigationMenu_MenuItemClick(object sender, MenuEventArgs e)
        {
            MenuItem mitem = (sender as Menu).SelectedItem;
            switch (mitem.Text)
            {
                case "我的项目":
                    Response.Redirect("~/MyProjects.aspx");
                    break;
                case "位置精度检测项目信息入库":
                case "平面及高程精度检测点成果入库":
                case "间距边长精度检测点成果表入库":
                    Response.Redirect("~/NewProject.aspx?id=" + mitem.Value);
                    break;
                case "主页":
                    Response.Redirect("~/Default.aspx");
                    break;
                case "关于":
                    Response.Redirect("~/About.aspx");
                    break;


            }
        }
    }
}

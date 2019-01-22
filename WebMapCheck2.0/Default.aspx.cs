using System;
using System.Web;
using System.Net;
using DatabaseDesignPlus;
using Authentication.Class;

namespace WebMapCheck
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string loginuserid = Session["userid"] as string;
            if (loginuserid != null && loginuserid!="")
            {
                string redirecturl = string.Format("~/WebCheckProjects.aspx?userid={0}", loginuserid);
                Response.Redirect(redirecturl);

            }
        }

        protected void bt_login_Click(object sender, EventArgs e)
        {
            //使用统一认证中心进行用户认证
            string userobjson = "";
            string url = string.Format(@"http://localhost:5155/userauth?username={0}&password={1}", HttpUtility.UrlEncode(tb_username.Text), HttpUtility.UrlEncode(tb_password.Text));
            HttpWebRequest myHttpWebRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            using (HttpWebResponse res = (HttpWebResponse)myHttpWebRequest.GetResponse())
            {
                if (res.StatusCode == HttpStatusCode.OK || res.StatusCode == HttpStatusCode.PartialContent)//返回为200或206
                {
                    string dd = res.ContentEncoding;
                    System.IO.Stream strem = res.GetResponseStream();
                    System.IO.StreamReader r = new System.IO.StreamReader(strem);
                    userobjson = r.ReadToEnd();
                }
            }
            string dbconnection = System.Configuration.ConfigurationManager.AppSettings["Login"];
            IDatabaseReaderWriter dbReader = null;
            dbReader = new ClsPostgreSql(dbconnection);
            UserAuthenticate userauth = new UserAuthenticate(dbReader);
            UserObject userobj = userauth.FetchUser(userobjson);
            //////////////////////////////////
            if (userobj.username == null)
            {
                //MessageBox.Show("未找到对应的用户名和密码，请检查输入是否正确！");
                ClientScript.RegisterStartupScript(this.GetType(), "温馨提示", "alert('提示：未找到对应的用户名和密码，请检查输入是否正确！');", true);

                return;
            }

            if (userobj.authorized != "1" || userobj.authorized == null)
            {
                //MessageBox.Show("您当前用户名在本机还未授权，请申请授权或等待管理员授权！");
                ClientScript.RegisterStartupScript(this.GetType(), "温馨提示", "alert('提示：您当前用户名在本机还未授权，请申请授权或等待管理员授权！');", true);

                return;
            }

            
            //登录用户的id将保存到session中，如果session失效则不能访问
            Session["userid"] = userobj.userid;
            //string returnURL = string.Format(@"~/MyProjects.aspx?userid={0}", HttpUtility.UrlEncode(userobj.userid));
            //Response.Redirect(returnURL);
            //登录到Ext.NET用户项目界面
            string returnURL = string.Format(@"~/WebCheckProjects.aspx?userid={0}", HttpUtility.UrlEncode(userobj.userid));
            //Server.Transfer(returnURL, true);
            //A bit strange, the way I fixed this..
            //Earlier i was using Server.Transfer(..) to redirect the page then I started facing the same problem
            ///Then I searched a lot and in the end I changed that Server.Transfer(..) to Response.Redirect(..), And ta-da it worked perfectly..
            //Hope this helps you :-)

            Response.Redirect(returnURL);
        }
    }
}

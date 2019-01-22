using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Authentication.Class;
using DatabaseDesignPlus;
using System.Configuration;

namespace AuthenticationService
{
    public class AuthenticationServer : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }
        public void ProcessRequest(HttpContext context)
        {
            IDatabaseReaderWriter dbReader = null;
            dbReader = new ClsPostgreSql(ConfigurationManager.AppSettings["ConnectionString"]);
            string username = HttpUtility.UrlDecode(context.Request["username"]);
            string password = HttpUtility.UrlDecode(context.Request["password"]);
            UserAuthenticate userauth = new UserAuthenticate(dbReader);

            UserObject userobj = userauth.FetchUser(username, password);
            string userobjosn = userauth.ToUserObjson(userobj);

            context.Response.Write(userobjosn);
        }
    }
}
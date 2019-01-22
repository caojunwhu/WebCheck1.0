using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseDesignPlus;
using System.Data;
using Newtonsoft.Json;
using System.IO;

namespace Authentication.Class
{
    //json作为网络通信的一种机制，被应用到用户信息传输，作为通信协议，.net服务器定制通信机制，定义传输内容。
    public class UserAuthenticate
    {
        string usertablename = "用户表";
        IDatabaseReaderWriter ireadwrite;
        public  UserAuthenticate(IDatabaseReaderWriter drw)
        {
            ireadwrite = drw;
        }

        //通过用户标识码获取用户数据包
        public static UserObject GetUserObject(IDatabaseReaderWriter ireadwrite, string userid)
        {
            UserObject userobj = new UserObject();
            if (userid == null)
                return userobj;

            string usertablename = "用户表";

            string queryuser_sql = string.Format("select userid,username,password,ipaddress,macaddress,createtime,authorized,company from {0} where userid='{1}'  ", ireadwrite.GetTableName(usertablename), userid);

            DataTable dt = ireadwrite.GetDataTableBySQL(queryuser_sql);
            if (dt != null && dt.Rows.Count == 1)
            {
                DataRow dr = dt.Rows[0];
                foreach (DataColumn colname in dt.Columns)
                {
                    System.Reflection.PropertyInfo property = userobj.GetType().GetProperty(colname.ColumnName);
                    property.SetValue(userobj, dr[colname] as string, null);
                }
                //查询成功时记录到用户动作表中，便于后期数据分析

            }

            return userobj;
        }

        //将查询到的用户信息打包传回客户端，如果查询不到则传回默认用户，可以是空用户可以是匿名；
        //查询操作成功时，需要记录用户登陆信息
        public UserObject FetchUser(string username, string password)
        {
            UserObject userobj = new UserObject();
            string queryuser_sql = string.Format("select userid,username,password,ipaddress,macaddress,createtime,authorized,company from {0} where username='{1}' and password='{2}' ",ireadwrite.GetTableName( usertablename),username,password);

            DataTable dt = ireadwrite.GetDataTableBySQL(queryuser_sql);
            if (dt != null && dt.Rows.Count == 1)
            {
                DataRow dr = dt.Rows[0];
                foreach (DataColumn colname in dt.Columns)
                {
                    System.Reflection.PropertyInfo property = userobj.GetType().GetProperty(colname.ColumnName);
                    property.SetValue(userobj, dr[colname] as string, null);
                }
                //查询成功时记录到用户动作表中，便于后期数据分析
                string logdtname = "用户日志";
                if(ireadwrite.GetSchameDataTableNames().IndexOf(logdtname)>=0)
                {
                    string insertsql = string.Format("insert into {0}(userid,username,company,logtime)values('{1}','{2}','{3}','{4}')", logdtname, userobj.userid, userobj.username, userobj.company, DateTime.Now.ToShortDateString());
                    ireadwrite.ExecuteSQL(insertsql);

                }
                else
                {
                    string createLogdt = string.Format("create table {0} (userid text,username text,company text,logtime text)", logdtname);
                    ireadwrite.ExecuteSQL(createLogdt);

                    string insertsql = string.Format("insert into {0}(userid,username,company,logtime)values('{1}','{2}','{3}','{4}')",logdtname, userobj.userid, userobj.username, userobj.company,DateTime.Now.ToLongDateString());
                    ireadwrite.ExecuteSQL(insertsql);
                }
            }

            return userobj;
        }

        //用户信息从json格式中还原出来，作为一种格式转换，网络与后台交互的方式
        public UserObject FetchUser(string userobjson)
        {
            //输入检查
            //数据转换
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(userobjson);
            UserObject userobj = (UserObject)serializer.Deserialize(new JsonTextReader(sr), typeof(UserObject));
            return userobj;
        }

        //将用户信息重新打包为json格式方便网络传输
        public string ToUserObjson(UserObject userobj)
        {
            string userobjson = "";
            StringWriter sw = new StringWriter();
            JsonTextWriter jw = new JsonTextWriter(sw);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(jw, userobj);
            //sw.Write(userobjson);
            userobjson = sw.ToString();
            return userobjson;
        }

    }
}

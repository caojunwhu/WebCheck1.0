using DatabaseDesignPlus;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;

namespace WebMapCheck
{
    /// <summary>
    /// AutoComplete 的摘要说明
    /// </summary>
    public class AutoComplete : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //text表示用户在文本框输入的内容
            string text = context.Request.QueryString["q"];
            string action = context.Request.QueryString["action"];
            string value = context.Request.QueryString["value"];
            // string strResult = "guo\ntong\nchang\nwang\nhao\nbang";
            // context.Response.Write(strResult);

            string inputstr = value;
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);
            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);

            string select_sql = string.Format("select distinct 错漏内容 from  ah错漏分类表 where 错漏内容 like '%{0}%'", text);
            DataTable datatable = datareadwrite.GetDataTableBySQL(select_sql);
            string strResult = "";
            foreach (DataRow dr in datatable.Rows)
            {
                strResult += dr[0] as string + "\n";
            }
            context.Response.Write(strResult);

        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

}
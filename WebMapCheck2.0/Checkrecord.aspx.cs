using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseDesignPlus;
using System.Data;
using System.IO;
using ReportPrinter;
using Newtonsoft.Json;
using Core;

namespace WebMapCheck
{
    public partial class Checkrecord : System.Web.UI.Page
    {
        DatabaseDesignPlus.IDatabaseReaderWriter datareadwrite;
        string _sDbConnectionString = "";
        string MapNumber = "";
        Root root;
        string rooturl = "http://localhost:5152/";
        string rootvirtualpath = "~/";

        protected void Page_Load(object sender, EventArgs e)
        {
            MapNumber = HttpUtility.UrlDecode(Request["mapnumber"]);
            Label1.Text = string.Format("抽样图幅号——{0}", MapNumber); 
            root = Root.FromXML(Server.MapPath(rootvirtualpath) + "Functions.xml");

            if (!Page.IsPostBack)
            {

                _sDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
                datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _sDbConnectionString);

                string sqlfillcb1 = string.Format("select * from {0} where mapnumber = '{1}' order by ptid  asc ", "平面及高程精度检测点成果表", MapNumber);
                DataTable checkrecord = datareadwrite.GetDataTableBySQL(sqlfillcb1);
                GridView1.DataSource = checkrecord;
                GridView1.DataBind();

                string sqlfillcb2 = string.Format("select * from {0} where mapnumber = '{1}' order by ptid  asc ", "间距边长精度检测点成果表", MapNumber);
                DataTable checkrecord2 = datareadwrite.GetDataTableBySQL(sqlfillcb2);
                GridView2.DataSource = checkrecord2;
                GridView2.DataBind();
            }
        }


        //该样本平面与高程误差统计表
        protected void Button1_Click(object sender, EventArgs e)
        {
            Function printfunc = root.GetFunction("检测点精度统计表打印");
            Dictionary<string, string> Configs = new Dictionary<string, string>();
            foreach (Config cfig in printfunc.Configs)
            {
                Configs.Add(cfig.Key, cfig.Value);
            }
            Dictionary<string, string> Databases = new Dictionary<string, string>();
            foreach (Database rdbs in root.Databases)
            {
                Databases.Add(rdbs.Key, rdbs.Value.Replace(@"{*}\", Server.MapPath(rootvirtualpath)));
            }

            //string sProjectName = cb1.Text;
            string sMapNumber = MapNumber;

            //计算中误差
            /*  PositionMeanError pme = new PositionMeanError();
              pme.QueryParameter(sMapNumber);
              pme.Calc(sMapNumber);
              pme.UpdateReslut(sMapNumber);

              HeightMeanError hme = new HeightMeanError();
              hme.QueryParameter(sMapNumber);
              hme.Calc(sMapNumber);
              hme.UpdateReslut(sMapNumber);*/

            //打印中误差统计表
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(Configs["CheckPointsErrorStatisticPrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));
            PrintReport pr = new PrintReport();
            pr.AppPath = Server.MapPath(rootvirtualpath);
            pr.Databases = Databases;
            pr.Configs = Configs;
            pr.PrintInit(pPrintEnvironment);
            try
            {
                string word = pr.PrintWord(sMapNumber);
                if (File.Exists(word))
                {
                    string reporturl = string.Format("{0}{1}", rooturl, Path.GetFileName(word));
                    System.Diagnostics.Process.Start(reporturl);
                }
            }
            catch (Exception ex)
            {
                //==MessageBox.Show(ex.Message);
                return;
            }
        }

        //该样本间距误差统计表
        protected void Button2_Click(object sender, EventArgs e)
        {
            Function printfunc = root.GetFunction("间距边长误差统计表打印");
            Dictionary<string, string> Configs = new Dictionary<string, string>();
            foreach (Config cfig in printfunc.Configs)
            {
                Configs.Add(cfig.Key, cfig.Value);
            }
            Dictionary<string, string> Databases = new Dictionary<string, string>();
            foreach (Database rdbs in root.Databases)
            {
                Databases.Add(rdbs.Key, rdbs.Value.Replace(@"{*}\", Server.MapPath(rootvirtualpath)));
            }

            //string sProjectName = cb1.Text;
            string sMapNumber = MapNumber;

            //计算误差
            /* RelativeMeanError rme = new RelativeMeanError();
             rme.QueryParameter(sMapNumber);
             rme.Calc(sMapNumber);
             rme.UpdateReslut(sMapNumber);*/

            //打印误差统计表
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(Configs["RelativeErrorStatisticPrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));
            PrintReport pr = new PrintReport();
            pr.AppPath = Server.MapPath(rootvirtualpath);
            pr.Databases = Databases;
            pr.Configs = Configs;
            pr.PrintInit(pPrintEnvironment);
            try
            {
                string word = pr.PrintWord(sMapNumber);
                if (File.Exists(word))
                {
                    string reporturl = string.Format("{0}{1}", rooturl, Path.GetFileName(word));
                    System.Diagnostics.Process.Start(reporturl);
                }
            }
            catch (Exception ex)
            {
                //==MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}
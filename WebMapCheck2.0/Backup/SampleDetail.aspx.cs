using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DatabaseDesignPlus;
using System.Data;
using Newtonsoft.Json;
using System.IO;
using ReportPrinter;
using Eipsoft.Common;
using Core;
using System.Diagnostics;

namespace WebMapCheck
{
    public partial class SampleDetail : System.Web.UI.Page
    {
        DatabaseDesignPlus.IDatabaseReaderWriter datareadwrite;
        string _sDbConnectionString = "";
        string projectname = "";
        Root root;
        string rooturl = "~/";//"http://localhost:5152/";
        string rootvirtualpath = "~/";

        protected void Page_Load(object sender, EventArgs e)
        {
            projectname = HttpUtility.UrlDecode(Request["project"]);
            _sDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _sDbConnectionString);
            root = Root.FromXML(Server.MapPath(rootvirtualpath) + "Functions.xml");
            Label1.Text = string.Format("检验项目——“{0}”抽样详情：", projectname);
            if (!Page.IsPostBack)
            {

                string sqlfillcb1 = string.Format("select 抽样分区,流水号,图幅号,点位中误差,  点位粗差比率,  点位精度得分,  高程中误差,  高程粗差比率,  高程精度得分,  等高线中误差,  等高线粗差比率,  等高线精度得分,  间距中误差,  间距粗差比率,  间距精度得分,  备注 from {0} where 成果名称 = '{1}' order by 抽样分区,流水号 asc ", "位置精度检测项目信息表", projectname);
                DataTable projects = datareadwrite.GetDataTableBySQL(sqlfillcb1);
                GridView1.DataSource = projects;
                GridView1.DataBind();
            }
        }
        private void beginProgress()
        {
            //根据ProgressBar.htm显示进度条界面   
            string templateFileName = System.IO.Path.Combine(Server.MapPath("."), "ProgressBar.htm");
            System.IO.StreamReader reader = new System.IO.StreamReader(@templateFileName, System.Text.Encoding.GetEncoding("GB2312"));
            string html = reader.ReadToEnd();
            reader.Close();
            Response.Write(html);
            Response.Flush();
        }

        private void setProgress(int percent,string status)
        {
            string jsBlock = "<script>SetPorgressBar('" +  percent.ToString()+"','" + status+ "'); </script>";
            Response.Write(jsBlock);
            Response.Flush();
        }

        private void finishProgress()
        {
            string jsBlock = "<script>SetCompleted();</script>";
            Response.Write(jsBlock);
            Response.Flush();
        }

        //打印统计表时，需要对当前用户是否正在进行打印的状态进行判断，
        //如果后台仍然在计算，则拒绝该用户的点击请求，已节省服务器资源
        //==Todo同时需要检查服务器word进程，如果没有进程了，则要释放打印状态（异常情况处理）
        int printstate = 0;
        bool GetPrintState()
        {
            //检查系统word进程，如果数量为〇则绝对是没有打印
            Process[] process = Process.GetProcesses();
            int wordcount = 0;
            foreach (Process proc in process)
            {
                if (proc.ProcessName.ToUpper() == "WINWORD")
                    wordcount++;
            }
            if (wordcount > 2)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "温馨提示", "alert('提示：服务器正忙着打印别家的文档，请稍后再试!');", true);
                return true;
            }

            if(Session["printstate"] ==null)
                return false;
            if (Convert.ToInt32(Session["printstate"])  == 0)
                return false;

            if (Convert.ToInt32(Session["printstate"])==1)
            {
                //word进程熄灭后自动恢复打印状态
                if (wordcount == 0)
                {
                    Session["printstate"] = 0;
                    return false;
                }

                ClientScript.RegisterStartupScript(this.GetType(), "温馨提示", "alert('提示：服务器正忙着打印您的文档，请稍后再试!');", true);
                return true;
            }
            return true;
        }
        void SetPrintState(int printstate)
        {
            Session["printstate"] = printstate;
        }

        //打印间距精度统计表
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (GetPrintState() == true)
                return;
            SetPrintState(1);
            Btn_ExportALLClick(sender, e);
            SetPrintState(0);
        }

        void Btn_ExportALLClick(object sender, System.EventArgs e)
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

            string sProjectName = projectname;
            string sMapNumber = "";
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(Configs["CheckPointsErrorStatisticPrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));

            DataTable dt = null;
            try
            {
                string sqlGetMapNumbers = string.Format("select 图幅号 from {0} where 成果名称='{1}' order by 抽样分区 desc,流水号 desc", Configs["TopographicMapLocationAccuracyCheckDataTableName"], sProjectName);
                dt = datareadwrite.GetDataTableBySQL( sqlGetMapNumbers);
            }
            catch (Exception ex)
            {
                string sMsg = string.Format("项目{0}中图幅数为零，请检查是否已经正确导入检测点成果表！系统报错：{1}。提示：批量生成报告需要按抽样分区和流水号排序，流水号应为数字，不可以使用字母。", sProjectName, ex.Message);
                //==MessageBox.Show(sMsg);
                return;
            }

            //创建进度条
            beginProgress();
            string[] wordList = new string[dt.Rows.Count];
            int wordindex = 0;
            int istep = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string WordFilePath = "";
                try
                {
                    sMapNumber = Convert.ToString(dr["图幅号"]);
                    /* //计算中误差
                     PositionMeanError pme = new PositionMeanError();
                     pme.QueryParameter(sMapNumber);
                     pme.Calc(sMapNumber);
                     pme.UpdateReslut(sMapNumber);

                     HeightMeanError hme = new HeightMeanError();
                     hme.QueryParameter(sMapNumber);
                     hme.Calc(sMapNumber);
                     hme.UpdateReslut(sMapNumber);*/

                    istep++;
                    int percent =(int)Math.Floor((float)istep/dt.Rows.Count*99);
                    string status = string.Format("正在打印样本{0}检测点精度统计表,当前总体进度：{1}%", sMapNumber,percent);
                    setProgress(percent, status);
                    //==Invoke(showProgress, new object[] { dt.Rows.Count, istep });

                    PrintReport pr = new PrintReport();
                    pr.AppPath = Server.MapPath(rootvirtualpath);
                    pr.Databases = Databases;
                    pr.Configs = Configs;
                    pr.PrintInit(pPrintEnvironment);
                    WordFilePath = pr.PrintWord(sMapNumber);

                }
                catch (Exception ex)
                {
                    string sMsg = string.Format("导出项目{0}图幅{1}报告失败：{2}", sProjectName, sMapNumber, ex.Message);
                    //MessageBox.Show(sMsg);
                    //return;
                    LogOut.Info(sMsg);
                }
                if (File.Exists(WordFilePath))
                {
                    wordList[wordindex++] = WordFilePath;
                }

            }

            setProgress(100, "正在汇总，请稍候");

            //记录第一个 
            string finalWordPath = "";
            try
            {
                if (wordindex > 1)
                {
                    string wordOrg = wordList[0];
                    string[] wordToMerge = new string[wordindex - 1];
                    Array.Copy(wordList, 1, wordToMerge, 0, wordindex - 1);
                    finalWordPath = string.Format("{0}\\{1}{2}.doc", Server.MapPath(rootvirtualpath), sProjectName, Configs["CheckPointsTableName"]);
                    WordDocumentMerger dm = new WordDocumentMerger();
                    //逆序插入
                    Array.Reverse(wordToMerge);
                    dm.InsertMerge(wordOrg, wordToMerge, finalWordPath);
                }
                else
                {
                    finalWordPath = wordList[0];
                }
                // 做好清理工作
                if (wordindex > 1)
                {
                    foreach (string word in wordList)
                    {
                        if (File.Exists(word))
                            File.Delete(word);
                    }
                }
                string sMsg = string.Format("质检项目 ({0}) 的检测报告成功导出，是否打开查看！", sProjectName);
                //==if (MessageBox.Show(sMsg, "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    finalWordPath = string.Format("{0}{1}{2}.doc", rooturl, sProjectName, Configs["CheckPointsTableName"]);
                    System.Diagnostics.Process.Start(finalWordPath);
                };
            }
            catch (Exception ex)
            {
                string sMsg = string.Format("合并项目{0}报告时失败：{1}", sProjectName, ex.Message);
                //==MessageBox.Show(sMsg);
            }
            //结束进度条显示
            finishProgress();
        }

        //样本图幅检测精度统计表打印
        protected void Button3_Click(object sender, EventArgs e)
        {
            if (GetPrintState() == true)
                return;
            SetPrintState(1);

            Function printfunc = root.GetFunction("样本图幅检测精度统计表打印");
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


            string sProjectName = projectname;

            //打印中误差统计表
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(Configs["PrintCheckItemsQulityReportEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));
            PrintQulityReport pr = new PrintQulityReport();
            pr.AppPath = Server.MapPath(rootvirtualpath);
            pr.Databases = Databases;
            pr.Configs = Configs;
            pr.PrintInit(pPrintEnvironment);
            try
            {
                string word = pr.PrintWord(sProjectName);
                if (File.Exists(word))
                {
                    string reporturl = string.Format("{0}{1}", rooturl,Path.GetFileName(word));
                    System.Diagnostics.Process.Start(reporturl);
                }
            }
            catch (Exception ex)
            {
                //==MessageBox.Show(ex.Message);
                return;
            }
            SetPrintState(0);
        }
        //打印间距统计表
        protected void Button2_Click(object sender, EventArgs e)
        {
            if (GetPrintState() == true)
                return;
            SetPrintState(1);

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


            string sProjectName = projectname;
            string sMapNumber = "";

            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(Configs["RelativeErrorStatisticPrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));

            DataTable dt = null;
            try
            {
                string sqlGetMapNumbers = string.Format("select 图幅号 from {0} where 成果名称='{1}' order by 抽样分区 desc,流水号 desc", Configs["TopographicMapLocationAccuracyCheckDataTableName"], sProjectName);
                //Clsmdb mdb = new Clsmdb();
                //dt = mdb.GetMdbDataTableBySql(CallClass.Databases["SPIDatabaseFilePath"], sqlGetMapNumbers);
                dt = datareadwrite.GetDataTableBySQL( sqlGetMapNumbers);
            }
            catch (Exception ex)
            {
                string sMsg = string.Format("项目{0}中图幅数为零，请检查是否已经正确导入检测点成果表！系统报错：{1}。提示：批量生成报告需要按抽样分区和流水号排序，流水号应为数字，不可以使用字母。", sProjectName, ex.Message);
                //==MessageBox.Show(sMsg);
                return;
            }
            //打开进度条
            beginProgress();

            string[] wordList = new string[dt.Rows.Count];
            int wordindex = 0;
            int istep = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string WordFilePath = "";
                try
                {
                    sMapNumber = Convert.ToString(dr["图幅号"]);
                    //计算误差
                    /*RelativeMeanError rme = new RelativeMeanError();
                    rme.QueryParameter(sMapNumber);
                    rme.Calc(sMapNumber);
                    rme.UpdateReslut(sMapNumber);*/


                    istep++;
                    //==Invoke(showProgress, new object[] { dt.Rows.Count, istep });
                    int percent = (int)Math.Floor((float)istep / dt.Rows.Count * 99);
                    string status = string.Format("正在打印样本{0}间距边长误差统计表,当前总体进度：{1}%", sMapNumber, percent);
                    setProgress(percent, status);

                    PrintReport pr = new PrintReport();
                    pr.AppPath = Server.MapPath(rootvirtualpath);
                    pr.Databases = Databases;
                    pr.Configs = Configs;
                    pr.PrintInit(pPrintEnvironment);
                    WordFilePath = pr.PrintWord(sMapNumber);

                }
                catch (Exception ex)
                {
                    string sMsg = string.Format("导出项目{0}图幅{1}报告失败：{2}", sProjectName, sMapNumber, ex.Message);
                    //MessageBox.Show(sMsg);
                    //return;
                    LogOut.Info(sMsg);
                }
                if (File.Exists(WordFilePath))
                {
                    wordList[wordindex++] = WordFilePath;
                }

            }

            setProgress(100, "正在汇总，请稍候");

            //记录第一个 
            string finalWordPath = "";
            try
            {
                if (wordindex > 1)
                {
                    string wordOrg = wordList[0];
                    string[] wordToMerge = new string[wordindex - 1];
                    Array.Copy(wordList, 1, wordToMerge, 0, wordindex - 1);
                    finalWordPath = string.Format("{0}\\{1}{2}.doc", Server.MapPath(rootvirtualpath), sProjectName, Configs["RelativeCheckPointsTableName"]);
                    WordDocumentMerger dm = new WordDocumentMerger();
                    //逆序插入
                    Array.Reverse(wordToMerge);
                    dm.InsertMerge(wordOrg, wordToMerge, finalWordPath);
                }
                else
                {
                    finalWordPath = wordList[0];
                }
                // 做好清理工作
                if (wordindex > 1)
                {
                    foreach (string word in wordList)
                    {
                        if (File.Exists(word))
                            File.Delete(word);
                    }
                }
                string sMsg = string.Format("质检项目 ({0}) 的检测报告成功导出，是否打开查看！", sProjectName);
               //== if (MessageBox.Show(sMsg, "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    finalWordPath = string.Format("{0}{1}{2}.doc", rooturl, sProjectName, Configs["RelativeCheckPointsTableName"]);
                    System.Diagnostics.Process.Start(finalWordPath);
                };
            }
            catch (Exception ex)
            {
                string sMsg = string.Format("合并项目{0}报告时失败：{1}", sProjectName, ex.Message);
               //== MessageBox.Show(sMsg);
            }
            //结束进度条
            finishProgress();
            SetPrintState(0);
        }

        protected void btn_compPosition_Click(object sender, EventArgs e)
        {
     
            Btn_CalcPositionHeightClick(sender, e);

        }

        protected void btn_relativeComp_Click(object sender, EventArgs e)
        {

            Btn_CalcRelativeClick(sender, e);

        }

        void Btn_CalcPositionHeightClick(object sender, System.EventArgs e)
        {
            Function printfunc = root.GetFunction("检测中误差及得分计算");
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

            string sProjectName = projectname; ;
            string sMapNumber = "";
            DataTable dt = null;
            try
            {
                string sqlGetMapNumbers = string.Format("select 图幅号 from {0} where 成果名称='{1}' order by 抽样分区 desc,流水号 desc", Configs["TopographicMapLocationAccuracyCheckDataTableName"], sProjectName);
                //Clsmdb mdb = new Clsmdb();
                //dt = mdb.GetMdbDataTableBySql(CallClass.Databases["SPIDatabaseFilePath"], sqlGetMapNumbers);
                dt = datareadwrite.GetDataTableBySQL( sqlGetMapNumbers);
            }
            catch (Exception ex)
            {
                string sMsg = string.Format("项目{0}中图幅数为零，请检查是否已经正确导入检测点成果表！系统报错：{1}。提示：批量生成报告需要按抽样分区和流水号排序，流水号应为数字，不可以使用字母。", sProjectName, ex.Message);
                //==MessageBox.Show(sMsg);
                return;
            }

            //打开进度条
            beginProgress();

            int istep = 0;
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    sMapNumber = Convert.ToString(dr["图幅号"]);
                    //计算中误差
                    PositionMeanError pme = new PositionMeanError(Configs,Databases);
                    pme.QueryParameter(sMapNumber);
                    pme.Calc(sMapNumber);
                    pme.UpdateReslut(sMapNumber);

                    HeightMeanError hme = new HeightMeanError(Configs, Databases);
                    hme.QueryParameter(sMapNumber);
                    hme.Calc(sMapNumber);
                    hme.UpdateReslut(sMapNumber);

                    istep++;
                    int percent = (int)Math.Floor((float)istep / dt.Rows.Count * 100);
                    string status = string.Format("正在统计样本{0}平面高程误差精度,当前总体进度：{1}%", sMapNumber, percent);
                    setProgress(percent, status);
                    //==Invoke(showProgress, new object[] { dt.Rows.Count, istep });

                }
                catch (Exception ex)
                {
                    string sMsg = string.Format("导出项目{0}图幅{1}报告失败：{2}", sProjectName, sMapNumber, ex.Message);
                    LogOut.Info(sMsg);
                }
            }
            //结束进度条
            finishProgress();
            //ClientScript.RegisterStartupScript(this.GetType(), "updataOK", "alert('提示：数据导入成功!');", true);
            //Response.Redirect("~/MyProjects.aspx");
        }

        void Btn_CalcRelativeClick(object sender, System.EventArgs e)
        {
            Function printfunc = root.GetFunction("检测中误差及得分计算");
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

            string sProjectName = projectname; ;
            string sMapNumber = "";

            DataTable dt = null;
            try
            {
                string sqlGetMapNumbers = string.Format("select 图幅号 from {0} where 成果名称='{1}' order by 抽样分区 desc,流水号 desc", Configs["TopographicMapLocationAccuracyCheckDataTableName"], sProjectName);
                //Clsmdb mdb = new Clsmdb();
                //dt = mdb.GetMdbDataTableBySql(CallClass.Databases["SPIDatabaseFilePath"], sqlGetMapNumbers);
                dt = datareadwrite.GetDataTableBySQL(sqlGetMapNumbers);
            }
            catch (Exception ex)
            {
                string sMsg = string.Format("项目{0}中图幅数为零，请检查是否已经正确导入检测点成果表！系统报错：{1}。提示：批量生成报告需要按抽样分区和流水号排序，流水号应为数字，不可以使用字母。", sProjectName, ex.Message);
                //==MessageBox.Show(sMsg);
                return;
            }


            //打开进度条
            beginProgress();

            int istep = 0;
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    sMapNumber = Convert.ToString(dr["图幅号"]);
                    //计算误差
                    RelativeMeanError rme = new RelativeMeanError(Configs, Databases);
                    rme.QueryParameter(sMapNumber);
                    rme.Calc(sMapNumber);
                    rme.UpdateReslut(sMapNumber);

                    istep++;
                    int percent = (int)Math.Floor((float)istep / dt.Rows.Count * 100);
                    string status = string.Format("正在统计样本{0}间距误差精度,当前总体进度：{1}%", sMapNumber, percent);
                    setProgress(percent, status);

                    //Invoke(showProgress, new object[] { dt.Rows.Count, istep });
                }
                catch (Exception ex)
                {
                    string sMsg = string.Format("导出项目{0}图幅{1}报告失败：{2}", sProjectName, sMapNumber, ex.Message);
                    LogOut.Info(sMsg);
                }
            }
            //结束进度条
            finishProgress();
            //ClientScript.RegisterStartupScript(this.GetType(), "updataOK", "alert('提示：数据导入成功!');", true);
            //Response.Redirect("~/MyProjects.aspx");

        }
    }
}
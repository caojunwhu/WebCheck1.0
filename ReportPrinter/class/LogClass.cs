using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace ReportPrinter
{
public class LogOut
    {
    public static Dictionary<string, string> Configs { get; set; }
    public static Dictionary<string, string> Databases { get; set; }

        public static void Debug(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("LogOut");
            if (log.IsDebugEnabled)
            {
                log.Debug(message);
            }
            log = null;
        }

        public static void Error(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("LogOut");
            if (log.IsErrorEnabled)
            {
                log.Error(message);
                string showMessage = string.Format("系统错误：{0}。{1}。",message,"出错时点击“是”按钮打开日志获取更详细的信息，点击“否”按钮上传日志，点击“取消”按钮直接退出提示框");
                DialogResult dr = MessageBox.Show(showMessage,"警告",MessageBoxButtons.YesNoCancel);
                //出错时点击“是”按钮打开日志获取更详细的信息，点击“否”按钮上传日志，点击“取消”按钮直接退出提示框
                if(dr == DialogResult.Yes)
                {
                    if(File.Exists( Databases["ApplicationLogFilePath"]))
                    {
                        Process.Start( Databases["ApplicationLogFilePath"]);
                    }
                }else if(dr == DialogResult.No)
                {
                    if(Directory.Exists( Configs["ApplicationLogUploadPath"]))
                    {
                        string destFileName = string.Format("{0}\\{1}\\{2}\\{3}UpLoadLog.txt", Databases["ApplicationLogUploadPath"],GetClientIp(),DateTime.Now.Ticks);
                        File.Copy( Databases["ApplicationLogFilePath"],destFileName);
                    }
                }
            }
            log = null;
        }

        public static void Fatal(string message)
        {

            log4net.ILog log = log4net.LogManager.GetLogger("LogOut");
            if (log.IsFatalEnabled)
            {
                log.Fatal(message);
            }
            log = null;
        }
        public static void Info(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("LogOut");
            if (log.IsInfoEnabled)
            {
                log.Info(message);
            }
            log = null;
        }

        public static void Warn(string message)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("LogOut");
            if (log.IsWarnEnabled)
            {
                log.Warn(message);
            }
            log = null;
        }

        /// <summary>
        /// 获取客户端Ip
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp()
        {
            string clientIP = "";
            if (System.Web.HttpContext.Current != null)
            {
                clientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(clientIP) || (clientIP.ToLower() == "unknown"))
                {
                    clientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];
                    if (string.IsNullOrEmpty(clientIP))
                    {
                        clientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                }
                else
                {
                    clientIP = clientIP.Split(',')[0];
                }
            }
            return clientIP;
        }
    }
}

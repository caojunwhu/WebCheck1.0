using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Policy;

namespace ReportPrinter
{
    public  partial  class WordPrewview : Form
    {
        public WordPrewview()
        {
            InitializeComponent();
        }
        public void LoadWord(string wordfile)
        {

            InitOfficeControl(wordfile);
        }

        private void InitOfficeControl(string _sFilePath)
        {
            try
            {
                string sExt = System.IO.Path.GetExtension(_sFilePath).Replace(".", "");
                String sOpenType = LoadOpenFileType(sExt);
                //this.axFramerControl1.Open(_sFilePath, false, sOpenType, "", "");
                if (sOpenType.Equals("Word.Document"))
                {
                   // this.axFramerControl1.ShowView(3); //3这个值好像是页面视图
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("無法開啟文件: {0},系统错误：{1}", _sFilePath,ex.Message));
                this.Close();
            }
        }
        private string LoadOpenFileType(string _sExten)
        {
            try
            {
                string sOpenType = String.Empty;
                switch (_sExten.ToLower())
                {
                    case "xls":
                    case "xlsx":
                        sOpenType = "Excel.Sheet";
                        break;
                    case "doc":
                    case "docx":
                        sOpenType = "Word.Document";
                        break;
                    case "ppt":
                    case "pptx":
                        sOpenType = "PowerPoint.Show";
                        break;
                    default:
                        sOpenType = "Word.Document";
                        break;
                }
                return sOpenType;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void WordPrewview_Load(object sender, EventArgs e)
        {


        }

    }
}

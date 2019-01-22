using Authentication.Class;
using DatabaseDesignPlus;
using DevComponents.DotNetBar.SuperGrid;
using ESRI.ArcGIS.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginUI.Frms
{
    public partial class FrmJDCCCompanyInfo : Form
    {
        //private string pgconnectionstring;
        string dbconnection = "";
        AxMapControl _Map;
        public FrmJDCCCompanyInfo(Dictionary<string, string> configs, Dictionary<string, string> databases, UserObject oLoginUser, AxMapControl map)
        {
            InitializeComponent();

            _Map = map;

            dbconnection = System.Configuration.ConfigurationManager.AppSettings["Login"];
            dbconnection = DataBaseConfigs.RePlaceConfig(dbconnection);
            DatabaseDesignPlus.DatabaseReaderWriter dbReaderWriter = new DatabaseDesignPlus.ClsPostgreSql(dbconnection);
            string datatablename = "抽检安排计划表";
            DataTable table = dbReaderWriter.GetDataTable(datatablename);
            superGridControl1.PrimaryGrid.DataSource = table;
        }

        private void superGridControl1_Click(object sender, EventArgs e)
        {
            SelectedElementCollection sel = superGridControl1.PrimaryGrid.GetSelectedRows();
            if (sel.Count <= 0) return;

            string company = (sel[0] as GridRow)[1].Value as string;
            string sql = string.Format("select * from companydetial where companyname = '{0}'", company);
            DatabaseDesignPlus.DatabaseReaderWriter dbReaderWriter = new DatabaseDesignPlus.ClsPostgreSql(dbconnection.Replace("SurveryProductCheckDatabase", "mydb"));
            DataTable table = dbReaderWriter.GetDataTableBySQL(sql);
            

            string info = "";
            foreach(DataColumn dc in table.Columns)
            {
                if (table.Rows.Count <= 0) break;
                info += string.Format("{0}\r\n", table.Rows[0][dc]);
            }

            richTextBox1.Text = info;
        }

        private void FrmJDCCCompanyInfo_FormClosed(object sender, FormClosedEventArgs e)
        {
            string datatablename = "抽检安排计划表";
            DatabaseDesignPlus.DatabaseReaderWriter dbReaderWriter = new DatabaseDesignPlus.ClsPostgreSql(dbconnection);
            DataTable table = dbReaderWriter.GetDataTable(datatablename);

            DatabaseDesignPlus.DatabaseReaderWriter dbReaderWriter2 = new DatabaseDesignPlus.ClsPostgreSql(dbconnection.Replace("SurveryProductCheckDatabase", "mydb"));

            foreach(DataRow dr in table.Rows)
            {
                string filename = "C:\\output\\";
                foreach (DataColumn dc in table.Columns)
                {
                    if(table.Columns.IndexOf(dc)==0)
                    {
                        //index
                        string index = dr[dc] as string;
                        if (index.Length == 1)
                            filename += "0" + index+"-";
                        else
                            filename += index+"-";
                    }
                    else
                    {
                        filename += string.Format("{0}-", dr[dc] as string);
                    }
                    
                }
                filename += ".txt";

                FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
                if (File.Exists(filename) == false)
                    fs= File.Create(filename);
                StreamWriter sw = new StreamWriter(fs);

                string companyname = dr[1] as string;
                string sql2 = string.Format("select * from companydetial where companyname = '{0}'", companyname);
                DataTable table2 = dbReaderWriter2.GetDataTableBySQL(sql2);
                string info = "";
                foreach (DataColumn dc in table2.Columns)
                {
                    if (table2.Rows.Count <= 0) break;
                    info += string.Format("{0}\r\n", table2.Rows[0][dc]);
                }
                sw.Write(info);
                sw.Flush();
                fs.Flush();
                sw.Close();
                fs.Close();

            }

        

        }
    }
}

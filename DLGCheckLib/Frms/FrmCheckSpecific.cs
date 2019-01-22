using DatabaseDesignPlus;
using DevComponents.DotNetBar.SuperGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLGCheckLib.Frms
{
    public partial class FrmCheckSpecific : Form
    {
        public FrmCheckSpecific()
        {
            InitializeComponent();
        }
        string projectid = "";
        string preselectedSpecific = "";

        string selectedSpecifics = "";

        public string SelectedSpecific
        {
            get
            {
                GridItemsCollection sel = superGridControl1.PrimaryGrid.Rows;
                selectedSpecifics = "";
                foreach (GridRow r in sel)
                {
                    if(Convert.ToBoolean(r[0].Value) == true)
                    {
                        string number = r[1].Value as string;
                        selectedSpecifics += number + ";";
                    }
                }
                return selectedSpecifics;
            }

            set
            {
                selectedSpecifics = value;
            }
        }

        public FrmCheckSpecific(string sprojectid,string selectedSpecific)
        {
            InitializeComponent();

            projectid = sprojectid;
            preselectedSpecific = selectedSpecific;

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);
            string sql_checkspecific = string.Format("select 是否默认选中 ,文档编号,名称 from ah检验依据 order by 文档编号");

            DataTable dt = dbread.GetDataTableBySQL(sql_checkspecific);
            superGridControl1.PrimaryGrid.DataSource = dt;
            superGridControl1.Refresh();
           // System.Threading.Thread.Sleep(1000);




        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);
            string specifictable = "projectspecific";
            //create specifictable
            if (dbread.GetSchameDataTableNames().IndexOf(specifictable)<0)
            {
                string sql_create = string.Format("create table {0} (projectid text,specificnumber text)", specifictable);
                dbread.ExecuteSQL(sql_create);
            }

            string sql_clear = string.Format("delete from {0} where projectid='{1}'", specifictable, projectid);
            dbread.ExecuteSQL(sql_clear);

            //insert into specifictable values
            GridItemsCollection sel = superGridControl1.PrimaryGrid.Rows;

            foreach(GridRow r in  sel)
            {
                if(Convert.ToBoolean(r[0].Value)  == true)
                {
                    string number = r[1].Value as string;
                    string insertsql = string.Format("insert into {0} values('{1}','{2}')", specifictable, projectid, number);
                    dbread.ExecuteSQL(insertsql);
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void superGridControl1_DataBindingComplete(object sender, GridDataBindingCompleteEventArgs e)
        {
            if (preselectedSpecific != "")
            {
                string[] Specifics = preselectedSpecific.Split(';');

                GridItemsCollection sel = superGridControl1.PrimaryGrid.Rows;

                foreach (GridRow r in sel)
                {
                    r[0].Value = false;
                    string number = r[1].Value as string;
                    foreach (string s in Specifics)
                    {
                        if (number == s)
                            r[0].Value = true;
                    }
                }
            }
        }
    }
}

using DatabaseDesignPlus;
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
    public partial class FrmSurveyCompanySelecter : Form
    {
        public FrmSurveyCompanySelecter()
        {
            InitializeComponent();


        }
        private string selectedCompanyName = "";

        public string SelectedCompanyName
        {
            get
            {
                if (cklst_company.CheckedItems.Count == 1)
                {
                    selectedCompanyName = cklst_company.CheckedItems[0] as string;
                }
                return selectedCompanyName;
            }

            set
            {

                selectedCompanyName = value;
            }
        }

        public FrmSurveyCompanySelecter(string companyname)
        {
            InitializeComponent();

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);

            //1.province,district,city
            string sql_province = string.Format("select distinct province from companylocation order by province");
            List<string> province = dbread.GetSingleFieldValueList("province", sql_province);
            DatabaseDesignPlus.DatabaseReaderWriterFactory.FillCombox(province, cmb_province);

            // 2. match the given companyname 
            string sql_companyname = string.Format("select district ,province,city from companylocation where companyname = '{0}'",companyname);
            DataTable dt = dbread.GetDataTableBySQL(sql_companyname);
            if(dt.Rows.Count==1)
            {
                cmb_province.Text = dt.Rows[0]["province"] as string;

                string sql_city = string.Format("select distinct city from companylocation where province='{0}' order by city ",cmb_province.Text);
                List<string> city = dbread.GetSingleFieldValueList("city", sql_city);
                DatabaseReaderWriterFactory.FillCombox(city, cmb_city);
                cmb_city.Text = dt.Rows[0]["city"] as string;

                string sql_county = string.Format("select distinct district from companylocation where city = '{0}' order by district",cmb_city.Text);
                List<string> county = dbread.GetSingleFieldValueList("district", sql_county);
                DatabaseReaderWriterFactory.FillCombox(county, cmb_county);
                cmb_county.Text = dt.Rows[0]["district"] as string;

                //3. select companynames
                cklst_company.SelectionMode = SelectionMode.One;
                cklst_company.SelectedItems.Clear();
                string sql_companynames = string.Format("select companyname from companylocation where district ='{0}' order by companyname",cmb_county.Text);
                List<string> companynames = dbread.GetSingleFieldValueList("companyname", sql_companynames);
                //DatabaseReaderWriterFactory.FillCombox(companynames,cklst_company);
                foreach(string name in companynames)
                {
                    cklst_company.Items.Add(name);
                }
                int index = cklst_company.Items.IndexOf(companyname);
                if(index>=0)  cklst_company.SetItemChecked(index, true);

                string sql_companydetial = string.Format("select * from companydetial where companyname='{0}'", companyname);
                DataTable dt1 = dbread.GetDataTableBySQL(sql_companydetial);
                if(dt1.Rows.Count==1)
                {
                    string companydetial = "";
                    foreach(DataColumn dc in  dt1.Columns)
                    {
                        companydetial += dt1.Rows[0][dc] as string + "\r";
                    }
                    rtb_companydetial.Text = companydetial;
                }
            }
        }

        private void cmb_province_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);
            string sql_city = string.Format("select distinct city from companylocation where province='{0}' order by city ", cmb_province.Text);
            List<string> city = dbread.GetSingleFieldValueList("city", sql_city);
            DatabaseReaderWriterFactory.FillCombox(city, cmb_city);
            cmb_city.Text = "";
            cklst_company.Items.Clear();
        }

        private void cmb_city_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);
            string sql_county = string.Format("select distinct district from companylocation where city = '{0}' order by district", cmb_city.Text);
            List<string> county = dbread.GetSingleFieldValueList("district", sql_county);
            DatabaseReaderWriterFactory.FillCombox(county, cmb_county);
            cmb_county.Text = "";
            cklst_company.Items.Clear();
        }

        private void cmb_county_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);
            //3. select companynames
            cklst_company.Items.Clear();
            cklst_company.SelectionMode = SelectionMode.One;
            
            //cklst_company.SelectedItems.Clear();
            string sql_companynames = string.Format("select companyname from companylocation where district ='{0}' order by companyname", cmb_county.Text);
            List<string> companynames = dbread.GetSingleFieldValueList("companyname", sql_companynames);
            //DatabaseReaderWriterFactory.FillCombox(companynames,cklst_company);
            foreach (string name in companynames)
            {
                cklst_company.Items.Add(name);
            }
            //int index = cklst_company.Items.IndexOf(companyname);
            //if (index >= 0) cklst_company.SetItemChecked(index, true);

        }

        private void cklst_company_SelectedIndexChanged(object sender, EventArgs e)
        {
            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);

            string companyname = cklst_company.SelectedItem as string;
            for(int j=0;j<cklst_company.Items.Count;j++)
            {
                //int i = cklst_company.Items.IndexOf(item);
                cklst_company.SetItemChecked(j, false);
            }
            

            int index = cklst_company.Items.IndexOf(cklst_company.SelectedItem);
            cklst_company.SetItemChecked(index, true);

            string sql_companydetial = string.Format("select * from companydetial where companyname='{0}'", companyname);
            DataTable dt1 = dbread.GetDataTableBySQL(sql_companydetial);
            if (dt1.Rows.Count == 1)
            {
                string companydetial = "";
                foreach (DataColumn dc in dt1.Columns)
                {
                    companydetial += dt1.Rows[0][dc] as string + "\r";
                }
                rtb_companydetial.Text = companydetial;
            }
        }

        private void cklst_company_Click(object sender, EventArgs e)
        {
            //int index = cklst_company.Items.IndexOf(cklst_company.SelectedItem);
            //cklst_company.SelectedItems.Clear();
            //cklst_company.SetItemChecked(index, true);
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

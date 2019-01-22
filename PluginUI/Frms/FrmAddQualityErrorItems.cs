using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseDesignPlus;

namespace PluginUI.Frms
{
    public partial class FrmAddQualityErrorItems : Form
    {
        Dictionary<string,string> qualityElementCode;
        public FrmAddQualityErrorItems(string childqualityitem)
        {
            InitializeComponent();

            string errorelementTable = "错漏分类编码";
            string qualityelementTable = "质量元素编码";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            List<string> tableNames = datareadwrite.GetSchameDataTableNames();
            if (tableNames.Contains(errorelementTable) == true)
            {
                string sql_qualitSClass = string.Format("select 错漏分类编码,错漏参考描述 from {0} as a,{1} as b where length(b.质量元素编码)=4 and left(a.错漏分类编码,4) = b.质量元素编码 and b.质量元素='{2}'", errorelementTable, qualityelementTable, childqualityitem);
                DataTable qualitySClass = datareadwrite.GetDataTableBySQL(sql_qualitSClass);

                dataTableToListview(listView1,qualitySClass);
            }

        }

        public Dictionary<string,string> QualityElementCode
        {
            get
            {
                return qualityElementCode;
            }

            set
            {
                qualityElementCode = value;
            }
        }

        private static void dataTableToListview(ListView lv, DataTable dt)
        {
            if (dt == null)
                return;

            lv.Items.Clear();
            lv.Columns.Clear();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                lv.Columns.Add(dt.Columns[i].Caption.ToString());
            }
            foreach (DataRow dr in dt.Rows)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.SubItems[0].Text = dr[0].ToString();

                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    lvi.SubItems.Add(dr[i].ToString());
                }

                lv.Items.Add(lvi);
            }
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        //选择好需要的项
        private void button1_Click(object sender, EventArgs e)
        {
            if (QualityElementCode == null)
                QualityElementCode = new Dictionary<string, string>();
            if(QualityElementCode.Count>0)
            {
                QualityElementCode.Clear();
            }
            foreach(ListViewItem lvi in listView1.CheckedItems)
            {
                QualityElementCode.Add(lvi.SubItems[0].Text,lvi.SubItems[1].Text);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

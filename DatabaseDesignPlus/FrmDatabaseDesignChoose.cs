using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DatabaseDesignPlus;

namespace DatabaseDesignPlus
{
    public partial class FrmDatabaseDesignChoose : Form
    {
        public FrmDatabaseDesignChoose()
        {
            InitializeComponent();
        }

        DataTableDesign sDataTableDesign = new DataTableDesign();
        public DataTableDesign TheDataTableDesign
        {
            get { return sDataTableDesign; }
        }

        string sDataTableDesignVersion = "";
        public string DataTableDesignVersion
        {
            get { return sDataTableDesignVersion; }
        }

        string sDataTableDesignFieldAttributesNameJson = "";
        public string DataTableDesignFieldAttributesNameJson
        {
            get { return sDataTableDesignFieldAttributesNameJson; }
        }

        string sDataTableDesignDbType = "";
        public string DataTableDesignDbType
        {
            get { return sDataTableDesignDbType; }
        }
        string sDataTableDesignDbConnectionID = "";
        public string DataTableDesignDbConnectionID
        {
            get { return sDataTableDesignDbConnectionID; }
        }
        string sDataTableDesignDataTableName = "";
        public string DataTableDesignDataTableName
        {
            get { return sDataTableDesignDataTableName; }
        }

        public FrmPostgresSetting Fps
        {
            get
            {
                return fps;
            }

            set
            {
                fps = value;
            }
        }

        FrmPostgresSetting fps = new FrmPostgresSetting();

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
                return;

            switch (comboBox1.Text)
            {
                case "Excel":
                    {
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "Excel2003|*.xls";
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            sDataTableDesignDbConnectionID = ofd.FileName;
                            textBox1.Text = sDataTableDesignDbConnectionID;
                        }
                    } break;
                case "Mdb":
                    {
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "mdb2003|*.mdb";
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            sDataTableDesignDbConnectionID = ofd.FileName;
                            textBox1.Text = sDataTableDesignDbConnectionID;
                        }
                    } break;
                case "PostgreSQL":
                    {
                        //FrmPostgresSetting fps = new FrmPostgresSetting();
                        if (Fps.ShowDialog() == DialogResult.OK)
                        {
                            sDataTableDesignDbConnectionID = Fps.PGConnectionstring;
                            textBox1.Text = Fps.PGConnectionstring;
                        }
                    }
                    break;
             }
            IDatabaseReaderWriter dbReader = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter(comboBox1.Text, sDataTableDesignDbConnectionID);
            List<string> sTableNames = dbReader.GetSchameDataTableNames();
            List<string> sTableNameFilter = new List<string>();
            sTableNameFilter.Add("表设计");
            DatabaseReaderWriterFactory.FillCombox(sTableNames, comboBox2, sTableNameFilter);
            comboBox2.SelectedIndex = 0;
            List<string> sTableNameFilter2 = new List<string>();
            sTableNameFilter2.Add("表设计版本");
            DatabaseReaderWriterFactory.FillCombox(sTableNames, comboBox3,sTableNameFilter2);
            comboBox3.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            sDataTableDesignDataTableName = comboBox2.Text;
            sDataTableDesignDbType = comboBox1.Text;
            sDataTableDesignVersion = comboBox4.Text;

            sDataTypeRelationDatabaseType = comboBox5.Text;
            sDatatableDataTypeRelationDataTableName = comboBox6.Text;
            
            IDatabaseReaderWriter dbReader = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter(comboBox1.Text, sDataTableDesignDbConnectionID);
            string sSelectJson = string.Format("select {0} from {1} where DataTableDesignVersion = '{2}'", "DataTableDesignFieldAttributesName", dbReader.GetTableName(comboBox3.Text), sDataTableDesignVersion);

            sDataTableDesignFieldAttributesNameJson = dbReader.GetScalar(sSelectJson) as string;

            sDataTableDesign.InitalizeParameters(
                sDataTableDesignDbConnectionID, sDataTableDesignDbType, 
                sDataTableDesignDataTableName, sDataTableDesignVersion,
                sDataTableDesignFieldAttributesNameJson,
                sDataTypeRelationDatatbaseConnectionID,
                sDataTypeRelationDatabaseType,
                sDatatableDataTypeRelationDataTableName
                );

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            IDatabaseReaderWriter dbReader = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter(comboBox1.Text, sDataTableDesignDbConnectionID);
            string sSelectVersion = string.Format("select {0} from {1}", "DataTableDesignVersion", dbReader.GetTableName(comboBox3.Text));
            List<string> sFieldValues = dbReader.GetSingleFieldValueList("DataTableDesignVersion", sSelectVersion);
            DatabaseReaderWriterFactory.FillCombox(sFieldValues, comboBox4);
            comboBox4.SelectedIndex = 0;
        }

        string sDataTypeRelationDatatbaseConnectionID = "";
        string sDataTypeRelationDatabaseType = "";
        string sDatatableDataTypeRelationDataTableName = "";

        private void buttonDataTypeRelation_Click(object sender, EventArgs e)
        {
            if (comboBox5.Text == "")
                return;
            switch (comboBox5.Text)
            {
                case "Excel":
                    {
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "Excel2003|*.xls";
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            sDataTypeRelationDatatbaseConnectionID = ofd.FileName;
                            textBox2.Text = sDataTypeRelationDatatbaseConnectionID;
                        }
                    } break;
                case "Mdb":
                    {
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "mdb2003|*.mdb";
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            sDataTypeRelationDatatbaseConnectionID = ofd.FileName;
                            textBox2.Text = sDataTypeRelationDatatbaseConnectionID;
                        }
                    } break;
                case "PostgreSQL":
                    {
                        //FrmPostgresSetting fps = new FrmPostgresSetting();
                        if (Fps.ShowDialog() == DialogResult.OK)
                        {
                            sDataTypeRelationDatatbaseConnectionID = Fps.PGConnectionstring;
                            textBox2.Text = sDataTypeRelationDatatbaseConnectionID;
                        }
                    }
                    break;
            }
            IDatabaseReaderWriter dbReader = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter(comboBox5.Text, sDataTypeRelationDatatbaseConnectionID);
            List<string> sTableNames = dbReader.GetSchameDataTableNames();
            List<string> sTableNameFilter = new List<string>();
            sTableNameFilter.Add("数据类型关系表");
            DatabaseReaderWriterFactory.FillCombox(sTableNames, comboBox6, sTableNameFilter);
            comboBox6.SelectedIndex = 0;
        }
    }
}

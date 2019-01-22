using DatabaseDesignPlus;
using DevComponents.AdvTree;
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
    public partial class FrmSelectCheckItems : Form
    {
        public FrmSelectCheckItems()
        {
            InitializeComponent();
        }

        QualityItems qitems;

        public QualityItems Qitems
        {
            get
            {
                return qitems;
            }

            set
            {
                qitems = value;
            }
        }
        private string projectid;

        public FrmSelectCheckItems(string sprojectid,string json)
        {
            InitializeComponent();

            projectid = sprojectid;

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);

            string select_firstclass = string.Format("select distinct 项目类型 from ahselecteditems");
            List<string> firstclass = dbread.GetSingleFieldValueList("项目类型", select_firstclass);
            foreach(string fclass in firstclass)
            {
                Node node = new Node();
                node.Text = fclass;
                //node.Image = 
                node.Cells.Add(new Cell("项目类型"));
                //node.Cells.Add(new Cell());
                //node.Cells.Add(new Cell());
                node.Cells.Add(new Cell());
                node.Cells.Add(new Cell());
                node.Cells.Add(new Cell());
                advTree1_selectcheckitems.Nodes.Add(node);
                node.CheckBoxVisible = true;
                // We will load drive content on demand
                node.ExpandVisibility = eNodeExpandVisibility.Visible;

                string select_secondclass = string.Format("select distinct 成果种类 from ahselecteditems where 项目类型='{0}'", node.Text);
                List<string> secondclass = dbread.GetSingleFieldValueList("成果种类", select_secondclass);
                foreach(string sclass in secondclass)
                {
                    Node subnode = new Node();
                    subnode.Text = sclass;
                    //node.Image = 
                    subnode.Cells.Add(new Cell("成果种类"));
                    //subnode.Cells.Add(new Cell());
                    //subnode.Cells.Add(new Cell());
                    subnode.Cells.Add(new Cell());
                    subnode.Cells.Add(new Cell());
                    subnode.Cells.Add(new Cell());
                    node.Nodes.Add(subnode);
                    subnode.CheckBoxVisible = true;
                    // We will load drive content on demand
                    subnode.ExpandVisibility = eNodeExpandVisibility.Visible;

                    string select_qualityitem = string.Format("select distinct   质量元素,质量元素权 from ahselecteditems where 项目类型='{0}' and 成果种类 = '{1}'", node.Text, subnode.Text);
                    DataTable qualityitem = dbread.GetDataTableBySQL(select_qualityitem);
                    foreach (DataRow qitem in qualityitem.Rows)
                    {
                        Node qitemnode = new Node();
                        qitemnode.Text = qitem["质量元素"] as string;
                        //node.Image = 
                        qitemnode.Cells.Add(new Cell("质量元素"));
                        //qitemnode.Cells.Add(new Cell(qitem["质量元素"] as string));
                        //qitemnode.Cells.Add(new Cell());
                        qitemnode.Cells.Add(new Cell());
                        qitemnode.Cells.Add(new Cell(qitem["质量元素权"] as string));
                        qitemnode.Cells.Add(new Cell());
                        subnode.Nodes.Add(qitemnode);
                        qitemnode.CheckBoxVisible = true;
                        // We will load drive content on demand
                        qitemnode.ExpandVisibility = eNodeExpandVisibility.Visible;

                        string select_subqualityitem = string.Format("select distinct   质量元素,质量子元素,检查项,质量元素权,质量子元素权 from ahselecteditems where 项目类型='{0}' and 成果种类 = '{1}' and 质量元素='{2}' order  by 质量子元素", node.Text, subnode.Text, qitemnode.Text);
                        DataTable subqualityitem = dbread.GetDataTableBySQL(select_subqualityitem);
                        foreach (DataRow sqitem in subqualityitem.Rows)
                        {
                            Node sqitemnode = new Node();
                            sqitemnode.Text = sqitem["质量子元素"] as string;
                            //node.Image = 
                            sqitemnode.Cells.Add(new Cell("质量子元素"));
                            //sqitemnode.Cells.Add(new Cell(sqitem["质量元素"] as string));
                            //sqitemnode.Cells.Add(new Cell(sqitem["质量子元素"] as string));
                            sqitemnode.Cells.Add(new Cell(sqitem["检查项"] as string));
                            sqitemnode.Cells.Add(new Cell(sqitem["质量元素权"] as string));
                            sqitemnode.Cells.Add(new Cell(Convert.ToString(sqitem["质量子元素权"])));
                            qitemnode.Nodes.Add(sqitemnode);
                            sqitemnode.CheckBoxVisible = true;
                            // We will load drive content on demand
                            sqitemnode.ExpandVisibility = eNodeExpandVisibility.Visible;
                        }
                    }
                }
            }

            //reset the check state
            if (json != "")
            {
                if (Qitems == null) Qitems = new QualityItems();
                Qitems = QualityItems.FromJson(json);

                foreach (Node fclassnode in advTree1_selectcheckitems.Nodes)
                {
                    foreach (Node sclassnode in fclassnode.Nodes)
                    {
                        //find the secondclass
                        if (Qitems.QualityName == sclassnode.Text)
                        {
                            fclassnode.Expand();
                            sclassnode.Expand();

                            sclassnode.SetChecked(true, eTreeAction.Expand);
                            //find the qualityitem and qualitysubitems;
                            foreach(QualityItem qitem in Qitems.QualityItemList)
                            {
                                foreach (Node qitemnode in sclassnode.Nodes)
                                {
                                    if(qitemnode.Text == qitem.QualityItemName)
                                    {
                                        qitemnode.SetChecked(true, eTreeAction.Expand);
                                        // the subqualityitems
                                        foreach(SubQualityItem sitem in qitem.SubQualitys)
                                        {                                            
                                            foreach (Node qsubitem in qitemnode.Nodes)
                                            {
                                                if (qsubitem.Cells[2].Text == sitem.CheckItem)
                                                    qsubitem.SetChecked(true, eTreeAction.Expand);
                                            }
                                        }

                                    }
                                }
                            }

                        }
                    }
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Qitems == null) Qitems = new QualityItems();

            foreach (Node fclassnode in advTree1_selectcheckitems.Nodes)
            {
                foreach(Node sclassnode in fclassnode.Nodes)
                {
                    if(sclassnode.Checked)
                    {
                        Qitems.QualityName = sclassnode.Text;
                        Qitems.QualityItemList = new List<QualityItem>();
                        
                        //qualityitem node level
                        foreach(Node qitemnode in sclassnode.Nodes)
                        {
                            if(qitemnode.Checked)
                            {
                                QualityItem item = new QualityItem();
                                item.QualityItemName = qitemnode.Text;
                                item.SubQualitys = new List<SubQualityItem>();
                                Qitems.QualityItemList.Add(item);

                                //qualitysubitem node level
                                foreach(Node subqnode in qitemnode.Nodes)
                                {
                                    if(subqnode.Checked)
                                    {
                                        SubQualityItem sqitem = new SubQualityItem();
                                        sqitem.SubQualityItemName = subqnode.Text;
                                        //sqitem.Checkitems = new List<QualityCheckItem>();
                                        sqitem.CheckItem = subqnode.Cells[2].Text;
                                        item.SubQualitys.Add(sqitem);

                                        //checkitem level
                                       /* foreach(Node checkitem in subqnode.Nodes)
                                        {
                                            QualityCheckItem citem = new QualityCheckItem();
                                            citem.CheckItem = checkitem.Text;
                                            sqitem.Checkitems.Add(citem);
                                        }*/
                                    }
                                }
                            }
                        }
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);
            string qulitycheckitemtable = "projectqulitycheckitem";
            //create specifictable
            if (dbread.GetSchameDataTableNames().IndexOf(qulitycheckitemtable) < 0)
            {
                string sql_create = string.Format("create table {0} (projectid text,qulitycheckitem text)", qulitycheckitemtable);
                dbread.ExecuteSQL(sql_create);
            }

            string sql_clear = string.Format("delete from {0} where projectid='{1}'", qulitycheckitemtable, projectid);
            dbread.ExecuteSQL(sql_clear);

            string json = Qitems.ToJson();
            string insertsql = string.Format("insert into {0} values('{1}','{2}')", qulitycheckitemtable, projectid,json.Replace("\"","\\"));
            dbread.ExecuteSQL(insertsql);
        }

        private void advTree1_selectcheckitems_NodeClick(object sender, TreeNodeMouseEventArgs e)
        {
            if(e.Node.Level==1)
            {
                e.Node.SetChecked(!e.Node.Checked, eTreeAction.Expand);
                foreach(Node secodnode in e.Node.Nodes)
                {
                    secodnode.SetChecked(!secodnode.Checked, eTreeAction.Expand);
                    foreach(Node qualityitem in secodnode.Nodes)
                    {
                        qualityitem.SetChecked(!qualityitem.Checked, eTreeAction.Expand);
                    }
                }
            }
        }
    }
}

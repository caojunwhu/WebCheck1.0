using DatabaseDesignPlus;
using DLGCheckLib;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebMapCheck
{
    public partial class SelectCheckItems : System.Web.UI.Page
    {
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
        string _sMapid;
        protected void Page_Load(object sender, EventArgs e)
        {
            projectid = "SYS500DLG20161115";

            _sMapid = HttpUtility.UrlDecode(Request["mapid"]);

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);

            string select_firstclass = string.Format("select distinct 项目类型 from ahselecteditems");
            List<string> firstclass = dbread.GetSingleFieldValueList("项目类型", select_firstclass);

            Node root = new Node() { Text = "质量模型" };
            root.Expanded = true;
            TreePanel1.Root.Add(root);
            foreach (string fclass in firstclass)
            {
                Node node = new Node() { Text = fclass };
                node.Icon = Icon.Folder;
                node.CustomAttributes.Add(new ConfigItem("项目类型", fclass, ParameterMode.Value));
                TreePanel1.Root[0].Children.Add(node);

                string select_secondclass = string.Format("select distinct 成果种类 from ahselecteditems where 项目类型='{0}'", node.Text);
                List<string> secondclass = dbread.GetSingleFieldValueList("成果种类", select_secondclass);
                foreach (string sclass in secondclass)
                {
                    Node subnode = new Node() { Text = sclass };
                    subnode.CustomAttributes.Add(new ConfigItem("项目类型", "成果种类", ParameterMode.Value));
                    subnode.CustomAttributes.Add(new ConfigItem("成果种类", sclass, ParameterMode.Value));
                    subnode.CustomAttributes.Add(new ConfigItem("检查项", "", ParameterMode.Value));
                    subnode.CustomAttributes.Add(new ConfigItem("质量元素权", "", ParameterMode.Value));
                    subnode.CustomAttributes.Add(new ConfigItem("质量子元素权", "", ParameterMode.Value));
                    subnode.CustomAttributes.Add(new ConfigItem("选择", "false", ParameterMode.Value));
                    node.Children.Add(subnode);

                    string select_qualityitem = string.Format("select distinct   质量元素,质量元素权 from ahselecteditems where 项目类型='{0}' and 成果种类 = '{1}'", node.Text, subnode.Text);
                    DataTable qualityitem = dbread.GetDataTableBySQL(select_qualityitem);
                    foreach (DataRow qitem in qualityitem.Rows)
                    {
                        Node qitemnode = new Node() { };
                        qitemnode.Text = qitem["质量元素"] as string;
                        string qitemweight =   qitem["质量元素权"] as string;
                        qitemnode.CustomAttributes.Add(new ConfigItem("项目类型", "质量元素", ParameterMode.Value));
                        qitemnode.CustomAttributes.Add(new ConfigItem("成果种类", qitemnode.Text, ParameterMode.Value));
                        qitemnode.CustomAttributes.Add(new ConfigItem("检查项", "", ParameterMode.Value));
                        qitemnode.CustomAttributes.Add(new ConfigItem("质量元素权", qitemweight, ParameterMode.Value));
                        qitemnode.CustomAttributes.Add(new ConfigItem("质量子元素权", "", ParameterMode.Value));
                        qitemnode.CustomAttributes.Add(new ConfigItem("选择", "false", ParameterMode.Value));
                        subnode.Children.Add(qitemnode);

                        string select_subqualityitem = string.Format("select distinct   质量元素,质量子元素,检查项,质量元素权,质量子元素权 from ahselecteditems where 项目类型='{0}' and 成果种类 = '{1}' and 质量元素='{2}' order  by 质量子元素", node.Text, subnode.Text, qitemnode.Text);
                        DataTable subqualityitem = dbread.GetDataTableBySQL(select_subqualityitem);
                        foreach (DataRow sqitem in subqualityitem.Rows)
                        {

                            Node sqitemnode = new Node();

                            sqitemnode.Text = sqitem["检查项"] as string;

                            sqitemnode.CustomAttributes.Add(new ConfigItem("项目类型", "质量子元素及检查项", ParameterMode.Value));

                            string text1 = sqitem["质量子元素"] as string;
                            sqitemnode.CustomAttributes.Add(new ConfigItem("成果种类", text1, ParameterMode.Value));

                            string checkItem = sqitem["检查项"] as string;
                            sqitemnode.CustomAttributes.Add(new ConfigItem("检查项", checkItem, ParameterMode.Value));

                            string qitemweight2  = Convert.ToString(sqitem["质量元素权"]);
                            sqitemnode.CustomAttributes.Add(new ConfigItem("质量元素权", qitemweight2 , ParameterMode.Value));

                            string sqitemweight = Convert.ToString(sqitem["质量子元素权"]);
                            sqitemnode.CustomAttributes.Add(new ConfigItem("质量子元素权", sqitemweight , ParameterMode.Value));

                            sqitemnode.CustomAttributes.Add(new ConfigItem("选择", "false", ParameterMode.Value));

                            sqitemnode.Leaf = true;
                            qitemnode.Children.Add(sqitemnode);

                        }
                    }
                }
                }

              InitTreeParams(_sMapid);
            }

        protected void InitTreeParams(string mapid)
        {
            string sql_select = string.Format("select qualitymodel from webchecksamples where mapid='{0}'", mapid);

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);

            string qualitymodel = dbread.GetScalar(sql_select) as string;
            if (Qitems == null) Qitems = new QualityItems();

            if (qualitymodel == null) return;


            Qitems = QualityItems.FromJson(qualitymodel.Replace('\\','"'));

            foreach (Node fclassnode in TreePanel1.Root)
            {
                foreach (Node sclassnode in fclassnode.Children)
                {
                    foreach (Node tclassnode in sclassnode.Children)
                    {
                        //find the secondclass
                        if (Qitems.QualityName == tclassnode.Text)
                        {
                            fclassnode.Expanded=true;
                            sclassnode.Expanded = true;
                            tclassnode.Expanded = true;
                            
                            //tclassnode.Checked = true;
                            tclassnode.CustomAttributes[5].Value = "true";
                            
                            //find the qualityitem and qualitysubitems;
                            foreach (QualityItem qitem in Qitems.QualityItemList)
                            {
                                foreach (Node qitemnode in tclassnode.Children)
                                {
                                    if (qitemnode.Text == qitem.QualityItemName)
                                    {
                                        //qitemnode.Checked=true;
                                        qitemnode.CustomAttributes[5].Value = "true";
                                        // the subqualityitems
                                        foreach (SubQualityItem sitem in qitem.SubQualitys)
                                        {
                                            foreach (Node qsubitem in qitemnode.Children)
                                            {
                                                if (qsubitem.CustomAttributes[2].Value == sitem.CheckItem)
                                                {
                                                    //qsubitem.Checked = true;
                                                    qsubitem.CustomAttributes[5].Value = "true";
                                                }                                               
                                            }
                                        }

                                    }
                                }
                            }
                            /*                        sclassnode.SetChecked(true, eTreeAction.Expand);
                                                    //find the qualityitem and qualitysubitems;
                                                    foreach (QualityItem qitem in Qitems.QualityItemList)
                                                    {
                                                        foreach (Node qitemnode in sclassnode.Nodes)
                                                        {
                                                            if (qitemnode.Text == qitem.QualityItemName)
                                                            {
                                                                qitemnode.SetChecked(true, eTreeAction.Expand);
                                                                // the subqualityitems
                                                                foreach (SubQualityItem sitem in qitem.SubQualitys)
                                                                {
                                                                    foreach (Node qsubitem in qitemnode.Nodes)
                                                                    {
                                                                        if (qsubitem.Cells[2].Text == sitem.CheckItem)
                                                                            qsubitem.SetChecked(true, eTreeAction.Expand);
                                                                    }
                                                                }

                                                            }
                                                        }
                                                    }*/

                        }
                    }

                }
            }

        }

    }
    
}
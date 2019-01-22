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
using DLGCheckLib;

namespace PluginUI.Frms
{
    public partial class FrmAddSampleErrorPlus : Form
    {
        DataTable _QualityErrorTable;
        DataRow oldQualityError;
        public FrmAddSampleErrorPlus()
        {
            InitializeComponent();
        }

        string _sMapnumber;
        Authentication.Class.UserObject _loginuser;
        string _sprojectid;
        string _producttype;
        QualityItems _qitem;
        PinErrorItem _pinerror;

        public PinErrorItem Pinerror
        {
            get
            {
                return _pinerror;
            }

            set
            {
                _pinerror = value;
            }
        }

        public FrmAddSampleErrorPlus(string sMapnumber,Authentication.Class.UserObject loginuser,string projectid,string producttype, PinErrorItem pinerror = null)
        {
            InitializeComponent();
            _sMapnumber = sMapnumber;
            _loginuser = loginuser;
            _sprojectid = projectid;
            _producttype = producttype;
            _pinerror = pinerror;

            _qitem = QualityItems.FromJson(_producttype);

            tb_mapnumber.Text = _sMapnumber;

            foreach(QualityItem qi in _qitem.QualityItemList)
            {
                cmb_qualityitem.Items.Add(qi.QualityItemName);

                foreach(SubQualityItem si in qi.SubQualitys)
                {
                    if(cmb_subqualityitem.Items.IndexOf(si.SubQualityItemName)<0)
                    cmb_subqualityitem.Items.Add(si.SubQualityItemName);

                    cmb_checkitem.Items.Add(si.CheckItem);
                }
            }

            if(Pinerror!=null)
            {
                int k = cmb_qualityitem.Items.IndexOf(Pinerror.QualityItem);
                cmb_qualityitem.SelectedIndex = k;

                int j = cmb_subqualityitem.Items.IndexOf(Pinerror.SubQualityItem);
                cmb_subqualityitem.SelectedIndex = j;

                int i = cmb_checkitem.Items.IndexOf(Pinerror.CheckItem);
                cmb_checkitem.SelectedIndex = i;

                if (pinerror.Comment != "")
                    rtb_errorofsample.Text = Pinerror.Error + ":" + pinerror.Comment;
                else
                    rtb_errorofsample.Text = Pinerror.Error;

                int l = cmb_errorclass.Items.IndexOf(Pinerror.ErrorType);
                cmb_errorclass.SelectedIndex = l;

                int f = cmb_preerror.Items.IndexOf(Pinerror.Error);
                cmb_preerror.SelectedIndex = f;

                xiugairichTextBox3.Text = Pinerror.Modify;
                fucharichTextBox1.Text = Pinerror.Review;
                chulirichTextBox1.Text = Pinerror.Feedback;

            }


        }
        //质量元素切换时导致质量子元素跟着切换
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_subqualityitem.Items.Clear();
            cmb_checkitem.Items.Clear();
            string qualityname = cmb_qualityitem.Text;
            cmb_subqualityitem.Text = "";
            cmb_checkitem.Text = "";
            foreach (QualityItem qi in _qitem.QualityItemList)
            {
                if(qualityname==qi.QualityItemName)
                {
                    //cmb_qualityitem.Items.Add(qi.QualityItemName);

                    foreach (SubQualityItem si in qi.SubQualitys)
                    {
                        if (cmb_subqualityitem.Items.IndexOf(si.SubQualityItemName) < 0)
                            cmb_subqualityitem.Items.Add(si.SubQualityItemName);

                        cmb_checkitem.Items.Add(si.CheckItem);
                    }
                }

            }
        }
        //质量子元素切换时，查询对应
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_checkitem.Items.Clear();
            string qualityname = cmb_qualityitem.Text;
            string subqualityname = cmb_subqualityitem.Text;
            cmb_checkitem.Text = "";
            foreach (QualityItem qi in _qitem.QualityItemList)
            {
                if (qualityname == qi.QualityItemName)
                {
                   // cmb_qualityitem.Items.Add(qi.QualityItemName);

                    foreach (SubQualityItem si in qi.SubQualitys)
                    {
                        if(subqualityname==si.SubQualityItemName)
                        {
                            if (cmb_subqualityitem.Items.IndexOf(si.SubQualityItemName) < 0)
                                cmb_subqualityitem.Items.Add(si.SubQualityItemName);

                            cmb_checkitem.Items.Add(si.CheckItem);
                        }
                    }
                }
            }

            cmb_errorclass.Text = "";
            cmb_preerror.Text = "";
            cmb_preerror.Items.Clear();
            rtb_errorofsample.Text = "";
        }
        //错漏类型切换时，根据质量子元素编码，错漏类别，查询错漏描述列表，进行填充更新
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string producttype = _qitem.QualityName;
            string subqualityname = cmb_subqualityitem.Text;
            string errorclass = cmb_errorclass.Text;

            string sql_preerror = string.Format("select 错漏内容 from ah错漏分类表 where 成果种类='{0}' and  质量子元素='{1}' and  错漏类型='{2}'", producttype, subqualityname, errorclass);
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);
            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);

            List<string> preerror = datareadwrite.GetSingleFieldValueList("错漏内容", sql_preerror);
            DatabaseReaderWriterFactory.FillCombox(preerror, cmb_preerror);

        }
        //错漏参考描述列表切换时，对错漏描述框进行切换
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            rtb_errorofsample.Text = cmb_preerror.Text;
        }




        private void FrmAddSampleError_Load(object sender, EventArgs e)
        {

        }

        //打开多选项窗口
        private void button1_Click(object sender, EventArgs e)
        {
            FrmAddQualityErrorItems frmqitem = new FrmAddQualityErrorItems(cmb_subqualityitem.Text);
            if (frmqitem.ShowDialog() == DialogResult.OK)
            {
                if (frmqitem.QualityElementCode.Count > 0)
                {
                    string qualityFClass = cmb_qualityitem.Text;
                    string qualitySClass = cmb_subqualityitem.Text;
                    string qualityErrorClass = cmb_errorclass.Text;
                    string qualityError = rtb_errorofsample.Text;
                    string fucha = fucharichTextBox1.Text;
                    string chuli = chulirichTextBox1.Text;
                    string xiugai = xiugairichTextBox3.Text;

                    foreach (KeyValuePair<string, string> code in frmqitem.QualityElementCode)
                    {
                        string errorclass = code.Key.Substring(4, 1);
                        string error = code.Value;

                    }
                    //dataGridViewX1.Refresh();
                }
            }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            //新增一条检查记录，传递到标注窗口
            Pinerror = new PinErrorItem();
            Pinerror.Projectid = _sprojectid;
            Pinerror.Mapnumber = _sMapnumber;
            Pinerror.Error = rtb_errorofsample.Text;
            Pinerror.QualityItem = cmb_qualityitem.Text;
            Pinerror.SubQualityItem = cmb_subqualityitem.Text;
            Pinerror.CheckItem = cmb_checkitem.Text;
            Pinerror.ErrorType = cmb_errorclass.Text;
            Pinerror.Checker = _loginuser.username;
            Pinerror.CheckTime = DateTime.Now.ToString();
            Pinerror.Feedback = chulirichTextBox1.Text;
            Pinerror.Modify = xiugairichTextBox3.Text;
            Pinerror.Review = fucharichTextBox1.Text;
            Pinerror.Comment = "";
            Pinerror.Shape = "";

            if(Pinerror.Error==""|| Pinerror.QualityItem==""|| Pinerror.SubQualityItem==""||
               Pinerror.ErrorType==""|| Pinerror.Mapnumber==""|| Pinerror.CheckItem=="")
            {
                MessageBox.Show("缺乏必要的标注信息，请补充！");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void FrmAddSampleErrorPlus_FormClosed(object sender, FormClosedEventArgs e)
        {
           if(this.Pinerror.Error != "")
            {
                this.DialogResult = DialogResult.OK;
            }
           else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}

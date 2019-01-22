using DatabaseDesignPlus;
using DLGCheckLib;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginUI.Frms
{
    public partial class FrmCheckPGDBLayersAtributes : Form
    {
        private IFeatureLayer pScaterFeatureLayer;
        private IFeatureLayer pMapBindingFeatureLayer;
        private IMap localMap;
        private AxMapControl localmapControl;
        private NewLineFeedback pLineFeedback;
        private IPoint snapVetex;
        private IPoint startVetex;
        private string Startlayer;
        private bool bPickState = false;
        private bool bEditState = false;
        private double snapBufferlength = 0.1;
        private int MapScalar = 100;
        private esriControlsMousePointer oldCursor = esriControlsMousePointer.esriPointerDefault;
        private List<BufferSearchTargetItem> TempBufferSearchResult;

        public RelativeLineCollection localRelativeChecklines { set; get; }
        public DLGCheckProjectClass localCheckProject { set; get; }
        public SearchTargetSetting localSearchTargetSetting { set; get; }
        DataTable dtvalue;

        public string SMapnumber
        {
            
            get
            {
                _sMapnumber = localCheckProject.CurrentMapnumber;
                return _sMapnumber;
            }

            set
            {
                _sMapnumber = value;
            }
        }

        int sampleareaidex_inpara = -1;
        int sampleserial_inpara = -1;
        string szMapnumber_inpara = "";

        DatabaseDesignPlus.DataTableDesign pgdblayerattrdesign;
        DataTable _QualityErrorTable;
        string SampleerrorCollectionTableName = "检查意见记录表";

        string _sMapnumber;
        string _sprojectid;

        string DataTableDesignFieldAttributesName = "{'FieldIndex':'层号','FieldName':'属性项','FieldCode':'属性项','FieldClass':'要素分层','FieldInput':'数据体几何','FieldSource':'值域','FieldType':'字段类型','FieldLength':'长度','FieldPrecision':'小数位数','FieldValue':'填写示例','FieldIsNull':'是否允许为空','FieldRemarks':'描述','FieldImportType':'重要性','FieldImportIDCode':'导入识别码'}";
        DataTable Dataattributedefine = null;

        List<FieldDesign> FeatureClassFieldDesign = null;

        public FrmCheckPGDBLayersAtributes(DLGCheckProjectClass GlobeCheckProject, SearchTargetSetting GlobeSearchtargetSetting, AxMapControl mapControl, int sampleareaidex = -1, int sampleserial = -1, string szMapnumber = "")
        {
            InitializeComponent();

            _QualityErrorTable = new DataTable();
            _QualityErrorTable.Columns.Add("质量元素");
            _QualityErrorTable.Columns.Add("质量子元素");
            _QualityErrorTable.Columns.Add("错漏类别");
            _QualityErrorTable.Columns.Add("错漏描述");
            _QualityErrorTable.Columns.Add("处理意见");
            _QualityErrorTable.Columns.Add("复查情况");
            _QualityErrorTable.Columns.Add("修改情况");
            _QualityErrorTable.Columns.Add("检查者");
            _QualityErrorTable.Columns.Add("检查日期");

            SMapnumber = GlobeCheckProject.CurrentMapnumber;
            _sprojectid = GlobeCheckProject.ProjectID;
            localMap = mapControl.Map;
            localCheckProject = GlobeCheckProject;
            //读取数据分层定义表


            DatabaseDesignPlus.IDatabaseReaderWriter datareader =
             DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", CallClass.Databases["SurveryProductCheckDatabase"]);
            string sql = string.Format("select * from {0} where projectid ='{1}'",datareader.GetTableName("基础地理信息数据分层属性定义表"), localCheckProject.ProjectID);
            Dataattributedefine = datareader.GetDataTableBySQL(sql);

            string sql2 = string.Format("select * from {0} where projectid ='{1}'", datareader.GetTableName("基础地理信息数据分层属性枚举值表"), localCheckProject.ProjectID);
            dtvalue = datareader.GetDataTableBySQL(sql2);

            // superGridControl1.PrimaryGrid.DataSource = Dataattributedefine;

            string dbconnection = CallClass.Databases["SurveryProductCheckDatabase"];
            string datatablename = "基础地理信息数据分层属性定义表设计";
            string createdatatablename = "基础地理信息数据分层属性定义表";
            string datarelationname = "";
            if (dbconnection == "" || datatablename == "")
                return;

            pgdblayerattrdesign =
                new DatabaseDesignPlus.DataTableDesign(dbconnection, "PostgreSQL", createdatatablename, "1.2", DataTableDesignFieldAttributesName,
                dbconnection, "PostgreSQL", datarelationname);

            pgdblayerattrdesign.InitializeDataTableDesign("PostgreSQL", createdatatablename, string.Format("projectid ='{0}'",localCheckProject.ProjectID));
            //pgdblayerattrdesign.InitializeDataTableDesign("PostgreSQL", createdatatablename, "");

            superGridControl1.PrimaryGrid.DataSource = pgdblayerattrdesign.FieldDesigns;


        }

        private void SelectDefineFile_Click(object sender, EventArgs e)
        {
            //获取当前路径和文件名
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "数据分层属性定义文件(*.xls)|*.xls|All Files(*.*)|*.*";
            dlg.Title = "Open LayerAttributeDefinefile Data file";
            if(dlg.ShowDialog()==DialogResult.OK)
            {
                string strFullPath = dlg.FileName;
                textBox1.Text = strFullPath;
                DatabaseDesignPlus.IDatabaseReaderWriter datareader =
                      DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("Excel", strFullPath);

                List<string> tablename = datareader.GetSchameDataTableNames();
                DatabaseDesignPlus.DatabaseReaderWriterFactory.FillCombox(tablename, comboBox1);
                DatabaseDesignPlus.DatabaseReaderWriterFactory.FillCombox(tablename, comboBox2);


            }
        }
        private void LoadDesign()
        {

            string dbconnection = textBox1.Text;
            string datatablename = comboBox1.Text;
            string datarelationname = comboBox2.Text;
            if (dbconnection == "" ||  datatablename == "")
                return;


            pgdblayerattrdesign =
                new DatabaseDesignPlus.DataTableDesign(dbconnection, "Excel", datatablename,"1.2",DataTableDesignFieldAttributesName,
                dbconnection, "PGDB", datarelationname  );

            pgdblayerattrdesign.InitializeDataTableDesign("PGDB", datatablename);

            superGridControl1.PrimaryGrid.DataSource = pgdblayerattrdesign.FieldDesigns;

        }
        private void CheckFieldValue()
        {
            //对需要进行值域检查的属性项建立搜索条件
            if (pgdblayerattrdesign == null || dtvalue==null)
            {
                MessageBox.Show("请先设置图层属性定义表和枚举值表");
                return;
            }; 

            string qualityFClass = "属性精度";
            string qualitySClass = "属性正确性";
            string qualityErrorClass = "";
            string qualityError = "";
            string fucha = "";
            string chuli = "";
            string xiugai = "";

            //首先选出FieldClass/FieldName/FieldSource列表，其中FieldSource中含有对值域表的比较定义
            List<FieldDesign> fieldtobeCaompared = pgdblayerattrdesign.FieldDesigns.FindAll(ao => ao.FieldSource.Length > 0).ToList();
            foreach (FieldDesign fd in fieldtobeCaompared)
            {
                string EnumValue = "";
                string FieldName = "";
                string FieldClass = "";
                string[] s1 = fd.FieldSource.Split('=');
                EnumValue = s1[0];
                string[] s2 = s1[1].Split(',');
                FieldClass = s2[0].Remove(0, 1);
                FieldName = s2[1].Remove(s2[1].Length - 1, 1);

                //DataRow[] eumvlist =  dtvalue.Select(string.Format("{0}='{1}' and SubString({3},2,3)='{4}'", FieldName, fd.FieldName,FieldClass ,fd.FieldClass));
                var query = from r in dtvalue.AsEnumerable()
                            where r.Field<string>(FieldClass)!=null &&
                            r.Field<string>(FieldClass).IndexOf(fd.FieldClass) >= 0 &&
                            r.Field<string>(FieldName) == fd.FieldName
                            select r;
                List<string> enumvlist = new List<string>();
                foreach (var r in query)
                {
                    string v = Convert.ToString(r[EnumValue]);
                    if (enumvlist.IndexOf(v) < 0)
                        enumvlist.Add(v);
                }

                //打开该图层，该字段，统计其值，
                List<string> maplayers = new List<string>();
                for (int i = 0; i < localMap.LayerCount; i++)
                {
                    maplayers.Add(localMap.Layer[i].Name);
                }
                int index = maplayers.IndexOf(fd.FieldClass);
                //该数据集没有该图层，则跳过
                if (index < 0) continue;
                IFeatureLayer ifeatlayer = localMap.Layer[index] as IFeatureLayer;
                string layername = ifeatlayer.Name;
                //取出该字段的值，建立valuelist
                int nfeatcount = ifeatlayer.FeatureClass.FeatureCount(null);

                List<string> fieldsLayer = new List<string>();
                for (int i = 0; i < ifeatlayer.FeatureClass.Fields.FieldCount; i++)
                {
                    IField field = ifeatlayer.FeatureClass.Fields.Field[i];
                    fieldsLayer.Add(field.Name);
                }

                //遍历feature
                IFeatureCursor pCursor = ifeatlayer.FeatureClass.Search(null, true);
                List <string> valuelist = new List<string>();
                //for (int i = 1; i <= nfeatcount; i++)
                IFeature pFeature = pCursor.NextFeature();
                int index2 = fieldsLayer.IndexOf(fd.FieldName);
                if (index2 < 0)
                    continue;

                while (pFeature != null)
                {
                    string value =Convert.ToString(pFeature.Value[index2] );
                    if (valuelist.IndexOf(value) < 0)
                        valuelist.Add(value);

                    pFeature = pCursor.NextFeature();
                }

                //开始检查
                //1检查valuelist中值是否在enumlist中
                //超出范围的进行记录
                foreach (string v in valuelist)
                {
                    if (enumvlist.IndexOf(v) < 0)
                    {
                        //记录该值为非法值
                        qualityErrorClass = "不符合";
                        qualityError = string.Format("图层{0}的属性项{1}的值{2}不在参考值范围内；", fd.FieldClass, fd.FieldName, v);
                        //记录缺少此图层
                        DataRow dr = _QualityErrorTable.NewRow();
                        dr["质量元素"] = qualityFClass;
                        dr["质量子元素"] = qualitySClass;
                        dr["错漏类别"] = qualityErrorClass;
                        dr["错漏描述"] = qualityError;
                        dr["处理意见"] = chuli;
                        dr["复查情况"] = fucha;
                        dr["修改情况"] = xiugai;

                        dr["检查者"] = localCheckProject.currentuser;
                        dr["检查日期"] = DateTime.Now.ToString();
                        _QualityErrorTable.Rows.Add(dr);

                    }
                }

            }

        }
        //检查不可为空值的项是否全为空值
        private void CheckNullValue()
        {
            //对需要进行值域检查的属性项建立搜索条件
            if (pgdblayerattrdesign == null)
            {
                MessageBox.Show("请先设置图层属性定义表");
                return;
            };

            string qualityFClass = "完整性";
            string qualitySClass = "遗漏";
            string qualityErrorClass = "";
            string qualityError = "";
            string fucha = "";
            string chuli = "";
            string xiugai = "";

            //打开该图层，该字段，统计其值，
            List<string> maplayers = new List<string>();
            for (int i = 0; i < localMap.LayerCount; i++)
            {
                maplayers.Add(localMap.Layer[i].Name);
            }

            //首先选出FieldImportIDCode不为空的项
            List<FieldDesign> fieldtobeCaompared = pgdblayerattrdesign.FieldDesigns.FindAll(ao => ao.FieldImportType.Length> 0).ToList();
            foreach (FieldDesign fd in fieldtobeCaompared)
            {

                int index = maplayers.IndexOf(fd.FieldClass);
                //该数据集没有该图层，则跳过
                if (index < 0) continue;
                IFeatureLayer ifeatlayer = localMap.Layer[index] as IFeatureLayer;
                string layername = ifeatlayer.Name;
                //取出该字段的值，建立valuelist
                int nfeatcount = ifeatlayer.FeatureClass.FeatureCount(null);

                List<string> fieldsLayer = new List<string>();
                for (int i = 0; i < ifeatlayer.FeatureClass.Fields.FieldCount; i++)
                {
                    IField field = ifeatlayer.FeatureClass.Fields.Field[i];
                    fieldsLayer.Add(field.Name);
                }

                //遍历feature
                IFeatureCursor pCursor = ifeatlayer.FeatureClass.Search(null, true);
                List<string> valuelist = new List<string>();
                //for (int i = 1; i <= nfeatcount; i++)
                IFeature pFeature = pCursor.NextFeature();
                int index2 = fieldsLayer.IndexOf(fd.FieldName);
                bool isAllNull = true;
                while (pFeature != null)
                {
                    string value = Convert.ToString(pFeature.Value[index2]);
                    if (value != "")
                        isAllNull = false;
                    pFeature = pCursor.NextFeature();
                }
                if(isAllNull==true)
                {
                    //记录该值为非法值
                    qualityErrorClass = "不符合";
                    qualityError = string.Format("图层{0}的属性项{1}的值全部为空，该属性标记为{2}；", fd.FieldClass, fd.FieldName,fd.FieldImportType);
                    //记录缺少此图层
                    DataRow dr = _QualityErrorTable.NewRow();
                    dr["质量元素"] = qualityFClass;
                    dr["质量子元素"] = qualitySClass;
                    dr["错漏类别"] = qualityErrorClass;
                    dr["错漏描述"] = qualityError;
                    dr["处理意见"] = chuli;
                    dr["复查情况"] = fucha;
                    dr["修改情况"] = xiugai;

                    dr["检查者"] = localCheckProject.currentuser;
                    dr["检查日期"] = DateTime.Now.ToString();
                    _QualityErrorTable.Rows.Add(dr);
                }

            }
         }
        //读取值域
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strFullPath = textBox1.Text;
            if (strFullPath == "" || comboBox2.Text=="")
                            return;

            DatabaseDesignPlus.IDatabaseReaderWriter datareader =
                  DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("Excel", strFullPath);

            dtvalue = datareader.GetDataTable(comboBox2.Text);

        }
        //分层属性表选择好以后，即可已进行分层定义设置读取
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDesign();

        }

        //本项以逻辑一致性概念一致性为主要检查对象，重点检查数据集（层）定义、属性定义是否符合要求。
        //以自动化检查为主，检查结果可以到检查意见记录表打印功能中查看和打印，但不能编辑。
        private void AutoCheckbutton1_Click(object sender, EventArgs e)
        {
            string qualityFClass = "逻辑一致性";
            string qualitySClass = "概念一致性";
            string qualityErrorClass = "";
            string qualityError = "";
            string fucha = "";
            string chuli = "";
            string xiugai = "";

            if (localMap.LayerCount == 0)
            {
                MessageBox.Show("请先添加样本图幅!");
                return;
            }
            if(Dataattributedefine==null&& pgdblayerattrdesign == null)
            {
                MessageBox.Show("请先设置图层属性定义表");
                return;
            }


            if (_QualityErrorTable.Rows.Count > 0)
                _QualityErrorTable.Clear();

            //标准图层名列表
            List<string> layersdefine = new List<string>();
            foreach(DatabaseDesignPlus.FieldDesign fd in pgdblayerattrdesign.FieldDesigns)
            {
                if(layersdefine.IndexOf(fd.FieldClass)<0)
                layersdefine.Add(fd.FieldClass);
            }

            List<string> maplayers = new List<string>();
            for(int i=0;i<localMap.LayerCount;i++)
            {
                maplayers.Add(localMap.Layer[i].Name);
            }

            //1 检查图层列表一致性
            //1)检查图层是否有缺，
            foreach(string name in  layersdefine)
            {
                if(maplayers.IndexOf(name)<0)
                {
                    qualityErrorClass = "不符合";
                    qualityError = string.Format("缺少图层：{0}；", name);
                    //记录缺少此图层
                    DataRow dr = _QualityErrorTable.NewRow();
                    dr["质量元素"] = qualityFClass;
                    dr["质量子元素"] = qualitySClass;
                    dr["错漏类别"] = qualityErrorClass;
                    dr["错漏描述"] = qualityError;
                    dr["处理意见"] = chuli;
                    dr["复查情况"] = fucha;
                    dr["修改情况"] = xiugai;

                    dr["检查者"] = localCheckProject.currentuser;
                    dr["检查日期"] = DateTime.Now.ToString();
                    _QualityErrorTable.Rows.Add(dr);

                }
            }
            List<string> macthedLayername = new List<string>();
            //2）检查图层是否多余
            foreach(string name in maplayers)
            {
                if(layersdefine.IndexOf(name)<0)
                {
                    //记录是多余图层
                    qualityErrorClass = "不符合";
                    qualityError = string.Format("多余图层：{0}；", name);
                    //记录缺少此图层
                    DataRow dr = _QualityErrorTable.NewRow();
                    dr["质量元素"] = qualityFClass;
                    dr["质量子元素"] = qualitySClass;
                    dr["错漏类别"] = qualityErrorClass;
                    dr["错漏描述"] = qualityError;
                    dr["处理意见"] = chuli;
                    dr["复查情况"] = fucha;
                    dr["修改情况"] = xiugai;

                    dr["检查者"] = localCheckProject.currentuser;
                    dr["检查日期"] = DateTime.Now.ToString();
                    _QualityErrorTable.Rows.Add(dr);

                }
                else
                {
                    if(macthedLayername.IndexOf(name)<0)
                         macthedLayername.Add(name);
                }
            }

            //2 检查图层类型是否一致
            foreach(string name in macthedLayername)
            {
                int index = maplayers.IndexOf(name);
                IFeatureLayer featlayer = localMap.Layer[index] as IFeatureLayer;

               // if(featlayer.FeatureClass. =ESRI.ArcGIS.Geodatabase.esriFeatureType. 
            }

            //3 检查图层字段一致性
           
            foreach (string name in macthedLayername)
            {
                int index = maplayers.IndexOf(name);
                IFeatureLayer featlayer = localMap.Layer[index] as IFeatureLayer;

                List<FieldDesign> layerdesign = pgdblayerattrdesign.FieldDesigns.FindAll(ao => ao.FieldClass == name).ToList();
                List<string> fieldsdefine = new List<string>();
                foreach(FieldDesign fd in layerdesign)
                {
                    if (fieldsdefine.IndexOf(fd.FieldName) < 0)
                        fieldsdefine.Add(fd.FieldName);
                }
                List < string > fieldsLayer = new List<string>();
                for(int i=0;i< featlayer.FeatureClass.Fields.FieldCount;i++)
                {
                    IField field= featlayer.FeatureClass.Fields.Field[i];
                    fieldsLayer.Add(field.Name);
                } 
                //1)检查字段是否有缺
                foreach(string fdefine in fieldsdefine)
                {
                    if(fieldsLayer.IndexOf(fdefine)<0)
                    {
                        qualityErrorClass = "不符合";
                        qualityError = string.Format("图层{0}缺字段：{1}；", name,fdefine);
                        //记录缺少此图层
                        DataRow dr = _QualityErrorTable.NewRow();
                        dr["质量元素"] = qualityFClass;
                        dr["质量子元素"] = qualitySClass;
                        dr["错漏类别"] = qualityErrorClass;
                        dr["错漏描述"] = qualityError;
                        dr["处理意见"] = chuli;
                        dr["复查情况"] = fucha;
                        dr["修改情况"] = xiugai;

                        dr["检查者"] = localCheckProject.currentuser;
                        dr["检查日期"] = DateTime.Now.ToString();
                        _QualityErrorTable.Rows.Add(dr);
                    }
                }
                List<string> fieldsMatch = new List<string>();
                //2）检查字段是否多余
                foreach(string layerfield in fieldsLayer)
                {
                    if (layerfield.ToUpper() == "OBJECTID"||layerfield.ToUpper()=="SHAPE"||
                        layerfield.ToUpper()=="SHAPE_AREA"||layerfield.ToUpper()=="SHAPE_LENGTH")
                        continue;

                    if(fieldsdefine.IndexOf(layerfield)<0)
                    {
                        qualityErrorClass = "不符合";
                        qualityError = string.Format("图层{0}多余字段：{1}；", name, layerfield);
                        //记录缺少此图层
                        DataRow dr = _QualityErrorTable.NewRow();
                        dr["质量元素"] = qualityFClass;
                        dr["质量子元素"] = qualitySClass;
                        dr["错漏类别"] = qualityErrorClass;
                        dr["错漏描述"] = qualityError;
                        dr["处理意见"] = chuli;
                        dr["复查情况"] = fucha;
                        dr["修改情况"] = xiugai;

                        dr["检查者"] = localCheckProject.currentuser;
                        dr["检查日期"] = DateTime.Now.ToString();
                        _QualityErrorTable.Rows.Add(dr);
                    }else
                    {
                        if(fieldsMatch.IndexOf(layerfield)<0)
                        {
                            fieldsMatch.Add(layerfield);
                        }
                    }
                }
                //3)检查字段定义是否一致
                foreach(string mfield in fieldsMatch)
                {
                    int indexfieldlayer = fieldsLayer.IndexOf(mfield);
                    IField field = featlayer.FeatureClass.Fields.Field[indexfieldlayer];
                    int indexfieldsdefine = fieldsdefine.IndexOf(mfield);
                    FieldDesign fielddesign = layerdesign.Find(ao=>ao.FieldName==mfield);

                    //是否允许为空值检查
                    bool isNullable = fielddesign.FieldIsNull == "Yes"?true:false;
                    if (field.IsNullable!= isNullable)
                    {
                        qualityErrorClass = "不符合";
                        qualityError = string.Format("图层{0}的字段{1}是否允许为空值不一致：", name, mfield);
                        //记录缺少此图层
                        DataRow dr = _QualityErrorTable.NewRow();
                        dr["质量元素"] = qualityFClass;
                        dr["质量子元素"] = qualitySClass;
                        dr["错漏类别"] = qualityErrorClass;
                        dr["错漏描述"] = qualityError;
                        dr["处理意见"] = chuli;
                        dr["复查情况"] = fucha;
                        dr["修改情况"] = xiugai;

                        dr["检查者"] = localCheckProject.currentuser;
                        dr["检查日期"] = DateTime.Now.ToString();
                        _QualityErrorTable.Rows.Add(dr);
                    }

                    if(field.Type== esriFieldType.esriFieldTypeInteger )
                    {
                        //类型定义不一致检查
                        if(fielddesign.FieldType != "Long Integer")
                        {
                            qualityErrorClass = "不符合";
                            qualityError = string.Format("图层{0}的字段{1}数据类型不一致：", name, mfield);
                            //记录缺少此图层
                            DataRow dr = _QualityErrorTable.NewRow();
                            dr["质量元素"] = qualityFClass;
                            dr["质量子元素"] = qualitySClass;
                            dr["错漏类别"] = qualityErrorClass;
                            dr["错漏描述"] = qualityError;
                            dr["处理意见"] = chuli;
                            dr["复查情况"] = fucha;
                            dr["修改情况"] = xiugai;

                            dr["检查者"] = localCheckProject.currentuser;
                            dr["检查日期"] = DateTime.Now.ToString();
                            _QualityErrorTable.Rows.Add(dr);

                        }

                    }
                    if(field.Type == esriFieldType.esriFieldTypeDouble)
                    {
                        if(fielddesign.FieldType!="Double")
                        {
                            qualityErrorClass = "不符合";
                            qualityError = string.Format("图层{0}的字段{1}数据类型不一致：", name, mfield);
                            //记录缺少此图层
                            DataRow dr = _QualityErrorTable.NewRow();
                            dr["质量元素"] = qualityFClass;
                            dr["质量子元素"] = qualitySClass;
                            dr["错漏类别"] = qualityErrorClass;
                            dr["错漏描述"] = qualityError;
                            dr["处理意见"] = chuli;
                            dr["复查情况"] = fucha;
                            dr["修改情况"] = xiugai;

                            dr["检查者"] = localCheckProject.currentuser;
                            dr["检查日期"] = DateTime.Now.ToString();
                            _QualityErrorTable.Rows.Add(dr);

                        }
                    }
                    if (field.Type == esriFieldType.esriFieldTypeString)
                    {
                        if(fielddesign.FieldType!="Text")
                        {
                            qualityErrorClass = "不符合";
                            qualityError = string.Format("图层{0}的字段{1}数据类型不一致：", name, mfield);
                            //记录缺少此图层
                            DataRow dr = _QualityErrorTable.NewRow();
                            dr["质量元素"] = qualityFClass;
                            dr["质量子元素"] = qualitySClass;
                            dr["错漏类别"] = qualityErrorClass;
                            dr["错漏描述"] = qualityError;
                            dr["处理意见"] = chuli;
                            dr["复查情况"] = fucha;
                            dr["修改情况"] = xiugai;

                            dr["检查者"] = localCheckProject.currentuser;
                            dr["检查日期"] = DateTime.Now.ToString();
                            _QualityErrorTable.Rows.Add(dr);

                        }
                        else if(field.Length!=fielddesign.FieldLength)
                        {
                            qualityErrorClass = "不符合";
                            qualityError = string.Format("图层{0}的字段{1}是长度不一致：", name, mfield);
                            //记录缺少此图层
                            DataRow dr = _QualityErrorTable.NewRow();
                            dr["质量元素"] = qualityFClass;
                            dr["质量子元素"] = qualitySClass;
                            dr["错漏类别"] = qualityErrorClass;
                            dr["错漏描述"] = qualityError;
                            dr["处理意见"] = chuli;
                            dr["复查情况"] = fucha;
                            dr["修改情况"] = xiugai;

                            dr["检查者"] = localCheckProject.currentuser;
                            dr["检查日期"] = DateTime.Now.ToString();
                            _QualityErrorTable.Rows.Add(dr);
                        }
                    }
                    if (field.Type == esriFieldType.esriFieldTypeSingle)
                    {
                        if (fielddesign.FieldType != "Float")
                        {
                            qualityErrorClass = "不符合";
                            qualityError = string.Format("图层{0}的字段{1}数据类型不一致：", name, mfield);
                            //记录缺少此图层
                            DataRow dr = _QualityErrorTable.NewRow();
                            dr["质量元素"] = qualityFClass;
                            dr["质量子元素"] = qualitySClass;
                            dr["错漏类别"] = qualityErrorClass;
                            dr["错漏描述"] = qualityError;
                            dr["处理意见"] = chuli;
                            dr["复查情况"] = fucha;
                            dr["修改情况"] = xiugai;

                            dr["检查者"] = localCheckProject.currentuser;
                            dr["检查日期"] = DateTime.Now.ToString();
                            _QualityErrorTable.Rows.Add(dr);

                        }
                    }
                }           
                
            }

            CheckFieldValue();
            CheckNullValue();
            SaveToDb();

        }

        private void SaveToDb()
        {
            /////////////更新到数据库
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string SampleerrorCollectionTableName = "检查意见记录表";
            //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
            List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
            if (tableNames2.Contains(SampleerrorCollectionTableName) == false)
            {
                string sql_createTable = string.Format("create table {0}( ProjectID text,Mapnumber text,质量元素 text,质量子元素 text,错漏类别 text ,错漏描述 text ,处理意见 text, 修改情况 text, 复查情况 text,检查者 text,检查日期 text)", SampleerrorCollectionTableName);
                datareadwrite.ExecuteSQL(sql_createTable);
                //记录
            }
            //逐条记录到数据表SearchTargetSetting
            string sql_queryitem = string.Format("select Mapnumber from {0} where ProjectID = '{1}' and Mapnumber='{2}' ", SampleerrorCollectionTableName, _sprojectid, SMapnumber);
            object o = datareadwrite.GetScalar(sql_queryitem);
            if (o == null)//insert
            {
            }
            else // delete
            {
                string sql_delete = string.Format("delete from {0} where  ProjectID = '{1}' and Mapnumber='{2}' ", SampleerrorCollectionTableName, _sprojectid, SMapnumber);
                datareadwrite.ExecuteSQL(sql_delete);
            }


            //写入新的问题记录
            foreach (DataRow dr in _QualityErrorTable.Rows)
            {
                string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", SampleerrorCollectionTableName, _sprojectid, SMapnumber,
                    dr["质量元素"] as string, dr["质量子元素"] as string, dr["错漏类别"] as string, dr["错漏描述"] as string, dr["处理意见"] as string, dr["修改情况"] as string, dr["复查情况"] as string, dr["检查者"] as string, dr["检查日期"] as string);
                datareadwrite.ExecuteSQL(sql_insert);
                //记录搜索到的目标
            }

            MessageBox.Show("提交数据库完毕！可以在打印检查意见记录表窗口中查看结果！");

        }

        private void button1_ListEnumValues_Click(object sender, EventArgs e)
        {
            FrmDatatableViewer fdtv = new FrmDatatableViewer(dtvalue);
            fdtv.ShowDialog();
        }

        //
    }
}

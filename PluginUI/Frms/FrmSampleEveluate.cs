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
using DLGCheckLib;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ReportPrinter;
using Utils;
using DatabaseDesignPlus;
using Newtonsoft.Json;
//using PluginUI.CallClass;

namespace PluginUI.Frms
{
    public partial class FrmSampleEveluate : Form
    {
        private IFeatureLayer pScaterFeatureLayer;
        private IFeatureLayer pMapBindingFeatureLayer;
        private IMap localMap;
        private AxMapControl localmapControl;
        public RelativeLineCollection localRelativeChecklines { set; get; }
        public DLGCheckProjectClass localCheckProject { set; get; }
        public SearchTargetSetting localSearchTargetSetting { set; get; }

        int sampleareaidex_inpara = -1;
        int sampleserial_inpara = -1;
        string szMapnumber_inpara = "";

        public FrmSampleEveluate(DLGCheckProjectClass GlobeCheckProject, SearchTargetSetting GlobeSearchtargetSetting,AxMapControl mapControl,int sampleareaidex=-1, int sampleserial=-1, string szMapnumber="")
        {
            if (GlobeCheckProject == null)
            {
                MessageBox.Show("请先打开或新建一个项目！");
                return;
            }
            sampleareaidex_inpara = sampleareaidex;
            sampleserial_inpara = sampleserial;
            szMapnumber_inpara = szMapnumber;

            InitializeComponent();
            localCheckProject = GlobeCheckProject;
            localSearchTargetSetting = GlobeSearchtargetSetting;
            localMap = mapControl.Map;
            localmapControl = mapControl;

            IFeatureLayer pScaterLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName("scater", localmapControl.Map);
            IFeatureLayer pBindingLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName("mapbindingtable", localmapControl.Map);

            pScaterFeatureLayer = pScaterLayer;
            pMapBindingFeatureLayer = pBindingLayer;

            LoadCombboxItems(localCheckProject);
            //LoadCheckLines(pLayer);

            localmapControl.OnMouseMove += axMapControl1_OnMouseMove;
            localmapControl.OnMouseDown += axMapControl1_OnMouseDown;
        }

        void LoadCombboxItems(DLGCheckProjectClass localproject)
        {
            //comb1 填充抽样区唯一值
            List<int> samplearea = new List<int>();
            foreach(MapSampleItemSetting mset in localCheckProject.MapSampleSetting)
            {
                samplearea.Add(mset.SampleAreaIndex);
            }
            samplearea = samplearea.Distinct().ToList().OrderBy(ao => ao).ToList();

            foreach(int ara in samplearea)
            {
                SampleAreaComboBox1.Items.Add(ara);
            }
            if (samplearea.IndexOf(sampleareaidex_inpara) >= 0)
            {
                SampleAreaComboBox1.Text = Convert.ToString(sampleareaidex_inpara);
            }

        }

        private void SampleAreaComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MapNumberComboBox3.Text = "";
            SampleSerialComboBox2.Text = "";
            SampleSerialComboBox2.Items.Clear();
            //comb2 填充流水号唯一值
            List<int> sampleserial = new List<int>();
            foreach (MapSampleItemSetting mset in localCheckProject.MapSampleSetting)
            {
                sampleserial.Add(mset.SampleSerial);
            }
            sampleserial = sampleserial.Distinct().ToList().OrderBy(ao => ao).ToList();

            foreach (int aserial in sampleserial)
            {
                SampleSerialComboBox2.Items.Add(aserial);
            }
            if (sampleserial.IndexOf(sampleserial_inpara) >= 0)
            {
                SampleSerialComboBox2.Text = Convert.ToString(sampleserial_inpara);
            }

        }

        private void SampleSerialComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            superGridControl1.PrimaryGrid.DataSource = null;
            //dataGridViewX1.DataSource = null;

            MapNumberComboBox3.Text = "";
            foreach (MapSampleItemSetting mset in localCheckProject.MapSampleSetting)
            {
                if(mset.SampleAreaIndex == Convert.ToInt32(SampleAreaComboBox1.SelectedItem) &&
                    mset.SampleSerial == Convert.ToInt32(SampleSerialComboBox2.SelectedItem))
                {
                    MapNumberComboBox3.Text = mset.MapNumber;

                    //根据图号查出结合表范围线，空间查询离散点                    
                    IGeoFeatureLayer geoFeatureLayer = pMapBindingFeatureLayer as IGeoFeatureLayer;
                    IFeatureCursor pCursor = geoFeatureLayer.Search(null, false);

                    IFeature pMapBindFeature =null;
                    IFeature pFeature = pCursor.NextFeature();
                    while (pFeature != null)
                    {
                        int nIndex = pFeature.Fields.FindField("mapnumber");
                        string mapnumber = pFeature.Value[nIndex] as string;
                        if(MapNumberComboBox3.Text == mapnumber)
                        {
                            pMapBindFeature = pFeature;
                            break;
                        }
                        pFeature = pCursor.NextFeature();
                    }

                    if(pMapBindFeature!=null)
                    {
                        //LoadCheckLines(localCheckProject.ProjectID,MapNumberComboBox3.Text,localCheckProject.currentuser);
                        //LoadPinError(localCheckProject.ProjectID, MapNumberComboBox3.Text, localCheckProject.currentuser);
                        ValidandLoadElevation(localCheckProject.ProjectID, MapNumberComboBox3.Text);
                    }                    
                }
            }

            //显示更新统计精度
            string sMapNumber = MapNumberComboBox3.Text;
            //CalcAndDisplayPricision(sMapNumber);
            StatisticState();

            if (localCheckProject.SampleFileFormat == "DWG")
                LoadDWGSample(sMapNumber, localCheckProject, localmapControl, localSearchTargetSetting);

        }

        //确认该项目id下mapnumber样本是否建立了质量评分表，没有表新建，补充记录；没有记录补充记录；记录不一致的进行更新；
        private void ValidandLoadElevation(string projectID,string sMapnumber )
        {
            QualityItems qitems = QualityItems.FromJson(localCheckProject.productType);

            /* string samplecheckrecordtable = "检查意见记录表";
             string samplequalitytable = "样本质量评价表";
             string selecteditems = "ahselecteditems";
             string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
             SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

             IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
             //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
             List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
             if (tableNames2.Contains(samplequalitytable) == false)
             {
                 string sql_createTable = string.Format("create table {0}( ProjectID text,Mapnumber text,样本质量得分 numeric(5,2) ,质量元素 text,质量元素权 numeric(5,2),质量元素得分 numeric(5,2) ,质量子元素 text,质量子元素权 numeric(5,2) ,质量子元素得分 numeric(5,2),A integer,B integer,C integer,D integer,Comment text)", samplequalitytable);
                 datareadwrite.ExecuteSQL(sql_createTable);
             }

             ///根据质量模型拉取各项指标进行数据跟新
             ///
             //写入质量元素/子元素/权值/等信息；
             //补充点位/间距/高程精度得分/粗差率情况/中误差值等，记录到表格
             //从检查记录中提取相应记录写入到评分表格中，针对对应元素值更新
             string sql_projectquality = string.Format("select * from {0} where projectid='{1}' and mapnumber = '{2}'", samplequalitytable, localCheckProject.ProjectID, MapNumberComboBox3.Text);
             if(datareadwrite.GetDataTableBySQL(sql_projectquality).Rows.Count!= qitems.Count)
             {
                 string sql_delete = string.Format("delete  from {0} where projectid='{1}' and mapnumber = '{2}'", samplequalitytable, localCheckProject.ProjectID, MapNumberComboBox3.Text);
                 datareadwrite.ExecuteSQL(sql_delete);

                 foreach (KeyValuePair<string, string> DQPair in qitems.DicQItems)
                 {
                     string sql_insertandupdate = "";
                     string sql_selectqitem = string.Format("select * from {0} where 质量元素 = '{0}' and 质量子元素='{1}' and projectid='{2}' and mapnumber='{3}'", samplequalitytable, DQPair.Value, DQPair.Key, localCheckProject.ProjectID, MapNumberComboBox3.Text);
                     if (datareadwrite.GetDataTableBySQL(sql_selectqitem).Rows.Count < 1)
                     {
                         string sql_qitemweight = string.Format("select 质量元素权 from {0} where 成果种类 = '{1}' and 质量元素 = '{2}'", selecteditems, qitems.QualityName, DQPair.Value);
                         double qitemweight = Convert.ToDouble(datareadwrite.GetScalar(sql_qitemweight));
                         string sql_sqitemweight = string.Format("select 质量子元素权 from {0} where  成果种类 = '{1}' and 质量元素 = '{2}' and 质量子元素='{3}'", selecteditems, qitems.QualityName, DQPair.Value, DQPair.Key);
                         double sqitemweight = Convert.ToDouble(datareadwrite.GetScalar(sql_sqitemweight));
                         sql_insertandupdate = string.Format("insert into {0} values('{1}','{2}',{3},'{4}',{5},{6},'{7}',{8},{9},{10},{11},{12},{13},'{14}')",
                             samplequalitytable, localCheckProject.ProjectID, MapNumberComboBox3.Text, 100, DQPair.Value, qitemweight, 100, DQPair.Key, sqitemweight, 100, 0, 0, 0, 0, "");

                         datareadwrite.ExecuteSQL(sql_insertandupdate);
                     }
                 }
             }
             else
             {
                 foreach(KeyValuePair<string,string>dicqitem in  qitems.DicQItems)
                 {
                     //重置所有打分/扣分记录为初始值
                     string sql_reset = string.Format("update {0} set 样本质量得分=100 ,质量元素得分 =100 ,质量子元素得分 = 100, a=0,b=0,c=0,d=0  where projectid = '{1}' and mapnumber = '{2}' and 质量元素 = '{3}' and  质量子元素='{4}'",
                         samplequalitytable, localCheckProject.ProjectID, MapNumberComboBox3.Text, dicqitem.Value, dicqitem.Key);
                     datareadwrite.ExecuteSQL(sql_reset);

                     //更新对应的质量元素得分/扣分情况
                     string sql_error = string.Format("select 错漏类别,错漏描述 from {0} where projectid = '{1}' and mapnumber = '{2}' and 质量元素 = '{3}' and  质量子元素='{4}'", samplecheckrecordtable, localCheckProject.ProjectID, MapNumberComboBox3.Text,dicqitem.Value,dicqitem.Key);
                     DataTable dataerror = datareadwrite.GetDataTableBySQL(sql_error);
                     foreach(DataRow dr in dataerror.Rows)
                     {
                         string errorclass = dr["错漏类别"] as string;
                         string error = dr["错漏描述"] as string;
                         int errorcount = Convert.ToInt32(errorclass.Substring(1, errorclass.Length-2));
                         string errortype = errorclass.Substring(errorclass.Length - 1, 1);

                         string sql_update = string.Format("update {0} set {1}={2}+{3} where projectid = '{4}' and mapnumber = '{5}' and 质量元素 = '{6}' and  质量子元素='{7}'", samplequalitytable, errortype.ToLower(), errortype.ToLower(), errorcount, localCheckProject.ProjectID, MapNumberComboBox3.Text, dicqitem.Value, dicqitem.Key);
                         datareadwrite.ExecuteSQL(sql_update);
                     }

                 }
                 string sMapNumber = MapNumberComboBox3.Text;
                 string sql_updateScore = "";
                 string sql_updateSoreNULL = "";
                 //更新中误差得分
                 //计算中误差

                 HeightMeanError hme = new HeightMeanError(CallClass.Configs, CallClass.Databases);
                 hme.QueryParameter(sMapNumber);
                 hme.Calc(sMapNumber);
                 hme.UpdateReslut(sMapNumber);

                 sql_updateScore = string.Format("update {0} set 质量子元素得分={1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素='{5}'", samplequalitytable, hme.vScore, localCheckProject.ProjectID, MapNumberComboBox3.Text, "数学精度", "高程精度");
                 sql_updateSoreNULL = string.Format("update {0} set 质量子元素得分=NULL where projectid = '{1}' and mapnumber = '{2}' and 质量元素 = '{3}' and  质量子元素='{4}'", samplequalitytable, localCheckProject.ProjectID, MapNumberComboBox3.Text, "数学精度", "高程精度");
                 datareadwrite.ExecuteSQL(hme.vScore > 0 ? sql_updateScore : sql_updateSoreNULL);

                 //计算中误差
                 PositionMeanError pme = new PositionMeanError(CallClass.Configs, CallClass.Databases);
                 pme.QueryParameter(sMapNumber);
                 pme.Calc(sMapNumber);
                 pme.UpdateReslut(sMapNumber);

                 RelativeMeanError rme = new RelativeMeanError(CallClass.Configs, CallClass.Databases);
                 rme.QueryParameter(sMapNumber);
                 rme.Calc(sMapNumber);
                 rme.UpdateReslut(sMapNumber);


                 sql_updateSoreNULL = string.Format("update {0} set 质量子元素得分=NULL where projectid = '{1}' and mapnumber = '{2}' and 质量元素 = '{3}' and  质量子元素='{4}'", samplequalitytable, localCheckProject.ProjectID, MapNumberComboBox3.Text, "数学精度", "平面精度");
                 if(rme.vScore<0&&pme.vScore<0)
                 {
                     datareadwrite.ExecuteSQL(sql_updateSoreNULL);
                 }else if(rme.vScore > 0 && pme.vScore > 0)
                 {
                     sql_updateScore = string.Format("update {0} set 质量子元素得分={1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素='{5}'", samplequalitytable, (rme.vScore + pme.vScore)/2, localCheckProject.ProjectID, MapNumberComboBox3.Text, "数学精度", "平面精度");
                     datareadwrite.ExecuteSQL(sql_updateScore);
                 }else if(rme.vScore >0 && pme.vScore <0)
                 {
                     sql_updateScore = string.Format("update {0} set 质量子元素得分={1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素='{5}'", samplequalitytable, rme.vScore , localCheckProject.ProjectID, MapNumberComboBox3.Text, "数学精度", "平面精度");
                     datareadwrite.ExecuteSQL(sql_updateScore);

                 }
                 else if (rme.vScore < 0 && pme.vScore > 0)
                 {
                     sql_updateScore = string.Format("update {0} set 质量子元素得分={1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素='{5}'", samplequalitytable,  pme.vScore, localCheckProject.ProjectID, MapNumberComboBox3.Text, "数学精度", "平面精度");
                     datareadwrite.ExecuteSQL(sql_updateScore);

                 }

                 //根据扣分情况求取质量子元素得分
                 foreach (KeyValuePair<string, string> dicqitem in qitems.DicQItems)
                 {
                     double reduceScore = SampleScorer.ReductionScore(localCheckProject.ProjectID, MapNumberComboBox3.Text, dicqitem.Value, dicqitem.Key);
                     sql_updateScore = string.Format("update {0} set 质量子元素得分=质量子元素得分-{1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素='{5}'", samplequalitytable, reduceScore, localCheckProject.ProjectID, MapNumberComboBox3.Text, dicqitem.Value, dicqitem.Key);
                     datareadwrite.ExecuteSQL(sql_updateScore);

                 }
                 //获取质量元素下属的质量子元素得分/权值/计算加权平均值
                 //如存在得分小于60分的，将整个元素得分置为0;

                 foreach(string qitemName in  qitems.QualityItemNames)
                 {
                     double weightAdded = 0;
                     double weightScoreAdded = 0;
                     bool hasLowScore = false;
                     List<string> sqitemName = (from q in qitems.DicQItems where q.Value == qitemName select q.Key).ToList<string>();
                     foreach(string sqName in sqitemName)
                     {
                         string sql_weight = string.Format("select 质量子元素权 from {0} where  成果种类 = '{1}' and 质量元素 = '{2}' and 质量子元素='{3}'", selecteditems, qitems.QualityName, qitemName, sqName);
                         string sql_Score = string.Format("select 质量子元素得分 from {0} where  质量元素 = '{1}' and 质量子元素='{2}' and projectid = '{3}' and mapnumber = '{4}'", samplequalitytable, qitemName, sqName, localCheckProject.ProjectID, MapNumberComboBox3.Text);

                         double weight = Convert.ToDouble(datareadwrite.GetScalar(sql_weight));
                         double Score = Convert.ToDouble(datareadwrite.GetScalar(sql_Score));

                         if (Score < 60) hasLowScore = true;

                         weightAdded += weight;
                         weightScoreAdded += weight * Score;
                     }

                     //更新质量元素得分
                     weightScoreAdded = weightScoreAdded / weightAdded;
                     if (hasLowScore == true) weightScoreAdded = 0;
                     sql_updateScore = string.Format("update {0} set 质量元素得分={1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' ", samplequalitytable, weightScoreAdded, localCheckProject.ProjectID, MapNumberComboBox3.Text, qitemName);
                     datareadwrite.ExecuteSQL(sql_updateScore);
                 }

                 //获取质量元素得分/权值/计算加权平均值
                 double weightAdded0 = 0;
                 double weightScoreAdded0 = 0;
                 bool hasLowScore0 = false;
                 foreach (string qitemName in qitems.QualityItemNames)
                 {
                     string sql_weight = string.Format("select 质量元素权 from {0} where  成果种类 = '{1}' and 质量元素 = '{2}' ", selecteditems, qitems.QualityName, qitemName);
                     string sql_Score = string.Format("select 质量元素得分 from {0} where  质量元素 = '{1}' and projectid = '{2}' and mapnumber = '{3}'", samplequalitytable, qitemName,  localCheckProject.ProjectID, MapNumberComboBox3.Text);

                     double weight = Convert.ToDouble(datareadwrite.GetScalar(sql_weight));
                     double Score = Convert.ToDouble(datareadwrite.GetScalar(sql_Score));
                     if (Score < 60) hasLowScore0 = true;

                     weightAdded0 += weight;
                     weightScoreAdded0 += weight * Score;
                 }
                 //更新样本单位图幅质量元素得分
                 weightScoreAdded0 = weightScoreAdded0 / weightAdded0;
                 if (hasLowScore0 == true) weightScoreAdded0 = 0;

                 sql_updateScore = string.Format("update {0} set 样本质量得分={1} where projectid = '{2}' and mapnumber = '{3}' ", samplequalitytable, weightScoreAdded0, localCheckProject.ProjectID, MapNumberComboBox3.Text);
                 datareadwrite.ExecuteSQL(sql_updateScore);

                 //更新抽样样本质量得分，在位置精度检测项目信息表中备注字段写入得分值
                 sql_updateScore = string.Format("update 位置精度检测项目信息表 set 备注='{0}' where projectid='{1}' and 图幅号='{2}'", weightScoreAdded0, localCheckProject.ProjectID, MapNumberComboBox3.Text);
                 datareadwrite.ExecuteSQL(sql_updateScore);
                 */
                SampleScorer.EvluationOfSample(localCheckProject, MapNumberComboBox3.Text, CallClass.Configs, CallClass.Databases);

            ///读取记录并显示
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            string samplequalitytable = "样本质量评价表";
            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string sql_select = string.Format("select 样本质量得分 , 质量元素,质量元素权, 质量元素得分, 质量子元素, 质量子元素权, 质量子元素得分, a as A类, b as B类, c as C类, d as D类 from {0} where projectid='{1}' and mapnumber = '{2}' ", samplequalitytable, localCheckProject.ProjectID, MapNumberComboBox3.Text);
                DataTable data = datareadwrite.GetDataTableBySQL(sql_select);
                superGridControl1.PrimaryGrid.DataSource = data;


        }

        void LoadDWGSample(string szMapnumber, DLGCheckProjectClass GlobeProject, AxMapControl axMapControl1, SearchTargetSetting GlobeSearchtargetSetting)
        {
            //判断：先打开项目才可以加载数据，并提示先设置好项目坐标系统
            if (GlobeProject == null || axMapControl1.Map.LayerCount == 0)
            {
                MessageBox.Show("请先打开或新建一个项目！");
                return;
            }

            if (MessageBox.Show("是否加载样本图幅？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }

            //判断：加载样本有效
            if (PluginUI.ArcGISHelper.GetFeatureLayerByName("Point", axMapControl1.Map) != null ||
                PluginUI.ArcGISHelper.GetFeatureLayerByName("Polyline", axMapControl1.Map) != null)
            {
                DialogResult dlgresult = MessageBox.Show("已经加载了样本！是否加载新样本？", "提示", MessageBoxButtons.YesNo);
                if (dlgresult == DialogResult.Yes)
                {
                    IFeatureLayer layer = PluginUI.ArcGISHelper.GetFeatureLayerByName("Point", axMapControl1.Map);
                    if (layer != null) axMapControl1.Map.DeleteLayer(layer);
                    layer = PluginUI.ArcGISHelper.GetFeatureLayerByName("Polyline", axMapControl1.Map);
                    if (layer != null) axMapControl1.Map.DeleteLayer(layer);
                    layer = PluginUI.ArcGISHelper.GetFeatureLayerByName("Annotation", axMapControl1.Map);
                    if (layer != null) axMapControl1.Map.DeleteLayer(layer);

                }
                else if (dlgresult == DialogResult.No)
                {

                    return;
                }
            }

            IEnvelope currentViewBox = null;
            IWorkspaceFactory pWorkspaceFactory;
            IFeatureWorkspace pFeatureWorkspace;
            IFeatureLayer pFeatureLayer;
            IFeatureDataset pFeatureDataset;
            string filePath = "";
            string strFullPath = "";
            string fileName = "";
            if (GlobeProject.SampleFilePath != null)
            {
                filePath = GlobeProject.SampleFilePath;
                fileName = szMapnumber + ".dwg";
                strFullPath = filePath + "\\" + fileName;
            }
            //路径不存在的时候打开文件选择框进行选择
            if (File.Exists(strFullPath) == false)
            {
                //获取当前路径和文件名
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "CAD(*.dwg)|*.dwg|All Files(*.*)|*.*";
                dlg.Title = "Open CAD Data file";
                dlg.FileName = szMapnumber;
                if (dlg.ShowDialog() == DialogResult.Cancel) return;

                strFullPath = dlg.FileName;
                if (strFullPath == "" || File.Exists(strFullPath) == false)
                {
                    MessageBox.Show("请选择有效文件！");
                    return;
                }

                int Index = strFullPath.LastIndexOf("\\");
                filePath = strFullPath.Substring(0, Index);
                fileName = strFullPath.Substring(Index + 1);
                //对路径更新
                GlobeProject.SampleFilePath = filePath;
            }

            //打开CAD数据集
            pWorkspaceFactory = new CadWorkspaceFactoryClass();
            pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(filePath, 0);
            //打开一个要素集
            pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset(fileName);
            //IFeaturClassContainer可以管理IFeatureDataset中的每个要素类   
            IFeatureClassContainer pFeatClassContainer = (IFeatureClassContainer)pFeatureDataset;
            //对CAD文件中的要素进行遍历处理 
            for (int i = 0; i < pFeatClassContainer.ClassCount; i++)
            {
                IFeatureClass pFeatClass = pFeatClassContainer.get_Class(i);
                string sLayerName = pFeatClass.AliasName;
                if (pFeatClass.FeatureType == esriFeatureType.esriFTCoverageAnnotation)
                {
                    //如果是注记，则添加注记层
                    pFeatureLayer = new CadAnnotationLayerClass();
                    pFeatureLayer.FeatureClass = pFeatClass;
                    pFeatureLayer.Name = pFeatClass.AliasName;
                    axMapControl1.Map.AddLayer(pFeatureLayer);
                }
                else//如果是点、线、面，则添加要素层
                {
                    pFeatureLayer = new FeatureLayerClass();
                    pFeatureLayer.Name = pFeatClass.AliasName;
                    // 暂不处理MultiPatch/Polygon图层
                    if (pFeatureLayer.Name == "MultiPatch" || pFeatureLayer.Name == "Polygon")
                        continue;

                    pFeatureLayer.FeatureClass = pFeatClass;
                    GlobeSearchtargetSetting.RenderSearchTargetLayer(pFeatureLayer);
                    axMapControl1.Map.AddLayer(pFeatureLayer);

                    currentViewBox = pFeatureLayer.AreaOfInterest;
                }
            }
            //ChangeProjectCoordSystem(GlobeProject.SrText);

            //缩放到结合表
            axMapControl1.ActiveView.Extent = currentViewBox;
            axMapControl1.ActiveView.Refresh();

        }

        public void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {

            IPoint pt = new PointClass();
            pt.PutCoords(e.mapX, e.mapY);

        }

        //右键时出现菜单，并对菜单选择的对象进行个性化设置：设置为起点，取消，设置为终点等
        public void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
          

        }
        private void FrmSearchCheckLines_FormClosed(object sender, FormClosedEventArgs e)
        {
            localmapControl.OnMouseMove -= axMapControl1_OnMouseMove;
            localmapControl.OnMouseDown -= axMapControl1_OnMouseDown;

            DialogResult dr = MessageBox.Show("是否将检测线提交到数据库！", "提示", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
            {
                //string sMapNumber = MapNumberComboBox3.Text;
                // SaveCheckLines(sMapNumber);
                //SavePinError();
            }
        }

        private void SaveCheckLines(string sMapnumber)
        {
            if (localRelativeChecklines == null)
                return;

            //将检测线结果写入数据库
            localRelativeChecklines.Write();
            MessageBox.Show("检测线提交数据库完毕！");
        }


        void StatisticState()
        {
            //statistic the A B C D 错漏个数
            int numA=0, numB = 0, numC = 0, numD = 0;

            string errorstatistic = string.Format("{0}存在错漏：A类{1}个；B类{2}个；C类{3}个；D类{4}个",MapNumberComboBox3.Text, numA, numB, numC, numD);
           relativeErrortoolStripStatusLabel1.Text = errorstatistic;
        }

        private void TSB_PrintQualityReportOfSample_Click(object sender, EventArgs e)
        {
            string szMapnumber = MapNumberComboBox3.Text;
            if (szMapnumber == "")
            {
                MessageBox.Show("请选择图幅号!");
                return;
            }

            //打印质量问题统计表
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(CallClass.Configs["QualityErrorStatisticPrintEnvironment"]);
            PrintEnvironment pPrintEnvironment = (PrintEnvironment)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintEnvironment));
            //未进行问题记录较多的分页处理
            PrintCheckRecordReport pr = new PrintCheckRecordReport();
            pr.Configs = CallClass.Configs;
            pr.Databases = CallClass.Databases;
            pr.AppPath = CallClass.AppPath;

            pr.PrintInit(pPrintEnvironment);
            try
            {
                string word = pr.PrintWord(szMapnumber, localCheckProject.ProjectID);
                if (File.Exists(word))
                    System.Diagnostics.Process.Start(word);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}

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
//using PluginUI.CallClass;

namespace PluginUI.Frms
{
    public partial class FrmPintErrors : Form
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

        int sampleareaidex_inpara = -1;
        int sampleserial_inpara = -1;
        string szMapnumber_inpara = "";

        public PinErrorCollection pinErrorCollection { set; get; }

        public FrmPintErrors(DLGCheckProjectClass GlobeCheckProject, SearchTargetSetting GlobeSearchtargetSetting,AxMapControl mapControl,int sampleareaidex=-1, int sampleserial=-1, string szMapnumber="")
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

        public  void LoadCheckLines(string szProjectid,string szMapnumber,string szCurrentuser)
        {
            if (localRelativeChecklines == null)
                localRelativeChecklines = new RelativeLineCollection();

            localRelativeChecklines.Read(szProjectid,szMapnumber,szCurrentuser);
            List<RelativeLineItem> rlist2 = localRelativeChecklines.relativelineList.OrderByDescending(ao => Convert.ToInt32(ao.PtID)).ToList();
            dataGridViewX1.DataSource = rlist2;
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
            dataGridViewX1.DataSource = null;

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
                        LoadPinError(localCheckProject.ProjectID, MapNumberComboBox3.Text, localCheckProject.currentuser);
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

        private void LoadPinError(string projectID, string text, string currentuser)
        {
            if (pinErrorCollection == null)
                pinErrorCollection = new PinErrorCollection();
            pinErrorCollection.Read(projectID, MapNumberComboBox3.Text);
            if (pinErrorCollection.ErrorItems == null) pinErrorCollection.ErrorItems = new List<PinErrorItem>();
            dataGridViewX1.DataSource = pinErrorCollection.ErrorItems;

            
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

        //拾取间距边长检测线
        private void PickCheckLineButton_Click(object sender, EventArgs e)
        {
            //判断：加载样本有效
            if (PluginUI.ArcGISHelper.GetFeatureLayerByName("Point", localMap) == null)
            {
                MessageBox.Show("请先加载一个样本！");
                return;
            }
            //如果正在拾取，则返回，保护好操作
            if (bPickState == true)
            {
                if(MessageBox.Show("标注工具已打开，是否关闭","警告",MessageBoxButtons.YesNo)==DialogResult.Yes)
                {
                    bPickState = false;
                    SearchCheckLineButton.Text = "开始标注错漏";
                    pLineFeedback = null;
                    startVetex = null;
                }
                return;
            }
            //打开拾取工具，必须关闭现在的Maptool工具,使得工具条暂时禁用。
            localmapControl.CurrentTool = null;
            //
            SearchCheckLineButton.Text = "关闭标注工具";

            //如果起始节点为空，则重新开始
            if (startVetex==null)
            {
                bPickState = true;
                MessageBox.Show("开始标注错漏，请移动鼠标至图面错漏处,单击左键键操作！");
            }

        }
        void pMap_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            IEnvelope envelope = localmapControl.Extent;
            IPoint pt = new PointClass();
            pt.X = (envelope.XMax+envelope.XMin)/ 2;
            pt.Y = (envelope.YMax + envelope.YMin) / 2;
            localmapControl.FlashShape(pt);
        }
        private void editError()
        {
            if (dataGridViewX1.Rows.Count <= 0)
                return;

            this.Hide();
            //选择并浏览到该错漏

            List<PinErrorItem> rlist = dataGridViewX1.DataSource as List<PinErrorItem>;
            //获取这个点的查询结果
            //RelativeLineItem item=localRelativeChecklines.relativelineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            PinErrorItem item = rlist.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);

            Authentication.Class.UserObject usr = new Authentication.Class.UserObject();
            usr.username = localCheckProject.currentuser;
            FrmAddSampleErrorPlus frm = new FrmAddSampleErrorPlus(MapNumberComboBox3.Text, usr, localCheckProject.ProjectID, localCheckProject.productType, item);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                frm.Pinerror.Shape = item.Shape;
                item = frm.Pinerror;

                rlist.RemoveAt(dataGridViewX1.CurrentRow.Index);
                rlist.Add(item);

                List<PinErrorItem> databind = rlist.OrderByDescending(ao => ao.CheckTime).ToList();
                dataGridViewX1.DataSource = databind;

                //updata the collection
                pinErrorCollection.ErrorItems = databind;

                this.Show();
            }
        }

        //双击条目进行编辑，主窗口隐藏；操作结束后显示。
        private void dataGridViewX1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            editError();

        }

        public void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {

            IPoint pt = new PointClass();
            pt.PutCoords(e.mapX, e.mapY);

        }

        //右键时出现菜单，并对菜单选择的对象进行个性化设置：设置为起点，取消，设置为终点等
        public void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (bPickState != true) return;

            IPoint pt = new PointClass();
            pt.PutCoords(e.mapX, e.mapY);
            string pstartpointwkt = Converters.ConvertGeometryToWKT(pt);
            Authentication.Class.UserObject user = new Authentication.Class.UserObject();
            user.username = localCheckProject.currentuser;
            this.Hide();
            //get the latest record 
            List<PinErrorItem> rlist = dataGridViewX1.DataSource as List<PinErrorItem>;
            List<PinErrorItem> databind = rlist.OrderByDescending(ao => ao.CheckTime).ToList();
            PinErrorItem item = null; 
            if (databind.Count > 0)
            {
                item = rlist[0];
               // item.Error = "";
               // item.Comment = "";
               // item.Shape = "";                
            }
            
            FrmAddSampleErrorPlus frm = new FrmAddSampleErrorPlus(MapNumberComboBox3.Text, user, localCheckProject.ProjectID,localCheckProject.productType,item);
            if(frm.ShowDialog()==DialogResult.OK)
            {
                //对标准问题进行记录入表格
                if(frm.Pinerror!=null)
                {
                    frm.Pinerror.Shape = pstartpointwkt;
                    PinErrorItem pinerror = new PinErrorItem();
                    pinerror = frm.Pinerror;
                    if (pinErrorCollection.ErrorItems == null) pinErrorCollection.ErrorItems = new List<PinErrorItem>();
                    pinErrorCollection.ErrorItems.Add(pinerror);

                    databind = pinErrorCollection.ErrorItems.OrderByDescending(ao => ao.CheckTime).ToList();
                    dataGridViewX1.DataSource = databind;
                    dataGridViewX1.Refresh();
                }    
            }
                this.Show();     

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
                SavePinError();
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

        private void SavePinError()
        {
            if (pinErrorCollection.ErrorItems.Count == 0)
                return;

            //错漏记录到记录数据集中
            pinErrorCollection.Write();

            //错漏记录到检查意见记录表中
            QualityItems qItems = new QualityItems();
            qItems = QualityItems.FromJson(localCheckProject.productType);
            pinErrorCollection.Write(qItems);

            MessageBox.Show("检测线提交数据库完毕！");

        }

        //将检测线结果写入数据库
        private void SaveCheckLinestoolStripButton1_Click(object sender, EventArgs e)
        {            
            string sMapNumber = MapNumberComboBox3.Text;

            SavePinError();
            //SaveCheckLines(sMapNumber);
            //显示更新统计精度
            //CalcAndDisplayPricision(sMapNumber);
            StatisticState();
        }

        //显示图幅精度
        private void CalcAndDisplayPricision(string sMapNumber)
        {
            //计算中误差
            RelativeMeanError rme = new RelativeMeanError(CallClass.Configs, CallClass.Databases);
            rme.QueryParameter(sMapNumber);
            rme.Calc(sMapNumber);
            rme.UpdateReslut(sMapNumber);

            string relativeErrorState = string.Format("间距边长精度：{0:N2}，限值{1:N2}，检测点{2}个，粗差限{3:N2}，{4}%；", rme.vError, rme.vMaxError, rme.nValidErrorCount, rme.vGrossError, rme.nGrossErrorRatio);
            relativeErrortoolStripStatusLabel1.Text = relativeErrorState;

            //对表格顺带刷新
            dataGridViewX1.Refresh();
        }

        void StatisticState()
        {
            //statistic the A B C D 错漏个数
            int numA=0, numB = 0, numC = 0, numD = 0;
            List<PinErrorItem> rlist = dataGridViewX1.DataSource as List<PinErrorItem>;
            numA = rlist.FindAll(ao => ao.ErrorType == "A").Count;
            numB = rlist.FindAll(ao => ao.ErrorType == "B").Count;
            numC = rlist.FindAll(ao => ao.ErrorType == "C").Count;
            numD = rlist.FindAll(ao => ao.ErrorType == "D").Count;


            string errorstatistic = string.Format("{0}存在错漏：A类{1}个；B类{2}个；C类{3}个；D类{4}个",MapNumberComboBox3.Text, numA, numB, numC, numD);
           relativeErrortoolStripStatusLabel1.Text = errorstatistic;
        }

        private void RemoveRelativeLinetoolStripButton1_Click(object sender, EventArgs e)
        {
            if (dataGridViewX1.Rows.Count <= 0)
                return;
            string currentPtID = dataGridViewX1.CurrentRow.Cells[1].Value as string;
            if (currentPtID == null || currentPtID == "")
                return;

           if(MessageBox.Show("确定移除间距边长检测线["+ currentPtID + "]吗？","",MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                //获取这个点的查询结果
                List<RelativeLineItem> rlist1= dataGridViewX1.DataSource as List<RelativeLineItem>;
                RelativeLineItem item = rlist1.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);

                //移除间距边长线
                item.Remove(localCheckProject.ProjectID);

                //将测量值显示到列表
                //先计算
                CalcAndDisplayPricision(MapNumberComboBox3.Text);
                //再读取
                localRelativeChecklines.Read(localCheckProject.ProjectID, MapNumberComboBox3.Text, localCheckProject.currentuser);
                List<RelativeLineItem> rlist2 = localRelativeChecklines.relativelineList.OrderBy(ao => Convert.ToInt32(ao.PtID)).ToList();
                dataGridViewX1.DataSource = rlist2;
                dataGridViewX1.Refresh();

                MessageBox.Show("已移除了该检测线。");

            }

        }


        //响应跳转显示比例尺变化
        private void toolStripComboBox1_TextChanged(object sender, EventArgs e)
        {
            if(toolStripComboBox1.Text.IndexOf(':')>0)
            {
                string sMapScalar = toolStripComboBox1.Text.Substring(toolStripComboBox1.Text.IndexOf(':') + 1);
                MapScalar = Convert.ToInt32(sMapScalar);
            }
        }

        private void dataGridViewX1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewX1.Rows.Count <= 0)
                return;
            //选择并浏览到该错漏
            //获取该错漏的位置，将图面中心点平移至此
            string currentPt = dataGridViewX1.CurrentRow.Cells[13].Value as string;
            if (currentPt == "")
                return;

            List<PinErrorItem> rlist = dataGridViewX1.DataSource as List<PinErrorItem>;
            //获取这个点的查询结果
            //RelativeLineItem item=localRelativeChecklines.relativelineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            PinErrorItem item = rlist.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);

            IGeometry newCenter = Utils.Converters.ConvertWKTToGeometry(currentPt);
            //localMap.MapScale = localCheckProject.MapScale/2.5;
            //地图显示比例尺切换为间距测量工具的显示比例尺
            localMap.MapScale = MapScalar == 0 ? localCheckProject.MapScale / 2.5 : MapScalar;

            IEnvelope envelope = localmapControl.Extent;
            envelope.CenterAt(newCenter.Envelope.LowerRight);
            localmapControl.Extent = envelope;

            localmapControl.FlashShape(newCenter);
        }

        private void deletetoolStripButton1_Click(object sender, EventArgs e)
        {
            if (dataGridViewX1.Rows.Count <= 0)
                return;

            //选择并浏览到该错漏

            List<PinErrorItem> rlist = dataGridViewX1.DataSource as List<PinErrorItem>;
            //获取这个点的查询结果
            //RelativeLineItem item=localRelativeChecklines.relativelineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            int deleteidex = dataGridViewX1.CurrentRow.Index;

            if(MessageBox.Show(string.Format("请确认删除第{0}条记录！", deleteidex), "提醒",MessageBoxButtons.OKCancel)==DialogResult.OK)
            {
                PinErrorItem item = rlist.ElementAtOrDefault(deleteidex);
                rlist.RemoveAt(deleteidex);
                //rlist.Add(item);

                List<PinErrorItem> databind = rlist.OrderByDescending(ao => ao.CheckTime).ToList();
                dataGridViewX1.DataSource = databind;

                //updata the collection
                pinErrorCollection.ErrorItems = databind;

                MessageBox.Show(string.Format("已删除第{0}条记录！", deleteidex));

            }

        }

        private void toolStripButton1_editerror_Click(object sender, EventArgs e)
        {
            editError();
        }
    }
}

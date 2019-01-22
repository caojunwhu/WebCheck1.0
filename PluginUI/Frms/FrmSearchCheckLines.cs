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
using ESRI.ArcGIS.Geoprocessing;
using ReportPrinter;
//using PluginUI.CallClass;

namespace PluginUI.Frms
{
    public partial class FrmSearchCheckLines : Form
    {
        private IFeatureLayer pScaterFeatureLayer;
        private IFeatureLayer pMapBindingFeatureLayer;
        private IMap localMap;
        private AxMapControl localmapControl;
        private NewLineFeedback pLineFeedback;
        private IPoint snapVetex;
        private double searchHeightBufferlength = 0;
        private int MapScalar = 100;
        private List<BufferSearchTargetItem> TempBufferSearchResult;
        public delegate void LoadDwgSample(string szMapnumber);
        public LoadDwgSample localLoadDwgSample { set; get; }
        public  CheckLineCollection localChecklines { set; get; }
        public DLGCheckProjectClass localCheckProject { set; get; }
        public SearchTargetSetting localSearchTargetSetting { set; get; }

        int sampleareaidex_inpara = -1;
        int sampleserial_inpara = -1;
        string szMapnumber_inpara = "";
        public FrmSearchCheckLines(DLGCheckProjectClass GlobeCheckProject, SearchTargetSetting GlobeSearchtargetSetting,AxMapControl mapControl, int sampleareaidex=-1, int sampleserial=-1,string szMapnumber ="")
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
            if(samplearea.IndexOf(sampleareaidex_inpara)>=0)
            {
                SampleAreaComboBox1.Text = Convert.ToString(sampleareaidex_inpara);
            }
        }

        public  void LoadCheckLines(IFeatureLayer pScaterLayer,IGeometry mapBind)
        {
            if (localChecklines == null)
                localChecklines = new CheckLineCollection(localCheckProject.ProjectID);

            localChecklines.Initialize(pScaterLayer, mapBind);

            dataGridViewX1.DataSource = localChecklines.checklineList;
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

            if(sampleserial.IndexOf(sampleserial_inpara)>=0)
            {
                SampleSerialComboBox2.Text = Convert.ToString(sampleserial_inpara);
            }
        }

        private IFeature GetMapBind(string sMapnumber)
        {
            IFeature pMapBindFeature = null;
            foreach (MapSampleItemSetting mset in localCheckProject.MapSampleSetting)
            {
                if (mset.SampleAreaIndex == Convert.ToInt32(SampleAreaComboBox1.SelectedItem) &&
                    mset.SampleSerial == Convert.ToInt32(SampleSerialComboBox2.SelectedItem))
                {
                    //根据图号查出结合表范围线，空间查询离散点                    
                    IGeoFeatureLayer geoFeatureLayer = pMapBindingFeatureLayer as IGeoFeatureLayer;
                    IFeatureCursor pCursor = geoFeatureLayer.Search(null, false);

                    IFeature pFeature = pCursor.NextFeature();
                    while (pFeature != null)
                    {
                        int nIndex = pFeature.Fields.FindField("mapnumber");
                        string mapnumber = pFeature.Value[nIndex] as string;
                        if (sMapnumber == mapnumber)
                        {
                            pMapBindFeature = pFeature;
                            break;
                        }
                        pFeature = pCursor.NextFeature();
                    }
                }
            }
            return pMapBindFeature;
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
                        LoadCheckLines(pScaterFeatureLayer, pFeature.Shape);
                    }                    
                }
            }

            //显示更新统计精度
            string sMapNumber = MapNumberComboBox3.Text;
            CalcAndDisplayPricision(sMapNumber);

            //打开本幅样本
            if(localLoadDwgSample!=null)
            {
                localLoadDwgSample(sMapNumber);
            }
            if (localCheckProject.SampleFileFormat == "DWG")
                LoadDWGSample(sMapNumber, localCheckProject, localmapControl, localSearchTargetSetting);

            //显示搜索半径等
            MapSampleItemSetting samplesetting = localCheckProject.GetMapSampleItemSetting(MapNumberComboBox3.Text);
            //计算粗差限
            double baseV = samplesetting.PErrorMax * localCheckProject.MapScale / 1000;
            double ratio = samplesetting.CheckType == "同精度" ? 2 * 1.414 : 2;
            double plainBufferLength = baseV * ratio;
            double grossHeightDiff = samplesetting.HErrorMax * ratio;
            double heightBufferLength = samplesetting.ElevationNoteInterval / 3;

            toolStripTextBox1_heigthbufferlength.Text = string.Format("{0:N1}",Convert.ToString(heightBufferLength));
            toolStripTextBox2_Plainbufferlength.Text = Convert.ToString(plainBufferLength);
            toolStripTextBox1_heigthdiff.Text = Convert.ToString(grossHeightDiff);

        }

        void LoadDWGSample(string szMapnumber, DLGCheckProjectClass GlobeProject,AxMapControl axMapControl1, SearchTargetSetting GlobeSearchtargetSetting)
        {

            //判断：先打开项目才可以加载数据，并提示先设置好项目坐标系统
            if (GlobeProject == null || axMapControl1.Map.LayerCount == 0)
            {
                MessageBox.Show("请先打开或新建一个项目！");
                return;
            }

            if(MessageBox.Show("是否加载样本图幅？","提示",MessageBoxButtons.YesNo)==DialogResult.No)
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
            string filePath="";
            string strFullPath="";
            string fileName = "";
            if (GlobeProject.SampleFilePath!=null)
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
                if(dlg.ShowDialog()==DialogResult.Cancel)return;

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

        //搜索检测线
        private void SearchCheckLineButton_Click(object sender, EventArgs e)
        {
            //判断：加载样本有效
            if (PluginUI.ArcGISHelper.GetFeatureLayerByName("Point", localMap) == null)
            {
                MessageBox.Show("请先加载一个样本！");
                return;
            }
            //将Polyline转换成Polygon
            //**//IFeatureLayer polylineFeatureLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName("Polyline", localMap);
            //**//IFeatureLayer polygonFeaturelayer = PluginUI.ArcGISHelper.TransvertToPolygon(polylineFeatureLayer);


            //制作缓冲区
            MapSampleItemSetting samplesetting = localCheckProject.GetMapSampleItemSetting(MapNumberComboBox3.Text);
            //计算粗差限
            double baseV = samplesetting.PErrorMax * localCheckProject.MapScale / 1000;
            double ratio = samplesetting.CheckType == "同精度" ? 2 * 1.414 : 2;
            double plainBufferLength = baseV * ratio;
            double grossHeightDiff = samplesetting.HErrorMax * ratio;
            //针对设置的搜索半径进行设置，默认设置为三分之一注记间距
            double heightBufferLength = searchHeightBufferlength<=0?samplesetting.ElevationNoteInterval / 3: searchHeightBufferlength;

            //遍历每个点
            foreach (CheckLineItem item in localChecklines.checklineList)
            {
                List<BufferSearchTargetItem> bufferSearchResult = new List<BufferSearchTargetItem>();
                //IFeature pPointFeature = PluginUI.ArcGISHelper.GetFeature(pScaterFeatureLayer, "ptid", item.PtID);
                //IGeometry pGeometry = pPointFeature.Shape;
                IGeometry pGeometry = null;
                IPoint point = new PointClass();
                point.X = item.SX;
                point.Y = item.SY;
                point.Z = item.SZ;

                pGeometry = point;

                //这里需要把pGeometry的坐标系统删除，应为Dwg图层是没有坐标系的，
                pGeometry.SpatialReference = null;

                foreach (DwgLayerInfoItem layerinfoitem in localSearchTargetSetting.DwglayerinfoList)
                {
                    //遍历每个图层
                    IFeatureLayer pLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(layerinfoitem.LayerName,localMap);
                    PluginUI.ArcGISHelper.BufferSearchTarget(pLayer, pGeometry, plainBufferLength, layerinfoitem, bufferSearchResult);
                }

                //这个点累积选中多少个要素，需要统计一下
                //所有自动检测目标，均取值第一个，包括平面和高程
                bufferSearchResult = bufferSearchResult.OrderBy(ao => ao.Distance).ToList();
                if(bufferSearchResult.Count>0&& (bufferSearchResult[0].PointType == CheckPointType.PlanCheck|| bufferSearchResult[0].PointType == CheckPointType.WaitToCheck))
                {
                    item.HeightError = bufferSearchResult[0].Height;
                    item.PlanError = bufferSearchResult[0].Distance;
                    item.PointType = bufferSearchResult[0].PointType;
                    item.Comment = "";
                    item.BufferSearchResults = bufferSearchResult;
                }
                else
                {
                    //没有选中，说明这个点不适于做平面检测点，可能划归到非检测点、超限平面检测点、高程检测点、控制点、定向点等类型
                    //进一步筛查是否属于高程点
                    IFeatureLayer pLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(localSearchTargetSetting.ElevSearchsetting.LayerName, localMap);
                    DwgLayerInfoItem layerinfoitem = localSearchTargetSetting.DwglayerinfoList.Find(ao=>ao.LayerName== localSearchTargetSetting.ElevSearchsetting.LayerName&&ao.Layer== localSearchTargetSetting.ElevSearchsetting.Layer);
                    PluginUI.ArcGISHelper.BufferSearchTarget(pLayer, pGeometry, heightBufferLength, layerinfoitem, bufferSearchResult, false,localSearchTargetSetting.ElevSearchsetting.ElevationField);

                    bufferSearchResult = bufferSearchResult.OrderBy(ao => ao.Height).ToList();
                    if(bufferSearchResult.Count>0&& bufferSearchResult[0].PointType == CheckPointType.HeightCheck&& bufferSearchResult[0].Height <= grossHeightDiff)
                    {
                        item.PlanError = 0;
                        item.HeightError = bufferSearchResult[0].Height;
                        item.PointType = bufferSearchResult[0].PointType;
                        item.BufferSearchResults = bufferSearchResult;
                        item.Comment = "";
                    }
                    else
                    {
                        item.PointType = CheckPointType.WaitToCheck;
                        item.PlanError = 0;
                        item.HeightError = 0;
                        item.Comment = "";
                    }
                }

                if (item.BufferSearchResults == null)
                    item.BufferSearchResults = new List<BufferSearchTargetItem>();
                
            }
            MessageBox.Show("搜索平面检测线完成!");
            dataGridViewX1.Refresh();

        }


        void pMap_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            IEnvelope envelope = localmapControl.Extent;
            IPoint pt = new PointClass();
            pt.X = (envelope.XMax+envelope.XMin)/ 2;
            pt.Y = (envelope.YMax + envelope.YMin) / 2;
            localmapControl.FlashShape(pt);
        }
        //双击条目进行编辑，主窗口隐藏；操作结束后显示。
    private void dataGridViewX1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string currentPtID = dataGridViewX1.CurrentRow.Cells[0].Value as string;
            if (currentPtID == null || currentPtID == "")
                return;

            //获取这个点的查询结果
            CheckLineItem item=localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            IPoint pt = new PointClass();
            pt.X = item.SX;
            pt.Y = item.SY;
            pt.Z = item.SZ;

            //切换比例尺
            //localMap.MapScale = localCheckProject.MapScale/8;
            localMap.MapScale = MapScalar == 0 ? localCheckProject.MapScale / 8 : MapScalar;
            //localmapControl.OnExtentUpdated += new IMapControlEvents2_Ax_OnExtentUpdatedEventHandler(pMap_OnExtentUpdated);

            IEnvelope envelope = localmapControl.Extent;
            envelope.CenterAt(pt);
            localmapControl.Extent = envelope;

            //双击条目后，跟踪线重新开始
            if(pLineFeedback!=null)
            {
                pLineFeedback = null;
            }
            switch(item.PointType)
            {
                case CheckPointType.PlanCheck:
                    {
                        ////////////显示检测线
                        try
                        {
                            ILine pLine = new LineClass();
                            pLine.FromPoint = pt;
                            pLine.PutCoords(pt, item.BufferSearchResults[0].pTargetPoint);
                            // QI到ISegment
                            ISegment pSegment = pLine as ISegment;
                            // 创建一个Path对象
                            ISegmentCollection pPath = new PathClass();
                            object o = Type.Missing;
                            // 通过Isegmentcoletcion接口为Path对象添加Segment对象
                            pPath.AddSegment(pSegment, ref o, ref o);
                            // 创建一个Polyline对象
                            IGeometryCollection pPolyline = new PolylineClass();
                            pPolyline.AddGeometry(pPath as IGeometry, ref o, ref o);

                            IPolyline pPLine = pPolyline as IPolyline;

                            localmapControl.FlashShape(pPLine);
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    break;
                case CheckPointType.HeightCheck:
                    {
                        //如果是高程线，且设置了目标点的，则显示检测线
                        if(item.BufferSearchResults!=null&&item.BufferSearchResults.Count>=1)
                        {
                            try
                            {
                                ILine pLine = new LineClass();
                                pLine.FromPoint = pt;
                                pLine.PutCoords(pt, item.BufferSearchResults[0].pTargetPoint);
                                // QI到ISegment
                                ISegment pSegment = pLine as ISegment;
                                // 创建一个Path对象
                                ISegmentCollection pPath = new PathClass();
                                object o = Type.Missing;
                                // 通过Isegmentcoletcion接口为Path对象添加Segment对象
                                pPath.AddSegment(pSegment, ref o, ref o);
                                // 创建一个Polyline对象
                                IGeometryCollection pPolyline = new PolylineClass();
                                pPolyline.AddGeometry(pPath as IGeometry, ref o, ref o);

                                IPolyline pPLine = pPolyline as IPolyline;

                                localmapControl.FlashShape(pPLine);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        //如果是高程点，且未设置目标点的，则鼠标拖动橡皮线到附近的高程点，自动吸附，获取高程值即可
                        else if(item.BufferSearchResults != null && item.BufferSearchResults.Count == 0)
                        {
                            if (pLineFeedback == null)
                            {
                                pLineFeedback = new NewLineFeedback();                            
                                pLineFeedback.Display = localmapControl.ActiveView.ScreenDisplay;
                                pLineFeedback.Start(pt);
                            }
                            else
                            {
                                pLineFeedback.AddPoint(pt);
                            }
                        }
                    }break;
                case CheckPointType.ControlPoint:
                    {

                    }break;
                case CheckPointType.NonCheck:
                    {
                        pLineFeedback = new NewLineFeedback();
                        pLineFeedback.Display = localmapControl.ActiveView.ScreenDisplay;
                        pLineFeedback.Start(pt);
                    }
                    break;
                case CheckPointType.WaitToCheck:
                    {
                        pLineFeedback = new NewLineFeedback();
                        pLineFeedback.Display = localmapControl.ActiveView.ScreenDisplay;
                        pLineFeedback.Start(pt);
                    }
                    break;

            }
        }

        public void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            IPoint pt = new PointClass();
            pt.PutCoords(e.mapX, e.mapY);
            if(pLineFeedback!=null)
            {
                pLineFeedback.MoveTo(pt);

                double mapSize = 0.5;//设置0.5米为吸附半径
                snapVetex = PluginUI.ArcGISHelper.GetNearestVertex(localmapControl.ActiveView, pt,mapSize,localSearchTargetSetting);
                if(snapVetex!=null)
                {
                    pLineFeedback.MoveTo(snapVetex);
                }
            }
        }
        //右键时出现菜单，并对菜单选择的对象进行个性化设置：高程点、平面点、非检测点设置等
        public void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (pLineFeedback!=null&&snapVetex!=null)
            {
                //pLineFeedback.AddPoint(snapVetex);
                //IPolyline pLine = pLineFeedback.Stop();
                //IPoint ptSnapPoint = pLine.ToPoint;
                //如果点击的是右键
                if(e.button == 2)
                {
                    //通过snapvetex点选取吸附的对象，距离区间设置为1cm，即0.01米，仅可以选取吸附的对象哦，进行空间查询
                    CheckLineItem item = localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
                    IPoint pt = new PointClass();
                    pt.X = item.SX;
                    pt.Y = item.SY;
                    pt.Z = item.SZ;
                    IProximityOperator ipo = snapVetex as IProximityOperator;
                    double distance = ipo.ReturnDistance(pt);

                    List<BufferSearchTargetItem> bufferSearchResult = new List<BufferSearchTargetItem>();
                    double bufferLength = 0.01;

                    //这里需要把pGeometry的坐标系统删除，应为Dwg图层是没有坐标系的，
                    IGeometry pGeometry = snapVetex;
                    pGeometry.SpatialReference = null;
                    foreach (DwgLayerInfoItem layerinfoitem in localSearchTargetSetting.DwglayerinfoList)
                    {
                        //遍历每个图层
                        IFeatureLayer pLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(layerinfoitem.LayerName, localMap);
                        PluginUI.ArcGISHelper.BufferSearchTarget(pLayer, pGeometry, bufferLength, layerinfoitem, bufferSearchResult,true);
                    }

                        ContextMenuStrip menu = new ContextMenuStrip();                    
                        System.Drawing.Point pP = new System.Drawing.Point(e.x, e.y);

                    //这个点累积选中多少个要素，需要统计一下
                    if (bufferSearchResult.Count > 0)
                    {
                        bufferSearchResult = bufferSearchResult.OrderBy(ao => ao.Distance).ToList();
                        //item.PlanError = bufferSearchResult[0].Distance;
                        //item.PointType = CheckPointType.PlanCheck;
                        TempBufferSearchResult = bufferSearchResult;
                        foreach(BufferSearchTargetItem bsitem in bufferSearchResult)
                        {
                            string menustring = "";
                            if(bsitem.PointType == CheckPointType.PlanCheck)
                            {
                                menustring = string.Format("吸附到{0}:距离:{1:N1}米。", bsitem.Layer, distance);
                                ToolStripItem stripSnapPlainCheckPoint = menu.Items.Add(menustring);
                                stripSnapPlainCheckPoint.Click += new EventHandler(StripSetSnapPlainCheckPoint);
                            }
                            else if(bsitem.PointType == CheckPointType.HeightCheck)
                            {
                                menustring = string.Format("吸附到{0}:距离:{1:N1}米；高差{2:N2}米。", bsitem.Layer, distance, bsitem.Height-pt.Z);
                                ToolStripItem stripSnapHeightCheckPoint =  menu.Items.Add(menustring);
                                stripSnapHeightCheckPoint.Click += new EventHandler(StripSetHeightCheckPoint);
                                stripSnapHeightCheckPoint.Tag = bsitem.Height - pt.Z;
                            }
                        }
                            //menu.Items.Add("设置为平面点");
                            //menu.Items.Add("设置为高程点");
                    }
                    menu.Show(localmapControl, pP);
                }
            }
            else if (pLineFeedback != null && snapVetex == null)
            {
                if(e.button==2)
                {
                    //没有选中，说明这个点不适于做平面检测点，可能划归到非检测点、超限平面检测点、高程检测点、控制点、定向点等类型
                    ContextMenuStrip menu = new ContextMenuStrip();
                    System.Drawing.Point pP = new System.Drawing.Point(e.x, e.y);
                    //item.PointType = CheckPointType.NonCheck;
                    //item.PlanError = 0;
                    //item.HeightError = 0;
                    ToolStripItem stripSetControlPoint = menu.Items.Add("设置为控制点");
                    stripSetControlPoint.Click += new EventHandler(StripSetControlPoint);

                    stripSetControlPoint = menu.Items.Add("设置为非检测点");
                    stripSetControlPoint.Click += new EventHandler(StripNonCheckPoint);

                    //如果没有动态吸附到任何图形，也可以强行锚定此点，用做没有图层配置或者影像位置精度检查
                    CheckLineItem item = localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
                    IPoint pt = new PointClass();
                    pt.X = item.SX;
                    pt.Y = item.SY;
                    pt.Z = item.SZ;

                    IPoint ptLock = new PointClass();
                    ptLock.X = e.mapX;
                    ptLock.Y = e.mapY;
                    ptLock.Z = 0;
                    IProximityOperator ipo = pt as IProximityOperator;
                    double distance = ipo.ReturnDistance(ptLock);

                    string menustring = string.Format("锚定此点：XY({0:N3},{1:N3}，间距：{2:N3})。", e.mapX, e.mapY, distance);
                    ToolStripItem stripSnapLockCheckPoint = menu.Items.Add(menustring);
                    stripSnapLockCheckPoint.Click += new EventHandler(StripLockCheckPoint);

                    stripSnapLockCheckPoint.Tag = ptLock;

                    menu.Show(localmapControl, pP);
                    //pLineFeedback = null;     
                }               
            }
        }
        //将吸附点设置为平面检测点
        private void StripSetSnapPlainCheckPoint(object sender, System.EventArgs e)
        {
            MapSampleItemSetting samplesetting = localCheckProject.GetMapSampleItemSetting(MapNumberComboBox3.Text);
            //计算粗差限
            double baseV = samplesetting.PErrorMax * localCheckProject.MapScale / 1000;
            double ratio = samplesetting.CheckType == "同精度" ? 2 * 1.414 : 2;
            double plainBufferLength = baseV * ratio;
            double grossHeightDiff = samplesetting.HErrorMax * ratio;

            CheckLineItem item = localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            item.PointType = CheckPointType.PlanCheck;
            //item.HeightError = 0;
            //item.PlanError = 0;

            IPoint pt = new PointClass();
            pt.X = item.SX;
            pt.Y = item.SY;
            pt.Z = item.SZ;
            IProximityOperator ipo = snapVetex as IProximityOperator;
            double distance = ipo.ReturnDistance(pt);
            //item.PlanError = distance;
            item.Comment = distance > plainBufferLength ? "粗差" : "";

            item.CheckLineValueSave(localCheckProject.ProjectID, MapNumberComboBox3.Text, localCheckProject.currentuser, CheckPointType.PlanCheck, distance, 0, TempBufferSearchResult);
            CalcAndDisplayPricision(MapNumberComboBox3.Text);

            pLineFeedback = null;
        }

        //锚定此点
        private void StripLockCheckPoint(object sender, System.EventArgs e)
        {
            MapSampleItemSetting samplesetting = localCheckProject.GetMapSampleItemSetting(MapNumberComboBox3.Text);
            //计算粗差限
            double baseV = samplesetting.PErrorMax * localCheckProject.MapScale / 1000;
            double ratio = samplesetting.CheckType == "同精度" ? 2 * 1.414 : 2;
            double plainBufferLength = baseV * ratio;
            double grossHeightDiff = samplesetting.HErrorMax * ratio;

            CheckLineItem item = localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            item.PointType = CheckPointType.PlanCheck;
            //item.HeightError = 0;
            //item.PlanError = 0;
            ToolStripItem stripitem = sender as ToolStripItem;
            IPoint ptLock = stripitem.Tag as IPoint;

            IPoint pt = new PointClass();
            pt.X = item.SX;
            pt.Y = item.SY;
            pt.Z = item.SZ;
            IProximityOperator ipo = pt as IProximityOperator;
            double distance = ipo.ReturnDistance(ptLock);
            //item.PlanError = distance;
            item.Comment = distance > plainBufferLength ? "粗差" : "";

            item.CheckLineValueSave(localCheckProject.ProjectID, MapNumberComboBox3.Text, localCheckProject.currentuser, CheckPointType.PlanCheck, distance, 0, null);
            CalcAndDisplayPricision(MapNumberComboBox3.Text);

            pLineFeedback = null;
        }

        //将吸附点设置为高程检测点
        private void StripSetHeightCheckPoint(object sender, System.EventArgs e)
        {
            MapSampleItemSetting samplesetting = localCheckProject.GetMapSampleItemSetting(MapNumberComboBox3.Text);
            //计算粗差限
            double baseV = samplesetting.PErrorMax * localCheckProject.MapScale / 1000;
            double ratio = samplesetting.CheckType == "同精度" ? 2 * 1.414 : 2;
            double plainBufferLength = baseV * ratio;
            double grossHeightDiff = samplesetting.HErrorMax * ratio;

            CheckLineItem item = localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            //item.PointType = CheckPointType.HeightCheck;
            //item.HeightError =Convert.ToDouble(( sender  as ToolStripItem).Tag);
            //item.PlanError = 0;
            double heightdiff = Convert.ToDouble((sender as ToolStripItem).Tag);
            item.Comment = heightdiff > grossHeightDiff ? "粗差" : "";

            item.CheckLineValueSave(localCheckProject.ProjectID, MapNumberComboBox3.Text, localCheckProject.currentuser, CheckPointType.HeightCheck, 0, Convert.ToDouble((sender as ToolStripItem).Tag), TempBufferSearchResult);
            CalcAndDisplayPricision(MapNumberComboBox3.Text);

            pLineFeedback = null;
        }

        //将点设置为控制点
        private void StripSetControlPoint(object sender,System.EventArgs e)
        {
            CheckLineItem item = localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            //item.PointType = CheckPointType.ControlPoint;
            //item.HeightError = 0;
            //item.PlanError = 0;
            item.CheckLineValueSave(localCheckProject.ProjectID, MapNumberComboBox3.Text, localCheckProject.currentuser, CheckPointType.ControlPoint, 0, 0, null);
            CalcAndDisplayPricision(MapNumberComboBox3.Text);

            pLineFeedback = null;
        }
        //将点设置为非检测点
        private void StripNonCheckPoint(object sender, System.EventArgs e)
        {
            CheckLineItem item = localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            //item.PointType = CheckPointType.NonCheck;
            //item.HeightError = 0;
            //item.PlanError = 0;
            item.CheckLineValueSave(localCheckProject.ProjectID, MapNumberComboBox3.Text, localCheckProject.currentuser, CheckPointType.NonCheck, 0, 0, null);
            CalcAndDisplayPricision(MapNumberComboBox3.Text);

            pLineFeedback = null;
        }
        //将点设置为待检测点
        private void ResetToWaitToCheck()
        {
            CheckLineItem item = localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            //item.PointType = CheckPointType.NonCheck;
            //item.HeightError = 0;
            //item.PlanError = 0;
            item.CheckLineValueSave(localCheckProject.ProjectID, MapNumberComboBox3.Text, localCheckProject.currentuser, CheckPointType.WaitToCheck, 0, 0, null);
            CalcAndDisplayPricision(MapNumberComboBox3.Text);

            pLineFeedback = null;
        }
        private void FrmSearchCheckLines_FormClosed(object sender, FormClosedEventArgs e)
        {
            localmapControl.OnMouseMove -= axMapControl1_OnMouseMove;
            localmapControl.OnMouseDown -= axMapControl1_OnMouseDown;

            DialogResult dr = MessageBox.Show("是否将检测线提交到数据库！", "提示", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
            {
                string sMapNumber = MapNumberComboBox3.Text;
                SaveCheckLines(sMapNumber);
            }
        }

        private void SaveCheckLines(string sMapnumber)
        {
            if (localChecklines == null)
                return;

            //首先对检测线有效性进行检查，检测线有效性检查是指，检测线集合中没有WaitToCheck点
            bool validState = true;
            foreach (CheckLineItem item in localChecklines.checklineList)
            {
                if (item.PointType == CheckPointType.WaitToCheck)
                {
                    validState = false;
                }
            }
            if (validState == false)
            {
                MessageBox.Show("提醒：检测线没有完全判断完毕，会影响样本精度统计准确性，不建议提交数据库！本次操作不会阻止提交数据库，但可以待完成人工判定后重新提交数据库以覆盖。");
                // return;
            }
            //将检测线结果写入数据库
            localChecklines.Write(sMapnumber, localCheckProject.currentuser);
            //写入粗差错漏情况
            WriteError(sMapnumber);
            MessageBox.Show("检测线提交数据库完毕！");
        }
        //将检测线结果写入数据库
        private void SaveCheckLinestoolStripButton1_Click(object sender, EventArgs e)
        {            
            string sMapNumber = MapNumberComboBox3.Text;
            SaveCheckLines(sMapNumber);
            //显示更新统计精度
            CalcAndDisplayPricision(sMapNumber);
        }

        //2017-5-4写入高程、平面粗差、超限情况
        private void WriteError(string sMapNumber)
        {
            //计算中误差
            PositionMeanError pme = new PositionMeanError(CallClass.Configs, CallClass.Databases);
            pme.QueryParameter(sMapNumber);
            pme.Calc(sMapNumber);
            pme.UpdateReslut(sMapNumber);

            HeightMeanError hme = new HeightMeanError(CallClass.Configs, CallClass.Databases);
            hme.QueryParameter(sMapNumber);
            hme.Calc(sMapNumber);
            hme.UpdateReslut(sMapNumber);  
            
            //写入粗差错漏
            foreach (CheckLineItem item in localChecklines.checklineList)
            {
                if (item.PointType == CheckPointType.WaitToCheck)
                {
                    
                }else
                {
                    if(item.PointType == CheckPointType.PlanCheck)
                    {
                        if(Math.Abs(item.PlanError)>pme.vGrossError)
                        {
                            //新增一条检查记录，传递到标注窗口
                            PinErrorItem Pinerror = new PinErrorItem();
                            Pinerror.Projectid = localCheckProject.ProjectID; 
                            Pinerror.Mapnumber = sMapNumber;
                            Pinerror.Error = "绝对位置粗差";
                            Pinerror.QualityItem = "数学精度";
                            Pinerror.SubQualityItem ="平面精度";
                            Pinerror.CheckItem = "绝对位置中误差";
                            Pinerror.ErrorType = "C";
                            Pinerror.Checker = localCheckProject.currentuser;
                            Pinerror.CheckTime = DateTime.Now.ToString();
                            Pinerror.Feedback = "";
                            Pinerror.Modify = "";
                            Pinerror.Review = "";
                            Pinerror.Comment = string.Format("绝对位置粗差限值{0}m，实测{1:F2}m", pme.vGrossError, item.PlanError);

                            if(item.BufferSearchResults!=null)
                            {
                                IPoint ptStart = new PointClass();
                                ptStart.X = item.SX;
                                ptStart.Y = item.SY;
                                IPolyline pPline = Utils.ShapeFactory.CreatePolylineFromTwoPoints(ptStart, item.BufferSearchResults[0].pTargetPoint);
                                string wkt = Utils.Converters.ConvertGeometryToWKT(pPline);
                                Pinerror.Shape = wkt;
                            }else
                            {
                                IPoint pt = new PointClass();
                                pt.X = item.SX;
                                pt.Y = item.SY;
                                string wkt = Utils.Converters.ConvertGeometryToWKT(pt);
                                Pinerror.Shape = wkt;
                            }
                            Pinerror.Write();
                        }
                    }
                    if(item.PointType==CheckPointType.HeightCheck)
                    {
                        if(Math.Abs(item.HeightError)>hme.vGrossError)
                        {
                            //新增一条检查记录，传递到标注窗口
                            PinErrorItem Pinerror = new PinErrorItem();
                            Pinerror.Projectid = localCheckProject.ProjectID;
                            Pinerror.Mapnumber = sMapNumber;
                            Pinerror.Error = "注记点高程粗差";
                            Pinerror.QualityItem = "数学精度";
                            Pinerror.SubQualityItem = "高程精度";
                            Pinerror.CheckItem = "注记点高程中误差";
                            Pinerror.ErrorType = "C";
                            Pinerror.Checker = localCheckProject.currentuser;
                            Pinerror.CheckTime = DateTime.Now.ToString();
                            Pinerror.Feedback = "";
                            Pinerror.Modify = "";
                            Pinerror.Review = "";
                            Pinerror.Comment = string.Format("注记点高程粗差限值{0}m，实测{1:F2}m", hme.vGrossError, item.HeightError);

                            if (item.BufferSearchResults != null)
                            {
                                IPoint ptStart = new PointClass();
                                ptStart.X = item.SX;
                                ptStart.Y = item.SY;
                                IPolyline pPline = Utils.ShapeFactory.CreatePolylineFromTwoPoints(ptStart, item.BufferSearchResults[0].pTargetPoint);
                                string wkt = Utils.Converters.ConvertGeometryToWKT(pPline);
                                Pinerror.Shape = wkt;
                            }
                            else
                            {
                                IPoint pt = new PointClass();
                                pt.X = item.SX;
                                pt.Y = item.SY;
                                string wkt = Utils.Converters.ConvertGeometryToWKT(pt);
                                Pinerror.Shape = wkt;
                            }
                            Pinerror.Write();
                        }
                    }

                }
            }

            //写入超限错漏
            if(pme.vError>=0&&pme.vError>pme.vMaxError)
            {
                //新增一条检查记录，传递到标注窗口
                PinErrorItem Pinerror = new PinErrorItem();
                Pinerror.Projectid = localCheckProject.ProjectID;
                Pinerror.Mapnumber = sMapNumber;
                Pinerror.Error = "绝对位置中误差超限";
                Pinerror.QualityItem = "数学精度";
                Pinerror.SubQualityItem = "平面精度";
                Pinerror.CheckItem = "绝对位置中误差";
                Pinerror.ErrorType = "A";
                Pinerror.Checker = localCheckProject.currentuser;
                Pinerror.CheckTime = DateTime.Now.ToString();
                Pinerror.Feedback = "";
                Pinerror.Modify = "";
                Pinerror.Review = "";
                Pinerror.Comment = string.Format("绝对位置中误差{0:F2}m，限值{1:F2}m", pme.vError,pme.vMaxError);

                IFeature pBind = GetMapBind(sMapNumber);
                if (pBind != null)
                {
                    IPoint pt = new PointClass();
                    pt.X = (pBind.Shape.Envelope.XMax + pBind.Shape.Envelope.XMin) / 2+pme.vError;
                    pt.Y = (pBind.Shape.Envelope.YMax + pBind.Shape.Envelope.YMin) / 2;
                    string wkt = Utils.Converters.ConvertGeometryToWKT(pt);
                    Pinerror.Shape = wkt;
                }

                Pinerror.Write();
            }
            if(hme.vError>=0&&hme.vError>hme.vMaxError)
            {
                //新增一条检查记录，传递到标注窗口
                PinErrorItem Pinerror = new PinErrorItem();
                Pinerror.Projectid = localCheckProject.ProjectID;
                Pinerror.Mapnumber = sMapNumber;
                Pinerror.Error = "注记点高程中误差超限";
                Pinerror.QualityItem = "数学精度";
                Pinerror.SubQualityItem = "高程精度";
                Pinerror.CheckItem = "注记点高程中误差";
                Pinerror.ErrorType = "A";
                Pinerror.Checker = localCheckProject.currentuser;
                Pinerror.CheckTime = DateTime.Now.ToString();
                Pinerror.Feedback = "";
                Pinerror.Modify = "";
                Pinerror.Review = "";
                Pinerror.Comment = string.Format("注记点高程中误差{0:F2}m，限值{1:F2}m", hme.vError,  hme.vMaxError); ;

                IFeature pBind = GetMapBind(sMapNumber);
                if (pBind != null)
                {
                    IPoint pt = new PointClass();
                    pt.X = (pBind.Shape.Envelope.XMax + pBind.Shape.Envelope.XMin) / 2 + hme.vError;
                    pt.Y = (pBind.Shape.Envelope.YMax + pBind.Shape.Envelope.YMin) / 2;
                    string wkt = Utils.Converters.ConvertGeometryToWKT(pt);
                    Pinerror.Shape = wkt;
                }

                Pinerror.Write();
            }

            if(pme.nGrossErrorRatio>5)
            {
                //新增一条检查记录，传递到标注窗口
                PinErrorItem Pinerror = new PinErrorItem();
                Pinerror.Projectid = localCheckProject.ProjectID;
                Pinerror.Mapnumber = sMapNumber;
                Pinerror.Error = "绝对位置粗差率超限";
                Pinerror.QualityItem = "数学精度";
                Pinerror.SubQualityItem = "平面精度";
                Pinerror.CheckItem = "绝对位置中误差";
                Pinerror.ErrorType = "A";
                Pinerror.Checker = localCheckProject.currentuser;
                Pinerror.CheckTime = DateTime.Now.ToString();
                Pinerror.Feedback = "";
                Pinerror.Modify = "";
                Pinerror.Review = "";
                Pinerror.Comment = string.Format("绝对位置粗差率{0}%,限值5%", pme.nGrossErrorRatio);

                IFeature pBind = GetMapBind(sMapNumber);
                if (pBind != null)
                {
                    IPoint pt = new PointClass();
                    pt.X = (pBind.Shape.Envelope.XMax + pBind.Shape.Envelope.XMin) / 2 + pme.nGrossErrorRatio;
                    pt.Y = (pBind.Shape.Envelope.YMax + pBind.Shape.Envelope.YMin) / 2;
                    string wkt = Utils.Converters.ConvertGeometryToWKT(pt);
                    Pinerror.Shape = wkt;
                }

                Pinerror.Write();
            }
            if(hme.nGrossErrorRatio>5)
            {
                //新增一条检查记录，传递到标注窗口
                PinErrorItem Pinerror = new PinErrorItem();
                Pinerror.Projectid = localCheckProject.ProjectID;
                Pinerror.Mapnumber = sMapNumber;
                Pinerror.Error = "注记点高程粗差率超限";
                Pinerror.QualityItem = "数学精度";
                Pinerror.SubQualityItem = "高程精度";
                Pinerror.CheckItem = "注记点高程中误差";
                Pinerror.ErrorType = "A";
                Pinerror.Checker = localCheckProject.currentuser;
                Pinerror.CheckTime = DateTime.Now.ToString();
                Pinerror.Feedback = "";
                Pinerror.Modify = "";
                Pinerror.Review = "";
                Pinerror.Comment = string.Format("注记点高程粗差率{0}%,限值5%", hme.nGrossErrorRatio); 

                IFeature pBind = GetMapBind(sMapNumber);
                if (pBind != null)
                {
                    IPoint pt = new PointClass();
                    pt.X = (pBind.Shape.Envelope.XMax + pBind.Shape.Envelope.XMin) / 2 +hme.nGrossErrorRatio;
                    pt.Y = (pBind.Shape.Envelope.YMax + pBind.Shape.Envelope.YMin) / 2;
                    string wkt = Utils.Converters.ConvertGeometryToWKT(pt);
                    Pinerror.Shape = wkt;
                }

                Pinerror.Write();
            }


        }
        //显示图幅精度
        private void CalcAndDisplayPricision(string sMapNumber)
        {
            //计算中误差
            PositionMeanError pme = new PositionMeanError(CallClass.Configs, CallClass.Databases);
            pme.QueryParameter(sMapNumber);
            pme.Calc(sMapNumber);
            pme.UpdateReslut(sMapNumber);

            HeightMeanError hme = new HeightMeanError(CallClass.Configs, CallClass.Databases);
            hme.QueryParameter(sMapNumber);
            hme.Calc(sMapNumber);
            hme.UpdateReslut(sMapNumber);

            string planErrorState = string.Format("平面精度：{0:N2}，限值{1:N2}，检测点{2}个，粗差限{3:N2}，{4}%；", pme.vError,pme.vMaxError, pme.nValidErrorCount,pme.vGrossError, pme.nGrossErrorRatio);
            string heightErrorState = string.Format("高程精度：{0:N2}，限值{1:N2}，检测点{2}个，粗差限{3:N2}，{4}%，{5}检测。", hme.vError,hme.vMaxError, hme.nValidErrorCount, hme.vGrossError, hme.nGrossErrorRatio,hme.sFactorType);
            //string planErrorState = string.Format("平面精度：{0:N2}，检测点{1}个，粗差率{2:0%}；", pme.vError,  pme.nValidErrorCount,  pme.nGrossErrorRatio);
            //string heightErrorState = string.Format("高程精度：{0:N2}，检测点{1}个，粗差率{2:0%}。", hme.vError, hme.nValidErrorCount,hme.nGrossErrorRatio);
            planErrortoolStripStatusLabel1.Text = planErrorState;
            heightErrortoolStripStatusLabel2.Text = heightErrorState;

            //对表格顺带刷新
            dataGridViewX1.Refresh();
        }
        //对已经判断完毕的点进行重设，状态更改为WaitToCheck
        private void ReSetCheckPointToWaittoolStripButton1_Click(object sender, EventArgs e)
        {
            ResetToWaitToCheck();
            MessageBox.Show("重新设置为WaitToCheck点完毕！请继续操作。");
        }
        //切换跳转比例尺
        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox1.Text.IndexOf(':') > 0)
            {
                string sMapScalar = toolStripComboBox1.Text.Substring(toolStripComboBox1.Text.IndexOf(':') + 1);
                MapScalar = Convert.ToInt32(sMapScalar);
            }

        }
        //高程注记搜索半径调整
        private void toolStripTextBox1_heigthbufferlength_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox1_heigthbufferlength.Text != "")
            {
                searchHeightBufferlength = Convert.ToDouble(toolStripTextBox1_heigthbufferlength.Text);
            }

        }
    }
}

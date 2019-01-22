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
//using PluginUI.CallClass;

namespace PluginUI.Frms
{
    public partial class FrmRelativeCheckLines : Form
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

        public FrmRelativeCheckLines(DLGCheckProjectClass GlobeCheckProject, SearchTargetSetting GlobeSearchtargetSetting,AxMapControl mapControl,int sampleareaidex=-1, int sampleserial=-1, string szMapnumber="")
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
                        LoadCheckLines(localCheckProject.ProjectID,MapNumberComboBox3.Text,localCheckProject.currentuser);
                    }                    
                }
            }

            //显示更新统计精度
            string sMapNumber = MapNumberComboBox3.Text;
            CalcAndDisplayPricision(sMapNumber);

            if (localCheckProject.SampleFileFormat == "DWG")
                LoadDWGSample(sMapNumber, localCheckProject, localmapControl, localSearchTargetSetting);

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
                if(MessageBox.Show("拾取工具已打开，是否关闭","警告",MessageBoxButtons.YesNo)==DialogResult.Yes)
                {
                    bPickState = false;
                    SearchCheckLineButton.Text = "拾取间距边长";
                    pLineFeedback = null;
                    startVetex = null;
                }
                return;
            }
            //打开拾取工具，必须关闭现在的Maptool工具,使得工具条暂时禁用。
            localmapControl.CurrentTool = null;
            //
            SearchCheckLineButton.Text = "关闭拾取工具";

            //如果起始节点为空，则重新开始
            if (startVetex==null)
            {
                bPickState = true;
                MessageBox.Show("开始拾取间距边长，请移动鼠标至平面特征点,点击右键操作！");
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
        //双击条目进行编辑，主窗口隐藏；操作结束后显示。
        private void dataGridViewX1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string currentPtID = dataGridViewX1.CurrentRow.Cells[0].Value as string;
            if (currentPtID == null || currentPtID == "")
                return;
            List<RelativeLineItem> rlist = dataGridViewX1.DataSource as List<RelativeLineItem>;
            //获取这个点的查询结果
            //RelativeLineItem item=localRelativeChecklines.relativelineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            RelativeLineItem item = rlist.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            if (item.StartPoint == null || item.TargetPoint == null)
                return;

            //localMap.MapScale = localCheckProject.MapScale/2.5;
            //地图显示比例尺切换为间距测量工具的显示比例尺
            localMap.MapScale = MapScalar==0? localCheckProject.MapScale / 2.5:MapScalar;

            IEnvelope envelope = localmapControl.Extent;
            envelope.CenterAt(item.StartPoint);
            localmapControl.Extent = envelope;

            //双击条目后，跟踪线重新开始
            if (pLineFeedback != null)
            {
                pLineFeedback = null;
            }
            ////////////显示检测线
            try
            {
                ILine pLine = new LineClass();
                pLine.FromPoint = item.StartPoint;
                pLine.PutCoords(item.StartPoint,item.TargetPoint);
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

        public void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {

            IPoint pt = new PointClass();
            pt.PutCoords(e.mapX, e.mapY);

            //确保maptool为null
            if(bPickState==true&&localmapControl.CurrentTool!=null)
            {
                localmapControl.CurrentTool = null;
                MessageBox.Show("关闭拾取工具后才可以正常使用其他工具！");
            }

            //double mapSize = 0.5;//设置0.5米为吸附半径
            double mapSize =snapBufferlength == 0 ? 0.5 : snapBufferlength;
            snapVetex = PluginUI.ArcGISHelper.GetNearestVertex(localmapControl.ActiveView, pt,mapSize,localSearchTargetSetting);
            if (snapVetex != null&& bPickState == true)
            {
                oldCursor = localmapControl.MousePointer;
                localmapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            }
            else
            {
                //跑出了选中区域则鼠标自动切换为原来的鼠标了
                localmapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }

            if (pLineFeedback!=null)
            {
                pLineFeedback.MoveTo(pt);

                if(snapVetex!= null &&bPickState == true)
                {
                    //拾取状态
                    pLineFeedback.MoveTo(snapVetex);
                    oldCursor = localmapControl.MousePointer;
                    localmapControl.MousePointer =esriControlsMousePointer.esriPointerCrosshair;
                }
                else if(snapVetex == null && bPickState == true)
                {
                    //跑出了选中区域则鼠标自动切换为原来的鼠标了
                    localmapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
                }
            }
            else
            {
                //开始拾取了
                if(bPickState==true)
                {
                    //将起点设置好了，接着添加右键菜单，设置起始点或取消起始点
                    startVetex = pt;


                }
            }
        }

        //右键时出现菜单，并对菜单选择的对象进行个性化设置：设置为起点，取消，设置为终点等
        public void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            //开始拾取了
            if(pLineFeedback==null && bPickState==true)
            {
                if(e.button == 2)
                {
                    //通过startVetex点选取吸附的对象，距离区间设置为1cm，即0.01米，仅可以选取吸附的对象哦，进行空间查询
                    IProximityOperator ipo = startVetex as IProximityOperator;
                    double distance = ipo.ReturnDistance(startVetex);

                    List<BufferSearchTargetItem> bufferSearchResult = new List<BufferSearchTargetItem>();
                    double bufferLength = snapBufferlength == 0 ? 0.1 : snapBufferlength;

                    //这里需要把pGeometry的坐标系统删除，应为Dwg图层是没有坐标系的，
                    IGeometry pGeometry = startVetex;
                    pGeometry.SpatialReference = null;
                    foreach (DwgLayerInfoItem layerinfoitem in localSearchTargetSetting.DwglayerinfoList)
                    {
                        //遍历每个图层
                        IFeatureLayer pLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(layerinfoitem.LayerName, localMap);
                        PluginUI.ArcGISHelper.BufferSearchTarget(pLayer, pGeometry, bufferLength, layerinfoitem, bufferSearchResult, true);
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
                        foreach (BufferSearchTargetItem bsitem in bufferSearchResult)
                        {
                            string menustring = "";
                            if (bsitem.PointType == CheckPointType.PlanCheck)
                            {
                                menustring = string.Format("吸附到{0}:距离:{1:N2}米,作为间距边长检测线起点", bsitem.Layer, bsitem.Distance);
                                ToolStripItem stripSnapPlainCheckPoint = menu.Items.Add(menustring);
                                stripSnapPlainCheckPoint.Click += new EventHandler(StripSetStartCheckPoint);
                                stripSnapPlainCheckPoint.Tag = bsitem;
                            }
                        }
                        //添加取消按钮菜单
                        ToolStripItem stripSnapCancel = menu.Items.Add("取消拾取操作！");
                        stripSnapCancel.Click += new EventHandler(StripSnapConcel);

                    }
                    menu.Show(localmapControl, pP);
                }            
            }else if (pLineFeedback != null && bPickState == true)
            {
                if (e.button == 2)
                {
                    //通过startVetex点选取吸附的对象，距离区间设置为1cm，即0.01米，仅可以选取吸附的对象哦，进行空间查询
                    IProximityOperator ipo = snapVetex as IProximityOperator;
                    if (snapVetex == null)
                    {
                        ContextMenuStrip menu0 = new ContextMenuStrip();
                        System.Drawing.Point pP0 = new System.Drawing.Point(e.x, e.y);
                        //添加取消按钮菜单
                        ToolStripItem stripSnapCancel = menu0.Items.Add("取消拾取操作！");
                        stripSnapCancel.Click += new EventHandler(StripSnapConcel);
                        menu0.Show(localmapControl, pP0);

                        return;
                    }
                    //计算起点到终点的距离
                    double distance = ipo.ReturnDistance(startVetex);

                    List<BufferSearchTargetItem> bufferSearchResult = new List<BufferSearchTargetItem>();
                    double bufferLength = 0.1;

                    //这里需要把pGeometry的坐标系统删除，应为Dwg图层是没有坐标系的，
                    IGeometry pGeometry = snapVetex;
                    pGeometry.SpatialReference = null;
                    foreach (DwgLayerInfoItem layerinfoitem in localSearchTargetSetting.DwglayerinfoList)
                    {
                        //遍历每个图层
                        IFeatureLayer pLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(layerinfoitem.LayerName, localMap);
                        PluginUI.ArcGISHelper.BufferSearchTarget(pLayer, pGeometry, bufferLength, layerinfoitem, bufferSearchResult, true);
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
                        foreach (BufferSearchTargetItem bsitem in bufferSearchResult)
                        {
                            string menustring = "";
                            if (bsitem.PointType == CheckPointType.PlanCheck)
                            {
                                menustring = string.Format("吸附到{0}:距离:{1:N2}米,作为间距边长检测线终点", bsitem.Layer, distance);
                                ToolStripItem stripSnapPlainCheckPoint = menu.Items.Add(menustring);
                                stripSnapPlainCheckPoint.Click += new EventHandler(StripSetEndCheckPoint);
                                stripSnapPlainCheckPoint.Tag = bsitem;
                            }
                        }

                        //添加取消按钮菜单
                        ToolStripItem stripSnapCancel = menu.Items.Add("取消拾取操作！");
                        stripSnapCancel.Click += new EventHandler(StripSnapConcel);

                    }
                    menu.Show(localmapControl, pP);
                }
            }
        }
        //
        private void ReSetTargetPoint()
        {
            if (dataGridViewX1.Rows.Count <= 0)
                return;
            string currentPtID = dataGridViewX1.CurrentRow.Cells[1].Value as string;
            if (currentPtID == null || currentPtID == "")
                return;

            //获取这个点的查询结果
            RelativeLineItem item = localRelativeChecklines.relativelineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);

            //重新生成起始点发出的线
            //设置为拾取标志
            bPickState = true;
            startVetex = item.StartPoint;
            Startlayer = item.StartLayer;

            pLineFeedback = new NewLineFeedback();
            pLineFeedback.Display = localmapControl.ActiveView.ScreenDisplay;
            pLineFeedback.Start(startVetex);

            bEditState = true;

        }
        //取消操作按钮
        private void StripSnapConcel(object sender, System.EventArgs e)
        {
            pLineFeedback = null;
            //bPickState = false;
            startVetex = null;
        }
        //将吸附点设置为间距边长起点
        private void StripSetStartCheckPoint(object sender,System.EventArgs e)
        {
            //起点设置好后就可以绘制线了
            ToolStripItem stripSnapPlainCheckPoint = sender as ToolStripItem;
            BufferSearchTargetItem bsitem = stripSnapPlainCheckPoint.Tag as BufferSearchTargetItem;
            startVetex = bsitem.pTargetPoint;
            pLineFeedback = new NewLineFeedback();
            pLineFeedback.Display = localmapControl.ActiveView.ScreenDisplay;
            pLineFeedback.Start(startVetex);
            Startlayer = bsitem.Layer;

        }
        //将吸附点设置为间距边长终点
        private void StripSetEndCheckPoint(object sender, System.EventArgs e)
        {
            //此处用以存储检测线，弹出窗口输入实测值
            FrmInputRelativeValue frmrv = new FrmInputRelativeValue();
            if(frmrv.ShowDialog()==DialogResult.OK)
            {
                ToolStripItem stripSnapPlainCheckPoint = sender as ToolStripItem;
                BufferSearchTargetItem bsitem = stripSnapPlainCheckPoint.Tag as BufferSearchTargetItem;
                IPoint endVertex = bsitem.pTargetPoint;
               //将起始点练成线保存到数据库；
               if(localRelativeChecklines!=null)
                {
                    RelativeLineItem item = new RelativeLineItem();
                    item.StartLayer = Startlayer;
                    item.StartPoint = startVetex;
                    item.TargetLayer =bsitem.Layer;
                    item.TargetPoint = endVertex;
                    item.Checker = localCheckProject.currentuser;
                    item.Date = DateTime.Now.ToString();

                    if(bEditState==true)
                    {
                        string currentPtID = dataGridViewX1.CurrentRow.Cells[1].Value as string;
                        if (currentPtID == null || currentPtID == "")
                            return;
                        item.PtID = currentPtID;
                    }else
                    {
                        List<RelativeLineItem>rlist=localRelativeChecklines.relativelineList.OrderBy(ao => Convert.ToInt32(ao.PtID)).ToList();
                        int ptid = rlist.Count > 0 ? Convert.ToInt32(rlist[rlist.Count - 1].PtID) + 1 : 1;
                        item.PtID = Convert.ToString(ptid);
                    }

                    item.Mapnumber = MapNumberComboBox3.Text;
                    item.SL = Convert.ToDouble(frmrv.SRelativeValue);
                    IProximityOperator ipo = item.StartPoint as IProximityOperator;
                    double distance = ipo.ReturnDistance(item.TargetPoint);
                    item.TL = distance;
                    item.DL = item.TL-item.SL;

                    item.Comment = "";
                    localRelativeChecklines.relativelineList.Add(item);
                    item.RelativeLineSave(localCheckProject.ProjectID, MapNumberComboBox3.Text, localCheckProject.currentuser);

                    //将测量值显示到列表
                    //先计算
                    CalcAndDisplayPricision(MapNumberComboBox3.Text);
                    //再读取
                    localRelativeChecklines.Read(localCheckProject.ProjectID, MapNumberComboBox3.Text, localCheckProject.currentuser);
                    List<RelativeLineItem> rlist2 = localRelativeChecklines.relativelineList.OrderByDescending(ao => Convert.ToInt32(ao.PtID)).ToList();
                    dataGridViewX1.DataSource = rlist2;
                    dataGridViewX1.Refresh();
                }
                pLineFeedback = null;
                //bPickState = false;
                startVetex = null;
                if (bEditState == true) bEditState = false;
            }
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
            if (localRelativeChecklines == null)
                return;

            //将检测线结果写入数据库
            localRelativeChecklines.Write();
            MessageBox.Show("检测线提交数据库完毕！");
        }
        //将检测线结果写入数据库
        private void SaveCheckLinestoolStripButton1_Click(object sender, EventArgs e)
        {            
            string sMapNumber = MapNumberComboBox3.Text;
            SaveCheckLines(sMapNumber);
            //WriteError
            WriteError(sMapNumber);
            //显示更新统计精度
            CalcAndDisplayPricision(sMapNumber);
        }
        //2017-5-4写入  间距粗差、中误差超限、粗差率超限情况
        private void WriteError(string sMapNumber)
        {
            //计算中误差
            RelativeMeanError rme = new RelativeMeanError(CallClass.Configs, CallClass.Databases);
            rme.QueryParameter(sMapNumber);
            rme.Calc(sMapNumber);
            rme.UpdateReslut(sMapNumber);

            //写入粗差错漏
            foreach (RelativeLineItem item in localRelativeChecklines.relativelineList)
            {

                if (Math.Abs(item.DL) > rme.vGrossError)
                {
                    //新增一条检查记录，传递到标注窗口
                    PinErrorItem Pinerror = new PinErrorItem();
                    Pinerror.Projectid = localCheckProject.ProjectID;
                    Pinerror.Mapnumber = sMapNumber;
                    Pinerror.Error = "相对位置粗差";
                    Pinerror.QualityItem = "数学精度";
                    Pinerror.SubQualityItem = "平面精度";
                    Pinerror.CheckItem = "相对位置中误差";
                    Pinerror.ErrorType = "C";
                    Pinerror.Checker = localCheckProject.currentuser;
                    Pinerror.CheckTime = DateTime.Now.ToString();
                    Pinerror.Feedback = "";
                    Pinerror.Modify = "";
                    Pinerror.Review = "";
                    Pinerror.Comment = string.Format("相对位置粗差限值{0}m，实测{1:F2}m", rme.vGrossError, item.DL);

                    IPolyline pPline = Utils.ShapeFactory.CreatePolylineFromTwoPoints(item.StartPoint, item.TargetPoint);
                    string wkt = Utils.Converters.ConvertGeometryToWKT(pPline);
                    Pinerror.Shape = wkt;
                    Pinerror.Write();

                }
            }

            //写入超限错漏
            if (rme.vError > rme.vMaxError)
            {
                //新增一条检查记录，传递到标注窗口
                PinErrorItem Pinerror = new PinErrorItem();
                Pinerror.Projectid = localCheckProject.ProjectID;
                Pinerror.Mapnumber = sMapNumber;
                Pinerror.Error = "相对位置中误差超限";
                Pinerror.QualityItem = "数学精度";
                Pinerror.SubQualityItem = "平面精度";
                Pinerror.CheckItem = "相对位置中误差";
                Pinerror.ErrorType = "A";
                Pinerror.Checker = localCheckProject.currentuser;
                Pinerror.CheckTime = DateTime.Now.ToString();
                Pinerror.Feedback = "";
                Pinerror.Modify = "";
                Pinerror.Review = "";
                Pinerror.Comment = string.Format("相对位置中误差{0}m，限值{1}m，实测{2:F2}m", "", rme.vMaxError, rme.vError);

                IFeature pBind = GetMapBind(sMapNumber);
                if (pBind != null)
                {
                    IPoint pt = new PointClass();
                    pt.X = (pBind.Shape.Envelope.XMax + pBind.Shape.Envelope.XMin) / 2;
                    pt.Y = (pBind.Shape.Envelope.YMax + pBind.Shape.Envelope.YMin) / 2 + rme.vError;
                    string wkt = Utils.Converters.ConvertGeometryToWKT(pt);
                    Pinerror.Shape = wkt;
                }

                Pinerror.Write();
            }

            if (rme.nGrossErrorRatio > 5)
            {
                //新增一条检查记录，传递到标注窗口
                PinErrorItem Pinerror = new PinErrorItem();
                Pinerror.Projectid = localCheckProject.ProjectID;
                Pinerror.Mapnumber = sMapNumber;
                Pinerror.Error = "相对位置粗差率超限";
                Pinerror.QualityItem = "数学精度";
                Pinerror.SubQualityItem = "平面精度";
                Pinerror.CheckItem = "相对位置中误差";
                Pinerror.ErrorType = "A";
                Pinerror.Checker = localCheckProject.currentuser;
                Pinerror.CheckTime = DateTime.Now.ToString();
                Pinerror.Feedback = "";
                Pinerror.Modify = "";
                Pinerror.Review = "";
                Pinerror.Comment = string.Format("相对位置粗差率限值5%，实测{0}%", rme.nGrossErrorRatio);

                IFeature pBind = GetMapBind(sMapNumber);
                if (pBind != null)
                {
                    IPoint pt = new PointClass();
                    pt.X = (pBind.Shape.Envelope.XMax + pBind.Shape.Envelope.XMin) / 2 ;
                    pt.Y = (pBind.Shape.Envelope.YMax + pBind.Shape.Envelope.YMin) / 2 + rme.nGrossErrorRatio;
                    string wkt = Utils.Converters.ConvertGeometryToWKT(pt);
                    Pinerror.Shape = wkt;
                }

                Pinerror.Write();
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
        //对已经判断完毕的点进行重设，状态更改为WaitToCheck
        private void ReSetCheckPointToWaittoolStripButton1_Click(object sender, EventArgs e)
        {
            ReSetTargetPoint();
            MessageBox.Show("已处于编辑状态，起点不变，请移动鼠标操作。");
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

        //响应吸附半径变化
        private void toolStripTextBox2_TextChanged(object sender, EventArgs e)
        {
            if(toolStripTextBox2.Text!="")
            {
                snapBufferlength = Convert.ToDouble(toolStripTextBox2.Text)/100;
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
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ReportPrinter;
//using PluginUI.CallClass;

namespace DLGCheckLib.Frms
{
    public partial class FrmSearchCheckLines : Form
    {
        private IFeatureLayer pScaterFeatureLayer;
        private IFeatureLayer pMapBindingFeatureLayer;
        private IMap localMap;
        private AxMapControl localmapControl;
        private NewLineFeedback pLineFeedback;
        private IPoint snapVetex;

        public  CheckLineCollection localChecklines { set; get; }
        public DLGCheckProjectClass localCheckProject { set; get; }
        public SearchTargetSetting localSearchTarget { set; get; }
        public FrmSearchCheckLines(IFeatureLayer pScaterLayer, IFeatureLayer pBindingFeatureLayer,
            DLGCheckProjectClass GlobeCheckProject, SearchTargetSetting GlobeSearchtarget,IMap map,AxMapControl mapControl)
        {
            InitializeComponent();
            localCheckProject = GlobeCheckProject;
            localSearchTarget = GlobeSearchtarget;
            localMap =map;
            localmapControl = mapControl;

            pScaterFeatureLayer = pScaterLayer;
            pMapBindingFeatureLayer = pBindingFeatureLayer;

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
        }
        //搜索检测线
        private void SearchCheckLineButton_Click(object sender, EventArgs e)
        {
            //制作缓冲区
            MapSampleItemSetting samplesetting = localCheckProject.GetMapSampleItemSetting(MapNumberComboBox3.Text);
            //计算粗差限
            double baseV = samplesetting.PErrorMax * localCheckProject.MapScale / 1000;
            double ratio = samplesetting.CheckType == "同精度" ? 2 * 1.414 : 2;
            double plainBufferLength = baseV * ratio;
            double grossHeightDiff = samplesetting.HErrorMax * ratio;

            double heightBufferLength = samplesetting.ElevationNoteInterval / 3;

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

                foreach (DwgLayerInfoItem layerinfoitem in localSearchTarget.DwglayerinfoList)
                {
                    //遍历每个图层
                    IFeatureLayer pLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(layerinfoitem.LayerName,localMap);
                    PluginUI.ArcGISHelper.BufferSearchTarget(pLayer, pGeometry, plainBufferLength, layerinfoitem, bufferSearchResult);
                }
                //这个点累积选中多少个要素，需要统计一下
                //所有自动检测目标，均取值第一个，包括平面和高程
                bufferSearchResult = bufferSearchResult.OrderBy(ao => ao.Distance).ToList();
                if(bufferSearchResult.Count>0&& bufferSearchResult[0].PointType == CheckPointType.PlanCheck)
                {
                    item.PlanError = bufferSearchResult[0].Distance;
                    item.PointType = bufferSearchResult[0].PointType;
                    item.BufferSearchResults = bufferSearchResult;
                }
                else
                {
                    //没有选中，说明这个点不适于做平面检测点，可能划归到非检测点、超限平面检测点、高程检测点、控制点、定向点等类型
                    //进一步筛查是否属于高程点
                    IFeatureLayer pLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(localSearchTarget.ElevSearchsetting.LayerName, localMap);
                    DwgLayerInfoItem layerinfoitem = localSearchTarget.DwglayerinfoList.Find(ao=>ao.LayerName== localSearchTarget.ElevSearchsetting.LayerName&&ao.Layer== localSearchTarget.ElevSearchsetting.Layer);
                    PluginUI.ArcGISHelper.BufferSearchTarget(pLayer, pGeometry, heightBufferLength, layerinfoitem, bufferSearchResult, false,localSearchTarget.ElevSearchsetting.ElevationField);

                    bufferSearchResult = bufferSearchResult.OrderBy(ao => ao.Height).ToList();
                    if(bufferSearchResult.Count>0&& bufferSearchResult[0].PointType == CheckPointType.HeightCheck&& bufferSearchResult[0].Height <= grossHeightDiff)
                    {
                        item.PlanError = 0;
                        item.HeightError = bufferSearchResult[0].Height;
                        item.PointType = bufferSearchResult[0].PointType;
                        item.BufferSearchResults = bufferSearchResult;
                    }
                    else
                    {
                        item.PointType = CheckPointType.WaitToCheck;
                        item.PlanError = 0;
                        item.HeightError = 0;
                    }
                }
                if (item.BufferSearchResults == null)
                    item.BufferSearchResults = new List<BufferSearchTargetItem>();
                
            }
            MessageBox.Show("搜索平面检测线完成，请点击表格刷新结果。");

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
            localMap.MapScale = localCheckProject.MapScale/8;

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
                snapVetex = PluginUI.ArcGISHelper.GetNearestVertex(localmapControl.ActiveView, pt,mapSize,localSearchTarget);
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
                    foreach (DwgLayerInfoItem layerinfoitem in localSearchTarget.DwglayerinfoList)
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

                    menu.Show(localmapControl, pP);
                    //pLineFeedback = null;     
                }               
            }
        }
        //将吸附点设置为平面检测点
        private void StripSetSnapPlainCheckPoint(object sender, System.EventArgs e)
        {
            CheckLineItem item = localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            item.PointType = CheckPointType.PlanCheck;
            item.HeightError = 0;
            item.PlanError = 0;

            IPoint pt = new PointClass();
            pt.X = item.SX;
            pt.Y = item.SY;
            pt.Z = item.SZ;
            IProximityOperator ipo = snapVetex as IProximityOperator;
            double distance = ipo.ReturnDistance(pt);
            item.PlanError = distance;

            pLineFeedback = null;
        }

        //将吸附点设置为高程检测点
        private void StripSetHeightCheckPoint(object sender, System.EventArgs e)
        {
            CheckLineItem item = localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            item.PointType = CheckPointType.HeightCheck;
            item.HeightError =Convert.ToDouble(( sender  as ToolStripItem).Tag);
            item.PlanError = 0;

            pLineFeedback = null;
        }

        //将点设置为控制点
        private void StripSetControlPoint(object sender,System.EventArgs e)
        {
            CheckLineItem item = localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            item.PointType = CheckPointType.ControlPoint;
            item.HeightError = 0;
            item.PlanError = 0;
            
            pLineFeedback = null;
        }
        //将点设置为非检测点
        private void StripNonCheckPoint(object sender, System.EventArgs e)
        {
            CheckLineItem item = localChecklines.checklineList.ElementAtOrDefault(dataGridViewX1.CurrentRow.Index);
            item.PointType = CheckPointType.NonCheck;
            item.HeightError = 0;
            item.PlanError = 0;

            pLineFeedback = null;
        }
        private void FrmSearchCheckLines_FormClosed(object sender, FormClosedEventArgs e)
        {
            localmapControl.OnMouseMove -= axMapControl1_OnMouseMove;
            localmapControl.OnMouseDown -= axMapControl1_OnMouseDown;
        }

        //将检测线结果写入数据库
        private void SaveCheckLinestoolStripButton1_Click(object sender, EventArgs e)
        {
            //首先对检测线有效性进行检查，检测线有效性检查是指，检测线集合中没有WaitToCheck点
            bool validState = true;
            foreach (CheckLineItem item in localChecklines.checklineList)
            {
                if(item.PointType == CheckPointType.WaitToCheck)
                {
                    validState = false;
                }
            }
            if(validState == false)
            {
                MessageBox.Show("提醒：检测线没有完全判断完毕，会影响样本精度统计准确性，不建议提交数据库！本次操作不会阻止提交数据库，但可以待完成人工判定后重新提交数据库以覆盖。");
               // return;
            }
            //将检测线结果写入数据库
            localChecklines.Write(MapNumberComboBox3.Text,localCheckProject.currentuser);
            MessageBox.Show("检测线提交数据库完毕！");

            
            //显示更新统计精度
            string sMapNumber = MapNumberComboBox3.Text;
            //计算中误差

            
            PositionMeanError pme = new PositionMeanError(CallClass.Configs, CallClass.Databases);
            pme.QueryParameter(sMapNumber);
            pme.Calc(sMapNumber);
            pme.UpdateReslut(sMapNumber);

            HeightMeanError hme = new HeightMeanError(CallClass.Configs, CallClass.Databases);
            hme.QueryParameter(sMapNumber);
            hme.Calc(sMapNumber);
            hme.UpdateReslut(sMapNumber);
            
        }
    }
}

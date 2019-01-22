using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLGCheckLib
{
    public class AutoMatchScater
    {
        private IFeatureLayer pScaterFeatureLayer;
        private IFeatureLayer pMapBindingFeatureLayer;
        private IMap localMap;
        private AxMapControl localmapControl;
        private NewLineFeedback pLineFeedback;
        private IPoint snapVetex;
        private List<BufferSearchTargetItem> TempBufferSearchResult;
        public delegate void LoadDwgSample(string szMapnumber);
        public LoadDwgSample localLoadDwgSample { set; get; }
        public CheckLineCollection localChecklines { set; get; }
        public DLGCheckProjectClass localCheckProject { set; get; }
        public SearchTargetSetting localSearchTargetSetting { set; get; }

        int sampleareaidex_inpara = -1;
        int sampleserial_inpara = -1;
        string szMapnumber_inpara = "";
        //1、初始化一副样本自动化匹配散点类的参数
        public AutoMatchScater(DLGCheckProjectClass GlobeCheckProject, SearchTargetSetting GlobeSearchtargetSetting, AxMapControl mapControl, int SampleAreaIndex ,int SampleSerial,string Mapnumber)
        {
            sampleareaidex_inpara = SampleAreaIndex;
            sampleserial_inpara = SampleSerial;
            szMapnumber_inpara = Mapnumber;

            localCheckProject = GlobeCheckProject;
            localSearchTargetSetting = GlobeSearchtargetSetting;
            localMap = mapControl.Map;
            localmapControl = mapControl;

        }
        //2、加载样本文件
        public void LoadDwgFile()
        {
            //判断：加载样本有效
            if (PluginUI.ArcGISHelper.GetFeatureLayerByName("Point", localmapControl.Map) != null ||
                PluginUI.ArcGISHelper.GetFeatureLayerByName("Polyline", localmapControl.Map) != null)
            {
                //DialogResult dlgresult = MessageBox.Show("已经加载了样本！是否加载新样本？", "提示", MessageBoxButtons.YesNo);
                //if (dlgresult == DialogResult.Yes)
                {
                    IFeatureLayer layer = PluginUI.ArcGISHelper.GetFeatureLayerByName("Point", localmapControl.Map);
                    if (layer != null) localmapControl.Map.DeleteLayer(layer);
                    layer = PluginUI.ArcGISHelper.GetFeatureLayerByName("Polyline", localmapControl.Map);
                    if (layer != null) localmapControl.Map.DeleteLayer(layer);
                    layer = PluginUI.ArcGISHelper.GetFeatureLayerByName("Annotation", localmapControl.Map);
                    if (layer != null) localmapControl.Map.DeleteLayer(layer);

                }
                //else if (dlgresult == DialogResult.No)
                {
                //    return;
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
            if (localCheckProject.SampleFilePath != null)
            {
                filePath = localCheckProject.SampleFilePath;
                fileName = szMapnumber_inpara + ".dwg";
                strFullPath = filePath + "\\" + fileName;
            }
            //路径不存在的时候打开文件选择框进行选择
            if (File.Exists(strFullPath) == false)
            {
                //获取当前路径和文件名
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "CAD(*.dwg)|*.dwg|All Files(*.*)|*.*";
                dlg.Title = "Open CAD Data file";
                dlg.FileName = szMapnumber_inpara;
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
                localCheckProject.SampleFilePath = filePath;
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
                    localmapControl.Map.AddLayer(pFeatureLayer);
                }
                else//如果是点、线、面，则添加要素层
                {
                    pFeatureLayer = new FeatureLayerClass();
                    pFeatureLayer.Name = pFeatClass.AliasName;
                    // 暂不处理MultiPatch/Polygon图层
                    if (pFeatureLayer.Name == "MultiPatch" || pFeatureLayer.Name == "Polygon")
                        continue;

                    pFeatureLayer.FeatureClass = pFeatClass;
                    localSearchTargetSetting.RenderSearchTargetLayer(pFeatureLayer);
                    localmapControl.Map.AddLayer(pFeatureLayer);

                    currentViewBox = pFeatureLayer.AreaOfInterest;
                }
            }
            //ChangeProjectCoordSystem(GlobeProject.SrText);

            //缩放到结合表
            localmapControl.ActiveView.Extent = currentViewBox;
            localmapControl.ActiveView.Refresh();

            IFeatureLayer pScaterLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName("scater", localmapControl.Map);
            IFeatureLayer pBindingLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName("mapbindingtable", localmapControl.Map);

            pScaterFeatureLayer = pScaterLayer;
            pMapBindingFeatureLayer = pBindingLayer;

        }
        //3、加载检测线
        public void LoadCheckLines()
        {
            foreach (MapSampleItemSetting mset in localCheckProject.MapSampleSetting)
            {
                if (mset.SampleAreaIndex == sampleareaidex_inpara && mset.SampleSerial == sampleserial_inpara)
                {
                    //根据图号查出结合表范围线，空间查询离散点                    
                    IGeoFeatureLayer geoFeatureLayer = pMapBindingFeatureLayer as IGeoFeatureLayer;
                    IFeatureCursor pCursor = geoFeatureLayer.Search(null, false);

                    IFeature pMapBindFeature = null;
                    IFeature pFeature = pCursor.NextFeature();
                    while (pFeature != null)
                    {
                        int nIndex = pFeature.Fields.FindField("mapnumber");
                        string mapnumber = pFeature.Value[nIndex] as string;
                        if (szMapnumber_inpara == mapnumber)
                        {
                            pMapBindFeature = pFeature;
                            break;
                        }
                        pFeature = pCursor.NextFeature();
                    }

                    if (pMapBindFeature != null)
                    {
                        //LoadCheckLines(pScaterFeatureLayer, pFeature.Shape);
                        if (localChecklines == null)
                            localChecklines = new CheckLineCollection(localCheckProject.ProjectID);

                        localChecklines.Initialize(pScaterFeatureLayer, pMapBindFeature.Shape);

                    }
                }
            }

        }

        //4、开始自动匹配
        public void AutoMatch()
        {
            //样本图幅没有打点的，自动跳过
            if (localChecklines.checklineList.Count == 0)
                return;
            //如果这一幅样本的检测线存在，检查检测线个数与待检测点个数是否一致，不一致则是匹配过的，应保留原记录，自动跳过
            List<CheckLineItem> newList = localChecklines.checklineList.Where(ao => ao.PointType.Equals(CheckPointType.WaitToCheck)).ToList<CheckLineItem>();
            if (newList.Count< localChecklines.checklineList.Count)
                return;

            //如果是没有匹配过的，点数一致则进行如下匹配算法

            //制作缓冲区
            MapSampleItemSetting samplesetting = localCheckProject.GetMapSampleItemSetting(szMapnumber_inpara);
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

                foreach (DwgLayerInfoItem layerinfoitem in localSearchTargetSetting.DwglayerinfoList)
                {
                    //遍历每个图层
                    IFeatureLayer pLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(layerinfoitem.LayerName, localMap);
                    PluginUI.ArcGISHelper.BufferSearchTarget(pLayer, pGeometry, plainBufferLength, layerinfoitem, bufferSearchResult);
                }

                //这个点累积选中多少个要素，需要统计一下
                //所有自动检测目标，均取值第一个，包括平面和高程
                bufferSearchResult = bufferSearchResult.OrderBy(ao => ao.Distance).ToList();
                if (bufferSearchResult.Count > 0 && (bufferSearchResult[0].PointType == CheckPointType.PlanCheck || bufferSearchResult[0].PointType == CheckPointType.WaitToCheck))
                {
                    item.HeightError = bufferSearchResult[0].Height;
                    item.PlanError = bufferSearchResult[0].Distance;
                    item.PointType = bufferSearchResult[0].PointType;
                    item.BufferSearchResults = bufferSearchResult;
                }
                else
                {
                    //没有选中，说明这个点不适于做平面检测点，可能划归到非检测点、超限平面检测点、高程检测点、控制点、定向点等类型
                    //进一步筛查是否属于高程点
                    IFeatureLayer pLayer = PluginUI.ArcGISHelper.GetFeatureLayerByName(localSearchTargetSetting.ElevSearchsetting.LayerName, localMap);
                    DwgLayerInfoItem layerinfoitem = localSearchTargetSetting.DwglayerinfoList.Find(ao => ao.LayerName == localSearchTargetSetting.ElevSearchsetting.LayerName && ao.Layer == localSearchTargetSetting.ElevSearchsetting.Layer);
                    PluginUI.ArcGISHelper.BufferSearchTarget(pLayer, pGeometry, heightBufferLength, layerinfoitem, bufferSearchResult, false, localSearchTargetSetting.ElevSearchsetting.ElevationField);

                    bufferSearchResult = bufferSearchResult.OrderBy(ao => ao.Height).ToList();
                    if (bufferSearchResult.Count > 0 && bufferSearchResult[0].PointType == CheckPointType.HeightCheck && bufferSearchResult[0].Height <= grossHeightDiff)
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

            //配准结果写入数据库
            localChecklines.Write(szMapnumber_inpara, localCheckProject.currentuser);
        }
    }
}

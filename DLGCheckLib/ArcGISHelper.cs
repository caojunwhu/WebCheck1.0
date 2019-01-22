using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;
using OSGeo.OGR;
using System;
using System.Diagnostics;
using Utils;
using System.Windows.Forms;
using System.IO;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using DLGCheckLib;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.DataSourcesFile;
using System.Text;
using System.Data;
using DatabaseDesignPlus;

namespace PluginUI
{
    public static class ArcGISHelper
    {
        [DllImport("gdal111.dll", EntryPoint = "OGR_F_GetFieldAsString")]
        public extern static System.IntPtr OGR_F_GetFieldAsString(HandleRef handle, int i);
        public static void BufferSearchTarget(IFeatureLayer pLayer, IGeometry point, double bufferLength,
            DwgLayerInfoItem layerinfoitem, List<BufferSearchTargetItem> bufferSearchResult,bool bSnapState = false,string ElevationField="Elevation")
        {
            if (layerinfoitem.PlanSearch == false&&layerinfoitem.HeightSearch==false&&bSnapState==false)
                return;

            IGeometry pBufferGeometry = point;
            ITopologicalOperator pTopologicalOperator = pBufferGeometry as ITopologicalOperator;
            //增大缓冲区100倍，看查询效果，测试
            pBufferGeometry =  pTopologicalOperator.Buffer(bufferLength);//bufferLength是粗差限缓冲区
            pTopologicalOperator.Simplify();

            IFeatureClass pFeatureClass = pLayer.FeatureClass;
            IGeoFeatureLayer geoFeatureLayer = pLayer as IGeoFeatureLayer;
            ESRI.ArcGIS.Geodatabase.ISpatialFilter spatialFilter = new ESRI.ArcGIS.Geodatabase.SpatialFilterClass();
            spatialFilter.Geometry = pBufferGeometry;
            spatialFilter.GeometryField = "Shape";

            esriGeometryType geomtype = pBufferGeometry.GeometryType;

            //设置空间查询关系
            spatialFilter.SpatialRel = ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelIntersects;

            IFeatureCursor pCursor = geoFeatureLayer.Search(spatialFilter, false);

            IFeature pFeature = pCursor.NextFeature();
            while (pFeature != null)
            {
                int nLayerIndex = pFeature.Fields.FindField("Layer");
                string layerstring = pFeature.Value[nLayerIndex] as string;
                //为搜索对象，且是平面搜索对象，添加到目标列表里去，获取对象间距离
                if (layerinfoitem.Layer == layerstring &&( layerinfoitem.PlanSearch == true||layerinfoitem.HeightSearch == true))
                {
                    IProximityOperator ipo = point as IProximityOperator;
                    double distance = ipo.ReturnDistance(pFeature.Shape);
                    //搜索结果赋初始值
                    BufferSearchTargetItem item = new BufferSearchTargetItem();
                    if(bSnapState) item.Distance = distance;
                    item.Shape = pFeature.Shape;
                    item.pTargetPoint = null;
                    item.Layer = layerstring;

                    if(layerinfoitem.PlanSearch==true)
                    {
                        item.PointType = CheckPointType.PlanCheck;
                        item.Distance = distance;
                    }
                    else if(layerinfoitem.HeightSearch==true)
                    {
                        item.PointType = CheckPointType.HeightCheck;
                    }
                    
                    ////////////////////////将距离更新，选择Shape形状中节点到检测点最近的距离，将距离、节点、形状一起存储起来
                    switch(pFeature.Shape.GeometryType)
                    {
                        case esriGeometryType.esriGeometryPoint:
                            {
                                IPoint pTargetPoint = pFeature.Shape as IPoint;
                                item.pTargetPoint = pTargetPoint;
                                item.Shape = pTargetPoint;
                                item.Layer = layerstring;
                                //////处理高程值
                                if(item.PointType == CheckPointType.HeightCheck)
                                {
                                    int nElevationIndex = pFeature.Fields.FindField(ElevationField);
                                    double Elevation = Convert.ToDouble(pFeature.Value[nElevationIndex]);
                                    IPoint pt = point as IPoint;
                                    item.Height = bSnapState==true?Elevation:Math.Abs(pt.Z - Elevation);
                                }
                            }break;
                        case esriGeometryType.esriGeometryPolyline:
                            {
                                IPolyline pTargetPolyline = pFeature.Shape as IPolyline;
                                IPointCollection pPc = pFeature.Shape as IPointCollection;
                                List<BufferSearchTargetItem> poItems = new List<BufferSearchTargetItem>();
                                List<BufferSearchTargetItem> pValidItem = new List<BufferSearchTargetItem>();

                                for(int i=0;i<pPc.PointCount;i++)
                                {
                                    IPoint pt = pPc.Point[i];
                                    BufferSearchTargetItem pi = new BufferSearchTargetItem();
                                    IProximityOperator ip = pt as IProximityOperator;
                                    pi.Distance = ip.ReturnDistance(point);
                                    pi.pTargetPoint = pt;
                                    pi.Shape = pt;
                                    poItems.Add(pi);

                                    if (pi.Distance <= bufferLength)
                                        pValidItem.Add(pi);
                                }
                                pValidItem = pValidItem.OrderBy(ao => ao.Distance).ToList();
                                //更新item
                                //1、初值选择：选取第一个Distance小于bufferlength的点，赋值给搜索目标对象
                                foreach(BufferSearchTargetItem po in pValidItem)
                                {
                                    if(po.Distance<=bufferLength)
                                    {
                                        item.Distance = po.Distance;
                                        item.pTargetPoint = po.pTargetPoint;
                                        item.Shape = pTargetPolyline;
                                        item.Layer = layerstring;
                                        break;
                                    }
                                }
                                //2、判断是否是悬挂点、接边点，如果前或后无点，说明是端头，应该舍弃
                                //判断方法：判断此点是否是最后一个点，这个点与起点距离是否为0，如果为端，且距离不是0，需舍弃
                                //poItems 未排序，0号为起点，count-1号为终点
                                if(poItems.Count>=2&&item.pTargetPoint!=null)
                                {
                                    BufferSearchTargetItem pThis = poItems.Find(p => p.Distance == item.Distance);
                                    int index = poItems.IndexOf(pThis);
                                    //如果搜索目标是线段起点或终点，且终点始点不重合
                                    int index2 = -1;
                                    if (index==poItems.Count-1||index==0)
                                    {
                                        index2 = poItems.Count - 1 - index;
                                        BufferSearchTargetItem pThat = poItems[index2];
                                        IProximityOperator pd = pThis.Shape as IProximityOperator;
                                        double dist = pd.ReturnDistance(pThat.Shape);
                                        if (dist != 0)
                                        {
                                        //    item.pTargetPoint = null;
                                        }
                                    }
                                }
                                //3、用角度决定是否为搜索对象，实测点搜索的目标点，它前后是否有点，且前后点夹角是否在限值范围内，
                                //，如果前后夹角大于限差值则认为不是角点

                                //4、目标搜索失败
                                //如果没有找到距离小于bufferlength的点，则目标图形设置为空值
                                if(item.pTargetPoint==null)
                                {
                                    item.PointType = CheckPointType.WaitToCheck;
                                    item.Layer = "";
                                    item.Distance = 0;
                                    item.Height = 0;
                                    item.Shape = null;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    bufferSearchResult.Add(item);
                }
                pFeature = pCursor.NextFeature();
            }

        }

        //获取最近的结点,然后在  OnMouseMove中显示
        //pnt：鼠标移动点
        //mapSize:设置的地理范围
        //搜索配置

        public static IPoint GetNearestVertex(IActiveView actview, IPoint pnt, double mapSize, SearchTargetSetting sTarget)
        {
            IPoint vetex = null;
            IPoint hitPnt = new PointClass();
            IHitTest hitTest = null;
            IPointCollection pntColl = new MultipointClass();
            IProximityOperator prox = null;
            double hitdis = 0;
            int hitpartindex = 0, hitsegindex = 0;
            Boolean rside = false;
            IFeatureCache2 featCache = new FeatureCacheClass();
            double pixelSize = ConvertMapUnitsToPixels(actview, mapSize);  //将地理范围转化为像素
            featCache.Initialize(pnt, pixelSize);  //初始化缓存
            for (int i = 0; i < actview.FocusMap.LayerCount; i++)
            {
                //只有点、线、面并且可视的图层才加入缓存
                //searchTarget中出配置的图层才可以
                ILayer player= actview.FocusMap.get_Layer(i);

                IFeatureLayer featLayer = null;

                try
                {
                    featLayer = (IFeatureLayer)player;
                }
                catch
                {
                }

                //非矢量图层需要忽略
                if (featLayer == null)
                    continue;

                var layerInfos = sTarget.DwglayerinfoList.Where(p => p.LayerName == featLayer.Name);
                if(featLayer != null&&featLayer.Visible == true&&layerInfos.Count() > 0 )
                {
                    featCache.AddFeatures(featLayer.FeatureClass, null);
                    for (int j = 0; j < featCache.Count; j++)
                    {
                        IFeature feat = featCache.get_Feature(j);
                        hitTest = (IHitTest)feat.Shape;

                        //捕捉节点，另外可以设置esriGeometryHitPartType，捕捉边线点，中间点等。
                        if (hitTest.HitTest(pnt, mapSize, esriGeometryHitPartType.esriGeometryPartVertex, hitPnt, ref hitdis, ref hitpartindex, ref hitsegindex, ref rside))
                        {
                            object obj = Type.Missing;
                            pntColl.AddPoint(hitPnt, ref obj, ref obj);
                            break;
                        }
                    }
                }
            }
            prox = (IProximityOperator)pnt;
            double minDis = 0, dis = 0;
            for (int i = 0; i < pntColl.PointCount; i++)
            {
                IPoint tmpPnt = pntColl.get_Point(i);
                dis = prox.ReturnDistance(tmpPnt);
                if (i == 0)
                {
                    minDis = dis;
                    vetex = tmpPnt;
                }
                else
                {
                    if (dis < minDis)
                    {
                        minDis = dis;
                        vetex = tmpPnt;
                    }
                }
            }
            return vetex;
        }
        public  static double ConvertPixelsToMapUnits(IActiveView pActiveView, double pixelUnits)
        {
            // Uses the ratio of the size of the map in pixels to map units to do the conversion  
            IPoint p1 = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.UpperLeft;
            IPoint p2 = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.UpperRight;
            int x1, x2, y1, y2;
            pActiveView.ScreenDisplay.DisplayTransformation.FromMapPoint(p1, out x1, out y1);
            pActiveView.ScreenDisplay.DisplayTransformation.FromMapPoint(p2, out x2, out y2);
            double pixelExtent = x2 - x1;
            double realWorldDisplayExtent = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
            double sizeOfOnePixel = realWorldDisplayExtent / pixelExtent;
            return pixelUnits * sizeOfOnePixel;
        }
        /// <summary>

        /// 地图距离转屏幕像素数,不同缩放比例尺下同样距离对应像素数不同的，有特殊需要时设置sMapBLC

        /// </summary>
        /// <param name="pAV">The p AV.</param>
        /// <param name="dMapUnits">地图距离</param>
        /// <returns></returns>
        public static double ConvertMapUnitsToPixels(IActiveView pAV, double dMapUnits)
        {
            IDisplayTransformation pDT = pAV.ScreenDisplay.DisplayTransformation;
            tagRECT pDeviceFrame = pDT.get_DeviceFrame();
            int iDeviceLeft = pDeviceFrame.left;
            int iDeviceRight = pDeviceFrame.right;
            int iPixelExtent = iDeviceRight - iDeviceLeft;
            double dRealWorldExtent = pAV.Extent.Width;
            double dSizeOfEachPixel = dRealWorldExtent / iPixelExtent;
            return dMapUnits / dSizeOfEachPixel;
        }
        public static void ExportFeature(IFeatureClass pInFeatureClass, string pPath)
        {
            // create a new Access workspace factory       
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
            //string parentPath = pPath.Substring(0, pPath.LastIndexOf('\\'));
            string fileName = pPath.Substring(pPath.LastIndexOf('\\') + 1, pPath.Length - pPath.LastIndexOf('\\') - 1);
            //IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create(parentPath, fileName, null, 0);
            IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create(pPath, "Output", null, 0);
            // Cast for IName       
            IName name = (IName)pWorkspaceName;
            //Open a reference to the access workspace through the name object       
            IWorkspace pOutWorkspace = (IWorkspace)name.Open();

            IDataset pInDataset = pInFeatureClass as IDataset;
            IFeatureClassName pInFCName = pInDataset.FullName as IFeatureClassName;
            IWorkspace pInWorkspace = pInDataset.Workspace;
            IDataset pOutDataset = pOutWorkspace as IDataset;
            IWorkspaceName pOutWorkspaceName = pOutDataset.FullName as IWorkspaceName;
            IFeatureClassName pOutFCName = new FeatureClassNameClass();
            IDatasetName pDatasetName = pOutFCName as IDatasetName;
            pDatasetName.WorkspaceName = pOutWorkspaceName;
            pDatasetName.Name = pInFeatureClass.AliasName;
            IFieldChecker pFieldChecker = new FieldCheckerClass();
            pFieldChecker.InputWorkspace = pInWorkspace;
            pFieldChecker.ValidateWorkspace = pOutWorkspace;
            IFields pFields = pInFeatureClass.Fields;
            IFields pOutFields;
            IEnumFieldError pEnumFieldError;
            pFieldChecker.Validate(pFields, out pEnumFieldError, out pOutFields);
            IFeatureDataConverter pFeatureDataConverter = new FeatureDataConverterClass();
            pFeatureDataConverter.ConvertFeatureClass(pInFCName, null, null, pOutFCName, null, pOutFields, "", 100, 0);
        }
        public static IFeature GetFeature(IFeatureLayer pLayer,string field,string fieldVule)
        {
            IFeature pRetFeature = null;

            IFeatureClass pFeatureClass = pLayer.FeatureClass;
            IGeoFeatureLayer geoFeatureLayer = pLayer as IGeoFeatureLayer;

            IFeatureCursor pCursor = geoFeatureLayer.Search(null, false);

            IFeature pFeature = pCursor.NextFeature();
            while (pFeature != null)
            {
                int nIndex = pFeature.Fields.FindField(field);
                string fieldvaluestring = pFeature.Value[nIndex] as string;
                if (fieldvaluestring == fieldVule)
                {
                    pRetFeature = pFeature;
                    break;
                }
                pFeature = pCursor.NextFeature();
            }
            return pRetFeature;

        }

        public static IFeature GetFeature(IFeatureClass pClass, string field, string fieldVule)
        {
            if (pClass == null) return null;

            IFeature pRetFeature = null;

            IFeatureClass pFeatureClass = pClass;
            IFeatureCursor pCursor = pFeatureClass.Search(null, false);

            IFeature pFeature = pCursor.NextFeature();
            while (pFeature != null)
            {
                int nIndex = pFeature.Fields.FindField(field);
                string fieldvaluestring = Convert.ToString(pFeature.Value[nIndex]);
                if (fieldvaluestring == fieldVule)
                {
                    pRetFeature = pFeature;
                    break;
                }
                pFeature = pCursor.NextFeature();
            }
            return pRetFeature;

        }

        public static IFeatureLayer GetFeatureLayerByName(string LayerName, ESRI.ArcGIS.Carto.IMap map)
        {
            int nLayerCount = map.LayerCount;
            IFeatureLayer pFeatureLayer = null;
            for (int i = 0; i < nLayerCount; i++)
            {
                string LocalLayerName = map.Layer[i].Name;
                if (LocalLayerName == LayerName)
                {
                    return pFeatureLayer = map.Layer[i] as IFeatureLayer;
                }
            }
            return pFeatureLayer;
        }
        public static void EnableFeatureLayerLabel(IFeatureLayer pFeaturelayer, string sLableField, IRgbColor pRGB, int size, string angleField)
        {
            //判断图层是否为空
            if (pFeaturelayer == null)
                return;
            IGeoFeatureLayer pGeoFeaturelayer = (IGeoFeatureLayer)pFeaturelayer;
            IAnnotateLayerPropertiesCollection pAnnoLayerPropsCollection;
            pAnnoLayerPropsCollection = pGeoFeaturelayer.AnnotationProperties;
            pAnnoLayerPropsCollection.Clear();

            //stdole.IFontDisp  pFont; //字体
            ITextSymbol pTextSymbol;

            //pFont.Name = "新宋体";
            //pFont.Size = 9;
            //未指定字体颜色则默认为黑色
            if (pRGB == null)
            {
                pRGB = new RgbColorClass();
                pRGB.Red = 0;
                pRGB.Green = 0;
                pRGB.Blue = 0;
            }

            pTextSymbol = new TextSymbolClass();
            pTextSymbol.Color = (IColor)pRGB;
            pTextSymbol.Size = size; //标注大小

            IBasicOverposterLayerProperties4 pBasicOverposterlayerProps4 = new BasicOverposterLayerPropertiesClass();
            switch (pFeaturelayer.FeatureClass.ShapeType)//判断图层类型
            {
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                    pBasicOverposterlayerProps4.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolygon;
                    break;
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                    pBasicOverposterlayerProps4.FeatureType = esriBasicOverposterFeatureType.esriOverposterPoint;
                    break;
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                    pBasicOverposterlayerProps4.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolyline;
                    break;
            }
            pBasicOverposterlayerProps4.PointPlacementMethod = esriOverposterPointPlacementMethod.esriRotationField;
            pBasicOverposterlayerProps4.RotationField = angleField;

            ILabelEngineLayerProperties pLabelEnginelayerProps = new LabelEngineLayerPropertiesClass();
            pLabelEnginelayerProps.Expression = "[" + sLableField + "]";
            pLabelEnginelayerProps.Symbol = pTextSymbol;
            pLabelEnginelayerProps.BasicOverposterLayerProperties = pBasicOverposterlayerProps4 as IBasicOverposterLayerProperties;
            pAnnoLayerPropsCollection.Add((IAnnotateLayerProperties)pLabelEnginelayerProps);
            pGeoFeaturelayer.DisplayAnnotation = true;//很重要，必须设置 
                                                      //axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null); }
        }
        public static IStyleGalleryItem ReadStyleServer(string styleName, IStyleGallery tStyleGallery, string stylefilePath)
        {
            IEnumStyleGalleryItem tStyleGalleryItems = tStyleGallery.get_Items("Marker Symbols", stylefilePath, "Default");

            IStyleGalleryItem styleitem = null;
            tStyleGalleryItems.Reset();
            IStyleGalleryItem tStyleGalleryItem = tStyleGalleryItems.Next();
            try
            {
                while (tStyleGalleryItem != null)
                {
                    string tName = tStyleGalleryItem.Name;

                    if (tName == styleName)
                    {
                        styleitem =  tStyleGalleryItem;
                        break;
                    }                     

                    tStyleGalleryItem = tStyleGalleryItems.Next();
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                string tErrorMessage = ex.Message +
                ex.ErrorCode;
            }
            return styleitem;
        }

        private static void ReleaseCom(object o)
        {
            while (System.Runtime.InteropServices.Marshal.ReleaseComObject(o) > 0)
            {
                
            }
        }
        public static ESRI.ArcGIS.Geometry.esriGeometryType GetesriGeometryType(OSGeo.OGR.wkbGeometryType geometrytype)
        {
            esriGeometryType esrigeomtype = esriGeometryType.esriGeometryAny;
            switch(geometrytype)
            {
                case OSGeo.OGR.wkbGeometryType.wkbPoint25D:
                case OSGeo.OGR.wkbGeometryType.wkbPoint:
                    esrigeomtype = esriGeometryType.esriGeometryPoint;
                    break;
                case OSGeo.OGR.wkbGeometryType.wkbMultiPoint25D :
                case OSGeo.OGR.wkbGeometryType.wkbMultiPoint:
                    esrigeomtype = esriGeometryType.esriGeometryMultipoint;
                    break;
                case OSGeo.OGR.wkbGeometryType.wkbLineString25D :
                case OSGeo.OGR.wkbGeometryType.wkbLineString:                    
                    esrigeomtype = esriGeometryType.esriGeometryPolyline;
                    break;
                case OSGeo.OGR.wkbGeometryType.wkbMultiLineString25D:
                case OSGeo.OGR.wkbGeometryType.wkbMultiLineString:
                    esrigeomtype = esriGeometryType.esriGeometryPolyline;
                    break;
                case OSGeo.OGR.wkbGeometryType.wkbMultiPolygon25D :
                case OSGeo.OGR.wkbGeometryType.wkbMultiPolygon:
                case OSGeo.OGR.wkbGeometryType.wkbPolygon:
                    esrigeomtype = esriGeometryType.esriGeometryPolygon;
                    break;// no mapping to single polygon

            }

            return esrigeomtype;
        }
        public static ISpatialReference GetesriISpatialReference(OSGeo.OSR.SpatialReference spatialreference,string srText)
        {

            //ISpatialReference ispatialref = null;
            ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference spatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
            //string prjstr = "";

            //////////////////////调试代码
            if(spatialreference==null)
            {
                //prjstr = "GEOGCS[\"China Geodetic Coordinate System 2000\",DATUM[\"D_CGCS_2000\",SPHEROID[\"CGCS_2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]";
                spatialreference =new  OSGeo.OSR.SpatialReference(srText);

                spatialreference.ExportToWkt(out srText);
            }

            //int prjint = 0;
            //prjint = spatialreference.EPSGTreatsAsNorthingEasting();
            //spatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem(prjint);
            //spatialReference = spatialReferenceFactory.CreateESRISpatialReferenceFromPRJ(prjstr);

            string pszProjection = null;
            if (spatialreference.IsGeographic() == 0)
            {
                pszProjection = spatialreference.GetAttrValue("PROJCS", 0);
                pszProjection = pszProjection.Replace("CGCS2000", "CGCS 2000");
                pszProjection = pszProjection.Replace("Gauss-Kruger", "GK");
                pszProjection = pszProjection.Replace("/ ", "");
                pszProjection = pszProjection.Replace("-", " ");
                pszProjection = pszProjection.Replace("zone", "Zone");
                pszProjection = pszProjection.Replace("\\", "");
            }
            string pszGeodetic = spatialreference.GetAttrValue("GEOGCS", 0);
            pszGeodetic = pszGeodetic.Replace("\\", "");

            string spatialrefname = spatialreference.IsGeographic() == 0 ? pszProjection : pszGeodetic;
            spatialrefname = spatialrefname.Replace("\\", "");
            string coorsysfiledir = Directory.GetCurrentDirectory() + "\\coordinate systems\\";
            coorsysfiledir = spatialreference.IsGeographic() == 0 ? coorsysfiledir + "Projected Coordinate Systems" : coorsysfiledir + "\\Geographic Coordinate Systems";
            string coorsysfilepath = "";

            if (spatialreference.IsGeographic() == 0)
            {
                coorsysfilepath = coorsysfiledir + "\\" + pszGeodetic + "\\" + spatialrefname + ".prj";
            }
            else
            {
                coorsysfilepath = coorsysfiledir + "\\" + spatialrefname + ".prj";
            }

            spatialReference = spatialReferenceFactory.CreateESRISpatialReferenceFromPRJFile(coorsysfilepath);

            return spatialReference;
        }
        public static IFields MapFields(DataTable datatable, OSGeo.OSR.SpatialReference spatialreference, OSGeo.OGR.wkbGeometryType geometrytype, string srText)
        {
            IField field = new FieldClass();
            IFieldEdit fieldEdit = field as IFieldEdit;
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = fields as IFieldsEdit;

            fieldEdit.Name_2 = "OBJECTID";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldEdit.IsNullable_2 = false;
            fieldEdit.Required_2 = false;
            fieldsEdit.AddField(field);

            field = new FieldClass();
            fieldEdit = field as IFieldEdit;
            IGeometryDef geoDef = new GeometryDefClass();
            IGeometryDefEdit geoDefEdit = (IGeometryDefEdit)geoDef;
            geoDefEdit.AvgNumPoints_2 = 5;
            geoDefEdit.GeometryType_2 = GetesriGeometryType(geometrytype);
            geoDefEdit.GridCount_2 = 1;
            geoDefEdit.HasM_2 = false;
            geoDefEdit.HasZ_2 = false;
            geoDefEdit.SpatialReference_2 = GetesriISpatialReference(spatialreference, srText);
            fieldEdit.Name_2 = "SHAPE";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.GeometryDef_2 = geoDef;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Required_2 = true;
            fieldsEdit.AddField(field);


            for (int i = 0; i < datatable.Columns.Count; i++)//(DataColumn col in fr.Table.Columns)
            {
                if (datatable.Columns[i].ColumnName == "shape") continue;

                //string FieldTypeName = fielddef.GetFieldTypeName(coltype);
                field = new FieldClass();
                fieldEdit = field as IFieldEdit;
                fieldEdit.Name_2 = datatable.Columns[i].ColumnName;
                fieldEdit.IsNullable_2 = datatable.Columns[i].AllowDBNull;

                switch (datatable.Columns[i].DataType.ToString())
                {
                    case "System.Double":
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        fieldEdit.Length_2 = datatable.Columns[i].MaxLength;
                        break;
                    case "System.String":
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                        break;
                    default:
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                        break;
                }
                fieldsEdit.AddField(field);
            }
            return fields;
        }
        public static IFields MapFields(OSGeo.OGR.FeatureDefn featuredef, OSGeo.OSR.SpatialReference spatialreference, OSGeo.OGR.wkbGeometryType geometrytype,string srText)
        {
            IField field = new FieldClass();
            IFieldEdit fieldEdit = field as IFieldEdit;
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = fields as IFieldsEdit;

            string fieldName = "ogc_id";
            fieldEdit.Name_2 = "OBJECTID";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldEdit.IsNullable_2 = false;
            fieldEdit.Required_2 = false;
            fieldsEdit.AddField(field);

            fieldName = "wkb_geometry";
            field = new FieldClass();
            fieldEdit = field as IFieldEdit;
            IGeometryDef geoDef = new GeometryDefClass();
            IGeometryDefEdit geoDefEdit = (IGeometryDefEdit)geoDef;
            geoDefEdit.AvgNumPoints_2 = 5;
            geoDefEdit.GeometryType_2 = GetesriGeometryType(geometrytype);
            geoDefEdit.GridCount_2 = 1;
            geoDefEdit.HasM_2 = false;
            geoDefEdit.HasZ_2 = false;
            geoDefEdit.SpatialReference_2 = GetesriISpatialReference(spatialreference,srText);
            fieldEdit.Name_2 = "SHAPE";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.GeometryDef_2 = geoDef;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Required_2 = true;
            fieldsEdit.AddField(field);


            for (int i = 0; i < featuredef.GetFieldCount(); i++)//(DataColumn col in fr.Table.Columns)
            {
                OSGeo.OGR.FieldDefn fielddef = featuredef.GetFieldDefn(i);
                fieldName = fielddef.GetName();

                OSGeo.OGR.FieldType coltype = fielddef.GetFieldType();
                //string FieldTypeName = fielddef.GetFieldTypeName(coltype);
                field = new FieldClass();
                fieldEdit = field as IFieldEdit;
                fieldEdit.Name_2 = fielddef.GetName();
                fieldEdit.IsNullable_2 = true;

                switch (coltype)
                {
                    case OSGeo.OGR.FieldType.OFTInteger:
                    case OSGeo.OGR.FieldType.OFTIntegerList:
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;

                        break;
                    case OSGeo.OGR.FieldType.OFTReal:
                    case OSGeo.OGR.FieldType.OFTRealList:
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        fieldEdit.Length_2 = fielddef.GetWidth();
                        fieldEdit.Precision_2 = fielddef.GetPrecision();
                        //fieldEdit.Scale = fielddef.ge
                        break;
                    case OSGeo.OGR.FieldType.OFTString:
                    case OSGeo.OGR.FieldType.OFTStringList:
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;

                        break;
                    case OSGeo.OGR.FieldType.OFTWideString:
                    case OSGeo.OGR.FieldType.OFTWideStringList:
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;

                        break;
                    case OSGeo.OGR.FieldType.OFTBinary:
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeBlob;

                        break;
                    case OSGeo.OGR.FieldType.OFTDate:
                    case OSGeo.OGR.FieldType.OFTTime:
                    case OSGeo.OGR.FieldType.OFTDateTime:
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDate;
                        break;

                    default:
                        break;                       
                }
                fieldsEdit.AddField(field);
            }
            return fields;
        }

        public static IFeature OgrFeatureToesriFeature(IFeature  esriFeature,OSGeo.OGR.Feature ogrFeature)
        {
            Int32 fdrIndex = 0;
            for (int iField = 0; iField < ogrFeature.GetFieldCount(); iField++)
            {
                if (!ogrFeature.IsFieldSet(iField)) {  continue; }
                string ogrFieldName = ogrFeature.GetFieldDefnRef(iField).GetName();
                fdrIndex = esriFeature.Fields.FindField(ogrFieldName);
                try
                {
                    switch (ogrFeature.GetFieldType(iField))
                    {
                        case OSGeo.OGR.FieldType.OFTString:
                        case OSGeo.OGR.FieldType.OFTWideString:
                            string strValue = ogrFeature.GetFieldAsString(iField);
                            esriFeature.Value[fdrIndex] = strValue;

                           /* IntPtr pchar = OGR_F_GetFieldAsString(OSGeo.OGR.Feature.getCPtr(ogrFeature), iField);
                            string strValue = Marshal.PtrToStringAnsi(pchar);
                            //string s = "";

                            byte[] buffer = Encoding.Default.GetBytes(strValue);
                            string Text = Encoding.UTF8.GetString(buffer);

                            string utf8String = strValue;// "骞垮憡涓戦椈";
                            // Create two different encodings.
                            Encoding utf8 = Encoding.UTF8;
                            Encoding defaultCode = Encoding.Default ;

                            // Convert the string into a byte[].
                            byte[] utf8Bytes = defaultCode.GetBytes(utf8String);
                            //byte[] utf8Bytes = utf8.GetBytes(utf8String);
                            // Perform the conversion from one encoding to the other.
                            byte[] defaultBytes = Encoding.Convert( utf8,defaultCode, utf8Bytes);

                            // Convert the new byte[] into a char[] and then into a string.
                            // This is a slightly different approach to converting to illustrate
                            // the use of GetCharCount/GetChars.
                            char[] defaultChars = new char[defaultCode.GetCharCount(defaultBytes, 0, defaultBytes.Length)];
                            defaultCode.GetChars(defaultBytes, 0, defaultBytes.Length, defaultChars, 0);
                            string defaultString = new string(defaultChars);

                            //byte[] buffer = Encoding.GetEncoding("GBK ").GetBytes(pchar);
                            //byte[] bUtf8 = System.Text.Encoding.Convert(Encoding.GetEncoding("GBK"), Encoding.UTF8, buffer);
                            //Text = Encoding.UTF8.GetString(bUtf8);                        

                            //增加一个开关，负责对乱码??进行转换
                            //defaultString = defaultString.Replace("??", "-");
                            esriFeature.Value[fdrIndex] = defaultString;*/

                            break;
                        case OSGeo.OGR.FieldType.OFTStringList:
                        case OSGeo.OGR.FieldType.OFTWideStringList:
                            break;
                        case OSGeo.OGR.FieldType.OFTInteger:
                            esriFeature.Value[fdrIndex] = ogrFeature.GetFieldAsInteger(iField);
                            break;
                        case OSGeo.OGR.FieldType.OFTIntegerList:
                            break;
                        case OSGeo.OGR.FieldType.OFTReal:
                            esriFeature.Value[fdrIndex] = ogrFeature.GetFieldAsDouble(iField);
                            break;
                        case OSGeo.OGR.FieldType.OFTRealList:
                            break;
                        case OSGeo.OGR.FieldType.OFTDate:
                        case OSGeo.OGR.FieldType.OFTDateTime:
                        case OSGeo.OGR.FieldType.OFTTime:
                            Int32 y, m, d, h, mi, s, tz;
                            ogrFeature.GetFieldAsDateTime(iField, out y, out m, out d, out h, out mi, out s, out tz);
                            try
                            {
                                if (y == 0 && m == 0 && d == 0)
                                    esriFeature.Value[fdrIndex] = DateTime.MinValue.AddMinutes(h * 60 + mi);
                                else
                                    esriFeature.Value[fdrIndex] = new DateTime(y, m, d, h, mi, s);
                            }
                            catch { }
                            break;
                        default:
                            Debug.WriteLine(string.Format("Cannot handle Ogr DataType '{0}'", ogrFeature.GetFieldType(iField)));
                            break;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            esriFeature.Shape = ParseOgrGeometry(ogrFeature.GetGeometryRef());
            esriFeature.Store();

            return esriFeature;
        }
        public static IGeometry ParseOgrGeometry(Geometry ogrGeometry)
        {
            if (ogrGeometry != null)
            {
                //Just in case it isn't 2D
                ogrGeometry.FlattenTo2D();
                byte[] wkbBuffer = new byte[ogrGeometry.WkbSize()];
                ogrGeometry.ExportToWkb(wkbBuffer);
                IGeometry geom = Converters.ConvertWKBToGeometry(wkbBuffer);
                if (geom == null)
                    Debug.WriteLine(string.Format("Failed to parse '{0}'", ogrGeometry.GetGeometryType()));
                return geom;
            }
            return null;
        }

        //将线图层转换为面图层，调用GP工具
        public static IFeatureLayer TransvertToPolygon(IFeatureLayer polyline)
        {
            IFeatureLayer polygon = null;
            string layername = "Polygon";
            Geoprocessor GP = new Geoprocessor();
            ESRI.ArcGIS.DataManagementTools.FeatureToPolygon feature2polygon = new ESRI.ArcGIS.DataManagementTools.FeatureToPolygon();

            ESRI.ArcGIS.Geodatabase.IWorkspaceFactory pWSF = new ESRI.ArcGIS.DataSourcesGDB.InMemoryWorkspaceFactoryClass();
            IWorkspaceName pWSName = pWSF.Create(null, "MyWorkspace", null, 0);
            IName pName = (IName)pWSName;

            IWorkspace memoryWS = (IWorkspace)pName.Open();
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)memoryWS;
            // 为图层添加字段信息
            GP.OverwriteOutput = true;
            GP.SetEnvironmentValue("workspace", memoryWS.PathName);
            feature2polygon.in_features = @"C:\Users\cjun\Desktop\1\1.shp";// polyline;
            //feature2polygon.in_features = polyline;
            //feature2polygon.out_feature_class = memoryWS.PathName+"\\"+layername;
            feature2polygon.out_feature_class = @"C:\Users\cjun\Desktop\1\Polygon.shp";
            feature2polygon.attributes = "ATTRIBUTES";
            feature2polygon.cluster_tolerance = 0.01;
            feature2polygon.label_features = "";

            GP.Execute(feature2polygon, null);
            //RunTool(GP, feature2polygon, null);

            IFeatureClass featureClass = null;
            featureClass = featureWorkspace.OpenFeatureClass(layername);

            return polygon;
        }
        private static void RunTool(Geoprocessor geoprocessor, IGPProcess process, ITrackCancel TC)
        {
            try
            {
                geoprocessor.Execute(process, null);
                ReturnMessages(geoprocessor);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                ReturnMessages(geoprocessor);
            }
        }

        // Function for returning the tool messages.
        private static void ReturnMessages(Geoprocessor gp)
        {
            string ms = "";
            if (gp.MessageCount > 0)
            {
                for (int Count = 0; Count <= gp.MessageCount - 1; Count++)
                {
                    ms += gp.GetMessage(Count);
                }
            }
        }
        public static IFeatureClass CreateMemoryFeatureFromGeometry(string layername, string srText)
        {
            IFeatureClass featureClass = null;
            ESRI.ArcGIS.Geodatabase.IWorkspaceFactory pWSF = new ESRI.ArcGIS.DataSourcesGDB.InMemoryWorkspaceFactoryClass();
            IWorkspaceName pWSName = pWSF.Create(null, "MyGPSWorkspace", null, 0);
            IName pName = (IName)pWSName;

            IWorkspace memoryWS = (IWorkspace)pName.Open();

            // 为图层添加字段信息
            IField field = new FieldClass();
            IFieldEdit fieldEdit = field as IFieldEdit;
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = fields as IFieldsEdit;

            fieldEdit.Name_2 = "OBJECTID";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldEdit.IsNullable_2 = false;
            fieldEdit.Required_2 = false;
            fieldsEdit.AddField(field);

            field = new FieldClass();
            fieldEdit = field as IFieldEdit;
            IGeometryDef geoDef = new GeometryDefClass();
            IGeometryDefEdit geoDefEdit = (IGeometryDefEdit)geoDef;
            geoDefEdit.AvgNumPoints_2 = 1;
            geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
            geoDefEdit.GridCount_2 = 1;
            geoDefEdit.HasM_2 = false;
            geoDefEdit.HasZ_2 = false;

            OSGeo.OSR.SpatialReference spatialreference = new OSGeo.OSR.SpatialReference(srText);

            ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference wgs84spatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);

            geoDefEdit.SpatialReference_2 = GetesriISpatialReference(spatialreference, srText); ;
            fieldEdit.Name_2 = "SHAPE";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.GeometryDef_2 = geoDef;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Required_2 = true;
            fieldsEdit.AddField(field);

            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)memoryWS;
            featureClass = featureWorkspace.CreateFeatureClass(layername, fields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");

            return featureClass;
        }
        public static void DeleteOGRFeature(string gisdbconnection, string layername, OSGeo.OGR.Feature pFeature)
        {
            OSGeo.OGR.Ogr.RegisterAll();
            OSGeo.OGR.Driver pgDriver = OSGeo.OGR.Ogr.GetDriverByName("PostgreSQL");
            OSGeo.OGR.DataSource pgDatasource = null;
            OSGeo.OGR.Layer ogrpgLayer = null;

            string pgds = DataBaseConfigs.GetGISDbDatasource(DataBaseConfigs.DatabaseEngineType);
            pgds = string.Format("PG:{0}", pgds);
            pgDatasource = pgDriver.Open(pgds, 1);
            ogrpgLayer = pgDatasource.GetLayerByName(layername);
            ogrpgLayer.DeleteFeature(pFeature.GetFID());
        }
        public static void CreateOGRFeature(string gisdbconnection, string layername, OSGeo.OGR.Feature pFeatureInsert)
        {
            OSGeo.OGR.Ogr.RegisterAll();
            OSGeo.OGR.Driver pgDriver = OSGeo.OGR.Ogr.GetDriverByName("PostgreSQL");
            OSGeo.OGR.DataSource pgDatasource = null;
            OSGeo.OGR.Layer ogrpgLayer = null;

            string pgds = DataBaseConfigs.GetGISDbDatasource(DataBaseConfigs.DatabaseEngineType);
            pgds = string.Format("PG:{0}", pgds);
            pgDatasource = pgDriver.Open(pgds, 1);
            ogrpgLayer = pgDatasource.GetLayerByName(layername);
            bool tobeOK = false;
            int newFeatureid;
            int index = 1;
            while (!tobeOK)
            {
                try
                {
                    //newFeatureid = ogrpgLayer.GetFeatureCount(1) + index;
                    //pFeatureInsert.SetFID(newFeatureid);
                    ogrpgLayer.CreateFeature(pFeatureInsert);
                    tobeOK = true;
                }
                catch
                {
                    index++;
                    tobeOK = false;
                }
            }

        }
        public static OSGeo.OGR.Feature GetFeatureFromPostGIS(string gisdbconnection, string layername, string sqlClause = "")
        {
            OSGeo.OGR.Feature pFeature = null; ;
            OSGeo.OGR.Ogr.RegisterAll();
            OSGeo.OGR.Driver pgDriver = OSGeo.OGR.Ogr.GetDriverByName("PostgreSQL");
            OSGeo.OGR.DataSource pgDatasource = null;
            //OSGeo.OSR.SpatialReference spatialref = new OSGeo.OSR.SpatialReference(srText);
            OSGeo.OGR.Layer ogrpgLayer = null;

            string pgds = DataBaseConfigs.GetGISDbDatasource(DataBaseConfigs.DatabaseEngineType);
            pgds = string.Format("PG:{0}", pgds);
            pgDatasource = pgDriver.Open(pgds, 0);
            ogrpgLayer = pgDatasource.GetLayerByName(layername);
            ogrpgLayer.SetAttributeFilter(sqlClause);
            ogrpgLayer.ResetReading();
            OSGeo.OGR.Feature ogrfeature = ogrpgLayer.GetNextFeature();
            if (ogrfeature != null)
            {
                pFeature = ogrfeature;
                //ogrfeature = ogrpgLayer.GetNextFeature();
            }
            return pFeature;
        }
        public static IFeatureClass CreateMemoryFeatureFromGPSPoint(string layername)
        {
            IFeatureClass featureClass = null;
            ESRI.ArcGIS.Geodatabase.IWorkspaceFactory pWSF = new ESRI.ArcGIS.DataSourcesGDB.InMemoryWorkspaceFactoryClass();
            IWorkspaceName pWSName = pWSF.Create(null, "MyGPSWorkspace", null, 0);
            IName pName = (IName)pWSName;

            IWorkspace memoryWS = (IWorkspace)pName.Open();

            // 为图层添加字段信息
            IField field = new FieldClass();
            IFieldEdit fieldEdit = field as IFieldEdit;
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = fields as IFieldsEdit;

            fieldEdit.Name_2 = "OBJECTID";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldEdit.IsNullable_2 = false;
            fieldEdit.Required_2 = false;
            fieldsEdit.AddField(field);

            field = new FieldClass();
            fieldEdit = field as IFieldEdit;
            IGeometryDef geoDef = new GeometryDefClass();
            IGeometryDefEdit geoDefEdit = (IGeometryDefEdit)geoDef;
            geoDefEdit.AvgNumPoints_2 = 1;
            geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
            geoDefEdit.GridCount_2 = 1;
            geoDefEdit.HasM_2 = false;
            geoDefEdit.HasZ_2 = false;

            ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference wgs84spatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);

            geoDefEdit.SpatialReference_2 = wgs84spatialReference;
            fieldEdit.Name_2 = "SHAPE";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.GeometryDef_2 = geoDef;
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Required_2 = true;
            fieldsEdit.AddField(field);

            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)memoryWS;
            featureClass = featureWorkspace.CreateFeatureClass(layername, fields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");
            
            return featureClass;
        }
        public static IFeatureClass CreateMemoryFeatureClassFromPostGIS( string pgconnection,string layername,
            string sqlClause = "",
            string srText = "GEOGCS['unnamed',DATUM['D_WGS_1984',SPHEROID['World Geodetic System of 1984',6378137.0,298.257222932867]],PRIMEM['Greenwich',0.0],UNIT['degree',0.0174532925199433]]",
            List<int>SelectedOIDs=null)
        {
            IFeatureClass featureClass = null;
            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "gb2312");
            OSGeo.OGR.Ogr.RegisterAll();
            OSGeo.OGR.Driver pgDriver = OSGeo.OGR.Ogr.GetDriverByName("PostgreSQL");
            OSGeo.OGR.DataSource pgDatasource = null;
            OSGeo.OSR.SpatialReference spatialref =  new OSGeo.OSR.SpatialReference(srText);
            OSGeo.OGR.Layer ogrpgLayer = null;

            string pgds = string.Format("PG:{0}", pgconnection);

            pgDatasource = pgDriver.Open(pgds, 0);
            ogrpgLayer = pgDatasource.GetLayerByName(layername);

            OSGeo.OGR.FeatureDefn featuredef = ogrpgLayer.GetLayerDefn();
            spatialref = ogrpgLayer.GetSpatialRef();

            ESRI.ArcGIS.Geodatabase.IWorkspaceFactory pWSF = new ESRI.ArcGIS.DataSourcesGDB.InMemoryWorkspaceFactoryClass();
            IWorkspaceName pWSName = pWSF.Create(null, "MyWorkspace", null, 0);
            IName pName = (IName)pWSName;

            IWorkspace memoryWS = (IWorkspace)pName.Open();

            // 为图层添加字段信息
            IFields fields = MapFields(featuredef,spatialref,ogrpgLayer.GetGeomType(),srText);

            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)memoryWS;
            featureClass = featureWorkspace.CreateFeatureClass(layername, fields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");

            ogrpgLayer.SetAttributeFilter(sqlClause);
            ogrpgLayer.ResetReading();
            OSGeo.OGR.Feature ogrfeature = ogrpgLayer.GetNextFeature();
            while(ogrfeature != null)
            {
                if (SelectedOIDs != null && SelectedOIDs.IndexOf(ogrfeature.GetFID())<0)
                {
                    ogrfeature = ogrpgLayer.GetNextFeature();
                    continue;
                }

                //Fill the esri feature by ogrfeature;
                IFeature feat = featureClass.CreateFeature();
                OgrFeatureToesriFeature(feat, ogrfeature);

                ogrfeature = ogrpgLayer.GetNextFeature();
            }

            return featureClass;
        }
        public static IFeatureClass CreateMemoryFeatureClassFromPostGISWkt(string pgconnection, string layername,
                    string sqlClause = "",
                    string srText = "GEOGCS['unnamed',DATUM['D_WGS_1984',SPHEROID['World Geodetic System of 1984',6378137.0,298.257222932867]],PRIMEM['Greenwich',0.0],UNIT['degree',0.0174532925199433]]",
                    List<int> SelectedOIDs = null)
        {
            IFeatureClass featureClass = null;

            //search the matched items
            string sql_error = string.Format("select * from {0} where {1}", layername, sqlClause);
            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgconnection);

            DataTable data = datareadwrite.GetDataTableBySQL(sql_error);
            OSGeo.OSR.SpatialReference spatialref = new OSGeo.OSR.SpatialReference(srText);

            ESRI.ArcGIS.Geodatabase.IWorkspaceFactory pWSF = new ESRI.ArcGIS.DataSourcesGDB.InMemoryWorkspaceFactoryClass();
            IWorkspaceName pWSName = pWSF.Create(null, "MyWktWorkspace", null, 0);
            IName pName = (IName)pWSName;

            IWorkspace memoryWS = (IWorkspace)pName.Open();

            // 为图层添加字段信息
            IFields fields = MapFields(data, spatialref, OSGeo.OGR.wkbGeometryType.wkbPoint, srText);

            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)memoryWS;
            featureClass = featureWorkspace.CreateFeatureClass(layername, fields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");

            //ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            //ISpatialReference wgs84spatialReference = spatialReferenceFactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984);

            foreach(DataRow r in data.Rows)
            {
                IFeature feature = featureClass.CreateFeature();
                int oid = feature.OID;
                //feature.Shape.SpatialReference = 
                int i = 2;
                foreach(DataColumn dc in data.Columns)
                {
                    int index = feature.Fields.FindField(dc.ColumnName);
                    if(index>=0)
                    {
                        if(dc.ColumnName=="shape")
                        {

                        }else
                        {
                            string value = Convert.ToString(r[dc]);
                            feature.Value[index] = value;
                        }

                    }
                    i++;
                }
                string shapewkt = r["Shape"] as string;
                IGeometry shape = Utils.Converters.ConvertWKTToGeometry(shapewkt);

                if(shape.GeometryType==esriGeometryType.esriGeometryPoint)
                {
                    feature.Shape = shape;
                }
                else if (shape.GeometryType == esriGeometryType.esriGeometryPolyline)
                {
                    feature.Shape = (shape as IPolyline).FromPoint;
                }
                

                feature.Store();
            }
            
            return featureClass;
        }

        public static IFeatureClass CreateMemoryFeatureClass(ISpatialReference spatialReference, esriGeometryType geometryType, string name = "Temp")
        {
            // 创建内存工作空间  
            ESRI.ArcGIS.Geodatabase.IWorkspaceFactory pWSF = new ESRI.ArcGIS.DataSourcesGDB.InMemoryWorkspaceFactoryClass();  
            IWorkspaceName pWSName = pWSF.Create("", "Temp", null, 0);  
            IName pName = (IName)pWSName;  
            IWorkspace memoryWS = (IWorkspace)pName.Open();  
  
            IField field = new FieldClass();  
            IFieldEdit fieldEdit = field as IFieldEdit;  
            IFields fields = new FieldsClass();  
            IFieldsEdit fieldsEdit = fields as IFieldsEdit;  
  
  
            fieldEdit.Name_2 = "OBJECTID";  
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;  
            fieldEdit.IsNullable_2 = false;  
            fieldEdit.Required_2 = false;  
            fieldsEdit.AddField(field);  
  
            field = new FieldClass();  
            fieldEdit = field as IFieldEdit;  
            IGeometryDef geoDef = new GeometryDefClass();  
            IGeometryDefEdit geoDefEdit = (IGeometryDefEdit)geoDef;  
            geoDefEdit.AvgNumPoints_2 = 5;  
            geoDefEdit.GeometryType_2 = geometryType;  
            geoDefEdit.GridCount_2 = 1;  
            geoDefEdit.HasM_2 = false;  
            geoDefEdit.HasZ_2 = false;  
            geoDefEdit.SpatialReference_2 = spatialReference;  
            fieldEdit.Name_2 = "SHAPE";  
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;  
            fieldEdit.GeometryDef_2 = geoDef;  
            fieldEdit.IsNullable_2 = true;  
            fieldEdit.Required_2 = true;  
            fieldsEdit.AddField(field);  
  
            field = new FieldClass();  
            fieldEdit = field as IFieldEdit;  
            fieldEdit.Name_2 = "Code";  
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;  
            fieldEdit.IsNullable_2 = true;  
            fieldsEdit.AddField(field);  
  
            //创建要素类  
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)memoryWS;  
            IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(name, fields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");
            IFeature feat = featureClass.CreateFeature();
            //feat.Shape.
  
            return featureClass;  
        }

        /// <summary>
        /// 将IFeatureClass拷贝到ipFeaWs中
        /// </summary>
        /// <param name="pDs">要创建的源图层</param>
        /// <param name="ipFeaWs">待拷贝库的工作空间</param>
        /// <returns>如果不能创建，则返回false</returns>
        public static bool CreateFtCls(IDataset pDs, IFeatureDataset ipFeaWs,string dsName = "")
        {
            IFeatureClass pFtCls = (IFeatureClass)pDs;
            IFeatureClass _featureClass = null;
            
            //IWorkspace2 pWs = (IWorkspace2)ipFeaWs;
            //if (!ipFeaWs.get_NameExists(esriDatasetType.esriDTFeatureClass, dsName))
            try
            {
                _featureClass = ipFeaWs.CreateFeatureClass(dsName, pFtCls.Fields, pFtCls.CLSID,
                                                 pFtCls.EXTCLSID, pFtCls.FeatureType,
                                                 pFtCls.ShapeFieldName, "");
                IFeatureClassLoad pFCLoad = _featureClass as IFeatureClassLoad;
                pFCLoad.LoadOnlyMode = true;

                //更新图层别名
                IClassSchemaEdit ipEdit = (IClassSchemaEdit)_featureClass;
                ipEdit.AlterAliasName(dsName);

                IFeatureCursor _featureCursor = pFtCls.Search(null, false);
                IFeature _feature = _featureCursor.NextFeature();
                while (_feature != null)
                {
                    IFeature _featureNew = _featureClass.CreateFeature();

                    for (int k = 0; k < _featureClass.Fields.FieldCount; k++)
                    {
                        ESRI.ArcGIS.Geodatabase.IField _field = _featureNew.Fields.get_Field(k);
                        if (_field.Editable == true)
                        {
                            _featureNew.set_Value(k, _feature.get_Value(k));
                        }
                        Marshal.ReleaseComObject(_field);
                    }
                    _featureNew.Store();
                    _feature = _featureCursor.NextFeature();
                }
                pFCLoad.LoadOnlyMode = false;
            }
            catch(Exception ex)
          //  else
            {
          //      MessageBox.Show("同名的" + pDs.Name + "已存在，无法导入！", "提示");
          //      return false;
            }

            return true;
        }

        public static IFeatureClass GetFeatureClass(IWorkspace pWorkspace, string pName)
        {
            IFeatureClass rClassObj = null;
            IFeatureWorkspace wkSpace = pWorkspace as IFeatureWorkspace;
            if (wkSpace == null) return null;
            List<string> aContainerList = QueryFeatureDatasetName(pWorkspace, true, true);
            foreach (string aContainer in aContainerList)
            {
                try
                {
                    IFeatureDataset fds = wkSpace.OpenFeatureDataset(aContainer);
                    IFeatureClassContainer aContainerObj = fds as IFeatureClassContainer;
                    rClassObj = aContainerObj.get_ClassByName(pName);
                    if (rClassObj != null)
                        break;
                }
                catch (Exception ex) { }
            }
            return rClassObj;
        }

        public static IFeatureClass GetFeatureClass(IWorkspace pWorkspace, int pID)
        {
            IFeatureClass rClassObj = null;
            IFeatureWorkspace wkSpace = pWorkspace as IFeatureWorkspace;
            if (wkSpace == null) return null;
            List<string> aContainerList = QueryFeatureDatasetName(pWorkspace, true, true);
            foreach (string aContainer in aContainerList)
            {
                try
                {
                    IFeatureDataset fds = wkSpace.OpenFeatureDataset(aContainer);
                    IFeatureClassContainer aContainerObj = fds as IFeatureClassContainer;
                    rClassObj = aContainerObj.get_ClassByID(pID);
                    if (rClassObj != null)
                        break;
                }
                catch (Exception ex) { }
            }
            return rClassObj;
        }

        public static List<string> QueryFeatureDatasetName(IWorkspace pWorkspace, bool v1, bool v2)
        {
            IEnumDataset pEnumDataset = pWorkspace.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureDataset);
            //IEnumDataset pEnumDataset = pWorkspace.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureClass);
            List<string> FeatureDatasetNameList = new List<string>();
            pEnumDataset.Reset();
            IDataset pDataset = pEnumDataset.Next();
            while (pDataset != null)
            {
                //找到TOPO数据集，将所缺的layer复制到该数据集中
                if (pDataset is IFeatureDataset)
                {
                    FeatureDatasetNameList.Add(pDataset.Name);
                }
                pDataset = pEnumDataset.Next();
            }
            return FeatureDatasetNameList;
        }

        public static List<string> QueryFeatureClassName(IWorkspace pWorkspace, bool v1, bool v2)
        {
            //IEnumDataset pEnumDataset = pWorkspace.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureDataset);
            IEnumDataset pEnumDataset = pWorkspace.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureClass);
            List<string> FeatureDatasetNameList = new List<string>();
            pEnumDataset.Reset();
            IDataset pDataset = pEnumDataset.Next();
            while (pDataset != null)
            {
                //找到TOPO数据集，将所缺的layer复制到该数据集中
                if (pDataset is IFeatureClass)
                {
                    FeatureDatasetNameList.Add(pDataset.Name);
                }
                pDataset = pEnumDataset.Next();
            }
            return FeatureDatasetNameList;
        }
    }
}

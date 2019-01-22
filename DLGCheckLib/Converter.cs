using NetTopologySuite.IO;
using ESRI.ArcGIS.Geometry;
using System;

namespace Utils
{
    public static class NumberConver
    {
        /// <summary>
        /// 给数字加上圆圈。
        /// </summary>
        /// <param name="num">要加圆圈的数字。</param>
        /// <returns></returns>
       public  static string NumAddCircle(int num)
        {
            if (num < 0) return string.Empty;   //负数不处理。
            if (num.ToString().Contains("0")) return string.Empty;  //有0的不处理
            string resultStr = string.Empty;
            foreach (char str in num.ToString())
            {
                resultStr += (char)(9312 + Int32.Parse(str.ToString()) - 1);
            }
            return resultStr;
        }
    }

    public static class ShapeFactory
    {
        public static IPolyline CreatePolylineFromTwoPoints(IPoint ptFrom,IPoint ptEnd)
        {
            ILine pLine = new LineClass();
            pLine.FromPoint = ptFrom;
            pLine.PutCoords(ptFrom, ptEnd);
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

            return pPLine;
        }
    }


    /// <summary>
    /// This class is used to convert a GeoAPI Geometry to ESRI and vice-versa.
    /// It can also convert a ESRI Geometry to WKB/WKT and vice-versa.
    /// </summary>
    public static class Converters
    {

        public static byte[] ConvertGeometryToWKB(IGeometry geometry)
        {
            IWkb wkb = geometry as IWkb;
            ITopologicalOperator oper = geometry as ITopologicalOperator;
            oper.Simplify();

            IGeometryFactory3 factory = new GeometryEnvironment() as IGeometryFactory3;
            byte[] b = factory.CreateWkbVariantFromGeometry(geometry) as byte[];
            return b;
        }


        public static byte[] ConvertWKTToWKB(string wkt)
        {
            WKBWriter writer = new WKBWriter();
            WKTReader reader = new WKTReader();
            return writer.Write(reader.Read(wkt));
        }

        public static string ConvertWKBToWKT(byte[] wkb)
        {
            WKTWriter writer = new WKTWriter();
            WKBReader reader = new WKBReader();
            return writer.Write(reader.Read(wkb));
        }

        public static string ConvertGeometryToWKT(IGeometry geometry)
        {
            byte[] b = ConvertGeometryToWKB(geometry);
            WKBReader reader = new WKBReader();
            GeoAPI.Geometries.IGeometry g = reader.Read(b);
            WKTWriter writer = new WKTWriter();
            return writer.Write(g);
        }

        public static IGeometry ConvertWKTToGeometry(string wkt)
        {
            if (wkt == null||wkt=="")
            return null;

            byte[] wkb = ConvertWKTToWKB(wkt);
            return ConvertWKBToGeometry(wkb);
        }

        public static IGeometry ConvertWKBToGeometry(byte[] wkb)
        {
            IGeometry geom;
            int countin = wkb.GetLength(0);
            IGeometryFactory3 factory = new GeometryEnvironment() as IGeometryFactory3;
            factory.CreateGeometryFromWkbVariant(wkb, out geom, out countin);
            return geom;
        }


        public static IGeometry ConvertGeoAPIToESRI(GeoAPI.Geometries.IGeometry geometry)
        {
            WKBWriter writer = new WKBWriter();
            byte[] bytes = writer.Write(geometry);
            return ConvertWKBToGeometry(bytes);
        }

        public static GeoAPI.Geometries.IGeometry ConvertESRIToGeoAPI(IGeometry geometry)
        {
            byte[] wkb = ConvertGeometryToWKB(geometry);
            WKBReader reader = new WKBReader();
            return reader.Read(wkb);
        }
    }
}

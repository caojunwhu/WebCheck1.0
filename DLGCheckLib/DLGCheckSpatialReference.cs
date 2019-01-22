using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseDesignPlus;
using ESRI.ArcGIS.Geometry;

namespace DLGCheckLib
{
    public class DLGCheckCoordinateSystem
    {
        DatabaseDesignPlus.IDatabaseReaderWriter datareadwrite;

        public DLGCheckCoordinateSystem()
        {
            Initialize();
        }
        public List<DLGCheckSpatialReference> spatialreferences;
        public List<string> geogcsnames;
        void Initialize()
        {
            spatialreferences = new List<DLGCheckSpatialReference>();
            geogcsnames = new List<string>();

            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string sql_spatialref = string.Format("select * from spatial_ref_sys");
            DataTable datatable = datareadwrite.GetDataTableBySQL(sql_spatialref);
            foreach(DataRow dr in datatable.Rows)
            {
                DLGCheckSpatialReference spatialref = new DLGCheckSpatialReference();
                spatialref.srid = Convert.ToInt32(dr["srid"]);
                spatialref.auth_name = dr["auth_name"] as string;
                spatialref.srtext = Convert.ToString(dr["srtext"]);
                spatialref.proj4text = Convert.ToString(dr["proj4text"]);
                spatialref.spatialreftitle = string.Format("{0}:{1}", spatialref.auth_name, spatialref.srid);

                //ISpatialReferenceFactory spatialReferenceFactory = new SpatialReferenceEnvironmentClass();
                //ISpatialReference spatialReference = spatialReferenceFactory.CreateESRISpatialReferenceFromPRJ(spatialref.srtext);

                OSGeo.OSR.SpatialReference spatialreference = new OSGeo.OSR.SpatialReference(spatialref.srtext);
                string spatialrefname = spatialreference.IsGeographic() == 0 ? spatialreference.GetAttrValue("PROJCS", 0) : spatialreference.GetAttrValue("GEOGCS", 0);
                spatialref.IsProject = spatialreference.IsGeographic() == 0 ? true:false;
                spatialref.geogcs = spatialreference.GetAttrValue("GEOGCS", 0);

                spatialref.Name = spatialrefname;

                if(geogcsnames.Contains(spatialref.geogcs)==false)
                {
                    geogcsnames.Add(spatialref.geogcs);
                }

                spatialreferences.Add(spatialref);

            }
            geogcsnames.Sort();
        }
    }
    public class DLGCheckSpatialReference
    {
         public string Name { set; get; }
        public bool IsProject { set; get; }
        public string spatialreftitle { get; set; }
         public string srtext { get; set; }
        public string auth_name { get; set; }
        public int srid { get; set; }
        public string proj4text { get; set; }
        public string geogcs { set; get; }


    }
}

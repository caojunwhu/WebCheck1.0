using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseDesignPlus;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using PluginUI;
using Utils;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using System.Windows.Forms;

namespace DLGCheckLib
{
    public class ElevationSearchSetting
    {
        string localProjectID = "";
        public string LayerName { set; get; }
        public string Layer { set; get; }
        public string ElevationField { set; get; }

        public ElevationSearchSetting(string projectid)
        {
            localProjectID = projectid;
            //this.LayerName = "Point";
            //this.Layer = "GCD";
            //this.ElevationField = "Elevation";
            //Read();
        }
        public void Read()
        {
            string ElevationSearchSettingTableName = "elevationlayerdef";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            List<string> tableNames = datareadwrite.GetSchameDataTableNames();
            if (tableNames.Contains(ElevationSearchSettingTableName) == true)
            {
                string sql_searchtargetsetting = string.Format("select layername , layer ,elevationfield  from {0} where projectid = '{1}'", ElevationSearchSettingTableName, localProjectID);
                DataTable datatable = datareadwrite.GetDataTableBySQL(sql_searchtargetsetting);
                if (datatable.Rows.Count > 0)
                {
                    DataRow datarow = datatable.Rows[0];
                    this.LayerName = datarow["layername"] as string;
                    this.Layer = datarow["layer"] as string;
                    this.ElevationField = datarow["elevationfield"] as string;
                }
            }
        }
        public void Write()
        {
            string ElevationSearchSettingTableName = "elevationlayerdef";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            List<string> tableNames = datareadwrite.GetSchameDataTableNames();
            if (tableNames.Contains(ElevationSearchSettingTableName) == false)
            {
                //创建表
                string sql_createTable = string.Format("create table {0}( ProjectID text,LayerName text,Layer text,elevationfield text )", ElevationSearchSettingTableName);
                datareadwrite.ExecuteSQL(sql_createTable);
            }
            else
            {
                //逐条记录到数据表SearchTargetSetting
                string sql_queryitem = string.Format("select Layer from {0} where ProjectID = '{1}' and LayerName='{2}' and Layer='{3}' ", ElevationSearchSettingTableName, localProjectID, this.LayerName, this.Layer);
                object o = datareadwrite.GetScalar(sql_queryitem);
                if (o == null)//insert
                {
                    string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}')", ElevationSearchSettingTableName, localProjectID, this.LayerName, this.Layer, this.ElevationField);
                    datareadwrite.ExecuteSQL(sql_insert);
                }
                else // update
                {
                    string sql_update = string.Format("update {0} set  layer ='{1}',elevationfield = '{2}' where ProjectID = '{3}' and LayerName='{4}'  ", ElevationSearchSettingTableName, this.Layer,this.ElevationField, localProjectID, this.LayerName);
                    datareadwrite.ExecuteSQL(sql_update);
                }
            }
        }
    }
    //
    public class BufferSearchTargetItem
    {
        public string Layer { set; get; }
        public CheckPointType PointType { set; get; }
        public double Distance { set; get; }
        public double Height { set; get; }
        public IPoint pTargetPoint { set; get; }
        public IGeometry Shape { set; get; }
    }

    //2017-5-3新增记录错漏的标注矢量，用户存储显示、定位等操作，详实记录问题错漏，便于打分，同时表可以读取存储于位置精度记录中的粗差情况
    public class PinErrorItem
    {

        public string Projectid { set; get; }
        public string Mapnumber { set; get; }
        public string Error { set; get; }
        public string QualityItem { set; get; }
        public string SubQualityItem { set; get; }
        public string CheckItem { set; get; }
        public string ErrorType { set; get; }
        public string Checker { set; get; }
        public string CheckTime { set; get; }
        public string Feedback { set; get; }
        public string Modify { set; get; }
        public string Review { set; get; }
        public string Comment { set; get; }
        public string Shape { set; get; }
    
        public void Write()
        {
            /////////////更新到数据库
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string SampleerrorCollectionTableName = "sampleerrorplus";
            //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
            List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
            if (tableNames2.Contains(SampleerrorCollectionTableName) == false)
            {
                string sql_createTable = string.Format("create table {0}( ProjectID text,Mapnumber text,质量元素 text,质量子元素 text,检查项 text,错漏类别 text ,错漏描述 text ,处理意见 text, 修改情况 text, 复查情况 text,检查者 text,检查日期 text,备注 text,Shape text)", SampleerrorCollectionTableName);
                datareadwrite.ExecuteSQL(sql_createTable);
                //记录
            }
            //delete the old shape indentified record
            string sql_delete = string.Format("delete from {0} where projectid='{1}' and mapnumber ='{2}' and shape='{3}'", SampleerrorCollectionTableName,this.Projectid,this.Mapnumber,this.Shape);
            datareadwrite.ExecuteSQL(sql_delete);

            string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')", SampleerrorCollectionTableName, Projectid, Mapnumber,
                                QualityItem,SubQualityItem,CheckItem,ErrorType,Error,Feedback,Modify,Review,Checker,CheckTime,Comment,Shape);
            datareadwrite.ExecuteSQL(sql_insert);


        }
    }
    public class PinErrorCollection
    {
        string projectid;
        string mapnumber;
        List<PinErrorItem> errorItems;

        public string Projectid
        {
            get
            {
                return projectid;
            }

            set
            {
                projectid = value;
            }
        }

        public string Mapnumber
        {
            get
            {
                return mapnumber;
            }

            set
            {
                mapnumber = value;
            }
        }

        public List<PinErrorItem> ErrorItems
        {
            get
            {
                return errorItems;
            }

            set
            {
                errorItems = value;
            }
        }

        public void Read(string sprojected,string smapnumber)
        {
            Projectid = sprojected;
            Mapnumber = smapnumber;

            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string SampleerrorCollectionTableName = "sampleerrorplus";
            //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
            List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
            if (tableNames2.Contains(SampleerrorCollectionTableName) == false)
            {
                string sql_createTable = string.Format("create table {0}( ProjectID text,Mapnumber text,质量元素 text,质量子元素 text,检查项 text,错漏类别 text ,错漏描述 text ,处理意见 text, 修改情况 text, 复查情况 text,检查者 text,检查日期 text,备注 text,Shape text)", SampleerrorCollectionTableName);
                datareadwrite.ExecuteSQL(sql_createTable);
                //记录
            }
            //逐条记录到数据表SearchTargetSetting
            string sql_queryitem = string.Format("select * from {0} where ProjectID = '{1}' and Mapnumber='{2}' order by 检查日期 desc ", SampleerrorCollectionTableName, Projectid, Mapnumber);

            DataTable dt = datareadwrite.GetDataTableBySQL(sql_queryitem);
            if(dt.Rows.Count>0)
            {
                if (ErrorItems != null) ErrorItems.Clear();
                if (ErrorItems == null) ErrorItems = new List<PinErrorItem>();

                foreach (DataRow dr in dt.Rows)
                {
                    PinErrorItem perror = new PinErrorItem();
                    perror.Projectid = dr["ProjectID"] as string;
                     perror.Mapnumber = dr["Mapnumber"] as string;
                    perror.Error = dr["错漏描述"] as string;
                    perror.QualityItem = dr["质量元素"] as string;
                    perror.SubQualityItem = dr["质量子元素"] as string;
                    perror.CheckItem = dr["检查项"] as string;
                    perror.ErrorType = dr["错漏类别"] as string;
                    perror.Checker = dr["检查者"] as string;
                    perror.CheckTime = dr["检查日期"] as string;
                    perror.Feedback = dr["处理意见"] as string;
                    perror.Modify = dr["修改情况"] as string;
                    perror.Review = dr["复查情况"] as string;
                    perror.Comment = dr["备注"] as string;
                    perror.Shape = dr["Shape"] as string;

                    ErrorItems.Add(perror);
                }
            }


        }

        public void Write()
        {
            if (ErrorItems == null || ErrorItems.Count == 0)
                return;

            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string SampleerrorCollectionTableName = "sampleerrorplus";
            //逐条记录到数据表SearchTargetSetting
            string sql_queryitem = string.Format("select Mapnumber from {0} where ProjectID = '{1}' and Mapnumber='{2}' ", SampleerrorCollectionTableName, Projectid, Mapnumber);
            object o = datareadwrite.GetScalar(sql_queryitem);
            if (o == null)//insert
            {
            }
            else // delete
            {
                string sql_delete = string.Format("delete from {0} where  ProjectID = '{1}' and Mapnumber='{2}' ", SampleerrorCollectionTableName, Projectid, Mapnumber);
                datareadwrite.ExecuteSQL(sql_delete);
            }

            foreach (PinErrorItem item in ErrorItems)
            {
                item.Write();
            }
        }

        //根据质量元素对错漏进行编号，输入到检查意见记录表中，方便格式化打印
        public void Write(QualityItems qualityItems)
        {
            if (ErrorItems == null || ErrorItems.Count == 0|| qualityItems==null ||
                qualityItems.QualityItemList==null || qualityItems.QualityItemList.Count==0)
                return;

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
            string sql_queryitem = string.Format("select Mapnumber from {0} where ProjectID = '{1}' and Mapnumber='{2}' ", SampleerrorCollectionTableName, Projectid, Mapnumber);
            object o = datareadwrite.GetScalar(sql_queryitem);
            if (o == null)//insert
            {
            }
            else // delete
            {
                string sql_delete = string.Format("delete from {0} where  ProjectID = '{1}' and Mapnumber='{2}' ", SampleerrorCollectionTableName, Projectid, Mapnumber);
                datareadwrite.ExecuteSQL(sql_delete);
            }

            string pinerrortablename = "sampleerrorplus";
            string sql_statistic = string.Format("select distinct 质量元素,质量子元素,错漏类别,错漏描述,处理意见,修改情况,复查情况,检查者 from {0} where  ProjectID = '{1}' and Mapnumber='{2}'", pinerrortablename, Projectid, Mapnumber);
            DataTable data = datareadwrite.GetDataTableBySQL(sql_statistic);
            foreach(DataRow dr in data.Rows)
            {
                string sql_count = string.Format("select count(*) from {0} where 质量元素='{1}' and 质量子元素='{2}' and 错漏类别='{3}' and 错漏描述='{4}' and 处理意见='{5}' and 修改情况='{6}' and 复查情况='{7}' and 检查者='{8}'and  ProjectID = '{9}' and Mapnumber='{10}'", pinerrortablename, dr["质量元素"] as string, dr["质量子元素"] as string, dr["错漏类别"] as string, dr["错漏描述"] as string, dr["处理意见"] as string, dr["修改情况"] as string, dr["复查情况"] as string, dr["检查者"] as string, Projectid, Mapnumber);
                int count = Convert.ToInt32(datareadwrite.GetScalar(sql_count));
                QualityItem qItem = qualityItems.QualityItemList.Find(ao => ao.QualityItemName == Convert.ToString(dr["质量元素"]));
                int indexofqualityitem = qualityItems.QualityItemList.IndexOf(qItem)+1;
                string errorClass = string.Format("{0}{1}{2}", Utils.NumberConver.NumAddCircle(indexofqualityitem), count == 0 ? "" : count.ToString(), Convert.ToString(dr["错漏类别"]));

                string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", SampleerrorCollectionTableName, Projectid, Mapnumber,
                    dr["质量元素"] as string, dr["质量子元素"] as string, errorClass, dr["错漏描述"] as string, dr["处理意见"] as string, dr["修改情况"] as string, dr["复查情况"] as string, dr["检查者"] as string,DateTime.Now.ToString());
                datareadwrite.ExecuteSQL(sql_insert);
            }


            foreach (PinErrorItem item in ErrorItems)
            {
                //string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", SampleerrorCollectionTableName, Projectid, Mapnumber,
                //    dr["质量元素"] as string, dr["质量子元素"] as string, dr["错漏类别"] as string, dr["错漏描述"] as string, dr["处理意见"] as string, dr["修改情况"] as string, dr["复查情况"] as string, dr["检查者"] as string, dr["检查日期"] as string);
                //datareadwrite.ExecuteSQL(sql_insert);
                //记录搜索到的目标

            }
        }
        void ExportErrorShape(string shapeType, OSGeo.OGR.wkbGeometryType geomtype, string targetLayername, string sourcelayername, DLGCheckProjectClass checkProject,string workDirectory, string gdbfilename)
        {
            //创建数据集，并创建ErrorPoints ErrorLines错漏数据层
            ESRI.ArcGIS.Geodatabase.IWorkspaceFactory pWSF = null;
            IWorkspaceName pWSName=null;
            IName pName = null;

            string sqlClause = string.Format("projectid='{0}' and mapnumber='{1}' and position('{2}' in shape)>0",checkProject.ProjectID,Mapnumber, shapeType);
            string srText = checkProject.SrText;
            IWorkspace memoryWS = null;

            string gdbfilepath = workDirectory + "\\" + gdbfilename;
            if (!Directory.Exists(gdbfilepath))
            {
                pWSF = new ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactoryClass();
                pWSName = pWSF.Create(workDirectory, gdbfilename, null, 0);
                pName = (IName)pWSName;
                memoryWS = (IWorkspace)pName.Open();

            }
            else
            {
                pWSF = new ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactoryClass();
                memoryWS = pWSF.OpenFromFile(gdbfilepath, 0);
            }

            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IFeatureClass featureClass = null;

            //search the matched items
            string sql_error = string.Format("select * from {0} where {1}", sourcelayername, sqlClause);
            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);

            DataTable data = datareadwrite.GetDataTableBySQL(sql_error);
            OSGeo.OSR.SpatialReference spatialref = new OSGeo.OSR.SpatialReference(srText);

            // 为图层添加字段信息

            List<string> aContainerList = ArcGISHelper.QueryFeatureClassName(memoryWS, true, true);

            if(aContainerList.IndexOf(targetLayername)>=0)
            {
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)memoryWS;
                featureClass = featureWorkspace.OpenFeatureClass(targetLayername);
            }                

            if (featureClass==null)
            {
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)memoryWS;
                IFields fields = ArcGISHelper.MapFields(data, spatialref, geomtype, srText);
                featureClass = featureWorkspace.CreateFeatureClass(targetLayername, fields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");
            }

            foreach (DataRow r in data.Rows)
            {
                IFeature feature = featureClass.CreateFeature();
                int oid = feature.OID;
                //feature.Shape.SpatialReference = 
                int i = 2;
                foreach (DataColumn dc in data.Columns)
                {
                    int index = feature.Fields.FindField(dc.ColumnName);
                    if (index >= 0)
                    {
                        if (dc.ColumnName == "shape")
                        {

                        }
                        else
                        {
                            string value = Convert.ToString(r[dc]);
                            feature.Value[index] = value;
                        }

                    }
                    i++;
                }
                string shapewkt = r["Shape"] as string;
                IGeometry shape = Utils.Converters.ConvertWKTToGeometry(shapewkt);
                feature.Shape = shape;

                feature.Store();
            }
        }

        public void Export(string workDirectory,DLGCheckProjectClass checkProject)
        {
            
            string shapeType = "POINT";
            OSGeo.OGR.wkbGeometryType geomtype = OSGeo.OGR.wkbGeometryType.wkbPoint;
            string targetLayername = "PointErrors";
            string layername = "sampleerrorplus";
            string gdbfilename = string.Format("{0}错漏数据集{1}.gdb", checkProject.ProjectName, DateTime.Now.ToString("yyyy-MM-dd"));

            //Export point
            ExportErrorShape(shapeType, geomtype, targetLayername, layername, checkProject, workDirectory, gdbfilename);
            //export line
            shapeType = "LINESTRING";
            targetLayername = "LineErrors";
            geomtype = OSGeo.OGR.wkbGeometryType.wkbLineString;
            ExportErrorShape(shapeType, geomtype, targetLayername, layername, checkProject, workDirectory, gdbfilename);

            MessageBox.Show(string.Format("成功导出错漏数据集：{0}",gdbfilename));
        }
    }

    //用于存储间距边长检测线，吸附的起始点、终点，可以实时改写检测线信息
    public class RelativeLineItem
    {
        public string Mapnumber { set; get; }
        public string PtID { set; get; }
        public double TL { set; get; }
        public double SL { set; get; }
        public double DL { set; get; }
        public string Checker { set; get; }
        public string Date { set; get; }
        public string Comment { set; get; }
        public string StartLayer { set; get; }
        public string TargetLayer { set; get; }
        public IPoint StartPoint { set; get; }
        public IPoint TargetPoint { set; get; }
        //对新增的插入，更新的进行改变
        public bool RelativeLineSave(string projectid,string szMapNumber,string currentuser)
        {
            //检查数据有效性
            //非本人修改不可以
            if(StartPoint==null|| TargetPoint==null||szMapNumber!=Mapnumber||this.Checker!=currentuser)
            {
                return false;
            }
            //对RelativeLine进行拆分存储，Mapnumber,PtID,TL,SL,DL,Checker,Date存储到"间距边长精度检测点成果表"中，
            /////////////更新到数据库
            string RelativeCheckTableName = "间距边长精度检测点成果表";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string sql_queryitem = string.Format("select ptid from {0} where mapnumber = '{1}' and ptid='{2}' ", RelativeCheckTableName, szMapNumber, this.PtID);
            object o = datareadwrite.GetScalar(sql_queryitem);
            if (o == null)//insert
            {
                string sql_insert = string.Format("insert into {0} Values('{1}','{2}',{3},{4},{5},'{6}','{7}')", RelativeCheckTableName, szMapNumber, this.PtID, this.TL, this.SL, this.DL, currentuser, this.Date);
                datareadwrite.ExecuteSQL(sql_insert);
                //记录搜索到的目标
            }
            else // update
            {
                string sql_update = string.Format("update {0} set  tl ={1},sl = {2},dl={3},检查者='{4}',检查日期='{5}',comment='{6}' where mapnumber = '{7}' and ptid='{8}'", RelativeCheckTableName, this.TL, this.SL, this.DL,currentuser,this.Date,this.Comment,szMapNumber, this.PtID);
                datareadwrite.ExecuteSQL(sql_update);
            }

            //projectid,Mapnumber,PtID,StartPoint,StartLayer,TargetPoint,TargetLayer存储到RelativeLineCollection中
            string RelativeLineCollection = "relativelinecollection";
            //首先检查表RelativeLineCollection是否存在，不存在的需要新建
            //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
            List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
            if (tableNames2.Contains(RelativeLineCollection) == false)
            {
                string sql_createTable = string.Format("create table {0}( projectid text,mapnumber text,ptid text,startpoint text,startlayer text,targetpoint text,targetlayer text)", RelativeLineCollection);
                datareadwrite.ExecuteSQL(sql_createTable);
                //记录
            }

            sql_queryitem = string.Format("select ptid from {0} where mapnumber = '{1}' and ptid='{2}' and projectid='{3}'", RelativeLineCollection, szMapNumber, this.PtID,projectid);
            o = datareadwrite.GetScalar(sql_queryitem);
            if (o == null)//insert
            {
                string pstartpointwkt = Converters.ConvertGeometryToWKT(this.StartPoint);
                string ptargetpointwkt = Converters.ConvertGeometryToWKT(this.TargetPoint);

                string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}')", RelativeLineCollection, projectid,szMapNumber, this.PtID, pstartpointwkt, this.StartLayer, ptargetpointwkt, this.TargetLayer);
                datareadwrite.ExecuteSQL(sql_insert);
                //记录搜索到的目标
            }
            else // update
            {
                string pstartpointwkt = Converters.ConvertGeometryToWKT(this.StartPoint);
                string ptargetpointwkt = Converters.ConvertGeometryToWKT(this.TargetPoint);

                string sql_update = string.Format("update {0} set  startpoint ='{1}',startlayer = '{2}',targetpoint='{3}',targetlayer='{4}' where projectid='{5}'and  mapnumber = '{6}' and ptid='{7}'", RelativeLineCollection,pstartpointwkt,this.StartLayer,ptargetpointwkt,this.TargetLayer, projectid, szMapNumber, this.PtID);
                datareadwrite.ExecuteSQL(sql_update);
            }
            return true;
        }

        public bool Remove(string projectid)
        {
            //检查数据有效性
            //非本人修改不可以
            //移除"间距边长精度检测点成果表"中的记录
            string RelativeCheckTableName = "间距边长精度检测点成果表";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string sql_queryitem = string.Format("select ptid from {0} where mapnumber = '{1}' and ptid='{2}' ", RelativeCheckTableName, this.Mapnumber, this.PtID);
            object o = datareadwrite.GetScalar(sql_queryitem);
            if (o != null)//delete
            {
                string sql_delete = string.Format("delete from {0} where mapnumber = '{1}' and ptid='{2}' ", RelativeCheckTableName, this.Mapnumber, this.PtID);
                datareadwrite.ExecuteSQL(sql_delete);
            }

            string RelativeLineCollection = "relativelinecollection";
            //首先检查表RelativeLineCollection是否存在
            List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
            if (tableNames2.Contains(RelativeLineCollection) == true)
            {
                sql_queryitem = string.Format("select ptid from {0} where mapnumber = '{1}' and ptid='{2}' and projectid='{3}'", RelativeLineCollection, this.Mapnumber, this.PtID, projectid);
                o = datareadwrite.GetScalar(sql_queryitem);
                if (o != null)//delete
                {
                    string sql_delete = string.Format("delete from {0} where mapnumber = '{1}' and ptid='{2}' and projectid='{3}'", RelativeLineCollection, this.Mapnumber, this.PtID, projectid);
                    datareadwrite.ExecuteSQL(sql_delete);
                    //记录搜索到的目标
                }

            }

            return true;
        }
    }
    //用于存储一个样本内的间距边长检测线，进行整体存取，修改等操作
    public class RelativeLineCollection
    {
        public List<RelativeLineItem> relativelineList;
        public double RelativeError { set; get; }
        string localProjectid;
        string localMapnumber;
        string localCurrentuser;
        public void Read(string szProjectid,string szMapnumber,string szCurrentuser)
        {
            localProjectid = szProjectid;
            localMapnumber = szMapnumber;
            localCurrentuser = szCurrentuser;

            if(relativelineList!=null)
            {
                relativelineList.Clear();
            }else
            {
                relativelineList = new List<RelativeLineItem>();
            }
            //从"间距边长精度检测点成果表"读取Mapnumber,PtID,TL,SL,DL,Checker,Date，形成间距边长检测线记录
            string RelativeCheckTableName = "间距边长精度检测点成果表";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string sql_queryitem = string.Format("select * from {0} where mapnumber = '{1}'", RelativeCheckTableName, szMapnumber);
            DataTable relativeLines = datareadwrite.GetDataTableBySQL(sql_queryitem);

            //逐条读取，并填充数据
            foreach(DataRow dr in relativeLines.Rows)
            {
                RelativeLineItem rlatitem = new RelativeLineItem();
                rlatitem.Mapnumber = szMapnumber;
                rlatitem.PtID = dr["ptid"] as string;
                rlatitem.TL = Convert.ToDouble(dr["tl"]);
                rlatitem.SL = Convert.ToDouble(dr["sl"]);
                rlatitem.DL = Convert.ToDouble(dr["dl"]);
                rlatitem.Checker = dr["检查者"] as string ;
                rlatitem.Date = Convert.ToString(dr["检查日期"]);
                rlatitem.Comment = dr["comment"] as string;
                relativelineList.Add(rlatitem);
            }

            //从RelativeLineCollection读取projectid,Mapnumber,PtID,StartPoint,StartLayer,TargetPoint,TargetLayer更新到对应的检测线中，如果查询不到则不处理
            string RelativeLineCollection = "relativelinecollection";
            //首先检查表RelativeLineCollection是否存在，不存在的需要新建
            //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
            List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
            if (tableNames2.Contains(RelativeLineCollection) == true)
            {
                //记录
                foreach(RelativeLineItem item in relativelineList)
                {
                    sql_queryitem = string.Format("select * from {0} where mapnumber = '{1}' and projectid='{2}' and ptid='{3}' ", RelativeLineCollection, szMapnumber,szProjectid,item.PtID);
                    relativeLines = datareadwrite.GetDataTableBySQL(sql_queryitem);
                    //查询到结果后进行填充
                    if(relativeLines.Rows.Count>0)
                    {
                        item.StartPoint = Converters.ConvertWKTToGeometry(relativeLines.Rows[0]["startpoint"] as string) as IPoint;
                        item.StartLayer = relativeLines.Rows[0]["startlayer"] as string;
                        item.TargetPoint = Converters.ConvertWKTToGeometry(relativeLines.Rows[0]["targetpoint"] as string) as IPoint;
                        item.TargetLayer = relativeLines.Rows[0]["targetlayer"] as string;
                    }
                }
            }
        }

        public void Write()
        {
            if (relativelineList == null)
                return;

            foreach (RelativeLineItem item in relativelineList)
            {
                item.RelativeLineSave(localProjectid, localMapnumber, localCurrentuser);
            }
        }
    }
    
    //用于存储搜索匹配、吸附的检测线，可以实时改写检测线信息；
    public class CheckLineItem
    {
        public string PtID { set; get; }
        public double SX { set; get; }
        public double SY { set; get; }
        public double SZ { set; get; }
        public CheckPointType PointType { set; get; }
        public double PlanError { set; get; }
        public double HeightError { set; get; }
        public string Comment { set; get; }
        public List<BufferSearchTargetItem> BufferSearchResults { set; get; }

        //将发生改变的检测线进行更新，暂不支持新增、删除记录，仅更新记录
        public void CheckLineValueSave(string projectid,string szMapNumber,string currentuser,CheckPointType pointtype,double planerror, double heighterror, List<BufferSearchTargetItem> buffersearchresult)
        {
            this.PointType = pointtype;
            this.PlanError = planerror;
            this.HeightError = heighterror;
            if(buffersearchresult!=null)
            {
                if (this.BufferSearchResults != null)
                    this.BufferSearchResults.Clear();
                else
                    this.BufferSearchResults = new List<BufferSearchTargetItem>();

                foreach (BufferSearchTargetItem bsti in buffersearchresult)
                {
                    this.BufferSearchResults.Add(bsti);
                }
            }
            /////////////更新到数据库
            string CheckLineCollectionTableName = "checklinecollection";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            //逐条记录到数据表SearchTargetSetting
            string sql_queryitem = string.Format("select ptid from {0} where ProjectID = '{1}' and ptid='{2}' and sx={3} and sy={4} and sz={5}", CheckLineCollectionTableName, projectid, this.PtID, this.SX, this.SY, this.SZ);
            object o = datareadwrite.GetScalar(sql_queryitem);
            if (o == null)//insert
            {
                string sql_insert = string.Format("insert into {0} Values('{1}','{2}',{3},{4},{5},'{6}',{7},{8})", CheckLineCollectionTableName, projectid, this.PtID, this.SX, this.SY, this.SZ, this.PointType, this.PlanError, this.HeightError);
                datareadwrite.ExecuteSQL(sql_insert);
                //记录搜索到的目标
            }
            else // update
            {
                string sql_update = string.Format("update {0} set  pointtype ='{1}',planerror = {2},heighterror={3} where projectid = '{4}' and ptid='{5}' and sx={6} and sy={7} and sz={8}", CheckLineCollectionTableName, this.PointType, this.PlanError, this.HeightError, projectid, this.PtID, this.SX, this.SY, this.SZ);
                datareadwrite.ExecuteSQL(sql_update);
            }

            ///////////////////////////////////////////////////////////////////////
            string buffersearchresultTableName = "buffersearchresults";
            //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
            List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
            if (tableNames2.Contains(buffersearchresultTableName) == false)
            {
                string sql_createTable = string.Format("create table {0}( ProjectID text,ptid text,sx numeric(19,11),sy numeric(19,11),sz numeric(19,11),layer text, pointtype text,distance numeric(19,11),height numeric(19,11),ptargetpoint text,shape text)", buffersearchresultTableName);
                datareadwrite.ExecuteSQL(sql_createTable);
                //记录
            }
            ///////////////////////////////////////////////////////////////////
            //删除这个点所有搜索目标，为了重新记录
            string sql_select = string.Format("select * from {0} where projectid='{1}' and ptid='{2}' and sx = {3} and sy = {4} and sz = {5}", buffersearchresultTableName, projectid, this.PtID, this.SX, this.SY, this.SZ);
            o = datareadwrite.GetScalar(sql_select);
            if (o != null)
            {
                string sql_delete = string.Format("delete  from {0} where projectid = '{1}' and ptid = '{2}' and sx = {3} and sy = {4} and sz = {5}", buffersearchresultTableName, projectid, this.PtID, this.SX, this.SY, this.SZ);
                datareadwrite.ExecuteSQL(sql_delete);
            }

            //插入新纪录
            if (this.BufferSearchResults != null && this.BufferSearchResults.Count > 0)
            {
                foreach (BufferSearchTargetItem bufferitem in this.BufferSearchResults)
                {
                    string ptargetpointwkt = "";
                    if (bufferitem.pTargetPoint != null)
                    {
                        ptargetpointwkt = Converters.ConvertGeometryToWKT(bufferitem.pTargetPoint);
                    }
                    string shapewkt = "";
                    if (bufferitem.Shape != null)
                    {
                        shapewkt = Converters.ConvertGeometryToWKT(bufferitem.Shape);
                    }
                    string sql_insert = string.Format("insert into {0} values('{1}','{2}',{3},{4},{5},'{6}','{7}',{8},{9},'{10}','{11}')", buffersearchresultTableName, projectid, this.PtID, this.SX, this.SY, this.SZ, bufferitem.Layer, bufferitem.PointType, bufferitem.Distance, bufferitem.Height, ptargetpointwkt, shapewkt);
                    datareadwrite.ExecuteSQL(sql_insert);
                }
            }

            ///////////////////////////////////////////////
            //将记录为PlanCheck 和 HeightCheck的检测线结果记录到“平面及高程精度检测点成果表”中
            string PlanHeightcheckpointTable = "平面及高程精度检测点成果表";
            if (this.PointType == CheckPointType.HeightCheck || this.PointType == CheckPointType.PlanCheck)
            {
                //不存在表，创建
                List<string> tableNames3 = datareadwrite.GetSchameDataTableNames();
                if (tableNames3.Contains(PlanHeightcheckpointTable) == false)
                {
                    //创建表
                    string sql_createTable = string.Format("create table {0}(mapnumber character varying(255) NOT NULL, ptid character varying(50) NOT NULL, dl numeric(6, 3), dh numeric(6, 3), dc numeric(6, 3), x double precision, y double precision, h double precision, \"检查者\" character varying(255), \"检查日期\" date, comment character varying(255))", PlanHeightcheckpointTable);
                    datareadwrite.ExecuteSQL(sql_createTable);
                }
                string dl = this.PointType == CheckPointType.PlanCheck ? Convert.ToString(this.PlanError) : "NULL";
                string dh = this.PointType == CheckPointType.HeightCheck ? Convert.ToString(this.HeightError) : "NULL";
                //存在则先查询该记录是否存在，有则更新，否则插入，较为简单。//
                string sql_queryckpt = string.Format("select ptid from {0} where mapnumber = '{1}' and ptid='{2}'and x={3} and y={4} and h={5} ", PlanHeightcheckpointTable, szMapNumber, this.PtID, this.SX, this.SY, this.SZ);
                o = datareadwrite.GetScalar(sql_queryckpt);
                if (o == null)//insert
                {
                    string sql_insert = string.Format("insert into {0} Values('{1}','{2}',{3},{4},NULL,{5},{6},{7},'{8}','{9}','')", PlanHeightcheckpointTable, szMapNumber, this.PtID, dl, dh, this.SX, this.SY, this.SZ, currentuser, DateTime.Now.ToShortDateString());
                    datareadwrite.ExecuteSQL(sql_insert);
                    //记录搜索到的目标
                }
                else // update//
                {
                    string sql_update = string.Format("update {0} set  dl = {1},dh={2},\"检查者\"='{3}',\"检查日期\"='{4}',comment='{5}' where mapnumber = '{6}' and ptid='{7}' and x={8} and y={9} and h={10}", PlanHeightcheckpointTable, dl, dh, currentuser, DateTime.Now.ToShortDateString(), "", szMapNumber, this.PtID, this.SX, this.SY, this.SZ);
                    datareadwrite.ExecuteSQL(sql_update);
                }
            }
            else if(this.PointType==CheckPointType.WaitToCheck)
            {
                //重新设置为WaitToCheck点后，将“平面及高程精度检测点成果表”中的该点记录进行删除处理
                //存在则先查询该记录是否存在，有则更新，否则插入，较为简单。//
                string sql_queryckpt = string.Format("select ptid from {0} where mapnumber = '{1}' and ptid='{2}'and x={3} and y={4} and h={5} ", PlanHeightcheckpointTable, szMapNumber, this.PtID, this.SX, this.SY, this.SZ);
                o = datareadwrite.GetScalar(sql_queryckpt);
                if (o != null)//存在点记录值delete
                {
                    string sql_delete = string.Format("delete from {0} where mapnumber = '{1}' and ptid='{2}'and x={3} and y={4} and h={5} ", PlanHeightcheckpointTable, szMapNumber, this.PtID, this.SX, this.SY, this.SZ);
                    datareadwrite.ExecuteSQL(sql_delete);
                }
            }
        }
        //public event CheckLineValueChanged OnCheckLineValueChange;
    }
    //public delegate void CheckLineValueChanged(string projectid, string szMapNumber, string currentuser, double planerror, double heighterror, List<BufferSearchTargetItem> buffersearchresult);

    //用于存储高程、平面检测线集合，可以读取、写入数据库
    public class CheckLineCollection
    {
        public List<CheckLineItem> checklineList;
        public double PlanMeanError { set; get; }
        public double HeightMeanError { set; get; }
        string localProjectid = "";
        public CheckLineCollection(string projectid)
        {
            localProjectid = projectid;
        }
        CheckPointType CheckPointTypeFromString(string checkpointtypestring)
        {
            CheckPointType type = CheckPointType.WaitToCheck; ;
            switch (checkpointtypestring)
            {
                case "PlanCheck":
                    type = CheckPointType.PlanCheck;
                    break;
                case "HeightCheck":
                    type = CheckPointType.HeightCheck;
                    break;
                case "ControlPoint":
                    type = CheckPointType.ControlPoint;
                    break;
                case "WaitToCheck":
                    type = CheckPointType.WaitToCheck;
                    break;
                case "NonCheck":
                    type = CheckPointType.NonCheck;
                    break;
            }
            return type;
        }
        public void Read()
        {
            if (checklineList == null)
                return;

            string CheckLineCollectionTableName = "checklinecollection";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            List<string> tableNames = datareadwrite.GetSchameDataTableNames();
            if (tableNames.Contains(CheckLineCollectionTableName) == false)
                return;

            ////////////////////////////////////////////////////////////////////
            foreach (CheckLineItem item in  checklineList)
            {
                string sql_queryitem = string.Format("select * from {0} where ProjectID = '{1}' and ptid='{2}' and sx={3} and sy={4} and sz={5}", CheckLineCollectionTableName, localProjectid, item.PtID, item.SX, item.SY, item.SZ);
                DataTable datatable = datareadwrite.GetDataTableBySQL(sql_queryitem);
                if (datatable.Rows.Count == 1)
                {
                    DataRow datarow = datatable.Rows[0];
                    item.PointType = CheckPointTypeFromString(datarow["pointtype"] as string);
                    item.PlanError = Convert.ToDouble(datarow["planerror"]);
                    item.HeightError = Convert.ToDouble(datarow["heighterror"]);

                    string buffersearchresultTableName = "buffersearchresults";
                    //如果存在表
                    List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
                    if (tableNames2.Contains(buffersearchresultTableName) == true)
                    {
                        string sql_select = string.Format("select * from {0} where projectid='{1}' and ptid='{2}' and sx = {3} and sy = {4} and sz = {5}", buffersearchresultTableName, localProjectid, item.PtID, item.SX, item.SY, item.SZ);
                        DataTable datatable2 = datareadwrite.GetDataTableBySQL(sql_select);
                        if (datatable2.Rows.Count > 0) item.BufferSearchResults = new List<BufferSearchTargetItem>();

                        foreach (DataRow dr in datatable2.Rows)
                        {
                            BufferSearchTargetItem bufferitem = new BufferSearchTargetItem();

                            bufferitem.Layer = dr["layer"] as string;
                            bufferitem.PointType = CheckPointTypeFromString(dr["pointtype"] as string);
                            bufferitem.Distance = Convert.ToDouble(dr["distance"]);
                            bufferitem.Height = Convert.ToDouble(dr["height"]);
                            bufferitem.pTargetPoint = Converters.ConvertWKTToGeometry(dr["ptargetpoint"] as string) as IPoint;
                            bufferitem.Shape = Converters.ConvertWKTToGeometry(dr["shape"] as string);

                            item.BufferSearchResults.Add(bufferitem);
                        }
                    }
                }
            }
        }
        public void Write(string szMapNumber,string currentuser)
        {
            string CheckLineCollectionTableName = "checklinecollection";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            List<string> tableNames = datareadwrite.GetSchameDataTableNames();
            if (tableNames.Contains(CheckLineCollectionTableName) == false)
            {
                //创建表
                string sql_createTable = string.Format("create table {0}( ProjectID text,ptid text,sx numeric(19,11),sy numeric(19,11),sz numeric(19,11), pointtype text,planerror numeric(19,11),heighterror numeric(19,11))", CheckLineCollectionTableName);
                datareadwrite.ExecuteSQL(sql_createTable);
            }
            else
            {
                foreach(CheckLineItem item in checklineList)
                {
                    //逐条记录到数据表SearchTargetSetting
                    string sql_queryitem = string.Format("select ptid from {0} where ProjectID = '{1}' and ptid='{2}' and sx={3} and sy={4} and sz={5}", CheckLineCollectionTableName, localProjectid, item.PtID, item.SX,item.SY,item.SZ);
                    object o = datareadwrite.GetScalar(sql_queryitem);
                    if (o == null)//insert
                    {
                        string sql_insert = string.Format("insert into {0} Values('{1}','{2}',{3},{4},{5},'{6}',{7},{8})", CheckLineCollectionTableName, localProjectid, item.PtID, item.SX, item.SY, item.SZ, item.PointType, item.PlanError, item.HeightError);
                        datareadwrite.ExecuteSQL(sql_insert);
                        //记录搜索到的目标
                    }
                    else // update
                    {
                        string sql_update = string.Format("update {0} set  pointtype ='{1}',planerror = {2},heighterror={3} where projectid = '{4}' and ptid='{5}' and sx={6} and sy={7} and sz={8}", CheckLineCollectionTableName, item.PointType,item.PlanError,item.HeightError, localProjectid, item.PtID, item.SX, item.SY, item.SZ);
                        datareadwrite.ExecuteSQL(sql_update);
                    }

                    ///////////////////////////////////////////////////////////////////////
                    string buffersearchresultTableName = "buffersearchresults";
                    //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
                    List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
                    if (tableNames2.Contains(buffersearchresultTableName) == false)
                    {
                        string sql_createTable = string.Format("create table {0}( ProjectID text,ptid text,sx numeric(19,11),sy numeric(19,11),sz numeric(19,11),layer text, pointtype text,distance numeric(19,11),height numeric(19,11),ptargetpoint text,shape text)", buffersearchresultTableName);
                        datareadwrite.ExecuteSQL(sql_createTable);
                        //记录
                    }
                    ///////////////////////////////////////////////////////////////////
                    //删除这个点所有搜索目标，为了重新记录
                    string sql_select = string.Format("select * from {0} where projectid='{1}' and ptid='{2}' and sx = {3} and sy = {4} and sz = {5}", buffersearchresultTableName, localProjectid, item.PtID, item.SX, item.SY, item.SZ);
                    o = datareadwrite.GetScalar(sql_select);
                    if(o!=null)
                    {
                        string sql_delete = string.Format("delete  from {0} where projectid = '{1}' and ptid = '{2}' and sx = {3} and sy = {4} and sz = {5}", buffersearchresultTableName, localProjectid, item.PtID, item.SX, item.SY, item.SZ);
                        datareadwrite.ExecuteSQL(sql_delete);
                    }

                    //插入新纪录
                    if (item.BufferSearchResults!=null && item.BufferSearchResults.Count>0)
                    {
                        foreach(BufferSearchTargetItem bufferitem in item.BufferSearchResults)
                        {
                            string ptargetpointwkt = "";
                            if (bufferitem.pTargetPoint!=null)
                            {
                                ptargetpointwkt = Converters.ConvertGeometryToWKT(bufferitem.pTargetPoint);
                            }
                            string shapewkt = "";
                            if(bufferitem.Shape!=null)
                            {
                                shapewkt = Converters.ConvertGeometryToWKT(bufferitem.Shape);
                            }
                            string sql_insert = string.Format("insert into {0} values('{1}','{2}',{3},{4},{5},'{6}','{7}',{8},{9},'{10}','{11}')", buffersearchresultTableName, localProjectid, item.PtID, item.SX, item.SY, item.SZ,bufferitem.Layer,bufferitem.PointType,bufferitem.Distance,bufferitem.Height,ptargetpointwkt,shapewkt);
                            datareadwrite.ExecuteSQL(sql_insert);
                        }
                    }

                    ///////////////////////////////////////////////
                    //将记录为PlanCheck 和 HeightCheck的检测线结果记录到“平面及高程精度检测点成果表”中
                    string PlanHeightcheckpointTable = "平面及高程精度检测点成果表";
                    if(item.PointType == CheckPointType.HeightCheck || item.PointType==CheckPointType.PlanCheck)
                    {
                        //不存在表，创建
                        List<string> tableNames3 = datareadwrite.GetSchameDataTableNames();
                        if (tableNames3.Contains(PlanHeightcheckpointTable) == false)
                        {
                            //创建表
                            string sql_createTable = string.Format("create table {0}(mapnumber character varying(255) NOT NULL, ptid character varying(50) NOT NULL, dl numeric(6, 3), dh numeric(6, 3), dc numeric(6, 3), x double precision, y double precision, h double precision, \"检查者\" character varying(255), \"检查日期\" date, comment character varying(255))", PlanHeightcheckpointTable);
                            datareadwrite.ExecuteSQL(sql_createTable);
                        }
                        string dl = item.PointType == CheckPointType.PlanCheck ? Convert.ToString(item.PlanError): "NULL";
                        string dh = item.PointType == CheckPointType.HeightCheck ? Convert.ToString(item.HeightError) : "NULL";
                        //存在则先查询该记录是否存在，有则更新，否则插入，较为简单。//
                        string sql_queryckpt = string.Format("select ptid from {0} where mapnumber = '{1}' and ptid='{2}'and x={3} and y={4} and h={5} ", PlanHeightcheckpointTable, szMapNumber, item.PtID, item.SX, item.SY, item.SZ);
                        o = datareadwrite.GetScalar(sql_queryckpt);
                        if (o == null)//insert
                        {
                            string sql_insert = string.Format("insert into {0} Values('{1}','{2}',{3},{4},NULL,{5},{6},{7},'{8}','{9}','')", PlanHeightcheckpointTable, szMapNumber, item.PtID, dl, dh, item.SX, item.SY, item.SZ,currentuser,DateTime.Now.ToShortDateString());
                            datareadwrite.ExecuteSQL(sql_insert);
                            //记录搜索到的目标
                        }
                        else // update//
                        {
                            string sql_update = string.Format("update {0} set  dl = {1},dh={2},\"检查者\"='{3}',\"检查日期\"='{4}',comment='{5}' where mapnumber = '{6}' and ptid='{7}' and x={8} and y={9} and h={10}", PlanHeightcheckpointTable, dl, dh, currentuser, DateTime.Now.ToShortDateString(),"",szMapNumber,item.PtID, item.SX, item.SY, item.SZ);
                            datareadwrite.ExecuteSQL(sql_update);
                        }
                    }
                }
            }
        }

        public void Initialize(IFeatureLayer pScaterLayer, IGeometry mapBind)
        {
            checklineList = new List<CheckLineItem>();
            IFeatureClass pFeatureClass = pScaterLayer.FeatureClass;
            IGeoFeatureLayer geoFeatureLayer = pScaterLayer as IGeoFeatureLayer;
            ESRI.ArcGIS.Geodatabase.ISpatialFilter spatialFilter = new ESRI.ArcGIS.Geodatabase.SpatialFilterClass();
            spatialFilter.Geometry = mapBind;

            //设置空间查询关系
            spatialFilter.SpatialRel = ESRI.ArcGIS.Geodatabase.esriSpatialRelEnum.esriSpatialRelContains;

            IFeatureCursor pCursor = geoFeatureLayer.Search(spatialFilter, false);

            IFeature pFeature = pCursor.NextFeature();
            while(pFeature!=null)
            {
                int nProjectidIndex = pFeature.Fields.FindField("projectid");
                string projectidstring = pFeature.Value[nProjectidIndex] as string;
                if(localProjectid == projectidstring)
                {
                    CheckLineItem checkitem = new CheckLineItem();
                    int nIndex = pFeature.Fields.FindField("ptid");
                    checkitem.PtID = pFeature.Value[nIndex] as string;
                    checkitem.PointType = CheckPointType.WaitToCheck;

                    nIndex = pFeature.Fields.FindField("sx");
                    checkitem.SX = Convert.ToDouble(pFeature.Value[nIndex]);

                    nIndex = pFeature.Fields.FindField("sy");
                    checkitem.SY = Convert.ToDouble(pFeature.Value[nIndex]);

                    nIndex = pFeature.Fields.FindField("sz");
                    checkitem.SZ = Convert.ToDouble(pFeature.Value[nIndex]);


                    checklineList.Add(checkitem);
                } 
                pFeature = pCursor.NextFeature();
            }

            this.Read();
        }
    }

    public enum CheckPointType
    {
        PlanCheck,
        HeightCheck,
        ControlPoint,
        WaitToCheck,
        NonCheck
    }


    //图层显示、搜索、吸附配置表，具有实时改写，表格化显示；读取、写入数据库功能；
    public class DwgLayerInfoItem:IEquatable<DwgLayerInfoItem>
    {
        public DwgLayerInfoItem()
        {
        }
        public string LayerName { set; get; }
        public string Layer { set; get; }
        public string Name { set; get; }
        public string Color { set; get; }
        public string ColorValue { set; get; }
        public bool PlanSearch { set; get; }
        public bool HeightSearch { set; get; }
        public IColor AECurrentColor { set; get; }
        public IColor ColorToIColor(Color color)
        {
            IColor pColor = new RgbColorClass();
            pColor.RGB = color.B * 65536 + color.G * 256 + color.R;
            return pColor;
        }
        public Color ColorStringToColor(string colorstring)
        {
            if (colorstring == null || colorstring == "")
            {
                colorstring = "0,0,0";
            }
            string[] rgb = colorstring.Split(',');
            int r =Convert.ToInt32( rgb[0]);
            int g = Convert.ToInt32(rgb[1]);
            int b = Convert.ToInt32(rgb[2]);
            Color color = System.Drawing.Color.FromArgb(r, g, b);
            return color;
        }
        public bool Equals(DwgLayerInfoItem other)
        {
            return this.LayerName + this.Layer == other.LayerName + other.Layer;
        }
        public override int GetHashCode()
        {
            return Layer.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("name:{0},\taddress:{1}", LayerName, Layer);
        }
    }
    public class SearchTargetSetting
    {
        public void Read()
        {
            ElevSearchsetting.Read();

            string searchtargetsettingTableName = "searchtargetsetting";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            List<string> tableNames = datareadwrite.GetSchameDataTableNames();
            if (tableNames.Contains(searchtargetsettingTableName) == true)
            {
                string sql_searchtargetsetting = string.Format("select layername , layer , name ,color , colorvalue ,plansearch,heightsearch from {0} where projectid = '{1}'", searchtargetsettingTableName, localProjectID);
                DataTable datatable = datareadwrite.GetDataTableBySQL(sql_searchtargetsetting);
                if(datatable.Rows.Count>0)
                {
                    DwglayerinfoList = new List<DwgLayerInfoItem>();
                    foreach (DataRow datarow in datatable.Rows)
                    {
                        DwgLayerInfoItem item = new DwgLayerInfoItem();
                        item.LayerName = datarow["layername"] as string;
                        item.Layer = datarow["layer"] as string;
                        item.Name = datarow["name"] as string;
                        item.Color = datarow["color"] as string;
                        item.ColorValue = datarow["colorvalue"] as string;
                        item.PlanSearch = Convert.ToBoolean(datarow["plansearch"]);
                        item.HeightSearch = Convert.ToBoolean(datarow["heightsearch"]);
                        Color color = item.ColorStringToColor(item.ColorValue);
                        item.AECurrentColor = item.ColorToIColor(color);

                        DwglayerinfoList.Add(item);
                    }
                }
            }
        }
        public void Write()
        {
            ElevSearchsetting.Write();

            string searchtargetsettingTableName = "searchtargetsetting";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            List<string> tableNames = datareadwrite.GetSchameDataTableNames();
            if(tableNames.Contains(searchtargetsettingTableName)==false)
            {
                //创建表
                string sql_createTable = string.Format("create table {0}( ProjectID text,LayerName text,Layer text,Name text,Color text,ColorValue text,PlanSearch boolean,HeightSearch boolean )", searchtargetsettingTableName);
                datareadwrite.ExecuteSQL(sql_createTable);
            }
            else
            {
                //逐条记录到数据表SearchTargetSetting
                foreach (DwgLayerInfoItem item in DwglayerinfoList)
                {
                    string sql_queryitem = string.Format("select Layer from {0} where ProjectID = '{1}' and LayerName='{2}' and Layer='{3}' ", searchtargetsettingTableName, localProjectID, item.LayerName, item.Layer);
                    object o = datareadwrite.GetScalar(sql_queryitem);
                    if(o==null)//insert
                    {
                        string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}',{7},{8})", searchtargetsettingTableName, localProjectID, item.LayerName , item.Layer , item.Name , item.Color , item.ColorValue , item.PlanSearch , item.HeightSearch );
                        datareadwrite.ExecuteSQL(sql_insert);

                    }else // update
                    {
                        string sql_update = string.Format("update {0} set  PlanSearch ={1},HeightSearch = {2} where ProjectID = '{3}' and LayerName='{4}' and Layer='{5}' ", searchtargetsettingTableName, item.PlanSearch, item.HeightSearch, localProjectID, item.LayerName, item.Layer);
                        datareadwrite.ExecuteSQL(sql_update);
                    }
                }
            }
        }
        public SearchTargetSetting(ESRI.ArcGIS.Carto.IMap map,string projectid)
        {
            localProjectID = projectid;
            //首先从数据库读取
            if (ElevSearchsetting == null)
                ElevSearchsetting = new ElevationSearchSetting(projectid);

            LayerRendererList = new Dictionary<string, IFeatureRenderer>();

            this.Read();
            //读取不到的从图中创建
            if(DwglayerinfoList == null)
            {
                Initialize(map);
            }            
        }
        string localProjectID = "";
        public List<DwgLayerInfoItem> DwglayerinfoList { set; get; }
        public Dictionary<string, IFeatureRenderer> LayerRendererList { set; get; }
        public ElevationSearchSetting ElevSearchsetting { set; get; }
        public void Initialize(ESRI.ArcGIS.Carto.IMap map)
        {
            DwglayerinfoList = new List<DwgLayerInfoItem>();

            int nLayerCount = map.LayerCount;
            for (int i = 0; i < nLayerCount; i++)
            {
                string LayerName = map.Layer[i].Name;
                //对无关的图层进行过滤，比如Annotation图层是不允许进行符号化显示的
                if (LayerName != "Point" && LayerName != "Polyline")
                    continue;

                IFeatureLayer pFeatureLayer = map.Layer[i] as IFeatureLayer;
                if (pFeatureLayer == null)
                    continue;

                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                IFields fields = pFeatureClass.Fields;
                int nFieldCount = fields.FieldCount;
                int nIndex = fields.FindField("Layer");
                if (nIndex < 0) continue;

                //提取dwglayer 中layer字段的值，加入列表
                string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
                SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

                IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);

                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = "";
                IFeatureCursor cursor = pFeatureClass.Search(filter, false);
                IFeature feature = cursor.NextFeature();


                while (feature != null)
                {
                    string Layer = feature.Value[nIndex] as string;
                    string sql_Layer = string.Format("select {0},{1},{2} from dwglayerdefine where {3}='{4}'", datareadwrite.GetFieldString("Name"), datareadwrite.GetFieldString("Color"), datareadwrite.GetFieldString("ColorValue"), datareadwrite.GetFieldString("Layer"), Layer);
                    DataTable dt = datareadwrite.GetDataTableBySQL(sql_Layer);
                    DwgLayerInfoItem dwglayerinfoitem = new DwgLayerInfoItem();
                    string Name = "";

                    if (dt.Rows.Count != 0)
                    {
                        DataRow dr = dt.Rows[0];
                        Name = Convert.ToString(dr["Name"]);
                        dwglayerinfoitem.Color = Convert.ToString(dr["Color"]);
                        dwglayerinfoitem.ColorValue = Convert.ToString(dr["ColorValue"]);
                        Color color = dwglayerinfoitem.ColorStringToColor(dwglayerinfoitem.ColorValue);
                        dwglayerinfoitem.AECurrentColor = dwglayerinfoitem.ColorToIColor(color);
                    }

                    dwglayerinfoitem.Layer = Layer;
                    dwglayerinfoitem.Name = Name;
                    dwglayerinfoitem.LayerName = LayerName;

                    dwglayerinfoitem.HeightSearch = false;
                    dwglayerinfoitem.PlanSearch = false;

                    DwglayerinfoList.Add(dwglayerinfoitem);

                    feature = cursor.NextFeature();
                }
            }
            DwglayerinfoList = DwglayerinfoList.Distinct().ToList();

        }

        //唯一值渲染
        public void RenderSearchTargetLayer(IFeatureLayer pFeatureLayer)//ESRI.ArcGIS.Carto.IMap map)
        {
            if (pFeatureLayer == null)
                return;

            //string LayerName = layinfo.LayerName;
            //IFeatureLayer pFeatureLayer = ArcGISHelper.GetFeatureLayerByName(LayerName, map);
            //IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;

            foreach (DwgLayerInfoItem layinfo in this.DwglayerinfoList)
            {
                if (layinfo.LayerName != pFeatureLayer.Name)
                    continue; 

                IGeoFeatureLayer pGeoFeatureLayer = pFeatureLayer as IGeoFeatureLayer;
                IUniqueValueRenderer pGeoFeatureLayerRenderer = pGeoFeatureLayer.Renderer as IUniqueValueRenderer; 

                switch (layinfo.LayerName)
                {
                    case "Polyline":
                        {
                            if (layinfo.HeightSearch == true || layinfo.PlanSearch == true)
                            {
                                if (pGeoFeatureLayerRenderer == null)
                                {
                                    IUniqueValueRenderer pUniqueValueRenderer = new UniqueValueRendererClass();
                                    //layinfo.AEOldRender = pGeoFeatureLayer.Renderer;
                                    if(LayerRendererList.ContainsKey(layinfo.LayerName)==false)
                                        LayerRendererList.Add(layinfo.LayerName, pGeoFeatureLayer.Renderer);

                                    pGeoFeatureLayer.Renderer = pUniqueValueRenderer as IFeatureRenderer;

                                    //设置渲染字段对象        
                                    pUniqueValueRenderer.FieldCount = 1;
                                    pUniqueValueRenderer.set_Field(0, "Layer");
                                    //创建填充符号
                                    ISimpleLineSymbol pMarkerSymbol = new SimpleLineSymbolClass();
                                    pMarkerSymbol.Color = layinfo.AECurrentColor;
                                    pMarkerSymbol.Width = 1;
                                    pUniqueValueRenderer.DefaultSymbol = (ISymbol)pMarkerSymbol;
                                    pUniqueValueRenderer.UseDefaultSymbol = false;

                                    pUniqueValueRenderer.AddValue(layinfo.Layer, "Layer", pMarkerSymbol as ISymbol); 
                                }
                                else
                                {
                                    IUniqueValueRenderer pUniqueValueRenderer = pGeoFeatureLayerRenderer as IUniqueValueRenderer;
                                    //创建填充符号
                                    ISimpleLineSymbol pMarkerSymbol = new SimpleLineSymbolClass();
                                    pMarkerSymbol.Color = layinfo.AECurrentColor;
                                    pMarkerSymbol.Width = 1;
                                    pUniqueValueRenderer.DefaultSymbol = (ISymbol)pMarkerSymbol;
                                    pUniqueValueRenderer.UseDefaultSymbol = false;

                                    pUniqueValueRenderer.AddValue(layinfo.Layer, "Layer", pMarkerSymbol as ISymbol);
                                }
                            }
                            else
                            {
                                if (pGeoFeatureLayerRenderer == null)
                                {
                                    IUniqueValueRenderer pUniqueValueRenderer = new UniqueValueRendererClass();
                                    //layinfo.AEOldRender = pGeoFeatureLayer.Renderer;
                                    if (LayerRendererList.ContainsKey(layinfo.LayerName) == false)
                                        LayerRendererList.Add(layinfo.LayerName, pGeoFeatureLayer.Renderer);

                                    pGeoFeatureLayer.Renderer = pUniqueValueRenderer as IFeatureRenderer;

                                    //设置渲染字段对象        
                                    pUniqueValueRenderer.FieldCount = 1;
                                    pUniqueValueRenderer.set_Field(0, "Layer");
                                    //创建填充符号
                                    ISimpleLineSymbol pMarkerSymbol = new SimpleLineSymbolClass();
                                    pMarkerSymbol.Color = layinfo.ColorToIColor(Color.Black);
                                    pMarkerSymbol.Width = 1;
                                    pUniqueValueRenderer.DefaultSymbol = (ISymbol)pMarkerSymbol;
                                    pUniqueValueRenderer.UseDefaultSymbol = false;

                                    pUniqueValueRenderer.AddValue(layinfo.Layer, "Layer", pMarkerSymbol as ISymbol);
                                }
                                else
                                {
                                    IUniqueValueRenderer pUniqueValueRenderer = pGeoFeatureLayerRenderer as IUniqueValueRenderer;
                                    //创建填充符号
                                    ISimpleLineSymbol pMarkerSymbol = new SimpleLineSymbolClass();
                                    pMarkerSymbol.Color = layinfo.ColorToIColor(Color.Black);
                                    pMarkerSymbol.Width = 1;
                                    pUniqueValueRenderer.DefaultSymbol = (ISymbol)pMarkerSymbol;
                                    pUniqueValueRenderer.UseDefaultSymbol = false;

                                    pUniqueValueRenderer.AddValue(layinfo.Layer, "Layer", pMarkerSymbol as ISymbol);
                                }
                            }
                        }
                        break;
                    case "Polygon":
                        break;
                    case "Point":
                        {
                            if (layinfo.HeightSearch == true || layinfo.PlanSearch == true)
                            {
                                if (pGeoFeatureLayerRenderer == null)
                                {
                                    IUniqueValueRenderer pUniqueValueRenderer = new UniqueValueRendererClass();
                                    //layinfo.AEOldRender = pGeoFeatureLayer.Renderer;
                                    if (LayerRendererList.ContainsKey(layinfo.LayerName) == false)
                                        LayerRendererList.Add(layinfo.LayerName, pGeoFeatureLayer.Renderer);

                                    pGeoFeatureLayer.Renderer = pUniqueValueRenderer as IFeatureRenderer;
                                    //设置渲染字段对象        
                                    pUniqueValueRenderer.FieldCount = 1;
                                    pUniqueValueRenderer.set_Field(0, "Layer");
                                    //创建填充符号
                                    ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
                                    pMarkerSymbol.Color = layinfo.AECurrentColor;
                                    pMarkerSymbol.Size = 2;
                                    pUniqueValueRenderer.DefaultSymbol = (ISymbol)pMarkerSymbol;
                                    pUniqueValueRenderer.UseDefaultSymbol = false;

                                    pUniqueValueRenderer.AddValue(layinfo.Layer, "Layer", pMarkerSymbol as ISymbol);
                                }
                                else
                                {
                                    IUniqueValueRenderer pUniqueValueRenderer = pGeoFeatureLayerRenderer as IUniqueValueRenderer;
                                    //创建填充符号
                                    ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
                                    pMarkerSymbol.Color = layinfo.AECurrentColor;
                                    pMarkerSymbol.Size = 2;
                                    pUniqueValueRenderer.DefaultSymbol = (ISymbol)pMarkerSymbol;
                                    pUniqueValueRenderer.UseDefaultSymbol = false;

                                    pUniqueValueRenderer.AddValue(layinfo.Layer, "Layer", pMarkerSymbol as ISymbol);
                                }
                            }
                            else
                            {
                                if (pGeoFeatureLayerRenderer == null)
                                {
                                    IUniqueValueRenderer pUniqueValueRenderer = new UniqueValueRendererClass();
                                    //layinfo.AEOldRender = pGeoFeatureLayer.Renderer;
                                    if (LayerRendererList.ContainsKey(layinfo.LayerName) == false)
                                        LayerRendererList.Add(layinfo.LayerName, pGeoFeatureLayer.Renderer);

                                    pGeoFeatureLayer.Renderer = pUniqueValueRenderer as IFeatureRenderer;
                                    //设置渲染字段对象        
                                    pUniqueValueRenderer.FieldCount = 1;
                                    pUniqueValueRenderer.set_Field(0, "Layer");
                                    //创建填充符号
                                    ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
                                    pMarkerSymbol.Color = layinfo.ColorToIColor(Color.Black);
                                    pMarkerSymbol.Size = 2;
                                    pUniqueValueRenderer.DefaultSymbol = (ISymbol)pMarkerSymbol;
                                    pUniqueValueRenderer.UseDefaultSymbol = false;

                                    pUniqueValueRenderer.AddValue(layinfo.Layer, "Layer", pMarkerSymbol as ISymbol);
                                }
                                else
                                {
                                    IUniqueValueRenderer pUniqueValueRenderer = pGeoFeatureLayerRenderer as IUniqueValueRenderer;
                                    //创建填充符号
                                    ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
                                    pMarkerSymbol.Color = layinfo.ColorToIColor(Color.Black);
                                    pMarkerSymbol.Size = 2;
                                    pUniqueValueRenderer.DefaultSymbol = (ISymbol)pMarkerSymbol;
                                    pUniqueValueRenderer.UseDefaultSymbol = false;

                                    pUniqueValueRenderer.AddValue(layinfo.Layer, "Layer", pMarkerSymbol as ISymbol);
                                }
                            }
                        }
                        break;
                    case "MulitPatch":
                        break;
                    default:
                        break;
                }                
            }
        }        


    }
}

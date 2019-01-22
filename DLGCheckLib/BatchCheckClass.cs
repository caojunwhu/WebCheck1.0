using DatabaseDesignPlus;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DLGCheckLib
{
    public class SelectLayerPara
    {
        public bool Selected { set; get; }
        public string LayerName { set; get; }
    }
    public class BatchCheckClass
    {
        public List<CheckItem> checkItems { set; get; }
        public string ProjectID { set; get; }
        public string MapNumber { set; get; }
        IMap localMap { set; get; }
        AxMapControl localMapControl { set; get; }
        public IWorkspace localWorkspace { set; get; }
        IDatabaseReaderWriter DataReadWrite { set; get; }
        public BatchCheckClass(DataTable checkdatatable, IWorkspace pWorkSpace,IMap GlobeMap,AxMapControl GlobeMapControl, 
            IDatabaseReaderWriter datawrite, string projectid, string mapnumber)
        {
            localMap = GlobeMap;
            localWorkspace = pWorkSpace;
            localMapControl = GlobeMapControl;
            DataReadWrite = datawrite;
            ProjectID = projectid;
            MapNumber = mapnumber;

            checkItems = new List<CheckItem>();
            if (checkdatatable.Rows.Count <= 0 || pWorkSpace == null) return;

            foreach(DataRow r in checkdatatable.Rows)
            {
                
                string DescribeName = r["DescribeName"] as string;
                string ClassName = r["ClassName"] as string;
                string ParaTypes = r["ParaTypes"] as string;
                string Paras = r["Paras"] as string;
                bool tobecheck = Convert.ToBoolean( r["tobecheck"]);
                {
                    //此处应有一个开关，控制是否为拓扑检查项
                CheckItem item = new TopoCheckItem(GlobeMap);
                CheckClass chkclass= CheckClassFactory.CreateCheckClass(ClassName);
                item.TobeCheck = tobecheck;
                item.DescribeName = DescribeName;
                item.ClassName = ClassName;
                item.ParaTypes = ParaTypes;
                item.Paras = Paras;
                item.Category = GetTopoCheckCategory(item.ClassName, DataReadWrite);
                item.Checkentry = chkclass;
                item.pWorkSpace = pWorkSpace;
                item.topocheckrecordcollection = new TopocheckRecordCollections(projectid,mapnumber);

                checkItems.Add(item);
                }              

            }
        }
        private string GetTopoCheckCategory(string CheckClassName,IDatabaseReaderWriter datareader)
        {
            string CheckCategory = "";

            //DatabaseDesignPlus.IDatabaseReaderWriter datareader = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", CallClass.Databases["SurveryProductCheckDatabase"]);
            string sql = string.Format("select {0} from {1} where {2} ='{3}'", datareader.GetFieldString("Category"), datareader.GetTableName("拓扑检查项注册表"), datareader.GetFieldString("ClassName"), CheckClassName);
            CheckCategory = datareader.GetScalar(sql) as string;
            return CheckCategory;

        }
        public void Write(string projectid, IDatabaseReaderWriter datawrite)
        {
            if (checkItems.Count <= 0 || datawrite == null)
                return;

            //现将列表projectid的项删除
            string sql = string.Format("delete from {0} where projectid ='{1}'", datawrite.GetTableName("基础地理信息数据拓扑检查设置表"), projectid);
            datawrite.ExecuteSQL(sql);

            foreach (CheckItem chkitem in checkItems)
            {
                string insertsql = string.Format("insert into {0} (describename,classname,paratypes,paras,projectid,tobecheck)values('{1}','{2}','{3}','{4}','{5}','{6}')", datawrite.GetTableName("基础地理信息数据拓扑检查设置表"),chkitem.DescribeName,chkitem.ClassName,chkitem.ParaTypes,chkitem.Paras,projectid,chkitem.TobeCheck.ToString());
                datawrite.ExecuteSQL(insertsql);
            }
        }

        //写入检查意见记录表
        //对之前的记录进行清理，同样的描述的清除
        public void WriteCheckProblemRecord(string projectid, string szMapNumber, string currentuser)
        {
            foreach(CheckItem item in checkItems)
            {
                if (item.TobeCheck== false)
                    continue;

                if (ProjectID != projectid)
                    return;
                //删除原有记录
                //先将该样本中的问题记录进行清空
                ///////////////////////////////////////////////////////////////////////
                /////////////更新到数据库
                string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
                SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

                IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
                string CheckProblemTableName = "检查意见记录表";
                //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
                List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
                if (tableNames2.Contains(CheckProblemTableName) == false)
                {
                    string sql_createTable = string.Format("create table {0}( projectid text,  mapnumber text,  质量元素 text,  质量子元素 text,  错漏类别 text,  错漏描述 text,  处理意见 text,  修改情况 text,  复查情况 text,  检查者 text,  检查日期 text)", CheckProblemTableName);
                    datareadwrite.ExecuteSQL(sql_createTable);
                    //记录
                }
                
                //对检查记录进行更新，删除原记录，插入新记录
                foreach (TopocheckRecord record in item.topocheckrecordcollection.ResultList)
                {
                    if (szMapNumber != record.MapNumber)
                        return;

                    if (record.Checker == null || record.Checker == "")
                        record.Checker = currentuser;

                    if (record.Checker != currentuser)
                        return;


                    //逐条记录到数据表SearchTargetSetting
                    string sql_queryitem = string.Format("select mapnumber from {0} where projectid = '{1}' and mapnumber='{2}' and  质量元素='{3}' and 质量子元素='{4}' and 错漏描述='{5}'", CheckProblemTableName, ProjectID, MapNumber, item.Category, item.CheckType,record.Remark);
                    object o = datareadwrite.GetScalar(sql_queryitem);
                    if (o == null)//insert
                    {
                    }
                    else // delete
                    {
                        string sql_delete = string.Format("delete from {0} where  projectid = '{1}' and mapnumber='{2}' and  质量元素='{3}' and 质量子元素='{4}' and 错漏描述='{5}'", CheckProblemTableName, ProjectID, MapNumber, item.Category, item.CheckType, record.Remark);
                        datareadwrite.ExecuteSQL(sql_delete);
                    }

                    string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", CheckProblemTableName,
                        record.ProjectID, record.MapNumber, record.Category, record.CheckType, "一般", record.Remark, "", "", "", record.Checker, record.CheckTime);
                    datareadwrite.ExecuteSQL(sql_insert);

                }
                MessageBox.Show("检查意见提交到数据库，可以在检查意见记录表中查看和打印！");
            }
        }


    }

    //拓扑检查记录集
    public class TopocheckRecord
    {

        public TopocheckRecord()
        {

        }
        public string ProjectID { set; get; }
        public string MapNumber { set; get; }
        public string SerialNumber { set; get; }
        public string Category { set; get; }
        public string CheckType { set; get; }
        public string CheckItemName { set; get; }
        public string FeatureClassA { set; get; }
        public string FeatureClassB { set; get; }
        public string FeatureA { set; get; }
        public string FeatureB { set; get; }
        public string Attribute { set; get; }
        public string Remark { set; get; }
        public string CheckTime { set; get; }
        public string Checker { set; get; }
        public string GeomType { set; get; }
        public string GeomWkt { set; get; }

    }

    public class TopocheckRecordCollections
    {
        public string ProjectID { set; get; }
        public string MapNumber { set; get; }
        public List<TopocheckRecord> ResultList;
        public TopocheckRecordCollections(string projectid,string mapnumber)
        {
            ProjectID = projectid;
            MapNumber = mapnumber;
            if (ResultList == null)
                ResultList = new List<TopocheckRecord>();

        }
        public void Read()
        {
            if (ResultList == null)
                ResultList = new List<TopocheckRecord>();
            else
            {
                ResultList.Clear();
            }

            string TopoCheckRecordCollectionTableName = "topocheckrecordcollection";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            List<string> tableNames = datareadwrite.GetSchameDataTableNames();
            if (tableNames.Contains(TopoCheckRecordCollectionTableName) == false)
                return;
            DataTable vTable = datareadwrite.GetDataTable(TopoCheckRecordCollectionTableName);
            string sql = string.Format("ProjectID='{0}' and MapNumber='{1}'", ProjectID,MapNumber);
            DataRow[] rows = vTable.Select(sql);
            foreach (DataRow r in rows)
            {
                TopocheckRecord record = new TopocheckRecord();
                Type type = record.GetType();
                System.Reflection.PropertyInfo[] properties = type.GetProperties();
                foreach(System.Reflection.PropertyInfo property in properties)
                {
                    foreach(DataColumn dc in vTable.Columns)
                    {
                        if(property.Name.ToLower().IndexOf(dc.ColumnName)>=0)
                        {
                            var porperty = type.GetProperty(property.Name);
                            porperty.SetValue(record, r[dc.ColumnName] as string);
                        }
                    }
                }
                ResultList.Add(record);
            }
        }
        public void Write(string projectid, string szMapNumber, string currentuser)
        {
            if (ResultList == null)
                return;
            if (ProjectID != projectid)
                return;
            //删除原有记录
            //先将该样本中的问题记录进行清空
            ///////////////////////////////////////////////////////////////////////
            /////////////更新到数据库
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            string TopoCheckRecordCollectionTableName = "topocheckrecordcollection";
            //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
            List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
            if (tableNames2.Contains(TopoCheckRecordCollectionTableName) == false)
            {
                string sql_createTable = string.Format("create table {0}( ProjectID text,MapNumber text,SerialNumber Text, Category  Text,CheckType Text,CheckItemName  Text, FeatureClassA  Text, FeatureClassB  Text, FeatureA  Text, FeatureB  Text, Attribute  Text, Remark  Text, CheckTime  Text, Checker  Text,GeomType Text,GeomWkt Text)", TopoCheckRecordCollectionTableName);
                datareadwrite.ExecuteSQL(sql_createTable);
                //记录
            }

            //对检查记录进行更新，删除原记录，插入新记录
            foreach (TopocheckRecord record in  ResultList)
            {
                if (szMapNumber != record.MapNumber)
                    return;

                if (record.Checker == null || record.Checker == "")
                    record.Checker = currentuser;

                if (record.Checker != currentuser)
                    return;

                //逐条记录到数据表SearchTargetSetting
                string sql_queryitem = string.Format("select Mapnumber from {0} where ProjectID = '{1}' and MapNumber='{2}' and Category='{3}' and CheckType='{4}' and Remark='{5}'", TopoCheckRecordCollectionTableName, ProjectID, MapNumber, record.Category,record.CheckType,record.Remark);
                object o = datareadwrite.GetScalar(sql_queryitem);
                if (o == null)//insert
                {
                }
                else // delete
                {
                    string sql_delete = string.Format("delete from {0} where  ProjectID = '{1}' and MapNumber='{2}'  and Category='{3}' and CheckType='{4}' and Remark='{5}'", TopoCheckRecordCollectionTableName, ProjectID, MapNumber, record.Category, record.CheckType,record.Remark);
                    datareadwrite.ExecuteSQL(sql_delete);
                }

                string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}')", TopoCheckRecordCollectionTableName,
                    record.ProjectID, record.MapNumber, record.SerialNumber, record.Category,record.CheckType, record.CheckItemName, record.FeatureClassA, record.FeatureClassB, record.FeatureA, record.FeatureB, record.Attribute, record.Remark, record.CheckTime, record.Checker,record.GeomType,record.GeomWkt);
                datareadwrite.ExecuteSQL(sql_insert);
            }
            MessageBox.Show("拓扑检查记录提交到数据库完毕！");
        }


    }

    //拓扑检查容差类设计
    public class Tolerance
    {
        public string Category { set; get; }
        public CheckItem CheckItemBelongTo { set; get; }
        public string Name { set; get; }
        public double Value { set; get; }
        public string Description { set; get; }
        public string Text { set; get; }
    }
    //拓扑检查项基类
    public class CheckItem
    {
        public CheckItem(IMap GlobeMap)
        {
            localMap = GlobeMap;
        }
        public IMap localMap { set; get; }
        public IWorkspace pWorkSpace { set; get; }
        public bool TobeCheck { set; get; }
        public string Category { set; get; }
        public string CheckType { get { if(Checkentry!=null)return Checkentry.CheckType;return null; } }
        public string DescribeName { set; get; }
        public string ClassName { set; get; }
        public string ParaTypes { set; get; }
        public string Paras { set; get; }
        public CheckClass Checkentry {set;get;}
        public TopocheckRecordCollections topocheckrecordcollection { set; get; }

        public virtual void Check(CheckItem paras) { }
    }
    //拓扑检查项基类
    public class TopoCheckItem: CheckItem
    {
        public TopoCheckItem(IMap GlobeMap):base(GlobeMap)
        {

        }
        public override void Check(CheckItem paras)
        {
            Checkentry.Check(paras);
        }
    }
    //检查类工厂
     public class CheckClassFactory
    {
        static public  CheckClass CreateCheckClass(string classname)
        {
            //此处根据传入的元数据描述利用反射功能创建该类型
            //TopoCheckClass checkentry = new TopoCheckClass();
            string modulename = "DLGCheckLib";
            string MainDll = string.Format("{0}.{1}", modulename,"dll");
            string dllpath = Path.Combine(Application.StartupPath, MainDll);

            Assembly assembly = Assembly.LoadFile(dllpath);
            string clasname2 = string.Format("{0}.{1}", modulename, classname);
            object o = assembly.CreateInstance(clasname2);
            CheckClass checkentry = o as CheckClass;
            return checkentry;
        }
    }
    //拓扑检查类
    public class CheckClass
    {
        public string CheckName { set; get; }
        public string CheckType { set; get; }
        public List<string> LayersTobeCheck { set; get; }
        public Tolerance tolerance { set; get; }
        public virtual void Check(CheckItem paras) { }

    }
    public class GeometryCheckClass:CheckClass
    {
        public TopologyChecker TopoCheck { set; get; }
        public List<IFeatureClass> featureList { set; get; }
        public GeometryCheckClass()
        {
            CheckType = "几何异常";
        }
        //检查的策略是
        public override void Check(CheckItem paras)
        {
            string[] layers = paras.Paras.Split(',');
            LayersTobeCheck = new List<string>();
            foreach (string layer in layers)
            {
                if (layer != "")
                    LayersTobeCheck.Add(layer);
            }
            if (LayersTobeCheck.Count == 0)
                return;

            TopoCheck = new TopologyChecker(paras,LayersTobeCheck,  CheckName);//传入要处理的要素数据集  

            TopoCheck.PUB_TopoBuild(CheckName);//构建拓扑的名字  

            featureList = TopoCheck.PUB_GetAllFeatureClass();
            TopoCheck.PUB_AddFeatureClass(featureList);//将该要素中全部要素都加入拓扑  

        }
    }
    //拓扑检查算法入口
    public class TopoCheckClass:CheckClass
    {
        public TopologyChecker TopoCheck { set; get; }
        public List<IFeatureClass> featureList { set; get; }
        public TopoCheckClass()
        {
            CheckType = "拓扑一致性";
        }
        //检查的策略是
        public override void Check(CheckItem paras)
        {
            string[] layers = paras.Paras.Split(',');
            LayersTobeCheck = new List<string>();
            foreach (string layer in layers)
            {
                if(layer!="")
                    LayersTobeCheck.Add(layer);
            }
            if (LayersTobeCheck.Count == 0)
                return;

            TopoCheck = new TopologyChecker(paras,LayersTobeCheck, CheckName);//传入要处理的要素数据集  

            TopoCheck.PUB_TopoBuild(CheckName);//构建拓扑的名字  

            featureList = TopoCheck.PUB_GetAllFeatureClass();
            TopoCheck.PUB_AddFeatureClass(featureList);//将该要素中全部要素都加入拓扑  

        }
    }
    //要素面之间无空隙检查
    public class PolygonsGap:TopoCheckClass
    {
        public PolygonsGap()
        {
            CheckName = "PolygonsGap";
        }
        public  override void Check(CheckItem paras)
        {
            base.Check(paras);
            //添加规则  
            foreach(IFeatureClass ifeatureclass in featureList)
            {
                TopoCheck.PUB_AddRuleToTopology(TopologyChecker.TopoErroType.面要素之间无空隙, ifeatureclass);
            }

        }

    }
    //线要素不允许有悬挂点
    public class SuspensionLine : TopoCheckClass
    {
        public SuspensionLine()
        {
            CheckName = "SuspensionLine";
        }

        public override void Check(CheckItem paras)
        {
            base.Check(paras);
            //添加规则  
            foreach (IFeatureClass ifeatureclass in featureList)
            {
                TopoCheck.PUB_AddRuleToTopology(TopologyChecker.TopoErroType.线要素不允许有悬挂点, ifeatureclass);
            }
        }
    }

    //线要素不允许有假节点
    public class LineNoPseudosOfPolylineCheck:TopoCheckClass
    {
        public LineNoPseudosOfPolylineCheck()
        {
            CheckName = "LineNoPseudosOfPolylineCheck";
        }
        public override void Check(CheckItem paras)
        {
            base.Check(paras);
            //添加规则  
            foreach (IFeatureClass ifeatureclass in featureList)
            {
                TopoCheck.PUB_AddRuleToTopology(TopologyChecker.TopoErroType.线要素不允许有假节点, ifeatureclass);
            }
        }
    }

    //同层要素重合检查
    public class TwoGeometriesOverlapInOneLayer:TopoCheckClass
    {
        public TwoGeometriesOverlapInOneLayer()
        {
            CheckName = "TwoGeometriesOverlapInOneLayer";
        }
        public override void Check(CheckItem paras)
        {
            base.Check(paras);
            //添加规则  
            foreach (IFeatureClass ifeatureclass in featureList)
            {
                if(ifeatureclass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                {
                    TopoCheck.PUB_AddRuleToTopology(TopologyChecker.TopoErroType.线要素间不能有相互重叠部分, ifeatureclass);
                }
                else if (ifeatureclass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                {
                    TopoCheck.PUB_AddRuleToTopology(TopologyChecker.TopoErroType.点要素重合点要素, ifeatureclass);
                }
                else if (ifeatureclass.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                {
                    TopoCheck.PUB_AddRuleToTopology(TopologyChecker.TopoErroType.面要素间无重叠, ifeatureclass);
                }

            }
        }
    }

    //线要素不能自相交
    public class SelfIntersectOfPolylineCheck : GeometryCheckClass
    {
        public SelfIntersectOfPolylineCheck()
        {
            CheckName = "SelfIntersectOfPolylineCheck";
        }
        public override void Check(CheckItem paras)
        {
            base.Check(paras);
            //添加规则  
            foreach (IFeatureClass ifeatureclass in featureList)
            {
                TopoCheck.PUB_AddRuleToTopology(TopologyChecker.TopoErroType.线要素不能自相交, ifeatureclass);
            }
        }
    }
}

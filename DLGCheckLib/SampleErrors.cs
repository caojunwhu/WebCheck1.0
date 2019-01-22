using DatabaseDesignPlus;
using ReportPrinter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLGCheckLib
{
    //2017-5-3新增记录错漏的标注矢量，用户存储显示、定位等操作，详实记录问题错漏，便于打分，同时表可以读取存储于位置精度记录中的粗差情况
   //在pinerroritem,pinerrorcollection类基础上构建了WebErrorRecord 和 WebErrorRecordCollection
    public class WebErrorRecord
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

        public static void CleanCheckItemErrorRecord(string projectid,string mapnumber,string sCheckItem)
        {
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
            //把该检查项下的内容全部清理
            string sql_delete = string.Format("delete from {0} where projectid='{1}' and mapnumber ='{2}' and 检查项='{3}'", SampleerrorCollectionTableName, projectid, mapnumber, sCheckItem);
            datareadwrite.ExecuteSQL(sql_delete);

        }
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
            //delete the old  indentified record first using CleanCheckItemErrorRecord

            string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')", SampleerrorCollectionTableName, Projectid, Mapnumber,
                                QualityItem, SubQualityItem, CheckItem, ErrorType, Error, Feedback, Modify, Review, Checker, CheckTime, Comment, Shape);
            datareadwrite.ExecuteSQL(sql_insert);


        }
    }
    public class WebErrorRecordCollection
    {
        string projectid;
        string mapnumber;
        List<WebErrorRecord> errorItems;

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

        public List<WebErrorRecord> ErrorItems
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

        public void Read(string sprojected, string smapnumber)
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
            if (dt.Rows.Count > 0)
            {
                if (ErrorItems != null) ErrorItems.Clear();
                if (ErrorItems == null) ErrorItems = new List<WebErrorRecord>();

                foreach (DataRow dr in dt.Rows)
                {
                    WebErrorRecord perror = new WebErrorRecord();
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

            foreach (WebErrorRecord item in ErrorItems)
            {
                item.Write();
            }
        }

        //根据质量元素对错漏进行编号，输入到检查意见记录表中，方便格式化打印
        public void Write(QualityItems qualityItems)
        {
            if (ErrorItems == null || ErrorItems.Count == 0 || qualityItems == null ||
                qualityItems.QualityItemList == null || qualityItems.QualityItemList.Count == 0)
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
            foreach (DataRow dr in data.Rows)
            {
                string sql_count = string.Format("select count(*) from {0} where 质量元素='{1}' and 质量子元素='{2}' and 错漏类别='{3}' and 错漏描述='{4}' and 处理意见='{5}' and 修改情况='{6}' and 复查情况='{7}' and 检查者='{8}'and  ProjectID = '{9}' and Mapnumber='{10}'", pinerrortablename, dr["质量元素"] as string, dr["质量子元素"] as string, dr["错漏类别"] as string, dr["错漏描述"] as string, dr["处理意见"] as string, dr["修改情况"] as string, dr["复查情况"] as string, dr["检查者"] as string, Projectid, Mapnumber);
                int count = Convert.ToInt32(datareadwrite.GetScalar(sql_count));
                QualityItem qItem = qualityItems.QualityItemList.Find(ao => ao.QualityItemName == Convert.ToString(dr["质量元素"]));
                int indexofqualityitem = qualityItems.QualityItemList.IndexOf(qItem) + 1;
                string errorClass = string.Format("{0}{1}{2}", Utils.NumberConver.NumAddCircle(indexofqualityitem), count == 0 ? "" : count.ToString(), Convert.ToString(dr["错漏类别"]));

                string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", SampleerrorCollectionTableName, Projectid, Mapnumber,
                    dr["质量元素"] as string, dr["质量子元素"] as string, errorClass, dr["错漏描述"] as string, dr["处理意见"] as string, dr["修改情况"] as string, dr["复查情况"] as string, dr["检查者"] as string, DateTime.Now.ToString());
                datareadwrite.ExecuteSQL(sql_insert);
            }


            foreach (WebErrorRecord item in ErrorItems)
            {
                //string sql_insert = string.Format("insert into {0} Values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", SampleerrorCollectionTableName, Projectid, Mapnumber,
                //    dr["质量元素"] as string, dr["质量子元素"] as string, dr["错漏类别"] as string, dr["错漏描述"] as string, dr["处理意见"] as string, dr["修改情况"] as string, dr["复查情况"] as string, dr["检查者"] as string, dr["检查日期"] as string);
                //datareadwrite.ExecuteSQL(sql_insert);
                //记录搜索到的目标

            }
        }

    }

    //负责将数学精度计算后的平面、高程、相对位置中误差超限、粗差超限、粗差；
    //地籍图界址点绝对位置、相对位置，宗地图界址点绝对位置超限情况记录到检查意见记录中sampleerrorplus中
    public class SampleErrors
    {
        public static void UpdateErrorsRecord(MeanError meanerror ,string maptype,string mapnumber,string username,string projectid)
        {
            string sCheckItem = "";
            switch (meanerror.sErrorType)
            {
                //粗差记录格式：同精度检测粗差限值{}，粗差值{}。
                case "平面绝对位置误差":
                    {
                        PositionMeanError pme = meanerror as PositionMeanError;
                        //宗地图、地籍图平面绝对位置超5%，10%两项
                        if (maptype == "宗地图" || maptype == "地籍图")
                        {

                        }
                        //其他类型包括大比例尺地形图、房产分幅图、房产分丘图、数字影像图（DOM）、数字高程模型（DEM）
                        //含宗地图、地籍图，需记录中误差超限、粗差、粗差率等错漏项
                        if (maptype == "宗地图" || maptype == "地籍图" || maptype == "大比例尺地形图" || maptype == "房产分幅图"
                             || maptype == "房产分丘图" || maptype == "数字影像图（DOM）" || maptype == "数字高程模型（DEM）")
                        {
                            sCheckItem = "平面绝对位置粗差";
                            WebErrorRecord.CleanCheckItemErrorRecord(projectid, mapnumber, sCheckItem);
                            //写入粗差错漏
                            foreach (DataRow dr in meanerror.dPointValue.Rows)
                            {
                                double error = Convert.ToDouble(dr["v"]);

                                if (error > pme.vGrossError)
                                {
                                    //新增一条检查记录，传递到标注窗口
                                    WebErrorRecord Pinerror = new WebErrorRecord();
                                    Pinerror.Projectid = projectid;
                                    Pinerror.Mapnumber = mapnumber;
                                    Pinerror.Error = "绝对位置粗差";
                                    Pinerror.QualityItem = "数学精度";
                                    Pinerror.SubQualityItem = "平面精度";
                                    Pinerror.CheckItem = "绝对位置中误差";
                                    Pinerror.ErrorType = "C";
                                    Pinerror.Checker = username;
                                    Pinerror.CheckTime = DateTime.Now.ToString();
                                    Pinerror.Feedback = "";
                                    Pinerror.Modify = "";
                                    Pinerror.Review = "";
                                    Pinerror.Comment = string.Format("{0}检测，绝对位置粗差限值{1}m，实测{2:F2}m",pme.sFactorType, pme.vGrossError, error);
                                    Pinerror.Shape = "";

                                    Pinerror.Write();
                                }
                            }
                            //写入超限错漏
                            if (pme.vError >= 0 && pme.vError > pme.vMaxError)
                            {
                                sCheckItem = "平面绝对位置中误差";
                                WebErrorRecord.CleanCheckItemErrorRecord(projectid, mapnumber, sCheckItem);
                                //新增一条检查记录，传递到标注窗口
                                WebErrorRecord Pinerror = new WebErrorRecord();
                                Pinerror.Projectid = projectid;
                                Pinerror.Mapnumber = mapnumber;
                                Pinerror.Error = "平面绝对位置中误差超限";
                                Pinerror.QualityItem = "数学精度";
                                Pinerror.SubQualityItem = "平面精度";
                                Pinerror.CheckItem = sCheckItem;
                                Pinerror.ErrorType = "A";
                                Pinerror.Checker = username;
                                Pinerror.CheckTime = DateTime.Now.ToString();
                                Pinerror.Feedback = "";
                                Pinerror.Modify = "";
                                Pinerror.Review = "";
                                Pinerror.Comment = string.Format("{0}检测，绝对位置中误差{1:F2}m，限值{2:F2}m",pme.sFactorType, pme.vError, pme.vMaxError);
                                Pinerror.Shape = "";

                                Pinerror.Write();
                            }
                            if (pme.nGrossErrorRatio > 5)
                            {
                                sCheckItem = "平面绝对位置中误差";
                                WebErrorRecord.CleanCheckItemErrorRecord(projectid, mapnumber, sCheckItem);
                                //新增一条检查记录，传递到标注窗口
                                WebErrorRecord Pinerror = new WebErrorRecord();
                                Pinerror.Projectid = projectid;
                                Pinerror.Mapnumber = mapnumber;
                                Pinerror.Error = "绝对位置粗差率超限";
                                Pinerror.QualityItem = "数学精度";
                                Pinerror.SubQualityItem = "平面精度";
                                Pinerror.CheckItem = sCheckItem;
                                Pinerror.ErrorType = "A";
                                Pinerror.Checker = username;
                                Pinerror.CheckTime = DateTime.Now.ToString();
                                Pinerror.Feedback = "";
                                Pinerror.Modify = "";
                                Pinerror.Review = "";
                                Pinerror.Comment = string.Format("{0}检测，绝对位置粗差率{1}%,限值5%",pme.sFactorType, pme.nGrossErrorRatio);
                                Pinerror.Shape = "";

                                Pinerror.Write();
                            }
                        }
                    }
                        break;
                case "平面相对位置误差":
                    {
                        RelativeMeanError rme = meanerror as RelativeMeanError;
                        // 地籍图平面相对位置超5%，10%两项
                        if (  maptype == "地籍图")
                        {

                        }
                        //其他类型包括大比例尺地形图、房产分幅图、房产分丘图 
                        //含 地籍图，需记录中误差超限、粗差、粗差率等错漏项
                        if (maptype == "地籍图" || maptype == "大比例尺地形图" || 
                            maptype == "房产分幅图"|| maptype == "房产分丘图" )
                        {
                            sCheckItem = "平面相对位置粗差";
                            WebErrorRecord.CleanCheckItemErrorRecord(projectid, mapnumber, sCheckItem);
                            //写入粗差错漏
                            foreach (DataRow dr in meanerror.dPointValue.Rows)
                            {
                                double error = Convert.ToDouble(dr["v"]);
                                if (Math.Abs(error) > rme.vGrossError)
                                {
                                    //新增一条检查记录，传递到标注窗口
                                    WebErrorRecord Pinerror = new WebErrorRecord();
                                    Pinerror.Projectid = projectid;
                                    Pinerror.Mapnumber = mapnumber;
                                    Pinerror.Error = "平面相对位置粗差";
                                    Pinerror.QualityItem = "数学精度";
                                    Pinerror.SubQualityItem = "平面精度";
                                    Pinerror.CheckItem = sCheckItem;
                                    Pinerror.ErrorType = "C";
                                    Pinerror.Checker = username;
                                    Pinerror.CheckTime = DateTime.Now.ToString();
                                    Pinerror.Feedback = "";
                                    Pinerror.Modify = "";
                                    Pinerror.Review = "";
                                    Pinerror.Comment = string.Format("{0}检测：平面相对位置粗差限值{1}m，实测{2:F2}m", rme.sFactorType,rme.vGrossError, error);
                                    Pinerror.Shape = "";

                                    Pinerror.Write();

                                }
                            }

                            //写入超限错漏
                            if (rme.vError > rme.vMaxError)
                            {
                                sCheckItem = "平面相对位置中误差";
                                WebErrorRecord.CleanCheckItemErrorRecord(projectid, mapnumber, sCheckItem);
                                //新增一条检查记录，传递到标注窗口
                                WebErrorRecord Pinerror = new WebErrorRecord();
                                Pinerror.Projectid = projectid;
                                Pinerror.Mapnumber = mapnumber;
                                Pinerror.Error = "相对位置中误差超限";
                                Pinerror.QualityItem = "数学精度";
                                Pinerror.SubQualityItem = "平面精度";
                                Pinerror.CheckItem = sCheckItem;
                                Pinerror.ErrorType = "A";
                                Pinerror.Checker = username;
                                Pinerror.CheckTime = DateTime.Now.ToString();
                                Pinerror.Feedback = "";
                                Pinerror.Modify = "";
                                Pinerror.Review = "";
                                Pinerror.Comment = string.Format("{0}检测，平面相对位置中误差{10}m，限值{2}m，实测{3:F2}m", "", rme.sFactorType, rme.vMaxError, rme.vError);
                                Pinerror.Shape = "";
                                Pinerror.Write();
                            }

                            if (rme.nGrossErrorRatio > 5)
                            {
                                sCheckItem = "平面相对位置误差粗差率";
                                WebErrorRecord.CleanCheckItemErrorRecord(projectid, mapnumber, sCheckItem);
                                //新增一条检查记录，传递到标注窗口
                                WebErrorRecord Pinerror = new WebErrorRecord();
                                Pinerror.Projectid = projectid;
                                Pinerror.Mapnumber = mapnumber;
                                Pinerror.Error = "平面相对位置粗差率超限";
                                Pinerror.QualityItem = "数学精度";
                                Pinerror.SubQualityItem = "平面精度";
                                Pinerror.CheckItem = sCheckItem;
                                Pinerror.ErrorType = "A";
                                Pinerror.Checker = username;
                                Pinerror.CheckTime = DateTime.Now.ToString();
                                Pinerror.Feedback = "";
                                Pinerror.Modify = "";
                                Pinerror.Review = "";
                                Pinerror.Comment = string.Format("{0}检测，平面相对位置误差粗差率限值5%，实测{1}%", rme.sFactorType, rme.nGrossErrorRatio);
                                Pinerror.Shape = "";

                                Pinerror.Write();
                            }

                        }
                    }
                    break;
                case "高程绝对位置误差":
                    {
                        HeightMeanError hme = meanerror as HeightMeanError;
                        //其他类型包括大比例尺地形图、 数字高程模型（DEM） 
                        // 需记录中误差超限、粗差、粗差率等错漏项
                        if (maptype == "大比例尺地形图" || maptype == "数字高程模型（DEM）")
                        {
                            sCheckItem = "注记点高程粗差";
                            WebErrorRecord.CleanCheckItemErrorRecord(projectid, mapnumber, sCheckItem);
                            foreach (DataRow dr in meanerror.dPointValue.Rows)
                            {
                                double error = Convert.ToDouble(dr["v"]);
                                if (error > hme.vGrossError)
                                {
                                    //新增一条检查记录，传递到标注窗口
                                    WebErrorRecord Pinerror = new WebErrorRecord();
                                    Pinerror.Projectid = projectid;
                                    Pinerror.Mapnumber = mapnumber;
                                    Pinerror.Error = "注记点高程粗差";
                                    Pinerror.QualityItem = "数学精度";
                                    Pinerror.SubQualityItem = "高程精度";
                                    Pinerror.CheckItem = sCheckItem;
                                    Pinerror.ErrorType = "C";
                                    Pinerror.Checker = username;
                                    Pinerror.CheckTime = DateTime.Now.ToString();
                                    Pinerror.Feedback = "";
                                    Pinerror.Modify = "";
                                    Pinerror.Review = "";
                                    Pinerror.Comment = string.Format("{0}检测，注记点高程粗差限值{1}m，实测{2:F2}m",hme.sFactorType, hme.vGrossError, error);
                                    Pinerror.Shape = "";

                                    Pinerror.Write();
                                }
                            }
                            if (hme.vError >= 0 && hme.vError > hme.vMaxError)
                            {
                                sCheckItem = "注记点高程中误差";
                                WebErrorRecord.CleanCheckItemErrorRecord(projectid, mapnumber, sCheckItem);
                                //新增一条检查记录，传递到标注窗口
                                WebErrorRecord Pinerror = new WebErrorRecord();
                                Pinerror.Projectid = projectid;
                                Pinerror.Mapnumber = mapnumber;
                                Pinerror.Error = "注记点高程中误差超限";
                                Pinerror.QualityItem = "数学精度";
                                Pinerror.SubQualityItem = "高程精度";
                                Pinerror.CheckItem = sCheckItem;
                                Pinerror.ErrorType = "A";
                                Pinerror.Checker = username;
                                Pinerror.CheckTime = DateTime.Now.ToString();
                                Pinerror.Feedback = "";
                                Pinerror.Modify = "";
                                Pinerror.Review = "";
                                Pinerror.Comment = string.Format("{0}检测，注记点高程中误差{1:F2}m，限值{2:F2}m",hme.sFactorType, hme.vError, hme.vMaxError); ;
                                Pinerror.Shape = "";

                                Pinerror.Write();
                            }

                            if (hme.nGrossErrorRatio > 5)
                            {
                                sCheckItem = "注记点高程误差粗差率";
                                WebErrorRecord.CleanCheckItemErrorRecord(projectid, mapnumber, sCheckItem);
                                //新增一条检查记录，传递到标注窗口
                                WebErrorRecord Pinerror = new WebErrorRecord();
                                Pinerror.Projectid = projectid;
                                Pinerror.Mapnumber = mapnumber;
                                Pinerror.Error = "注记点高程粗差率超限";
                                Pinerror.QualityItem = "数学精度";
                                Pinerror.SubQualityItem = "高程精度";
                                Pinerror.CheckItem = sCheckItem;
                                Pinerror.ErrorType = "A";
                                Pinerror.Checker = username;
                                Pinerror.CheckTime = DateTime.Now.ToString();
                                Pinerror.Feedback = "";
                                Pinerror.Modify = "";
                                Pinerror.Review = "";
                                Pinerror.Comment = string.Format("{0}检测,注记点高程粗差率{1}%,限值5%",hme.sFactorType, hme.nGrossErrorRatio);
                                Pinerror.Shape = "";

                                Pinerror.Write();
                            }

                        }
                    }
                    break;                
            }
        }
    }
}

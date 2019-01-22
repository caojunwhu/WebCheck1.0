using DatabaseDesignPlus;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DLGCheckLib
{
    public class DLGCheckProjectClass
    {
        public DLGCheckProjectClass() { }
        public string ProjectName { get; set; }
        public string ProjectID { get; set; }
        public int nLots { get; set; }
        public int nSampleAreaCount { get; set; }
        public int nSampleSize { get; set; }
        public int MapScale { get; set; }
        public string SrText { set; get; }
        public string department { set; get; }
        public string owner { set; get; }
        public string shared { set; get; }
        public string lastOpentime { set; get; }
        public string currentuser { set; get; }
        public string SampleFileFormat { set; get; }
        public string SampleFilePath { set; get; }
        public string CurrentMapnumber { set; get; }

        //info plus
        public string delegateCompany { set; get; }
        public string productCompany { set; get; }
        public string productTime { set; get; }
        public string sampler { set; get; }
        public string sampletime { set; get; }
        public string sampleplace { set; get; }
        public string checktype { set; get; }
        public string plaincorsys { set; get; }
        public string heightsys { set; get; }
        public string comment { set; get; }
        public string productType { set; get; }
        public string specific { set; get; }

        public List<MapSampleItemSetting> MapSampleSetting { get; set; }

        public List<MapSampeItemQuality> MapSampleQuality { get; set; }

        public List<MapSampleCheckState> MapSampleStation { set; get; }

        public delegate void InvokeDelegate(string mess, int count);
        [XmlIgnore]
        public InvokeDelegate ShowProgress;

        public delegate void InvokeJumpToPlainHeightCheckStaion(int sampleareaidex ,int sampleserial,string szMapnumber);
        [XmlIgnore]
        public InvokeJumpToPlainHeightCheckStaion FuncDelegateJumpToPlainHeightCheckStaion;

        public delegate void InvokeJumpToRelativeCheckStaion(int sampleareaidex, int sampleserial, string szMapnumber);
        [XmlIgnore]
        public InvokeJumpToRelativeCheckStaion FuncDelegateJumpToRelativeCheckStaion;

        public void JumpToPlainHeightCheckStation(int sampleareaidex, int sampleserial, string szMapnumber)
        {
            if(FuncDelegateJumpToPlainHeightCheckStaion!=null)
            {
                FuncDelegateJumpToPlainHeightCheckStaion(sampleareaidex, sampleserial, szMapnumber);
            }
        }

        public void JumpToRelativeCheckStation(int sampleareaidex, int sampleserial, string szMapnumber)
        {
            if (FuncDelegateJumpToRelativeCheckStaion != null)
                FuncDelegateJumpToRelativeCheckStaion(sampleareaidex, sampleserial, szMapnumber);
        }
        public DLGCheckProjectClass(string sProjectID, string curuser)
        {
            currentuser = curuser;
            ReadProject(sProjectID);
        }
        //   
        //项目基本信息
        public void ReadProject(string sProjectID)
        {
            ProjectID = sProjectID;
            ////拉取dlgcheckproject中信息
            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);
            string sqlproject = string.Format("select projectname,projectid,lotsize,samplesize,samplearea,scale ,srtext,department,owner,shared,lastopentime,samplefiletype from dlgcheckproject where projectid = '{0}' order by lastopentime desc", sProjectID);
            DataTable datatable = dbread.GetDataTableBySQL(sqlproject);
            DataRow datarow = datatable.Rows[0];
            this.ProjectName = datarow["projectname"] as string;
            this.nLots = Convert.ToInt32(datarow["lotsize"]);
            this.nSampleAreaCount = Convert.ToInt32(datarow["samplearea"]);
            this.nSampleSize = Convert.ToInt32(datarow["samplesize"]);
            this.MapScale = Convert.ToInt32(datarow["scale"]);
            this.SrText = Convert.ToString(datarow["srtext"]);
            this.department = Convert.ToString(datarow["department"]);
            this.owner = Convert.ToString(datarow["owner"]);
            this.shared = Convert.ToString(datarow["shared"]);
            this.lastOpentime = Convert.ToString(datarow["lastopentime"]);
            this.SampleFileFormat = Convert.ToString(datarow["samplefiletype"]);

            //info plus
            sqlproject = string.Format("select projectid,委托单位,生产单位,生产日期,抽样者,抽样日期,抽样地点,检验类别,平面坐标系,高程基准,备注 from checkprojectinfoplus where projectid = '{0}'", sProjectID);
            datatable = dbread.GetDataTableBySQL(sqlproject);
            //更新版本与旧版本衔接处处理
            if(datatable.Rows.Count>0)
            {
                datarow = datatable.Rows[0];
                this.delegateCompany = datarow["委托单位"] as string;
                this.productCompany = datarow["生产单位"] as string;
                this.productTime = datarow["生产日期"] as string;
                this.sampler = datarow["抽样者"] as string;
                this.sampletime = datarow["抽样日期"] as string;
                this.sampleplace = datarow["抽样地点"] as string;
                this.checktype = datarow["检验类别"] as string;
                this.plaincorsys = datarow["平面坐标系"] as string;
                this.heightsys = datarow["高程基准"] as string;
                this.comment = datarow["备注"] as string;

                string specifictable = "projectspecific";
                string sql_specific = string.Format("select specificnumber from {0} where projectid='{1}'", specifictable,ProjectID);
                List<string> specifics = dbread.GetSingleFieldValueList("specificnumber", sql_specific);            
                this.specific = "";
                foreach (string s in specifics)
                {
                    this.specific += s + ";";
                }
                string sql_checkitem = string.Format("select qulitycheckitem from projectqulitycheckitem where projectid ='{0}'", ProjectID);

                this.productType = dbread.GetScalar(sql_checkitem) as string;
                this.productType = this.productType.Replace("\\", "\"");

            }

        }
        //读取样本检查状态
        public void ReadSampleCheckState(string sProjectID)
        {
            if (ProjectID != sProjectID)
                return;

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);

            //拉区位置精度检测项目信息表信息
            string sqlProjecDetial = string.Format("select * from 位置精度检测项目信息表 where 成果名称='{0}' order by 抽样分区,流水号", ProjectName);
            DataTable datatable = dbread.GetDataTableBySQL(sqlProjecDetial);
            MapSampleStation = new List<MapSampleCheckState>();

            int index = 0;
            if (ShowProgress != null)
            {
                ShowProgress("SETMIN", 0);
                ShowProgress("SETMAX", datatable.Rows.Count);

            }

            foreach (DataRow dr in datatable.Rows)
            {
                MapSampleCheckState Checkstate = new MapSampleCheckState();
                Checkstate.MapNumber = Convert.ToString(dr["图幅号"]);
                Checkstate.SampleSerial = Convert.ToInt32(dr["流水号"]);
                Checkstate.SampleAreaIndex = Convert.ToInt32(dr["抽样分区"]);

                string sql_select = string.Format("select count(ptid) from scater as s ,mapbindingtable as m where st_within(s.wkb_geometry, m.wkb_geometry) = true and m.projectid = '{0}' and m.mapnumber='{1}'", ProjectID, Checkstate.MapNumber);
                Checkstate.PHCheckPoints = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select count(ptid) from checklinecollection as s ,mapbindingtable as m where st_within(st_point(sx, sy), m.wkb_geometry) = true and m.projectid = '{0}' and  m.mapnumber='{1}' and pointtype='{2}' ", ProjectID, Checkstate.MapNumber, "PlanCheck");
                Checkstate.PCheckPoints = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select count(ptid) from checklinecollection as s , mapbindingtable as m where st_within(st_point(sx, sy), m.wkb_geometry) = true and m.projectid = '{0}' and  m.mapnumber = '{1}' and pointtype = '{2}' ", ProjectID, Checkstate.MapNumber, "HeightCheck");
                Checkstate.HCheckPoints = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select count(ptid) from 间距边长精度检测点成果表 where mapnumber='{0}'", Checkstate.MapNumber);
                Checkstate.RCheckPoints = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select count(ptid) from checklinecollection as s ,mapbindingtable as m where st_within(st_point(sx, sy), m.wkb_geometry) = true and m.projectid = '{0}' and  m.mapnumber='{1}' and pointtype='{2}' ", ProjectID, Checkstate.MapNumber, "ControlPoint");
                Checkstate.ControlPoints = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select count(ptid) from checklinecollection as s ,mapbindingtable as m where st_within(st_point(sx, sy), m.wkb_geometry) = true and m.projectid = '{0}' and  m.mapnumber='{1}' and pointtype='{2}' ", ProjectID, Checkstate.MapNumber, "NonCheck");
                Checkstate.NonCheckPoints = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select count(ptid) from checklinecollection as s ,mapbindingtable as m where st_within(st_point(sx, sy), m.wkb_geometry) = true and m.projectid = '{0}' and  m.mapnumber='{1}' and pointtype='{2}' ", ProjectID, Checkstate.MapNumber, "WaitToCheck");
                Checkstate.WaitToChecks = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select distinct 检查者 from 平面及高程精度检测点成果表 where mapnumber='{0}'", Checkstate.MapNumber);
                Checkstate.PHChecker = Convert.ToString(dbread.GetScalar(sql_select));
                sql_select = string.Format("select distinct 检查者 from 间距边长精度检测点成果表 where mapnumber='{0}'", Checkstate.MapNumber);
                Checkstate.RChecker = Convert.ToString(dbread.GetScalar(sql_select));

                MapSampleStation.Add(Checkstate);


                if (ShowProgress != null)
                {
                    ShowProgress("SETVALUE", ++index);
                    if (index == datatable.Rows.Count)
                    {
                        ShowProgress("SETVALUE", 0);
                    }
                }
            }

        }
        //读取样本质量信息
        public void ReadSampleQuality(string sProjectID)
        {

            if (ProjectID != sProjectID)
                return;

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);

            //拉区位置精度检测项目信息表信息
            string sqlProjecDetial = string.Format("select * from 位置精度检测项目信息表 where 成果名称='{0}' order by 抽样分区,流水号", ProjectName);
            DataTable datatable = dbread.GetDataTableBySQL(sqlProjecDetial);
            MapSampleQuality = new List<MapSampeItemQuality>();

            int index = 0;
            if (ShowProgress != null)
            {
                ShowProgress("SETMIN", 0);
                ShowProgress("SETMAX", datatable.Rows.Count);

            }

            foreach (DataRow dr in datatable.Rows)
            {
                //////////////////抽样质量
                MapSampeItemQuality samplequality = new MapSampeItemQuality();
                samplequality.MapNumber = Convert.ToString(dr["图幅号"]);
                samplequality.SampleSerial = Convert.ToInt32(dr["流水号"]);
                samplequality.SampleAreaIndex = Convert.ToInt32(dr["抽样分区"]);

                samplequality.HeightError = new ErrorItem();
                samplequality.HeightError.MeanError = Convert.IsDBNull(dr["高程中误差"]) == true ? 999999 : Convert.ToDouble(dr["高程中误差"]);
                samplequality.HeightError.ErrorRatio = Convert.IsDBNull(dr["高程粗差比率"]) == true ? 999999 : Convert.ToDouble(dr["高程粗差比率"]);
                samplequality.HeightError.ErrorScore = Convert.IsDBNull(dr["高程精度得分"]) == true ? 999999 : Convert.ToDouble(dr["高程精度得分"]);
                samplequality.HeightError.ErrorComment = Convert.IsDBNull(dr["备注"]) == true ? "" : Convert.ToString(dr["备注"]);

                samplequality.PositionError = new ErrorItem();
                samplequality.PositionError.MeanError = Convert.IsDBNull(dr["点位中误差"]) == true ? 999999 : Convert.ToDouble(dr["点位中误差"]);
                samplequality.PositionError.ErrorRatio = Convert.IsDBNull(dr["点位粗差比率"]) == true ? 999999 : Convert.ToDouble(dr["点位粗差比率"]);
                samplequality.PositionError.ErrorScore = Convert.IsDBNull(dr["点位精度得分"]) == true ? 999999 : Convert.ToDouble(dr["点位精度得分"]);
                samplequality.PositionError.ErrorComment = Convert.IsDBNull(dr["备注"]) == true ? "" : Convert.ToString(dr["备注"]);

                /*samplequality.CountourError = new ErrorItem();
                samplequality.CountourError.MeanError = Convert.IsDBNull(dr["等高线中误差"]) == true ? 999999 : Convert.ToDouble(dr["等高线中误差"]);
                samplequality.CountourError.ErrorRatio = Convert.IsDBNull(dr["等高线粗差比率"]) == true ? 999999 : Convert.ToDouble(dr["等高线粗差比率"]);
                samplequality.CountourError.ErrorScore = Convert.IsDBNull(dr["等高线精度得分"]) == true ? 999999 : Convert.ToDouble(dr["等高线精度得分"]);
                samplequality.CountourError.ErrorComment = Convert.IsDBNull(dr["备注"]) == true ? "" : Convert.ToString(dr["备注"]);
                */
                samplequality.RelativeError = new ErrorItem();
                samplequality.RelativeError.MeanError = Convert.IsDBNull(dr["间距中误差"]) == true ? 999999 : Convert.ToDouble(dr["间距中误差"]);
                samplequality.RelativeError.ErrorRatio = Convert.IsDBNull(dr["间距粗差比率"]) == true ? 999999 : Convert.ToDouble(dr["间距粗差比率"]);
                samplequality.RelativeError.ErrorScore = Convert.IsDBNull(dr["间距精度得分"]) == true ? 999999 : Convert.ToDouble(dr["间距精度得分"]);
                samplequality.RelativeError.ErrorComment = Convert.IsDBNull(dr["备注"]) == true ? "" : Convert.ToString(dr["备注"]);

                samplequality.Comment = Convert.IsDBNull(dr["备注"]) == true ? "" : Convert.ToString(dr["备注"]);

                MapSampleQuality.Add(samplequality);

                if (ShowProgress != null)
                {
                    ShowProgress("SETVALUE", ++index);
                    if (index == datatable.Rows.Count)
                    {
                        ShowProgress("SETVALUE", 0);
                    }
                }
            }
        }
        //读取样本抽样设置信息
        public void ReadSampleSetting(string sProjectID)
        {
            if (ProjectID != sProjectID)
                return;

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);

            //拉区位置精度检测项目信息表信息
            string sqlProjecDetial = string.Format("select * from 位置精度检测项目信息表 where 成果名称='{0}' order by 抽样分区,流水号", ProjectName);
            DataTable datatable = dbread.GetDataTableBySQL(sqlProjecDetial);
            MapSampleSetting = new List<MapSampleItemSetting>();

            int index = 0;
            if (ShowProgress != null)
            {
                ShowProgress("SETMIN", 0);
                ShowProgress("SETMAX", datatable.Rows.Count);

            }

            foreach (DataRow dr in datatable.Rows)
            {
                //////////////////抽样设置
                MapSampleItemSetting sampleset = new MapSampleItemSetting();
                sampleset.CheckType = Convert.ToString(dr["检测精度类型"]);
                sampleset.RErrorMax = Convert.ToDouble(dr["间距精度限差（mm）"]);
                sampleset.HErrorMax = Convert.ToDouble(dr["高程精度限差（m）"]);
                sampleset.PErrorMax = Convert.ToDouble(dr["平面精度限差（mm）"]);
                sampleset.Terrian = Convert.ToString(dr["地形"]);
                sampleset.MapNumber = Convert.ToString(dr["图幅号"]);
                sampleset.SampleSerial = Convert.ToInt32(dr["流水号"]);
                sampleset.SampleAreaIndex = Convert.ToInt32(dr["抽样分区"]);
                string elevationintervaltablename = "ElevationNoteIntervalDef";
                string sql_elevationInterval = string.Format("select 高程注记点间距 as a from {0} where 地形 = '{1}' and 比例尺='{2}'", dbread.GetTableName(elevationintervaltablename), sampleset.Terrian, this.MapScale.ToString());
                sampleset.ElevationNoteInterval = Convert.ToDouble(dbread.GetScalar(sql_elevationInterval));

                MapSampleSetting.Add(sampleset);

                if (ShowProgress != null)
                {
                    ShowProgress("SETVALUE", ++index);
                    if (index == datatable.Rows.Count)
                    {
                        ShowProgress("SETVALUE", 0);
                    }
                }
            }

        }

        //读取样本详细信息
        public void ReadSampleDetail(string sProjectID)
        {

            if (ProjectID != sProjectID)
                return;

            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);

            //拉区位置精度检测项目信息表信息
            string sqlProjecDetial = string.Format("select * from 位置精度检测项目信息表 where 成果名称='{0}' order by 抽样分区,流水号", ProjectName);
            DataTable datatable = dbread.GetDataTableBySQL(sqlProjecDetial);
            MapSampleSetting = new List<MapSampleItemSetting>();
            MapSampleQuality = new List<MapSampeItemQuality>();
            MapSampleStation = new List<MapSampleCheckState>();

            int index = 0;
            if (ShowProgress != null)
            {
                ShowProgress("SETMIN", 0);
                ShowProgress("SETMAX", datatable.Rows.Count);

            }

            foreach (DataRow dr in datatable.Rows)
            {
                //////////////////抽样设置
                MapSampleItemSetting sampleset = new MapSampleItemSetting();
                sampleset.CheckType = Convert.ToString(dr["检测精度类型"]);
                sampleset.RErrorMax = Convert.ToDouble(dr["间距精度限差（mm）"]);
                sampleset.HErrorMax = Convert.ToDouble(dr["高程精度限差（m）"]);
                sampleset.PErrorMax = Convert.ToDouble(dr["平面精度限差（mm）"]);
                sampleset.Terrian = Convert.ToString(dr["地形"]);
                sampleset.MapNumber = Convert.ToString(dr["图幅号"]);
                sampleset.SampleSerial = Convert.ToInt32(dr["流水号"]);
                sampleset.SampleAreaIndex = Convert.ToInt32(dr["抽样分区"]);
                string elevationintervaltablename = "ElevationNoteIntervalDef";
                string sql_elevationInterval = string.Format("select 高程注记点间距 as a from {0} where 地形 = '{1}' and 比例尺='{2}'", dbread.GetTableName(elevationintervaltablename), sampleset.Terrian, this.MapScale.ToString());
                sampleset.ElevationNoteInterval = Convert.ToDouble(dbread.GetScalar(sql_elevationInterval));

                MapSampleSetting.Add(sampleset);

                //////////////////抽样质量
                MapSampeItemQuality samplequality = new MapSampeItemQuality();
                samplequality.MapNumber = Convert.ToString(dr["图幅号"]);
                samplequality.SampleSerial = Convert.ToInt32(dr["流水号"]);
                samplequality.SampleAreaIndex = Convert.ToInt32(dr["抽样分区"]);

                samplequality.HeightError = new ErrorItem();
                samplequality.HeightError.MeanError = Convert.IsDBNull(dr["高程中误差"]) == true ? 999999 : Convert.ToDouble(dr["高程中误差"]);
                samplequality.HeightError.ErrorRatio = Convert.IsDBNull(dr["高程粗差比率"]) == true ? 999999 : Convert.ToDouble(dr["高程粗差比率"]);
                samplequality.HeightError.ErrorScore = Convert.IsDBNull(dr["高程精度得分"]) == true ? 999999 : Convert.ToDouble(dr["高程精度得分"]);
                samplequality.HeightError.ErrorComment = Convert.IsDBNull(dr["备注"]) == true ? "" : Convert.ToString(dr["备注"]);

                samplequality.PositionError = new ErrorItem();
                samplequality.PositionError.MeanError = Convert.IsDBNull(dr["点位中误差"]) == true ? 999999 : Convert.ToDouble(dr["点位中误差"]);
                samplequality.PositionError.ErrorRatio = Convert.IsDBNull(dr["点位粗差比率"]) == true ? 999999 : Convert.ToDouble(dr["点位粗差比率"]);
                samplequality.PositionError.ErrorScore = Convert.IsDBNull(dr["点位精度得分"]) == true ? 999999 : Convert.ToDouble(dr["点位精度得分"]);
                samplequality.PositionError.ErrorComment = Convert.IsDBNull(dr["备注"]) == true ? "" : Convert.ToString(dr["备注"]);

                /*samplequality.CountourError = new ErrorItem();
                samplequality.CountourError.MeanError = Convert.IsDBNull(dr["等高线中误差"]) == true ? 999999 : Convert.ToDouble(dr["等高线中误差"]);
                samplequality.CountourError.ErrorRatio = Convert.IsDBNull(dr["等高线粗差比率"]) == true ? 999999 : Convert.ToDouble(dr["等高线粗差比率"]);
                samplequality.CountourError.ErrorScore = Convert.IsDBNull(dr["等高线精度得分"]) == true ? 999999 : Convert.ToDouble(dr["等高线精度得分"]);
                samplequality.CountourError.ErrorComment = Convert.IsDBNull(dr["备注"]) == true ? "" : Convert.ToString(dr["备注"]);
                */
                samplequality.RelativeError = new ErrorItem();
                samplequality.RelativeError.MeanError = Convert.IsDBNull(dr["间距中误差"]) == true ? 999999 : Convert.ToDouble(dr["间距中误差"]);
                samplequality.RelativeError.ErrorRatio = Convert.IsDBNull(dr["间距粗差比率"]) == true ? 999999 : Convert.ToDouble(dr["间距粗差比率"]);
                samplequality.RelativeError.ErrorScore = Convert.IsDBNull(dr["间距精度得分"]) == true ? 999999 : Convert.ToDouble(dr["间距精度得分"]);
                samplequality.RelativeError.ErrorComment = Convert.IsDBNull(dr["备注"]) == true ? "" : Convert.ToString(dr["备注"]);

                samplequality.Comment = Convert.IsDBNull(dr["备注"]) == true ? "" : Convert.ToString(dr["备注"]);

                MapSampleQuality.Add(samplequality);

                MapSampleCheckState Checkstate = new MapSampleCheckState();
                Checkstate.MapNumber = Convert.ToString(dr["图幅号"]);
                Checkstate.SampleSerial = Convert.ToInt32(dr["流水号"]);
                Checkstate.SampleAreaIndex = Convert.ToInt32(dr["抽样分区"]);

                string sql_select = string.Format("select count(ptid) from scater as s ,mapbindingtable as m where st_within(s.wkb_geometry, m.wkb_geometry) = true and m.projectid = '{0}' and m.mapnumber='{1}'", ProjectID, samplequality.MapNumber);
                Checkstate.PHCheckPoints = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select count(ptid) from checklinecollection as s ,mapbindingtable as m where st_within(st_point(sx, sy), m.wkb_geometry) = true and m.projectid = '{0}' and  m.mapnumber='{1}' and pointtype='{2}' ", ProjectID, samplequality.MapNumber, "PlanCheck");
                Checkstate.PCheckPoints = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select count(ptid) from checklinecollection as s , mapbindingtable as m where st_within(st_point(sx, sy), m.wkb_geometry) = true and m.projectid = '{0}' and  m.mapnumber = '{1}' and pointtype = '{2}' ", ProjectID, samplequality.MapNumber, "HeightCheck");
                Checkstate.HCheckPoints = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select count(ptid) from 间距边长精度检测点成果表 where mapnumber='{0}'", samplequality.MapNumber);
                Checkstate.RCheckPoints = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select count(ptid) from checklinecollection as s ,mapbindingtable as m where st_within(st_point(sx, sy), m.wkb_geometry) = true and m.projectid = '{0}' and  m.mapnumber='{1}' and pointtype='{2}' ", ProjectID, samplequality.MapNumber, "ControlPoint");
                Checkstate.ControlPoints = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select count(ptid) from checklinecollection as s ,mapbindingtable as m where st_within(st_point(sx, sy), m.wkb_geometry) = true and m.projectid = '{0}' and  m.mapnumber='{1}' and pointtype='{2}' ", ProjectID, samplequality.MapNumber, "NonCheck");
                Checkstate.NonCheckPoints = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select count(ptid) from checklinecollection as s ,mapbindingtable as m where st_within(st_point(sx, sy), m.wkb_geometry) = true and m.projectid = '{0}' and  m.mapnumber='{1}' and pointtype='{2}' ", ProjectID, samplequality.MapNumber, "WaitToCheck");
                Checkstate.WaitToChecks = Convert.ToInt32(dbread.GetScalar(sql_select));
                sql_select = string.Format("select distinct 检查者 from 平面及高程精度检测点成果表 where mapnumber='{0}'", samplequality.MapNumber);
                Checkstate.PHChecker = Convert.ToString(dbread.GetScalar(sql_select));
                sql_select = string.Format("select distinct 检查者 from 间距边长精度检测点成果表 where mapnumber='{0}'", samplequality.MapNumber);
                Checkstate.RChecker = Convert.ToString(dbread.GetScalar(sql_select));

                MapSampleStation.Add(Checkstate);


                if (ShowProgress != null)
                {
                    ShowProgress("SETVALUE", ++index);
                    if(index == datatable.Rows.Count)
                    {
                        ShowProgress("SETVALUE", 0);
                    }
                }
            }
        }

        public void UpdateLastOpenTime()
        {
            string lastopendatetime = DateTime.Now.ToString();
            string sql_update = string.Format("update {0} set lastopentime='{1}' where projectid = '{2}'", "dlgcheckproject", lastopendatetime, ProjectID);
            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);

            DatabaseDesignPlus.IDatabaseReaderWriter dbread = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);

            dbread.ExecuteSQL(sql_update);
        }
        public MapSampleItemSetting GetMapSampleItemSetting(string mapnumber)
        {
            MapSampleItemSetting mapsampleset = null;
            foreach (MapSampleItemSetting item in this.MapSampleSetting)
            {
                if (item.MapNumber == mapnumber)
                    mapsampleset = item;
            }
            return mapsampleset;
        }
        //以mapnumber、项目名称为关键词在项目范围内更新、插入单位成果检查设置
        public bool UpdateMapSampleItemSetting(MapSampleItemSetting NewItem)
        {
            bool bSuccess = false;
            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);
            DatabaseDesignPlus.IDatabaseReaderWriter dbwrite = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);
            //检查该单位成果设置是否存在，存在则更新，不存在则插入新记录
            if (this.GetMapSampleItemSetting(NewItem.MapNumber)!=null)
            {
                string updatesql = string.Format("update 位置精度检测项目信息表 set 检测精度类型 = '{0}',间距精度限差（mm）={1}, 高程精度限差（m）={2},平面精度限差（mm）={3},地形='{4}',流水号={5},抽样分区={6} where projectid='{7}' and 图幅号='{8}'", 
                    NewItem.CheckType, NewItem.RErrorMax,NewItem.HErrorMax,NewItem.PErrorMax,NewItem.Terrian,NewItem.SampleSerial,NewItem.SampleAreaIndex,this.ProjectID,NewItem.MapNumber);

                if (dbwrite.ExecuteSQL(updatesql) > 0)
                    bSuccess = true;

            }
            else
            {
                //成果名称，批量，样本数量，批量单位，抽样分区，流水号,图幅号,比例尺,地形,平面精度限差（mm）,高程精度限差（m）,间距精度限差（mm）,检测精度类型,点位中误差,点位粗差比率,点位精度得分,高程中误差,高程粗差比率,高程精度得分,等高线中误差,等高线粗差比率,等高线精度得分,间距中误差,间距粗差比率,间距精度得分,备注,projectid
                string insertsql = string.Format("insert into  位置精度检测项目信息表 values('{0}',{1},{2},'{3}',{4},{5},'{6}',{7},'{8}',{9},{10},{11},'{12}',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'{13}')",
                    this.ProjectName, this.nLots, this.nSampleSize,'幅',NewItem.SampleAreaIndex,NewItem.SampleSerial,NewItem.MapNumber,this.MapScale,NewItem.Terrian,NewItem.PErrorMax,NewItem.HErrorMax,NewItem.HErrorMax,NewItem.CheckType,this.ProjectID);

                if (dbwrite.ExecuteSQL(insertsql) > 0)
                    bSuccess = true;
            }

            return bSuccess;
        }
        public bool DeleteMapSampleItemSetting(MapSampleItemSetting deleteItem)
        {
            bool bSuccess = false;
            string pgdbconstr = System.Configuration.ConfigurationManager.AppSettings["Login"];
            pgdbconstr = DataBaseConfigs.RePlaceConfig(pgdbconstr);
            DatabaseDesignPlus.IDatabaseReaderWriter dbwrite = DatabaseDesignPlus.DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", pgdbconstr);
            //检查该单位成果设置是否存在，存在则更新，不存在则插入新记录
            if (this.GetMapSampleItemSetting(deleteItem.MapNumber) != null)
            {
                string deletesql = string.Format("delete from 位置精度检测项目信息表 where 图幅号='{0}' and projectid='{1}'", deleteItem.MapNumber, this.ProjectID);
                if (dbwrite.ExecuteSQL(deletesql) > 0)
                    bSuccess = true;
            }
            return bSuccess;
        }

        public void Export(string savefilepath)
        {
            try
            {
                Stream fstream = new FileStream(savefilepath, FileMode.Create);
                XmlSerializer xmlFormat = new XmlSerializer(typeof(DLGCheckLib.DLGCheckProjectClass));
                xmlFormat.Serialize(fstream, this);
                fstream.Dispose();
            }catch
            {

            }

        }
    }

    public class MapSampleItemSetting
    {
        public int SampleAreaIndex { get; set; }
        public int SampleSerial { get; set; }
        public string MapNumber { get; set; }
        public string Terrian { get; set; }
        public double PErrorMax { get; set; }
        public double HErrorMax { get; set; }
        public double RErrorMax { get; set; }
        public string CheckType { get; set; }
        //高程注记点间距，用来设置高程检测线搜索半径，一般采用1/3间距即可；
        public double ElevationNoteInterval { set; get; }

    }
    public class MapSampeItemQuality
    {
        public int SampleAreaIndex { get; set; }
        public int SampleSerial { get; set; }
        public string MapNumber { get; set; }
        public ErrorItem PositionError { get; set; }
        public ErrorItem HeightError { get; set; }
        //public ErrorItem CountourError { get; set; }
        public ErrorItem RelativeError { get; set; }
        public string PErrorGrossRatio { get { return PositionError.ErrorRatio == 999999 ? "--" : string.Format("{0:N0}", PositionError.ErrorRatio); } }
        public string HErrorGrossRatio { get { return HeightError.ErrorRatio == 999999 ? "--" : string.Format("{0:N0}", HeightError.ErrorRatio); } }
        public string RErrorGrossRatio { get { return RelativeError.ErrorRatio == 999999 ? "--" : string.Format("{0:N0}", RelativeError.ErrorRatio); } }
        public string Comment { set; get; }


    }
    public class MapSampleCheckState
        {
        public int SampleAreaIndex { get; set; }
        public int SampleSerial { get; set; }
        public string MapNumber { get; set; }

        public int PHCheckPoints { set; get; }
        public int WaitToChecks { set; get; }
        public int ControlPoints { set; get; }
        public int NonCheckPoints { set; get; }
        public int PCheckPoints { set; get; }
        public int HCheckPoints { set; get; }
        public string PHChecker { set; get; }
        public int RCheckPoints { set; get; }
        public string RChecker { set; get; }

    }
    public class ErrorItem
    {
        public double MeanError { get; set; }
        public double ErrorRatio { get; set; }
        public  double ErrorScore { get; set; }
        public string ErrorComment { get; set; }
        public override string ToString()
        {
            if (MeanError == 999999) return "--";
            else
            return string.Format("{0:N2}",MeanError);
        }

    }
}

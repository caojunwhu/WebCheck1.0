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
    public class SampleScorer
    {
        public static bool EvluationOfSample(DLGCheckProjectClass oProject, string sMapnumber, Dictionary<string, string> Configs, Dictionary<string, string> Databases)
        {
            bool bSuccess = true;

            QualityItems qitems = QualityItems.FromJson(oProject.productType);

            string samplecheckrecordtable = "检查意见记录表";
            string samplequalitytable = "样本质量评价表";
            string selecteditems = "ahselecteditems";
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);
            //如果不存在表创建表，如果存在表，删除原有记录，插入新记录
            List<string> tableNames2 = datareadwrite.GetSchameDataTableNames();
            if (tableNames2.Contains(samplequalitytable) == false)
            {
                string sql_createTable = string.Format("create table {0}( ProjectID text,Mapnumber text,样本质量得分 numeric(5,2) ,质量元素 text,质量元素权 numeric(5,2),质量元素得分 numeric(5,2) ,质量子元素 text,质量子元素权 numeric(5,2) ,质量子元素得分 numeric(5,2),A integer,B integer,C integer,D integer,Comment text)", samplequalitytable);
                datareadwrite.ExecuteSQL(sql_createTable);
            }

            ///根据质量模型拉取各项指标进行数据跟新
            ///
            //写入质量元素/子元素/权值/等信息；
            //补充点位/间距/高程精度得分/粗差率情况/中误差值等，记录到表格
            //从检查记录中提取相应记录写入到评分表格中，针对对应元素值更新
            string sql_projectquality = string.Format("select * from {0} where projectid='{1}' and mapnumber = '{2}'", samplequalitytable, oProject.ProjectID, sMapnumber);
            if (datareadwrite.GetDataTableBySQL(sql_projectquality).Rows.Count != qitems.Count)
            {
                string sql_delete = string.Format("delete  from {0} where projectid='{1}' and mapnumber = '{2}'", samplequalitytable, oProject.ProjectID, sMapnumber);
                datareadwrite.ExecuteSQL(sql_delete);

                foreach (KeyValuePair<string, string> DQPair in qitems.DicQItems)
                {
                    string sql_insertandupdate = "";
                    string sql_selectqitem = string.Format("select * from {0} where 质量元素 = '{0}' and 质量子元素='{1}' and projectid='{2}' and mapnumber='{3}'", samplequalitytable, DQPair.Value, DQPair.Key, oProject.ProjectID, sMapnumber);
                    if (datareadwrite.GetDataTableBySQL(sql_selectqitem).Rows.Count < 1)
                    {
                        string sql_qitemweight = string.Format("select 质量元素权 from {0} where 成果种类 = '{1}' and 质量元素 = '{2}'", selecteditems, qitems.QualityName, DQPair.Value);
                        double qitemweight = Convert.ToDouble(datareadwrite.GetScalar(sql_qitemweight));
                        string sql_sqitemweight = string.Format("select 质量子元素权 from {0} where  成果种类 = '{1}' and 质量元素 = '{2}' and 质量子元素='{3}'", selecteditems, qitems.QualityName, DQPair.Value, DQPair.Key);
                        double sqitemweight = Convert.ToDouble(datareadwrite.GetScalar(sql_sqitemweight));
                        sql_insertandupdate = string.Format("insert into {0} values('{1}','{2}',{3},'{4}',{5},{6},'{7}',{8},{9},{10},{11},{12},{13},'{14}')",
                            samplequalitytable, oProject.ProjectID, sMapnumber, 100, DQPair.Value, qitemweight, 100, DQPair.Key, sqitemweight, 100, 0, 0, 0, 0, "");

                        datareadwrite.ExecuteSQL(sql_insertandupdate);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> dicqitem in qitems.DicQItems)
                {
                    //重置所有打分/扣分记录为初始值
                    string sql_reset = string.Format("update {0} set 样本质量得分=100 ,质量元素得分 =100 ,质量子元素得分 = 100, a=0,b=0,c=0,d=0  where projectid = '{1}' and mapnumber = '{2}' and 质量元素 = '{3}' and  质量子元素='{4}'",
                        samplequalitytable, oProject.ProjectID, sMapnumber, dicqitem.Value, dicqitem.Key);
                    datareadwrite.ExecuteSQL(sql_reset);

                    //更新对应的质量元素得分/扣分情况
                    string sql_error = string.Format("select 错漏类别,错漏描述 from {0} where projectid = '{1}' and mapnumber = '{2}' and 质量元素 = '{3}' and  质量子元素='{4}'", samplecheckrecordtable, oProject.ProjectID, sMapnumber, dicqitem.Value, dicqitem.Key);
                    DataTable dataerror = datareadwrite.GetDataTableBySQL(sql_error);
                    foreach (DataRow dr in dataerror.Rows)
                    {
                        string errorclass = dr["错漏类别"] as string;
                        string error = dr["错漏描述"] as string;
                        int errorcount = Convert.ToInt32(errorclass.Substring(1, errorclass.Length - 2));
                        string errortype = errorclass.Substring(errorclass.Length - 1, 1);

                        string sql_update = string.Format("update {0} set {1}={2}+{3} where projectid = '{4}' and mapnumber = '{5}' and 质量元素 = '{6}' and  质量子元素='{7}'", samplequalitytable, errortype.ToLower(), errortype.ToLower(), errorcount, oProject.ProjectID, sMapnumber, dicqitem.Value, dicqitem.Key);
                        datareadwrite.ExecuteSQL(sql_update);
                    }

                }
                string sql_updateScore = "";
                string sql_updateSoreNULL = "";
                //更新中误差得分
                //计算中误差

                HeightMeanError hme = new HeightMeanError(Configs, Databases);
                hme.QueryParameter(sMapnumber);
                hme.Calc(sMapnumber);
                hme.UpdateReslut(sMapnumber);

                sql_updateScore = string.Format("update {0} set 质量子元素得分={1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素='{5}'", samplequalitytable, hme.vScore, oProject.ProjectID, sMapnumber, "数学精度", "高程精度");
                sql_updateSoreNULL = string.Format("update {0} set 质量子元素得分=NULL where projectid = '{1}' and mapnumber = '{2}' and 质量元素 = '{3}' and  质量子元素='{4}'", samplequalitytable, oProject.ProjectID, sMapnumber, "数学精度", "高程精度");
                datareadwrite.ExecuteSQL(hme.vScore > 0 ? sql_updateScore : sql_updateSoreNULL);

                //计算中误差
                PositionMeanError pme = new PositionMeanError(Configs, Databases);
                pme.QueryParameter(sMapnumber);
                pme.Calc(sMapnumber);
                pme.UpdateReslut(sMapnumber);

                RelativeMeanError rme = new RelativeMeanError(Configs, Databases);
                rme.QueryParameter(sMapnumber);
                rme.Calc(sMapnumber);
                rme.UpdateReslut(sMapnumber);


                sql_updateSoreNULL = string.Format("update {0} set 质量子元素得分=NULL where projectid = '{1}' and mapnumber = '{2}' and 质量元素 = '{3}' and  质量子元素='{4}'", samplequalitytable, oProject.ProjectID, sMapnumber, "数学精度", "平面精度");
                if (rme.vScore < 0 && pme.vScore < 0)
                {
                    datareadwrite.ExecuteSQL(sql_updateSoreNULL);
                }
                else if (rme.vScore > 0 && pme.vScore > 0)
                {
                    sql_updateScore = string.Format("update {0} set 质量子元素得分={1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素='{5}'", samplequalitytable, (rme.vScore + pme.vScore) / 2, oProject.ProjectID, sMapnumber, "数学精度", "平面精度");
                    datareadwrite.ExecuteSQL(sql_updateScore);
                }
                else if (rme.vScore > 0 && pme.vScore < 0)
                {
                    sql_updateScore = string.Format("update {0} set 质量子元素得分={1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素='{5}'", samplequalitytable, rme.vScore, oProject.ProjectID, sMapnumber, "数学精度", "平面精度");
                    datareadwrite.ExecuteSQL(sql_updateScore);

                }
                else if (rme.vScore < 0 && pme.vScore > 0)
                {
                    sql_updateScore = string.Format("update {0} set 质量子元素得分={1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素='{5}'", samplequalitytable, pme.vScore, oProject.ProjectID, sMapnumber, "数学精度", "平面精度");
                    datareadwrite.ExecuteSQL(sql_updateScore);

                }

                //根据扣分情况求取质量子元素得分
                foreach (KeyValuePair<string, string> dicqitem in qitems.DicQItems)
                {
                    double reduceScore = SampleScorer.ReductionScore(oProject.ProjectID, sMapnumber, dicqitem.Value, dicqitem.Key);
                    sql_updateScore = string.Format("update {0} set 质量子元素得分=质量子元素得分-{1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素='{5}'", samplequalitytable, reduceScore, oProject.ProjectID, sMapnumber, dicqitem.Value, dicqitem.Key);
                    datareadwrite.ExecuteSQL(sql_updateScore);

                }
                //获取质量元素下属的质量子元素得分/权值/计算加权平均值
                //如存在得分小于60分的，将整个元素得分置为0;

                foreach (string qitemName in qitems.QualityItemNames)
                {
                    double weightAdded = 0;
                    double weightScoreAdded = 0;
                    bool hasLowScore = false;
                    List<string> sqitemName = (from q in qitems.DicQItems where q.Value == qitemName select q.Key).ToList<string>();
                    foreach (string sqName in sqitemName)
                    {
                        string sql_weight = string.Format("select 质量子元素权 from {0} where  成果种类 = '{1}' and 质量元素 = '{2}' and 质量子元素='{3}'", selecteditems, qitems.QualityName, qitemName, sqName);
                        string sql_Score = string.Format("select 质量子元素得分 from {0} where  质量元素 = '{1}' and 质量子元素='{2}' and projectid = '{3}' and mapnumber = '{4}'", samplequalitytable, qitemName, sqName, oProject.ProjectID, sMapnumber);

                        double weight = Convert.ToDouble(datareadwrite.GetScalar(sql_weight));
                        double Score = Convert.ToDouble(datareadwrite.GetScalar(sql_Score));

                        if (Score < 60) hasLowScore = true;

                        weightAdded += weight;
                        weightScoreAdded += weight * Score;
                    }

                    //更新质量元素得分
                    weightScoreAdded = weightScoreAdded / weightAdded;
                    if (hasLowScore == true) weightScoreAdded = 0;
                    sql_updateScore = string.Format("update {0} set 质量元素得分={1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' ", samplequalitytable, weightScoreAdded, oProject.ProjectID, sMapnumber, qitemName);
                    datareadwrite.ExecuteSQL(sql_updateScore);
                }

                //获取质量元素得分/权值/计算加权平均值
                double weightAdded0 = 0;
                double weightScoreAdded0 = 0;
                bool hasLowScore0 = false;
                foreach (string qitemName in qitems.QualityItemNames)
                {
                    string sql_weight = string.Format("select 质量元素权 from {0} where  成果种类 = '{1}' and 质量元素 = '{2}' ", selecteditems, qitems.QualityName, qitemName);
                    string sql_Score = string.Format("select 质量元素得分 from {0} where  质量元素 = '{1}' and projectid = '{2}' and mapnumber = '{3}'", samplequalitytable, qitemName, oProject.ProjectID, sMapnumber);

                    double weight = Convert.ToDouble(datareadwrite.GetScalar(sql_weight));
                    double Score = Convert.ToDouble(datareadwrite.GetScalar(sql_Score));
                    if (Score < 60) hasLowScore0 = true;

                    weightAdded0 += weight;
                    weightScoreAdded0 += weight * Score;
                }
                //更新样本单位图幅质量元素得分
                weightScoreAdded0 = weightScoreAdded0 / weightAdded0;
                if (hasLowScore0 == true) weightScoreAdded0 = 0;

                sql_updateScore = string.Format("update {0} set 样本质量得分={1} where projectid = '{2}' and mapnumber = '{3}' ", samplequalitytable, weightScoreAdded0, oProject.ProjectID, sMapnumber);
                datareadwrite.ExecuteSQL(sql_updateScore);

                //更新抽样样本质量得分，在位置精度检测项目信息表中备注字段写入得分值
                sql_updateScore = string.Format("update 位置精度检测项目信息表 set 备注='{0}' where projectid='{1}' and 图幅号='{2}'", weightScoreAdded0, oProject.ProjectID, sMapnumber);
                datareadwrite.ExecuteSQL(sql_updateScore);
               
            }
            return bSuccess;
        }
        public static double ReduceScore(string errorType)
        {
            double reduceScorce = 0;
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);

            string sourceTable = "ah成果质量错漏扣分标准";
            string sql = string.Format("select 扣分值 from {0} where 差错类型= '{1}' ", sourceTable, errorType);
            reduceScorce = Convert.ToDouble(datareadwrite.GetScalar(sql));

            return reduceScorce;
        }

        //获取某一样本某一质量子元素所有扣分值之和
        public static double ReductionScore(string projectid ,string mapnumber,string qitemName,string sqitemName)
        {
            string SDbConnectionString = System.Configuration.ConfigurationManager.AppSettings["Login"];
            SDbConnectionString = DataBaseConfigs.RePlaceConfig(SDbConnectionString);

            IDatabaseReaderWriter datareadwrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", SDbConnectionString);

            string sourceTable = "样本质量评价表";
            double reductionScore = 0;
            string sql_ErrorNumberOfType = "";
            int numA = 0, numB = 0, numC = 0, numD = 0;

            sql_ErrorNumberOfType = string.Format("select {0} from {1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素 = '{5}' ", 
                "a",sourceTable,projectid,mapnumber,qitemName,sqitemName);

            numA = Convert.ToInt32(datareadwrite.GetScalar(sql_ErrorNumberOfType));

            sql_ErrorNumberOfType = string.Format("select {0} from {1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素 = '{5}' ",
                "b", sourceTable, projectid, mapnumber, qitemName, sqitemName);

            numB = Convert.ToInt32(datareadwrite.GetScalar(sql_ErrorNumberOfType));

            sql_ErrorNumberOfType = string.Format("select {0} from {1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素 = '{5}' ",
                  "c", sourceTable, projectid, mapnumber, qitemName, sqitemName);

            numC = Convert.ToInt32(datareadwrite.GetScalar(sql_ErrorNumberOfType));

            sql_ErrorNumberOfType = string.Format("select {0} from {1} where projectid = '{2}' and mapnumber = '{3}' and 质量元素 = '{4}' and  质量子元素 = '{5}' ",
                 "d", sourceTable, projectid, mapnumber, qitemName, sqitemName);

            numD = Convert.ToInt32(datareadwrite.GetScalar(sql_ErrorNumberOfType));


            reductionScore = ReduceScore("A") * numA + ReduceScore("B") * numB + ReduceScore("C") * numC + ReduceScore("D") * numD;


            return reductionScore;
        }
    }
}

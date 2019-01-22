using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseDesign;
using System.Data;
using Newtonsoft.Json;
using System.IO;
using DatabaseDesignPlus;

namespace ReportPrinter
{
	interface IMeanError
	{
        DataTableDesign ProjectDataTableDesign { get; }
        DataTableDesign ErrorDataTableDesign { get; }
        DataTableDesign MeanErrorDataTableDesign { get; }
        double vMaxError { set; get; }
        double vGrossError { set; get; }
        string sFactorType { set; get; }// "同精度";
        double vFactor { set; get; }//; //超精度时为√2，通精度时为1，统计粗差
        int vScale { set; get; }// 1;
        int nPointCount { set; get; }
        int nGrossErrorCount { set; get; }
        int nValidErrorCount{ set; get; }
        double nGrossErrorRatio { set; get; }
        double vAvgError { set; get; }
        double vError { set; get; }
        string sErrorType { set; get; }

        DataTable dPointValue { set; get; }
        //void Calc(string sMapNumber);
	}
    interface IParameterQuery
    {
        void QueryParameter(string sMapNumber);
        
    }
    interface IResultUpdate
    {
        void UpdateReslut(string sMapNumber);
        void UpdateScore(string sMapNumber);
    }
    interface ICalc
    {
        void Calc(string sMapNumber);
    }

    public class MeanError : IMeanError, IParameterQuery, IResultUpdate, ICalc
    {
        public MeanError(Dictionary<string, string> Configs, Dictionary<string, string> Databases)
        {
            this.Configs = Configs;
            this.Databases = Databases;
            _ProjectDataTableDesign = new DataTableDesign(
                    Databases["SurveryProductCheckDatabase"],
                        "PostgreSQL",

                    Configs["TopographicMapLocationAccuracyCheckDataTableDesignName"],
                    Configs["DataTableDesignVersion"],
                    Configs["DataTableDesignFieldAttributesName"],

                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",
                    Configs["DatatableDataTypeRelationDataTableName"]);

            _ProjectDataTableDesign.InitializeDataTableDesign("PostgreSQL", Configs["TopographicMapLocationAccuracyCheckDataTableName"]);
        }
        public MeanError()
        {
            _ProjectDataTableDesign = new DataTableDesign(
                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",

                    Configs["TopographicMapLocationAccuracyCheckDataTableDesignName"],
                    Configs["DataTableDesignVersion"],
                    Configs["DataTableDesignFieldAttributesName"],

                    Databases["SurveryProductCheckDatabase"],
                    "PostgreSQL",
                    Configs["DatatableDataTypeRelationDataTableName"]);

            _ProjectDataTableDesign.InitializeDataTableDesign("PostgreSQL",Configs["TopographicMapLocationAccuracyCheckDataTableName"]);
        }

        DataTableDesign _ProjectDataTableDesign = null;
        public DataTableDesign ProjectDataTableDesign { set { _ProjectDataTableDesign = value; } get { return _ProjectDataTableDesign; } }

        public DataTableDesign _ErrorDataTableDesign = null;
        public DataTableDesign ErrorDataTableDesign { set { _ErrorDataTableDesign = value; } get { return _ErrorDataTableDesign; } }

        public DataTableDesign _MeanErrorDataTableDesign = null;
        public DataTableDesign MeanErrorDataTableDesign { set { _MeanErrorDataTableDesign = value; } get { return _MeanErrorDataTableDesign; } }

        double _vMaxError = 0;
        public double vMaxError { set { _vMaxError = value; } get { return _vMaxError; } }//允许误差

        double _vGrossError = 0;
        public double vGrossError { set { _vGrossError = value; } get { return _vGrossError; } }//粗差限

        string _sFactorType = "";
        public string sFactorType { set { _sFactorType = value; } get { return _sFactorType; } }// "同精度";

        double _vFactor = 0;
        public double vFactor { set { _vFactor = value; } get { return _vFactor; } }//; //超精度时为√2，通精度时为1，统计粗差

        int _vScale = 0;
        public int vScale { set { _vScale = value; } get { return _vScale; } }// 1;

        double _vScore = 0;
        public double vScore { set { _vScore = value; } get { return _vScore; } }//精度数学得分

        int _nPointCount = 0;
        public int nPointCount { set { _nPointCount = value; } get { return _nPointCount; } }

        DataTable _dPointValue = null;
        public DataTable dPointValue { set { _dPointValue = value; } get { return _dPointValue; } }

        public void Calc(string sMapNumber) { }
        public void UpdateScore(string sMapNumber) { }

        public int nGrossErrorCount { set; get; }
        public int nValidErrorCount { set; get; }
        public double nGrossErrorRatio { set; get; }
        public double vAvgError { set; get; }
        public double vError { set; get; }
        public string sErrorType { set; get; }

        public Dictionary<string, string> Configs { get; set; }
        public Dictionary<string, string> Databases { get; set; }


        //根据GB/T 24356-2009 《测绘成果质量检查与验收》5.4.1 数学精度评分方法设计计算精度函数
        public double CalculateMathScore(double m1, double m2, double M)
        {
            double S = 0.0;
            double M0 = 0.0;

            M0 = Math.Sqrt(m1 * m1 + m2 * m2);
            if (M <= M0 / 3)
            {
                S = 100;
            }
            else if (M > M0 / 3 && M <= M0 / 2)
            {
                S = (M - M0 / 3) / (M0 / 2 - M0 / 3) * 10.0 + 90;
            }
            else if (M > M0 / 2 && M <= M0 * 3 / 4)
            {
                S = (M - M0 / 2) / (M0 * 3 / 4 - M0 / 2) * 15 + 75;
            }
            else if (M > M0 * 3 / 4 && M <= M0)
            {
                S = (M - M0 * 3 / 4) / (M0 / 4) * 15 + 60;
            }
            return S;

        }

        public void QueryParameter(string sMapNumber)
        {
            //Clsmdb mdb = new Clsmdb();
            //DataTable meanerrordatatable = mdb.GetMdbDataTable(_MeanErrorDataTableDesign.DataTableDesignSorceFilePath, _MeanErrorDataTableDesign.CreateTableName);

            //DataTable meanerrordatatable = ClsPostgreSql.GetDataTableByName(_MeanErrorDataTableDesign.DataTableDesignSorceFilePath, _MeanErrorDataTableDesign.CreateTableName);

            IDatabaseReaderWriter dbReadWrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", Databases["SurveryProductCheckDatabase"]);
            DataTable meanerrordatatable = dbReadWrite.GetDataTable(_MeanErrorDataTableDesign.CreateTableName);

            foreach (DataRow dr in meanerrordatatable.Rows)
            {
                string attributename = "";
                try
                {
                    attributename = dr[_MeanErrorDataTableDesign.FieldDesigns[2].FieldCode] as string;
                    System.Reflection.PropertyInfo property = this.GetType().GetProperty(attributename);
                    JsonSerializer serializer = new JsonSerializer();
                    StringReader sr = new StringReader(dr[_MeanErrorDataTableDesign.FieldDesigns[3].FieldCode] as string);
                    DataValueQuery dq = (DataValueQuery)serializer.Deserialize(new JsonTextReader(sr), typeof(DataValueQuery));
                    //替换参数
                    string sql = dq.QueryCondition;
                    dq.QueryCondition = sql.Replace("{*}", sMapNumber);
                    dq.DbFilePath = Databases["SurveryProductCheckDatabase"];
                    property.SetValue(this, dq.QueryValue, null);

                    if (attributename == "nPointCount" && nPointCount == 0)
                        return;
                }
                catch(Exception ex)
                {
                    
                }
            }
        }

        public void UpdateReslut(string sMapNumber)
        {
            try
            {
                //Clsmdb mdb = new Clsmdb();
                //将所有点备注均设置为""
                IDatabaseReaderWriter dbReadWrite;
                dbReadWrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", Databases["SurveryProductCheckDatabase"]);
               foreach (DataRow dr in dPointValue.Rows)
                {
                    string id = Convert.ToString(dr["ID"]);
                    string updateGrossErrorValuesMark = string.Format("update {0} set COMMENT = ''  where MapNumber = '{2}' and PtID = '{3}'", ErrorDataTableDesign.CreateTableName, "粗差", sMapNumber, id);
                    //mdb.GetSaclar(CallClass.Databases["SPIDatabaseFilePath"], updateGrossErrorValuesMark);
                    //ClsPostgreSql.GetSaclar( Databases["SurveryProductCheckDatabase"], updateGrossErrorValuesMark);
                    dbReadWrite.ExecuteSQL(updateGrossErrorValuesMark);
                }

                //查找需要更新的参数，及判断是否超限
                string updateErrorValues = string.Format("update {0} set ", ProjectDataTableDesign.CreateTableName);
                //DataTable meanerrordatatable = mdb.GetMdbDataTable(_MeanErrorDataTableDesign.DataTableDesignSorceFilePath, _MeanErrorDataTableDesign.CreateTableName);
                //DataTable meanerrordatatable = ClsPostgreSql.GetDataTableByName(_MeanErrorDataTableDesign.DataTableDesignSorceFilePath, _MeanErrorDataTableDesign.CreateTableName);
                dbReadWrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", Databases["SurveryProductCheckDatabase"]);
                DataTable meanerrordatatable = dbReadWrite.GetDataTable(_MeanErrorDataTableDesign.CreateTableName);

                foreach (DataRow dr in meanerrordatatable.Rows)
                {
                    string isUpdate = dr[_MeanErrorDataTableDesign.FieldDesigns[4].FieldCode] as string;
                    if (isUpdate == "是")
                    {
                        string attributename = dr[_MeanErrorDataTableDesign.FieldDesigns[2].FieldCode] as string;
                        string updatefieldname = dr[_MeanErrorDataTableDesign.FieldDesigns[1].FieldCode] as string;
                        System.Reflection.PropertyInfo property = this.GetType().GetProperty(attributename);
                        double updatevalue = Convert.ToDouble(property.GetValue(this, null));

                        if (updatevalue != -1)
                        {
                            updateErrorValues = string.Format("{0} {1} = '{2}',", updateErrorValues, updatefieldname, updatevalue.ToString("0.00"));
                        }
                        else
                        {
                            updateErrorValues = string.Format("{0} {1} = NULL,", updateErrorValues, updatefieldname);
                        }
                    }
                }
                if (vError > vMaxError)
                {
                    updateErrorValues = string.Format("{0} 备注='超 限'", updateErrorValues);
                }
                else
                {
                    updateErrorValues = updateErrorValues.Remove(updateErrorValues.Length - 1);
                }

                updateErrorValues = string.Format("{0} where 图幅号 = '{1}'", updateErrorValues, sMapNumber);
                //mdb.GetSaclar(CallClass.Databases["SPIDatabaseFilePath"], updateErrorValues);
                //ClsPostgreSql.GetSaclar( Databases["SurveryProductCheckDatabase"], updateErrorValues);
                dbReadWrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", Databases["SurveryProductCheckDatabase"]);
                //ClsPostgreSql.GetSaclar( Databases["SurveryProductCheckDatabase"], updateGrossErrorValuesMark);
                dbReadWrite.ExecuteSQL(updateErrorValues);


                foreach (DataRow dr in dPointValue.Rows)
                {
                    string id = Convert.ToString(dr["ID"]);
                    double value = Convert.ToDouble(dr["V"]);
                    if (Math.Abs(value) > vGrossError)
                    {
                        string updateGrossErrorValuesMark = string.Format("update {0} set COMMENT = '{1}'  where MapNumber = '{2}' and PtID = '{3}'", ErrorDataTableDesign.CreateTableName, "粗差", sMapNumber, id);
                       // mdb.GetSaclar(CallClass.Databases["SPIDatabaseFilePath"], updateGrossErrorValuesMark);
                       // ClsPostgreSql.GetSaclar( Databases["SurveryProductCheckDatabase"], updateGrossErrorValuesMark);
                        dbReadWrite.ExecuteSQL(updateGrossErrorValuesMark);
                    }
                }
            }
               catch(Exception ex)
            {
               // throw new Exception(ex.Message);
            }  
       
   }

    }
}

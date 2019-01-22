using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;

namespace DatabaseDesignPlus
{
	public interface IDatabaseReaderWriter
	{
         string DatatbaseConnectionID { set; get; }
        string GetTableName(string sTableName);
        string GetConnectionString();
        DbDataAdapter GetDbDataAdapter(string sSQL, DbConnection dbConn);
        DbConnection GetDbConnection();
        DbCommand GetDbCommand(string sSQL, DbConnection dbConn);
        List<string> GetSchameDataTableNames(string whereclause = "");
        DataTable GetDataTable(string sTableName);
        DataTable GetDataTable(List<string> sFieldsList, string sTableName);
        DataTable GetDataTableBySQL(string sSql);
        Dictionary<string, string> GetMultiplyFieldValueDictonary(string sSql);
        Dictionary<string, string> GetKeyPairValueDictionary(string sSql);
        List<string> GetSingleFieldValueList(string sFieldName, string sSql);
        string GetCreateSQL(string tablename, DataTable pDataTable);
        List<string> GetAllFieldsNames(DataTable pDataTable);
        object GetScalar(string sSQL);
        int ExecuteSQL(string sSQL);
        string GetFieldListString(List<string> sTargetFields);
        string GetFieldString(string sTargetField);
        List<FieldDesign> GetFieldDesign(string sTableName);
        bool ImportDataTableRecords(string sTableName, List<string> sTargetFields, List<string> sTargetAddedFields,
            List<string> sIdentifyFieldIndexTarget, DataTable sDataTable,
            List<string> sSourceFields, List<string> sIdentifyFieldIndexSource,int nImportFlag=0);
        bool ImportDataTableRecords(string sTableName, DataTable sDataTable, int nImportFlag = 0);
        bool ImportDataTableRecords(string sTableName, DataTable sDataTable, DataTableDesign sDataTableDesign, int nImportFlag = 0);
	}


    public class DatabaseReaderWriter:IDatabaseReaderWriter
    {

        public virtual string DatatbaseConnectionID
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public virtual string GetTableName(string sTableName)
        {
            throw new NotImplementedException();
        }
        public virtual string GetConnectionString()
        {
            throw new NotImplementedException();
        }
        public virtual DbDataAdapter GetDbDataAdapter(string sSQL, DbConnection dbConn)
        {
            throw new NotImplementedException();
        }
        public virtual DbConnection GetDbConnection() {
            throw new NotImplementedException();
        }

        public virtual DbCommand GetDbCommand(string sSQL, DbConnection dbConn)
        {
            throw new NotImplementedException();
        }
        public virtual List<string> GetSchameDataTableNames(string whereclause = "")
        {
            throw new NotImplementedException();
        }
        public DataTable GetDataTable(string sTableName)
        {
            string TableName = GetTableName(sTableName);
            //List<string>TableNames=GetSchameDataTableNames();
            //if (TableNames.Contains(TableName) == false)
            //    throw new Exception(string.Format("查询表{0}不存在。",sTableName));

            try
            {
                DbConnection con = GetDbConnection();
                //获取数据表
                if (!(con.State == ConnectionState.Open))
                {
                    con.Open();
                }

                string pSql = "select * from " + TableName;
                DbDataAdapter pAdapter = GetDbDataAdapter(pSql, con);
                DataSet pDS = new DataSet();
                pAdapter.Fill(pDS);
                DataTable pTable = pDS.Tables[0];
                con.Close();
                return pTable;
            }
            catch (DbException ex)
            {
                string err = string.Format("读取表{0}错误：{1}\n", TableName, ex.Message);
                //MessageBox.Show("读取表" + TableName + "错误：\n" + ex.Message);
                //return null;
                throw new Exception(err);
            }
        }
        public DataTable GetDataTableBySQL(string sSql)
        {
            try
            {
                //获取数据表
                DbConnection con = GetDbConnection();
                con.Open();
                DbDataAdapter pAdapter = GetDbDataAdapter(sSql, con);
                DataSet pDS = new DataSet();
                pAdapter.Fill(pDS);
                DataTable pTable = pDS.Tables[0];
                con.Close();
                return pTable;
            }
            catch (DbException ex)
            {
                MessageBox.Show("执行" + sSql + "错误：\n" + ex.Message);
                return null;
            }
        }
        public Dictionary<string, string> GetMultiplyFieldValueDictonary(string sSql)
        {
            DataTable dt = GetDataTableBySQL(sSql);
            if (dt == null) return null;
            Dictionary<string, string> pMultiplyFieldValueDictionary = new Dictionary<string, string>();
            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    string key = dc.ColumnName;
                    string value = Convert.IsDBNull(dr[dc]) ? "" : Convert.ToString(dr[dc]);
                    pMultiplyFieldValueDictionary.Add(key, value);
                }
            }
            return pMultiplyFieldValueDictionary;
        }
        public Dictionary<string,string> GetKeyPairValueDictionary(string sSql)
        {
            DataTable dt = GetDataTableBySQL(sSql);
            if (dt == null) return null;
            Dictionary<string, string> pMultiplyFieldValueDictionary = new Dictionary<string, string>();
            if (dt.Columns.Count < 2) return null;

            foreach (DataRow dr in dt.Rows)
            {
                string key = Convert.IsDBNull(dr[0]) ? "" : Convert.ToString(dr[0]);
                string value = Convert.IsDBNull(dr[1]) ? "" : Convert.ToString(dr[1]);
                pMultiplyFieldValueDictionary.Add(key, value);
            }
            return pMultiplyFieldValueDictionary;
        }
        public List<string> GetSingleFieldValueList(string sFieldName, string sSql)
        {
            DataTable dt = GetDataTableBySQL(sSql);
            if (dt == null) return null;
            List<string> pFiledsList = new List<string>();

            foreach (DataRow dr in dt.Rows)
            {
                string fieldvalue = Convert.ToString(dr[sFieldName]);
                pFiledsList.Add(fieldvalue);
            }
            return pFiledsList;
        }
        public virtual string GetCreateSQL(string tablename, DataTable pDataTable)
        {
            throw new NotImplementedException();
        }
        public List<string> GetAllFieldsNames(DataTable pDataTable)
        {
            if (pDataTable == null)
            { return null; }
            List<string> pFiledsList = new List<string>();
            int pColmCount = pDataTable.Columns.Count;
            if (pColmCount > 0)
            {
                for (int i = 0; i < pColmCount; i++)
                {
                    Type pType = pDataTable.Columns[i].DataType;
                    string pFieldName;

                    pFieldName = pDataTable.Columns[i].ColumnName;
                    pFiledsList.Add(pFieldName);
                }
                return pFiledsList;
            }
            else
            { return null; }
        }
        public object GetScalar(string sSQL)
        {
            object reslut = null;
            try
            {
                //获取数据表
                DbConnection con = GetDbConnection();
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                DbCommand command = GetDbCommand(sSQL, con);
                reslut = command.ExecuteScalar();
                command.Dispose();
                con.Close();
                return reslut;
            }
            catch (DbException ex)
            {
                MessageBox.Show("读取表错误：\n" + ex.Message);
                return 0;
            }
        }
        public int ExecuteSQL(string sSQL)
        {
            int result = 0;
            try
            {
                //获取数据表
                DbConnection con = GetDbConnection();
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                DbCommand odbcommand = GetDbCommand(sSQL,con);
                result = odbcommand.ExecuteNonQuery();
                odbcommand.Dispose();
                con.Close();
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        public virtual string GetFieldListString(List<string> sTargetFields)
        {
            throw new NotImplementedException();
        }
        public virtual string GetFieldString(string sTargetField)
        {
            throw new NotImplementedException();
        }
        public DataTable GetDataTable(List<string> sFieldsList, string sTableName)
        {
            throw new NotImplementedException();
        }
        public List<FieldDesign> GetFieldDesign(string sTableName)
        {
            List<FieldDesign> fielddesign = new List<FieldDesign>();
            try
            {
                string seltop1 = string.Format("select  * from {0} limit 1",sTableName);     
                //DataTable dt = GetDataTable(sTableName);
                DataTable dt = GetDataTableBySQL(seltop1);
                foreach(DataColumn dc in dt.Columns)
                {
                    FieldDesign field = new FieldDesign();

                    field.FieldCode = dc.ColumnName;
                    field.FieldName = dc.ColumnName;
                    field.FieldType = dc.DataType.ToString();
                    field.FieldLength = dc.MaxLength;
                    field.FieldIsNull = dc.AllowDBNull==true?"是":"否";

                    fielddesign.Add(field);
                }
            }catch(Exception ex)
            {

            }

            return fielddesign;
        }
        void DropDataTable(string sTableName)
        {
                       //检索数据库中是否存在记录表
            List<string> sTableNames = GetSchameDataTableNames();
            if (sTableNames.Contains(sTableName) == false)
                return;

            string dropsql = string.Format("drop table {0}",GetTableName(sTableName));
            ExecuteSQL(dropsql);

        }
        // importtablerecords 从datatable导入新记录，其中记录标识符一致的提供插入新记录、部分属性更新（复写特定属性）、全部属性覆盖（删除全部原记录后新增）
        //nImportFlag = 0用来描述导入方式，0为覆盖，1为追加，覆盖时先删除表
        public bool ImportDataTableRecords(string sTableName,
            List<string> sTargetFields, List<string> sTargetAddedFields,
            List<string> sIdentifyFieldIndexTarget,
            DataTable sDataTable,
            List<string> sSourceFields, List<string> sIdentifyFieldIndexSource, int nImportFlag = 0)
        {
            bool bSuccess = false;
            if (sTargetFields.Count != sSourceFields.Count)
                throw new Exception(string.Format("请检查数据库{0}及导入数据表字段是否匹配！"));

            //nimportflag
            if (nImportFlag == 0)
                DropDataTable(sTableName);

            string sFieldList = GetFieldListString(sTargetFields);
            //检索数据库中是否存在记录表，否则创建新记录表
            List<string> sTableNames = GetSchameDataTableNames();
            //没有现成的表，需要新建一个表，并将记录逐条插入
            if (sTableNames.Contains(sTableName) == false)
            {
                try
                {
                    string sCreateTableSql = GetCreateSQL(sTableName, sDataTable);
                    ExecuteSQL(sCreateTableSql);

                    foreach (DataRow dr in sDataTable.Rows)
                    {
                        // 分析每条记录                        
                        string sFieldValues = "";
                        for (int i = 0; i < sTargetFields.Count; i++)
                        {
                            // 判断扩充字段，特殊处理
                            string sFieldValue = "";
                            #region //处理增加字段部分，在数据库表设计里体现
                            string sSourceFieldName = Convert.ToString(sSourceFields[i]);
                            string sTargetFieldName = Convert.ToString(sTargetFields[i]);
                            if (!sTargetAddedFields.Contains(sSourceFieldName))
                            {
                                //处理可选字段包括：检查者、检查日期，如果导入字段为空时，系统自动录入
                                /*if (sTargetFieldName == "检查者")
                                {
                                    //sFieldValue = CallClass.Configs["LoginUserName"];
                                    sFieldValue = Convert.ToString(sSourceFields[i]) == "" ? CallClass.Configs["LoginUserName"] : Convert.IsDBNull(dr[Convert.ToString(sSourceFields[i])]) == true ? CallClass.Configs["LoginUserName"] : Convert.ToString(dr[Convert.ToString(sSourceFields[i])]);
                                    sFieldValues += " '" + sFieldValue + "',";
                                    continue;
                                }
                                else */if (sTargetFieldName == "检查日期")
                                {
                                    sFieldValue = DateTime.Now.Date.ToShortDateString();
                                    sFieldValues += " '" + sFieldValue + "',";
                                    continue;
                                }
                                else//此处应根据数据库表设计判断值类型进行转换，需要扩充，支持文本型、数字型、二进制型
                                {
                                    sFieldValue = Convert.ToString(sSourceFields[i]) == "" ? "NULL" : Convert.IsDBNull(dr[Convert.ToString(sSourceFields[i])]) == true ? "NULL" : Convert.ToString(dr[Convert.ToString(sSourceFields[i])]);
                                    sFieldValues += (sFieldValue == "NULL" ? "NULL," : "'" + sFieldValue + "',");
                                }
                            }
                            else //如增加的点位中误差、点位粗差率等，一律补空值
                            {
                                sFieldValues += "NULL,";
                            }
                            #endregion
                        }
                        string sInsertSql = string.Format("insert into {0} ({1})values({2})", GetTableName(sTableName), sFieldList, sFieldValues.Remove(sFieldValues.Length - 1));
                        ExecuteSQL(sInsertSql);
                    }
                    // MessageBox.Show(string.Format("成功插入{0}条点检查记录！",sDataTable.Rows.Count));
                }
                catch (Exception exp)
                {
                    //MessageBox.Show(exp.Message);
                    //向上抛出错误，反馈给用户
                    throw new Exception(string.Format("新建表{0}出错:{1}，请检查数据是否存在异常值！", sTableName, exp.Message));
                }
            }
            //如果已经存在目标表，插入记录时先按识别字段删除记录再重新插入
            else
            {
                try
                {
                    foreach (DataRow dr in sDataTable.Rows)
                    {
                        //判断是否存在原记录，如果存在先删除
                        //构造查询代码
                        if (sIdentifyFieldIndexTarget.Count > 0)
                        {
                            string checksql = string.Format("select * from {0} where", GetTableName(sTableName));
                            for (int j = 0; j < sIdentifyFieldIndexTarget.Count; j++)
                            {
                                string sFieldNameSource = Convert.ToString(sIdentifyFieldIndexSource[j]);
                                string sFiedlNameTarget = Convert.ToString(sIdentifyFieldIndexTarget[j]);
                                checksql += string.Format(" {0}='{1}' and ", GetFieldString(sFiedlNameTarget), dr[sFieldNameSource]);
                            }
                            checksql = checksql.Remove(checksql.Length - 5);

                            if (GetScalar(checksql) != null)
                            {
                                string delectsql = checksql.Replace("select", "delete");
                                ExecuteSQL(delectsql.Replace("*",""));
                            }
                        }

                        // 插入新记录
                        string sFieldValues = "";
                        for (int i = 0; i < sTargetFields.Count; i++)
                        {
                            #region// 判断可选字段，特殊处理COMMENT字段
                            string sFieldValue = "";
                            string sSourceFieldName = Convert.ToString(sSourceFields[i]);
                            string sTargetFieldName = Convert.ToString(sTargetFields[i]);
                            if (!sTargetAddedFields.Contains(sSourceFieldName))
                            {
                                //处理可选字段包括：检查者、检查日期，如果导入字段为空时，系统自动录入
                                /*if (sTargetFieldName == "检查者")
                                {
                                    //sFieldValue = CallClass.Configs["LoginUserName"];
                                    sFieldValue = Convert.ToString(sSourceFields[i]) == "" ? CallClass.Configs["LoginUserName"] : Convert.IsDBNull(dr[Convert.ToString(sSourceFields[i])]) == true ? CallClass.Configs["LoginUserName"] : Convert.ToString(dr[Convert.ToString(sSourceFields[i])]);
                                    sFieldValues += " '" + sFieldValue + "',";
                                    continue;
                                }
                                else*/ if (sTargetFieldName == "检查日期")
                                {
                                    sFieldValue = DateTime.Now.Date.ToShortDateString();
                                    sFieldValues += " '" + sFieldValue + "',";
                                    continue;
                                }
                                else
                                {
                                    sFieldValue = Convert.ToString(sSourceFields[i]) == "" ? "NULL" : Convert.IsDBNull(dr[Convert.ToString(sSourceFields[i])]) == true ? "NULL" : Convert.ToString(dr[Convert.ToString(sSourceFields[i])]);
                                    sFieldValues += (sFieldValue == "NULL" ? "NULL," : " '" + sFieldValue + "',");
                                }
                            }
                            else //如增加的点位中误差、点位粗差率等，一律补空值
                            {
                                sFieldValues += "NULL,";
                            }
                            #endregion
                        }
                        string sInsertSql = string.Format("insert into {0} ({1})values({2})", GetTableName(sTableName), sFieldList, sFieldValues.Remove(sFieldValues.Length - 1));
                        ExecuteSQL(sInsertSql);
                    }
                    //MessageBox.Show(string.Format("更新{0}条点检查记录！",sDataTable.Rows.Count));
                }
                catch (Exception exp)
                {
                    //MessageBox.Show(exp.Message);
                    throw new Exception(string.Format("更新表{0}出错:{1}，请检查数据是否存在异常值！", sTableName, exp.Message));
                }
            }

            bSuccess = true;
            return bSuccess;
        }
        //nImportFlag = 0用来描述导入方式，0为覆盖，1为追加，覆盖时先删除表
        public bool ImportDataTableRecords(string sTableName, DataTable sDataTable, int nImportFlag = 0)
        {
            List<string> Fields = GetAllFieldsNames(sDataTable);
            List<string> indentFields = new List<string>();
            List<string> addedFields = new List<string>();
            return ImportDataTableRecords(sTableName, Fields, addedFields, indentFields, sDataTable, Fields, indentFields, nImportFlag);
        }

        //根据表设计导入数据记录，涉及到表设计版本数据类型、.NET数据类型与标准SQL数据类型的转换，是比较复杂和精细的工作
        //数据类型涉及到文本、数字、货币、二进制等常用类型，在数据类型转换表中有定义，尤其是二进制类型，导入时需要将二进制转换成二进制编码字符串
        //nImportFlag = 0用来描述导入方式，0为覆盖，1为追加，覆盖时先删除表
        public bool ImportDataTableRecords(string sTableName, DataTable sDataTable, DataTableDesign sDataTableDesign, int nImportFlag = 0)
        {
            if (sDataTableDesign == null)
            {
                return ImportDataTableRecords(sTableName, sDataTable);
            }
            //从表设计中取出设计信息
            List<string> sTargetFields = sDataTableDesign.TargetFieldsCodeList;
            List<string> sTargetAddedFields = sDataTableDesign.TargetAddedFieldCodeList;
            List<string> sIdentifyFieldIndexTarget = sDataTableDesign.IndetifyFieldCodeIndexTarget;
            List<string> sSourceFields = sDataTableDesign.TargetOrginFieldCodeList;
            // 默认情况下，导入数据表与表设计中的唯一识别符一致，否则可以用多参数导入函数
            List<string> sIdentifyFieldIndexSource = sDataTableDesign.IndetifyFieldCodeIndexTarget;

            //开始导入流程
            bool bSuccess = false;
            if (sTargetFields.Count != sSourceFields.Count+sTargetAddedFields.Count)
                throw new Exception(string.Format("请检查数据库{0}及导入数据表字段是否匹配！"));

            //nimportflag
            if (nImportFlag == 0)
                DropDataTable(sTableName);

            string sFieldList = GetFieldListString(sTargetFields);
            //检索数据库中是否存在记录表，否则创建新记录表
            List<string> sTableNames = GetSchameDataTableNames();
            #region //没有现成的表，需要新建一个表，并将记录逐条插入
            if (sTableNames.Contains(sTableName) == false)
            {
                try
                {
                    //此处由DataTableDesign根据表设计自动创建创建表SQL语句
                    string sCreateTableSql = sDataTableDesign.GetCreateTableSQL(GetTableName(sTableName));// GetCreateSQL(sTableName, sDataTable);
                    ExecuteSQL(sCreateTableSql);

                    foreach (DataRow dr in sDataTable.Rows)
                    {
                        // 分析每条记录                        
                        string sFieldValues = "";
                        for (int i = 0; i < sTargetFields.Count; i++)
                        {
                            // 判断扩充字段，特殊处理
                            string sFieldValue = "";
                            #region //处理增加字段部分，在数据库表设计里体现
                            string sSourceFieldName = Convert.ToString(sSourceFields[i]);
                            string sTargetFieldName = Convert.ToString(sTargetFields[i]);
                            FieldDesign fd = sDataTableDesign.TargetFieldsList[i];

                            if (!sTargetAddedFields.Contains(sSourceFieldName))
                            {
                                #region 后台增加字段处理
                                //处理可选字段包括：检查者、检查日期，如果导入字段为空时，系统自动录入
                                /*if (sTargetFieldName == "检查者")
                                {
                                    //sFieldValue = CallClass.Configs["LoginUserName"];
                                    sFieldValue = Convert.ToString(sSourceFields[i]) == "" ? CallClass.Configs["LoginUserName"] : Convert.IsDBNull(dr[Convert.ToString(sSourceFields[i])]) == true ? CallClass.Configs["LoginUserName"] : Convert.ToString(dr[Convert.ToString(sSourceFields[i])]);
                                    sFieldValues += " '" + sFieldValue + "',";
                                    continue;
                                }
                                else*/ if (sTargetFieldName == "检查日期")
                                {
                                    sFieldValue = DateTime.Now.Date.ToShortDateString();
                                    sFieldValues += " '" + sFieldValue + "',";
                                    continue;
                                }
                                #endregion 
                                else//此处应根据数据库表设计判断值类型进行转换，需要扩充，支持文本型、数字型、二进制型
                                {
                                    string sDataTypeClass = sDataTableDesign.GetDataTypeClass(fd.FieldType);
                                    switch (sDataTypeClass)
                                    {
                                        #region 常规字符类型，需要加引号
                                        case "字符":
                                        case "货币":                                           
                                        case "日期时间型":
                                            {
                                                if(fd.FieldIsNull == "是")
                                                {
                                                    if (Convert.IsDBNull(dr[sSourceFields[i]]) == true)
                                                    {
                                                        throw new Exception("不允许为空的字段有空值");
                                                    }
                                                    else
                                                    {
                                                        sFieldValue = Convert.ToString(dr[sSourceFields[i]]);
                                                        sFieldValues += string.Format("'{0}',", sFieldValue);
                                                    }
                                                }else
                                                {
                                                    if (Convert.IsDBNull(dr[sSourceFields[i]]) == true)
                                                    {
                                                        sFieldValue = "NULL";
                                                        sFieldValues += string.Format("{0},", sFieldValue);
                                                    }
                                                    else
                                                    {
                                                        sFieldValue = Convert.ToString(dr[sSourceFields[i]]);
                                                        sFieldValues += string.Format("'{0}',", sFieldValue);
                                                    }
                                                }
                                            } break;
                                        #endregion 
                                        #region 数值类型，无需引号
                                        case "精确数值型":
                                        case "整型":
                                        case "近似数值型":
                                        {
                                            if (fd.FieldIsNull == "是")
                                            {
                                                if (Convert.IsDBNull(dr[sSourceFields[i]]) == true)
                                                {
                                                    throw new Exception("不允许为空的字段有空值");
                                                }
                                                else
                                                {
                                                    sFieldValue = Convert.ToString(dr[sSourceFields[i]]);
                                                    sFieldValues += string.Format("{0},", sFieldValue);
                                                }
                                            }
                                            else
                                            {
                                                if (Convert.IsDBNull(dr[sSourceFields[i]]) == true)
                                                {
                                                    sFieldValue = "NULL";
                                                    sFieldValues += string.Format("{0},", sFieldValue);
                                                }
                                                else
                                                {
                                                    sFieldValue = Convert.ToString(dr[sSourceFields[i]]);
                                                    sFieldValues += string.Format("'{0}',", sFieldValue);
                                                }
                                            }
                                        }break;
                                        #endregion
                                        #region 二进制类型，分为普通和Postgis geometry类型
                                        case "二进制型":
                                        {
                                            if (dr[sSourceFields[i]].GetType() != typeof(Byte[]))
                                                throw new Exception("二进制类型不匹配");

                                            if (fd.FieldIsNull == "是")
                                            {
                                                if (Convert.IsDBNull(dr[sSourceFields[i]]) == true)
                                                {
                                                    throw new Exception("不允许为空的字段有空值");
                                                }
                                                else
                                                {
                                                    if (fd.FieldType.Contains("geometry"))
                                                    {
                                                        Byte[] bytearray = (Byte[])dr[sSourceFields[i]];
                                                        sFieldValue = CommonUtil.ByteArrayToString(bytearray);
                                                        sFieldValues += string.Format("'{0}',", sFieldValue);
                                                    }
                                                    else
                                                    {
                                                        Byte[] bytearray = (Byte[])dr[sSourceFields[i]];
                                                        sFieldValue = CommonUtil.bytesToHexString(bytearray);
                                                        sFieldValues += string.Format("'{0}',", sFieldValue);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Convert.IsDBNull(dr[sSourceFields[i]]) == true)
                                                {
                                                    sFieldValue = "NULL";
                                                    sFieldValues += string.Format("{0},", sFieldValue);
                                                }
                                                else
                                                {
                                                    if (fd.FieldType.Contains("geometry"))
                                                    {
                                                        Byte[] bytearray = (Byte[])dr[sSourceFields[i]];
                                                        sFieldValue = CommonUtil.ByteArrayToString(bytearray);
                                                        sFieldValues += string.Format("'{0}',", sFieldValue);
                                                    }
                                                    else
                                                    {
                                                        Byte[] bytearray = (Byte[])dr[sSourceFields[i]];
                                                        sFieldValue = CommonUtil.bytesToHexString(bytearray);
                                                        sFieldValues += string.Format("'{0}',", sFieldValue);
                                                    }
                                                }
                                            }
                                        }break;
                                        #endregion 
                                        default:
                                            {
                                                
                                            } break;
                                    }
                                }
                            }
                            else //如增加的点位中误差、点位粗差率等，一律补空值
                            {
                                sFieldValues += "NULL,";
                            }
                            #endregion
                        }
                        string sInsertSql = string.Format("insert into {0} ({1})values({2})", GetTableName(sTableName), sFieldList, sFieldValues.Remove(sFieldValues.Length - 1));
                        ExecuteSQL(sInsertSql);
                    }
                }
                catch (Exception exp)
                {
                    //向上抛出错误，反馈给用户
                    throw new Exception(string.Format("新建表{0}出错:{1}，请检查数据是否存在异常值！", sTableName, exp.Message));
                }
            }
            #endregion
            #region //如果已经存在目标表，插入记录时先按识别字段删除记录再重新插入
            else
            {
                try
                {
                    foreach (DataRow dr in sDataTable.Rows)
                    {
                        //判断是否存在原记录，如果存在先删除
                        //构造查询代码
                        if (sIdentifyFieldIndexTarget.Count > 0)
                        {
                            string checksql = string.Format("select * from {0} where", GetTableName(sTableName));
                            for (int j = 0; j < sIdentifyFieldIndexTarget.Count; j++)
                            {
                                string sFieldNameSource = Convert.ToString(sIdentifyFieldIndexSource[j]);
                                string sFiedlNameTarget = Convert.ToString(sIdentifyFieldIndexTarget[j]);
                                //此处根据字段类型判断是否加引号
                                string sDataTypeClass = sDataTableDesign.GetDataTypeClass(sDataTableDesign.IndetifyFieldIndexTarget[j].FieldType);
                                string sSaperatFlag = sDataTableDesign.GetDataTypeReturnValueSaperatFlag(sDataTypeClass);
                                checksql += string.Format(" {0}={1}{2}{3} and ", GetFieldString(sFiedlNameTarget), sSaperatFlag, dr[sFieldNameSource], sSaperatFlag);
                            }
                            checksql = checksql.Remove(checksql.Length - 5);

                            if (GetScalar(checksql) != null)
                            {
                                string delectsql = checksql.Replace("select", "delete");
                                delectsql = delectsql.Replace("*", "");
                                ExecuteSQL(delectsql);
                            }
                        }
                    }

                    foreach (DataRow dr in sDataTable.Rows)
                    {
                        // 分析每条记录                        
                        string sFieldValues = "";
                        for (int i = 0; i < sTargetFields.Count; i++)
                        {
                            // 判断扩充字段，特殊处理
                            string sFieldValue = "";
                            #region //处理增加字段部分，在数据库表设计里体现
                            string sTargetFieldName = Convert.ToString(sTargetFields[i]);
                            FieldDesign fd = sDataTableDesign.TargetFieldsList[i];

                            if (!sTargetAddedFields.Contains(sTargetFieldName))
                            {
                               string sSourceFieldName = Convert.ToString(sSourceFields[i]);
                               #region 后台增加字段处理
                                //处理可选字段包括：检查者、检查日期，如果导入字段为空时，系统自动录入
                                /*if (sTargetFieldName == "检查者")
                                {
                                    //sFieldValue = CallClass.Configs["LoginUserName"];
                                    sFieldValue = Convert.ToString(sSourceFields[i]) == "" ? CallClass.Configs["LoginUserName"] : Convert.IsDBNull(dr[Convert.ToString(sSourceFields[i])]) == true ? CallClass.Configs["LoginUserName"] : Convert.ToString(dr[Convert.ToString(sSourceFields[i])]);
                                    sFieldValues += " '" + sFieldValue + "',";
                                    continue;
                                }
                                else*/ if (sTargetFieldName == "检查日期")
                                {
                                    sFieldValue = DateTime.Now.Date.ToShortDateString();
                                    sFieldValues += " '" + sFieldValue + "',";
                                    continue;
                                }
                                #endregion
                                else//此处应根据数据库表设计判断值类型进行转换，需要扩充，支持文本型、数字型、二进制型
                                {
                                    string sDataTypeClass = sDataTableDesign.GetDataTypeClass(fd.FieldType);
                                    switch (sDataTypeClass)
                                    {
                                        #region 常规字符类型，需要加引号
                                        case "字符":
                                        case "货币":
                                        case "日期时间型":
                                            {
                                                if (fd.FieldIsNull == "是")
                                                {
                                                    if (Convert.IsDBNull(dr[sSourceFields[i]]) == true)
                                                    {
                                                        throw new Exception("不允许为空的字段有空值");
                                                    }
                                                    else
                                                    {
                                                        sFieldValue = Convert.ToString(dr[sSourceFields[i]]);
                                                        sFieldValues += string.Format("'{0}',", sFieldValue);
                                                    }
                                                }
                                                else
                                                {
                                                    if (Convert.IsDBNull(dr[sSourceFields[i]]) == true)
                                                    {
                                                        sFieldValue = "NULL";
                                                        sFieldValues += string.Format("{0},", sFieldValue);
                                                    }
                                                    else
                                                    {
                                                        sFieldValue = Convert.ToString(dr[sSourceFields[i]]);
                                                        sFieldValues += string.Format("'{0}',", sFieldValue);
                                                    }
                                                }
                                            } break;
                                        #endregion
                                        #region 数值类型，无需引号
                                        case "精确数值型":
                                        case "整型":
                                        case "近似数值型":
                                            {
                                                if (fd.FieldIsNull == "是")
                                                {
                                                    if (Convert.IsDBNull(dr[sSourceFields[i]]) == true)
                                                    {
                                                        throw new Exception("不允许为空的字段有空值");
                                                    }
                                                    else
                                                    {
                                                        sFieldValue = Convert.ToString(dr[sSourceFields[i]]);
                                                        sFieldValues += string.Format("{0},", sFieldValue);
                                                    }
                                                }
                                                else
                                                {
                                                    if (Convert.IsDBNull(dr[sSourceFields[i]]) == true)
                                                    {
                                                        sFieldValue = "NULL";
                                                        sFieldValues += string.Format("{0},", sFieldValue);
                                                    }
                                                    else
                                                    {
                                                        sFieldValue = Convert.ToString(dr[sSourceFields[i]]);
                                                        sFieldValues += string.Format("'{0}',", sFieldValue);
                                                    }
                                                }
                                            } break;
                                        #endregion
                                        #region 二进制类型，分为普通和Postgis geometry类型
                                        case "二进制型":
                                            {
                                                if (dr[sSourceFields[i]].GetType() != typeof(Byte[]))
                                                    throw new Exception("二进制类型不匹配");

                                                if (fd.FieldIsNull == "是")
                                                {
                                                    if (Convert.IsDBNull(dr[sSourceFields[i]]) == true)
                                                    {
                                                        throw new Exception("不允许为空的字段有空值");
                                                    }
                                                    else
                                                    {
                                                        if (fd.FieldType.Contains("geometry"))
                                                        {
                                                            Byte[] bytearray = (Byte[])dr[sSourceFields[i]];
                                                            sFieldValue = CommonUtil.ByteArrayToString(bytearray);
                                                            sFieldValues += string.Format("'{0}',", sFieldValue);
                                                        }
                                                        else
                                                        {
                                                            Byte[] bytearray = (Byte[])dr[sSourceFields[i]];
                                                            sFieldValue = CommonUtil.bytesToHexString(bytearray);
                                                            sFieldValues += string.Format("'{0}',", sFieldValue);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (Convert.IsDBNull(dr[sSourceFields[i]]) == true)
                                                    {
                                                        sFieldValue = "NULL";
                                                        sFieldValues += string.Format("{0},", sFieldValue);
                                                    }
                                                    else
                                                    {
                                                        if (fd.FieldType.Contains("geometry"))
                                                        {
                                                            Byte[] bytearray = (Byte[])dr[sSourceFields[i]];
                                                            sFieldValue = CommonUtil.ByteArrayToString(bytearray);
                                                            sFieldValues += string.Format("'{0}',", sFieldValue);
                                                        }
                                                        else
                                                        {
                                                            Byte[] bytearray = (Byte[])dr[sSourceFields[i]];
                                                            sFieldValue = CommonUtil.bytesToHexString(bytearray);
                                                            sFieldValues += string.Format("'{0}',", sFieldValue);
                                                        }
                                                    }
                                                }
                                            } break;
                                        #endregion
                                        default:
                                            {

                                            } break;
                                    }
                                }
                            }
                            else //如增加的点位中误差、点位粗差率等，一律补空值
                            {
                                //针对projectid等类型
                                //sTargetFieldName
                                if(dr.Table.Columns.IndexOf(sTargetFieldName)>=0)
                                {
                                    sFieldValue = Convert.ToString(dr[sTargetFieldName]);
                                    sFieldValues += string.Format("'{0}',", sFieldValue);
                                }
                                else
                                {
                                    sFieldValues += "NULL,";
                                }
                            }
                            #endregion
                        }
                        string sInsertSql = string.Format("insert into {0} ({1})values({2})", GetTableName(sTableName), sFieldList, sFieldValues.Remove(sFieldValues.Length - 1));
                        ExecuteSQL(sInsertSql);
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("更新表{0}出错:{1}，请检查数据是否存在异常值！", sTableName, exp.Message));
                }
            }
                #endregion

            bSuccess = true;
            return bSuccess;
        }
    }
}

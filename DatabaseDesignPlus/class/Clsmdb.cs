using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace DatabaseDesignPlus
{
    public class Clsmdb : DatabaseReaderWriter
    {
        
        string _DatatbaseConnectionID = null;
        public Clsmdb(string DatatbaseConnectionID)
        {
            _DatatbaseConnectionID = DatatbaseConnectionID;
        }


        /// <summary>
        /// 读取datable里面为数值类型的字段名称
        /// </summary>
        /// <param name="pDataTable"></param>
        /// <returns></returns>
        public List<string> GetNumFieldsName(DataTable pDataTable)
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
                    if (pType.Equals(typeof(double)) || pType.Equals(typeof(int)))
                    {
                        pFieldName = pDataTable.Columns[i].ColumnName;
                        pFiledsList.Add(pFieldName);
                    }
                    //    pFieldName = pDataTable.Columns[i].ColumnName;
                    //pFiledsList.Add(pFieldName);
                    /////////////////////////////是否判断 ，只取数字型字段的名称（有些坐标字段是string类型）

                }
                return pFiledsList;
            }
            else
            { return null; }
        }

        #region  规则验证 ，string为数字


        public static  bool IsZhengIntNumber(string itemValue)
        {
            string pExpression = "^\\+?[1-9][0-9]*$";
            return IsRegEx(pExpression, itemValue);

        }

        /// <summary>
        /// 验证规则，判断字符串 是不是数字串
        /// </summary>
        /// <param name="itemValue"></param>
        /// <returns></returns>
        public static  bool IsDoubleNumber(string itemValue)
        {
            return IsRegEx("^(-?[0-9]*[.]*[0-9]{0,9})$", itemValue);
            //return IsRegEx("^[-+]?(\\d+(\\.\\d*)?|\\.\\d+)([eE]([-+]?([012]?\\d{1,2}|30[0-7])|-3([01]?[4-9]|[012]?[0-3])))?[dD]?$ ", itemValue);
            // return IsRegEx("^-?([1-9]\\d*\\.\\d*|0\\.\\d*[1-9]\\d*|0?\\.0+|0)$", itemValue);

        }

        /// <summary>
        /// 规则表达验证
        /// </summary>
        /// <param name="regExValue"></param>
        /// <param name="itemValue"></param>
        /// <returns></returns>
        private static bool IsRegEx(string regExValue, string itemValue)
        {

            try
            {
                Regex regex = new System.Text.RegularExpressions.Regex(regExValue);
                if (regex.IsMatch(itemValue)) return true;
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
            }

        }

        #endregion

/// <summary>
/// ////////////////////////////////////////////////////////////////////////////////////
/// </summary>
        public override string DatatbaseConnectionID
        {
            get
            {
                return _DatatbaseConnectionID;
            }

        }
        public override string GetTableName(string sTableName)
        {
            return string.Format("[{0}]", sTableName);
        }
        public override string GetConnectionString()
        {
            if (File.Exists(_DatatbaseConnectionID) == false)
                throw new Exception(string.Format("数据库文件{0}不存在",_DatatbaseConnectionID));
            return string.Format(" Provider = Microsoft.Jet.oledb.4.0; Data Source = {0}", _DatatbaseConnectionID); 
        }
        public override System.Data.Common.DbConnection GetDbConnection()
        {
            OleDbConnection conn = new OleDbConnection(GetConnectionString());
            return conn;
        }
        public override System.Data.Common.DbDataAdapter GetDbDataAdapter(string sSQL, System.Data.Common.DbConnection dbConn)
        {
            OleDbDataAdapter oada = new OleDbDataAdapter(sSQL, GetDbConnection() as OleDbConnection);
            return oada;
        }
        public override System.Data.Common.DbCommand GetDbCommand(string sSQL, System.Data.Common.DbConnection dbConn)
        {
            OleDbCommand command = new OleDbCommand(sSQL, dbConn as OleDbConnection);
            return command;
        }

        public override List<string> GetSchameDataTableNames(string whereclause = "")
        {
            OleDbConnection conn = GetDbConnection() as OleDbConnection;
            if (!(conn.State == ConnectionState.Open))
            {
                conn.Open();
            }
            try
            {
                //获取数据表
                DataTable shemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                int n = shemaTable.Rows.Count;
                List<string> strTable = new List<string>();
                int m = shemaTable.Columns.IndexOf("TABLE_NAME");
                for (int i = 0; i < n; i++)
                {
                    DataRow m_DataRow = shemaTable.Rows[i];
                    strTable.Add(m_DataRow.ItemArray.GetValue(m).ToString());
                }
                return strTable;
            }
            catch (OleDbException ex)
            {
                MessageBox.Show("指定的限制集无效:\n" + ex.Message);
                return null;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
        //在此处实现内存数据库字段类型到MDB数据库的字段类型映射
        public override string GetCreateSQL(string tablename, DataTable pDataTable)
        {
            if (pDataTable == null)
            { return null; }
            string fieldsdef = "";

            int pColmCount = pDataTable.Columns.Count;
            if (pColmCount > 0)
            {
                for (int i = 0; i < pColmCount; i++)
                {
                    Type pType = pDataTable.Columns[i].DataType;
                    string pFieldName;
                    pFieldName = pDataTable.Columns[i].ColumnName;
                    if (pType.Equals(typeof(double)) || pType.Equals(typeof(int)))
                    {
                        fieldsdef += "[" + pFieldName + "] double" + ",";
                    }
                    else if (pType.Equals(typeof(string)))
                    {
                        fieldsdef += "[" + pFieldName + "] text" + ",";
                    }
                }
            }
            else
            { return null; }

            fieldsdef = fieldsdef.Remove(fieldsdef.Length - 1);
            string createsql = string.Format("create table [{0}]({1})", tablename, fieldsdef);

            return createsql;
        }

        public override string GetFieldListString(List<string> sTargetFields)
        {
            string sFieldList = "";
            for (int i = 0; i < sTargetFields.Count; i++)
            {
                sFieldList += "[" + sTargetFields[i] + "],";
            }
            sFieldList = sFieldList.Remove(sFieldList.Length - 1);
            return sFieldList;
        }
        public override string GetFieldString(string sTargetField)
        {
            return "[" + sTargetField + "]";
        }
    }
}

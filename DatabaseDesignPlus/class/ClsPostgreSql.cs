using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Npgsql;
using System.Windows.Forms;
using System.Collections;
using System.Data.Common;

namespace DatabaseDesignPlus
{
    public class ClsPostgreSql : DatabaseReaderWriter
	{
        string _DatatbaseConnectionID = null;
        public ClsPostgreSql(string DatatbaseConnectionID)
        {
            _DatatbaseConnectionID = DatatbaseConnectionID;
        }

 

/// <summary>
/// /////////////////////////////////////////////////////////////////////////////////////////////////
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
            return string.Format("\"{0}\"", sTableName) ;
            //return string.Format("{0}", sTableName);
        }

        public override string GetConnectionString()
        {
            return _DatatbaseConnectionID;
        }

        public override System.Data.Common.DbDataAdapter GetDbDataAdapter(string sSQL, System.Data.Common.DbConnection dbConn)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(sSQL, dbConn as NpgsqlConnection);
            NpgsqlDataAdapter ad = new NpgsqlDataAdapter(cmd);
            return ad;
        }

        public override System.Data.Common.DbConnection GetDbConnection()
        {
            NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString());
            return conn;
        }

        public override System.Data.Common.DbCommand GetDbCommand(string sSQL, System.Data.Common.DbConnection dbConn)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(sSQL, dbConn as NpgsqlConnection);
            return cmd;
        }

        public override List<string> GetSchameDataTableNames(string whereclaus = "")
        {
            DbConnection conn = GetDbConnection();
            try
            {
                conn.Open();
                string sql = "SELECT   tablename   FROM   pg_tables" + whereclaus;
                //DbCommand cmd = GetDbCommand(sql, conn);
                DbDataAdapter ad = GetDbDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                //获取数据表
                List<string> strTable = new List<string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow m_DataRow = dt.Rows[i];
                    strTable.Add(m_DataRow.ItemArray.GetValue(0).ToString());
                }
                return strTable;
            }
            catch (Exception ex)
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
                    /*if (pType.Equals(typeof(double)) || pType.Equals(typeof(int))
                        ||pType.Equals(typeof(Single))||pType.Equals(typeof(Int64)))
                    {
                        fieldsdef += "\"" + pFieldName + "\" double precision" + ",";
                    }
                    else if (pType.Equals(typeof(string)))*/
                    {
                        fieldsdef += "\"" + pFieldName + "\" text" + ",";
                    }
                }
            }
            else
            { return null; }

            fieldsdef = fieldsdef.Remove(fieldsdef.Length - 1);
            string createsql = string.Format("create table \"{0}\"({1})", tablename, fieldsdef);

            return createsql;
        }

        public override string GetFieldListString(List<string> sTargetFields)
        {
            string sFieldList = "";
            for (int i = 0; i < sTargetFields.Count; i++)
            {
                sFieldList += "\"" + sTargetFields[i] + "\",";
            }
            sFieldList = sFieldList.Remove(sFieldList.Length - 1);
            return sFieldList;
        }

        public override string GetFieldString(string sTargetField)
        {
            return "\"" + sTargetField + "\"";
        }
    }
}

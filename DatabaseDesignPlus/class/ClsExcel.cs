using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.Collections;
namespace DatabaseDesignPlus
{
    public class ClsExcel : DatabaseReaderWriter
    {
        string _DatatbaseConnectionID = null;
        public ClsExcel(string DatatbaseConnectionID)
        {
            _DatatbaseConnectionID = DatatbaseConnectionID;
        }

////////////////////////////////////////////////////////////////////////////////////////////
        public override string DatatbaseConnectionID
        {
            get
            {
                return _DatatbaseConnectionID;
            }
        }

        public override string GetTableName(string sTableName)
        {
            return string.Format("[{0}$]",sTableName);
        }

        public override string GetConnectionString()
        {
            if (!File.Exists(_DatatbaseConnectionID))
            {
                throw new Exception("指定的Excel文件不存在！");
            }
            return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _DatatbaseConnectionID + ";Extended properties=\"Excel 8.0;Imex=2;HDR=Yes;\"";   

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
            List<string> alTables = new List<string>();
            OleDbConnection odn = new OleDbConnection(GetConnectionString());
            odn.Open();
            DataTable dt = new DataTable();
            dt = odn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dt == null)
            {
                throw new Exception("无法获取指定Excel的架构。");
            }
            foreach (DataRow dr in dt.Rows)
            {
                string tempName = dr["Table_Name"].ToString();
                int iDolarIndex = tempName.IndexOf('$');
                if (iDolarIndex > 0)
                {
                    tempName = tempName.Substring(0, iDolarIndex);
                }
                //修正了Excel2003中某些工作薄名称为汉字的表无法正确识别的BUG。
                if (tempName[0] == '\'')
                {
                    if (tempName[tempName.Length - 1] == '\'')
                    {
                        tempName = tempName.Substring(1, tempName.Length - 2);
                    }
                    else
                    {
                        tempName = tempName.Substring(1, tempName.Length - 1);
                    }
                }
                if (!alTables.Contains(tempName))
                {
                    alTables.Add(tempName);
                }
            }
            odn.Close();
            if (alTables.Count == 0)
            {
                return null;
            }
            return alTables;
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
            string createsql = string.Format("create table [{0}$]({1})", tablename, fieldsdef);

            return createsql;
        }

        public override string GetFieldListString(List<string> sTargetFields)
        {
            string sFieldList = "";
            for (int i = 0; i < sTargetFields.Count; i++)
            {
                sFieldList +=  sTargetFields[i] + ",";
            }
            sFieldList = sFieldList.Remove(sFieldList.Length - 1);
            return sFieldList;
        }
        public override string GetFieldString(string sTargetField)
        {
            return sTargetField ;
        }


    }
}

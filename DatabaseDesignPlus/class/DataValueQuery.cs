using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace DatabaseDesignPlus
{
    public class DataValueQuery : IQuery
    {
        public DataValueQuery() { }


        public string _QueryFields = "";
        public string QueryFields
        {
            set
            {
                _QueryFields = value;
                _QueryFieldArray.Clear();

                string[] fieldsStringArray = _QueryFields.Split(',');
                foreach (string field in fieldsStringArray)
                {
                    string localfield = field;
                    int index  = field.ToLower().IndexOf(" as ");
                    if (index > 0)
                    {
                        localfield = localfield.Substring(index + 4, localfield.Length - index -4);
                    }

                    _QueryFieldArray.Add(localfield.Trim());
                }
            }
            get
            {
                return _QueryFields;
            }
        }

        public List<string> _QueryFieldArray = new List<string>();
        public List<string> QueryFieldsArray
        {
            get
            {
                return _QueryFieldArray;
            }
        }
        public string _QueryCondition = "";
        public string QueryCondition
        {
            set
            {
                _QueryCondition = value;
            }
            get
            {
                return _QueryCondition;
            }
        }

        public string QueryFormat { set; get; }
        public string QueryValueType { set; get; }

        public object QueryValue
        {
            get
            {
                object o = null;
                if (QueryValueType == "String" && QueryFormat != "")
                {
                    o = GetQueryFormatedValue();
                }
                if(QueryValueType=="StringListAsString")
                {
                    o = GetQueryDataTable();
                    string s="";
                    DataTable d = o as DataTable;
                    foreach(DataRow dr in d.Rows)
                    {
                        s += dr[0] as string+";";
                    }
                    o = s;
                }
                else if (QueryValueType == "DataTable")
                {
                    o = GetQueryDataTable();
                }
                else if (QueryValueType == "Date" )
                {
                    o = Convert.ToDateTime( GetQueryScalar());
                }
                else if (QueryValueType == "Double")
                {
                    o = Convert.ToDouble(GetQueryScalar());
                }
                else if(QueryValueType == "Integer")
                {
                    o = Convert.ToInt32(GetQueryScalar());
                }
                else if (QueryFormat == "")
                {
                    o = GetQueryScalar();
                }
                return o;

            }
        }

        public string QuerySql
        {
            get
            {
                return string.Format("select {0} {1} ", QueryFields, QueryCondition);
            }
        }

        public string _DbFilePath = "";
        public string DbFilePath
        {
            set { _DbFilePath = value; }
            get { return _DbFilePath; }
        }

        IDatabaseReaderWriter GetDbReader()
        {
            IDatabaseReaderWriter dbreader = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", _DbFilePath);
            return dbreader;
        }

        //public Clsmdb _Mdb = new Clsmdb();
        public string GetQueryScalar()
        {
            string sScalar = "";
            try
            {
                sScalar = Convert.ToString(GetDbReader().GetScalar(QuerySql));

            }
            catch (Exception) { }

            return sScalar;
        }

        public DataTable GetQueryDataTable()
        {

            DataTable dt = null;
            try
            {
                dt = GetDbReader().GetDataTableBySQL(QuerySql);
            }
            catch (Exception)
            {
            }
            return dt;

        }

        public DataRow GetQueryDataRow()
        {
            DataRow _QueryDataRow = null;
            try
            {
                DataTable dt = GetDbReader().GetDataTableBySQL(QuerySql);
                _QueryDataRow = dt.Rows[0];
            }
            catch (Exception)
            { }
            return _QueryDataRow;
        }


        public string GetQueryFormatedValue()
        {

            string sQueryFormatedValue = QueryFormat;
            try
            {
                DataRow _QueryDataRow = GetQueryDataRow();
                List<string> queryfieldararyy = QueryFieldsArray;
                for (int i = 0; i < queryfieldararyy.Count; i++)
                {
                    string valuestring = Convert.ToString(_QueryDataRow.ItemArray[i]);
                    sQueryFormatedValue = sQueryFormatedValue.Replace(queryfieldararyy[i] as string, valuestring);
                }
            }
            catch (Exception ex)
            { }
            return sQueryFormatedValue;
        }

    }
}

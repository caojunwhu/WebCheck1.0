using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace DatabaseDesignPlus
{
    public class MergeDataTableQuery: IQuery
    {
        public DataValueQuery Query { set; get; }
        public DataValueQuery QueryMerged { set; get; }
        public string QueryFormat { set; get; }
        public string QueryValueType { set; get; }
        //public string QuerySql { get; }
        public string QueryFields { set; get; }

        public List<string> _QueryFieldArray = new List<string>();
        public List<string> QueryFieldsArray
        {
            get
            {
                throw new NotImplementedException();
                //return _QueryFieldArray;
            }
        }
        private string _DbFilePath = "";
        public string DbFilePath
        {
            set {
                _DbFilePath = value;
                Query.DbFilePath = _DbFilePath;
                QueryMerged.DbFilePath = _DbFilePath;
            }
            get { return _DbFilePath; }
        }

        private string _QueryCondition = "";
        public string QueryCondition
        {
            set
            {
                _QueryCondition = value;
                string[] queryconditons = _QueryCondition.Split(';');
                if (queryconditons.Length  == 2)
                {
                    Query.QueryCondition = queryconditons[0];
                    QueryMerged.QueryCondition = queryconditons[1];
                }
            }
            get
            {
                return string.Format("{0};{1}",Query.QueryCondition,QueryMerged.QueryCondition);
            }
        }

        public DataTable GetQueryDataTable()
        {

            DataTable dt = null;

            try
            {
                DataTable LatterPointTable = Query.GetQueryDataTable() == null ? new DataTable() : Query.GetQueryDataTable();
                DataTable NumericPointTable = QueryMerged.GetQueryDataTable() == null ? new DataTable() : QueryMerged.GetQueryDataTable();
                if (LatterPointTable.Rows.Count == 0 && NumericPointTable.Rows.Count == 0)
                    throw new Exception("合并查询中表无数据！");

                IEnumerable<DataRow> query = NumericPointTable.AsEnumerable().Union(LatterPointTable.AsEnumerable(), DataRowComparer.Default);
                //两个数据源的并集集合
                dt = query.CopyToDataTable();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dt;
        }



        public object QueryValue
        {
            get
            {
                object o = null;
                if (QueryValueType == "DataTable")
                {
                    o = GetQueryDataTable();
                }
                return o;
            }
        }
    }
}

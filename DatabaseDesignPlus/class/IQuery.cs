using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Collections;

namespace DatabaseDesignPlus
{
    public interface IQuery
    {

        string QueryFields { set; get; }
        List<string> QueryFieldsArray { get; }
        string QueryCondition { set; get; }
        string QueryFormat { set; get; }
        string QueryValueType { set; get; }
        object QueryValue { get; }
        //string QuerySql { get; }
        string DbFilePath { set; get; }
        //string GetQueryScalar();
        DataTable GetQueryDataTable();
        //DataRow GetQueryDataRow();
        //string GetQueryFormatedValue();

    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace DatabaseDesignPlus
{
	public  class QueryFactory
	{
        public  string QueryType = "";
        public  string QueryJsonBody = "";

        public  IQuery Creator()
        {
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                StringReader sr = new StringReader(QueryJsonBody);
                IQuery iquery = null;
                switch (QueryType.ToLower())
                {
                    case "datavaluequery":
                        {
                            iquery = (DataValueQuery)serializer.Deserialize(new JsonTextReader(sr), typeof(DataValueQuery));
                        }
                        break;
                    case "mergedatatablequery":
                        {
                            iquery = (MergeDataTableQuery)serializer.Deserialize(new JsonTextReader(sr), typeof(MergeDataTableQuery));
                        }
                        break;
                }
                return iquery;
            }
            catch
            {
            }
            return null;
        }

	}
}

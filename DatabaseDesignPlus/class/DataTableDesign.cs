using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Data;
using System.Collections;

namespace DatabaseDesignPlus
{
    public class DataTableDesign
    {

        public DataTableDesign()
        {
        }
        public DataTableDesign(string sDatatbaseDesignConnectionID,
            string sDatabaseDesignType, 
            string sDataTableDesignTableName, 
            string sDataTableDesignVersion, 
            string sDataTableDesignFieldAttributesNameJson,
            string sDataTypeRelationDatatbaseConnectionID,
            string sDataTypeRelationDatabaseType,
            string sDatatableDataTypeRelationDataTableName
            )
        {
            
            InitalizeParameters(sDatatbaseDesignConnectionID,
            sDatabaseDesignType, 
            sDataTableDesignTableName, 
            sDataTableDesignVersion, 
            sDataTableDesignFieldAttributesNameJson,
            sDataTypeRelationDatatbaseConnectionID,
            sDataTypeRelationDatabaseType,
            sDatatableDataTypeRelationDataTableName );
        }

        public void InitalizeParameters(string sDatatbaseDesignConnectionID,
            string sDatabaseDesignType,
            string sDataTableDesignTableName,
            string sDataTableDesignVersion,
            string sDataTableDesignFieldAttributesNameJson,
            string sDataTypeRelationDatatbaseConnectionID,
            string sDataTypeRelationDatabaseType,
            string sDatatableDataTypeRelationDataTableName            
            )
        {
            _DatatableDesignDbReader = GetDbReader(sDatatbaseDesignConnectionID, sDatabaseDesignType);
            DataTableDesignTableName = sDataTableDesignTableName;
            DataTableDesignVersion = sDataTableDesignVersion;
            DataTableDesignFieldAttributesNameJson = sDataTableDesignFieldAttributesNameJson;

            _DatatableDataTypeRelationDbReader = GetDbReader(sDataTypeRelationDatatbaseConnectionID, sDataTypeRelationDatabaseType);
            DatatableDataTypeRelationDataTableName = sDatatableDataTypeRelationDataTableName;
        }

        //sDataTableDesignSorceFile 表设计存储文件名
        //sDataTableDesignTableName 表设计表名
        //sDataTableDesignVersion 表设计版本号
        //sDataTableDesignFieldAttributesNameJson 表设计属性中英文对照json对象
        //sCreateTableName 表设计对应创建数据库的表名
        public void InitializeDataTableDesign(
            string sCreateTableType,
            string sCreateTableName,
            string sWhere = "")
        {

            _CreateTableName = sCreateTableName;
            _sCreateTableType = sCreateTableType;

            JsonSerializer serializer = new JsonSerializer(); 
            StringReader sr = new StringReader(DataTableDesignFieldAttributesNameJson);
            TheFieldAttributesName = (FieldAttributesName)serializer.Deserialize(new JsonTextReader(sr), typeof(FieldAttributesName));

            DataTable sourceDataTableDesign = null;
            try
            {
                string sql_select = string.Format("select * from {0} order by cast(序号 as integer) ", DataTableDesignTableName);
                //sourceDataTableDesign = DatatableDesignDbReader.GetDataTable(DataTableDesignTableName);
                sourceDataTableDesign = DatatableDesignDbReader.GetDataTableBySQL(sql_select);
                List<DataRow> rows = new List<DataRow>();
                if(sWhere!="")
                {
                   DataRow[] rs = sourceDataTableDesign.Select(sWhere);
                    foreach (DataRow r in rs)
                    {
                        rows.Add(r);
                    }
                }
                else
                {
                    foreach(DataRow r in sourceDataTableDesign.Rows)
                    {
                        rows.Add(r);
                    }
                }
                foreach (DataRow dr in rows)
                {
                    FieldDesign fiedldesign = new FieldDesign();
                    fiedldesign.FieldAttributesName = TheFieldAttributesName;
                    fiedldesign.FieldIndex = Convert.ToInt32(dr[TheFieldAttributesName.FieldIndex]);
                    fiedldesign.FieldCode = Convert.ToString(dr[TheFieldAttributesName.FieldCode]).ToLower();//目前仅支持小写格式，因Postgresql等数据库版本对大小写定义不一致
                    fiedldesign.FieldClass = TheFieldAttributesName.FieldClass == null ? "" : Convert.ToString(dr[TheFieldAttributesName.FieldClass]);
                    fiedldesign.FieldSource = TheFieldAttributesName.FieldSource == null ? "" : Convert.ToString(dr[TheFieldAttributesName.FieldSource]);
                    fiedldesign.FieldInput = TheFieldAttributesName.FieldInput == null ? "" : Convert.ToString(dr[TheFieldAttributesName.FieldInput]);

                    fiedldesign.FieldName = Convert.ToString(dr[TheFieldAttributesName.FieldName]);
                    fiedldesign.FieldType = Convert.ToString(dr[TheFieldAttributesName.FieldType]);
                    fiedldesign.FieldLength =Convert.IsDBNull(dr[TheFieldAttributesName.FieldLength])?-1: Convert.ToInt32(dr[TheFieldAttributesName.FieldLength]);
                    fiedldesign.FieldPrecision = Convert.ToString(dr[TheFieldAttributesName.FieldPrecision]) == "" ? -1 : Convert.ToInt32(dr[TheFieldAttributesName.FieldPrecision]);
                    fiedldesign.FieldValue = Convert.ToString(dr[TheFieldAttributesName.FieldValue]) == "" ? "-1" : Convert.ToString(dr[TheFieldAttributesName.FieldValue]);
                    fiedldesign.FieldIsNull = Convert.ToString(dr[TheFieldAttributesName.FieldIsNull]);
                    fiedldesign.FieldRemarks = Convert.ToString(dr[TheFieldAttributesName.FieldRemarks]) == "" ? "" : Convert.ToString(dr[TheFieldAttributesName.FieldRemarks]);
                    fiedldesign.FieldImportType = Convert.ToString(dr[TheFieldAttributesName.FieldImportType]);
                    fiedldesign.FieldImportIDCode = Convert.ToString(dr[TheFieldAttributesName.FieldImportIDCode]);
                    _FieldDesigns.Add(fiedldesign);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        IDatabaseReaderWriter GetDbReader(string sDatatbaseConnectionID, string sDatabaseType)
        {   
            //"PostgreSQL"
            return DatabaseReaderWriterFactory.GetDatabaseReaderWriter(sDatabaseType, sDatatbaseConnectionID);
        }

        IDatabaseReaderWriter _DatatableDesignDbReader = null;
        public IDatabaseReaderWriter DatatableDesignDbReader
        {
            get { return _DatatableDesignDbReader; }
        }

        IDatabaseReaderWriter _DatatableDataTypeRelationDbReader = null;
        public IDatabaseReaderWriter DatatableDataTypeRelationDbReader
        {
            get { return _DatatableDataTypeRelationDbReader; }
        }

        public string DataTableDesignVersion  { set; get; }
        public string DataTableDesignFieldAttributesNameJson { set; get; }
        public FieldAttributesName TheFieldAttributesName { set; get; }

        public List<FieldDesign> _FieldDesigns = new List<FieldDesign>();
        public List<FieldDesign> FieldDesigns { get { return _FieldDesigns; } }

        public string DataTableDesignTableName { set; get; }
        public string DatatableDataTypeRelationDataTableName { set; get; }

        public string _CreateTableName = "";
        public string CreateTableName
        {
            set
            {
                _CreateTableName = value;
            }
            get
            {
                return _CreateTableName;
            }
        }

        string _sCreateTableType = "";
        public string CreateTableType
        {
            get { return _sCreateTableType; }
        }

        //根据字段定义中FieldType到数据类型关系表中查询其中文类别
        public string GetDataTypeClass(string sFieldType)
        {
            string sDataTypeClass = "";
            string selectDataTypeClass = string.Format("select {0} from {1} where lower({2})='{3}' and {4} = '{5}'",
                        DatatableDataTypeRelationDbReader.GetFieldString("数据类型类别"),
                        DatatableDataTypeRelationDbReader.GetTableName(DatatableDataTypeRelationDataTableName),
                        DatatableDataTypeRelationDbReader.GetFieldString("数据类型"),
                        sFieldType.ToLower(),
                        DatatableDataTypeRelationDbReader.GetFieldString("数据库引擎"),
                        CreateTableType);

            sDataTypeClass = DatatableDataTypeRelationDbReader.GetScalar(selectDataTypeClass) as string;
            return sDataTypeClass;
        }

        //根据字段中文类别选择返回值分隔符，如是否为引号或空
        public string GetDataTypeReturnValueSaperatFlag(string sDataTypeClass)
        {
            string sSaperatFlag = "";
             switch (sDataTypeClass)
            {
                case "货币":
                case "二进制型":
                case "日期时间型": 
                case "字符":
                    {
                        sSaperatFlag = "'";
                    } break;
                case "精确数值型":
                case "整型":
                case "近似数值型":
                default:
                    {
                        sSaperatFlag = "";
                    }break;
            }
             return sSaperatFlag;
        }

        public string _CreateTableSQL;
        public string GetCreateTableSQL(string sCreateTableName)
        {
            _CreateTableSQL = string.Format("create table {0}(", _CreateTableName);
            switch (_sCreateTableType)
            {
                #region "PostgreSQL":
                case "PostgreSQL":
                case "Mdb": // Mdb 和 Excel均使用Microsoft Jet数据类型，可以一并处理
                case "Excel":
                {
                    foreach (FieldDesign fd in FieldDesigns)
                    {
                        string sDataTypeClass = GetDataTypeClass(fd.FieldType);

                        #region
                        //string length = fd.FieldLength == -1 ? "" : Convert.ToString(fd.FieldLength);
                        //string precision = fd.FieldPrecision == -1 ? "" : Convert.ToString(fd.FieldPrecision);
                        /* if (length != "" && fd.FieldType == "varchar")
                         {
                             _CreateTableSQL += string.Format("{0} {1}({2}) {3},", fd.FieldCode, fd.FieldType, length, isnull);
                         }
                         else if (length != "" && precision != "" && fd.FieldType == "numeric")
                         {
                             _CreateTableSQL += string.Format("{0} {1}({2},{3}) {4},", fd.FieldCode, fd.FieldType, length, precision, isnull);
                         }
                         else
                         {
                             _CreateTableSQL += string.Format("{0} {1} {2},", fd.FieldCode, fd.FieldType, isnull);
                         } */
                        #endregion

                        string isnull = fd.FieldIsNull == "是" ? " NOT NULL" : " NULL";
                        switch (sDataTypeClass)
                        {
                            case "字符":
                                {
                                    if(fd.FieldType.ToLower()=="text")
                                        _CreateTableSQL += string.Format("{0} {1} {2},", fd.FieldCode, fd.FieldType,isnull);
                                    else
                                         _CreateTableSQL += string.Format("{0} {1}({2}) {3},", fd.FieldCode, fd.FieldType, fd.FieldLength, isnull);
                                } break;
                            case "精确数值型":
                                {
                                   _CreateTableSQL += string.Format("{0} {1}({2},{3}) {4},", fd.FieldCode, fd.FieldType, fd.FieldLength, fd.FieldPrecision, isnull);
                                } break;
                            case "整型":
                            case "货币":
                            case "近似数值型":
                            case "二进制型":
                            case "日期时间型":
                            default:
                                {
                                    _CreateTableSQL += string.Format("{0} {1} {2},", fd.FieldCode, fd.FieldType, isnull);
                                }break;
                        }
                    }
                    _CreateTableSQL = _CreateTableSQL.Remove(_CreateTableSQL.Length - 1) + ")";
                            
                } break;
                #endregion
            }
            return _CreateTableSQL;
   
        }

        public static FieldDesign FindFieldDesign(List<FieldDesign>FieldDesignList, string FieldName)
        {
            FieldDesign fielddesign = null;
            foreach (FieldDesign fd in FieldDesignList)
            {
                if (fd.FieldName == FieldName)
                {
                    fielddesign = fd;
                    break;
                }
            }
            return fielddesign;
        }

         List<FieldDesign> _TargetFieldsList = new List<FieldDesign>();
        public List<FieldDesign> TargetFieldsList
        {
            get
            {
                _TargetFieldsList.Clear();

                foreach (FieldDesign fd in FieldDesigns)
                {
                    _TargetFieldsList.Add(fd);

                }
                return _TargetFieldsList;
            }
        }

         List<FieldDesign> _TargetAddedFieldList = new List<FieldDesign>();
        public List<FieldDesign> TargetAddedFieldList
        {
            get
            {
                _TargetAddedFieldList.Clear();
                foreach (FieldDesign fd in FieldDesigns)
                {
                    if(fd.FieldImportType=="扩充")
                    _TargetAddedFieldList.Add(fd);

                }
                return _TargetAddedFieldList;
            }

        }
         List<FieldDesign> _TargetOrginFieldList = new List<FieldDesign>();
        public List<FieldDesign> TargetOrginFieldList
        {
            get
            {
                _TargetOrginFieldList.Clear();

                foreach (FieldDesign fd in FieldDesigns)
                {
                    if (fd.FieldImportType != "扩充")
                        _TargetOrginFieldList.Add(fd);

                }
                return _TargetOrginFieldList;
            }
        }

         List<FieldDesign> _IndetifyFieldIndexTarget = new List<FieldDesign>();

        public List<FieldDesign> IndetifyFieldIndexTarget
        {
            get
            {
                _IndetifyFieldIndexTarget.Clear();
                foreach (FieldDesign fd in FieldDesigns)
                {
                    if (fd.FieldImportIDCode == "是")
                        _IndetifyFieldIndexTarget.Add(fd);

                }
                return _IndetifyFieldIndexTarget;

            }
        }

        /// <summary>
        /// /code
        /// </summary>
        List<string> _TargetFieldsCodeList = new List<string>();
        public List<string> TargetFieldsCodeList
        {
            get
            {
                _TargetFieldsCodeList.Clear();

                foreach (FieldDesign fd in FieldDesigns)
                {
                    _TargetFieldsCodeList.Add(fd.FieldCode);

                }
                return _TargetFieldsCodeList;
            }
        }

         List<string> _TargetAddedFieldCodeList = new List<string>();
        public List<string> TargetAddedFieldCodeList
        {
            get
            {
                _TargetAddedFieldCodeList.Clear();
                foreach (FieldDesign fd in FieldDesigns)
                {
                    if (fd.FieldImportType == "扩充")
                        _TargetAddedFieldCodeList.Add(fd.FieldCode);

                }
                return _TargetAddedFieldCodeList;
            }

        }
         List<string> _TargetOrginFieldCodeList = new List<string>();
        public List<string> TargetOrginFieldCodeList
        {
            get
            {
                _TargetOrginFieldCodeList.Clear();

                foreach (FieldDesign fd in FieldDesigns)
                {
                    if (fd.FieldImportType != "扩充")
                        _TargetOrginFieldCodeList.Add(fd.FieldCode);

                }
                return _TargetOrginFieldCodeList;
            }
        }

         List<string> _IndetifyFieldCodeIndexTarget = new List<string>();

        public List<string> IndetifyFieldCodeIndexTarget
        {
            get
            {
                _IndetifyFieldCodeIndexTarget.Clear();
                foreach (FieldDesign fd in FieldDesigns)
                {
                    if (fd.FieldImportIDCode == "是")
                        _IndetifyFieldCodeIndexTarget.Add(fd.FieldCode);

                }
                return _IndetifyFieldCodeIndexTarget;

            }
        }

    }
}

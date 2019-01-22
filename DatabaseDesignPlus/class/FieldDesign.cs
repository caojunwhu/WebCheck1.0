using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace DatabaseDesignPlus
{
    public class FieldDesign
    {
        public FieldDesign(){ }
        //public DataTable 
        //2015-2-12对应于FieldAttributesName，增加了属性类别、属性来源、输入框三项
        public FieldAttributesName FieldAttributesName { set; get; }
        public int FieldIndex { set; get; }
        public string FieldCode { set; get; }
        public string FieldName { set; get; }
        public string FieldClass { set; get; }
        public string FieldSource { set; get; }
        public string FieldInput { set; get; }
        public string FieldType { set; get; } 
        public int FieldLength { set; get; }
        public int FieldPrecision { set; get; }
        public string FieldValue { set; get; }
        public string FieldIsNull { set; get; }
        public string FieldRemarks { set; get; }
        public string FieldImportType { set; get; }
        public string FieldImportIDCode { set; get; }//导入识别码

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseDesignPlus
{
    public class FieldAttributes : FieldAttributesName { }

    public class FieldAttributesName
    {
        public FieldAttributesName() { }
        //json对象：{'FieldIndex':'序号','FieldName':'名称','FieldCode':'代码','FieldType':'类型','FieldLength':'长度','FieldPrecision':'小数位数','FieldValue':'值域','FieldIsNull':'是否必填','FieldRemarks':'说明','FieldImportType':'导入类型','FieldImportIDCode':'导入识别码'}
        //2015-2-12对对象属性进行扩充,增加了属性类别、属性来源、输入框三项，分别用于对属性分类，值域范围查询、设置输入框等，后面根据实际需求还可以增加项目
        //{'FieldIndex':'序号','FieldName':'名称','FieldCode':'代码','FieldClass':'属性类别','FieldSource':'属性来源','FieldInput':'输入框','FieldType':'类型','FieldLength':'长度','FieldPrecision':'小数位数','FieldValue':'值域','FieldIsNull':'是否必填','FieldRemarks':'说明','FieldImportType':'导入类型','FieldImportIDCode':'导入识别码'}
        public string FieldIndex { set; get; }
        public string FieldName { set; get; }
        public string FieldCode { set; get; }
        public string FieldClass {set;get;}
        public string FieldSource { set; get; }
        public string FieldInput { set; get; }
        public string FieldType { set; get; }
        public string FieldLength { set; get; }
        public string FieldPrecision { set; get; }
        public string FieldValue { set; get; }
        public string FieldIsNull { set; get; }
        public string FieldRemarks { set; get; }
        public string FieldImportType { set; get; }
        public string FieldImportIDCode { set; get; }//导入识别码
    }
}

using DatabaseDesignPlus;

namespace ReportPrinter
{
    public class PrintSetting
    {

        public string PrintValueType { set; get; }
        public int TableItemCount { set; get; }
        public int TableColunm { set; get; }
        public int FirstColumnCount { set; get; }
        public int SecondColumnCount { set; get; }

    }

    public class PrintItem
    {
        public int Index { set; get; }
        public string Bookmark{set;get;}
        public string PrintMode { set; get; }
        public IQuery PrintQuery{set;get;}
        public PrintSetting PrintSetting{set;get;}
    }

    public class PrintEnvironment
    {
        public string PrintDesignDatabaseFilePath { set; get; } 
        public string PrintDesignDataTableName{ set; get; }
        public string PrintSettingDataTableName{ set; get; }
        public string PrintDaTableDesignVersion{set;get;}
        public string DataTableDesignFieldAttributesName{set;get;}   
        public string SourceDatabaseFileName{ set; get; }
        public string SourceDatatableName { set; get; }
        public string CheckReportTemplateFilePath { set; get; }


    }

}

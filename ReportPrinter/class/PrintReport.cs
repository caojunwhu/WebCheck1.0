using System;
using System.Collections.Generic;
using System.Data;
using WordAddinSample;
using Eipsoft.Common;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using DatabaseDesignPlus;
//using Core;

namespace ReportPrinter
{
    public class PrintReport
    {
        public List<PrintItem> _PrintItemList = null;
        public  WordHelp _Word = null;
        public  DataTableDesign _PrintSettingDataTableDesign = null;
        public PrintEnvironment PrintEnvironment { set; get; }
        public Dictionary<string, string> Configs { get; set; }
        public Dictionary<string, string> Databases { get; set; }
        public string AppPath { get; set; }

        public static bool IsInteger(string s)
        {
            string pattern = @"^\d*$";
            return Regex.IsMatch(s, pattern);

        }
        public bool IsNumeric(string s)
        {
            /* Regex rg = new Regex("^([0-9]{1,})$");
             if (rg.IsMatch(s.Trim()))
             { return true; }*/

            Regex rg = new Regex("^([0-9]{1,}[.][0-9]*)$");
            if (rg.IsMatch(s.Trim()))
            {
                return true;
            }
            return false;

        }
        public void PrintInit(PrintEnvironment pPrintEnvironment)
        {
            PrintEnvironment = pPrintEnvironment;
            _PrintItemList = new List<PrintItem>();

            // 2.读取打印设置表本身
            string sPrintDesignDataTableName =  Databases[PrintEnvironment.PrintDesignDatabaseFilePath];
            string sPrintSettingDataTableName =  Configs[PrintEnvironment.PrintSettingDataTableName];
            //DataTable printreportdesign = ClsExcel.GetDataTable(sPrintDesignDataTableName, sPrintSettingDataTableName);
            //DataTable printreportdesign = ClsPostgreSql.GetDataTableByName(sPrintDesignDataTableName, sPrintSettingDataTableName);
            IDatabaseReaderWriter dbReadWrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", Databases["SurveryProductCheckDatabase"]);
            DataTable printreportdesign = dbReadWrite.GetDataTable(sPrintSettingDataTableName);

            //1读取打印设置表的设计
            _PrintSettingDataTableDesign = new DataTableDesign(
                                 Databases[PrintEnvironment.PrintDesignDatabaseFilePath],
                                "PostgreSQL",

                                 Configs[PrintEnvironment.PrintDesignDataTableName], 
                                 Configs[PrintEnvironment.PrintDaTableDesignVersion],
                                 Configs[PrintEnvironment.DataTableDesignFieldAttributesName],

                                Databases["SurveryProductCheckDatabase"],
                                "PostgreSQL",
                                Configs["DatatableDataTypeRelationDataTableName"]);

            _PrintSettingDataTableDesign.InitializeDataTableDesign("PostgreSQL", sPrintSettingDataTableName);

            // 3.初始化打印项，存入打印项列表中
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = null;
            foreach (DataRow dr in printreportdesign.Rows)
            {
                string sFieldName = "";
                PrintItem printitem = new PrintItem();

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[0]);
                printitem.Index = Convert.ToInt32(dr[sFieldName]);

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[1]);
                printitem.Bookmark = Convert.ToString(dr[sFieldName]);

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[2]);
                printitem.PrintMode = Convert.ToString(dr[sFieldName]);

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[3]);

                if (!Convert.IsDBNull(dr[sFieldName]))
                {
                    string stringquery = Convert.ToString(dr[sFieldName]);
                    sr = new StringReader(stringquery);
                    
                    QueryFactory  qf = (QueryFactory)serializer.Deserialize(new JsonTextReader(sr), typeof(QueryFactory));
                    printitem.PrintQuery = qf.Creator();
                    printitem.PrintQuery.DbFilePath =  Databases[PrintEnvironment.SourceDatabaseFileName];
                }

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[4]);
                string stringprintsetting = Convert.ToString(dr[sFieldName]);
                if (!Convert.IsDBNull(dr[sFieldName]))
                {

                    sr = new StringReader(stringprintsetting);
                    printitem.PrintSetting = (PrintSetting)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintSetting));

                }
                _PrintItemList.Add(printitem);
            }            
        }

        public virtual string PrintWord(string sMapNumber,string sProjectid="")
        {
            string finalword = "";
            //组织相应信息写入模板中
            //实现数字、字符分别排序
            int nPageCount = 0;
            string[] wordArray = null;
            int iPageIndex = 0;
            string wordOrg = "";
            string reportName = "";
            string mapLongName = "";

            PrintItem tablePrintItem = null;
            foreach (PrintItem pitem in _PrintItemList)
            {
                //替换参数
                string sql = pitem.PrintQuery.QueryCondition;
                pitem.PrintQuery.QueryCondition = sql.Replace("{*}", sMapNumber);

                if(sProjectid!="")
                {
                    sql = pitem.PrintQuery.QueryCondition;
                    pitem.PrintQuery.QueryCondition = sql.Replace("{#}", sProjectid);
                }

                if (pitem.PrintSetting.PrintValueType == "DataTable")
                {
                    tablePrintItem = pitem;
                    continue;
                }
            }

            nPageCount = (int)Math.Ceiling((tablePrintItem.PrintQuery.QueryValue as DataTable).Rows.Count / (double)tablePrintItem.PrintSetting.TableItemCount);
            mapLongName = _PrintItemList[0].PrintQuery.QueryValue as string;
            reportName = mapLongName +  Configs[PrintEnvironment.SourceDatatableName];
            wordArray = new string[nPageCount>1?nPageCount-1:1];
            for (iPageIndex = 0; iPageIndex < nPageCount; iPageIndex++)
            {
                string sPageFileName = this.PrintPage(iPageIndex, tablePrintItem);
                //最后一页做记录
                if (iPageIndex == nPageCount-1)
                {
                    wordOrg = sPageFileName;
                }
                else
                {
                    wordArray[iPageIndex] = sPageFileName;
                }
            }

            //合并word
            if (iPageIndex > 1)
            {
                finalword = string.Format("{0}\\{1}.doc",  AppPath, reportName);
                WordDocumentMerger dm = new WordDocumentMerger();
                //逆序插入
                Array.Reverse(wordArray);
                dm.InsertMerge(wordOrg, wordArray, finalword);
                // 做好清理工作
                foreach (string wordFile in wordArray)
                {
                    File.Delete(wordOrg);
                    if (File.Exists(wordFile))
                        File.Delete(wordFile);
                }  
            }
            else
            {
                finalword = wordOrg;
            } 
            return finalword;
        }

        public  virtual string PrintPage(int iPageIndex,PrintItem DataTablePrintItem)
        {
            string PageWordPath = "";
            PrintSetting TablePrintSetting = DataTablePrintItem.PrintSetting;
            DataTable PointErrorTable = DataTablePrintItem.PrintQuery.QueryValue as DataTable;
            int nPageCount = (int)Math.Ceiling((DataTablePrintItem.PrintQuery.QueryValue as DataTable).Rows.Count / (double)DataTablePrintItem.PrintSetting.TableItemCount);
            int countInPaper = TablePrintSetting.TableItemCount;
            int pointcount = PointErrorTable.Rows.Count;
            int firstColCount = TablePrintSetting.FirstColumnCount;
            int secondColCount = TablePrintSetting.SecondColumnCount;
            string sTemplateFilePath =  Databases[PrintEnvironment.CheckReportTemplateFilePath];
            string mapLongName =  _PrintItemList[0].PrintQuery.QueryValue as string;
            PageWordPath = nPageCount == 1 ? string.Format("{0}\\{1}.doc", AppPath, mapLongName) : string.Format("{0}\\{1}-{2}.doc",  AppPath, mapLongName, iPageIndex);         

            try
            {
                _Word = new WordHelp();
                _Word.LoadDotFile(sTemplateFilePath);

                foreach (PrintItem pitem in _PrintItemList)
                {
                    if (pitem.PrintMode == "系统默认")
                        continue;

                    if (pitem.PrintQuery.QueryValueType == "String")
                    {
                        _Word.GotoBookMark(pitem.Bookmark);
                        string printvalue = pitem.PrintQuery.QueryValue as string;
                        _Word.bookmarkReplace(pitem.Bookmark, printvalue);
                    }
                    else if (pitem.PrintQuery.QueryValueType == "Date")
                    {
                        DateTime checktime = Convert.ToDateTime(pitem.PrintQuery.QueryValue);
                        _Word.bookmarkReplace(pitem.Bookmark, string.Format("{0}年{1}月{2}日", checktime.Year,checktime.Date.Month, checktime.Date.Day));
                    }
                }

                //写入点信息
                _Word.FindTable(DataTablePrintItem.Bookmark);
                for (int j = 0; j + iPageIndex * countInPaper < PointErrorTable.Rows.Count; j++)
                {
                    //PtID,TL,SL,DL,COMMENT
                    string sID = Convert.ToString(PointErrorTable.Rows[j + iPageIndex * countInPaper]["ID"]);
                    double sV1 = Convert.IsDBNull(PointErrorTable.Rows[j + iPageIndex * countInPaper]["V1"]) ? -1 : Convert.ToDouble(PointErrorTable.Rows[j + iPageIndex * countInPaper]["V1"]);
                    double sV2 = Convert.IsDBNull(PointErrorTable.Rows[j + iPageIndex * countInPaper]["V2"]) ? -1 : Convert.ToDouble(PointErrorTable.Rows[j + iPageIndex * countInPaper]["V2"]);
                    double sV3 = Convert.IsDBNull(PointErrorTable.Rows[j + iPageIndex * countInPaper]["V3"]) ? -1 : Convert.ToDouble(PointErrorTable.Rows[j + iPageIndex * countInPaper]["V3"]);
                    string sComment = Convert.ToString(PointErrorTable.Rows[j + iPageIndex * countInPaper]["COMMENT"]);

                    _Word.SetCellValue(sID);
                    _Word.MoveNextCell();
                    _Word.SetCellValue(sV1 == -1 ? "" : sV1.ToString("0.00"));
                    _Word.MoveNextCell();
                    _Word.SetCellValue(sV2 == -1 ? "" : sV2.ToString("0.00"));
                    _Word.MoveNextCell();
                    _Word.SetCellValue(sV3 == -1 ? "" : sV3.ToString("0.00"));
                    _Word.MoveNextCell();
                    _Word.SetCellValue(sComment);

                    //
                    if ((pointcount >= (iPageIndex * countInPaper + firstColCount + 1))
                        && j + iPageIndex * countInPaper >= iPageIndex * countInPaper && j + iPageIndex * countInPaper <= iPageIndex * countInPaper + secondColCount-1
                        && j + iPageIndex * countInPaper + firstColCount < pointcount)
                    {

                        sID = Convert.ToString(PointErrorTable.Rows[j + iPageIndex * countInPaper + firstColCount]["ID"]);
                        sV1 = Convert.IsDBNull(PointErrorTable.Rows[j + iPageIndex * countInPaper + firstColCount]["V1"]) ? -1 : Convert.ToDouble(PointErrorTable.Rows[j + iPageIndex * countInPaper + firstColCount]["V1"]);
                        sV2 = Convert.IsDBNull(PointErrorTable.Rows[j + iPageIndex * countInPaper + firstColCount]["V2"]) ? -1 : Convert.ToDouble(PointErrorTable.Rows[j + iPageIndex * countInPaper + firstColCount]["V2"]);
                        sV3 = Convert.IsDBNull(PointErrorTable.Rows[j + iPageIndex * countInPaper + firstColCount]["V3"]) ? -1 : Convert.ToDouble(PointErrorTable.Rows[j + iPageIndex * countInPaper + firstColCount]["V3"]);
                        sComment = Convert.ToString(PointErrorTable.Rows[j + iPageIndex * countInPaper + firstColCount]["COMMENT"]);

                        _Word.MoveNextCell();
                        _Word.MoveNextCell();
                        _Word.SetCellValue(sID);
                        _Word.MoveNextCell();
                        _Word.SetCellValue(sV1 == -1 ? "" : sV1.ToString("0.00"));
                        _Word.MoveNextCell();
                        _Word.SetCellValue(sV2 == -1 ? "" : sV2.ToString("0.00"));
                        _Word.MoveNextCell();
                        _Word.SetCellValue(sV3 == -1 ? "" : sV3.ToString("0.00"));
                        _Word.MoveNextCell();
                        _Word.SetCellValue(sComment);

                        if (j <= (iPageIndex * countInPaper + secondColCount))
                        {
                            _Word.GoToLeftCell();
                            _Word.GoToLeftCell();
                            _Word.GoToLeftCell();
                            _Word.GoToLeftCell();
                            _Word.GoToLeftCell();
                            _Word.GoToLeftCell();

                        }
                    }

                    if (j + iPageIndex * countInPaper >= iPageIndex * countInPaper + firstColCount - 1) break;
                    {
                        _Word.GoToLeftCell();
                        _Word.GoToLeftCell();
                        _Word.GoToLeftCell();
                        _Word.GoToLeftCell();
                        _Word.GoToDownCell();
                    }
                }

            }
            catch (Exception ex)
            {
                string sMsg = string.Format("写入word模板时出错：写入项目{0}的检查点要素失败。系统报错：{1}", mapLongName, ex.Message);
                _Word.Quit();
                _Word = null;
                throw new Exception(sMsg);

            }
            _Word.SaveAs(PageWordPath);
            _Word.Quit();

            return PageWordPath;
        }

    }

    public class PrintCheckRecordReport:PrintReport
    {
        public new string PrintWord(string sMapNumber, string sProjectid = "")
        {
            string finalword = "";
            //组织相应信息写入模板中
            //实现数字、字符分别排序
            int nPageCount = 0;
            string[] wordArray = null;
            int iPageIndex = 0;
            string wordOrg = "";
            string reportName = "";
            string mapLongName = "";

            PrintItem tablePrintItem = null;
            foreach (PrintItem pitem in _PrintItemList)
            {
                //替换参数
                string sql = pitem.PrintQuery.QueryCondition;
                pitem.PrintQuery.QueryCondition = sql.Replace("{*}", sMapNumber);

                if (sProjectid != "")
                {
                    sql = pitem.PrintQuery.QueryCondition;
                    pitem.PrintQuery.QueryCondition = sql.Replace("{#}", sProjectid);
                }

                if (pitem.PrintSetting.PrintValueType == "DataTable")
                {
                    tablePrintItem = pitem;
                    continue;
                }
            }
            if(tablePrintItem!=null)
            {
                nPageCount = (int)Math.Ceiling((tablePrintItem.PrintQuery.QueryValue as DataTable).Rows.Count / (double)tablePrintItem.PrintSetting.TableItemCount);

            }else
            {
                nPageCount = 1;
            }
            mapLongName = _PrintItemList[1].PrintQuery.QueryValue as string;
            reportName = mapLongName + Configs[PrintEnvironment.SourceDatatableName];
            wordArray = new string[nPageCount > 1 ? nPageCount - 1 : 1];
            for (iPageIndex = 0; iPageIndex < nPageCount; iPageIndex++)
            {
                string sPageFileName = this.PrintPage(iPageIndex, tablePrintItem);
                //最后一页做记录
                if (iPageIndex == nPageCount - 1)
                {
                    wordOrg = sPageFileName;
                }
                else
                {
                    wordArray[iPageIndex] = sPageFileName;
                }
            }

            //合并word
            if (iPageIndex > 1)
            {
                if (reportName.IndexOf(':') >= 0) reportName = reportName.Replace(':', '-');
                finalword = string.Format("{0}\\{1}.doc", AppPath, reportName);

                WordDocumentMerger dm = new WordDocumentMerger();
                //逆序插入
                Array.Reverse(wordArray);
                dm.InsertMerge(wordOrg, wordArray, finalword);
                // 做好清理工作
                foreach (string wordFile in wordArray)
                {
                    File.Delete(wordOrg);
                    if (File.Exists(wordFile))
                        File.Delete(wordFile);
                }
            }
            else
            {
                finalword = wordOrg;
            }
            return finalword;
        }

        public new void PrintInit(PrintEnvironment pPrintEnvironment)
        {
            PrintEnvironment = pPrintEnvironment;
            _PrintItemList = new List<PrintItem>();

            // 2.读取打印设置表本身
            string sPrintDesignDataTableName = Databases[PrintEnvironment.PrintDesignDatabaseFilePath];
            string sPrintSettingDataTableName = Configs[PrintEnvironment.PrintSettingDataTableName];
            // DataTable printreportdesign = ClsExcel.GetDataTable(sPrintDesignDataTableName, sPrintSettingDataTableName);
            //DataTable printreportdesign = ClsPostgreSql.GetDataTableByName(sPrintDesignDataTableName, sPrintSettingDataTableName);

            IDatabaseReaderWriter dbReadWrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", Databases["SurveryProductCheckDatabase"]);
            DataTable printreportdesign = dbReadWrite.GetDataTable(sPrintSettingDataTableName);

            //1读取打印设置表的设计
            _PrintSettingDataTableDesign = new DataTableDesign(
                                 Databases[PrintEnvironment.PrintDesignDatabaseFilePath],
                                 "PostgreSQL",

                                 Configs[PrintEnvironment.PrintDesignDataTableName],
                                 Configs[PrintEnvironment.PrintDaTableDesignVersion],
                                 Configs[PrintEnvironment.DataTableDesignFieldAttributesName],

                                 Databases["SurveryProductCheckDatabase"],
                                 "PostgreSQL",
                                 Configs["DatatableDataTypeRelationDataTableName"]);

            _PrintSettingDataTableDesign.InitializeDataTableDesign("PostgreSQL", sPrintSettingDataTableName);

            // 3.初始化打印项，存入打印项列表中
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = null;
            foreach (DataRow dr in printreportdesign.Rows)
            {
                string sFieldName = "";
                PrintItem printitem = new PrintItem();

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[0]);
                //printitem.Index = Convert.ToInt32(dr[sFieldName]);
                string strIndex = dr[sFieldName] as string;
                int index = 0;
                int.TryParse(strIndex,out index);
                printitem.Index = index;

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[1]);
                printitem.Bookmark = Convert.ToString(dr[sFieldName]);

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[2]);
                printitem.PrintMode = Convert.ToString(dr[sFieldName]);

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[3]);

                if (!Convert.IsDBNull(dr[sFieldName]))
                {
                    string stringquery = Convert.ToString(dr[sFieldName]);
                    sr = new StringReader(stringquery);
                    printitem.PrintQuery = (DataValueQuery)serializer.Deserialize(new JsonTextReader(sr), typeof(DataValueQuery));
                    printitem.PrintQuery.DbFilePath = Databases[PrintEnvironment.SourceDatabaseFileName];
                }

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[4]);
                string stringprintsetting = Convert.ToString(dr[sFieldName]);
                if (!Convert.IsDBNull(dr[sFieldName]))
                {

                    sr = new StringReader(stringprintsetting);
                    printitem.PrintSetting = (PrintSetting)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintSetting));

                }
                _PrintItemList.Add(printitem);
            }
        }

        public new string PrintPage(int iPageIndex, PrintItem DataTablePrintItem)
        {

            string PageWordPath = "";
            PrintSetting TablePrintSetting = null;

            DataTable PointErrorTable=null;           
            int nPageCount = 1;
            int pointcount = 0;
            int countInPaper = 0;
            int firstColCount = 0;
            int secondColCount = 0;

            if (DataTablePrintItem!=null)
            {
                TablePrintSetting = DataTablePrintItem.PrintSetting;
                PointErrorTable = DataTablePrintItem.PrintQuery.QueryValue as DataTable;
                nPageCount = (int)Math.Ceiling((DataTablePrintItem.PrintQuery.QueryValue as DataTable).Rows.Count / (double)DataTablePrintItem.PrintSetting.TableItemCount);
                pointcount = PointErrorTable.Rows.Count;
                countInPaper = TablePrintSetting.TableItemCount;
                firstColCount = TablePrintSetting.FirstColumnCount;
                secondColCount = TablePrintSetting.SecondColumnCount;
            }                      


            string sTemplateFilePath = Databases[PrintEnvironment.CheckReportTemplateFilePath];
            string mapLongName = _PrintItemList[1].PrintQuery.QueryValue as string;
            if (mapLongName.IndexOf(':') >= 0) mapLongName = mapLongName.Replace(':', '-');

            PageWordPath = nPageCount == 1 ? string.Format("{0}\\{1}.doc", AppPath, mapLongName) : string.Format("{0}\\{1}-{2}.doc", AppPath, mapLongName, iPageIndex);

            try
            {
                _Word = new WordHelp();
                _Word.LoadDotFile(sTemplateFilePath);

                foreach (PrintItem pitem in _PrintItemList)
                {
                    if (pitem.PrintMode == "系统默认")
                        continue;

                    if (pitem.PrintQuery.QueryValueType.IndexOf("String")>=0)
                    {
                        _Word.GotoBookMark(pitem.Bookmark);
                        string printvalue = pitem.PrintQuery.QueryValue as string;
                        _Word.bookmarkReplace(pitem.Bookmark, printvalue);
                    }
                    else if (pitem.PrintQuery.QueryValueType == "Date")
                    {
                        DateTime checktime = Convert.ToDateTime(pitem.PrintQuery.QueryValue);
                        _Word.bookmarkReplace(pitem.Bookmark, string.Format("{0}年{1}月{2}日", checktime.Year, checktime.Date.Month, checktime.Date.Day));
                    }
                }

                //写入点信息
                if (PointErrorTable == null)
                {
                    _Word.SaveAs(PageWordPath);
                    _Word.Quit();

                    return PageWordPath;
                };

                _Word.FindTable(DataTablePrintItem.Bookmark);
                int stopindex = PointErrorTable.Rows.Count  > iPageIndex * countInPaper + firstColCount ? iPageIndex * countInPaper + firstColCount : PointErrorTable.Rows.Count;
                for (int j = 0; j + iPageIndex * countInPaper < stopindex; j++)
                {

                    //使用循环打印
                    int subindex = 0;
                    foreach (string field in DataTablePrintItem.PrintQuery.QueryFieldsArray)
                    {
                        string fieldstr = field;
                        //如果field 使用了别名，as 后面的字符进行提取后识别
                        if (fieldstr.IndexOf(" as ") > 0)
                        {
                            fieldstr = fieldstr.Substring(field.IndexOf(" as ") + 4);
                            fieldstr.Trim();
                        }
                        try
                        {
                            string printvalue = "";
                            DataRow dr = PointErrorTable.Rows[j + iPageIndex * countInPaper];
                            if (Convert.IsDBNull(dr[field]) == true)
                            {
                                printvalue = "——";
                            }
                            else
                            {
                                printvalue = Convert.ToString(dr[field]);
                                if (IsNumeric(printvalue) == true)
                                {
                                    printvalue = Convert.ToDouble(printvalue).ToString("0.00");

                                }

                            }

                            _Word.SetCellValue(printvalue);
                            if (subindex < DataTablePrintItem.PrintQuery.QueryFieldsArray.Count - 1)
                            {
                                _Word.MoveNextCell();
                            }
                            else if (subindex == DataTablePrintItem.PrintQuery.QueryFieldsArray.Count - 1 && j < PointErrorTable.Rows.Count - 1)
                            {
                                _Word.MoveNextRow();
                            }
                            subindex++;
                        }
                        catch
                        {
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string sMsg = string.Format("写入word模板时出错：写入项目{0}的检查点要素失败。系统报错：{1}", mapLongName, ex.Message);
                _Word.Quit();
                _Word = null;
                throw new Exception(sMsg);

            }

            _Word.SaveAs(PageWordPath);
            _Word.Quit();

            return PageWordPath;
            //return base.PrintPage(iPageIndex, DataTablePrintItem);
        }
    }

    public class PrintQulityReport:PrintReport
    {

        public new void PrintInit(PrintEnvironment pPrintEnvironment)
        {
            PrintEnvironment = pPrintEnvironment;
            _PrintItemList = new List<PrintItem>();

            // 2.读取打印设置表本身
            string sPrintDesignDataTableName =  Databases[PrintEnvironment.PrintDesignDatabaseFilePath];
            string sPrintSettingDataTableName =  Configs[PrintEnvironment.PrintSettingDataTableName];
           // DataTable printreportdesign = ClsExcel.GetDataTable(sPrintDesignDataTableName, sPrintSettingDataTableName);
            //DataTable printreportdesign = ClsPostgreSql.GetDataTableByName(sPrintDesignDataTableName, sPrintSettingDataTableName);

            IDatabaseReaderWriter dbReadWrite = DatabaseReaderWriterFactory.GetDatabaseReaderWriter("PostgreSQL", Databases["SurveryProductCheckDatabase"]);
            DataTable printreportdesign = dbReadWrite.GetDataTable(sPrintSettingDataTableName);

            //1读取打印设置表的设计
            _PrintSettingDataTableDesign = new DataTableDesign(
                                 Databases[PrintEnvironment.PrintDesignDatabaseFilePath],
                                 "PostgreSQL",

                                 Configs[PrintEnvironment.PrintDesignDataTableName],
                                 Configs[PrintEnvironment.PrintDaTableDesignVersion],
                                 Configs[PrintEnvironment.DataTableDesignFieldAttributesName],

                                 Databases["SurveryProductCheckDatabase"],
                                 "PostgreSQL",
                                 Configs["DatatableDataTypeRelationDataTableName"]);

            _PrintSettingDataTableDesign.InitializeDataTableDesign("PostgreSQL", sPrintSettingDataTableName);

            // 3.初始化打印项，存入打印项列表中
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = null;
            foreach (DataRow dr in printreportdesign.Rows)
            {
                string sFieldName = "";
                PrintItem printitem = new PrintItem();

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[0]);
                printitem.Index = Convert.ToInt32(dr[sFieldName]);

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[1]);
                printitem.Bookmark = Convert.ToString(dr[sFieldName]);

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[2]);
                printitem.PrintMode = Convert.ToString(dr[sFieldName]);

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[3]);

                if (!Convert.IsDBNull(dr[sFieldName]))
                {
                    string stringquery = Convert.ToString(dr[sFieldName]);
                    sr = new StringReader(stringquery);
                    printitem.PrintQuery = (DataValueQuery)serializer.Deserialize(new JsonTextReader(sr), typeof(DataValueQuery));
                    printitem.PrintQuery.DbFilePath = Databases[PrintEnvironment.SourceDatabaseFileName];
                }

                sFieldName = Convert.ToString(_PrintSettingDataTableDesign.TargetFieldsCodeList[4]);
                string stringprintsetting = Convert.ToString(dr[sFieldName]);
                if (!Convert.IsDBNull(dr[sFieldName]))
                {

                    sr = new StringReader(stringprintsetting);
                    printitem.PrintSetting = (PrintSetting)serializer.Deserialize(new JsonTextReader(sr), typeof(PrintSetting));

                }
                _PrintItemList.Add(printitem);
            }
        }

        public new string PrintWord(string sProjectName, string sProjectid = "")
        {
            //组织相应信息写入模板中
            //实现数字、字符分别排序
            int iPageIndex = 0;
            string reportName = "";
            string mapLongName = "";

           // PrintItem tablePrintItem = null;
            List<PrintItem> DataTablePrintItem = new List<PrintItem>();
            foreach (PrintItem pitem in _PrintItemList)
            {
                //替换参数
                string sql = pitem.PrintQuery.QueryCondition;
                pitem.PrintQuery.QueryCondition = sql.Replace("{*}", sProjectName);

                if (sProjectid != "")
                {
                    sql = pitem.PrintQuery.QueryCondition;
                    pitem.PrintQuery.QueryCondition = sql.Replace("{#}", sProjectid);
                }

                if (pitem.PrintSetting.PrintValueType == "DataTable")
                {
                    DataTablePrintItem.Add(pitem);
                    continue;
                }
            }

            if (sProjectName.IndexOf(':') >= 0) sProjectName = sProjectName.Replace(':', '-');
            reportName = string.Format("{0}\\{1}{2}.doc",  AppPath, sProjectName, Configs[PrintEnvironment.SourceDatatableName]);
            string sTemplateFilePath =  Databases[PrintEnvironment.CheckReportTemplateFilePath];

            try
            {
                _Word = new WordHelp();
                _Word.LoadDotFile(sTemplateFilePath);

                foreach (PrintItem pitem in _PrintItemList)
                {
                    if (pitem.PrintMode == "系统默认")
                        continue;

                    if (pitem.PrintQuery.QueryValueType.IndexOf("String")>=0)
                    {
                        _Word.GotoBookMark(pitem.Bookmark);
                        string printvalue = pitem.PrintQuery.QueryValue as string;
                        _Word.bookmarkReplace(pitem.Bookmark, printvalue);
                    }
                    else if (pitem.PrintQuery.QueryValueType == "Date")
                    {
                        DateTime checktime = Convert.ToDateTime(pitem.PrintQuery.QueryValue);
                        _Word.bookmarkReplace(pitem.Bookmark, string.Format("{0}年{1}月{2}日", checktime.Year, checktime.Date.Month, checktime.Date.Day));
                    }
                }


                foreach (PrintItem pdtitem in DataTablePrintItem)
                {
                    PrintSetting TablePrintSetting = pdtitem.PrintSetting;
                    DataTable PointErrorTable = pdtitem.PrintQuery.QueryValue as DataTable;
                    int countInPaper = TablePrintSetting.TableItemCount;
                    int pointcount = PointErrorTable.Rows.Count;
                    int firstColCount = TablePrintSetting.FirstColumnCount;
                    int secondColCount = TablePrintSetting.SecondColumnCount;

                    //if firstColCount=0,print a word
                    //if firstColCount=a number,print page number = point number / firstColCount;
                    //写入点信息
                    _Word.FindTable(pdtitem.Bookmark);
                    for (int j = 0; j < PointErrorTable.Rows.Count; j++)
                    {
                        //使用循环打印
                        int subindex = 0;
                        foreach (string field in pdtitem.PrintQuery.QueryFieldsArray)
                        {
                            string fieldstr = field;
                            //如果field 使用了别名，as 后面的字符进行提取后识别
                            if(fieldstr.IndexOf(" as ")>0)
                            {
                                fieldstr = fieldstr.Substring(field.IndexOf(" as ") + 4);
                                fieldstr.Trim();
                            }
                            try
                            {
                                string printvalue = "";
                                DataRow dr = PointErrorTable.Rows[j + iPageIndex * countInPaper];
                                if (Convert.IsDBNull(dr[field]) == true)
                                {
                                    printvalue = "——";
                                }
                                else
                                {
                                    printvalue = Convert.ToString(dr[field]);
                                    if (IsNumeric(printvalue) == true)
                                    {
                                        printvalue = Convert.ToDouble(printvalue).ToString("0.00");

                                    }
                                    
                                }

                                _Word.SetCellValue(printvalue);
                                if (subindex < pdtitem.PrintQuery.QueryFieldsArray.Count - 1)
                                {
                                    _Word.MoveNextCell();
                                }
                                else if (subindex == pdtitem.PrintQuery.QueryFieldsArray.Count - 1 && j < PointErrorTable.Rows.Count - 1)
                                {
                                    _Word.MoveNextRow();
                                }
                                subindex++;
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string sMsg = string.Format("写入word模板时出错：写入项目{0}的检查点要素失败。系统报错：{1}", mapLongName, ex.Message);
                _Word.Quit();
                _Word = null;
                throw new Exception(sMsg);
            }
            _Word.SaveAs(reportName);
            _Word.Quit();

            return reportName;
        }

    }
}

/***************************************************************************
 * word辅助类
 * 作者：chengfellow
 * 日期：2008.8.18
 * 注意事项：
 * 1、开发环境居于office 2003；
 * 2、需要添加Com引用：Microsoft Office 11.0 Object Library和
 *    Microsoft Word 11.0 Object Library。
 * 
 
****************************************************************************/

using
 System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Data;
namespace WordAddinSample
{
    public class WordHelp
    {
        #region - 属性 -
        private Microsoft.Office.Interop.Word.ApplicationClass oWord;    // a reference to Word application，应用程序
        private Microsoft.Office.Interop.Word.Document oDoc;            // a reference to the document，具体文档
        object missing = System.Reflection.Missing.Value;
        public Microsoft.Office.Interop.Word.ApplicationClass WordApplication
        {
            get { return oWord; }
        }
        public string ActiveWindowCaption {
            get {
                return oWord.ActiveWindow.Caption;
            }
            set {
                oWord.ActiveWindow.Caption = value;
            }
        }
        public enum OwdWrapType
        {
            嵌入型, //wdWrapInline
            四周型, //Square.
            紧密型, //Tight.
            衬于文字下方,//Behind text.
            衬于文字上方 //Top and bottom.
        }
        #endregion
        #region  - 创建关闭文档 -
        public WordHelp() //构造函数 1
        {
            // activate the interface with the COM object of Microsoft Word
            oWord = new Microsoft.Office.Interop.Word.ApplicationClass();
        }
        public WordHelp(Microsoft.Office.Interop.Word.ApplicationClass wordapp) //构造函数 2
        {
            oWord = wordapp;
        }
        // Open a file (the file must exists) and activate it，打开已存在
        public void Open(string strFileName)
        {
            object fileName = strFileName;
            object readOnly = false;
            object isVisible = true;
            oDoc = oWord.Documents.Open(ref fileName, ref missing, ref readOnly,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref isVisible, ref missing, ref missing, ref missing, ref missing);
            oDoc.Activate();
        }
        // Open a new document，创建新文档
        public void Open()
        {
            oDoc = oWord.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            oDoc.Activate();
        }
        public void Quit()
        {
            oDoc.Close(ref missing, ref missing, ref missing);
            if (oDoc != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
                oDoc = null;
            }
           // oWord.Application.Quit(ref missing, ref missing, ref missing); tjt
            oWord.Quit(ref missing, ref missing, ref missing);
            if (oWord != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oWord);
                oWord = null;
            }
            //释放word进程
            GC.Collect();
        }       
        /// <summary>  
        /// 从模板创建新的Word文档，  
        /// </summary>  
        /// <param name="templateName">模板文件名</param>  
        /// <returns></returns>  
        public bool LoadDotFile(string templateName)
        {
            if (!string.IsNullOrEmpty(templateName))
            {
                oWord.Visible = false;
                oWord.Caption = "";
                oWord.Options.CheckSpellingAsYouType = false;
                oWord.Options.CheckGrammarAsYouType = false;
                Object Template = templateName;
                // Optional Object. The name of the template to be used for the new document. If this argument is omitted, the Normal template is used.  
                Object NewTemplate = false;// Optional Object. True to open the document as a template. The default value is False.  
                Object DocumentType = Microsoft.Office.Interop.Word.WdNewDocumentType.wdNewBlankDocument; 
                // Optional Object. Can be one of the following WdNewDocumentType constants: wdNewBlankDocument, wdNewEmailMessage, wdNewFrameset, or wdNewWebPage. The default constant is wdNewBlankDocument.  
                Object Visible = true;
                //Optional Object. True to open the document in a visible window. If this value is False, Microsoft Word opens the document but sets the Visible property of the document window to False. The default value is True.  
                try
                {
                    oDoc = oWord.Documents.Add(ref Template, ref NewTemplate, ref DocumentType, ref Visible);
                    return true;
                }
                catch (Exception ex)
                {
                    string err = string.Format("创建Word文档出错，错误原因：{0}。系统依赖于微软word自动化模块，产生此类问题的原因可能是进程管理器中存在异常的word进程，请手动关闭。", ex.Message);
                    throw new Exception(err, ex);
                }               
            }
            return false;
        }
        ///  
        /// 打开Word文档,并且返回对象oDoc
        /// 完整Word文件路径+名称  
        /// 返回的Word.Document oDoc对象 
        public Microsoft.Office.Interop.Word.Document CreateWordDocument(string FileName, bool HideWin)
        {
            if (FileName == "") return null;
            oWord.Visible = HideWin;
            oWord.Caption = "";
            oWord.Options.CheckSpellingAsYouType = false;
            oWord.Options.CheckGrammarAsYouType = false;
            Object filename = FileName;
            Object ConfirmConversions = false;
            Object ReadOnly = true;
            Object AddToRecentFiles = false;
            Object PasswordDocument = System.Type.Missing;
            Object PasswordTemplate = System.Type.Missing;
            Object Revert = System.Type.Missing;
            Object WritePasswordDocument = System.Type.Missing;
            Object WritePasswordTemplate = System.Type.Missing;
            Object Format = System.Type.Missing;
            Object Encoding = System.Type.Missing;
            Object Visible = System.Type.Missing;
            Object OpenAndRepair = System.Type.Missing;
            Object DocumentDirection = System.Type.Missing;
            Object NoEncodingDialog = System.Type.Missing;
            Object XMLTransform = System.Type.Missing;
            try
            {
                Microsoft.Office.Interop.Word.Document wordDoc = oWord.Documents.Open(ref filename, ref ConfirmConversions,
                ref ReadOnly, ref AddToRecentFiles, ref PasswordDocument, ref PasswordTemplate,
                ref Revert, ref WritePasswordDocument, ref WritePasswordTemplate, ref Format,
                ref Encoding, ref Visible, ref OpenAndRepair, ref DocumentDirection,
                ref NoEncodingDialog, ref XMLTransform);
                return wordDoc;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public void SaveAs(Microsoft.Office.Interop.Word.Document oDoc, string strFileName)
        {
            object fileName = strFileName;
            if (File.Exists(strFileName))
            {
                if (MessageBox.Show("文件'" + strFileName + "'已经存在，选确定覆盖原文件，选取消退出操作！", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    oDoc.SaveAs(ref fileName, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                              ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
                }
                else
                {
                    Clipboard.Clear();
                }
            }
            else
            {
                oDoc.SaveAs(ref fileName, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
            }
        }
        public void SaveAsHtml(Microsoft.Office.Interop.Word.Document oDoc, string strFileName)
        {
            object fileName = strFileName;
            //wdFormatWebArchive保存为单个网页文件
            //wdFormatFilteredHTML保存为过滤掉word标签的htm文件，缺点是有图片的话会产生网页文件夹
            if (File.Exists(strFileName))
            {
                if (MessageBox.Show("文件'" + strFileName + "'已经存在，选确定覆盖原文件，选取消退出操作！", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    object Format = (int)Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatWebArchive;
                    oDoc.SaveAs(ref fileName, ref Format, ref missing, ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
                }
                else
                {
                    Clipboard.Clear();
                }
            }
            else
            {
                object Format = (int)Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatWebArchive;
                oDoc.SaveAs(ref fileName, ref Format, ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing, 
                    ref missing, ref missing, ref missing, ref missing, ref missing);
            }
        }
        public void Save()
        {
            oDoc.Save();
        }
        public void SaveAs(string strFileName)
        {
            object fileName = strFileName;
            oDoc.SaveAs(ref fileName, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
        }
        // Save the document in HTML format
        public void SaveAsHtml(string strFileName)
        {
            object fileName = strFileName;
            object Format = (int)Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML;
            oDoc.SaveAs(ref fileName, ref Format, ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
        }
        #endregion
        #region 添加菜单(工具栏)项
        //添加单独的菜单项
        public void AddMenu(Microsoft.Office.Core.CommandBarPopup popuBar)
        {
            Microsoft.Office.Core.CommandBar menuBar = null;
            menuBar = this.oWord.CommandBars["Menu Bar"];
            popuBar = (Microsoft.Office.Core.CommandBarPopup)this.oWord.CommandBars.FindControl(Microsoft.Office.Core.MsoControlType.msoControlPopup, missing, popuBar.Tag, true);
            if (popuBar == null)
            {
                popuBar = (Microsoft.Office.Core.CommandBarPopup)menuBar.Controls.Add(Microsoft.Office.Core.MsoControlType.msoControlPopup, missing, missing, missing, missing);
            }
        }
        //添加单独工具栏
        public void AddToolItem(string strBarName, string strBtnName)
        {
            Microsoft.Office.Core.CommandBar toolBar = null;
            toolBar = (Microsoft.Office.Core.CommandBar)this.oWord.CommandBars.FindControl(Microsoft.Office.Core.MsoControlType.msoControlButton, missing, strBarName, true);
            if (toolBar == null)
            {
                toolBar = (Microsoft.Office.Core.CommandBar)this.oWord.CommandBars.Add(Microsoft.Office.Core.MsoControlType.msoControlButton,
                     missing, missing, missing);
                toolBar.Name = strBtnName;
                toolBar.Visible = true;
            }
        }
        #endregion
        #region 移动光标位置
        // Go to a predefined bookmark, if the bookmark doesn't exists the application will raise an error
        public void GotoBookMark(string strBookMarkName)
        {
            // VB :  Selection.GoTo What:=wdGoToBookmark, Name:="nome"
            object Bookmark = (int)Microsoft.Office.Interop.Word.WdGoToItem.wdGoToBookmark;
            object NameBookMark = strBookMarkName;
            oWord.Selection.GoTo(ref Bookmark, ref missing, ref missing, ref NameBookMark);
        }
        public void GoToTheEnd()
        {
            // VB :  Selection.EndKey Unit:=wdStory
            object unit;
            unit = Microsoft.Office.Interop.Word.WdUnits.wdStory;
            oWord.Selection.EndKey(ref unit, ref missing);
        }
        public void GoToLineEnd()
        {
            object unit = Microsoft.Office.Interop.Word.WdUnits.wdLine;
            object ext = Microsoft.Office.Interop.Word.WdMovementType.wdExtend;
            oWord.Selection.EndKey(ref unit, ref ext);
        }
        public void GoToTheBeginning()
        {
            // VB : Selection.HomeKey Unit:=wdStory
            object unit;
            unit = Microsoft.Office.Interop.Word.WdUnits.wdStory;
            oWord.Selection.HomeKey(ref unit, ref missing);
        }
        public void GoToTheTable(int ntable)
        {
            //    Selection.GoTo What:=wdGoToTable, Which:=wdGoToFirst, Count:=1, Name:=""
            //    Selection.Find.ClearFormatting
            //    With Selection.Find
            //        .Text = ""
            //        .Replacement.Text = ""
            //        .Forward = True
            //        .Wrap = wdFindContinue
            //        .Format = False
            //        .MatchCase = False
            //        .MatchWholeWord = False
            //        .MatchWildcards = False
            //        .MatchSoundsLike = False
            //        .MatchAllWordForms = False
            //    End With
            object what;
            what = Microsoft.Office.Interop.Word.WdUnits.wdTable;
            object which;
            which = Microsoft.Office.Interop.Word.WdGoToDirection.wdGoToFirst;
            object count;
            count = 1;
            oWord.Selection.GoTo(ref what, ref which, ref count, ref missing);
            oWord.Selection.Find.ClearFormatting();
            oWord.Selection.Text = "";
        }
        public void GoToRightCell()
        {
            // Selection.MoveRight Unit:=wdCell
            object direction;
            direction = Microsoft.Office.Interop.Word.WdUnits.wdCell;
            oWord.Selection.MoveRight(ref direction, ref missing, ref missing);
        }
        public void GoToLeftCell()
        {
            // Selection.MoveRight Unit:=wdCell
            object direction;
            direction = Microsoft.Office.Interop.Word.WdUnits.wdCell;
            oWord.Selection.MoveLeft(ref direction, ref missing, ref missing);
        }
        public void GoToDownCell()
        {
            // Selection.MoveRight Unit:=wdCell
            object direction;
            direction = Microsoft.Office.Interop.Word.WdUnits.wdLine;
            oWord.Selection.MoveDown(ref direction, ref missing, ref missing);
        }
        public void GoToUpCell()
        {
            // Selection.MoveRight Unit:=wdCell
            object direction;
            direction = Microsoft.Office.Interop.Word.WdUnits.wdLine;
            oWord.Selection.MoveUp(ref direction, ref missing, ref missing);
        }
        #endregion
        #region  - 插入操作  -
        public void InsertText(string strText) //插入文本
        {
            oWord.Selection.TypeText(strText);
        }
        public void InsertLineBreak() //插入换行符
        {
            oWord.Selection.TypeParagraph();
        }        
        /// <summary>
        /// 插入多个空行
        /// </summary>
        /// <param name="nline"></param>
        public void InsertLineBreak(int nline)
        {
            for (int i = 0; i < nline; i++)
                oWord.Selection.TypeParagraph();
        }
        public void InsertPagebreak() //插入分页符
        {
            // VB : Selection.InsertBreak Type:=wdPageBreak
            object pBreak = 
(int)Microsoft.Office.Interop.Word.WdBreakType.wdPageBreak;
            oWord.Selection.InsertBreak(ref pBreak);
        }
        // 插入页码
        public void InsertPageNumber() //在正文中插入页码
        {
            object wdFieldPage = 
Microsoft.Office.Interop.Word.WdFieldType.wdFieldPage;
            object preserveFormatting = true;
            oWord.Selection.Fields.Add(oWord.Selection.Range, ref 
wdFieldPage, ref missing, ref preserveFormatting);
        }
        // 插入页码
        public void InsertPageNumber(string strAlign)
        {
            object wdFieldPage = 
Microsoft.Office.Interop.Word.WdFieldType.wdFieldPage;
            object preserveFormatting = true;
            oWord.Selection.Fields.Add(oWord.Selection.Range, ref 
wdFieldPage, ref missing, ref preserveFormatting);
            SetAlignment(strAlign);
        }
        #region - 插入页脚 -
        public bool InsertPageFooter(string text)
        {
            try
            {
                oWord.ActiveWindow.View.SeekView = 
Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageFooter;//页脚 
                oWord.Selection.InsertAfter(text); //.InsertAfter(text);
 
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertPageHeader(string text)
        {
            try
            {
                oWord.ActiveWindow.View.SeekView = 
Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader;//页眉
                oWord.Selection.InsertAfter(text); 
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool InsertPageFooterNumber()
        {
            try
            {
                oWord.ActiveWindow.View.SeekView = 
Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader; //页眉
                oWord.Selection.WholeStory();
                
oWord.Selection.ParagraphFormat.Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderBottom].LineStyle
 = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleNone; //取消页眉的下划线
                oWord.ActiveWindow.View.SeekView = 
Microsoft.Office.Interop.Word.WdSeekView.wdSeekMainDocument; //转到正文
                oWord.ActiveWindow.View.SeekView = 
Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageFooter;//页脚 
                oWord.Selection.TypeText("第");
                object page = 
Microsoft.Office.Interop.Word.WdFieldType.wdFieldPage; //当前页码
                oWord.Selection.Fields.Add(oWord.Selection.Range, ref 
page, ref missing, ref missing);
                oWord.Selection.TypeText("页/共");
                object pages = 
Microsoft.Office.Interop.Word.WdFieldType.wdFieldNumPages; //总页码
                oWord.Selection.Fields.Add(oWord.Selection.Range, ref 
pages, ref missing, ref missing);
                oWord.Selection.TypeText("页");
                oWord.ActiveWindow.View.SeekView = 
Microsoft.Office.Interop.Word.WdSeekView.wdSeekMainDocument;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
        public void InsertLine(float left, float top, float width, float
 weight, int r, int g, int b)
        {
            //SetFontColor("red");
            //SetAlignment("Center");
            object Anchor = oWord.Selection.Range;
            //int pLeft = 0, pTop = 0, pWidth = 0, pHeight = 0;
            //oWord.ActiveWindow.GetPoint(out pLeft, out pTop, out pWidth, out pHeight,missing);
            //MessageBox.Show(pLeft + "," + pTop + "," + pWidth + "," +pHeight);
            object rep = false;
            //left += oWord.ActiveDocument.PageSetup.LeftMargin;
            left = oWord.CentimetersToPoints(left);
            top = oWord.CentimetersToPoints(top);
            width = oWord.CentimetersToPoints(width);
            Microsoft.Office.Interop.Word.Shape s = oWord.ActiveDocument.Shapes.AddLine(0, top, width, top, ref Anchor);
            s.Line.ForeColor.RGB = RGB(r, g, b);
            s.Line.Visible = Microsoft.Office.Core.MsoTriState.msoTrue;
            s.Line.Style = Microsoft.Office.Core.MsoLineStyle.msoLineSingle;
            s.Line.Weight = weight;
        }
        #endregion
        #region - 插入图片 -
        public void InsertImage(string strPicPath, float picWidth, float picHeight)
        {
            string FileName = strPicPath;
            object LinkToFile = false;
            object SaveWithDocument = true;
            object Anchor = oWord.Selection.Range;
            oWord.ActiveDocument.InlineShapes.AddPicture(FileName, ref LinkToFile, ref SaveWithDocument, ref Anchor).Select();
            oWord.Selection.InlineShapes[1].Width = picWidth; // 图片宽度 
            oWord.Selection.InlineShapes[1].Height = picHeight; // 图片高度
        }
        //public void InsertImage(string strPicPath, float picWidth, float picHeight, OwdWrapType owdWrapType)
        //{
        //    string FileName = strPicPath;
        //    object LinkToFile = false;
        //    object SaveWithDocument = true;
        //    object Anchor = oWord.Selection.Range;
        //    oWord.ActiveDocument.InlineShapes.AddPicture(FileName, ref LinkToFile, ref SaveWithDocument, ref Anchor).Select();
        //    oWord.Selection.InlineShapes[1].Width = picWidth; // 图片宽度 
        //    oWord.Selection.InlineShapes[1].Height = picHeight; // 图片高度
        //    // 将图片设置为四面环绕型 
        //  //  Microsoft.Office.Interop.Word.Shape s = oWord.Selection.InlineShapes[1].ConvertToShape();
        //  //  s.WrapFormat.Type = Microsoft.Office.Interop.Word.WdWrapType.wdWrapNone; //wdWrapSquare 四周环绕型
        //}
        #endregion
        #region - 插入表格 -
        public bool InsertTable(DataTable dt, bool haveBorder, double[] 
colWidths)
        {
            try
            {
                object Nothing = System.Reflection.Missing.Value;
                int lenght = oDoc.Characters.Count - 1;
                object start = lenght;
                object end = lenght;
                //表格起始坐标
                Microsoft.Office.Interop.Word.Range tableLocation = 
oDoc.Range(ref start, ref end);
                //添加Word表格     
                Microsoft.Office.Interop.Word.Table table = 
oDoc.Tables.Add(tableLocation, dt.Rows.Count, dt.Columns.Count, ref 
Nothing, ref Nothing);
                if (colWidths != null)
                {
                    for (int i = 0; i < colWidths.Length; i++)
                    {
                        table.Columns[i + 1].Width = (float)(28.5F * 
colWidths[i]);
                    }
                }
                ///设置TABLE的样式
                table.Rows.HeightRule = 
Microsoft.Office.Interop.Word.WdRowHeightRule.wdRowHeightAtLeast;
                table.Rows.Height = 
oWord.CentimetersToPoints(float.Parse("0.8"));
                table.Range.Font.Size = 10.5F;
                table.Range.Font.Name = "宋体";
                table.Range.Font.Bold = 0;
                table.Range.ParagraphFormat.Alignment = 
Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table.Range.Cells.VerticalAlignment = 
Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                if (haveBorder == true)
                {
                    //设置外框样式
                    table.Borders.OutsideLineStyle = 
Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                    table.Borders.InsideLineStyle = 
Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                    //样式设置结束
                }
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    for (int col = 0; col < dt.Columns.Count; col++)
                    {
                        table.Cell(row + 1, col + 1).Range.Text = 
dt.Rows[row][col].ToString();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "错误提示", 
MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
            }
        }
        public bool InsertTable(DataTable dt, bool haveBorder)
        {
            return InsertTable(dt, haveBorder, null);
        }
        public bool InsertTable(DataTable dt)
        {
            return InsertTable(dt, false, null);
        }
        //插入表格结束
        #endregion 
        #region 设置样式
        /// <summary>
        /// Change the paragraph alignement
        /// </summary>
        /// <param name="strType"></param>
        public void SetAlignment(string strType)
        {
            switch (strType.ToLower())
            {
                case "center":
                    oWord.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    break;
                case "left":
                    oWord.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    break;
                case "right":
                    oWord.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                    break;
                case "justify":
                    oWord.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphJustify;
                    break;
            }
        }

        // if you use thif function to change the font you should call it again with 
        // no parameter in order to set the font without a particular format
        public void SetFont(string strType)
        {
            switch (strType)
            {
                case "Bold":
                    oWord.Selection.Font.Bold = 1;
                    break;
                case "Italic":
                    oWord.Selection.Font.Italic = 1;
                    break;
                case "Underlined":
                    oWord.Selection.Font.Subscript = 0;
                    break;
            }
        }
        // disable all the style 
        public void SetFont()
        {
            oWord.Selection.Font.Bold = 0;
            oWord.Selection.Font.Italic = 0;
            oWord.Selection.Font.Subscript = 0;
            SetFontName("宋体"); //默认宋体，tjt
            SetFontSize(10.5f);  //默认五号字体，tjt
        }
        public void SetFontName(string strType)
        {
            oWord.Selection.Font.Name = strType;
        }
        public void SetFontSize(float nSize)
        {
            SetFontSize(nSize, 100);
        }
        public void SetFontSize(float nSize, int scaling)
        {
            if (nSize > 0f)
                oWord.Selection.Font.Size = nSize;
            if (scaling > 0)
                oWord.Selection.Font.Scaling = scaling;
        }
        public void SetFontColor(string strFontColor)
        {
            switch (strFontColor.ToLower())
            {
                case "blue":
                    oWord.Selection.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorBlue;
                    break;
                case "gold":
                    oWord.Selection.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorGold;
                    break;
                case "gray":
                    oWord.Selection.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorGray875;
                    break;
                case "green":
                    oWord.Selection.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorGreen;
                    break;
                case "lightblue":
                    oWord.Selection.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorLightBlue;
                    break;
                case "orange":
                    oWord.Selection.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorOrange;
                    break;
                case "pink":
                    oWord.Selection.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorPink;
                    break;
                case "red":
                    oWord.Selection.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorRed;
                    break;
                case "yellow":
                    oWord.Selection.Font.Color = Microsoft.Office.Interop.Word.WdColor.wdColorYellow;
                    break;
            }
        }
        public void SetPageNumberAlign(string strType, bool bHeader)
        {
            object alignment;
            object bFirstPage = false;
            object bF = true;
            //if (bHeader == true)
            
//WordApplic.Selection.HeaderFooter.PageNumbers.ShowFirstPageNumber = bF;
            switch (strType)
            {
                case "Center":
                    alignment = Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberCenter;
                    
//WordApplic.Selection.HeaderFooter.PageNumbers.Add(ref alignment,ref bFirstPage);
                    //Microsoft.Office.Interop.Word.Selection objSelection = WordApplic.pSelection;
                    
oWord.Selection.HeaderFooter.PageNumbers[1].Alignment = Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberCenter;
                    break;
                case "Right":
                    alignment = Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberRight;
                    
oWord.Selection.HeaderFooter.PageNumbers[1].Alignment = Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberRight;
                    break;
                case "Left":
                    alignment = Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberLeft;
                    oWord.Selection.HeaderFooter.PageNumbers.Add(ref alignment, ref bFirstPage);
                    break;
            }
        }
        /// <summary>
        /// 设置页面为标准A4公文样式
        /// </summary>
        private void SetA4PageSetup()
        {
            oWord.ActiveDocument.PageSetup.TopMargin = oWord.CentimetersToPoints(3.7f);
            //oWord.ActiveDocument.PageSetup.BottomMargin = oWord.CentimetersToPoints(1f);
            oWord.ActiveDocument.PageSetup.LeftMargin = oWord.CentimetersToPoints(2.8f);
            oWord.ActiveDocument.PageSetup.RightMargin = oWord.CentimetersToPoints(2.6f);
            //oWord.ActiveDocument.PageSetup.HeaderDistance = oWord.CentimetersToPoints(2.5f);
            //oWord.ActiveDocument.PageSetup.FooterDistance = oWord.CentimetersToPoints(1f);
            oWord.ActiveDocument.PageSetup.PageWidth = oWord.CentimetersToPoints(21f);
            oWord.ActiveDocument.PageSetup.PageHeight = oWord.CentimetersToPoints(29.7f);
        }
        #endregion
        #region 替换
        ///<summary>
        /// 在word 中查找一个字符串直接替换所需要的文本
        /// </summary>
        /// <param name="strOldText">原文本</param>
        /// <param name="strNewText">新文本</param>
        /// <returns></returns>
        public bool Replace(string strOldText, string strNewText)
        {
            if (oDoc == null)
                oDoc = oWord.ActiveDocument;
            this.oDoc.Content.Find.Text = strOldText;
            object FindText, ReplaceWith, Replace;// 
            FindText = strOldText;//要查找的文本
            ReplaceWith = strNewText;//替换文本
            Replace = 
Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;/**//*wdReplaceAll -
 替换找到的所有项。
                                                      * wdReplaceNone - 
不替换找到的任何项。
                                                    * wdReplaceOne - 
替换找到的第一项。
                                                    * */
            oDoc.Content.Find.ClearFormatting();//移除Find的搜索文本和段落格式设置
            if (oDoc.Content.Find.Execute(
                ref FindText, ref missing,
                ref missing, ref missing,
                ref missing, ref missing,
                ref missing, ref missing, ref missing,
                ref ReplaceWith, ref Replace,
                ref missing, ref missing,
                ref missing, ref missing))
            {
                return true;
            }
            return false;
        }
        public bool SearchReplace(string strOldText, string strNewText)
        {
            object replaceAll = 
Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;
            //首先清除任何现有的格式设置选项，然后设置搜索字符串 strOldText。
            oWord.Selection.Find.ClearFormatting();
            oWord.Selection.Find.Text = strOldText;
            oWord.Selection.Find.Replacement.ClearFormatting();
            oWord.Selection.Find.Replacement.Text = strNewText;
            if (oWord.Selection.Find.Execute(
                ref missing, ref missing, ref missing, ref missing, ref 
missing,
                ref missing, ref missing, ref missing, ref missing, ref 
missing,
                ref replaceAll, ref missing, ref missing, ref missing, 
ref missing))
            {
                return true;
            }
            return false;
        }
        #endregion
        #region - 表格操作 -
        public bool FindTable(string bookmarkTable)
        {
            try
            {
                object bkObj = bookmarkTable;
                if (oWord.ActiveDocument.Bookmarks.Exists(bookmarkTable) == true)
                {
                    oWord.ActiveDocument.Bookmarks.get_Item(ref bkObj).Select();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void MoveNextCell()
        {
            try
            {
                Object unit = 
Microsoft.Office.Interop.Word.WdUnits.wdCell;
                Object count = 1;
                oWord.Selection.Move(ref unit, ref count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SetCellValue(string value)
        {
            try
            {
                oWord.Selection.TypeText(value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void MoveNextRow()
        {
            try
            {
                Object extend = Microsoft.Office.Interop.Word.WdMovementType.wdExtend;
                Object unit = Microsoft.Office.Interop.Word.WdUnits.wdCell;
                Object count = 1;
                oWord.Selection.MoveRight(ref unit, ref count, ref extend);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //表格操作结束
        #endregion 
        #region 填充书签
        /// <summary>  
        /// 填充书签  
        /// </summary>  
        /// <param name="bookmark">书签</param>  
        /// <param name="value">值</param>  
        public void bookmarkReplace(string bookmark, string value)
        {
            try
            {
                object bkObj = bookmark;
                if (oWord.ActiveDocument.Bookmarks.Exists(bookmark) == 
true)
                {
                    oWord.ActiveDocument.Bookmarks.get_Item(ref 
bkObj).Select();
                }
                else return;
                oWord.Selection.TypeText(value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// rgb转换函数
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        int RGB(int r, int g, int b)
        {
            return ((b << 16) | (ushort)(((ushort)g << 8) | r));
        }
        Color RGBToColor(int color)
        {
            int r = 0xFF & color;
            int g = 0xFF00 & color;
            g >>= 8;
            int b = 0xFF0000 & color;
            b >>= 16;
            return Color.FromArgb(r, g, b);
        }
    }
}
/*
（1） 插入图片后，如果后面不再插入内容，则图片会包含；如果继续插入内容，则图片会被程序删除。解决方法是：
       插入图片后，执行跳转，光标转移到图片后面，再插入东西，就可以了。
            word.InsertImage("d://111.jpg",400.0f,300.0f);    //插入图片   
            word.GoToTheEnd();
 （2）
oWord.ActiveWindow.View.SeekView = 
Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageHeader; //页眉 
oWord.ActiveWindow.View.SeekView = 
Microsoft.Office.Interop.Word.WdSeekView.wdSeekCurrentPageFooter; //页脚 
oWord.ActiveWindow.View.SeekView = 
Microsoft.Office.Interop.Word.WdSeekView.wdSeekMainDocument; //转到正文
object page = Microsoft.Office.Interop.Word.WdFieldType.wdFieldPage; 
//当前页码
object pages = 
Microsoft.Office.Interop.Word.WdFieldType.wdFieldNumPages;  //总页码
 * 
*/
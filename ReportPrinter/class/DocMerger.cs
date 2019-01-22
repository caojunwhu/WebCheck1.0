using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Word;
using System.Reflection;
using System.IO;
using System.Diagnostics;
namespace Eipsoft.Common
{
    ///
    /// Word�ĵ��ϲ���
    ///
    public class WordDocumentMerger
    {
        private ApplicationClass objApp = null;
        private Document objDocLast = null;
        private Document objDocBeforeLast = null;
        public WordDocumentMerger()
        {
            objApp = new ApplicationClass();
        }
        #region ���ļ�
        private void Open(string tempDoc)
        {
            object objTempDoc = tempDoc;
            object objMissing = System.Reflection.Missing.Value;

            objDocLast = objApp.Documents.Open(
                 ref objTempDoc,    //FileName
                 ref objMissing,   //ConfirmVersions
                 ref objMissing,   //ReadOnly
                 ref objMissing,   //AddToRecentFiles
                 ref objMissing,   //PasswordDocument
                 ref objMissing,   //PasswordTemplate
                 ref objMissing,   //Revert
                 ref objMissing,   //WritePasswordDocument
                 ref objMissing,   //WritePasswordTemplate
                 ref objMissing,   //Format
                 ref objMissing,   //Enconding
                 ref objMissing,   //Visible
                 ref objMissing,   //OpenAndRepair
                 ref objMissing,   //DocumentDirection
                 ref objMissing,   //NoEncodingDialog
                 ref objMissing    //XMLTransform
                 );

            objDocLast.Activate();
        }
        #endregion

        #region �����ļ������ģ��
        private void SaveAs(string outDoc)
        {
            object objMissing = System.Reflection.Missing.Value;
            object objOutDoc = outDoc;
            objDocLast.SaveAs(
              ref objOutDoc,      //FileName
              ref objMissing,     //FileFormat
              ref objMissing,     //LockComments
              ref objMissing,     //PassWord    
              ref objMissing,     //AddToRecentFiles
              ref objMissing,     //WritePassword
              ref objMissing,     //ReadOnlyRecommended
              ref objMissing,     //EmbedTrueTypeFonts
              ref objMissing,     //SaveNativePictureFormat
              ref objMissing,     //SaveFormsData
              ref objMissing,     //SaveAsAOCELetter,
              ref objMissing,     //Encoding
              ref objMissing,     //InsertLineBreaks
              ref objMissing,     //AllowSubstitutions
              ref objMissing,     //LineEnding
              ref objMissing      //AddBiDiMarks
              );
        }
        #endregion

        #region ѭ���ϲ�����ļ������ƺϲ��ظ����ļ���
        ///
        /// ѭ���ϲ�����ļ������ƺϲ��ظ����ļ���
        ///
        /// ģ���ļ�
        /// ��Ҫ�ϲ����ļ�
        /// �ϲ��������ļ�
        public void CopyMerge(string tempDoc, string[] arrCopies, string outDoc)
        {
            object objMissing = Missing.Value;
            object objFalse = false;
            object objTarget = WdMergeTarget.wdMergeTargetSelected;
            object objUseFormatFrom = WdUseFormattingFrom.wdFormattingFromSelected;
            try
            {
                //��ģ���ļ�
                Open(tempDoc);
                foreach (string strCopy in arrCopies)
                {
                    objDocLast.Merge(
                      strCopy,                //FileName   
                      ref objTarget,          //MergeTarget
                      ref objMissing,         //DetectFormatChanges
                      ref objUseFormatFrom,   //UseFormattingFrom
                      ref objMissing          //AddToRecentFiles
                      );
                    objDocBeforeLast = objDocLast;
                    objDocLast = objApp.ActiveDocument;
                    if (objDocBeforeLast != null)
                    {
                        objDocBeforeLast.Close(
                          ref objFalse,     //SaveChanges
                          ref objMissing,   //OriginalFormat
                          ref objMissing    //RouteDocument
                          );
                    }
                }
                //���浽����ļ�
                SaveAs(outDoc);
                foreach (Document objDocument in objApp.Documents)
                {
                    objDocument.Close(
                      ref objFalse,     //SaveChanges
                      ref objMissing,   //OriginalFormat
                      ref objMissing    //RouteDocument
                      );
                }
            }
            finally
            {
                objApp.Quit(
                  ref objMissing,     //SaveChanges
                  ref objMissing,     //OriginalFormat
                  ref objMissing      //RoutDocument
                  );
                objApp = null;
            }
        }
        ///
        /// ѭ���ϲ�����ļ������ƺϲ��ظ����ļ���
        ///
        /// ģ���ļ�
        /// ��Ҫ�ϲ����ļ�
        /// �ϲ��������ļ�
        public void CopyMerge(string tempDoc, string strCopyFolder, string outDoc)
        {
            string[] arrFiles = Directory.GetFiles(strCopyFolder);
            CopyMerge(tempDoc, arrFiles, outDoc);
        }
        #endregion

        #region ѭ���ϲ�����ļ�������ϲ��ļ���
        ///
        /// ѭ���ϲ�����ļ�������ϲ��ļ���
        ///
        /// ģ���ļ�
        /// ��Ҫ�ϲ����ļ�
        /// �ϲ��������ļ�
        public void InsertMerge(string tempDoc, string[] arrCopies, string outDoc)
        {
            object objMissing = Missing.Value;
            object objFalse = false;
            object confirmConversion = false;
            object link = false;
            object attachment = false;
            try
            {
                //��ģ���ļ�
                Open(tempDoc);
                foreach (string strCopy in arrCopies)
                {
                    objApp.Selection.InsertFile(
                        strCopy,
                        ref objMissing,
                        ref confirmConversion,
                        ref link,
                        ref attachment
                        );
                }
                //���浽����ļ�
                SaveAs(outDoc);
                foreach (Document objDocument in objApp.Documents)
                {
                    objDocument.Close(
                      ref objFalse,     //SaveChanges
                      ref objMissing,   //OriginalFormat
                      ref objMissing    //RouteDocument
                      );
                }
            }
            finally
            {
                objApp.Quit(
                  ref objMissing,     //SaveChanges
                  ref objMissing,     //OriginalFormat
                  ref objMissing      //RoutDocument
                  );
                objApp = null;
            }
        }
        ///
        /// ѭ���ϲ�����ļ�������ϲ��ļ���
        ///
        /// ģ���ļ�
        /// ��Ҫ�ϲ����ļ�
        /// �ϲ��������ļ�
        public void InsertMerge(string tempDoc, string strCopyFolder, string outDoc)
        {
            string[] arrFiles = Directory.GetFiles(strCopyFolder);
            InsertMerge(tempDoc, arrFiles, outDoc);
        }
        #endregion
    }
}
using NetOffice.ExcelApi;
using NetOffice.ExcelApi.Enums;
using System;

namespace GenioServer.PreViewer.ConvertersCS
{
    class ExcelOrCSVToPDF
    {
        public static string PDFEXTENSION = ".pdf";

        public static string ConvertDocument(string inputPath, string outputPath, string docName, string nameSHA1, bool pdfa = false)
        {
            string sourceDocument = inputPath + "\\" + docName;
            string outPutDocName = nameSHA1 + PDFEXTENSION;
            string outPutDocument = outputPath + "\\" + outPutDocName.Replace("&", "");

            using (Application excelApplication = new Application())
            {
                if (pdfa)
                {
                    Microsoft.Win32.RegistryKey key_PDF_A = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Office\\" + excelApplication.Application.Version + "\\Common\\FixedFormat", true);
                    //int old_value = (int)key_PDF_A.GetValue("LastIso19005-1");
                    key_PDF_A.SetValue("LastISO19005-1", true, Microsoft.Win32.RegistryValueKind.DWord);
                }
                excelApplication.DisplayAlerts = false;

                using (Workbook excelDoc = excelApplication.Workbooks.Open(sourceDocument))
                {
                    XlFixedFormatType paramExportFormat = XlFixedFormatType.xlTypePDF;
                    XlFixedFormatQuality paramExportQuality = XlFixedFormatQuality.xlQualityStandard;
                    excelDoc.ExportAsFixedFormat(paramExportFormat,
                        outPutDocument, paramExportQuality,
                        true, false,
                        Type.Missing, Type.Missing, true, Type.Missing);
                }
                excelApplication.Quit();
            }

            return outPutDocName;
        }
    }
}

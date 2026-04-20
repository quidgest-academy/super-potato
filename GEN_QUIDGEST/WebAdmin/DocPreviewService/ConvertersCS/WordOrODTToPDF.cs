using NetOffice.WordApi;
using NetOffice.WordApi.Enums;
using System;
using System.IO;

namespace GenioServer.PreViewer.ConvertersCS
{
    public class CvLog
    {
        private static string _tempLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", "Log.txt");

        public static void Log(string message)
        {
            using (StreamWriter outputFile = new StreamWriter(_tempLog, true))
                outputFile.WriteLine(message);
        }
    }
    public class WordOrODTToPDF
    {
        public static string PDFEXTENSION = ".pdf";

        public static string ConvertDocument(string inputPath, string outputPath, string docName, string nameSHA1,bool pdfa = false)
        {
            string sourceDocument = inputPath + "\\" + docName;
            string outPutDocName = nameSHA1 + PDFEXTENSION;
            string outPutDocument = outputPath + "\\" + outPutDocName.Replace("&", "");

            //foreach (var x in NetOffice.Factory.Assemblies)
            //    Console.WriteLine(x.ComponentGuid);

            CvLog.Log(sourceDocument);
            CvLog.Log(string.Format("UserInteractive: {0}", Environment.UserInteractive));

            using (Application wordApplication = new Application())
            {
                wordApplication.DisplayAlerts = WdAlertLevel.wdAlertsNone;

                Console.WriteLine("Converting " + docName);
                using (Document wordDocument = wordApplication.Documents.Open(sourceDocument))
                {
                    if(wordDocument == null)
                        CvLog.Log("Unable to open word document");

                    wordDocument.ExportAsFixedFormat(outPutDocument, WdExportFormat.wdExportFormatPDF,
                        false, WdExportOptimizeFor.wdExportOptimizeForPrint, 0, 1, 1, 0, false, true, 0, true, true, pdfa);
                }
                Console.WriteLine("Finished " + docName);
                wordApplication.Quit(false);
            }

            return outPutDocName;
        }
    }
}
using NetOffice.VisioApi;
using NetOffice.VisioApi.Enums;

namespace GenioServer.PreViewer.ConvertersCS
{
    public class VisioToPdf
    {
        public static string PDFEXTENSION = ".pdf";

        public static string ConvertDocument(string inputPath, string outputPath, string docName, string nameSHA1,bool pdfa= false)
        {
            string sourceDocument = inputPath + "\\" + docName;
            string outPutDocName = nameSHA1 + PDFEXTENSION;
            string outPutDocument = outputPath + "\\" + outPutDocName.Replace("&", "");

            using (IVApplication visioApplication = new Application())
            {
                visioApplication.AlertResponse = 3; //http://msdn.microsoft.com/en-us/library/office/aa215114(v=office.11).aspx

                using (IVDocument visioDocument = visioApplication.Documents.Open(sourceDocument))
                {
                    visioDocument.ExportAsFixedFormat(VisFixedFormatTypes.visFixedFormatPDF, outPutDocument,
                        VisDocExIntent.visDocExIntentScreen, VisPrintOutRange.visPrintAll, 1, -1, false, true, false, true, pdfa);
                }

                visioApplication.Quit();
            }

            return outPutDocName;
        }
    }
}
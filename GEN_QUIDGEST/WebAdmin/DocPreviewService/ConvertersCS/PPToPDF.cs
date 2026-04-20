using NetOffice;
using NetOffice.OfficeApi.Enums;
using NetOffice.PowerPointApi;
using NetOffice.PowerPointApi.Enums;

namespace GenioServer.PreViewer.ConvertersCS
{
    public class PPToPDF
    {
        public static string PDFEXTENSION = ".pdf";

        public static string ConvertDocument(string inputPath, string outputPath, string docName, string nameSHA1,bool pdfa = false)
        {
            DebugConsole.Mode = ConsoleMode.MemoryList;
            string sourceDocument = inputPath + "\\" + docName;
            string outPutDocName = nameSHA1 + PDFEXTENSION;
            string outPutDocument = outputPath + "\\" + outPutDocName.Replace("&", "");

            using (Application powerApplication = new Application())
            {
                powerApplication.DisplayAlerts = PpAlertLevel.ppAlertsNone;

                using (Presentation presentation = powerApplication.Presentations.Open(sourceDocument, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse))
                {
                    //Este codigo é preciso pois exists um bug no power point que inicia o primeiro slide a 0 quando de facto começa em 1
                    PrintRanges ranges = presentation.PrintOptions.Ranges;
                    PrintRange range = ranges.Add(1, 1);
                    //-----

                    presentation.ExportAsFixedFormat(outPutDocument,
                        PpFixedFormatType.ppFixedFormatTypePDF,
                            PpFixedFormatIntent.ppFixedFormatIntentPrint,
                            MsoTriState.msoTrue,
                            PpPrintHandoutOrder.ppPrintHandoutHorizontalFirst,
                            PpPrintOutputType.ppPrintOutputSlides,
                            MsoTriState.msoTrue,
                            range,
                            PpPrintRangeType.ppPrintAll,
                            null,
                            true,
                            true,
                            true,
                            false,
                            pdfa,
                            null);
                }

                powerApplication.Quit();
            }

            return outPutDocName;
        }
    }
}
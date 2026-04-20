using NetOffice;
using NetOffice.MSProjectApi;
using NetOffice.MSProjectApi.Enums;
using System;

namespace GenioServer.PreViewer.ConvertersCS
{
    public class ProjectToPdf
    {
        public static string PDFEXTENSION = ".pdf";

        public static string ConvertDocument(string inputPath, string outputPath, string docName, string nameSHA1,bool pdfa = false)
        {
            DebugConsole.Mode = ConsoleMode.MemoryList;
            string sourceDocument = inputPath + "\\" + docName;
            string outPutDocName = nameSHA1 + PDFEXTENSION;
            string outPutDocument = outputPath + "\\" + outPutDocName.Replace("&", "");

            using (Application projectApplication = new Application())
            {
                if (pdfa)
                {
                    Microsoft.Win32.RegistryKey key_PDF_A = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Office\\" + projectApplication.Application.Version + "\\Common\\FixedFormat", true);
                    //int old_value = (int)key_PDF_A.GetValue("LastIso19005-1");
                    key_PDF_A.SetValue("LastISO19005-1", true, Microsoft.Win32.RegistryValueKind.DWord);
                }

                projectApplication.DisplayAlerts = false;
                projectApplication.Visible = false;

                projectApplication.FileOpen(sourceDocument, true, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, PjPoolOpen.pjPoolReadOnly, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing);

                projectApplication.DocumentExport(outPutDocument, PjDocExportType.pjPDF, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                projectApplication.Quit(PjSaveType.pjDoNotSave);
            }

            return outPutDocName;
        }
    }
}
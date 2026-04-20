using System;
using System.IO;
using System.Security.Cryptography;

namespace GenioServer.PreViewer
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Microsoft does not currently recommend, and does not support, Automation of Microsoft Office applications from any unattended, 
    /// non-interactive client application or component (including ASP, ASP.NET, DCOM, and NT Services), 
    /// because Office may exhibit unstable behavior and/or deadlock when Office is run in this environment.
    /// https://support.microsoft.com/en-au/topic/considerations-for-server-side-automation-of-office-48bcfe93-8a89-47f1-0bce-017433ad79e2
    /// 
    /// Suggested alternatives to document conversion to pdf are:
    /// Sharepoint Server: https://learn.microsoft.com/en-us/previous-versions/office/developer/sharepoint-2010/ee558830(v=office.14)
    /// Spire.Doc: https://www.e-iceblue.com/Buy/Spire.Doc.html
    /// Adobe online conversion: https://www.adobe.com/acrobat/how-to/convert-word-to-pdf.html
    /// </remarks>
    class PreViewerMain
    {
        private const string PDFEXTENSION = ".pdf";
        private const string HTMLEXTENSION = ".html";
        //Word extensions
        private const string WordExtension1 = ".docx";
        private const string WordExtension2 = ".doc";
        private const string ODTExtension = ".odt";
        private const string RTFExtension = ".rtf";
        //Exel extensions
        private const string ExcelExtension1 = ".xls";
        private const string ExcelExtension2 = ".xlsx";
        private const string CommaSeparatedValues = ".csv";
        private const string ExcelExtension3 = ".ods";
        
        //Power point extensions
        private const string PowerPointExtension1 = ".ppt";
        private const string PowerPointExtension2 = ".pptx";
        private const string PowerPointExtension3 = ".odp";
        //Publish extensions
        private const string PublishExtension = ".pub";
        //Project extensions
        private const string ProjectExtension = ".mpp";
        //Visio extensions
        private const string VisioExtension = ".vsd";
        //Text file Extensions
        private const string TextExtension = ".txt";

        private const int MAXPDFDOC = 100;
        private const int MAXMAILS = 1;

        static PreViewerMain()
        {
            NetOffice.Factory.Initialize();
        }

        public static string ConvertDocument(string documentPath, bool pdfa = false)
        {
            string docExt = Path.GetExtension(documentPath);
            string docName = Path.GetFileName(documentPath);

            string filePath = Path.GetDirectoryName(documentPath);
            string tmpPath = filePath; //A pasta to onde irei converter será a mesma mas deixo com as variaveis separadas caso seja preciso por numa outra pasta

            string SHA1 = GetDocSHA1(documentPath);
            string nameSHA1 = Path.GetFileNameWithoutExtension(docName);
            if (!docName.Contains(SHA1))
                nameSHA1 += " " + SHA1;

            string outPutFileName = "";

            if (!InCache(SHA1, tmpPath))
            {
                switch (docExt)
                {
                    case WordExtension1:
                    case WordExtension2:
                    case ODTExtension:
                    case RTFExtension:
                        outPutFileName = ConvertersCS.WordOrODTToPDF.ConvertDocument(filePath, tmpPath, docName, nameSHA1, pdfa);
                        break;
                    case ExcelExtension1:
                    case ExcelExtension2:
                    case ExcelExtension3:
                    case CommaSeparatedValues:
                        outPutFileName = ConvertersCS.ExcelOrCSVToPDF.ConvertDocument(filePath, tmpPath, docName, nameSHA1, pdfa);
                        break;
                    case PowerPointExtension1:
                    case PowerPointExtension2:
                    case PowerPointExtension3:
                        outPutFileName = ConvertersCS.PPToPDF.ConvertDocument(filePath, tmpPath, docName, nameSHA1, pdfa);
                        break;
                    case ProjectExtension:
                        outPutFileName = ConvertersCS.ProjectToPdf.ConvertDocument(filePath, tmpPath, docName, nameSHA1, pdfa);
                        break;
                    case VisioExtension:
                        outPutFileName = ConvertersCS.VisioToPdf.ConvertDocument(filePath, tmpPath, docName, nameSHA1, pdfa);
                        break;
                    case TextExtension:
                        outPutFileName = ConvertersCS.TXTToPDF.ConvertTXTToPDF(filePath, tmpPath, docName, nameSHA1, pdfa);
                        break;
                }
            }
            else
                outPutFileName = cacheSynchronization(SHA1, tmpPath, nameSHA1, docName, docExt);

            //verifica se existem ficheiros a mais e apaga os
            checkDelete(tmpPath);

            return outPutFileName;
        }

        public static void checkDelete(string tmpFolder)
        {

            string[] files = Directory.GetFiles( tmpFolder, "*" + PDFEXTENSION, SearchOption.TopDirectoryOnly);

            if (files.Length > MAXPDFDOC) //verifica se ha pdf a +... é apagado o ultimo a ser acedido
            {
                FileInfo lastAccessFile = null;
                DateTime lastAccessFileDate = DateTime.Now;

                foreach (string s in files)
                {
                    FileInfo fi = null;
                    fi = new System.IO.FileInfo(s);

                    if (DateTime.Compare(fi.LastAccessTime, lastAccessFileDate) < 0)
                    {
                        lastAccessFile = fi;
                        lastAccessFileDate = fi.LastAccessTime;
                    }
                }

				if(lastAccessFile != null)
					File.Delete(lastAccessFile.FullName);
            }
        }

        private static Boolean InCache(string SHA1, string tmpPath)
        {
            Boolean found = false;
            string nameToSearch;

            nameToSearch = "*" + SHA1 + PDFEXTENSION;
            string[] files = Directory.GetFiles(tmpPath, nameToSearch, SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    if (file.Contains("~$"))
                    {
                        try { File.Delete(file); } catch { }
                    }
                    else
                        found = true;
                }
            }
            return found;
        }

        private static string GetDocSHA1(string docPath)
        {
            byte[] hashBytes;
            using (var inputFileStream = File.Open(docPath, System.IO.FileMode.Open))
            {
                var sha1 = SHA1.Create();
                hashBytes = sha1.ComputeHash(inputFileStream);
            }
            string hex = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            return hex;
        }

        /* Este metodo é to renomear o file convertido que contem o mesmo SHA1 
         e atribuir o name do novo file to o user não pensar que esta
         a abrir o documento errado e to verificar o tipo de exportação */
        private static string cacheSynchronization(string sha1, string tmpFilesPath, string nameSHA1, string docName, string originalExtension)
        {
            DirectoryInfo searchDirectory = new DirectoryInfo(tmpFilesPath);

            string folderfinalName;
            string returnName;
            
            FileInfo[] filesInDir = searchDirectory.GetFiles("*" + sha1 + PDFEXTENSION);

            returnName = nameSHA1 + PDFEXTENSION;
            folderfinalName = tmpFilesPath +"\\"+ returnName;

            filesInDir[0].MoveTo(folderfinalName);
            
            return returnName;
        }
    }
}

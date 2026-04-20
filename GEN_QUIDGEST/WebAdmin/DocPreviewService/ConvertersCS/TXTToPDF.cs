using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace GenioServer.PreViewer.ConvertersCS
{
    public class TXTToPDF
    {
        public static string PDFEXTENSION = ".pdf";

        public static string ConvertTXTToPDF(string pathInputTXT, string pathOutPutPDF, string docName, string nameSHA1,bool pdfa= false)
        {
            //Read the Data from Input File
            using (StreamReader rdr = new StreamReader(pathInputTXT + "\\" + docName))
            {
                string outPutDocName = nameSHA1 + PDFEXTENSION;

                //Novo pdf name
                string paramExportFilePath = pathOutPutPDF + "\\" + outPutDocName.Replace("&", "");

                //Create a New instance on Document Class
                Document doc = new Document();
                //Create a New instance of PDFWriter Class for Output File
                PdfWriter.GetInstance(doc, new FileStream(paramExportFilePath, FileMode.Create));

                //Open the Document
                doc.Open();

                //Add the content of Text File to PDF File
                doc.Add(new Paragraph(rdr.ReadToEnd()));

                //Close the Document
                doc.Close();

                return outPutDocName;
            }
        }
    }
}
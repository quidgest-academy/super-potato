using Nancy;
using Nancy.Responses;
using org.bouncycastle.util.encoders;
using System;
using System.IO;
using System.Linq;


namespace DocPreviewService
{
    public class DocConvertModule : NancyModule
    {
        readonly string _tempDir;
        public DocConvertModule()
        {
            _tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");

            if (!Directory.Exists(_tempDir))
                Directory.CreateDirectory(_tempDir);

            Get("/", Index);
            Post("/Convert", Convert);            
        }

        //this is a very naif way to solve the concurrency problem of multiple conversion calls to the same file
        //a more robust solution would involve a producer consumer thread, or a per-file lock.
        private static object convertLock = new object();

        public object Index(dynamic _args)
        {
            return new HtmlResponse(contents: (s) =>
            {
                var w = new StreamWriter(s);
                w.Write(@"<!DOCTYPE html>
<html lang=""en"">
  <body>
    <div>Convert service running</div>
    <hr/>
    <div>This is a test form:</div>
    <form method=""post"" enctype=""multipart/form-data"" action=""/Convert"">
      <input type=""file"" id=""myFile"" name=""filename"">
      <input type=""submit"">
    </form>
  </body>
</html>
");
                w.Flush();
            });
        }

        public object Convert(dynamic _args)
        {
            try
            {
                var file = this.Request.Files.FirstOrDefault();
                if (file == null)
                    return new TextResponse("File upload is mandatory to call this method")
                    {
                        StatusCode = HttpStatusCode.BadRequest
                    };

                string tempFile = CreateTempFilename(file.Name);
                using (var outfile = new FileStream(tempFile, FileMode.Create))
                    file.Value.CopyTo(outfile);

                string docconvert;
                lock (convertLock)
                {
                    docconvert = GenioServer.PreViewer.PreViewerMain.ConvertDocument(tempFile);
                }
                File.Delete(tempFile);

                string pathdocconvert = Path.Combine(_tempDir, docconvert);
                var response = new StreamResponse(
                    () => new FileStream(pathdocconvert, FileMode.Open),
                    MimeTypes.GetMimeType(pathdocconvert));
                //return response.AsAttachment(docconvert);
                return response;
            }
            catch (Exception ex)
            {
                //using (StreamWriter outputFile = new StreamWriter("temp/Log.txt", true))
                //{
                //    outputFile.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                //}

                return new TextResponse(ex.Message + Environment.NewLine + ex.StackTrace)
                {
                    //StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        private string CreateTempFilename(string srcfilename)
        {
            Random randomNumber = new Random();
            int suf = randomNumber.Next();
            int ponto = srcfilename.LastIndexOf(".");
            string name = srcfilename.Substring(0, ponto);
            string extension = srcfilename.Substring(ponto);
            string fileName = Path.Combine(_tempDir, name + suf.ToString() + extension);

            while (File.Exists(fileName))
            {
                suf = randomNumber.Next();
                fileName = Path.Combine(_tempDir, name + suf.ToString() + extension);
            }

            return fileName;
        }
    }

}

using System.Net;
using Microsoft.AspNetCore.Mvc;
using CSGenio.persistence;
using CSGenio.framework;

namespace Administration.Controllers
{
    public class ControllerBase : Controller
    {
        private string _year;
        public string CurrentYear
        {
            get
            {
                if (_year == null) _year = GetYearFromRoute();
                return _year;
            }
        }

        /// <summary>
        /// Returns the application year taking into account the value in the route
        /// </summary>
        /// <returns>String with the year</returns>
        public string GetYearFromRoute()
        {
            string year = Configuration.DefaultYear;
            if (HttpContext.Request.RouteValues.TryGetValue("system", out var value))
            {
                year = value?.ToString() ?? Configuration.DefaultYear;

                // Ensure that the year exists in the configuration
                if (!Configuration.Years.Contains(year))
                    year = Configuration.DefaultYear;
            }
            return year;
        }

        /// <summary>
        /// Returns the exported file to download
        /// </summary>
        /// <param name="id">File ID</param>
        /// <param name="type">File type</param>
        /// <returns>Exported file</returns>
        [HttpGet]
        [ActionName("downloadExportFile")]
        public HttpResponseMessage downloadExportFile(string id, string type)
        {
            byte[] file = (byte[])QCache.Instance.ExportFiles.Get("webadmin_exportfile_" + id);
            var dataStream = new MemoryStream(file);
            QCache.Instance.ExportFiles.Invalidate("webadmin_exportfile_" + id);

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StreamContent(dataStream);
            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = id;

            switch (type)
            {
                case "pdf":
                    httpResponseMessage.Content.Headers.ContentType = 
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    break;
                case "xlsx":
                    httpResponseMessage.Content.Headers.ContentType = 
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    break;
                case "ods":
                    httpResponseMessage.Content.Headers.ContentType = 
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.oasis.opendocument.spreadsheet");
                    break;
                case "csv":
                    httpResponseMessage.Content.Headers.ContentType = 
                        new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
                    break;
                case "xml":
                    httpResponseMessage.Content.Headers.ContentType = 
                        new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
                    break;
                default:
                    httpResponseMessage.Content.Headers.ContentType = 
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    break;
            }

            return httpResponseMessage;
        }

        protected List<string> GetModelStateErrors()
        {
            return (from item in ModelState
                    where item.Value.Errors.Any()
                    select item.Value.Errors[0].ErrorMessage).ToList();
        }

        [NonAction]
        public string FromQuery(string key)
        {
            return Request.Query[key];
        }

        public PersistentSupport GetPersistentSupport()
        {
            return PersistentSupport.getPersistentSupport(CurrentYear);
        }
    }
}
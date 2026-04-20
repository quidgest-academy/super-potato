using CSGenio.persistence;
using System;
using System.Collections.Generic;

namespace CSGenio.framework
{
    public class ProgressReport
    {
        public string Locstring { get; set; }
        public double Percent { get; set; }
        public bool Show { get; set; }
        public string Success { get; set; }
        public string Message { get; set; }
        public ErrorHandling Errors { get; set; }
        public string Id { get; set; }
        public bool Finished { get; set; }

        public void Report(string s, double i = -1, bool show = true, string suc = null, string msg = null, ErrorHandling errors = null, string id = null)
        {
            if (s != null)
                Locstring = s;

            if(i >= 0)
                Percent = i;

            this.Show = show;
            this.Success = suc;
            this.Message = msg;
            this.Errors = errors;
            this.Id = id;
            this.Finished = false;
        }

    }

    public class ErrorHandling
    {
        public List<string> ErrorLog { get; set; }

        public string ErrorResponse { get; set; }

        public ErrorHandling()
        {
            this.ErrorLog = new List<string>();
        }
    }
}
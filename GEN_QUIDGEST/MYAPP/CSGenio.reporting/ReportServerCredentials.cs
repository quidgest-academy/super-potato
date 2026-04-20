#if NETFRAMEWORK
using Microsoft.Reporting.WebForms;
#else
using Microsoft.Reporting.NETCore;
#endif

namespace CSGenio.reporting
{
    public class ReportServerCredentials : IReportServerCredentials
    {
        private string _userName;
        private string _password;
        private string _domain;

        public ReportServerCredentials(string userName, string password, string domain)
        {
            _userName = userName;
            _password = password;
            _domain = domain;
        }

        public System.Security.Principal.WindowsIdentity ImpersonationUser
        {
            get
            {
                // Use default identity.
                return null;
            }
        }

        public System.Net.ICredentials NetworkCredentials
        {
            get
            {
                // Use default identity.
                return new System.Net.NetworkCredential(_userName, _password, _domain);
            }
        }

        public bool GetFormsCredentials(out System.Net.Cookie authCookie, out string user, out string password, out string authority)
        {
            // Do not use forms credentials to authenticate.
            authCookie = null;
            user = password = authority = null;
            return false;
        }
    }
}

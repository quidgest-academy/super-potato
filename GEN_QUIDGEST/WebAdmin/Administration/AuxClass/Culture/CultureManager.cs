using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Administration.AuxClass.Culture
{
    public class CultureManager : DelegatingHandler
    {

        const string EngCultureName = "en-us";

        public static CultureInfo DefaultCulture
        {
            get
            {
                if(SupportedCultures == null)
                    InitializeSupportedCultures();
                    
                return SupportedCultures[EngCultureName];
            }
        }

        public static Dictionary<string, CultureInfo> SupportedCultures { get; set; }


        static void AddSupportedCulture(string name)
        {
            SupportedCultures.Add(name, CultureInfo.CreateSpecificCulture(name));
        }

        static void InitializeSupportedCultures()
        {
            SupportedCultures = new Dictionary<string, CultureInfo>();
            AddSupportedCulture(EngCultureName);
        }

        public CultureManager()
        {
            InitializeSupportedCultures();
        }

        private bool SetHeaderIfAcceptLanguageMatchesSupportedLanguage(HttpRequestMessage request)
        {
            var headers = request.Headers;

            headers.TryGetValues("culture", out IEnumerable<string> _curCulture);
            var curCulture = _curCulture.FirstOrDefault()?.ToLower() ?? string.Empty;

            if (SupportedCultures.ContainsKey(curCulture))
            {
                SetCulture(request, curCulture);
                return true;
            }

            return false;
        }

        private bool SetHeaderIfGlobalAcceptLanguageMatchesSupportedLanguage(HttpRequestMessage request)
        {
            foreach (var lang in request.Headers.AcceptLanguage)
            {
                var globalLang = lang.Value.Substring(0, 2);
                if (SupportedCultures.Any(t => t.Key.StartsWith(globalLang)))
                {
                    SetCulture(request, SupportedCultures.First(i => i.Key.StartsWith(globalLang)).Value.Name);
                    return true;
                }
            }

            return false;
        }

        private void SetCulture(HttpRequestMessage request, string lang)
        {
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(lang));
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!SetHeaderIfAcceptLanguageMatchesSupportedLanguage(request))
            {
                // Whoops no localization found. Lets try Globalisation
                if (!SetHeaderIfGlobalAcceptLanguageMatchesSupportedLanguage(request))
                {
                    // no global or localization found
                    SetCulture(request, DefaultCulture.Name);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}

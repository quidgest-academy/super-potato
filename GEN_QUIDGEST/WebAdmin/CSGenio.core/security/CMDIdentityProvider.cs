using CSGenio;
using CSGenio.core.di;
using CSGenio.framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web;

namespace GenioServer.security
{
    [CredentialProvider(typeof(TokenCredential))]
    [Description("Establishes identity using citizen Digital Mobile Key.")]
    [DisplayName("Digital Mobile Key")]
    public class CMDIdentityProvider : BaseIdentityProvider
    {
        /// <summary>
        /// Authority are the url use when making OpenIdConnect calls.
        /// </summary>
        [SecurityProviderOption()]
        [Description("Authority is the url to use when making OpenIdConnect calls")]
        public string Authority { get; set; }
        /// <summary>
        /// OAuth 2.0 Client Identifier valid at the Authorization Server.
        /// </summary>
        [SecurityProviderOption()]
        [Description("OAuth 2.0 Client Identifier valid at the Authorization Server")]
        public string ClientId { get; set; }
        /// <summary>
        /// OAuth 2.0 Response Type value that determines the authorization processing flow to be used, including what parameters are returned from the endpoints used. When using the Authorization Code Flow, this value is code.
        /// </summary>
        public List<string> ResponseType { get; set; }
        public string ResponseMode { get; set; }
        /// <summary>
        /// Gets the list of permissions to request.
        /// </summary>
        [JsonIgnore]
        public List<string> Scope { get; private set; }

        /// <summary>
        /// API adress to get data associated with authenticated token
        /// </summary>
        [SecurityProviderOption()]
        [Description("API adress to get data associated with authenticated token")]
        public string DataAPI { get; set; }

        /// <summary>
        /// field used to connect to a database user Ex: "http://interop.gov.pt/MDC/Cidadao/NIF"
        /// </summary>
        [SecurityProviderOption()]
        [Description("Field reference to represent the unique user id: 'http://interop.gov.pt/MDC/Cidadao/NIF'")]
        public string UserIdField { get; set; }

        /// <summary>
        /// Gets the list of permissions to request (config).
        /// </summary>
        /// <remarks>
        ///Common scopes are
        ///http://interop.gov.pt/MDC/Cidadao/NIF,
		///http://interop.gov.pt/MDC/Cidadao/NomeCompleto,
        ///http://interop.gov.pt/MDC/Cidadao/NomeProprio,
		///http://interop.gov.pt/MDC/Cidadao/NomeApelido,
		///The NIC is exclusive to Portuguese citizens and is mandatory
		///which is why it gives an error when CMD is not Portuguese
        ///http://interop.gov.pt/MDC/Cidadao/NIC,
		///In order for the CMD to be compatible with foreign users
		///the following DocType and DocNationality and DocNumber properties have been added
		///which are used instead of the NIC when the CMD is not Portuguese
        ///http://interop.gov.pt/MDC/Cidadao/DocType,
        ///http://interop.gov.pt/MDC/Cidadao/DocNationality,
        ///http://interop.gov.pt/MDC/Cidadao/DocNumber
        /// </remarks>
        [SecurityProviderOption(optional: true)]
        [Description("List of user information properties to request")]
        public List<string> Scopes { get; set; }

        /// <inheritdoc/>
        public CMDIdentityProvider(IdentityProviderCfgEl config) : base(config)
        {
            ResponseType =
            [
                "token" //The ID_Token is represented as a JSON Web Token (JWT), which uses JSON Web Signature (JWS) and JSON Web Encryption (JWE) specifications enabling the claims to be digitally signed or MACed and/or encrypted.                
            ];
            ResponseMode = "form_post";

            //scope needs to request at least UserIdField for the matching to work
            Scope = new List<string>(Scopes ?? ["http://interop.gov.pt/MDC/Cidadao/NIF"]);
            if (!Scope.Contains(UserIdField))
                Scope.Add(UserIdField);
        }


        /// <inheritdoc/>
        public override bool HasRedirectLogin() => true;

        /// <inheritdoc/>
        public override string GetRedirectLoginUrl(string callback = null, string state = null)
        {
            if (string.IsNullOrEmpty(Authority) || string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(callback))
                throw new InvalidOperationException("It's mandatory to configure Authority, ClientId and CallbackPath options");

            var uriBuilder = new UriBuilder(Authority);
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["response_type"] = string.Join("+", ResponseType);
            parameters["response_mode"] = ResponseMode;
            if (Scope.Contains("openid")) //this parameter is mandatory when we have scope to made authentication using openid connect
                parameters["nonce"] = Guid.NewGuid().ToString("N").ToUpper(); //String value used to associate a Client session with an ID Token, and to mitigate replay attacks. 
            parameters["client_id"] = ClientId;
            parameters["redirect_uri"] = callback;
            parameters["scope"] = string.Join(" ", Scope);

            if (!string.IsNullOrEmpty(state))
                parameters["state"] = state;

            uriBuilder.Query = System.Web.HttpUtility.UrlDecode(parameters.ToString());

            if (Log.IsDebugEnabled)
                Log.Debug(string.Format("GetRedirectLoginUrl: {0}", uriBuilder.Uri.ToString()));

            return uriBuilder.Uri.ToString();
        }

        protected async Task<GenioIdentity> Validate(TokenCredential credential)
        {
            using (var http = GenioDI.HttpFactory.CreateClient("cmd"))
            {
                http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var resp = await http.GetAsync(DataAPI + "?token=" + credential.Token);
                resp.EnsureSuccessStatusCode();
                string jsonResult = await resp.Content.ReadAsStringAsync();

                var identity = new GenioIdentity();
                identity.AuthenticationType = this.GetType().Name;
                identity.IdProperty = GenioIdentityType.ExternalId;

                JArray jsonPayload = JArray.Parse(jsonResult);
                if (jsonPayload.Count == 0)
                    return null;

                foreach (JObject item in jsonPayload)
                {
                    string name = item.GetValue("name")?.ToString() ?? "";
                    string value = item.GetValue("value")?.ToString() ?? "";

                    identity.Claims[name] = value;
                    if (name.Equals(UserIdField, StringComparison.OrdinalIgnoreCase))
                        identity.Name = value;
                }

                if (string.IsNullOrEmpty(identity.Name))
                    return null;

                identity.IsAuthenticated = true;
                return identity;
            }
        }

        /// <inheritdoc/>
        public override GenioIdentity Authenticate(Credential credential)
        {
            if (credential is TokenCredential token)
                return Authenticate(token);

            return null;
        }

        protected GenioIdentity Authenticate(TokenCredential credential) 
        {
            return Validate(credential).Result;
        }
    }    
}

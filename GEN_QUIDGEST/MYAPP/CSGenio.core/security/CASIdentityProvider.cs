using CSGenio;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;

namespace GenioServer.security
{
    /// <summary>
    /// CAS protocol provider
    /// </summary>
    /// <remarks>
    /// https://jasigcas.readthedocs.io/en/latest/cas-server-documentation/protocol/CAS-Protocol-Specification.html
    /// https://apereo.github.io/cas/7.0.x/protocol/CAS-Protocol.html
    /// 
    /// For test machines use docker compose for fast setup:
    /// docker-compose.yml
    /// -------------------------------------------------------
    /// services:
    /// cas:
    /// image: apereo/cas:7.1.1
    /// container_name: casserver
    /// ports:
    ///   - "18080:18080"
    /// environment:
    ///   - SERVER_SSL_ENABLED=false
    ///   - SERVER_PORT=18080
    /// volumes:
    ///   - ./cas_config:/etc/cas
    /// -------------------------------------------------------
    /// cas_config/config/cas.properties
    /// -------------------------------------------------------
    /// cas.authn.accept.users=username::password
    /// cas.service-registry.core.init-from-json=true
    /// cas.service-registry.json.location=file:/etc/cas/services
    /// -------------------------------------------------------
    /// cas_config/services/localTest-1001.json
    /// -------------------------------------------------------
    /// {
    ///   "@class" : "org.apereo.cas.services.CasRegisteredService",
    ///   "serviceId" : "^https://localhost:5173/auth/CASLogin/Cas",
    ///   "name" : "localTest",
    ///   "id" : 1001,
    ///   "serviceTicketExpirationPolicy": {
    ///       "@class": "org.apereo.cas.services.DefaultRegisteredServiceServiceTicketExpirationPolicy",
    ///       "numberOfUses": 1,
    ///       "timeToLive": "100"
    ///     }
    /// }
    /// </remarks>

    [CredentialProvider(typeof(TokenCredential))]
    [Description("Establishes identity using Central Authentication Service protocol.")]
    [DisplayName("Central Authentication Service (CAS)")]
    public class CASIdentityProvider : BaseIdentityProvider
    {
        /// <summary> 
        /// Authority are the url use when making CAS calls.
        /// </summary>
        [SecurityProviderOption()]
        [Description("Authority is the url to use when making CAS calls")]
        public string Authority { get; set; }
        /// <summary>
        /// Attribute from callback returned from CAS Server to validate if user exist
        /// </summary>
        [SecurityProviderOption(optional: true)]
        [Description("Attribute from callback returned from CAS Server to validate if user exist")]
        public string AttribValidation { get; set; }

        /// <inheritdoc/>
        public CASIdentityProvider(IdentityProviderCfgEl config) : base(config)
        {
        }

        /// <inheritdoc/>
        public override bool HasRedirectLogin() => true;

        /// <inheritdoc/>
        public override string GetRedirectLoginUrl(string callback, string state = null)
        {
            if (string.IsNullOrEmpty(Authority) || string.IsNullOrEmpty(callback))
                throw new InvalidOperationException("It's mandatory to configure Authority, and callbackPath options");

            var uriBuilder = new UriBuilder(Authority);
            uriBuilder.Path += "/login";
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["service"] = callback;
            if (!string.IsNullOrEmpty(state))
                parameters["state"] = state;

            uriBuilder.Query = parameters.ToString();

            return uriBuilder.Uri.ToString();
        }


        /// <inheritdoc/>
        public override GenioIdentity Authenticate(Credential credential)
        {
            if (credential is TokenCredential token)
                return Authenticate(token);

            return null;
        }

        private GenioIdentity Authenticate(TokenCredential credential)
        {
            string usernameCred = "";

            if (string.IsNullOrEmpty(credential.Token))
                return null;

            //Find on response from CAS server the username
            XmlDocument xmlReturn = getResponseCAS(credential.Token, credential.OriginUrl);

            string tagname = string.IsNullOrEmpty(AttribValidation) ? "user" : AttribValidation;
            XmlNodeList userAttrib = xmlReturn.GetElementsByTagName("cas:" + tagname);
            if (userAttrib.Count > 0)
                usernameCred = userAttrib[0].InnerText;
            else
                return null;

            //CAS expects the username to be the same as the internal id
            return new()
            {
                Name = usernameCred,
                IdProperty = GenioIdentityType.InternalId,
                IsAuthenticated = true,
                AuthenticationType = GetType().Name,
            };
        }


        /// <summary>
        /// Method that validates the authentication of a user and returns all data related to the same
        /// https://stackoverflow.com/questions/4791794/client-to-send-soap-request-and-receive-response
        /// </summary>
        /// <param name="ticket">Validator identifier returned in the first CAS request</param>
		/// <param name="originUrl">Place of origin from which the order was placed, the CAS service validates that the ticket is valid from the place where the order was requested</param>
        /// <returns>The validation response xml with all authenticated user data</returns>
        private XmlDocument getResponseCAS(string ticket, string originUrl)
        {
            var uriVal = new UriBuilder(Authority);
            uriVal.Path += "/serviceValidate";

            var param = HttpUtility.ParseQueryString(string.Empty);
            param.Add("service", originUrl);
            param.Add("ticket", ticket);
            uriVal.Query = param.ToString();

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriVal.ToString());
            webRequest.Method = "GET";

            string soapResult = "";
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
            }

            //convert response from server to xmldocument
            XmlDocument xmlCASResult = null;
            if (!string.IsNullOrEmpty(soapResult))
            {
                try
                {
                    xmlCASResult = new XmlDocument();
                    xmlCASResult.LoadXml(soapResult);
                }
                catch
                {
                    xmlCASResult = null;
                }
            }

            return xmlCASResult;
        }

    }
}

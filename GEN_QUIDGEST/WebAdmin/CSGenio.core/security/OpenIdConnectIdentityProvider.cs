using CSGenio;
using CSGenio.core.di;
using CSGenio.framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Web;

namespace GenioServer.security
{
    /* Example of External Identity Providers
     *  - GOOGLE https://developers.google.com/identity/protocols/OpenIDConnect:
     *      Authorize URI -> https://accounts.google.com/o/oauth2/v2/auth
     *      TokenEndpoint -> https://oauth2.googleapis.com/token
     *  - AzureAD https://learn.microsoft.com/en-us/entra/identity-platform/v2-protocols-oidc: 
     *      Authorize URI -> https://login.microsoftonline.com/[tennentId]/oauth2/v2.0/authorize
     *      TokenEndpoint -> https://login.microsoftonline.com/[tennentId]/oauth2/v2.0/token
     */

    [CredentialProvider(typeof(TokenCredential))]
    [Description("Establishes identity using an external OpenIdConnect provider.")]
    [DisplayName("OpenIdConnect")]
    public class OpenIdConnectIdentityProvider : BaseIdentityProvider
    {
        /// <summary>
        /// Authority is the url used when making OpenIdConnect calls.
        /// </summary>
        [SecurityProviderOption()]
        [Description("The authorization endpoint in the provider where the user will login with his credentials")]
        public string Authority { get; set; }
        /// <summary>
        /// To obtain an Access Token, an ID Token, and optionally a Refresh Token, the RP (Client) sends a Token Request to the Token Endpoint to obtain a Token Response. More info at https://openid.net/specs/openid-connect-core-1_0.html#TokenEndpoint
        /// </summary>
        [SecurityProviderOption(optional: true)]
        [Description("Endpoint to validate the authorization token sent in the callback, and to exchange it for a user id token")]
        public string TokenEndpoint { get; set; }
        /// <summary>
        /// To obtain an Access Token, an ID Token, and optionally a Refresh Token, the RP (Client) sends a Token Request to the Token Endpoint to obtain a Token Response. More info at https://openid.net/specs/openid-connect-core-1_0.html#TokenEndpoint
        /// </summary>
        [SecurityProviderOption(optional: true)]
        [Description("Logout endpoint to clear the current user authenticated state")]
        public string LogoutEndpoint { get; set; }
        /// <summary>
        /// OAuth 2.0 Client Identifier valid at the Authorization Server.
        /// </summary>
        [SecurityProviderOption()]
        [Description("OAuth 2.0 Client Identifier valid at the Authorization Server")]
        public string ClientId { get; set; }
        // <summary>
        /// Client secret provided by the idenity provider to double check if user are authenticated successfully.
        /// </summary>
        [SecurityProviderOption()]
        [Description("Client secret provided by the identity provider used to access the TokenEndpoint")]
        public string ClientSecret { get; set; }
        /// <summary>
        /// Gets the list of permissions to request.
        /// </summary>
        public List<string> Scope { get; private set; } = new List<string>();
        /// <summary>
        /// field used to connect to a database user Ex: "email"
        /// </summary>
        [SecurityProviderOption(optional: true)]
        [Description("Name of the JWT property used to match against the user email field instead of the extenal userid")]
        public string UserIdField { get; set; }
        /// <summary>
        /// Gets the list of permissions to request (config).
        /// </summary>
        [SecurityProviderOption(optional: true)]
        [Description("List of user information properties to request")]
        public List<string> Scopes { get; set; }


        /// <inheritdoc/>
        public OpenIdConnectIdentityProvider(IdentityProviderCfgEl config) : base(config)
        {
            Scope = [
                "openid", //This is the only mandatory scope and will return a sub claim which represents a unique identifier for the authenticated user.
                "profile" //This scope value requests access to the End-User’s default profile Claims, which are: name, family_name, given_name, middle_name, nickname, preferred_username, profile, picture, website, gender, birthdate, zoneinfo, locale, and updated_at.                
            ];

            if (Scopes != null)
                Scope.AddRange(Scopes);
        }


        /// <inheritdoc/>
        public override bool HasRedirectLogin() => true;

        /// <inheritdoc/>
        public override string GetRedirectLoginUrl(string callback, string state = null)
        {
            if (string.IsNullOrEmpty(Authority) || string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(callback))
                throw new InvalidOperationException("It's mandatory to configure Authority, ClientId and CallbackPath options");

            var uriBuilder = new UriBuilder(Authority);
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            //authorization flow
            if (!string.IsNullOrEmpty(TokenEndpoint))
                parameters["response_type"] = "code";
            //implicit flow
            else
                parameters["response_type"] = "id_token";
            parameters["response_mode"] = "form_post";
            parameters["nonce"] = Guid.NewGuid().ToString("N").ToUpper();
            parameters["client_id"] = ClientId;
            parameters["redirect_uri"] = callback;
            parameters["scope"] = string.Join(" ", Scope);

            if (!string.IsNullOrEmpty(state))
                parameters["state"] = state;

            uriBuilder.Query = HttpUtility.UrlDecode(parameters.ToString());

            if (Log.IsDebugEnabled)
                Log.Debug(string.Format("GetRedirectLoginUrl: {0}", uriBuilder.Uri.ToString()));

            return uriBuilder.Uri.ToString();
        }

        /// <summary>
        /// Will check credentials and will find the "authenticated" user are on our application
        /// </summary>
        /// <param name="credential">Token identification to user on external provider</param>
        /// <returns>Internal Identity when user are found and success login on external provider</returns>
        public override GenioIdentity Authenticate(Credential credential)
        {
            if (credential is TokenCredential token)
                return Authenticate(token);

            return null;
        }
        
        /// <summary>
        /// Will check credentials and will find the "authenticated" user are on our application
        /// </summary>
        /// <param name="credential">Token identification to user on external provider</param>
        /// <param name="code">When not empty will double check if code returned from JWT are alright authenticated on external provider</param>
        /// <returns>Internal Identity when user are found and success login on external provider</returns>
        private GenioIdentity Authenticate(TokenCredential credential)
        {
            return ValidateToken(credential);
        }

        /// <summary>
        /// Validate token with optional to make other call to external identity provider to double check authentication validity
        /// </summary>
        /// <param name="credential">Token identification to user on external provider</param>
        /// <returns>The external identity if it suceeds or null in case it fails</returns>
        private GenioIdentity ValidateToken(TokenCredential tokenCredential)
        {
            //Authorization Flow - Exchange the auth token for the identity token on the external service
            if (!string.IsNullOrEmpty(TokenEndpoint))
            {
                if (string.IsNullOrEmpty(ClientSecret))
                    return null;
                if (string.IsNullOrEmpty(tokenCredential.Auth))
                    return null;

                using var http = GenioDI.HttpFactory.CreateClient("openid");
                http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    { "grant_type", "authorization_code" },
                    { "code", tokenCredential.Auth },
                    { "client_id", ClientId },
                    { "client_secret", ClientSecret },
                    { "redirect_uri", tokenCredential.OriginUrl },
                });

                var resp = http.PostAsync(TokenEndpoint, content).Result;
                resp.EnsureSuccessStatusCode();
                string responseInString = resp.Content.ReadAsStringAsync().Result;

                var json = JsonNode.Parse(responseInString);
                var jwtstring = json["id_token"].GetValue<string>();
                var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(jwtstring);
                var ext_username = CalcExternalUsername(jwt);

                return ext_username;
            }
            //Implicit Flow - Trust the jwt directly sent by the authentication callback
            // This is NOT recommended, and is considered a very insecure method
            else
            {
                if (string.IsNullOrEmpty(tokenCredential.Token))
                    return null;
                var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(tokenCredential.Token);
                //TODO: we should at least validate if the JWT public key matched the authorized issuer
                var ext_username = CalcExternalUsername(jwt);
                if (Log.IsDebugEnabled)
                    Log.Debug(string.Format("ValidateToken Implicit Flow: {0}", ext_username.Name));

                return ext_username;
            }
        }

        private GenioIdentity CalcExternalUsername(System.IdentityModel.Tokens.Jwt.JwtSecurityToken jwt)
        {
            GenioIdentity res = new()
            {
                Name = string.IsNullOrEmpty(UserIdField)
                    ? jwt.Subject + "@" + jwt.Issuer
                    : jwt.Payload[UserIdField].ToString(),
                IsAuthenticated = true,
                AuthenticationType = this.GetType().Name,
                IdProperty = string.IsNullOrEmpty(UserIdField)
                    ? GenioIdentityType.ExternalId
                    : GenioIdentityType.Email
            };

            foreach (var prop in jwt.Payload)
                res.Claims.Add(prop.Key, prop.Value.ToString());

            return res;
        }



    }
}

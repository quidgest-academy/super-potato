using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.config;
using System.ComponentModel.DataAnnotations;

namespace Administration.Models
{
    /// <summary>
    /// Interface EmailProperties
    /// </summary>
    public class EmailPropertiesModel : ModelBase
    {

        /// <summary>
        /// Class to store information on Email properties to be used when sending messages through email
        /// </summary>
        [Key]
		/// <summary>Campo : "PK da tabela" Tipo: "+" Formula:  ""</summary>
        public string ValCodpmail { get; set; } //Properties identifier

        [Display(Name = "ID36840", ResourceType = typeof(Resources.Resources))]
        public string ValId { get; set; } //Properties identifier

        [Display(Name = "NOME_DO_REMETENTE60175", ResourceType = typeof(Resources.Resources))]
        public string ValDispname { get; set; } //sender name

        [Display(Name = "REMETENTE47685", ResourceType = typeof(Resources.Resources))]
        public string ValFrom { get; set; } //sender email

        [Display(Name = "SERVIDOR_DE_SMTP00309", ResourceType = typeof(Resources.Resources))]
        public string ValSmtpserver { get; set; } // email server

        [Display(Name = "PORTA55707", ResourceType = typeof(Resources.Resources))]
        public int ValPort { get; set; } // porta smtp 

        [Display(Name = "SSL")] 
        public bool ValSsl { get; set; } // ssl connection required

        public AuthType ValAuthType { get; set; }

        public OAuth2Options ValOAuth2Options { get; set; }
        public string OAuth2ClientSecret { get; set; }
        public string OAuth2CertificateThumbprint{ get; set; }

        public bool HasOAuth2ClientSecret { get; set; }
        public bool HasOAuth2Certificate { get; set; }

        [Display(Name = "UTILIZADOR52387", ResourceType = typeof(Resources.Resources))]
        public string ValUsername { get; set; }

        [Display(Name = "PASSWORD09467", ResourceType = typeof(Resources.Resources))]
        public string ValPassword { get; set; }

        public bool HasPassword { get; set; }

        public string OldId { get; set; }
        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        public object SelectLists
        {
            get
            {
                return new
                {
                    AuthType = Enum.GetNames<AuthType>().Select(item => new { Text = item, Value = item }),
                };
            }
        }

        /// <summary>
        /// Help object for configuring providers with default values.
        /// </summary>
        public object OAuthHelp
        {
            get
            {
                return new
                {
                    TokenEndpoint = new Dictionary<string, string>()
                    {
                        { "Microsoft", "https://login.microsoftonline.com/[<Tenant Id>]/oauth2/v2.0/token" }
                    },
                    Scope = new Dictionary<string, string[]>()
                    {
                        { "Microsoft", ["https://outlook.office365.com/.default"] }
                    }
                };
            }
        }

        public void MapToModel(EmailServer m)
        {
            if (m == null)
            {
                Log.Error("Map ViewModel (EmailPropertiesModel) to Model (EmailServer) - Model is a null reference");
                throw new Exception("Model not found");
            }
            try
            {
                m.Name = ValDispname;
                m.From = ValFrom;
                
                m.SMTPServer = ValSmtpserver;
                m.Port = ValPort;
                m.AuthType = ValAuthType;
                m.SSL = ValSsl;
                m.Username = ValUsername ?? "";
                
                // Handle authentication settings
                if(ValAuthType != AuthType.None)
                {
                    // Username is required for any authentication type other than None
                    if (string.IsNullOrEmpty(ValUsername))
                    {
                        var errorMsg = string.Format(Resources.Resources.O_CAMPO__0__E_OBRIGA36687, "Username");
                        throw new BusinessException(errorMsg, "EmailPropertiesModel.MapToModel", errorMsg);
                    }

                    if(ValAuthType == AuthType.BasicAuth)
                    {
                        // Decrypt current password to check if user has changed it
                        string oldPassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(m.Password ?? ""));

                        // Change password if its different or if it wasn't inserted before
                        if (!HasPassword || oldPassword != ValPassword)
                        {
                            if (string.IsNullOrEmpty(ValPassword))
                            {
                                var errorMsg = string.Format(Resources.Resources.O_CAMPO__0__E_OBRIGA36687, "Password");
                                throw new BusinessException(errorMsg, "EmailPropertiesModel.MapToModel", errorMsg);
                            }

                            // Convert the new password to Base64 ("encrypt")
                            byte[] pass_bytes = System.Text.Encoding.UTF8.GetBytes(ValPassword ?? "");
                            m.Password = Convert.ToBase64String(pass_bytes, Base64FormattingOptions.None);
                        }

                        // Clear OAuth2 options if using Basic Authentication
                        m.OAuth2Options = null;
                    }
                    else if(ValAuthType == AuthType.OAuth2)
                    {
                        // OAuth2 options are required for OAuth2 authentication
                        if(ValOAuth2Options == null)
                            throw new BusinessException(Resources.Resources.OAUTH2_OPTIONS_ARE_R50941, "EmailPropertiesModel.MapToModel", Resources.Resources.OAUTH2_OPTIONS_ARE_R50941);

                        if (string.IsNullOrEmpty(ValOAuth2Options.ClientId))
                        {
                            var errorMsg = string.Format(Resources.Resources.O_CAMPO__0__E_OBRIGA36687, "Client ID");
                            throw new BusinessException(errorMsg, "EmailPropertiesModel.MapToModel", errorMsg);
                        }

                        // Determine if either ClientSecret or Certificate is provided
                        // If there is already a previously encrypted and saved value and no new value is provided, we use the old one without decrypting it.
                        var hasClientSecret = !string.IsNullOrEmpty(OAuth2ClientSecret) || !string.IsNullOrEmpty(m.OAuth2Options?.ClientSecret);
                        var hasCertificate = !string.IsNullOrEmpty(OAuth2CertificateThumbprint) || !string.IsNullOrEmpty(m.OAuth2Options?.CertificateThumbprint);

                        // At least one of ClientSecret or Certificate is required
                        if (!hasClientSecret && !hasCertificate)
                            throw new BusinessException(Resources.Resources.EITHER__CLIENT_SECRE43020, "EmailPropertiesModel.MapToModel", Resources.Resources.EITHER__CLIENT_SECRE43020);

                        if (string.IsNullOrEmpty(ValOAuth2Options.TokenEndpoint))
                        {
                            var errorMsg = string.Format(Resources.Resources.O_CAMPO__0__E_OBRIGA36687, "Token Endpoint");
                            throw new BusinessException(errorMsg, "EmailPropertiesModel.MapToModel", errorMsg);
                        }

                        var scope = ValOAuth2Options.Scope?.Where(scope => !string.IsNullOrWhiteSpace(scope)).ToArray();
                        if (scope == null || scope.Length == 0)
                        {
                            var errorMsg = string.Format(Resources.Resources.O_CAMPO__0__E_OBRIGA36687, "Scope");
                            throw new BusinessException(errorMsg, "EmailPropertiesModel.MapToModel", errorMsg);
                        }

                        var newOAuthOptions = new OAuth2Options()
                        {
                            ClientId = ValOAuth2Options.ClientId,
                            ClientSecret = m.OAuth2Options?.ClientSecret,
                            CertificateThumbprint = m.OAuth2Options?.CertificateThumbprint,
                            TokenEndpoint = ValOAuth2Options.TokenEndpoint,
                            Scope = scope
                        };

                        // Encrypt ClientSecret if a new one is provided.
                        if(!string.IsNullOrEmpty(OAuth2ClientSecret))
                            newOAuthOptions.ClientSecretDecrypted = OAuth2ClientSecret;

                        // Encrypt Certificate if a new one is provided.
                        if(!string.IsNullOrEmpty(OAuth2CertificateThumbprint))
                            newOAuthOptions.CertificateThumbprintDecrypted = OAuth2CertificateThumbprint;

                        // Update OAuth2 options in the model
                        m.OAuth2Options = newOAuthOptions;

                        // Clear password if using OAuth2
                        m.Password = null;
                    }
                    else if(ValAuthType == AuthType.None)
                    {
                        // Clear authentication credentials if no authentication is selected
                        m.Username = null;
                        m.Password = null;
                        m.OAuth2Options = null;
                    }
                }

                m.Id = ValId;
				m.Codpmail = ValCodpmail;
            }
            catch (Exception)
            {
                Log.Error("Map ViewModel (EmailPropertiesModel) to Model (EmailServer) - Error during mapping");
                throw;
            }
        }

        public void MapFromModel(EmailServer m)
        {
            if (m == null)
            {
                Log.Error("Map ViewModel (EmailPropertiesModel) to Model (EmailServer) - Model is a null reference");
                throw new Exception("Model not found");
            }
            try
            {
                ValDispname = m.Name;
                ValFrom = m.From;

                ValSmtpserver = m.SMTPServer;
                ValPort = m.Port;
                ValAuthType = m.AuthType;
                ValOAuth2Options = new OAuth2Options()
                {
                    ClientId = m.OAuth2Options?.ClientId,
                    ClientSecret = null, // never send for client-side
                    CertificateThumbprint = null, // never send for client-side
                    TokenEndpoint = m.OAuth2Options?.TokenEndpoint,
                    Scope = m.OAuth2Options?.Scope,
                };
                HasOAuth2ClientSecret = !string.IsNullOrEmpty(m.OAuth2Options?.ClientSecret);
                HasOAuth2Certificate = !string.IsNullOrEmpty(m.OAuth2Options?.CertificateThumbprint);
                ValSsl = m.SSL;
                ValUsername = m.Username;
                ValPassword = null;

                if (string.IsNullOrEmpty(m.Password)) HasPassword = false;
                else HasPassword = true;

                ValId = m.Id;
				ValCodpmail = m.Codpmail;
            }
            catch (Exception)
            {
                Log.Error("Map ViewModel (EmailPropertiesModel) to Model (EmailServer) - Error during mapping");
                throw;
            }
        }
    }


}


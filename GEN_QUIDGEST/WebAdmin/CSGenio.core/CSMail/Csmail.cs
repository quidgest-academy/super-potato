using System;
using System.Collections.Generic;
using System.IO;
using MimeKit;
using MailKit.Net.Smtp;
using System.Text.RegularExpressions;
using MimeKit.Utils;
using MailKit.Security;
using System.Threading.Tasks;
using System.Threading;
using CSGenio.framework;

namespace CSGenio.core
{
    /// <summary>
    /// Classe que representa um email
    /// exists a posiblidade de enviar vários ficheros em attachment, basta passar um array de string com os nomes dos ficheiros a anexar. atenção que os nomes dos ficheiros tem que ser caminhos completos.    
    /// também e possível enviar um mail to vários destinatários, basta criar uma string com os mail separados por vírgula (,)ex:"quidgest@quidgest.pt,jpedro@quidgest.pt"    
    /// </summary>
    public class CSmail
    {
        private string de;//e-mail do remetente
        private string to;//e-mail(s) do destinatário(s)
        private string subject;//subject do e-mail
        private string body;//body do e_email
        private bool bodyhtml;//indica se o body do e-mail vai em html //(FFS 2014.10.16)
        private string[] attachment;//lista com os nomes dos ficheiros anexos
        private string smtpServer; // GenioServer de mail 
        private bool ssl = false; // Ligação ssl (MA 2009.10.07)
        private int port = 25; // porta smtp (MA 2009.10.07)
        private string user;
        private string pass;
        private string cc; //endereços em CC (JMT 2011.04.04)
        private string bcc; //endereços em Bcc (PR 2014.10.16)
        private string textass; //text após imagem da assinatura (SF 2016.02.10)
        private string pathimg; //imagem da assinatura (SF 2016.02.10)
        private string nomeremetente; //nome a apresentar no remetente
        private Dictionary<string, Stream> dictionaryanexos; //Anexos por stream (ao invés de path)
        private List<Stream> streamimagens; //Imagens no corpo do email, por stream (ao invés de path)
        public string ReplyTo { get; set; } // Propriedade para o endereço "Reply-To"
        public bool SendInvite { get; set; } //Allows to send an invite to all destinations
        public InviteData InviteDataInfo { get; set; } //Needed information to complete the invitation
        public StatusMessage StatusMessage { get; set; }

        /// <summary>
        /// A collection of linked resources, such as embedded images, used in the HTML body of the email.
        /// Each linked resource is represented by an instance of the EmailLinkedResource class, 
        /// and can be referenced in the email content via a Content ID.
        /// </summary>
        public List<CSMail.EmailLinkedResource> LinkedResources { get; private set; } = [];

        /// <summary>
        /// Specifies the type of authentication to use when connecting to the SMTP server.
        /// </summary>
        public CSGenio.config.AuthType AuthType { get; set; }

        /// <summary>
        /// Contains the options required for OAuth2 autentication.
        /// This should be set if <see cref="AuthType"/> is set to <c>authType.OAuth2</c>. 
        /// </summary>
        public CSGenio.config.OAuth2Options OAuth2Options { get; set; }

        /// <summary>
        /// Constructor dum Qfield que nao é formula, nem array,  nem tem Qvalue default
        /// </summary>
        /// <param name="de"></param>
        /// <param name="para"></param>
        /// <param name="assunto"></param>
        /// <param name="anexo"></param>
        /// <param name="smtpServer"></param>
        public CSmail(string de,
                         string to,
                         string subject,
                         string body,
                         string[] attachment,
                         string smtpServer,
                         int port, // (MA 2009.10.07)
                         bool ssl,  // (MA 2009.10.07)
                         string cc,  // (JMT 2011.04.04)
                         string bcc,
                         string textass,
                         string pathimg,
                         bool bodyhtml //(FFS 2014.10.16)
        )
        {
            this.de = de;
            this.to = to;
            this.subject = subject;
            this.body = body;
            this.bodyhtml = bodyhtml;//(FFS 2014.10.16)
            this.attachment = attachment;
            this.smtpServer = smtpServer;
            this.port = port; // (MA 2009.10.07)
            this.ssl = ssl; // (MA 2009.10.07)
            this.cc = cc;   // (JMT 2011.04.04)
            this.bcc = bcc;
            this.textass = textass;
            this.pathimg = pathimg;
        }

        public CSmail(string de,
                         string to,
                         string subject,
                         string body,
                         string[] attachment,
                         string smtpServer,
                         int port, // (MA 2009.10.07)
                         bool ssl,  // (MA 2009.10.07)
                         string cc  // (JMT 2011.04.04)
        )
        {
            this.de = de;
            this.to = to;
            this.subject = subject;
            this.body = body;
            this.bodyhtml = false;//(FFS 2014.10.16)
            this.attachment = attachment;
            this.smtpServer = smtpServer;
            this.port = port; // (MA 2009.10.07)
            this.ssl = ssl; // (MA 2009.10.07)
            this.cc = cc;   // (JMT 2011.04.04)
            this.bcc = "";
            this.textass = "";
            this.pathimg = "";
        }
        public CSmail(string de,
                         string para,
                         string assunto,
                         string corpo,
                         string[] anexo,
                         string smtpServer,
                         int port, // (MA 2009.10.07)
                         bool ssl,  // (MA 2009.10.07)
                         string cc,  // (JMT 2011.04.04)
                         string nomeremetente
        )
        {
            this.de = de;
            this.to = para;
            this.subject = assunto;
            this.body = corpo;
            this.bodyhtml = false;//(FFS 2014.10.16)
            this.attachment = anexo;
            this.smtpServer = smtpServer;
            this.port = port; // (MA 2009.10.07)
            this.ssl = ssl; // (MA 2009.10.07)
            this.cc = cc;   // (JMT 2011.04.04)
            this.bcc = "";
            this.textass = "";
            this.pathimg = "";
            this.nomeremetente = nomeremetente;
        }

        public CSmail(string nomeremetente,
                         string de,
                         string para,
                         string assunto,
                         string corpo,
                         Dictionary<string, Stream> dictionaryanexos, //nome_anexo + anexo
                         string smtpServer,
                         int port, // (MA 2009.10.07)
                         bool ssl,  // (MA 2009.10.07)
                         string cc,  // (JMT 2011.04.04)
                         string bcc,
                         List<Stream> imagens,
                         string textass,
                         bool bodyhtml //(FFS 2014.10.16)
        )
        {
            this.nomeremetente = nomeremetente;
            this.de = de;
            this.to = para;
            this.subject = assunto;
            this.body = corpo;
            this.dictionaryanexos = dictionaryanexos;
            this.smtpServer = smtpServer;
            this.port = port; // (MA 2009.10.07)
            this.ssl = ssl; // (MA 2009.10.07)
            this.cc = cc;   // (JMT 2011.04.04)
            this.bcc = bcc;
            this.streamimagens = imagens;
            this.textass = textass;
            this.bodyhtml = bodyhtml;
        }

        /// <summary>
        /// Constructor dum Qfield que nao é formula, nem array,  nem tem Qvalue default
        /// </summary>
        public CSmail()
        {
            de = "quidgest@quidgest.pt";
            to = "quidmail@quidgest.pt";
            subject = "";
            body = "E-mail enviado pelo programa RQW";
            bodyhtml = false;//(FFS 2014.10.16)
            attachment = new string[1] { "" };
            smtpServer = "cp99.webserver.pt";
            port = 25;  // (MA 2009.10.07)
            ssl = false; // (MA 2009.10.07)
            cc = "";    //(JMT 2011.04.04)
            bcc = ""; //(PR 2012.04.03)
            textass = "";//(SF 2016.02.10)
            pathimg = "";//(SF 2012.02.10)
        }

        /// <summary>
        /// Sends an email synchronously.
        /// </summary>
        /// <returns>True if the email was sent sucessfully; otherwise, false.</returns>
        public bool Send()
        {
            try
            {
                // Call the asynchronous method and wait for it to complete
                return SendAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new Exception("CSmail.Send - Error sending email. " + ex?.Message, ex);
            }
        }

        /// <summary>
        /// Validates the configuration of email addresses across multiple fields 
        /// (e.g., Bcc, ReplyTo, CC, To) and returns a status message with the results.
        /// </summary>
        /// <returns>
        /// A <see cref="StatusMessage"/> indicating the validation results, including warnings 
        /// for any invalid email addresses.
        /// </returns>
        public StatusMessage ValidateConfiguraitons()
        {
            StatusMessage statusMessage = new StatusMessage();

            if (!validateMail(From))
                statusMessage.MergeStatusMessage(StatusMessage.Error("The sender's email is invalid."));

            int validEmails;
            ValidateEmailAddresses("Bcc", Bcc, statusMessage);
            ValidateEmailAddresses("ReplyTo", ReplyTo, statusMessage);
            ValidateEmailAddresses("CC", CC, statusMessage);
            validEmails = ValidateEmailAddresses("To", To, statusMessage);

            if(validEmails == 0)
                statusMessage.MergeStatusMessage(StatusMessage.Error("There is no valid destination email."));

            return statusMessage;
        }
        /// <summary>
        /// Validates email addresses from a given field and appends any warnings 
        /// about invalid addresses to the provided status message.
        /// </summary>
        /// <param name="emailField">The name of the email field being validated (e.g., "Bcc", "To").</param>
        /// <param name="addresses">A string containing email addresses separated by ';' or ','.</param>
        /// <param name="statusMessage">The status message object to append validation warnings.</param>
        private int ValidateEmailAddresses(string emailField, string addresses, StatusMessage statusMessage)
        {
            if (string.IsNullOrEmpty(addresses))
                return 0 ;

            //Number o valid emails
            int ValidEmails = 0;

            foreach (var address in addresses.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!validateMail(address))
                {
                    statusMessage.MergeStatusMessage(StatusMessage.Warning($"The email address '{address}' in {emailField} is invalid and will be ignored."));
                }
                else 
                    ValidEmails++;
            }

            return ValidEmails;
        }


        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for task management.</param>
        /// <returns>A task that returns true if the email was sent successfully; otherwise, false.</returns>
        public async Task<bool> SendAsync(CancellationToken cancellationToken = default)
        {
            if (validate())
            {
                // Ensure TLS 1.2 is enabled (should be set at application startup)
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

                // Create a new email message
                using MimeMessage msg = new();
                msg.From.Add(new MailboxAddress(nomeremetente, de));

                // Add recipients
                AddValidEmails(to, msg.To);
                AddValidEmails(ReplyTo, msg.ReplyTo);
                AddValidEmails(cc, msg.Cc);
                AddValidEmails(bcc, msg.Bcc);

                msg.Subject = subject;

                // Build the email body
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = bodyhtml ? body : null,
                    TextBody = bodyhtml ? null : body
                };

                // Add signature images to the email body
                if (!string.IsNullOrEmpty(pathimg) && File.Exists(pathimg))
                {
                    // Add image from file path
                    var image = bodyBuilder.LinkedResources.Add(pathimg);
                    image.ContentId = MimeUtils.GenerateMessageId();
                    bodyBuilder.HtmlBody ??= string.Empty;
                    bodyBuilder.HtmlBody += $"<img src=\"cid:{image.ContentId}\">{textass}";
                }
                else if (streamimagens?.Count > 0)
                {
                    //  Add images from streams
                    bodyBuilder.HtmlBody ??= string.Empty;
                    foreach (var imageStream in streamimagens)
                    {
                        var linkedResource = new MimePart(new ContentType("application", "octet-stream"))
                        {
                            ContentId = MimeUtils.GenerateMessageId(),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            Content = new MimeContent(imageStream),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Inline)
                        };
                        bodyBuilder.LinkedResources.Add(linkedResource);
                        bodyBuilder.HtmlBody += $"<br/><img src=\"cid:{linkedResource.ContentId}\"/>";
                    }
                    bodyBuilder.HtmlBody += $"<br/>{textass}";
                    bodyBuilder.HtmlBody = bodyBuilder.HtmlBody.Replace(Environment.NewLine, "<br/>");
                }
                else
                {
                    // Add text signature to the email body
                    bodyBuilder.TextBody += textass;
                }

                // Add any additional linked resources
                LinkedResources?.ForEach(linkedResource => bodyBuilder.LinkedResources.Add(linkedResource.Resource));


                // Add attachments from file paths (string[])
                if (attachment != null)
                {
                    foreach (var attachmentFile in attachment)
                    {
                        if (!string.IsNullOrEmpty(attachmentFile) && File.Exists(attachmentFile))
                        {
                            bodyBuilder.Attachments.Add(attachmentFile);
                        }
                    }
                }

                // Add attachments from streams (Dictionary<string, stream>)
                if (dictionaryanexos != null)
                {
                    foreach (var attachmentFile in dictionaryanexos)
                    {
                        bodyBuilder.Attachments.Add(attachmentFile.Key, attachmentFile.Value);
                    }
                }

                //if it's supposed to send an invite
                if(this.SendInvite)
                {
                    if(this.InviteDataInfo == null)
                        throw new InvalidOperationException("Invite data is not complete");

                    //Create invite structure in ics extension
                    string calendarEvent = this.InviteDataInfo.CreateCalendarEvent(this.to);

                    //Create the event and attach it to the email
                    var calendarPart = new MimePart("text", "calendar")
                    {
                        Content = new MimeContent(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(calendarEvent))),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = "Invite.ics"
                    };

                    bodyBuilder.Attachments.Add(calendarPart);
                }

                // Set the email body
                msg.Body = bodyBuilder.ToMessageBody();

                // Create and configure the SMTP client
                using SmtpClient client = new();


                // Quick fix: This workaround addresses issues when clients use port 25,
                // but the server has a misconfigured or untrusted certificate.
                // In the future, this should be configurable via WebAdmin.
                SecureSocketOptions secureOption = ssl
                    ? SecureSocketOptions.StartTls
                    : (port == 0 || port == 25
                        ? SecureSocketOptions.None
                        : SecureSocketOptions.Auto);

                await client.ConnectAsync(smtpServer, port, secureOption, cancellationToken).ConfigureAwait(false);

                if (AuthType == CSGenio.config.AuthType.BasicAuth)
                {
                    // Authenticate using username and password
                    await client.AuthenticateAsync(user, pass, cancellationToken).ConfigureAwait(false);
                }
                else if (AuthType == CSGenio.config.AuthType.OAuth2)
                {
                    // Authenticate using OAuth2
                    if (OAuth2Options == null)
                        throw new InvalidOperationException($"CSMail SendAsync - OAuth2 options are required for XOAuth2 authentication.");

                    // Obtain the token provider based on the OAuth2 optins
                    var tokenProvider = CSMail.OAuth2TokenManager.GetTokenProvider(OAuth2Options);

                    // Retrive the access token
                    var accessToken = await tokenProvider.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);

                    // Create an XOAUTH2 authentication mechanism using the username and access token
                    var xOAuth2 = new SaslMechanismOAuth2(user, accessToken);

                    // Authenticate with the SMTP server using XOAUTH2
                    await client.AuthenticateAsync(xOAuth2, cancellationToken).ConfigureAwait(false);
                }

                // Send the email message
                await client.SendAsync(msg, cancellationToken).ConfigureAwait(false);

                // Disconnect from the SMTP server
                await client.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);

                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddValidEmails(string addresses, InternetAddressList mailAddressList)
        {
            if (string.IsNullOrEmpty(addresses))
                return;

            foreach (string address in addresses.Split([';', ',']))
            {
                if (validateMail(address))
                {
                    mailAddressList.Add(new MailboxAddress(null, address));
                }
            }
        }

        /// <summary>
        /// Método que dado um array de strings preenche os destinatario ( DQ - 14072006)
        /// </summary>
        /// <param name="destin"></param>
        public void fillRecipient(object[] destin)
        {
            this.to = "";
            for (int i = 0; i < destin.Length; i++)
            {
                if (validateMail(destin[i].ToString()))
                    to += destin[i].ToString() + ",";
            }
            this.to = this.to.Remove(this.to.LastIndexOf(","));
        }

        /// <summary>
        /// English Version - Fills multiple mail destination addresses
        /// </summary>
        /// <param name="destin"></param>
        public void fillDestinations(object[] destin)
        {
            fillRecipient(destin);
        }

        /// <summary>
        /// Método que verifica se o email é válido.
        /// </summary>
        /// <param name="inputEmail"></param>
        public static bool validateMail(string inputEmail)
        {
            string strRegex = @"^[a-zA-Z0-9_+&*-]+(?>\.[a-zA-Z0-9_+&*-]+)*@(?>[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,20}$";
            Regex re = new Regex(strRegex, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        /// <summary>
        /// Método que faz as validações dos parâmetros do email são válidos.
        /// </summary>
        public bool validate()
        {
            if (validateMail(de))
            {
                if (smtpServer.Equals(""))
                    return false;

            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds the provided linked resource to the email's linked resources collection.
        /// This method is used when you already have an instance of a linked resource and want to add 
        /// it to the collection of resources that will be referenced in the HTML body of the email.
        /// </summary>
        /// <param name="linkedResource">The linked resource to be added, which contains an image or other media to be embedded in the email.</param>
        /// <returns>Returns the same linked resource that was added to the collection.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided linked resource is null.</exception>
        public CSMail.EmailLinkedResource AddHtmlImgLink(CSMail.EmailLinkedResource linkedResource)
        {
            // Validate that the linked resource is not null
            if (linkedResource == null)
                throw new ArgumentNullException(nameof(linkedResource), "Linked resource cannot be null.");

            LinkedResources.Add(linkedResource);

            // Return the linked resource for potential chaining or reference
            return linkedResource;
        }

        /// <summary>
        /// Adds an HTML image link (a linked resource) using the image file at the specified path.
        /// This method creates a linked resource from a file path and associates it with a Content ID, 
        /// allowing the image to be referenced in the HTML body of the email.
        /// </summary>
        /// <param name="contentId">The Content ID for referencing the image in the email body.</param>
        /// <param name="filePath">The full path to the image file to be embedded.</param>
        /// <returns>Returns the created linked resource associated with the image.</returns>
        public CSMail.EmailLinkedResource AddHtmlImgLink(string contentId, string filePath)
        {
            // Create a new EmailLinkedResource object using the file path and content ID
            var linkedResource = new CSMail.EmailLinkedResource(filePath, contentId);
            return AddHtmlImgLink(linkedResource);
        }

        /// <summary>
        /// Adds an HTML image link (a linked resource) using the provided byte array representing the image data.
        /// This method creates a linked resource from a byte array and associates it with a Content ID, 
        /// allowing the image to be referenced in the HTML body of the email.
        /// </summary>
        /// <param name="contentId">The Content ID for referencing the image in the email body.</param>
        /// <param name="fileData">The byte array containing the image data to be embedded.</param>
        /// <param name="mimeType">The MIME type of the image (e.g., "image/jpeg", "image/png").</param>
        /// <returns>Returns the created linked resource associated with the image.</returns>
        public CSMail.EmailLinkedResource AddHtmlImgLink(string contentId, byte[] fileData, ContentType mimeType)
        {
            // Create a new EmailLinkedResource object using the byte array, content ID, and MIME type
            var linkedResource = new CSMail.EmailLinkedResource(fileData, contentId, mimeType);
            return AddHtmlImgLink(linkedResource);
        }

        /// <summary>
        /// Adds an HTML image link (a linked resource) using the provided stream containing the image data.
        /// This method creates a linked resource from a stream and associates it with a Content ID, 
        /// allowing the image to be referenced in the HTML body of the email.
        /// </summary>
        /// <param name="contentId">The Content ID for referencing the image in the email body.</param>
        /// <param name="fileStream">The stream containing the image data to be embedded.</param>
        /// <param name="mimeType">The MIME type of the image (e.g., "image/jpeg", "image/png").</param>
        /// <returns>Returns the created linked resource associated with the image.</returns>
        public CSMail.EmailLinkedResource AddHtmlImgLink(string contentId, Stream fileStream, ContentType mimeType)
        {
            // Create a new EmailLinkedResource object using the stream, content ID, and MIME type
            var linkedResource = new CSMail.EmailLinkedResource(fileStream, contentId, mimeType);
            return AddHtmlImgLink(linkedResource);
        }

        /// <summary>
        /// Método que devolve ou coloca o remetente da mensagem
        /// </summary>
        public string From
        {
            get { return de; }
            set { de = value; }
        }


        /// <summary>
        /// Método que devolve ou coloca o(s) destinatários da mensagem
        /// </summary>
        public string To
        {
            get { return to; }
            set { to = value; }
        }


        /// <summary>
        /// Método que devolve ou coloca o subject da mensagem
        /// </summary>          
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }


        /// <summary>
        /// Método que devolve ou coloca o body da mensagem
        /// </summary>
        public string Body
        {
            get { return body; }
            set { body = value; }
        }


        /// <summary>
        /// Método que devolve e coloca a lista de ficheiros anexos
        /// </summary>
        public string[] Attachment
        {
            get { return attachment; }
            set { attachment = value; }
        }

        /// <summary>
        /// Método que devolve e coloca o servidor smtp
        /// </summary>
        public string SmtpServer
        {
            get { return smtpServer; }
            set { smtpServer = value; }
        }

        /// <summary>
        /// Método que define se a ligação é ssl - (MA 2009.10.07)
        /// </summary>
        public bool SSL
        {
            get { return ssl; }
            set { ssl = value; }
        }

        /// <summary>
        /// Método que define a porta smtp - (MA 2009.10.07)
        /// </summary>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// Indicates whether authentication is required.
        /// Obsolete: Use <see cref="AuthType"/> instead for specifying the authentication type.
        /// </summary>
        [Obsolete("For backward compatibility. Use the new AuthType property instead.")]
        public bool Auth
        {
            get { return AuthType == CSGenio.config.AuthType.BasicAuth; }
            set { AuthType = value ? CSGenio.config.AuthType.BasicAuth : CSGenio.config.AuthType.None; }
        }
        /// <summary>
        /// Método que devolve e coloca o servidor smtp
        /// </summary>
        public string User
        {
            get { return user; }
            set { user = value; }
        }
        /// <summary>
        /// Método que devolve e coloca o servidor smtp
        /// </summary>
        public string Pass
        {
            get { return pass; }
            set { pass = value; }
        }

        /// <summary>
        /// Método que devolve e coloca os endereços em CC - (JMT 2011.04.04)
        /// </summary>
        public string CC
        {
            get { return cc; }
            set { cc = value; }
        }

        /// <summary>
        /// Método que devolve e coloca o body do e-mail em html - (FFS 2014.10.16)
        /// </summary>
        public bool BodyHtml
        {
            get { return bodyhtml; }
            set { bodyhtml = value; }
        }

        /// <summary>
        /// Método que devolve e coloca os endereços em Bcc - (PR 2012.10.16)
        /// </summary>
        public string Bcc
        {
            get { return bcc; }
            set { bcc = value; }
        }

        /// <summary>
        /// Método que devolve e coloca a pasta da imagem to a assinatura - (SF 2016.02.10)
        /// </summary>
        public string Pathimg
        {
            get { return pathimg; }
            set { pathimg = value; }
        }

        /// <summary>
        /// Método que devolve e coloca o text após a imagem da assinatura - (SF 2016.02.10)
        /// </summary>
        public string Textass
        {
            get { return textass; }
            set { textass = value; }
        }

        /// <summary>
        /// Método que devolve e coloca o nome do remetente a apresentar no email
        /// </summary>
        public string NomeRemetente
        {
            get { return nomeremetente; }
            set { nomeremetente = value; }
        }

        /// <summary>
        /// Método que devolve e preenche a lista de imagens a adicionar ao corpo do email
        /// </summary>
        public List<Stream> StreamImagens
        {
            get { return streamimagens; }
            set { streamimagens = value; }
        }

        /// <summary>
        /// Método que devolve e preenche o dicionário de dados com os anexos a adicionar ao email
        /// </summary>
        public Dictionary<string, Stream> DictionaryAnexos
        {
            get { return dictionaryanexos; }
            set { dictionaryanexos = value; }
        }
    }
}
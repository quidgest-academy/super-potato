using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using CSGenio.framework;
using System.Text;
using System.Threading.Tasks;
using CSGenio.business;
using System.Web;
using CSGenio.persistence;
using System.Net;


namespace GenioServer.security
{
    public static class UserRegistration
    {      

        public const string CONTENT_PATH = "Content\\Email";

        public static string GetEmailForLanguage(string templateName, string language)
        {
            string fileName = templateName + "." + language + ".html";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(basePath, CONTENT_PATH, fileName);

            return File.ReadAllText(fullPath);
        }

        /// <summary>
        /// Body with HTML template with logo by default.
        /// </summary>
        /// <param name="user">User data to fill the template</param>
        /// <param name="LogoPath">Company logo path to add to the template header</param>
        /// <param name="TemplatePath">Template path to build the email</param>
        /// <returns></returns>
        public static AlternateView CreateBody(string path, string ticket, string UserName, string language = "")
        {
            string body = string.Empty;
            string templLang = string.Empty;
            if (!String.IsNullOrWhiteSpace(language))
                templLang = "." + language;
			
            string TemplatePath = path + $"Content\\Email\\EmailTemplate{templLang}.html";
            string LogoPath = path + "Content\\Email\\Logo.png";

            using (StreamReader reader = new StreamReader(TemplatePath))
            {

                body = reader.ReadToEnd();

            }

            body = body.Replace("{fldToken}", ticket);
            body = body.Replace("{fldName}", UserName);
			AlternateView alternateView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
            try
            {
				LinkedResource imglogo = new LinkedResource(LogoPath);
                //Associates the variable imgLogo to the placeholder cid:Logo on the template.
                imglogo.ContentId = "Logo";
                alternateView.LinkedResources.Add(imglogo);

            }
            catch
            {
                Log.Error("Could not link logo to the registration email");
            }
            return alternateView;

        }
		
				/// <summary>
        /// Body with HTML from a given template. Linked resources should correspond to cid:keyword and email keywords should be in the {keyword} format in the template
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="language"></param>
        /// <param name="emailKeywords"></param>
        /// <param name="emailLinkedResources"></param>
        /// <returns></returns>
        public static AlternateView CreateBodyFromTemplate(string templateName, string language, Dictionary<string, string> emailKeywords, Dictionary<string, string> emailLinkedResources)
        {
            string body = GetEmailForLanguage(templateName, language);
            foreach(var keyword in emailKeywords)
            {
                body = body.Replace($"{{{keyword.Key}}}", keyword.Value);
            }

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
            foreach(var linkedResource in emailLinkedResources)
            {
                try
                {
                    var resource = new LinkedResource(linkedResource.Value)
                    {
                        ContentId = linkedResource.Key
                    };
                    alternateView.LinkedResources.Add(resource);
                }
                catch
                {
                    Log.Error($"Could not link {linkedResource.Key} to the email");
                }
            }

            return alternateView;
        }

        /// <summary>
        /// Sends an email w/ HTML template for body (AlternateView).
        /// </summary>
        /// <param name="to">Email address where we want to send the mail to</param>
        /// <param name="view"></param>
        /// <param name="language"></param>        
        public static void SendEmail(CSGenio.EmailServer emailServer, string to, string subject, AlternateView view )
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
				client.Host = emailServer.SMTPServer;
                client.Port = emailServer.Port;
                client.EnableSsl = emailServer.SSL;
				
				if(emailServer.SSL)
                    // To turn on TLS 1.2 without affecting other protocols. It is preferred that it be configured at application startup.
                    System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

                if(emailServer.AuthType == CSGenio.config.AuthType.BasicAuth)
                {
                    client.UseDefaultCredentials = false;
                    byte[] data = Convert.FromBase64String(emailServer.Password);
                    string decodedPass = Encoding.UTF8.GetString(data);
                    client.Credentials = new NetworkCredential(emailServer.Username, decodedPass);
                }
                else if (emailServer.AuthType == CSGenio.config.AuthType.OAuth2)
                {
                    // TODO: For some reason, the user registration and password recovery didn't use the CSmail class. In the next phase, it would be refactored to use CSMail.
                    throw new NotImplementedException();
                }
                else
                {
                    client.UseDefaultCredentials = true;
                }
                               

                //inserted inside a using to dispose images in alternativeviews
                using (MailMessage mail = new MailMessage(emailServer.From, to))
                {
                    mail.IsBodyHtml = true;
                    mail.Subject = subject;
                    mail.AlternateViews.Add(view);
                    client.Send(mail);
                }


            }
            catch (Exception e)
            {
                throw new FrameworkException("Error sending email", "FuncoesGlobais.enviarEmail", "Exception when sending email: " + e.Message);
            }
        }

    }
}

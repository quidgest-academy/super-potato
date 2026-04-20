using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace GenioServer.PreViewer.ConvertMsg.Outlook
{
    #region Interface IReader
    public interface IReader
    {
        /// <summary>
        /// Extract the input msg file to the given output folder
        /// </summary>
        /// <param name="inputFile">The msg file</param>
        /// <param name="outputFolder">The folder where to extract the msg file</param>
        /// <param name="hyperlinks">When true then hyperlinks are generated for the To, CC, BCC and attachments</param>
        /// <returns>String array containing the message body and its (inline) attachments</returns>
        [DispId(1)]
        string[] ExtractToFolder(string inputFile, string outputFolder, bool hyperlinks);

        /// <summary>
        /// Get the last know error message. When the string is empty there are no errors
        /// </summary>
        /// <returns></returns>
        [DispId(2)]
        string GetErrorMessage();
    }
    #endregion

    /// <summary>
    /// This c
    /// </summary>
    [Guid("E9641DF0-18FC-11E2-BC95-1ACF6088709B")]
    [ComVisible(true)]
    public class Reader : IReader
    {
        #region Fields
        /// <summary>
        /// Contains an error message when something goes wrong in the <see cref="ExtractToFolder"/> method.
        /// This message can be retreived with the GetErrorMessage. This way we keep .NET exceptions inside
        /// when this code is called from a COM language
        /// </summary>
        private string _errorMessage;
        #endregion

        #region Private nested class Recipient
        /// <summary>
        /// Used as a placeholder for the recipients from the MSG file itself or from the "internet"
        /// headers when this message is send outside an Exchange system
        /// </summary>
        private class Recipient
        {
            public string EmailAddress { get; set; }
            public string DisplayName { get; set; }
        }
        #endregion

        #region ExtractToFolder
        /// <summary>
        /// This method reads the <see cref="inputFile"/> and when the file is supported it will do the following: <br/>
        /// - Extract the HTML, RTF (will be converted to html) or TEXT body (in these order) <br/>
        /// - Puts a header (with the sender, to, cc, etc... (depends on the message type) on top of the body so it looks 
        ///   like if the object is printed from Outlook <br/>
        /// - Reads all the attachents <br/>
        /// And in the end writes everything to the given <see cref="outputFolder"/>
        /// </summary>
        /// <param name="inputFile">The msg file</param>
        /// <param name="outputFolder">The folder where to save the extracted msg file</param>
        /// <param name="hyperlinks">When true hyperlinks are generated for the To, CC, BCC and attachments</param>
        /// <returns>String array containing the full path to the message body and its attachments</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public string[] ExtractToFolder(string inputFile, string outputFolder, bool hyperlinks)
        {
            outputFolder = FileManager.CheckForBackSlash(outputFolder);
            _errorMessage = string.Empty;

            try
            {
                using (var stream = File.Open(inputFile, FileMode.Open, FileAccess.Read))
                using (var message = new Storage.Message(stream))
                {
                    switch (message.Type)
                    {
                        case Storage.Message.MessageType.Email:
                            return WriteEmail(message, outputFolder, hyperlinks).ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                _errorMessage = GetInnerException(e);
                return new string[0];
            }

            // If we return here then the file was not supported
            return new string[0];
        }
        #endregion

        #region ReplaceFirstOccurence
        /// <summary>
        /// Method to replace the first occurence of the <see cref="search"/> string with a
        /// <see cref="replace"/> string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="search"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public string ReplaceFirstOccurence(string text, string search, string replace)
        {
            var index = text.IndexOf(search, StringComparison.Ordinal);
            if (index < 0)
                return text;

            return text.Substring(0, index) + replace + text.Substring(index + search.Length);
        }
        #endregion

        #region WriteEmail
        /// <summary>
        /// Writes the body of the MSG E-mail to html or text and extracts all the attachments. The
        /// result is returned as a List of strings
        /// </summary>
        /// <param name="message"><see cref="Storage.Message"/></param>
        /// <param name="outputFolder">The folder where we need to write the output</param>
        /// <param name="hyperlinks">When true then hyperlinks are generated for the To, CC, BCC and attachments</param>
        /// <returns></returns>
        private List<string> WriteEmail(Storage.Message message, string outputFolder, bool hyperlinks)
        {
            var fileName = "email";
            bool htmlBody;
            string body;
            List<string> attachmentList;
            List<string> files;

            PreProcessMesssage(message,
                               hyperlinks,
                               outputFolder,
                               ref fileName,
                               out htmlBody,
                               out body,
                               out attachmentList,
                               out files);

            string emailHeader;

            if (htmlBody)
            {
                #region Html body
                // Start of table
                emailHeader =
                    "<table style=\"width:100%; font-family: Times New Roman; font-size: 12pt;\">" + Environment.NewLine;

                // From
                emailHeader +=
                    "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"width: 100px; font-weight: bold; \">" +
                    LanguageConsts.EmailFromLabel + ":</td><td>" + GetEmailSender(message, hyperlinks, true) +
                    "</td></tr>" + Environment.NewLine;

                // Sent on
                if (message.SentOn != null)
                    emailHeader +=
                        "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"width: 100px; font-weight: bold; \">" +
                        LanguageConsts.EmailSentOnLabel + ":</td><td>" +
                        ((DateTime)message.SentOn).ToString(LanguageConsts.DataFormat, new CultureInfo(LanguageConsts.DateFormatCulture)) + "</td></tr>" +
                        Environment.NewLine;

                // To
                emailHeader +=
                    "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"width: 100px; font-weight: bold; \">" +
                    LanguageConsts.EmailToLabel + ":</td><td>" +
                    GetEmailRecipients(message, Storage.Recipient.RecipientType.To, hyperlinks, true) + "</td></tr>" +
                    Environment.NewLine;

                // CC
                var cc = GetEmailRecipients(message, Storage.Recipient.RecipientType.Cc, hyperlinks, false);
                if (cc != string.Empty)
                    emailHeader +=
                        "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"width: 100px; font-weight: bold; \">" +
                        LanguageConsts.EmailCcLabel + ":</td><td>" + cc + "</td></tr>" + Environment.NewLine;

                // BCC
                var bcc = GetEmailRecipients(message, Storage.Recipient.RecipientType.Bcc, hyperlinks, false);
                if (bcc != string.Empty)
                    emailHeader +=
                        "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"width: 100px; font-weight: bold; \">" +
                        LanguageConsts.EmailBccLabel + ":</td><td>" + bcc + "</td></tr>" + Environment.NewLine;

                // Subject
                emailHeader +=
                    "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"width: 100px; font-weight: bold; \">" +
                    LanguageConsts.EmailSubjectLabel + ":</td><td>" + message.Subject + "</td></tr>" + Environment.NewLine;

                //// Attachments
                //if (attachmentList.Count != 0)
                //    emailHeader +=
                //        "<tr style=\"height: 18px; vertical-align: top; \"><td style=\"width: 100px; font-weight: bold; \">" +
                //        LanguageConsts.EmailAttachmentsLabel + ":</td><td>" + string.Join(", ", attachmentList) + "</td></tr>" +
                //        Environment.NewLine;

                // Empty line
                emailHeader += "<tr><td colspan=\"2\" style=\"height: 18px; \">&nbsp</td></tr>" + Environment.NewLine;

                // End of table + empty line
                emailHeader += "</table><br/>" + Environment.NewLine;

                body = InjectHeader(body, emailHeader);
                #endregion
            }
            else
            {
                #region Text body
                // Read all the language consts and get the longest string
                var languageConsts = new List<string>
                {
                    LanguageConsts.EmailFromLabel,
                    LanguageConsts.EmailSentOnLabel,
                    LanguageConsts.EmailToLabel,
                    LanguageConsts.EmailCcLabel,
                    LanguageConsts.EmailBccLabel,
                    LanguageConsts.EmailSubjectLabel,
                    LanguageConsts.ImportanceLabel,
                    LanguageConsts.EmailAttachmentsLabel,
                    LanguageConsts.EmailFollowUpFlag,
                    LanguageConsts.EmailFollowUpLabel,
                    LanguageConsts.EmailFollowUpStatusLabel,
                    LanguageConsts.EmailFollowUpCompletedText,
                    LanguageConsts.TaskStartDateLabel,
                    LanguageConsts.TaskDueDateLabel,
                    LanguageConsts.TaskDateCompleted,
                    LanguageConsts.EmailCategoriesLabel
                };

                var maxLength = languageConsts.Select(languageConst => languageConst.Length).Concat(new[] { 0 }).Max() + 2;

                // From
                emailHeader =
                    (LanguageConsts.EmailFromLabel + ":").PadRight(maxLength) + GetEmailSender(message, false, false) + Environment.NewLine;

                // Sent on
                if (message.SentOn != null)
                    emailHeader +=
                        (LanguageConsts.EmailSentOnLabel + ":").PadRight(maxLength) +
                        ((DateTime)message.SentOn).ToString(LanguageConsts.DataFormat, new CultureInfo(LanguageConsts.DateFormatCulture)) + Environment.NewLine;

                // To
                emailHeader +=
                    (LanguageConsts.EmailToLabel + ":").PadRight(maxLength) +
                    GetEmailRecipients(message, Storage.Recipient.RecipientType.To, false, false) + Environment.NewLine;

                // CC
                var cc = GetEmailRecipients(message, Storage.Recipient.RecipientType.Cc, false, false);
                if (cc != string.Empty)
                    emailHeader += (LanguageConsts.EmailCcLabel + ":").PadRight(maxLength) + cc + Environment.NewLine;

                // BCC
                var bcc = GetEmailRecipients(message, Storage.Recipient.RecipientType.Bcc, false, false);
                if (bcc != string.Empty)
                    emailHeader += (LanguageConsts.EmailCcLabel + ":").PadRight(maxLength) + bcc + Environment.NewLine;

                // Subject
                emailHeader += (LanguageConsts.EmailSubjectLabel + ":").PadRight(maxLength) + message.Subject + Environment.NewLine;

                //// Attachments
                //if (attachmentList.Count != 0)
                //    emailHeader += (LanguageConsts.EmailAttachmentsLabel + ":").PadRight(maxLength) +
                //                          string.Join(", ", attachmentList) + Environment.NewLine + Environment.NewLine;

                body = emailHeader + body;
                #endregion
            }

            // Write the body to a file
            File.WriteAllText(fileName, body, Encoding.UTF8);

            return files;
        }
        #endregion

        #region PreProcessMesssage
        /// <summary>
        /// This function parses the attachments from RTF typed message like Appointments, Tasks and Contacts
        /// </summary>
        /// <param name="message"></param>
        /// <param name="htmlBody"></param>
        /// <param name="hyperlinks"></param>
        /// <param name="fileName"></param>
        /// <param name="outputFolder"></param>
        /// <param name="body"></param>
        /// <param name="attachments"></param>
        /// <param name="files"></param>
        private void PreProcessMesssage(Storage.Message message, 
                                        bool hyperlinks,
                                        string outputFolder,
                                        ref string fileName,
                                        out bool htmlBody,
                                        out string body,
                                        out List<string> attachments,
                                        out List<string> files)
        {
            const string rtfInlineObject = "[*[RTFINLINEOBJECT]*]";

            htmlBody = true;
            attachments = new List<string>();
            files = new List<string>();
            var htmlConvertedFromRtf = false;
            
            if (!string.IsNullOrEmpty(message.BodyHtml))
                body = message.BodyHtml;
            else if (!string.IsNullOrEmpty(message.BodyText))
            {
                body = message.BodyText;
                htmlBody = false;
            }
            else
            {
                body = message.BodyRtf;
                htmlBody = false;
            }

            fileName = outputFolder +
                       (!string.IsNullOrEmpty(message.Subject)
                           ? FileManager.RemoveInvalidFileNameChars(message.Subject)
                           : fileName) + (htmlBody ? ".htm" : ".txt");

            files.Add(fileName);

            var inlineAttachments = new SortedDictionary<int, string>();

            foreach (var attachment in message.Attachments)
            {
                FileInfo fileInfo = null;
                var attachmentFileName = string.Empty;
                var renderingPosition = -1;
                var isInline = false;

                // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                if (attachment is Storage.Attachment)
                {
                    var attach = (Storage.Attachment)attachment;
                    attachmentFileName = attach.FileName;
                    renderingPosition = attach.RenderingPosition;
                    fileInfo = new FileInfo(FileManager.FileExistsMakeNew(outputFolder + attachmentFileName));
                    File.WriteAllBytes(fileInfo.FullName, attach.Data);
                    isInline = attach.IsInline;

                    if (!htmlConvertedFromRtf)
                    {
                        // When we find an inline attachment we have to replace the CID tag inside the html body
                        // with the name of the inline attachment. But before we do this we check if the CID exists.
                        // When the CID does not exists we treat the inline attachment as a normal attachment
                        if (htmlBody && !string.IsNullOrEmpty(attach.ContentId) && body.Contains(attach.ContentId))
                            body = body.Replace("cid:" + attach.ContentId, fileInfo.Name);
                        else
                            // If we didn't find the cid tag we treat the inline attachment as a normal one 
                            isInline = false;
                    }
                }
                // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                else if (attachment is Storage.Message)
                {
                    var msg = (Storage.Message)attachment;
                    attachmentFileName = msg.FileName;
                    renderingPosition = msg.RenderingPosition;

                    fileInfo = new FileInfo(FileManager.FileExistsMakeNew(outputFolder + attachmentFileName));
                    msg.Save(fileInfo.FullName);
                }

                if (fileInfo == null) continue;

                if (!isInline)
                    files.Add(fileInfo.FullName);
                
                // Check if the attachment has a render position. This property is only filled when the
                // body is RTF and the attachment is made inline
                if (htmlBody && renderingPosition != -1)
                {
                    if (!isInline)
                        using (var icon = RtfFileIcon.GetFileIcon(fileInfo.FullName))
                        {
                            var iconFileName = outputFolder + Guid.NewGuid() + ".png";
                            icon.Save(iconFileName, ImageFormat.Png);
                            inlineAttachments.Add(renderingPosition, iconFileName + "|" + attachmentFileName);
                        }
                    else
                        inlineAttachments.Add(renderingPosition, attachmentFileName);    
                }

                if (!isInline)
                {
                    if (htmlBody)
                    {
                        if (hyperlinks)
                            attachments.Add("<a href=\"" + HttpUtility.HtmlEncode(fileInfo.Name) + "\">" +
                                            HttpUtility.HtmlEncode(attachmentFileName) + "</a> (" +
                                            FileManager.GetFileSizeString(fileInfo.Length) + ")");
                        else
                            attachments.Add(HttpUtility.HtmlEncode(attachmentFileName) + " (" +
                                            FileManager.GetFileSizeString(fileInfo.Length) + ")");
                    }
                    else
                        attachments.Add(attachmentFileName + " (" + FileManager.GetFileSizeString(fileInfo.Length) + ")");
                }
            }

            if (htmlBody)
                foreach (var inlineAttachment in inlineAttachments)
                {
                    var names = inlineAttachment.Value.Split('|');

                    if (names.Length == 2)
                        body = ReplaceFirstOccurence(body, rtfInlineObject,
                            "<table style=\"width: 70px; display: inline; text-align: center; font-family: Times New Roman; font-size: 12pt;\"><tr><td><img alt=\"\" src=\"" +
                            names[0] + "\"></td></tr><tr><td>" + HttpUtility.HtmlEncode(names[1]) + "</td></tr></table>");
                    else
                        body = ReplaceFirstOccurence(body, rtfInlineObject, "<img alt=\"\" src=\"" + names[0] + "\">");
                }
        }
        #endregion

        #region GetErrorMessage
        /// <summary>
        /// Get the last know error message. When the string is empty there are no errors
        /// </summary>
        /// <returns></returns>
        public string GetErrorMessage()
        {
            return _errorMessage;
        }
        #endregion

        #region RemoveSingleQuotes
        /// <summary>
        /// Removes trailing en ending single quotes from an E-mail address when they exist
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static string RemoveSingleQuotes(string email)
        {
            if (string.IsNullOrEmpty(email))
                return string.Empty;

            if (email.StartsWith("'"))
                email = email.Substring(1, email.Length - 1);

            if (email.EndsWith("'"))
                email = email.Substring(0, email.Length - 1);

            return email;
        }
        #endregion

        #region IsEmailAddressValid
        /// <summary>
        /// Return true when the E-mail address is valid
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        private static bool IsEmailAddressValid(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return false;

            var regex = new Regex(@"^[a-zA-Z0-9_+&*-]+(?>\.[a-zA-Z0-9_+&*-]+)*@(?>[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,7}$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
            var matches = regex.Matches(emailAddress);

            return matches.Count == 1;
        }
        #endregion

        #region GetEmailSender
        /// <summary>
        /// Change the E-mail sender addresses to a human readable format
        /// </summary>
        /// <param name="message">The Storage.Message object</param>
        /// <param name="convertToHref">When true then E-mail addresses are converted to hyperlinks</param>
        /// <param name="html">Set this to true when the E-mail body format is html</param>
        /// <returns></returns>
        private static string GetEmailSender(Storage.Message message, bool convertToHref, bool html)
        {
            var output = string.Empty;

            if (message == null) return string.Empty;
            
            var tempEmailAddress = message.Sender.Email;
            var tempDisplayName = message.Sender.DisplayName;

            if (string.IsNullOrEmpty(tempEmailAddress) && message.Headers != null && message.Headers.From != null)
                tempEmailAddress = RemoveSingleQuotes(message.Headers.From.Address);
            
            if (string.IsNullOrEmpty(tempDisplayName) && message.Headers != null && message.Headers.From != null)
                tempDisplayName = message.Headers.From.DisplayName;

            var emailAddress = tempEmailAddress;
            var displayName = tempDisplayName;

            // Sometimes the E-mail address and displayname get swapped so check if they are valid
            if (!IsEmailAddressValid(tempEmailAddress) && IsEmailAddressValid(tempDisplayName))
            {
                // Swap them
                emailAddress = tempDisplayName;
                displayName = tempEmailAddress;
            }
            else if (IsEmailAddressValid(tempDisplayName))
            {
                // If the displayname is an emailAddress them move it
                emailAddress = tempDisplayName;
                displayName = tempDisplayName;
            }

            if (string.Equals(emailAddress, displayName, StringComparison.InvariantCultureIgnoreCase))
                displayName = string.Empty;

            if (html)
            {
                emailAddress = HttpUtility.HtmlEncode(emailAddress);
                displayName = HttpUtility.HtmlEncode(displayName);
            }

            if (convertToHref && html && !string.IsNullOrEmpty(emailAddress))
                output += "<a href=\"mailto:" + emailAddress + "\">" +
                          (!string.IsNullOrEmpty(displayName)
                              ? displayName
                              : emailAddress) + "</a>";

            else
            {
                if (!string.IsNullOrEmpty(displayName))
                    output = displayName;

                var beginTag = string.Empty;
                var endTag = string.Empty;
                if (!string.IsNullOrEmpty(displayName))
                {
                    if (html)
                    {
                        beginTag = "&nbsp&lt;";
                        endTag = "&gt;";
                    }
                    else
                    {
                        beginTag = " <";
                        endTag = ">";
                    }
                }

                if (!string.IsNullOrEmpty(emailAddress))
                    output += beginTag + emailAddress + endTag;
            }

            return output;
        }
        #endregion

        #region GetEmailRecipients
        /// <summary>
        /// Change the E-mail sender addresses to a human readable format
        /// </summary>
        /// <param name="message">The Storage.Message object</param>
        /// <param name="convertToHref">When true the E-mail addresses are converted to hyperlinks</param>
        /// <param name="type">Selects the Recipient type to retrieve</param>
        /// <param name="html">Set this to true when the E-mail body format is html</param>
        /// <returns></returns>
        private static string GetEmailRecipients(Storage.Message message,
                                                 Storage.Recipient.RecipientType type,
                                                 bool convertToHref,
                                                 bool html)
        {
            var output = string.Empty;

            var recipients = new List<Recipient>();

            if (message == null)
                return output;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var recipient in message.Recipients)
            {
                // First we filter for the correct recipient type
                if (recipient.Type == type)
                    recipients.Add(new Recipient { EmailAddress = recipient.Email, DisplayName = recipient.DisplayName });
            }

            if (recipients.Count == 0 && message.Headers != null)
            {
                switch (type)
                {
                    case Storage.Recipient.RecipientType.To:
                        if (message.Headers.To != null)
                            recipients.AddRange(message.Headers.To.Select(to => new Recipient {EmailAddress = to.Address, DisplayName = to.DisplayName}));
                        break;

                    case Storage.Recipient.RecipientType.Cc:
                        if (message.Headers.Cc != null)
                            recipients.AddRange(message.Headers.Cc.Select(cc => new Recipient { EmailAddress = cc.Address, DisplayName = cc.DisplayName }));
                        break;

                    case Storage.Recipient.RecipientType.Bcc:
                        if (message.Headers.Bcc != null)
                            recipients.AddRange(message.Headers.Bcc.Select(bcc => new Recipient { EmailAddress = bcc.Address, DisplayName = bcc.DisplayName }));
                        break;
                }
            }

            foreach (var recipient in recipients)
            {
                if (output != string.Empty)
                    output += "; ";

                var tempEmailAddress = RemoveSingleQuotes(recipient.EmailAddress);
                var tempDisplayName = RemoveSingleQuotes(recipient.DisplayName);

                var emailAddress = tempEmailAddress;
                var displayName = tempDisplayName;

                // Sometimes the E-mail address and displayname get swapped so check if they are valid
                if (!IsEmailAddressValid(tempEmailAddress) && IsEmailAddressValid(tempDisplayName))
                {
                    // Swap them
                    emailAddress = tempDisplayName;
                    displayName = tempEmailAddress;
                }
                else if (IsEmailAddressValid(tempDisplayName))
                {
                    // If the displayname is an emailAddress them move it
                    emailAddress = tempDisplayName;
                    displayName = tempDisplayName;
                }

                if (string.Equals(emailAddress, displayName, StringComparison.InvariantCultureIgnoreCase))
                    displayName = string.Empty;

                if (html)
                {
                    emailAddress = HttpUtility.HtmlEncode(emailAddress);
                    displayName = HttpUtility.HtmlEncode(displayName);
                }

                if (convertToHref && html && !string.IsNullOrEmpty(emailAddress))
                    output += "<a href=\"mailto:" + emailAddress + "\">" +
                              (!string.IsNullOrEmpty(displayName)
                                  ? displayName
                                  : emailAddress) + "</a>";

                else
                {
                    if (!string.IsNullOrEmpty(displayName))
                        output += displayName;

                    var beginTag = string.Empty;
                    var endTag = string.Empty;
                    if(!string.IsNullOrEmpty(displayName))
                    {
                        if (html)
                        {
                            beginTag = "&nbsp&lt;";
                            endTag = "&gt;";
                        }
                        else
                        {
                            beginTag = " <";
                            endTag = ">";
                        }
                    }

                    if (!string.IsNullOrEmpty(emailAddress))
                        output += beginTag + emailAddress + endTag;
                }
            }

            return output;
        }
        #endregion

        #region InjectHeader
        /// <summary>
        /// Inject an outlook style header into the top of the html
        /// </summary>
        /// <param name="body"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        private static string InjectHeader(string body, string header)
        {
            var temp = body.ToUpperInvariant();

            var begin = temp.IndexOf("<BODY", StringComparison.Ordinal);

            if (begin > 0)
            {
                begin = temp.IndexOf(">", begin, StringComparison.Ordinal);
                return body.Insert(begin + 1, header);
            }

            return header + body;
        }
        #endregion

        #region GetInnerException
        /// <summary>
        /// Get the complete inner exception tree
        /// </summary>
        /// <param name="e">The exception object</param>
        /// <returns></returns>
        private static string GetInnerException(Exception e)
        {
            var exception = e.Message + "\n";
            if (e.InnerException != null)
                exception += GetInnerException(e.InnerException);
            return exception;
        }
        #endregion
    }
}
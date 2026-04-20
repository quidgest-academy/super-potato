using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

// USE /[MANUAL FOR INCLUDE_MAILER]/

namespace GenioMVC.Helpers
{
	public class Mailer
	{
		public void SendMailExample()
		{
			new Thread(() =>
			{
				// Send the emails here
				CSGenio.core.CSmail mail = new CSGenio.core.CSmail();
				mail.SmtpServer = "your.smtp.server";
				mail.Port = 25;
				mail.User = "username@mail.com";
				mail.Pass = "XXXXXXX";
				mail.AuthType = CSGenio.config.AuthType.BasicAuth;

				mail.From = "senders@address.com";
				mail.To = "receiver@address.com";

				mail.Subject = "Business is Business";

				// You can use string/html on your emails
				mail.Body = @"<body>
								<p>Testing</p>
								I dont think so¶
								</body>";
				mail.Send();

			}).Start();
		}

// USE /[MANUAL FOR MAILER]/
	}
}

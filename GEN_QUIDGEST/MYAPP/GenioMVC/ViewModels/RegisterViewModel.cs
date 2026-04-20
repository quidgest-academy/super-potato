using System;

namespace GenioMVC.ViewModels
{
	public class RegisterViewModel
	{
		public object FormData { get; set; }

		public string partialView { get; set; }

		public string partialViewJS { get; set; }

		public string redirect { get; set; }

		public string DivID { get; set; }

		public int FormDataOrdem { get; set; }

		public int FormPswOrdem { get; set; }

		public object FormPswData { get; set; }

		public string PswpartialView { get; set; }

		public string Pswredirect { get; set; }

		public string PswDivID { get; set; }
	}
}

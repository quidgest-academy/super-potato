using System;

namespace GenioMVC.Models.Exception
{
	public class UserUnavailableException : System.Exception
	{
		public UserUnavailableException() { }

		public UserUnavailableException(string message) : base(message) { }

		public UserUnavailableException(string message, System.Exception inner) : base(message, inner) { }
	}
}

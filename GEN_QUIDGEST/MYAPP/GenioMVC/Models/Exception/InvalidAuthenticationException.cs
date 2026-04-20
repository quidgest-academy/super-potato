using System;

namespace GenioMVC.Models.Exception
{
	public class InvalidAuthenticationException : System.Exception
	{
		public InvalidAuthenticationException() { }

		public InvalidAuthenticationException(string message) : base(message) { }

		public InvalidAuthenticationException(string message, System.Exception inner) : base(message, inner) { }
	}
}

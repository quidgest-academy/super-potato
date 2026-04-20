using System;

namespace GenioMVC.Models.Exception
{
	[Serializable]
	public class ModelNotFoundException : System.Exception
	{
		public ModelNotFoundException(string message) : base(message) { }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GenioMVC.Models
{
	/// <summary>
	/// Represent the report query parameters helper
	/// </summary>
	public class ReportQueryParameter
	{
		/// <summary>
		/// Report preview mode
		/// </summary>
		public Boolean preview { get; set; }

		/// <summary>
		/// Record primary key
		/// </summary>
		public string PKey { get; set; }

		/// <summary>
		/// Report name
		/// </summary>
		public string name { get; set; }
	}
}

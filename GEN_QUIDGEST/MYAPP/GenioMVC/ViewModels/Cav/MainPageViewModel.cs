using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using GenioMVC.Models.Cav;

namespace GenioMVC.ViewModels.Cav
{
	public class MainPageViewModel
	{
		/// <summary>
		/// Table list
		/// </summary>
		public List<CAVTable> Tables { get; set; }

		public string BaseLinkTableId { get; set; }

		public string BaseLinkTableDesc { get; set; }

		public List<List<string>> BaseLinlkUpTables { get; set; }

		public List<List<string>> BaseLinkDownTables { get; set; }

		public MainPageViewModel (List<CAVTable> tables)
		{
			Tables = tables;

			// Order tables 
			Tables.Sort(delegate (CAVTable t1, CAVTable t2) { return String.Compare(t1.Description, t2.Description, StringComparison.InvariantCultureIgnoreCase); });
		}
	}
}

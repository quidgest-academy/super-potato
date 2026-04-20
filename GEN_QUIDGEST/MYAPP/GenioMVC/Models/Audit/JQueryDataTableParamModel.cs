using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GenioMVC.Models
{
	public class JQueryDataTableParamModel
	{
		/// <summary>
		/// Request sequence number sent by DataTable,
		/// same value must be returned in response
		/// </summary>
		public int draw { get; set; }

		/// <summary>
		/// Text used for filtering
		/// </summary>
		public DTSearch search { get; set; }

		/// <summary>
		/// Number of records that should be shown in table
		/// </summary>
		public int length { get; set; }

		/// <summary>
		/// First record that should be shown(used for paging)
		/// </summary>
		public int start { get; set; }

		/// <summary>
		/// Columns in table
		/// </summary>
		public IEnumerable<DTColumn> columns { get; set; }

		/// <summary>
		/// Columns to which ordering should be applied
		/// </summary>
		public IEnumerable<DTOrder> order { get; set; }
	}

	public class DTSearch
	{
		/// <summary>
		/// If the search is regex
		/// </summary>
		public bool regex {get; set;}

		/// <summary>
		/// Search value
		/// </summary>
		public string value {get; set;}
	}

	public class DTColumn
	{
		/// <summary>
		/// Column's data source (if defined)
		/// </summary>
		public string data { get; set; }

		/// <summary>
		/// Column's name
		/// </summary>
		public string name { get; set; }

		/// <summary>
		/// Flag to indicate if this column is orderable
		/// </summary>
		public bool orderable { get; set; }

		/// <summary>
		/// Flag to indicate if this column is searchable
		/// </summary>
		public bool searchable { get; set; }

		/// <summary>
		/// Search to apply to this specific column
		/// </summary>
		public DTSearch search { get; set; }
	}

	public class DTOrder
	{
		/// <summary>
		/// Column index to which ordering should be applied
		/// </summary>
		public int column { get; set; }

		/// <summary>
		/// Ordering direction for this column
		/// </summary>
		public string dir { get; set; }
	}
}

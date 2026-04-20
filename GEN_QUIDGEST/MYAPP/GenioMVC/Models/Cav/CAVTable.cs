using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GenioMVC.Models.Cav
{
	/// <summary>
	/// Represents a CAV table
	/// </summary>
	public class CAVTable
	{
		/// <summary>
		/// Table ID
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Table description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///
		/// </summary>
		public string TableUpId { get; set; }

		/// <summary>
		/// Field list
		/// </summary>
		public List<Field> Fields { get; set; }

		/// <summary>
		/// If this table is below the base, it becomes 'true,' and 'false' for the base table and tables above.
		/// (Prototype for implementation testing)
		/// </summary>
		public bool Down { get; set; }
	}
}

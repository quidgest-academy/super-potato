using System;
using System.Collections.Generic;

namespace GenioMVC.Models.Cav
{
	/// <summary>
	/// Represent a field from a CAV table
	/// </summary>
	public class Field
	{
		/// <summary>
		/// Field identifier
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Table identifier
		/// </summary>
		public string TableId { get; set; }

		/// <summary>
		/// Filed description text
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Filed type
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// List of field array
		/// </summary>
		public List<FieldArray> ArrayElements { get; set; }
	}

	/// <summary>
	/// class that represents a field array
	/// </summary>
	public class FieldArray
	{
		/// <summary>
		///
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		///
		/// </summary>
		public string ArrayId { get; set; }

		/// <summary>
		///
		/// </summary>
		public string Description { get; set; }
	}
}

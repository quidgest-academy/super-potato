using System;

namespace GenioMVC.Models.Navigation
{
	/// <summary>
	/// List of interaction modes for a form
	/// </summary>
	[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
	public enum FormMode
	{
		/// <summary>
		/// Undefined
		/// </summary>
		None = 0,
		/// <summary>
		/// DBEdit List (Menu)
		/// </summary>
		List,
		/// <summary>
		/// The list for consultation only.
		/// </summary>
		ConsultationList,
		/// <summary>
		/// Full Text Search
		/// </summary>
		FullTextSearch,
		/// <summary>
		/// Show
		/// </summary>
		Show,
		/// <summary>
		/// Delete
		/// </summary>
		Delete,
		/// <summary>
		/// New
		/// </summary>
		New,
		/// <summary>
		/// Edit
		/// </summary>
		Edit,
		/// <summary>
		/// Duplicate
		/// </summary>
		Duplicate
	}
}

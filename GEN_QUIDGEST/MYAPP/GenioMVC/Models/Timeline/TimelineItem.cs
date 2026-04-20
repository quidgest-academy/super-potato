using System;
using System.Collections.Generic;
using System.Linq;
using GenioMVC.Models.Navigation;

namespace GenioMVC.Models
{
	/// <summary>
	/// This class provides constant values for different time line styles.
	/// </summary>
	public static class TimeLineStyle
	{
		/// <summary>
		/// Represents the primary time line style
		/// </summary>
		public static readonly string Primary = "P";

		/// <summary>
		/// Represents the dynamic time line style
		/// </summary>
		public static readonly string Dynamic = "D";
	}

	/// <summary>
	/// Class that represents a timeline search
	/// </summary>
	public class TimelineItem
	{
		/// <summary>
		/// Date order field
		/// </summary>
		public Object Data { get; set; }

		/// <summary>
		/// Title field
		/// </summary>
		public string Texto { get; set; }

		/// <summary>
		/// Control identifier
		/// </summary>
		public string Identifier { get; set; }

		/// <summary>
		/// Scale type of search
		/// un - Individual
		/// dd - Dias
		/// ww - Semanas
		/// mm - Meses
		/// yy - Anos
		/// </summary>
		public string Escala { get; set; }
		
		/// <summary>
		/// Type of the Timeline
		/// S - Simple
		/// D - Detailed
		/// </summary>
		public string TipoTimeLine {get;set;}
		
		/// <summary>
		/// Associated icon
		/// </summary>
		public string Icon { get; set; }

		//List of all fields except image kind
		public List<TimelineColumn> Columns { get; set; }

		/// <summary>
		/// list of all image fileds
		/// </summary>
		public List<TimelineColumn> ImagesColumns { get; set; }

		/// <summary>
		/// Help form URL
		/// </summary>
		public ItemActionDescriptor Url { get; set; }

		public string Background { get; set; }

		public bool IsPopupForm { get; set; }

		public string SupportForm { get; set; }

		public string Style { get; set; }

		public TimelineItem()
		{
			this.Style = TimeLineStyle.Primary;
		}
	}

	/// <summary>
	/// Class that represents a column field
	/// </summary>
	public class TimelineColumn
	{
		/// <summary>
		/// Column order
		/// </summary>
		public int Order { get; set; }

		/// <summary>
		/// Label of the column
		/// </summary>
		public string Titulo { get; set; }

		/// <summary>
		/// Icon of the column
		/// </summary>
		public string Icone { get; set; }

		/// <summary>
		/// Text of the column
		/// </summary>
		public string Valor { get; set; }

		/// <summary>
		/// Image content in case the filed is image kind
		/// </summary>
		public byte[] Image { get; set; }

		/// <summary>
		/// Url to zoom in on field image kind
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Field type
		/// </summary>
		public string fieldType { get; set; }
	}
}

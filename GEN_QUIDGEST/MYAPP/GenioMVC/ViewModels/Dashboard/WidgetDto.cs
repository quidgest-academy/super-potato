namespace GenioMVC.ViewModels.Dashboard
{
	public class DashboardSaveRequest
	{
		public List<WidgetDto> Widgets { get; set; }
	}

	public class WidgetDto
	{
		/// <summary>
		/// The widget identifier.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// For widgets with multiple pages, it is the current page.
		/// </summary>
		public string Rowkey { get; set; }

		/// <summary>
		/// Whether the widget is in use or not.
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// The horizontal position of the widget.
		/// </summary>
		public int Hposition { get; set; }

		/// <summary>
		/// The vertical position of the widget.
		/// </summary>
		public int Vposition { get; set; }
	}
}

namespace GenioMVC.ViewModels.Dashboard
{
	/// <summary>
	/// Types of widgets
	/// </summary>
	public enum WidgetType
	{
		Alert,
		Bookmark,
		Custom,
		CustomPaginated,
		Menu
	}

	/// <summary>
	/// Types of refresh modes
	/// </summary>
	public enum WidgetRefreshMode
	{
		None,
		Manual,
		Automatic
	}

	/// <summary>
	/// Instantion method
	/// </summary>
	public enum WidgetInstantionMethod
	{
		Aggregate,
		Split,
		Both
	}

	public class WidgetProperties
	{
		/// <summary>
		/// Unique identifier
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Row key
		/// </summary>
		public string Rowkey { get; set; }

		/// <summary>
		/// The default position of the widget in the dashboard
		/// </summary>
		public int Order { get; set; }

		/// <summary>
		/// The horizontal position of the widget
		/// </summary>
		public int Hposition { get; set; }

		/// <summary>
		/// The vertical position of the widget
		/// </summary>
		public int Vposition { get; set; }

		/// <summary>
		/// The width (units) of the widget
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// The height (units) of the widget
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// Whether the widget can be removed from the dashboard or not
		/// </summary>
		public bool Required { get; set; }

		/// <summary>
		/// Whether the widget is visible by default
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// Whether the widget is visible or not based on a condition
		/// </summary>
		public CSGenio.business.Logical ShowWidget { get; set; }

		/// <summary>
		/// The role required to use the widget
		/// </summary>
		public CSGenio.framework.Role Role;

		/// <summary>
		/// The destination module of the widget that could not be the current one
		/// </summary>
		public string Module { get; set; }
		
		/// <summary>
		/// The title of the widget
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// The group that contains this widget
		/// </summary>
		public string Group { get; set; }
		
		/// <summary>
		/// The style of the widget
		/// </summary>
		public string Style { get; set; }

		/// <summary>
		/// The border style of the widget
		/// </summary>
		public string BorderStyle { get; set; }

		/// <summary>
		/// The refresh mode of the widget
		/// </summary>
		public WidgetRefreshMode RefreshMode { get; set; }

		/// <summary>
		/// The refresh rate of the widget (in seconds)
		/// </summary>
		public int RefreshRate { get; set; }

		/// <summary>
		/// Whether the widget data should be cached or not
		/// </summary>
		public bool UsesCache { get; set; }

		/// <summary>
		/// The time to live of the widget's cache
		/// </summary>
		public int CacheTTL { get; set; }

		/// <summary>
		/// Defines the instantion method of the widget
		/// </summary>
		public WidgetInstantionMethod InstantionMethod { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="WidgetProperties" /> class.
		/// </summary>
		public WidgetProperties()
		{
			Hposition = -1;
			Vposition = -1;
			ShowWidget = true;
			Style = "";
			BorderStyle = "";
		}
	}
}

using System.Text.Json.Serialization;

namespace GenioMVC.Models.Navigation
{
	/// <summary>
	/// Interface Alert
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum AlertType
	{
		success, info, warning, error
	}

	/// <summary>
	/// A client-side alert.
	/// </summary>
	public class Alert
	{
		private AlertType _type;

		public string Idalert { get; set; }

		public float Count { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public AlertClickTarget Target { get; set; }

		public string Type
		{
			get
			{
				switch (_type)
				{
					case AlertType.success:
						return "success";
					case AlertType.info:
						return "info";
					case AlertType.warning:
						return "warning";
					case AlertType.error:
						return "error";
					default:
						return "success";
				}
			}
			set
			{
				switch (value)
				{
					case "success":
						_type = AlertType.success;
						break;
					case "info":
						_type = AlertType.info;
						break;
					case "warning":
						_type = AlertType.warning;
						break;
					case "error":
						_type = AlertType.error;
						break;
					default:
						_type = AlertType.info;
						break;
				}
			}
		}
	}

	/// <summary>
	/// Represents a target page to navigate to
	/// on an alert click.
	/// </summary>
	public struct AlertClickTarget
	{
		public string Name { get; set; }

		public string Type { get; set; }

		public string Id { get; set; }
	}
}

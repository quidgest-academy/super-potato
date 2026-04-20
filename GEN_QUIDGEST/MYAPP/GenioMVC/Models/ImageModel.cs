using System.Text.Json.Serialization;

namespace GenioMVC.Models;

// New properties added here will need to be accounted for in ImageModelJsonConverter.
public class ImageModel
{
	/// <summary>
	/// Store the image date in byte[] format, but will not be serialized directly
	/// </summary>
	[JsonIgnore]
	public byte[] OriginalData { get; private set; } = null;

	[JsonPropertyName("data")]
	public string Data { get; set; }

	[JsonPropertyName("dataFormat")]
	public string DataFormat { get; set; }

	[JsonPropertyName("fileName")]
	public string FileName { get; set; }

	[JsonPropertyName("encoding")]
	public string Encoding { get; set; } = "base64";

	[JsonPropertyName("isThumbnail")]
	public bool IsThumbnail { get; set; }

	/// <summary>
	/// Property for storing the ticket
	/// </summary>
	[JsonPropertyName("ticket")]
	public string Ticket { get; set; } = null;

	/// <summary>
	/// Indicates wheter the byte[] has already been converted to base64 or not.
	/// There can be two scenarios:
	///		1 - The byte[] comes from the Model and is converted during serialization.
	///		2 - The ImageModel was directly assigned, already converted, without the original byte[].
	/// </summary>
	[JsonIgnore]
	public bool IsAlreadyConverted { get { return (OriginalData?.Length > 0 && Data?.Length > 0) || (OriginalData == null && Data?.Length > 0); } }

	/// <summary>
	/// Default constructor
	/// </summary>
	public ImageModel() { }

	/// <summary>
	/// Constructor that accepts byte[] for image data
	/// </summary>
	/// <param name="imageData">Original image data</param>
	public ImageModel(byte[] imageData)
	{
		OriginalData = imageData;
	}

	/// <summary>
	/// Implicit conversion to byte[] (to allow usage as byte[])
	/// </summary>
	/// <param name="imageModel"></param>
	public static implicit operator byte[](ImageModel imageModel)
	{
		// Return the original byte[] data
		return imageModel?.OriginalData;
	}

	/// <summary>
	/// Implicit conversion from byte[] (to create ImageModel from byte[])
	/// </summary>
	/// <param name="data"></param>
	public static implicit operator ImageModel(byte[] data)
	{
		return data != null ? new ImageModel(data) : null;
	}
}
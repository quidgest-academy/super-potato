using System.Drawing.Imaging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using GenioMVC.Models;

namespace GenioMVC;

[AttributeUsage(AttributeTargets.Property)]
public class ImageThumbnailJsonConverterAttribute : JsonConverterAttribute
{
	private readonly int width;
	private readonly int height;
	private readonly bool maintainRatio;
	private readonly bool resizeImage;

	public ImageThumbnailJsonConverterAttribute(int width = 75, int height = 75, bool maintainRatio = true, bool resizeImage = true)
	{
		this.width = width;
		this.height = height;
		this.maintainRatio = maintainRatio;
		this.resizeImage = resizeImage;
	}

	public override JsonConverter CreateConverter(Type typeToConvert)
	{
		if (typeToConvert == typeof(byte[]))
			return new ByteImageJsonConverter(width, height, maintainRatio, resizeImage);
		else if (typeToConvert == typeof(List<byte[]>))
			return new MultipleByteImageJsonConverter(width, height, maintainRatio, resizeImage);
		else if (typeToConvert == typeof(ImageModel))
			return new ImageModelJsonConverter(width, height, maintainRatio, resizeImage);

		throw new ArgumentException($"This converter doesn't support the type '{typeToConvert.Name}'.");
	}
}

public abstract class ImageJsonConverter<T> : JsonConverter<T>
{
	protected readonly int width;
	protected readonly int height;
	protected readonly bool maintainRatio;
	protected readonly bool resizeImage;

	// If the image is an svg or gif, doesn't resize it. Otherwise, the svg won't work and the gif will be a static image.
	protected static readonly string[] notResizableImageFormats = ["xml", "svg+xml", "gif"];

	protected ImageJsonConverter(int width = 75, int height = 75, bool maintainRatio = true, bool resizeImage = true)
	{
		this.width = width;
		this.height = height;
		this.maintainRatio = maintainRatio;
		this.resizeImage = resizeImage;
	}

	public override bool CanConvert(Type typeToConvert)
	{
		return typeof(T) == typeToConvert;
	}

	protected ImageModel GetImageModel(byte[] image, string ticket = null)
	{
		var imageFormat = ImageResizer.GetImageFormat(image);
		bool isThumbnail = false;

		// If the image is an svg or gif, doesn't resize it. Otherwise, the svg won't work and the gif will be a static image.
		// We should think on replacing this "if" by a thumbnail on the database.
		if (resizeImage
			&& image != null && image.Length > 0 && width > 0 && height > 0
			&& !notResizableImageFormats.Contains(imageFormat))
		{
			image = ImageResizer.ResizeImage(image, width, height, maintainRatio);
			isThumbnail = true;
		}

		ImageModel imageModel = null;
		if (image?.Length > 0)
		{
			imageModel = new(image)
			{
				Data = System.Convert.ToBase64String(image),
				DataFormat = imageFormat,
				FileName = "", // TODO: Save the file name and format.
				IsThumbnail = isThumbnail,
				Ticket = ticket
			};
		}

		return imageModel;
	}
}

public class ByteImageJsonConverter : ImageJsonConverter<byte[]>
{
	public ByteImageJsonConverter(int width, int height, bool maintainRatio, bool resizeImage) : base(width, height, maintainRatio, resizeImage) { }

	public override byte[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		// We are never supposed to receive images in byte array format here.
		throw new JsonException("Byte array images are not supported.");
	}

	public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, GetImageModel(value), options);
	}
}

public class MultipleByteImageJsonConverter : ImageJsonConverter<List<byte[]>>
{
	public MultipleByteImageJsonConverter(int width, int height, bool maintainRatio, bool resizeImage) : base(width, height, maintainRatio, resizeImage) { }

	public override List<byte[]>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		// We are never supposed to receive images in byte array format here.
		throw new JsonException("List byte array images are not supported.");
	}

	public override void Write(Utf8JsonWriter writer, List<byte[]> value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, value?.Select(val => GetImageModel(val)).ToList(), options);
	}
}

public class ImageModelJsonConverter : ImageJsonConverter<ImageModel>
{
	public ImageModelJsonConverter(int width, int height, bool maintainRatio, bool resizeImage) : base(width, height, maintainRatio, resizeImage) { }

	public override ImageModel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported.");

		ImageModel imageModel = new();

		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				break;

			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				string propertyName = reader.GetString();

				if (propertyName == "isThumbnail")
					imageModel.IsThumbnail = JsonSerializer.Deserialize<bool>(ref reader, options);
				else
				{
					string propertyValue = JsonSerializer.Deserialize<string>(ref reader, options);

					if (propertyName == "data")
						imageModel.Data = propertyValue;
					else if (propertyName == "dataFormat")
						imageModel.DataFormat = propertyValue;
					else if (propertyName == "fileName")
						imageModel.FileName = propertyValue;
					else if (propertyName == "encoding")
						imageModel.Encoding = propertyValue;
				}
			}
		}

		return imageModel;
	}

	public override void Write(Utf8JsonWriter writer, ImageModel value, JsonSerializerOptions options)
	{
		byte[] image = value.OriginalData ?? (value.Data?.Length > 0 ? System.Convert.FromBase64String(value.Data) : []);
		JsonSerializer.Serialize(writer, GetImageModel(image, value?.Ticket), options);
	}
}

public static class ImageResizer
{
	public static byte[] ResizeImage(byte[] image, int width, int height, bool maintainRatio)
	{
		try
		{
			using System.IO.MemoryStream ms = new(image);
			using System.Drawing.Image originalImage = System.Drawing.Image.FromStream(ms);

			int scaledWidth = width;
			int scaledHeight = height;

			if (maintainRatio)
			{
				decimal scale = Math.Min((decimal)width / originalImage.Width, (decimal)height / originalImage.Height);
				scaledWidth = (int)(originalImage.Width * scale);
				scaledHeight = (int)(originalImage.Height * scale);
			}

			using System.Drawing.Image resizedImage = new System.Drawing.Bitmap(originalImage, new System.Drawing.Size(scaledWidth, scaledHeight));
			return (byte[])new System.Drawing.ImageConverter().ConvertTo(resizedImage, typeof(byte[]));
		}
		catch
		{
			// For other formats (for example: ".webp").
			return image;
		}
	}

	public static string GetImageFormat(byte[]? img)
	{
		string imageFormat = "unknown";

		if (img == null || img.Length == 0)
			return imageFormat;

		// Convert the byte array to a string.
		string fileContent = Encoding.UTF8.GetString(img);

		// SVG is a subset of XML, must be checked first.
		if (IsValidSvg(fileContent))
			imageFormat = "svg+xml";
		// Not an SVG, check if it is regular XML.
		else if (IsValidXml(fileContent))
			imageFormat = "xml";
		// Everything else.
		else
		{
			using System.IO.MemoryStream ms = new(img);

			try
			{
				using System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

				if (ImageFormat.Jpeg.Equals(image.RawFormat))
					imageFormat = "jpeg";
				else if (ImageFormat.Png.Equals(image.RawFormat))
					imageFormat = "png";
				else if (ImageFormat.Gif.Equals(image.RawFormat))
					imageFormat = "gif";
				else if (ImageFormat.Icon.Equals(image.RawFormat))
					imageFormat = "ico";
				else if (ImageFormat.Bmp.Equals(image.RawFormat))
					imageFormat = "bmp";
			}
			catch
			{
				// For other formats (for example: ".webp").
				return imageFormat;
			}
		}

		return imageFormat;
	}

	private static bool IsValidXml(string xmlContent)
	{
		return xmlContent.StartsWith("<?xml");
	}

	private static bool IsValidSvg(string svgContent)
	{
		// Check if the content contains the SVG root element and namespace.
		bool containsSvgRootElement = svgContent.Contains("<svg");
		bool containsSvgNamespace = svgContent.Contains("http://www.w3.org/2000/svg");

		// Check if the file content indicates SVG.
		return containsSvgRootElement && containsSvgNamespace;
	}
}

using System;
using System.IO;
using MimeKit;
using MimeKit.Utils;

namespace CSGenio.core.CSMail;

/// <summary>
/// This class is responsible for managing linked resources (such as embedded images) 
/// that are used in email bodies. It supports loading images from file paths, streams, 
/// or byte arrays and associates them with a Content ID, allowing reference in the email content.
/// </summary>
public class EmailLinkedResource
{
	/// <summary>
	/// The Content ID used to reference the resource in the email body. 
	/// If the resource is null, it returns null.
	/// </summary>
	public string ContentId { get { return Resource?.ContentId; } }

	/// <summary>
	/// The linked resource representing the image.
	/// This is a MimePart object that encapsulates the content of the resource.
	/// </summary>
	public MimePart Resource { get; private set; }

	/// <summary>
	/// Creates a linked resource from a file path.
	/// This constructor loads the file, determines its MIME type based on the file extension,
	/// and sets up the linked resource to be embedded in the email body.
	/// </summary>
	/// <param name="filePath">The full path to the image file.</param>
	/// <param name="contentId">The Content ID for referencing this image in the email body.</param>
	/// <exception cref="ArgumentNullException">Thrown when the file path is null or empty.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the specified file is not found.</exception>
	public EmailLinkedResource(string filePath, string contentId)
	{
		if (string.IsNullOrEmpty(filePath))
			throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");

		if (!File.Exists(filePath))
			throw new FileNotFoundException("File not found.", filePath);

		var fileName = Path.GetFileName(filePath);
		// Determines the MIME type of the image based on its file extension.
		var mimeType = GetMimeType(filePath);
		// Load the file content as a byte array and convert it to a memory stream.
		var fileData = File.ReadAllBytes(filePath);
		var fileStream = new MemoryStream(fileData);

		// Set the linked resource using the file stream, content ID, MIME type, and file name.
		SetResource(fileStream, contentId, mimeType, fileName);
	}

	/// <summary>
	/// Creates a linked resource from a byte array.
	/// This constructor is useful when you already have the image data in memory 
	/// as a byte array and want to embed it in an email.
	/// </summary>
	/// <param name="imageBytes">The byte array representing the file data.</param>
	/// <param name="contentId">The Content ID for referencing this image in the email body.</param>
	/// <param name="mimeType">The MIME type of the linked resource (e.g., "image/jpeg").</param>
	/// <exception cref="ArgumentNullException">Thrown when the byte array is null or empty.</exception>
	public EmailLinkedResource(byte[] imageBytes, string contentId, ContentType mimeType)
	{
		if (imageBytes == null || imageBytes.Length == 0)
			throw new ArgumentNullException(nameof(imageBytes), "Image data cannot be null or empty.");

		var fileStream = new MemoryStream(imageBytes);

		// Set the linked resource using the byte stream, content ID, and MIME type.
		SetResource(fileStream, contentId, mimeType);
	}

	/// <summary>
	/// Creates a linked resource from a stream.
	/// This constructor is useful when you want to stream the image data directly 
	/// from a source without fully loading it into memory.
	/// </summary>
	/// <param name="imageStream">The stream containing the image data.</param>
	/// <param name="contentId">The Content ID for referencing this image in the email body.</param>
	/// <param name="mimeType">The MIME type of the linked resource (e.g., "image/png").</param>
	/// <exception cref="ArgumentNullException">Thrown when the image stream is null.</exception>
	public EmailLinkedResource(Stream imageStream, string contentId, ContentType mimeType)
	{
		if (imageStream == null)
			throw new ArgumentNullException(nameof(imageStream), "Image stream cannot be null.");

		// Set the linked resource using the provided stream, content ID, and MIME type.
		SetResource(imageStream, contentId, mimeType);
	}

	/// <summary>
	/// Helper method to set the linked resource properties, which includes 
	/// creating a MimePart object with appropriate content, content type, 
	/// content disposition, and content transfer encoding.
	/// </summary>
	/// <param name="fileStream">The stream containing the image data.</param>
	/// <param name="contentId">The content identifier used to reference the image in the email body.</param>
	/// <param name="mimeType">The MIME type of the image (defaults to "application/octet-stream" if not provided).</param>
	/// <param name="fileName">The file name of the image (defaults to an empty string if not provided).</param>
	/// <returns>Returns the created MimePart representing the linked resource.</returns>
	public MimePart SetResource(Stream fileStream, string contentId = null, ContentType mimeType = null, string fileName = null)
	{
		// If no MIME type is provided, default to "application/octet-stream".
		mimeType ??= new ContentType("application", "octet-stream");
		fileName ??= string.Empty;

		// Create a new MimePart for the resource with the appropriate properties.
		Resource = new MimePart(mimeType)
		{
			ContentId = contentId ?? MimeUtils.GenerateMessageId(), // Generate a content ID if not provided.
			ContentTransferEncoding = ContentEncoding.Base64, // Use base64 encoding for the resource.
			Content = new MimeContent(fileStream), // Attach the file stream as the resource content.
			ContentDisposition = new ContentDisposition(ContentDisposition.Inline), // Set content disposition as inline.
			FileName = fileName, // Set the file name for the resource.
			ContentLocation = new Uri(fileName, UriKind.Relative) // Set content location for reference.
		};

		// Ensure that the content type includes the file name.
		Resource.ContentType.Name = fileName;

		return Resource;
	}

	/// <summary>
	/// Helper method to determine the MIME type based on the file extension.
	/// Uses the MimeKit library's GetMimeType method to map file extensions to MIME types.
	/// </summary>
	/// <param name="fileName">The file name or path to determine the MIME type for.</param>
	/// <returns>Returns the corresponding MIME type as a ContentType object.</returns>
	/// <exception cref="NotSupportedException">Thrown when the file extension is not recognized or supported.</exception>
	public static ContentType GetMimeType(string fileName)
	{
		// Use MimeTypes class from MimeKit to determine the correct MIME type.
		var mimeType = MimeTypes.GetMimeType(fileName);

		// Parse the MIME type into a ContentType object.
		return ContentType.Parse(mimeType);
	}
}
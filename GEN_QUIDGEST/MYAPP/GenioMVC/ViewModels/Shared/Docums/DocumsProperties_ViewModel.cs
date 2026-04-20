using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
using JsonPropertyName = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.Globalization;

using CSGenio.business;
using CSGenio.framework;
using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels
{
	public class DocumsProperties_ViewModel : ViewModelBase
	{
		[JsonPropertyName("versionId")]
		public string VersionId;

		[JsonPropertyName("documentId")]
		public string DocumentId { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("size")]
		public string Size { get; set; }

		[JsonPropertyName("fileType")]
		public string FileType { get; set; }

		[JsonPropertyName("author")]
		public string Author { get; set; }

		[JsonPropertyName("createdDate")]
		public string CreatedDate { get; set; }

		[JsonPropertyName("version")]
		public string Version { get; set; }

		[JsonPropertyName("editing")]
		public bool Editing { get; set; }

		[JsonPropertyName("editor")]
		public string Editor { get; set; }

		[JsonIgnore]
		public bool IsCurrentUserEditing => m_userContext.User.Name == Editor;

		[JsonIgnore]
		public bool IsEmpty => Versions == null || Versions.Count == 0;

		[JsonPropertyName("versions")]
		public SortedList<string, string> Versions { get; set; }

		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// A call to Init() needs to be manually invoked after this constructor
		/// </summary>
		[Obsolete("For deserialization only")]
		public DocumsProperties_ViewModel() : base(null!) { }

		public DocumsProperties_ViewModel(UserContext userContext) : base(userContext) { }

		public DocumsProperties_ViewModel(UserContext userContext, string versionId, string documentId, string name, string size, string extension, string author, string createdDate, string version, bool editing, string editor, SortedList<string, string> versions) : base(userContext)
		{
			SetProperties(versionId, documentId, name, size, extension, author, createdDate, version, editing, editor, versions);
		}

		public DocumsProperties_ViewModel(UserContext userContext, DBFile file) : base(userContext)
		{
			string fileSize = file.GetSizeUnit();
			string editor = file.CheckoutEditor?.Length > 0 ? file.CheckoutEditor : file.CurrentUser;

			SetProperties(file.Coddocums, file.DocumId, file.Name, fileSize, file.Extension, file.Author, file.CreatedAt, file.Version, file.IsCheckout, editor, file.Versions);
		}

		private void SetProperties(string versionId, string documentId, string name, string size, string extension, string author, string createdDate, string version, bool editing, string editor, SortedList<string, string> versions)
		{
			VersionId = versionId;
			DocumentId = documentId;
			Name = name;
			Size = size;
			FileType = extension;
			Author = author;
			if (!string.IsNullOrEmpty(createdDate))
				CreatedDate = DateTime.Parse(createdDate, CultureInfo.CurrentCulture).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
			Version = version;
			Editing = editing;
			Editor = editor;
			Versions = versions;
		}
	}

	public class DocumsControl_ViewModel : DocumsProperties_ViewModel
	{
		[JsonPropertyName("model")]
		public string Model { get; set; }

		[JsonPropertyName("fieldName")]
		public string FieldName { get; set; }

		[JsonPropertyName("fieldNameFK")]
		public string FieldNameFK { get; set; }

		[JsonPropertyName("modelKey")]
		public string ModelKey { get; set; }

		[JsonPropertyName("usesTemplates")]
		public bool UsesTemplates { get; set; }

		[JsonPropertyName("ticket")]
		public string Ticket { get; set; }

		public DocumsControl_ViewModel(UserContext userContext, string ticket, string model, string fldname, string modelKey, string documentId, string versionId, string name, string size, string extension, string author, string createdDate, string version, bool editing, string editor, SortedList<string, string> versions, bool usesTemplates)
			: base(userContext, versionId, documentId, name, size, extension, author, createdDate, version, editing, editor, versions)
		{
			Ticket = ticket;
			Model = model;
			FieldName = fldname;
			FieldNameFK = fldname + "fk";
			ModelKey = modelKey;
			UsesTemplates = usesTemplates;
		}
	}
}

using CSGenio.framework;

namespace GenioMVC.Helpers
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DocumentAttribute : Attribute
	{
		private string fieldName;
		private bool versioning;
		private bool usesTemplates;
		private bool isRequired;
		private DocumentViewTypeMode viewType;

		public DocumentAttribute(string fieldName, bool versioning, bool usesTemplates = false, bool isRequired = false, DocumentViewTypeMode viewType = DocumentViewTypeMode.Print)
		{
			this.fieldName = fieldName;
			this.versioning = versioning;
			this.usesTemplates = usesTemplates;
			this.isRequired = isRequired;
			this.viewType = viewType;
		}

		public string GetFieldName()
		{
			return this.fieldName;
		}

		public bool UsesVersioning()
		{
			return this.versioning;
		}

		public bool UsesTemplates()
		{
			return this.usesTemplates;
		}

		public bool IsRequired()
		{
			return this.isRequired;
		}

		public DocumentViewTypeMode ViewType()
		{
			return this.viewType;
		}
	}
}

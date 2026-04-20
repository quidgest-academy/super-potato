using JsonPropertyName = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.Collections.Specialized;

using CSGenio.business;
using CSGenio.core.framework.table;
using CSGenio.framework;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.ViewModels
{
	public class DocumsVersionsDBEdit_ViewModel(UserContext userContext, string ticket, string documentId, string tableName, string fieldName) : ViewModelBase(userContext)
	{
		[JsonPropertyName("documentVersions")]
		public TablePartial<Models.Docums> DocumentVersions { get; set; }

		[JsonPropertyName("documentId")]
		public string DocumentId { get; set; } = documentId;

		[JsonPropertyName("ticket")]
		public string Ticket { get; set; } = ticket;

		[JsonPropertyName("tableName")]
		public string TableName { get; set; } = tableName;

		[JsonPropertyName("fieldName")]
		public string FieldName { get; set; } = fieldName;

		public void Load(int numberListItems, NameValueCollection requestValues)
		{
			DocumentVersions = new TablePartial<Models.Docums>();
			CriteriaSet filters = CriteriaSet.And();

			DocumentVersions.SetFilters(bool.Parse(requestValues["_DocumsVersionsDBEdit_tableFilters"] ?? "false"), false);

			CriteriaSet search_filters = ProcessSearchFilters(DocumentVersions, GetSearchColumns(), new TableConfiguration());

			filters.SubSets.Add(search_filters);

			string currentModule = m_userContext.User.CurrentModule;
			if (!m_userContext.User.IsAdmin(currentModule))
				filters.Equal(CSGenioAdocums.FldZzstate, 0);

			filters.Equal(CSGenioAdocums.FldDocumid, DocumentId);

			int pageNumber = !string.IsNullOrEmpty(requestValues["p_DocumsVersionsDBEdit"]) ? int.Parse(requestValues["p_DocumsVersionsDBEdit"]) : 1;
			ColumnSort columnSort = GetRequestSort(DocumentVersions, "s_DocumsVersionsDBEdit", "d_DocumsVersionsDBEdit", requestValues, "docums");

			List<ColumnSort> sorts = [];
			if (columnSort != null)
				sorts.Add(columnSort);
			else
				sorts.Add(new ColumnSort(new ColumnReference(CSGenioAdocums.FldDatacria), SortOrder.Descending));

			FieldRef[] fields = [CSGenioAdocums.FldCoddocums, CSGenioAdocums.FldVersao, CSGenioAdocums.FldNome, CSGenioAdocums.FldTamanho, CSGenioAdocums.FldOpercria, CSGenioAdocums.FldDatacria];
			ListingMVC<CSGenioAdocums> listing = Models.ModelBase.Where<CSGenioAdocums>(m_userContext, false, filters, fields, (pageNumber - 1) * numberListItems, numberListItems, sorts);

			User u = m_userContext.User;
			DocumentVersions.Elements = listing.RowsForViewModel(x => new Models.Docums(m_userContext, x));
			DocumentVersions.SetPagination(pageNumber, listing.NumRegs, listing.HasMore, listing.GetTotal, listing.TotalRecords);
		}

		public List<TableSearchColumn> GetSearchColumns()
		{
			return [
				new TableSearchColumn("ValVersao", CSGenioAdocums.FldVersao, typeof(string), true),
				new TableSearchColumn("ValNome", CSGenioAdocums.FldNome, typeof(string), true),
				new TableSearchColumn("ValTamanho", CSGenioAdocums.FldTamanho, typeof(int), true),
				new TableSearchColumn("ValOpercria", CSGenioAdocums.FldOpercria, typeof(string), true),
				new TableSearchColumn("ValDatacria", CSGenioAdocums.FldDatacria, typeof(string), true)
			];
		}
	}
}

using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
using JsonPropertyName = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.ComponentModel.DataAnnotations;

using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.Models
{
	public class Docums(UserContext userContext, CSGenioAdocums? val = null) : ModelBase(userContext)
	{
		[JsonIgnore]
		public CSGenioAdocums klass = val;

		[Key]
		[JsonPropertyName("id")]
		public string ValCoddocums { get => klass.ValCoddocums; set => klass.ValCoddocums = value; }

		[JsonIgnore]
		public string ValModelkey => klass.ValChave;

		[JsonIgnore]
		public string ValModelname => klass.ValTabela;

		[JsonIgnore]
		public string ValModelfield => klass.ValCampo;

		[JsonIgnore]
		public string ValDocumid { get => klass.ValDocumid; set => klass.ValDocumid = value; }

		[JsonIgnore]
		public byte[] ValDocument { get => klass.ValDocument; set => klass.ValDocument = value; }

		[JsonIgnore]
		public string ValDocpath { get => klass.ValDocpath; set => klass.ValDocpath = value; }

		[JsonPropertyName("fileName")]
		public string ValNome { get => klass.ValNome; set => klass.ValNome = value; }

		[JsonPropertyName("bytes")]
		public string ValTamanho { get => klass.ValTamanho; set => klass.ValTamanho = value; }

		[JsonIgnore]
		public string ValExtensao { get => klass.ValExtensao; set => klass.ValExtensao = value; }

		[JsonPropertyName("author")]
		public string ValOpercria { get => klass.ValOpercria; set => klass.ValOpercria = value; }

		[JsonPropertyName("createdOn")]
		[DataType(DataType.DateTime)]
		public DateTime ValDatacria { get => klass.ValDatacria; set => klass.ValDatacria = value; }

		[JsonPropertyName("version")]
		public string ValVersao { get => klass.ValVersao; set => klass.ValVersao = value; }

		/// <summary>
		/// Search the row by key.
		/// </summary>
		/// <param name="id">The primary key.</param>
		/// <returns>Model or NULL</returns>
		public static Docums Find(string id, UserContext userContext)
		{
			if (string.IsNullOrEmpty(id))
				return null;

			CriteriaSet args = CriteriaSet.And();
			args.Equal(CSGenio.business.CSGenioAdocums.FldCoddocums, id);

			List<Docums> results = Where(userContext, false, args).RowsForViewModel(d => new Docums(userContext, d));
			if (results.Count == 0)
				return null;

			return results.First();
		}

		public static ListingMVC<CSGenioAdocums> Where(UserContext userContext, bool distinct, CriteriaSet args = null, FieldRef[] fields = null, int offset = 0, int numRegs = 0, List<ColumnSort> sorts = null)
		{
			User u = userContext.User;
			PersistentSupport sp = userContext.PersistentSupport;

			args = Docums.AddEPH<CSGenioAdocums>(ref u, args);

			ColumnSort sortPk = new(new ColumnReference(CSGenioAdocums.FldVersao), SortOrder.Descending);
			if (sorts != null && !sorts.Exists(x => x == sortPk))
				sorts.Add(sortPk);

			ListingMVC<CSGenioAdocums> listing = new(fields, sorts, offset, numRegs, distinct, u, false);

			CSGenioAdocums.searchListAdvancedWhere(sp, u, args, listing);

			return listing;
		}
	}
}

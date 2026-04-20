using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

using GenioMVC.Helpers.Cav;
using GenioMVC.Models.Cav;

namespace GenioMVC.ViewModels.Cav
{
	/// <summary>
	/// Represents CAV Viewmodel
	/// </summary>
	public class GlobalViewModel
	{
		/// <summary>
		/// List of tables read from CAV metadata
		/// </summary>
		public List<CAVTable> Tables { get; set; }

		/// <summary>
		/// List of report fields
		/// </summary>
		public List<ReportField> FieldsSelectedList { get; set; }

		/// <summary>
		/// List of report orderby fields
		/// </summary>
		public IEnumerable<string> OrderBySelected { get; set; }

		/// <summary>
		/// List of report conditions
		/// </summary>
		public IEnumerable<string> ConditionsSelected { get; set; }

		/// <summary>
		/// List of report groups
		/// </summary>
		public IEnumerable<string> GroupingsSelected { get; set; }

		/// <summary>
		/// Report definition object
		/// </summary>
		public ReportDefinition Query { get; set; }

		/// <summary>
		/// Repot description (Title)
		/// </summary>
		public string ReportTitle { get; set; }

		/// <summary>
		///
		/// </summary>
		public List<Dictionary<string, List<ReportField>>> Totals { get; set; }

		/// <summary>
		///
		/// </summary>
		public List<List<ReportField>> TotalFieldsPerGroup { get; set; }

		/// <summary>
		/// Initializes a new instance of GlobalViewModel
		/// </summary>
		/// <param name="tables">Cav metadata table list</param>
		/// <param name="query">Report definition object</param>
		public GlobalViewModel(List<CAVTable> tables, ReportDefinition query)
		{
			this.Tables = tables;
			this.Query = query;

			this.FieldsSelectedList = new List<ReportField>();
			Totals = new List<Dictionary<string, List<ReportField>>>();
			TotalFieldsPerGroup = new List<List<ReportField>>();

			//If report is defined
			if (this.Query != null)
			{
				if (this.Query.DetailsGroup != null && this.Query.DetailsGroup.Fields != null)
				{
					// selected fields
					foreach (ReportField field in this.Query.DetailsGroup.Fields.FindAll(x => string.IsNullOrEmpty(x.TotalType)))
						this.FieldsSelectedList.Add(field);

					//  Clobal totalizers
					List<ReportField> totalizadores = this.Query.DetailsGroup.Fields.FindAll(x => !string.IsNullOrEmpty(x.TotalType));


					Dictionary<string, List<ReportField>> totalizadoresGlobais = new Dictionary<string, List<ReportField>>();
					foreach (ReportField f in totalizadores)
					{
						List<ReportField> campos = null;
						if (totalizadoresGlobais.TryGetValue(f.FieldId, out campos))
							campos.Add(f);
						else
							totalizadoresGlobais[f.FieldId] = new List<ReportField>() { f };
					}
					Totals.Add(totalizadoresGlobais);

					// Others totalizers
					if (this.Query.Groups != null)
					{
						foreach (ReportGroup g in this.Query.Groups)
						{
							Dictionary<string, List<ReportField>> totalizadoresGrupo = new Dictionary<string, List<ReportField>>();

							if (g.Fields != null)
							{
								foreach (ReportField f in g.Fields.FindAll(x => !string.IsNullOrEmpty(x.TotalType)))
								{
									List<ReportField> campos = null;
									if (totalizadoresGrupo.TryGetValue(f.FieldId, out campos))
										campos.Add(f);
									else
										totalizadoresGrupo[f.FieldId] = new List<ReportField>() { f };
								}
							}

							Totals.Add(totalizadoresGrupo);
						}

						List<ReportField> start = Query.DetailsGroup.Fields.FindAll(x => string.IsNullOrEmpty(x.TotalType));

						TotalFieldsPerGroup.Add(new List<ReportField>(start));

						foreach (ReportGroup g in this.Query.Groups)
						{
							if (g.Fields != null)
							{
								foreach (var f in g.Fields.FindAll(x => string.IsNullOrEmpty(x.TotalType)))
									start.RemoveAll(x => x.FieldId == f.FieldId);
							}
							TotalFieldsPerGroup.Add(new List<ReportField>(start));
						}
					}
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		public static Dictionary<string, Dictionary<string, string>> types_to_operands = new Dictionary<string, Dictionary<string, string>>()
		{
			{ "A" ,
				new Dictionary<string, string>() {{"EQ", "igual a"},{"GT",  "maior que"},{"GET", "maior ou igual que" },{"LT", "menor que" }, { "LET", "menor ou igual que" }, {"NEQ", "diferente de"}, { "LIKE", "inclui"}, {"BETWEEN", "entre"}, {"IN", "um de"}, {"ISNULL", "é vazio"}, {"ISNOTNULL", "não é vazio"} }},
			{"D",
				new Dictionary<string, string>() {{"EQ", "igual a"},{"GT", "maior que"},{"GET", "maior ou igual que"},{"LT", "menor que"},{"LET", "menor ou igual que"},{"NEQ", "diferente de"},{"BETWEEN", "entre"}, {"IN", "um de"}, {"ISNULL", "é vazio"}, {"ISNOTNULL", "não é vazio"} }},
			{"H",
				new Dictionary<string, string>() {{"EQ", "igual a"},{"GT", "maior que"}, {"GET","maior ou igual que"},{"LT", "menor que"},{"LET", "menor ou igual que"},{"NEQ", "diferente de"}, {"BETWEEN", "entre"}, {"IN", "um de"}, {"ISNULL", "é vazio"}, {"ISNOTNULL", "não é vazio"} }},
			{"B",
				new Dictionary<string, string>() {{"EQ", "igual a"}, {"NEQ", "diferente de"}, {"ISNULL", "é vazio"}, {"ISNOTNULL", "não é vazio"} }},
			{"N",
				new Dictionary<string, string>() {{"EQ", "="},{"GT", ">"},{"GET", ">="},{"LT", "<"},{"LET", "<="},{"NEQ", "diferente de"},{"BETWEEN", "entre"},{"IN", "um de" }, {"ISNULL", "é vazio"}, {"ISNOTNULL", "não é vazio"} }},
			{"T",
				new Dictionary<string, string>() {{"EQ", "igual a"},{"GT", "maior que"},{"GET", "maior ou igual que"},{"LT", "menor que"},{"LET", "menor ou igual que"},{"NEQ", "diferente de"},{"BETWEEN", "entre" }, {"ISNULL", "é vazio"}, {"ISNOTNULL", "não é vazio"} }},
			{"$",
				new Dictionary<string, string>() {{"EQ", "="},{"GT", ">"},{"GET", ">="},{"LT", "<"},{"LET", "<="},{"NEQ", "diferente de"},{"BETWEEN", "entre"},{"IN", "um de"}, {"ISNULL", "é vazio"}, {"ISNOTNULL", "não é vazio"} }},
			{"ARRAY",
				new Dictionary<string, string>() {{"EQ", "igual a"}, {"NEQ", "diferente de"}, {"IN", "um de"}, {"ISNULL", "é vazio"}, {"ISNOTNULL", "não é vazio"} }},
		};

		/// <summary>
		///
		/// </summary>
		public static Dictionary<string, string> order = new Dictionary<string, string>()
		{
			{"ASC", "Asc"},{"DESC", "Desc"}
		};

		/// <summary>
		///
		/// </summary>
		public static Dictionary<string, string> logicos = new Dictionary<string, string>()
		{
			{"0", "Falso"},{"1", "Verdadeiro"}
		};

		/// <summary>
		///
		/// </summary>
		public static Dictionary<string, Dictionary<string, string>> total = new Dictionary<string, Dictionary<string, string>>()
		{
		   {"A",
				new Dictionary<string, string>(){{"COUNT", "TOTAL_DE_ELEMENTOS21962" },{"MAX", "MAXIMO52072" },{"MIN", "MINIMO33485" } }},
		   {"D",
				new Dictionary<string, string>(){{"COUNT", "TOTAL_DE_ELEMENTOS21962" },{"MAX", "MAXIMO52072" },{"MIN", "MINIMO33485" } }},
		   {"H",
				new Dictionary<string, string>(){{"COUNT", "TOTAL_DE_ELEMENTOS21962" },{"MAX", "MAXIMO52072" },{"MIN", "MINIMO33485" } }},
		   {"B",
				new Dictionary<string, string>(){{"SUM", "SOMATORIO37638"},{"AVG", "MEDIA55090"},{"COUNT", "TOTAL_DE_ELEMENTOS21962" },{"MAX", "MAXIMO52072" },{"MIN", "MINIMO33485" } }},
		   {"N",
				new Dictionary<string, string>(){{"SUM", "SOMATORIO37638"},{"AVG", "MEDIA55090"},{"COUNT", "TOTAL_DE_ELEMENTOS21962" },{"MAX", "MAXIMO52072" },{"MIN", "MINIMO33485" } }},
		   {"T",
				new Dictionary<string, string>(){{"COUNT", "TOTAL_DE_ELEMENTOS21962" },{"MAX", "MAXIMO52072" },{"MIN", "MINIMO33485" } }},
		   {"$",
				new Dictionary<string, string>(){{"SUM", "SOMATORIO37638"},{"AVG", "MEDIA55090"},{"COUNT", "TOTAL_DE_ELEMENTOS21962" },{"MAX", "MAXIMO52072" },{"MIN", "MINIMO33485" } }}
		};

		/// <summary>
		///
		/// </summary>
		public static Dictionary<string, string> classes = new Dictionary<string, string>()
		{
			{"D", "date"},{"H", "hour"},{"N","numeric"},{"T","time"},{"$","money"}
		};
	}
}

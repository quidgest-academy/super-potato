using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence.GenericQuery;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace GenioMVC.Models
{
	public class AuditModel : ModelBase
	{
		public AuditModel(UserContext userContext) : base(userContext) { }

		[Display(Name = "SELECIONE_A_TABELA_A28300", ResourceType = typeof(Resources.Resources))]
		public AuditModel.LogTables LogTable { get; set; }

		[Display(Name = "ESTADO07788", ResourceType = typeof(Resources.Resources))]
		public string ResultMsg { get; set; }

		public AuditResult Result { get; set; }

		public int TotalRows { get; set; }

		public int TotalFilteredRows { get; set; }

		public string SelectedRow { get; set; }

		public List<string> SelectedRowValues { get; set; }

		public string Year { get; set; }

		[Display(Name = "DADOS_43180", ResourceType = typeof(Resources.Resources))]
		public bool LogDatabaseSelected { get; set; }

		public bool LogDatabaseExists { get; set; }

		public bool LockTable { get; set; }

		public bool LockRow { get; set; }

		public int MaxLogRowDays { get; set; }

		[JsonIgnore]
		public DataSystemXml DataSystem { get; set; }

		/// <summary>
		/// Operation types
		/// </summary>
		public static Dictionary<string, string> OperationTypes
		{
			get
			{
				return new Dictionary<string, string>
				{
					{ "I", "INSERT" },
					{ "U", "UPDATE" },
					{ "D", "DELETE" }
				};
			}
		}

		/// <summary>
		/// Audit Log views
		/// </summary>
		[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
		public enum LogTables
		{
			[Display(Name = "VER_TODAS10532", ResourceType = typeof(Resources.Resources))]
			logFORall
		}

		public static Dictionary<string, string> LogFields
		{
			get
			{
				return new Dictionary<string, string>
				{
					{ "", "" }
				};
			}
		}

		public void Where(PersistentSupport sp, int offset, int numberListItems, CriteriaSet filter, List<ColumnSort> sortColumns, bool isExport)
		{
			// Retrieve view details
			AreaInfo viewInfo = null;
			string viewName;

			if (LogTable == AuditModel.LogTables.logFORall)
				viewName = LogTable.ToString();
			else
			{
				viewInfo = CSGenio.business.Area.GetInfoArea(LogTable.ToString().ToLower());
				viewName = AuditModel.LogTables.logFORall.ToString() + "_" + LogTable.ToString() + "_VIEW";
			}

			// Filtered search
			var query = new SelectQuery();

			// Select fields
			query.Select(viewName, "date")
				.Select(viewName, "who")
				.Select(viewName, "op")
				.Select(viewName, "cod")
				.From(viewName);

			if (LogTable == AuditModel.LogTables.logFORall)
			{
				query.Select(viewName, "logtable")
					.Select(viewName, "logfield")
					.Select(viewName, "val");
			}
			else if (viewInfo != null)
			{
				foreach (var field in viewInfo.DBFieldsList.Where(field => field.CriaLog))
					query.Select(viewName, string.Format("{0}.{1}", field.Alias, field.Name));
			}

			// Set pagination
			if (!isExport)
			{
				query.Offset(offset);
				query.Page(1);
				query.PageSize(numberListItems);
			}

			// Search filters
			CriteriaSet searchFilter = CriteriaSet.And();
			if (!string.IsNullOrEmpty(SelectedRow))
				searchFilter.Equal(viewName, "cod", SelectedRow);

			if (filter != null)
				searchFilter.SubSet(filter);

			query.Where(searchFilter);

			// Sorting
			if (sortColumns != null)
				query.OrderBy(sortColumns);
			else if (LogTable == AuditModel.LogTables.logFORall)
			{
				query
					.OrderBy(viewName, "date", SortOrder.Descending)
					.OrderBy(viewName, "cod", SortOrder.Descending)
					.OrderBy(viewName, "logtable", SortOrder.Descending)
					.OrderBy(viewName, "op", SortOrder.Descending);
			}
			else
			{
				query
					.OrderBy(viewName, "date", SortOrder.Descending)
					.OrderBy(viewName, "op", SortOrder.Descending);
			}

			// Fill result values
			DataMatrix values = sp.Execute(query);

			Result = new AuditResult { Values = new List<LogRow>(), RawData = values };
			Dictionary<string, string> operationTypes = AuditModel.OperationTypes;

			// No need for value processing nor row count when exporting
			if (isExport)
				return;

			// If the selected database is the log database, human key values are in the system database
			PersistentSupport systemSp = sp;
			if (this.LogDatabaseSelected)
				systemSp = PersistentSupport.getPersistentSupport(this.Year, "");

			for (var i = 0; i < values.NumRows; i++)
			{
				var row = new LogRow() { ColumnValues = new List<string>() };

				// First field is date
				row.ColumnValues.Add(values.GetDate(i, 0).ToString(CultureInfo.CurrentCulture));

				// Secound is who (string)
				row.ColumnValues.Add(values.GetString(i, 1));

				// Third is operation
				row.ColumnValues.Add(operationTypes[values.GetString(i, 2)]);

				// Fourth is primary key
				row.ColumnValues.Add(values.GetString(i, 3));

				// Next columns are strings
				if (viewInfo == null || string.IsNullOrEmpty(SelectedRow))
				{
					for (var j = 4; j < values.NumCols; j++)
						row.ColumnValues.Add(values.GetString(i, j));
				}
				else
				{
					// If row is selected, check for field type
					var camposLog = viewInfo.DBFieldsList.Where(field => field.CriaLog).ToList();
					for (var j = 0; j < camposLog.Count; j++)
					{
						string text = values.GetString(i, j + 4);
						var field = camposLog[j];

						// Change value for special field types
						text = GetHumanValue(systemSp, viewInfo, field, text, m_userContext.User.Language);

						row.ColumnValues.Add(text);
					}
				}

				Result.Values.Add(row);
			}

			// Filtered Count
			TotalFilteredRows = DBConversion.ToInteger(sp.ExecuteScalar(QueryUtils.buildQueryCount(query)));

			// Unfiltered count
			query = new SelectQuery()
				.Select(SqlFunctions.Count(1), "Count")
				.Offset(0)
				.Page(1)
				.PageSize(null)
				.From(viewName);

			TotalRows = DBConversion.ToInteger(sp.ExecuteScalar(query));
		}

		/// <summary>
		/// Fill model result column names
		/// </summary>
		public void FillColumnNames()
		{
			Result.ColumnNames = new List<string>()
			{
				Resources.Resources.DATA18071,
				Resources.Resources.NOME_DE_UTILIZADOR58858,
				Resources.Resources.OPERACAO29482,
				Resources.Resources.CHAVE_PRIMARIA03485
			};

			if (LogTable == AuditModel.LogTables.logFORall)
			{
				Result.ColumnNames.Add(Resources.Resources.TABELA44049);
				Result.ColumnNames.Add(Resources.Resources.CAMPO46284);
				Result.ColumnNames.Add(Resources.Resources.VALOR32448);
			}
			else
			{
				AreaInfo table = CSGenio.business.Area.GetInfoArea(LogTable.ToString().ToLower());
				var logfieldResources = AuditModel.LogFields;
				if (table == null)
					return;

				foreach (var field in table.DBFieldsList.Where(field => field.CriaLog))
				{
					string fieldKey = table.Alias + "." + field.Name;
					string fieldName = logfieldResources.ContainsKey(fieldKey) ? logfieldResources[fieldKey] : "";
					Result.ColumnNames.Add(fieldName);
				}
			}
		}

		/// <summary>
		/// Get model result column definitions
		/// </summary>
		public List<Exports.QColumn> getColumnDefs()
		{
			// Retrieve view details
			AreaInfo viewInfo = null;
			string viewName;

			if (LogTable == AuditModel.LogTables.logFORall)
				viewName = LogTable.ToString();
			else
			{
				viewInfo = CSGenio.business.Area.GetInfoArea(LogTable.ToString().ToLower());
				viewName = AuditModel.LogTables.logFORall.ToString() + "_" + LogTable.ToString() + "_VIEW";
			}

			// The first 4 columns are predefined.
			List<Exports.QColumn> columnDefs = new List<Exports.QColumn>()
			{
				new Exports.QColumn(new Quidgest.Persistence.FieldRef(viewName, "date"), FieldType.DATETIMESECONDS, Resources.Resources.DATA18071, 19, 0, true),
				new Exports.QColumn(new Quidgest.Persistence.FieldRef(viewName, "who"), FieldType.TEXT, Resources.Resources.NOME_DE_UTILIZADOR58858, 50, 0, true),
				new Exports.QColumn(new Quidgest.Persistence.FieldRef(viewName, "op"), FieldType.TEXT, Resources.Resources.OPERACAO29482, 1, 0, true), // TODO: Add array rule to export
				new Exports.QColumn(new Quidgest.Persistence.FieldRef(viewName, "cod"), FieldType.KEY_VARCHAR, Resources.Resources.CHAVE_PRIMARIA03485, 38, 0, true)
			};

			// Other columns vary by view
			if (LogTable == AuditModel.LogTables.logFORall)
			{
				columnDefs.Add(new Exports.QColumn(new Quidgest.Persistence.FieldRef(viewName, "logtable"), FieldType.TEXT, Resources.Resources.TABELA44049, 50, 0, true));
				columnDefs.Add(new Exports.QColumn(new Quidgest.Persistence.FieldRef(viewName, "logfield"), FieldType.TEXT, Resources.Resources.CAMPO46284, 50, 0, true));
				columnDefs.Add(new Exports.QColumn(new Quidgest.Persistence.FieldRef(viewName, "val"), FieldType.MEMO, Resources.Resources.VALOR32448, 50, 0, true));
			}
			else
			{
				AreaInfo table = CSGenio.business.Area.GetInfoArea(LogTable.ToString().ToLower());
				var logfieldResources = AuditModel.LogFields;
				if (table == null)
					return columnDefs;

				foreach (var field in table.DBFieldsList.Where(field => field.CriaLog))
				{
					string fieldKey = table.Alias + "." + field.Name;
					string fieldName = logfieldResources.ContainsKey(fieldKey) ? logfieldResources[fieldKey] : "";
					columnDefs.Add(new Exports.QColumn(new Quidgest.Persistence.FieldRef(viewName, string.Format("{0}.{1}", field.Alias, field.Name)), FieldType.MEMO, fieldName, 50, 0, true));
				}
			}

			return columnDefs;
		}

		/// <summary>
		/// Fill selected row values
		/// </summary>
		/// <param name="sp">PersistentSupport reference</param>
		public void FillSelectedRowValues(PersistentSupport sp)
		{
			AreaInfo area = CSGenio.business.Area.GetInfoArea(LogTable.ToString().ToLower());
			var camposLog = area.DBFieldsList.Where(field => field.CriaLog).ToList();
			var query = new SelectQuery();

			query.Select(area.TableName, area.PrimaryKeyName);

			foreach (var field in camposLog)
				query.Select(area.TableName, field.Name);

			query.From(area.TableName);
			query.Where(CriteriaSet.And().Equal(area.TableName, area.PrimaryKeyName, SelectedRow));

			DataMatrix values = sp.Execute(query);
			SelectedRowValues = new List<string>();

			if (values.NumRows != 1)
				return;

			for (var j = 0; j < camposLog.Count; j++)
			{
				string text = values.GetString(0, j);
				var field = camposLog[j];

				// Change value for sepcial field types
				text = GetHumanValue(sp, area, field, text, m_userContext.User.Language);

				SelectedRowValues.Add(text);
			}
		}

		/// <summary>
		/// Checks for special field types in order to
		/// get the human value for the selected field
		/// </summary>
		/// <param name="sp">PersistentSupport reference</param>
		/// <param name="area">Table area info</param>
		/// <param name="field">Field info</param>
		/// <param name="text">Current text value</param>
		/// <returns>Human value</returns>
		public static string GetHumanValue(PersistentSupport sp, AreaInfo area, CSGenio.framework.Field field, string text, string language)
		{
			if (field.isKey() &&  field.Alias != area.Alias && GenFunctions.emptyG(text) == 0)
			{
				// Foreign keys are replaced by referenced tables' human key
				Relation relation = area.ParentTables.Values.FirstOrDefault(x => x.SourceRelField == field.Name);
				if (relation is null)
					return text;
				AreaInfo table = CSGenio.business.Area.GetInfoArea(relation.AliasTargetTab);

				// There can be multiple fields marked as human key
				var humanKey = table.HumanKeyName.Split(',').ToList();

				// Select query for retrieving human field values
				var humanKeyQuery = new SelectQuery();
				foreach (string humanField in humanKey)
					humanKeyQuery.Select(table.Alias, humanField);

				humanKeyQuery
					.From(table.TableName, table.Alias)
					.Where(CriteriaSet.And().Equal(table.Alias, relation.TargetRelField, text));

				DataMatrix humanValues = sp.Execute(humanKeyQuery);

				// Concatenate human key fields to get human key
				var humanKeyText = "";
				if (humanValues.NumRows == 1)
					for (var k = 0; k < humanValues.NumCols; k++)
						humanKeyText += humanValues.GetString(0, k);

				text = humanKeyText;
			}
			else if ((field.FieldType == FieldType.ARRAY_TEXT ||
					field.FieldType == FieldType.ARRAY_NUMERIC ||
					field.FieldType == FieldType.ARRAY_LOGIC) &&
					!string.IsNullOrEmpty(field.ArrayName) && !string.IsNullOrEmpty(text))
			{
				// Convert array name
				string arrayPrefix = string.Empty;
				if (field.FieldType == FieldType.ARRAY_NUMERIC)
					arrayPrefix = "dbo.GetValArrayN";
				else if (field.FieldType == FieldType.ARRAY_LOGIC)
					arrayPrefix = "dbo.GetValArrayL";
				else
					arrayPrefix = "dbo.GetValArrayC";

				string arrayName = field.ArrayName.Replace(arrayPrefix, "");
				arrayName = char.ToUpper(arrayName[0]) + arrayName.Substring(1);

				try
				{
					// Get array description from value
					var array = new ArrayInfo(arrayName);
					text = array.GetDescription(text, language);
				}
				catch (System.Exception)
				{
					// If unable to get array description, value remains unchanged
				}
			}
			else if (field.FieldType == FieldType.DATE)
			{
				if (!string.IsNullOrEmpty(text))
					text = (Convert.ToDateTime(text)).ToString(Configuration.DateFormat.Date, CultureInfo.InvariantCulture); //standard convertion across platforms. OLD=> text = text.Replace("-", "/").Substring(0, 10);
			}

			return text;
		}
	}

	public class AuditResult
	{
		public List<LogRow> Values { get; set; }

		public List<string> ColumnNames { get; set; }

		[JsonIgnore] // Just used for export
		public DataMatrix RawData { get; set; }
	}

	public class LogRow
	{
		public List<string> ColumnValues { get; set; }
	}
}

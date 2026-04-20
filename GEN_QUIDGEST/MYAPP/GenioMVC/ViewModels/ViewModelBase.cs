using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using CSGenio.business;
using CSGenio.core.framework.table;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Helpers;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.ViewModels
{
	public interface IPreparableForSerialization
	{
		/// <summary>
		/// Method that prepares the ViewModel content to be returned to the client-side.
		///    - Sanitizes the ViewModel content by cleaning HTML fragments and documents from constructs that could lead to XSS attacks and compromise application security.
		///    - Assigns ticket to Image fields.
		/// </summary>
		void PrepareContentForClientSide();
	}

	public interface IViewModel
	{
		// Interface Properties
		[JsonIgnore]
		bool NestedForm { get; set; }

		[JsonIgnore]
		StatusMessage flashMessage { get; set; }

		// Interface Methods
		void setModes(string m);

		StatusMessage CheckPermissions(FormMode mode);
	}

	public abstract class ViewModelBase(UserContext userContext) : IViewModel, IConditionalSerializer
	{
		protected UserContext m_userContext = userContext;

		/// <summary>
		/// Call this method *immediately* after deserializing this object or allocating it with the empty constructor
		/// </summary>
		/// <param name="userContext"></param>
		public virtual void Init(UserContext userContext)
		{
			m_userContext = userContext;

			//initialize context in instance properties of type GridTableList
			//To avoid using reflection:
			// This is likely better done in the generator by overriding the Init method in the instance classes.

			//local function to match generic types
			bool IsClassMapping(Type? t, Type openGeneric)
			{
				while (t != null)
				{
					if (t.IsGenericType &&
						t.GetGenericTypeDefinition() == openGeneric)
						return true;
					t = t.BaseType;
				}
				return false;
			}

			foreach (var p in GetType().GetProperties())
			{
				Type propType = p.PropertyType;
				if (IsClassMapping(propType, typeof(GridTableList<>)))
				{
					var v = p.GetValue(this);
					if (v != null)
						propType.InvokeMember("Init", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, v, new[] { m_userContext });
				}
			}
		}

		/// <summary>
		/// [MH] - [17-08-2015]: temporary bugfix - need be rafactored Identifier of menu
		/// </summary>
		[JsonIgnore]
		public string Identifier { get; set; }

		[JsonIgnore]
		public bool NestedForm { get; set; }

		public bool ShouldSerialize(string tag)
		{
			return FieldsToSerialize?.Contains(tag) ?? false;
		}

		/// <summary>
		/// Gets the list of fields to serialize.
		/// </summary>
		virtual protected string[] FieldsToSerialize { get; }

		#region Form modes

		protected string _modes;
		public string GetQSModes() { return _modes; }

		/// <summary>
		/// Set allowed modes for this form
		/// </summary>
		/// <param name="m">data from quary strign (recived with key "m")</param>
		public void setModes(string m)
		{
			_modes = m ?? string.Empty;
		}

		/// <summary>
		/// Check if determined mode is allowed for this form
		/// </summary>
		/// <param name="cFormMode">FormMode to be validated</param>
		/// <returns></returns>
		public bool checkMode(FormMode cFormMode)
		{
			var allowedMode = false;
			if (string.IsNullOrWhiteSpace(_modes))
				_modes = string.Empty;

			switch (cFormMode)
			{
				case FormMode.Edit:
					allowedMode = _modes.Contains("e"); break;
				case FormMode.New:
					allowedMode = _modes.Contains("i"); break;
				case FormMode.Delete:
					allowedMode = _modes.Contains("a"); break;
				case FormMode.Duplicate:
					allowedMode = _modes.Contains("d"); break;
				case FormMode.Show:
					allowedMode = _modes.Contains("v"); break;
			}

			if (allowedMode)
				allowedMode = CheckVMPermissions(cFormMode);

			if (!allowedMode && this.Navigation != null)
				allowedMode = this.Navigation.CurrentLevel.FormMode == cFormMode;

			return allowedMode;
		}

		[JsonIgnore]
		public bool CanEdit => this.checkMode(FormMode.Edit);

		[JsonIgnore]
		public bool CanInsert => this.checkMode(FormMode.New);

		[JsonIgnore]
		public bool CanDelete => this.checkMode(FormMode.Delete);

		[JsonIgnore]
		public bool CanDuplicate => this.checkMode(FormMode.Duplicate);

		[JsonIgnore]
		public bool CanView => this.checkMode(FormMode.Show);

		#endregion

		#region History

		//MH - refatorização dos historicos
		[JsonIgnore]
		public NavigationContext Navigation => m_userContext?.CurrentNavigation;

		#endregion

		//use the full qualified name to prevent problems with tables with name ROLE
		[JsonIgnore]
		public CSGenio.framework.Role RoleToShow { get; protected set; }

		[JsonIgnore]
		public CSGenio.framework.Role RoleToEdit { get; protected set; }

		//Used by original sorting method. Only returns sorting for 1 column.
		protected ColumnSort GetRequestSort<TModel>(TablePartial<TModel> t, string sortStr, string directionStr, NameValueCollection qs, string area) where TModel: class
		{
			ColumnSort sort = null;

			if (!string.IsNullOrEmpty(qs[sortStr]))
			{
				SortOrder direction = SortOrder.Ascending;
				string dir = (string)qs[directionStr];
				if (!string.IsNullOrEmpty(dir) && dir == "DESC")
					direction = SortOrder.Descending;
				string column = (string)qs[sortStr];

				string[] args = column.Split(['.']);
				Type areaType = null;
				FieldRef fieldRef = null;
				if (args.Count() == 1)
					areaType = CSGenio.business.Area.GetTypeArea(area);
				else if (args.Count() == 2)
					areaType = CSGenio.business.Area.GetTypeArea(args[0].ToLower());
				else if (args.Count() > 2)
					areaType = CSGenio.business.Area.GetTypeArea(args[args.Count() - 2].ToLower());
				if (areaType == null)
					return null;

				string fieldName = args[args.Count() - 1];
				int index = fieldName.IndexOf("Val");
				fieldName = fieldName.Remove(index,3);

				fieldRef = (FieldRef)((PropertyInfo)areaType.GetMember("Fld" + fieldName).GetValue(0)).GetValue(areaType);
				sort = new ColumnSort(new ColumnReference(fieldRef), direction);

				var areaInfo = (CSGenio.business.AreaInfo)areaType.GetMethod("GetInformation").Invoke(areaType, null);
				var field = areaInfo.DBFields[fieldRef.Field];

				if (field.FieldType == FieldType.IMAGE)
					return null;

				t.SetSort(column, dir);
			}

			return sort;
		}

		/// <summary>
		/// Gets the database column name from the form field name.
		/// </summary>
		/// <remarks>FOR: MENU LIST SORTING</remarks>
		/// <param name="formFieldName">Field name in form</param>
		/// <returns>Database column name</returns>
		public string GetDBColumnNameFromFormFieldName(string formFieldName)
		{
			int requestIndex = formFieldName.IndexOf("Val");
			formFieldName = formFieldName.Remove(requestIndex,3);
			return formFieldName.ToUpper();
		}

		/// <summary>
		/// Gets the database column name in FieldRef format.
		/// </summary>
		/// <remarks>FOR: MENU LIST SORTING</remarks>
		/// <param name="columnName">Column name</param>
		/// <returns>Database column name</returns>
		public string GetFieldRefColumnName(string columnName)
		{
			columnName = columnName.ToLower();
			return char.ToUpper(columnName[0]) + columnName.Substring(1);
		}

		/// <summary>
		/// Generates and returns a List<ColumnSorts> with all columns to sort by, in order, based on the column clicked and the data structure that represents all sortings for the menu list.
		/// </summary>
		/// <remarks>FOR: MENU LIST SORTING</remarks>
		/// <param name="t">Menu list</param>
		/// <param name="tableConfig">The table configuration</param>
		/// <param name="area">Table/Area name</param>
		/// <param name="allSortOrders">Structure of all sortings for the menu list, grouped by column name</param>
		/// <returns>List of ColumnSorts in the same order the columns are in the sorting for the column clicked.</returns>
		protected List<ColumnSort> GetRequestSorts<TModel>(TablePartial<TModel> t, TableConfiguration tableConfig, string area, Dictionary<string, OrderedDictionary> allSortOrders) where TModel : class
		{
			// TODO: Start using several columns for the sorting, instead of just the first one found.
			ColumnConfiguration columnConfig = tableConfig.ColumnConfigurations.Find(c => c.SortOrder > 0);

			if (tableConfig.ColumnConfigurations.Count == 0)
				return [];

			List<ColumnSort> allRequestSorts = [];

			// If no sorting is defined, use the first column.
			columnConfig ??= tableConfig.ColumnConfigurations[0];

			//< Get name, sort direction, area of column clicked
			string requestColumn = columnConfig.Name;
			string requestDir = columnConfig.SortAsc ? "ASC" : "DESC";

			string[] requestArgs = requestColumn.Split(['.']);
			string requestFieldName = GetDBColumnNameFromFormFieldName(requestArgs[requestArgs.Count() - 1]);

			string requestArea = area.ToUpper();
			if (requestArgs.Count() > 1)
				requestArea = requestArgs[requestArgs.Count() - 2].ToUpper();

			string requestFieldNameFull = requestArea + "." + requestFieldName;
			//> Get name, sort direction, area of column clicked

			t.SetSort(requestColumn, requestDir);

			//If requested column is not in the sorting dictionary, add a sorting by the requested column only.
			if (!allSortOrders.ContainsKey(requestFieldNameFull))
			{
				OrderedDictionary requestColumnOrder = new()
				{
					{ requestFieldNameFull, "A" }
				};
				allSortOrders.Add(requestFieldNameFull, requestColumnOrder);
			}

			//Iterate through OrderedDictionary of column clicked
			foreach (DictionaryEntry sortOrderEntry in allSortOrders[requestFieldNameFull])
			{
				//< Get name, sort direction, area of column in this sorting
				string column = (string)sortOrderEntry.Key;
				string dir = (string)sortOrderEntry.Value;

				//For the column that was clicked, use sort direction passed in
				if (string.Equals(column, requestFieldNameFull))
					dir = requestDir;

				SortOrder direction = SortOrder.Ascending;
				if (!string.IsNullOrEmpty(dir) && (dir == "DESC" || dir == "D"))
					direction = SortOrder.Descending;

				//Get area type
				string[] args = column.Split(['.']);
				Type areaType = null;
				if (args.Count() == 1)
					areaType = CSGenio.business.Area.GetTypeArea(area);
				else if (args.Count() > 1)
					areaType = CSGenio.business.Area.GetTypeArea(args[args.Count() - 2].ToLower());
				if (areaType == null)
					continue;
				//> Get name, sort direction, area of column in this sorting

				//Get column name in FieldRef style
				string fieldName = GetFieldRefColumnName(args[args.Count() - 1]);

				//Check MemberInfo to avoid trying to access undefined members
				MemberInfo[] areaTypeInfo = areaType.GetMember("Fld" + fieldName);
				if (areaTypeInfo == null || areaTypeInfo.Length < 1)
					continue;

				//< Create column reference and check if sortable
				FieldRef fieldRef = (FieldRef)((PropertyInfo)areaType.GetMember("Fld" + fieldName).GetValue(0)).GetValue(areaType);

				var areaInfo = (AreaInfo)areaType.GetMethod("GetInformation").Invoke(areaType, null);
				var field = areaInfo.DBFields[fieldRef.Field];

				//Column types that are not sorted
				if (field.FieldType == FieldType.IMAGE)
					continue;
				//> Create column reference and check if sortable

				allRequestSorts.Add(new ColumnSort(new ColumnReference(fieldRef), direction));
			}

			return allRequestSorts;
		}

		/// <summary>
		/// Determines if a column is visible, accounting for the user column configuration.
		/// </summary>
		/// <param name="searchColumn">The table column.</param>
		/// <param name="columnConfig">Column configuration specified by the user.</param>
		protected bool IsColumnVisible(TableSearchColumn searchColumn, List<ColumnConfiguration> userColumns)
		{
			// If there is a user column, use the visibility from the user column, otherwise use the TableSearchColumn value
			if (userColumns != null)
			{
				// MH (10/03/2020) - The "Replace('.', '_')" is necessary because the column identifiers in the TableSearchColumn follow the same logic (if I'm not mistaken because of JavaScript).
				var userColumn = userColumns.Find(uc => searchColumn.Field.Equals(uc.Name.Replace('.', '_')));
				if (userColumn != null)
					return userColumn.Visibility == 1;
			}

			return searchColumn.Visible;
		}

		protected virtual List<Field> GlobalFilters { get; }

		/// <summary>
		/// Process the form field filters that affect a table list.
		/// </summary>
		/// <param name="fieldFilters">Dictionary of global filter fields and their respective values.</param>
		/// <returns>The set of conditions determined by the filter values.</returns>
		protected CriteriaSet ProcessFieldFilters(Dictionary<string, IGlobalFilter> fieldFilters)
		{
			CriteriaSet criteria = CriteriaSet.And();

			if (GlobalFilters == null || fieldFilters == null || fieldFilters.Count == 0)
				return criteria;

			foreach (Field field in GlobalFilters)
			{
				if (fieldFilters.TryGetValue(field.FullName, out IGlobalFilter filter))
				{
					ColumnReference colRef = new(field.Alias, field.Name);
					if (filter.Value is ICollection collection)
					{
						if (collection.Count == 0)
							continue;

						// If the filter is an array of multiple values, process each one into an "Or" criteria set
						CriteriaSet orCriteria = CriteriaSet.Or();
						foreach (object val in collection)
							orCriteria.Equal(colRef, QueryUtils.ToValidDbValue(val, field));

						criteria.SubSets.Add(orCriteria);
					}
					else
					{
						object filterVal = QueryUtils.ToValidDbValue(filter.Value, field);
						// If empty, should not filter anything.
						if (field.isEmptyValue(filterVal) && !filter.IsArrayOption(filterVal))
							continue;

						if (field.FieldType == FieldType.TEXT || field.FieldType == FieldType.MEMO)
						{
							// TODO: the default should probably be a "startsWith", but for now we're using a "contains",
							// to be compatible with the default search of table lists.
							criteria.Like(colRef, $"%{filterVal}%");
						}
						else
							criteria.Equal(colRef, filterVal);
					}
				}
			}

			return criteria;
		}

		/// <summary>
		/// Process the search filters (advanced filters, column filters, searchbar filters) from a table
		/// </summary>
		/// <typeparam name="A"></typeparam>
		/// <param name="menu">Render helper object</param>
		/// <param name="searchColumns">Searchable columns in the table</param>
		/// <param name="tableConfig">Table configuration object</param>
		/// <returns>A set of conditions</returns>
		protected CriteriaSet ProcessSearchFilters<A>(TablePartial<A> menu, List<TableSearchColumn> searchColumns, TableConfiguration tableConfig) where A : class
		{
			// Create dictionary of search columns using full names as keys (TABLE.COLUMN)
			Dictionary<string, TableSearchColumn> searchColumnsDic = [];
			foreach (TableSearchColumn tsc in searchColumns)
				searchColumnsDic.Add(tsc.AreaField.FullName.ToUpper(), tsc);

			List<ColumnFilter> validSearchFilters = tableConfig.GetValidSearchFilters(menu.TableName, searchColumnsDic.Keys.ToList());
			CriteriaSet searchFilters = CriteriaSet.And();

			if (validSearchFilters != null)
			{
				foreach (ColumnFilter filter in validSearchFilters)
				{
					// If the condition is inactive, or it's a global filter, continue
					if (!filter.IsActive || string.IsNullOrWhiteSpace(filter.Field))
						continue;

					// If the column doesn't exist anymore in the list
					if (!searchColumnsDic.ContainsKey(filter.Field))
					{
						filter.IsActive = false;
						continue;
					}

					CriteriaSet conditions = CriteriaSet.Or();
					TableSearchColumn sc = searchColumnsDic[filter.Field];
					Field fieldInfo = CSGenio.business.Area.GetFieldInfo(sc.AreaField);

					if (sc.FieldType.Equals(typeof(DateTime?)))
					{
						// Values must be an array because the number of values depends on the operation
						DateTime[] Values = new DateTime[filter.Values.Count];
						DateTime parsedValue = new();
						int x = 0;
						foreach (string value in filter.Values)
							if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedValue) && GenFunctions.emptyD(parsedValue) == 0)
								Values[x++] = parsedValue;

						// Create criteria based on operator code
						switch (filter.Operator)
						{
							case "BETW":
								if (x < 2)
									continue;
								CriteriaSet between = CriteriaSet.And();
								between.GreaterOrEqual(sc.AreaField, Values[0]);
								between.LesserOrEqual(sc.AreaField, Values[1]);
								conditions.SubSets.Add(between);
								break;
							case "EQ":
								if (x < 1)
									continue;
								// The types Date, DateTime and DateTimeSeconds are all stored as full date times, including fractional seconds
								// To account for this, comparisons for equals and not equals must use a range
								// with the lower bound rounded down to the unit of highest precision being compared
								// and the upper bound one unit higher than the lower bound
								if (fieldInfo.FieldType.GetFormatting() == FieldFormatting.DATASEGUNDO)
								{
									CriteriaSet eqRange = CriteriaSet.And();
									eqRange.GreaterOrEqual(sc.AreaField, Values[0]);
									eqRange.Lesser(sc.AreaField, Values[0].AddSeconds(1));
									conditions.SubSets.Add(eqRange);
								}
								else if (fieldInfo.FieldType.GetFormatting() == FieldFormatting.DATAHORA)
								{
									CriteriaSet eqRange = CriteriaSet.And();
									eqRange.GreaterOrEqual(sc.AreaField, Values[0]);
									eqRange.Lesser(sc.AreaField, Values[0].AddMinutes(1));
									conditions.SubSets.Add(eqRange);
								}
								else if (fieldInfo.FieldType.GetFormatting() == FieldFormatting.DATA)
								{
									CriteriaSet eqRange = CriteriaSet.And();
									eqRange.GreaterOrEqual(sc.AreaField, Values[0]);
									eqRange.Lesser(sc.AreaField, Values[0].AddDays(1));
									conditions.SubSets.Add(eqRange);
								}
								else
									conditions.Equal(sc.AreaField, Values[0]);
								break;
							case "NOTEQ":
								if (x < 1)
									continue;
								// The types Date, DateTime and DateTimeSeconds are all stored as full date times, including fractional seconds
								// To account for this, comparisons for equals and not equals must use a range
								// with the lower bound rounded down to the unit of highest precision being compared
								// and the upper bound one unit higher than the lower bound
								if (fieldInfo.FieldType.GetFormatting() == FieldFormatting.DATASEGUNDO)
								{
									CriteriaSet eqRange = CriteriaSet.And();
									eqRange.Lesser(sc.AreaField, Values[0]);
									eqRange.GreaterOrEqual(sc.AreaField, Values[0].AddSeconds(1));
									conditions.SubSets.Add(eqRange);
								}
								else if (fieldInfo.FieldType.GetFormatting() == FieldFormatting.DATAHORA)
								{
									CriteriaSet eqRange = CriteriaSet.And();
									eqRange.Lesser(sc.AreaField, Values[0]);
									eqRange.GreaterOrEqual(sc.AreaField, Values[0].AddMinutes(1));
									conditions.SubSets.Add(eqRange);
								}
								else if (fieldInfo.FieldType.GetFormatting() == FieldFormatting.DATA)
								{
									CriteriaSet eqRange = CriteriaSet.And();
									eqRange.Lesser(sc.AreaField, Values[0]);
									eqRange.GreaterOrEqual(sc.AreaField, Values[0].AddDays(1));
									conditions.SubSets.Add(eqRange);
								}
								else
									conditions.NotEqual(sc.AreaField, Values[0]);
								break;
							case "AFT":
								if (x < 1)
									continue;
								conditions.Greater(sc.AreaField, Values[0]);
								break;
							case "BEF":
								if (x < 1)
									continue;
								conditions.Lesser(sc.AreaField, Values[0]);
								break;
							case "AFTEQ":
								if (x < 1)
									continue;
								conditions.GreaterOrEqual(sc.AreaField, Values[0]);
								break;
							case "BEFEQ":
								if (x < 1)
									continue;
								conditions.LesserOrEqual(sc.AreaField, Values[0]);
								break;
							case "SET":
								conditions.NotEqual(sc.AreaField, null);
								break;
							case "NOTSET":
								conditions.Equal(sc.AreaField, null);
								break;
						}
					}
					else if (sc.FieldType.Equals(typeof(bool)))
					{
						switch (filter.Operator)
						{
							case "TRUE":
								conditions.Equal(sc.AreaField, 1);
								break;
							case "FALSE":
								conditions.Equal(sc.AreaField, 0);
								break;
						}
					}
					else if (sc.FieldType.Equals(typeof(decimal?)))
					{
						// Values must be an array because the number of values depends on the operation
						decimal[] values = new decimal[filter.Values.Count];
						int x = 0;
						bool valueOutOfFieldRangeFound = false;

						foreach (decimal value in filter.Values.Select(Convert.ToDecimal))
						{
							string[] valueParts = value.ToString(CultureInfo.InvariantCulture).Split('.');
							string integerDigits = valueParts[0];
							string decimalDigits = valueParts.Length > 1 ? valueParts[1] : "";

							// Verify if the number of the search value is more than what the field allows
							if ((integerDigits != null && integerDigits.Length > fieldInfo.IntegerDigits) ||
								(decimalDigits != null && decimalDigits.Length > fieldInfo.Decimals))
							{
								valueOutOfFieldRangeFound = true;
							}
							values[x++] = value;
						}

						switch (filter.Operator)
						{
							case "EQ":
								// If the search value is more than what the field allows, no results will be returned.
								if (valueOutOfFieldRangeFound || x < 1)
									conditions.Equal("1", "0");
								else
									conditions.Equal(sc.AreaField, values[0].ToString(CultureInfo.InvariantCulture));
								break;
							case "NOTEQ":
								if (x < 1)
									continue;
								conditions.NotEqual(sc.AreaField, values[0].ToString(CultureInfo.InvariantCulture));
								break;
							case "GREAT":
								if (x < 1)
									continue;
								conditions.Greater(sc.AreaField, values[0].ToString(CultureInfo.InvariantCulture));
								break;
							case "LESS":
								if (x < 1)
									continue;
								conditions.Lesser(sc.AreaField, values[0].ToString(CultureInfo.InvariantCulture));
								break;
							case "GREATEQ":
								if (x < 1)
									continue;
								conditions.GreaterOrEqual(sc.AreaField, values[0].ToString(CultureInfo.InvariantCulture));
								break;
							case "LESSEQ":
								if (x < 1)
									continue;
								conditions.LesserOrEqual(sc.AreaField, values[0].ToString(CultureInfo.InvariantCulture));
								break;
							case "BETW":
								if (x < 2)
									continue;
								CriteriaSet between = CriteriaSet.And();
								between.GreaterOrEqual(sc.AreaField, values[0].ToString(CultureInfo.InvariantCulture));
								between.LesserOrEqual(sc.AreaField, values[1].ToString(CultureInfo.InvariantCulture));
								conditions.SubSets.Add(between);
								break;
							case "SET":
								conditions.NotEqual(sc.AreaField, null);
								break;
							case "NOTSET":
								conditions.Equal(sc.AreaField, null);
								break;
						}
					}
					else if (!string.IsNullOrEmpty(sc.ArrayName))
					{
						// Get enumeration dictionary
						ArrayInfo arrayInfo = new(StringUtils.CapFirst(sc.ArrayName));

						// Get enumeration codes
						// Values must be an array because the number of values depends on the operation
						List<string> values = [];
						foreach (var value in filter.Values)
						{
							List<string> stringValues = [];

							if (value is List<object> list)
							{
								foreach (object item in list)
									stringValues.Add(item.ToString());
							}
							else
								stringValues.Add(value.ToString());

							foreach (string stringValue in stringValues)
								if (arrayInfo.Elements.Contains(stringValue))
									values.Add(stringValue);
						}

						switch (filter.Operator)
						{
							case "IS":
								if (values.Count < 1)
								{
									// If the user used the search bar but searched for something that doesn't match any element of the enum
									// add this condition to make it return no data instead of "ignoring" the user's search and returning all data that matches the other filters
									conditions.NotEqual(sc.AreaField, sc.AreaField);
									continue;
								}
								conditions.Equal(sc.AreaField, values[0]);
								break;
							case "ISNOT":
								if (values.Count < 1)
									continue;
								conditions.NotEqual(sc.AreaField, values[0]);
								break;
							case "IN":
								if (values.Count < 1)
									continue;
								conditions.In(sc.AreaField, values);
								break;
							case "SET":
								conditions.In(sc.AreaField, arrayInfo.Elements);
								break;
							case "NOTSET":
								conditions.NotIn(sc.AreaField, arrayInfo.Elements);
								conditions.Equal(sc.AreaField, null);
								break;
						}
					}
					else
					{
						switch (filter.Operator)
						{
							case "LIKE":
								conditions.Like(sc.AreaField, filter.Values[0]);
								break;
							case "STRTWTH":
								conditions.Like(sc.AreaField, filter.Values[0] + "%");
								break;
							case "CON":
								conditions.Like(sc.AreaField, "%" + filter.Values[0] + "%");
								break;
							case "NOTCON":
								conditions.NotLike(sc.AreaField, "%" + filter.Values[0] + "%");
								break;
							case "EQ":
								conditions.Equal(sc.AreaField, filter.Values[0]);
								break;
							case "NOTEQ":
								conditions.NotEqual(sc.AreaField, filter.Values[0]);
								break;
							case "SET":
								CriteriaSet and = CriteriaSet.And();
								and.NotEqual(sc.AreaField, null);
								and.NotEqual(sc.AreaField, "");
								conditions.SubSets.Add(and);
								break;
							case "NOTSET":
								conditions.Equal(sc.AreaField, null);
								conditions.Equal(sc.AreaField, "");
								break;
						}
					}

					if (filter.BinaryOperator == FilterBinaryOperator.OR)
						searchFilters.SubSets.Last().SubSets.Add(conditions);
					else
						searchFilters.SubSets.Add(conditions);
				}
			}

			// If the user wants to search all columns
			string query = menu.Filters.Query = menu.Query;
			List<string> globalFilters = validSearchFilters
				.Where(f => string.IsNullOrWhiteSpace(f.Field))
				.SelectMany(f => f.Values.Select(v => v.ToString()))
				.ToList();

			if (!string.IsNullOrWhiteSpace(query))
				globalFilters.Add(query);
			if (globalFilters.Count > 0)
				searchFilters.SubSets.Add(SearchAllColumns(searchColumns, globalFilters));

			return searchFilters;
		}

		/// <summary>
		/// Builds a criteria set that searches all given columns for a given query
		/// </summary>
		/// <param name="searchColumns">The list of columns to search</param>
		/// <param name="queries">A list of strings to search</param>
		/// <returns>A criteria set with all the given columns</returns>
		private static CriteriaSet SearchAllColumns(List<TableSearchColumn> searchColumns, List<string> queries)
		{
			CriteriaSet searchFilters = CriteriaSet.Or();

			foreach (string query in queries)
			{
				foreach (TableSearchColumn sc in searchColumns)
				{
					if (sc.FieldType.Equals(typeof(DateTime?)))
					{
						if (DateTime.TryParse(query, Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out DateTime t) && GenFunctions.emptyD(t) == 0)
							searchFilters.Equal(sc.AreaField, t);
					}
					else if (!string.IsNullOrEmpty(sc.ArrayName))
					{
						var arrayInfo = new ArrayInfo(sc.ArrayName);
						var objectDic = arrayInfo.GetDictionaryObject();

						Dictionary<string, string> dic;

						// For normal enums the text exists in the resources, but for dynamic enums those texts don't exist.
						// So, when the value is null, we add the key as the value to ensure searches with the search bar work properly.
						if (objectDic is Dictionary<string, string>)
							dic = (objectDic as Dictionary<string, string>).ToDictionary(p => p.Key, p => Helpers.Helpers.GetTextFromResources(p.Value) ?? p.Key);
						else if (objectDic is Dictionary<int, string>)
							dic = (objectDic as Dictionary<int, string>).ToDictionary(p => p.Key.ToString(), p => Helpers.Helpers.GetTextFromResources(p.Value) ?? p.Key.ToString());
						else
							dic = (objectDic as Dictionary<decimal, string>).ToDictionary(p => p.Key.ToString(), p => Helpers.Helpers.GetTextFromResources(p.Value) ?? p.Key.ToString());

						foreach (var pair in dic)
							if (pair.Value.ToLower().Contains(query.ToLower()))
								searchFilters.Equal(sc.AreaField, pair.Key);
					}
					else if (sc.FieldType.Equals(typeof(decimal?)))
					{
						decimal value = 0;
						if (decimal.TryParse(query, NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out value))
							searchFilters.Like(SqlFunctions.Cast(SqlFunctions.Cast(sc.AreaField, CustomDbType.StandardDecimalSearch), CustomDbType.StandardAnsiString), "%" + value.ToString(CultureInfo.InvariantCulture) + "%");
					}
					else
						searchFilters.Like(sc.AreaField, "%" + query + "%");
				}
			}

			return searchFilters;
		}

		/// <summary>
		/// Process the active filter from a table
		/// </summary>
		/// <typeparam name="A"></typeparam>
		/// <param name="menu">Render helper object</param>
		/// <param name="tableConfig">The table configuration</param>
		/// <returns>A builded condition</returns>
		protected CriteriaSet ProcessActiveFilter<A>(TablePartial<A> menu, TableConfiguration tableConfig)
		{
			// Assumes there's at most one active filter (currently only one can be defined through Genio)
			ActiveFilter activeFilter = tableConfig.ActiveFilters.FirstOrDefault();

			CriteriaSet filters = CriteriaSet.And();
			DateTime hojeDt = DateTime.Today;
			bool activo = false;
			bool inactivo = false;
			bool futuro = false;

			// Set active = true, at first load
			if (activeFilter == null)
				activo = true;
			else
			{
				activo = activeFilter.Current;
				inactivo = activeFilter.Previous;
				futuro = activeFilter.Upcoming;
				DateTime.TryParse(activeFilter.Date, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out hojeDt);
			}

			FieldRef datainiColumn = menu.Filters.FilterDateStart;
			FieldRef datafimColumn = menu.Filters.FilterDateEnd;

			// There are 8 diferent cases
			int value = 0;
			if (activo)
				value += 1;
			if (inactivo)
				value += 2;
			if (futuro)
				value += 4;

			switch (value)
			{
				case 0:
					{
						//Estados incongruentes (Data de saída inferior à data de entrada)
						filters.Lesser(datafimColumn, datainiColumn);
						return filters;
					}
				case 1:
					{
						//So activos
						filters.SubSet(CriteriaSet.Or()
								.GreaterOrEqual(hojeDt, datainiColumn)
								.Equal(datainiColumn, null))
							.SubSet(CriteriaSet.Or()
								.LesserOrEqual(hojeDt, datafimColumn)
								.Equal(datafimColumn, null));

						return filters;
					}
				case 2:
					{
						//So inactivos
						filters.Greater(hojeDt, datafimColumn)
							.NotEqual(datafimColumn, null);

						return filters;
					}
				case 3:
					{
						//So activos e inactivos
						filters.SubSet(CriteriaSet.Or()
							.GreaterOrEqual(hojeDt, datainiColumn)
							.Equal(datainiColumn, null));

						return filters;
					}
				case 4:
					{
						//So futuros
						filters.SubSet(CriteriaSet.Or()
							.Lesser(hojeDt, datainiColumn) // data actual inferior à data de início
							.SubSet(CriteriaSet.And() // data de fim é superior à actual e a de início não exists
								.Greater(datafimColumn, hojeDt)
								.Equal(datainiColumn, null))
							.SubSet(CriteriaSet.And() // data de início e de fim vazias
								.Equal(datainiColumn, null)
								.Equal(datafimColumn, null)));

						return filters;
					}
				case 5:
					{
						//So activos e futuros
						filters.SubSet(CriteriaSet.Or()
							.LesserOrEqual(hojeDt, datafimColumn)
							.Equal(datafimColumn, null));

						return filters;
					}
				case 6:
					{
						//So inactivos e futuros
						filters.SubSet(CriteriaSet.Or()
							.Lesser(hojeDt, datainiColumn)
							.SubSet(CriteriaSet.And()
								.Greater(hojeDt, datafimColumn)
								.NotEqual(datafimColumn, null)));

						return filters;
					}
				case 7:
					{
						//Todos, nao limita nada
						return filters;
					}
				default:
					break;
			}

			return filters;
		}

		protected CriteriaSet GetConditionsToNN(AreaRef table, FieldRef tableKey, AreaRef tableNN, AreaRef otherTable, FieldRef otherTableKey, string otherTableSelectedValue, string identifier = "")
		{
			//old call
			return GetConditionsToNN(table, tableKey, tableNN, otherTable, otherTableKey, otherTableSelectedValue, null, null, null, false, identifier);
		}

		protected CriteriaSet GetConditionsToNN(AreaRef table, FieldRef tableKey, AreaRef tableNN, AreaRef otherTable, FieldRef otherTableKey, string otherTableSelectedValue, AreaRef areaCompare, FieldRef areaCompareKey, string areaCompareSelectedValue, bool naoAplicaSeNulo, string identifier = "")
		{
			CriteriaSet criteria = CriteriaSet.And();
			SelectQuery qs = null;

			CSGenio.business.AreaInfo NN_AreaInfo = CSGenio.business.Area.GetInfoArea(tableNN.Alias.ToLower());
			CSGenio.framework.Relation NN_relationWithOtherTbl = null;
			NN_AreaInfo.ParentTables.TryGetValue(table.Alias.ToLower(), out Relation NN_relationWithTbl);
			if (otherTable != null)
				NN_AreaInfo.ParentTables.TryGetValue(otherTable.Alias.ToLower(), out NN_relationWithOtherTbl);

			if (NN_relationWithTbl != null)
			{
				qs = new SelectQuery()
				//.Distinct(true)
				.Select(tableKey)
				.From(tableNN)
				.Join(table)
					.On(CriteriaSet.And().Equal(new FieldRef(NN_relationWithTbl.AliasSourceTab, NN_relationWithTbl.SourceRelField), tableKey));
			}
			else
				return criteria;

			CriteriaSet whereConds = CriteriaSet.Or();
			if (NN_relationWithOtherTbl != null)
			{
				FieldRef NN_FldOtherTbl = new FieldRef(NN_relationWithOtherTbl.AliasSourceTab, NN_relationWithOtherTbl.SourceRelField);
				qs.Join(otherTable)
					.On(CriteriaSet.And().Equal(NN_FldOtherTbl, otherTableKey));
				whereConds.Equal(NN_FldOtherTbl, otherTableSelectedValue);

				if (naoAplicaSeNulo) //only apply if null condition
				{
					SelectQuery qs2 = (SelectQuery)qs.Clone();
					CriteriaSet whereConds2 = (CriteriaSet)whereConds.Clone();
					qs2.Where(whereConds2);
					whereConds.NotExists(qs2);
				}
			}

			//added limit for areaCompare that will limit NN
			if (areaCompare != null)
			{
				//CSGenio.business.AreaInfo NN_AreaInfo = getInformacaoMethod.Invoke(null, new object[] { }) as CSGenio.business.AreaInfo;
				NN_AreaInfo.ParentTables.TryGetValue(areaCompare.Alias.ToLower(), out Relation NN_relationWithAreaCompare);

				if (NN_relationWithAreaCompare != null)
				{
					FieldRef NN_FldAreaCompare = new FieldRef(NN_relationWithAreaCompare.AliasSourceTab, NN_relationWithAreaCompare.SourceRelField);
					qs.Join(areaCompare)
						.On(CriteriaSet.And().Equal(NN_FldAreaCompare, areaCompareKey));

					whereConds.Equal(NN_FldAreaCompare, areaCompareSelectedValue);
				}
			}

			// Apply the PHE
			CSGenio.business.Area areaNN = CSGenio.business.Area.createArea(NN_AreaInfo.Alias, m_userContext.User, m_userContext.User.CurrentModule);
			CriteriaSet condEph = Listing.CalculateConditionsEphGeneric(areaNN, identifier);
			if (condEph != null && (condEph.Criterias.Count > 0 || condEph.SubSets.Count > 0))
				qs.Where(CriteriaSet.And().SubSet(whereConds).SubSet(condEph));
			else
				qs.Where(whereConds);

			criteria.In(tableKey, qs);

			return criteria;
		}

		/// <summary>
		/// Adds a limitation to a CriteriaSet based on the history value.
		/// Takes into account the EPH entries as well.
		/// </summary>
		/// <param name="baseCondition">The condition to expand</param>
		/// <param name="targetField">The field being subjected to the filter</param>
		/// <param name="history">The history entry to be consulted</param>
		/// <returns>True if the entry was found, false otherwise</returns>
		public void AddHistoryLimit(CriteriaSet baseCondition, FieldRef targetField, string history)
		{
			if (Navigation.CheckKey(history))
				baseCondition.Equal(targetField, Navigation.GetValue(history));

			var ephs = m_userContext.User.fieldsEph(history);
			if (ephs != null)
			{
				if (ephs.Length > 1)
					baseCondition.In(targetField, ephs);
				else if (ephs.Length > 0)
					baseCondition.Equal(targetField, ephs[0]);
			}
		}

		/// <summary>
		/// Limitation by Zzstate
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="zzstateField"></param>
		/// <param name="opercriaField">(optional)</param>
		/// <returns></returns>
		protected CriteriaSet extendWithZzstateCondition(CriteriaSet condition, FieldRef zzstateField, FieldRef opercriaField)
		{
			if (opercriaField == null)
				condition.Criterias.Add(new Criteria(new ColumnReference(zzstateField), CriteriaOperator.NotEqual, 1));
			else
			{
				var zzstateCondition = CriteriaSet.Or().Equal(zzstateField, 0);
				var user = m_userContext.User;

				if (user.IsAdmin(user.CurrentModule))
				{
					zzstateCondition = zzstateCondition
						.Equal(zzstateField, 11)
							.SubSet(CriteriaSet.And()
								.Equal(zzstateField, 1)
								.Equal(opercriaField, user.Name));
				}
				else
				{
					zzstateCondition = zzstateCondition
						.SubSet(CriteriaSet.And()
							.Equal(opercriaField, user.Name)
								.SubSet(CriteriaSet.Or()
									.Equal(zzstateField, 1)
									.Equal(zzstateField, 11)));
				}

				condition = condition.SubSet(zzstateCondition);
			}

			return condition;
		}

		/// <summary>
		/// Adds a query condition to the CriteriaSet based on the historical value of the specified area or the current ViewModel value.
		/// If the value is an array, it adds an 'In' condition.
		/// If the value is not empty, it adds an 'Equal' condition.
		/// </summary>
		/// <param name="crs">The CriteriaSet to which the condition will be added.</param>
		/// <param name="fieldref">The FieldRef indicating the field for the query condition.</param>
		/// <param name="area">The area from which to retrieve the historical value for the condition.</param>
		/// <param name="fieldValue">The current ViewModel value to use as a fallback if the historical value is empty.</param>
		/// <param name="isMandatory">Indicates whether the limit is mandatory.</param>
		/// <param name="tryByGlobalFilter">Try applying the limit with the value that comes from a global lookup filter if the normal one doesn't exist.</param>
		/// <returns>True if the condition was successfully added or not needed, False if it's a mandatory limit and the value is empty.</returns>
		protected bool AddCriteriaAreaLimit(CriteriaSet crs, FieldRef fieldref, string area, string fieldValue, bool isMandatory, bool tryByGlobalFilter = false)
		{
			var histValue = Navigation.GetValue(area);
			var value = GenFunctions.emptyG(histValue) == 1 ? fieldValue : histValue;

			// If the key value is empty, try the key that comes from the global filter
			// (usually only applied between global filters of the Lookup type).
			if (tryByGlobalFilter && GenFunctions.emptyG(value) == 1)
				value = Navigation.CurrentLevel.GetEntry<string>($"global-filter-{area}");

			// Add an 'In' condition if the value is an array
			if (value is Array arrayValue)
			{
				crs.In(fieldref, arrayValue);
			}
			// Handle empty value based on 'isMandatory'
			else if (GenFunctions.emptyG(value) == 1)
			{
				return isMandatory ? false : true;
			}
			// Add an 'Equal' condition if the value is not empty
			else
			{
				crs.Equal(fieldref, value);
			}

			// Successfully applied the limit
			return true;
		}

		/// <summary>
		/// Adds a query condition to the CriteriaSet based on the historical value of the specified key.
		/// If the value is an array, it adds an 'In' condition.
		/// If the value is not empty, it adds an 'Equal' condition.
		/// </summary>
		/// <param name="crs">The CriteriaSet to which the condition will be added.</param>
		/// <param name="fieldref">The FieldRef indicating the field for the query condition.</param>
		/// <param name="operationType">The type of the limit operation.</param>
		/// <param name="key">The key from which to retrieve the historical value for the condition.</param>
		/// <param name="isMandatory">Indicates whether the limit is mandatory.</param>
		/// <returns>True if the condition was successfully added or not needed, False if it's a mandatory limit and the value is empty.</returns>
		protected bool AddCriteriaHistoryLimit(
			CriteriaSet crs,
			FieldRef fieldref,
			OperationType operationType,
			string key,
			bool isMandatory
		)
		{
			var histValue = Navigation.GetValue(key);
			var fieldInfo = CSGenio.business.Area.GetFieldInfo(fieldref);

			// Add an 'In' condition if the value is an array
			if (histValue is Array arrayValue)
			{
				crs.In(fieldref, arrayValue);
			}
			// Handle empty value based on 'isMandatory'
			else if (histValue == null)
			{
				return !isMandatory;
			}

			// Add the condition according to the operation type
			var value = QueryUtils.ToValidDbValue(histValue, fieldInfo);

			switch (operationType)
			{
				case OperationType.EQUAL:
					crs.Equal(fieldref, value);
					break;
				case OperationType.LESS:
					crs.Lesser(fieldref, value);
					break;
				case OperationType.LESSEQUAL:
					crs.LesserOrEqual(fieldref, value);
					break;
				case OperationType.GREAT:
					crs.Greater(fieldref, value);
					break;
				case OperationType.GREATEQUAL:
					crs.GreaterOrEqual(fieldref, value);
					break;
				case OperationType.DIFF:
					crs.NotEqual(fieldref, value);
					break;
				default:
					throw new InvalidOperationException("Invalid operation type: " + operationType);
			}

			// Successfully applied the limit
			return true;
		}

		[JsonIgnore]
		public StatusMessage flashMessage { get; set; }

		#region Permissions

		private bool CheckVMPermissions(FormMode mode)
		{
			if (Maintenance.Current.IsActive && (mode != FormMode.Show && mode != FormMode.FullTextSearch && mode != FormMode.List ))
				return false;

			//Form permissions
			User user = m_userContext.User;
			//use the full qualified name to prevent problems with tables with name ROLE
			CSGenio.framework.Role role;

			if (mode.Equals(FormMode.Show) || mode.Equals(FormMode.List))
				role = RoleToShow;
			else
				role = RoleToEdit;

			return user.VerifyAccess(role);
		}

		public virtual string GetPermissionMessage(FormMode mode)
		{
			string msg = String.Empty;
			if (Maintenance.Current.IsActive && (mode != FormMode.Show && mode != FormMode.FullTextSearch && mode != FormMode.List))
				msg = Resources.Resources.SISTEMA_EM_MANUTENCA49570;

			switch (mode)
			{
				case FormMode.List:
				case FormMode.FullTextSearch:
				case FormMode.Show:
					msg = Resources.Resources.O_UTILIZADOR_NAO_TEM03504;
					break;
				case FormMode.New:
				case FormMode.Duplicate:
					msg = Resources.Resources.NAO_TEM_PERMISSOES_P32156;
					break;
				case FormMode.Edit:
					msg = Resources.Resources.NAO_TEM_PERMISSOES_P04791;
					break;
				case FormMode.Delete:
					msg = Resources.Resources.O_UTILIZADOR_NAO_TEM12871;
					break;
				default:
					throw new FrameworkException(Resources.Resources.OCORREU_UM_ERRO_34773, "GetPermissionMessage", "FormMode not implemented: " + mode);
			}

			return msg;
		}

		protected StatusMessage CheckPermissions(Models.ModelBase model, FormMode mode)
		{
			bool hasPermission = CheckVMPermissions(mode);

			if (hasPermission)
				hasPermission = model.CheckTablePermissions(mode);

			if (!hasPermission)
			   return StatusMessage.Error(GetPermissionMessage(mode));
			return StatusMessage.OK();
		}

		public virtual StatusMessage CheckPermissions(FormMode mode)
		{
			if (!CheckVMPermissions(mode))
				return StatusMessage.Error(GetPermissionMessage(mode));
			return StatusMessage.OK();
		}

		#endregion

		/// <summary>
		/// Load the list of slot report from the database
		/// </summary>
		/// <param name="slotIdentifier">Slot id</param>
		/// <returns>list of slot report</returns>
		public List<object> GetSlotReports(string slotIdentifier)
		{
			List<object> resultList = new List<object>();
			List<CSGenioAreportlist> reportList = CSGenioAreportlist.searchList(
				m_userContext.PersistentSupport,
				m_userContext.User,
				CriteriaSet.And()
				.Equal(CSGenioAreportlist.FldSlotid, slotIdentifier)
				.Equal(CSGenioAreportlist.FldZzstate, 0)
			);
			reportList.ForEach (p => resultList.Add(p));
			return resultList;
		}

		/// <summary>CHN - Used in ViewModels Load() to reference the information of a EPH limit over the listing</summary>
		/// <param name="area_limit">(ref) limit to be loaded with the information aquired</param>
		/// <param name="model_limit_area">Area class object responsible for this limit</param>
		/// <param name="menu_identifier">Menu identifier where to check the EPHs existence</param>
		/// <returns>Returns the List existing EPHs that area being applied to current listing</returns>
		public List<Limit> EPH_Limit_Filler(ref Limit area_limit, CSGenio.business.Area model_limit_area, string menu_identifier)
		{
			string current_area = model_limit_area.Alias;
			AreaInfo area_info = model_limit_area.Information;
			string limit_field = model_limit_area.PrimaryKeyName;
			string limit_field_value = "";
			//string nav_limit_area = Navigation.GetStrValue(limit_area);
			User user = m_userContext.User;
			string module = user.CurrentModule;
			List<Limit> list_area_limit = new List<Limit>();

			//var ephs = user.fieldsEph(current_area); //m_userContext.User.Ephs[new Par(];// .fieldsEph(limit_field_value);
			List<EPHOfArea> ephsDaArea = model_limit_area.CalculateAreaEphs(model_limit_area.User.Ephs, menu_identifier, false);

			foreach (EPHOfArea eph in ephsDaArea)
			{
				limit_field = eph.Eph.Field; //double check this inference
				limit_field_value = eph.Eph.Name;

				area_limit.TipoLimiteOperator = eph.Eph.Operator;

				if (eph.Relation != null) //its related to another table, that is setting the EPH limit
				{
					CSGenio.business.Area parent_area = CSGenio.business.Area.createArea(eph.Relation.AliasTargetTab, m_userContext.User, m_userContext.User.CurrentModule);
					CSGenio.business.Area model_limit_area2 = parent_area; //change model to the one being related by foreign key
					area_info = model_limit_area2.Information;
					//limit_field = model_limit_area2.PrimaryKeyName; //need to confirm this inference, but it seems correct at a first glance
					Limit_Filler(ref area_limit, model_limit_area2, limit_field, limit_field_value, null, LimitAreaType.AreaLimita);
				}
				else if (eph.Relation2 != null) //its related to another table, via EPH2, that is setting the EPH limit
				{
					area_limit.TipoLimiteOperator = eph.Eph.Operator2;
					limit_field = eph.Eph.Field2; //double check this inference
					CSGenio.business.Area parent_area = CSGenio.business.Area.createArea(eph.Relation2.AliasTargetTab, m_userContext.User, m_userContext.User.CurrentModule);
					CSGenio.business.Area model_limit_area2 = parent_area; //change model to the one being related by foreign key
					area_info = model_limit_area2.Information;
					//limit_field = model_limit_area2.PrimaryKeyName; //need to confirm this inference, but it seems correct at a first glance
					Limit_Filler(ref area_limit, model_limit_area2, limit_field, limit_field_value, null, LimitAreaType.AreaLimita);
				}
				else
					Limit_Filler(ref area_limit, model_limit_area, limit_field, limit_field_value, null, LimitAreaType.AreaLimita);

				list_area_limit.Add(area_limit);
			}

			return list_area_limit;
		}

		/// <summary>CHN - Used in ViewModels Load() to reference the information of a EPH limit over the listing</summary>
		/// <param name="area_limit">(ref) limit to be loaded with the information aquired</param>
		/// <param name="model_limit_area">Area class object responsible for this limit</param>
		/// <param name="menu_identifier">Menu identifier where to check the EPHs existence</param>
		/// <returns>Returns the List existing EPHs that area being applied to current listing</returns>
		public void Limit_Filler(ref Limit area_limit, CSGenio.business.Area model_limit_area, string limit_field, string limit_field_value, object this_limit_field, LimitAreaType limitAreaType)
		{
			//Limit area information
			string limit_area = model_limit_area.Alias;
			AreaInfo area_info = model_limit_area.Information;
			string nav_limit_area = Navigation.GetStrValue(limit_area);
			bool filledbyeph = false;

			//Limit field information
			CSGenio.framework.Field field = null;
			string field_value = string.Empty;
			//check if necessary a change in limit_field to check for limit_field_value (usual in history manipulations)
			limit_field = limit_field == area_info.PrimaryKeyName && model_limit_area.DBFields.ContainsKey(limit_field_value) ? limit_field_value : limit_field;
			//fill field with object information
			field = model_limit_area.DBFields[limit_field];

			//special cases that have to select whats written in navigation or history to limit following related areas with value defined
			if (!string.IsNullOrEmpty(limit_field_value) && (area_limit.TipoLimite == LimitType.HM || area_limit.TipoLimite == LimitType.SH || area_limit.TipoLimite == LimitType.H))
			{
				nav_limit_area = Navigation.GetStrValue(limit_field_value);
				filledbyeph = true;
			}
			if ((string.IsNullOrEmpty(nav_limit_area) && filledbyeph) || area_limit.TipoLimite == LimitType.EPH) //If not filled, and its suposed to be by EPHs, checks EPH limits. If EPH, then check filling
			{
				var ephs = m_userContext.User.fieldsEph(limit_field_value);
				if (ephs != null)
				{
					nav_limit_area = ephs[0];
					limit_field_value = nav_limit_area;
				}
				else
					limit_field_value = string.Empty; //clears string, so it wont be mistaken as the value to be set in field value
			}

			//Tries to position area and field to a real record: if we have information about the area key, then it will be enough, otherwise, it will use a 'virtual' positioning on the first record and field variable will be manually set
			//Model has a field with the desired value filled acting as the limit (As an example Limit type "C" (field) is expecting this to be happening on AreaLimitaN)
			if ((field.isKey()) && //field a key
				(GenFunctions.emptyG(this_limit_field) == 0 || GenFunctions.emptyG(nav_limit_area) == 0)) //and the key is present either in this_limit_field or in nav_limit_area
			{
				if (GenFunctions.emptyG(this_limit_field) == 0) //this will give priority to field value with key to position the record.
					nav_limit_area = this_limit_field.ToString();

				//if limit_field is refering to a related area, then update model to the correct parent
				string parent_table_name = model_limit_area.ParentTables.Where(x => x.Value.SourceRelField == field.Name).Select(x => x.Value.AliasTargetTab).FirstOrDefault();
				if (parent_table_name != null)
				{//double check this case!
					CSGenio.business.Area parent_area = CSGenio.business.Area.createArea(parent_table_name, m_userContext.User, m_userContext.User.CurrentModule);
					model_limit_area = parent_area; //change model to the one being related by foreign key
					area_info = model_limit_area.Information;
					limit_field = model_limit_area.PrimaryKeyName; //need to confirm this inference, but it seems correct at a first glance
				}

				List<string> List_fields = new List<string>(){ area_info.Alias + "." + area_info.PrimaryKeyName, area_info.Alias + "." + limit_field }; //3 fields to select, primary, limit and humankey fields

				//area direct positioning to the desired record, using the key value that we want.
				//decompose human key into fields:
				string[] human_fields_array = area_info.HumanKeyName.Split(',');
				human_fields_array = human_fields_array.Where(x => !string.IsNullOrEmpty(x)).ToArray();
				if (GenFunctions.emptyC(area_info.HumanKeyName) == 0)
				{
					foreach (string human_field in human_fields_array)
						List_fields.Add(area_info.Alias + "." + human_field);
				}
				else
					List_fields.Add(area_info.Alias + "." + limit_field);

				string[] fields = List_fields.ToArray();

				model_limit_area.insertNamesFields(fields);
				model_limit_area.selectOne(CriteriaSet.And().Equal(area_info.Alias, area_info.PrimaryKeyName, nav_limit_area), null, "", m_userContext.PersistentSupport, 0);
				//select first human key that has a value.
				foreach (string human_field in human_fields_array)
				{
					field = model_limit_area.DBFields[human_field];
					field_value = ((CSGenio.framework.RequestedField)model_limit_area.Fields[model_limit_area.Alias + "." + field.Name]).Value.ToString();
					if (GenFunctions.emptyC(field_value) == 0) //if has a value, exit loop
						break;
				}

				if (GenFunctions.emptyC(area_info.HumanKeyName) == 1 || GenFunctions.emptyC(field_value) == 1) //last resort: displays primary key, better check human key table definitions to avoid this
				{
					field = model_limit_area.DBFields[area_info.PrimaryKeyName];
					field_value = ((CSGenio.framework.RequestedField)model_limit_area.Fields[model_limit_area.Alias + "." + field.Name]).Value.ToString();
				}
			}
			else //only matters the field value (applied to the current limit area)
			{
				string[] fields = [area_info.Alias + "." + area_info.PrimaryKeyName, area_info.Alias + "." + limit_field, area_info.Alias + "." + (area_info.HumanKeyName.Split(',')[0] == "" ? area_info.PrimaryKeyName : area_info.HumanKeyName.Split(',')[0])]; //3 fields to select, primary, limit and humankey field
				model_limit_area.insertNamesFields(fields);
				SelectQuery select_top1_limit = new();
				// Fields to select
				select_top1_limit.PageSize(0); //this is intended, to fill only the names, not any values that could misslead to believe that those were selected. At this moment no record is being selected.
				select_top1_limit.Select(model_limit_area.Alias, area_info.PrimaryKeyName);
				select_top1_limit.From(model_limit_area.QSystem, model_limit_area.TableName, model_limit_area.Alias);
				//get one random record from the desired table
				try
				{
					model_limit_area.selectOne(CriteriaSet.And().Equal(area_info.Alias, area_info.PrimaryKeyName, select_top1_limit), null, "", m_userContext.PersistentSupport, 0);
				}
				catch //this will always be landing on catch. explanation above.
				{ }

				field = model_limit_area.DBFields[limit_field];
				field_value = GenFunctions.emptyC(this_limit_field) == 0 ? this_limit_field.ToString() : (!string.IsNullOrEmpty(Navigation.GetStrValue(limit_field_value)) ? Navigation.GetStrValue(limit_field_value) : limit_field_value);

				//Get history value for fullname, on its variants (this should be consistent, but it isnt on some limit "S..." types, maybe review it later.)
				string field_Fullname = string.Empty;
				field_Fullname = limit_area + "." + limit_field;
				string field_FullnameVar = StringUtils.CapFirst(limit_area) + "Val" + StringUtils.CapFirst(limit_field);
				if (string.IsNullOrEmpty(field_value))
					field_value = Navigation.GetStrValue(field_Fullname);
				if (string.IsNullOrEmpty(field_value))
					field_value = Navigation.GetStrValue(field_FullnameVar);
				//Apply value to model to be rendered correctly
				((CSGenio.framework.RequestedField)model_limit_area.Fields[model_limit_area.Alias + "." + field.Name]).Value = field_value;
			}

			//Field value information on special limit fields that dont exist yet, only the value will be colected, so its not important to fill all fields object with stuff that will not be used.
			if (area_limit.TipoLimite == LimitType.SE)
			{
				DateTime minLim = Navigation.GetDateValue("min" + StringUtils.CapFirst(model_limit_area.Alias) + "Val" + StringUtils.CapFirst(limit_field)).GetValueOrDefault();
				DateTime maxLim = Navigation.GetDateValue("max" + StringUtils.CapFirst(model_limit_area.Alias) + "Val" + StringUtils.CapFirst(limit_field)).GetValueOrDefault();

				model_limit_area.Fields.Add(model_limit_area.Alias + "." + "minLim", new RequestedField("minLim", "") { Value = minLim });
				model_limit_area.Fields.Add(model_limit_area.Alias + "." + "maxLim", new RequestedField("maxLim", "") { Value = maxLim });
			}

			switch (limitAreaType)
			{
				case LimitAreaType.AreaLimita:
					{
						area_limit.AreaLimita = model_limit_area;
						area_limit.CampoLimita = field;
						break;
					}
				case LimitAreaType.AreaLimitaN:
					{
						area_limit.AreaLimitaN = model_limit_area;
						area_limit.CampoLimitaN = field;
						break;
					}
				case LimitAreaType.AreaComparar:
					{
						area_limit.AreaComparar = model_limit_area;
						area_limit.CampoComparar = field;
						break;
					}
				default:
					{
						area_limit.AreaLimita = model_limit_area;
						area_limit.CampoLimita = field;
						break;
					}
			}
		}

		///<summary>CHN - Added function in ViewModels to set a variable using reflexion</summary>
		///<param name="propertyName">ViewModel variable name</param>
		///<param name="value"> the value to be set</param>
		public void SetMethodInvoke(string propertyName, dynamic value)
		{
			var propertyInfo = this.GetType().GetProperty(propertyName);
			propertyInfo.SetMethod.Invoke(this, [value]);
		}

		///<summary>CHN - Added function in ViewModels to get a variable value using reflexion</summary>
		///<param name="propertyName">ViewModel variable name</param>
		public dynamic GetMethodInvoke(string propertyName)
		{
			var propertyInfo = this.GetType().GetProperty(propertyName);
			return propertyInfo.GetMethod.Invoke(this, null);
		}

		/// <summary>
		/// Gets the view model value for the specified identifier and model value
		/// </summary>
		/// <param name="identifier">The field identifier (area.field)</param>
		/// <param name="modelValue">The value of the field in the model</param>
		/// <returns>An object representing the value of the field in the view model</returns>
		protected virtual object GetViewModelValue(string identifier, object modelValue)
		{
			// This method is overridden in the view model of every form.
			return null;
		}

		/// <summary>
		/// Will be used by GetDependants to obtain the values of the fields in the ViewModel
		/// </summary>
		/// <param name="refDependantFields">The fields</param>
		/// <param name="values">The field values in the model</param>
		/// <returns>A dictionary with the values of the fields in the ViewModel</returns>
		protected ConcurrentDictionary<string, object> GetViewModelFieldValues(FieldRef[] refDependantFields, ArrayList values = null)
		{
			if (values != null && refDependantFields.Length != values.Count)
				throw new Exception("Wrong field count");

			ConcurrentDictionary<string, object> res = new();

			for (int i = 0; i < refDependantFields.Length; i++)
			{
				FieldRef fieldRef = refDependantFields[i];
				CSGenio.framework.Field dbField = CSGenio.business.Area.GetFieldInfo(fieldRef);
				object internalVal = values == null ? dbField.GetValorEmpty() : DBConversion.ToInternal(values[i], dbField.FieldFormat);
				res.TryAdd(fieldRef.FullName, GetViewModelValue(fieldRef.FullName, internalVal));
			}

			return res;
		}

		/// <summary>
		/// Converts the specified model values to view model values
		/// </summary>
		/// <param name="modelValues">A dictionary with the model values</param>
		/// <returns>A dictionary with the view model values</returns>
		public IDictionary<string, object> ConvertModelToViewModelValues(IDictionary<string, object> modelValues)
		{
			ConcurrentDictionary<string, object> viewModelValues = new();

			foreach (var value in modelValues)
			{
				string identifier = value.Key;
				viewModelValues[identifier] = GetViewModelValue(identifier, value.Value);
			}

			return viewModelValues;
		}

		/// <summary>
		///  Sanitizes the contents of fields with HTML support on the client-side by cleaning HTML fragments and documents of constructs that could lead to XSS attacks and compromise application security.
		/// </summary>
		protected virtual void SanitizeHTMLFields() { /* Method intentionally left empty. */ }

		/// <summary>
		/// Assigns ticket to Image fields.
		/// </summary>
		protected virtual void SetTicketToImageFields() { /* Method intentionally left empty. */ }

		/// <summary>
		/// Method that prepares the ViewModel content to be returned to the client-side.
		/// 	- Sanitizes the ViewModel content by cleaning HTML fragments and documents from constructs that could lead to XSS attacks and compromise application security.
		/// 	- Assigns ticket to Image fields.
		/// </summary>
		public void PrepareContentForClientSide()
		{
			SanitizeHTMLFields();
			SetTicketToImageFields();
		}
	}

	public static class ViewModelConversion
	{
		public static List<T> ToList<T>(object value)
		{
			return value is IEnumerable collection
				? [.. collection.Cast<T>()]
				: [];
		}

		public static decimal ToDouble(object value)
		{
			return DBConversion.ToNumeric(value);
		}

		public static decimal ToNumeric(object value)
		{
			if (value == null || value == DBNull.Value)
				return 0.0M;
			if (value is double)
				return Convert.ToDecimal(value);
			if (value is int)
				return (decimal)((int)value);
			if (value is decimal)
				return (decimal)value;
			if (value is string)
			{
				if (value.Equals(""))
					return 0.0M;

				decimal temp = 0.0M;
				if (!decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out temp) &&
					!decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out temp))
					return 0.0M;
				else
					return temp;
			}

			return 0.0M;
		}

		public static string ToString(object value)
		{
			return DBConversion.ToString(value);
		}

		public static int ToInteger(object value)
		{
			return DBConversion.ToInteger(value);
		}

		public static DateTime ToDateTime(object value)
		{
			return DBConversion.ToDateTime(value);
		}

		public static bool ToLogic(object value)
		{
			return DBConversion.ToLogic(value) == 1;
		}

		public static byte[] ToBinary(object value)
		{
			return DBConversion.ToBinary(value);
		}

		public static byte[] ToImage(Models.ImageModel value)
		{
			byte[] data = value?.OriginalData ?? (value?.Data?.Length > 0 ? System.Convert.FromBase64String(value.Data) : null);
			return DBConversion.ToBinary(data);
		}

		public static Models.ImageModel ToImage(object value)
		{
			byte[] image = value is Models.ImageModel imageModelValue ? DBConversion.ToBinary(imageModelValue?.OriginalData) : DBConversion.ToBinary(value);
			string imageFormat = ImageResizer.GetImageFormat(image);

			Models.ImageModel imageModel = null;
			if (image?.Length > 0)
			{
				imageModel = new(image)
				{
					Data = System.Convert.ToBase64String(image),
					DataFormat = imageFormat,
					FileName = "" // TODO: Save the file name and format.
				};
			}

			return imageModel;
		}

		public static CSGenio.framework.Geography.GeographicData ToGeographicShape(object value)
		{
			return DBConversion.ToGeographicShape(value);
		}

		/// <summary>
		/// Converts a JSON value to its corresponding raw value.
		/// </summary>
		/// <param name="value">The JSON value to convert.</param>
		/// <returns>
		/// The raw value corresponding to the given JSON value. This will be a direct translation for strings, numbers, booleans, and nulls;
		/// for other types, it returns the raw JSON text.
		/// </returns>
		/// <remarks>
		/// This method is particularly useful for extracting values from JSON elements that are pre-filled during the insertion of fields,
		/// handling different JSON value kinds appropriately.
		/// </remarks>
		public static object ToRawValue(object value)
		{
			// Check if the value is a JsonElement which can come from "prefillValues" during the pre-filling of fields.
			if (value is System.Text.Json.JsonElement je)
			{
				// Return the appropriate type based on the JsonValueKind of the JsonElement.
				switch (je.ValueKind)
				{
					case System.Text.Json.JsonValueKind.String:
						return je.GetString() ?? ""; // Convert JSON string to C# string, ensuring nulls are converted to empty strings.
					case System.Text.Json.JsonValueKind.Number:
						return je.GetDouble(); // Convert JSON number to double. TODO: preserve whole numbers - long/int
					case System.Text.Json.JsonValueKind.True:
						return true; // Convert JSON true to boolean true.
					case System.Text.Json.JsonValueKind.False:
						return false; // Convert JSON false to boolean false.
					case System.Text.Json.JsonValueKind.Undefined:
					case System.Text.Json.JsonValueKind.Null:
						return null; // Return null for undefined or null JSON values.
					default:
						return je.GetRawText(); // For other types (arrays, objects), return the raw JSON text.
				}
			}
			else
			{
				return value; // If not a JsonElement, return the value as is.
			}
		}
	}
}

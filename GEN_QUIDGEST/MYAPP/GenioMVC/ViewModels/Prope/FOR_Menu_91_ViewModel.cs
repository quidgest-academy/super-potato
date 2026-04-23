using SelectList = Microsoft.AspNetCore.Mvc.Rendering.SelectList;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;

using CSGenio.business;
using CSGenio.core.di;
using CSGenio.core.framework.table;
using CSGenio.framework;
using GenioMVC.Helpers;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.ViewModels.Prope
{
	public class FOR_Menu_91_ViewModel : MenuListViewModel<Models.Prope>
	{
		/// <summary>
		/// Gets or sets the object that represents the table and its elements.
		/// </summary>
		[JsonPropertyName("table")]
		public TablePartial<FOR_Menu_91_RowViewModel> Menu { get; set; }

		/// <inheritdoc/>
		[JsonIgnore]
		public override string TableAlias => "prope";

		/// <inheritdoc/>
		[JsonPropertyName("uuid")]
		public override string Uuid => "51904689-f05c-4ca0-951a-0ecfffef417a";

		/// <inheritdoc/>
		protected override string[] FieldsToSerialize => _fieldsToSerialize;

		/// <inheritdoc/>
		protected override List<TableSearchColumn> SearchableColumns => _searchableColumns;

		/// <summary>
		/// The context of the parent.
		/// </summary>
		[JsonIgnore]
		public Models.ModelBase ParentCtx { get; set; }

		/// <inheritdoc/>
		[JsonIgnore]
		public override CriteriaSet StaticLimits
		{
			get
			{
				CriteriaSet conditions = CriteriaSet.And();

				return conditions;
			}
		}

		/// <inheritdoc/>
		[JsonIgnore]
		public override CriteriaSet BaseConditions
		{
			get
			{
				CriteriaSet conds = CriteriaSet.And();

				return conds;
			}
		}

		/// <inheritdoc/>
		[JsonIgnore]
		public override List<Relation> Relations
		{
			get
			{
				List<Relation> relations = null;
				return relations;
			}
		}

		public override CriteriaSet GetCustomizedStaticLimits(CriteriaSet crs)
		{
// USE /[MANUAL FOR LIST_LIMITS 91]/

			return crs;
		}

		public override int GetCount(User user)
		{
			CSGenio.persistence.PersistentSupport sp = m_userContext.PersistentSupport;
			var areaBase = CSGenio.business.Area.createArea("prope", user, "FOR");

			//gets eph conditions to be applied in listing
			CriteriaSet conditions = CSGenio.business.Listing.CalculateConditionsEphGeneric(areaBase, "ML91");
			conditions.Equal(CSGenioAprope.FldZzstate, 0); //valid zzstate only

			// Fixed limits and relations:
			conditions.SubSets.Add(GetCustomizedStaticLimits(StaticLimits));

			// Checks for foreign tables in fields and conditions
			FieldRef[] fields = new FieldRef[] { CSGenioAprope.FldCodprope, CSGenioAprope.FldZzstate, CSGenioAprope.FldCodagent, CSGenioAagent.FldCodagent, CSGenioAagent.FldName, CSGenioAprope.FldPrice, CSGenioAprope.FldBuildage, CSGenioAprope.FldFloornr, CSGenioAprope.FldId, CSGenioAprope.FldCodcity, CSGenioAcity.FldCodcity, CSGenioAcity.FldCity, CSGenioAprope.FldBuildtyp, CSGenioAprope.FldDtconst, CSGenioAprope.FldDescript, CSGenioAprope.FldDtsold, CSGenioAprope.FldProfit, CSGenioAprope.FldTypology, CSGenioAprope.FldBathnr, CSGenioAprope.FldGrdsize, CSGenioAprope.FldSold, CSGenioAprope.FldTax, CSGenioAprope.FldAverage, CSGenioAprope.FldTitle, CSGenioAprope.FldPhoto, CSGenioAprope.FldSize };

			ListingMVC<CSGenioAprope> listing = new(fields, null, 1, 1, false, user, true, string.Empty, false);
			SelectQuery qs = sp.getSelectQueryFromListingMVC(conditions, listing);

			// Menu relations:
			if (qs.FromTable == null)
				qs.From(areaBase.QSystem, areaBase.TableName, areaBase.Alias);



			//operation: Count menu records
			return CSGenio.persistence.DBConversion.ToInteger(sp.ExecuteScalar(CSGenio.persistence.QueryUtils.buildQueryCount(qs)));
		}

		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// </summary>
		[Obsolete("For deserialization only")]
		public FOR_Menu_91_ViewModel() : base(null!) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="FOR_Menu_91_ViewModel" /> class.
		/// </summary>
		/// <param name="userContext">The current user request context</param>
		public FOR_Menu_91_ViewModel(UserContext userContext) : base(userContext)
		{
			this.RoleToShow = CSGenio.framework.Role.ROLE_1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FOR_Menu_91_ViewModel" /> class.
		/// </summary>
		/// <param name="userContext">The current user request context</param>
		/// <param name="parentCtx">The context of the parent</param>
		public FOR_Menu_91_ViewModel(UserContext userContext, Models.ModelBase parentCtx) : this(userContext)
		{
			ParentCtx = parentCtx;
		}

		/// <inheritdoc/>
		public override List<Exports.QColumn> GetColumnsToExport()
		{
			return
			[
				new Exports.QColumn(CSGenioAagent.FldName, FieldType.TEXT, Resources.Resources.AGENT_S_NAME42642, 30, 0, true),
				new Exports.QColumn(CSGenioAprope.FldPrice, FieldType.CURRENCY, Resources.Resources.PRICE06900, 12, 0, true),
				new Exports.QColumn(CSGenioAprope.FldBuildage, FieldType.NUMERIC, Resources.Resources.BUILDING_AGE27311, 4, 0, true),
				new Exports.QColumn(CSGenioAprope.FldFloornr, FieldType.NUMERIC, Resources.Resources.FLOOR19993, 2, 0, true),
				new Exports.QColumn(CSGenioAprope.FldId, FieldType.NUMERIC, string.Empty, 5, 0, true),
				new Exports.QColumn(CSGenioAcity.FldCity, FieldType.TEXT, Resources.Resources.CITY42505, 30, 0, true),
				new Exports.QColumn(CSGenioAprope.FldBuildtyp, FieldType.ARRAY_TEXT, Resources.Resources.BUILDING_TYPE57152, 1, 0, true, "buildtyp"),
				new Exports.QColumn(CSGenioAprope.FldDtconst, FieldType.DATE, Resources.Resources.CONSTRUCTION_DATE18132, 8, 0, true),
				new Exports.QColumn(CSGenioAprope.FldDescript, FieldType.MEMO, Resources.Resources.DESCRIPTION07383, 30, 5, true),
				new Exports.QColumn(CSGenioAprope.FldDtsold, FieldType.DATE, Resources.Resources.SOLD_DATE37976, 8, 0, true),
				new Exports.QColumn(CSGenioAprope.FldProfit, FieldType.CURRENCY, Resources.Resources.PROFIT55910, 12, 0, true),
				new Exports.QColumn(CSGenioAprope.FldTypology, FieldType.ARRAY_NUMERIC, Resources.Resources.BUILDING_TYPOLOGY54011, 1, 0, true, "typology"),
				new Exports.QColumn(CSGenioAprope.FldBathnr, FieldType.NUMERIC, Resources.Resources.BATHROOMS_NUMBER52698, 2, 0, true),
				new Exports.QColumn(CSGenioAprope.FldGrdsize, FieldType.NUMERIC, Resources.Resources.GROUND_SIZE01563, 9, 0, true),
				new Exports.QColumn(CSGenioAprope.FldSold, FieldType.LOGIC, Resources.Resources.SOLD59824, 1, 0, true),
				new Exports.QColumn(CSGenioAprope.FldTax, FieldType.NUMERIC, Resources.Resources.TAX37977, 5, 2, true),
				new Exports.QColumn(CSGenioAprope.FldAverage, FieldType.NUMERIC, Resources.Resources.AVERAGEPRICE13700, 12, 0, true),
				new Exports.QColumn(CSGenioAprope.FldTitle, FieldType.TEXT, Resources.Resources.TITLE21885, 30, 0, true),
				new Exports.QColumn(CSGenioAprope.FldSize, FieldType.NUMERIC, Resources.Resources.SIZE__M2_57059, 8, 0, true),
			];
		}

		public void LoadToExport(out ListingMVC<CSGenioAprope> listing, out CriteriaSet conditions, out List<Exports.QColumn> columns, NameValueCollection requestValues, bool ajaxRequest = false)
		{
			CSGenio.core.framework.table.TableConfiguration tableConfig = new();
			LoadToExport(out listing, out conditions, out columns, tableConfig, requestValues, ajaxRequest);
		}

		public void LoadToExport(out ListingMVC<CSGenioAprope> listing, out CriteriaSet conditions, out List<Exports.QColumn> columns, CSGenio.core.framework.table.TableConfiguration tableConfig, NameValueCollection requestValues, bool ajaxRequest = false)
		{
			listing = null;
			conditions = null;
			columns = this.GetExportColumns(tableConfig.ColumnConfigurations);

			// Store number of records to reset it after loading
			int rowsPerPage = tableConfig.RowsPerPage;
			tableConfig.RowsPerPage = -1;

			Load(tableConfig, requestValues, ajaxRequest, true, ref listing, ref conditions);

			// Reset number of records to original value
			tableConfig.RowsPerPage = rowsPerPage;
		}

		/// <inheritdoc/>
		public override CriteriaSet BuildCriteriaSet(NameValueCollection requestValues, out bool tableReload, CriteriaSet crs = null, bool isToExport = false)
		{
			CSGenio.core.framework.table.TableConfiguration tableConfig = new();
			return BuildCriteriaSet(tableConfig, requestValues, out tableReload, crs, isToExport);
		}

		/// <inheritdoc/>
		public override CriteriaSet BuildCriteriaSet(CSGenio.core.framework.table.TableConfiguration tableConfig, NameValueCollection requestValues, out bool tableReload, CriteriaSet crs = null, bool isToExport = false)
		{
			User u = m_userContext.User;
			tableReload = true;

			crs ??= CriteriaSet.And();

			Menu ??= new TablePartial<FOR_Menu_91_RowViewModel>();
			// Set table name (used in getting searchable column names)
			Menu.TableName = TableAlias;

			Menu.SetFilters(false, false);

			crs.SubSets.Add(ProcessSearchFilters(Menu, GetSearchColumns(tableConfig.ColumnConfigurations), tableConfig));


			//Subfilters
			CriteriaSet subfilters = CriteriaSet.And();


			crs.SubSets.Add(subfilters);

			// Form field filters
			crs.SubSets.Add(ProcessFieldFilters(tableConfig.GlobalFilters));

			crs.SubSets.Add(GetCustomizedStaticLimits(StaticLimits));

			if (isToExport)
			{
				// EPH
				crs = Models.Prope.AddEPH<CSGenioAprope>(ref u, crs, "ML91");

				// Export only records with ZZState == 0
				crs.Equal(CSGenioAprope.FldZzstate, 0);

				return crs;
			}

			// Limitation by Zzstate
			if (!Navigation.checkFormMode("PROPE", FormMode.New)) // TODO: Check in Duplicate mode
				crs = extendWithZzstateCondition(crs, CSGenioAprope.FldZzstate, null);


			if (tableReload)
			{
				string QMVC_POS_RECORD = Navigation.GetStrValue("QMVC_POS_RECORD_prope");
				Navigation.DestroyEntry("QMVC_POS_RECORD_prope");
				if (!string.IsNullOrEmpty(QMVC_POS_RECORD))
					crs.Equals(Models.Prope.AddEPH<CSGenioAprope>(ref u, null, "ML91"));
			}

			return crs;
		}

		/// <summary>
		/// Loads the list with the specified number of rows.
		/// </summary>
		/// <param name="numberListItems">The number of rows to load.</param>
		/// <param name="ajaxRequest">Whether the request was initiated via AJAX.</param>
		public void Load(int numberListItems, bool ajaxRequest = false)
		{
			Load(numberListItems, new NameValueCollection(), ajaxRequest);
		}

		/// <summary>
		/// Loads the list with the specified number of rows.
		/// </summary>
		/// <param name="numberListItems">The number of rows to load.</param>
		/// <param name="requestValues">The request values.</param>
		/// <param name="ajaxRequest">Whether the request was initiated via AJAX.</param>
		/// <param name="conditions">The conditions.</param>
		public void Load(int numberListItems, NameValueCollection requestValues, bool ajaxRequest = false, CriteriaSet conditions = null)
		{
			ListingMVC<CSGenioAprope> listing = null;

			Load(numberListItems, requestValues, ajaxRequest, false, ref listing, ref conditions);
		}

		/// <summary>
		/// Loads the list with the specified number of rows.
		/// </summary>
		/// <param name="numberListItems">The number of rows to load.</param>
		/// <param name="requestValues">The request values.</param>
		/// <param name="ajaxRequest">Whether the request was initiated via AJAX.</param>
		/// <param name="isToExport">Whether the list is being loaded to be exported</param>
		/// <param name="Qlisting">The rows.</param>
		/// <param name="conditions">The conditions.</param>
		public void Load(int numberListItems, NameValueCollection requestValues, bool ajaxRequest, bool isToExport, ref ListingMVC<CSGenioAprope> Qlisting, ref CriteriaSet conditions)
		{
			CSGenio.core.framework.table.TableConfiguration tableConfig = new();

			tableConfig.RowsPerPage = numberListItems;

			Load(tableConfig, requestValues, ajaxRequest, isToExport, ref Qlisting, ref conditions);
		}

		/// <summary>
		/// Loads the table with the specified configuration.
		/// </summary>
		/// <param name="tableConfig">The table configuration object</param>
		/// <param name="requestValues">The request values.</param>
		/// <param name="ajaxRequest">Whether the request was initiated via AJAX.</param>
		/// <param name="isToExport">Whether the list is being loaded to be exported</param>
		/// <param name="conditions">The conditions.</param>
		public void Load(CSGenio.core.framework.table.TableConfiguration tableConfig, NameValueCollection requestValues, bool ajaxRequest, bool isToExport = false, CriteriaSet conditions = null)
		{
			ListingMVC<CSGenioAprope> listing = null;

			Load(tableConfig, requestValues, ajaxRequest, isToExport, ref listing, ref conditions);
		}

		/// <summary>
		/// Loads the table with the specified configuration.
		/// </summary>
		/// <param name="tableConfig">The table configuration object</param>
		/// <param name="requestValues">The request values.</param>
		/// <param name="ajaxRequest">Whether the request was initiated via AJAX.</param>
		/// <param name="isToExport">Whether the list is being loaded to be exported</param>
		/// <param name="Qlisting">The rows.</param>
		/// <param name="conditions">The conditions.</param>
		public void Load(CSGenio.core.framework.table.TableConfiguration tableConfig, NameValueCollection requestValues, bool ajaxRequest, bool isToExport, ref ListingMVC<CSGenioAprope> Qlisting, ref CriteriaSet conditions)
		{
			User u = m_userContext.User;
			Menu = new TablePartial<FOR_Menu_91_RowViewModel>();

			CriteriaSet for_menu_91Conds = CriteriaSet.And();
			bool tableReload = true;

			//FOR: MENU LIST SORTING
			Dictionary<string, OrderedDictionary> allSortOrders = new Dictionary<string, OrderedDictionary>();
			allSortOrders.Add("PROPE.DTCONST", new OrderedDictionary());
			allSortOrders["PROPE.DTCONST"].Add("PROPE.DTCONST", "A");


			int numberListItems = tableConfig.RowsPerPage;
			var pageNumber = ajaxRequest ? tableConfig.Page : 1;

			// Added to avoid 0 or -1 pages when setting number of records to -1 to disable pagination
			if (pageNumber < 1)
				pageNumber = 1;

			List<ColumnSort> sorts = GetRequestSorts(this.Menu, tableConfig, "prope", allSortOrders);

			if (sorts == null || sorts.Count == 0)
			{
				sorts = new List<ColumnSort>();
				sorts.Add(new ColumnSort(new ColumnReference(CSGenioAprope.FldDtconst), SortOrder.Ascending));

			}

			FieldRef[] fields = new FieldRef[] { CSGenioAprope.FldCodprope, CSGenioAprope.FldZzstate, CSGenioAprope.FldCodagent, CSGenioAagent.FldCodagent, CSGenioAagent.FldName, CSGenioAprope.FldPrice, CSGenioAprope.FldBuildage, CSGenioAprope.FldFloornr, CSGenioAprope.FldId, CSGenioAprope.FldCodcity, CSGenioAcity.FldCodcity, CSGenioAcity.FldCity, CSGenioAprope.FldBuildtyp, CSGenioAprope.FldDtconst, CSGenioAprope.FldDescript, CSGenioAprope.FldDtsold, CSGenioAprope.FldProfit, CSGenioAprope.FldTypology, CSGenioAprope.FldBathnr, CSGenioAprope.FldGrdsize, CSGenioAprope.FldSold, CSGenioAprope.FldTax, CSGenioAprope.FldAverage, CSGenioAprope.FldTitle, CSGenioAprope.FldPhoto, CSGenioAprope.FldSize };

			// List of column names that should display totalized (aggregated) values.
			List<string> totalizerColumns = [];
			List<FieldRef> fieldsWithTotalizers = [.. fields.Where(field => totalizerColumns.Contains(field.FullName))];

			FieldRef firstVisibleColumn = null;
			if (sorts.Count == 0)
			{
				firstVisibleColumn = tableConfig?.GetFirstVisibleColumn(TableAlias);

				firstVisibleColumn ??= new FieldRef("agent", "name");
			}
			// Limitations
			this.TableLimits ??= [];
			// Comparer to check if limit is already present in TableLimits
			LimitComparer limitComparer = new();

			//Tooltip for EPHs affecting this viewmodel list
			{
				Limit limit = new Limit();
				limit.TipoLimite = LimitType.EPH;
				CSGenioAprope model_limit_area = new CSGenioAprope(m_userContext.User);
				List<Limit> area_EPH_limits = EPH_Limit_Filler(ref limit, model_limit_area, "ML91");
				if (area_EPH_limits.Count > 0)
					this.TableLimits.AddRange(area_EPH_limits);
			}
			//Tooltip for "Manual Filter" affecting this viewmodel list
			{
				Limit limit = new Limit();
				limit.TipoLimite = LimitType.OVERRQ;
				using (CSGenio.core.di.GenioDI.MetricsOtlp.RecordTime("manua_exec_time", new System.Diagnostics.TagList([
					new("Name", "OVERRQ_TOOLTIP"),
					new("Parameter", "91"),
					new("ModuleOrSystem", "FOR")
				]), "ms", "Time to execute the manual code.")) {
//Platform: MVC | Type: OVERRQ_TOOLTIP | Module: FOR | Parameter: 91 | File:  | Order: 0
//BEGIN_MANUALCODE_CODMANUA:c9cc2dde-efe0-4966-b6f5-fbac56bed1c8
limit.ManualHTMLText = "Properties without contacts";
this.TableLimits.Add(limit);
//END_MANUALCODE
				}

			}


			if (conditions == null)
				conditions = CriteriaSet.And();

			conditions.SubSets.Add(for_menu_91Conds);
			for_menu_91Conds = BuildCriteriaSet(tableConfig, requestValues, out bool hasAllRequiredLimits, conditions, isToExport);
			tableReload &= hasAllRequiredLimits;

				using (CSGenio.core.di.GenioDI.MetricsOtlp.RecordTime("manua_exec_time", new System.Diagnostics.TagList([
					new("Name", "OVERRQ"),
					new("Parameter", "91"),
					new("ModuleOrSystem", "FOR")
				]), "ms", "Time to execute the manual code.")) {
//Platform: MVC | Type: OVERRQ | Module: FOR | Parameter: 91 | File:  | Order: 0
//BEGIN_MANUALCODE_CODMANUA:28840411-255b-424e-902d-ff2dca2e603a
for_menu_91Conds = CriteriaSet.And().Equal(CSGenioAprope.FldNumbercontacts, 0);
//END_MANUALCODE
				}


			bool distinct = false;

			if (isToExport)
			{
				if (!tableReload)
					return;

				var exportColumns = GetExportColumns(tableConfig.ColumnConfigurations);
				var exportFieldRefs = exportColumns.Select(eCol => eCol.Field).Where(fldRef => fldRef != null).ToArray();

				Qlisting = Models.ModelBase.BuildListingForExport<CSGenioAprope>(m_userContext, false, ref for_menu_91Conds, exportFieldRefs, (pageNumber - 1) * numberListItems, numberListItems, sorts, "ML91", true, firstVisibleColumn: firstVisibleColumn);

// USE /[MANUAL FOR OVERRQLSTEXP 91]/

				return;
			}

			if (tableReload)
			{
// USE /[MANUAL FOR OVERRQLIST 91]/

				string QMVC_POS_RECORD = Navigation.GetStrValue("QMVC_POS_RECORD_prope");
				Navigation.DestroyEntry("QMVC_POS_RECORD_prope");
				CriteriaSet m_PagingPosEPHs = null;

				if (!string.IsNullOrEmpty(QMVC_POS_RECORD))
				{
					var m_iCurPag = m_userContext.PersistentSupport.getPagingPos(CSGenioAprope.GetInformation(), QMVC_POS_RECORD, sorts, for_menu_91Conds, m_PagingPosEPHs, firstVisibleColumn: firstVisibleColumn);
					if (m_iCurPag != -1)
						pageNumber = ((m_iCurPag - 1) / numberListItems) + 1;
				}

				ListingMVC<CSGenioAprope> listing = Models.ModelBase.Where<CSGenioAprope>(m_userContext, distinct, for_menu_91Conds, fields, (pageNumber - 1) * numberListItems, numberListItems, sorts, "ML91", true, false, QMVC_POS_RECORD, m_PagingPosEPHs, firstVisibleColumn, fieldsWithTotalizers, tableConfig.SelectedRows);

				if (listing.CurrentPage > 0)
					pageNumber = listing.CurrentPage;

				//Added to avoid 0 or -1 pages when setting number of records to -1 to disable pagination
				if (pageNumber < 1)
					pageNumber = 1;

				//Set document field values to objects
				SetDocumentFields(listing);

				Menu.Elements = MapFOR_Menu_91(listing);

				Menu.Identifier = "ML91";
				Menu.Slots = new Dictionary<string, List<object>>();

				// Last updated by [CJP] at [2015.02.03]
				// Adds the identifier to each element
				foreach (var element in Menu.Elements)
					element.Identifier = "ML91";

				Menu.SetPagination(pageNumber, listing.NumRegs, listing.HasMore, listing.GetTotal, listing.TotalRecords);

				// Set table totalizers
				if (listing.Totalizers != null && listing.Totalizers.Count > 0)
					Menu.SetTotalizers(listing.Totalizers);
			}

			// Set table limits display property
			FillTableLimitsDisplayData();

			// Store table configuration so it gets sent to the client-side to be processed
			CurrentTableConfig = tableConfig;

			// Load the user table configuration names and default name
			LoadUserTableConfigNameProperties();
		}

		private List<FOR_Menu_91_RowViewModel> MapFOR_Menu_91(ListingMVC<CSGenioAprope> Qlisting)
		{
			List<FOR_Menu_91_RowViewModel> Elements = [];
			int i = 0;

			if (Qlisting.Rows != null)
			{
				foreach (var row in Qlisting.Rows)
				{
					if (Qlisting.NumRegs > 0 && i >= Qlisting.NumRegs) // Copiado da versão antiga do RowsToViewModels
						break;
					Elements.Add(MapFOR_Menu_91(row));
					i++;
				}
			}

			return Elements;
		}

		/// <summary>
		/// Maps a single CSGenioAprope row
		/// to a FOR_Menu_91_RowViewModel object.
		/// </summary>
		/// <param name="row">The row.</param>
		private FOR_Menu_91_RowViewModel MapFOR_Menu_91(CSGenioAprope row)
		{
			var model = new FOR_Menu_91_RowViewModel(m_userContext, true, _fieldsToSerialize);
			if (row == null)
				return model;

			foreach (RequestedField Qfield in row.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "prope":
						model.klass.insertNameValueField(Qfield.FullName, Qfield.Value); break;
					case "agent":
						model.Agent.klass.insertNameValueField(Qfield.FullName, Qfield.Value); break;
					case "city":
						model.City.klass.insertNameValueField(Qfield.FullName, Qfield.Value); break;
					default:
						break;
				}
			}

			model.InitRowData();

			SetTicketToImageFields(model);
			return model;
		}

		/// <summary>
		/// Checks the loaded model for pending rows (zzsttate not 0).
		/// </summary>
		public bool CheckForZzstate()
		{
			if (Menu?.Elements == null)
				return false;

			return Menu.Elements.Any(row => row.ValZzstate != 0);
		}

		/// <summary>
		/// Sets the document field values to objects.
		/// </summary>
		/// <param name="listing">The rows</param>
		private void SetDocumentFields(ListingMVC<CSGenioAprope> listing)
		{
		}

		#region Mapper

		/// <inheritdoc />
		public override void MapFromModel(Models.Prope m)
		{
		}

		/// <inheritdoc />
		public override void MapToModel(Models.Prope m)
		{
		}

		#endregion

		#region Custom code

// USE /[MANUAL FOR VIEWMODEL_CUSTOM FOR_MENU_91]/

		#endregion

		private static readonly string[] _fieldsToSerialize =
		[
			"Prope", "Prope.ValCodprope", "Prope.ValZzstate", "Agent", "Agent.ValName", "Prope.ValPrice", "Prope.ValBuildage", "Prope.ValFloornr", "Prope.ValId", "City", "City.ValCity", "Prope.ValBuildtyp", "Prope.ValDtconst", "Prope.ValDescript", "Prope.ValDtsold", "Prope.ValProfit", "Prope.ValTypology", "Prope.ValBathnr", "Prope.ValGrdsize", "Prope.ValSold", "Prope.ValTax", "Prope.ValAverage", "Prope.ValTitle", "Prope.ValPhoto", "Prope.ValSize", "Prope.ValCodagent", "Prope.ValCodcity"
		];

		private static readonly List<TableSearchColumn> _searchableColumns =
		[
			new TableSearchColumn("Agent_ValName", CSGenioAagent.FldName, typeof(string)),
			new TableSearchColumn("ValPrice", CSGenioAprope.FldPrice, typeof(decimal?)),
			new TableSearchColumn("ValBuildage", CSGenioAprope.FldBuildage, typeof(decimal?)),
			new TableSearchColumn("ValFloornr", CSGenioAprope.FldFloornr, typeof(decimal?)),
			new TableSearchColumn("ValId", CSGenioAprope.FldId, typeof(decimal?)),
			new TableSearchColumn("City_ValCity", CSGenioAcity.FldCity, typeof(string)),
			new TableSearchColumn("ValBuildtyp", CSGenioAprope.FldBuildtyp, typeof(string), array : "buildtyp"),
			new TableSearchColumn("ValDtconst", CSGenioAprope.FldDtconst, typeof(DateTime?)),
			new TableSearchColumn("ValDescript", CSGenioAprope.FldDescript, typeof(string)),
			new TableSearchColumn("ValDtsold", CSGenioAprope.FldDtsold, typeof(DateTime?)),
			new TableSearchColumn("ValProfit", CSGenioAprope.FldProfit, typeof(decimal?)),
			new TableSearchColumn("ValTypology", CSGenioAprope.FldTypology, typeof(decimal), array : "typology"),
			new TableSearchColumn("ValBathnr", CSGenioAprope.FldBathnr, typeof(decimal?)),
			new TableSearchColumn("ValGrdsize", CSGenioAprope.FldGrdsize, typeof(decimal?)),
			new TableSearchColumn("ValSold", CSGenioAprope.FldSold, typeof(bool)),
			new TableSearchColumn("ValTax", CSGenioAprope.FldTax, typeof(decimal?)),
			new TableSearchColumn("ValAverage", CSGenioAprope.FldAverage, typeof(decimal?)),
			new TableSearchColumn("ValTitle", CSGenioAprope.FldTitle, typeof(string), defaultSearch : true),
			new TableSearchColumn("ValSize", CSGenioAprope.FldSize, typeof(decimal?)),
		];
		protected void SetTicketToImageFields(Models.Prope row)
		{
			if (row == null)
				return;

			row.ValPhotoQTicket = Helpers.Helpers.GetFileTicket(m_userContext.User, CSGenio.business.Area.AreaPROPE, CSGenioAprope.FldPhoto.Field, null, row.ValCodprope);
		}
	}
}

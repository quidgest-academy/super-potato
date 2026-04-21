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

namespace GenioMVC.ViewModels.Agent
{
	public class FOR_Menu_11_ViewModel : MenuListViewModel<Models.Agent>
	{
		/// <summary>
		/// Gets or sets the object that represents the table and its elements.
		/// </summary>
		[JsonPropertyName("table")]
		public TablePartial<FOR_Menu_11_RowViewModel> Menu { get; set; }

		/// <inheritdoc/>
		[JsonIgnore]
		public override string TableAlias => "agent";

		/// <inheritdoc/>
		[JsonPropertyName("uuid")]
		public override string Uuid => "5de40750-e12b-4e68-807b-fd93d7c1a364";

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
// USE /[MANUAL FOR LIST_LIMITS 11]/

			return crs;
		}

		public override int GetCount(User user)
		{
			CSGenio.persistence.PersistentSupport sp = m_userContext.PersistentSupport;
			var areaBase = CSGenio.business.Area.createArea("agent", user, "FOR");

			//gets eph conditions to be applied in listing
			CriteriaSet conditions = CSGenio.business.Listing.CalculateConditionsEphGeneric(areaBase, "ML11");
			conditions.Equal(CSGenioAagent.FldZzstate, 0); //valid zzstate only

			// Fixed limits and relations:
			conditions.SubSets.Add(GetCustomizedStaticLimits(StaticLimits));

			// Checks for foreign tables in fields and conditions
			FieldRef[] fields = new FieldRef[] { CSGenioAagent.FldCodagent, CSGenioAagent.FldZzstate, CSGenioAagent.FldCodcaddr, CSGenioAcaddr.FldCodcount, CSGenioAcaddr.FldCountry, CSGenioAagent.FldLastprop, CSGenioAagent.FldNrprops, CSGenioAagent.FldProfit, CSGenioAagent.FldAge, CSGenioAagent.FldBirthdat, CSGenioAagent.FldPhotography, CSGenioAagent.FldEmail, CSGenioAagent.FldTelephon, CSGenioAagent.FldName, CSGenioAagent.FldCborn, CSGenioAcborn.FldCodcount, CSGenioAcborn.FldCountry };

			ListingMVC<CSGenioAagent> listing = new(fields, null, 1, 1, false, user, true, string.Empty, false);
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
		public FOR_Menu_11_ViewModel() : base(null!) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="FOR_Menu_11_ViewModel" /> class.
		/// </summary>
		/// <param name="userContext">The current user request context</param>
		public FOR_Menu_11_ViewModel(UserContext userContext) : base(userContext)
		{
			this.RoleToShow = CSGenio.framework.Role.ROLE_1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FOR_Menu_11_ViewModel" /> class.
		/// </summary>
		/// <param name="userContext">The current user request context</param>
		/// <param name="parentCtx">The context of the parent</param>
		public FOR_Menu_11_ViewModel(UserContext userContext, Models.ModelBase parentCtx) : this(userContext)
		{
			ParentCtx = parentCtx;
		}

		/// <inheritdoc/>
		public override List<Exports.QColumn> GetColumnsToExport()
		{
			return
			[
				new Exports.QColumn(CSGenioAcaddr.FldCountry, FieldType.TEXT, Resources.Resources.COUNTRY64133, 30, 0, true),
				new Exports.QColumn(CSGenioAagent.FldLastprop, FieldType.CURRENCY, Resources.Resources.LAST_PROPERTY_SOLD__49162, 12, 0, true),
				new Exports.QColumn(CSGenioAagent.FldNrprops, FieldType.NUMERIC, Resources.Resources.NUMBER_OF_PROPERTIES01169, 5, 0, true),
				new Exports.QColumn(CSGenioAagent.FldProfit, FieldType.CURRENCY, Resources.Resources.PROFIT55910, 14, 0, true),
				new Exports.QColumn(CSGenioAagent.FldAge, FieldType.NUMERIC, Resources.Resources.AGE28663, 3, 0, true),
				new Exports.QColumn(CSGenioAagent.FldBirthdat, FieldType.DATE, Resources.Resources.BIRTHDATE22743, 8, 0, true),
				new Exports.QColumn(CSGenioAagent.FldEmail, FieldType.TEXT, Resources.Resources.E_MAIL42251, 30, 0, true),
				new Exports.QColumn(CSGenioAagent.FldTelephon, FieldType.TEXT, Resources.Resources.TELEPHONE28697, 14, 0, true),
				new Exports.QColumn(CSGenioAagent.FldName, FieldType.TEXT, Resources.Resources.AGENT_S_NAME42642, 30, 0, true),
				new Exports.QColumn(CSGenioAcborn.FldCountry, FieldType.TEXT, Resources.Resources.COUNTRY64133, 30, 0, true),
			];
		}

		public void LoadToExport(out ListingMVC<CSGenioAagent> listing, out CriteriaSet conditions, out List<Exports.QColumn> columns, NameValueCollection requestValues, bool ajaxRequest = false)
		{
			CSGenio.core.framework.table.TableConfiguration tableConfig = new();
			LoadToExport(out listing, out conditions, out columns, tableConfig, requestValues, ajaxRequest);
		}

		public void LoadToExport(out ListingMVC<CSGenioAagent> listing, out CriteriaSet conditions, out List<Exports.QColumn> columns, CSGenio.core.framework.table.TableConfiguration tableConfig, NameValueCollection requestValues, bool ajaxRequest = false)
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

			Menu ??= new TablePartial<FOR_Menu_11_RowViewModel>();
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
				crs = Models.Agent.AddEPH<CSGenioAagent>(ref u, crs, "ML11");

				// Export only records with ZZState == 0
				crs.Equal(CSGenioAagent.FldZzstate, 0);

				return crs;
			}

			// Limitation by Zzstate
			if (!Navigation.checkFormMode("AGENT", FormMode.New)) // TODO: Check in Duplicate mode
				crs = extendWithZzstateCondition(crs, CSGenioAagent.FldZzstate, null);


			if (tableReload)
			{
				string QMVC_POS_RECORD = Navigation.GetStrValue("QMVC_POS_RECORD_agent");
				Navigation.DestroyEntry("QMVC_POS_RECORD_agent");
				if (!string.IsNullOrEmpty(QMVC_POS_RECORD))
					crs.Equals(Models.Agent.AddEPH<CSGenioAagent>(ref u, null, "ML11"));
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
			ListingMVC<CSGenioAagent> listing = null;

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
		public void Load(int numberListItems, NameValueCollection requestValues, bool ajaxRequest, bool isToExport, ref ListingMVC<CSGenioAagent> Qlisting, ref CriteriaSet conditions)
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
			ListingMVC<CSGenioAagent> listing = null;

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
		public void Load(CSGenio.core.framework.table.TableConfiguration tableConfig, NameValueCollection requestValues, bool ajaxRequest, bool isToExport, ref ListingMVC<CSGenioAagent> Qlisting, ref CriteriaSet conditions)
		{
			User u = m_userContext.User;
			Menu = new TablePartial<FOR_Menu_11_RowViewModel>();

			CriteriaSet for_menu_11Conds = CriteriaSet.And();
			bool tableReload = true;

			//FOR: MENU LIST SORTING
			Dictionary<string, OrderedDictionary> allSortOrders = new Dictionary<string, OrderedDictionary>();
			allSortOrders.Add("AGENT.BIRTHDAT", new OrderedDictionary());
			allSortOrders["AGENT.BIRTHDAT"].Add("AGENT.BIRTHDAT", "A");


			int numberListItems = tableConfig.RowsPerPage;
			var pageNumber = ajaxRequest ? tableConfig.Page : 1;

			// Added to avoid 0 or -1 pages when setting number of records to -1 to disable pagination
			if (pageNumber < 1)
				pageNumber = 1;

			List<ColumnSort> sorts = GetRequestSorts(this.Menu, tableConfig, "agent", allSortOrders);

			if (sorts == null || sorts.Count == 0)
			{
				sorts = new List<ColumnSort>();
				sorts.Add(new ColumnSort(new ColumnReference(CSGenioAagent.FldBirthdat), SortOrder.Ascending));

			}

			FieldRef[] fields = new FieldRef[] { CSGenioAagent.FldCodagent, CSGenioAagent.FldZzstate, CSGenioAagent.FldCodcaddr, CSGenioAcaddr.FldCodcount, CSGenioAcaddr.FldCountry, CSGenioAagent.FldLastprop, CSGenioAagent.FldNrprops, CSGenioAagent.FldProfit, CSGenioAagent.FldAge, CSGenioAagent.FldBirthdat, CSGenioAagent.FldPhotography, CSGenioAagent.FldEmail, CSGenioAagent.FldTelephon, CSGenioAagent.FldName, CSGenioAagent.FldCborn, CSGenioAcborn.FldCodcount, CSGenioAcborn.FldCountry };

			// List of column names that should display totalized (aggregated) values.
			List<string> totalizerColumns = [];
			List<FieldRef> fieldsWithTotalizers = [.. fields.Where(field => totalizerColumns.Contains(field.FullName))];

			FieldRef firstVisibleColumn = null;
			if (sorts.Count == 0)
			{
				firstVisibleColumn = tableConfig?.GetFirstVisibleColumn(TableAlias);

				firstVisibleColumn ??= new FieldRef("caddr", "country");
			}
			// Limitations
			this.TableLimits ??= [];
			// Comparer to check if limit is already present in TableLimits
			LimitComparer limitComparer = new();

			//Tooltip for EPHs affecting this viewmodel list
			{
				Limit limit = new Limit();
				limit.TipoLimite = LimitType.EPH;
				CSGenioAagent model_limit_area = new CSGenioAagent(m_userContext.User);
				List<Limit> area_EPH_limits = EPH_Limit_Filler(ref limit, model_limit_area, "ML11");
				if (area_EPH_limits.Count > 0)
					this.TableLimits.AddRange(area_EPH_limits);
			}


			if (conditions == null)
				conditions = CriteriaSet.And();

			conditions.SubSets.Add(for_menu_11Conds);
			for_menu_11Conds = BuildCriteriaSet(tableConfig, requestValues, out bool hasAllRequiredLimits, conditions, isToExport);
			tableReload &= hasAllRequiredLimits;

// USE /[MANUAL FOR OVERRQ 11]/

			bool distinct = false;

			if (isToExport)
			{
				if (!tableReload)
					return;

				var exportColumns = GetExportColumns(tableConfig.ColumnConfigurations);
				var exportFieldRefs = exportColumns.Select(eCol => eCol.Field).Where(fldRef => fldRef != null).ToArray();

				Qlisting = Models.ModelBase.BuildListingForExport<CSGenioAagent>(m_userContext, false, ref for_menu_11Conds, exportFieldRefs, (pageNumber - 1) * numberListItems, numberListItems, sorts, "ML11", true, firstVisibleColumn: firstVisibleColumn);

// USE /[MANUAL FOR OVERRQLSTEXP 11]/

				return;
			}

			if (tableReload)
			{
// USE /[MANUAL FOR OVERRQLIST 11]/

				string QMVC_POS_RECORD = Navigation.GetStrValue("QMVC_POS_RECORD_agent");
				Navigation.DestroyEntry("QMVC_POS_RECORD_agent");
				CriteriaSet m_PagingPosEPHs = null;

				if (!string.IsNullOrEmpty(QMVC_POS_RECORD))
				{
					var m_iCurPag = m_userContext.PersistentSupport.getPagingPos(CSGenioAagent.GetInformation(), QMVC_POS_RECORD, sorts, for_menu_11Conds, m_PagingPosEPHs, firstVisibleColumn: firstVisibleColumn);
					if (m_iCurPag != -1)
						pageNumber = ((m_iCurPag - 1) / numberListItems) + 1;
				}

				ListingMVC<CSGenioAagent> listing = Models.ModelBase.Where<CSGenioAagent>(m_userContext, distinct, for_menu_11Conds, fields, (pageNumber - 1) * numberListItems, numberListItems, sorts, "ML11", true, false, QMVC_POS_RECORD, m_PagingPosEPHs, firstVisibleColumn, fieldsWithTotalizers, tableConfig.SelectedRows);

				if (listing.CurrentPage > 0)
					pageNumber = listing.CurrentPage;

				//Added to avoid 0 or -1 pages when setting number of records to -1 to disable pagination
				if (pageNumber < 1)
					pageNumber = 1;

				//Set document field values to objects
				SetDocumentFields(listing);

				Menu.Elements = MapFOR_Menu_11(listing);

				Menu.Identifier = "ML11";
				Menu.Slots = new Dictionary<string, List<object>>();

				// Last updated by [CJP] at [2015.02.03]
				// Adds the identifier to each element
				foreach (var element in Menu.Elements)
					element.Identifier = "ML11";

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

		private List<FOR_Menu_11_RowViewModel> MapFOR_Menu_11(ListingMVC<CSGenioAagent> Qlisting)
		{
			List<FOR_Menu_11_RowViewModel> Elements = [];
			int i = 0;

			if (Qlisting.Rows != null)
			{
				foreach (var row in Qlisting.Rows)
				{
					if (Qlisting.NumRegs > 0 && i >= Qlisting.NumRegs) // Copiado da versão antiga do RowsToViewModels
						break;
					Elements.Add(MapFOR_Menu_11(row));
					i++;
				}
			}

			return Elements;
		}

		/// <summary>
		/// Maps a single CSGenioAagent row
		/// to a FOR_Menu_11_RowViewModel object.
		/// </summary>
		/// <param name="row">The row.</param>
		private FOR_Menu_11_RowViewModel MapFOR_Menu_11(CSGenioAagent row)
		{
			var model = new FOR_Menu_11_RowViewModel(m_userContext, true, _fieldsToSerialize);
			if (row == null)
				return model;

			foreach (RequestedField Qfield in row.Fields.Values)
			{
				switch (Qfield.Area)
				{
					case "agent":
						model.klass.insertNameValueField(Qfield.FullName, Qfield.Value); break;
					case "caddr":
						model.Caddr.klass.insertNameValueField(Qfield.FullName, Qfield.Value); break;
					case "cborn":
						model.Cborn.klass.insertNameValueField(Qfield.FullName, Qfield.Value); break;
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
		private void SetDocumentFields(ListingMVC<CSGenioAagent> listing)
		{
		}

		#region Mapper

		/// <inheritdoc />
		public override void MapFromModel(Models.Agent m)
		{
		}

		/// <inheritdoc />
		public override void MapToModel(Models.Agent m)
		{
		}

		#endregion

		#region Custom code

// USE /[MANUAL FOR VIEWMODEL_CUSTOM FOR_MENU_11]/

		#endregion

		private static readonly string[] _fieldsToSerialize =
		[
			"Agent", "Agent.ValCodagent", "Agent.ValZzstate", "Caddr", "Caddr.ValCountry", "Agent.ValLastprop", "Agent.ValNrprops", "Agent.ValProfit", "Agent.ValAge", "Agent.ValBirthdat", "Agent.ValPhotography", "Agent.ValEmail", "Agent.ValTelephon", "Agent.ValName", "Cborn", "Cborn.ValCountry", "Agent.ValCodcaddr", "Agent.ValCborn"
		];

		private static readonly List<TableSearchColumn> _searchableColumns =
		[
			new TableSearchColumn("Caddr_ValCountry", CSGenioAcaddr.FldCountry, typeof(string)),
			new TableSearchColumn("ValLastprop", CSGenioAagent.FldLastprop, typeof(decimal?)),
			new TableSearchColumn("ValNrprops", CSGenioAagent.FldNrprops, typeof(decimal?)),
			new TableSearchColumn("ValProfit", CSGenioAagent.FldProfit, typeof(decimal?)),
			new TableSearchColumn("ValAge", CSGenioAagent.FldAge, typeof(decimal?)),
			new TableSearchColumn("ValBirthdat", CSGenioAagent.FldBirthdat, typeof(DateTime?)),
			new TableSearchColumn("ValEmail", CSGenioAagent.FldEmail, typeof(string)),
			new TableSearchColumn("ValTelephon", CSGenioAagent.FldTelephon, typeof(string)),
			new TableSearchColumn("ValName", CSGenioAagent.FldName, typeof(string), defaultSearch : true),
			new TableSearchColumn("Cborn_ValCountry", CSGenioAcborn.FldCountry, typeof(string)),
		];
		protected void SetTicketToImageFields(Models.Agent row)
		{
			if (row == null)
				return;

			row.ValPhotographyQTicket = Helpers.Helpers.GetFileTicket(m_userContext.User, CSGenio.business.Area.AreaAGENT, CSGenioAagent.FldPhotography.Field, null, row.ValCodagent);
		}
	}
}

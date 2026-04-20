using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Helpers;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Text.Json.Serialization;

namespace GenioMVC.ViewModels.City
{
	public class City_ViewModel : FormViewModel<Models.City>, IPreparableForSerialization
	{
		[JsonIgnore]
		public override bool HasWriteConditions { get => false; }

		/// <summary>
		/// Reference for the Models MsqActive property
		/// </summary>
		[JsonIgnore]
		public bool MsqActive { get; set; } = false;

		#region Foreign keys
		/// <summary>
		/// Title: "Country" | Type: "CE"
		/// </summary>
		public string ValCodcount { get; set; }

		#endregion

		/// <summary>
		/// Title: "City" | Type: "C"
		/// </summary>
		public string ValCity { get; set; }
		/// <summary>
		/// Title: "Country" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.Count> TableCountCountry { get; set; }

		#region Navigations
		#endregion

		#region Auxiliar Keys for Image controls



		#endregion

		#region Extra database fields



		#endregion

		#region Fields for formulas


		#endregion

		public string ValCodcity { get; set; }


		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// A call to Init() needs to be manually invoked after this constructor
		/// </summary>
		[Obsolete("For deserialization only")]
		public City_ViewModel() : base(null!) { }

		public City_ViewModel(UserContext userContext, bool nestedForm = false) : base(userContext, "FCITY", nestedForm) { }

		public City_ViewModel(UserContext userContext, Models.City row, bool nestedForm = false) : base(userContext, "FCITY", row, nestedForm) { }

		public City_ViewModel(UserContext userContext, string id, bool nestedForm = false, string[]? fieldsToLoad = null) : this(userContext, nestedForm)
		{
			this.Navigation.SetValue("city", id);
			Model = Models.City.Find(id, userContext, "FCITY", fieldsToQuery: fieldsToLoad);
			if (Model == null)
				throw new ModelNotFoundException("Model not found");
			InitModel();
		}

		protected override void InitLevels()
		{
			this.RoleToShow = CSGenio.framework.Role.ROLE_1;
			this.RoleToEdit = CSGenio.framework.Role.ROLE_1;
		}

		#region Form conditions

		public override StatusMessage InsertConditions()
		{
			return InsertConditions(m_userContext);
		}

		public static StatusMessage InsertConditions(UserContext userContext)
		{
			var m_userContext = userContext;
			StatusMessage result = new StatusMessage(Status.OK, "");
			Models.City model = new Models.City(userContext) { Identifier = "FCITY" };

			var navigation = m_userContext.CurrentNavigation;
			// The "LoadKeysFromHistory" must be after the "LoadEPH" because the PHE's in the tree mark Foreign Keys to null
			// (since they cannot assign multiple values to a single field) and thus the value that comes from Navigation is lost.
			// And this makes it more like the order of loading the model when opening the form.
			model.LoadEPH("FCITY");
			if (navigation != null)
				model.LoadKeysFromHistory(navigation, navigation.CurrentLevel.Level);

			var tableResult = model.EvaluateTableConditions(ConditionType.INSERT);
			result.MergeStatusMessage(tableResult);
			return result;
		}

		public override StatusMessage UpdateConditions()
		{
			StatusMessage result = new StatusMessage(Status.OK, "");
			var model = Model;

			var tableResult = model.EvaluateTableConditions(ConditionType.UPDATE);
			result.MergeStatusMessage(tableResult);
			return result;
		}

		public override StatusMessage DeleteConditions()
		{
			StatusMessage result = new StatusMessage(Status.OK, "");
			var model = Model;

			var tableResult = model.EvaluateTableConditions(ConditionType.DELETE);
			result.MergeStatusMessage(tableResult);
			return result;
		}

		public override StatusMessage ViewConditions()
		{
			var model = Model;
			StatusMessage result = model.EvaluateTableConditions(ConditionType.VIEW);
			var tableResult = model.EvaluateTableConditions(ConditionType.VIEW);
			result.MergeStatusMessage(tableResult);
			return result;
		}

		public override StatusMessage EvaluateWriteConditions(bool isApply)
		{
			StatusMessage result = new StatusMessage(Status.OK, "");
			return result;
		}

		public StatusMessage EvaluateTableConditions(ConditionType type)
		{
			return Model.EvaluateTableConditions(type);
		}

		#endregion

		#region Mapper

		/// <inheritdoc />
		public override void MapFromModel(Models.City m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map Model (City) to ViewModel (City) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				ValCodcount = ViewModelConversion.ToString(m.ValCodcount);
				ValCity = ViewModelConversion.ToString(m.ValCity);
				ValCodcity = ViewModelConversion.ToString(m.ValCodcity);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("Map Model (City) to ViewModel (City) - Error during mapping");
				throw;
			}
		}

		/// <inheritdoc />
		public override void MapToModel()
		{
			MapToModel(this.Model);
		}

		/// <inheritdoc />
		public override void MapToModel(Models.City m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map ViewModel (City) to Model (City) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				m.ValCodcount = ViewModelConversion.ToString(ValCodcount);
				m.ValCity = ViewModelConversion.ToString(ValCity);
				m.ValCodcity = ViewModelConversion.ToString(ValCodcity);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error($"Map ViewModel (City) to Model (City) - Error during mapping. All user values: {HasDisabledUserValuesSecurity}");
				throw;
			}
		}

		/// <inheritdoc />
		public override void SetViewModelValue(string fullFieldName, object value)
		{
			try
			{
				ArgumentNullException.ThrowIfNull(fullFieldName);
				// Obtain a valid value from JsonValueKind that can come from "prefillValues" during the pre-filling of fields during insertion
				var _value = ViewModelConversion.ToRawValue(value);

				switch (fullFieldName)
				{
					case "city.codcount":
						this.ValCodcount = ViewModelConversion.ToString(_value);
						break;
					case "city.city":
						this.ValCity = ViewModelConversion.ToString(_value);
						break;
					case "city.codcity":
						this.ValCodcity = ViewModelConversion.ToString(_value);
						break;
					default:
						Log.Error($"SetViewModelValue (City) - Unexpected field identifier {fullFieldName}");
						break;
				}
			}
			catch (Exception ex)
			{
				throw new FrameworkException(Resources.Resources.PEDIMOS_DESCULPA__OC63848, "SetViewModelValue (City)", "Unexpected error", ex);
			}
		}

		#endregion

		/// <summary>
		/// Reads the Model from the database based on the key that is in the history or that was passed through the parameter
		/// </summary>
		/// <param name="id">The primary key of the record that needs to be read from the database. Leave NULL to use the value from the History.</param>
		public override void LoadModel(string id = null)
		{
			try { Model = Models.City.Find(id ?? Navigation.GetStrValue("city"), m_userContext, "FCITY"); }
			finally { Model ??= new Models.City(m_userContext) { Identifier = "FCITY" }; }

			base.LoadModel();
		}

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;
			CSGenio.business.Area oldvalues = null;

			// TODO: Deve ser substituido por search do CSGenioA
			try
			{
				Model = Models.City.Find(Navigation.GetStrValue("city"), m_userContext, "FCITY");
			}
			finally
			{
				if (Model == null)
					throw new ModelNotFoundException("Model not found");

				if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
					LoadDefaultValues();
				else
					oldvalues = Model.klass;
			}

			Model.Identifier = "FCITY";
			InitModel(qs, lazyLoad);

			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Edit || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				// MH - Voltar calcular as formulas to "atualizar" os Qvalues dos fields fixos
				// Conexão deve estar aberta de fora. Podem haver formulas que utilizam funções "manuais".
				// TODO: It needs to be analyzed whether we should disable the security of field filling here. If there is any case where the field with the block condition can only be calculated after the double calculation of the formulas.
				MapToModel(Model);

				// If it's inserting or duplicating, needs to fill the default values.
				if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
				{
					FunctionType funcType = Navigation.CurrentLevel.FormMode == FormMode.New
						? FunctionType.INS
						: FunctionType.DUP;

					Model.baseklass.fillValuesDefault(m_userContext.PersistentSupport, funcType);
				}

				// Preencher operações internas
				Model.klass.fillInternalOperations(m_userContext.PersistentSupport, oldvalues);
				MapFromModel(Model);
			}

			// Load just the selected row primary keys for checklists.
			// Needed for submitting forms incase checklists are in collapsible zones that have not been expanded to load the checklist data.
			LoadChecklistsSelectedIDs();
		}

		protected override void FillExtraProperties()
		{
		}

		protected override void LoadDocumentsProperties(Models.City row)
		{
		}

		/// <summary>
		/// Load Partial
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public override void LoadPartial(NameValueCollection qs, bool lazyLoad = false)
		{
			// MH [bugfix] - Quando o POST da ficha falha, ao recaregar a view os documentos na BD perdem alguma informação (ex: name do file)
			if (Model == null)
			{
				// Precisamos fazer o Find to obter as chaves dos documentos que já foram anexados
				// TODO: Conseguir passar estas chaves no POST to poder retirar o Find.
				Model = Models.City.Find(Navigation.GetStrValue("city"), m_userContext, "FCITY");
				if (Model == null)
				{
					Model = new Models.City(m_userContext) { Identifier = "FCITY" };
					Model.klass.QPrimaryKey = Navigation.GetStrValue("city");
				}
				MapToModel(Model);
				LoadDocumentsProperties(Model);
			}
			// Add characteristics
			Characs = new List<string>();

			Load_City____countcountry_(qs, lazyLoad);

// USE /[MANUAL FOR VIEWMODEL_LOADPARTIAL CITY]/
		}

// USE /[MANUAL FOR VIEWMODEL_NEW CITY]/

		// Preencher Qvalues default dos fields do form
		protected override void LoadDefaultValues()
		{
		}

		public override CrudViewModelValidationResult Validate()
		{
			CrudViewModelFieldValidator validator = new(m_userContext.User.Language);

			validator.StringLength("ValCity", Resources.Resources.CITY42505, ValCity, 50);


			return validator.GetResult();
		}

		public override void Init(UserContext userContext)
		{
			base.Init(userContext);
		}
// USE /[MANUAL FOR VIEWMODEL_SAVE CITY]/
		public override void Save()
		{


			base.Save();
		}

// USE /[MANUAL FOR VIEWMODEL_APPLY CITY]/

// USE /[MANUAL FOR VIEWMODEL_DUPLICATE CITY]/

// USE /[MANUAL FOR VIEWMODEL_DESTROY CITY]/
		public override void Destroy(string id)
		{
			Model = Models.City.Find(id, m_userContext, "FCITY");
			if (Model == null)
				throw new ModelNotFoundException("Model not found");
			this.flashMessage = Model.Destroy();
		}

		/// <summary>
		/// Load selected row primary keys for all checklists
		/// </summary>
		public void LoadChecklistsSelectedIDs()
		{
		}

		/// <summary>
		/// TableCountCountry -> (DB)
		/// </summary>
		/// <param name="qs"></param>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void Load_City____countcountry_(NameValueCollection qs, bool lazyLoad = false)
		{
			bool city____countcountry_DoLoad = true;
			CriteriaSet city____countcountry_Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("count", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					city____countcountry_Conds.Equal(CSGenioAcount.FldCodcount, hValue);
					this.ValCodcount = DBConversion.ToString(hValue);
				}
			}

			TableCountCountry = new TableDBEdit<Models.Count>();

			if (lazyLoad)
			{
				if (Navigation.CurrentLevel.GetEntry("RETURN_count") != null)
				{
					this.ValCodcount = Navigation.GetStrValue("RETURN_count");
					Navigation.CurrentLevel.SetEntry("RETURN_count", null);
				}
				FillDependant_CityTableCountCountry(lazyLoad);
				return;
			}

			if (city____countcountry_DoLoad)
			{
				List<ColumnSort> sorts = [];
				ColumnSort requestedSort = GetRequestSort(TableCountCountry, "sTableCountCountry", "dTableCountCountry", qs, "count");
				if (requestedSort != null)
					sorts.Add(requestedSort);

				string query = "";
				if (!string.IsNullOrEmpty(qs["TableCountCountry_tableFilters"]))
					TableCountCountry.TableFilters = bool.Parse(qs["TableCountCountry_tableFilters"]);
				else
					TableCountCountry.TableFilters = false;

				query = qs["qTableCountCountry"];

				//RS 26.07.2016 O preenchimento da lista de ajuda dos Dbedits passa a basear-se apenas no campo do próprio DbEdit
				// O interface de pesquisa rápida não fica coerente quando se visualiza apenas uma coluna mas a pesquisa faz matching com 5 ou 6 colunas diferentes
				//  tornando confuso to o user porque determinada row foi devolvida quando o Qresult não mostra como o matching foi feito
				CriteriaSet search_filters = CriteriaSet.And();
				if (!string.IsNullOrEmpty(query))
				{
					search_filters.Like(CSGenioAcount.FldCountry, query + "%");
				}
				city____countcountry_Conds.SubSet(search_filters);

				string tryParsePage = qs["pTableCountCountry"] != null ? qs["pTableCountCountry"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = [CSGenioAcount.FldCodcount, CSGenioAcount.FldCountry, CSGenioAcount.FldZzstate];

// USE /[MANUAL FOR OVERRQ CITY_COUNTCOUNTRY]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("count", FormMode.New) || Navigation.checkFormMode("count", FormMode.Duplicate))
					city____countcountry_Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioAcount.FldZzstate, 0)
						.Equal(CSGenioAcount.FldCodcount, Navigation.GetStrValue("count")));
				else
					city____countcountry_Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioAcount.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("count", "country");
				ListingMVC<CSGenioAcount> listing = Models.ModelBase.Where<CSGenioAcount>(m_userContext, false, city____countcountry_Conds, fields, offset, numberItems, sorts, "LED_CITY____COUNTCOUNTRY_", true, false, firstVisibleColumn: firstVisibleColumn);

				TableCountCountry.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TableCountCountry.Query = query;
				TableCountCountry.Elements = listing.RowsForViewModel((r) => new GenioMVC.Models.Count(m_userContext, r, true, _fieldsToSerialize_CITY____COUNTCOUNTRY_));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_count") != null)
				{
					this.ValCodcount = Navigation.GetStrValue("RETURN_count");
					Navigation.CurrentLevel.SetEntry("RETURN_count", null);
				}

				TableCountCountry.List = new SelectList(TableCountCountry.Elements.ToSelectList(x => x.ValCountry, x => x.ValCodcount,  x => x.ValCodcount == this.ValCodcount), "Value", "Text", this.ValCodcount);
				FillDependant_CityTableCountCountry();
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TableCountCountry (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of Count</param>
		public ConcurrentDictionary<string, object> GetDependant_CityTableCountCountry(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioAcount.FldCodcount, CSGenioAcount.FldCountry];

			var returnEmptyDependants = false;
			CriteriaSet wherecodition = CriteriaSet.And();

			// Return default values
			if (GenFunctions.emptyG(PKey) == 1)
				returnEmptyDependants = true;

			// Check if the limit(s) is filled if exists
			// - - - - - - - - - - - - - - - - - - - - -

			if (returnEmptyDependants)
				return GetViewModelFieldValues(refDependantFields);

			PersistentSupport sp = m_userContext.PersistentSupport;
			User u = m_userContext.User;

			CSGenioAcount tempArea = new(u);

			// Fields to select
			SelectQuery querySelect = new();
			querySelect.PageSize(1);
			foreach (FieldRef field in refDependantFields)
				querySelect.Select(field);

			querySelect.From(tempArea.QSystem, tempArea.TableName, tempArea.Alias)
				.Where(wherecodition.Equal(CSGenioAcount.FldCodcount, PKey));

			string[] dependantFields = refDependantFields.Select(f => f.FullName).ToArray();
			QueryUtils.SetInnerJoins(dependantFields, null, tempArea, querySelect);

			ArrayList values = sp.executeReaderOneRow(querySelect);
			bool useDefaults = values.Count == 0;

			if (useDefaults)
				return GetViewModelFieldValues(refDependantFields);
			return GetViewModelFieldValues(refDependantFields, values);
		}

		/// <summary>
		/// Fill Dependant fields values -> TableCountCountry (DB)
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void FillDependant_CityTableCountCountry(bool lazyLoad = false)
		{
			var row = GetDependant_CityTableCountCountry(this.ValCodcount);
			try
			{

				// Fill List fields
				this.ValCodcount = ViewModelConversion.ToString(row["count.codcount"]);
				TableCountCountry.Value = (string)row["count.country"];
				if (GenFunctions.emptyG(this.ValCodcount) == 1)
				{
					this.ValCodcount = "";
					TableCountCountry.Value = "";
					Navigation.ClearValue("count");
				}
				else if (lazyLoad)
				{
					TableCountCountry.SetPagination(1, 0, false, false, 1);
					TableCountCountry.List = new SelectList(new List<SelectListItem>()
					{
						new SelectListItem
						{
							Value = Convert.ToString(this.ValCodcount),
							Text = Convert.ToString(TableCountCountry.Value),
							Selected = true
						}
					}, "Value", "Text", this.ValCodcount);
				}

				TableCountCountry.Selected = this.ValCodcount;
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(string.Format("FillDependant_Error (TableCountCountry): {0}; {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		private readonly string[] _fieldsToSerialize_CITY____COUNTCOUNTRY_ = ["Count", "Count.ValCodcount", "Count.ValZzstate", "Count.ValCountry"];

		protected override object GetViewModelValue(string identifier, object modelValue)
		{
			return identifier switch
			{
				"city.codcount" => ViewModelConversion.ToString(modelValue),
				"city.city" => ViewModelConversion.ToString(modelValue),
				"city.codcity" => ViewModelConversion.ToString(modelValue),
				"count.codcount" => ViewModelConversion.ToString(modelValue),
				"count.country" => ViewModelConversion.ToString(modelValue),
				_ => modelValue
			};
		}

		#region Charts


		#endregion

		#region Custom code

// USE /[MANUAL FOR VIEWMODEL_CUSTOM CITY]/

		#endregion
	}
}

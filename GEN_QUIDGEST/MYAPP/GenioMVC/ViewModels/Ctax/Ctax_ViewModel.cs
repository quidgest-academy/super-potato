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

namespace GenioMVC.ViewModels.Ctax
{
	public class Ctax_ViewModel : FormViewModel<Models.Ctax>, IPreparableForSerialization
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
		/// Title: "City" | Type: "CE"
		/// </summary>
		public string ValCodcity { get; set; }

		#endregion

		/// <summary>
		/// Title: "City" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.City> TableCityCity { get; set; }
		/// <summary>
		/// Title: "Tax" | Type: "N"
		/// </summary>
		public decimal? ValTax { get; set; }

		#region Navigations
		#endregion

		#region Auxiliar Keys for Image controls



		#endregion

		#region Extra database fields



		#endregion

		#region Fields for formulas


		#endregion

		public string ValCodctax { get; set; }


		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// A call to Init() needs to be manually invoked after this constructor
		/// </summary>
		[Obsolete("For deserialization only")]
		public Ctax_ViewModel() : base(null!) { }

		public Ctax_ViewModel(UserContext userContext, bool nestedForm = false) : base(userContext, "FCTAX", nestedForm) { }

		public Ctax_ViewModel(UserContext userContext, Models.Ctax row, bool nestedForm = false) : base(userContext, "FCTAX", row, nestedForm) { }

		public Ctax_ViewModel(UserContext userContext, string id, bool nestedForm = false, string[]? fieldsToLoad = null) : this(userContext, nestedForm)
		{
			this.Navigation.SetValue("ctax", id);
			Model = Models.Ctax.Find(id, userContext, "FCTAX", fieldsToQuery: fieldsToLoad);
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
			Models.Ctax model = new Models.Ctax(userContext) { Identifier = "FCTAX" };

			var navigation = m_userContext.CurrentNavigation;
			// The "LoadKeysFromHistory" must be after the "LoadEPH" because the PHE's in the tree mark Foreign Keys to null
			// (since they cannot assign multiple values to a single field) and thus the value that comes from Navigation is lost.
			// And this makes it more like the order of loading the model when opening the form.
			model.LoadEPH("FCTAX");
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
		public override void MapFromModel(Models.Ctax m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map Model (Ctax) to ViewModel (Ctax) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				ValCodcity = ViewModelConversion.ToString(m.ValCodcity);
				ValTax = ViewModelConversion.ToNumeric(m.ValTax);
				ValCodctax = ViewModelConversion.ToString(m.ValCodctax);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("Map Model (Ctax) to ViewModel (Ctax) - Error during mapping");
				throw;
			}
		}

		/// <inheritdoc />
		public override void MapToModel()
		{
			MapToModel(this.Model);
		}

		/// <inheritdoc />
		public override void MapToModel(Models.Ctax m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map ViewModel (Ctax) to Model (Ctax) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				m.ValCodcity = ViewModelConversion.ToString(ValCodcity);
				m.ValTax = ViewModelConversion.ToNumeric(ValTax);
				m.ValCodctax = ViewModelConversion.ToString(ValCodctax);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error($"Map ViewModel (Ctax) to Model (Ctax) - Error during mapping. All user values: {HasDisabledUserValuesSecurity}");
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
					case "ctax.codcity":
						this.ValCodcity = ViewModelConversion.ToString(_value);
						break;
					case "ctax.tax":
						this.ValTax = ViewModelConversion.ToNumeric(_value);
						break;
					case "ctax.codctax":
						this.ValCodctax = ViewModelConversion.ToString(_value);
						break;
					default:
						Log.Error($"SetViewModelValue (Ctax) - Unexpected field identifier {fullFieldName}");
						break;
				}
			}
			catch (Exception ex)
			{
				throw new FrameworkException(Resources.Resources.PEDIMOS_DESCULPA__OC63848, "SetViewModelValue (Ctax)", "Unexpected error", ex);
			}
		}

		#endregion

		/// <summary>
		/// Reads the Model from the database based on the key that is in the history or that was passed through the parameter
		/// </summary>
		/// <param name="id">The primary key of the record that needs to be read from the database. Leave NULL to use the value from the History.</param>
		public override void LoadModel(string id = null)
		{
			try { Model = Models.Ctax.Find(id ?? Navigation.GetStrValue("ctax"), m_userContext, "FCTAX"); }
			finally { Model ??= new Models.Ctax(m_userContext) { Identifier = "FCTAX" }; }

			base.LoadModel();
		}

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;
			CSGenio.business.Area oldvalues = null;

			// TODO: Deve ser substituido por search do CSGenioA
			try
			{
				Model = Models.Ctax.Find(Navigation.GetStrValue("ctax"), m_userContext, "FCTAX");
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

			Model.Identifier = "FCTAX";
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

		protected override void LoadDocumentsProperties(Models.Ctax row)
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
				Model = Models.Ctax.Find(Navigation.GetStrValue("ctax"), m_userContext, "FCTAX");
				if (Model == null)
				{
					Model = new Models.Ctax(m_userContext) { Identifier = "FCTAX" };
					Model.klass.QPrimaryKey = Navigation.GetStrValue("ctax");
				}
				MapToModel(Model);
				LoadDocumentsProperties(Model);
			}
			// Add characteristics
			Characs = new List<string>();

			Load_Ctax____city_city____(qs, lazyLoad);

// USE /[MANUAL FOR VIEWMODEL_LOADPARTIAL CTAX]/
		}

// USE /[MANUAL FOR VIEWMODEL_NEW CTAX]/

		// Preencher Qvalues default dos fields do form
		protected override void LoadDefaultValues()
		{
		}

		public override CrudViewModelValidationResult Validate()
		{
			CrudViewModelFieldValidator validator = new(m_userContext.User.Language);



			return validator.GetResult();
		}

		public override void Init(UserContext userContext)
		{
			base.Init(userContext);
		}
// USE /[MANUAL FOR VIEWMODEL_SAVE CTAX]/
		public override void Save()
		{


			base.Save();
		}

// USE /[MANUAL FOR VIEWMODEL_APPLY CTAX]/

// USE /[MANUAL FOR VIEWMODEL_DUPLICATE CTAX]/

// USE /[MANUAL FOR VIEWMODEL_DESTROY CTAX]/
		public override void Destroy(string id)
		{
			Model = Models.Ctax.Find(id, m_userContext, "FCTAX");
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
		/// TableCityCity -> (DB)
		/// </summary>
		/// <param name="qs"></param>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void Load_Ctax____city_city____(NameValueCollection qs, bool lazyLoad = false)
		{
			bool ctax____city_city____DoLoad = true;
			CriteriaSet ctax____city_city____Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("city", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					ctax____city_city____Conds.Equal(CSGenioAcity.FldCodcity, hValue);
					this.ValCodcity = DBConversion.ToString(hValue);
				}
			}

			TableCityCity = new TableDBEdit<Models.City>();

			if (lazyLoad)
			{
				if (Navigation.CurrentLevel.GetEntry("RETURN_city") != null)
				{
					this.ValCodcity = Navigation.GetStrValue("RETURN_city");
					Navigation.CurrentLevel.SetEntry("RETURN_city", null);
				}
				FillDependant_CtaxTableCityCity(lazyLoad);
				return;
			}

			if (ctax____city_city____DoLoad)
			{
				List<ColumnSort> sorts = [];
				ColumnSort requestedSort = GetRequestSort(TableCityCity, "sTableCityCity", "dTableCityCity", qs, "city");
				if (requestedSort != null)
					sorts.Add(requestedSort);
				sorts.Add(new ColumnSort(new ColumnReference(CSGenioAcity.FldCity), SortOrder.Ascending));

				string query = "";
				if (!string.IsNullOrEmpty(qs["TableCityCity_tableFilters"]))
					TableCityCity.TableFilters = bool.Parse(qs["TableCityCity_tableFilters"]);
				else
					TableCityCity.TableFilters = false;

				query = qs["qTableCityCity"];

				//RS 26.07.2016 O preenchimento da lista de ajuda dos Dbedits passa a basear-se apenas no campo do próprio DbEdit
				// O interface de pesquisa rápida não fica coerente quando se visualiza apenas uma coluna mas a pesquisa faz matching com 5 ou 6 colunas diferentes
				//  tornando confuso to o user porque determinada row foi devolvida quando o Qresult não mostra como o matching foi feito
				CriteriaSet search_filters = CriteriaSet.And();
				if (!string.IsNullOrEmpty(query))
				{
					search_filters.Like(CSGenioAcity.FldCity, query + "%");
				}
				ctax____city_city____Conds.SubSet(search_filters);

				string tryParsePage = qs["pTableCityCity"] != null ? qs["pTableCityCity"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = [CSGenioAcity.FldCodcity, CSGenioAcity.FldCity, CSGenioAcity.FldZzstate];

// USE /[MANUAL FOR OVERRQ CTAX_CITYCITY]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("city", FormMode.New) || Navigation.checkFormMode("city", FormMode.Duplicate))
					ctax____city_city____Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioAcity.FldZzstate, 0)
						.Equal(CSGenioAcity.FldCodcity, Navigation.GetStrValue("city")));
				else
					ctax____city_city____Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioAcity.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("city", "city");
				ListingMVC<CSGenioAcity> listing = Models.ModelBase.Where<CSGenioAcity>(m_userContext, false, ctax____city_city____Conds, fields, offset, numberItems, sorts, "LED_CTAX____CITY_CITY____", true, false, firstVisibleColumn: firstVisibleColumn);

				TableCityCity.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TableCityCity.Query = query;
				TableCityCity.Elements = listing.RowsForViewModel((r) => new GenioMVC.Models.City(m_userContext, r, true, _fieldsToSerialize_CTAX____CITY_CITY____));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_city") != null)
				{
					this.ValCodcity = Navigation.GetStrValue("RETURN_city");
					Navigation.CurrentLevel.SetEntry("RETURN_city", null);
				}

				TableCityCity.List = new SelectList(TableCityCity.Elements.ToSelectList(x => x.ValCity, x => x.ValCodcity,  x => x.ValCodcity == this.ValCodcity), "Value", "Text", this.ValCodcity);
				FillDependant_CtaxTableCityCity();
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TableCityCity (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of City</param>
		public ConcurrentDictionary<string, object> GetDependant_CtaxTableCityCity(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioAcity.FldCodcity, CSGenioAcity.FldCity];

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

			CSGenioAcity tempArea = new(u);

			// Fields to select
			SelectQuery querySelect = new();
			querySelect.PageSize(1);
			foreach (FieldRef field in refDependantFields)
				querySelect.Select(field);

			querySelect.From(tempArea.QSystem, tempArea.TableName, tempArea.Alias)
				.Where(wherecodition.Equal(CSGenioAcity.FldCodcity, PKey));

			string[] dependantFields = refDependantFields.Select(f => f.FullName).ToArray();
			QueryUtils.SetInnerJoins(dependantFields, null, tempArea, querySelect);

			ArrayList values = sp.executeReaderOneRow(querySelect);
			bool useDefaults = values.Count == 0;

			if (useDefaults)
				return GetViewModelFieldValues(refDependantFields);
			return GetViewModelFieldValues(refDependantFields, values);
		}

		/// <summary>
		/// Fill Dependant fields values -> TableCityCity (DB)
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void FillDependant_CtaxTableCityCity(bool lazyLoad = false)
		{
			var row = GetDependant_CtaxTableCityCity(this.ValCodcity);
			try
			{

				// Fill List fields
				this.ValCodcity = ViewModelConversion.ToString(row["city.codcity"]);
				TableCityCity.Value = (string)row["city.city"];
				if (GenFunctions.emptyG(this.ValCodcity) == 1)
				{
					this.ValCodcity = "";
					TableCityCity.Value = "";
					Navigation.ClearValue("city");
				}
				else if (lazyLoad)
				{
					TableCityCity.SetPagination(1, 0, false, false, 1);
					TableCityCity.List = new SelectList(new List<SelectListItem>()
					{
						new SelectListItem
						{
							Value = Convert.ToString(this.ValCodcity),
							Text = Convert.ToString(TableCityCity.Value),
							Selected = true
						}
					}, "Value", "Text", this.ValCodcity);
				}

				TableCityCity.Selected = this.ValCodcity;
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(string.Format("FillDependant_Error (TableCityCity): {0}; {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		private readonly string[] _fieldsToSerialize_CTAX____CITY_CITY____ = ["City", "City.ValCodcity", "City.ValZzstate", "City.ValCity"];

		protected override object GetViewModelValue(string identifier, object modelValue)
		{
			return identifier switch
			{
				"ctax.codcity" => ViewModelConversion.ToString(modelValue),
				"ctax.tax" => ViewModelConversion.ToNumeric(modelValue),
				"ctax.codctax" => ViewModelConversion.ToString(modelValue),
				"city.codcity" => ViewModelConversion.ToString(modelValue),
				"city.city" => ViewModelConversion.ToString(modelValue),
				_ => modelValue
			};
		}

		#region Charts


		#endregion

		#region Custom code

// USE /[MANUAL FOR VIEWMODEL_CUSTOM CTAX]/

		#endregion
	}
}

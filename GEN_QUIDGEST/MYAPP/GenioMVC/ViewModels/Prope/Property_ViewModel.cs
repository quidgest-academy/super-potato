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

namespace GenioMVC.ViewModels.Prope
{
	public class Property_ViewModel : FormViewModel<Models.Prope>, IPreparableForSerialization
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
		/// Title: "Agent's name" | Type: "CE"
		/// </summary>
		public string ValCodagent { get; set; }
		/// <summary>
		/// Title: "City" | Type: "CE"
		/// </summary>
		public string ValCodcity { get; set; }

		#endregion

		/// <summary>
		/// Title: "Order" | Type: "N"
		/// </summary>
		public decimal? ValId { get; set; }
		/// <summary>
		/// Title: "Sold" | Type: "L"
		/// </summary>
		public bool ValSold { get; set; }
		/// <summary>
		/// Title: "Sold date" | Type: "D"
		/// </summary>
		public DateTime? ValDtsold { get; set; }
		/// <summary>
		/// Title: "Last Visit" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public string ValLastvisit { get; set; }
		/// <summary>
		/// Title: "AveragePrice" | Type: "N"
		/// </summary>
		[ValidateSetAccess]
		public decimal? ValAverage { get; set; }
		/// <summary>
		/// Title: "City" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.City> TableCityCity { get; set; }
		/// <summary>
		/// Title: "Country" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public string CityCountValCountry
		{
			get
			{
				return funcCityCountValCountry != null ? funcCityCountValCountry() : _auxCityCountValCountry;
			}
			set { funcCityCountValCountry = () => value; }
		}

		[JsonIgnore]
		public Func<string> funcCityCountValCountry { get; set; }

		private string _auxCityCountValCountry { get; set; }
		/// <summary>
		/// Title: "Price" | Type: "$"
		/// </summary>
		public decimal? ValPrice { get; set; }
		/// <summary>
		/// Title: "Building typology" | Type: "AN"
		/// </summary>
		public decimal ValTypology { get; set; }
		/// <summary>
		/// Title: "Building type" | Type: "AC"
		/// </summary>
		public string ValBuildtyp { get; set; }
		/// <summary>
		/// Title: "Ground size" | Type: "N"
		/// </summary>
		public decimal? ValGrdsize { get; set; }
		/// <summary>
		/// Title: "Floor" | Type: "N"
		/// </summary>
		public decimal? ValFloornr { get; set; }
		/// <summary>
		/// Title: "Size (m2)" | Type: "N"
		/// </summary>
		public decimal? ValSize { get; set; }
		/// <summary>
		/// Title: "Bathrooms number" | Type: "N"
		/// </summary>
		public decimal? ValBathnr { get; set; }
		/// <summary>
		/// Title: "Construction date" | Type: "D"
		/// </summary>
		public DateTime? ValDtconst { get; set; }
		/// <summary>
		/// Title: "Building age" | Type: "N"
		/// </summary>
		[ValidateSetAccess]
		public decimal? ValBuildage { get; set; }
		/// <summary>
		/// Title: "Agent's name" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.Agent> TableAgentName { get; set; }
		/// <summary>
		/// Title: "Photography" | Type: "IJ"
		/// </summary>
		[ImageThumbnailJsonConverter(30, 50)]
		[ValidateSetAccess]
		public GenioMVC.Models.ImageModel AgentValPhotography
		{
			get
			{
				return funcAgentValPhotography != null ? funcAgentValPhotography() : _auxAgentValPhotography;
			}
			set { funcAgentValPhotography = () => value; }
		}

		[JsonIgnore]
		public Func<GenioMVC.Models.ImageModel> funcAgentValPhotography { get; set; }

		private GenioMVC.Models.ImageModel _auxAgentValPhotography { get; set; }
		/// <summary>
		/// Title: "E-mail" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public string AgentValEmail
		{
			get
			{
				return funcAgentValEmail != null ? funcAgentValEmail() : _auxAgentValEmail;
			}
			set { funcAgentValEmail = () => value; }
		}

		[JsonIgnore]
		public Func<string> funcAgentValEmail { get; set; }

		private string _auxAgentValEmail { get; set; }
		/// <summary>
		/// Title: "Profit" | Type: "$"
		/// </summary>
		[ValidateSetAccess]
		public decimal? ValProfit { get; set; }
		/// <summary>
		/// Title: "Tax" | Type: "N"
		/// </summary>
		[ValidateSetAccess]
		public decimal? ValTax { get; set; }
		/// <summary>
		/// Title: "Title" | Type: "C"
		/// </summary>
		public string ValTitle { get; set; }
		/// <summary>
		/// Title: "Main photo" | Type: "IJ"
		/// </summary>
		[ImageThumbnailJsonConverter(30, 50)]
		public GenioMVC.Models.ImageModel ValPhoto { get; set; }
		/// <summary>
		/// Title: "Description" | Type: "MO"
		/// </summary>
		public string ValDescript { get; set; }

		#region Navigations
		#endregion

		#region Auxiliar Keys for Image controls



		#endregion

		#region Extra database fields



		#endregion

		#region Fields for formulas

		// Field for formula
		/// <summary>Field: "" Tipo: "+"</summary>
		[ValidateSetAccess]
		public string ValCodprope { get; set; }

		#endregion



		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// A call to Init() needs to be manually invoked after this constructor
		/// </summary>
		[Obsolete("For deserialization only")]
		public Property_ViewModel() : base(null!) { }

		public Property_ViewModel(UserContext userContext, bool nestedForm = false) : base(userContext, "FPROPERTY", nestedForm) { }

		public Property_ViewModel(UserContext userContext, Models.Prope row, bool nestedForm = false) : base(userContext, "FPROPERTY", row, nestedForm) { }

		public Property_ViewModel(UserContext userContext, string id, bool nestedForm = false, string[]? fieldsToLoad = null) : this(userContext, nestedForm)
		{
			this.Navigation.SetValue("prope", id);
			Model = Models.Prope.Find(id, userContext, "FPROPERTY", fieldsToQuery: fieldsToLoad);
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
			Models.Prope model = new Models.Prope(userContext) { Identifier = "FPROPERTY" };

			var navigation = m_userContext.CurrentNavigation;
			// The "LoadKeysFromHistory" must be after the "LoadEPH" because the PHE's in the tree mark Foreign Keys to null
			// (since they cannot assign multiple values to a single field) and thus the value that comes from Navigation is lost.
			// And this makes it more like the order of loading the model when opening the form.
			model.LoadEPH("FPROPERTY");
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
		public override void MapFromModel(Models.Prope m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map Model (Prope) to ViewModel (Property) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				ValCodagent = ViewModelConversion.ToString(m.ValCodagent);
				ValCodcity = ViewModelConversion.ToString(m.ValCodcity);
				ValId = ViewModelConversion.ToNumeric(m.ValId);
				ValSold = ViewModelConversion.ToLogic(m.ValSold);
				ValDtsold = ViewModelConversion.ToDateTime(m.ValDtsold);
				ValLastvisit = ViewModelConversion.ToString(m.ValLastvisit);
				ValAverage = ViewModelConversion.ToNumeric(m.ValAverage);
				ValPrice = ViewModelConversion.ToNumeric(m.ValPrice);
				ValTypology = ViewModelConversion.ToNumeric(m.ValTypology);
				ValBuildtyp = ViewModelConversion.ToString(m.ValBuildtyp);
				ValGrdsize = ViewModelConversion.ToNumeric(m.ValGrdsize);
				ValFloornr = ViewModelConversion.ToNumeric(m.ValFloornr);
				ValSize = ViewModelConversion.ToNumeric(m.ValSize);
				ValBathnr = ViewModelConversion.ToNumeric(m.ValBathnr);
				ValDtconst = ViewModelConversion.ToDateTime(m.ValDtconst);
				ValBuildage = ViewModelConversion.ToNumeric(m.ValBuildage);
				funcAgentValPhotography = () => ViewModelConversion.ToImage(m.Agent.ValPhotography);
				funcAgentValEmail = () => ViewModelConversion.ToString(m.Agent.ValEmail);
				ValProfit = ViewModelConversion.ToNumeric(m.ValProfit);
				ValTax = ViewModelConversion.ToNumeric(m.ValTax);
				ValTitle = ViewModelConversion.ToString(m.ValTitle);
				ValPhoto = ViewModelConversion.ToImage(m.ValPhoto);
				ValDescript = ViewModelConversion.ToString(m.ValDescript);
				ValCodprope = ViewModelConversion.ToString(m.ValCodprope);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("Map Model (Prope) to ViewModel (Property) - Error during mapping");
				throw;
			}
		}

		/// <inheritdoc />
		public override void MapToModel()
		{
			MapToModel(this.Model);
		}

		/// <inheritdoc />
		public override void MapToModel(Models.Prope m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map ViewModel (Property) to Model (Prope) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				m.ValCodagent = ViewModelConversion.ToString(ValCodagent);
				m.ValCodcity = ViewModelConversion.ToString(ValCodcity);
				m.ValId = ViewModelConversion.ToNumeric(ValId);
				m.ValSold = ViewModelConversion.ToLogic(ValSold);
				m.ValDtsold = ViewModelConversion.ToDateTime(ValDtsold);
				m.ValPrice = ViewModelConversion.ToNumeric(ValPrice);
				m.ValTypology = ViewModelConversion.ToNumeric(ValTypology);
				m.ValBuildtyp = ViewModelConversion.ToString(ValBuildtyp);
				m.ValGrdsize = ViewModelConversion.ToNumeric(ValGrdsize);
				m.ValFloornr = ViewModelConversion.ToNumeric(ValFloornr);
				m.ValSize = ViewModelConversion.ToNumeric(ValSize);
				m.ValBathnr = ViewModelConversion.ToNumeric(ValBathnr);
				m.ValDtconst = ViewModelConversion.ToDateTime(ValDtconst);
				m.ValTitle = ViewModelConversion.ToString(ValTitle);
				if (ValPhoto == null || !ValPhoto.IsThumbnail)
					m.ValPhoto = ViewModelConversion.ToImage(ValPhoto);
				m.ValDescript = ViewModelConversion.ToString(ValDescript);

				/*
					At this moment, in the case of runtime calculation of server-side formulas, to improve performance and reduce database load,
						the values coming from the client-side will be accepted as valid, since they will not be saved and are only being used for calculation.
				*/
				if (!HasDisabledUserValuesSecurity)
					return;

				m.ValLastvisit = ViewModelConversion.ToString(ValLastvisit);
				m.ValAverage = ViewModelConversion.ToNumeric(ValAverage);
				m.ValBuildage = ViewModelConversion.ToNumeric(ValBuildage);
				m.ValProfit = ViewModelConversion.ToNumeric(ValProfit);
				m.ValTax = ViewModelConversion.ToNumeric(ValTax);
				m.ValCodprope = ViewModelConversion.ToString(ValCodprope);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error($"Map ViewModel (Property) to Model (Prope) - Error during mapping. All user values: {HasDisabledUserValuesSecurity}");
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
					case "prope.codagent":
						this.ValCodagent = ViewModelConversion.ToString(_value);
						break;
					case "prope.codcity":
						this.ValCodcity = ViewModelConversion.ToString(_value);
						break;
					case "prope.id":
						this.ValId = ViewModelConversion.ToNumeric(_value);
						break;
					case "prope.sold":
						this.ValSold = ViewModelConversion.ToLogic(_value);
						break;
					case "prope.dtsold":
						this.ValDtsold = ViewModelConversion.ToDateTime(_value);
						break;
					case "prope.price":
						this.ValPrice = ViewModelConversion.ToNumeric(_value);
						break;
					case "prope.typology":
						this.ValTypology = ViewModelConversion.ToNumeric(_value);
						break;
					case "prope.buildtyp":
						this.ValBuildtyp = ViewModelConversion.ToString(_value);
						break;
					case "prope.grdsize":
						this.ValGrdsize = ViewModelConversion.ToNumeric(_value);
						break;
					case "prope.floornr":
						this.ValFloornr = ViewModelConversion.ToNumeric(_value);
						break;
					case "prope.size":
						this.ValSize = ViewModelConversion.ToNumeric(_value);
						break;
					case "prope.bathnr":
						this.ValBathnr = ViewModelConversion.ToNumeric(_value);
						break;
					case "prope.dtconst":
						this.ValDtconst = ViewModelConversion.ToDateTime(_value);
						break;
					case "prope.title":
						this.ValTitle = ViewModelConversion.ToString(_value);
						break;
					case "prope.photo":
						this.ValPhoto = ViewModelConversion.ToImage(_value);
						break;
					case "prope.descript":
						this.ValDescript = ViewModelConversion.ToString(_value);
						break;
					default:
						Log.Error($"SetViewModelValue (Property) - Unexpected field identifier {fullFieldName}");
						break;
				}
			}
			catch (Exception ex)
			{
				throw new FrameworkException(Resources.Resources.PEDIMOS_DESCULPA__OC63848, "SetViewModelValue (Property)", "Unexpected error", ex);
			}
		}

		#endregion

		/// <summary>
		/// Reads the Model from the database based on the key that is in the history or that was passed through the parameter
		/// </summary>
		/// <param name="id">The primary key of the record that needs to be read from the database. Leave NULL to use the value from the History.</param>
		public override void LoadModel(string id = null)
		{
			try { Model = Models.Prope.Find(id ?? Navigation.GetStrValue("prope"), m_userContext, "FPROPERTY"); }
			finally { Model ??= new Models.Prope(m_userContext) { Identifier = "FPROPERTY" }; }

			base.LoadModel();
		}

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;
			CSGenio.business.Area oldvalues = null;

			// TODO: Deve ser substituido por search do CSGenioA
			try
			{
				Model = Models.Prope.Find(Navigation.GetStrValue("prope"), m_userContext, "FPROPERTY");
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

			Model.Identifier = "FPROPERTY";
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

		protected override void LoadDocumentsProperties(Models.Prope row)
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
				Model = Models.Prope.Find(Navigation.GetStrValue("prope"), m_userContext, "FPROPERTY");
				if (Model == null)
				{
					Model = new Models.Prope(m_userContext) { Identifier = "FPROPERTY" };
					Model.klass.QPrimaryKey = Navigation.GetStrValue("prope");
				}
				MapToModel(Model);
				LoadDocumentsProperties(Model);
			}
			// Add characteristics
			Characs = new List<string>();

			Load_Propertycity_city____(qs, lazyLoad);
			Load_Propertyagentname____(qs, lazyLoad);

// USE /[MANUAL FOR VIEWMODEL_LOADPARTIAL PROPERTY]/
		}

// USE /[MANUAL FOR VIEWMODEL_NEW PROPERTY]/

		// Preencher Qvalues default dos fields do form
		protected override void LoadDefaultValues()
		{
		}

		public override CrudViewModelValidationResult Validate()
		{
			CrudViewModelFieldValidator validator = new(m_userContext.User.Language);

			validator.StringLength("ValLastvisit", Resources.Resources.LAST_VISIT61343, ValLastvisit, 50);
			validator.StringLength("CityCountValCountry", Resources.Resources.COUNTRY64133, CityCountValCountry, 50);

			validator.Required("ValPrice", Resources.Resources.PRICE06900, ViewModelConversion.ToNumeric(ValPrice), FieldType.CURRENCY.GetFormatting());
			validator.StringLength("AgentValEmail", Resources.Resources.E_MAIL42251, AgentValEmail, 80);
			validator.StringLength("ValTitle", Resources.Resources.TITLE21885, ValTitle, 50);

			validator.Required("ValTitle", Resources.Resources.TITLE21885, ViewModelConversion.ToString(ValTitle), FieldType.TEXT.GetFormatting());


			return validator.GetResult();
		}

		public override void Init(UserContext userContext)
		{
			base.Init(userContext);
		}
// USE /[MANUAL FOR VIEWMODEL_SAVE PROPERTY]/
		public override void Save()
		{


			base.Save();
		}

// USE /[MANUAL FOR VIEWMODEL_APPLY PROPERTY]/

// USE /[MANUAL FOR VIEWMODEL_DUPLICATE PROPERTY]/

// USE /[MANUAL FOR VIEWMODEL_DESTROY PROPERTY]/
		public override void Destroy(string id)
		{
			Model = Models.Prope.Find(id, m_userContext, "FPROPERTY");
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
		public void Load_Propertycity_city____(NameValueCollection qs, bool lazyLoad = false)
		{
			bool propertycity_city____DoLoad = true;
			CriteriaSet propertycity_city____Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("city", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					propertycity_city____Conds.Equal(CSGenioAcity.FldCodcity, hValue);
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
				FillDependant_PropertyTableCityCity(lazyLoad);
				return;
			}

			if (propertycity_city____DoLoad)
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
				propertycity_city____Conds.SubSet(search_filters);

				string tryParsePage = qs["pTableCityCity"] != null ? qs["pTableCityCity"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = [CSGenioAcity.FldCodcity, CSGenioAcity.FldCity, CSGenioAcity.FldZzstate];

// USE /[MANUAL FOR OVERRQ PROPERTY_CITYCITY]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("city", FormMode.New) || Navigation.checkFormMode("city", FormMode.Duplicate))
					propertycity_city____Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioAcity.FldZzstate, 0)
						.Equal(CSGenioAcity.FldCodcity, Navigation.GetStrValue("city")));
				else
					propertycity_city____Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioAcity.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("city", "city");
				ListingMVC<CSGenioAcity> listing = Models.ModelBase.Where<CSGenioAcity>(m_userContext, false, propertycity_city____Conds, fields, offset, numberItems, sorts, "LED_PROPERTYCITY_CITY____", true, false, firstVisibleColumn: firstVisibleColumn);

				TableCityCity.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TableCityCity.Query = query;
				TableCityCity.Elements = listing.RowsForViewModel((r) => new GenioMVC.Models.City(m_userContext, r, true, _fieldsToSerialize_PROPERTYCITY_CITY____));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_city") != null)
				{
					this.ValCodcity = Navigation.GetStrValue("RETURN_city");
					Navigation.CurrentLevel.SetEntry("RETURN_city", null);
				}

				TableCityCity.List = new SelectList(TableCityCity.Elements.ToSelectList(x => x.ValCity, x => x.ValCodcity,  x => x.ValCodcity == this.ValCodcity), "Value", "Text", this.ValCodcity);
				FillDependant_PropertyTableCityCity();
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TableCityCity (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of City</param>
		public ConcurrentDictionary<string, object> GetDependant_PropertyTableCityCity(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioAcity.FldCodcity, CSGenioAcity.FldCity, CSGenioAcount.FldCodcount, CSGenioAcount.FldCountry];

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
		public void FillDependant_PropertyTableCityCity(bool lazyLoad = false)
		{
			var row = GetDependant_PropertyTableCityCity(this.ValCodcity);
			try
			{
				this.funcCityCountValCountry = () => (string)row["count.country"];

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

		private readonly string[] _fieldsToSerialize_PROPERTYCITY_CITY____ = ["City", "City.ValCodcity", "City.ValZzstate", "City.ValCity"];

		/// <summary>
		/// TableAgentName -> (DB)
		/// </summary>
		/// <param name="qs"></param>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void Load_Propertyagentname____(NameValueCollection qs, bool lazyLoad = false)
		{
			bool propertyagentname____DoLoad = true;
			CriteriaSet propertyagentname____Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("agent", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					propertyagentname____Conds.Equal(CSGenioAagent.FldCodagent, hValue);
					this.ValCodagent = DBConversion.ToString(hValue);
				}
			}

			TableAgentName = new TableDBEdit<Models.Agent>();

			if (lazyLoad)
			{
				if (Navigation.CurrentLevel.GetEntry("RETURN_agent") != null)
				{
					this.ValCodagent = Navigation.GetStrValue("RETURN_agent");
					Navigation.CurrentLevel.SetEntry("RETURN_agent", null);
				}
				FillDependant_PropertyTableAgentName(lazyLoad);
				return;
			}

			if (propertyagentname____DoLoad)
			{
				List<ColumnSort> sorts = [];
				ColumnSort requestedSort = GetRequestSort(TableAgentName, "sTableAgentName", "dTableAgentName", qs, "agent");
				if (requestedSort != null)
					sorts.Add(requestedSort);
				sorts.Add(new ColumnSort(new ColumnReference(CSGenioAagent.FldName), SortOrder.Ascending));

				string query = "";
				if (!string.IsNullOrEmpty(qs["TableAgentName_tableFilters"]))
					TableAgentName.TableFilters = bool.Parse(qs["TableAgentName_tableFilters"]);
				else
					TableAgentName.TableFilters = false;

				query = qs["qTableAgentName"];

				//RS 26.07.2016 O preenchimento da lista de ajuda dos Dbedits passa a basear-se apenas no campo do próprio DbEdit
				// O interface de pesquisa rápida não fica coerente quando se visualiza apenas uma coluna mas a pesquisa faz matching com 5 ou 6 colunas diferentes
				//  tornando confuso to o user porque determinada row foi devolvida quando o Qresult não mostra como o matching foi feito
				CriteriaSet search_filters = CriteriaSet.And();
				if (!string.IsNullOrEmpty(query))
				{
					search_filters.Like(CSGenioAagent.FldName, query + "%");
				}
				propertyagentname____Conds.SubSet(search_filters);

				string tryParsePage = qs["pTableAgentName"] != null ? qs["pTableAgentName"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = [CSGenioAagent.FldCodagent, CSGenioAagent.FldName, CSGenioAagent.FldEmail, CSGenioAagent.FldZzstate];

// USE /[MANUAL FOR OVERRQ PROPERTY_AGENTNAME]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("agent", FormMode.New) || Navigation.checkFormMode("agent", FormMode.Duplicate))
					propertyagentname____Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioAagent.FldZzstate, 0)
						.Equal(CSGenioAagent.FldCodagent, Navigation.GetStrValue("agent")));
				else
					propertyagentname____Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioAagent.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("agent", "name");
				ListingMVC<CSGenioAagent> listing = Models.ModelBase.Where<CSGenioAagent>(m_userContext, false, propertyagentname____Conds, fields, offset, numberItems, sorts, "LED_PROPERTYAGENTNAME____", true, false, firstVisibleColumn: firstVisibleColumn);

				TableAgentName.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TableAgentName.Query = query;
				TableAgentName.Elements = listing.RowsForViewModel((r) => new GenioMVC.Models.Agent(m_userContext, r, true, _fieldsToSerialize_PROPERTYAGENTNAME____));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_agent") != null)
				{
					this.ValCodagent = Navigation.GetStrValue("RETURN_agent");
					Navigation.CurrentLevel.SetEntry("RETURN_agent", null);
				}

				TableAgentName.List = new SelectList(TableAgentName.Elements.ToSelectList(x => x.ValName, x => x.ValCodagent,  x => x.ValCodagent == this.ValCodagent), "Value", "Text", this.ValCodagent);
				FillDependant_PropertyTableAgentName();
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TableAgentName (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of Agent</param>
		public ConcurrentDictionary<string, object> GetDependant_PropertyTableAgentName(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioAagent.FldCodagent, CSGenioAagent.FldName, CSGenioAagent.FldPhotography, CSGenioAagent.FldEmail];

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

			CSGenioAagent tempArea = new(u);

			// Fields to select
			SelectQuery querySelect = new();
			querySelect.PageSize(1);
			foreach (FieldRef field in refDependantFields)
				querySelect.Select(field);

			querySelect.From(tempArea.QSystem, tempArea.TableName, tempArea.Alias)
				.Where(wherecodition.Equal(CSGenioAagent.FldCodagent, PKey));

			string[] dependantFields = refDependantFields.Select(f => f.FullName).ToArray();
			QueryUtils.SetInnerJoins(dependantFields, null, tempArea, querySelect);

			ArrayList values = sp.executeReaderOneRow(querySelect);
			bool useDefaults = values.Count == 0;

			if (useDefaults)
				return GetViewModelFieldValues(refDependantFields);
			return GetViewModelFieldValues(refDependantFields, values);
		}

		/// <summary>
		/// Fill Dependant fields values -> TableAgentName (DB)
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void FillDependant_PropertyTableAgentName(bool lazyLoad = false)
		{
			var row = GetDependant_PropertyTableAgentName(this.ValCodagent);
			try
			{
				this.funcAgentValPhotography = () => (GenioMVC.Models.ImageModel)row["agent.photography"];
				this.funcAgentValEmail = () => (string)row["agent.email"];

				// Fill List fields
				this.ValCodagent = ViewModelConversion.ToString(row["agent.codagent"]);
				TableAgentName.Value = (string)row["agent.name"];
				if (GenFunctions.emptyG(this.ValCodagent) == 1)
				{
					this.ValCodagent = "";
					TableAgentName.Value = "";
					Navigation.ClearValue("agent");
				}
				else if (lazyLoad)
				{
					TableAgentName.SetPagination(1, 0, false, false, 1);
					TableAgentName.List = new SelectList(new List<SelectListItem>()
					{
						new SelectListItem
						{
							Value = Convert.ToString(this.ValCodagent),
							Text = Convert.ToString(TableAgentName.Value),
							Selected = true
						}
					}, "Value", "Text", this.ValCodagent);
				}

				TableAgentName.Selected = this.ValCodagent;
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(string.Format("FillDependant_Error (TableAgentName): {0}; {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		private readonly string[] _fieldsToSerialize_PROPERTYAGENTNAME____ = ["Agent", "Agent.ValCodagent", "Agent.ValZzstate", "Agent.ValName", "Agent.ValEmail"];

		protected override object GetViewModelValue(string identifier, object modelValue)
		{
			return identifier switch
			{
				"prope.codagent" => ViewModelConversion.ToString(modelValue),
				"prope.codcity" => ViewModelConversion.ToString(modelValue),
				"prope.id" => ViewModelConversion.ToNumeric(modelValue),
				"prope.sold" => ViewModelConversion.ToLogic(modelValue),
				"prope.dtsold" => ViewModelConversion.ToDateTime(modelValue),
				"prope.lastvisit" => ViewModelConversion.ToString(modelValue),
				"prope.average" => ViewModelConversion.ToNumeric(modelValue),
				"count.country" => ViewModelConversion.ToString(modelValue),
				"prope.price" => ViewModelConversion.ToNumeric(modelValue),
				"prope.typology" => ViewModelConversion.ToNumeric(modelValue),
				"prope.buildtyp" => ViewModelConversion.ToString(modelValue),
				"prope.grdsize" => ViewModelConversion.ToNumeric(modelValue),
				"prope.floornr" => ViewModelConversion.ToNumeric(modelValue),
				"prope.size" => ViewModelConversion.ToNumeric(modelValue),
				"prope.bathnr" => ViewModelConversion.ToNumeric(modelValue),
				"prope.dtconst" => ViewModelConversion.ToDateTime(modelValue),
				"prope.buildage" => ViewModelConversion.ToNumeric(modelValue),
				"agent.photography" => ViewModelConversion.ToImage(modelValue),
				"agent.email" => ViewModelConversion.ToString(modelValue),
				"prope.profit" => ViewModelConversion.ToNumeric(modelValue),
				"prope.tax" => ViewModelConversion.ToNumeric(modelValue),
				"prope.title" => ViewModelConversion.ToString(modelValue),
				"prope.photo" => ViewModelConversion.ToImage(modelValue),
				"prope.descript" => ViewModelConversion.ToString(modelValue),
				"prope.codprope" => ViewModelConversion.ToString(modelValue),
				"city.codcity" => ViewModelConversion.ToString(modelValue),
				"city.city" => ViewModelConversion.ToString(modelValue),
				"count.codcount" => ViewModelConversion.ToString(modelValue),
				"agent.codagent" => ViewModelConversion.ToString(modelValue),
				"agent.name" => ViewModelConversion.ToString(modelValue),
				_ => modelValue
			};
		}

		/// <inheritdoc/>
		protected override void SetTicketToImageFields()
		{
			if (AgentValPhotography != null)
				AgentValPhotography.Ticket = Helpers.Helpers.GetFileTicket(m_userContext.User, CSGenio.business.Area.AreaAGENT, CSGenioAagent.FldPhotography.Field, null, ValCodagent);
			if (ValPhoto != null)
				ValPhoto.Ticket = Helpers.Helpers.GetFileTicket(m_userContext.User, CSGenio.business.Area.AreaPROPE, CSGenioAprope.FldPhoto.Field, null, ValCodprope);
		}

		#region Charts


		#endregion

		#region Custom code

// USE /[MANUAL FOR VIEWMODEL_CUSTOM PROPERTY]/

		#endregion
	}
}

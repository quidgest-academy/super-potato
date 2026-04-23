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

namespace GenioMVC.ViewModels.Conta
{
	public class Contact_ViewModel : FormViewModel<Models.Conta>, IPreparableForSerialization
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
		/// Title: "Title" | Type: "CE"
		/// </summary>
		public string ValCodprope { get; set; }

		#endregion

		/// <summary>
		/// Title: "Date" | Type: "D"
		/// </summary>
		public DateTime? ValDate { get; set; }
		/// <summary>
		/// Title: "Title" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.Prope> TablePropeTitle { get; set; }
		/// <summary>
		/// Title: "Client name" | Type: "C"
		/// </summary>
		public string ValClient { get; set; }
		/// <summary>
		/// Title: "Email do cliente" | Type: "C"
		/// </summary>
		public string ValEmail { get; set; }
		/// <summary>
		/// Title: "Phone number" | Type: "C"
		/// </summary>
		public string ValPhone { get; set; }
		/// <summary>
		/// Title: "Description" | Type: "MO"
		/// </summary>
		public string ValDescript { get; set; }
		/// <summary>
		/// Title: "Visit Date" | Type: "D"
		/// </summary>
		public DateTime? ValVisit_date { get; set; }
		/// <summary>
		/// Title: "Property" | Type: "+"
		/// </summary>
		[ValidateSetAccess]
		public string PropeValCodprope
		{
			get
			{
				return funcPropeValCodprope != null ? funcPropeValCodprope() : _auxPropeValCodprope;
			}
			set { funcPropeValCodprope = () => value; }
		}

		[JsonIgnore]
		public Func<string> funcPropeValCodprope { get; set; }

		private string _auxPropeValCodprope { get; set; }

		#region Navigations
		#endregion

		#region Auxiliar Keys for Image controls



		#endregion

		#region Extra database fields



		#endregion

		#region Fields for formulas


		#endregion

		public string ValCodconta { get; set; }


		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// A call to Init() needs to be manually invoked after this constructor
		/// </summary>
		[Obsolete("For deserialization only")]
		public Contact_ViewModel() : base(null!) { }

		public Contact_ViewModel(UserContext userContext, bool nestedForm = false) : base(userContext, "FCONTACT", nestedForm) { }

		public Contact_ViewModel(UserContext userContext, Models.Conta row, bool nestedForm = false) : base(userContext, "FCONTACT", row, nestedForm) { }

		public Contact_ViewModel(UserContext userContext, string id, bool nestedForm = false, string[]? fieldsToLoad = null) : this(userContext, nestedForm)
		{
			this.Navigation.SetValue("conta", id);
			Model = Models.Conta.Find(id, userContext, "FCONTACT", fieldsToQuery: fieldsToLoad);
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
			Models.Conta model = new Models.Conta(userContext) { Identifier = "FCONTACT" };

			var navigation = m_userContext.CurrentNavigation;
			// The "LoadKeysFromHistory" must be after the "LoadEPH" because the PHE's in the tree mark Foreign Keys to null
			// (since they cannot assign multiple values to a single field) and thus the value that comes from Navigation is lost.
			// And this makes it more like the order of loading the model when opening the form.
			model.LoadEPH("FCONTACT");
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
		public override void MapFromModel(Models.Conta m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map Model (Conta) to ViewModel (Contact) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				ValCodprope = ViewModelConversion.ToString(m.ValCodprope);
				ValDate = ViewModelConversion.ToDateTime(m.ValDate);
				ValClient = ViewModelConversion.ToString(m.ValClient);
				ValEmail = ViewModelConversion.ToString(m.ValEmail);
				ValPhone = ViewModelConversion.ToString(m.ValPhone);
				ValDescript = ViewModelConversion.ToString(m.ValDescript);
				ValVisit_date = ViewModelConversion.ToDateTime(m.ValVisit_date);
				funcPropeValCodprope = () => ViewModelConversion.ToString(m.Prope.ValCodprope);
				ValCodconta = ViewModelConversion.ToString(m.ValCodconta);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("Map Model (Conta) to ViewModel (Contact) - Error during mapping");
				throw;
			}
		}

		/// <inheritdoc />
		public override void MapToModel()
		{
			MapToModel(this.Model);
		}

		/// <inheritdoc />
		public override void MapToModel(Models.Conta m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map ViewModel (Contact) to Model (Conta) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				m.ValCodprope = ViewModelConversion.ToString(ValCodprope);
				m.ValDate = ViewModelConversion.ToDateTime(ValDate);
				m.ValClient = ViewModelConversion.ToString(ValClient);
				m.ValEmail = ViewModelConversion.ToString(ValEmail);
				m.ValPhone = ViewModelConversion.ToString(ValPhone);
				// Block When condition(s)
				if (HasDisabledUserValuesSecurity || !(Logical)((((string)m.ValPhone) == "")))
				{
					m.ValDescript = ViewModelConversion.ToString(ValDescript);
				}
				m.ValVisit_date = ViewModelConversion.ToDateTime(ValVisit_date);
				m.ValCodconta = ViewModelConversion.ToString(ValCodconta);

				/*
					At this moment, in the case of runtime calculation of server-side formulas, to improve performance and reduce database load,
						the values coming from the client-side will be accepted as valid, since they will not be saved and are only being used for calculation.
				*/
				if (!HasDisabledUserValuesSecurity)
					return;

			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error($"Map ViewModel (Contact) to Model (Conta) - Error during mapping. All user values: {HasDisabledUserValuesSecurity}");
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
					case "conta.codprope":
						this.ValCodprope = ViewModelConversion.ToString(_value);
						break;
					case "conta.date":
						this.ValDate = ViewModelConversion.ToDateTime(_value);
						break;
					case "conta.client":
						this.ValClient = ViewModelConversion.ToString(_value);
						break;
					case "conta.email":
						this.ValEmail = ViewModelConversion.ToString(_value);
						break;
					case "conta.phone":
						this.ValPhone = ViewModelConversion.ToString(_value);
						break;
					case "conta.descript":
						this.ValDescript = ViewModelConversion.ToString(_value);
						break;
					case "conta.visit_date":
						this.ValVisit_date = ViewModelConversion.ToDateTime(_value);
						break;
					case "conta.codconta":
						this.ValCodconta = ViewModelConversion.ToString(_value);
						break;
					default:
						Log.Error($"SetViewModelValue (Contact) - Unexpected field identifier {fullFieldName}");
						break;
				}
			}
			catch (Exception ex)
			{
				throw new FrameworkException(Resources.Resources.PEDIMOS_DESCULPA__OC63848, "SetViewModelValue (Contact)", "Unexpected error", ex);
			}
		}

		#endregion

		/// <summary>
		/// Reads the Model from the database based on the key that is in the history or that was passed through the parameter
		/// </summary>
		/// <param name="id">The primary key of the record that needs to be read from the database. Leave NULL to use the value from the History.</param>
		public override void LoadModel(string id = null)
		{
			try { Model = Models.Conta.Find(id ?? Navigation.GetStrValue("conta"), m_userContext, "FCONTACT"); }
			finally { Model ??= new Models.Conta(m_userContext) { Identifier = "FCONTACT" }; }

			base.LoadModel();
		}

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;
			CSGenio.business.Area oldvalues = null;

			// TODO: Deve ser substituido por search do CSGenioA
			try
			{
				Model = Models.Conta.Find(Navigation.GetStrValue("conta"), m_userContext, "FCONTACT");
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

			Model.Identifier = "FCONTACT";
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

		protected override void LoadDocumentsProperties(Models.Conta row)
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
				Model = Models.Conta.Find(Navigation.GetStrValue("conta"), m_userContext, "FCONTACT");
				if (Model == null)
				{
					Model = new Models.Conta(m_userContext) { Identifier = "FCONTACT" };
					Model.klass.QPrimaryKey = Navigation.GetStrValue("conta");
				}
				MapToModel(Model);
				LoadDocumentsProperties(Model);
			}
			// Add characteristics
			Characs = new List<string>();

			Load_Contact_propetitle___(qs, lazyLoad);

// USE /[MANUAL FOR VIEWMODEL_LOADPARTIAL CONTACT]/
		}

// USE /[MANUAL FOR VIEWMODEL_NEW CONTACT]/

		// Preencher Qvalues default dos fields do form
		protected override void LoadDefaultValues()
		{
		}

		public override CrudViewModelValidationResult Validate()
		{
			CrudViewModelFieldValidator validator = new(m_userContext.User.Language);

			validator.StringLength("ValClient", Resources.Resources.CLIENT_NAME39245, ValClient, 50);

			validator.Required("ValClient", Resources.Resources.CLIENT_NAME39245, ViewModelConversion.ToString(ValClient), FieldType.TEXT.GetFormatting());
			validator.StringLength("ValEmail", Resources.Resources.EMAIL_DO_CLIENTE30111, ValEmail, 80);

			validator.Required("ValEmail", Resources.Resources.EMAIL_DO_CLIENTE30111, ViewModelConversion.ToString(ValEmail), FieldType.TEXT.GetFormatting());
			validator.StringLength("ValPhone", Resources.Resources.PHONE_NUMBER20774, ValPhone, 14);

			validator.Required("ValVisit_date", Resources.Resources.VISIT_DATE27188, ViewModelConversion.ToDateTime(ValVisit_date), FieldType.DATE.GetFormatting());


			return validator.GetResult();
		}

		public override void Init(UserContext userContext)
		{
			base.Init(userContext);
		}
// USE /[MANUAL FOR VIEWMODEL_SAVE CONTACT]/
		public override void Save()
		{


			base.Save();
		}

// USE /[MANUAL FOR VIEWMODEL_APPLY CONTACT]/

// USE /[MANUAL FOR VIEWMODEL_DUPLICATE CONTACT]/

// USE /[MANUAL FOR VIEWMODEL_DESTROY CONTACT]/
		public override void Destroy(string id)
		{
			Model = Models.Conta.Find(id, m_userContext, "FCONTACT");
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
		/// TablePropeTitle -> (DB)
		/// </summary>
		/// <param name="qs"></param>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void Load_Contact_propetitle___(NameValueCollection qs, bool lazyLoad = false)
		{
			bool contact_propetitle___DoLoad = true;
			CriteriaSet contact_propetitle___Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("prope", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					contact_propetitle___Conds.Equal(CSGenioAprope.FldCodprope, hValue);
					this.ValCodprope = DBConversion.ToString(hValue);
				}
			}

			TablePropeTitle = new TableDBEdit<Models.Prope>();

			if (lazyLoad)
			{
				if (Navigation.CurrentLevel.GetEntry("RETURN_prope") != null)
				{
					this.ValCodprope = Navigation.GetStrValue("RETURN_prope");
					Navigation.CurrentLevel.SetEntry("RETURN_prope", null);
				}
				FillDependant_ContactTablePropeTitle(lazyLoad);
				return;
			}

			if (contact_propetitle___DoLoad)
			{
				List<ColumnSort> sorts = [];
				ColumnSort requestedSort = GetRequestSort(TablePropeTitle, "sTablePropeTitle", "dTablePropeTitle", qs, "prope");
				if (requestedSort != null)
					sorts.Add(requestedSort);
				sorts.Add(new ColumnSort(new ColumnReference(CSGenioAprope.FldTitle), SortOrder.Ascending));

				string query = "";
				if (!string.IsNullOrEmpty(qs["TablePropeTitle_tableFilters"]))
					TablePropeTitle.TableFilters = bool.Parse(qs["TablePropeTitle_tableFilters"]);
				else
					TablePropeTitle.TableFilters = false;

				query = qs["qTablePropeTitle"];

				//RS 26.07.2016 O preenchimento da lista de ajuda dos Dbedits passa a basear-se apenas no campo do próprio DbEdit
				// O interface de pesquisa rápida não fica coerente quando se visualiza apenas uma coluna mas a pesquisa faz matching com 5 ou 6 colunas diferentes
				//  tornando confuso to o user porque determinada row foi devolvida quando o Qresult não mostra como o matching foi feito
				CriteriaSet search_filters = CriteriaSet.And();
				if (!string.IsNullOrEmpty(query))
				{
					search_filters.Like(CSGenioAprope.FldTitle, query + "%");
				}
				contact_propetitle___Conds.SubSet(search_filters);

				string tryParsePage = qs["pTablePropeTitle"] != null ? qs["pTablePropeTitle"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = [CSGenioAprope.FldCodprope, CSGenioAprope.FldTitle, CSGenioAprope.FldPrice, CSGenioAprope.FldZzstate];

// USE /[MANUAL FOR OVERRQ CONTACT_PROPETITLE]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("prope", FormMode.New) || Navigation.checkFormMode("prope", FormMode.Duplicate))
					contact_propetitle___Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioAprope.FldZzstate, 0)
						.Equal(CSGenioAprope.FldCodprope, Navigation.GetStrValue("prope")));
				else
					contact_propetitle___Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioAprope.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("prope", "title");
				ListingMVC<CSGenioAprope> listing = Models.ModelBase.Where<CSGenioAprope>(m_userContext, false, contact_propetitle___Conds, fields, offset, numberItems, sorts, "LED_CONTACT_PROPETITLE___", true, false, firstVisibleColumn: firstVisibleColumn);

				TablePropeTitle.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TablePropeTitle.Query = query;
				TablePropeTitle.Elements = listing.RowsForViewModel((r) => new GenioMVC.Models.Prope(m_userContext, r, true, _fieldsToSerialize_CONTACT_PROPETITLE___));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_prope") != null)
				{
					this.ValCodprope = Navigation.GetStrValue("RETURN_prope");
					Navigation.CurrentLevel.SetEntry("RETURN_prope", null);
				}

				TablePropeTitle.List = new SelectList(TablePropeTitle.Elements.ToSelectList(x => x.ValTitle, x => x.ValCodprope,  x => x.ValCodprope == this.ValCodprope), "Value", "Text", this.ValCodprope);
				FillDependant_ContactTablePropeTitle();
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TablePropeTitle (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of Prope</param>
		public ConcurrentDictionary<string, object> GetDependant_ContactTablePropeTitle(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioAprope.FldCodprope, CSGenioAprope.FldTitle];

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

			CSGenioAprope tempArea = new(u);

			// Fields to select
			SelectQuery querySelect = new();
			querySelect.PageSize(1);
			foreach (FieldRef field in refDependantFields)
				querySelect.Select(field);

			querySelect.From(tempArea.QSystem, tempArea.TableName, tempArea.Alias)
				.Where(wherecodition.Equal(CSGenioAprope.FldCodprope, PKey));

			string[] dependantFields = refDependantFields.Select(f => f.FullName).ToArray();
			QueryUtils.SetInnerJoins(dependantFields, null, tempArea, querySelect);

			ArrayList values = sp.executeReaderOneRow(querySelect);
			bool useDefaults = values.Count == 0;

			if (useDefaults)
				return GetViewModelFieldValues(refDependantFields);
			return GetViewModelFieldValues(refDependantFields, values);
		}

		/// <summary>
		/// Fill Dependant fields values -> TablePropeTitle (DB)
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void FillDependant_ContactTablePropeTitle(bool lazyLoad = false)
		{
			var row = GetDependant_ContactTablePropeTitle(this.ValCodprope);
			try
			{
				this.funcPropeValCodprope = () => (string)row["prope.codprope"];

				// Fill List fields
				this.ValCodprope = ViewModelConversion.ToString(row["prope.codprope"]);
				TablePropeTitle.Value = (string)row["prope.title"];
				if (GenFunctions.emptyG(this.ValCodprope) == 1)
				{
					this.ValCodprope = "";
					TablePropeTitle.Value = "";
					Navigation.ClearValue("prope");
				}
				else if (lazyLoad)
				{
					TablePropeTitle.SetPagination(1, 0, false, false, 1);
					TablePropeTitle.List = new SelectList(new List<SelectListItem>()
					{
						new SelectListItem
						{
							Value = Convert.ToString(this.ValCodprope),
							Text = Convert.ToString(TablePropeTitle.Value),
							Selected = true
						}
					}, "Value", "Text", this.ValCodprope);
				}

				TablePropeTitle.Selected = this.ValCodprope;
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(string.Format("FillDependant_Error (TablePropeTitle): {0}; {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		private readonly string[] _fieldsToSerialize_CONTACT_PROPETITLE___ = ["Prope", "Prope.ValCodprope", "Prope.ValZzstate", "Prope.ValTitle", "Prope.ValPrice"];

		protected override object GetViewModelValue(string identifier, object modelValue)
		{
			return identifier switch
			{
				"conta.codprope" => ViewModelConversion.ToString(modelValue),
				"conta.date" => ViewModelConversion.ToDateTime(modelValue),
				"conta.client" => ViewModelConversion.ToString(modelValue),
				"conta.email" => ViewModelConversion.ToString(modelValue),
				"conta.phone" => ViewModelConversion.ToString(modelValue),
				"conta.descript" => ViewModelConversion.ToString(modelValue),
				"conta.visit_date" => ViewModelConversion.ToDateTime(modelValue),
				"prope.codprope" => ViewModelConversion.ToString(modelValue),
				"conta.codconta" => ViewModelConversion.ToString(modelValue),
				"prope.title" => ViewModelConversion.ToString(modelValue),
				_ => modelValue
			};
		}

		#region Charts


		#endregion

		#region Custom code

// USE /[MANUAL FOR VIEWMODEL_CUSTOM CONTACT]/

		#endregion
	}
}

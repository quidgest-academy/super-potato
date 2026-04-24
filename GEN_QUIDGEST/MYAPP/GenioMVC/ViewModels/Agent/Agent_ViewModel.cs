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

namespace GenioMVC.ViewModels.Agent
{
	public class Agent_ViewModel : FormViewModel<Models.Agent>, IPreparableForSerialization
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
		/// Title: "Country of residence" | Type: "CE"
		/// </summary>
		public string ValCodcaddr { get; set; }
		/// <summary>
		/// Title: "Country of birth" | Type: "CE"
		/// </summary>
		public string ValCborn { get; set; }

		#endregion

		/// <summary>
		/// Title: "Agent's name" | Type: "C"
		/// </summary>
		public string ValName { get; set; }
		/// <summary>
		/// Title: "Birthdate" | Type: "D"
		/// </summary>
		public DateTime? ValBirthdat { get; set; }
		/// <summary>
		/// Title: "Age" | Type: "N"
		/// </summary>
		[ValidateSetAccess]
		public decimal? ValAge { get; set; }
		/// <summary>
		/// Title: "E-mail" | Type: "C"
		/// </summary>
		public string ValEmail { get; set; }
		/// <summary>
		/// Title: "Telephone" | Type: "C"
		/// </summary>
		public string ValTelephon { get; set; }
		/// <summary>
		/// Title: "Country of birth" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.Cborn> TableCbornCountry { get; set; }
		/// <summary>
		/// Title: "Country of residence" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.Caddr> TableCaddrCountry { get; set; }
		/// <summary>
		/// Title: "Number of properties" | Type: "N"
		/// </summary>
		[ValidateSetAccess]
		public decimal? ValNrprops { get; set; }
		/// <summary>
		/// Title: "Profit" | Type: "$"
		/// </summary>
		[ValidateSetAccess]
		public decimal? ValProfit { get; set; }
		/// <summary>
		/// Title: "AveragePrice" | Type: "$"
		/// </summary>
		[ValidateSetAccess]
		public decimal? ValAverage_price { get; set; }
		/// <summary>
		/// Title: "Last property sold (price)" | Type: "$"
		/// </summary>
		[ValidateSetAccess]
		public decimal? ValLastprop { get; set; }
		/// <summary>
		/// Title: "Photography" | Type: "IJ"
		/// </summary>
		[ImageThumbnailJsonConverter(30, 50)]
		public GenioMVC.Models.ImageModel ValPhotography { get; set; }

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
		public string ValCodagent { get; set; }

		#endregion



		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// A call to Init() needs to be manually invoked after this constructor
		/// </summary>
		[Obsolete("For deserialization only")]
		public Agent_ViewModel() : base(null!) { }

		public Agent_ViewModel(UserContext userContext, bool nestedForm = false) : base(userContext, "FAGENT", nestedForm) { }

		public Agent_ViewModel(UserContext userContext, Models.Agent row, bool nestedForm = false) : base(userContext, "FAGENT", row, nestedForm) { }

		public Agent_ViewModel(UserContext userContext, string id, bool nestedForm = false, string[]? fieldsToLoad = null) : this(userContext, nestedForm)
		{
			this.Navigation.SetValue("agent", id);
			Model = Models.Agent.Find(id, userContext, "FAGENT", fieldsToQuery: fieldsToLoad);
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
			Models.Agent model = new Models.Agent(userContext) { Identifier = "FAGENT" };

			var navigation = m_userContext.CurrentNavigation;
			// The "LoadKeysFromHistory" must be after the "LoadEPH" because the PHE's in the tree mark Foreign Keys to null
			// (since they cannot assign multiple values to a single field) and thus the value that comes from Navigation is lost.
			// And this makes it more like the order of loading the model when opening the form.
			model.LoadEPH("FAGENT");
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
		public override void MapFromModel(Models.Agent m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map Model (Agent) to ViewModel (Agent) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				ValCodcaddr = ViewModelConversion.ToString(m.ValCodcaddr);
				ValCborn = ViewModelConversion.ToString(m.ValCborn);
				ValName = ViewModelConversion.ToString(m.ValName);
				ValBirthdat = ViewModelConversion.ToDateTime(m.ValBirthdat);
				ValAge = ViewModelConversion.ToNumeric(m.ValAge);
				ValEmail = ViewModelConversion.ToString(m.ValEmail);
				ValTelephon = ViewModelConversion.ToString(m.ValTelephon);
				ValNrprops = ViewModelConversion.ToNumeric(m.ValNrprops);
				ValProfit = ViewModelConversion.ToNumeric(m.ValProfit);
				ValAverage_price = ViewModelConversion.ToNumeric(m.ValAverage_price);
				ValLastprop = ViewModelConversion.ToNumeric(m.ValLastprop);
				ValPhotography = ViewModelConversion.ToImage(m.ValPhotography);
				ValCodagent = ViewModelConversion.ToString(m.ValCodagent);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("Map Model (Agent) to ViewModel (Agent) - Error during mapping");
				throw;
			}
		}

		/// <inheritdoc />
		public override void MapToModel()
		{
			MapToModel(this.Model);
		}

		/// <inheritdoc />
		public override void MapToModel(Models.Agent m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map ViewModel (Agent) to Model (Agent) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				m.ValCodcaddr = ViewModelConversion.ToString(ValCodcaddr);
				m.ValCborn = ViewModelConversion.ToString(ValCborn);
				m.ValName = ViewModelConversion.ToString(ValName);
				m.ValBirthdat = ViewModelConversion.ToDateTime(ValBirthdat);
				m.ValEmail = ViewModelConversion.ToString(ValEmail);
				m.ValTelephon = ViewModelConversion.ToString(ValTelephon);
				if (ValPhotography == null || !ValPhotography.IsThumbnail)
					m.ValPhotography = ViewModelConversion.ToImage(ValPhotography);

				/*
					At this moment, in the case of runtime calculation of server-side formulas, to improve performance and reduce database load,
						the values coming from the client-side will be accepted as valid, since they will not be saved and are only being used for calculation.
				*/
				if (!HasDisabledUserValuesSecurity)
					return;

				m.ValAge = ViewModelConversion.ToNumeric(ValAge);
				m.ValNrprops = ViewModelConversion.ToNumeric(ValNrprops);
				m.ValProfit = ViewModelConversion.ToNumeric(ValProfit);
				m.ValAverage_price = ViewModelConversion.ToNumeric(ValAverage_price);
				m.ValLastprop = ViewModelConversion.ToNumeric(ValLastprop);
				m.ValCodagent = ViewModelConversion.ToString(ValCodagent);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error($"Map ViewModel (Agent) to Model (Agent) - Error during mapping. All user values: {HasDisabledUserValuesSecurity}");
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
					case "agent.codcaddr":
						this.ValCodcaddr = ViewModelConversion.ToString(_value);
						break;
					case "agent.cborn":
						this.ValCborn = ViewModelConversion.ToString(_value);
						break;
					case "agent.name":
						this.ValName = ViewModelConversion.ToString(_value);
						break;
					case "agent.birthdat":
						this.ValBirthdat = ViewModelConversion.ToDateTime(_value);
						break;
					case "agent.email":
						this.ValEmail = ViewModelConversion.ToString(_value);
						break;
					case "agent.telephon":
						this.ValTelephon = ViewModelConversion.ToString(_value);
						break;
					case "agent.photography":
						this.ValPhotography = ViewModelConversion.ToImage(_value);
						break;
					default:
						Log.Error($"SetViewModelValue (Agent) - Unexpected field identifier {fullFieldName}");
						break;
				}
			}
			catch (Exception ex)
			{
				throw new FrameworkException(Resources.Resources.PEDIMOS_DESCULPA__OC63848, "SetViewModelValue (Agent)", "Unexpected error", ex);
			}
		}

		#endregion

		/// <summary>
		/// Reads the Model from the database based on the key that is in the history or that was passed through the parameter
		/// </summary>
		/// <param name="id">The primary key of the record that needs to be read from the database. Leave NULL to use the value from the History.</param>
		public override void LoadModel(string id = null)
		{
			try { Model = Models.Agent.Find(id ?? Navigation.GetStrValue("agent"), m_userContext, "FAGENT"); }
			finally { Model ??= new Models.Agent(m_userContext) { Identifier = "FAGENT" }; }

			base.LoadModel();
		}

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;
			CSGenio.business.Area oldvalues = null;

			// TODO: Deve ser substituido por search do CSGenioA
			try
			{
				Model = Models.Agent.Find(Navigation.GetStrValue("agent"), m_userContext, "FAGENT");
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

			Model.Identifier = "FAGENT";
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

		protected override void LoadDocumentsProperties(Models.Agent row)
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
				Model = Models.Agent.Find(Navigation.GetStrValue("agent"), m_userContext, "FAGENT");
				if (Model == null)
				{
					Model = new Models.Agent(m_userContext) { Identifier = "FAGENT" };
					Model.klass.QPrimaryKey = Navigation.GetStrValue("agent");
				}
				MapToModel(Model);
				LoadDocumentsProperties(Model);
			}
			// Add characteristics
			Characs = new List<string>();

			Load_Agent___cborncountry_(qs, lazyLoad);
			Load_Agent___caddrcountry_(qs, lazyLoad);

// USE /[MANUAL FOR VIEWMODEL_LOADPARTIAL AGENT]/
		}

// USE /[MANUAL FOR VIEWMODEL_NEW AGENT]/

		// Preencher Qvalues default dos fields do form
		protected override void LoadDefaultValues()
		{
		}

		public override CrudViewModelValidationResult Validate()
		{
			CrudViewModelFieldValidator validator = new(m_userContext.User.Language);

			validator.StringLength("ValName", Resources.Resources.AGENT_S_NAME42642, ValName, 50);

			validator.Required("ValName", Resources.Resources.AGENT_S_NAME42642, ViewModelConversion.ToString(ValName), FieldType.TEXT.GetFormatting());
			validator.StringLength("ValEmail", Resources.Resources.E_MAIL42251, ValEmail, 80);

			validator.Required("ValEmail", Resources.Resources.E_MAIL42251, ViewModelConversion.ToString(ValEmail), FieldType.TEXT.GetFormatting());
			validator.StringLength("ValTelephon", Resources.Resources.TELEPHONE28697, ValTelephon, 14);


			return validator.GetResult();
		}

		public override void Init(UserContext userContext)
		{
			base.Init(userContext);
		}
// USE /[MANUAL FOR VIEWMODEL_SAVE AGENT]/
		public override void Save()
		{


			base.Save();
		}

// USE /[MANUAL FOR VIEWMODEL_APPLY AGENT]/

// USE /[MANUAL FOR VIEWMODEL_DUPLICATE AGENT]/

// USE /[MANUAL FOR VIEWMODEL_DESTROY AGENT]/
		public override void Destroy(string id)
		{
			Model = Models.Agent.Find(id, m_userContext, "FAGENT");
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
		/// TableCbornCountry -> (DB)
		/// </summary>
		/// <param name="qs"></param>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void Load_Agent___cborncountry_(NameValueCollection qs, bool lazyLoad = false)
		{
			bool agent___cborncountry_DoLoad = true;
			CriteriaSet agent___cborncountry_Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("cborn", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					agent___cborncountry_Conds.Equal(CSGenioAcborn.FldCodcount, hValue);
					this.ValCborn = DBConversion.ToString(hValue);
				}
			}

			TableCbornCountry = new TableDBEdit<Models.Cborn>();

			if (lazyLoad)
			{
				if (Navigation.CurrentLevel.GetEntry("RETURN_cborn") != null)
				{
					this.ValCborn = Navigation.GetStrValue("RETURN_cborn");
					Navigation.CurrentLevel.SetEntry("RETURN_cborn", null);
				}
				FillDependant_AgentTableCbornCountry(lazyLoad);
				return;
			}

			if (agent___cborncountry_DoLoad)
			{
				List<ColumnSort> sorts = [];
				ColumnSort requestedSort = GetRequestSort(TableCbornCountry, "sTableCbornCountry", "dTableCbornCountry", qs, "cborn");
				if (requestedSort != null)
					sorts.Add(requestedSort);

				string query = "";
				if (!string.IsNullOrEmpty(qs["TableCbornCountry_tableFilters"]))
					TableCbornCountry.TableFilters = bool.Parse(qs["TableCbornCountry_tableFilters"]);
				else
					TableCbornCountry.TableFilters = false;

				query = qs["qTableCbornCountry"];

				//RS 26.07.2016 O preenchimento da lista de ajuda dos Dbedits passa a basear-se apenas no campo do próprio DbEdit
				// O interface de pesquisa rápida não fica coerente quando se visualiza apenas uma coluna mas a pesquisa faz matching com 5 ou 6 colunas diferentes
				//  tornando confuso to o user porque determinada row foi devolvida quando o Qresult não mostra como o matching foi feito
				CriteriaSet search_filters = CriteriaSet.And();
				if (!string.IsNullOrEmpty(query))
				{
					search_filters.Like(CSGenioAcborn.FldCountry, query + "%");
				}
				agent___cborncountry_Conds.SubSet(search_filters);

				string tryParsePage = qs["pTableCbornCountry"] != null ? qs["pTableCbornCountry"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = [CSGenioAcborn.FldCodcount, CSGenioAcborn.FldCountry, CSGenioAcborn.FldZzstate];

// USE /[MANUAL FOR OVERRQ AGENT_CBORNCOUNTRY]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("cborn", FormMode.New) || Navigation.checkFormMode("cborn", FormMode.Duplicate))
					agent___cborncountry_Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioAcborn.FldZzstate, 0)
						.Equal(CSGenioAcborn.FldCodcount, Navigation.GetStrValue("cborn")));
				else
					agent___cborncountry_Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioAcborn.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("cborn", "country");
				ListingMVC<CSGenioAcborn> listing = Models.ModelBase.Where<CSGenioAcborn>(m_userContext, false, agent___cborncountry_Conds, fields, offset, numberItems, sorts, "LED_AGENT___CBORNCOUNTRY_", true, false, firstVisibleColumn: firstVisibleColumn);

				TableCbornCountry.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TableCbornCountry.Query = query;
				TableCbornCountry.Elements = listing.RowsForViewModel((r) => new GenioMVC.Models.Cborn(m_userContext, r, true, _fieldsToSerialize_AGENT___CBORNCOUNTRY_));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_cborn") != null)
				{
					this.ValCborn = Navigation.GetStrValue("RETURN_cborn");
					Navigation.CurrentLevel.SetEntry("RETURN_cborn", null);
				}

				TableCbornCountry.List = new SelectList(TableCbornCountry.Elements.ToSelectList(x => x.ValCountry, x => x.ValCodcount,  x => x.ValCodcount == this.ValCborn), "Value", "Text", this.ValCborn);
				FillDependant_AgentTableCbornCountry();
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TableCbornCountry (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of Cborn</param>
		public ConcurrentDictionary<string, object> GetDependant_AgentTableCbornCountry(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioAcborn.FldCodcount, CSGenioAcborn.FldCountry];

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

			CSGenioAcborn tempArea = new(u);

			// Fields to select
			SelectQuery querySelect = new();
			querySelect.PageSize(1);
			foreach (FieldRef field in refDependantFields)
				querySelect.Select(field);

			querySelect.From(tempArea.QSystem, tempArea.TableName, tempArea.Alias)
				.Where(wherecodition.Equal(CSGenioAcborn.FldCodcount, PKey));

			string[] dependantFields = refDependantFields.Select(f => f.FullName).ToArray();
			QueryUtils.SetInnerJoins(dependantFields, null, tempArea, querySelect);

			ArrayList values = sp.executeReaderOneRow(querySelect);
			bool useDefaults = values.Count == 0;

			if (useDefaults)
				return GetViewModelFieldValues(refDependantFields);
			return GetViewModelFieldValues(refDependantFields, values);
		}

		/// <summary>
		/// Fill Dependant fields values -> TableCbornCountry (DB)
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void FillDependant_AgentTableCbornCountry(bool lazyLoad = false)
		{
			var row = GetDependant_AgentTableCbornCountry(this.ValCborn);
			try
			{

				// Fill List fields
				this.ValCborn = ViewModelConversion.ToString(row["cborn.codcount"]);
				TableCbornCountry.Value = (string)row["cborn.country"];
				if (GenFunctions.emptyG(this.ValCborn) == 1)
				{
					this.ValCborn = "";
					TableCbornCountry.Value = "";
					Navigation.ClearValue("cborn");
				}
				else if (lazyLoad)
				{
					TableCbornCountry.SetPagination(1, 0, false, false, 1);
					TableCbornCountry.List = new SelectList(new List<SelectListItem>()
					{
						new SelectListItem
						{
							Value = Convert.ToString(this.ValCborn),
							Text = Convert.ToString(TableCbornCountry.Value),
							Selected = true
						}
					}, "Value", "Text", this.ValCborn);
				}

				TableCbornCountry.Selected = this.ValCborn;
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(string.Format("FillDependant_Error (TableCbornCountry): {0}; {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		private readonly string[] _fieldsToSerialize_AGENT___CBORNCOUNTRY_ = ["Cborn", "Cborn.ValCodcount", "Cborn.ValZzstate", "Cborn.ValCountry"];

		/// <summary>
		/// TableCaddrCountry -> (DB)
		/// </summary>
		/// <param name="qs"></param>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void Load_Agent___caddrcountry_(NameValueCollection qs, bool lazyLoad = false)
		{
			bool agent___caddrcountry_DoLoad = true;
			CriteriaSet agent___caddrcountry_Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("caddr", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					agent___caddrcountry_Conds.Equal(CSGenioAcaddr.FldCodcount, hValue);
					this.ValCodcaddr = DBConversion.ToString(hValue);
				}
			}

			TableCaddrCountry = new TableDBEdit<Models.Caddr>();

			if (lazyLoad)
			{
				if (Navigation.CurrentLevel.GetEntry("RETURN_caddr") != null)
				{
					this.ValCodcaddr = Navigation.GetStrValue("RETURN_caddr");
					Navigation.CurrentLevel.SetEntry("RETURN_caddr", null);
				}
				FillDependant_AgentTableCaddrCountry(lazyLoad);
				return;
			}

			if (agent___caddrcountry_DoLoad)
			{
				List<ColumnSort> sorts = [];
				ColumnSort requestedSort = GetRequestSort(TableCaddrCountry, "sTableCaddrCountry", "dTableCaddrCountry", qs, "caddr");
				if (requestedSort != null)
					sorts.Add(requestedSort);

				string query = "";
				if (!string.IsNullOrEmpty(qs["TableCaddrCountry_tableFilters"]))
					TableCaddrCountry.TableFilters = bool.Parse(qs["TableCaddrCountry_tableFilters"]);
				else
					TableCaddrCountry.TableFilters = false;

				query = qs["qTableCaddrCountry"];

				//RS 26.07.2016 O preenchimento da lista de ajuda dos Dbedits passa a basear-se apenas no campo do próprio DbEdit
				// O interface de pesquisa rápida não fica coerente quando se visualiza apenas uma coluna mas a pesquisa faz matching com 5 ou 6 colunas diferentes
				//  tornando confuso to o user porque determinada row foi devolvida quando o Qresult não mostra como o matching foi feito
				CriteriaSet search_filters = CriteriaSet.And();
				if (!string.IsNullOrEmpty(query))
				{
					search_filters.Like(CSGenioAcaddr.FldCountry, query + "%");
				}
				agent___caddrcountry_Conds.SubSet(search_filters);

				string tryParsePage = qs["pTableCaddrCountry"] != null ? qs["pTableCaddrCountry"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = [CSGenioAcaddr.FldCodcount, CSGenioAcaddr.FldCountry, CSGenioAcaddr.FldZzstate];

// USE /[MANUAL FOR OVERRQ AGENT_CADDRCOUNTRY]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("caddr", FormMode.New) || Navigation.checkFormMode("caddr", FormMode.Duplicate))
					agent___caddrcountry_Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioAcaddr.FldZzstate, 0)
						.Equal(CSGenioAcaddr.FldCodcount, Navigation.GetStrValue("caddr")));
				else
					agent___caddrcountry_Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioAcaddr.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("caddr", "country");
				ListingMVC<CSGenioAcaddr> listing = Models.ModelBase.Where<CSGenioAcaddr>(m_userContext, false, agent___caddrcountry_Conds, fields, offset, numberItems, sorts, "LED_AGENT___CADDRCOUNTRY_", true, false, firstVisibleColumn: firstVisibleColumn);

				TableCaddrCountry.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TableCaddrCountry.Query = query;
				TableCaddrCountry.Elements = listing.RowsForViewModel((r) => new GenioMVC.Models.Caddr(m_userContext, r, true, _fieldsToSerialize_AGENT___CADDRCOUNTRY_));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_caddr") != null)
				{
					this.ValCodcaddr = Navigation.GetStrValue("RETURN_caddr");
					Navigation.CurrentLevel.SetEntry("RETURN_caddr", null);
				}

				TableCaddrCountry.List = new SelectList(TableCaddrCountry.Elements.ToSelectList(x => x.ValCountry, x => x.ValCodcount,  x => x.ValCodcount == this.ValCodcaddr), "Value", "Text", this.ValCodcaddr);
				FillDependant_AgentTableCaddrCountry();
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TableCaddrCountry (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of Caddr</param>
		public ConcurrentDictionary<string, object> GetDependant_AgentTableCaddrCountry(string PKey)
		{
			FieldRef[] refDependantFields = [CSGenioAcaddr.FldCodcount, CSGenioAcaddr.FldCountry];

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

			CSGenioAcaddr tempArea = new(u);

			// Fields to select
			SelectQuery querySelect = new();
			querySelect.PageSize(1);
			foreach (FieldRef field in refDependantFields)
				querySelect.Select(field);

			querySelect.From(tempArea.QSystem, tempArea.TableName, tempArea.Alias)
				.Where(wherecodition.Equal(CSGenioAcaddr.FldCodcount, PKey));

			string[] dependantFields = refDependantFields.Select(f => f.FullName).ToArray();
			QueryUtils.SetInnerJoins(dependantFields, null, tempArea, querySelect);

			ArrayList values = sp.executeReaderOneRow(querySelect);
			bool useDefaults = values.Count == 0;

			if (useDefaults)
				return GetViewModelFieldValues(refDependantFields);
			return GetViewModelFieldValues(refDependantFields, values);
		}

		/// <summary>
		/// Fill Dependant fields values -> TableCaddrCountry (DB)
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public void FillDependant_AgentTableCaddrCountry(bool lazyLoad = false)
		{
			var row = GetDependant_AgentTableCaddrCountry(this.ValCodcaddr);
			try
			{

				// Fill List fields
				this.ValCodcaddr = ViewModelConversion.ToString(row["caddr.codcount"]);
				TableCaddrCountry.Value = (string)row["caddr.country"];
				if (GenFunctions.emptyG(this.ValCodcaddr) == 1)
				{
					this.ValCodcaddr = "";
					TableCaddrCountry.Value = "";
					Navigation.ClearValue("caddr");
				}
				else if (lazyLoad)
				{
					TableCaddrCountry.SetPagination(1, 0, false, false, 1);
					TableCaddrCountry.List = new SelectList(new List<SelectListItem>()
					{
						new SelectListItem
						{
							Value = Convert.ToString(this.ValCodcaddr),
							Text = Convert.ToString(TableCaddrCountry.Value),
							Selected = true
						}
					}, "Value", "Text", this.ValCodcaddr);
				}

				TableCaddrCountry.Selected = this.ValCodcaddr;
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(string.Format("FillDependant_Error (TableCaddrCountry): {0}; {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : ""));
			}
		}

		private readonly string[] _fieldsToSerialize_AGENT___CADDRCOUNTRY_ = ["Caddr", "Caddr.ValCodcount", "Caddr.ValZzstate", "Caddr.ValCountry"];

		protected override object GetViewModelValue(string identifier, object modelValue)
		{
			return identifier switch
			{
				"agent.codcaddr" => ViewModelConversion.ToString(modelValue),
				"agent.cborn" => ViewModelConversion.ToString(modelValue),
				"agent.name" => ViewModelConversion.ToString(modelValue),
				"agent.birthdat" => ViewModelConversion.ToDateTime(modelValue),
				"agent.age" => ViewModelConversion.ToNumeric(modelValue),
				"agent.email" => ViewModelConversion.ToString(modelValue),
				"agent.telephon" => ViewModelConversion.ToString(modelValue),
				"agent.nrprops" => ViewModelConversion.ToNumeric(modelValue),
				"agent.profit" => ViewModelConversion.ToNumeric(modelValue),
				"agent.average_price" => ViewModelConversion.ToNumeric(modelValue),
				"agent.lastprop" => ViewModelConversion.ToNumeric(modelValue),
				"agent.photography" => ViewModelConversion.ToImage(modelValue),
				"agent.codagent" => ViewModelConversion.ToString(modelValue),
				"cborn.codcount" => ViewModelConversion.ToString(modelValue),
				"cborn.country" => ViewModelConversion.ToString(modelValue),
				"caddr.codcount" => ViewModelConversion.ToString(modelValue),
				"caddr.country" => ViewModelConversion.ToString(modelValue),
				_ => modelValue
			};
		}

		/// <inheritdoc/>
		protected override void SetTicketToImageFields()
		{
			if (ValPhotography != null)
				ValPhotography.Ticket = Helpers.Helpers.GetFileTicket(m_userContext.User, CSGenio.business.Area.AreaAGENT, CSGenioAagent.FldPhotography.Field, null, ValCodagent);
		}

		#region Charts


		#endregion

		#region Custom code

// USE /[MANUAL FOR VIEWMODEL_CUSTOM AGENT]/

		#endregion
	}
}

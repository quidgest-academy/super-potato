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

namespace GenioMVC.ViewModels.Photo
{
	public class Album_ViewModel : FormViewModel<Models.Photo>, IPreparableForSerialization
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
		/// Title: "Property" | Type: "CE"
		/// </summary>
		public string ValCodprope { get; set; }

		#endregion

		/// <summary>
		/// Title: "Photo" | Type: "IJ"
		/// </summary>
		[ImageThumbnailJsonConverter(30, 50)]
		public GenioMVC.Models.ImageModel ValPhoto { get; set; }
		/// <summary>
		/// Title: "Title" | Type: "C"
		/// </summary>
		public string ValTitle { get; set; }
		/// <summary>
		/// Title: "Property" | Type: "C"
		/// </summary>
		[ValidateSetAccess]
		public TableDBEdit<GenioMVC.Models.Prope> TablePropeTitle { get; set; }

		#region Navigations
		#endregion

		#region Auxiliar Keys for Image controls



		#endregion

		#region Extra database fields



		#endregion

		#region Fields for formulas


		#endregion

		public string ValCodphoto { get; set; }


		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// A call to Init() needs to be manually invoked after this constructor
		/// </summary>
		[Obsolete("For deserialization only")]
		public Album_ViewModel() : base(null!) { }

		public Album_ViewModel(UserContext userContext, bool nestedForm = false) : base(userContext, "FALBUM", nestedForm) { }

		public Album_ViewModel(UserContext userContext, Models.Photo row, bool nestedForm = false) : base(userContext, "FALBUM", row, nestedForm) { }

		public Album_ViewModel(UserContext userContext, string id, bool nestedForm = false, string[]? fieldsToLoad = null) : this(userContext, nestedForm)
		{
			this.Navigation.SetValue("photo", id);
			Model = Models.Photo.Find(id, userContext, "FALBUM", fieldsToQuery: fieldsToLoad);
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
			Models.Photo model = new Models.Photo(userContext) { Identifier = "FALBUM" };

			var navigation = m_userContext.CurrentNavigation;
			// The "LoadKeysFromHistory" must be after the "LoadEPH" because the PHE's in the tree mark Foreign Keys to null
			// (since they cannot assign multiple values to a single field) and thus the value that comes from Navigation is lost.
			// And this makes it more like the order of loading the model when opening the form.
			model.LoadEPH("FALBUM");
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
		public override void MapFromModel(Models.Photo m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map Model (Photo) to ViewModel (Album) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				ValCodprope = ViewModelConversion.ToString(m.ValCodprope);
				ValPhoto = ViewModelConversion.ToImage(m.ValPhoto);
				ValTitle = ViewModelConversion.ToString(m.ValTitle);
				ValCodphoto = ViewModelConversion.ToString(m.ValCodphoto);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("Map Model (Photo) to ViewModel (Album) - Error during mapping");
				throw;
			}
		}

		/// <inheritdoc />
		public override void MapToModel()
		{
			MapToModel(this.Model);
		}

		/// <inheritdoc />
		public override void MapToModel(Models.Photo m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map ViewModel (Album) to Model (Photo) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				m.ValCodprope = ViewModelConversion.ToString(ValCodprope);
				if (ValPhoto == null || !ValPhoto.IsThumbnail)
					m.ValPhoto = ViewModelConversion.ToImage(ValPhoto);
				m.ValTitle = ViewModelConversion.ToString(ValTitle);
				m.ValCodphoto = ViewModelConversion.ToString(ValCodphoto);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error($"Map ViewModel (Album) to Model (Photo) - Error during mapping. All user values: {HasDisabledUserValuesSecurity}");
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
					case "photo.codprope":
						this.ValCodprope = ViewModelConversion.ToString(_value);
						break;
					case "photo.photo":
						this.ValPhoto = ViewModelConversion.ToImage(_value);
						break;
					case "photo.title":
						this.ValTitle = ViewModelConversion.ToString(_value);
						break;
					case "photo.codphoto":
						this.ValCodphoto = ViewModelConversion.ToString(_value);
						break;
					default:
						Log.Error($"SetViewModelValue (Album) - Unexpected field identifier {fullFieldName}");
						break;
				}
			}
			catch (Exception ex)
			{
				throw new FrameworkException(Resources.Resources.PEDIMOS_DESCULPA__OC63848, "SetViewModelValue (Album)", "Unexpected error", ex);
			}
		}

		#endregion

		/// <summary>
		/// Reads the Model from the database based on the key that is in the history or that was passed through the parameter
		/// </summary>
		/// <param name="id">The primary key of the record that needs to be read from the database. Leave NULL to use the value from the History.</param>
		public override void LoadModel(string id = null)
		{
			try { Model = Models.Photo.Find(id ?? Navigation.GetStrValue("photo"), m_userContext, "FALBUM"); }
			finally { Model ??= new Models.Photo(m_userContext) { Identifier = "FALBUM" }; }

			base.LoadModel();
		}

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;
			CSGenio.business.Area oldvalues = null;

			// TODO: Deve ser substituido por search do CSGenioA
			try
			{
				Model = Models.Photo.Find(Navigation.GetStrValue("photo"), m_userContext, "FALBUM");
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

			Model.Identifier = "FALBUM";
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

		protected override void LoadDocumentsProperties(Models.Photo row)
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
				Model = Models.Photo.Find(Navigation.GetStrValue("photo"), m_userContext, "FALBUM");
				if (Model == null)
				{
					Model = new Models.Photo(m_userContext) { Identifier = "FALBUM" };
					Model.klass.QPrimaryKey = Navigation.GetStrValue("photo");
				}
				MapToModel(Model);
				LoadDocumentsProperties(Model);
			}
			// Add characteristics
			Characs = new List<string>();

			Load_Album___propetitle___(qs, lazyLoad);

// USE /[MANUAL FOR VIEWMODEL_LOADPARTIAL ALBUM]/
		}

// USE /[MANUAL FOR VIEWMODEL_NEW ALBUM]/

		// Preencher Qvalues default dos fields do form
		protected override void LoadDefaultValues()
		{
		}

		public override CrudViewModelValidationResult Validate()
		{
			CrudViewModelFieldValidator validator = new(m_userContext.User.Language);

			validator.StringLength("ValTitle", Resources.Resources.TITLE21885, ValTitle, 50);


			return validator.GetResult();
		}

		public override void Init(UserContext userContext)
		{
			base.Init(userContext);
		}
// USE /[MANUAL FOR VIEWMODEL_SAVE ALBUM]/
		public override void Save()
		{


			base.Save();
		}

// USE /[MANUAL FOR VIEWMODEL_APPLY ALBUM]/

// USE /[MANUAL FOR VIEWMODEL_DUPLICATE ALBUM]/

// USE /[MANUAL FOR VIEWMODEL_DESTROY ALBUM]/
		public override void Destroy(string id)
		{
			Model = Models.Photo.Find(id, m_userContext, "FALBUM");
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
		public void Load_Album___propetitle___(NameValueCollection qs, bool lazyLoad = false)
		{
			bool album___propetitle___DoLoad = true;
			CriteriaSet album___propetitle___Conds = CriteriaSet.And();
			{
				object hValue = Navigation.GetValue("prope", true);
				if (hValue != null && !(hValue is Array) && !string.IsNullOrEmpty(Convert.ToString(hValue)))
				{
					album___propetitle___Conds.Equal(CSGenioAprope.FldCodprope, hValue);
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
				FillDependant_AlbumTablePropeTitle(lazyLoad);
				return;
			}

			if (album___propetitle___DoLoad)
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
				album___propetitle___Conds.SubSet(search_filters);

				string tryParsePage = qs["pTablePropeTitle"] != null ? qs["pTablePropeTitle"].ToString() : "1";
				int page = !string.IsNullOrEmpty(tryParsePage) ? int.Parse(tryParsePage) : 1;
				int numberItems = CSGenio.framework.Configuration.NrRegDBedit;
				int offset = (page - 1) * numberItems;

				FieldRef[] fields = [CSGenioAprope.FldCodprope, CSGenioAprope.FldTitle, CSGenioAprope.FldPrice, CSGenioAprope.FldZzstate];

// USE /[MANUAL FOR OVERRQ ALBUM_PROPETITLE]/

				// Limitation by Zzstate
				/*
					Records that are currently being inserted or duplicated will also be included.
					Client-side persistence will try to fill the "text" value of that option.
				*/
				if (Navigation.checkFormMode("prope", FormMode.New) || Navigation.checkFormMode("prope", FormMode.Duplicate))
					album___propetitle___Conds.SubSet(CriteriaSet.Or()
						.Equal(CSGenioAprope.FldZzstate, 0)
						.Equal(CSGenioAprope.FldCodprope, Navigation.GetStrValue("prope")));
				else
					album___propetitle___Conds.Criterias.Add(new Criteria(new ColumnReference(CSGenioAprope.FldZzstate), CriteriaOperator.Equal, 0));

				FieldRef firstVisibleColumn = new FieldRef("prope", "title");
				ListingMVC<CSGenioAprope> listing = Models.ModelBase.Where<CSGenioAprope>(m_userContext, false, album___propetitle___Conds, fields, offset, numberItems, sorts, "LED_ALBUM___PROPETITLE___", true, false, firstVisibleColumn: firstVisibleColumn);

				TablePropeTitle.SetPagination(page, numberItems, listing.HasMore, listing.GetTotal, listing.TotalRecords);
				TablePropeTitle.Query = query;
				TablePropeTitle.Elements = listing.RowsForViewModel((r) => new GenioMVC.Models.Prope(m_userContext, r, true, _fieldsToSerialize_ALBUM___PROPETITLE___));

				//created by [ MH ] at [ 14.04.2016 ] - Foi alterada a forma de retornar a key do novo registo inserido / editado no form de apoio do DBEdit.
				//last update by [ MH ] at [ 10.05.2016 ] - Validação se key encontra-se no level atual, as chaves dos niveis anteriores devem ser ignorados.
				if (Navigation.CurrentLevel.GetEntry("RETURN_prope") != null)
				{
					this.ValCodprope = Navigation.GetStrValue("RETURN_prope");
					Navigation.CurrentLevel.SetEntry("RETURN_prope", null);
				}

				TablePropeTitle.List = new SelectList(TablePropeTitle.Elements.ToSelectList(x => x.ValTitle, x => x.ValCodprope,  x => x.ValCodprope == this.ValCodprope), "Value", "Text", this.ValCodprope);
				FillDependant_AlbumTablePropeTitle();
			}
		}

		/// <summary>
		/// Get Dependant fields values -> TablePropeTitle (DB)
		/// </summary>
		/// <param name="PKey">Primary Key of Prope</param>
		public ConcurrentDictionary<string, object> GetDependant_AlbumTablePropeTitle(string PKey)
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
		public void FillDependant_AlbumTablePropeTitle(bool lazyLoad = false)
		{
			var row = GetDependant_AlbumTablePropeTitle(this.ValCodprope);
			try
			{

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

		private readonly string[] _fieldsToSerialize_ALBUM___PROPETITLE___ = ["Prope", "Prope.ValCodprope", "Prope.ValZzstate", "Prope.ValTitle", "Prope.ValPrice"];

		protected override object GetViewModelValue(string identifier, object modelValue)
		{
			return identifier switch
			{
				"photo.codprope" => ViewModelConversion.ToString(modelValue),
				"photo.photo" => ViewModelConversion.ToImage(modelValue),
				"photo.title" => ViewModelConversion.ToString(modelValue),
				"photo.codphoto" => ViewModelConversion.ToString(modelValue),
				"prope.codprope" => ViewModelConversion.ToString(modelValue),
				"prope.title" => ViewModelConversion.ToString(modelValue),
				_ => modelValue
			};
		}

		/// <inheritdoc/>
		protected override void SetTicketToImageFields()
		{
			if (ValPhoto != null)
				ValPhoto.Ticket = Helpers.Helpers.GetFileTicket(m_userContext.User, CSGenio.business.Area.AreaPHOTO, CSGenioAphoto.FldPhoto.Field, null, ValCodphoto);
		}

		#region Charts


		#endregion

		#region Custom code

// USE /[MANUAL FOR VIEWMODEL_CUSTOM ALBUM]/

		#endregion
	}
}

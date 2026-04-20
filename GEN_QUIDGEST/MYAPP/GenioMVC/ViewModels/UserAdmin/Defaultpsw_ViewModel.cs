using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

using CSGenio.framework;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels.Psw
{
	public class Defaultpsw_ViewModel : FormViewModel<Models.Psw>
	{
		public override bool HasWriteConditions { get => false; }

		/// <summary>
		/// Reference for the Models MsqActive property
		/// </summary>
		[JsonIgnore]
		public bool MsqActive { get; set; } = false;

		/// <summary>
		/// Title: "Name" | Type: "C"
		/// </summary>
		public string ValNome { get; set; }

		/// <summary>
		/// Title: "Email" | Type: "C"
		/// </summary>
		public string ValEmail { get; set; }

		/// <summary>
		/// Title: "Password" | Type: "C"
		/// </summary>
		public string ValPassword { get; set; }

		/// <summary>
		/// Title: "Confirm Password" | Type: "C"
		/// </summary>
		public string ConfirmValPassword { get; set; }

		public string ValCodpsw { get; set; }

		#region ViewModel Pswnew (Password)

		/// <summary>
		/// FOR DESERIALIZATION ONLY
		/// A call to Init() needs to be manually invoked after this constructor
		/// </summary>
		[Obsolete("For deserialization only")]
		public Defaultpsw_ViewModel() : base(null!) { }

		public Defaultpsw_ViewModel(UserContext userContext) : base(userContext) { }

		public Defaultpsw_ViewModel(UserContext userContext, Models.Psw row, bool nestedForm = false) : base(userContext, null, nestedForm)
		{
			if (row == null)
				throw new ModelNotFoundException("Model not found");

			Model = row;
			InitModel(new NameValueCollection(), false, false);
		}

		public Defaultpsw_ViewModel(UserContext userContext, string id, bool nestedForm = false) : base(userContext, null, nestedForm)
		{
			this.Navigation.SetValue("psw", id);
			Model = Models.Psw.Find(id, userContext, "FPSWNEW");
			if (Model == null)
				throw new ModelNotFoundException("Model not found");
			InitModel(new NameValueCollection(), false, false);
		}

		protected override void InitLevels()
		{
			this.RoleToShow = CSGenio.framework.Role.AUTHORIZED;
			this.RoleToEdit = CSGenio.framework.Role.AUTHORIZED;
		}

		#region Mapper

		/// <inheritdoc />
		public override void MapFromModel(Models.Psw m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map Model (Psw) to ViewModel (Pswnew) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				ValNome = ViewModelConversion.ToString(m.ValNome);
				ValEmail = ViewModelConversion.ToString(m.ValEmail);
				ValCodpsw = ViewModelConversion.ToString(m.ValCodpsw);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error("Map Model (Psw) to ViewModel (Pswnew) - Error during mapping");
				throw;
			}
		}

		/// <inheritdoc />
		public override void MapToModel()
		{
			MapToModel(Model);
		}

		/// <inheritdoc />
		public override void MapToModel(Models.Psw m)
		{
			if (m == null)
			{
				CSGenio.framework.Log.Error("Map ViewModel (Pswnew) to Model (Psw) - Model is a null reference");
				throw new ModelNotFoundException("Model not found");
			}

			try
			{
				m.ValNome = ViewModelConversion.ToString(ValNome);
				m.ValPasswordDecrypted = ViewModelConversion.ToString(ValPassword);
				m.ValEmail = ViewModelConversion.ToString(ValEmail);
				m.ValCodpsw = ViewModelConversion.ToString(ValCodpsw);
			}
			catch (Exception)
			{
				CSGenio.framework.Log.Error($"Map ViewModel (Pswnew) to Model (Psw) - Error during mapping. All user values: {HasDisabledUserValuesSecurity}");
				throw;
			}
		}

		/// <summary>
		/// Sets the value of a single property of the view model based on the provided table and field names.
		/// </summary>
		/// <param name="fullFieldName">The full field name in the format "table.field".</param>
		/// <param name="value">The field value.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="fullFieldName"/> is null.</exception>
		public override void SetViewModelValue(string fullFieldName, object value)
		{
			try
			{
				ArgumentNullException.ThrowIfNull(fullFieldName);
				// Obtain a valid value from JsonValueKind that can come from "prefillValues" during the pre-filling of fields during insertion
				var _value = ViewModelConversion.ToRawValue(value);

				switch (fullFieldName)
				{
					case "psw.nome":
						this.ValNome = ViewModelConversion.ToString(_value);
						break;
					case "psw.email":
						this.ValEmail = ViewModelConversion.ToString(_value);
						break;
					case "psw.codpsw":
						this.ValCodpsw = ViewModelConversion.ToString(_value);
						break;
					default:
						CSGenio.framework.Log.Error($"SetViewModelValue (Psw) - Unexpected field identifier {fullFieldName}");
						break;
				}
			}
			catch (Exception ex)
			{
				throw new FrameworkException(Resources.Resources.PEDIMOS_DESCULPA__OC63848, "SetViewModelValue (Psw)", "Unexpected error", ex);
			}
		}

		#endregion

		public override void LoadModel(string id = null)
		{
			try { Model = Models.Psw.Find(id ?? Navigation.GetStrValue("psw"), m_userContext, "FPSWNEW"); }
			finally { Model ??= new Models.Psw(m_userContext) { Identifier = "FPSWNEW" }; }
		}

		public override void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false)
		{
			this.editable = editable;
			CSGenio.business.Area oldvalues = null;
			// TODO: Deve ser substituido por search do CSGenioA
			try
			{
				Model = Models.Psw.Find(Navigation.GetStrValue("psw"), m_userContext,"FPSWNEW");
			}
			finally
			{
				// TODO: Remove FormMode ?
				if ((Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate))
				{
					if (Model == null)
					{
						Model = new Models.Psw(m_userContext) { Identifier = "FPSWNEW" };
						Model.klass.QPrimaryKey = Navigation.GetStrValue("psw");
					}
				}
				else
				{
					if (Model == null)
						throw new ModelNotFoundException("Model not found");

					oldvalues = Model.klass;
				}
			}

			Model.Identifier = "FPSWNEW";
			InitModel(qs, lazyLoad, false);

			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Edit || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				// MH - Voltar calcular as formulas to "atualizar" os Qvalues dos fields fixos
				// Conexão deve estar aberta de fora. Podem haver formulas que utilizam funções "manuais".
				MapToModel(Model);
				// Preencher operações internas
				Model.klass.fillInternalOperations(m_userContext.PersistentSupport, oldvalues);
				MapFromModel(Model);
			}
		}

		/// <summary>
		/// Load Partial
		/// </summary>
		/// <param name="lazyLoad">Lazy loading of dropdown items</param>
		public override void LoadPartial(NameValueCollection qs, bool lazyLoad = false)
		{
			// MH [bugfix] - Quando o POST da ficha falha, ao recaregar a view os documentos na BD perdem alguma informação (ex: name do file)
			var codpsw = Navigation.GetStrValue("psw");
			if (!string.IsNullOrEmpty(codpsw))
			{
				// Precisamos fazer o Find to obter as chaves dos documentos que já foram anexados
				// TODO: Conseguir passar estas chaves no POST to poder retirar o Find.
				Model = Models.Psw.Find(codpsw, m_userContext, "FPSWNEW");
				if (Model == null)
				{
					Model = new Models.Psw(m_userContext) { Identifier = "FPSWNEW" };
					Model.klass.QPrimaryKey = codpsw;
				}
				MapToModel(Model);
			}
			//add characteristics
			Characs = new List<string>();
		}

		// Loads all the information needed to present the form in insert mode
		public override void NewLoad()
		{
			this.LoadPartial(new NameValueCollection());

			//after the interface contextual fill, we give a last chance for the row to update internal formulas
			if (Model == null) // To não perder o Qvalue do ZZState executa inicialização do Model só quando o objeto está vazio.
				Model = new Models.Psw(m_userContext) { Identifier = "FPSWNEW" };
			MapToModel(Model);
			// Preencher Qvalues default
			Model.klass.fillValuesDefault(m_userContext.PersistentSupport, FunctionType.INS);
			// Preencher Qvalues default dos fields do form
			// Preencher operações internas
			Model.klass.fillInternalOperations(m_userContext.PersistentSupport, null);
			MapFromModel(Model);
		}

		public override CrudViewModelValidationResult Validate()
		{
			CrudViewModelFieldValidator validator = new(m_userContext.User.Language);

			validator.Required("ValNome", Resources.Resources.UTILIZADOR52387, ValNome);
			validator.Required("ValPassword", Resources.Resources.PALAVRA_CHAVE39832, ValPassword);
			validator.Required("ValEmail", Resources.Resources.EMAIL25170, ValEmail);

			return validator.GetResult();
		}

		public override void Save()
		{
			MapToModel(Model);
			this.flashMessage = Model.Save();
		}

		public override void Destroy(string id)
		{
			Model = Models.Psw.Find(id, m_userContext, "FPSWNEW");
			if (Model == null)
				throw new ModelNotFoundException("Model not found");
			this.flashMessage = Model.Destroy();
		}

		#endregion

		#region Required methods - Empties

		protected override void LoadDefaultValues() { /* Method intentionally left empty. */ }

		public override StatusMessage EvaluateWriteConditions(bool isApply) => null;

		protected override void LoadDocumentsProperties(Models.Psw model) { /* Method intentionally left empty. */ }

		public override StatusMessage ViewConditions() => null;

		public override StatusMessage InsertConditions() => null;

		public override StatusMessage UpdateConditions() => null;

		public override StatusMessage DeleteConditions() => null;

		#endregion
	}
}

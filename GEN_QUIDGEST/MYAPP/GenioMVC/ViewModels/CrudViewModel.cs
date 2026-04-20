using System.Collections.Generic;
using System.Collections.Specialized;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

using CSGenio.business;
using CSGenio.framework;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels
{
	/// <summary>
	/// Represents an interface for a CRUD ViewModel.
	/// </summary>
	public interface ICrudViewModel : IViewModel
	{
		// Interface Properties

		/// <summary>
		/// Gets the primary key used for querying the ViewModel data.
		/// </summary>
		string QPrimaryKey { get; }

		/// <summary>
		/// Gets the navigation context, providing information about the current navigation state.
		/// </summary>
		NavigationContext Navigation { get; }

		/// <summary>
		/// Indicates whether the ViewModel has write conditions, determining if modifications are allowed.
		/// </summary>
		bool HasWriteConditions { get; }

		// Interface Methods

		/// <summary>
		/// Initializes the CRUD ViewModel with the provided user context.
		/// </summary>
		/// <param name="userContext">The user context for initializing the ViewModel.</param>
		void Init(UserContext userContext);

		/// <summary>
		/// Validates the state of the CRUD ViewModel, checking for any broken validation rules.
		/// </summary>
		/// <returns>A result object containing information about the validation status.</returns>
		CrudViewModelValidationResult Validate();

		/// <summary>
		/// Saves the changes made to the ViewModel.
		/// </summary>
		void Save();

		/// <summary>
		/// Applies any pending changes to the ViewModel.
		/// </summary>
		void Apply();

		/// <summary>
		/// Creates a duplicate of the ViewModel with the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the ViewModel to duplicate.</param>
		void Duplicate(string id);

		/// <summary>
		/// Destroys the ViewModel, removing it from the system.
		/// </summary>
		void Destroy();

		/// <summary>
		/// Destroys the ViewModel with the specified identifier, removing it from the system.
		/// </summary>
		/// <param name="id">The identifier of the ViewModel to destroy.</param>
		void Destroy(string id);

		/// <summary>
		/// Initializes the ViewModel for creating a new instance.
		/// </summary>
		void New();

		/// <summary>
		/// Loads data into the ViewModel.
		/// </summary>
		void Load();

		/// <summary>
		/// Loads data into the ViewModel based on the provided query string, editable status, and additional parameters.
		/// </summary>
		/// <param name="qs">The query string parameters for loading data.</param>
		/// <param name="editable">Specifies whether the ViewModel should be loaded in an editable state.</param>
		/// <param name="ajaxRequest">Indicates whether the request is an AJAX request.</param>
		/// <param name="lazyLoad">Specifies whether lazy loading should be applied.</param>
		void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false);

		/// <summary>
		/// Loads partial data into the ViewModel based on the provided query string and lazy loading option.
		/// </summary>
		/// <param name="qs">The query string parameters for loading data.</param>
		/// <param name="lazyLoad">Specifies whether lazy loading should be applied.</param>
		void LoadPartial(NameValueCollection qs, bool lazyLoad = false);

		/// <summary>
		/// Reads the Model from the database based on the key that is in the history or that was passed through the parameter.
		/// </summary>
		/// <param name="id">The primary key of the record that needs to be read from the database. Leave NULL to use the value from the History.</param>
		void LoadModel(string id = null);

		/// <summary>
		/// Executes the calculation of the Model’s internal formulas.
		/// </summary>
		void ExecuteModelFormulas();

		/// <summary>
		/// Initializes the ViewModel for creating a new instance and loads data.
		/// </summary>
		void NewLoad();

		/// <summary>
		/// Maps data from the underlying data model to the ViewModel.
		/// </summary>
		void MapFromModel();

		/// <summary>
		/// Performs the mapping of field values from the ViewModel to the Model.
		/// </summary>
		void MapToModel();

		/// <summary>
		/// Disable the protection that prevents mapping the fields from the ViewModel to the Model that could not be edited in this form.
		/// </summary>
		/// <param name="disabled">If TRUE, allows filling in the Model fields that could not be edited in this form.</param>
		/// <remarks>
		/// This should only be used in controlled cases; otherwise,
		/// it can lead to data security issues, allowing a form that could not edit a specific field to change its value.
		/// </remarks>
		void DisableUserValuesSecurity(bool disabled = true);

		/// <summary>
		/// Displays conditions relevant to the current view.
		/// </summary>
		/// <returns>A status message containing information about view conditions.</returns>
		StatusMessage ViewConditions();

		/// <summary>
		/// Displays conditions relevant to inserting data into the ViewModel.
		/// </summary>
		/// <returns>A status message containing information about insert conditions.</returns>
		StatusMessage InsertConditions();

		/// <summary>
		/// Displays conditions relevant to updating data in the ViewModel.
		/// </summary>
		/// <returns>A status message containing information about update conditions.</returns>
		StatusMessage UpdateConditions();

		/// <summary>
		/// Displays conditions relevant to deleting data from the ViewModel.
		/// </summary>
		/// <returns>A status message containing information about delete conditions.</returns>
		StatusMessage DeleteConditions();

		/// <summary>
		/// Evaluates the write conditions.
		/// </summary>
		/// <param name="isApply">Whether it's an apply</param>
		/// <returns>A status message containing information about the operation result.</returns>
		StatusMessage EvaluateWriteConditions(bool isApply);

		/// <summary>
		/// Validates the model fields.
		/// </summary>
		/// <param name="isApply">Whether it's an apply</param>
		/// <returns>A status message containing information about the operation result.</returns>
		StatusMessage Validate(bool isApply);

		/// <summary>
		/// Loads global data into the ViewModel based on the provided query string, editable status, and additional parameters.
		/// </summary>
		/// <param name="qs">The query string parameters for loading data.</param>
		/// <param name="editable">Specifies whether the ViewModel should be loaded in an editable state.</param>
		/// <param name="ajaxRequest">Indicates whether the request is an AJAX request.</param>
		void LoadGlob(NameValueCollection qs, bool editable, bool ajaxRequest = false);

		/// <summary>
		/// Sets the value of a single property of the view model based on the provided table and field names.
		/// </summary>
		/// <param name="fullFieldName">The full field name in the format "table.field".</param>
		/// <param name="value">The field value.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="fullFieldName"/> is null.</exception>
		void SetViewModelValue(string fullFieldName, object value);

		/// <summary>
		/// Populates the properties of the view model with values from the provided dictionary.
		/// </summary>
		/// <param name="values">A dictionary containing the keys in the format "table.field" and values to populate the view model with. Must not be null.</param>
		void PopulateViewModel(Dictionary<string, object> values);

		/// <summary>
		/// Indicates whether saving is permitted despite warnings being present.
		/// </summary>
		bool CanSaveWithWarnings { get; set; }

		/// <summary>
		/// Configures whether saving is allowed even when warnings are present.
		/// </summary>
		/// <param name="enabled">
		/// If set to <c>true</c>, the save operation will be permitted despite active warnings.
		/// If <c>false</c>, warnings will prevent saving.
		/// </param>
		/// <remarks>
		/// Use this method when the operation should proceed with non-critical issues
		/// that do not require user intervention or correction.
		/// </remarks>
		void AllowSavingWithWarnings(bool enabled);
	}

	public abstract class CrudViewModel<T> : ViewModelBase, ICrudViewModel where T : Models.ModelBase
	{
		/// <summary>
		/// The model
		/// </summary>
		protected T Model;

		/// <summary>
		/// The base CSGenioA class with the old (DB) values
		/// </summary>
		protected CSGenio.business.Area oldValues;

		/// <summary>
		/// Allocates a new empty ModelBase of the correct type
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public T CreateModelBase()
		{
			return Activator.CreateInstance(typeof(T), m_userContext, false, null) as T ?? throw new InvalidOperationException("Failed to create ModelBase of type " + typeof(T));
		}

		/// <summary>
		/// The model's queue list property
		/// </summary>
		[JsonIgnore]
		public List<string> GetQueueList
		{
			get
			{
				List<string> queueList = new List<string>();

				if (Model?.baseklass?.Information?.QueuesList != null)
					foreach (var item in Model.baseklass.Information.QueuesList)
						queueList.Add(item.Name);

				return queueList;
			}
		}

		/// <inheritdoc />
		public string QPrimaryKey { get => Model?.baseklass.QPrimaryKey; }

		/// <summary>
		/// Dictionary with custom properties to be sent to the client-side
		/// </summary>
		public IDictionary<string, object> ExtraProperties { get; private set; }

		protected CrudViewModel(UserContext userContext, string? identifier = null, bool nestedForm = false) : base(userContext)
		{
			ExtraProperties = new Dictionary<string, object>();
			InitLevels();
			// Fill the values that can already be filled (those that don't depend on the model).
			FillExtraProperties();
			Identifier = identifier;
			NestedForm = nestedForm;
		}

		protected CrudViewModel(UserContext userContext, string identifier, T row, bool nestedForm = false) : this(userContext, identifier, nestedForm)
		{
			Model = row ?? throw new ModelNotFoundException("Model not found");
			InitModel();
		}

		protected void InitModel(NameValueCollection qs = null, bool lazyLoad = false, bool loadDocuments = true)
		{
			if (Model == null)
				return;

			Model.LoadKeysFromHistory(this.Navigation, this.Navigation.CurrentLevel.Level);
			MapFromModel(Model);
			if (loadDocuments)
				LoadDocumentsProperties(Model);

			// Here we already have access to the model, so we can fill the remaining values.
			FillExtraProperties();
			LoadPartial(qs ?? [], lazyLoad);
		}

		/// <summary>
		/// Executes the calculation of the Model’s internal formulas.
		/// </summary>
		public void ExecuteModelFormulas()
		{
			Model?.baseklass.fillInternalOperations(m_userContext.PersistentSupport, oldValues);
		}

		/// <summary>
		/// Fills the ExtraProperties dictionary with any additional values that might be necessary
		/// </summary>
		protected virtual void FillExtraProperties() { /* Method intentionally left empty. */ }

		/// <inheritdoc />
		public void Load()
		{
			Load(new NameValueCollection(), false, false);
		}

		/// <inheritdoc />
		public void Destroy()
		{
			Destroy(QPrimaryKey);
		}

		/// <inheritdoc />
		public void MapFromModel()
		{
			MapFromModel(Model);
		}

		/// <summary>
		/// Reads the Model from the database based on the key that is in the history or that was passed through the parameter.
		/// </summary>
		/// <param name="id">The primary key of the record that needs to be read from the database. Leave NULL to use the value from the History.</param>
		/// <remarks>
		/// Each view model must implement its own model load.
		/// This virtual method only implements storing the old values for formula calculation and 'recalc if' conditions
		/// before merging with values received from the interface, requiring further implementation in subclasses.
		/// </remarks>
		public virtual void LoadModel(string id = null)
		{
			// Store the Old Values to calculate formulas and 'recalc if' before merge with values that come from the interface
			if (Model != null)
			{
				oldValues = CSGenio.business.Area.createArea(Model.baseklass.Alias, m_userContext.User, m_userContext.User.CurrentModule);
				foreach (RequestedField fld in Model.baseklass.Fields.Values)
					oldValues.insertNameValueField(fld.FullName, fld.Value);
			}
			else
				oldValues = null;
		}

		/// <inheritdoc />
		[JsonIgnore]
		public abstract bool HasWriteConditions { get; }

		[JsonIgnore]
		public bool editable { get; set; }

		[JsonIgnore]
		public List<string> Characs { get; set; }

		public abstract CrudViewModelValidationResult Validate();

		/// <inheritdoc />
		public abstract void Save();

		/// <inheritdoc />
		public abstract void Apply();

		/// <inheritdoc />
		public abstract void Duplicate(string id);

		/// <inheritdoc />
		public abstract void Destroy(string id);

		/// <inheritdoc />
		public abstract void New();

		/// <inheritdoc />
		public abstract void Load(NameValueCollection qs, bool editable, bool ajaxRequest = false, bool lazyLoad = false);

		/// <inheritdoc />
		public abstract void LoadPartial(NameValueCollection qs, bool lazyLoad = false);

		/// <inheritdoc />
		public abstract void NewLoad();

		/// <summary>
		/// Performs the mapping of field values from the Model to the ViewModel.
		/// </summary>
		/// <param name="model">The Model to be filled.</param>
		public abstract void MapFromModel(T model);

		/// <summary>
		/// Performs the mapping of field values from the ViewModel to the Model.
		/// </summary>
		/// <param name="model">The Model to be filled.</param>
		/// <exception cref="ModelNotFoundException">Thrown if <paramref name="model"/> is null.</exception>
		public abstract void MapToModel(T model);

		/// <inheritdoc />
		public abstract void MapToModel();

		/// <inheritdoc />
		public abstract void SetViewModelValue(string fullFieldName, object value);

		/// <inheritdoc />
		public abstract StatusMessage ViewConditions();

		/// <inheritdoc />
		public abstract StatusMessage InsertConditions();

		/// <inheritdoc />
		public abstract StatusMessage UpdateConditions();

		/// <inheritdoc />
		public abstract StatusMessage DeleteConditions();

		protected abstract void InitLevels();

		protected abstract void LoadDefaultValues();

		protected abstract void LoadDocumentsProperties(T model);

		/// <inheritdoc />
		public abstract StatusMessage EvaluateWriteConditions(bool isApply);

		/// <inheritdoc />
		public StatusMessage Validate(bool isApply)
		{
			return Validation.validateFieldsChange(Model.baseklass, m_userContext.PersistentSupport, m_userContext.User, isApply);
		}

		/// <inheritdoc />
		public virtual void LoadGlob(NameValueCollection qs, bool editable, bool ajaxRequest = false) { }

		/// <summary>
		/// Indicates whether the protection that prevents mapping the fields from the ViewModel to the Model that could not be edited in this form is disabled.
		/// </summary>
		[JsonIgnore]
		public bool HasDisabledUserValuesSecurity { get; private set; }

		/// <summary>
		/// Disable the protection that prevents mapping the fields from the ViewModel to the Model that could not be edited in this form.
		/// </summary>
		/// <param name="disabled">Allows filling in the Model fields that could not be edited in this form.</param>
		/// <remarks>
		/// This should only be used in controlled cases; otherwise,
		/// it can lead to data security issues, allowing a form that could not edit a specific field to change its value.
		/// </remarks>
		public void DisableUserValuesSecurity(bool disabled = true)
		{
			HasDisabledUserValuesSecurity = disabled;
		}

		/// <summary>
		/// Populates the properties of the view model with values from the provided dictionary.
		/// </summary>
		/// <param name="values">A dictionary containing the keys in the format "table.field" and values to populate the view model with. Must not be null.</param>
		public void PopulateViewModel(Dictionary<string, object> values)
		{
			foreach (var kvp in values ?? [])
				SetViewModelValue(kvp.Key, kvp.Value);
		}

		/// <summary>
		/// Indicates whether saving is permitted despite warnings being present.
		/// </summary>
		[ShouldSerialize("CanSaveWithWarnings")]
		public bool CanSaveWithWarnings { get; set; } = false;

		/// <summary>
		/// Configures whether saving is allowed even when warnings are present.
		/// </summary>
		/// <param name="enabled">
		/// If set to <c>true</c>, the save operation will be permitted despite active warnings.
		/// If <c>false</c>, warnings will prevent saving.
		/// </param>
		/// <remarks>
		/// Use this method when the operation should proceed with non-critical issues
		/// that do not require user intervention or correction.
		/// </remarks>
		public void AllowSavingWithWarnings(bool enabled)
		{
			CanSaveWithWarnings = enabled;
		}
	}
}

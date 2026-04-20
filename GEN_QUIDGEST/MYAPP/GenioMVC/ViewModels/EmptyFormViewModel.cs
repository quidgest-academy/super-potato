using System.Collections.Specialized;

using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels;

public abstract class EmptyFormViewModel : ViewModelBase
{
	/// <summary>
	/// Dictionary with custom properties to be sent to the client-side
	/// </summary>
	public IDictionary<string, object> ExtraProperties { get; private set; }

	public EmptyFormViewModel(UserContext userContext, bool nestedForm = false) : base(userContext)
	{
		ExtraProperties = new Dictionary<string, object>();
		InitLevels();
		// Fill the values that can already be filled (those that don't depend on the model).
		FillExtraProperties();
		NestedForm = nestedForm;
	}

	protected virtual void FillExtraProperties() { /* Method intentionally left empty. */ }

	protected abstract void InitLevels();

	/// <summary>
	/// Loads data into the ViewModel based on the provided query string.
	/// </summary>
	/// <param name="qs">The query string parameters for loading data.</param>
	public void Load(NameValueCollection qs)
	{
		Load(qs, false);
	}

	/// <summary>
	/// Loads data into the ViewModel based on the provided query string.
	/// </summary>
	/// <param name="qs">The query string parameters for loading data.</param>
	/// <param name="lazyLoad">Specifies whether lazy loading should be applied.</param>
	public void Load(NameValueCollection qs, bool lazyLoad)
	{
		LoadPartial(qs, lazyLoad);
	}

	/// <summary>
	/// Loads partial data into the ViewModel based on the provided query string and lazy loading option.
	/// </summary>
	/// <param name="qs">The query string parameters for loading data.</param>
	/// <param name="lazyLoad">Specifies whether lazy loading should be applied.</param>
	public abstract void LoadPartial(NameValueCollection qs, bool lazyLoad = false);

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
	/// Sets the value of a single property of the view model based on the provided table and field names.
	/// </summary>
	/// <param name="fullFieldName">The full field name in the format "table.field".</param>
	/// <param name="value">The field value.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="fullFieldName"/> is null.</exception>
	public abstract void SetViewModelValue(string fullFieldName, object value);
}

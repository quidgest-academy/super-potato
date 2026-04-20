using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels;

public abstract class MenuListViewModel<T> : ListViewModel
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MenuListViewModel" /> class.
	/// </summary>
	/// <param name="userContext">The current user request context</param>
	public MenuListViewModel(UserContext userContext) : base(userContext) {}

	/// <summary>
	/// Performs the mapping of field values from the Model to the ViewModel.
	/// </summary>
	/// <param name="model">The Model to be filled.</param>
	public abstract void MapFromModel(T model);

	/// <summary>
	/// Performs the mapping of field values from the ViewModel to the Model.
	/// </summary>
	/// <param name="model">The Model to be filled.</param>
	public abstract void MapToModel(T model);
}

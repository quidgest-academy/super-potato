using CSGenio.business;
using CSGenio.persistence;

namespace GenioMVC.ViewModels;

/// <summary>
/// Defines the base interface for a Kanban row ViewModel.
/// </summary>
/// <typeparam name="T">The type of the data model row, constrained to IArea.</typeparam>
public interface IKanbanRowBaseViewModel<T> where T : IArea
{
	/// <summary>
	/// Sets the data model row associated with this ViewModel.
	/// Used for dynamic type invocation in the base class.
	/// </summary>
	/// <param name="row">The data model row.</param>
	void SetRow(T row);
	/// <summary>
	/// Maps data from the associated model row to the ViewModel properties.
	/// </summary>
	void MapFromModel();

	/// <summary>
	/// Maps data from the provided model row to the ViewModel properties.
	/// </summary>
	/// <param name="row">The data model row.</param>
	void MapFromModel(T row);
}

/// <summary>
/// Abstract base class for Kanban row ViewModels.
/// Implements the IKanbanRowBaseViewModel interface.
/// </summary>
/// <typeparam name="T">The type of the data model row, constrained to DbArea.</typeparam>
public abstract class KanbanRowBaseViewModel<T> : IKanbanRowBaseViewModel<T> where T : DbArea
{
	/// <summary>
	/// The data model row associated with this ViewModel.
	/// </summary>
	protected T Row { get; private set; }

	/// <summary>
	/// Default constructor.
	/// </summary>
	public KanbanRowBaseViewModel() { }

	/// <summary>
	/// Constructor that initializes the ViewModel with a data model row.
	/// </summary>
	/// <param name="row">The data model row.</param>
	public KanbanRowBaseViewModel(T row)
	{
		Row = row;
	}


	/// <summary>
	/// Sets the data model row associated with this ViewModel.
	/// Allows dynamic type invocation in the base class.
	/// </summary>
	/// <param name="row">The data model row.</param>
	public void SetRow(T row)
	{
		Row = row;
	}

	/// <summary>
	/// Maps data from the associated model row to the ViewModel properties.
	/// </summary>
	public virtual void MapFromModel()
	{
		MapFromModel(Row);
	}

	/// <summary>
	/// Map data from the provided model row to the ViewModel properties.
	/// </summary>
	/// <param name="row">The data model row.</param>
	public abstract void MapFromModel(T row);
}
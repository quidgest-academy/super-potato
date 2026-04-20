using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System.Text.Json.Serialization;

namespace GenioMVC.ViewModels;

/// <summary>
/// Abstract base class for Kanban ViewModels.
/// Inherits from ViewModelBase and provides generic loading and mapping functionality.
/// Contains handler for events, such as Drag & Drop.
/// </summary>
/// <typeparam name="TColumn">The type of the column data model, constrained to DbArea.</typeparam>
/// <typeparam name="TCard">The type of the card data model, constrained to DbArea.</typeparam>
/// <typeparam name="TViewModelColumn">The type of the Kanban row ViewModel, which must implement IKanbanRowBaseViewModel.</typeparam>
/// <typeparam name="TViewModelCard">The type of the Kanban row ViewModel, which must implement IKanbanRowBaseViewModel.</typeparam>
public abstract class KanbanBaseViewModel<TColumn, TCard, TViewModelColumn, TViewModelCard>(UserContext userContext)  : ViewModelBase(userContext)
	where TColumn : DbArea 
	where TCard : DbArea 
	where TViewModelColumn : IKanbanRowBaseViewModel<TColumn>, new()
	where TViewModelCard : IKanbanRowBaseViewModel<TCard>, new()
{
	/// <summary>
	/// Gets the list of all Column FieldRefs necessary for the query.
	/// These fields are used to select the required data from the database.
	/// </summary>
	[JsonIgnore]
	public abstract FieldRef[] ColumnFields { get; }
	/// <summary>
	/// Gets the list of all Card FieldRefs necessary for the query.
	/// These fields are used to select the required data from the database.
	/// </summary>
	[JsonIgnore]
	public abstract FieldRef[] CardFields { get; }

	/// <summary>
	/// Cards table
	/// </summary>
	protected abstract AreaRef CardsArea { get; }
	/// <summary>
	/// he Foreign Key field in the Cards table linking to the Columns table.
	/// </summary>
	protected abstract FieldRef CardGroupIdField { get; }
	/// <summary>
	/// Column sorting field
	/// </summary>
	protected abstract FieldRef ColumnOrderField { get; }
	/// <summary>
	/// Card sorting field
	/// </summary>
	protected abstract FieldRef CardOrderField { get; }
	/// <summary>
	/// Indicates if the Kanban is editable, allowing Drag & Drop.
	/// </summary>
	protected virtual bool IsEditable { get; }
	/// <summary>
	/// Indicates if the column sorting field is reorderable. The numeric field must have the sorting option enabled.
	/// </summary>
	protected virtual bool CanReorderColumns { get; }
	/// <summary>
	/// Indicates if the cards sorting field is reorderable. The numeric field must have the sorting option enabled.
	/// </summary>
	protected virtual bool CanReorderCards { get; }

	/// <summary>
	/// Retrieves the Column query limits based on the current or provided navigation context.
	/// If no navigation context is provided, the current one is used.
	/// </summary>
	/// <param name="navigation">Optional navigation context to use for limits.</param>
	/// <returns>A CriteriaSet representing the query limits.</returns>
	public abstract CriteriaSet GetColumnLimits(NavigationContext navigation = null);

	/// <summary>
	/// Retrieves the Card query limits based on the current or provided navigation context.
	/// If no navigation context is provided, the current one is used.
	/// </summary>
	/// <param name="navigation">Optional navigation context to use for limits.</param>
	/// <returns>A CriteriaSet representing the query limits.</returns>
	public abstract CriteriaSet GetCardLimits(NavigationContext navigation = null);

	/// <summary>
	/// The list containing the Kanban columns.
	/// </summary>
	public List<TViewModelColumn> Columns { get; set; }

	/// <summary>
	/// The list containing the Kanban cards.
	/// </summary>
	public List<TViewModelCard> Cards { get; set; }


	/// <summary>
	/// Loads the data for the Kanban board in a generic way.
	/// </summary>
	public void Load()
	{
		// Get the query limits based on the navigation context
		var columnLimits = GetColumnLimits();
		if(columnLimits == null) // If some limit is missing, do not load anything
			return;

		// Perform the query to retrieve the columns data
		var listingColumns = Models.ModelBase.Where<TColumn>(
			m_userContext,
			false,
			args: columnLimits,
			fields: ColumnFields,
			-1,
			-1,
			[new ColumnSort(new ColumnReference(ColumnOrderField), SortOrder.Ascending)],
			Identifier
		);

		// Map the columns data to the ViewModel
		Columns = MapFromModel<TColumn, TViewModelColumn>(listingColumns);

		// Get the query limits based on the navigation context
		var cardLimits = GetCardLimits();
		if(cardLimits == null) // If some limit is missing, do not load anything
			return;

		// Perform the query to retrieve the columns data
		var listingCards = Models.ModelBase.Where<TCard>(
			m_userContext,
			false,
			args: cardLimits,
			fields: CardFields,
			-1,
			-1,
			[new ColumnSort(new ColumnReference(CardOrderField), SortOrder.Ascending)],
			Identifier
		);

		// Map the cards data to the ViewModel
		Cards = MapFromModel<TCard, TViewModelCard>(listingCards);
	}

	/// <summary>
	/// Maps the data from the model listing to a list of row ViewModels.
	/// </summary>
	/// <param name="listing">The listing of data model rows.</param>
	/// <returns>A list of Kanban row ViewModels.</returns>
	public List<TRow> MapFromModel<TModel, TRow>(ListingMVC<TModel> listing) 
		where TModel : DbArea
		where TRow : IKanbanRowBaseViewModel<TModel>, new()
	{
		var rows = new List<TRow>();
		listing.Rows?.ForEach(row => { 
			var rowViewModel = new TRow();
			rowViewModel.SetRow(row);
			rowViewModel.MapFromModel();
			rows.Add(rowViewModel);
		});

		return rows;
	}

	/// <summary>
	/// Handler for events, such as Drag & Drop.
	/// </summary>
	/// <param name="eventData">The event data</param>
	/// <returns>Response to be returned to the client-side</returns>
	public object EventHandler(Models.RequestKanbanModel eventData)
	{
		switch (eventData.EventType)
		{
			case Models.KanbanEventType.DragDrop:
				{
					switch (eventData.ElementType)
					{
						case Models.KanbanElementType.Column:
							{
								ReorderColumn(eventData.SourceKey, eventData.NewOrder);
							}
							break;
						case Models.KanbanElementType.Card:
							{
								MoveCard(eventData.SourceKey, eventData.DestinationKey, eventData.NewOrder);
							}
							break;
					}
				}
				break;
		}
		return new { };
	}

	/// <summary>
	/// If the column sorting field is reorderable, it will update the order of the Kanban columns in the database.
	/// </summary>
	/// <param name="id">The key of the column record being moved.</param>
	/// <param name="position">The new position for the record.</param>
	/// <exception cref="BusinessException">An error is emitted if reordering is not allowed or if reordering fails.</exception>
	private void ReorderColumn(string id, int position)
	{
		if(!IsEditable || !CanReorderColumns)
		{
			// TODO: Translate error message
			var errorMsg = $"Reordering of columns in the '{Identifier}' Kanban is not allowed.";
			throw new BusinessException(errorMsg, $"{Identifier} kanban - ReorderColumn", errorMsg);
		}

		var user = m_userContext.User;
		var sp = m_userContext.PersistentSupport;
		try
		{
			sp.openTransaction();
			var row = GetColumnRecord(sp, id, user);
			row.insertNameValueField(ColumnOrderField, position);
			row.update(sp);
		}
		catch (Exception ex)
		{
			sp.rollbackTransaction();
			var errorMsg = $"Error reordering columns in the '{Identifier}' Kanban.";
			throw new BusinessException(errorMsg, $"{Identifier} kanban - ReorderColumn", errorMsg, ex);
		}
		finally
		{
			sp.closeTransaction();
		}
	}

	/// <summary>
	/// Allows moving cards between columns and/or reordering them.
	/// If the card sorting field is reorderable, it will update the order of Kanban cards in both the source and destination columns in the database.
	/// </summary>
	/// <param name="id">The key of the card record being moved.</param>
	/// <param name="columnId">The key of the destination column.</param>
	/// <param name="position">The new position for the record.</param>
	/// <exception cref="BusinessException">An error is emitted if moving and/or reordering is not allowed or if the move and/or reordering fails.</exception>
	private void MoveCard(string id, string columnId, int position)
	{
		var user = m_userContext.User;
		var sp = m_userContext.PersistentSupport;
		try
		{
			sp.openTransaction();
			var row = GetCardRecord(sp, id, user);

            row.insertNameValueField(CardGroupIdField, columnId);
			if(CanReorderCards)
				row.insertNameValueField(CardOrderField, position);
            row.update(sp);
		}
		catch (Exception ex)
		{
			sp.rollbackTransaction();
			var errorMsg = $"Error moving and/or reordering cards in the '{Identifier}' Kanban.";
			throw new BusinessException(errorMsg, $"{Identifier} kanban - MoveCard", errorMsg, ex);
		}
		finally
		{
			sp.closeTransaction();
		}
	}

	/// <summary>
	/// Allows retrieving the column record from the database.
	/// </summary>
	/// <param name="sp">The instance of Persistent Support.</param>
	/// <param name="id">The key of the record to search for.</param>
	/// <param name="user">The instance of the user to be used for retrieving the record.</param>
	/// <returns>The requested record, or null if not found.</returns>
	/// <exception cref="NotImplementedException">If the order field is sortable, this method must be overridden to call the specific method generated in CSGenioA for this order field.</exception>
	protected virtual TColumn GetColumnRecord(PersistentSupport sp, string id, User user)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Allows retrieving the card record from the database.
	/// </summary>
	/// <param name="sp">The instance of Persistent Support.</param>
	/// <param name="id">The key of the record to search for.</param>
	/// <param name="user">The instance of the user to be used for retrieving the record.</param>
	/// <returns>The requested record, or null if not found.</returns>
	/// <exception cref="NotImplementedException">If the order field is sortable, this method must be overridden to call the specific method generated in CSGenioA for this order field.</exception>
	protected virtual TCard GetCardRecord(PersistentSupport sp, string id, User user)
	{
		throw new NotImplementedException();
	}
}
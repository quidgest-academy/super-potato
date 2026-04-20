using System.Text.Json.Serialization;

using CSGenio.business;
using CSGenio.framework;
using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels.Prope;

public class FOR_Menu_511_RowViewModel : Models.Prope
{
	#region Constructors

	public FOR_Menu_511_RowViewModel(UserContext userContext, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext, isEmpty, fieldsToSerialize)
	{
		InitRowProperties();
	}

	public FOR_Menu_511_RowViewModel(UserContext userContext, CSGenioAprope val, bool isEmpty = false, string[]? fieldsToSerialize = null) : base(userContext, val, isEmpty, fieldsToSerialize)
	{
		InitRowProperties();
	}

	#endregion

	#region Private methods

	private void InitRowProperties()
	{
		SetColumns();
		SetCustomActions();
	}

	private void SetColumns()
	{
		Columns ??= [
			new ListColumn()
			{
				Order = 1,
				Area = "PROPE",
				Field = "PHOTO",
			},
			new ListColumn()
			{
				Order = 2,
				Area = "AGENT",
				Field = "NAME",
			},
			new ListColumn()
			{
				Order = 3,
				Area = "PROPE",
				Field = "DESCRIPT",
			},
			new ListColumn()
			{
				Order = 4,
				Area = "CITY",
				Field = "CITY",
			},
			new ListColumn()
			{
				Order = 5,
				Area = "PROPE",
				Field = "SIZE",
			},
			new ListColumn()
			{
				Order = 6,
				Area = "PROPE",
				Field = "BATHNR",
			},
			new ListColumn()
			{
				Order = 7,
				Area = "PROPE",
				Field = "DTCONST",
			},
			new ListColumn()
			{
				Order = 8,
				Area = "PROPE",
				Field = "BUILDTYP",
			},
			new ListColumn()
			{
				Order = 9,
				Area = "PROPE",
				Field = "TYPOLOGY",
			},
		];
	}

	private void SetButtonPermissions()
	{
		if (BtnPermission != null)
			return;

		bool canView = true;
		bool canEdit = true;
		bool canDelete = true;
		bool canDuplicate = true;
		bool canInsert = true;

		using (new CSGenio.persistence.ScopedPersistentSupport(m_userContext.PersistentSupport))
		{

			// Table PROPE CRUD conditions.
		}

		BtnPermission = new TableRowCrudButtonPermissions()
		{
			ViewBtnDisabled = !canView,
			EditBtnDisabled = !canEdit,
			DeleteBtnDisabled = !canDelete,
			DuplicateBtnDisabled = !canDuplicate,
			InsertBtnDisabled = !canInsert
		};
	}

	private void SetCustomActions()
	{
		CustomActions ??= new()
		{
		};
	}

	#endregion

	/// <summary>
	/// The state of the row (it's an internal value, therefore it shouldn't be sent to the client-side)
	/// </summary>
	[JsonIgnore]
	public override int ValZzstate => base.ValZzstate;

	/// <summary>
	/// Whether the row is in a valid state
	/// </summary>
	[JsonPropertyName("isValid")]
	public bool IsValid => ValZzstate == 0;

	/// <summary>
	/// The list columns
	/// </summary>
	[JsonPropertyName("columns")]
	public List<ListColumn> Columns { get; private set; }

	/// <summary>
	/// The button permissions
	/// </summary>
	[JsonPropertyName("btnPermission")]
	public TableRowCrudButtonPermissions BtnPermission { get; private set; }

	/// <summary>
	/// The custom action buttons
	/// </summary>
	[JsonPropertyName("customActions")]
	public Dictionary<string, ListCustomAction> CustomActions { get; private set; }

	/// <summary>
	/// The foreground color
	/// </summary>
	[JsonPropertyName("foregroundColor")]
	public string ForegroundColor => "";

	/// <summary>
	/// The background color
	/// Formula: iif([PROPE->PRICE]>=100000, HEXCOLOUR("4deb94"), iif([PROPE->PRICE]>=50000 && [PROPE->PRICE]<100000, HEXCOLOUR("f7c65c"), HEXCOLOUR("b5b5b5")))
	/// </summary>
	[JsonPropertyName("backgroundColor")]
	public string BackgroundColor => ((((decimal)this.ValPrice)>=100000)?("#"+"4deb94"):(((((decimal)this.ValPrice)>=50000&&((decimal)this.ValPrice)<100000)?("#"+"f7c65c"):("#"+"b5b5b5"))));

	/// <summary>
	/// Runs init logic that depends on row data.
	/// </summary>
	public void InitRowData()
	{
		SetButtonPermissions();
	}
}

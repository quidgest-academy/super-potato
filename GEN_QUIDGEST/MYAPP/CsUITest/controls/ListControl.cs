using System.Collections.Generic;
using System.Linq;

namespace quidgest.uitests.controls;

public class ListControl : ControlObject
{
    /// <summary>
    /// Table ID
    /// </summary>
    private string id => m_control.GetAttribute("id");

    /// <summary>
    /// Row elements
    /// </summary>
    private IList<IWebElement> rows => m_control.FindElements(By.CssSelector("tbody tr:not(.c-table__row--empty)"));

    /// <summary>
    /// Column elements
    /// </summary>
    private IList<IWebElement> columns => m_control.FindElements(By.CssSelector("thead th"));

    /// <summary>
    /// Insert button
    /// </summary>
    private IWebElement insertBtn => GetElement(m_control, By.CssSelector("[data-key='insert']"));

    /// <summary>
    /// Loading state
    /// </summary>
    private bool loading => m_control.GetAttribute("data-loading") == "true";

    /// <summary>
    /// Search bar
    /// </summary>
    public SearchControl Search => new SearchControl(driver, m_containerLocator, m_controlLocator);

    /// <summary>
    /// Table configuration menu button and items
    /// </summary>
    private IWebElement configBtn => GetElement(m_control, ByData.Testid("table-config-details"));

    /// <summary>
    /// Column config button
    /// </summary>
    private IWebElement columnConfigBtn => GetElement(driver, By.CssSelector("li[data-key='columns']"));

    /// <summary>
    /// Column config button
    /// </summary>
    private IWebElement filtersBtn => GetElement(driver, By.CssSelector("li[data-key='filters']"));

    /// <summary>
    /// Table row reorder mode toggle button
    /// </summary>
    private IWebElement rowReorderBtn => GetElement(m_control, By.CssSelector("button[id$='row-reorder-btn']"));

    /// <summary>
    /// Total record counter
    /// </summary>
    private IWebElement recordCounter => GetElement(m_control, By.CssSelector(".e-counter__text"));

    /// <summary>
    /// Gets a value indicating whether the user can insert in this table.
    /// </summary>
    public bool CanInsert => insertBtn.Enabled;

    /// <summary>
    /// Row count
    /// </summary>
    public int RowCount
    {
        get
        {
            WaitForLoading();
            return rows.Count;
        }
    }

    public int TotalRecordCount
    {
        get
        {
            WaitForLoading();
            if (recordCounter == null)
                throw new InvalidOperationException($"List {id} does not have a record counter.");

            return int.Parse(recordCounter.Text);
        }
    }

    public ListControl(IWebDriver driver, By containerLocator, string css) :
        base(driver, containerLocator, By.CssSelector(css))
    {
        WaitForLoading();
    }

    /// <summary>
    /// Wait for page to load
    /// </summary>
    private void WaitForLoading()
    {
        wait.Until(c => !loading);
        wait.Message = $"Could not load list {id}";
        wait.Until(c => m_control.Displayed);
        wait.Message = $"The list {id} is not visible";
    }

    /// <summary>
    /// Get a row element from it's primary key
    /// </summary>
    /// <param name="pk">Row's primary key</param>
    /// <returns>Row element</returns>
    public int GetRowByPk(string pk)
    {
        WaitForLoading();
        return rows.FindIndex(r => r.GetAttribute("data-key") == pk);
    }

    /// <summary>
    /// Get a column's index from it's name
    /// </summary>
    /// <param name="fieldRef">Column's name</param>
    /// <returns>Column index</returns>
    private int GetRawColumnIndex(string fieldRef)
    {
        string column_locator;

        var parts = fieldRef.Split('.', 2);

        // If the field is given as an exact name, but not as TABLE.COLUMN
        if (parts.Length == 1)
            column_locator = fieldRef;
        // If the field is given as TABLE.COLUMN
        else
            column_locator = CapFirst(parts[0]) + ".Val" + CapFirst(parts[1]);

        return columns.FindIndex(h => h.GetAttribute("data-column-name") == column_locator);
    }

    /// <summary>
    /// Get a column's name from it's index
    /// </summary>
    /// <param name="index">Column's index</param>
    /// <returns>Column name</returns>
    private string GetRawColumnNameByIndex(int index)
    {
        // Bounds checking
        if (index < 0 || index >= columns.Count)
            return null;

        return columns[index].GetAttribute("data-column-name");
    }

    /// <summary>
    /// Get the number of non-data columns at the beginning of the column array
    /// </summary>
    /// <returns>Number of non-data columns</returns>
    private int NonDataColumnAtBeginningCount()
    {
        // Find the first column that is not one of the special types of columns
        int firstNonDataColumnIndex = columns.FindIndex(col =>
        {
            string columnName = col.GetAttribute("data-column-name");
            return !(columnName.Equals("actions") || columnName.Equals("Checkbox") || columnName.Equals("ExtendedAction") || columnName.Equals("dragAndDrop"));
        });

        // If all columns are special columns
        if (firstNonDataColumnIndex == -1)
            return columns.Count;

        // Index of first data column is the number of non-data columns
        return firstNonDataColumnIndex;
    }

    /// <summary>
    /// Get a column's index from it's name, relative to the starting index of data columns
    /// </summary>
    /// <param name="fieldRef">Column's name</param>
    /// <returns>Column index</returns>
    public int GetColumnIndex(string fieldRef)
    {
        WaitForLoading();

        // Get actual column index
        int columnIndex = GetRawColumnIndex(fieldRef);

        // Column not found
        if (columnIndex == -1)
            return -1;

        // Return index starting from where the data columns start
        return columnIndex - NonDataColumnAtBeginningCount();
    }

    /// <summary>
    /// Get a column's name from it's index, relative to the starting index of data columns
    /// </summary>
    /// <param name="index">Column's index</param>
    /// <returns>Column name</returns>
    public string GetColumnNameByIndex(int index)
    {
        WaitForLoading();

        // Add number of non data columns to index to use the actual index when finding the column
        return GetRawColumnNameByIndex(index + NonDataColumnAtBeginningCount());
    }

    /// <summary>
    /// Capitalize the first letter of a string
    /// </summary>
    /// <param name="s">string</param>
    /// <returns>string with the first letter capitalized</returns>
    private string CapFirst(string s)
    {
        if (s.Length == 0) return s;
        if (s.Length == 1) return s.ToUpperInvariant();
        return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1).ToLowerInvariant();
    }

    /// <summary>
    /// Get a cell's value from it's row index and column name
    /// </summary>
    /// <param name="row">Row index</param>
    /// <param name="fieldRef">Column name</param>
    /// <returns>Cell value</returns>
    public string GetValue(int row, string fieldRef)
    {
        // RowCount waits for loading
        if (row < 0 || (row + 1) > RowCount) return null;

        int cix = GetRawColumnIndex(fieldRef);
        var cell = rows[row].FindElements(By.TagName("td"))[cix];

        // If the column is a boolean, get the value from the icon
        var boolIcons = cell.FindElements(By.CssSelector("[component='q-render-boolean']"));
        // Find elements avoids NotFound exceptions - if an icon is not found, the column value is assumed to be text
        if (boolIcons.Count > 0)
        {
            var iconClass = boolIcons[0].GetAttribute("class");
            return iconClass.Contains("true-icon").ToString();
        }

        return cell.Text;
    }

    /// <summary>
    /// Click on a row
    /// </summary>
    public void ClickRow(int index)
    {
        WaitForLoading();
        if (index >= rows.Count)
            throw new ArgumentException($"Invalid row index: {index}");

        rows[index].Click();
    }

    /// <summary>
    /// Click on the insert button
    /// </summary>
    public void Insert()
    {
        insertBtn.Click();
    }

    /// <summary>
    /// Run a row's action from it's row index and action name
    /// </summary>
    /// <param name="index">Row index</param>
    /// <param name="action">Action name</param>
    /// <param name="orderIndex">Index to move row to (only used with row reordering)</param>
    public void ExecuteAction(int index, String action, int orderIndex = 0)
    {
        // RowCount waits for loading
        if (index >= RowCount || index < 0)
            throw new ArgumentException($"Invalid row index: {index}");

        var row = rows[index];

        // For row reorder functions, call specific functions
        if (action.Equals(ReorderAction.Reorder))
            ReorderRow(index, orderIndex);
        else if (action.Equals(ReorderAction.ReorderUp))
            ReorderRowUpOrDown(index, false);
        else if (action.Equals(ReorderAction.ReorderDown))
            ReorderRowUpOrDown(index, true);
        // Normal actions
        else
        {
            var cell = row.FindElement(By.CssSelector("td.row-actions"));

            //if it's a dropdown menu, click the button to open the dropdown. If the dropdown doesn't exist, the buttons are inlined.
            var dropdownButton = cell.FindElements(By.CssSelector("[data-type=options-button]"));
            if (dropdownButton.Count() > 0)
                dropdownButton[0].Click();

            var actionButton = GetElement(driver, By.CssSelector("[role=listbox] [role=option][data-key='" + action + "']"));

            actionButton.Click();
        }
    }

    private void OpenDropdown(int rowIndex)
    {
        // RowCount waits for loading
        if (rowIndex < 0 || rowIndex >= RowCount)
            throw new ArgumentException($"Invalid row index: {rowIndex}");

        IWebElement row = rows[rowIndex];

        // Close any dropdown that may already be open.
        var dropdownUnderlay = driver.FindElements(By.CssSelector(".q-overlay__underlay"));
        if (dropdownUnderlay.Count() > 0)
            dropdownUnderlay[0].Click();

        // Used to clear any current overlays inline
        row.SendKeys(Keys.Escape);

        // Set row again because DOM changes can cause the reference to become stale
        row = rows[rowIndex];
        IWebElement cell = row.FindElement(By.CssSelector("td.row-actions"));

        // If it's a dropdown menu, click the button to open the dropdown. If the dropdown doesn't exist, the buttons are inlined.
        var dropdownButton = cell.FindElements(By.CssSelector("[data-type=options-button]"));
        if (dropdownButton.Count() > 0)
            dropdownButton[0].Click();

        WaitForLoading();
    }

    /// <summary>
    /// Checks if a row's action exists from it's row index and action name
    /// </summary>
    /// <param name="index">Row index</param>
    /// <param name="action">Action name</param>
    public bool IsActionAvailable(int index, string action)
    {
        // OpenDropdown waits for loading
        OpenDropdown(index);
        var actionButton = driver.FindElements(By.CssSelector("[role=listbox] [role=option][data-key='" + action + "']"));

        return actionButton.Count != 0;
    }

    /// <summary>
    /// Gets the number of actions that exist for the specified row
    /// </summary>
    /// <param name="rowIndex">The index of the row</param>
    /// <returns>The number of available actions</returns>
    /// <exception cref="ArgumentException"></exception>
    public int GetActionCount(int rowIndex)
    {
        // OpenDropdown waits for loading
        OpenDropdown(rowIndex);
        var actionButtons = GetElement(driver, ByData.Testid("dropdown-content")).FindElements(By.CssSelector("[role=listbox] [role=option]"));

        return actionButtons.Count;
    }

    /// <summary>
    /// Move the row at the given index to a new index
    /// </summary>
    /// <param name="currentIndex">Index of the row</param>
    /// <param name="newIndex">Index to move the row to</param>
    private void ReorderRow(int currentIndex, int newIndex)
    {
        // The element index starts at 0, it's always 1 less than the column order value
        int newOrderValue = newIndex + 1;
        // Get the row identifier
        string rowId = $"{id}_row-{currentIndex}";

        BaseInputControl rowOrderInput = new BaseInputControl(driver, By.Id(rowId), rowId, "[data-testid='column-config-order']");
        rowOrderInput.SetValue(newOrderValue.ToString());

        // Confirm the value
        rowOrderInput.Confirm();
    }

    /// <summary>
    /// Move the row at the given index up or down one
    /// </summary>
    /// <param name="currentIndex">Index of the row</param>
    /// <param name="incrememnt">Whether to move the row up or down one</param>
    private void ReorderRowUpOrDown(int currentIndex, bool incrememnt)
    {
        string direction = incrememnt ? "down" : "up";

        ButtonControl reorderUpDownBtn = new ButtonControl(driver, By.Id(id + "_row-" + currentIndex), "#" + id + "_row-" + currentIndex + "-reorder-" + direction);

        reorderUpDownBtn.Click();
    }

    /// <summary>
    /// Sort a table by a column
    /// </summary>
    /// <param name="index">Column index</param>
    public void SortTable(int index)
    {
        WaitForLoading();
        var header = GetElement(m_control, By.CssSelector("thead tr"));
        var cells = header.FindElements(By.CssSelector("th"));

        cells[index].Click();
    }

    /// <summary>
    /// Open the table's column configuration interface
    /// </summary>
    public void OpenColumnConfig()
    {
        // Click the table configuration button which can open a menu or popup,
        // depending on how many configuration interfaces are available
        configBtn?.Click();

        // If there are multiple table configuration options,
        // the table configuration menu should be open
        // and the column configuration button should be clicked
        columnConfigBtn?.Click();
    }

    /// <summary>
    /// Toggle row reorder mode
    /// </summary>
    public void ToggleRowReorderMode()
    {
        rowReorderBtn.Click();
    }

    /// <summary>
    /// Add a filter
    /// </summary>
    /// <param name="columnName">Column's name</param>
    /// <param name="operation">Operator</param>
    /// <param name="value">Value</param>
    public void AddFilter(string columnName, string operation, string value)
    {
        configBtn?.Click();

        if (filtersBtn == null)
            return;

        // Open filters popup
        filtersBtn.Click();

        TableFilterPage filterPopup = new(driver, id);

        filterPopup.Create.Click();

        filterPopup.Field.SetValue(columnName);
        filterPopup.Operation.SetValue(operation);
        filterPopup.Value.SetValue(value);

        try
        {
            // For certain field types (like dates), SetValue triggers an implicit Enter key
            // press, which immediately submits the filters. When that happens, the Save button
            // is no longer present in the DOM, so attempting to click it would throw an exception.
            // We safely ignore that case because the filter has already been submitted.
            filterPopup.Save.Click();
        }
        catch { }

        // Wait until the configuration popup closes.
        wait.Until(driver => GetElement(driver, By.CssSelector(".q-table-config")) == null);
    }

    /// <summary>
    /// Clears all the filters in the list
    /// </summary>
    public void ClearFilters()
    {
        WaitForLoading();
        var clearFilters = m_container.FindElements(ByData.Testid("clear-filters"));
        if (clearFilters.Count > 0)
        {
            TestContext.WriteLine($"Clearing filters in list {id}");
            clearFilters[0].Click();
            WaitForLoading();
        }
    }

    /// <summary>
    /// Gets the count of visible column headers in the table, excluding headers with the "thead-actions" class.
    /// </summary>
    /// <returns>The number of visible column headers, excluding those that have the "thead-actions" class.</returns>
    public int GetVisibleColumnCount()
    {
        WaitForLoading();
        return columns.Count() - NonDataColumnAtBeginningCount();
    }

    /// <summary>
    /// Retrieves the cell element at the specified row and column in the table.
    /// </summary>
    /// <param name="rowIndex">Zero-based index of the row containing the cell.</param>
    /// <param name="columnName">Name of the column whose cell is being retrieved.</param>
    public IWebElement GetCellElement(int rowIndex, string columnName)
    {
        // RowCount waits for loading
        if (rowIndex < 0 || rowIndex >= RowCount)
            throw new ArgumentOutOfRangeException($"Invalid row index: {rowIndex}");

        int colIndex = GetRawColumnIndex(columnName);
        if (colIndex < 0)
            throw new ArgumentException($"Column not found: {columnName}", nameof(columnName));

        var cells = rows[rowIndex].FindElements(By.TagName("td"));

        return cells[colIndex];
    }

    /// <summary>
    /// Gets the checkbox element for the specified row.
    /// </summary>
    /// <param name="rowIndex">Zero-based index of the row containing the checkbox.</param>
    /// <param name="columnName">Name of the column whose cell contains the checkbox.</param>
    /// <returns>The checkbox element.</returns>
    private IWebElement GetCellCheckbox(int rowIndex, string columnName)
    {
        // GetCellElement waits for loading
        var cell = GetCellElement(rowIndex, columnName);
        return cell.FindElement(By.CssSelector(".checklist-col-base"));
    }

    /// <summary>
    /// Retrieves the visibility state of the column at the specified index.
    /// Assumes there is a checkbox in the row that indicates the column's visibility.
    /// </summary>
    /// <param name="rowIndex">Zero-based index of the row containing the checkbox.</param>
    /// <param name="columnName">Name of the column whose visibility checkbox should be toggled.</param>
    /// <returns>True if the column is visible; otherwise, false.</returns>
    public bool GetCellCheckboxState(int rowIndex, string columnName)
    {
        // RowCount waits for loading
        if (rowIndex < 0 || rowIndex >= RowCount)
            throw new ArgumentOutOfRangeException(nameof(rowIndex), $"Invalid row index: {rowIndex}");

        var checkbox = GetCellCheckbox(rowIndex, columnName);
        bool attrValue = checkbox.FindElement(By.CssSelector("input")).Selected;
        return attrValue.Equals(true);
    }

    /// <summary>
    /// Sets the visibility state of the column at the specified index.
    /// If the desired state is different from the current state, the method clicks the checkbox.
    /// </summary>
    /// <param name="rowIndex">Zero-based index of the row containing the checkbox.</param>
    /// <param name="columnName">Name of the column whose visibility checkbox should be toggled.</param>
    /// <param name="visible">Desired visibility state (true for visible, false for hidden).</param>
    public void SetCellCheckboxState(int rowIndex, string columnName, bool visible)
    {
        // RowCount waits for loading
        if (rowIndex < 0 || rowIndex >= RowCount)
            throw new ArgumentOutOfRangeException(nameof(rowIndex), $"Invalid row index: {rowIndex}");

        var checkbox = GetCellCheckbox(rowIndex, columnName);

        bool currentState = GetCellCheckboxState(rowIndex, columnName);

        if (checkbox != null)
        {
            // Check if the current state differs from the desired state.
            if (currentState != visible)
            {
                // Click the checkbox to toggle its state.
                checkbox.Click();
                // Optionally wait until the checkbox state updates.
                wait.Until(driver => GetCellCheckboxState(rowIndex, columnName) == visible);
            }
        }
    }

    /// <summary>
    /// Get all values of a specific table column
    /// </summary>
    /// <param name="columnName">The name of the column. Corresponds to `data-column-name` in the HTML.</param>
    /// <returns>A list of the column values, in row order.</returns>
    public List<string> GetAllColumnValues(string columnName)
    {
        // GetValue waits for loading
        return [.. rows.Select((_, idx) => GetValue(idx, columnName))];
    }
}

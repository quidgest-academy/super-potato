using System.Collections.Generic;

namespace quidgest.uitests.controls;

public abstract class BaseGridControl : ControlObject
{
    protected int currentRowIndex = 0;
    protected string currentRowPk = "";

    protected By lineLocator => new ByChained(m_controlLocator, ByData.Key(currentRowPk));

    protected IList<IWebElement> rows => m_control.FindElements(By.CssSelector("tbody tr"));
    protected IWebElement currentRow => m_control.FindElement(ByData.Key(currentRowPk));
    protected IWebElement delButton => currentRow.FindElement(By.CssSelector(".grid-table-row__action button[data-testid=delete]"));
    protected IWebElement undoButton => currentRow.FindElement(By.CssSelector(".grid-table-row__action button[data-testid=undo]"));


    protected BaseGridControl(IWebDriver driver, By containerLocator, By controlLocator)
        : base(driver, containerLocator, controlLocator)
    {
        WaitForLoad();
    }

    private void WaitForLoad()
    {
        wait.Until(c => m_control.GetAttribute("data-loading") == "false" || m_control.GetAttribute("data-loading") == null);
    }

    public string GetCurrentPk()
    {
        return currentRowPk;
    }

    public void SetCurrentRow(int ix)
    {
        currentRowIndex = ix;
        currentRowPk = rows[ix].GetAttribute("data-key");
    }

    public int FindRowByPk(string pk)
    {
        return rows.FindIndex(c => c.GetAttribute("data-key") == pk);
    }

    public int RowCount => rows.Count;

    public void SetInsertRow()
    {
        //the insert row is always the last row
        var rowix = rows.Count - 1;
        if (rowix < 0) rowix = 0;
        SetCurrentRow(rowix);
    }

    public void DeleteRow()
    {
        delButton.Click();
    }

    public void UndoRow()
    {
        undoButton.Click();
    }
}
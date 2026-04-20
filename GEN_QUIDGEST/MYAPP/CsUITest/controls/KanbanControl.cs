using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace quidgest.uitests.controls;

public class KanbanControl: ControlObject
{
    private IList<IWebElement> columnList => m_control.FindElements(By.CssSelector(".q-kanban-column"));

    private Dictionary<int,IList<IWebElement>> cardList => columnList.Select((m_column, index) => new {
        Index = index,
        Cards = m_column.FindElements(By.CssSelector(".q-kanban-item"))
    }).ToDictionary(x => x.Index, x => (IList<IWebElement>)x.Cards);
    
    private bool loading => m_control.FindElements(By.CssSelector("[data-loading=true]")).Any();    

    public KanbanControl(IWebDriver driver, By containerLocator, string css) :
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
    }

    public IWebElement GetColumnByIndex(int index)
    {
        if (index >= columnList.Count || index < 0)
           return null;

        return columnList[index];
    }

    public IWebElement GetCardByIndex(int columnIndex, int index)
    {
        if (index >= cardList[columnIndex].Count || index < 0)
            return null;

        return cardList[columnIndex][index];
    }

    /// <summary>
    /// Get the column title
    /// </summary>
    /// <param name="index">Column index</param>
    /// <returns>Title</returns>
    public string GetColumnName(int index)
    {
        IWebElement m_column = GetColumnByIndex(index);
        if (m_column == null)
            throw new ArgumentException($"Invalid card index: {index}");
        
        var title = m_column.FindElement(By.CssSelector(".q-kanban-column__title"));
        return title.Text;
    }

    /// <summary>
    /// Click on the insert button from a column index
    /// </summary>
    /// <param name="index">Column index</param>
    public void Insert(int index)
    {
        IWebElement m_column = GetColumnByIndex(index);
        if (m_column == null)
            throw new ArgumentException($"Invalid card index: {index}");

        IWebElement insertButton = m_column.FindElement(By.CssSelector("button.q-kanban__add"));
        insertButton.Click();
    }
}
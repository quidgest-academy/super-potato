using System.Collections.Generic;
using System.Linq;

namespace quidgest.uitests.controls;

public class CardsControl: ControlObject
{
    /// <summary>
    /// Search bar
    /// </summary>
    public SearchControl Search => new SearchControl(driver, m_containerLocator, m_controlLocator);
    private IList<IWebElement> cardList => m_control.FindElements(ByData.Testid("q-card-view"));
    private IWebElement insertCard => m_control.FindElement(By.CssSelector(".q-card-view--insert"));
    private bool loading => m_control.FindElements(By.CssSelector("[data-loading=true]")).Any();    

    public CardsControl(IWebDriver driver, By containerLocator, string css) :
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

    private IWebElement GetCardByIndex(int index)
    {
        if (index >= cardList.Count || index < 0)
            throw new ArgumentException($"Invalid card index: {index}");

        return cardList[index];
    }

    /// <summary>
    /// Get the card title
    /// </summary>
    /// <param name="index">Card index</param>
    /// <returns>Title</returns>
    public string GetCardTitle(int index)
    {
        var card = GetCardByIndex(index);
        var title = card.FindElement(By.CssSelector(".q-card-view__title"));
        return title.Text;
    }

    /// <summary>
    /// Get the card subtitle
    /// </summary>
    /// <param name="index">Card index</param>
    /// <returns>Subtitle</returns>
    public string GetCardSubTitle(int index)
    {
        var card = GetCardByIndex(index);
        var subtitle = card.FindElement(By.CssSelector(".q -card__subtitle"));
        return subtitle.Text;        
    }

    /// <summary>
    /// Click on a card at specific index
    /// </summary>
    public void ClickCard(int index)
    {
        WaitForLoading();        
        GetCardByIndex(index).Click();
    }

    /// <summary>
    /// Run a Card's action from it's card index and action name
    /// </summary>
    /// <param name="index">Row index</param>
    /// <param name="action">Action name</param>
    public void ExecuteAction(int index, string action)
    {
        WaitForLoading();        
        var card = GetCardByIndex(index); 
        //TODO: instead of title it should be data-key=action
        var button = card.FindElement(By.CssSelector($"[data-key={action}]"));

        button.Click();
    }

    /// <summary>
    /// Click on the insert card
    /// </summary>
    public void Insert()
    {
        insertCard.Click();
    }

    /// <summary>
    /// Get a propertie value from a card's index and field name
    /// </summary>
    /// <param name="row">card index</param>
    /// <param name="fieldRef">field name [TABLE].[FIELD]</param>
    /// <returns>Cell value</returns>
    public string GetValue(int index, string fieldRef)
    {
        WaitForLoading();
        var card = GetCardByIndex(index);
        var paragraph = card.FindElement(By.CssSelector($"[data-field=\"{fieldRef.ToUpper()}\"]"));
        var dataField = paragraph.FindElement(By.CssSelector($"[data-field-value=true]"));
        return dataField.Text;      
    }
}


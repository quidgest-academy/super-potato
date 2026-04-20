using System.Collections.Generic;

namespace quidgest.uitests.controls;

public class WidgetFavoritesControl
{
    public WidgetFavoritesControl(IWebDriver driver)
    {
        Widgets = [];

        List<IWebElement> favs = [.. driver.FindElements(By.CssSelector("[id ^= 'w-Bookmark_']"))];
        if (favs == null)
            return;

        foreach (IWebElement fav in favs)
        {
            Widgets.Add(new WidgetMenuControl(driver, By.Id(fav.GetAttribute("id")), ".q-widget"));
        }
    }

    public List<WidgetMenuControl> Widgets;

    public WidgetMenuControl GetBookmarkWidget(string title)
    {
        return Widgets.Find(w => w.IsVisible && w.GetTitle() == title);
    }
}

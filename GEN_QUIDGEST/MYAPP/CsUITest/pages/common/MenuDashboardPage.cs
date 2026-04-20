using quidgest.uitests.pages.common;

namespace quidgest.uitests.pages;

public class MenuDashboardPage<DSType> : MenuPage
where DSType : BaseDashboardControl
{
	public MenuDashboardPage(IWebDriver driver, string module, string menuId): base(driver) {
		if (string.IsNullOrEmpty(module)) throw new ArgumentException($"{nameof(module)} must contain value.");
		if (string.IsNullOrEmpty(menuId)) throw new ArgumentException($"{nameof(menuId)} must contain value.");
		
		id = module.ToUpperInvariant() + "-menu-" + module.ToUpperInvariant() + "_" + menuId;
        Dashboard = (DSType)Activator.CreateInstance(
            typeof(DSType),
            driver,
            By.Id("form-container"),
            ".q-dashboard"
        );

		wait.Until(c => page);
	}

    public DSType Dashboard;
}

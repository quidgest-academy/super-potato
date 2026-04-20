using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quidgest.uitests.pages.common
{
    public class MenuHomePage : MenuPage
    {
        public MenuHomePage(IWebDriver driver, string module) : base(driver)
        {
            if (string.IsNullOrEmpty(module)) throw new ArgumentException($"{nameof(module)} must contain value.");
            this.id = $"{module.ToUpperInvariant()}-home-{module.ToUpperInvariant()}";
            
            wait.Until(c => page);
        }
    }
}

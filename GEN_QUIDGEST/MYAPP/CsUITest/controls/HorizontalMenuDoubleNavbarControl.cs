using quidgest.uitests.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quidgest.uitests.controls
{
    public class HorizontalMenuDoubleNavbarControl : HorizontalMenuControl
    {
        private IWebElement SecondNavMenus => driver.FindElement(By.ClassName("n-menu__navbar--double-l2"));
        public HorizontalMenuDoubleNavbarControl(IWebDriver driver, MenuTree menuTree) : base(driver, menuTree)
        {
            wait.Until(c => SecondNavMenus);            
        }

        protected override void WaitForLoading()
        {
            base.WaitForLoading();
            wait.Until(c => SecondNavMenus);
        }

        protected override void ClickParentRecursive(string moduleId, MenuTreeNode node)
        {
            var parent = node.Parent;
            if (parent != null)
                ClickParentRecursive(moduleId, parent);

            //the menu belongs to the first Navbar
            if (parent == null)
            {
                var liTarget = menus.FindElement(By.Id(moduleId + node.Id));
                liTarget.Click();
            }
            else
            {
                //the menu belongs to the second Navbar
                var liTarget = SecondNavMenus.FindElement(By.Id(moduleId + node.Id));
                try
                {
                    var btn = liTarget.FindElement(By.ClassName("dropdown-toggle"));
                    btn.Click();
                }
                catch (Exception)
                {
                    liTarget.Click();
                }
            }
        }
    }
}

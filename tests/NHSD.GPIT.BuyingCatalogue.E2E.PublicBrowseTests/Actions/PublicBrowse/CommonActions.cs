using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse
{
    internal sealed class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver)
            : base(driver)
        {
        }

        internal string GetBreadcrumbNames(string breadcrumbItem)
        {
            var rows = Driver.FindElements(SolutionObjects.BreadcrumbsBanner);
            var row = rows.Single(r => r.FindElement(By.TagName("a")).Text.Contains(breadcrumbItem, StringComparison.OrdinalIgnoreCase));
            return row.FindElement(By.TagName("a")).Text;
        }
    }
}

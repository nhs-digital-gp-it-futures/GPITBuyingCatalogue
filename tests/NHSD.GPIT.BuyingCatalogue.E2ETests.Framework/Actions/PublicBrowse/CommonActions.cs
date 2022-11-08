using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.PublicBrowse
{
    public sealed class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver)
            : base(driver)
        {
        }

        public string GetBreadcrumbNames(string breadcrumbItem)
        {
            var rows = Driver.FindElements(SolutionObjects.BreadcrumbsBanner);
            var row = rows.First(r => r.FindElement(By.TagName("a")).Text.Contains(breadcrumbItem, StringComparison.OrdinalIgnoreCase));
            return row.FindElement(By.TagName("a")).Text;
        }
    }
}

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

        internal string PageTitle()
        {
            return Driver.FindElement(CommonObjects.PageTitle).Text;
        }

        internal void ClickGoBackLink()
        {
            Driver.FindElement(CommonObjects.GoBackLink).Click();
        }

        internal void ClickContinueButton()
        {
            Driver.FindElement(CommonObjects.ContinueButton).Click();
        }

        internal void ClickLoginLink()
        {
            Driver.FindElement(CommonObjects.LoginLink).Click();
        }

        internal void ClickHomeBreadcrumb()
        {
            Driver.FindElement(HomepageObjects.HomePageCrumb).Click();
        }

        internal bool BreadcrumbBannerDisplayed()
        {
            return ElementDisplayed(SolutionObjects.BreadcrumbsBanner);
        }

        internal string GetBreadcrumbNames(string breadcrumbItem)
        {
            var rows = Driver.FindElements(SolutionObjects.BreadcrumbsBanner);
            var row = rows.Single(r => r.FindElement(By.TagName("a")).Text.Contains(breadcrumbItem, System.StringComparison.OrdinalIgnoreCase));
            return row.FindElement(By.TagName("a")).Text;
        }

        internal void ClickCatalogueSolutionBreadcrumb()
        {
            Driver.FindElement(SolutionObjects.CatalogueSolutionCrumb).Click();
        }

        private bool ElementDisplayed(By by)
        {
            try
            {
                Driver.FindElement(by);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Admin
{
    public sealed class Dashboard : ActionBase
    {
        public Dashboard(IWebDriver driver)
            : base(driver)
        {
        }

        public IEnumerable<string> GetOrganisationNamesOnPage()
        {
            return Driver.FindElements(Objects.Admin.DashboardObjects.OrganisationNames).Select(s => s.Text);
        }

        public IEnumerable<string> GetOrganisationOdsCodesOnPage()
        {
            return Driver.FindElements(Objects.Admin.DashboardObjects.OrganisationOdsCodes).Select(s => s.Text);
        }

        public IEnumerable<string> GetOrganisationLinkIdsOnPage()
        {
            return Driver.FindElements(Objects.Admin.DashboardObjects.OrganisationLinks).Select(s => s.GetAttribute("data-org-id"));
        }

        public void ClickBuyerOrgLink()
        {
            Driver.FindElement(Objects.Admin.DashboardObjects.BuyerOrgLink).Click();
        }
    }
}

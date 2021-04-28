using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal sealed class MarketingPageActions
    {
        public MarketingPageActions(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {
                DashboardActions = new DashboardActions(driver),
                SolutionDescriptionActions = new SolutionDescriptionActions(driver)
            };
        }

        public ActionCollection PageActions { get; set; }
    }
}

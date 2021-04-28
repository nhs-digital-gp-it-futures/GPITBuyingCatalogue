using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal sealed class MarketingPageActions
    {
        public MarketingPageActions(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {
                AboutSupplierActions = new(driver),
                CommonActions = new(driver),
                DashboardActions = new(driver),
                FeaturesActions = new(driver),
                SolutionDescriptionActions = new(driver)
            };
        }

        public ActionCollection PageActions { get; set; }
    }
}

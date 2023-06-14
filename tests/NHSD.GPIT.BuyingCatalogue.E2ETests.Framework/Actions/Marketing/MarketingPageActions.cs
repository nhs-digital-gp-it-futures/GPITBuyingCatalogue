using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Marketing
{
    public sealed class MarketingPageActions
    {
        public MarketingPageActions(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {
                ContactDetailsActions = new(driver),
                DashboardActions = new(driver),
                FeaturesActions = new(driver),
                ApplicationTypeActions = new(driver),
                PreviewActions = new(driver),
                HostingTypeActions = new(driver),
            };
        }

        public ActionCollection PageActions { get; set; }
    }
}

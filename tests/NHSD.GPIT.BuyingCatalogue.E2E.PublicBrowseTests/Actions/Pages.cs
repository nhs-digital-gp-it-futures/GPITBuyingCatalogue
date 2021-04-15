using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Actions
{
    public sealed class Pages
    {
        public Pages(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {
                HomePageActions = new HomepageActions(driver),
            };
        }

        internal ActionCollection PageActions { get; }
    }
}

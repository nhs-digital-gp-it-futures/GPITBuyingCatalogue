using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Actions
{
    public sealed class Pages
    {
        public Pages(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {
                BuyersGuideActions = new BuyersGuideActions(driver),
                CommonActions = new CommonActions(driver),
                HomePageActions = new HomepageActions(driver),
                SolutionsActions = new SolutionsActions(driver),
            };
        }

        internal ActionCollection PageActions { get; }
    }
}

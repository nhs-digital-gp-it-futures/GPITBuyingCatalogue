using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse
{
    public sealed class PublicBrowsePages
    {
        public PublicBrowsePages(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {
                BuyersGuideActions = new BuyersGuideActions(driver),
                CommonActions = new CommonActions(driver),
                HomePageActions = new HomepageActions(driver),
                SolutionsActions = new SolutionsActions(driver),
                SolutionAction = new SolutionAction(driver),
            };
        }

        internal ActionCollection PageActions { get; }
    }
}

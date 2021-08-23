using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse
{
    public sealed class PublicBrowsePages
    {
        public PublicBrowsePages(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {
                BuyersGuideActions = new(driver),
                CommonActions = new(driver),
                SolutionsActions = new(driver),
                SolutionAction = new(driver),
                CapabilitySelectorActions = new(driver),
            };
        }

        internal ActionCollection PageActions { get; }
    }
}

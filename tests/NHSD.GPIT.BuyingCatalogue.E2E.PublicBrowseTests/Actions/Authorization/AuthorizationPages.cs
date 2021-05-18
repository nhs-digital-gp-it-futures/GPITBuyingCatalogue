using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Authorization
{
    internal sealed class AuthorizationPages
    {
        public AuthorizationPages(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {
                CommonActions = new(driver),
                LoginActions = new(driver),
            };
        }

        internal ActionCollection PageActions { get; set; }
    }
}

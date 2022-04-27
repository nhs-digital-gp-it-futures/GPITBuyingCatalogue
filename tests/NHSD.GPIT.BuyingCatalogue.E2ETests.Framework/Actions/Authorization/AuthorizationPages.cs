using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Authorization
{
    public sealed class AuthorizationPages
    {
        public AuthorizationPages(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {
                CommonActions = new(driver),
                LoginActions = new(driver),
            };
        }

        public ActionCollection PageActions { get; set; }
    }
}

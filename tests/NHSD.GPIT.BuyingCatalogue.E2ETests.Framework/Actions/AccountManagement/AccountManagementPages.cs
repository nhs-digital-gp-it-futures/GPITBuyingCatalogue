using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.AccountManagement
{
    public sealed class AccountManagementPages
    {
        public AccountManagementPages(IWebDriver driver)
        {
            PageActions = new()
            {
                AddUser = new(driver),
                CommonActions = new(driver),
                Details = new(driver),
            };
        }

        public ActionCollection PageActions { get; set; }
    }
}

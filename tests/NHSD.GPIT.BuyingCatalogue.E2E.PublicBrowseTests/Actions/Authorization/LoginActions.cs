using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Authorization;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Authorization
{
    internal sealed class LoginActions : ActionBase
    {
        public LoginActions(IWebDriver driver)
            : base(driver)
        {
        }

        internal bool EmailAddressInputDisplayed()
        {
            return ElementDisplayed(AuthorizationObjects.EmailAddress);
        }

        internal bool PasswordInputDisplayed()
        {
            return ElementDisplayed(AuthorizationObjects.Password);
        }

        internal bool LoginButtonDisplayed()
        {
            return ElementDisplayed(AuthorizationObjects.LoginButton);
        }

        internal void Login(string userEmail, string password)
        {
            Driver.FindElement(AuthorizationObjects.EmailAddress).SendKeys(userEmail);
            Driver.FindElement(AuthorizationObjects.Password).SendKeys(password);

            Driver.FindElement(AuthorizationObjects.LoginButton).Click();
        }

        private bool ElementDisplayed(By by)
        {
            try
            {
                Driver.FindElement(by);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

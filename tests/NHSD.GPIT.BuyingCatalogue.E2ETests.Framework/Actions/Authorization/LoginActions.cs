using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Authorization;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Authorization
{
    public sealed class LoginActions : ActionBase
    {
        public LoginActions(IWebDriver driver)
            : base(driver)
        {
        }

        public bool EmailAddressInputDisplayed()
        {
            return ElementDisplayed(AuthorizationObjects.EmailAddress);
        }

        public bool PasswordInputDisplayed()
        {
            return ElementDisplayed(AuthorizationObjects.Password);
        }

        public bool LoginButtonDisplayed()
        {
            return ElementDisplayed(AuthorizationObjects.LoginButton);
        }

        public void Login(string userEmail, string password)
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

using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Authorization;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Authorization
{
    public sealed class UpdatePasswordActions : ActionBase
    {
        public UpdatePasswordActions(IWebDriver driver)
            : base(driver)
        {
        }

        public bool CurrentPasswordInputDisplayed()
        {
            return ElementDisplayed(UpdatePasswordObjects.CurrentPassword);
        }

        public bool NewPasswordInputDisplayed()
        {
            return ElementDisplayed(UpdatePasswordObjects.NewPassword);
        }

        public bool ConfirmPasswordInputDisplayed()
        {
            return ElementDisplayed(UpdatePasswordObjects.ConfirmPassword);
        }

        public bool SavePasswordButtonDisplayed()
        {
            return ElementDisplayed(UpdatePasswordObjects.SavePasswordButton);
        }

        public void SavePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            Driver.FindElement(UpdatePasswordObjects.CurrentPassword).SendKeys(currentPassword);
            Driver.FindElement(UpdatePasswordObjects.NewPassword).SendKeys(newPassword);
            Driver.FindElement(UpdatePasswordObjects.ConfirmPassword).SendKeys(confirmPassword);
            Driver.FindElement(AuthorizationObjects.LoginButton).Click();
        }

        public void ClickSavePassword()
        {
            Driver.FindElement(UpdatePasswordObjects.SavePasswordButton).Click();
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

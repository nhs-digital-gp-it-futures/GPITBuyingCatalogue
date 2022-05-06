using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Admin
{
    public sealed class UserDetails : ActionBase
    {
        public UserDetails(IWebDriver driver)
            : base(driver)
        {
        }

        public string GetOrganisationName()
        {
            return Driver.FindElement(Objects.Admin.UserDetailsObjects.OrganisationName).Text;
        }

        public string GetUserName()
        {
            return Driver.FindElement(Objects.Admin.UserDetailsObjects.UserName).Text;
        }

        public string GetContactDetails()
        {
            return Driver.FindElement(Objects.Admin.UserDetailsObjects.ContactDetails).Text;
        }

        public string GetEmailAddress()
        {
            return Driver.FindElement(Objects.Admin.UserDetailsObjects.Email).Text;
        }

        public void DisableEnableUser()
        {
            Driver.FindElement(Objects.Admin.UserDetailsObjects.ToggleDisabledButton).Click();
        }
    }
}

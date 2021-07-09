using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin
{
    internal sealed class UserDetails : ActionBase
    {
        public UserDetails(IWebDriver driver)
            : base(driver)
        {
        }

        internal string GetOrganisationName()
        {
            return Driver.FindElement(Objects.Admin.UserDetailsObjects.OrganisationName).Text;
        }

        internal string GetUserName()
        {
            return Driver.FindElement(Objects.Admin.UserDetailsObjects.UserName).Text;
        }

        internal string GetContactDetails()
        {
            return Driver.FindElement(Objects.Admin.UserDetailsObjects.ContactDetails).Text;
        }

        internal string GetEmailAddress()
        {
            return Driver.FindElement(Objects.Admin.UserDetailsObjects.Email).Text;
        }

        internal void DisableEnableUser()
        {
            Driver.FindElement(Objects.Admin.UserDetailsObjects.ToggleDisabledButton).Click();
        }
    }
}

using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class UserDetailsObjects
    {
        public static By OrganisationName => ByExtensions.DataTestId("organisation-name");

        public static By UserName => ByExtensions.DataTestId("user-name");

        public static By ContactDetails => ByExtensions.DataTestId("user-contact-details");

        public static By Email => ByExtensions.DataTestId("user-email");

        public static By ToggleDisabledButton => ByExtensions.DataTestId("change-account-status-button", "button");
    }
}

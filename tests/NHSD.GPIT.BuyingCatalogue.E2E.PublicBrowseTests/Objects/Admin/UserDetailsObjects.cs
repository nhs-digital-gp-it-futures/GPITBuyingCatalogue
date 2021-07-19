using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class UserDetailsObjects
    {
        internal static By OrganisationName => ByExtensions.DataTestId("organisation-name");

        internal static By UserName => ByExtensions.DataTestId("user-name");

        internal static By ContactDetails => ByExtensions.DataTestId("user-contact-details");

        internal static By Email => ByExtensions.DataTestId("user-email");

        internal static By ToggleDisabledButton => ByExtensions.DataTestId("change-account-status-button", "button");
    }
}

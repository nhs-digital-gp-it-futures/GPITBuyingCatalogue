using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class UserDetailsObjects
    {
        internal static By OrganisationName => CustomBy.DataTestId("organisation-name");

        internal static By UserName => CustomBy.DataTestId("user-name");

        internal static By ContactDetails => CustomBy.DataTestId("user-contact-details");

        internal static By Email => CustomBy.DataTestId("user-email");

        internal static By ToggleDisabledButton => CustomBy.DataTestId("change-account-status-button", "button");
    }
}

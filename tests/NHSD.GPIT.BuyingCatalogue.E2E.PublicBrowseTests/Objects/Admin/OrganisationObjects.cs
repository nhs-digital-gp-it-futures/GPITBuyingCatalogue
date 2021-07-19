using System;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class OrganisationObjects
    {
        internal static By AddressLines => By.CssSelector("div[data-test-id^=org-page-address-]");

        internal static By OdsCode => ByExtensions.DataTestId("org-page-ods-code");

        internal static By AddUserButton => ByExtensions.DataTestId("add-user-button", "a");

        internal static By UserTable => ByExtensions.DataTestId("user-table", "table");

        internal static By AddRelatedOrgButton => ByExtensions.DataTestId("add-organisation-button", "a");

        internal static By RelatedOrgsTable => ByExtensions.DataTestId("related-org-table", "table");

        internal static By RelatedOrganisationRemoveConfirm => ByExtensions.DataTestId("submit-button", "button");

        internal static By RelatedOrgTableOrgName(Guid orgId) => ByExtensions.DataTestId($"related-org-name-{orgId}");

        internal static By RelatedOrgTableOdsCode(Guid orgId) => ByExtensions.DataTestId($"related-org-odsCode-{orgId}");

        internal static By RelatedOrganisationRemove(Guid orgId) => ByExtensions.DataTestId($"related-org-remove-{orgId}");

        internal static By UserName(string id) => ByExtensions.DataTestId($"user-name-{id}");
    }
}

using System;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class OrganisationObjects
    {
        internal static By AddressLines => By.CssSelector("div[data-test-id^=org-page-address-]");

        internal static By OdsCode => CustomBy.DataTestId("org-page-ods-code");

        internal static By AddUserButton => CustomBy.DataTestId("add-user-button", "a");

        internal static By UserTable => CustomBy.DataTestId("user-table", "table");

        internal static By AddRelatedOrgButton => CustomBy.DataTestId("add-organisation-button", "a");

        internal static By RelatedOrgsTable => CustomBy.DataTestId("related-org-table", "table");

        internal static By RelatedOrganisationRemoveConfirm => CustomBy.DataTestId("submit-button", "button");

        internal static By RelatedOrgTableOrgName(Guid orgId) => CustomBy.DataTestId($"related-org-name-{orgId}");

        internal static By RelatedOrgTableOdsCode(Guid orgId) => CustomBy.DataTestId($"related-org-odsCode-{orgId}");

        internal static By RelatedOrganisationRemove(Guid orgId) => CustomBy.DataTestId($"related-org-remove-{orgId}");

        internal static By UserName(string id) => CustomBy.DataTestId($"user-name-{id}");
    }
}

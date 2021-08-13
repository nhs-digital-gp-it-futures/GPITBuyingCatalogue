using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class OrganisationObjects
    {
        internal static By AddressLines => ByExtensions.DataTestId("org-page-address");

        internal static By OdsCode => ByExtensions.DataTestId("org-page-ods-code");

        internal static By AddUserButton => ByExtensions.DataTestId("add-user-button", "a");

        internal static By UserTable => ByExtensions.DataTestId("user-table", "table");

        internal static By AddRelatedOrgButton => ByExtensions.DataTestId("add-organisation-button", "a");

        internal static By RelatedOrgsTable => ByExtensions.DataTestId("related-org-table", "table");

        internal static By RelatedOrganisationRemoveConfirm => ByExtensions.DataTestId("submit-button", "button");

        internal static By RelatedOrgTableOrgName(int orgId) => ByExtensions.DataTestId($"related-org-name-{orgId}");

        internal static By RelatedOrgTableOdsCode(int orgId) => ByExtensions.DataTestId($"related-org-odsCode-{orgId}");

        internal static By RelatedOrganisationRemove(int orgId) => ByExtensions.DataTestId($"related-org-remove-{orgId}");

        internal static By UserName(int id) => ByExtensions.DataTestId($"user-name-{id}");
    }
}

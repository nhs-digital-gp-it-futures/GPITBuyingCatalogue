using System;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class OrganisationObjects
    {
        public static By AddressLines => By.CssSelector("div[data-test-id^=org-page-address-]");

        public static By OdsCode => CustomBy.DataTestId("org-page-ods-code");

        public static By AddUserButton => CustomBy.DataTestId("add-user-button", "a");

        public static By UserTable => CustomBy.DataTestId("user-table", "table");

        public static By AddRelatedOrgButton => CustomBy.DataTestId("add-organisation-button", "a");

        public static By RelatedOrgsTable => CustomBy.DataTestId("related-org-table", "table");

        internal static By RelatedOrgTableOrgName(Guid orgId) => CustomBy.DataTestId($"related-org-name-{orgId}");

        internal static By RelatedOrgTableOdsCode(Guid orgId) => CustomBy.DataTestId($"related-org-odsCode-{orgId}");
    }
}

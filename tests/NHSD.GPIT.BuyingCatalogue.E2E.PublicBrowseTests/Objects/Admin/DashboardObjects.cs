using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class DashboardObjects
    {
        internal static By AddOrgButton => CustomBy.DataTestId("add-org-button");

        internal static By OrganisationOdsCodes => CustomBy.DataTestId("org-ods-code");

        internal static By OrganisationNames => CustomBy.DataTestId("org-name");

        internal static By OrganisationLinks => CustomBy.DataTestId("org-link");
    }
}

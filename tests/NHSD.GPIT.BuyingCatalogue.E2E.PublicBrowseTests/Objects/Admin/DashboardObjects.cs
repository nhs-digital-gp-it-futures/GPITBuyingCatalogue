using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class DashboardObjects
    {
        internal static By OrganisationOdsCodes => ByExtensions.DataTestId("org-ods-code");

        internal static By OrganisationNames => ByExtensions.DataTestId("org-name");

        internal static By OrganisationLinks => ByExtensions.DataTestId("org-link");

        internal static By BuyerOrgLink => ByExtensions.DataTestId("buyer-org-link");
    }
}

using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class DashboardObjects
    {
        public static By OrganisationOdsCodes => ByExtensions.DataTestId("org-ods-code");

        public static By OrganisationNames => ByExtensions.DataTestId("org-name");

        public static By OrganisationLinks => ByExtensions.DataTestId("org-link");

        public static By BuyerOrgLink => ByExtensions.DataTestId("buyer-org-link");
    }
}

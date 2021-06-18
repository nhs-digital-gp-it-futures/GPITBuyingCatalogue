using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class DashboardObjects
    {
        internal static By AddOrgButton => CustomBy.DataTestId("add-org-button");

        internal static By OrganisationNameLinks => CustomBy.DataTestId("org-table", "a");
    }
}

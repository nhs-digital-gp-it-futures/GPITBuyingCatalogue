using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class ManageCatalogueSolutionObjects
    {
        internal static By CatalogueSolutionDashboardTable => ByExtensions.DataTestId("manage-solution-table");

        internal static By PublicationStatusInput => By.Id("manage-catalogue-solution");

        internal static By PublicationStatusInputError => By.Id("manage-catalogue-solution-error");
    }
}

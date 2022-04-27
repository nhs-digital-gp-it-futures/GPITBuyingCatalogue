using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class ManageCatalogueSolutionObjects
    {
        public static By CatalogueSolutionDashboardTable => ByExtensions.DataTestId("manage-solution-table");

        public static By PublicationStatusInput => By.Id("manage-catalogue-solution");

        public static By PublicationStatusInputError => By.Id("manage-catalogue-solution-error");
    }
}

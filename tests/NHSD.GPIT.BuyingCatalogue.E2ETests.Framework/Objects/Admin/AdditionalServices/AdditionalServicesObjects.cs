using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.AdditionalServices
{
    public static class AdditionalServicesObjects
    {
        public static By AdditionalServicesTable => ByExtensions.DataTestId("additional-services-table");

        public static By AdditionalServicesTableDashboard => ByExtensions.DataTestId("additional-service-dashboard-table");

        public static By PublicationStatusInputError => By.Id("edit-additional-service-error");
    }
}

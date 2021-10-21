using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.AdditionalServices
{
    internal static class AdditionalServices
    {
        internal static By AdditionalServicesTable => ByExtensions.DataTestId("additional-services-table");

        internal static By AdditionalServicesTableDashboard => ByExtensions.DataTestId("additional-service-dashboard-table");
    }
}

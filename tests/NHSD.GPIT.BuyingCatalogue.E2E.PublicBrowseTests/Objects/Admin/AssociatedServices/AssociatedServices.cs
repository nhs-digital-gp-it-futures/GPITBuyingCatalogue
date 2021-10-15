using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.AssociatedServices
{
    internal static class AssociatedServices
    {
        internal static By EditLink => ByExtensions.DataTestId("edit-link");

        internal static By AddAssociatedServiceContinueButton => By.LinkText("Continue");

        internal static By AssociatedServicesTable => ByExtensions.DataTestId("associated-services-table");

        internal static By AssociatedServiceDashboardTable => ByExtensions.DataTestId("associated-service-dashboard-table");
    }
}

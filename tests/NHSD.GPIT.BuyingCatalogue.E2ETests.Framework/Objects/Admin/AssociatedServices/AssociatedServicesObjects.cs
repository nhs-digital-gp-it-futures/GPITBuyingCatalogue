using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.AssociatedServices
{
    public static class AssociatedServicesObjects
    {
        public static By EditLink => ByExtensions.DataTestId("edit-link");

        public static By AddAssociatedServiceContinueButton => By.LinkText("Continue");

        public static By AddAssociatedServiceLink => By.LinkText("Add an Associated Service");

        public static By AssociatedServicesTable => ByExtensions.DataTestId("associated-services-table");

        public static By AssociatedServiceDashboardTable => ByExtensions.DataTestId("associated-service-dashboard-table");

        public static By AssociatedServiceRelatedSolutionsTable => ByExtensions.DataTestId("associated-service-related-solutions-table");

        public static By AssociatedServiceRelatedSolutionsInset => ByExtensions.DataTestId("associated-service-related-inset");

        public static By PublicationStatusInputError => By.Id("edit-associated-service-error");

        public static By EditPriceLink(string solutionId) => By.XPath($"//a[contains(@href, '/associated-services/{solutionId}/list-prices')]");
    }
}

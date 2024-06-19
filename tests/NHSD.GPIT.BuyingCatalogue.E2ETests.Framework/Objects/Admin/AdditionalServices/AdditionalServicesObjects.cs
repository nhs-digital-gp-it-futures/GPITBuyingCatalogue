using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.AdditionalServices
{
    public static class AdditionalServicesObjects
    {
        public static By AdditionalServicesTable => ByExtensions.DataTestId("additional-services-table");

        public static By AdditionalServicesTableDashboard => ByExtensions.DataTestId("additional-service-dashboard-table");

        public static By PublicationStatusInputError => By.Id("edit-additional-service-error");

        public static By AddAdditionalServiceLink => By.LinkText("Add an Additional Service");

        public static By EditCapabilitiesLink(string solutionId) => By.XPath($"//a[contains(@href, '/additional-services/{solutionId}/edit-capabilities')]");

        public static By EditPriceLink(string solutionId) => By.XPath($"//a[contains(@href, '/additional-services/{solutionId}/list-prices')]");
    }
}

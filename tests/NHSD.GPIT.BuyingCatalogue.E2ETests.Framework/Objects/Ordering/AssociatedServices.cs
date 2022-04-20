using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class AssociatedServices
    {
        public static By SelectAssociatedServiceErrorMessage => By.Id("select-associated-service-error");

        public static By SelectAssociatedServicePriceErrorMessage => By.Id("select-associated-service-price-error");

        public static By AssociatedServiceEditAssociatedServiceEstimationPeriodErrorMessage => By.Id("edit-associated-service-error");

        public static By AssociatedServicesEditAssociatedServiceDeleteAssociatedServiceLink =>
            By.LinkText("Delete Associated Service");
    }
}

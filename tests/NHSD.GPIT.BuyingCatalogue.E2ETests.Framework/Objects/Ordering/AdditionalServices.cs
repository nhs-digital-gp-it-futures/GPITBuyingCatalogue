using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class AdditionalServices
    {
        public static By SelectAdditionalServiceErrorMessage => By.Id("select-additional-service-error");

        public static By AdditionalServiceRecipientsDateErrorMessage => By.Id("select-additional-service-recipients-date-error");

        public static By SelectAdditionalServicePriceErrorMessage => By.Id("select-additional-service-price-error");

        public static By AddtionalServiceEditAdditionalServiceFirstDateInputErrorMessage => By.Id("edit-additional-service-error");

        public static By AdditionalServicesEditAdditionalServiceDeleteAdditionalServiceLink =>
            By.LinkText("Delete Additional Service");
    }
}

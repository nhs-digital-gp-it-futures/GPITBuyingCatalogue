using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class SupplierInformation
    {
        public static By SupplierSearchInput => By.Id("SearchString");

        public static By SupplierRadioContainer => By.ClassName("nhsuk-radios");

        public static By SupplierRadioErrorMessage => By.Id("supplier-search-select-error");

        public static By SupplierContactRadioErrorMessage => By.Id("supplier-error");

        public static By CreateNewContactLink => By.ClassName("nhsuk-action-link__text");
    }
}

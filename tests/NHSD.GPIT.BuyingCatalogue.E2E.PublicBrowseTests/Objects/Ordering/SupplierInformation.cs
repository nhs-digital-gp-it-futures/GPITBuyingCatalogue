using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class SupplierInformation
    {
        public static By SupplierSearchInput => By.Id("SearchString");

        public static By SupplierRadioContainer => By.ClassName("nhsuk-radios");

        public static By SupplierRadioErrorMessage => By.Id("supplier-search-select-error");

        public static By SupplierName => ByExtensions.DataTestId("supplier-name");

        public static By SupplierAddress => ByExtensions.DataTestId("supplier-address");

        public static By SupplierFirstName => By.Id("PrimaryContact_FirstName");

        public static By SupplierLastName => By.Id("PrimaryContact_LastName");

        public static By SupplierEmail => By.Id("PrimaryContact_Email");

        public static By SupplierPhone => By.Id("PrimaryContact_Phone");
    }
}

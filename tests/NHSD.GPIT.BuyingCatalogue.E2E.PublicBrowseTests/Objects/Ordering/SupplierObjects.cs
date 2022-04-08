using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class SupplierObjects
    {
        public static By SupplierAutoComplete => By.Id("SelectedSupplierId");

        public static By SupplierAutoCompleteError => By.Id("SelectedSupplierId-error");

        public static By SupplierRadioContainer => By.ClassName("nhsuk-radios");

        public static By SupplierContactRadioErrorMessage => By.Id("supplier-error");

        public static By CreateNewContactLink => By.ClassName("nhsuk-action-link__text");

        internal static By SearchListBox => By.Id("SelectedSupplierId__listbox");

        internal static By SearchResultsErrorMessage => new ByChained(SearchListBox, By.ClassName("autocomplete__option--no-results"));

        internal static By SearchResult(uint index) => By.Id($"SelectedSupplierId__option--{index}");
    }
}

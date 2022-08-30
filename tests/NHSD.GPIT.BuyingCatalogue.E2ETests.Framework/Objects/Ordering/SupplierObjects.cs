using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class SupplierObjects
    {
        public static By AssociatedServicesInset => By.Id("associated-services-inset");

        public static By SupplierAutoComplete => By.Id("SelectedSupplierId");

        public static By SupplierAutoCompleteError => By.Id("SelectedSupplierId-error");

        public static By SupplierRadioContainer => By.ClassName("nhsuk-radios");

        public static By SupplierContactRadioErrorMessage => By.Id("supplier-error");

        public static By CreateNewContactLink => By.ClassName("nhsuk-action-link__text");

        public static By SearchListBox => By.Id("SelectedSupplierId__listbox");

        public static By SearchResultsErrorMessage => new ByChained(SearchListBox, By.ClassName("autocomplete__option--no-results"));

        public static By SearchResult(uint index) => By.Id($"SelectedSupplierId__option--{index}");
    }
}

using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ManageSuppliers
{
    public static class ManageSuppliers
    {
        public static By InactiveSuppliersContainer => By.Id("inactive-items-checkbox");

        public static By SuppliersTable => ByExtensions.DataTestId("manage-suppliers-table");

        public static By InactiveSupplierRow => By.ClassName("inactive");

        public static By EditLink => ByExtensions.DataTestId("edit-link");

        public static By SupplierDetailsSupplierName => By.Id("SupplierName");

        public static By SupplierDetailsSupplierNameErrorMessage => By.Id("SupplierName-error");

        public static By SupplierDetailsSupplierLegalName => By.Id("SupplierLegalName");

        public static By SupplierDetailsSupplierLegalNameErrorMessage => By.Id("SupplierLegalName-error");

        public static By SupplierDetailsAboutSupplier => By.Id("AboutSupplier");

        public static By SupplierDetailsSupplierWebsite => By.Id("SupplierWebsite");

        public static By EditSupplierTable => ByExtensions.DataTestId("edit-supplier-table");

        public static By EditSupplierDetailsLink => By.Id("EditSupplierDetailsLink");

        public static By EditSupplierAddressLink => By.Id("EditSupplierAddressLink");

        public static By EditSupplierContactsLink => By.Id("EditSupplierContactsLink");

        public static By EditSupplierSupplierStatusErrorMessage => By.Id("edit-supplier-error");

        public static By EditSupplierAddressStatus => ByExtensions.DataTestId("address-status");

        public static By EditSupplierContactStatus => ByExtensions.DataTestId("contact-status");

        public static By EditSupplierAddressAddressLine1 => By.Id("AddressLine1");

        public static By EditSupplierAddressAddressLine1Error => By.Id("AddressLine1-error");

        public static By EditSupplierAddressAddressLine2 => By.Id("AddressLine2");

        public static By EditSupplierAddressAddressLine3 => By.Id("AddressLine3");

        public static By EditSupplierAddressAddressLine4 => By.Id("AddressLine4");

        public static By EditSupplierAddressAddressLine5 => By.Id("AddressLine5");

        public static By EditSupplierAddressTown => By.Id("Town");

        public static By EditSupplierAddressTownError => By.Id("Town-error");

        public static By EditSupplierAddressCounty => By.Id("County");

        public static By EditSupplierAddressPostcode => By.Id("PostCode");

        public static By EditSupplierAddressPostcodeError => By.Id("PostCode-error");

        public static By EditSupplierAddressCountry => By.Id("Country");

        public static By EditSupplierAddressCountryError => By.Id("Country-error");

        public static By ManageSupplierContactsTable => ByExtensions.DataTestId("contacts-table");

        public static By ManageSupplierContactsEditLink => By.LinkText("Edit");

        public static By ManageSupplierContinueButton => By.LinkText("Continue");

        public static By DeleteSupplierContactCancelLink => By.LinkText("Cancel");

        public static By AddASupplierLink => By.LinkText("Add a supplier");

        public static By SearchBar => By.Id("suppliers-suggestion-search");

        public static By SearchListBox => By.Id("suppliers-suggestion-search__listbox");

        public static By SearchButton => By.ClassName("suggestion-search-search__submit");

        public static By NoResults => By.ClassName("suggestion-search__option--no-results");

        public static By NoResultsElement => By.Id("no-results-search");

        public static By SearchResult(uint index) => By.Id($"suppliers-suggestion-search__option--{index}");

        public static By SearchResultTitle(uint index) => new ByChained(
            SearchResult(index),
            By.ClassName("suggestion-search__option-title"));

        public static By SearchResultDescription(uint index) => new ByChained(
            SearchResult(index),
            By.ClassName("suggestion-search__option-category"));
    }
}

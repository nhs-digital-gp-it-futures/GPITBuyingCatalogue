using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ManageSuppliers
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

        public static By EditSupplierContactFirstName => By.Id("FirstName");

        public static By EditSupplierContactFirstNameError => By.Id("FirstName-error");

        public static By EditSupplierContactLastName => By.Id("LastName");

        public static By EditSupplierContactLastNameError => By.Id("LastName-error");

        public static By EditSupplierContactDepartment => By.Id("Department");

        public static By EditSupplierContactDepartmentError => By.Id("Department-error");

        public static By EditSupplierContactPhoneNumber => By.Id("PhoneNumber");

        public static By EditSupplierContactPhoneNumberError => By.Id("PhoneNumber-error");

        public static By EditSupplierContactEmail => By.Id("Email");

        public static By EditSupplierContactEmailError => By.Id("Email-error");

        public static By EditSupplierContactContainerError => By.Id("edit-contact-error");

        public static By DeleteSupplierContactCancelLink => By.LinkText("Cancel");
    }
}

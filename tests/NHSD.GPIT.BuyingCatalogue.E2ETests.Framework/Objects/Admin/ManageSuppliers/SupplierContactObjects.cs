using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ManageSuppliers
{
    public static class SupplierContactObjects
    {
        public static By FirstNameInput => By.Id("FirstName");

        public static By FirstNameError => By.Id("FirstName-error");

        public static By LastNameInput => By.Id("LastName");

        public static By LastNameError => By.Id("LastName-error");

        public static By DepartmentInput => By.Id("Department");

        public static By PhoneNumberInput => By.Id("PhoneNumber");

        public static By PhoneNumberError => By.Id("PhoneNumber-error");

        public static By EmailInput => By.Id("Email");

        public static By EmailError => By.Id("Email-error");

        public static By ContainerError => By.Id("edit-contact-error");

        public static By ReferencingSolutionsTable => ByExtensions.DataTestId("supplier-contact-related-solutions-table");

        public static By NoReferencingSolutionsInset => ByExtensions.DataTestId("no-related-solutions-inset");

        public static By DeleteLink => By.LinkText("Delete contact");

        public static By AddContact => By.LinkText("Add a contact");
    }
}

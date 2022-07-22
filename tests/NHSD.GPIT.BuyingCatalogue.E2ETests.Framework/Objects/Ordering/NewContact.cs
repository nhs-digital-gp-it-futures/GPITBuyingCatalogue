using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class NewContact
    {
        public static By AddNewContactLink => By.LinkText("Add a new contact");
        public static By FirstNameInput => By.Id("FirstName");

        public static By FirstNameError => By.Id("FirstName-error");

        public static By LastNameInput => By.Id("LastName");

        public static By LastNameError => By.Id("LastName-error");

        public static By DepartmentInput => By.Id("Department");

        public static By PhoneNumberInput => By.Id("PhoneNumber");

        public static By EmailInput => By.Id("Email");

        public static By EmailError => By.Id("Email-error");
    }
}

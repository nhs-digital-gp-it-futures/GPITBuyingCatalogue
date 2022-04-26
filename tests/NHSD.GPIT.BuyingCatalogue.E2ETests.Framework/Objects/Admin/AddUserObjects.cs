using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class AddUserObjects
    {
        public static By ConfirmationTitle => ByExtensions.DataTestId("add-user-confirmation-page-title", "h1");

        public static By AddUserButton => ByExtensions.DataTestId("add-user-button", "button");

        public static By FirstName => By.Id("FirstName");

        public static By FirstNameError => By.Id("FirstName-error");

        public static By LastName => By.Id("LastName");

        public static By LastNameError => By.Id("LastName-error");`www

        public static By Email => By.Id("EmailAddress");

        public static By EmailError => By.Id("EmailAddress-error");
    }
}

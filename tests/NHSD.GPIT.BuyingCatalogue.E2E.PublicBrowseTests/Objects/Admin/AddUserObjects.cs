using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class AddUserObjects
    {
        internal static By ConfirmationTitle => ByExtensions.DataTestId("add-user-confirmation-page-title", "h1");

        internal static By AddUserButton => ByExtensions.DataTestId("add-user-button", "button");

        internal static By FirstName => By.Id("FirstName");

        internal static By FirstNameError => By.Id("FirstName-error");

        internal static By LastName => By.Id("LastName");

        internal static By LastNameError => By.Id("LastName-error");

        internal static By TelephoneNumber => By.Id("TelephoneNumber");

        internal static By TelephoneNumberError => By.Id("TelephoneNumber-error");

        internal static By Email => By.Id("EmailAddress");

        internal static By EmailError => By.Id("EmailAddress-error");
    }
}

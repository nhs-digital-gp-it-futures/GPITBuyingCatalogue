using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class AddUserObjects
    {
        internal static By ConfirmationTitle => ByExtensions.DataTestId("add-user-confirmation-page-title", "h1");

        internal static By AddUserButton => ByExtensions.DataTestId("add-user-button", "button");

        internal static By FirstName => By.Id("FirstName");

        internal static By LastName => By.Id("LastName");

        internal static By TelephoneNumber => By.Id("TelephoneNumber");

        internal static By Email => By.Id("EmailAddress");
    }
}

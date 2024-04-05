using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation
{
    public static class AddUserObjects
    {
        public static By ConfirmationTitle => ByExtensions.DataTestId("add-user-confirmation-page-title", "h1");

        public static By AddUserButton => ByExtensions.DataTestId("add-user-button", "button");

        public static By FirstName => By.Id("FirstName");

        public static By FirstNameError => By.Id("FirstName-error");

        public static By LastName => By.Id("LastName");

        public static By LastNameError => By.Id("LastName-error");

        public static By Email => By.Id("EmailAddress");

        public static By EmailError => By.Id("EmailAddress-error");

        public static By Role => By.Id("selected-account-type");

        public static By RoleError => By.Id("selected-account-type-error");

        public static By Status => By.Id("is-active");

        public static By StatusError => By.Id("is-active-error");

        public static By Organisation => By.Id("SelectedOrganisationId");

        public static By UserEmail => By.Id("Email");
    }
}

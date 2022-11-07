using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation
{
    public static class OrganisationUsersObjects
    {
        public static By AddUserLink => By.LinkText("Add a user");

        public static By CancelLink => By.LinkText("Cancel");

        public static By ContinueLink => By.LinkText("Continue");

        public static By UsersTable => ByExtensions.DataTestId("users-table");

        public static By UsersErrorMessage => ByExtensions.DataTestId("users-error-message");

        public static By UserName => ByExtensions.DataTestId("user-name");

        public static By UserEmail => ByExtensions.DataTestId("user-email");

        public static By UserStatusLink => ByExtensions.DataTestId("user-status-link");
    }
}

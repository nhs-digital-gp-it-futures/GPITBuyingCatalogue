using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Authorization
{
    public static class AuthorizationObjects
    {
        public static By EmailAddress => By.Id("EmailAddress");

        public static By Password => By.Id("Password");

        public static By LoginButton => By.CssSelector("button[type=submit]");

        public static By LogoutLink => ByExtensions.DataTestId("logout-link");
    }
}

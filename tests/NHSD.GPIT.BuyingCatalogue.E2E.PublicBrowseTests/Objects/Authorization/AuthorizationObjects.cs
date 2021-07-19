using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Authorization
{
    internal static class AuthorizationObjects
    {
        internal static By EmailAddress => By.Id("EmailAddress");

        internal static By Password => By.Id("Password");

        internal static By LoginButton => By.CssSelector("button[type=submit]");

        internal static By LogoutLink => ByExtensions.DataTestId("logout-link");
    }
}

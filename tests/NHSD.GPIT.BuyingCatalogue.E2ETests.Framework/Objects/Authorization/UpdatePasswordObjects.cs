using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Authorization
{
    public static class UpdatePasswordObjects
    {

        public static By CurrentPassword => By.Id("CurrentPassword");

        public static By CurrentPasswordError => By.Id("CurrentPassword-error");

        public static By NewPassword => By.Id("NewPassword");

        public static By NewPasswordError => By.Id("NewPassword-error");

        public static By ConfirmPassword => By.Id("ConfirmPassword");

        public static By ConfirmPasswordError => By.Id("ConfirmPassword-error");

        public static By SavePasswordButton => By.CssSelector("button[type=submit]");
    }
}

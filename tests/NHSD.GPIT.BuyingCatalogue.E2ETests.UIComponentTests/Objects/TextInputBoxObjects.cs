using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects
{
    public static class TextInputBoxObjects
    {
        public static By TextBox100Character => (By.Id("TextInput100Character"));
        public static By TextBox500Character => (By.Id("TextInputNoLength"));
        public static By TextInputPassword => (By.Id("TextInputPassword"));
    }
}

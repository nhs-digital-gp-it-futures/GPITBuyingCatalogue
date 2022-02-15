using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects
{
    public sealed class TextAreaObject
    {
        public static By TextBox100Character => (By.Id("TextInput100Character"));
        public static By TextAreaLabel1500Characters => (By.XPath("//label[text()='Defaults to 1500 Characters Maximum']"));
        public static By TextAreaLabelCharacterCountTurnedOff => (By.XPath("//label[text()='Character Count is Turned off']"));
    }
}

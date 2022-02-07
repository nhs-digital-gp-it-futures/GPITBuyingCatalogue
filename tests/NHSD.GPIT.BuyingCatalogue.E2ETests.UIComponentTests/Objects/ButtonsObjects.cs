using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    internal sealed class ButtonsObjects
    {
        internal static By SaveAndContinueButton => By.Id("Submit");
        internal static By SecondaryButton => By.CssSelector("[href ='buttons']");
        internal static By ButtonHeader => By.XPath("//*[@id='maincontent']/div/div/div/div[2]/h1/text()");
    }
}

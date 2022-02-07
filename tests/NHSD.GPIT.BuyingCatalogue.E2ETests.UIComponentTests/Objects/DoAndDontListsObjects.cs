using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects
{
    internal sealed class DoAndDontListsObjects
    {
        internal static By DontList => By.XPath("//*[@id='maincontent']/div/div/div/div[2]/div[3]/h3");
        internal static By DoList => By.XPath("//h3[text()='Do']");
    }
}

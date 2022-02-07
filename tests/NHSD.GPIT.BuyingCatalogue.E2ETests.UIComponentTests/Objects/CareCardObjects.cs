using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects
{
    internal class CareCardObjects
    {
        internal static By NonUrgentAdvice => By.XPath("//*[@id='maincontent']/div/div/div/div[2]/div[2]/div[1]/h3/span/span");
        internal static By UrgentTestCare => By.XPath("//*[@id='maincontent']/div/div/div/div[2]/div[3]/div[1]/h3/span/span");
        internal static By ImmediateCareCardTitle => By.XPath("//*[@id='maincontent']/div/div/div/div[2]/div[4]/div[1]/h3/span/span");
    }
}

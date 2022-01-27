using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects
{
    internal sealed class DateInputObject
    {
        public static By Day => (By.Id("Day"));
        public static By Month => (By.Id("Month"));
        public static By Year => (By.Id("Year"));
    }
}

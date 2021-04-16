using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Actions
{
    internal class ActionBase
    {
        public ActionBase(IWebDriver driver)
        {
            Driver = driver;
        }

        protected IWebDriver Driver;
    }
}

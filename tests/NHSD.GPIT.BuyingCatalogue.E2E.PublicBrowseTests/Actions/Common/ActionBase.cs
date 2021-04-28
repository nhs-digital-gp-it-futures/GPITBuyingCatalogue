using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Common.Actions
{
    internal abstract class ActionBase
    {
        public ActionBase(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        }

        protected IWebDriver Driver;
        protected readonly WebDriverWait Wait;
    }
}

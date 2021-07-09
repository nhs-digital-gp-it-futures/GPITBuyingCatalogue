using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common
{
    internal abstract class ActionBase
    {
        public ActionBase(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        }

        public WebDriverWait Wait { get; protected set; }

        public IWebDriver Driver { get; protected set; }
    }
}

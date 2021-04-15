using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Actions;
using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Utils;
using OpenQA.Selenium;
using System;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Areas.Homepage
{
    public abstract class TestBase
    {
        private readonly Uri uri;

        public TestBase(LocalWebApplicationFactory factory, string urlArea = "")
        {
            this.factory = factory;
            driver = new BrowserFactory("chrome-local").Driver;
            uri = new Uri(factory.RootUri);
            driver.Navigate().GoToUrl(new Uri(uri, urlArea));
            Pages = new Pages(this.driver).PageActions;
        }

        protected readonly LocalWebApplicationFactory factory;
        protected readonly IWebDriver driver;

        internal ActionCollection Pages { get; }
    }
}
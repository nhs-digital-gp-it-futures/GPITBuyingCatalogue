using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Browsers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils
{
    public class WebApplicationConnector
    {
        private const string Browser = "chrome";

        internal IWebDriver Driver { get; }
        public string RootUri { get; }

        public WebApplicationConnector()
        {
            var browserFactory = new BrowserFactory(Browser);
            Driver = browserFactory.Driver;

            RootUri = Environment.GetEnvironmentVariable("RegressionTestUrl");
        }

        ~WebApplicationConnector()
        { 
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            Driver?.Quit();
        }
    }
}

using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Browsers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils
{
    public class WebApplicationConnector
    {
        private const string Browser = "chrome";

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

        public string RootUri { get; }

        internal IWebDriver Driver { get; }

        protected void Dispose(bool disposing)
        {
            Driver?.Quit();
        }
    }
}

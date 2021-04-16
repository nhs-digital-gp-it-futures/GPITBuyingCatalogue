using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using static System.Net.WebRequestMethods;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Utils
{
    internal sealed class BrowserFactory
    {
        private const string DefaultHubUrl = "http://localhost:4444/wd/hub";

        public BrowserFactory(string browser)
        {
            GridRunning = GetGridStatus();
            Driver = GetBrowser(browser);
        }

        public IWebDriver Driver { get; }

        public bool GridRunning { get; private set; }

        private static IWebDriver GetLocalChromeDriver()
        {
            var options = new ChromeOptions();
            options.AddArguments("start-maximized", "no-sandbox", "auto-open-devtools-for-tabs", "ignore-certificate-errors");

            return new ChromeDriver(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), options);
        }

        private static IWebDriver GetChromeDriver(string hubURL)
        {
            var options = new ChromeOptions();
            options.AddArguments("headless", "window-size=1920,1080", "no-sandbox", "ignore-certificate-errors");

            return new RemoteWebDriver(new Uri(hubURL), options);
        }

        private static IWebDriver GetFirefoxDriver(string hubURL)
        {
            var options = new FirefoxOptions();
            options.AddArguments("headless", "window-size=1920,1080", "no-sandbox", "acceptInsecureCerts");

            return new RemoteWebDriver(new Uri(hubURL), options);
        }

        private IWebDriver GetBrowser(string browser)
        {
            IWebDriver driver;

            if (Debugger.IsAttached || !GridRunning)
            {
                driver = GetLocalChromeDriver();
            }
            else
            {
                driver = browser.ToLower() switch
                {
                    "chrome" or "googlechrome" => GetChromeDriver(DefaultHubUrl),
                    "firefox" or "ff" or "mozilla" => GetFirefoxDriver(DefaultHubUrl),
                    _ => GetLocalChromeDriver(),
                };
            }

            return driver;
        }

        private static bool GetGridStatus()
        {
            var requestUri = new Uri("http://localhost:4444/grid/console");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
            request.Method = Http.Get;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                // do something with response.Headers to find out information about the request

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

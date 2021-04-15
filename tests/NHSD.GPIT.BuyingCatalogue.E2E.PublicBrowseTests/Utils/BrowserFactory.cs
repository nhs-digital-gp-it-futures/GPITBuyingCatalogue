using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System;
using System.Diagnostics;
using System.IO;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Utils
{
    internal sealed class BrowserFactory
    {
        private const string DefaultHubUrl = "http://localhost:4444/wd/hub";

        public BrowserFactory(string browser)
        {
            Driver = GetBrowser(browser);
        }

        public IWebDriver Driver { get; }

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

        private static IWebDriver GetBrowser(string browser)
        {
            IWebDriver driver;

            if (Debugger.IsAttached)
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
    }
}

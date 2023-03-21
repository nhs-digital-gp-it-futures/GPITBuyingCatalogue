using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Browsers
{
    public sealed class BrowserFactory : IDisposable
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
            return new ChromeDriver(
                Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory),
                GetChromeOptions(!Debugger.IsAttached));
        }

        private static IWebDriver GetChromeDriver(string hubURL)
        {
            return new RemoteWebDriver(new Uri(hubURL), GetChromeOptions(true));
        }

        private static IWebDriver GetFirefoxDriver(string hubURL)
        {
            var options = new FirefoxOptions();
            options.AddArguments("headless", "window-size=1920,1080", "no-sandbox", "acceptInsecureCerts");

            return new RemoteWebDriver(new Uri(hubURL), options);
        }

        private static bool GetGridStatus()
        {
            var requestUri = new Uri("http://localhost:4444/grid/console");
            using var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri);
            try
            {
                var response = httpClient.SendAsync(httpRequest).GetAwaiter().GetResult();

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private static ChromeOptions GetChromeOptions(bool headless)
        {
            var options = new ChromeOptions();

            if (headless)
            {
                options.AddArguments("headless=new", "window-size=1920,1080", "no-sandbox", "ignore-certificate-errors", "log-level=3");
            }
            else
            {
                options.AddArguments("start-maximized", "no-sandbox", "auto-open-devtools-for-tabs", "ignore-certificate-errors", "log-level=3");
            }

            options.AddUserProfilePreference("download.default_directory", Path.GetTempPath());

            return options;
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

        public void Dispose()
        {
            Driver?.Quit();
            Driver?.Dispose();
        }
    }
}

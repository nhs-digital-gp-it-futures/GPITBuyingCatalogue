using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class AddApplicationTypeObjects
    {
        public static By SupportedBrowserLink(string type) => By.XPath($"//a[contains(@href, '/application-type/browser-based/{type}')]");

        public static By PluginsOrExtensionsLink(string type) => By.XPath($"//a[contains(@href, '/application-type/browser-based/{type}')]");

        public static By ConnectivityAndResolutionLink(string type) => By.XPath($"//a[contains(@href, '/application-type/browser-based/{type}')]");

        public static By HardwareRequirementsLink(string type) => By.XPath($"//a[contains(@href, '/application-type/browser-based/{type}')]");

        public static By AdditionalInformationLink(string type) => By.XPath($"//a[contains(@href, '/application-type/browser-based/{type}')]");

        public static By SupportedOperatingSystemLink(string type) => By.XPath($"//a[contains(@href, '/application-type/mobiletablet/{type}')]");

        public static By ConnectivityLink(string type) => By.XPath($"//a[contains(@href, '/application-type/mobiletablet/{type}')]");

        public static By MemoryAndStorageLink(string type) => By.XPath($"//a[contains(@href, '/application-type/mobiletablet/{type}')]");

        public static By ThirdPartyComponentsLink(string type) => By.XPath($"//a[contains(@href, '/application-type/mobiletablet/{type}')]");

        public static By MobileTabletHardwareRequirementsLink(string type) => By.XPath($"//a[contains(@href, '/application-type/mobiletablet/{type}')]");

        public static By MobileTabletAdditionalInformationLink(string type) => By.XPath($"//a[contains(@href, '/application-type/mobiletablet/{type}')]");

        public static By DesktopOperatingSystemLink(string type) => By.XPath($"//a[contains(@href, '/application-type/desktop/{type}')]");

        public static By DesktopConnectivityLink(string type) => By.XPath($"//a[contains(@href, '/application-type/desktop/{type}')]");

        public static By DesktopMemoryAndStorageLink(string type) => By.XPath($"//a[contains(@href, '/application-type/desktop/{type}')]");

        public static By DesktopThirdPartyComponentsLink(string type) => By.XPath($"//a[contains(@href, '/application-type/desktop/{type}')]");

        public static By DesktopHardwareRequirementsLink(string type) => By.XPath($"//a[contains(@href, '/application-type/desktop/{type}')]");

        public static By DesktopAdditionalInformationLink(string type) => By.XPath($"//a[contains(@href, '/application-type/desktop/{type}')]");

        public static By AdditionalInformation => By.Id("AdditionalInformation");
    }
}

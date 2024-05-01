using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class AddApplicationTypeObjects
    {
        public static By SupportedBrowserLink(string type) => By.XPath($"//a[contains(@href, '/application-type/browser-based/" + type + "')]");

        public static By PluginsOrExtensionsLink(string type) => By.XPath($"//a[contains(@href, '/application-type/browser-based/" + type + "')]");

        public static By ConnectivityAndResolutionLink(string type) => By.XPath($"//a[contains(@href, '/application-type/browser-based/" + type + "')]");

        public static By HardwareRequirementsLink(string type) => By.XPath($"//a[contains(@href, '/application-type/browser-based/" + type + "')]");

        public static By AdditionalInformationLink(string type) => By.XPath($"//a[contains(@href, '/application-type/browser-based/" + type + "')]");

        public static By AdditionalInformation => By.Id("AdditionalInformation");
    }
}

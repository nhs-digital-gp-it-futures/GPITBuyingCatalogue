using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class AddApplicationTypeObjects
    {
        public static By SupportedBrowserLink(string type) => By.XPath($"//a[contains(@href, '/application-type/browser-based/" + type + "')]");
    }
}

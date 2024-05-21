using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing
{
    public static class HostingTypesObjects
    {
        public static By HostingType_Summary => By.Id("Summary");

        public static By HostingType_Link => By.Id("Link");

        public static By HostingType_HostingModel => By.Id("HostingModel");

        public static By DeleteHostingTypeButton => By.LinkText("Delete hosting type");

        public static By DeleteHostingTypeCancelLink => By.LinkText("Cancel");

        public static By HostingTypeEditLink(string type) => By.XPath($"//a[contains(@href, '/hosting-type/hosting-type-" + type + "')]");
    }
}

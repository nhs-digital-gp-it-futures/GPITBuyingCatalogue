using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing
{
    internal static class HostingTypesObjects
    {
        internal static By HostingType_Summary => By.Id("Summary");

        internal static By HostingType_Link => By.Id("Link");

        internal static By HostingType_HostingModel => By.Id("HostingModel");

        internal static By DeleteHostingTypeButton => By.LinkText("Delete hosting type");

        internal static By DeleteHostingTypeCancelLink => By.LinkText("Cancel");
    }
}

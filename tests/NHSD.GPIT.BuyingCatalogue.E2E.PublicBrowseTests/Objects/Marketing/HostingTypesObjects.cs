using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing
{
    internal static class HostingTypesObjects
    {
        internal static By PublicCloud_Summary => By.Id("PublicCloud_Summary");

        internal static By PublicCloud_Link => By.Id("PublicCloud_Link");

        internal static By PrivateCloud_Summary => By.Id("PrivateCloud_Summary");

        internal static By PrivateCloud_Link => By.Id("PrivateCloud_Link");

        internal static By PrivateCloud_HostingModel => By.Id("PrivateCloud_HostingModel");

        internal static By HybridHostingType_Summary => By.Id("HybridHostingType_Summary");

        internal static By HybridHostingType_Link => By.Id("HybridHostingType_Link");

        internal static By HybridHostingType_HostingModel => By.Id("HybridHostingType_HostingModel");

        internal static By OnPremise_Summary => By.Id("OnPremise_Summary");

        internal static By OnPremise_Link => By.Id("OnPremise_Link");

        internal static By OnPremise_HostingModel => By.Id("OnPremise_HostingModel");

        internal static By HSCN_Checkbox => By.CssSelector("input[type=checkbox]");
    }
}

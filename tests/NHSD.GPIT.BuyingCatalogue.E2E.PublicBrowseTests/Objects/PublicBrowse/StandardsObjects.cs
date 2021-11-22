using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    public static class StandardsObjects
    {
        public static By OverarchingTable => ByExtensions.DataTestId("overarching-table");

        public static By InteroperabilityTable => ByExtensions.DataTestId("interoperability-table");

        public static By CapabilityTable => ByExtensions.DataTestId("capability-table");
    }
}

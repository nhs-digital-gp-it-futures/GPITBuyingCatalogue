using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    public static class DevelopmentPlansObjects
    {
        public static By RoadmapTitle => ByExtensions.DataTestId("roadmap-title");

        public static By SupplierRoadmapSection => ByExtensions.DataTestId("supplier-roadmap-section");

        public static By ProgramRoadmapSection => ByExtensions.DataTestId("program-roadmap-section");

        public static By WorkOffPlansSection => ByExtensions.DataTestId("work-off-plans-section");
    }
}

using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse
{
    public static class ManageFilterObjects
    {
        public static By FiltersTable => ByExtensions.DataTestId("filters-table");

        public static By NoFiltersMessage => ByExtensions.DataTestId("no-filters");

        public static By CreateNewFilterLink => By.LinkText("Create a new filter");

        public static By ManageFilterLink => By.LinkText("Go to saved filters");

        public static By FilterName => ByExtensions.DataTestId("filter-name");

        public static By FilterDescription => ByExtensions.DataTestId("filter-description");

        public static By FilterLastUpdated => ByExtensions.DataTestId("filter-last-updated");

        public static By FilterViewLink => ByExtensions.DataTestId("filter-view-link");

        public static By FilterDetailsNameAndDescription => ByExtensions.DataTestId("name-and-description");

        public static By FilterDetailsCapabilities => ByExtensions.DataTestId("capabilities");

        public static By FilterDetailsCapabilitiesAndEpics => ByExtensions.DataTestId("capabilities-and-epics");

        public static By FilterDetailsAdditionalFilters => ByExtensions.DataTestId("additional-filters");

        public static By FilterDetailsViewSolutions => ByExtensions.DataTestId("view-solutions");

        public static By FilterDetailsViewLink => By.LinkText("View results for this filter (opens in a new tab)");

        public static By FilterDetailsDeleteLink => By.LinkText("Delete filter");
    }
}

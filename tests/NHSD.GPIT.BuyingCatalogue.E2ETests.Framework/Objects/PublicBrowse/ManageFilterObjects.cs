using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse
{
    public static class ManageFilterObjects
    {
        public static By FiltersTable => ByExtensions.DataTestId("filters-table");

        public static By NoFiltersMessage => ByExtensions.DataTestId("no-filters");

        public static By CreateNewFilterLink => By.LinkText("Create a new filter");

        public static By FilterName => ByExtensions.DataTestId("filter-name");

        public static By FilterDescription => ByExtensions.DataTestId("filter-description");

        public static By FilterLastUpdated => ByExtensions.DataTestId("filter-last-updated");

        public static By FilterViewLink => ByExtensions.DataTestId("filter-view-link");
    }
}

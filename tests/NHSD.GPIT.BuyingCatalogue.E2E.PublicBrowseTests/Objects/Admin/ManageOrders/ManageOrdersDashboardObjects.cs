using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ManageOrders
{
    internal static class ManageOrdersDashboardObjects
    {
        public static By OrdersTable => ByExtensions.DataTestId("orders-table");

        public static By HomeBreadCrumb => ByExtensions.DataTestId("home-crumb");

        public static By NoOrdersElement => By.Id("no-orders");

        internal static By SearchBar => By.Id("orders-dashboard-autocomplete");

        internal static By SearchListBox => By.Id("orders-dashboard-autocomplete__listbox");

        internal static By SearchButton => By.ClassName("autocomplete-search__submit");

        internal static By NoSearchResults => By.ClassName("autocomplete__option--no-results");

        internal static By NoResultsElement => By.Id("no-results-search");

        internal static By SearchResult(uint index) => By.Id($"orders-dashboard-autocomplete__option--{index}");

        internal static By SearchResultTitle(uint index) => new ByChained(
            SearchResult(index),
            By.ClassName("autocomplete__option-title"));

        internal static By SearchResultDescription(uint index) => new ByChained(
            SearchResult(index),
            By.ClassName("autocomplete__option-category"));
    }
}

using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ManageOrders
{
    public static class ManageOrdersDashboardObjects
    {
        public static By OrdersTable => ByExtensions.DataTestId("orders-table");

        public static By HomeBreadCrumb => ByExtensions.DataTestId("home-crumb");

        public static By NoOrdersElement => By.Id("no-orders");

        public static By SearchBar => By.Id("orders-suggestion-search");

        public static By SearchListBox => By.Id("orders-suggestion-search__listbox");

        public static By SearchButton => By.ClassName("suggestion-search-search__submit");

        public static By NoSearchResults => By.ClassName("suggestion-search__option--no-results");

        public static By NoResultsElement => By.Id("no-results-search");

        public static By SearchResult(uint index) => By.Id($"orders-suggestion-search__option--{index}");

        public static By SearchResultTitle(uint index) => new ByChained(
            SearchResult(index),
            By.ClassName("suggestion-search__option-title"));

        public static By SearchResultDescription(uint index) => new ByChained(
            SearchResult(index),
            By.ClassName("suggestion-search__option-category"));
    }
}

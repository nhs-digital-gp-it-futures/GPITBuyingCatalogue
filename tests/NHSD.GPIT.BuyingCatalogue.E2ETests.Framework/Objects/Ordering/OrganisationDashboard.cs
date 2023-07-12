using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class OrganisationDashboard
    {
        public static By ActOnBehalf => Common.ByExtensions.DataTestId("dashboard-page-proxy-on-behalf");

        public static By CreateOrderLink => By.ClassName("nhsuk-action-link__text");

        public static By CreateManageOrders => By.LinkText("Create or manage orders");

        public static By SearchBar => By.Id("orders-suggestion-search");

        public static By SearchListBox => By.Id("orders-suggestion-search__listbox");

        public static By SearchButton => By.ClassName("suggestion-search-search__submit");

        public static By NoResults => By.ClassName("suggestion-search__option--no-results");

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

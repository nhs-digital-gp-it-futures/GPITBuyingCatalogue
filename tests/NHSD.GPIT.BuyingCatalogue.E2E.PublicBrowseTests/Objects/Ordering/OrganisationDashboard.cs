using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class OrganisationDashboard
    {
        internal static By ActOnBehalf => Common.ByExtensions.DataTestId("dashboard-page-proxy-on-behalf");

        internal static By CreateOrderLink => By.ClassName("nhsuk-action-link__text");

        internal static By SearchBar => By.Id("orders-suggestion-search");

        internal static By SearchListBox => By.Id("orders-suggestion-search__listbox");

        internal static By SearchButton => By.ClassName("suggestion-search-search__submit");

        internal static By NoResults => By.ClassName("suggestion-search__option--no-results");

        internal static By NoResultsElement => By.Id("no-results-search");

        internal static By SearchResult(uint index) => By.Id($"orders-suggestion-search__option--{index}");

        internal static By SearchResultTitle(uint index) => new ByChained(
            SearchResult(index),
            By.ClassName("suggestion-search__option-title"));

        internal static By SearchResultDescription(uint index) => new ByChained(
            SearchResult(index),
            By.ClassName("suggestion-search__option-category"));
    }
}

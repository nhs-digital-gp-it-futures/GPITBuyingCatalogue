using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class OrganisationDashboard
    {
        internal static By CreateOrderLink => By.ClassName("nhsuk-action-link__text");

        internal static By SearchBar => By.Id("orders-dashboard-autocomplete");

        internal static By SearchListBox => By.Id("orders-dashboard-autocomplete__listbox");

        internal static By SearchButton => By.ClassName("autocomplete-search__submit");

        internal static By NoResults => By.ClassName("autocomplete__option--no-results");

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

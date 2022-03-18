using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class SolutionsObjects
    {
        internal static By SolutionList => ByExtensions.DataTestId("solution-cards");

        internal static By SolutionsInList => ByExtensions.DataTestId("solution-card");

        internal static By SolutionName => ByExtensions.DataTestId("solution-card-name");

        internal static By SolutionSupplierName => ByExtensions.DataTestId("solution-card-supplier");

        internal static By SolutionSummary => ByExtensions.DataTestId("solution-card-summary");

        internal static By SolutionCapabilityList => ByExtensions.DataTestId("capability-list");

        internal static By SortByLink => By.Id("sortby-link");

        internal static By SolutionsLink => By.CssSelector("div.nhsuk-grid-column-two-thirds h2 a");

        internal static By CapabilitesOverCountLink => ByExtensions.DataTestId("capabilities-overcount-link");

        internal static By FilterSolutionsExpander => By.CssSelector("#nhsuk-details0 > summary");

        internal static By FilterSolutionsFramework => By.CssSelector("#nhsuk-details1 > summary");

        internal static By FilterCapabilities => By.Id("filter-capabilities-details");

        internal static By SolutionsList => By.Id("solutions-list");

        internal static By SearchCriteriaSummary => ByExtensions.DataTestId("search-criteria-summary");

        internal static By SearchBar => By.Id("marketing-suggestion-search");

        internal static By SearchListBox => By.Id("marketing-suggestion-search__listbox");

        internal static By SearchButton => By.ClassName("suggestion-search-search__submit");

        internal static By NoResults => By.ClassName("suggestion-search__option--no-results");

        internal static By NoResultsElement => By.Id("no-results");

        internal static By SearchResult(uint index) => By.Id($"marketing-suggestion-search__option--{index}");

        internal static By SearchResultTitle(uint index) => new ByChained(
            SearchResult(index),
            By.ClassName("suggestion-search__option-title"));

        internal static By SearchResultDescription(uint index) => new ByChained(
            SearchResult(index),
            By.ClassName("suggestion-search__option-category"));
    }
}

using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse
{
    public static class SolutionsObjects
    {
        public static By SolutionList => ByExtensions.DataTestId("solution-cards");

        public static By SolutionsInList => ByExtensions.DataTestId("solution-card");

        public static By SolutionName => ByExtensions.DataTestId("solution-card-name");

        public static By SolutionSupplierName => ByExtensions.DataTestId("solution-card-supplier");

        public static By SolutionSummary => ByExtensions.DataTestId("solution-card-summary");

        public static By SolutionCapabilityList => ByExtensions.DataTestId("capability-list");

        public static By SortBySelect => By.Id("SelectedSortOption");

        public static By FilterCatalogueSolutionsLink => By.LinkText("Filter Catalogue Solutions");
        public static By StartNewSearch => By.LinkText("Start a new search");
        public static By EditCapabilities => By.LinkText("Edit Capabilities");
        public static By EditEpics => By.LinkText("Edit Epics");

        public static By SolutionsLink => By.CssSelector("div.nhsuk-grid-columns-full h2 a");

        public static By SolutionsList => By.Id("solutions-list");

        public static By SearchCriteriaSummary => ByExtensions.DataTestId("search-criteria-summary");

        public static By SearchTermTitle => ByExtensions.DataTestId("search-term-title");

        public static By SearchTermContent => ByExtensions.DataTestId("search-term-content");

        public static By FilterSummaryTitle => ByExtensions.DataTestId("filter-summary-title");

        public static By FilterSummaryContent => ByExtensions.DataTestId("filter-summary-content");

        public static By SearchBar => By.Id("marketing-suggestion-search");

        public static By SearchListBox => By.Id("marketing-suggestion-search__listbox");

        public static By SearchButton => By.ClassName("suggestion-search-search__submit");

        public static By NoResults => By.ClassName("suggestion-search__option--no-results");

        public static By NoResultsElement => By.Id("no-results");

        public static By SearchResult(uint index) => By.Id($"marketing-suggestion-search__option--{index}");

        public static By SearchResultTitle(uint index) => new ByChained(
            SearchResult(index),
            By.ClassName("suggestion-search__option-title"));

        public static By SearchResultDescription(uint index) => new ByChained(
            SearchResult(index),
            By.ClassName("suggestion-search__option-category"));
    }
}

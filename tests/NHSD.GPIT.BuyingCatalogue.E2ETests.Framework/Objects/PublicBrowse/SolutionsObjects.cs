﻿using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
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

        public static By SortByLink => By.Id("sortby-link");

        public static By SolutionsLink => By.CssSelector("div.nhsuk-grid-column-two-thirds h2 a");

        public static By CapabilitesOverCountLink => ByExtensions.DataTestId("capabilities-overcount-link");

        public static By FilterSolutionsExpander => ByExtensions.DataTestId("filter-solutions-expander");

        public static By FilterSolutionsFramework => ByExtensions.DataTestId("filter-framework-details");

        public static By FilterCapabilities => By.Id("filter-capabilities-details");

        public static By SolutionsList => By.Id("solutions-list");

        public static By SearchCriteriaSummary => ByExtensions.DataTestId("search-criteria-summary");

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

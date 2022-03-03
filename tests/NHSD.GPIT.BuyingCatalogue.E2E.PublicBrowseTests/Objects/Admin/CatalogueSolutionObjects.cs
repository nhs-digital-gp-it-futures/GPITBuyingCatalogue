using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class CatalogueSolutionObjects
    {
        internal static By AddSolutionLink => By.LinkText("Add a solution");

        internal static By SolutionIds => ByExtensions.DataTestId("solution-id");

        internal static By SolutionNames => ByExtensions.DataTestId("solution-name");

        internal static By SupplierNames => ByExtensions.DataTestId("supplier-name");

        internal static By SearchBar => By.Id("solutions-autocomplete");

        internal static By SearchButton => By.ClassName("autocomplete-search__submit");

        internal static By SearchErrorMessage => By.Id("search-error-message");

        internal static By SearchErrorMessageLink => By.Id("reset-search-link");

        internal static By SearchListBox => By.Id("solutions-autocomplete__listbox");

        internal static By SearchResultsErrorMessage => By.ClassName("autocomplete__option--no-results");

        internal static By SearchResult(uint index) => By.Id($"solutions-autocomplete__option--{index}");

        internal static By SearchResultTitle(uint index) => new ByChained(SearchResult(index), By.ClassName("autocomplete__option-title"));

        internal static By SearchResultDescription(uint index) => new ByChained(SearchResult(index), By.ClassName("autocomplete__option-category"));
    }
}

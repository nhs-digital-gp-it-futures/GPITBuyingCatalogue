using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class CatalogueSolutionObjects
    {
        public static By AddSolutionLink => By.LinkText("Add a solution");

        public static By SolutionIds => ByExtensions.DataTestId("solution-id");

        public static By SolutionNames => ByExtensions.DataTestId("solution-name");

        public static By SupplierNames => ByExtensions.DataTestId("supplier-name");

        public static By SearchBar => By.Id("solutions-search");

        public static By SearchButton => By.ClassName("suggestion-search-search__submit");

        public static By SearchErrorMessage => By.Id("search-error-message");

        public static By SearchErrorMessageLink => By.Id("reset-search-link");

        public static By SearchListBox => By.Id("solutions-search__listbox");

        public static By SearchResultsErrorMessage => By.ClassName("suggestion-search__option--no-results");

        public static By SearchResult(uint index) => By.Id($"solutions-search__option--{index}");

        public static By SearchResultTitle(uint index) => new ByChained(SearchResult(index), By.ClassName("suggestion-search__option-title"));

        public static By SearchResultDescription(uint index) => new ByChained(SearchResult(index), By.ClassName("suggestion-search__option-category"));
    }
}

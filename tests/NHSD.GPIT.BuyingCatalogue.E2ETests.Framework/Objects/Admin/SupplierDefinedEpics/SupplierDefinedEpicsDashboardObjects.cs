using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.SupplierDefinedEpics
{
    public static class SupplierDefinedEpicsDashboardObjects
    {
        public static By EpicsTable => ByExtensions.DataTestId("sde-table");

        public static By AddEpicLink => By.LinkText("Add a supplier defined Epic");

        public static By CapabilityNames => ByExtensions.DataTestId("capability-name");

        public static By EpicIds => ByExtensions.DataTestId("epic-link");

        public static By EpicNames => ByExtensions.DataTestId("epic-name");

        public static By SearchBar => By.Id("sde-suggestion-search");

        public static By SearchButton => By.ClassName("suggestion-search-search__submit");

        public static By SearchErrorMessage => By.Id("search-error-message");

        public static By SearchErrorMessageLink => By.Id("reset-search-link");

        public static By SearchListBox => By.Id("sde-suggestion-search__listbox");

        public static By SearchResultsErrorMessage => By.ClassName("suggestion-search__option--no-results");

        public static By SearchResult(uint index) => By.Id($"sde-suggestion-search__option--{index}");

        public static By SearchResultTitle(uint index) => new ByChained(SearchResult(index), By.ClassName("suggestion-search__option-title"));

        public static By SearchResultDescription(uint index) => new ByChained(SearchResult(index), By.ClassName("suggestion-search__option-category"));
    }
}

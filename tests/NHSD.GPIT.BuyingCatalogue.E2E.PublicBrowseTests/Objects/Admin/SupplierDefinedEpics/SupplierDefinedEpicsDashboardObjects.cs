using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.SupplierDefinedEpics
{
    internal static class SupplierDefinedEpicsDashboardObjects
    {
        internal static By EpicsTable => ByExtensions.DataTestId("sde-table");

        internal static By AddEpicLink => By.LinkText("Add a supplier defined Epic");

        internal static By InactiveItemsContainer => By.Id("inactive-items-checkbox");

        internal static By InactiveItemRow => By.ClassName("inactive");

        internal static By CapabilityNames => ByExtensions.DataTestId("capability-name");

        internal static By EpicIds => ByExtensions.DataTestId("epic-link");

        internal static By EpicNames => ByExtensions.DataTestId("epic-name");

        internal static By SearchBar => By.Id("sde-suggestion-search");

        internal static By SearchButton => By.ClassName("suggestion-search-search__submit");

        internal static By SearchErrorMessage => By.Id("search-error-message");

        internal static By SearchErrorMessageLink => By.Id("reset-search-link");

        internal static By SearchListBox => By.Id("sde-suggestion-search__listbox");

        internal static By SearchResultsErrorMessage => By.ClassName("suggestion-search__option--no-results");

        internal static By SearchResult(uint index) => By.Id($"sde-suggestion-search__option--{index}");

        internal static By SearchResultTitle(uint index) => new ByChained(SearchResult(index), By.ClassName("suggestion-search__option-title"));

        internal static By SearchResultDescription(uint index) => new ByChained(SearchResult(index), By.ClassName("suggestion-search__option-category"));
    }
}

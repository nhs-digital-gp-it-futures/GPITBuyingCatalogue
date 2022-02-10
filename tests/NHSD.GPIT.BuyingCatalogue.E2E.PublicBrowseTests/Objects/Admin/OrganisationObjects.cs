using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class OrganisationObjects
    {
        internal static By AddressLines => ByExtensions.DataTestId("org-page-address");

        internal static By OdsCode => ByExtensions.DataTestId("org-page-ods-code");

        internal static By AddUserButton => ByExtensions.DataTestId("add-user-button", "a");

        internal static By UserTable => ByExtensions.DataTestId("user-table", "table");

        internal static By AddOrganisationLink => By.LinkText("Add an organisation");

        internal static By AddRelatedOrgButton => ByExtensions.DataTestId("add-organisation-button", "a");

        internal static By OdsCodes => ByExtensions.DataTestId("org-ods-code");

        internal static By OrganisationLinks => ByExtensions.DataTestId("org-link");

        internal static By OrganisationNames => ByExtensions.DataTestId("org-name");

        internal static By OrganisationsTable => ByExtensions.DataTestId("org-table");

        internal static By RelatedOrgsTable => ByExtensions.DataTestId("related-org-table", "table");

        internal static By RelatedOrganisationRemoveConfirm => ByExtensions.DataTestId("submit-button", "button");

        internal static By SearchBar => By.Id("organisations-autocomplete");

        internal static By SearchButton => By.ClassName("autocomplete-search__submit");

        internal static By SearchErrorMessage => By.Id("search-error-message");

        internal static By SearchErrorMessageLink => By.Id("reset-search-link");

        internal static By SearchListBox => By.Id("organisations-autocomplete__listbox");

        internal static By SearchResultsErrorMessage => By.ClassName("autocomplete__option--no-results");

        internal static By SearchResult(uint index) => By.Id($"organisations-autocomplete__option--{index}");

        internal static By SearchResultTitle(uint index) => new ByChained(SearchResult(index), By.ClassName("autocomplete__option-title"));

        internal static By SearchResultDescription(uint index) => new ByChained(SearchResult(index), By.ClassName("autocomplete__option-category"));

        internal static By RelatedOrgTableOrgName(int orgId) => ByExtensions.DataTestId($"related-org-name-{orgId}");

        internal static By RelatedOrgTableOdsCode(int orgId) => ByExtensions.DataTestId($"related-org-odsCode-{orgId}");

        internal static By RelatedOrganisationRemove(int orgId) => ByExtensions.DataTestId($"related-org-remove-{orgId}");

        internal static By UserName(int id) => ByExtensions.DataTestId($"user-name-{id}");
    }
}

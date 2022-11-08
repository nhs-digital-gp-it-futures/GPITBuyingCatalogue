using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class OrganisationObjects
    {

        public static By UserAccountsLink => By.LinkText("User accounts");

        public static By RelatedOrganisationsLink => By.LinkText("Related organisations");

        public static By AddUserButton => ByExtensions.DataTestId("add-user-button", "a");

        public static By UserTable => ByExtensions.DataTestId("user-table", "table");

        public static By AddOrganisationLink => By.LinkText("Add an organisation");

        public static By ImportPracticeListsButton => By.LinkText("Import practice lists");

        public static By AddRelatedOrgButton => ByExtensions.DataTestId("add-organisation-button", "a");

        public static By OdsCodes => ByExtensions.DataTestId("org-ods-code");

        public static By OrganisationLinks => ByExtensions.DataTestId("org-link");

        public static By OrganisationNames => ByExtensions.DataTestId("org-name");

        public static By OrganisationsTable => ByExtensions.DataTestId("org-table");

        public static By RelatedOrgsTable => ByExtensions.DataTestId("related-org-table", "table");

        public static By RelatedOrganisationRemoveConfirm => ByExtensions.DataTestId("submit-button", "button");

        public static By SearchBar => By.Id("organisations-suggestion-search");

        public static By SearchButton => By.ClassName("suggestion-search-search__submit");

        public static By SearchErrorMessage => By.Id("search-error-message");

        public static By SearchErrorMessageLink => By.Id("reset-search-link");

        public static By SearchListBox => By.Id("organisations-suggestion-search__listbox");

        public static By SearchResultsErrorMessage => By.ClassName("suggestion-search__option--no-results");

        public static By SearchResult(uint index) => By.Id($"organisations-suggestion-search__option--{index}");

        public static By SearchResultTitle(uint index) => new ByChained(SearchResult(index), By.ClassName("suggestion-search__option-title"));

        public static By SearchResultDescription(uint index) => new ByChained(SearchResult(index), By.ClassName("suggestion-search__option-category"));

        public static By RelatedOrgTableOrgName(int orgId) => ByExtensions.DataTestId($"related-org-name-{orgId}");

        public static By RelatedOrgTableOdsCode(int orgId) => ByExtensions.DataTestId($"related-org-externalIdentifier-{orgId}");

        public static By RelatedOrganisationRemove(int orgId) => ByExtensions.DataTestId($"related-org-remove-{orgId}");

        public static By UserName(int id) => ByExtensions.DataTestId($"user-name-{id}");
    }
}

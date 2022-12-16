using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class UserObjects
    {
        public static By AccountTypeRadioButtons => By.Id("selected-account-type");

        public static By AccountTypeRadioButtonsError => By.Id("selected-account-type-error");

        public static By Status => By.Id("is-active");

        public static By StatusError => By.Id("is-active-error");

        public static By AddUserLink => By.LinkText("Add a new user");

        public static By EmailInput => By.Id("Email");

        public static By EmailInputError => By.Id("Email-error");

        public static By FirstNameInput => By.Id("FirstName");

        public static By FirstNameInputError => By.Id("FirstName-error");

        public static By LastNameInput => By.Id("LastName");

        public static By LastNameInputError => By.Id("LastName-error");

        public static By SearchBar => By.Id("users-suggestion-search");

        public static By SearchButton => By.ClassName("suggestion-search-search__submit");

        public static By SearchErrorMessage => By.Id("search-error-message");

        public static By SearchErrorMessageLink => By.Id("reset-search-link");

        public static By SearchListBox => By.Id("users-suggestion-search__listbox");

        public static By SearchResultsErrorMessage => By.ClassName("suggestion-search__option--no-results");

        public static By SelectedOrganisation => By.Id("SelectedOrganisationId");

        public static By SelectedOrganisationError => By.Id("SelectedOrganisationId-error");

        public static By UserEmail => ByExtensions.DataTestId("user-email");

        public static By UserFullName => ByExtensions.DataTestId("user-full-name");

        public static By UserLink => ByExtensions.DataTestId("user-link");

        public static By UserOrganisation => ByExtensions.DataTestId("user-organisation");

        public static By UserAccountType => ByExtensions.DataTestId("user-role");

        public static By UserStatus => ByExtensions.DataTestId("user-status");

        public static By UsersTable => ByExtensions.DataTestId("users-table");

        public static By AutoCompleteResult(uint index) => By.Id($"SelectedOrganisationId__option--{index}");

        public static By SearchResult(uint index) => By.Id($"users-suggestion-search__option--{index}");

        public static By SearchResultTitle(uint index) => new ByChained(SearchResult(index), By.ClassName("suggestion-search__option-title"));

        public static By SearchResultDescription(uint index) => new ByChained(SearchResult(index), By.ClassName("suggestion-search__option-category"));
    }
}

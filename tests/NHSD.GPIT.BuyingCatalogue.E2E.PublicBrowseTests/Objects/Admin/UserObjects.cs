using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class UserObjects
    {
        public static By AccountStatusDisplay => ByExtensions.DataTestId("user-account-status");

        public static By AccountStatusRadioButtons => By.Id("selected-account-status-id");

        public static By AccountStatusRadioButtonsError => By.Id("selected-account-status-id-error");

        public static By AccountTypeDisplay => ByExtensions.DataTestId("user-account-type");

        public static By AccountTypeRadioButtons => By.Id("selected-account-type");

        public static By AccountTypeRadioButtonsError => By.Id("selected-account-type-error");

        public static By AddUserLink => By.LinkText("Add a new user");

        public static By AutoCompleteListBox => By.Id("SelectedOrganisationId__listbox");

        public static By AutoCompleteErrorMessage => new ByChained(AutoCompleteListBox, By.ClassName("autocomplete__option--no-results"));

        public static By EditAccountStatusLink => ByExtensions.DataTestId("edit-account-status-link");

        public static By EditAccountTypeLink => ByExtensions.DataTestId("edit-account-type-link");

        public static By EditOrganisationLink => ByExtensions.DataTestId("edit-organisation-link");

        public static By EditPersonalDetailsLink => ByExtensions.DataTestId("edit-personal-details-link");

        public static By EmailDisplay => ByExtensions.DataTestId("user-email");

        public static By EmailInput => By.Id("Email");

        public static By EmailInputError => By.Id("Email-error");

        public static By FirstNameDisplay => ByExtensions.DataTestId("user-first-name");

        public static By FirstNameInput => By.Id("FirstName");

        public static By FirstNameInputError => By.Id("FirstName-error");

        public static By LastNameDisplay => ByExtensions.DataTestId("user-last-name");

        public static By LastNameInput => By.Id("LastName");

        public static By LastNameInputError => By.Id("LastName-error");

        public static By OrderCallOffId => ByExtensions.DataTestId("order-call-off-id");

        public static By OrderCreated => ByExtensions.DataTestId("order-created");

        public static By OrderDescription => ByExtensions.DataTestId("order-description");

        public static By OrderLink => ByExtensions.DataTestId("order-link");

        public static By OrderStatus => ByExtensions.DataTestId("order-status");

        public static By OrdersErrorMessage => By.Id("orders-error-message");

        public static By OrdersTable => By.Id("orders-table");

        public static By OrganisationDisplay => ByExtensions.DataTestId("user-organisation");

        public static By ResetPasswordLink => By.LinkText("Reset password");

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

        public static By UserStatus => ByExtensions.DataTestId("user-status");

        public static By UsersTable => ByExtensions.DataTestId("users-table");

        public static By AutoCompleteResult(uint index) => By.Id($"SelectedOrganisationId__option--{index}");

        public static By SearchResult(uint index) => By.Id($"users-suggestion-search__option--{index}");

        public static By SearchResultTitle(uint index) => new ByChained(SearchResult(index), By.ClassName("suggestion-search__option-title"));

        public static By SearchResultDescription(uint index) => new ByChained(SearchResult(index), By.ClassName("suggestion-search__option-category"));
    }
}

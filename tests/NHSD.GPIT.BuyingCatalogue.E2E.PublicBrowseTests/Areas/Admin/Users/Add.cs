using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Users
{
    [Collection(nameof(AdminCollection))]
    public class Add : AuthorityTestBase
    {
        private const string NhsDigitalOrganisationName = "NHS ENGLAND - X26";

        public Add(LocalWebApplicationFactory factory)
            : base(factory, typeof(UsersController), nameof(UsersController.Add))
        {
        }

        [Fact]
        public void Add_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.SelectedOrganisation).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.FirstNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.LastNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.EmailInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.AccountTypeRadioButtons).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.Status).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
        }

        [Fact]
        public void Add_ClickGoBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Add_BlankValues_ClickSave_DisplaysErrorMessage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Add)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.SelectedOrganisationError,
                UserDetailsModelValidator.OrganisationMissingErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.FirstNameInputError,
                UserDetailsModelValidator.FirstNameMissingErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.LastNameInputError,
                UserDetailsModelValidator.LastNameMissingErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.EmailInputError,
                UserDetailsModelValidator.EmailMissingErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.AccountTypeRadioButtonsError,
                $"Error: {UserDetailsModelValidator.AccountTypeMissingErrorMessage}").Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.StatusError,
                $"Error: {UserDetailsModelValidator.AccountStatusMissingErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void Add_EmailWrongFormat_ClickSave_DisplaysErrorMessage()
        {
            CommonActions.ElementAddValue(UserObjects.EmailInput, "email address");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Add)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.EmailInputError,
                UserDetailsModelValidator.EmailWrongFormatErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void Add_EmailInUse_ClickSave_DisplaysErrorMessage()
        {
            var context = GetEndToEndDbContext();
            var existingEmailAddress = context.AspNetUsers.First().Email;

            CommonActions.ElementAddValue(UserObjects.EmailInput, existingEmailAddress);

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Add)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.EmailInputError,
                UserDetailsModelValidator.EmailInUseErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void Add_EmailInvalidDomain_ClickSave_DisplaysErrorMessage()
        {
            CommonActions.ElementAddValue(UserObjects.EmailInput, "test@test.com");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Add)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.EmailInputError,
                UserDetailsModelValidator.EmailDomainInvalid).Should().BeTrue();
        }

        [Fact]
        public void Add_Buyer_ClickSave_DisplaysCorrectPage()
        {
            var context = GetEndToEndDbContext();

            var organisationName = context.Organisations
                .First(x => x.Name != NhsDigitalOrganisationName)
                .Name;

            var buyerEmail = Strings.RandomBuyerEmail();

            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, organisationName);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ElementAddValue(UserObjects.FirstNameInput, Strings.RandomString(10));
            CommonActions.ElementAddValue(UserObjects.LastNameInput, Strings.RandomString(10));
            CommonActions.ElementAddValue(UserObjects.EmailInput, buyerEmail);
            CommonActions.ClickRadioButtonWithText("Buyer");
            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();

            RemoveUserByEmail(buyerEmail).GetAwaiter().GetResult();
        }

        [Fact]
        public void AddUser_IncludesWhitespace_WhitespaceRemoved()
        {
            var context = GetEndToEndDbContext();

            var organisationName = context.Organisations
                .First(x => x.Name != NhsDigitalOrganisationName)
                .Name;

            var buyerEmail = Strings.RandomBuyerEmail();

            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, organisationName);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ElementAddValue(UserObjects.FirstNameInput, "    " + Strings.RandomString(10) + "    ");
            CommonActions.ElementAddValue(UserObjects.LastNameInput, "    " + Strings.RandomString(10) + "    ");
            CommonActions.ElementAddValue(UserObjects.EmailInput, "    " + buyerEmail + "    ");
            CommonActions.ClickRadioButtonWithText("Buyer");
            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();

            RemoveUserByEmail(buyerEmail).GetAwaiter().GetResult();
        }

        [Fact]
        public void Add_Admin_NotInNhsDigital_ClickSave_DisplaysErrorMessage()
        {
            var organisationName = GetEndToEndDbContext()
                .Organisations
                .First(x => x.Name != NhsDigitalOrganisationName)
                .Name;

            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, organisationName);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ClickRadioButtonWithText("Admin");
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Add)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.AccountTypeRadioButtonsError,
                $"Error: {UserDetailsModelValidator.MustBelongToNhsDigitalErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void Add_Admin_InNhsDigital_ClickSave_DisplaysCorrectPage()
        {
            var buyerEmail = Strings.RandomBuyerEmail();

            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, NhsDigitalOrganisationName);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ElementAddValue(UserObjects.FirstNameInput, Strings.RandomString(10));
            CommonActions.ElementAddValue(UserObjects.LastNameInput, Strings.RandomString(10));
            CommonActions.ElementAddValue(UserObjects.EmailInput, buyerEmail);
            CommonActions.ClickRadioButtonWithText("Admin");
            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();

            RemoveUserByEmail(buyerEmail).GetAwaiter().GetResult();
        }

        [Fact]
        public void Add_AccountManager_ClickSave_DisplaysCorrectPage()
        {
            var context = GetEndToEndDbContext();

            var organisationName = context.Organisations
                .First(x => x.Name != NhsDigitalOrganisationName)
                .Name;

            var buyerEmail = Strings.RandomBuyerEmail();

            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, organisationName);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ElementAddValue(UserObjects.FirstNameInput, Strings.RandomString(10));
            CommonActions.ElementAddValue(UserObjects.LastNameInput, Strings.RandomString(10));
            CommonActions.ElementAddValue(UserObjects.EmailInput, buyerEmail);
            CommonActions.ClickRadioButtonWithText("Account manager");
            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();

            RemoveUserByEmail(buyerEmail).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddUser_AccountManagerLimit_ClickSave_DisplaysErrorMessage()
        {
            await using var context = GetEndToEndDbContext();

            var organisation = context.Organisations
                .First(x => x.Name != NhsDigitalOrganisationName);
            var user1 = await CreateUser(organisation.Id, accountType: OrganisationFunction.AccountManager.Name);
            var user2 = await CreateUser(organisation.Id, accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, organisation.Name);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ClickRadioButtonWithText("Account manager");
            CommonActions.ClickRadioButtonWithText("Active");
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Add)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.AccountTypeRadioButtonsError,
                $"Error: {UserDetailsModelValidator.MustNotExceedAccountManagerLimit}");

            await RemoveUser(user1);
            await RemoveUser(user2);
        }

        [Fact]
        public async Task AddUser_AccountManagerLimit_Inactive_ClickSave_DisplaysErrorMessage()
        {
            await using var context = GetEndToEndDbContext();

            var organisation = context.Organisations
                .First(x => x.Name != NhsDigitalOrganisationName);
            var user1 = await CreateUser(organisation.Id, accountType: OrganisationFunction.AccountManager.Name);
            var user2 = await CreateUser(organisation.Id, accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, organisation.Name);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ClickRadioButtonWithText("Account manager");
            CommonActions.ClickRadioButtonWithText("Inactive");
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.ElementIsDisplayed(UserObjects.AccountTypeRadioButtonsError).Should().BeFalse();

            await RemoveUser(user1);
            await RemoveUser(user2);
        }

        private async Task RemoveUserByEmail(string email)
        {
            await using var context = GetEndToEndDbContext();
            var user = context.Users.FirstOrDefault(x => x.Email == email);
            if (user == null) return;

            context.Remove(user);
            await context.SaveChangesAsync();
        }

        private async Task<AspNetUser> CreateUser(int organisationId, bool isEnabled = true, string accountType = "Buyer")
        {
            await using var context = GetEndToEndDbContext();
            var user = GenerateUser.GenerateAspNetUser(context, organisationId, DefaultPassword, isEnabled, accountType);
            context.Add(user);
            await context.SaveChangesAsync();
            Driver.Navigate().Refresh();

            return user;
        }

        private async Task RemoveUser(AspNetUser user)
        {
            await using var context = GetEndToEndDbContext();
            var dbUser = context.AspNetUsers.First(x => x.Id == user.Id);

            context.Remove(dbUser);

            await context.SaveChangesAsync();
        }
    }
}

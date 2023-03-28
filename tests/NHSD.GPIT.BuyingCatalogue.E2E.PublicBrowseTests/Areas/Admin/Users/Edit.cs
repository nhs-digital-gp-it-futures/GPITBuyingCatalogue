using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Users
{
    [Collection(nameof(AdminCollection))]
    public class Edit : AuthorityTestBase, IDisposable
    {
        private const int UserId = 5;

        private const string NhsDigitalOrganisationName = "NHS Digital";
        private const string ValidEmailAddress = "a@nhs.net";

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(UserId), UserId.ToString() },
        };

        public Edit(LocalWebApplicationFactory factory)
            : base(factory, typeof(UsersController), nameof(UsersController.Edit), Parameters)
        {
        }

        [Fact]
        public async void Edit_AllElementsDisplayed()
        {
            var user = await GetUser();

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.PageTitle().Should().BeEquivalentTo("Edit user".FormatForComparison());

            CommonActions.ElementIsDisplayed(UserObjects.SelectedOrganisation).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.FirstNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.LastNameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.EmailInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.AccountTypeRadioButtons).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.Status).Should().BeTrue();

            var userRole = user.GetRoleName();
            var organisationFunction = OrganisationFunction.FromName(userRole).Name;

            CommonActions.InputValueEqualTo(UserObjects.SelectedOrganisation, user.PrimaryOrganisation.Name).Should().BeTrue();
            CommonActions.InputValueEqualTo(UserObjects.FirstNameInput, user.FirstName).Should().BeTrue();
            CommonActions.InputValueEqualTo(UserObjects.LastNameInput, user.LastName).Should().BeTrue();
            CommonActions.InputValueEqualTo(UserObjects.EmailInput, user.Email).Should().BeTrue();
            CommonActions.IsRadioButtonChecked(organisationFunction).Should().BeTrue();
            CommonActions.IsRadioButtonChecked((!user.Disabled).ToString()).Should().BeTrue();
        }

        [Fact]
        public void Edit_ClickGoBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Edit_BlankValues_ClickSave_DisplaysErrorMessage()
        {
            CommonActions.ClearInputElement(UserObjects.FirstNameInput);
            CommonActions.ClearInputElement(UserObjects.LastNameInput);
            CommonActions.ClearInputElement(UserObjects.EmailInput);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Edit)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.FirstNameInputError,
                UserDetailsModelValidator.FirstNameMissingErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.LastNameInputError,
                UserDetailsModelValidator.LastNameMissingErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.EmailInputError,
                UserDetailsModelValidator.EmailMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void Edit_EmailWrongFormat_ClickSave_DisplaysErrorMessage()
        {
            CommonActions.ClearInputElement(UserObjects.EmailInput);
            CommonActions.ElementAddValue(UserObjects.EmailInput, "email address");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Edit)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.EmailInputError,
                UserDetailsModelValidator.EmailWrongFormatErrorMessage).Should().BeTrue();
        }

        [Fact]
        public async void Edit_EmailInUse_ClickSave_DisplaysErrorMessage()
        {
            var testUser = await GetUser();
            var user = await CreateUser(testUser.PrimaryOrganisationId);

            CommonActions.ClearInputElement(UserObjects.EmailInput);
            CommonActions.ElementAddValue(UserObjects.EmailInput, user.Email);

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Edit)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.EmailInputError,
                UserDetailsModelValidator.EmailInUseErrorMessage).Should().BeTrue();

            await RemoveUser(user);
        }

        [Fact]
        public void Add_EmailInvalidDomain_ClickSave_DisplaysErrorMessage()
        {
            CommonActions.ClearInputElement(UserObjects.EmailInput);
            CommonActions.ElementAddValue(UserObjects.EmailInput, "test@test.com");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Edit)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.EmailInputError,
                UserDetailsModelValidator.EmailDomainInvalid).Should().BeTrue();
        }

        [Fact]
        public void Edit_ClickSave_DisplaysCorrectPage()
        {
            CommonActions.ClickRadioButtonWithText(OrganisationFunction.AccountManager.DisplayName);
            CommonActions.ClickRadioButtonWithText("Inactive");
            CommonActions.ClearInputElement(UserObjects.EmailInput);
            CommonActions.ElementAddValue(UserObjects.EmailInput, ValidEmailAddress);

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Edit_IncludesWhitespace_WhitespaceRemoved()
        {
            CommonActions.ClearInputElement(UserObjects.FirstNameInput);
            CommonActions.ElementAddValue(UserObjects.FirstNameInput, "    " + Strings.RandomString(10) + "    ");
            CommonActions.ClearInputElement(UserObjects.LastNameInput);
            CommonActions.ElementAddValue(UserObjects.LastNameInput, "    " + Strings.RandomString(10) + "    ");
            CommonActions.ClearInputElement(UserObjects.EmailInput);
            CommonActions.ElementAddValue(UserObjects.EmailInput, "    " + ValidEmailAddress + "    ");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Edit_Admin_NotInNhsDigital_ClickSave_DisplaysErrorMessage()
        {
            var nhsOrgName = GetEndToEndDbContext()
                .Organisations
                .First(x => x.Name != NhsDigitalOrganisationName)
                .Name;

            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, nhsOrgName);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ClickRadioButtonWithText("Admin");
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Edit)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.AccountTypeRadioButtonsError,
                $"Error: {UserDetailsModelValidator.MustBelongToNhsDigitalErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void Edit_Admin_InNhsDigital_ClickSave_DisplaysCorrectPage()
        {
            CommonActions.AutoCompleteAddValue(UserObjects.SelectedOrganisation, NhsDigitalOrganisationName);
            CommonActions.ClickLinkElement(UserObjects.AutoCompleteResult(0));
            CommonActions.ClickRadioButtonWithText("Admin");
            CommonActions.ClearInputElement(UserObjects.EmailInput);
            CommonActions.ElementAddValue(UserObjects.EmailInput, ValidEmailAddress);
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();
        }

        [Fact]
        public async Task EditUser_EditActiveBuyer_AccountManagerLimit_ClickSave_DisplaysErrorMessage()
        {
            var testUser = SetupUser(false, OrganisationFunction.Buyer.Name);

            var user1 = await CreateUser(testUser.PrimaryOrganisationId, accountType: OrganisationFunction.AccountManager.Name);
            var user2 = await CreateUser(testUser.PrimaryOrganisationId, accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.ClickRadioButtonWithText("Account manager");
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Edit)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.AccountTypeRadioButtonsError,
                $"Error: {UserDetailsModelValidator.MustNotExceedAccountManagerLimit}");

            await RemoveUser(user1);
            await RemoveUser(user2);
        }

        [Fact]
        public async Task EditUser_EditActiveAdmin_AccountManagerLimit_ClickSave_DisplaysErrorMessage()
        {
            var testUser = SetupUser(false, OrganisationFunction.Authority.Name);

            var user1 = await CreateUser(testUser.PrimaryOrganisationId, accountType: OrganisationFunction.AccountManager.Name);
            var user2 = await CreateUser(testUser.PrimaryOrganisationId, accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.ClickRadioButtonWithText("Account manager");
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Edit)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.AccountTypeRadioButtonsError,
                $"Error: {UserDetailsModelValidator.MustNotExceedAccountManagerLimit}");

            await RemoveUser(user1);
            await RemoveUser(user2);
        }

        [Fact]
        public async Task EditUser_EditInactiveAccountManager_AccountManagerLimit_ClickSave_DisplaysErrorMessage()
        {
            var testUser = SetupUser(true, OrganisationFunction.AccountManager.Name);

            var user1 = await CreateUser(testUser.PrimaryOrganisationId, accountType: OrganisationFunction.AccountManager.Name);
            var user2 = await CreateUser(testUser.PrimaryOrganisationId, accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.ClickRadioButtonWithText("Active");
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Edit)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.AccountTypeRadioButtonsError,
                $"Error: {UserDetailsModelValidator.MustNotExceedAccountManagerLimit}");

            await RemoveUser(user1);
            await RemoveUser(user2);
        }

        [Fact]
        public async Task EditUser_EditInactiveBuyer_AccountManagerLimit_ClickSave_DisplaysErrorMessage()
        {
            var testUser = SetupUser(true, OrganisationFunction.Buyer.Name);

            var user1 = await CreateUser(testUser.PrimaryOrganisationId, accountType: OrganisationFunction.AccountManager.Name);
            var user2 = await CreateUser(testUser.PrimaryOrganisationId, accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.ClickRadioButtonWithText("Account manager");
            CommonActions.ClickRadioButtonWithText("Active");
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Edit)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.AccountTypeRadioButtonsError,
                $"Error: {UserDetailsModelValidator.MustNotExceedAccountManagerLimit}");

            await RemoveUser(user1);
            await RemoveUser(user2);
        }

        [Fact]
        public async Task EditUser_EditInactiveAdmin_AccountManagerLimit_ClickSave_DisplaysErrorMessage()
        {
            var testUser = SetupUser(true, OrganisationFunction.Authority.Name);

            var user1 = await CreateUser(testUser.PrimaryOrganisationId, accountType: OrganisationFunction.AccountManager.Name);
            var user2 = await CreateUser(testUser.PrimaryOrganisationId, accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.ClickRadioButtonWithText("Account manager");
            CommonActions.ClickRadioButtonWithText("Active");
            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Edit)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.AccountTypeRadioButtonsError,
                $"Error: {UserDetailsModelValidator.MustNotExceedAccountManagerLimit}");

            await RemoveUser(user1);
            await RemoveUser(user2);
        }

        public void Dispose()
        {
            SetupUser(false, OrganisationFunction.AccountManager.Name);
        }

        private async Task<AspNetUser> GetUser()
        {
            await using var context = GetEndToEndDbContext();

            return context.Users.Where(x => x.Id == UserId).Include(x => x.PrimaryOrganisation)
                .Include(x => x.AspNetUserRoles)
                .ThenInclude(x => x.Role).OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToList().FirstOrDefault();
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

        private AspNetUser SetupUser(bool disabled, string roleName)
        {
            using var context = GetEndToEndDbContext();
            var user = context.Users.Where(x => x.Id == UserId)
                .Include(x => x.AspNetUserRoles).First();

            user.Disabled = disabled;

            user.AspNetUserRoles.ForEach(x => user.AspNetUserRoles.Remove(x));

            var role = context.Roles.First(r => r.Name == roleName);
            user.AspNetUserRoles = new List<AspNetUserRole>
            {
                new() { Role = role },
            };

            context.Update(user);
            context.SaveChanges();

            return user;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.AccountManagement
{
    public sealed class EditUser : AccountManagerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const int OrganisationId = 176;
        private const int UserId = 5;
        private const string ValidEmail = "a@nhs.net";
        private const string FirstNameRequired = "Enter a first name";
        private const string LastNameRequired = "Enter a last name";
        private const string EmailAddressRequired = "Enter an email address";
        private const string EmailFormatIncorrect = "Enter an email address in the correct format, like name@example.com";
        private const string EmailAlreadyExists = "A user with this email address is already registered on the Buying Catalogue";
        private const string EmailInvalidDomain =
            "This email domain cannot be used to register a new user account as it is not on the allow list";

        private const string RoleMustBeBuyer =
            "You can only add buyers for this organisation. This is because there are already 2 active account managers which is the maximum allowed.";

        private const string RoleError =
            "There are already 2 active account managers for this organisation which is the maximum allowed";

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
            { nameof(UserId), UserId.ToString() },
        };

        public EditUser(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ManageAccountController),
                  nameof(ManageAccountController.EditUser),
                  Parameters)
        {
        }

        [Fact]
        public async Task EditUser_AllSectionsDisplayed()
        {
            await using var context = GetEndToEndDbContext();

            var organisation = await context.Organisations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == OrganisationId);
            var user = await GetUser();

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.PageTitle().Should().BeEquivalentTo($"Edit user-{organisation.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(AddUserObjects.FirstName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.LastName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.Email).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.Role).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.Status).Should().BeTrue();

            var userRole = user.GetRoleName();
            var organisationFunction = OrganisationFunction.FromName(userRole).Name;

            CommonActions.InputValueEqualTo(AddUserObjects.FirstName, user.FirstName).Should().BeTrue();
            CommonActions.InputValueEqualTo(AddUserObjects.LastName, user.LastName).Should().BeTrue();
            CommonActions.InputValueEqualTo(AddUserObjects.Email, user.Email).Should().BeTrue();
            CommonActions.IsRadioButtonChecked(organisationFunction).Should().BeTrue();
            CommonActions.IsRadioButtonChecked((!user.Disabled).ToString()).Should().BeTrue();
        }

        [Fact]
        public void EditUser_ClickGoBackButton_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Users)).Should().BeTrue();
        }

        [Fact]
        public void EditUser_EditUser_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText(OrganisationFunction.AccountManager.DisplayName);
            CommonActions.ClickRadioButtonWithText("Inactive");
            CommonActions.ClearInputElement(AddUserObjects.Email);
            AccountManagementPages.AddUser.EnterEmailAddress(ValidEmail);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Users)).Should().BeTrue();
        }

        [Fact]
        public void EditUser_IncludesWhitespace_RemovesWhitespace()
        {
            CommonActions.ClearInputElement(AddUserObjects.FirstName);
            AccountManagementPages.AddUser.EnterFirstName("    Alice    ");
            CommonActions.ClearInputElement(AddUserObjects.LastName);
            AccountManagementPages.AddUser.EnterLastName("    Smith    ");
            CommonActions.ClearInputElement(AddUserObjects.Email);
            AccountManagementPages.AddUser.EnterEmailAddress("    " + ValidEmail + "    ");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Users)).Should().BeTrue();
        }

        [Fact]
        public void EditUser_EmptyInput_ThrowsErrors()
        {
            CommonActions.ClearInputElement(AddUserObjects.FirstName);
            CommonActions.ClearInputElement(AddUserObjects.LastName);
            CommonActions.ClearInputElement(AddUserObjects.Email);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.EditUser))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.FirstNameError, FirstNameRequired).Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.LastNameError, LastNameRequired).Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.EmailError, EmailAddressRequired).Should().BeTrue();
        }

        [Fact]
        public void EditUser_EmailIncorrectFormat_ThrowsError()
        {
            CommonActions.ClearInputElement(AddUserObjects.Email);
            AccountManagementPages.AddUser.EnterEmailAddress("test");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.EditUser))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.EmailError, EmailFormatIncorrect).Should().BeTrue();
        }

        [Fact]
        public async Task EditUser_EmailAlreadyExists_ThrowsError()
        {
            var user = await CreateUser();

            CommonActions.ClearInputElement(AddUserObjects.Email);
            AccountManagementPages.AddUser.EnterEmailAddress(user.Email);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.EditUser))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.EmailError, EmailAlreadyExists).Should().BeTrue();

            await RemoveUser(user);
        }

        [Fact]
        public void EditUser_EmailInvalidDomain_ThrowsError()
        {
            CommonActions.ClearInputElement(AddUserObjects.Email);
            AccountManagementPages.AddUser.EnterEmailAddress("test@test.com");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ManageAccountController),
                    nameof(ManageAccountController.EditUser))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.EmailError, EmailInvalidDomain).Should().BeTrue();
        }

        [Fact]
        public async Task EditUser_EditActiveBuyer_AccountManagerLimit_InsetTextDisplayed()
        {
            SetupUser(UserId, false, OrganisationFunction.Buyer.Name);

            var amUser1 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);
            var amUser2 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.ElementIsDisplayed(AddUserObjects.Role).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.NhsInsetText).Should().BeTrue();
            CommonActions.ElementTextContains(CommonSelectors.NhsInsetText, RoleMustBeBuyer).Should().BeTrue();

            await RemoveUser(amUser1);
            await RemoveUser(amUser2);
        }

        [Fact]
        public async Task EditUser_EditInactiveBuyer_AccountManagerLimit_InsetTextNotDisplayed()
        {
            SetupUser(UserId, true, OrganisationFunction.Buyer.Name);

            var amUser1 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);
            var amUser2 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.ElementIsDisplayed(AddUserObjects.Role).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.NhsInsetText).Should().BeFalse();

            await RemoveUser(amUser1);
            await RemoveUser(amUser2);
        }

        [Fact]
        public async Task EditUser_UpdateInactiveBuyerToActiveAccountManager_AccountManagerLimit_ErrorDisplayed()
        {
            SetupUser(UserId, true, OrganisationFunction.Buyer.Name);

            var amUser1 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);
            var amUser2 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.ClickRadioButtonWithText(OrganisationFunction.AccountManager.DisplayName);
            CommonActions.ClickRadioButtonWithText("Active");
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementTextContains(AddUserObjects.RoleError, RoleError).Should().BeTrue();

            await RemoveUser(amUser1);
            await RemoveUser(amUser2);
        }

        [Fact]
        public async Task EditUser_EditInactiveAccountManager_AccountManagerLimit_InsetTextNotDisplayed()
        {
            SetupUser(UserId, true, OrganisationFunction.AccountManager.Name);

            var amUser1 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);
            var amUser2 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.ElementIsDisplayed(AddUserObjects.Role).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.NhsInsetText).Should().BeFalse();

            await RemoveUser(amUser1);
            await RemoveUser(amUser2);
        }

        [Fact]
        public async Task EditUser_ActivateInactiveAccountManager_AccountManagerLimit_ErrorDisplayed()
        {
            SetupUser(UserId, true, OrganisationFunction.AccountManager.Name);

            var amUser1 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);
            var amUser2 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.ClickRadioButtonWithText("Active");
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementTextContains(AddUserObjects.RoleError, RoleError).Should().BeTrue();

            await RemoveUser(amUser1);
            await RemoveUser(amUser2);
        }

        public void Dispose()
        {
            SetupUser(UserId, false, OrganisationFunction.AccountManager.Name);
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

        private async Task<AspNetUser> CreateUser(bool isEnabled = true, string accountType = "Buyer")
        {
            await using var context = GetEndToEndDbContext();
            var user = GenerateUser.GenerateAspNetUser(context, OrganisationId, DefaultPassword, isEnabled, accountType);
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

        private void SetupUser(int userId, bool disabled, string roleName)
        {
            using var context = GetEndToEndDbContext();
            var user = context.Users.Where(x => x.Id == userId)
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
        }
    }
}

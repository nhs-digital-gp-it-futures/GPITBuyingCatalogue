using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers;
using Polly;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.AccountManagement
{
    public sealed class EditUser : AccountManagerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const int OrganisationId = 2;
        private const int UserId = 3;

        private const string FirstNameRequired = "Enter a first name";
        private const string LastNameRequired = "Enter a last name";
        private const string EmailAddressRequired = "Enter an email address";
        private const string EmailFormatIncorrect = "Enter an email address in the correct format, like name@example.com";
        private const string EmailAlreadyExists = "A user with this email address is already registered on the Buying Catalogue";

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
            var organisationFunction = OrganisationFunction.FromName(userRole).DisplayName;

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
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var user = context.Users.Where(x => x.Id == UserId)
                .Include(x => x.AspNetUserRoles).First();

            user.Disabled = false;

            user.AspNetUserRoles.ForEach(x => user.AspNetUserRoles.Remove(x));

            var buyer = context.Roles.First(r => r.Name == OrganisationFunction.Buyer.Name);
            user.AspNetUserRoles = new List<AspNetUserRole>
            {
                new() { Role = buyer },
            };

            context.Update(user);
            context.SaveChanges();
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

        private async Task<AspNetUser> CreateUser(bool isEnabled = true)
        {
            await using var context = GetEndToEndDbContext();
            var user = GenerateUser.GenerateAspNetUser(context, OrganisationId, DefaultPassword, isEnabled);
            context.Add(user);
            await context.SaveChangesAsync();
            Driver.Navigate().Refresh();

            return user;
        }
    }
}

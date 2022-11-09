using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
    public sealed class AddUser : AccountManagerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int OrganisationId = 2;

        private const string FirstNameRequired = "Enter a first name";
        private const string LastNameRequired = "Enter a last name";
        private const string EmailAddressRequired = "Enter an email address";
        private const string EmailFormatIncorrect = "Enter an email address in the correct format, like name@example.com";
        private const string EmailAlreadyExists = "A user with this email address is already registered on the Buying Catalogue";

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
        };

        public AddUser(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ManageAccountController),
                  nameof(ManageAccountController.AddUser),
                  Parameters)
        {
        }

        [Fact]
        public async Task AddUser_AllSectionsDisplayed()
        {
            await using var context = GetEndToEndDbContext();

            var organisation = await context.Organisations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == OrganisationId);

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.PageTitle().Should().BeEquivalentTo($"Add a new user-{organisation.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(AddUserObjects.FirstName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.LastName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.Email).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.Role).Should().BeTrue();
        }

        [Fact]
        public void AddUser_ClickGoBackButton_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Users)).Should().BeTrue();
        }

        [Fact]
        public void AddUser_AddUser_ExpectedResult()
        {
            var user = GenerateUser.Generate();

            AccountManagementPages.AddUser.EnterFirstName(user.FirstName);
            AccountManagementPages.AddUser.EnterLastName(user.LastName);
            AccountManagementPages.AddUser.EnterEmailAddress(user.EmailAddress);
            CommonActions.ClickRadioButtonWithText(OrganisationFunction.Buyer.Name);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Users)).Should().BeTrue();
        }

        [Fact]
        public void AddUser_EmptyInput_ThrowsErrors()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.AddUser))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.FirstNameError, FirstNameRequired).Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.LastNameError, LastNameRequired).Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.EmailError, EmailAddressRequired).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.RoleError).Should().BeTrue();
        }

        [Fact]
        public void AddUser_EmailIncorrectFormat_ThrowsError()
        {
            var user = GenerateUser.Generate();

            AccountManagementPages.AddUser.EnterFirstName(user.FirstName);
            AccountManagementPages.AddUser.EnterLastName(user.LastName);
            AccountManagementPages.AddUser.EnterEmailAddress("test");
            CommonActions.ClickRadioButtonWithText(OrganisationFunction.Buyer.Name);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.AddUser))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.EmailError, EmailFormatIncorrect).Should().BeTrue();
        }

        [Fact]
        public async Task AddUser_EmailAlreadyExists_ThrowsError()
        {
            var user = await CreateUser();

            AccountManagementPages.AddUser.EnterFirstName(user.FirstName);
            AccountManagementPages.AddUser.EnterLastName(user.LastName);
            AccountManagementPages.AddUser.EnterEmailAddress(user.Email);
            CommonActions.ClickRadioButtonWithText(OrganisationFunction.Buyer.Name);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.AddUser))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.EmailError, EmailAlreadyExists).Should().BeTrue();
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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Organisations
{
    [Collection(nameof(AdminCollection))]
    public sealed class AddUser : AuthorityTestBase
    {
        private const int NhsDigitalOrganisationId = 1;
        private const int OrganisationId = 2;
        private const string ValidEmail = "abc@nhs.net";
        private const string FirstNameRequired = "Enter a first name";
        private const string LastNameRequired = "Enter a last name";
        private const string EmailAddressRequired = "Enter an email address";
        private const string EmailFormatIncorrect = "Enter an email address in the correct format, like name@example.com";
        private const string EmailAlreadyExists = "A user with this email address is already registered on the Buying Catalogue";
        private const string EmailInvalidDomain =
            "This email domain cannot be used to register a new user account as it is not on the allow list";

        private const string RoleMustBeBuyer =
            "You can only add buyers for this organisation. This is because there are already 2 active account managers which is the maximum allowed.";

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
        };

        public AddUser(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(OrganisationsController),
                  nameof(OrganisationsController.AddUser),
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
            CommonActions.PageTitle().Should().BeEquivalentTo($"Add user-{organisation!.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(AddUserObjects.FirstName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.LastName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.Email).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.Role).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.Status).Should().BeTrue();
        }

        [Fact]
        public async Task AddUser_NhsDigital_RelevantSectionsDisplayed()
        {
            NavigateToUrl(
                typeof(OrganisationsController),
                nameof(OrganisationsController.AddUser),
                new Dictionary<string, string> { { nameof(OrganisationId), NhsDigitalOrganisationId.ToString() } });

            await using var context = GetEndToEndDbContext();

            var organisation =
                await context.Organisations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == NhsDigitalOrganisationId);

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.PageTitle().Should().BeEquivalentTo($"Add user-{organisation!.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(AddUserObjects.FirstName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.LastName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.Email).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.Role).Should().BeFalse();
            CommonActions.ElementIsDisplayed(AddUserObjects.Status).Should().BeTrue();
        }

        [Fact]
        public void AddUser_ClickGoBackButton_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Users)).Should().BeTrue();
        }

        [Fact]
        public async Task AddUser_AddUser_ExpectedResult()
        {
            var user = GenerateUser.Generate();

            AdminPages.AddUser.EnterFirstName(user.FirstName);
            AdminPages.AddUser.EnterLastName(user.LastName);
            AdminPages.AddUser.EnterEmailAddress(user.EmailAddress);
            CommonActions.ClickRadioButtonWithText(OrganisationFunction.Buyer.Name);
            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Users)).Should().BeTrue();

            await RemoveUserByEmail(user.EmailAddress);
        }

        [Fact]
        public async Task AddUser_IncludesWhitespace_WhitespaceRemoved()
        {
            var user = GenerateUser.Generate();
            user.EmailAddress = ValidEmail;
            AdminPages.AddUser.EnterFirstName("     " + user.FirstName + "     ");
            AdminPages.AddUser.EnterLastName("     " + user.LastName + "     ");
            AdminPages.AddUser.EnterEmailAddress("     " + user.EmailAddress + "     ");
            CommonActions.ClickRadioButtonWithText(OrganisationFunction.Buyer.Name);
            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Users)).Should().BeTrue();

            await RemoveUserByEmail(ValidEmail);
        }

        [Fact]
        public void AddUser_EmptyInput_ThrowsErrors()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.AddUser))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.FirstNameError, FirstNameRequired).Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.LastNameError, LastNameRequired).Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.EmailError, EmailAddressRequired).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.RoleError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.StatusError).Should().BeTrue();
        }

        [Fact]
        public void AddUser_EmailIncorrectFormat_ThrowsError()
        {
            var user = GenerateUser.Generate();

            AdminPages.AddUser.EnterFirstName(user.FirstName);
            AdminPages.AddUser.EnterLastName(user.LastName);
            AdminPages.AddUser.EnterEmailAddress("test");
            CommonActions.ClickRadioButtonWithText(OrganisationFunction.Buyer.Name);
            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.AddUser))
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

            AdminPages.AddUser.EnterFirstName(user.FirstName);
            AdminPages.AddUser.EnterLastName(user.LastName);
            AdminPages.AddUser.EnterEmailAddress(user.Email);
            CommonActions.ClickRadioButtonWithText(OrganisationFunction.Buyer.Name);
            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.AddUser))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.EmailError, EmailAlreadyExists).Should().BeTrue();

            await RemoveUser(user);
        }

        [Fact]
        public void AddUser_EmailInvalidDomain_ThrowsError()
        {
            var user = GenerateUser.Generate();

            AccountManagementPages.AddUser.EnterFirstName(user.FirstName);
            AccountManagementPages.AddUser.EnterLastName(user.LastName);
            AccountManagementPages.AddUser.EnterEmailAddress("email@test.com");
            CommonActions.ClickRadioButtonWithText(OrganisationFunction.Buyer.Name);
            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrganisationsController),
                    nameof(OrganisationsController.AddUser))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(AddUserObjects.EmailError, EmailInvalidDomain).Should().BeTrue();
        }

        [Fact]
        public async Task AddUser_AccountManagerLimit_InsetTextDisplayed()
        {
            await using var context = GetEndToEndDbContext();

            var user1 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);
            var user2 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.ElementIsDisplayed(AddUserObjects.Role).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.NhsInsetText).Should().BeTrue();
            CommonActions.ElementTextContains(CommonSelectors.NhsInsetText, RoleMustBeBuyer).Should().BeTrue();

            await RemoveUser(user1);
            await RemoveUser(user2);
        }

        [Fact]
        public async Task AddUser_AccountManagerLimit_ValidationError_InsetTextDisplayed()
        {
            await using var context = GetEndToEndDbContext();

            var user1 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);
            var user2 = await CreateUser(accountType: OrganisationFunction.AccountManager.Name);

            CommonActions.ClickSave();

            CommonActions.ElementIsDisplayed(AddUserObjects.Role).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.NhsInsetText).Should().BeTrue();
            CommonActions.ElementTextContains(CommonSelectors.NhsInsetText, RoleMustBeBuyer).Should().BeTrue();

            await RemoveUser(user1);
            await RemoveUser(user2);
        }

        [Fact]
        public async Task AddUser_AccountManagerLimit_Inactive_InsetTextNotDisplayed()
        {
            await using var context = GetEndToEndDbContext();

            var user1 = await CreateUser(false, OrganisationFunction.AccountManager.Name);
            var user2 = await CreateUser(false, OrganisationFunction.AccountManager.Name);

            CommonActions.ElementIsDisplayed(AddUserObjects.Role).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.NhsInsetText).Should().BeFalse();

            await RemoveUser(user1);
            await RemoveUser(user2);
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

        private async Task RemoveUserByEmail(string email)
        {
            await using var context = GetEndToEndDbContext();
            var dbUser = context.AspNetUsers.First(x => x.Email == email);

            context.Remove(dbUser);

            await context.SaveChangesAsync();
        }
    }
}

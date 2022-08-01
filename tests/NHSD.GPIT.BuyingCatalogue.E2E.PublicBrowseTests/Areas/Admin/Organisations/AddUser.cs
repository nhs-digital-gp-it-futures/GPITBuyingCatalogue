using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Organisations
{
    public sealed class AddUser : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int OrganisationId = 2;

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

            var organisation = await context.Organisations.AsNoTracking().SingleOrDefaultAsync(o => o.Id == OrganisationId);

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.PageTitle().Should().BeEquivalentTo($"Add a new user-{organisation.Name}".FormatForComparison());

            CommonActions.ElementIsDisplayed(AddUserObjects.FirstName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.LastName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.Email).Should().BeTrue();
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
        public void AddUser_AddUser_ExpectedResult()
        {
            var user = GenerateUser.Generate();

            AdminPages.AddUser.EnterFirstName(user.FirstName);
            AdminPages.AddUser.EnterLastName(user.LastName);
            AdminPages.AddUser.EnterEmailAddress(user.EmailAddress);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Users)).Should().BeTrue();
        }

        [Fact]
        public void AddUser_NoInput_ThrowsErrors()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.AddUser))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddUserObjects.EmailError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.FirstNameError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddUserObjects.LastNameError).Should().BeTrue();
        }

        private async Task<AspNetUser> CreateUser(bool isEnabled = true)
        {
            var user = GenerateUser.GenerateAspNetUser(OrganisationId, DefaultPassword, isEnabled);
            await using var context = GetEndToEndDbContext();
            context.Add(user);
            await context.SaveChangesAsync();
            Driver.Navigate().Refresh();

            return user;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Users
{
    [Collection(nameof(AdminCollection))]
    public class Index : AuthorityTestBase
    {
        public Index(LocalWebApplicationFactory factory)
            : base(factory, typeof(UsersController), nameof(UsersController.Index))
        {
        }

        [Fact]
        public void Index_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.AddUserLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.SearchBar).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.SearchButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.UsersTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.SearchErrorMessage).Should().BeFalse();
            CommonActions.ElementIsDisplayed(UserObjects.SearchErrorMessageLink).Should().BeFalse();
        }

        [Fact]
        public async Task Index_AllUsersDisplayed()
        {
            await VerifyAllUsersDisplayed();
        }

        [Fact]
        public void ClickAddUserLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(UserObjects.AddUserLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Add)).Should().BeTrue();
        }

        [Fact]
        public void ClickEditUserLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(UserObjects.UserLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Edit)).Should().BeTrue();
        }

        [Fact]
        public async Task Index_SearchTermEmpty_AllUsersDisplayed()
        {
            await RunTestWithRetryAsync(async () =>
            {
                CommonActions.ElementAddValue(UserObjects.SearchBar, string.Empty);
                CommonActions.ClickLinkElement(UserObjects.SearchButton);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(UsersController),
                    nameof(UsersController.Index)).Should().BeTrue();

                await VerifyAllUsersDisplayed();
            });
        }

        [Fact]
        public async Task Index_SearchTermValid_FilteredUsersDisplayed()
        {
            await RunTestWithRetryAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();

                var sampleUser = context.AspNetUsers
                    .Include(x => x.PrimaryOrganisation)
                    .First();

                await CommonActions.InputCharactersWithDelay(UserObjects.SearchBar, sampleUser.FullName);
                CommonActions.WaitUntilElementIsDisplayed(UserObjects.SearchListBox);

                CommonActions.ElementExists(UserObjects.SearchResult(0)).Should().BeTrue();
                CommonActions.ElementTextEqualTo(UserObjects.SearchResultTitle(0), sampleUser.FullName).Should().BeTrue();
                CommonActions.ElementTextEqualTo(UserObjects.SearchResultDescription(0), sampleUser.Email).Should().BeTrue();

                CommonActions.ClickLinkElement(UserObjects.SearchButton);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(UsersController),
                    nameof(UsersController.Index)).Should().BeTrue();

                var pageSummary = GetPageSummary();

                pageSummary.EmailAddresses.First().Should().Be(sampleUser.Email);
                pageSummary.FullNames.First().Should().Be(sampleUser.FullName);
                pageSummary.Organisations.First().Should().Be(sampleUser.PrimaryOrganisation.Name);
                pageSummary.Statuses.First().Should().Be(GetAccountStatus(sampleUser.Disabled));
            });
        }

        [Fact]
        public async Task Index_SearchTermValid_NoMatches_ErrorMessageDisplayed()
        {
            await RunTestWithRetryAsync(async () =>
            {
                await CommonActions.InputCharactersWithDelay(UserObjects.SearchBar, Strings.RandomString(10));
                CommonActions.WaitUntilElementIsDisplayed(UserObjects.SearchListBox);

                CommonActions.ElementIsDisplayed(UserObjects.SearchResultsErrorMessage).Should().BeTrue();

                CommonActions.ClickLinkElement(UserObjects.SearchButton);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(UsersController),
                    nameof(UsersController.Index)).Should().BeTrue();

                CommonActions.ElementIsDisplayed(UserObjects.AddUserLink).Should().BeTrue();
                CommonActions.ElementIsDisplayed(UserObjects.SearchBar).Should().BeTrue();
                CommonActions.ElementIsDisplayed(UserObjects.SearchButton).Should().BeTrue();
                CommonActions.ElementIsDisplayed(UserObjects.UsersTable).Should().BeFalse();
                CommonActions.ElementIsDisplayed(UserObjects.SearchErrorMessage).Should().BeTrue();
                CommonActions.ElementIsDisplayed(UserObjects.SearchErrorMessageLink).Should().BeTrue();

                var pageSummary = GetPageSummary();

                pageSummary.EmailAddresses.Should().BeEmpty();
                pageSummary.FullNames.Should().BeEmpty();
                pageSummary.Organisations.Should().BeEmpty();
                pageSummary.Statuses.Should().BeEmpty();

                CommonActions.ClickLinkElement(UserObjects.SearchErrorMessageLink);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(UsersController),
                    nameof(UsersController.Index)).Should().BeTrue();

                CommonActions.ElementIsDisplayed(UserObjects.AddUserLink).Should().BeTrue();
                CommonActions.ElementIsDisplayed(UserObjects.SearchBar).Should().BeTrue();
                CommonActions.ElementIsDisplayed(UserObjects.SearchButton).Should().BeTrue();
                CommonActions.ElementIsDisplayed(UserObjects.UsersTable).Should().BeTrue();
                CommonActions.ElementIsDisplayed(UserObjects.SearchErrorMessage).Should().BeFalse();
                CommonActions.ElementIsDisplayed(UserObjects.SearchErrorMessageLink).Should().BeFalse();
            });
        }

        private static string GetAccountStatus(bool disabled)
        {
            return disabled
                ? ServiceContracts.Enums.AccountStatus.Inactive.ToString()
                : ServiceContracts.Enums.AccountStatus.Active.ToString();
        }

        private PageSummary GetPageSummary() => new()
        {
            EmailAddresses = Driver.FindElements(UserObjects.UserEmail).Select(x => x.Text.Trim()),
            FullNames = Driver.FindElements(UserObjects.UserFullName).Select(x => x.Text.Trim()),
            Organisations = Driver.FindElements(UserObjects.UserOrganisation).Select(x => x.Text.Trim()),
            AccountTypes = Driver.FindElements(UserObjects.UserAccountType).Select(x => x.Text.Trim()),
            Statuses = Driver.FindElements(UserObjects.UserStatus).Select(x => x.Text.Trim()),
        };

        private async Task VerifyAllUsersDisplayed()
        {
            await using var context = GetEndToEndDbContext();

            var users = await context.AspNetUsers
                .Include(x => x.PrimaryOrganisation)
                .Include(x => x.AspNetUserRoles).ThenInclude(y => y.Role)
                .Select(x => new
                {
                    x.Email,
                    x.FullName,
                    OrganisationName = x.PrimaryOrganisation.Name,
                    Status = GetAccountStatus(x.Disabled),
                    AccountType = x.GetDisplayRoleName(),
                }).ToListAsync();

            var pageSummary = GetPageSummary();

            pageSummary.EmailAddresses.Should().BeEquivalentTo(users.Select(x => x.Email)).And.HaveCount(users.Count);
            pageSummary.FullNames.Should().BeEquivalentTo(users.Select(x => x.FullName)).And.HaveCount(users.Count);
            pageSummary.Organisations.Should().BeEquivalentTo(users.Select(x => x.OrganisationName)).And.HaveCount(users.Count);
            pageSummary.Statuses.Should().BeEquivalentTo(users.Select(x => x.Status)).And.HaveCount(users.Count);
            pageSummary.AccountTypes.Should().BeEquivalentTo(users.Select(x => x.AccountType)).And.HaveCount(users.Count);
        }

        private class PageSummary
        {
            public IEnumerable<string> EmailAddresses { get; init; }

            public IEnumerable<string> FullNames { get; init; }

            public IEnumerable<string> Organisations { get; init; }

            public IEnumerable<string> Statuses { get; init; }

            public IEnumerable<string> AccountTypes { get; init; }
        }
    }
}

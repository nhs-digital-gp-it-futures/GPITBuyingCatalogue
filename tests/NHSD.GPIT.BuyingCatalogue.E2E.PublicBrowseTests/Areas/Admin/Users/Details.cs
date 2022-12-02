using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Users
{
    public class Details : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int UserId = 2;
        private const int UserIdWithOrder = 4;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(UserId), $"{UserId}" },
        };

        public Details(LocalWebApplicationFactory factory)
            : base(factory, typeof(UsersController), nameof(UsersController.Details), Parameters)
        {
        }

        [Fact]
        public async Task Details_AllElementsDisplayed()
        {
            await AssertAllFieldsPresent(isUserActive: true);
        }

        [Fact]
        public async Task Details_InactiveUser_AllElementsDisplayed()
        {
            var context = GetEndToEndDbContext();
            var user = await context.Users.FirstAsync(x => x.Id == UserId);

            user.Disabled = true;
            await context.SaveChangesAsync();

            Driver.Navigate().Refresh();

            await AssertAllFieldsPresent(isUserActive: false);

            user.Disabled = false;
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task Details_WithOrder_AllElementsDisplayed()
        {
            var order = await GetOrder(UserIdWithOrder);

            NavigateToUrl(
                typeof(UsersController),
                nameof(UsersController.Details),
                new Dictionary<string, string>
                {
                    { nameof(UserId), $"{UserIdWithOrder}" },
                });

            CommonActions.ElementExists(UserObjects.OrdersErrorMessage).Should().BeFalse();
            CommonActions.ElementIsDisplayed(UserObjects.OrdersTable).Should().BeTrue();

            CommonActions.ElementIsDisplayed(UserObjects.OrderCallOffId).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.OrderDescription).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.OrderCreated).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.OrderStatus).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.OrderLink).Should().BeTrue();

            CommonActions.ElementTextEqualTo(UserObjects.OrderCallOffId, order.CallOffId.ToString()).Should().BeTrue();
            CommonActions.ElementTextEqualTo(UserObjects.OrderDescription, order.Description).Should().BeTrue();
            CommonActions.ElementTextEqualTo(UserObjects.OrderCreated, $"{order.Created:dd/MM/yyyy}").Should().BeTrue();
            CommonActions.ElementTextEqualTo(UserObjects.OrderStatus, order.OrderStatus.ToString()).Should().BeTrue();
        }

        [Fact]
        public void ClickHomeBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void ClickManageUsersBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonObjects.ManageUsersBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Index)).Should().BeTrue();
        }

        [Fact]
        public void ClickEditPersonalDetailsLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(UserObjects.EditPersonalDetailsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.PersonalDetails)).Should().BeTrue();
        }

        [Fact]
        public void ClickEditOrganisationLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(UserObjects.EditOrganisationLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Organisation)).Should().BeTrue();
        }

        [Fact]
        public void ClickEditAccountTypeLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(UserObjects.EditAccountTypeLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.AccountType)).Should().BeTrue();
        }

        [Fact]
        public void ClickEditAccountStatusLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(UserObjects.EditAccountStatusLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.AccountStatus)).Should().BeTrue();
        }

        [Fact]
        public void ClickOrderLink_DisplaysCorrectPage()
        {
            NavigateToUrl(
                typeof(UsersController),
                nameof(UsersController.Details),
                new Dictionary<string, string>
                {
                    { nameof(UserId), $"{UserIdWithOrder}" },
                });

            CommonActions.ClickLinkElement(UserObjects.OrderLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageOrdersController),
                nameof(ManageOrdersController.ViewOrder)).Should().BeTrue();

            CommonActions.ClickLinkElement(CommonSelectors.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();
        }

        private async Task AssertAllFieldsPresent(bool isUserActive)
        {
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonObjects.ManageUsersBreadcrumbLink).Should().BeTrue();

            CommonActions.ElementIsDisplayed(UserObjects.FirstNameDisplay).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.LastNameDisplay).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.EmailDisplay).Should().BeTrue();

            CommonActions.ElementIsDisplayed(UserObjects.OrganisationDisplay).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.AccountTypeDisplay).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.AccountStatusDisplay).Should().BeTrue();

            CommonActions.ElementIsDisplayed(UserObjects.EditPersonalDetailsLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.EditOrganisationLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.EditAccountTypeLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.EditAccountStatusLink).Should().BeTrue();

            CommonActions.ElementExists(UserObjects.OrdersTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(UserObjects.OrdersErrorMessage).Should().BeTrue();

            var user = await GetUser(UserId);
            var userRole = user.AspNetUserRoles.Select(r => r.Role).First().Name;

            var organisationFunction = OrganisationFunction.FromName(userRole).DisplayName;

            var accountStatus = user.Disabled
                ? ServiceContracts.Enums.AccountStatus.Inactive.ToString()
                : ServiceContracts.Enums.AccountStatus.Active.ToString();

            CommonActions.ElementTextEqualTo(UserObjects.FirstNameDisplay, user.FirstName).Should().BeTrue();
            CommonActions.ElementTextEqualTo(UserObjects.LastNameDisplay, user.LastName).Should().BeTrue();
            CommonActions.ElementTextEqualTo(UserObjects.EmailDisplay, user.Email).Should().BeTrue();
            CommonActions.ElementTextEqualTo(UserObjects.OrganisationDisplay, user.PrimaryOrganisation.Name).Should().BeTrue();
            CommonActions.ElementTextEqualTo(UserObjects.AccountTypeDisplay, organisationFunction).Should().BeTrue();
            CommonActions.ElementTextEqualTo(UserObjects.AccountStatusDisplay, accountStatus).Should().BeTrue();
        }

        private async Task<Order> GetOrder(int userId)
        {
            await using var context = GetEndToEndDbContext();

            return await context.Orders
                .Where(x => x.LastUpdatedBy == userId)
                .FirstOrDefaultAsync();
        }

        private async Task<AspNetUser> GetUser(int userId)
        {
            await using var context = GetEndToEndDbContext();

            return await context.AspNetUsers
                .Include(x => x.PrimaryOrganisation)
                .Include(u => u.AspNetUserRoles)
                .ThenInclude(r => r.Role)
                .FirstAsync(x => x.Id == userId);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.AccountManagement
{
    [Collection(nameof(SharedContextCollection))]
    public class Users : AccountManagerTestBase
    {
        private const int OrganisationId = 176;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
        };

        public Users(LocalWebApplicationFactory factory)
            : base(factory, typeof(ManageAccountController), nameof(ManageAccountController.Users), Parameters)
        {
        }

        [Fact]
        public async Task Users_WithUsers_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.OrganisationDetailsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.AddUserLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.UsersTable).Should().BeTrue();
            CommonActions.ElementExists(OrganisationUsersObjects.UsersErrorMessage).Should().BeFalse();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.ContinueLink).Should().BeTrue();

            var user = await GetUser();
            CommonActions.ElementTextEqualTo(OrganisationUsersObjects.UserName, user.GetDisplayName());
            CommonActions.ElementTextEqualTo(OrganisationUsersObjects.UserEmail, user.Email);
            CommonActions.ElementTextEqualTo(OrganisationUsersObjects.UserAccountType, user.GetDisplayRoleName());
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.UserEditLink).Should().BeTrue();
        }

        [Fact]
        public void Users_ClickHomeBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(WebApp.Controllers.HomeController),
                    nameof(WebApp.Controllers.HomeController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Users_ClickOrganisationDetailsBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.OrganisationDetailsBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Details)).Should().BeTrue();
        }

        [Fact]
        public void Users_ClickAddUserLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(OrganisationUsersObjects.AddUserLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.AddUser)).Should().BeTrue();
        }

        [Fact]
        public void Users_ClickEditUserLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(OrganisationUsersObjects.UserEditLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.EditUser)).Should().BeTrue();
        }

        [Fact]
        public void Users_ClickContinueLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(OrganisationUsersObjects.ContinueLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Details)).Should().BeTrue();
        }

        private async Task<AspNetUser> GetUser()
        {
            await using var context = GetEndToEndDbContext();

            return context.Users.Include(x => x.AspNetUserRoles)
                .ThenInclude(x => x.Role)
                .First(x => x.Id == UserSeedData.DaveId);
        }
    }
}

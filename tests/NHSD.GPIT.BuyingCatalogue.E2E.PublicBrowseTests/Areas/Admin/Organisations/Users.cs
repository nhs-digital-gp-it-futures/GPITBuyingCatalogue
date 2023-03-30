using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Organisations
{
    [Collection(nameof(AdminCollection))]
    public class Users : AuthorityTestBase, IDisposable
    {
        private const int OrganisationId = 2;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
        };

        public Users(LocalWebApplicationFactory factory)
            : base(factory, typeof(OrganisationsController), nameof(OrganisationsController.Users), Parameters)
        {
        }

        [Fact]
        public void Users_WithNoUsers_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonObjects.ManageBuyerOrganisationsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.OrganisationDetailsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.AddUserLink).Should().BeTrue();
            CommonActions.ElementExists(OrganisationUsersObjects.UsersTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.UsersErrorMessage).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.ContinueLink).Should().BeTrue();
        }

        [Fact]
        public async Task Users_WithUsers_AllElementsDisplayed()
        {
            var user = await AddUser();

            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonObjects.ManageBuyerOrganisationsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.OrganisationDetailsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.AddUserLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.UsersTable).Should().BeTrue();
            CommonActions.ElementExists(OrganisationUsersObjects.UsersErrorMessage).Should().BeFalse();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.ContinueLink).Should().BeTrue();

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
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Users_ClickManageBuyerOrganisationsBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonObjects.ManageBuyerOrganisationsBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Users_ClickOrganisationDetailsBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.OrganisationDetailsBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Details)).Should().BeTrue();
        }

        [Fact]
        public void Users_ClickAddUserLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(OrganisationUsersObjects.AddUserLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.AddUser)).Should().BeTrue();
        }

        [Fact]
        public async Task Users_ClickUserEditLink_DisplaysCorrectPage()
        {
            await AddUser();

            CommonActions.ClickLinkElement(OrganisationUsersObjects.UserEditLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.EditUser)).Should().BeTrue();
        }

        [Fact]
        public void Users_ClickContinueLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(OrganisationUsersObjects.ContinueLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Details)).Should().BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var users = context.AspNetUsers.Where(x => x.PrimaryOrganisationId == OrganisationId).ToList();
            users.ForEach(x => context.AspNetUsers.Remove(x));
            context.SaveChanges();
        }

        private async Task<AspNetUser> AddUser()
        {
            await using var context = GetEndToEndDbContext();
            var user = GenerateUser.GenerateAspNetUser(context, OrganisationId, DefaultPassword, isEnabled: true);
            context.Add(user);
            await context.SaveChangesAsync();
            Driver.Navigate().Refresh();

            return user;
        }
    }
}

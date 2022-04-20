using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Organisations
{
    public class Users : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
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
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.ManageBuyerOrganisationsBreadcrumbLink).Should().BeTrue();
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
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.ManageBuyerOrganisationsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.OrganisationDetailsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.AddUserLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.UsersTable).Should().BeTrue();
            CommonActions.ElementExists(OrganisationUsersObjects.UsersErrorMessage).Should().BeFalse();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.ContinueLink).Should().BeTrue();

            CommonActions.ElementTextEqualTo(OrganisationUsersObjects.UserName, user.GetDisplayName());
            CommonActions.ElementTextEqualTo(OrganisationUsersObjects.UserEmail, user.Email);
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.UserStatusLink).Should().BeTrue();
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
            CommonActions.ClickLinkElement(BreadcrumbObjects.ManageBuyerOrganisationsBreadcrumbLink);

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
        public async Task Users_ClickUserStatusLink_DisplaysCorrectPage()
        {
            await AddUser();

            CommonActions.ClickLinkElement(OrganisationUsersObjects.UserStatusLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.UserStatus)).Should().BeTrue();
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
            var user = GenerateUser.GenerateAspNetUser(OrganisationId, DefaultPassword, isEnabled: true);
            await using var context = GetEndToEndDbContext();
            context.Add(user);
            await context.SaveChangesAsync();
            Driver.Navigate().Refresh();

            return user;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.AccountManagement
{
    public class UserStatus : AccountManagerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const int OrganisationId = 2;
        private const int UserId = 0;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrganisationId), OrganisationId.ToString() },
            { nameof(UserId), UserId.ToString() },
        };

        public UserStatus(LocalWebApplicationFactory factory)
            : base(factory, typeof(ManageAccountController), nameof(ManageAccountController.UserStatus), Parameters)
        {
        }

        [Fact]
        public async Task UserStatus_AllElementsDisplayed()
        {
            await AddUser();

            CommonActions.ElementIsDisplayed(CommonSelectors.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationUsersObjects.CancelLink).Should().BeTrue();
        }

        [Fact]
        public async Task UserStatus_ClickGoBackLink_DisplaysCorrectPage()
        {
            await AddUser();

            CommonActions.ClickLinkElement(CommonSelectors.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Users)).Should().BeTrue();
        }

        [Fact]
        public async Task UserStatus_ClickCancelLink_DisplaysCorrectPage()
        {
            await AddUser();

            CommonActions.ClickLinkElement(OrganisationUsersObjects.CancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Users)).Should().BeTrue();
        }

        [Fact]
        public async Task UserStatus_ActiveUser_ClickContinue_DisplaysCorrectPage()
        {
            var user = await AddUser(isEnabled: true);

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Users)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var updatedUser = context.AspNetUsers.Single(x => x.Id == user.Id);

            updatedUser.Disabled.Should().BeTrue();
        }

        [Fact]
        public async Task UserStatus_InactiveUser_ClickContinue_DisplaysCorrectPage()
        {
            var user = await AddUser(isEnabled: false);

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageAccountController),
                nameof(ManageAccountController.Users)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var updatedUser = context.AspNetUsers.Single(x => x.Id == user.Id);

            updatedUser.Disabled.Should().BeFalse();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var users = context.AspNetUsers.Where(x => x.PrimaryOrganisationId == OrganisationId).ToList();
            users.ForEach(x => context.AspNetUsers.Remove(x));
            context.SaveChanges();
        }

        private async Task<AspNetUser> AddUser(bool isEnabled = true)
        {
            await using var context = GetEndToEndDbContext();
            var user = GenerateUser.GenerateAspNetUser(context, OrganisationId, DefaultPassword, isEnabled);
            context.Add(user);
            await context.SaveChangesAsync();

            NavigateToUrl(
                typeof(ManageAccountController),
                nameof(ManageAccountController.UserStatus),
                new Dictionary<string, string>
                {
                    { nameof(OrganisationId), $"{OrganisationId}" },
                    { nameof(UserId), $"{user.Id}" },
                });

            return user;
        }
    }
}

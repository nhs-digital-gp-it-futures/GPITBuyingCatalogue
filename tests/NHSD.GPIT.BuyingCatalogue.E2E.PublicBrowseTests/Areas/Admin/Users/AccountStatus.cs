using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Users
{
    public class AccountStatus : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const int UserId = 2;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(UserId),  $"{UserId}" },
        };

        public AccountStatus(LocalWebApplicationFactory factory)
            : base(factory, typeof(UsersController), nameof(UsersController.AccountStatus), Parameters)
        {
        }

        [Fact]
        public void AccountStatus_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.AccountStatusRadioButtons).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();

            var user = GetUser();

            CommonActions.IsRadioButtonChecked("Active").Should().Be(!user.Disabled);
            CommonActions.IsRadioButtonChecked("Inactive").Should().Be(user.Disabled);
        }

        [Fact]
        public void AccountStatus_ClickGoBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();
        }

        [Fact]
        public void AccountStatus_ClickActive_ThenSubmit_SetsValuesAndDisplaysCorrectPage()
        {
            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();

            GetUser().Disabled.Should().BeFalse();
        }

        [Fact]
        public void AccountStatus_ClickInactive_ThenSubmit_SetsValuesAndDisplaysCorrectPage()
        {
            CommonActions.ClickRadioButtonWithText("Inactive");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();

            GetUser().Disabled.Should().BeTrue();
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var user = context.AspNetUsers.Single(x => x.Id == UserId);
            user.Disabled = false;
            context.SaveChanges();
        }

        private AspNetUser GetUser() => GetEndToEndDbContext().AspNetUsers.Single(x => x.Id == UserId);
    }
}

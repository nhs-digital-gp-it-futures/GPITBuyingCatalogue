using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Users
{
    public class AccountType : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const int UserId = 2;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(UserId), $"{UserId}" },
        };

        public AccountType(LocalWebApplicationFactory factory)
            : base(factory, typeof(UsersController), nameof(UsersController.AccountType), Parameters)
        {
        }

        [Fact]
        public void AccountType_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(UserObjects.AccountTypeRadioButtons).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();

            var user = GetUser();
            var userRole = user.AspNetUserRoles.Select(u => u.Role).First().Name;

            CheckRadioButtons(userRole == OrganisationFunction.Authority.Name, userRole == OrganisationFunction.Buyer.Name, userRole == OrganisationFunction.AccountManager.Name);
        }

        [Fact]
        public void AccountType_ClickGoBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();
        }

        [Fact]
        public void AccountType_ClickAdmin_ThenSubmit_SetsValuesAndDisplaysCorrectPage()
        {
            CommonActions.ClickRadioButtonWithText("Admin");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();

            var userRole = GetUser().AspNetUserRoles.Select(u => u.Role).First().Name;

            userRole.Should().Be(OrganisationFunction.Authority.Name);
        }

        [Fact]
        public void AccountType_UserNotInNhsDigital_ClickAdmin_ThenSubmit_DisplaysErrorMessage()
        {
            var context = GetEndToEndDbContext();
            var user = context.AspNetUsers.First(x => x.Id == UserId);
            user.PrimaryOrganisationId = OrganisationConstants.NhsDigitalOrganisationId + 1;
            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ClickRadioButtonWithText("Admin");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.AccountType)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                UserObjects.AccountTypeRadioButtonsError,
                $"Error: {AccountTypeModelValidator.MustBelongToNhsDigitalErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void AccountType_ClickBuyer_ThenSubmit_SetsValuesAndDisplaysCorrectPage()
        {
            CommonActions.ClickRadioButtonWithText("Buyer");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();

            var userRole = GetUser().AspNetUserRoles.Select(u => u.Role).First().Name;
            userRole.Should().Be(OrganisationFunction.Buyer.Name);
        }

        [Fact]
        public void AccountType_ClickAccountManager_ThenSubmit_SetsValuesAndDisplaysCorrectPage()
        {
            CommonActions.ClickRadioButtonWithText("Account Manager");

            CommonActions.ClickLinkElement(CommonSelectors.SubmitButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(UsersController),
                nameof(UsersController.Details)).Should().BeTrue();

            var userRole = GetUser().AspNetUserRoles.Select(u => u.Role).First().Name;
            userRole.Should().Be(OrganisationFunction.AccountManager.Name);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var user = context.AspNetUsers.First(x => x.Id == UserId);
            user.PrimaryOrganisationId = OrganisationConstants.NhsDigitalOrganisationId;
            context.SaveChanges();
        }

        private AspNetUser GetUser() => GetEndToEndDbContext().AspNetUsers.Include(u => u.AspNetUserRoles).ThenInclude(r => r.Role).First(x => x.Id == UserId);

        private void CheckRadioButtons(bool isAuth, bool isBuyer, bool isAM)
        {
            CommonActions.IsRadioButtonChecked(OrganisationFunction.Authority.Name).Should().Be(isAuth);
            CommonActions.IsRadioButtonChecked(OrganisationFunction.Buyer.Name).Should().Be(isBuyer);
            CommonActions.IsRadioButtonChecked(OrganisationFunction.AccountManager.Name).Should().Be(isAM);
        }
    }
}

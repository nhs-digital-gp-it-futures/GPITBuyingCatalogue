using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
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
            var isAdmin = user.OrganisationFunction == OrganisationFunction.AuthorityName;

            CommonActions.IsRadioButtonChecked(OrganisationFunction.AuthorityName).Should().Be(isAdmin);
            CommonActions.IsRadioButtonChecked(OrganisationFunction.BuyerName).Should().Be(!isAdmin);
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

            GetUser().OrganisationFunction.Should().Be(OrganisationFunction.AuthorityName);
        }

        [Fact]
        public void AccountType_UserNotInNhsDigital_ClickAdmin_ThenSubmit_DisplaysErrorMessage()
        {
            var context = GetEndToEndDbContext();
            var user = context.AspNetUsers.Single(x => x.Id == UserId);
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

            GetUser().OrganisationFunction.Should().Be(OrganisationFunction.BuyerName);
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var user = context.AspNetUsers.Single(x => x.Id == UserId);
            user.OrganisationFunction = OrganisationFunction.AuthorityName;
            user.PrimaryOrganisationId = OrganisationConstants.NhsDigitalOrganisationId;
            context.SaveChanges();
        }

        private AspNetUser GetUser() => GetEndToEndDbContext().AspNetUsers.Single(x => x.Id == UserId);
    }
}

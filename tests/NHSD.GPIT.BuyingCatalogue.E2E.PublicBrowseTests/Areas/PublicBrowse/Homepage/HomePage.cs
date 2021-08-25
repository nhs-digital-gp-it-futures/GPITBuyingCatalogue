using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    public sealed class HomePage
        : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly Dictionary<string, string> Parameters = new();

        public HomePage(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(HomeController),
                  nameof(HomeController.Index),
                  Parameters)
        {
        }

        [Fact]
        public void HomePage_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.ActionLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Home.HomeSelectors.ManageOrdersLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Home.HomeSelectors.RequestAccountLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Home.HomeSelectors.NominateOrganisationLink).Should().BeTrue();
        }

        [Fact]
        public void HomePage_ClickBrowseSolutions_ExpectedResult()
        {
            CommonActions.ClickLinkElement(CommonSelectors.ActionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SolutionsController),
                nameof(SolutionsController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void HomePage_ClickManageOrders_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.ManageOrdersLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AccountController),
                nameof(AccountController.Login))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void HomePage_ClickRequestAccount_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.RequestAccountLink);

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(AccountController),
            nameof(AccountController.Registration))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void HomePage_ClickNominateOrganisation_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.NominateOrganisationLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(NominateOrganisationController),
                nameof(NominateOrganisationController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void HomePage_Buyer_ClickManageOrders_ExpectedResult()
        {
            NavigateToUrl(
                typeof(AccountController),
                nameof(AccountController.Login));

            BuyerLogin();

            CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.ManageOrdersLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void HomePage_Authority_ClickManageOrders_ExpectedResult()
        {
            NavigateToUrl(
                typeof(AccountController),
                nameof(AccountController.Login));

            AuthorityLogin();

            CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.ManageOrdersLink);

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(DashboardController),
            nameof(DashboardController.Index))
            .Should()
            .BeTrue();
        }

        public void Dispose()
        {
            NavigateToUrl(
                typeof(AccountController),
                nameof(AccountController.Logout));
        }
    }
}

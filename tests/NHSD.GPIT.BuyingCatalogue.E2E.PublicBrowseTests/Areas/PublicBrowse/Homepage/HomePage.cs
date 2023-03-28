using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;
using Xunit.Abstractions;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class HomePage
        : AnonymousTestBase, IDisposable
    {
        private static readonly Dictionary<string, string> Parameters = new();

        public HomePage(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(HomeController),
                  nameof(HomeController.Index),
                  Parameters,
                  testOutputHelper)
        {
        }

        [Fact]
        public void HomePage_AllSectionsDisplayed()
        {
            RunTest(() =>
            {
                CommonActions.ElementIsDisplayed(CommonSelectors.ActionLink).Should().BeTrue();
                CommonActions.ElementIsDisplayed(Objects.Home.HomeSelectors.ManageOrdersLink).Should().BeTrue();
                CommonActions.ElementIsDisplayed(Objects.Home.HomeSelectors.RequestAccountLink).Should().BeTrue();
                CommonActions.ElementIsDisplayed(Objects.Home.HomeSelectors.NominateOrganisationLink).Should().BeTrue();
            });
        }

        [Fact]
        public void HomePage_ClickBrowseSolutions_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(CommonSelectors.ActionLink);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SolutionsController),
                    nameof(SolutionsController.Index))
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void HomePage_ClickManageOrders_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.ManageOrdersLink);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AccountController),
                    nameof(AccountController.Login))
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void HomePage_ClickRequestAccount_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.RequestAccountLink);

                CommonActions.PageLoadedCorrectGetIndex(
                typeof(RegistrationController),
                nameof(RegistrationController.Index))
                .Should()
                .BeTrue();
            });
        }

        [Fact]
        public void HomePage_ClickNominateOrganisation_ExpectedResult()
        {
            RunTest(() =>
            {
                NavigateToUrl(
                    typeof(AccountController),
                    nameof(AccountController.Login));

                BuyerLogin();

                CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.NominateOrganisationLink);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(NominateOrganisationController),
                    nameof(NominateOrganisationController.Index))
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void HomePage_ClickContactUs_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.ContactUsLink);

                CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.ContactUs))
                .Should()
                .BeTrue();
            });
        }

        [Fact]
        public void HomePage_ClickTechInnovationFramework_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.TechInnovationLink);

                Driver.Url.Should().Be("https://digital.nhs.uk/services/digital-care-services-catalogue/tech-innovation-framework");
            });
        }

        [Fact]
        public void HomePage_ClickDFOCVCframeworkLink_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.DFOCVCframeworkLink);

                Driver.Url.Should().Be("https://digital.nhs.uk/services/digital-care-services-catalogue/digital-first-online-consultation-and-video-consultation-framework");
            });
        }
        [Fact]
        public void HomePage_ClickAdvancedTelephony_ExpectedResult()
        {
                CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.AdvancedTelephonyLink);

                CommonActions.PageLoadedCorrectGetIndex(
                        typeof(HomeController),
                        nameof(HomeController.AdvancedTelephonyBetterPurchaseFramework)).Should().BeTrue();
        }

        [Fact]
        public void HomePage_Buyer_ClickManageOrders_ExpectedResult()
        {
            RunTest(() =>
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
            });
        }

        [Fact]
        public void HomePage_AccountManager_ClickManageOrders_ExpectedResult()
        {
            RunTest(() =>
            {
                NavigateToUrl(
                    typeof(AccountController),
                    nameof(AccountController.Login));

                AccountManagerLogin();

                CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.ManageOrdersLink);

                CommonActions.PageLoadedCorrectGetIndex(
                        typeof(DashboardController),
                        nameof(DashboardController.Organisation))
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void HomePage_Authority_ClickManageOrders_ExpectedResult()
        {
            RunTest(() =>
            {
                NavigateToUrl(
                    typeof(AccountController),
                    nameof(AccountController.Login));

                AuthorityLogin();

                CommonActions.ClickLinkElement(Objects.Home.HomeSelectors.ManageOrdersLink);

                CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.NotAuthorized))
                .Should()
                .BeTrue();

                CommonActions.PageTitle().Should().Be("You're not logged in as a buyer".FormatForComparison());
                CommonActions.LedeText().Should().Be("Only users with a buyer account can create and manage orders on the Buying Catalogue.".FormatForComparison());
            });
        }

        [Fact]
        public void HomePage_Buyer_AdminDashboard_ExpectedResult()
        {
            RunTest(() =>
            {
                NavigateToUrl(
                    typeof(AccountController),
                    nameof(AccountController.Login));

                BuyerLogin();

                NavigateToUrl(
                    typeof(WebApp.Areas.Admin.Controllers.HomeController),
                    nameof(WebApp.Areas.Admin.Controllers.HomeController.Index));

                CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.NotAuthorized))
                .Should()
                .BeTrue();

                CommonActions.PageTitle().Should().Be("You're not logged in as an admin".FormatForComparison());
                CommonActions.LedeText().Should().Be("Only users with an admin account can access this section of the Buying Catalogue.".FormatForComparison());
            });
        }

        [Fact]
        public void HomePage_AccountManager_AdminDashboard_ExpectedResult()
        {
            RunTest(() =>
            {
                NavigateToUrl(
                    typeof(AccountController),
                    nameof(AccountController.Login));

                AccountManagerLogin();

                NavigateToUrl(
                    typeof(WebApp.Areas.Admin.Controllers.HomeController),
                    nameof(WebApp.Areas.Admin.Controllers.HomeController.Index));

                CommonActions.PageLoadedCorrectGetIndex(
                        typeof(HomeController),
                        nameof(HomeController.NotAuthorized))
                    .Should()
                    .BeTrue();

                CommonActions.PageTitle().Should().Be("You're not logged in as an admin".FormatForComparison());
                CommonActions.LedeText().Should().Be("Only users with an admin account can access this section of the Buying Catalogue.".FormatForComparison());
            });
        }

        [Fact]
        public void Hompage_NewTest()
        {
            RunTest(() =>
            {
                CommonActions.ElementIsDisplayed(Objects.Home.HomeSelectors.RequestAccountLink).Should().BeTrue();
            });
        }

        public void Dispose()
        {
            NavigateToUrl(
                typeof(AccountController),
                nameof(AccountController.Logout));
        }
    }
}

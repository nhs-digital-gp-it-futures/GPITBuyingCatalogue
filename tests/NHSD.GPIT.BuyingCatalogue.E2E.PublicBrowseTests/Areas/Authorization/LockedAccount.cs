using System;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Authorization;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.AccountManagement.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Authorization
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class LockedAccount : AnonymousTestBase
    {
        public LockedAccount(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AccountController),
                  nameof(AccountController.LockedAccount),
                  null)
        {
        }

        [Fact]
        public void LockedAccount_FormDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.PageTitle().Should().BeEquivalentTo("Account locked".FormatForComparison());
            AuthorizationPages.LoginActions.HomepageButtonDisplayed();
        }

        [Fact]
        public void LockedAccount_ClickBackBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(AccountController),
                    nameof(AccountController.Login))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void LockedAccount_ClickReturnToHomepageButton_DisplaysCorrectPage()
        {
            AuthorizationPages.LoginActions.ClickHomepageButton();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index))
                .Should()
                .BeTrue();
        }
    }
}

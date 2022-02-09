using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.NominateOrganisation
{
    public sealed class Confirmation : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public Confirmation(LocalWebApplicationFactory factory)
            : base(factory, typeof(NominateOrganisationController), nameof(NominateOrganisationController.Index), null)
        {
        }

        [Fact]
        public void Confirmation_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
        }

        [Fact]
        public void Confirmation_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        public void Dispose()
        {
            NavigateToUrl(
                typeof(AccountController),
                nameof(AccountController.Logout));
        }
    }
}

using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Registration
{
    public sealed class Index : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public Index(LocalWebApplicationFactory factory)
            : base(factory, typeof(RegistrationController), nameof(RegistrationController.Index), null)
        {
        }

        [Fact]
        public void Index_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(RegistrationObjects.RequestAnAccountLink).Should().BeTrue();
        }

        [Fact]
        public void Index_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Index_ClickNominateOrganisationLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(RegistrationObjects.RequestAnAccountLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(RegistrationController),
                nameof(RegistrationController.Details)).Should().BeTrue();
        }
    }
}

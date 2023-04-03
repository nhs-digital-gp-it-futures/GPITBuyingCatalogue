using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Registration
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class Index : AnonymousTestBase
    {
        public Index(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(RegistrationController),
                  nameof(RegistrationController.Index),
                  null,
                  testOutputHelper)
        {
        }

        [Fact]
        public void Index_AllSectionsDisplayed()
        {
            RunTest(() =>
            {
                CommonActions.GoBackLinkDisplayed().Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
                CommonActions.ElementIsDisplayed(RegistrationObjects.RequestAnAccountLink).Should().BeTrue();
            });
        }

        [Fact]
        public void Index_ClickGoBackLink_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickGoBackLink();

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.Index)).Should().BeTrue();
            });
        }

        [Fact]
        public void Index_ClickNominateOrganisationLink_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(RegistrationObjects.RequestAnAccountLink);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(RegistrationController),
                    nameof(RegistrationController.Details)).Should().BeTrue();
            });
        }
    }
}

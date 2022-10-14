using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    public class TechInnovation : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public TechInnovation(LocalWebApplicationFactory factory)
               : base(factory, typeof(HomeController), nameof(HomeController.TechInnovationFramework))
        {
        }

        [Fact]
        public void TechInnovation_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().Be("Tech Innovation framework".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(TechInnovationObjects.HomepageButton).Should().BeTrue();
        }

        [Fact]
        public void TechInnovation_ClickBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void TechInnovation_ClickContactUsCrumb_Redirects()
        {
            CommonActions.ClickLinkElement(TechInnovationObjects.HomepageButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }
    }
}

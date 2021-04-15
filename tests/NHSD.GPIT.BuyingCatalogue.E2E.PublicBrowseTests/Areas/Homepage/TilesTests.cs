using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Areas.Homepage
{
    public sealed class TilesTests : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public TilesTests(LocalWebApplicationFactory factory) 
            :base(factory)
        {
        }

        [Fact]
        internal void TilesTests_GPITFrameworkTileDisplayed()
        {
            Pages.HomePageActions.GpitTileDisplayed().Should().BeTrue();
        }

        [Fact]
        internal void TilesTests_DFOCVCFrameworkTileDisplayed()
        {
            Pages.HomePageActions.DFOCVCTileDisplayed().Should().BeTrue();
        }

        [Fact]
        internal void TilesTests_BuyersGuideTileDisplayed()
        {
            Pages.HomePageActions.BuyersGuideTileDisplayed().Should().BeTrue();
        }

        [Fact]
        internal void TilesTests_OrderFormTileDisplayed()
        {
            Pages.HomePageActions.OrderFormTileDisplayed().Should().BeTrue();
        }

        [Fact]
        internal void TilesTests_ClickBuyersGuideTile()
        {
            Pages.HomePageActions.ClickBuyersGuideTile();
            Pages.CommonActions.PageTitle().Should().ContainEquivalentOf("Buyer’s Guide");
        }

        [Fact]
        internal void TilesTests_ClickDFOCVCTile()
        {
            Pages.HomePageActions.ClickDFOCVCTile();
            Pages.CommonActions.PageTitle().Should().ContainEquivalentOf("DFOCVC framework - results");
        }
    }
}

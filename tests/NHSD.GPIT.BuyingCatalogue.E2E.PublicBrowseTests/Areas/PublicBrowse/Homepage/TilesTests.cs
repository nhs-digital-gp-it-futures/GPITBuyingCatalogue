using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.PublicBrowse.Areas.Homepage
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
            PublicBrowsePages.HomePageActions.GpitTileDisplayed().Should().BeTrue();
        }

        [Fact]
        internal void TilesTests_DFOCVCFrameworkTileDisplayed()
        {
            PublicBrowsePages.HomePageActions.DFOCVCTileDisplayed().Should().BeTrue();
        }

        [Fact]
        internal void TilesTests_BuyersGuideTileDisplayed()
        {
            PublicBrowsePages.HomePageActions.BuyersGuideTileDisplayed().Should().BeTrue();
        }

        [Fact]
        internal void TilesTests_OrderFormTileDisplayed()
        {
            PublicBrowsePages.HomePageActions.OrderFormTileDisplayed().Should().BeTrue();
        }

        [Fact]
        internal void TilesTests_ClickBuyersGuideTile()
        {
            PublicBrowsePages.HomePageActions.ClickBuyersGuideTile();
            PublicBrowsePages.CommonActions.PageTitle().Should().ContainEquivalentOf("Buyer’s Guide");
        }

        [Fact]
        internal void TilesTests_ClickDFOCVCTile()
        {
            PublicBrowsePages.HomePageActions.ClickDFOCVCTile();
            PublicBrowsePages.CommonActions.PageTitle().Should().ContainEquivalentOf("DFOCVC framework - results");
        }
    }
}

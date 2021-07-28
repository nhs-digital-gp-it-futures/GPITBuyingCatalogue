using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    public sealed class TilesTests : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public TilesTests(LocalWebApplicationFactory factory)
            : base(factory)
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
            CommonActions.PageTitle()
                .Should()
                .ContainEquivalentOf("Buyer’s Guide".FormatForComparison());
        }

        [Fact]
        internal void TilesTests_ClickDFOCVCTile()
        {
            PublicBrowsePages.HomePageActions.ClickDFOCVCTile();
            CommonActions.PageTitle()
                .Should()
                .ContainEquivalentOf("DFOCVC framework – results".FormatForComparison());
        }
    }
}

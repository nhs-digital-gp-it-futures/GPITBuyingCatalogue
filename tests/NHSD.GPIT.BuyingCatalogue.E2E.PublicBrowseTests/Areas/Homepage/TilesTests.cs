using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Utils;
using System;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Areas.Homepage
{
    public sealed class TilesTests : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
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

        public void Dispose()
        {
            driver?.Quit();
        }
    }
}

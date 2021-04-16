using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Areas.Solutions
{
    public sealed class DfocvcTests : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public DfocvcTests(LocalWebApplicationFactory factory) 
            : base(factory)
        {
        }

        [Fact]
        public void DFOCVCSolutions_ListOfSolutionsDisplayed()
        {
            Pages.HomePageActions.ClickDFOCVCTile();
            Pages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
        }
    }
}

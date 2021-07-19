using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public class ApplicationTypeDetails : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ApplicationTypeDetails(LocalWebApplicationFactory factory)
            : base(factory, "solutions/futures/99999-001/client-application-types")
        {
        }

        [Theory]
        [InlineData("browser-based application")]
        [InlineData("desktop application")]
        [InlineData("mobile application")]
        public void ApplicationTypeDetails_AllFieldsDisplayed(string rowHeader)
        {
            PublicBrowsePages.SolutionAction.GetTableRowContent(rowHeader).Should().NotBeNullOrEmpty();
        }
    }
}

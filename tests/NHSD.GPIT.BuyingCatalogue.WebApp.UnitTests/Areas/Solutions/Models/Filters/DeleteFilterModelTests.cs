using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models.ManageFilters
{
    public static class DeleteFilterModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_InitializesProperties(string filterName, int filterId)
        {
            var model = new DeleteFilterModel(filterId, filterName);

            model.FilterName.Should().Be(filterName);
            model.FilterId.Should().Be(filterId);
        }
    }
}

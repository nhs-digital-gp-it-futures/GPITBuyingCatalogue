using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.OrganisationModelsTests
{
    public static class FindOrganisationModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            string odsCode)
        {
            var actual = new FindOrganisationModel(odsCode);

            actual.OdsCode.Should().Be(odsCode);
        }
    }
}

using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class FindOrganisationModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            string odsCode)
        {
            var actual = new FindOrganisationModel(odsCode);

            actual.OdsCode.Should().Be(odsCode);
        }
    }
}

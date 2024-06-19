using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.OrganisationModelsTests
{
    public static class AddAnOrganisationErrorModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            string odsCode,
            string error)
        {
            var actual = new AddAnOrganisationErrorModel(odsCode, error);

            actual.OdsCode.Should().Be(odsCode);
            actual.Error.Should().Be(error);
            actual.BackLinkText.Should().Be("Back to find an organisation page");
        }
    }
}

using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.OrganisationModelsTests
{
    public static class ConfirmationModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            string organisationName)
        {
            var actual = new ConfirmationModel(organisationName);

            actual.BackLinkText.Should().Be("Back to dashboard");
            actual.Name.Should().Be(organisationName);
        }
    }
}

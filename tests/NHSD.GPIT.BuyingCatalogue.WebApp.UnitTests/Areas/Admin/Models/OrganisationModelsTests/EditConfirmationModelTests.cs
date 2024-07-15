using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.OrganisationModelsTests
{
    public static class EditConfirmationModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            string organisationName,
            int organisationId)
        {
            var actual = new EditConfirmationModel(organisationName, organisationId);

            actual.Name.Should().Be(organisationName);
            actual.BackLinkText.Should().Be("Back to dashboard");
        }
    }
}

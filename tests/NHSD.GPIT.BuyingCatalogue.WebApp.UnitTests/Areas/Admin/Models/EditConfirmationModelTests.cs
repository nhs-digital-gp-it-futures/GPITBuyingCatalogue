using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class EditConfirmationModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            string organisationName,
            int organisationId)
        {
            var actual = new EditConfirmationModel(organisationName, organisationId);

            actual.Name.Should().Be(organisationName);
            actual.BackLinkText.Should().Be("Back to dashboard");
            actual.BackLink.Should().Be($"/admin/organisations/{organisationId}");
        }
    }
}

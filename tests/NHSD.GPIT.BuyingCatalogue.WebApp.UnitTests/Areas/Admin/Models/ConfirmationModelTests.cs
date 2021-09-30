using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class ConfirmationModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            string organisationName)
        {
            var actual = new ConfirmationModel(organisationName);

            actual.BackLinkText.Should().Be("Back to dashboard");
            actual.BackLink.Should().Be("/admin/buyer-organisations");
            actual.Name.Should().Be(organisationName);
        }
    }
}

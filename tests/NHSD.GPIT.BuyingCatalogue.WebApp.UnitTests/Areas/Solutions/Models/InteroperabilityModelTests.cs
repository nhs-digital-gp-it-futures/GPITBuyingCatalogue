using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class InteroperabilityModelTests
    {
        [Fact]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(InteroperabilityModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Theory]
        [CommonAutoData]
        public static void TextDescriptionsProvided_MultipleIntegrations_ReturnsPluralText(
            Integration integration,
            InteroperabilityModel model)
        {
            model.IM1Integrations = new Integration[] { integration };

            var actual = model.TextDescriptionsProvided();

            actual.Should()
                .Be("IM1 and GP Connect offer integrations specified and assured by the NHS.");
        }

        [Theory]
        [CommonAutoData]
        public static void TextDescriptionsProvided_NoIntegration_ReturnsDefaultText(
            InteroperabilityModel model)
        {
            model.IM1Integrations = new Integration[0];
            model.GpConnectIntegrations = new Integration[0];

            var actual = model.TextDescriptionsProvided();

            actual.Should().Be("No integration yet");
        }

        [Theory]
        [CommonAutoData]
        public static void TextDescriptionsProvided_NullIntegration_ReturnsDefaultText(
            InteroperabilityModel model)
        {
            model.IM1Integrations = null;
            model.GpConnectIntegrations = null;

            var actual = model.TextDescriptionsProvided();

            actual.Should().Be("No integration yet");
        }
    }
}

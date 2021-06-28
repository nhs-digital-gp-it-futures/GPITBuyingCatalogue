using System.Collections.Generic;
using FluentAssertions;
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
            InteroperabilityModel model)
        {
            model.Integrations.Count.Should().BeGreaterThan(1);

            var actual = model.TextDescriptionsProvided();

            actual.Should()
                .Be("IM1 and GP Connect offer integrations specified and assured by the NHS.");
        }

        [Theory]
        [CommonAutoData]
        public static void TextDescriptionsProvided_NoIntegration_ReturnsDefaultText(
            InteroperabilityModel model)
        {
            model.Integrations = new List<IntegrationModel>();

            var actual = model.TextDescriptionsProvided();

            actual.Should().Be("No integration yet");
        }

        [Theory]
        [CommonAutoData]
        public static void TextDescriptionsProvided_NullIntegration_ReturnsDefaultText(
            InteroperabilityModel model)
        {
            model.Integrations = null;

            var actual = model.TextDescriptionsProvided();

            actual.Should().Be("No integration yet");
        }
    }
}

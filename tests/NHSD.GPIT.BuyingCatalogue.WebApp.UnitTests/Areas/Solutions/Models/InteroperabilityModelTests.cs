using System;
using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class InteroperabilityModelTests
    {
        [Test]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(InteroperabilityModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Test, AutoData]
        public static void TextDescriptionsProvided_MultipleIntegrations_ReturnsPluralText(
            InteroperabilityModel model)
        {
            model.Integrations.Count.Should().BeGreaterThan(1);

            var actual = model.TextDescriptionsProvided();

            actual.Should()
                .Be(
                    $"There are {model.Integrations.Count.ToEnglish()} types of integrations specified and assured by the NHS.");
        }
        
        [Test, AutoData]
        public static void TextDescriptionsProvided_OneIntegration_ReturnsSingularText(
            InteroperabilityModel model)
        {
            model.Integrations = new List<IntegrationModel>{model.Integrations[0]};

            var actual = model.TextDescriptionsProvided();

            actual.Should()
                .Be($"There is one type of integration specified and assured by the NHS.");
        }

        [Test, AutoData]
        public static void TextDescriptionsProvided_NoIntegration_ReturnsDefaultText(
            InteroperabilityModel model)
        {
            model.Integrations = new List<IntegrationModel>();

            var actual = model.TextDescriptionsProvided();

            actual.Should().Be($"No integration yet");
        }

        [Test, AutoData]
        public static void TextDescriptionsProvided_NullIntegration_ReturnsDefaultText(
            InteroperabilityModel model)
        {
            model.Integrations = null;

            var actual = model.TextDescriptionsProvided();

            actual.Should().Be($"No integration yet");
        }
    }
}

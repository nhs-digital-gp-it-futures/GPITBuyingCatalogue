using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Competitions;

public static class NonPriceElementExtensionsTests
{
    public static IEnumerable<object[]> HasNonPriceElementTestData => new[]
    {
        new object[] { NonPriceElement.Implementation, null, false },
        new object[]
        {
            NonPriceElement.Interoperability,
            new NonPriceElements { IntegrationTypes = new List<IntegrationType>() },
            false,
        },
        new object[] { NonPriceElement.Implementation, new NonPriceElements { Implementation = null }, false, },
        new object[] { NonPriceElement.ServiceLevel, new NonPriceElements { ServiceLevel = null }, false, },
        new object[]
        {
            NonPriceElement.Interoperability,
            new NonPriceElements { IntegrationTypes = new List<IntegrationType>() { new() } },
            true,
        },
        new object[] { NonPriceElement.Implementation, new NonPriceElements { Implementation = new() }, true, },
        new object[] { NonPriceElement.ServiceLevel, new NonPriceElements { ServiceLevel = new() }, true, },
    };

    public static IEnumerable<object[]> GetNonPriceWeightTestData => new[]
    {
        new object[] { NonPriceElement.Interoperability, new NonPriceElements { NonPriceWeights = new() }, null, },
        new object[] { NonPriceElement.Implementation, new NonPriceElements { NonPriceWeights = new() }, null, },
        new object[] { NonPriceElement.ServiceLevel, new NonPriceElements { NonPriceWeights = new() }, null, },
        new object[]
        {
            NonPriceElement.Interoperability,
            new NonPriceElements { NonPriceWeights = new() { Interoperability = 5 } },
            5,
        },
        new object[]
        {
            NonPriceElement.Implementation,
            new NonPriceElements { NonPriceWeights = new() { Implementation = 2 } },
            2,
        },
        new object[]
        {
            NonPriceElement.ServiceLevel,
            new NonPriceElements { NonPriceWeights = new() { ServiceLevel = 3 } },
            3,
        },
    };

    public static IEnumerable<object[]> HasIncompleteWeightingTestData => new[]
    {
        new object[] { new NonPriceElements(), false, },
        new object[] { new NonPriceElements { NonPriceWeights = new() }, false },
        new object[]
        {
            new NonPriceElements
            {
                IntegrationTypes = new List<IntegrationType>() { new() }, NonPriceWeights = new(),
            },
            true,
        },
        new object[] { new NonPriceElements { Implementation = new(), NonPriceWeights = new(), }, true, },
        new object[] { new NonPriceElements { ServiceLevel = new(), NonPriceWeights = new(), }, true, },
        new object[]
        {
            new NonPriceElements
            {
                IntegrationTypes = new List<IntegrationType>() { new() },
                NonPriceWeights = new() { Interoperability = 5 },
            },
            false,
        },
        new object[]
        {
            new NonPriceElements { Implementation = new(), NonPriceWeights = new() { Implementation = 4 }, },
            false,
        },
        new object[]
        {
            new NonPriceElements { ServiceLevel = new(), NonPriceWeights = new() { ServiceLevel = 2 }, }, false,
        },
    };

    public static IEnumerable<object[]> GetNonPriceElementsTestData => new[]
    {
        new object[] { new NonPriceElements(), Enumerable.Empty<NonPriceElement>(), },
        new object[]
        {
            new NonPriceElements { IntegrationTypes = new List<IntegrationType>() { new() } },
            new List<NonPriceElement> { NonPriceElement.Interoperability },
        },
        new object[]
        {
            new NonPriceElements { Implementation = new() },
            new List<NonPriceElement> { NonPriceElement.Implementation },
        },
        new object[]
        {
            new NonPriceElements { ServiceLevel = new() },
            new List<NonPriceElement> { NonPriceElement.ServiceLevel },
        },
    };

    [Theory]
    [MockAutoData]
    public static void HasNonPriceElement_InvalidValue_ThrowsException(NonPriceElements nonPriceElements) => FluentActions
        .Invoking(() => nonPriceElements.HasNonPriceElement((NonPriceElement)100))
        .Should()
        .Throw<ArgumentOutOfRangeException>();

    [Theory]
    [MockAutoData]
    public static void GetNonPriceWeight_InvalidValue_ThrowsException(NonPriceElements nonPriceElements)
    {
        nonPriceElements.NonPriceWeights = new();

        FluentActions
            .Invoking(() => nonPriceElements.GetNonPriceWeight((NonPriceElement)100))
            .Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [MockMemberAutoData(nameof(HasNonPriceElementTestData))]
    public static void HasNonPriceElement_ReturnsExpected(
        NonPriceElement nonPriceElement,
        NonPriceElements nonPriceElements,
        bool expected) => nonPriceElements.HasNonPriceElement(nonPriceElement).Should().Be(expected);

    [Theory]
    [MockMemberAutoData(nameof(GetNonPriceWeightTestData))]
    public static void GetNonPriceWeight_ReturnsExpected(
        NonPriceElement nonPriceElement,
        NonPriceElements nonPriceElements,
        int? expected) => nonPriceElements.GetNonPriceWeight(nonPriceElement).Should().Be(expected);

    [Theory]
    [MockMemberAutoData(nameof(HasIncompleteWeightingTestData))]
    public static void HasIncompleteWeighting_ReturnsExpected(
        NonPriceElements nonPriceElements,
        bool expected) => nonPriceElements.HasIncompleteWeighting().Should().Be(expected);

    [Theory]
    [MockMemberAutoData(nameof(GetNonPriceElementsTestData))]
    public static void GetNonPriceElements_ReturnsExpected(
        NonPriceElements nonPriceElements,
        IEnumerable<NonPriceElement> expected) => nonPriceElements.GetNonPriceElements().Should().BeEquivalentTo(expected);

    [Fact]
    public static void AsScoreType_InvalidInput_ThrowsException() => FluentActions
        .Invoking(() => ((NonPriceElement)int.MaxValue).AsScoreType())
        .Should()
        .Throw<ArgumentOutOfRangeException>();

    [Theory]
    [MockInlineAutoData(NonPriceElement.Features, ScoreType.Features)]
    [MockInlineAutoData(NonPriceElement.Implementation, ScoreType.Implementation)]
    [MockInlineAutoData(NonPriceElement.Interoperability, ScoreType.Interoperability)]
    [MockInlineAutoData(NonPriceElement.ServiceLevel, ScoreType.ServiceLevel)]
    public static void AsScoreType_ReturnsExpectedScoreType(
        NonPriceElement nonPriceElement,
        ScoreType expectedScoreType) => nonPriceElement.AsScoreType().Should().Be(expectedScoreType);

    [Theory]
    [MockAutoData]
    public static void RemoveNonPriceElement_Features_ClearsFeatures(
        List<FeaturesCriteria> featuresCriteria,
        NonPriceElements nonPriceElements)
    {
        nonPriceElements.Features = featuresCriteria;

        nonPriceElements.Features.Should().NotBeEmpty();

        nonPriceElements.RemoveNonPriceElement(NonPriceElement.Features);

        nonPriceElements.Features.Should().BeEmpty();
    }

    [Theory]
    [MockAutoData]
    public static void RemoveNonPriceElement_Implementation_ClearsImplementation(
        ImplementationCriteria implementationCriteria,
        NonPriceElements nonPriceElements)
    {
        nonPriceElements.Implementation = implementationCriteria;

        nonPriceElements.Implementation.Should().NotBeNull();

        nonPriceElements.RemoveNonPriceElement(NonPriceElement.Implementation);

        nonPriceElements.Implementation.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static void RemoveNonPriceElement_Interoperability_ClearsIntegrations(
        List<IntegrationType> integrationTypes,
        NonPriceElements nonPriceElements)
    {
        nonPriceElements.IntegrationTypes = integrationTypes;

        nonPriceElements.IntegrationTypes.Should().NotBeEmpty();

        nonPriceElements.RemoveNonPriceElement(NonPriceElement.Interoperability);

        nonPriceElements.IntegrationTypes.Should().BeEmpty();
    }

    [Theory]
    [MockAutoData]
    public static void RemoveNonPriceElement_ServiceLevels_ClearsServiceLevels(
        ServiceLevelCriteria serviceLevelCriteria,
        NonPriceElements nonPriceElements)
    {
        nonPriceElements.ServiceLevel = serviceLevelCriteria;

        nonPriceElements.ServiceLevel.Should().NotBeNull();

        nonPriceElements.RemoveNonPriceElement(NonPriceElement.ServiceLevel);

        nonPriceElements.ServiceLevel.Should().BeNull();
    }
}

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
            new NonPriceElements { Interoperability = new List<InteroperabilityCriteria>() },
            false,
        },
        new object[] { NonPriceElement.Implementation, new NonPriceElements { Implementation = null }, false, },
        new object[] { NonPriceElement.ServiceLevel, new NonPriceElements { ServiceLevel = null }, false, },
        new object[]
        {
            NonPriceElement.Interoperability,
            new NonPriceElements { Interoperability = new List<InteroperabilityCriteria> { new() } },
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
                Interoperability = new List<InteroperabilityCriteria> { new() }, NonPriceWeights = new(),
            },
            true,
        },
        new object[] { new NonPriceElements { Implementation = new(), NonPriceWeights = new(), }, true, },
        new object[] { new NonPriceElements { ServiceLevel = new(), NonPriceWeights = new(), }, true, },
        new object[]
        {
            new NonPriceElements
            {
                Interoperability = new List<InteroperabilityCriteria> { new() },
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
            new NonPriceElements { Interoperability = new List<InteroperabilityCriteria> { new() } },
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
    [CommonMemberAutoData(nameof(HasNonPriceElementTestData))]
    public static void HasNonPriceElement_ReturnsExpected(
        NonPriceElement nonPriceElement,
        NonPriceElements nonPriceElements,
        bool expected) => nonPriceElements.HasNonPriceElement(nonPriceElement).Should().Be(expected);

    [Theory]
    [CommonMemberAutoData(nameof(GetNonPriceWeightTestData))]
    public static void GetNonPriceWeight_ReturnsExpected(
        NonPriceElement nonPriceElement,
        NonPriceElements nonPriceElements,
        int? expected) => nonPriceElements.GetNonPriceWeight(nonPriceElement).Should().Be(expected);

    [Theory]
    [CommonMemberAutoData(nameof(HasIncompleteWeightingTestData))]
    public static void HasIncompleteWeighting_ReturnsExpected(
        NonPriceElements nonPriceElements,
        bool expected) => nonPriceElements.HasIncompleteWeighting().Should().Be(expected);

    [Theory]
    [CommonMemberAutoData(nameof(GetNonPriceElementsTestData))]
    public static void GetNonPriceElements_ReturnsExpected(
        NonPriceElements nonPriceElements,
        IEnumerable<NonPriceElement> expected) => nonPriceElements.GetNonPriceElements().Should().BeEquivalentTo(expected);
}

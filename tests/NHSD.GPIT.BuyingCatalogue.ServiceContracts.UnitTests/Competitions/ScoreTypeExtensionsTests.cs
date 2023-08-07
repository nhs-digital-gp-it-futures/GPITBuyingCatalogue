using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Competitions;

public static class ScoreTypeExtensionsTests
{
    [Theory]
    [CommonInlineAutoData(ScoreType.Implementation, NonPriceElement.Implementation)]
    [CommonInlineAutoData(ScoreType.Interoperability, NonPriceElement.Interoperability)]
    [CommonInlineAutoData(ScoreType.ServiceLevel, NonPriceElement.ServiceLevel)]
    [CommonInlineAutoData(ScoreType.Price, null)]
    public static void AsNonPriceElement_ReturnsExpected(
        ScoreType scoreType,
        NonPriceElement? nonPriceElement) => scoreType.AsNonPriceElement().Should().Be(nonPriceElement);
}

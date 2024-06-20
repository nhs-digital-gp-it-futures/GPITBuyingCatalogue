using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Competitions;

public static class ScoreTypeExtensionsTests
{
    [Theory]
    [MockInlineAutoData(ScoreType.Implementation, NonPriceElement.Implementation)]
    [MockInlineAutoData(ScoreType.Interoperability, NonPriceElement.Interoperability)]
    [MockInlineAutoData(ScoreType.ServiceLevel, NonPriceElement.ServiceLevel)]
    [MockInlineAutoData(ScoreType.Price, null)]
    public static void AsNonPriceElement_ReturnsExpected(
        ScoreType scoreType,
        NonPriceElement? nonPriceElement) => scoreType.AsNonPriceElement().Should().Be(nonPriceElement);
}

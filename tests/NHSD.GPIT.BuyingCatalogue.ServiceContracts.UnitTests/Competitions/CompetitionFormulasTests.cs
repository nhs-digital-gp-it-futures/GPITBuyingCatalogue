using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Competitions;

public static class CompetitionFormulasTests
{
    [Theory]
    [InlineData(1000, 3500, 1)]
    [InlineData(1000, 2000, 2)]
    [InlineData(1000, 1500, 3)]
    [InlineData(1000, 1100, 4)]
    [InlineData(1000, 1000, 5)]
    public static void CalculatePriceIncreaseScore_ReturnsExpected(
        decimal lowestPrice,
        decimal currentPrice,
        int expectedScore)
    {
        CompetitionFormulas.CalculatePriceIncreaseScore(lowestPrice, currentPrice).Should().Be(expectedScore);
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(0, 139, 1)]
    [InlineData(0, 5821, 1)]
    [InlineData(0, 81924, 1)]
    public static void CalculatePriceIncreaseScore_LowestPriceIsZero_ReturnsExpected(
        decimal lowestPrice,
        decimal currentPrice,
        int expectedScore)
    {
        CompetitionFormulas.CalculatePriceIncreaseScore(lowestPrice, currentPrice).Should().Be(expectedScore);
    }

    [Theory]
    [InlineData(5, 100, 5)]
    [InlineData(5, 50, 2.5)]
    public static void CalculateWeightedScore_Int_ReturnsExpected(
        int score,
        int weight,
        decimal expectedScore)
    {
        CompetitionFormulas.CalculateWeightedScore(score, weight).Should().Be(expectedScore);
    }

    [Theory]
    [InlineData(4.2, 100, 4.2)]
    [InlineData(4.2, 50, 2.1)]
    public static void CalculateWeightedScore_Decimal_ReturnsExpected(
        decimal score,
        int weight,
        decimal expectedScore)
    {
        CompetitionFormulas.CalculateWeightedScore(score, weight).Should().Be(expectedScore);
    }
}

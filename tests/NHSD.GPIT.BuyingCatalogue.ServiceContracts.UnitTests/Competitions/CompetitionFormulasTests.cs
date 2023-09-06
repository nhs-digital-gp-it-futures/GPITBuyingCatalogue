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
    [InlineData(1000, 1100,  4)]
    public static void CalculatePriceDifferenceScore_ReturnsExpected(
        decimal lowestPrice,
        decimal currentPrice,
        int expectedScore)
    {
        CompetitionFormulas.CalculatePriceDifferenceScore(lowestPrice, currentPrice).Should().Be(expectedScore);
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

using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public static class CompetitionFormulas
{
    public static decimal CalculateWeightedScore(int score, int weight)
        => score * ((decimal)weight / 100);

    public static decimal CalculateWeightedScore(decimal score, int weight)
        => score * ((decimal)weight / 100);

    public static int CalculatePriceIncreaseScore(decimal lowestPrice, decimal currentPrice)
    {
        const int minScore = 1;

        if (lowestPrice is 0M) return minScore;

        var difference = ((currentPrice - lowestPrice) / lowestPrice) * 100;

        return Math.Abs(difference) switch
        {
            0 => 5,
            > 0 and <= 25 => 4,
            > 25 and <= 50 => 3,
            > 50 and <= 100 => 2,
            _ or > 100 => minScore,
        };
    }
}

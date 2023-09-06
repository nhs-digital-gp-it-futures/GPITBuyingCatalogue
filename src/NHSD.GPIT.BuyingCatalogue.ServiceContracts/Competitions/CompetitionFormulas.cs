using System;
using System.Diagnostics;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public static class CompetitionFormulas
{
    public static decimal CalculateWeightedScore(int score, int weight)
        => score * ((decimal)weight / 100);

    public static decimal CalculateWeightedScore(decimal score, int weight)
        => score * ((decimal)weight / 100);

    public static int CalculatePriceDifferenceScore(decimal lowestPrice, decimal currentPrice)
    {
        var v1 = currentPrice - lowestPrice;
        var v2 = currentPrice + lowestPrice;

        var difference = (v1 / (v2 / 2)) * 100;

        return Math.Abs(difference) switch
        {
            >= 0 and <= 25 => 4,
            > 25 and <= 50 => 3,
            > 50 and <= 100 => 2,
            > 100 => 1,
            _ => throw new UnreachableException(),
        };
    }
}

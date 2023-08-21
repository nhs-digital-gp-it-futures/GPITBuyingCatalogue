using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public static class ScoreTypeExtensions
{
    private static readonly ScoreType[] NonPriceElementScores = new[]
    {
        ScoreType.Implementation, ScoreType.Interoperability, ScoreType.ServiceLevel,
    };

    public static NonPriceElement? AsNonPriceElement(this ScoreType scoreType) => scoreType switch
    {
        ScoreType.Implementation => NonPriceElement.Implementation,
        ScoreType.Interoperability => NonPriceElement.Interoperability,
        ScoreType.ServiceLevel => NonPriceElement.ServiceLevel,
        _ => null,
    };

    public static ScoreType[] GetNonPriceElementScores() => NonPriceElementScores;
}

using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public static class ScoreTypeExtensions
{
    public static NonPriceElement? AsNonPriceElement(this ScoreType scoreType) => scoreType switch
    {
        ScoreType.Implementation => NonPriceElement.Implementation,
        ScoreType.Interoperability => NonPriceElement.Interoperability,
        ScoreType.ServiceLevel => NonPriceElement.ServiceLevel,
        _ => null,
    };
}

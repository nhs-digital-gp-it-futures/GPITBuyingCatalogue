using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

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

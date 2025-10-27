using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class CompetitionAdditionalService : CompetitionCatalogueItem
{
    public CompetitionAdditionalService()
    {
    }

    public CompetitionAdditionalService(
        int competitionId,
        CatalogueItemId catalogueItemId,
        bool isRequired = false)
    {
        CompetitionId = competitionId;
        CatalogueItemId = catalogueItemId;
        IsRequired = isRequired;
    }

    public bool IsRequired { get; set; }
}

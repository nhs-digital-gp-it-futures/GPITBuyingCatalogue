using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class CompetitionAssociatedService : CompetitionCatalogueItem
{
    public CompetitionAssociatedService()
    {
    }

    public CompetitionAssociatedService(
        int competitionId,
        CatalogueItemId associatedServiceId)
        : base(competitionId, associatedServiceId)
    {
        CatalogueItemType = CatalogueItemType.AssociatedService;
    }
}

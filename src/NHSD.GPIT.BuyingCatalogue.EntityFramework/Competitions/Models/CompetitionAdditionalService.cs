using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class CompetitionAdditionalService : CompetitionCatalogueItem
{
    public CompetitionAdditionalService()
    {
    }

    public CompetitionAdditionalService(
        int competitionId,
        CatalogueItemId additionalServiceId,
        bool isRequired = false)
        : base(competitionId, additionalServiceId)
    {
        IsRequired = isRequired;
        CatalogueItemType = CatalogueItemType.AdditionalService;
    }

    public bool IsRequired { get; set; }

    public IEnumerable<CompetitionAssociatedService> CompetitionAssociatedServices =>
        Services.OfType<CompetitionAssociatedService>();

    public bool AssociatedServicesAvailable { get; set; }

    public bool AssociatedServicesRemaining { get; set; }
}

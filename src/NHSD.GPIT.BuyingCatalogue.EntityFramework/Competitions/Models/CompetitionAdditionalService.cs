using System.Collections.Generic;
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
    }

    public bool IsRequired { get; set; }

    public IEnumerable<CompetitionAssociatedService> AssociatedServices { get; set; }

    public bool AssociatedServicesAvailable { get; set; }

    public bool AssociatedServicesRemaining { get; set; }
}

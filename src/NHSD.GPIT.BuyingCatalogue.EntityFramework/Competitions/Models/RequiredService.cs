using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class RequiredService
{
    public int CompetitionId { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public CatalogueItemId ServiceId { get; set; }

    public AdditionalService Service { get; set; }
}

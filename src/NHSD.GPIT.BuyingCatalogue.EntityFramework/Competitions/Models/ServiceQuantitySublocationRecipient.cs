using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class ServiceQuantitySublocationRecipient : RecipientQuantityBase
{
    public int CompetitionId { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public CatalogueItemId ServiceId { get; set; }

    public CompetitionSublocationRecipient CompetitionSublocationRecipient { get; set; }

    public SolutionService SolutionService { get; set; }
}

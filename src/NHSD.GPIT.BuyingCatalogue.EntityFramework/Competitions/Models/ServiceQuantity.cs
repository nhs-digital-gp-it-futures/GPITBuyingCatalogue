using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class ServiceQuantity : RecipientQuantityBase
{
    public int CompetitionId { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public CatalogueItemId ServiceId { get; set; }

    public CompetitionRecipient CompetitionRecipient { get; set; }

    public SolutionService SolutionService { get; set; }
}

using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class SolutionQuantity : RecipientQuantityBase
{
    public int CompetitionId { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public CompetitionSolution CompetitionSolution { get; set; }

    public CompetitionRecipient CompetitionRecipient { get; set; }
}

using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class SolutionQuantitySublocationRecipient : RecipientQuantityBase
{
    public int CompetitionId { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public CompetitionSolution CompetitionSolution { get; set; }

    public CompetitionSublocationRecipient CompetitionSublocationRecipient { get; set; }
}

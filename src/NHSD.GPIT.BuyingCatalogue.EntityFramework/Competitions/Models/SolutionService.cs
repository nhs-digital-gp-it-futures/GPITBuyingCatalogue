using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class SolutionService : ICompetitionPriceEntity
{
    public SolutionService()
    {
    }

    public SolutionService(
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        bool isRequired)
    {
        CompetitionId = competitionId;
        SolutionId = solutionId;
        ServiceId = serviceId;
        IsRequired = isRequired;
    }

    public int CompetitionId { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public CatalogueItemId ServiceId { get; set; }

    public int? CompetitionItemPriceId { get; set; }

    public CatalogueItem Service { get; set; }

    public CompetitionCatalogueItemPrice Price { get; set; }

    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets the global quantity when the <see cref="Price"/> is a global pricing model.
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// Gets or sets the quantities for each service recipient, when the <see cref="Price"/> is based on the practice list size.
    /// </summary>
    public ICollection<ServiceQuantity> Quantities { get; set; } = new HashSet<ServiceQuantity>();
}

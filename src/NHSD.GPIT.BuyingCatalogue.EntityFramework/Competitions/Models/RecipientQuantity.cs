using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class RecipientQuantity
{
    public int CompetitionId { get; set; }

    public string OdsCode { get; set; }

    public CatalogueItemId CatalogueItemId { get; set; }

    public int Quantity { get; set; }
}

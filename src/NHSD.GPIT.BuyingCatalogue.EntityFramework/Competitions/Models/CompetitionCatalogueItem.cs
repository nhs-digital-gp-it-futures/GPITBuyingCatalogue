using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public abstract class CompetitionCatalogueItem
{
    public int Id { get; set; }

    public int CompetitionId { get; set; }

    public int? Quantity { get; set; }

    public int? ParentItemId { get; set; }

    public CatalogueItemId CatalogueItemId { get; set; }

    public CatalogueItemType CatalogueItemType { get; set; }

    public Competition Competition { get; set; }

    public CatalogueItem CatalogueItem { get; set; }

    public CompetitionCatalogueItemPrice Price { get; set; }

    public ICollection<CompetitionItemQuantity> Quantities { get; set; } = new HashSet<CompetitionItemQuantity>();
}

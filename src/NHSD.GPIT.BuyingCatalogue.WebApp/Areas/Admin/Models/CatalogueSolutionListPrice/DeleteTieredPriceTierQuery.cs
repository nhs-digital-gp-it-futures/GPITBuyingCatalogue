using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionListPrice;

public class DeleteTieredPriceTierQuery
{
    public SolutionType? SolutionType { get; init; }

    public CatalogueItemId? SolutionId { get; init; }

    public CatalogueItemId? AdditionalServiceId { get; init; }

    public int? SupplierId { get; init; }

    public bool? IsEditing { get; init; }
}

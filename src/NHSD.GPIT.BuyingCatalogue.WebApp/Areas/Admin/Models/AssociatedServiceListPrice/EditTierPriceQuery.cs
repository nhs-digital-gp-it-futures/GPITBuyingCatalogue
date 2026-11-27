using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServiceListPrice;

public sealed class EditTierPriceQuery
{
    public SolutionType? SolutionType { get; init; }

    public CatalogueItemId? SolutionId { get; init; }

    public CatalogueItemId? AdditionalServiceId { get; init; }

    public int? TierIndex { get; init; } = 0;
}

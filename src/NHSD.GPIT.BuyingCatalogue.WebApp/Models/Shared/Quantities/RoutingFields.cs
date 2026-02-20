using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

public class RoutingFields
{
    public CatalogueItemId? CatalogueItem { get; init; }

    public string InternalOrgId { get; init; }

    public CallOffId? CallOffId { get; init; }

    public CatalogueItemId? SolutionId { get; init; }

    public CatalogueItemId? ServiceId { get; init; }

    public int? CompetitionId { get; init; }
}

using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;

public class RemoveServiceModel : NavBaseModel
{
    public IList<SelectOption<bool>> RemoveServiceOptions =>
    [
        new($"Yes, I confirm I want to remove {ServiceName}", true),
        new($"No, I want to keep {ServiceName}", false),
    ];

    public required CatalogueItemType ServiceType { get; set; }

    public required string ServiceName { get; set; }

    public string EntityType { get; set; } = "Order";

    public bool? ConfirmRemoveService { get; set; }

    public RoutingSource? Source { get; set; }
}

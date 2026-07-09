using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services
{
    public class RemoveServiceModel : NavBaseModel
    {
        public RemoveServiceModel()
        {
        }

        public RemoveServiceModel(CatalogueItem service)
        {
            ServiceName = service.Name;
            ServiceType = service.CatalogueItemType;
        }

        public IList<SelectOption<bool>> RemoveServiceOptions =>
        [
            new($"Yes, I confirm I want to remove {ServiceName}", true),
            new($"No, I want to keep my current {ServiceType}s", false),
        ];

        [Required]
        public CatalogueItemType ServiceType { get; set; }

        public string ServiceName { get; set; }

        public string EntityType { get; set; } = "Order";

        public bool? ConfirmRemoveService { get; set; }

        public RoutingSource? Source { get; set; }
    }
}

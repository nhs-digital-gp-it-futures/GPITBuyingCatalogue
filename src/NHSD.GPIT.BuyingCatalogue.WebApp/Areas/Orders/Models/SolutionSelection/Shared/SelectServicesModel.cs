using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared
{
    public class SelectServicesModel : NavBaseModel
    {
        public SelectServicesModel()
        {
        }

        public SelectServicesModel(
            Order order,
            IEnumerable<CatalogueItem> services,
            CatalogueItemType catalogueItemType)
        {
            var existingServices = catalogueItemType switch
            {
                CatalogueItemType.AdditionalService => order.GetAdditionalServices().ToList(),
                CatalogueItemType.AssociatedService => order.GetAssociatedServices().ToList(),
                CatalogueItemType.Solution => throw new ArgumentOutOfRangeException(nameof(catalogueItemType), catalogueItemType, null),
                _ => throw new ArgumentOutOfRangeException(nameof(catalogueItemType), catalogueItemType, null),
            };

            var existingServiceIds = existingServices.Select(x => x.CatalogueItem.Id).ToList();

            Services = services.Select(x => new ServiceModel
            {
                CatalogueItemId = x.Id,
                Description = x.Name,
            })
            .ToList();

            foreach (var existingService in Services.Where(x => existingServiceIds.Contains(x.CatalogueItemId)))
            {
                existingService.IsSelected = true;
            }
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public List<ServiceModel> Services { get; set; }
    }
}

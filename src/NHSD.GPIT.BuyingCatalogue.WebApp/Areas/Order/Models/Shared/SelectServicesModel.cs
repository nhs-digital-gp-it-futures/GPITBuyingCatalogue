using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Shared
{
    public class SelectServicesModel : NavBaseModel
    {
        public SelectServicesModel()
        {
        }

        public SelectServicesModel(
            EntityFramework.Ordering.Models.Order order,
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

            var existingServiceIds = existingServices.Select(x => x.CatalogueItem.Id);

            ExistingServices = existingServices.Select(x => x.CatalogueItem.Name);
            Services = services
                .Where(x => !existingServiceIds.Contains(x.Id))
                .Select(x => new ServiceModel
                {
                    CatalogueItemId = x.Id,
                    Description = x.Name,
                })
                .ToList();
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public IEnumerable<string> ExistingServices { get; set; }

        public List<ServiceModel> Services { get; set; }
    }
}

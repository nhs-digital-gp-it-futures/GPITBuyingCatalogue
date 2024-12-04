using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services
{
    public class SelectServicesModel : NavBaseModel
    {
        public SelectServicesModel()
        {
        }

        public SelectServicesModel(
            IEnumerable<CatalogueItem> excludedServices,
            IEnumerable<CatalogueItem> allServices)
        {
            var previousServiceIds = excludedServices.Select(x => x.Id).ToList();

            Services = allServices
                .Where(x => !previousServiceIds.Contains(x.Id))
                .Select(
                    x => new ServiceModel
                    {
                        CatalogueItemId = x.Id,
                        Description = x.Name,
                    })
                .ToList();
        }

        public string EntityType { get; set; } = "Order";

        public string InternalOrgId { get; set; }

        public bool IsAmendment { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public string SolutionName { get; set; }

        public CatalogueItemId? SolutionId { get; set; }

        public List<ServiceModel> Services { get; set; }
    }
}

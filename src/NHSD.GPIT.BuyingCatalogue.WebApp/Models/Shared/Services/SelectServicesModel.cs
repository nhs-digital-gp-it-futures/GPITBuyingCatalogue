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
            IEnumerable<CatalogueItem> currentServices,
            IEnumerable<CatalogueItem> allServices)
        {
            var currentServiceIds = currentServices.Select(x => x.Id).ToList();

            Services = allServices
                .Where(x => !currentServiceIds.Contains(x.Id))
                .Select(
                    x => new ServiceModel
                    {
                        CatalogueItemId = x.Id,
                        Description = x.Name,
                        IsSelected = currentServiceIds.Contains(x.Id),
                    })
                .ToList();
        }

        public SelectServicesModel(
            IEnumerable<CatalogueItem> previousServices,
            IEnumerable<CatalogueItem> currentServices,
            IEnumerable<CatalogueItem> allServices)
            : this(currentServices, allServices)
        {
            var enumeratedServices = previousServices.ToList();

            var previousServiceIds = enumeratedServices.Select(x => x.Id).ToList();

            Services = Services.Where(x => !previousServiceIds.Contains(x.CatalogueItemId)).ToList();
            ExistingServices = enumeratedServices.Select(x => x.Name).ToList();
        }

        public string EntityType { get; set; } = "Order";

        public string InternalOrgId { get; set; }

        public bool IsAmendment { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public string SolutionName { get; set; }

        public CatalogueItemId? SolutionId { get; set; }

        public List<string> ExistingServices { get; set; } = Enumerable.Empty<string>().ToList();

        public List<ServiceModel> Services { get; set; }
    }
}

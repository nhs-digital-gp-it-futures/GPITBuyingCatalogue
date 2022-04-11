using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class SelectSolutionModel : NavBaseModel
    {
        public SelectSolutionModel()
        {
        }

        public SelectSolutionModel(
            EntityFramework.Ordering.Models.Order order,
            IEnumerable<CatalogueItem> solutions,
            IEnumerable<CatalogueItem> additionalServices)
        {
            SupplierName = order.Supplier.Name;

            CatalogueSolutions = solutions
                .Select(x => new SelectListItem(x.Name, $"{x.Id}"))
                .ToList();

            AdditionalServices = additionalServices
                .Select(x => new ServiceModel
                {
                    CatalogueItemId = x.Id,
                    Description = x.Name,
                    IsSelected = order.OrderItems.Any(o => o.CatalogueItemId == x.Id),
                })
                .ToList();
        }

        public string SupplierName { get; set; }

        public string SelectedCatalogueSolutionId { get; set; }

        public List<SelectListItem> CatalogueSolutions { get; set; }

        public List<ServiceModel> AdditionalServices { get; set; }

        public IEnumerable<CatalogueItemId> GetAdditionalServicesIdsForSelectedCatalogueSolution()
        {
            return AdditionalServices
                .Where(x => x.IsSelected
                        && $"{x.CatalogueItemId}".StartsWith(SelectedCatalogueSolutionId))
                .Select(x => x.CatalogueItemId);
        }
    }
}

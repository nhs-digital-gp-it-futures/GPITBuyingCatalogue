using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared
{
    public class SelectSolutionModel : NavBaseModel
    {
        public SelectSolutionModel()
        {
        }

        public SelectSolutionModel(
            Order order,
            IEnumerable<CatalogueItem> solutions,
            IEnumerable<CatalogueItem> additionalServices)
        {
            AssociatedServicesOnly = order.OrderType.AssociatedServicesOnly;
            CallOffId = order.CallOffId;
            SupplierName = order.Supplier?.Name;

            CatalogueSolutions = solutions
                .Select(x => new SelectOption<string>(x.Name, $"{x.Id}"))
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

        public bool AssociatedServicesOnly { get; set; }

        public CallOffId CallOffId { get; set; }

        public string SupplierName { get; set; }

        public string SelectedCatalogueSolutionId { get; set; }

        public List<SelectOption<string>> CatalogueSolutions { get; set; }

        public List<ServiceModel> AdditionalServices { get; set; }

        public IEnumerable<CatalogueItemId> GetAdditionalServicesIdsForSelectedCatalogueSolution()
        {
            return AdditionalServices?
                .Where(x => x.IsSelected
                    && $"{x.CatalogueItemId}".StartsWith(SelectedCatalogueSolutionId))
                .Select(x => x.CatalogueItemId) ?? Enumerable.Empty<CatalogueItemId>();
        }
    }
}

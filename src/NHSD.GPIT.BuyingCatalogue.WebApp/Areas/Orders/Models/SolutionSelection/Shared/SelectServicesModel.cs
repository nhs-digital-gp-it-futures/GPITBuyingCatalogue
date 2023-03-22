using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared
{
    public class SelectServicesModel : NavBaseModel
    {
        public SelectServicesModel()
        {
        }

        public SelectServicesModel(
            OrderWrapper wrapper,
            IEnumerable<CatalogueItem> services,
            CatalogueItemType catalogueItemType)
        {
            var existingServices = GetServices(wrapper.Previous, catalogueItemType);
            var existingServiceIds = existingServices.Select(x => x.CatalogueItem.Id).ToList();

            var currentServices = GetServices(wrapper.Order, catalogueItemType);
            var currentServiceIds = currentServices.Select(x => x.CatalogueItem.Id).ToList();

            ExistingServices = existingServices.Select(x => x.CatalogueItem.Name).ToList();

            Services = services
                .Where(x => !existingServiceIds.Contains(x.Id))
                .Select(x => new ServiceModel
                {
                    CatalogueItemId = x.Id,
                    Description = x.Name,
                })
                .ToList();

            foreach (var service in Services.Where(x => currentServiceIds.Contains(x.CatalogueItemId)))
            {
                service.IsSelected = true;
            }

            IsAmendment = wrapper.IsAmendment;
            AssociatedServicesOnly = wrapper.Order.AssociatedServicesOnly;
            SolutionName = AssociatedServicesOnly ? wrapper.RolledUp.Solution.Name : wrapper.RolledUp.GetSolution()?.CatalogueItem.Name;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public string SolutionName { get; set; }

        public List<string> ExistingServices { get; set; }

        public List<ServiceModel> Services { get; set; }

        private static List<OrderItem> GetServices(Order order, CatalogueItemType catalogueItemType)
        {
            if (order == null)
            {
                return new List<OrderItem>();
            }

            return catalogueItemType switch
            {
                CatalogueItemType.AdditionalService => order.GetAdditionalServices().ToList(),
                CatalogueItemType.AssociatedService => order.GetAssociatedServices().ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(catalogueItemType), catalogueItemType, null),
            };
        }
    }
}

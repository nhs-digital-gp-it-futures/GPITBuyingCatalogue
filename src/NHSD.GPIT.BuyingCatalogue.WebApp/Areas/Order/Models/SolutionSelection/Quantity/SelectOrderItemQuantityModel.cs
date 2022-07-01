using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Quantity
{
    public class SelectOrderItemQuantityModel : NavBaseModel
    {
        public SelectOrderItemQuantityModel()
        {
        }

        public SelectOrderItemQuantityModel(OrderItem orderItem)
        {
            if (orderItem == null)
            {
                throw new ArgumentNullException(nameof(orderItem));
            }

            ItemName = orderItem.CatalogueItem.Name;
            ItemType = orderItem.CatalogueItem.CatalogueItemType.Name();
            Quantity = orderItem.Quantity.HasValue ? $"{orderItem.Quantity}" : string.Empty;
            QuantityDescription = orderItem.OrderItemPrice.RangeDescription;
            ProvisioningType = orderItem.OrderItemPrice.ProvisioningType;
            BillingPeriod = orderItem.OrderItemPrice.BillingPeriod;
        }

        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public string Quantity { get; set; }

        public string QuantityDescription { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public TimeUnit? BillingPeriod { get; set; }

        public RoutingSource? Source { get; set; }
    }
}

using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
            QuantityDescription = orderItem.OrderItemPrice.RangeDescription;
        }

        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public string Quantity { get; set; }

        public string QuantityDescription { get; set; }
    }
}

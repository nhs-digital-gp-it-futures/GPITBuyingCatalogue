using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Quantity
{
    public class SelectServiceRecipientQuantityModel : NavBaseModel
    {
        public SelectServiceRecipientQuantityModel()
        {
        }

        public SelectServiceRecipientQuantityModel(OrderItem orderItem)
        {
            if (orderItem == null)
            {
                throw new ArgumentNullException(nameof(orderItem));
            }

            ItemName = orderItem.CatalogueItem.Name;
            ItemType = orderItem.CatalogueItem.CatalogueItemType.Name();
            ServiceRecipients = orderItem.OrderItemRecipients
                .Select(x => new ServiceRecipientQuantityModel
                {
                    OdsCode = x.OdsCode,
                    Name = x.Recipient?.Name,
                    InputQuantity = x.Quantity.HasValue ? $"{x.Quantity}" : string.Empty,
                })
                .ToArray();
        }

        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public ServiceRecipientQuantityModel[] ServiceRecipients { get; set; }

        public RoutingSource? Source { get; set; }
    }
}

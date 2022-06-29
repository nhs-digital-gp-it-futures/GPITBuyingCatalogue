using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
            ProvisioningType = orderItem.OrderItemPrice.ProvisioningType;
            RangeDefinition = orderItem.OrderItemPrice.RangeDescription;
            BillingPeriod = orderItem.OrderItemPrice.BillingPeriod;
        }

        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public string RangeDefinition { get; set; }

        public TimeUnit? BillingPeriod { get; set; }

        public ServiceRecipientQuantityModel[] ServiceRecipients { get; set; }

        public RoutingSource? Source { get; set; }

        public string QuantityColumnTitle => ProvisioningType switch
        {
            ProvisioningType.Patient => "Practice list size",
            _ => $"{RangeDefinition} {BillingPeriod?.Description() ?? string.Empty}",
        };
    }
}

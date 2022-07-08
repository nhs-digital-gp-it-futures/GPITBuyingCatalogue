using System;
using System.Linq;
using LinqKit;
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
            ProvisioningType = orderItem.OrderItemPrice.ProvisioningType;
            RangeDefinition = orderItem.OrderItemPrice.RangeDescription;
            BillingPeriod = orderItem.OrderItemPrice.BillingPeriod;
            ServiceRecipients = orderItem.OrderItemRecipients
                .Select(CreateServiceRecipient)
                .ToArray();
        }

        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public string RangeDefinition { get; set; }

        public TimeUnit? BillingPeriod { get; set; }

        public ServiceRecipientQuantityModel[] ServiceRecipients { get; set; }

        public RoutingSource? Source { get; set; }

        public bool ShouldShowInset => ProvisioningType is ProvisioningType.Patient;

        public string LedeText => ProvisioningType switch
        {
            ProvisioningType.Patient => "We’ve included the latest practice list sizes published by NHS Digital.",
            ProvisioningType.PerServiceRecipient => "You can only order one solution per Service Recipient.",
            _ => "Enter the quantity you want for each practice for the duration of your order.",
        };

        public string QuantityColumnTitle => ProvisioningType switch
        {
            ProvisioningType.Patient => "Practice list size",
            _ => RangeDefinition,
        };

        private ServiceRecipientQuantityModel CreateServiceRecipient(OrderItemRecipient recipient)
        {
            var recipientQuantityModel = new ServiceRecipientQuantityModel
            {
                OdsCode = recipient.OdsCode,
                Name = recipient.Recipient?.Name,
            };

            if (ProvisioningType is ProvisioningType.PerServiceRecipient)
            {
                recipientQuantityModel.Quantity = 1;
            }
            else
            {
                recipientQuantityModel.InputQuantity =
                    recipient.Quantity.HasValue ? $"{recipient.Quantity}" : string.Empty;
            }

            return recipientQuantityModel;
        }
    }
}

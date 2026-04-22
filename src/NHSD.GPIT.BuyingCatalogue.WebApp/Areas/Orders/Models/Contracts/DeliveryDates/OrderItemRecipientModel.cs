using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates
{
    public class OrderItemRecipientModel
    {
        public OrderItemRecipientModel()
        {
        }

        public OrderItemRecipientModel(OrderSublocationRecipient recipient, OrderItem orderItem)
        {
            CatalogueItemId = orderItem.CatalogueItemId;
            DeliveryDate = recipient.GetDeliveryDateForItem(orderItem.Id);
            OdsCode = recipient.RecipientOdsCode;
            RecipientName = recipient.RecipientOdsOrganisation?.Name;
        }

        public CatalogueItemId CatalogueItemId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string OdsCode { get; set; }

        public string RecipientName { get; set; }
    }
}

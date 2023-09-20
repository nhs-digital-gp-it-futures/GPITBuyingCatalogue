using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates
{
    public class OrderItemRecipientModel
    {
        public OrderItemRecipientModel()
        {
        }

        public OrderItemRecipientModel(OrderItemRecipient recipient)
        {
            CatalogueItemId = recipient.CatalogueItemId;
            DeliveryDate = recipient.DeliveryDate;
            OdsCode = recipient.OdsCode;
            RecipientName = recipient.Recipient?.OdsOrganisation.Name;
        }

        public CatalogueItemId CatalogueItemId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string OdsCode { get; set; }

        public string RecipientName { get; set; }
    }
}

using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates
{
    public class OrderItemRecipientModel
    {
        public OrderItemRecipientModel()
        {
        }

        public OrderItemRecipientModel(OrderRecipient recipient, CatalogueItemId catalogueItemId)
        {
            CatalogueItemId = catalogueItemId;
            DeliveryDate = recipient.GetDeliveryDateForItem(catalogueItemId);
            OdsCode = recipient.OdsCode;
            RecipientName = recipient.OdsOrganisation?.Name;
        }

        public CatalogueItemId CatalogueItemId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string OdsCode { get; set; }

        public string RecipientName { get; set; }
    }
}

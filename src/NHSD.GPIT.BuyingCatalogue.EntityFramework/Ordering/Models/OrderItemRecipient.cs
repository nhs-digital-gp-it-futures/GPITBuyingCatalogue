using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public sealed class OrderItemRecipient : IAudited
    {
        public OrderItemRecipient()
        {
        }

        public OrderItemRecipient(
            int orderId,
            string odsCode,
            CatalogueItemId catalogueItemId)
        {
            OrderId = orderId;
            OdsCode = odsCode;
            CatalogueItemId = catalogueItemId;
        }

        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string OdsCode { get; set; }

        public int? Quantity { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public OrderRecipient Recipient { get; set; }

        public OrderItem OrderItem { get; set; }
    }
}

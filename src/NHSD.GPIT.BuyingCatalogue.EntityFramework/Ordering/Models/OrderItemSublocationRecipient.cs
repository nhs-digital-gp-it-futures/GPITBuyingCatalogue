using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed class OrderItemSublocationRecipient : IAudited, ICloneable<OrderItemSublocationRecipient>
    {
        public OrderItemSublocationRecipient()
        {
        }

        public OrderItemSublocationRecipient(
            int orderId,
            string recipientOdsCode,
            CatalogueItemId catalogueItemId)
        {
            OrderId = orderId;
            RecipientOdsCode = recipientOdsCode;
            CatalogueItemId = catalogueItemId;
        }

        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string ParentSublocationOdsCode { get; set; }

        public string RecipientOdsCode { get; set; }

        public int? Quantity { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public OrderSublocationRecipient Recipient { get; set; }

        public OrderItem OrderItem { get; set; }

        public OrderItemSublocationRecipient Clone()
        {
            return new OrderItemSublocationRecipient
            {
                CatalogueItemId = CatalogueItemId,
                RecipientOdsCode = RecipientOdsCode,
                Quantity = Quantity,
                DeliveryDate = DeliveryDate,
            };
        }
    }
}

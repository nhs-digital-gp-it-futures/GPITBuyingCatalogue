using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItemRecipient
    {
        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string OdsCode { get; set; }

        public int Quantity { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public ServiceRecipient Recipient { get; set; }

        public OrderItem OrderItem { get; set; }
    }
}

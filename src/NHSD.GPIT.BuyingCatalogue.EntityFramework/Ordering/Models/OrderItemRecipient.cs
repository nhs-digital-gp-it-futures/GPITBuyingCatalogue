using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItemRecipient
    {
        public int OrderId { get; set; }

        // TODO: should be of type CatalogueItemId
        // Had to revert to string due to CatalogueItem currently having an incorrect ID type
        public string CatalogueItemId { get; set; }

        public string OdsCode { get; set; }

        public int Quantity { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public ServiceRecipient Recipient { get; set; }

        public OrderItem OrderItem { get; set; }
    }
}

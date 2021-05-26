using System;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class OrderItemRecipient
    {
        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string OdsCode { get; set; }

        public int Quantity { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public virtual ServiceRecipient OdsCodeNavigation { get; set; }

        public virtual OrderItem OrderItem { get; set; }
    }
}

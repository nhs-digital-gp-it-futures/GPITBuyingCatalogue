using System;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class DefaultDeliveryDate
    {
        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public DateTime DeliveryDate { get; set; }

        public virtual Order Order { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class DefaultDeliveryDate
    {
        public int OrderId { get; set; }
        public string CatalogueItemId { get; set; }
        public DateTime DeliveryDate { get; set; }

        public virtual Order Order { get; set; }
    }
}

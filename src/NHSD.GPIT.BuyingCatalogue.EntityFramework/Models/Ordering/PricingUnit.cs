using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class PricingUnit
    {
        public PricingUnit()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}

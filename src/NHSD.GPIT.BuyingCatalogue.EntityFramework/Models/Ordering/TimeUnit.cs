using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class TimeUnit
    {
        public TimeUnit()
        {
            OrderItemEstimationPeriods = new HashSet<OrderItem>();
            OrderItemTimeUnits = new HashSet<OrderItem>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<OrderItem> OrderItemEstimationPeriods { get; set; }

        public virtual ICollection<OrderItem> OrderItemTimeUnits { get; set; }
    }
}

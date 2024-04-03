using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public partial class ContractOrderNumber
    {
        public ContractOrderNumber()
        {
            OrderEvents = new HashSet<OrderEvent>();
        }

        public int Id { get; set; }

        public ICollection<OrderEvent> OrderEvents { get; set; }
    }
}

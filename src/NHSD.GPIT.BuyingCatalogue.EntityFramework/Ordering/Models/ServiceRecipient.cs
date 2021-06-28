using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed class ServiceRecipient
    {
        public ServiceRecipient()
        {
            OrderItemRecipients = new HashSet<OrderItemRecipient>();
        }

        public string OdsCode { get; set; }

        public string Name { get; set; }

        public ICollection<OrderItemRecipient> OrderItemRecipients { get; set; }
    }
}

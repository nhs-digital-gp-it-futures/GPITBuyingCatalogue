using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [ExcludeFromCodeCoverage]
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

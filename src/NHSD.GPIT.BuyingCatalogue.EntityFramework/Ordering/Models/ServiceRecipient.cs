using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed class ServiceRecipient : IAudited
    {
        public ServiceRecipient()
        {
            OrderItemRecipients = new HashSet<OrderItemRecipient>();
        }

        public string OdsCode { get; set; }

        public string Name { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public ICollection<OrderItemRecipient> OrderItemRecipients { get; set; }
    }
}

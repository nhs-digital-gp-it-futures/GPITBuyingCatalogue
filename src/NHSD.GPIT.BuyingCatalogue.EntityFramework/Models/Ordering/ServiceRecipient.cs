using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class ServiceRecipient
    {
        public ServiceRecipient()
        {
            OrderItemRecipients = new HashSet<OrderItemRecipient>();
            SelectedServiceRecipients = new HashSet<SelectedServiceRecipient>();
        }

        public string OdsCode { get; set; }
        public string Name { get; set; }

        public virtual ICollection<OrderItemRecipient> OrderItemRecipients { get; set; }
        public virtual ICollection<SelectedServiceRecipient> SelectedServiceRecipients { get; set; }
    }
}

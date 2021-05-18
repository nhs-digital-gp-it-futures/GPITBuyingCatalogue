using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class OrderingParty
    {
        public OrderingParty()
        {
            Orders = new HashSet<Order>();
        }

        public Guid Id { get; set; }

        public string OdsCode { get; set; }

        public string Name { get; set; }

        public int? AddressId { get; set; }

        public virtual Address Address { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}

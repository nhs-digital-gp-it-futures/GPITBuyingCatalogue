using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class CataloguePriceType
    {
        public CataloguePriceType()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}

using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class Address
    {
        public Address()
        {
            OrderingParties = new HashSet<OrderingParty>();
            Suppliers = new HashSet<Supplier>();
        }

        public int Id { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string Line3 { get; set; }

        public string Line4 { get; set; }

        public string Line5 { get; set; }

        public string Town { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }

        public virtual ICollection<OrderingParty> OrderingParties { get; set; }

        public virtual ICollection<Supplier> Suppliers { get; set; }
    }
}

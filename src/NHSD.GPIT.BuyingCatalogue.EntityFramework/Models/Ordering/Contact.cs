using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class Contact
    {
        public Contact()
        {
            OrderOrderingPartyContacts = new HashSet<Order>();
            OrderSupplierContacts = new HashSet<Order>();
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public virtual ICollection<Order> OrderOrderingPartyContacts { get; set; }

        public virtual ICollection<Order> OrderSupplierContacts { get; set; }
    }
}

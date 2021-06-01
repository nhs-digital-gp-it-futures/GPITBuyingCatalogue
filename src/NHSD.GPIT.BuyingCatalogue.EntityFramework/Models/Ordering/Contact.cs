using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(256)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(35)]
        public string Phone { get; set; }

        public virtual ICollection<Order> OrderOrderingPartyContacts { get; set; }

        public virtual ICollection<Order> OrderSupplierContacts { get; set; }
    }
}

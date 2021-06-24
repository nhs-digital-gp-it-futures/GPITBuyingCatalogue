using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed class Contact
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

        public ICollection<Order> OrderOrderingPartyContacts { get; set; }

        public ICollection<Order> OrderSupplierContacts { get; set; }
    }
}

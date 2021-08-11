using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class SupplierContact
    {
        public Guid Id { get; set; }

        public int SupplierId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }
    }
}

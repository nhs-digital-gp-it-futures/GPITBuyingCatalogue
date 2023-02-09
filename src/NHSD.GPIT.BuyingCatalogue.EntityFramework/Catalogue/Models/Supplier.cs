using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed partial class Supplier : IAudited
    {
        public Supplier()
        {
            CatalogueItems = new HashSet<CatalogueItem>();
            SupplierContacts = new HashSet<SupplierContact>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string LegalName { get; set; }

        public string Summary { get; set; }

        public string SupplierUrl { get; set; }

        public Address Address { get; set; }

        public bool Deleted { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public bool IsActive { get; set; }

        public ICollection<CatalogueItem> CatalogueItems { get; set; }

        public ICollection<SupplierContact> SupplierContacts { get; set; }
    }
}

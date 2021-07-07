using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class Supplier
    {
        public Supplier()
        {
            CatalogueItems = new HashSet<CatalogueItem>();
            SupplierContacts = new HashSet<SupplierContact>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string LegalName { get; set; }

        public string Summary { get; set; }

        public string SupplierUrl { get; set; }

        public Address Address { get; set; }

        public string OdsCode { get; set; }

        public Guid? CrmRef { get; set; }

        public bool Deleted { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public ICollection<CatalogueItem> CatalogueItems { get; set; }

        public ICollection<SupplierContact> SupplierContacts { get; set; }
    }
}

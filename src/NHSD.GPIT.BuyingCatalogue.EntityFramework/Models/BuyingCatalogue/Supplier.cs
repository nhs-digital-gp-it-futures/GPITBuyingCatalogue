using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class Supplier
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

        public virtual ICollection<CatalogueItem> CatalogueItems { get; set; }

        public virtual ICollection<SupplierContact> SupplierContacts { get; set; }
    }
}

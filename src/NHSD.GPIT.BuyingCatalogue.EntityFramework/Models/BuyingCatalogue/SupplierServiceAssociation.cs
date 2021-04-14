using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class SupplierServiceAssociation
    {
        public string AssociatedServiceId { get; set; }
        public string CatalogueItemId { get; set; }

        public virtual AssociatedService AssociatedService { get; set; }
        public virtual CatalogueItem CatalogueItem { get; set; }
    }
}

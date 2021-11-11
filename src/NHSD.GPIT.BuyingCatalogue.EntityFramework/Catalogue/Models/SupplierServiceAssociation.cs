using System;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class SupplierServiceAssociation : IAudited
    {
        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public CatalogueItemId AssociatedServiceId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public AssociatedService AssociatedService { get; set; }

        public CatalogueItem CatalogueItem { get; set; }
    }
}

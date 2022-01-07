using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public partial class CatalogueItem : IAudited
    {
        public CatalogueItem()
        {
            CatalogueItemCapabilities = new HashSet<CatalogueItemCapability>();
            CatalogueItemContacts = new HashSet<SupplierContact>();
            CataloguePrices = new HashSet<CataloguePrice>();
            CatalogueItemEpics = new HashSet<CatalogueItemEpic>();
            SupplierServiceAssociations = new HashSet<SupplierServiceAssociation>();
        }

        public CatalogueItemId Id { get; set; }

        public virtual string Name { get; set; }

        public int SupplierId { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public DateTime? LastPublished { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public virtual CatalogueItemType CatalogueItemType { get; set; }

        public PublicationStatus PublishedStatus { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual AdditionalService AdditionalService { get; set; }

        public virtual AssociatedService AssociatedService { get; set; }

        public virtual Solution Solution { get; set; }

        public ICollection<CataloguePrice> CataloguePrices { get; set; }

        public ICollection<CatalogueItemCapability> CatalogueItemCapabilities { get; set; }

        public ICollection<CatalogueItemEpic> CatalogueItemEpics { get; set; }

        public ICollection<SupplierServiceAssociation> SupplierServiceAssociations { get; set; }

        public ICollection<SupplierContact> CatalogueItemContacts { get; set; }
    }
}

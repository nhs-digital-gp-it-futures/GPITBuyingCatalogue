using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public partial class CatalogueItem
    {
        public CatalogueItem()
        {
            CataloguePrices = new HashSet<CataloguePrice>();
        }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string Name { get; set; }

        public string SupplierId { get; set; }

        public DateTime Created { get; set; }

        public virtual CatalogueItemType CatalogueItemType { get; set; }

        public PublicationStatus PublishedStatus { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual AdditionalService AdditionalService { get; set; }

        public virtual AssociatedService AssociatedService { get; set; }

        public virtual Solution Solution { get; set; }

        public ICollection<CataloguePrice> CataloguePrices { get; set; }
    }
}

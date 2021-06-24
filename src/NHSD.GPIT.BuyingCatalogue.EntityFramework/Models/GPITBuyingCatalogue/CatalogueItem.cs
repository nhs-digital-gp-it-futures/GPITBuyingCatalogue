using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class CatalogueItem
    {
        public CatalogueItem()
        {
            CataloguePrices = new HashSet<CataloguePrice>();
        }

        // TODO: Should be of type CatalogueItemId
        public string CatalogueItemId { get; set; }

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

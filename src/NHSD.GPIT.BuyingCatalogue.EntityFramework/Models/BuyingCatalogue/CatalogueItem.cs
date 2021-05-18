using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class CatalogueItem
    {
        public CatalogueItem()
        {
            CataloguePrices = new HashSet<CataloguePrice>();
        }

        public string CatalogueItemId { get; set; }

        public string Name { get; set; }

        public int CatalogueItemTypeId { get; set; }

        public string SupplierId { get; set; }

        public int PublishedStatusId { get; set; }

        public DateTime Created { get; set; }

        public virtual CatalogueItemType CatalogueItemType { get; set; }

        public virtual PublicationStatus PublishedStatus { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual AdditionalService AdditionalService { get; set; }

        public virtual AssociatedService AssociatedService { get; set; }

        public virtual Solution Solution { get; set; }

        public virtual ICollection<CataloguePrice> CataloguePrices { get; set; }
    }
}

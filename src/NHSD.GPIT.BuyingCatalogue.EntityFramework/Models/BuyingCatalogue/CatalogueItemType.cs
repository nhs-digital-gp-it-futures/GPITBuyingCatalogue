using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class CatalogueItemType
    {
        public CatalogueItemType()
        {
            CatalogueItems = new HashSet<CatalogueItem>();
        }

        public int CatalogueItemTypeId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CatalogueItem> CatalogueItems { get; set; }
    }
}

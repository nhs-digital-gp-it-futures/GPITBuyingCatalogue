using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class CataloguePriceType
    {
        public CataloguePriceType()
        {
            CataloguePrices = new HashSet<CataloguePrice>();
        }

        public int CataloguePriceTypeId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CataloguePrice> CataloguePrices { get; set; }
    }
}

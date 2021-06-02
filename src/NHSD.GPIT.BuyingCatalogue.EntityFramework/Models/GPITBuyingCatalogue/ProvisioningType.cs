using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class ProvisioningType
    {
        public ProvisioningType()
        {
            CataloguePrices = new HashSet<CataloguePrice>();
        }

        public int ProvisioningTypeId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CataloguePrice> CataloguePrices { get; set; }
    }
}

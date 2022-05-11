using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces
{
    public interface IPrice
    {
        public ICollection<IPriceTier> PriceTiers { get; }

        public string Description { get; }

        public ProvisioningType ProvisioningType { get; set; }

        public CataloguePriceCalculationType CataloguePriceCalculationType { get; set; }

        public TimeUnit? EstimationPeriod { get; }

        public CataloguePriceType CataloguePriceType { get; set; }

        public string CurrencyCode { get; set; }
    }
}

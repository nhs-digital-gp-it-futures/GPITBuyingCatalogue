using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces
{
    public interface IPrice
    {
        public ICollection<IPriceTier> PriceTiers { get; }

        public string Description { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public CataloguePriceCalculationType CataloguePriceCalculationType { get; set; }

        public CataloguePriceQuantityCalculationType? CataloguePriceQuantityCalculationType { get; set; }

        public TimeUnit? BillingPeriod { get; }

        public CataloguePriceType CataloguePriceType { get; set; }

        public string CurrencyCode { get; set; }

        public string RangeDescription { get; set; }

        public string ToPriceUnitString()
        {
            return $"{Description} {BillingPeriod?.Description() ?? string.Empty}".Trim();
        }

        public string GetRangeDescription(IPriceTier priceTier)
        {
            var upperRange = priceTier.UpperRange == null
                ? "+"
                : $" to {priceTier.UpperRange.Value}";

            return $"{priceTier.LowerRange}{upperRange} {RangeDescription}".Trim();
        }
    }
}

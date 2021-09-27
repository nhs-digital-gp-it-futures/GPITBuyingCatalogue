using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class SaveSolutionListPriceModel
    {
        public int CataloguePriceId { get; init; }

        public decimal? Price { get; init; }

        public ProvisioningType ProvisioningType { get; init; }

        public TimeUnit? TimeUnit { get; init; }

        public PricingUnit PricingUnit { get; init; }
    }
}

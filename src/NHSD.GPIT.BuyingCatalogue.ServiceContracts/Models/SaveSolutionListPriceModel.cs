using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class SaveSolutionListPriceModel
    {
        public int CataloguePriceId { get; set; }

        public decimal Price { get; set; }

        public string UnitDescription { get; set; }

        public string UnitDefinition { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public TimeUnit? TimeUnit { get; set; }

        public PricingUnit PricingUnit { get; set; }
    }
}

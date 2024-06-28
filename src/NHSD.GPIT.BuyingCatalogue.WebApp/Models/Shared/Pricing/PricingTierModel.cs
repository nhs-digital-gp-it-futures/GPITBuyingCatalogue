using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing
{
    public class PricingTierModel
    {
        public int Id { get; set; }

        public decimal ListPrice { get; set; }

        public string AgreedPrice { get; set; }

        public string Description { get; set; }

        public int LowerRange { get; set; }

        public int? UpperRange { get; set; }

        public PricingTierDto AgreedPriceDto => new PricingTierDto
        {
            LowerRange = LowerRange,
            UpperRange = UpperRange,
            Price = decimal.TryParse(AgreedPrice, out var price) ? price : decimal.Zero,
        };
    }
}

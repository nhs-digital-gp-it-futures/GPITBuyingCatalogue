namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public class PricingTierDto
    {
        public int LowerRange { get; set; }

        public int? UpperRange { get; set; }

        public decimal Price { get; set; }
    }
}

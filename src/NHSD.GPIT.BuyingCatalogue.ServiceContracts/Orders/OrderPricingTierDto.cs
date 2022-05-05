namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public class OrderPricingTierDto
    {
        public int LowerRange { get; set; }

        public int? UpperRange { get; set; }

        public decimal Price { get; set; }
    }
}

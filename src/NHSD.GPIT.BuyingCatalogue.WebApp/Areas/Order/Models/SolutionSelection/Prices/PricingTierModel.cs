using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices
{
    public class PricingTierModel
    {
        public int Id { get; set; }

        public decimal ListPrice { get; set; }

        public string AgreedPrice { get; set; }

        public string Description { get; set; }

        public int LowerRange { get; set; }

        public int? UpperRange { get; set; }

        public OrderPricingTierDto AgreedPriceDto => new OrderPricingTierDto
        {
            LowerRange = LowerRange,
            UpperRange = UpperRange,
            Price = decimal.TryParse(AgreedPrice, out var price) ? price : decimal.Zero,
        };
    }
}

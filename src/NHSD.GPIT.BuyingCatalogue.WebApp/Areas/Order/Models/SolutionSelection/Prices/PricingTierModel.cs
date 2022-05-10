namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices
{
    public class PricingTierModel
    {
        public PricingTierModel()
        {
        }

        public int Id { get; set; }

        public decimal ListPrice { get; set; }

        public string AgreedPrice { get; set; }

        public string Description { get; set; }

        public int LowerRange { get; set; }

        public int? UpperRange { get; set; }
    }
}

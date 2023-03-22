using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Prices.Base;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Prices
{
    public class ViewPriceModel : PricingModel
    {
        public const string AdviceText = "This is the price you originally agreed with the supplier and it cannot be changed.";

        public ViewPriceModel(CatalogueItem catalogueItem, int priceId, OrderItem orderItem)
            : base(catalogueItem, priceId, orderItem)
        {
        }

        public ViewPriceModel(OrderItem orderItem)
            : base(orderItem)
        {
        }

        public override string Advice => AdviceText;

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public string OnwardLink { get; set; }
    }
}

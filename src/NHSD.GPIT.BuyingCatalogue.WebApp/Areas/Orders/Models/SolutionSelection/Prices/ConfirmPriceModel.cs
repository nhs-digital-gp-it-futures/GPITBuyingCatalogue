using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Prices.Base;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Prices
{
    public class ConfirmPriceModel : PricingModel
    {
        public const string AdviceText = "Confirm the price you'll be paying. We've included the list price, but this can be changed if you’ve agreed a different rate with the supplier.";
        public const string HintText = "You can change the list price if you’ve agreed a different rate with the supplier.";
        public const string LabelText = "What price have you agreed, in pounds {0}?";

        public ConfirmPriceModel()
        {
        }

        public ConfirmPriceModel(CatalogueItem catalogueItem, int priceId, OrderItem orderItem)
            : base(catalogueItem, priceId, orderItem)
        {
        }

        public ConfirmPriceModel(OrderItem orderItem)
            : base(orderItem)
        {
        }

        public override string Advice => AdviceText;

        public string Hint => HintText;

        public string Label => string.Format(LabelText, Basis);

        public RoutingSource? Source { get; set; }

        public List<OrderPricingTierDto> AgreedPrices => Tiers?
            .Select(x => x.AgreedPriceDto)
            .ToList() ?? new List<OrderPricingTierDto>();
    }
}

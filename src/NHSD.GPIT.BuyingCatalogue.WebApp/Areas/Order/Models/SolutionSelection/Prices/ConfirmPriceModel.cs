using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices
{
    public class ConfirmPriceModel : NavBaseModel
    {
        public ConfirmPriceModel()
        {
        }

        public ConfirmPriceModel(CatalogueItem item, int priceId)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var price = item.CataloguePrices.Single(x => x.CataloguePriceId == priceId);

            Tiers = price.CataloguePriceTiers
                .OrderBy(x => x.LowerRange)
                .Select(x => new PricingTierModel
                {
                    Id = x.Id,
                    ListPrice = x.Price,
                    AgreedPrice = $"{x.Price:#,##0.00##}",
                    Description = x.GetRangeDescription(),
                    LowerRange = x.LowerRange,
                    UpperRange = x.UpperRange,
                })
                .ToArray();

            Basis = price.ToPriceUnitString();
            NumberOfTiers = price.CataloguePriceTiers.Count;
            ItemName = item.Name;
            ItemType = item.CatalogueItemType.Name();
        }

        public ConfirmPriceModel(OrderItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var price = item.OrderItemPrice;

            Tiers = price.OrderItemPriceTiers
                .OrderBy(x => x.LowerRange)
                .Select(x => new PricingTierModel
                {
                    Id = x.Id,
                    ListPrice = x.ListPrice,
                    AgreedPrice = $"{x.Price:#,##0.00##}",
                    Description = x.GetRangeDescription(),
                    LowerRange = x.LowerRange,
                    UpperRange = x.UpperRange,
                })
                .ToArray();

            Basis = price.ToPriceUnitString();
            NumberOfTiers = price.OrderItemPriceTiers.Count;
            ItemName = item.CatalogueItem.Name;
            ItemType = item.CatalogueItem.CatalogueItemType.Name();
        }

        public PricingTierModel[] Tiers { get; set; }

        public int NumberOfTiers { get; set; }

        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public string Basis { get; set; }
    }
}

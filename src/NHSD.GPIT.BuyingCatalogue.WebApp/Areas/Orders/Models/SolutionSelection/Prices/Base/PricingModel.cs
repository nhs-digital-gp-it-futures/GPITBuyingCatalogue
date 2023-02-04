using System;
using System.Globalization;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Prices.Base
{
    public abstract class PricingModel : NavBaseModel
    {
        public const string FourDecimalPlaces = "#,##0.00##";
        public const string TitleText = "Price of {0}";

        protected PricingModel()
        {
        }

        protected PricingModel(CatalogueItem item, int priceId, OrderItem orderItem)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var price = item.CataloguePrices.First(x => x.CataloguePriceId == priceId);

            var existingPrice = orderItem?.OrderItemPrice?.CataloguePriceId == priceId
                ? orderItem.OrderItemPrice
                : null;

            Tiers = GetTiers(price, existingPrice);
            PriceType = price.CataloguePriceType;
            CalculationType = price.CataloguePriceCalculationType;
            Basis = price.ToPriceUnitString();
            NumberOfTiers = price.CataloguePriceTiers.Count;
            ItemName = item.Name;
            ItemType = item.CatalogueItemType;
        }

        protected PricingModel(OrderItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var price = item.OrderItemPrice;

            Tiers = GetTiers(price);
            PriceType = price.CataloguePriceType;
            CalculationType = price.CataloguePriceCalculationType;
            Basis = price.ToPriceUnitString();
            NumberOfTiers = price.OrderItemPriceTiers.Count;
            ItemName = item.CatalogueItem.Name;
            ItemType = item.CatalogueItem.CatalogueItemType;
        }

        public override string Title => string.Format(TitleText, ItemType.Name());

        public override string Caption => ItemName;

        public CataloguePriceType PriceType { get; set; }

        public CataloguePriceCalculationType CalculationType { get; set; }

        public PricingTierModel[] Tiers { get; set; }

        public int NumberOfTiers { get; set; }

        public string ItemName { get; set; }

        public CatalogueItemType ItemType { get; set; }

        public string Basis { get; set; }

        private static PricingTierModel[] GetTiers(CataloguePrice price, OrderItemPrice existingPrice)
        {
            return price.CataloguePriceTiers
                .OrderBy(x => x.LowerRange)
                .Select(x => new PricingTierModel
                {
                    Id = x.Id,
                    ListPrice = x.Price,
                    AgreedPrice = AgreedPrice(x, existingPrice),
                    Description = x.GetRangeDescription(),
                    LowerRange = x.LowerRange,
                    UpperRange = x.UpperRange,
                })
                .ToArray();
        }

        private static PricingTierModel[] GetTiers(OrderItemPrice price)
        {
            return price.OrderItemPriceTiers
                .OrderBy(x => x.LowerRange)
                .Select(x => new PricingTierModel
                {
                    Id = x.Id,
                    ListPrice = x.ListPrice,
                    AgreedPrice = x.Price.ToString(FourDecimalPlaces, CultureInfo.InvariantCulture),
                    Description = x.GetRangeDescription(),
                    LowerRange = x.LowerRange,
                    UpperRange = x.UpperRange,
                })
                .ToArray();
        }

        private static string AgreedPrice(CataloguePriceTier tier, OrderItemPrice existingPrice)
        {
            var existingTier = existingPrice?.OrderItemPriceTiers?
                .FirstOrDefault(x => x.LowerRange == tier.LowerRange && x.UpperRange == tier.UpperRange);

            return existingTier == null
                ? tier.Price.ToString(FourDecimalPlaces, CultureInfo.InvariantCulture)
                : existingTier.Price.ToString(FourDecimalPlaces, CultureInfo.InvariantCulture);
        }
    }
}

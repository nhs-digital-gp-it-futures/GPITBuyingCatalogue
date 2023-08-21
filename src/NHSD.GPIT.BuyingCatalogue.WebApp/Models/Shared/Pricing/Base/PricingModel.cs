using System;
using System.Globalization;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing.Base
{
    public class PricingModel : NavBaseModel
    {
        public const string FourDecimalPlaces = "#,##0.00##";
        public const string TitleText = "Price of {0}";

        public PricingModel()
        {
        }

        public PricingModel(CatalogueItem item, IPrice price, IPrice existingPrice)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(price);

            Tiers = GetTiers(price, existingPrice);
            PriceType = price.CataloguePriceType;
            CalculationType = price.CataloguePriceCalculationType;
            Basis = price.ToPriceUnitString();
            NumberOfTiers = price.PriceTiers.Count;
            ItemName = item.Name;
            ItemType = item.CatalogueItemType;
        }

        public PricingModel(IPrice price, CatalogueItem catalogueItem)
        {
            ArgumentNullException.ThrowIfNull(price);

            Tiers = GetTiers(price);
            PriceType = price.CataloguePriceType;
            CalculationType = price.CataloguePriceCalculationType;
            Basis = price.ToPriceUnitString();
            NumberOfTiers = price.PriceTiers.Count;
            ItemName = catalogueItem.Name;
            ItemType = catalogueItem.CatalogueItemType;
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

        private static PricingTierModel[] GetTiers(IPrice price, IPrice existingPrice)
        {
            return price.PriceTiers
                .OrderBy(x => x.LowerRange)
                .Select(x => new PricingTierModel
                {
                    Id = x.Id,
                    ListPrice = x.Price,
                    AgreedPrice = AgreedPrice(x, existingPrice),
                    Description = price.GetRangeDescription(x),
                    LowerRange = x.LowerRange,
                    UpperRange = x.UpperRange,
                })
                .ToArray();
        }

        private static PricingTierModel[] GetTiers(IPrice price)
        {
            return price.PriceTiers
                .OrderBy(x => x.LowerRange)
                .Select(x => new PricingTierModel
                {
                    Id = x.Id,
                    ListPrice = x.Price,
                    AgreedPrice = x.Price.ToString(FourDecimalPlaces, CultureInfo.InvariantCulture),
                    Description = price.GetRangeDescription(x),
                    LowerRange = x.LowerRange,
                    UpperRange = x.UpperRange,
                })
                .ToArray();
        }

        private static string AgreedPrice(IPriceTier tier, IPrice existingPrice)
        {
            var existingTier = existingPrice?.PriceTiers?
                .FirstOrDefault(x => x.LowerRange == tier.LowerRange && x.UpperRange == tier.UpperRange);

            return existingTier == null
                ? tier.Price.ToString(FourDecimalPlaces, CultureInfo.InvariantCulture)
                : existingTier.Price.ToString(FourDecimalPlaces, CultureInfo.InvariantCulture);
        }
    }
}

using System.Globalization;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing.Base;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Prices.Base
{
    public class PricingModelTests
    {
        [Theory]
        [MockAutoData]
        public void WithValidCatalogueItem_PropertiesCorrectlySet(CatalogueItem item)
        {
            var price = item.CataloguePrices.First();

            var model = new PricingModel(item.CataloguePrices.First(), item);

            model!.Title.Should().Be(string.Format(PricingModel.TitleText, model.ItemType.Name()));
            model.Caption.Should().Be(model.ItemName);
            model.Basis.Should().Be(price.ToPriceUnitString());
            model.CalculationType.Should().Be(price.CataloguePriceCalculationType);
            model.ItemName.Should().Be(item.Name);
            model.ItemType.Should().Be(item.CatalogueItemType);
            model.NumberOfTiers.Should().Be(price.CataloguePriceTiers.Count);
            model.PriceType.Should().Be(price.CataloguePriceType);

            foreach (var tier in model.Tiers)
            {
                var pricingTier = price.CataloguePriceTiers.First(x => x.Id == tier.Id);

                tier.AgreedPrice.Should().Be(pricingTier.Price.ToString(PricingModel.FourDecimalPlaces, CultureInfo.InvariantCulture));
                tier.Description.Should().Be(pricingTier.GetRangeDescription());
                tier.ListPrice.Should().Be(pricingTier.Price);
                tier.LowerRange.Should().Be(pricingTier.LowerRange);
                tier.UpperRange.Should().Be(pricingTier.UpperRange);
            }
        }

        [Theory]
        [MockAutoData]
        public void WithValidCatalogueItem_AndExistingOrderItem_PropertiesCorrectlySet(CatalogueItem item, OrderItem orderItem)
        {
            var price = item.CataloguePrices.First();

            orderItem.OrderItemPrice.CataloguePriceId = price.CataloguePriceId;
            orderItem.OrderItemPrice.OrderItemPriceTiers = price.CataloguePriceTiers
                .Select(x => new OrderItemPriceTier(orderItem.OrderItemPrice, x)
                {
                    Price = x.Price / 2,
                })
                .ToList();

            var model = new PricingModel(item, price, orderItem.OrderItemPrice);

            model!.Title.Should().Be(string.Format(PricingModel.TitleText, model.ItemType.Name()));
            model.Caption.Should().Be(model.ItemName);
            model.Basis.Should().Be(price.ToPriceUnitString());
            model.CalculationType.Should().Be(price.CataloguePriceCalculationType);
            model.ItemName.Should().Be(item.Name);
            model.ItemType.Should().Be(item.CatalogueItemType);
            model.NumberOfTiers.Should().Be(price.CataloguePriceTiers.Count);
            model.PriceType.Should().Be(price.CataloguePriceType);

            foreach (var tier in model.Tiers)
            {
                var pricingTier = price.CataloguePriceTiers.First(x => x.Id == tier.Id);
                var tierPrice = pricingTier.Price / 2;

                tier.AgreedPrice.Should().Be(tierPrice.ToString(PricingModel.FourDecimalPlaces, CultureInfo.InvariantCulture));
                tier.Description.Should().Be(pricingTier.GetRangeDescription());
                tier.ListPrice.Should().Be(pricingTier.Price);
                tier.LowerRange.Should().Be(pricingTier.LowerRange);
                tier.UpperRange.Should().Be(pricingTier.UpperRange);
            }
        }
    }
}

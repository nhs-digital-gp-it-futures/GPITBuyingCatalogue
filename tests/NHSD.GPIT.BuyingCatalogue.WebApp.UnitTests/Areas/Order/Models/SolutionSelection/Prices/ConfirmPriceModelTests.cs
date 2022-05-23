using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.Prices
{
    public static class ConfirmPriceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void CatalogueItemIsNull_ThrowsException(int priceId)
        {
            FluentActions
                .Invoking(() => new ConfirmPriceModel(null, priceId))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public static void OrderItemIsNull_ThrowsException()
        {
            FluentActions
                .Invoking(() => new ConfirmPriceModel(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidCatalogueItem_PropertiesCorrectlySet(CatalogueItem item)
        {
            var price = item.CataloguePrices.First();

            var model = new ConfirmPriceModel(item, price.CataloguePriceId);

            model.Basis.Should().Be(price.ToPriceUnitString());
            model.ItemName.Should().Be(item.Name);
            model.ItemType.Should().Be(item.CatalogueItemType);
            model.NumberOfTiers.Should().Be(price.CataloguePriceTiers.Count);

            foreach (var tier in model.Tiers)
            {
                var pricingTier = price.CataloguePriceTiers.Single(x => x.Id == tier.Id);

                tier.AgreedPrice.Should().Be($"{pricingTier.Price:#,##0.00##}");
                tier.Description.Should().Be(pricingTier.GetRangeDescription());
                tier.ListPrice.Should().Be(pricingTier.Price);
                tier.LowerRange.Should().Be(pricingTier.LowerRange);
                tier.UpperRange.Should().Be(pricingTier.UpperRange);
            }
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidOrderItem_PropertiesCorrectlySet(OrderItem item)
        {
            var price = item.OrderItemPrice;

            var model = new ConfirmPriceModel(item);

            model.Basis.Should().Be(price.ToPriceUnitString());
            model.ItemName.Should().Be(item.CatalogueItem.Name);
            model.ItemType.Should().Be(item.CatalogueItem.CatalogueItemType);
            model.NumberOfTiers.Should().Be(price.OrderItemPriceTiers.Count);

            foreach (var tier in model.Tiers)
            {
                var pricingTier = price.OrderItemPriceTiers.Single(x => x.Id == tier.Id);

                tier.AgreedPrice.Should().Be($"{pricingTier.Price:#,##0.00##}");
                tier.Description.Should().Be(pricingTier.GetRangeDescription());
                tier.ListPrice.Should().Be(pricingTier.ListPrice);
                tier.LowerRange.Should().Be(pricingTier.LowerRange);
                tier.UpperRange.Should().Be(pricingTier.UpperRange);
            }
        }
    }
}

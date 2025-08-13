using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering;

public static class OrderItemPriceTests
{
    public static IEnumerable<object[]> TieredPricingTestData => new object[][]
    {
        [
            760_644,
            0,
            new List<PriceCalculationModel>
            {
                new(1, 89_999, 0.1660M),
                new(2, 670_645, 0.1330M),
                new(3, 0, 0.08M, 0),
            },
        ],
        [
            16_277,
            760_644,
            new List<PriceCalculationModel>
            {
                new(1, 0, 0.1660M, 0),
                new(2, 16_277, 0.1330M),
                new(3, 0, 0.08M, 0),
            },
        ],
        [
            1_792_389,
            0,
            new List<PriceCalculationModel>
            {
                new(1, 89_999, 0.1660M),
                new(2, 810_000, 0.1330M),
                new(3, 892_390, 0.08M),
            },
        ],
        [
            61_324,
            1_792_389,
            new List<PriceCalculationModel>
            {
                new(1, 0, 0.1660M, 0),
                new(2, 0, 0.1330M, 0),
                new(3, 61_324, 0.08M),
            },
        ],
    };

    [Theory]
    [MockAutoData]
    public static void Construct_WithPrice_SetsPropertiesAsExpected(
        IPrice price)
    {
        var model = new OrderItemPrice(price);

        model.CataloguePriceId.Should().Be(price.CataloguePriceId);
        model.ProvisioningType.Should().Be(price.ProvisioningType);
        model.CataloguePriceType.Should().Be(price.CataloguePriceType);
        model.CataloguePriceCalculationType.Should().Be(price.CataloguePriceCalculationType);
        model.CataloguePriceQuantityCalculationType.Should().Be(price.CataloguePriceQuantityCalculationType);
        model.BillingPeriod.Should().Be(price.BillingPeriod);
        model.CurrencyCode.Should().Be(price.CurrencyCode);
        model.Description.Should().Be(price.Description);
        model.RangeDescription.Should().Be(price.RangeDescription);
    }

    [Theory]
    [MockMemberAutoData(nameof(TieredPricingTestData))]
    public static void CalculateCumulativeCostPerTier_ReturnsExpectedPrice(
        int quantity,
        int quantityOffset,
        List<PriceCalculationModel> expectedPriceCalculations,
        OrderItemPrice orderItemPrice)
    {
        var expectedPrice = expectedPriceCalculations.Sum(x => x.Cost) / 12;

        orderItemPrice.BillingPeriod = TimeUnit.PerYear;
        orderItemPrice.CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative;
        orderItemPrice.CataloguePriceType = CataloguePriceType.Tiered;

        orderItemPrice.OrderItemPriceTiers = new List<OrderItemPriceTier>
        {
            new() { LowerRange = 1, UpperRange = 89_999, Price = 0.1660M, },
            new() { LowerRange = 90_000, UpperRange = 899_999, Price = 0.1330M, },
            new() { LowerRange = 900_000, UpperRange = null, Price = 0.08M, },
        };

        var total = ((IPrice)orderItemPrice).CalculateCostPerMonth(quantity, quantityOffset);

        total.Should().Be(expectedPrice);
    }

    [Theory]
    [MockMemberAutoData(nameof(TieredPricingTestData))]
    public static void CalculateCostPerTier_ReturnsExpectedTiers(
        int quantity,
        int quantityOffset,
        List<PriceCalculationModel> expectedPriceCalculations,
        OrderItemPrice orderItemPrice)
    {
        orderItemPrice.BillingPeriod = TimeUnit.PerYear;
        orderItemPrice.CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative;
        orderItemPrice.CataloguePriceType = CataloguePriceType.Tiered;

        orderItemPrice.OrderItemPriceTiers = new List<OrderItemPriceTier>
        {
            new() { LowerRange = 1, UpperRange = 89_999, Price = 0.1660M, },
            new() { LowerRange = 90_000, UpperRange = 899_999, Price = 0.1330M, },
            new() { LowerRange = 900_000, UpperRange = null, Price = 0.08M, },
        };

        var tiers = ((IPrice)orderItemPrice).CalculateCostPerTier(quantity, quantityOffset);

        tiers.Should().HaveCount(expectedPriceCalculations.Count);
        tiers.Should().BeEquivalentTo(expectedPriceCalculations);
    }
}

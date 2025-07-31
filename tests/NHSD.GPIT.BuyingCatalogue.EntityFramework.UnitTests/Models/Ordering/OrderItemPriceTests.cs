using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering;

public static class OrderItemPriceTests
{
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
    [MockAutoData]
    public static void CalculateCumulativeCostPerTier_WithNoOffset_ReturnsExpectedPrice(
        OrderItemPrice orderItemPrice)
    {
        const int quantity = 760_644;
        const decimal expectedPrice = 8_677.96825M;

        orderItemPrice.BillingPeriod = TimeUnit.PerYear;
        orderItemPrice.CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative;
        orderItemPrice.CataloguePriceType = CataloguePriceType.Tiered;

        orderItemPrice.OrderItemPriceTiers = new List<OrderItemPriceTier>
        {
            new() { LowerRange = 1, UpperRange = 89_999, Price = 0.1660M, },
            new() { LowerRange = 90_000, UpperRange = 899_999, Price = 0.1330M, },
            new() { LowerRange = 900_000, UpperRange = null, Price = 0.08M, },
        };

        var total = ((IPrice)orderItemPrice).CalculateCostPerMonth(quantity);

        total.Should().Be(expectedPrice);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateCumulativeCostPerTier_WithOffset_ReturnsExpectedPrice(
        OrderItemPrice orderItemPrice)
    {
        const int quantity = 16_277;
        const int quantityOffset = 760_644;
        const decimal expectedPrice = 180.4034M;

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

        total.Should().BeApproximately(expectedPrice, 0.0001M);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateCostPerTier_WithNoOffset_ReturnsExpectedTiers(
        OrderItemPrice orderItemPrice)
    {
        const int quantity = 760_644;

        orderItemPrice.BillingPeriod = TimeUnit.PerYear;
        orderItemPrice.CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative;
        orderItemPrice.CataloguePriceType = CataloguePriceType.Tiered;

        orderItemPrice.OrderItemPriceTiers = new List<OrderItemPriceTier>
        {
            new() { LowerRange = 1, UpperRange = 89_999, Price = 0.1660M, },
            new() { LowerRange = 90_000, UpperRange = 899_999, Price = 0.1330M, },
            new() { LowerRange = 900_000, UpperRange = null, Price = 0.08M, },
        };

        var expectedPriceCalculations = new List<PriceCalculationModel>
        {
            new(1, 89_999, 0.1660M, 89_999 * 0.1660M),
            new(2, 670_645, 0.1330M, 670_645 * 0.1330M),
            new(3, 0, 0.08M, 0),
        };

        var tiers = ((IPrice)orderItemPrice).CalculateCostPerTier(quantity);

        tiers.Should().HaveCount(3);
        tiers.Should().BeEquivalentTo(expectedPriceCalculations);
    }

    [Theory]
    [MockAutoData]
    public static void CalculateCostPerTier_WithOffset_ReturnsExpectedTiers(
        OrderItemPrice orderItemPrice)
    {
        const int quantity = 16_277;
        const int quantityOffset = 760_644;

        orderItemPrice.BillingPeriod = TimeUnit.PerYear;
        orderItemPrice.CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative;
        orderItemPrice.CataloguePriceType = CataloguePriceType.Tiered;

        orderItemPrice.OrderItemPriceTiers = new List<OrderItemPriceTier>
        {
            new() { LowerRange = 1, UpperRange = 89_999, Price = 0.1660M, },
            new() { LowerRange = 90_000, UpperRange = 899_999, Price = 0.1330M, },
            new() { LowerRange = 900_000, UpperRange = null, Price = 0.08M, },
        };

        var expectedPriceCalculations = new List<PriceCalculationModel>
        {
            new(1, 0, 0.1660M, 0),
            new(2, quantity, 0.1330M, quantity * 0.1330M),
            new(3, 0, 0.08M, 0),
        };

        var tiers = ((IPrice)orderItemPrice).CalculateCostPerTier(quantity, quantityOffset);

        tiers.Should().HaveCount(2);
    }
}

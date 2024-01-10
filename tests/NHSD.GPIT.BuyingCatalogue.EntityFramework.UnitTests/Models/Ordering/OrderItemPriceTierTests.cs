using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering;

public static class OrderItemPriceTierTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_WithOrderablePriceTier_SetsPropertiesAsExpected(
        IOrderablePriceTier priceTier)
    {
        var model = new OrderItemPriceTier(priceTier);

        model.Price.Should().Be(priceTier.Price);
        model.ListPrice.Should().Be(priceTier.ListPrice);
        model.LowerRange.Should().Be(priceTier.LowerRange);
        model.UpperRange.Should().Be(priceTier.UpperRange);
    }

    [Theory]
    [CommonAutoData]
    public static void GetRangeDescription_NoUpperRangeWithRangeDescription_ReturnsExpected(
        string rangeDescription,
        OrderItemPrice price,
        OrderItemPriceTier tier)
    {
        price.RangeDescription = rangeDescription;

        tier.UpperRange = null;
        tier.OrderItemPrice = price;

        tier.GetRangeDescription().Should().Be($"{tier.LowerRange}+ {rangeDescription}");
    }

    [Theory]
    [CommonAutoData]
    public static void GetRangeDescription_WithUpperRangeWithRangeDescription_ReturnsExpected(
        string rangeDescription,
        int upperRange,
        OrderItemPrice price,
        OrderItemPriceTier tier)
    {
        price.RangeDescription = rangeDescription;

        tier.UpperRange = upperRange;
        tier.OrderItemPrice = price;

        tier.GetRangeDescription().Should().Be($"{tier.LowerRange} to {upperRange} {rangeDescription}");
    }

    [Theory]
    [CommonAutoData]
    public static void GetRangeDescription_NoUpperRangeNoRangeDescription_ReturnsExpected(
        OrderItemPrice price,
        OrderItemPriceTier tier)
    {
        price.RangeDescription = null;

        tier.UpperRange = null;
        tier.OrderItemPrice = price;

        tier.GetRangeDescription().Should().Be($"{tier.LowerRange}+");
    }

    [Theory]
    [CommonAutoData]
    public static void GetRangeDescription_NoUpperRangeNoOrderItemPrice_ReturnsExpected(
        OrderItemPriceTier tier)
    {
        tier.UpperRange = null;
        tier.OrderItemPrice = null;

        tier.GetRangeDescription().Should().Be($"{tier.LowerRange}+");
    }
}

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
}

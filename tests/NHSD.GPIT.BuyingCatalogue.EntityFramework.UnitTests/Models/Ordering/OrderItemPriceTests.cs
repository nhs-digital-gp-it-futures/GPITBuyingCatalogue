using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering;

public static class OrderItemPriceTests
{
    [Theory]
    [CommonAutoData]
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
}

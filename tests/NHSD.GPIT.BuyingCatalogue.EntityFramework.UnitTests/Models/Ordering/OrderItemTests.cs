using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering;

public static class OrderItemTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(CatalogueItemId catalogueItemId)
    {
        var orderItem = new OrderItem(catalogueItemId);

        orderItem.CatalogueItemId.Should().Be(catalogueItemId);
    }
}

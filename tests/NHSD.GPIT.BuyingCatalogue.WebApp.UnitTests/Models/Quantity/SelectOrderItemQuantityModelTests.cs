using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Quantity
{
    public static class SelectOrderItemQuantityModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidOrderItem_PropertiesCorrectlySet(OrderItem item)
        {
            var model = new SelectOrderItemQuantityModel(item.CatalogueItem, item.OrderItemPrice, item.Quantity);

            model.ItemName.Should().Be(item.CatalogueItem.Name);
            model.ItemType.Should().Be(item.CatalogueItem.CatalogueItemType.Description());
            model.Quantity.Should().Be($"{item.Quantity}");
            model.QuantityDescription.Should().Be(item.OrderItemPrice.RangeDescription);
        }
    }
}

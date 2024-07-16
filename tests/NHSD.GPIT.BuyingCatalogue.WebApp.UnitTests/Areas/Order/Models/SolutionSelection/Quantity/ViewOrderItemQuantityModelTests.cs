using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.Quantity
{
    public static class ViewOrderItemQuantityModelTests
    {
        [Fact]
        public static void OrderItemIsNull_ThrowsException()
        {
            FluentActions
                .Invoking(() => new ViewOrderItemQuantityModel(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MockAutoData]
        public static void WithValidOrderItem_PropertiesCorrectlySet(OrderItem item)
        {
            var model = new ViewOrderItemQuantityModel(item);

            model.Title.Should().Be(string.Format(ViewOrderItemQuantityModel.TitleText, model.ItemType));
            model.Caption.Should().Be(model.ItemName);
            model.Advice.Should().Be(ViewOrderItemQuantityModel.AdviceText);
            model.ItemName.Should().Be(item.CatalogueItem.Name);
            model.ItemType.Should().Be(item.CatalogueItem.CatalogueItemType.Description());
            model.Quantity.Should().Be($"{item.Quantity:N0}");
            model.QuantityDescription.Should().Be(item.OrderItemPrice.RangeDescription);
        }
    }
}

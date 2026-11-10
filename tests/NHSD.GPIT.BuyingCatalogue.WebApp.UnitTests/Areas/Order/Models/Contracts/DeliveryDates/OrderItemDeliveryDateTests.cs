using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;
using OrderEntity = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.Order;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates;

public static class OrderItemDeliveryDateTests
{
    [Fact]
    public static void Constructor_WithAssociatedServiceAndAdditionalServiceParent_SetsCombinedCatalogueItemName()
    {
        var parentOrderItem = new OrderItem
        {
            CatalogueItem = new CatalogueItem
            {
                Name = "Additional service",
                CatalogueItemType = CatalogueItemType.AdditionalService,
            },
        };
        var orderItem = new OrderItem
        {
            CatalogueItem = new CatalogueItem
            {
                Name = "Associated service",
                CatalogueItemType = CatalogueItemType.AssociatedService,
            },
            Parent = parentOrderItem,
        };

        var model = new OrderItemDeliveryDate(1, orderItem, null, new OrderEntity());

        model.CatalogueItemName.Should().Be("Additional service - Associated service");
    }

    [Fact]
    public static void Constructor_WithNonAssociatedService_SetsCatalogueItemName()
    {
        var orderItem = new OrderItem
        {
            CatalogueItem = new CatalogueItem
            {
                Name = "Catalogue solution",
                CatalogueItemType = CatalogueItemType.Solution,
            },
        };

        var model = new OrderItemDeliveryDate(1, orderItem, null, new OrderEntity());

        model.CatalogueItemName.Should().Be("Catalogue solution");
    }

    [Fact]
    public static void Constructor_WithAssociatedServiceAndNonAdditionalServiceParent_SetsAssociatedServiceName()
    {
        var parentOrderItem = new OrderItem
        {
            CatalogueItem = new CatalogueItem
            {
                Name = "Catalogue solution",
                CatalogueItemType = CatalogueItemType.Solution,
            },
        };
        var orderItem = new OrderItem
        {
            CatalogueItem = new CatalogueItem
            {
                Name = "Associated service",
                CatalogueItemType = CatalogueItemType.AssociatedService,
            },
            Parent = parentOrderItem,
        };

        var model = new OrderItemDeliveryDate(1, orderItem, null, new OrderEntity());

        model.CatalogueItemName.Should().Be("Associated service");
    }
}

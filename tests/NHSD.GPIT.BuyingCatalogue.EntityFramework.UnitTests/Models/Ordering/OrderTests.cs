using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering;

public static class OrderTests
{
    [Fact]
    public static void OrderStatus_Deleted()
    {
        var order = new Order { IsDeleted = true };

        order.OrderStatus.Should().Be(OrderStatus.Deleted);
    }

    [Fact]
    public static void OrderStatus_Completed()
    {
        var order = new Order { Completed = DateTime.UtcNow };

        order.OrderStatus.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public static void OrderStatus_InProgress()
    {
        var order = new Order();

        order.OrderStatus.Should().Be(OrderStatus.InProgress);
    }

    [Theory]
    [CommonAutoData]
    public static void Order_GetSolutions_ReturnsExpectedResult(
        List<OrderItem> orderItems)
    {
        foreach (var item in orderItems)
        {
            item.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
        }

        orderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;

        var order = new Order { OrderItems = orderItems };

        var result = order.GetSolutions().As<IEnumerable<OrderItem>>();
        result.Should().NotBeNullOrEmpty();
        result.Count().Should().Be(orderItems.Count - 1);
    }

    [Fact]
    public static void ContractExpired_NoEndDate_PropertyCorrectlySet()
    {
        var order = new Order { MaximumTerm = null };

        order.ContractExpired.Should().Be(false);
    }

    [Theory]
    [CommonInlineAutoData(-1, true)]
    [CommonInlineAutoData(0, false)]
    [CommonInlineAutoData(1, false)]
    public static void ContractExpired_PropertyCorrectlySet(
        int remainingDaysOfContract,
        bool value)
    {
        const int maxTerm = 36;

        var order = new Order { MaximumTerm = maxTerm, CommencementDate = DateTime.UtcNow.AddMonths(-maxTerm).AddDays(1 + remainingDaysOfContract) };

        order.ContractExpired.Should().Be(value);
    }
}

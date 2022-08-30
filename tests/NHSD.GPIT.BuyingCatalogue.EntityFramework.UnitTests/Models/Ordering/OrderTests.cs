using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
}

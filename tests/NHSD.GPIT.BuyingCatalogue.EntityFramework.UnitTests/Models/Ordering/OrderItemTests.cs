using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering;

public static class OrderItemTests
{
    private const int MaximumTerm = 36;
    private static readonly DateTime CommencementDate = new DateTime(2022, 05, 12, 12, 0, 0);

    public static IEnumerable<object[]> GetCentralAllocationDateTestData
        => new[]
        {
            new object[] { 1000, 200, CommencementDate.AddDays(219) }, // Central allocation is 20% of the item price
            new object[] { 1000, 500, CommencementDate.AddDays(547.5) }, // Central allocation is 50% of the item price
            new object[] { 1000, 700, CommencementDate.AddDays(766.5) }, // Central allocation is 70% of the item price
        };

    [Theory]
    [CommonMemberAutoData(nameof(GetCentralAllocationDateTestData))]
    public static void GetCentralAllocationDate_ExpectedResults(
        int totalCost,
        int centralAllocation,
        DateTime forecastedDate,
        OrderItem orderItem)
    {
        orderItem.Order = new()
        {
            CommencementDate = CommencementDate,
            MaximumTerm = MaximumTerm,
        };

        orderItem.OrderItemFunding = new()
        {
            CentralAllocation = centralAllocation,
            LocalAllocation = totalCost - centralAllocation,
        };

        var actualDate = orderItem.GetCentralAllocationEndDate(totalCost);

        actualDate.Should()
            .HaveDay(forecastedDate.Day)
            .And.HaveMonth(forecastedDate.Month)
            .And.HaveYear(forecastedDate.Year);
    }
}

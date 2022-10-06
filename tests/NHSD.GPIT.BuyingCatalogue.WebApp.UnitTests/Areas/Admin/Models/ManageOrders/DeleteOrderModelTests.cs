using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ManageOrders
{
    public static class DeleteOrderModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new DeleteOrderModel(order);

            model.CallOffId.Should().Be(order.CallOffId);
        }

        [Theory]
        [CommonAutoData]
        public static void WithNullOrderDeletionApproval_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderDeletionApproval = null;
            var model = new DeleteOrderModel(order);

            model.ApprovalDay.Should().BeNull();
            model.ApprovalMonth.Should().BeNull();
            model.ApprovalYear.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void WithOrderDeletionApproval_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            const int testDay = 1;
            const int testMonth = 1;
            const int testYear = 2022;

            var oda = new OrderDeletionApproval() { DateOfApproval = new DateTime(testYear, testMonth, testDay) };
            order.OrderDeletionApproval = oda;
            var model = new DeleteOrderModel(order);

            model.ApprovalDate.Should().NotBeNull();
            model.ApprovalDay.Should().Be(testDay.ToString("00"));
            model.ApprovalMonth.Should().Be(testMonth.ToString("00"));
            model.ApprovalYear.Should().Be(testYear.ToString("0000"));
        }

        [Theory]
        [CommonAutoData]
        public static void ApprovalDate_InvalidDayMonthYear_ReturnsNull(
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new DeleteOrderModel(order);
            model.ApprovalDay = "A";
            model.ApprovalMonth = "B";
            model.ApprovalYear = "C";

            model.ApprovalDate.Should().BeNull();
        }
    }
}

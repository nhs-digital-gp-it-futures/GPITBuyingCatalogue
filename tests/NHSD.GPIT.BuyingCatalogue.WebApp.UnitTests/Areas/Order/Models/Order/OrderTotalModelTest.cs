using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class OrderTotalModelTest
    {
        [Theory]
        [MockAutoData]
        public static void Properties_Set_With_OneOffCostOnly_False(
            OrderType orderType,
            int? maximumTerm,
            decimal totalOneOffCost,
            decimal totalMonthlyCost,
            decimal totalAnnualCost,
            decimal totalCost)
        {
            var model = new OrderTotalModel(
                orderType,
                maximumTerm,
                totalOneOffCost,
                totalMonthlyCost,
                totalAnnualCost,
                totalCost);

            model.OrderType.Should().Be(orderType);
            model.MaximumTerm.Should().Be(maximumTerm);
            model.TotalOneOffCost.Should().Be(totalOneOffCost);
            model.TotalMonthlyCost.Should().Be(totalMonthlyCost);
            model.TotalAnnualCost.Should().Be(totalAnnualCost);
            model.TotalCost.Should().Be(totalCost);
            model.OneOffCostOnly.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void Properties_Set_With_OneOffCostOnly_True(
            OrderType orderType,
            int? maximumTerm,
            decimal totalOneOffCost,
            decimal totalCost)
        {
            var model = new OrderTotalModel(
                orderType,
                maximumTerm,
                totalOneOffCost,
                0M,
                0M,
                totalCost);

            model.OrderType.Should().Be(orderType);
            model.MaximumTerm.Should().Be(maximumTerm);
            model.TotalOneOffCost.Should().Be(totalOneOffCost);
            model.TotalMonthlyCost.Should().Be(0M);
            model.TotalAnnualCost.Should().Be(0M);
            model.TotalCost.Should().Be(totalCost);
            model.OneOffCostOnly.Should().BeTrue();
        }
    }
}

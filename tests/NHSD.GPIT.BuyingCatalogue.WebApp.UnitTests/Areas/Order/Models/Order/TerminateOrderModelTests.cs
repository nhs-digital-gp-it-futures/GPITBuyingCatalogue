using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class TerminateOrderModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            CallOffId callOffId)
        {
            var model = new TerminateOrderModel(internalOrgId, callOffId);

            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
        }

        [Fact]
        public static void WithDayMonthYear_DatePropertiesCorrectlySet()
        {
            var model = new TerminateOrderModel() { Day = "01", Month = "07", Year = "2023", };

            model.TerminationDate.Should().NotBeNull();
            model.TerminationDate.Value.Day.Should().Be(1);
            model.TerminationDate.Value.Month.Should().Be(7);
            model.TerminationDate.Value.Year.Should().Be(2023);
        }

        [Theory]
        [MockAutoData]
        public static void WithInvalidDayMonthYear_DatePropertyNull(
            string day,
            string month,
            string year)
        {
            var model = new TerminateOrderModel() { Day = day, Month = month, Year = year, };

            model.TerminationDate.Should().BeNull();
        }
    }
}

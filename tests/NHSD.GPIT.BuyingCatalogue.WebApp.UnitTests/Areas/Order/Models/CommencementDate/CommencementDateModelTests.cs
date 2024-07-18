using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.CommencementDate;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.CommencementDate
{
    public static class CommencementDateModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            int maximumTermUpperLimit,
            EntityFramework.Ordering.Models.Order order,
            DateTime commencementDate)
        {
            order.CommencementDate = commencementDate;
            var model = new CommencementDateModel(internalOrgId, order, maximumTermUpperLimit);

            model.InternalOrgId.Should().Be(internalOrgId);
            model.IsAmendment.Should().Be(order.IsAmendment);
            model.Day.Should().Be(order.CommencementDate!.Value.Day.ToString("00"));
            model.Month.Should().Be(order.CommencementDate!.Value.Month.ToString("00"));
            model.Year.Should().Be(order.CommencementDate!.Value.Year.ToString("00"));
            model.InitialPeriodValue.Should().Be(order.InitialPeriod);
            model.MaximumTermValue.Should().Be(order.MaximumTerm);
        }

        [Theory]
        [MockAutoData]
        public static void CommencementDate_WithValidDate_ParsesSuccessfully(
            CommencementDateModel model)
        {
            var date = DateTime.UtcNow;

            model.Day = date.Day.ToString();
            model.Month = date.Month.ToString();
            model.Year = date.Year.ToString();

            var deliveryDate = model.Date!.Value;

            deliveryDate.Day.Should().Be(date.Day);
            deliveryDate.Month.Should().Be(date.Month);
            deliveryDate.Year.Should().Be(date.Year);
        }

        [Theory]
        [MockAutoData]
        public static void CommencementDate_WithInvalidDate_DoesNotParse(
            CommencementDateModel model)
        {
            model.Day = model.Month = model.Year = null;

            model.Date.HasValue.Should().BeFalse();
        }
    }
}

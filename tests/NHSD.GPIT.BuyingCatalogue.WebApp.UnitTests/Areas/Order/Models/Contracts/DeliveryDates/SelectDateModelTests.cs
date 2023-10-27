using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates
{
    public static class SelectDateModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void NullOrder_ThrowsException(
            string internalOrgId,
            CallOffId callOffId)
        {
            FluentActions
                .Invoking(() => new SelectDateModel(internalOrgId, callOffId, null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            DateTime date)
        {
            order.DeliveryDate = date;
            var model = new SelectDateModel(internalOrgId, callOffId, order);

            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(callOffId);
            model.CommencementDate.Should().Be(order.CommencementDate);
            model.MaximumTerm.Should().Be(order.MaximumTerm);
            model.IsAmend.Should().Be(order.IsAmendment);
            model.TriageValue.Should().Be(order.OrderTriageValue);

            model.Date.Should().Be(date.Date);
            model.Day.Should().Be($"{date.Day:00}");
            model.Month.Should().Be($"{date.Month:00}");
            model.Year.Should().Be($"{date.Year:0000}");
        }

        [Theory]
        [CommonAutoData]
        public static void ContractEndDate_WithInvalidArguments_ReturnsNull(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new SelectDateModel(internalOrgId, callOffId, order);

            model.ContractEndDate.Should().NotBeNull();

            model.CommencementDate = null;

            model.ContractEndDate.Should().BeNull();

            model.CommencementDate = order.CommencementDate;
            model.MaximumTerm = null;

            model.ContractEndDate.Should().BeNull();
        }

        [Theory]
        [CommonInlineAutoData(OrderTriageValue.Under40K)]
        [CommonInlineAutoData(OrderTriageValue.Between40KTo250K)]
        [CommonInlineAutoData(OrderTriageValue.Over250K)]
        public static void ContractEndDate_ExpectedResult(
            OrderTriageValue triageValue,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderTriageValue = triageValue;

            var model = new SelectDateModel(internalOrgId, callOffId, order);

            var expected = order.CommencementDate!.Value
                .AddMonths(order.MaximumTerm!.Value)
                .AddDays(-1);

            model.ContractEndDate.Should().Be(expected);
        }
    }
}

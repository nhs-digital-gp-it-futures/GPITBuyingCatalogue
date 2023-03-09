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
    public static class AmendDateModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void NullOrder_ThrowsException(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            DateTime date)
        {
            var orderItem = order.OrderItems.First();

            FluentActions
                .Invoking(() => new AmendDateModel(internalOrgId, callOffId, orderItem.CatalogueItemId, null, date))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            DateTime date,
            RoutingSource source)
        {
            var orderItem = order.OrderItems.First();

            var model = new AmendDateModel(internalOrgId, callOffId, orderItem.CatalogueItemId, order, date)
            {
                Source = source,
            };

            model.Title.Should().Be(AmendDateModel.TitleText);
            model.Caption.Should().Be(model.ItemName);
            model.Advice.Should().Be(AmendDateModel.AdviceText);

            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(callOffId);
            model.CatalogueItemId.Should().Be(orderItem.CatalogueItemId);

            model.ItemName.Should().Be(orderItem.CatalogueItem.Name);

            model.CommencementDate.Should().Be(order.CommencementDate);
            model.MaximumTerm.Should().Be(order.MaximumTerm);
            model.TriageValue.Should().Be(order.OrderTriageValue);

            model.Source.Should().Be(source);

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
            EntityFramework.Ordering.Models.Order order,
            DateTime date,
            RoutingSource source)
        {
            var orderItem = order.OrderItems.First();

            var model = new AmendDateModel(internalOrgId, callOffId, orderItem.CatalogueItemId, order, date)
            {
                Source = source,
            };

            model.ContractEndDate.Should().NotBeNull();

            model.CommencementDate = null;

            model.ContractEndDate.Should().BeNull();

            model.CommencementDate = order.CommencementDate;
            model.MaximumTerm = null;

            model.ContractEndDate.Should().BeNull();

            model.MaximumTerm = order.MaximumTerm;
            model.TriageValue = null;

            model.ContractEndDate.Should().BeNull();

            model.TriageValue = order.OrderTriageValue;

            model.ContractEndDate.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void ContractEndDate_DirectAward_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            DateTime date,
            RoutingSource source)
        {
            var orderItem = order.OrderItems.First();

            var model = new AmendDateModel(internalOrgId, callOffId, orderItem.CatalogueItemId, order, date)
            {
                Source = source,
                TriageValue = OrderTriageValue.Under40K,
            };

            var expected = order.CommencementDate!.Value
                .AddMonths(order.MaximumTerm!.Value)
                .AddDays(-1);

            model.ContractEndDate.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(OrderTriageValue.Between40KTo250K)]
        [CommonInlineAutoData(OrderTriageValue.Over250K)]
        public static void ContractEndDate_OnOffCatalogueOrder_ExpectedResult(
            OrderTriageValue triageValue,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            DateTime date,
            RoutingSource source)
        {
            var orderItem = order.OrderItems.First();

            var model = new AmendDateModel(internalOrgId, callOffId, orderItem.CatalogueItemId, order, date)
            {
                Source = source,
                TriageValue = triageValue,
            };

            var expected = order.CommencementDate!.Value
                .AddMonths(EndDate.MaximumTermForOnOffCatalogueOrders)
                .AddDays(-1);

            model.ContractEndDate.Should().Be(expected);
        }
    }
}

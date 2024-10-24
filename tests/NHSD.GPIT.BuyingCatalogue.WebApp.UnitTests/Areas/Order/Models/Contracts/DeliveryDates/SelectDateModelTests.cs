﻿using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates
{
    public static class SelectDateModelTests
    {
        [Theory]
        [MockAutoData]
        public static void NullOrder_ThrowsException(
            string internalOrgId,
            CallOffId callOffId,
            bool applyToAll)
        {
            FluentActions
                .Invoking(() => new SelectDateModel(internalOrgId, callOffId, null, applyToAll))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            DateTime date)
        {
            order.DeliveryDate = date;
            var model = new SelectDateModel(internalOrgId, callOffId, order, null);

            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(callOffId);
            model.CommencementDate.Should().Be(order.CommencementDate);
            model.MaximumTerm.Should().Be(order.MaximumTerm);
            model.IsAmend.Should().Be(order.IsAmendment);

            model.Date.Should().Be(date.Date);
            model.Day.Should().Be($"{date.Day:00}");
            model.Month.Should().Be($"{date.Month:00}");
            model.Year.Should().Be($"{date.Year:0000}");
        }

        [Theory]
        [MockAutoData]
        public static void ContractEndDate_WithInvalidArguments_ReturnsNull(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new SelectDateModel(internalOrgId, callOffId, order, null);

            model.ContractEndDate.Should().NotBeNull();

            model.CommencementDate = null;

            model.ContractEndDate.Should().BeNull();

            model.CommencementDate = order.CommencementDate;
            model.MaximumTerm = null;

            model.ContractEndDate.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void ContractEndDate_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new SelectDateModel(internalOrgId, callOffId, order, null);

            var expected = order.CommencementDate!.Value
                .AddMonths(order.MaximumTerm!.Value)
                .AddDays(-1);

            model.ContractEndDate.Should().Be(expected);
        }
    }
}

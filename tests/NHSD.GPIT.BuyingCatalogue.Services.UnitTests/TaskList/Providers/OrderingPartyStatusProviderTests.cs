using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class OrderingPartyStatusProviderTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
            OrderingPartyStatusProvider service)
        {
            var actual = service.Get(null, new());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            OrderingPartyStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderingPartyContactIsNull_ReturnsNotStarted(
            Order order,
            OrderingPartyStatusProvider service)
        {
            order.OrderingPartyContact = null;

            var actual = service.Get(new(order), null);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderingPartyContactIsNotNull_ReturnsCompleted(
            Contact contact,
            Order order,
            OrderingPartyStatusProvider service)
        {
            order.OrderingPartyContact = contact;
            order.Revision = 1;

            var actual = service.Get(new(order), null);

            actual.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_IsAmendment_ContactEdited_ReturnsAmended(
            Contact originalContact,
            Contact editedContact,
            List<Order> orders,
            OrderingPartyStatusProvider service)
        {
            var previousOrder = orders.AsEnumerable().Reverse().Skip(1).First();
            var amendedOrder = orders.Last();

            amendedOrder.Revision = orders.Count;
            previousOrder.OrderingPartyContact = originalContact;
            amendedOrder.OrderingPartyContact = editedContact;

            var actual = service.Get(new(orders), null);

            actual.Should().Be(TaskProgress.Amended);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_IsAmendment_ContactUnchanged_ReturnsCompleted(
            Contact originalContact,
            List<Order> orders,
            OrderingPartyStatusProvider service)
        {
            var previousOrder = orders.AsEnumerable().Reverse().Skip(1).First();
            var amendedOrder = orders.Last();

            amendedOrder.Revision = orders.Count;
            previousOrder.OrderingPartyContact = originalContact;
            amendedOrder.OrderingPartyContact = originalContact;

            var actual = service.Get(new(orders), null);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}

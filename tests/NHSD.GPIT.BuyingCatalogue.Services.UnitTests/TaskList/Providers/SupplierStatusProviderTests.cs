using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class SupplierStatusProviderTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
            SupplierStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            SupplierStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            SupplierStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotApplicable)]
        [CommonInlineAutoData(TaskProgress.NotStarted)]
        [CommonInlineAutoData(TaskProgress.Optional)]
        public static void Get_OrderingPartyStatusNotComplete_ReturnsCannotStart(
            TaskProgress orderingPartyStatus,
            Order order,
            SupplierStatusProvider service)
        {
            var state = new OrderProgress
            {
                OrderingPartyStatus = orderingPartyStatus,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderHasNoSupplier_ReturnsNotStarted(
            Order order,
            SupplierStatusProvider service)
        {
            var state = new OrderProgress
            {
                OrderingPartyStatus = TaskProgress.Completed,
            };

            order.Supplier = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.Completed)]
        [CommonInlineAutoData(TaskProgress.Amended)]
        public static void Get_OrderHasNoSupplierContact_ReturnsInProgress(
            TaskProgress orderingPartyStatus,
            Supplier supplier,
            Order order,
            SupplierStatusProvider service)
        {
            var state = new OrderProgress
            {
                OrderingPartyStatus = orderingPartyStatus,
            };

            order.Supplier = supplier;
            order.SupplierContact = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.Completed)]
        [CommonInlineAutoData(TaskProgress.Amended)]
        public static void Get_OrderHasSupplierAndSupplierContact_ReturnsCompleted(
            TaskProgress orderingPartyStatus,
            Supplier supplier,
            Contact contact,
            Order order,
            SupplierStatusProvider service)
        {
            var state = new OrderProgress
            {
                OrderingPartyStatus = orderingPartyStatus,
            };

            order.Supplier = supplier;
            order.SupplierContact = contact;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.Completed)]
        [CommonInlineAutoData(TaskProgress.Amended)]
        public static void Get_OrderIsAnAmendment_SupplierContactTheSame_ReturnsCompleted(
            TaskProgress orderingPartyStatus,
            List<Order> orders,
            SupplierStatusProvider service)
        {
            var state = new OrderProgress
            {
                OrderingPartyStatus = orderingPartyStatus,
            };

            orders[0].Revision = 1;
            orders[1].Revision = 2;
            orders[2].Revision = 3;

            orders[2].SupplierContact = orders[1].SupplierContact;

            var actual = service.Get(new OrderWrapper(orders), state);

            actual.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.Completed)]
        [CommonInlineAutoData(TaskProgress.Amended)]
        public static void Get_OrderIsAnAmendment_SupplierContactDifferent_ReturnsAmended(
            TaskProgress orderingPartyStatus,
            List<Order> orders,
            SupplierStatusProvider service)
        {
            var state = new OrderProgress
            {
                OrderingPartyStatus = orderingPartyStatus,
            };

            orders[0].Revision = 1;
            orders[1].Revision = 2;
            orders[2].Revision = 3;

            var actual = service.Get(new OrderWrapper(orders), state);

            actual.Should().Be(TaskProgress.Amended);
        }
    }
}

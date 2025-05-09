using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class ServiceRecipientsStatusProviderTests
    {
        private static readonly OrderProgress ValidOrderState = new()
        {
            CommencementDateStatus = TaskProgress.Completed,
        };

        [Theory]
        [MockAutoData]
        public static void Get_OrderWrapperIsNull_Returns_CannotStart(
            ServiceRecipientsStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_OrderIsNull_Returns_CannotStart(
            ServiceRecipientsStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_StateIsNull_Returns_CannotStart(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_CommencementDateStatus_NotCompleted_Returns_CannotStart(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            var state = new OrderProgress()
            {
                CommencementDateStatus = TaskProgress.InProgress,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void No_New_Recipients_Returns_NotStarted(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            order.OrderRecipients.Clear();

            TaskProgress actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void Sublocations_But_No_Recipients_Returns_InProgress(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            order.OrderSublocations.ForEach(x => x.SublocationRecipients = []);

            TaskProgress actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void Complete_And_Incomplete_Sublocations_Returns_InProgress(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            OrderSublocation sublocationWithNoRecipients = order.OrderSublocations.First();

            sublocationWithNoRecipients.SublocationRecipients = [];

            TaskProgress actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void New_Sublocations_And_Recipients_And_IsAmendment_Returns_Amended(
            Order previousOrder,
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            order.OrderNumber = previousOrder.OrderNumber;
            previousOrder.Revision = 1;
            order.Revision = 2;
            previousOrder.OrderSublocations = order.OrderSublocations.Take(2).ToList();

            var orders = new List<Order> { previousOrder, order };

            TaskProgress actual = service.Get(new OrderWrapper(orders), ValidOrderState);

            actual.Should().Be(TaskProgress.Amended);
        }

        [Theory]
        [MockAutoData]
        public static void New_Sublocations_And_Recipients_Returns_Completed(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            TaskProgress actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;
using MoreEnumerable = MoreLinq.MoreEnumerable;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class DeliveryDatesStatusProviderTests
    {
        [Theory]
        [MockAutoData]
        public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
            DeliveryDatesStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            DeliveryDatesStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            DeliveryDatesStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockInlineAutoData(TaskProgress.CannotStart)]
        [MockInlineAutoData(TaskProgress.InProgress)]
        [MockInlineAutoData(TaskProgress.NotApplicable)]
        [MockInlineAutoData(TaskProgress.NotStarted)]
        [MockInlineAutoData(TaskProgress.Optional)]
        public static void Get_DeliveryDatesNotComplete_ReturnsCannotStart(
            TaskProgress status,
            Order order,
            List<OrderSublocation> orderSublocations,
            DeliveryDatesStatusProvider service)
        {
            order.OrderSublocations = orderSublocations;

            var state = new OrderProgress
            {
                SolutionOrService = status,
            };

            MoreEnumerable.ForEach(order.OrderRecipients, x => x.OrderItemRecipients.Clear());

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_NoDeliveryDatesEntered_ReturnsInProgress(
            Order order,
            List<OrderSublocation> orderSublocations,
            DeliveryDatesStatusProvider service)
        {
            order.OrderSublocations = orderSublocations;

            var state = new OrderProgress
            {
                SolutionOrService = TaskProgress.Completed,
            };

            MoreEnumerable.ForEach(order.OrderRecipients, x => x.OrderItemRecipients.Clear());

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void Get_NoDeliveryDatesOrDefaultDeliveryDateEntered_ReturnsNotStarted(
            Order order,
            List<OrderSublocation> orderSublocations,
            DeliveryDatesStatusProvider service)
        {
            order.OrderSublocations = orderSublocations;

            var state = new OrderProgress
            {
                SolutionOrService = TaskProgress.Completed,
            };

            MoreEnumerable.ForEach(order.FlattenedRecipients, x => x.OrderItemSublocationRecipients.Clear());
            order.DeliveryDate = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void Get_SomeDeliveryDatesEntered_ReturnsInProgress(
            Order order,
            List<OrderSublocation> orderSublocations,
            DeliveryDatesStatusProvider service)
        {
            order.OrderSublocations = orderSublocations;

            var state = new OrderProgress
            {
                SolutionOrService = TaskProgress.Completed,
            };

            MoreEnumerable.ForEach(order.OrderRecipients, x => x.OrderItemRecipients.Clear());
            var orderItem = order.OrderItems.First();
            MoreEnumerable.ForEach(order.OrderRecipients, x => x.SetDeliveryDateForItem(orderItem.CatalogueItemId, DateTime.Today));

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void Get_AllDeliveryDatesEntered_ReturnsCompleted(
            Order order,
            List<OrderSublocation> orderSublocations,
            OrderItem orderItem,
            DeliveryDatesStatusProvider service)
        {
            order.OrderItems = [orderItem];

            orderSublocations.ForEach(x =>
            {
                x.OrderId = order.Id;
                foreach (OrderSublocationRecipient sr in x.SublocationRecipients)
                {
                    sr.OrderItemSublocationRecipients =
                    [
                        new OrderItemSublocationRecipient(x.OrderId, sr.RecipientOdsCode, orderItem.CatalogueItemId)
                        {
                            DeliveryDate = order.DeliveryDate,
                        },
                    ];
                }
            });

            order.OrderSublocations = orderSublocations;

            var state = new OrderProgress
            {
                SolutionOrService = TaskProgress.Completed,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}

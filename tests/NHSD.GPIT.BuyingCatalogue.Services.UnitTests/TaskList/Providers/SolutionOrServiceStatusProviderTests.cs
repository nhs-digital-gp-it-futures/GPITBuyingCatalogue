using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class SolutionOrServiceStatusProviderTests
    {
        private static readonly OrderProgress ValidOrderState = new()
        {
            SupplierStatus = TaskProgress.Completed,
            ServiceRecipients = TaskProgress.Completed,
        };

        [Theory]
        [MockAutoData]
        public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
            SolutionOrServiceStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            SolutionOrServiceStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            SolutionOrServiceStatusProvider service)
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
        public static void Get_ServiceRecipientsNotComplete_ReturnsCannotStart(
            TaskProgress status,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                ServiceRecipients = status,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockInlineAutoData(TaskProgress.CannotStart)]
        [MockInlineAutoData(TaskProgress.InProgress)]
        [MockInlineAutoData(TaskProgress.NotApplicable)]
        [MockInlineAutoData(TaskProgress.NotStarted)]
        [MockInlineAutoData(TaskProgress.Optional)]
        public static void Get_SupplierStatusNotComplete_ReturnsCannotStart(
            TaskProgress status,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                SupplierStatus = status,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockInlineAutoData(TaskProgress.CannotStart)]
        [MockInlineAutoData(TaskProgress.InProgress)]
        [MockInlineAutoData(TaskProgress.NotApplicable)]
        [MockInlineAutoData(TaskProgress.NotStarted)]
        [MockInlineAutoData(TaskProgress.Optional)]
        public static void Get_ServiceRecipientsNotComplete_SupplierStatusNotComplete_ReturnsCannotStart(
            TaskProgress status,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                ServiceRecipients = status,
                SupplierStatus = status,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockInlineAutoData(CatalogueItemType.AdditionalService)]
        [MockInlineAutoData(CatalogueItemType.AssociatedService)]
        public static void Get_NoSolutionSelected_ReturnsNotStarted(
            CatalogueItemType itemType,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = itemType);

            var actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Amended_Order_With_Nothing_Added_ReturnsNotStarted(
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            order.Revision = 1;
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            var amendedOrder = order.BuildAmendment(2);

            var actual = service.Get(new OrderWrapper(amendedOrder, [order]), ValidOrderState);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Amended_NewRecipient_ReturnsInProgress(
            Order order,
            OrderSublocation newOrderSublocation,
            SolutionOrServiceStatusProvider service)
        {
            order.Revision = 1;
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution);
            var amendedOrder = order.BuildAmendment(2);
            order.OrderItems.ForEach(i =>
            {
                var matchingItem = amendedOrder.OrderItems.FirstOrDefault(a => a.CatalogueItemId == i.CatalogueItemId);
                matchingItem.CatalogueItem = i.CatalogueItem;
            });
            amendedOrder.OrderSublocations.Add(newOrderSublocation);

            var actual = service.Get(new OrderWrapper(amendedOrder, [order]), ValidOrderState);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Amended_NewOrderItem_ReturnsInProgress(
            OrderItem orderItemToAdd,
            Order order,
            OrderSublocation sublocation,
            SolutionOrServiceStatusProvider service)
        {
            order.Revision = 1;
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution);
            var amendedOrder = order.BuildAmendment(2);
            order.OrderItems.ForEach(i =>
            {
                var matchingItem = amendedOrder.OrderItems.FirstOrDefault(a => a.CatalogueItemId == i.CatalogueItemId);
                matchingItem.CatalogueItem = i.CatalogueItem;
            });
            amendedOrder.OrderItems.Add(orderItemToAdd);
            amendedOrder.OrderSublocations.Add(sublocation);

            var actual = service.Get(new OrderWrapper(amendedOrder, [order]), ValidOrderState);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void Get_NoSolutionSelected_AssociatedServicesOnly_ReturnsNotStarted(
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.AssociatedServicesOnlyDetails.SolutionId = null;

            var actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void Get_SolutionSelected_NoOrderItems_AssociatedServicesOnly_ReturnsInProgress(
            CatalogueItemId solutionId,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.AssociatedServicesOnlyDetails.SolutionId = solutionId;
            order.OrderItems.Clear();

            var actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void Get_SolutionSelected_NoQuantitiesSelected_ReturnsInProgress(
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            order.Revision = 1;
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x =>
            {
                x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            });
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            order.FlattenedRecipients.ForEach(x => x.OrderItemSublocationRecipients.ForEach(y => y.Quantity = null));

            var actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void Get_SolutionSelected_EverythingPopulated_ReturnsCompleted(
            int quantity,
            Order order,
            OrderSublocationRecipient recipient,
            OrderSublocation orderSublocation,
            SolutionOrServiceStatusProvider service)
        {
            order.Revision = 1;
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x =>
            {
                x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
                recipient.SetQuantityForItem(x, quantity);
            });
            orderSublocation.SublocationRecipients = new List<OrderSublocationRecipient> { recipient };
            order.OrderSublocations = new List<OrderSublocation> { orderSublocation };
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            TaskProgress actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(CatalogueItemType.AdditionalService)]
        [MockInlineAutoData(CatalogueItemType.AssociatedService)]
        public static void Get_SolutionSelected_EverythingPopulated_Amendment_ReturnsCompleted(
            CatalogueItemType catalogueItemType,
            int quantity,
            Order previousOrder,
            Order order,
            OrderSublocationRecipient recipient,
            OrderSublocation orderSublocation,
            SolutionOrServiceStatusProvider service)
        {
            order.Revision = 2;
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x =>
            {
                x.CatalogueItem.CatalogueItemType = catalogueItemType;
                recipient.SetQuantityForItem(x, quantity);
            });
            orderSublocation.SublocationRecipients = new List<OrderSublocationRecipient> { recipient };
            order.OrderSublocations = new List<OrderSublocation> { orderSublocation };
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            TaskProgress actual = service.Get(new OrderWrapper(order, [previousOrder]), ValidOrderState);

            actual.Should().Be(TaskProgress.Amended);
        }
    }
}

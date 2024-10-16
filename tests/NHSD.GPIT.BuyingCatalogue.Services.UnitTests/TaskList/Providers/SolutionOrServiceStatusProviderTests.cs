﻿using System.Linq;
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
        public static void Get_SupplierStatusNotComplete_ReturnsCannotStart(
            TaskProgress status,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                CommencementDateStatus = status,
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
            var state = new OrderProgress
            {
                ServiceRecipients = TaskProgress.Completed,
            };

            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = itemType);

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Amended_Order_With_Nothing_Added_ReturnsNotStarted(
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                ServiceRecipients = TaskProgress.Completed,
            };

            order.Revision = 1;
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            var amendedOrder = order.BuildAmendment(2);

            var actual = service.Get(new OrderWrapper(new[] { order, amendedOrder }), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Amended_NewRecipient_ReturnsInProgress(
            Order order,
            OrderRecipient orderRecipient,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                ServiceRecipients = TaskProgress.Completed,
            };

            order.Revision = 1;
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution);
            var amendedOrder = order.BuildAmendment(2);
            order.OrderItems.ForEach(i =>
            {
                var matchingItem = amendedOrder.OrderItems.FirstOrDefault(a => a.CatalogueItemId == i.CatalogueItemId);
                matchingItem.CatalogueItem = i.CatalogueItem;
            });
            amendedOrder.OrderRecipients.Add(orderRecipient);

            var actual = service.Get(new OrderWrapper(new[] { order, amendedOrder }), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Amended_NewOrderItem_ReturnsInProgress(
            OrderItem orderItemToAdd,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                ServiceRecipients = TaskProgress.Completed,
            };

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

            var actual = service.Get(new OrderWrapper(new[] { order, amendedOrder }), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void Get_NoSolutionSelected_AssociatedServicesOnly_ReturnsNotStarted(
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                ServiceRecipients = TaskProgress.Completed,
            };

            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.AssociatedServicesOnlyDetails.SolutionId = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void Get_SolutionSelected_NoOrderItems_AssociatedServicesOnly_ReturnsInProgress(
            CatalogueItemId solutionId,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                ServiceRecipients = TaskProgress.Completed,
            };

            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.AssociatedServicesOnlyDetails.SolutionId = solutionId;
            order.OrderItems.Clear();

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockInlineAutoData(1, TaskProgress.Completed)]
        [MockInlineAutoData(2, TaskProgress.Amended)]
        public static void Get_SolutionSelected_EverythingPopulated_ReturnsCompleted(
            int revision,
            TaskProgress expectedTaskProgress,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                ServiceRecipients = TaskProgress.Completed,
            };

            order.Revision = revision;
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(expectedTaskProgress);
        }

        [Theory]
        [MockInlineAutoData(1, TaskProgress.Completed)]
        [MockInlineAutoData(2, TaskProgress.Amended)]
        public static void Get_SolutionSelected_With_Everything_But_DeliveryDates_Returns_InProgress_For_An_Amendment(
            int revision,
            TaskProgress expectedTaskProgress,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                ServiceRecipients = TaskProgress.Completed,
            };

            order.Revision = revision;
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderRecipients.ForEach(r => r.OrderItemRecipients.ForEach(i => i.DeliveryDate = null));

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(expectedTaskProgress);
        }
    }
}

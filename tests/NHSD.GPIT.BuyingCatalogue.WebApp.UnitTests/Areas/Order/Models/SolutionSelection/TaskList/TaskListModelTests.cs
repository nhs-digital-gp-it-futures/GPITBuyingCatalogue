using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.TaskList
{
    public static class TaskListModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithNullOrder_ThrowsException(
            string internalOrgId,
            CallOffId callOffId)
        {
            FluentActions
                .Invoking(() => new TaskListModel(internalOrgId, callOffId, null, AssociatedServicesForAdditionalServices()))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderType = OrderTypeEnum.Solution;

            var solution = order.OrderItems.ElementAt(0);
            var additionalService = order.OrderItems.ElementAt(1);
            var associatedService = order.OrderItems.ElementAt(2);
            associatedService.ParentId = solution.Id;

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            additionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            var model = new TaskListModel(
                internalOrgId,
                callOffId,
                new OrderWrapper(order),
                AssociatedServicesForAdditionalServices(order));

            model.InternalOrgId.Should().BeEquivalentTo(internalOrgId);
            model.CallOffId.Should().BeEquivalentTo(callOffId);
            model.OrderType.Should().Be(order.OrderType);
            model.CatalogueSolution.CatalogueItemId.Should().BeEquivalentTo(solution.CatalogueItemId);
            model.AdditionalServices.Select(s => s.CatalogueItemId).Should().BeEquivalentTo(new[] { additionalService.CatalogueItemId });
            model.AssociatedServices.Select(s => s.CatalogueItemId).Should().BeEquivalentTo(new[] { associatedService.CatalogueItemId });

            model.Progress.Should().Be(TaskProgress.Completed);
            model.Title.Should().Be(TaskListModel.CompletedTitle);

            model.OrderItemModel(solution.CatalogueItemId).Should().NotBeNull();
            model.OrderItemModel(additionalService.CatalogueItemId).Should().NotBeNull();
            model.OrderItemModel(associatedService.CatalogueItemId).Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_Amendment_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            int parentId,
            EntityFramework.Ordering.Models.Order order,
            OrderItem orderItem)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            order.OrderNumber = callOffId.OrderNumber;
            order.Revision = callOffId.Revision;

            order.FlattenedRecipients.ForEach(x => order.OrderItems.ForEach(y =>
                x.OrderItemSublocationRecipients.Add(
                    new OrderItemSublocationRecipient(order.Id, x.RecipientOdsCode, y)
                    {
                        Quantity = 5, DeliveryDate = new DateTime(2024, 01, 01),
                    })));

            var amendment = order.BuildAmendment(2);

            order.OrderType = OrderTypeEnum.Solution;

            var solution = order.OrderItems.ElementAt(0);
            var additionalService = order.OrderItems.ElementAt(1);
            var associatedService = order.OrderItems.ElementAt(2);

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            additionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            associatedService.ParentId = solution.Id;

            orderItem.CatalogueItem = new CatalogueItem()
            {
                CatalogueItemType = CatalogueItemType.AssociatedService,
                Id = associatedService.CatalogueItemId,
            };
            orderItem.CatalogueItemId = orderItem.CatalogueItem.Id;
            orderItem.Order = order;
            orderItem.ParentId = parentId;

            amendment.OrderItems = new List<OrderItem>()
            {
                new OrderItem() { Id = parentId, Order = order, CatalogueItem = new CatalogueItem() { CatalogueItemType = CatalogueItemType.Solution, Id = solution.CatalogueItemId }, CatalogueItemId = solution.CatalogueItemId, OrderItemPrice = orderItem.OrderItemPrice },
                new OrderItem() { Order = order, CatalogueItem = new CatalogueItem() { CatalogueItemType = CatalogueItemType.AdditionalService, Id = associatedService.CatalogueItemId }, CatalogueItemId = additionalService.CatalogueItemId, OrderItemPrice = orderItem.OrderItemPrice },
                orderItem,
            };

            var model = new TaskListModel(
                internalOrgId,
                callOffId,
                new OrderWrapper(amendment, [order]),
                AssociatedServicesForAdditionalServices(amendment));
            amendment.FlattenedRecipients.ForEach(x => amendment.OrderItems.ForEach(y =>
                x.OrderItemSublocationRecipients.Add(
                    new OrderItemSublocationRecipient(order.Id, x.RecipientOdsCode, y)
                    {
                        Quantity = 5, DeliveryDate = new DateTime(2024, 01, 01),
                    })));

            model.InternalOrgId.Should().BeEquivalentTo(internalOrgId);
            model.CallOffId.Should().BeEquivalentTo(callOffId);
            model.OrderType.Should().Be(order.OrderType);
            model.CatalogueSolution.CatalogueItemId.Should().BeEquivalentTo(solution.CatalogueItemId);
            model.AdditionalServices.Select(x => x.CatalogueItemId).Should().BeEquivalentTo(new[] { additionalService.CatalogueItemId });
            model.AssociatedServices.Select(x => x.CatalogueItemId).Should().BeEquivalentTo(new[] { associatedService.CatalogueItemId });

            model.Progress.Should().Be(TaskProgress.Completed);
            model.Title.Should().Be(TaskListModel.CompletedTitle);

            model.OrderItemModel(solution.CatalogueItemId).Should().NotBeNull();
            model.OrderItemModel(additionalService.CatalogueItemId).Should().NotBeNull();
            model.OrderItemModel(associatedService.CatalogueItemId).Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_AssociatedServicesOnly_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItem serviceSolution,
            EntityFramework.Ordering.Models.Order order)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderNumber = callOffId.OrderNumber;
            order.Revision = callOffId.Revision;

            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.AssociatedServicesOnlyDetails.Solution = serviceSolution;

            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            var model = new TaskListModel(
                internalOrgId,
                callOffId,
                new OrderWrapper(order),
                AssociatedServicesForAdditionalServices(order));

            model.InternalOrgId.Should().BeEquivalentTo(internalOrgId);
            model.CallOffId.Should().BeEquivalentTo(callOffId);
            model.OrderType.Should().Be(order.OrderType);
            model.SolutionName.Should().Be(serviceSolution.Name);
            model.CatalogueSolution.Should().BeNull();
            model.AdditionalServices.Should().BeEmpty();
            model.AssociatedServices.Select(s => s.CatalogueItemId).Should().BeEquivalentTo(new[]
            {
                order.OrderItems.First().CatalogueItemId,
                order.OrderItems.ElementAt(1).CatalogueItemId,
                order.OrderItems.ElementAt(2).CatalogueItemId,
            });

            model.Progress.Should().Be(TaskProgress.Completed);
            model.Title.Should().Be(TaskListModel.CompletedTitle);

            for (var i = 0; i < 3; i++)
            {
                model.OrderItemModel(order.OrderItems.ElementAt(i).CatalogueItemId).Should().NotBeNull();
            }
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_IncompleteOrder_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            var solution = order.OrderItems.ElementAt(0);

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            order.OrderItems = new List<OrderItem> { solution };
            order.FlattenedRecipients.ForEach(r => r.OrderItemSublocationRecipients.Clear());

            var model = new TaskListModel(
                internalOrgId,
                callOffId,
                new OrderWrapper(order),
                AssociatedServicesForAdditionalServices(order));

            model.InternalOrgId.Should().BeEquivalentTo(internalOrgId);
            model.CallOffId.Should().BeEquivalentTo(callOffId);
            model.OrderType.Should().Be(order.OrderType);
            model.CatalogueSolution.CatalogueItemId.Should().BeEquivalentTo(solution.CatalogueItemId);
            model.AdditionalServices.Should().BeEmpty();
            model.AssociatedServices.Should().BeEmpty();

            model.Progress.Should().Be(TaskProgress.InProgress);
            model.Title.Should().Be(TaskListModel.InProgressTitle);

            model.OrderItemModel(solution.CatalogueItemId).Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static void Amendment_SetsTitleAndAdviceCorrectly(
            TaskListModel model)
        {
            model.CallOffId = new CallOffId(model.CallOffId.OrderNumber, 2);

            model.Title.Should().Be(TaskListModel.AmendmentTitle);
        }

        [Theory]
        [MockAutoData]
        public static void Model_ShouldReturnOrderItemModelForPreviousAssociatedServices(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderItem orderItem2,
            OrderSublocation sublocation,
            EntityFramework.Ordering.Models.Order order)
        {
            var initialCallOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderNumber = initialCallOffId.OrderNumber;
            order.Revision = initialCallOffId.Revision;
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            orderItem.Order = order;
            order.OrderItems = [orderItem];
            order.OrderSublocations = [sublocation];

            var amendedCallOffId = new CallOffId(callOffId.OrderNumber, 2);
            var amendedOrder = order.Clone();
            var solution = orderItem2;
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            amendedOrder.OrderNumber = amendedCallOffId.OrderNumber;
            amendedOrder.Revision = amendedCallOffId.Revision;
            amendedOrder.OrderItems = [solution];

            var orderWrapper = new OrderWrapper(amendedOrder, [order]);
            var taskListModel = new TaskListModel(
                internalOrgId,
                callOffId,
                orderWrapper,
                AssociatedServicesForAdditionalServices(amendedOrder));

            var expectedGrouping = new List<EntityFramework.Ordering.Models.Order> { order }
                .SelectMany(o => o.GetAssociatedServices())
                .GroupBy(o => o.Order.CallOffId);

            taskListModel.PreviousAssociatedServices.Should().NotBeEmpty();
            taskListModel.PreviousAssociatedServices.Should().HaveCount(1);
            taskListModel.PreviousAssociatedServices.Should().BeEquivalentTo(expectedGrouping);

            var expectedTaskListOrderItemModel = new TaskListOrderItemModel(
                internalOrgId,
                taskListModel.CallOffId,
                taskListModel.OrderType,
                [],
                orderItem)
            {
                OrderItemId = orderItem.Id,
                Source = RoutingSource.TaskList,
            };

            var orderItemModelForPrevious =
                taskListModel.OrderItemModelForPrevious(initialCallOffId, orderItem.CatalogueItemId);

            orderItemModelForPrevious.Should()
                .BeEquivalentTo(
                    expectedTaskListOrderItemModel,
                    opt => opt.Excluding(oi => oi.FromPreviousRevision)
                        .Excluding(oi => oi.HasNewRecipients)
                        .Excluding(oi => oi.NumberOfPrices)
                        .Excluding(oi => oi.PriceId)
                        .Excluding(oi => oi.CanBeRemoved)
                        .Excluding(oi => oi.QuantityStatus));
        }

        private static Dictionary<CatalogueItemId, int> AssociatedServicesForAdditionalServices(
            EntityFramework.Ordering.Models.Order order = null)
        {
            return order?.GetAdditionalServices()
                .ToDictionary(x => x.CatalogueItemId, _ => 0)
                ?? new Dictionary<CatalogueItemId, int>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.TaskList
{
    public static class TaskListModelTests
    {
        [Theory]
        [CommonAutoData(MockingFramework.NSubstitute)]
        public static void WithNullOrder_ThrowsException(
            string internalOrgId,
            CallOffId callOffId)
        {
            FluentActions
                .Invoking(() => new TaskListModel(internalOrgId, callOffId, null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData(MockingFramework.NSubstitute)]
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

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            additionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            var model = new TaskListModel(internalOrgId, callOffId, new OrderWrapper(order));

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
        [CommonAutoData(MockingFramework.NSubstitute)]
        public static void WithValidArguments_Amendment_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            var amendment = order.BuildAmendment(2);

            order.OrderType = OrderTypeEnum.Solution;

            var solution = order.OrderItems.ElementAt(0);
            var additionalService = order.OrderItems.ElementAt(1);
            var associatedService = order.OrderItems.ElementAt(2);

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            additionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            amendment.OrderItems = new List<OrderItem>()
            {
                new OrderItem() { CatalogueItem = new CatalogueItem() { CatalogueItemType = CatalogueItemType.Solution, Id = solution.CatalogueItemId }, CatalogueItemId = solution.CatalogueItemId },
                new OrderItem() { CatalogueItem = new CatalogueItem() { CatalogueItemType = CatalogueItemType.AdditionalService, Id = associatedService.CatalogueItemId }, CatalogueItemId = additionalService.CatalogueItemId },
                new OrderItem() { CatalogueItem = new CatalogueItem() { CatalogueItemType = CatalogueItemType.AssociatedService, Id = associatedService.CatalogueItemId }, CatalogueItemId = associatedService.CatalogueItemId },
            };

            var model = new TaskListModel(internalOrgId, callOffId, new OrderWrapper(new[] { order, amendment }));

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
        [CommonAutoData(MockingFramework.NSubstitute)]
        public static void WithValidArguments_AssociatedServicesOnly_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItem serviceSolution,
            EntityFramework.Ordering.Models.Order order)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.AssociatedServicesOnlyDetails.Solution = serviceSolution;

            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var model = new TaskListModel(internalOrgId, callOffId, new OrderWrapper(order));

            model.InternalOrgId.Should().BeEquivalentTo(internalOrgId);
            model.CallOffId.Should().BeEquivalentTo(callOffId);
            model.OrderType.Should().Be(order.OrderType);
            model.SolutionName.Should().Be(serviceSolution.Name);
            model.CatalogueSolution.CatalogueItemId.Should().BeEquivalentTo(order.OrderItems.First().CatalogueItemId);
            model.AdditionalServices.Should().BeEmpty();
            model.AssociatedServices.Select(s => s.CatalogueItemId).Should().BeEquivalentTo(new[]
            {
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
        [CommonAutoData(MockingFramework.NSubstitute)]
        public static void WithValidArguments_IncompleteOrder_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            var solution = order.OrderItems.ElementAt(0);

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            order.OrderItems = new List<OrderItem> { solution };
            order.OrderRecipients.ForEach(r => r.OrderItemRecipients.Clear());

            var model = new TaskListModel(internalOrgId, callOffId, new OrderWrapper(order));

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
        [CommonAutoData(MockingFramework.NSubstitute)]
        public static void Amendment_SetsTitleAndAdviceCorrectly(
            TaskListModel model)
        {
            model.CallOffId = new CallOffId(model.CallOffId.OrderNumber, 2);

            model.Title.Should().Be(TaskListModel.AmendmentTitle);
        }
    }
}

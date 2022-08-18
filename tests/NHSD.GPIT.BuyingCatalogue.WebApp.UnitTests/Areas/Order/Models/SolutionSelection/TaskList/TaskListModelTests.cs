using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.TaskList;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.TaskList
{
    public class TaskListModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithNullOrder_ThrowsException(
            string internalOrgId,
            CallOffId callOffId)
        {
            FluentActions
                .Invoking(() => new TaskListModel(internalOrgId, callOffId, null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.AssociatedServicesOnly = false;

            var solution = order.OrderItems.ElementAt(0);
            var additionalService = order.OrderItems.ElementAt(1);
            var associatedService = order.OrderItems.ElementAt(2);

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            additionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            var model = new TaskListModel(internalOrgId, callOffId, order);

            model.InternalOrgId.Should().BeEquivalentTo(internalOrgId);
            model.CallOffId.Should().BeEquivalentTo(callOffId);
            model.AssociatedServicesOnly.Should().Be(order.AssociatedServicesOnly);
            model.CatalogueSolution.Should().BeEquivalentTo(solution);
            model.AdditionalServices.Should().BeEquivalentTo(new[] { additionalService });
            model.AssociatedServices.Should().BeEquivalentTo(new[] { associatedService });

            model.Progress.Should().Be(TaskProgress.Completed);

            model.OrderItemModel(solution.CatalogueItemId).Should().NotBeNull();
            model.OrderItemModel(additionalService.CatalogueItemId).Should().NotBeNull();
            model.OrderItemModel(associatedService.CatalogueItemId).Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_AssociatedServicesOnly_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItem serviceSolution,
            EntityFramework.Ordering.Models.Order order)
        {
            order.AssociatedServicesOnly = true;
            order.Solution = serviceSolution;

            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var model = new TaskListModel(internalOrgId, callOffId, order);

            model.InternalOrgId.Should().BeEquivalentTo(internalOrgId);
            model.CallOffId.Should().BeEquivalentTo(callOffId);
            model.AssociatedServicesOnly.Should().Be(order.AssociatedServicesOnly);
            model.SolutionName.Should().Be(serviceSolution.Name);
            model.CatalogueSolution.Should().BeEquivalentTo(order.OrderItems.First());
            model.AdditionalServices.Should().BeEmpty();
            model.AssociatedServices.Should().BeEquivalentTo(new[]
            {
                order.OrderItems.ElementAt(1),
                order.OrderItems.ElementAt(2),
            });

            model.Progress.Should().Be(TaskProgress.Completed);

            for (var i = 0; i < 3; i++)
            {
                model.OrderItemModel(order.OrderItems.ElementAt(i).CatalogueItemId).Should().NotBeNull();
            }
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_IncompleteOrder_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            var solution = order.OrderItems.ElementAt(0);

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.OrderItemRecipients.Clear();

            order.OrderItems = new List<OrderItem> { solution };

            var model = new TaskListModel(internalOrgId, callOffId, order);

            model.InternalOrgId.Should().BeEquivalentTo(internalOrgId);
            model.CallOffId.Should().BeEquivalentTo(callOffId);
            model.AssociatedServicesOnly.Should().Be(order.AssociatedServicesOnly);
            model.CatalogueSolution.Should().BeEquivalentTo(solution);
            model.AdditionalServices.Should().BeEmpty();
            model.AssociatedServices.Should().BeEmpty();

            model.Progress.Should().Be(TaskProgress.InProgress);

            model.OrderItemModel(solution.CatalogueItemId).Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void WithAdditionalServices_SetsActionText(
            TaskListModel model)
        {
            model.AdditionalServices.Should().HaveCountGreaterThan(0);

            model.AdditionalServicesActionText.Should().Be("Change Additional Services");
        }

        [Theory]
        [CommonAutoData]
        public static void WithNoAdditionalServices_SetsActionText(
            TaskListModel model)
        {
            model.AdditionalServices = null;

            model.AdditionalServicesActionText.Should().Be("Add Additional Services");
        }

        [Theory]
        [CommonAutoData]
        public static void WithAssociatedServices_SetsActionText(
            TaskListModel model)
        {
            model.AssociatedServices.Should().HaveCountGreaterThan(0);

            model.AssociatedServicesActionText.Should().Be("Change Associated Services");
        }

        [Theory]
        [CommonAutoData]
        public static void WithNoAssociatedServicesSetsActionText(
            TaskListModel model)
        {
            model.AssociatedServices = null;

            model.AssociatedServicesActionText.Should().Be("Add Associated Services");
        }
    }
}

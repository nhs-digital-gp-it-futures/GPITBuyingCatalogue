using System.Linq;
using FluentAssertions;
using MoreLinq;
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
    public static class SolutionOrServiceStatusProviderTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            SolutionOrServiceStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            SolutionOrServiceStatusProvider service)
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
        [CommonInlineAutoData(CatalogueItemType.AdditionalService)]
        [CommonInlineAutoData(CatalogueItemType.AssociatedService)]
        public static void Get_NoSolutionSelected_ReturnsNotStarted(
            CatalogueItemType itemType,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                CommencementDateStatus = TaskProgress.Completed,
            };

            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = itemType);

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_NothingAmended_ReturnsNotStarted(
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                CommencementDateStatus = TaskProgress.Completed,
            };

            order.Revision = 1;
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            var amendedOrder = order.BuidAmendment(2);

            var actual = service.Get(new OrderWrapper(new[] { order, amendedOrder }), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(CatalogueItemType.Solution)]
        [CommonInlineAutoData(CatalogueItemType.AdditionalService)]
        [CommonInlineAutoData(CatalogueItemType.AssociatedService)]
        public static void Get_Amending_ReturnsInProgress(
            CatalogueItemType itemType,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                CommencementDateStatus = TaskProgress.Completed,
            };

            order.Revision = 1;
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = itemType);
            var amendedOrder = order.BuidAmendment(2);
            var orderItem = order.OrderItems.First();
            var orderItemToAdd = new OrderItem
            {
                OrderId = order.Id,
                CatalogueItemId = orderItem.CatalogueItemId,
                CatalogueItem = orderItem.CatalogueItem,
            };
            amendedOrder.OrderItems.Add(orderItemToAdd);

            var actual = service.Get(new OrderWrapper(new[] { order, amendedOrder }), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_NoSolutionSelected_AssociatedServicesOnly_ReturnsNotStarted(
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                CommencementDateStatus = TaskProgress.Completed,
            };

            order.AssociatedServicesOnly = true;
            order.SolutionId = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_SolutionSelected_NoOrderItems_AssociatedServicesOnly_ReturnsInProgress(
            CatalogueItemId solutionId,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                CommencementDateStatus = TaskProgress.Completed,
            };

            order.AssociatedServicesOnly = true;
            order.SolutionId = solutionId;
            order.OrderItems.Clear();

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData(1, TaskProgress.Completed)]
        [CommonInlineAutoData(2, TaskProgress.Amended)]
        public static void Get_SolutionSelected_EverythingPopulated_ReturnsCompleted(
            int revision,
            TaskProgress expectedTaskProgress,
            Order order,
            SolutionOrServiceStatusProvider service)
        {
            var state = new OrderProgress
            {
                CommencementDateStatus = TaskProgress.Completed,
            };

            order.Revision = revision;
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(expectedTaskProgress);
        }
    }
}

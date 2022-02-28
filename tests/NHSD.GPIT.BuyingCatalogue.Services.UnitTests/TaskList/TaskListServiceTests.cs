using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList
{
    public static class TaskListServiceTests
    {
        [Theory]
        [CommonAutoData]
        public static void NullOrder_Returns_DefaultModel(
            TaskListService service)
        {
            var expected = new OrderTaskList();

            var actual = service.GetTaskListStatusModelForOrder(null);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void CompleteOrder_AllStatuses_Correct(
            Order order,
            OrderItem solutionOrderItem,
            OrderItem associatedServiceOrderItem,
            OrderItem additionalServiceOrderItem,
            TaskListService service)
        {
            solutionOrderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            associatedServiceOrderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            additionalServiceOrderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            order.AddOrUpdateOrderItem(solutionOrderItem);
            order.AddOrUpdateOrderItem(associatedServiceOrderItem);
            order.AddOrUpdateOrderItem(additionalServiceOrderItem);

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.DescriptionStatus.Should().Be(TaskProgress.Completed);
            actual.OrderingPartyStatus.Should().Be(TaskProgress.Completed);
            actual.SupplierStatus.Should().Be(TaskProgress.Completed);
            actual.CommencementDateStatus.Should().Be(TaskProgress.Completed);
            actual.CatalogueSolutionsStatus.Should().Be(TaskProgress.Completed);
            actual.AdditionalServiceStatus.Should().Be(TaskProgress.Completed);
            actual.AssociatedServiceStatus.Should().Be(TaskProgress.Completed);
            actual.FundingSourceStatus.Should().Be(TaskProgress.Completed);
            actual.ReviewAndCompleteStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NoOrderingParty_OrderingPartyStatus_NotStarted(
            Order order,
            TaskListService service)
        {
            order.OrderingPartyContact = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.OrderingPartyStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NoSupplier_SupplierStatus_NotStarted(
            Order order,
            TaskListService service)
        {
            order.Supplier = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.SupplierStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NoSupplierContact_SupplierStatus_InProgress(
            Order order,
            TaskListService service)
        {
            order.SupplierContactId = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.SupplierStatus.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonAutoData]
        public static void NoSupplier_NoOrderingParty_SupplierStatus_CannotStart(
            Order order,
            TaskListService service)
        {
            order.Supplier = null;
            order.OrderingPartyContact = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.SupplierStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void NoCommencementDate_CommencementDateStatus_NotStarted(
            Order order,
            TaskListService service)
        {
            order.CommencementDate = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.CommencementDateStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NoCommencementDate_NoSupplierContact_CommencementDateStatus_CannotStart(
            Order order,
            TaskListService service)
        {
            order.CommencementDate = null;
            order.SupplierContactId = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.CommencementDateStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void NoSolutions_WithAssociatedServices_CatalogueSolutionStatus_Optional(
            Order order,
            OrderItem orderItem,
            TaskListService service)
        {
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            order.AddOrUpdateOrderItem(orderItem);

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.CatalogueSolutionsStatus.Should().Be(TaskProgress.Optional);
        }

        [Theory]
        [CommonAutoData]
        public static void NoSolutions_NoAssociatedServices_CatalogueSolutionStatus_NoStarted(
            Order order,
            TaskListService service)
        {
            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.CatalogueSolutionsStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NoSolutions_NoAssociatedServices_NoCommencementDate_CatalogueSolutionStatus_CannotStart(
             Order order,
             TaskListService service)
        {
            order.CommencementDate = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.CatalogueSolutionsStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void NoAdditionalServices_AdditionalServiceStatus_Optional(
            Order order,
            OrderItem orderItem,
            TaskListService service)
        {
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.AddOrUpdateOrderItem(orderItem);

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.AdditionalServiceStatus.Should().Be(TaskProgress.Optional);
        }

        [Theory]
        [CommonAutoData]
        public static void NoAdditionalServices_NoSolutions_AdditionalServiceStatus_CannotStart(
            Order order,
            TaskListService service)
        {
            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.AdditionalServiceStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void NoAssociatedServices_AssociatedServiceStatus_Optional(
            Order order,
            OrderItem orderItem,
            TaskListService service)
        {
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.AddOrUpdateOrderItem(orderItem);

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.AssociatedServiceStatus.Should().Be(TaskProgress.Optional);
        }

        [Theory]
        [CommonAutoData]
        public static void NoAssociatedServices_NoSolutions_AssociatedServiceStatus_NoStarted(
            Order order,
            TaskListService service)
        {
            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.AssociatedServiceStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NoAssociatedServices_NoSolutions_NoCommencementDate_AssociatedServiceStatus_CannotStart(
            Order order,
            TaskListService service)
        {
            order.CommencementDate = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.AssociatedServiceStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void WithSolutions_NoAssociatedServices_FundingSourceStatus_NotStarted(
            Order order,
            OrderItem orderItem,
            TaskListService service)
        {
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.AddOrUpdateOrderItem(orderItem);

            order.FundingSourceOnlyGms = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.FundingSourceStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NoSolutions_WithAssociatedServices_FundingSourceStatus_NotStarted(
            Order order,
            OrderItem orderItem,
            TaskListService service)
        {
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            order.AddOrUpdateOrderItem(orderItem);
            order.FundingSourceOnlyGms = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.FundingSourceStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void NoSolutions_NoAssociatedServices_FundingSourceStatus_CannotStart(
            Order order,
            TaskListService service)
        {
            order.FundingSourceOnlyGms = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.FundingSourceStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void NoFundingSource_ReviewAndCompleteStatus_CannotStart(
            Order order,
            TaskListService service)
        {
            order.FundingSourceOnlyGms = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.ReviewAndCompleteStatus.Should().Be(TaskProgress.CannotStart);
        }
    }
}

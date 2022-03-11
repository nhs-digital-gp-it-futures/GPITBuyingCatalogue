﻿using FluentAssertions;
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

        /*TODO - Tiered Pricing - Fix Test
        [Theory]
        [CommonAutoData]
        public static void CompleteOrder_AllStatuses_Correct(
            Order order,
            OrderItem solutionOrderItem,
            OrderItem associatedServiceOrderItem,
            OrderItem additionalServiceOrderItem,
            TaskListService service)
        {
            order.ConfirmedFundingSource = true;
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
            actual.ReviewAndCompleteStatus.Should().Be(TaskProgress.NotStarted);
        }*/

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

        /*TODO - Tiered Pricing - Fix Test
        [Theory]
        [CommonAutoData]
        public static void NoFundingSource_ReviewAndCompleteStatus_CannotStart(
            Order order,
            TaskListService service)
        {
            order.FundingSourceOnlyGms = null;
            order.ConfirmedFundingSource = null;

            var actual = service.GetTaskListStatusModelForOrder(order);

            actual.ReviewAndCompleteStatus.Should().Be(TaskProgress.CannotStart);
        }*/
    }
}

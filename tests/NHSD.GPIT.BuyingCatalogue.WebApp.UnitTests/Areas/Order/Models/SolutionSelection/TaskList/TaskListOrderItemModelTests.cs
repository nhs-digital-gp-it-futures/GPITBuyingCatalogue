﻿using System.Linq;
using FluentAssertions;
using LinqKit;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.TaskList;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.TaskList
{
    public class TaskListOrderItemModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(callOffId);
            model.CatalogueItemId.Should().Be(orderItem.CatalogueItemId);
            model.Name.Should().Be(orderItem.CatalogueItem.Name);
        }

        [Theory]
        [CommonAutoData]
        public static void ServiceRecipientStatus_NoServiceRecipientsEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemRecipients.Clear();

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.ServiceRecipientsStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void ServiceRecipientStatus_ServiceRecipientsEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.ServiceRecipientsStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void PriceStatus_NoServiceRecipientsEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemRecipients.Clear();

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.PriceStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void PriceStatus_NoPriceEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice.OrderItemPriceTiers.Clear();

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.PriceStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void PriceStatus_PriceEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.PriceStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_NoServiceRecipientsEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemRecipients.Clear();

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_NoPriceEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice.OrderItemPriceTiers.Clear();

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_NoQuantityEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.Quantity = null;
            orderItem.OrderItemRecipients.ForEach(x => x.Quantity = null);

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_PerServiceRecipientProvisioningType_OrderItemQuantityEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.PerServiceRecipient;

            orderItem.Quantity = 1;
            orderItem.OrderItemRecipients.ForEach(x => x.Quantity = null);

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_PerServiceRecipientProvisioningType_OrderItemRecipientQuantitiesEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.PerServiceRecipient;

            orderItem.Quantity = null;
            orderItem.OrderItemRecipients.ForEach(x => x.Quantity = 1);

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_PerOrderItemProvisioningType_OrderItemQuantityEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Declarative;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType =
                CataloguePriceQuantityCalculationType.PerSolutionOrService;

            orderItem.Quantity = 1;
            orderItem.OrderItemRecipients.ForEach(x => x.Quantity = null);

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_PerOrderItemProvisioningType_OrderItemRecipientQuantitiesEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Declarative;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType =
                CataloguePriceQuantityCalculationType.PerSolutionOrService;

            orderItem.Quantity = null;
            orderItem.OrderItemRecipients.ForEach(x => x.Quantity = 1);

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_OrderItemRecipientQuantitiesPartiallyEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.PerServiceRecipient;

            orderItem.Quantity = null;
            orderItem.OrderItemRecipients.ForEach(x => x.Quantity = null);
            orderItem.OrderItemRecipients.First().Quantity = 1;

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.InProgress);
        }
    }
}

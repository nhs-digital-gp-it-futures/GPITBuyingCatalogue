﻿using System;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList;
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
            OrderItem orderItem,
            int numberOfPrices,
            int priceId)
        {
            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, orderItem)
            {
                NumberOfPrices = numberOfPrices,
                PriceId = priceId,
            };

            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(callOffId);
            model.CatalogueItemId.Should().Be(orderItem.CatalogueItemId);
            model.Name.Should().Be(orderItem.CatalogueItem.Name);
            model.IsAmendment.Should().Be(callOffId.IsAmendment);
            model.NumberOfPrices.Should().Be(numberOfPrices);
            model.PriceId.Should().Be(priceId);
        }

        [Theory]
        [CommonAutoData]
        public static void PriceStatus_NoPriceEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice.OrderItemPriceTiers.Clear();

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, orderItem);

            model.PriceStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void PriceStatus_PriceEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, orderItem);

            model.PriceStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_NoPriceEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice.OrderItemPriceTiers.Clear();

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_NoQuantityEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.Quantity = null;
            recipients.ForEach(x => x.OrderItemRecipients.Clear());

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, recipients, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_PerServiceRecipientProvisioningType_OrderItemQuantityEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.PerServiceRecipient;

            orderItem.Quantity = 1;
            recipients.ForEach(x => x.OrderItemRecipients.Clear());

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, recipients, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_PerServiceRecipientProvisioningType_OrderItemRecipientQuantitiesEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.PerServiceRecipient;

            orderItem.Quantity = null;
            recipients.ForEach(x => x.SetQuantityForItem(orderItem.CatalogueItemId, 1));

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_Amendment_PerServiceRecipientProvisioningType_OrderItemRecipientQuantitiesEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.PerServiceRecipient;

            orderItem.Quantity = null;
            recipients.ForEach(x => x.SetQuantityForItem(orderItem.CatalogueItemId, 1));

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, recipients, orderItem)
            {
                FromPreviousRevision = true,
                HasCurrentAmendments = true,
            };

            model.QuantityStatus.Should().Be(TaskProgress.Amended);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_PerOrderItemProvisioningType_OrderItemQuantityEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Declarative;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType =
                CataloguePriceQuantityCalculationType.PerSolutionOrService;

            orderItem.Quantity = 1;
            recipients.ForEach(x => x.OrderItemRecipients.Clear());

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, recipients, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_Amendment_PerOrderItemProvisioningType_OrderItemQuantityEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Declarative;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType =
                CataloguePriceQuantityCalculationType.PerSolutionOrService;

            orderItem.Quantity = 1;
            recipients.ForEach(x => x.OrderItemRecipients.Clear());

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, recipients, orderItem)
            {
                FromPreviousRevision = true,
                HasCurrentAmendments = true,
            };

            model.QuantityStatus.Should().Be(TaskProgress.Amended);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_PerOrderItemProvisioningType_OrderItemRecipientQuantitiesEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.Declarative;
            orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType =
                CataloguePriceQuantityCalculationType.PerSolutionOrService;

            orderItem.Quantity = null;
            recipients.ForEach(x => x.SetQuantityForItem(orderItem.CatalogueItemId, 1));

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, recipients, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void QuantityStatus_OrderItemRecipientQuantitiesPartiallyEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = ProvisioningType.PerServiceRecipient;

            orderItem.Quantity = null;
            recipients.ForEach(x => x.OrderItemRecipients.Clear());
            recipients.First().SetQuantityForItem(orderItem.CatalogueItemId, 1);

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, recipients, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonAutoData]
        public static void DeliveryDatesStatus_NoOrderItemRecipients_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            var model = new TaskListOrderItemModel(internalOrgId, callOffId, recipients, orderItem);

            model.DeliveryDatesStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void DeliveryDatesStatus_NoDeliveryDatesEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            recipients.ForEach(r => r.OrderItemRecipients.Clear());
            recipients.ForEach(r => r.SetQuantityForItem(orderItem.CatalogueItemId, 1));

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, recipients, orderItem);

            model.DeliveryDatesStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void DeliveryDatesStatus_SomeDeliveryDatesEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            recipients.ForEach(r => r.OrderItemRecipients.Clear());
            recipients.ForEach(r => r.SetQuantityForItem(orderItem.CatalogueItemId, 1));
            recipients.First().SetDeliveryDateForItem(orderItem.CatalogueItemId, DateTime.Today);

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, recipients, orderItem);

            model.DeliveryDatesStatus.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonAutoData]
        public static void DeliveryDatesStatus_AllDeliveryDatesEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            recipients.ForEach(r => r.SetQuantityForItem(orderItem.CatalogueItemId, 1));
            recipients.ForEach(r => r.SetDeliveryDateForItem(orderItem.CatalogueItemId, DateTime.Today));

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, recipients, orderItem);

            model.DeliveryDatesStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void DeliveryDatesStatus_Amendment_AllDeliveryDatesEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            recipients.ForEach(r => r.SetQuantityForItem(orderItem.CatalogueItemId, 1));
            recipients.ForEach(r => r.SetDeliveryDateForItem(orderItem.CatalogueItemId, DateTime.Today));

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, recipients, orderItem)
            {
                FromPreviousRevision = true,
                HasCurrentAmendments = true,
            };

            model.DeliveryDatesStatus.Should().Be(TaskProgress.Amended);
        }
    }
}

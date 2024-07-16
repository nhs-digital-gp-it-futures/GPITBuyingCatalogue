using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.TaskList
{
    public class TaskListOrderItemModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesSetCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            OrderTypeEnum orderType,
            OrderItem orderItem,
            int numberOfPrices,
            int priceId)
        {
            var model = new TaskListOrderItemModel(internalOrgId, callOffId, orderType, null, orderItem)
            {
                NumberOfPrices = numberOfPrices,
                PriceId = priceId,
            };

            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(callOffId);
            model.OrderType.Value.Should().Be(orderType);
            model.CatalogueItemId.Should().Be(orderItem.CatalogueItemId);
            model.Name.Should().Be(orderItem.CatalogueItem.Name);
            model.IsAmendment.Should().Be(callOffId.IsAmendment);
            model.NumberOfPrices.Should().Be(numberOfPrices);
            model.PriceId.Should().Be(priceId);
        }

        [Theory]
        [MockAutoData]
        public static void PriceStatus_NoPriceEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice.OrderItemPriceTiers.Clear();

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, null, orderItem);

            model.PriceStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void PriceStatus_PriceEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, null, orderItem);

            model.PriceStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockAutoData]
        public static void QuantityStatus_NoPriceEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice.OrderItemPriceTiers.Clear();

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, null, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void QuantityStatus_NoQuantityEntered_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.Quantity = null;
            recipients.ForEach(x => x.OrderItemRecipients.Clear());

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, recipients, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData(ProvisioningType.Patient, null)]
        [MockInlineAutoData(ProvisioningType.OnDemand, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        [MockInlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        public static void QuantityStatus_PerServiceRecipient_Price_OrderItemQuantityEntered_ExpectedResult(
            ProvisioningType provisioningType,
            CataloguePriceQuantityCalculationType? cataloguePriceQuantityCalculationType,
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            if (cataloguePriceQuantityCalculationType.HasValue)
            {
                orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType.Value;
            }

            orderItem.Quantity = 1;
            recipients.ForEach(x => x.OrderItemRecipients.Clear());

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, recipients, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData(ProvisioningType.Patient, null)]
        [MockInlineAutoData(ProvisioningType.OnDemand, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        [MockInlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        public static void QuantityStatus_PerServiceRecipient_Price_OrderItemRecipientQuantitiesEntered_ExpectedResult(
            ProvisioningType provisioningType,
            CataloguePriceQuantityCalculationType? cataloguePriceQuantityCalculationType,
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            if (cataloguePriceQuantityCalculationType.HasValue)
            {
                orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType.Value;
            }

            orderItem.Quantity = null;
            recipients.ForEach(x => x.SetQuantityForItem(orderItem.CatalogueItemId, 1));

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, null, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(ProvisioningType.Patient, null)]
        [MockInlineAutoData(ProvisioningType.OnDemand, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        [MockInlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        public static void QuantityStatus_Amendment_PerServiceRecipient_Price_OrderItemRecipientQuantitiesEntered_ExpectedResult(
            ProvisioningType provisioningType,
            CataloguePriceQuantityCalculationType? cataloguePriceQuantityCalculationType,
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            if (cataloguePriceQuantityCalculationType.HasValue)
            {
                orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType.Value;
            }

            orderItem.Quantity = null;
            recipients.ForEach(x => x.SetQuantityForItem(orderItem.CatalogueItemId, 1));

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, recipients, orderItem)
            {
                FromPreviousRevision = true,
                HasNewRecipients = true,
            };

            model.QuantityStatus.Should().Be(TaskProgress.Amended);
        }

        [Theory]
        [MockAutoData]
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

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, recipients, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockAutoData]
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

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, recipients, orderItem)
            {
                FromPreviousRevision = true,
                HasNewRecipients = true,
                IsPerServiceRecipient = true,
            };

            model.QuantityStatus.Should().Be(TaskProgress.Amended);
        }

        [Theory]
        [MockAutoData]
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

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, recipients, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockInlineAutoData(ProvisioningType.Patient, null)]
        [MockInlineAutoData(ProvisioningType.OnDemand, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        [MockInlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        public static void QuantityStatus_OrderItemRecipientQuantitiesPartiallyEntered_ExpectedResult(
            ProvisioningType provisioningType,
            CataloguePriceQuantityCalculationType? cataloguePriceQuantityCalculationType,
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            if (cataloguePriceQuantityCalculationType.HasValue)
            {
                orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType.Value;
            }

            orderItem.Quantity = null;
            recipients.ForEach(x => x.OrderItemRecipients.Clear());
            recipients.First().SetQuantityForItem(orderItem.CatalogueItemId, 1);

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, recipients, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void QuantityStatus_AssociatedServiceAmendment_ExpectedResult(
            string internalOrgId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            var callOffId = new CallOffId(1, 2);

            orderItem.Quantity = null;

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, recipients, orderItem) { IsAssociatedService = true };

            model.QuantityStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(false, TaskProgress.Completed)]
        [MockInlineAutoData(true, TaskProgress.Amended)]
        public static void QuantityStatus_NonPerServiceRecipientAmendment_ExpectedResult(
            bool quantityChanged,
            TaskProgress taskProgress,
            string internalOrgId,
            OrderItem orderItem,
            OrderRecipient[] recipients)
        {
            var callOffId = new CallOffId(1, 2);

            orderItem.Quantity = null;

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, recipients, orderItem) { IsPerServiceRecipient = false, FromPreviousRevision = true, QuantityChanged = quantityChanged };

            model.QuantityStatus.Should().Be(taskProgress);
        }
    }
}

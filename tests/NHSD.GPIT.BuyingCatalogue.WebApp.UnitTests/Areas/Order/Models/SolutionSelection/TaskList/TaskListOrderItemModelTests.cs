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
            OrderSublocationRecipient[] recipients)
        {
            recipients.ForEach(x => x.OrderItemSublocationRecipients.Clear());

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
            OrderSublocationRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            if (cataloguePriceQuantityCalculationType.HasValue)
            {
                orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType.Value;
            }

            recipients.ForEach(x => x.OrderItemSublocationRecipients.Clear());

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
            OrderSublocationRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            if (cataloguePriceQuantityCalculationType.HasValue)
            {
                orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType.Value;
            }

            recipients.ForEach(x => x.SetQuantityForItem(orderItem, 1));

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, null, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(ProvisioningType.Patient, null)]
        [MockInlineAutoData(ProvisioningType.OnDemand, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        [MockInlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerServiceRecipient)]
        [MockInlineAutoData(ProvisioningType.OnDemand, CataloguePriceQuantityCalculationType.PerSolutionOrService)]
        [MockInlineAutoData(ProvisioningType.Declarative, CataloguePriceQuantityCalculationType.PerSolutionOrService)]
        public static void
            QuantityStatus_Amendment_PerServiceRecipient_Price_OrderItemRecipientQuantitiesEntered_ExpectedResult(
                ProvisioningType provisioningType,
                CataloguePriceQuantityCalculationType? cataloguePriceQuantityCalculationType,
                string internalOrgId,
                CallOffId callOffId,
                OrderItem orderItem,
                OrderSublocationRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            if (cataloguePriceQuantityCalculationType.HasValue)
            {
                orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType.Value;
            }

            recipients.ForEach(x => x.SetQuantityForItem(orderItem, 1));

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, recipients, orderItem)
            {
                FromPreviousRevision = true,
                HasNewRecipients = true,
            };

            model.QuantityStatus.Should().Be(TaskProgress.Amended);
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
            OrderSublocationRecipient[] recipients)
        {
            orderItem.OrderItemPrice.ProvisioningType = provisioningType;
            if (cataloguePriceQuantityCalculationType.HasValue)
            {
                orderItem.OrderItemPrice.CataloguePriceQuantityCalculationType = cataloguePriceQuantityCalculationType.Value;
            }

            recipients.ForEach(x => x.OrderItemSublocationRecipients.Clear());
            recipients.First().SetQuantityForItem(orderItem, 1);

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, recipients, orderItem);

            model.QuantityStatus.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void QuantityStatus_AssociatedServiceAmendment_ExpectedResult(
            string internalOrgId,
            OrderItem orderItem,
            OrderSublocationRecipient[] recipients)
        {
            var callOffId = new CallOffId(1, 2);

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, null, recipients, orderItem)
            {
                IsAssociatedService = true,
                FromPreviousRevision = true,
            };

            model.QuantityStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(false, 1, TaskProgress.Completed)]
        [MockInlineAutoData(true, 1, TaskProgress.InProgress)]
        [MockInlineAutoData(true, 0, TaskProgress.Completed)]
        public static void AssociatedServicesStatus_PreviousAssociatedServices_ReturnsExpectedResult(
            bool hasNewRecipients,
            int associatedServicesCount,
            TaskProgress expected,
            string internalOrgId,
            OrderItem orderItem,
            OrderItem associatedService,
            OrderSublocationRecipient[] recipients)
        {
            SetOrderItemPriceStatus(orderItem, true);
            SetOrderItemPriceStatus(associatedService, true);
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            recipients.ForEach(x => x.OrderItemSublocationRecipients.Clear());

            var model = new TaskListOrderItemModel(internalOrgId, new CallOffId(1, 1), OrderTypeEnum.Solution, recipients, orderItem)
            {
                HasNewRecipients = hasNewRecipients,
                PreviousAssociatedServicesOrderItems = 1,
                AssociatedServicesOrderItems = associatedServicesCount > 0 ? [associatedService] : [],
            };

            model.AssociatedServicesStatus.Should().Be(expected);
        }

        [Theory]
        [MockAutoData]
        public static void AssociatedServicesStatus_CurrentAssociatedServicesComplete_ReturnsCompleted(
            string internalOrgId,
            OrderItem orderItem,
            OrderItem associatedService,
            OrderSublocationRecipient[] recipients)
        {
            SetOrderItemPriceStatus(orderItem, true);
            SetOrderItemPriceStatus(associatedService, true);
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            recipients.ForEach(x =>
            {
                x.OrderItemSublocationRecipients.Clear();
                x.SetQuantityForItem(associatedService, 1);
            });

            var model = new TaskListOrderItemModel(internalOrgId, new CallOffId(1, 1), OrderTypeEnum.Solution, recipients, orderItem)
            {
                AssociatedServicesOrderItems = [associatedService],
            };

            model.AssociatedServicesStatus.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockAutoData]
        public static void AssociatedServicesStatus_CurrentAssociatedServicesCompleteForAmendment_ReturnsAmended(
            string internalOrgId,
            OrderItem orderItem,
            OrderItem associatedService,
            OrderSublocationRecipient[] recipients)
        {
            SetOrderItemPriceStatus(orderItem, true);
            SetOrderItemPriceStatus(associatedService, true);
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            recipients.ForEach(x =>
            {
                x.OrderItemSublocationRecipients.Clear();
                x.SetQuantityForItem(associatedService, 1);
            });

            var model = new TaskListOrderItemModel(internalOrgId, new CallOffId(1, 2), OrderTypeEnum.Solution, recipients, orderItem)
            {
                AssociatedServicesOrderItems = [associatedService],
            };

            model.AssociatedServicesStatus.Should().Be(TaskProgress.Amended);
        }

        [Theory]
        [MockAutoData]
        public static void AssociatedServicesStatus_CurrentAssociatedServicePriceNotStarted_ReturnsInProgress(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderItem associatedService,
            OrderSublocationRecipient[] recipients)
        {
            SetOrderItemPriceStatus(orderItem, true);
            SetOrderItemPriceStatus(associatedService, false);
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            recipients.ForEach(x =>
            {
                x.OrderItemSublocationRecipients.Clear();
                x.SetQuantityForItem(associatedService, 1);
            });

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, OrderTypeEnum.Solution, recipients, orderItem)
            {
                AssociatedServicesOrderItems = [associatedService],
            };

            model.AssociatedServicesStatus.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void AssociatedServicesStatus_CurrentAssociatedServiceQuantityNotStarted_ReturnsInProgress(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderItem associatedService,
            OrderSublocationRecipient[] recipients)
        {
            SetOrderItemPriceStatus(orderItem, true);
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            recipients.ForEach(x => x.OrderItemSublocationRecipients.Clear());

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, OrderTypeEnum.Solution, recipients, orderItem)
            {
                AssociatedServicesOrderItems = [associatedService],
            };

            model.AssociatedServicesStatus.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void AssociatedServicesStatus_NoAssociatedServicesQuantityComplete_ReturnsNotStarted(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderSublocationRecipient[] recipients)
        {
            SetOrderItemPriceStatus(orderItem, true);
            recipients.ForEach(x =>
            {
                x.OrderItemSublocationRecipients.Clear();
                x.SetQuantityForItem(orderItem, 1);
            });

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, OrderTypeEnum.Solution, recipients, orderItem);

            model.AssociatedServicesStatus.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void AssociatedServicesStatus_NoAssociatedServicesQuantityIncomplete_ReturnsOptional(
            string internalOrgId,
            CallOffId callOffId,
            OrderItem orderItem,
            OrderSublocationRecipient[] recipients)
        {
            SetOrderItemPriceStatus(orderItem, true);
            recipients.ForEach(x => x.OrderItemSublocationRecipients.Clear());

            var model = new TaskListOrderItemModel(internalOrgId, callOffId, OrderTypeEnum.Solution, recipients, orderItem);

            model.AssociatedServicesStatus.Should().Be(TaskProgress.Optional);
        }

        private static void SetOrderItemPriceStatus(OrderItem orderItem, bool completed)
        {
            orderItem.OrderItemPrice = new OrderItemPrice();
            orderItem.OrderItemPrice.OrderItemPriceTiers.Clear();

            if (completed)
            {
                orderItem.OrderItemPrice.OrderItemPriceTiers.Add(new OrderItemPriceTier());
            }
        }
    }
}

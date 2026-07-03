using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;
using Contract = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.Contract;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models
{
    public static class OrderSummaryModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            ImplementationPlan defaultPlan,
            Contract contract,
            Order order)
        {
            contract.ImplementationPlan = new ImplementationPlan()
            {
                Milestones = new List<ImplementationPlanMilestone>() { new ImplementationPlanMilestone(), },
            };

            order.Contract = contract;

            var model = new OrderSummaryModel(new OrderWrapper(order), defaultPlan);

            model.DefaultImplementationPlan.Should().Be(defaultPlan);
            model.BespokePlan.Should().BeEquivalentTo(order.Contract.ImplementationPlan);
            model.HasBespokeMilestones.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void NullBespokePlan_PropertiesCorrectlySet(
            ImplementationPlan defaultPlan,
            Contract contract,
            Order order)
        {
            contract.ImplementationPlan = null;

            order.Contract = contract;

            var model = new OrderSummaryModel(new OrderWrapper(order), defaultPlan);

            model.HasBespokeMilestones.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void NoMilestones_PropertiesCorrectlySet(
            ImplementationPlan defaultPlan,
            Contract contract,
            Order order)
        {
            contract.ImplementationPlan = new ImplementationPlan()
            {
                Milestones = new List<ImplementationPlanMilestone>(),
            };

            order.Contract = contract;

            var model = new OrderSummaryModel(new OrderWrapper(order), defaultPlan);

            model.HasBespokeMilestones.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void BuildAmendOrderItemModel_PropertiesCorrectlySet(
            ImplementationPlan implementationPlan,
            Order order)
        {
            var orderItem = order.OrderItems.First();
            var wrapper = new OrderWrapper(order);
            var model = new OrderSummaryModel(wrapper, implementationPlan);

            var result = model.BuildAmendOrderItemModel(orderItem);

            List<OrderSublocationRecipient> flattenedRecipients = wrapper.RolledUp.FlattenedRecipients.ToList();

            result.CallOffId.Should().Be(order.CallOffId);
            result.OrderType.Should().Be(order.OrderType);
            result.IsAmendment.Should().Be(order.IsAmendment);
            result.IsOrderItemAdded.Should().BeTrue();
            result.OrderItemPrice.Should().Be(orderItem.OrderItemPrice);
            result.CatalogueItem.Should().Be(orderItem.CatalogueItem);
            result.RolledUpRecipientsForItem.Should()
                .BeEquivalentTo(flattenedRecipients.ForCatalogueItem(orderItem.CatalogueItemId));
            result.RolledUpTotalQuantity.Should()
                .Be(orderItem.TotalQuantity(flattenedRecipients.ForCatalogueItem(orderItem.CatalogueItemId)));
            result.PreviousTotalQuantity.Should().Be(0);
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [MockInlineAutoData(OrderTypeEnum.Solution)]
        public static void BuildAmendOrderItemModel_PracticeReorganisationName(
            OrderTypeEnum orderType,
            ImplementationPlan implementationPlan,
            Order order)
        {
            order.OrderType = orderType;

            var orderItem = order.OrderItems.First();
            var wrapper = new OrderWrapper(order);
            var model = new OrderSummaryModel(wrapper, implementationPlan);

            var result = model.BuildAmendOrderItemModel(orderItem);
            result.PracticeReorganisationName.Should().BeNull();
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        public static void BuildAmendOrderItemModel_MergerSplit_PracticeReorganisationName(
            OrderTypeEnum orderType,
            ImplementationPlan implementationPlan,
            Order order)
        {
            order.OrderType = orderType;
            var orderItem = order.OrderItems.First();
            var wrapper = new OrderWrapper(order);
            var model = new OrderSummaryModel(wrapper, implementationPlan);

            var result = model.BuildAmendOrderItemModel(orderItem);
            var expectedName = $"{order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Name} ({order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode})";
            result.PracticeReorganisationName.Should().Be(expectedName);
        }

        [Theory]
        [MockAutoData]
        public static void AssociatedServices_PropertiesCorrectlySet(
            OrderItem orderItem,
            Order order)
        {
            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            orderItem.Order = order;
            order.Revision = 1;
            order.OrderItems = [orderItem];

            var newOrder = order.Clone();
            newOrder.Revision = 2;
            newOrder.OrderItems = [orderItem];

            var previousOrders = new List<Order> { order };

            var orderWrapper = new OrderWrapper(newOrder, previousOrders);

            var model = new OrderSummaryModel(orderWrapper, new ImplementationPlan());

            model.AssociatedServicesForCurrentOrder.Should().BeEquivalentTo([orderItem]);
            model.PreviousAssociatedServicesGrouping.Should().BeEquivalentTo(new List<OrderItem>() { orderItem }
                .GroupBy(oi => oi.Order.CallOffId));
        }

        [Theory]
        [MockAutoData]
        public static void AssociatedServicesForAdditionalServices_PropertyCorrectlySet(
            int orderItemId,
            OrderItem additionalService,
            OrderItem associatedService,
            CatalogueItem catalogueItem,
            Order order)
        {
            additionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            catalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            var additionalService2 = new OrderItem { Id = orderItemId, CatalogueItem = catalogueItem };
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            associatedService.ParentId = additionalService2.Id;
            associatedService.Parent = additionalService2;

            var associatedService2 = order.OrderItems.First();
            associatedService2.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            associatedService2.ParentId = additionalService.Id;
            associatedService2.Parent = additionalService;

            order.OrderItems.Clear();
            order.OrderItems.AddRange(new List<OrderItem> { additionalService, additionalService2, associatedService, associatedService2 });

            var expected = new Dictionary<int?, HashSet<OrderItem>>
            {
                { additionalService.Id, new HashSet<OrderItem> { associatedService2 } },
                { additionalService2.Id, new HashSet<OrderItem> { associatedService } },
            };

            var orderWrapper = new OrderWrapper(order);

            var model = new OrderSummaryModel(orderWrapper, new ImplementationPlan());

            model.AssociatedServicesForAdditionalServices.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public static void PreviousAssociatedServicesForAdditionalServices_Property_SetCorrectly(
            Order current,
            OrderItem solution,
            OrderItem ignoredAssociatedService,
            Order initialOrder,
            Order amendedOrder)
        {
            var additionalService1 = BuildOrderItem(
                1,
                new CatalogueItemId(1, "add1"),
                CatalogueItemType.AdditionalService,
                null);
            var additionalService2 = BuildOrderItem(
                2,
                new CatalogueItemId(2, "add2"),
                CatalogueItemType.AdditionalService,
                null);

            var associatedService1 = BuildOrderItem(
                3,
                default,
                CatalogueItemType.AssociatedService,
                additionalService1);
            var associatedService2 = BuildOrderItem(
                4,
                default,
                CatalogueItemType.AssociatedService,
                additionalService2);

            var amendAssociatedService1 = BuildOrderItem(
                5,
                default,
                CatalogueItemType.AssociatedService,
                additionalService1);
            var amendAssociatedService2 = BuildOrderItem(
                6,
                default,
                CatalogueItemType.AssociatedService,
                additionalService2);

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            ignoredAssociatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            ignoredAssociatedService.Parent = solution;

            initialOrder.OrderItems = [additionalService1, additionalService2, associatedService1, associatedService2];
            initialOrder.OrderItems.ForEach(oi => oi.Order = initialOrder);

            amendedOrder.OrderItems = [additionalService1, additionalService2, amendAssociatedService1, amendAssociatedService2, ignoredAssociatedService];
            amendedOrder.OrderItems.ForEach(oi => oi.Order = amendedOrder);

            var orderWrapper = new OrderWrapper(current, [initialOrder, amendedOrder]);

            var expected = new Dictionary<CatalogueItemId, Dictionary<CallOffId, List<OrderItem>>>
            {
                {
                    additionalService1.CatalogueItemId,
                    new Dictionary<CallOffId, List<OrderItem>> { { initialOrder.CallOffId, [associatedService1] }, { amendedOrder.CallOffId, [amendAssociatedService1] } }
                },
                {
                    additionalService2.CatalogueItemId,
                    new Dictionary<CallOffId, List<OrderItem>> { { initialOrder.CallOffId, [associatedService2] }, { amendedOrder.CallOffId, [amendAssociatedService2] } }
                },
            };

            var model = new OrderSummaryModel(orderWrapper, new ImplementationPlan());

            model.PreviousAssociatedServicesForAdditionalServices.Should().BeEquivalentTo(expected);
        }

        private static OrderItem BuildOrderItem(
            int id,
            CatalogueItemId catalogueItemId,
            CatalogueItemType catalogueItemType,
            OrderItem parent)
        {
            return new OrderItem
            {
                Id = id,
                CatalogueItemId = catalogueItemId,
                CatalogueItem = new CatalogueItem { CatalogueItemType = catalogueItemType },
                Parent = parent,
            };
        }
    }
}

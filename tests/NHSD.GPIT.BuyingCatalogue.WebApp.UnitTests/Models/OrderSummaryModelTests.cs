﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;
using Contract = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.Contract;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models
{
    public static class OrderSummaryModelTests
    {
        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
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
        [CommonAutoData]
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
        [CommonAutoData]
        public static void BuildAmendOrderItemModel_PropertiesCorrectlySet(
            ImplementationPlan implementationPlan,
            Order order)
        {
            var orderItem = order.OrderItems.First();
            var wrapper = new OrderWrapper(order);
            var model = new OrderSummaryModel(wrapper, implementationPlan);

            var result = model.BuildAmendOrderItemModel(orderItem);
            result.CallOffId.Should().Be(order.CallOffId);
            result.OrderType.Should().Be(order.OrderType);
            result.IsAmendment.Should().Be(order.IsAmendment);
            result.IsOrderItemAdded.Should().BeTrue();
            result.OrderItemPrice.Should().Be(orderItem.OrderItemPrice);
            result.CatalogueItem.Should().Be(orderItem.CatalogueItem);
            result.RolledUpRecipientsForItem.Should().BeEquivalentTo(wrapper.RolledUp.OrderRecipients.ForCatalogueItem(orderItem.CatalogueItemId));
            result.RolledUpTotalQuantity.Should().Be(orderItem.TotalQuantity(wrapper.RolledUp.OrderRecipients.ForCatalogueItem(orderItem.CatalogueItemId)));
            result.PreviousTotalQuantity.Should().Be(0);
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [CommonInlineAutoData(OrderTypeEnum.Solution)]
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
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
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
    }
}

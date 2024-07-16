using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class SummaryModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            bool hasSubsequentRevisions,
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new SummaryModel(new OrderWrapper(order), internalOrgId, hasSubsequentRevisions, new ImplementationPlan());

            model.InternalOrgId.Should().Be(internalOrgId);
            model.Order.Should().BeEquivalentTo(order);
            model.HasSubsequentRevisions.Should().Be(hasSubsequentRevisions);
            model.CanBeTerminated.Should().Be(order.OrderStatus == OrderStatus.Completed && !hasSubsequentRevisions);
            model.CanBeAmended.Should().Be(!order.OrderType.AssociatedServicesOnly && order.OrderStatus == OrderStatus.Completed && !hasSubsequentRevisions && !order.ContractExpired);
        }

        [Theory]
        [MockInlineAutoData(false, false, false)]
        [MockInlineAutoData(false, true, true)]
        [MockInlineAutoData(true, false, false)]
        [MockInlineAutoData(true, true, false)]
        public static void CanBeTerminated_PropertiesCorrectlySet(
            bool hasSubsequentRevisions,
            bool completed,
            bool result,
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.Completed = completed ? DateTime.Now : null;
            var model = new SummaryModel(new OrderWrapper(order), internalOrgId, hasSubsequentRevisions, new ImplementationPlan());

            model.CanBeTerminated.Should().Be(result);
        }

        [Theory]
        [MockInlineAutoData(false, false, false, false, false)]
        [MockInlineAutoData(false, false, false, true, true)]
        [MockInlineAutoData(false, false, true, false, false)]
        [MockInlineAutoData(false, false, true, true, false)]
        [MockInlineAutoData(false, true, false, false, false)]
        [MockInlineAutoData(false, true, false, true, false)]
        [MockInlineAutoData(false, true, true, false, false)]
        [MockInlineAutoData(false, true, true, true, false)]
        [MockInlineAutoData(true, false, false, false, false)]
        [MockInlineAutoData(true, false, false, true, false)]
        [MockInlineAutoData(true, false, true, false, false)]
        [MockInlineAutoData(true, false, true, true, false)]
        [MockInlineAutoData(true, true, false, false, false)]
        [MockInlineAutoData(true, true, false, true, false)]
        [MockInlineAutoData(true, true, true, false, false)]
        [MockInlineAutoData(true, true, true, true, false)]
        public static void CanBeAmended_PropertiesCorrectlySet(
            bool contractExpired,
            bool associatedServicesOnly,
            bool hasSubsequentRevisions,
            bool completed,
            bool result,
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.CommencementDate = contractExpired ? DateTime.Now.AddMonths(-2) : null;
            order.MaximumTerm = contractExpired ? 1 : null;
            order.OrderType = associatedServicesOnly
                ? OrderTypeEnum.AssociatedServiceOther
                : OrderTypeEnum.Solution;
            order.Completed = completed ? DateTime.Now : null;
            var model = new SummaryModel(new OrderWrapper(order), internalOrgId, hasSubsequentRevisions, new ImplementationPlan());

            model.CanBeAmended.Should().Be(result);
        }

        [Theory]
        [MockAutoData]
        public static void ValidContract_PropertiesCorrectlySet(
            string internalOrgId,
            bool hasSubsequentRevisions,
            EntityFramework.Ordering.Models.Order order,
            Contract contract,
            ContractBilling contractBilling,
            ImplementationPlan implementationPlan,
            ImplementationPlanMilestone milestone,
            ContractBillingItem billingItem,
            Requirement requirement)
        {
            implementationPlan.Milestones.Clear();
            implementationPlan.Milestones.Add(milestone);

            contractBilling.ContractBillingItems.Clear();
            contractBilling.ContractBillingItems.Add(billingItem);

            contractBilling.Requirements.Clear();
            contractBilling.Requirements.Add(requirement);

            contract.ContractBilling = contractBilling;
            contract.ImplementationPlan = implementationPlan;

            order.Contract = contract;
            var model = new SummaryModel(new OrderWrapper(order), internalOrgId, hasSubsequentRevisions, new ImplementationPlan());

            model.BespokePlan.Should().Be(contract.ImplementationPlan);
            model.BespokeBilling.Should().Be(contract.ContractBilling);
            model.HasBespokeMilestones.Should().BeTrue();
            model.HasBespokeBilling.Should().BeTrue();
            model.HasSpecificRequirements.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void BuildAmendOrderItemModel_PropertiesCorrectlySet(
            string internalOrgId,
            bool hasSubsequentRevisions,
            ImplementationPlan implementationPlan,
            EntityFramework.Ordering.Models.Order order)
        {
            var orderItem = order.OrderItems.First();
            var wrapper = new OrderWrapper(order);
            var model = new SummaryModel(wrapper, internalOrgId, hasSubsequentRevisions, implementationPlan);

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
        [MockAutoData]
        public static void BuildOrderTotals_PropertiesCorrectlySet(
            string internalOrgId,
            bool hasSubsequentRevisions,
            ImplementationPlan implementationPlan,
            EntityFramework.Ordering.Models.Order order)
        {
            var wrapper = new OrderWrapper(order);
            var model = new SummaryModel(wrapper, internalOrgId, hasSubsequentRevisions, implementationPlan);

            var result = model.BuildOrderTotals();
            result.OrderType.Should().Be(order.OrderType);
            result.MaximumTerm.Should().Be(order.MaximumTerm);
            result.TotalOneOffCost.Should().Be(order.TotalOneOffCost(null));
            result.TotalMonthlyCost.Should().Be(order.TotalMonthlyCost(null));
            result.TotalAnnualCost.Should().Be(order.TotalAnnualCost(null));
            result.TotalCost.Should().Be(wrapper.TotalCost());
        }
    }
}

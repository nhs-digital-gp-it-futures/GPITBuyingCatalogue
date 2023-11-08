using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class SummaryModelTests
    {
        [Theory]
        [CommonAutoData]
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
            model.CanBeAmended.Should().Be(!order.AssociatedServicesOnly && order.OrderStatus == OrderStatus.Completed && !hasSubsequentRevisions && !order.ContractExpired);
        }

        [Theory]
        [CommonInlineAutoData(false, false, false)]
        [CommonInlineAutoData(false, true, true)]
        [CommonInlineAutoData(true, false, false)]
        [CommonInlineAutoData(true, true, false)]
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
        [CommonInlineAutoData(false, false, false, false, false)]
        [CommonInlineAutoData(false, false, false, true, true)]
        [CommonInlineAutoData(false, false, true, false, false)]
        [CommonInlineAutoData(false, false, true, true, false)]
        [CommonInlineAutoData(false, true, false, false, false)]
        [CommonInlineAutoData(false, true, false, true, false)]
        [CommonInlineAutoData(false, true, true, false, false)]
        [CommonInlineAutoData(false, true, true, true, false)]
        [CommonInlineAutoData(true, false, false, false, false)]
        [CommonInlineAutoData(true, false, false, true, false)]
        [CommonInlineAutoData(true, false, true, false, false)]
        [CommonInlineAutoData(true, false, true, true, false)]
        [CommonInlineAutoData(true, true, false, false, false)]
        [CommonInlineAutoData(true, true, false, true, false)]
        [CommonInlineAutoData(true, true, true, false, false)]
        [CommonInlineAutoData(true, true, true, true, false)]
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
            order.AssociatedServicesOnly = associatedServicesOnly;
            order.Completed = completed ? DateTime.Now : null;
            var model = new SummaryModel(new OrderWrapper(order), internalOrgId, hasSubsequentRevisions, new ImplementationPlan());

            model.CanBeAmended.Should().Be(result);
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static void FundingTypeDescription_PropertiesCorrectlySet(
            string internalOrgId,
            bool hasSubsequentRevisions,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId catalogueItemId)
        {
            var model = new SummaryModel(new OrderWrapper(order), internalOrgId, hasSubsequentRevisions, new ImplementationPlan());

            var result = model.FundingTypeDescription(catalogueItemId);
            var actual = result.Should().BeOfType<FundingTypeDescriptionModel>().Subject;
            actual.Should().NotBeNull();
        }
    }
}

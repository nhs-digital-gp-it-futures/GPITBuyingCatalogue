﻿using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class AssociatedServicesBillingStatusProviderTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            AssociatedServicesBillingStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_NoAssociatedServices_ReturnsNotApplicable(
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var actual = service.Get(new OrderWrapper(order), new OrderProgress());

            actual.Should().Be(TaskProgress.NotApplicable);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotApplicable)]
        [CommonInlineAutoData(TaskProgress.NotStarted)]
        [CommonInlineAutoData(TaskProgress.Optional)]
        public static void Get_FundingSourceIncomplete_ContractBillingEntered_ReturnsInProgress(
            TaskProgress status,
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = status,
            };

            order.AssociatedServicesOnly = true;
            order.Contract = new Contract() { ContractBilling = new ContractBilling(), };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotStarted)]
        [CommonInlineAutoData(TaskProgress.Optional)]
        public static void Get_ImplementationPlanIncomplete_ContractBillingEntered_ReturnsInProgress(
            TaskProgress status,
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                ImplementationPlan = status,
            };

            order.AssociatedServicesOnly = true;
            order.Contract = new Contract() { ContractBilling = new ContractBilling(), };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotApplicable)]
        [CommonInlineAutoData(TaskProgress.NotStarted)]
        [CommonInlineAutoData(TaskProgress.Optional)]
        public static void Get_FundingSourceIncomplete_ImplementationPlanNotApplicable_ReturnsCannotStart(
            TaskProgress status,
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = status,
                ImplementationPlan = TaskProgress.NotApplicable,
            };

            order.AssociatedServicesOnly = true;
            order.Contract = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotStarted)]
        [CommonInlineAutoData(TaskProgress.Optional)]
        public static void Get_ImplementationPlanIncomplete_ReturnsCannotStart(
            TaskProgress status,
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = TaskProgress.Completed,
                ImplementationPlan = status,
            };

            order.AssociatedServicesOnly = true;
            order.Contract = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_NoContractInfoEntered_ReturnsNotStarted(
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                ImplementationPlan = TaskProgress.Completed,
            };

            order.AssociatedServicesOnly = true;
            order.Contract = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_PartialContractInfoEntered_ReturnsNotStarted(
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                ImplementationPlan = TaskProgress.Completed,
            };

            order.AssociatedServicesOnly = true;
            order.Contract = new Contract();

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ContractInfoEntered_RequirementsNotCompleted_ReturnsInProgress(
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = TaskProgress.Completed,
                ImplementationPlan = TaskProgress.Completed,
            };

            order.AssociatedServicesOnly = true;
            order.Contract = new Contract() { ContractBilling = new ContractBilling(), };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ContractInfoEntered_RequirementsCompleted_ReturnsInCompleted(
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = TaskProgress.Completed,
                ImplementationPlan = TaskProgress.Completed,
            };

            order.AssociatedServicesOnly = true;
            order.Contract = new Contract() { ContractBilling = new ContractBilling() { HasConfirmedRequirements = true, }, };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}

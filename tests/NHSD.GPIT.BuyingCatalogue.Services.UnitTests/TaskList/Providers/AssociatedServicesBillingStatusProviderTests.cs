using FluentAssertions;
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
        [CommonAutoData]
        public static void Get_FundingSourceInProgress_ContractInfoEntered_ReturnsInProgress(
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = TaskProgress.InProgress,
            };

            order.AssociatedServicesOnly = true;
            order.ContractFlags.HasSpecificRequirements = true;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ImplementationPlanInProgress_ContractInfoEntered_ReturnsInProgress(
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                ImplementationPlan = TaskProgress.InProgress,
            };

            order.AssociatedServicesOnly = true;
            order.ContractFlags.HasSpecificRequirements = true;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotApplicable)]
        [CommonInlineAutoData(TaskProgress.NotStarted)]
        [CommonInlineAutoData(TaskProgress.Optional)]
        public static void Get_FundingSourceIncomplete_ReturnsCannotStart(
            TaskProgress status,
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = status,
                ImplementationPlan = TaskProgress.Completed,
            };

            order.AssociatedServicesOnly = true;
            order.ContractFlags = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotApplicable)]
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
            order.ContractFlags = null;

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
                FundingSource = TaskProgress.Completed,
                ImplementationPlan = TaskProgress.Completed,
            };

            order.AssociatedServicesOnly = true;
            order.ContractFlags = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(true, null)]
        [CommonInlineAutoData(null, true)]
        public static void Get_PartialContractInfoEntered_ReturnsNotStarted(
            bool? hasSpecificRequirements,
            bool? useDefaultBilling,
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = TaskProgress.Completed,
                ImplementationPlan = TaskProgress.Completed,
            };

            order.AssociatedServicesOnly = true;
            order.ContractFlags.HasSpecificRequirements = hasSpecificRequirements;
            order.ContractFlags.UseDefaultBilling = useDefaultBilling;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ContractInfoEntered_ReturnsCompleted(
            Order order,
            AssociatedServicesBillingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = TaskProgress.Completed,
                ImplementationPlan = TaskProgress.Completed,
            };

            order.AssociatedServicesOnly = true;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}

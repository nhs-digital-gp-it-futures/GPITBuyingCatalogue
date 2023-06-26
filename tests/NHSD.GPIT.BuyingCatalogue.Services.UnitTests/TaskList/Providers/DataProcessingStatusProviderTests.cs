using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class DataProcessingStatusProviderTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            DataProcessingStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            DataProcessingStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_FundingSourceAndAssociatedServiceBillingIncomplete_ReturnsCannotStart(
            Order order,
            DataProcessingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = TaskProgress.InProgress,
                AssociatedServiceBilling = TaskProgress.InProgress,
            };

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
        public static void Get_FundingSourceIncomplete_ContractInfoEntered_ReturnsInProgress(
            TaskProgress status,
            Order order,
            DataProcessingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = status,
            };

            order.ContractFlags.UseDefaultImplementationPlan = true;
            order.ContractFlags.UseDefaultDataProcessing = true;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotStarted)]
        [CommonInlineAutoData(TaskProgress.Optional)]
        public static void Get_AssociatedServiceBillingIncomplete_ContractInfoEntered_ReturnsInProgress(
            TaskProgress status,
            Order order,
            DataProcessingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = TaskProgress.Completed,
                AssociatedServiceBilling = status,
            };

            order.ContractFlags.UseDefaultImplementationPlan = true;
            order.ContractFlags.UseDefaultDataProcessing = true;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ContractInfoNotEntered_ReturnsNotStarted(
            Order order,
            DataProcessingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = TaskProgress.Completed,
                AssociatedServiceBilling = TaskProgress.Completed,
            };

            order.ContractFlags = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(true)]
        public static void Get_ContractInfoEntered_ReturnsCompleted(
            bool useDefaultDataProcessing,
            Order order,
            DataProcessingStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = TaskProgress.Completed,
                ImplementationPlan = TaskProgress.Completed,
                AssociatedServiceBilling = TaskProgress.NotApplicable,
            };

            order.ContractFlags.UseDefaultDataProcessing = useDefaultDataProcessing;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}

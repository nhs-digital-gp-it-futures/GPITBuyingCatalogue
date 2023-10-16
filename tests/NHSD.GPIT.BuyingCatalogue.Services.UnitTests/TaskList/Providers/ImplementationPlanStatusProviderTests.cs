using Fare;
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
    public static class ImplementationPlanStatusProviderTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
            ImplementationPlanStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            ImplementationPlanStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            ImplementationPlanStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_AssociatedServicesOnly_ReturnsNotApplicable(
            Order order,
            ImplementationPlanStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = TaskProgress.Completed,
            };

            order.OrderType = OrderTypeEnum.AssociatedServiceOther;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotApplicable);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotApplicable)]
        [CommonInlineAutoData(TaskProgress.NotStarted)]
        [CommonInlineAutoData(TaskProgress.Optional)]
        public static void Get_FundingSourceNotComplete_NullContract_ReturnsCannotStart(
            TaskProgress status,
            Order order,
            ImplementationPlanStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = status,
            };

            order.Contract = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotApplicable)]
        [CommonInlineAutoData(TaskProgress.NotStarted)]
        [CommonInlineAutoData(TaskProgress.Optional)]
        public static void Get_FundingSourceNotComplete_NullImplementationPlan_ReturnsCannotStart(
            TaskProgress status,
            Order order,
            ImplementationPlanStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = status,
            };

            order.Contract = new Contract() { ImplementationPlan = null };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotApplicable)]
        [CommonInlineAutoData(TaskProgress.NotStarted)]
        [CommonInlineAutoData(TaskProgress.Optional)]
        public static void Get_FundingSourceNotComplete_ContractInfoAlreadyEntered_ReturnsInProgress(
            TaskProgress status,
            Order order,
            ImplementationPlanStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = status,
            };

            order.Contract = new Contract() { ImplementationPlan = new ImplementationPlan() };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.Completed)]
        [CommonInlineAutoData(TaskProgress.Amended)]
        public static void Get_ContractNull_ReturnsNotStarted(
            TaskProgress fundingTaskProgress,
            Order order,
            ImplementationPlanStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = fundingTaskProgress,
            };

            order.Contract = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.Completed)]
        [CommonInlineAutoData(TaskProgress.Amended)]
        public static void Get_ImplementationPlanNull_ReturnsNotStarted(
            TaskProgress fundingTaskProgress,
            Order order,
            ImplementationPlanStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = fundingTaskProgress,
            };

            order.Contract = new Contract() { ImplementationPlan = null };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.Completed)]
        [CommonInlineAutoData(TaskProgress.Amended)]
        public static void Get_ContractInfoEntered_ReturnsCompleted(
            TaskProgress fundingTaskProgress,
            Order order,
            ImplementationPlanStatusProvider service)
        {
            var state = new OrderProgress
            {
                FundingSource = fundingTaskProgress,
            };

            order.Contract = new Contract() { ImplementationPlan = new ImplementationPlan() };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}

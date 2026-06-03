using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class FundingSourceStatusProviderTests
    {
        private static readonly OrderProgress ValidOrderState = new()
        {
            SolutionOrService = TaskProgress.Completed,
        };

        [Theory]
        [MockAutoData]
        public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
            FundingSourceStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            FundingSourceStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            FundingSourceStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockInlineAutoData(TaskProgress.CannotStart)]
        [MockInlineAutoData(TaskProgress.InProgress)]
        [MockInlineAutoData(TaskProgress.NotApplicable)]
        [MockInlineAutoData(TaskProgress.NotStarted)]
        [MockInlineAutoData(TaskProgress.Optional)]
        public static void Get_SolutionOrServiceNotComplete_ReturnsCannotStart(
            TaskProgress status,
            Order order,
            FundingSourceStatusProvider service)
        {
            var state = new OrderProgress
            {
                SolutionOrService = status,
            };

            order.OrderItems.ForEach(x => x.OrderItemFunding = null);

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_NoFundingSourceInfoEntered_ReturnsNotStarted(
            Order order,
            FundingSourceStatusProvider service)
        {
            order.OrderItems.ForEach(x => x.OrderItemFunding = null);

            var actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void Get_SomeFundingSourceInfoEntered_ReturnsInProgress(
            OrderItemFunding funding,
            Order order,
            FundingSourceStatusProvider service)
        {
            order.OrderItems.ForEach(x => x.OrderItemFunding = null);
            order.OrderItems.First().OrderItemFunding = funding;

            var actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockInlineAutoData(1, TaskProgress.Completed)]
        public static void Get_AllFundingSourceInfoEntered_ReturnsCompleted(
            int revision,
            TaskProgress expectedTaskProgress,
            Order order,
            FundingSourceStatusProvider service)
        {
            order.Revision = revision;
            var wrapper = new OrderWrapper(order);

            var actual = service.Get(wrapper, ValidOrderState);

            actual.Should().Be(expectedTaskProgress);
        }

        [Theory]
        [MockInlineAutoData(2, TaskProgress.Amended)]
        public static void Get_AllFundingSourceInfoEntered_ReturnsAmended(
            int revision,
            TaskProgress expectedTaskProgress,
            Order order,
            FundingSourceStatusProvider service)
        {
            var previous = new Order { Revision = 1 };

            order.Revision = revision;
            var wrapper = new OrderWrapper(order, [previous]);

            var actual = service.Get(wrapper, ValidOrderState);

            actual.Should().Be(expectedTaskProgress);
        }
    }
}

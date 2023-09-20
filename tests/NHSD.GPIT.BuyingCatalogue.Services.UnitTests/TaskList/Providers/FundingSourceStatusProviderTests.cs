using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class FundingSourceStatusProviderTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
            FundingSourceStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            FundingSourceStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            FundingSourceStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonInlineAutoData(TaskProgress.CannotStart)]
        [CommonInlineAutoData(TaskProgress.InProgress)]
        [CommonInlineAutoData(TaskProgress.NotApplicable)]
        [CommonInlineAutoData(TaskProgress.NotStarted)]
        [CommonInlineAutoData(TaskProgress.Optional)]
        public static void Get_DeliveryDatesNotComplete_ReturnsCannotStart(
            TaskProgress status,
            Order order,
            FundingSourceStatusProvider service)
        {
            var state = new OrderProgress
            {
                DeliveryDates = status,
            };

            order.OrderItems.ForEach(x => x.OrderItemFunding = null);

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_NoFundingSourceInfoEntered_ReturnsNotStarted(
            Order order,
            FundingSourceStatusProvider service)
        {
            var state = new OrderProgress
            {
                DeliveryDates = TaskProgress.Completed,
            };

            order.OrderItems.ForEach(x => x.OrderItemFunding = null);

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_SomeFundingSourceInfoEntered_ReturnsInProgress(
            OrderItemFunding funding,
            Order order,
            FundingSourceStatusProvider service)
        {
            var state = new OrderProgress
            {
                DeliveryDates = TaskProgress.Completed,
            };

            order.OrderItems.ForEach(x => x.OrderItemFunding = null);
            order.OrderItems.First().OrderItemFunding = funding;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [CommonInlineAutoData(1, TaskProgress.Completed)]
        [CommonInlineAutoData(2, TaskProgress.Amended)]
        public static void Get_AllFundingSourceInfoEntered_ReturnsCompleted(
            int revision,
            TaskProgress expectedTaskProgress,
            Order order,
            FundingSourceStatusProvider service)
        {
            var state = new OrderProgress
            {
                DeliveryDates = TaskProgress.Completed,
            };

            order.Revision = revision;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(expectedTaskProgress);
        }
    }
}

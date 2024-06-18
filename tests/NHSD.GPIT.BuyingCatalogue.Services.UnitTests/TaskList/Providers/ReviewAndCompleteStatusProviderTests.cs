using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public static class ReviewAndCompleteStatusProviderTests
    {
        [Theory]
        [MockAutoData]
        public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
            ReviewAndCompleteStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            ReviewAndCompleteStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            ReviewAndCompleteStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_OrderIsCompleted_ReturnsCompleted(
            Order order,
            ReviewAndCompleteStatusProvider service)
        {
            order.Completed = DateTime.Today;

            var actual = service.Get(new OrderWrapper(order), new OrderProgress());

            actual.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockAutoData]
        public static void Get_DataProcessingCompleted_ReturnsNotStarted(
            Order order,
            ReviewAndCompleteStatusProvider service)
        {
            var state = new OrderProgress
            {
                DataProcessingInformation = TaskProgress.Completed,
            };

            order.Completed = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void Get_DataProcessingIncomplete_ReturnsCannotStart(
            Order order,
            ReviewAndCompleteStatusProvider service)
        {
            var state = new OrderProgress
            {
                DataProcessingInformation = TaskProgress.InProgress,
            };

            order.Completed = null;

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }
    }
}

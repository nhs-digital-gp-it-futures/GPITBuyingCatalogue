using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList.Providers
{
    public class TaskListStatusServiceTest
    {
        [Fact]
        public static void CompletedOrAmended_IsAmendend_ReturnsAmended()
        {
            var actual = TaskListStatusService.CompletedOrAmended(true);
            actual.Should().Be(TaskProgress.Amended);
        }

        [Fact]
        public static void CompletedOrAmended_NotAmendend_ReturnsCompleted()
        {
            var actual = TaskListStatusService.CompletedOrAmended(false);
            actual.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockInlineAutoData(TaskProgress.Completed)]
        [MockInlineAutoData(TaskProgress.Amended)]
        public static void IsTaskCompleted_ReturnsTrue(TaskProgress status)
        {
            var actual = TaskListStatusService.IsTaskCompleted(status);
            actual.Should().Be(true);
        }

        [Theory]
        [MockInlineAutoData(TaskProgress.Optional)]
        [MockInlineAutoData(TaskProgress.NotApplicable)]
        [MockInlineAutoData(TaskProgress.CannotStart)]
        [MockInlineAutoData(TaskProgress.NotStarted)]
        [MockInlineAutoData(TaskProgress.InProgress)]
        public static void IsTaskCompleted_ReturnsFalse(TaskProgress status)
        {
            var actual = TaskListStatusService.IsTaskCompleted(status);
            actual.Should().Be(false);
        }

        [Fact]
        public static void HasAssociatedServices_NullOrder_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => TaskListStatusService.HasAssociatedServices(null));
        }

        [Theory]
        [MockAutoData]
        public static void HasAssociatedServices_NoService_ReturnsFalse(Order order)
        {
            var actual = TaskListStatusService.HasAssociatedServices(order);
            actual.Should().Be(false);
        }

        [Theory]
        [MockAutoData]
        public static void HasAssociatedServices_HasService_ReturnsTrue(Order order)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.Contract = null;
            var actual = TaskListStatusService.HasAssociatedServices(order);
            actual.Should().Be(true);
        }
    }
}

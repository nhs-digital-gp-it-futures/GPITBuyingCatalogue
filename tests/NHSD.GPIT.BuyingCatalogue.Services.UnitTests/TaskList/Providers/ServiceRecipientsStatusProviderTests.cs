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
    public static class ServiceRecipientsStatusProviderTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
            ServiceRecipientsStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            ServiceRecipientsStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_StateIsNull_ReturnsCannotStart(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_CommencementDateStatus_NotCompleted_ReturnsCannotStart(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            var state = new OrderProgress()
            {
                CommencementDateStatus = TaskProgress.InProgress,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_CommencementDateStatus_Completed_No_New_Recipients_ReturnsNotStarted(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            var state = new OrderProgress()
            {
                CommencementDateStatus = TaskProgress.Completed,
            };

            order.OrderRecipients.Clear();

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_CommencementDateStatus_Completed_With_New_Recipients_ReturnsCompleted(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            var state = new OrderProgress()
            {
                CommencementDateStatus = TaskProgress.Completed,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}

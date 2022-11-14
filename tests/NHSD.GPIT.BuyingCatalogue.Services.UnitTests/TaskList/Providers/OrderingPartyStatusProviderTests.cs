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
    public static class OrderingPartyStatusProviderTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            OrderingPartyStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderingPartyContactIsNull_ReturnsNotStarted(
            Order order,
            OrderingPartyStatusProvider service)
        {
            order.OrderingPartyContact = null;

            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_OrderingPartyContactIsNotNull_ReturnsCompleted(
            Contact contact,
            Order order,
            OrderingPartyStatusProvider service)
        {
            order.OrderingPartyContact = contact;

            var actual = service.Get(new OrderWrapper(order), null);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}

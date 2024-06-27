using System.Collections.Generic;
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
    public static class DescriptionStatusProviderTests
    {
        [Theory]
        [MockAutoData]
        public static void Get_OrderWrapperIsNull_ReturnsCannotStart(
            DescriptionStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_OrderIsNull_ReturnsCannotStart(
            DescriptionStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockInlineAutoData("", TaskProgress.NotStarted)]
        [MockInlineAutoData("description", TaskProgress.Completed)]
        public static void Get_WithDescription_ReturnsExpectedResult(
            string description,
            TaskProgress expected,
            Order order,
            DescriptionStatusProvider service)
        {
            order.Revision = 1;
            order.Description = description;

            var actual = service.Get(new OrderWrapper(order), new OrderProgress());

            actual.Should().Be(expected);
        }

        [Theory]
        [MockAutoData]
        public static void Get_AmendedOrder_DescriptionUnchanged_ReturnsExpectedResult(
            List<Order> orders,
            DescriptionStatusProvider service)
        {
            orders.Select((x, i) => (x, i)).ForEach(x => x.x.Revision = x.i + 1);

            var wrapper = new OrderWrapper(orders);

            wrapper.Order.Description = wrapper.Last.Description;

            var actual = service.Get(wrapper, new OrderProgress());

            actual.Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [MockAutoData]
        public static void Get_AmendedOrder_DescriptionChanged_ReturnsExpectedResult(
            List<Order> orders,
            DescriptionStatusProvider service)
        {
            orders.Select((x, i) => (x, i)).ForEach(x => x.x.Revision = x.i + 1);

            var wrapper = new OrderWrapper(orders);

            wrapper.Order.Description = $"{wrapper.Last.Description} changed";

            var actual = service.Get(wrapper, new OrderProgress());

            actual.Should().Be(TaskProgress.Amended);
        }
    }
}

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
    public static class ServiceRecipientsStatusProviderTests
    {
        private static readonly OrderProgress ValidOrderState = new()
        {
            DescriptionStatus = TaskProgress.Completed,
        };

        [Theory]
        [MockAutoData]
        public static void Get_OrderWrapperIsNull_Returns_CannotStart(
            ServiceRecipientsStatusProvider service)
        {
            var actual = service.Get(null, new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_OrderIsNull_Returns_CannotStart(
            ServiceRecipientsStatusProvider service)
        {
            var actual = service.Get(new OrderWrapper(), new OrderProgress());

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void Get_StateIsNull_Returns_CannotStart(
            Order order,
            ServiceRecipientsStatusProvider service)
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
        public static void Get_DescriptionStatus_NotCompleted_Returns_CannotStart(
            TaskProgress descriptionStatus,
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            var state = new OrderProgress()
            {
                DescriptionStatus = descriptionStatus,
            };

            var actual = service.Get(new OrderWrapper(order), state);

            actual.Should().Be(TaskProgress.CannotStart);
        }

        [Theory]
        [MockAutoData]
        public static void No_New_Recipients_Returns_NotStarted(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            order.OrderSublocations.Clear();

            TaskProgress actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [MockAutoData]
        public static void Sublocations_But_No_Recipients_Returns_InProgress(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            order.OrderSublocations.ForEach(x => x.SublocationRecipients = []);

            TaskProgress actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static void SublocationsAndRecipients_But_No_PracticeOrganisation_Returns_InProgress(
            OrderTypeEnum orderType,
            Order order,
            List<OrderSublocationRecipient> sublocationRecipients,
            ServiceRecipientsStatusProvider service)
        {
            order.OrderSublocations.ForEach(x => x.SublocationRecipients = sublocationRecipients);

            order.OrderType = orderType;
            order.AssociatedServicesOnlyDetails = new AssociatedServicesOnlyDetails();

            TaskProgress actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void Complete_And_Incomplete_Sublocations_Returns_InProgress(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            OrderSublocation sublocationWithNoRecipients = order.OrderSublocations.First();

            sublocationWithNoRecipients.SublocationRecipients = [];

            TaskProgress actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.InProgress);
        }

        [Theory]
        [MockAutoData]
        public static void New_Sublocations_And_Recipients_And_IsAmendment_Returns_Amended(
            Order previousOrder,
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            order.OrderNumber = previousOrder.OrderNumber;
            previousOrder.Revision = 1;
            order.Revision = 2;
            previousOrder.OrderSublocations = order.OrderSublocations.Take(2).ToList();

            TaskProgress actual = service.Get(new OrderWrapper(order, [previousOrder]), ValidOrderState);

            actual.Should().Be(TaskProgress.Amended);
        }

        [Theory]
        [MockAutoData]
        public static void New_Sublocations_And_Recipients_Returns_Completed(
            Order order,
            ServiceRecipientsStatusProvider service)
        {
            TaskProgress actual = service.Get(new OrderWrapper(order), ValidOrderState);

            actual.Should().Be(TaskProgress.Completed);
        }
    }
}

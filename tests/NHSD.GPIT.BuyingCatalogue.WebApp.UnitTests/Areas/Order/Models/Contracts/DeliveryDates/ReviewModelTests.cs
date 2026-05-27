using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates
{
    public static class ReviewModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order,
            OrderItem additionalService,
            OrderItem associatedService,
            OrderItem solution,
            DateTime date)
        {
            additionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            associatedService.ParentId = solution.Id;

            order.OrderItems.Clear();
            order.OrderItems.Add(additionalService);
            order.OrderItems.Add(associatedService);
            order.OrderItems.Add(solution);

            order.DeliveryDate = date;
            var model = new ReviewModel(new OrderWrapper(order));

            model.InternalOrgId.Should().Be(order.OrderingParty.InternalIdentifier);
            model.CallOffId.Should().Be(order.CallOffId);
            model.OrderType.Should().Be(order.OrderType);
            model.SolutionName.Should().Be(order.OrderType.GetSolutionNameFromOrder(order));
            model.PracticeReorganisationName.Should().Be(order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Name);
            model.OrderWrapper.Order.Should().Be(order);
            model.DeliveryDate.Should().Be(order.DeliveryDate);
            model.AdditionalServiceIds.Count.Should().Be(1);
            model.AssociatedServiceIds.Count.Should().Be(1);

            model.SolutionId.Should().Be(order.GetSolutionOrderItem()?.Id);
        }

        [Theory]
        [MockAutoData]
        public static void OrderItemSublocationRecipients_ReturnsRecipientsForDate(
            EntityFramework.Ordering.Models.Order order,
            OrderItem solution,
            OrderSublocation sublocation,
            OrderSublocationRecipient recipient,
            OrderItemSublocationRecipient orderItemSublocationRecipient,
            DateTime date)
        {
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            sublocation.SublocationRecipients.Clear();
            recipient.OrderItemSublocationRecipients.Clear();
            order.OrderSublocations.Clear();
            order.OrderItems.Clear();

            orderItemSublocationRecipient.DeliveryDate = date;
            orderItemSublocationRecipient.OrderItem = solution;
            orderItemSublocationRecipient.OrderItemId = solution.Id;

            recipient.OrderItemSublocationRecipients.Add(orderItemSublocationRecipient);
            sublocation.SublocationRecipients.Add(recipient);
            order.OrderSublocations.Add(sublocation);
            order.OrderItems.Add(solution);

            var model = new ReviewModel(new OrderWrapper(order));

            var result = model.OrderItemRecipients(solution.Id, date);
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(List<(string OdsCode, string Name)>));
            result.Count.Should().Be(1);
        }

        [Theory]
        [MockAutoData]
        public static void OrderDates_ReturnsDatesForOrderItem(
            EntityFramework.Ordering.Models.Order order,
            OrderItem solution,
            OrderSublocation sublocation,
            OrderSublocationRecipient recipient,
            OrderItemSublocationRecipient orderItemSublocationRecipient,
            DateTime date)
        {
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            sublocation.SublocationRecipients.Clear();
            recipient.OrderItemSublocationRecipients.Clear();
            order.OrderSublocations.Clear();
            order.OrderItems.Clear();

            orderItemSublocationRecipient.DeliveryDate = date;

            recipient.OrderItemSublocationRecipients.Add(orderItemSublocationRecipient);
            sublocation.SublocationRecipients.Add(recipient);
            order.OrderSublocations.Add(sublocation);
            order.OrderItems.Add(solution);

            var model = new ReviewModel(new OrderWrapper(order));

            var result = model.OrderItemDates(solution.Id);
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(List<DateTime?>));
            result.Count.Should().Be(1);
        }
    }
}

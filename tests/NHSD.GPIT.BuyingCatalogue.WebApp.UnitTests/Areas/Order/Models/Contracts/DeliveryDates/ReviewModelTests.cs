using System;
using System.Collections.Generic;
using System.Linq;
using Bogus.DataSets;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates
{
    public static class ReviewModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order,
            OrderItem additionalService,
            OrderItem associatedService,
            DateTime date)
        {
            additionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            order.OrderItems.Clear();
            order.OrderItems.Add(additionalService);
            order.OrderItems.Add(associatedService);

            order.DeliveryDate = date;
            var model = new ReviewModel(new OrderWrapper(order));

            model.InternalOrgId.Should().Be(order.OrderingParty.InternalIdentifier);
            model.CallOffId.Should().Be(order.CallOffId);
            model.OrderWrapper.Order.Should().Be(order);
            model.DeliveryDate.Should().Be(order.DeliveryDate);
            model.AdditionalServiceIds.Count.Should().Be(1);
            model.AssociatedServiceIds.Count.Should().Be(1);

            model.SolutionId.Should().Be(order.GetSolution()?.CatalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static void OrderItemRecipients_ReturnsRecipientsForDate(
            EntityFramework.Ordering.Models.Order order,
            OrderItem solution,
            OrderRecipient recipient,
            OrderItemRecipient orderItemRecipient,
            DateTime date)
        {
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderItemRecipient.DeliveryDate = date;
            orderItemRecipient.CatalogueItemId = solution.CatalogueItemId;
            recipient.OrderItemRecipients.Clear();
            recipient.OrderItemRecipients.Add(orderItemRecipient);

            order.OrderRecipients.Clear();
            order.OrderRecipients = new List<OrderRecipient>() { recipient };

            order.OrderItems.Clear();
            order.OrderItems.Add(solution);

            var model = new ReviewModel(new OrderWrapper(order));

            var result = model.OrderItemRecipients(solution.CatalogueItemId, date);
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(List<(string OdsCode, string Name)>));
            result.Count.Should().Be(1);
        }

        [Theory]
        [CommonAutoData]
        public static void OrderDates_ReturnsDatesForOrderItem(
            EntityFramework.Ordering.Models.Order order,
            OrderItem solution,
            OrderRecipient recipient,
            OrderItemRecipient orderItemRecipient,
            DateTime date)
        {
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderItemRecipient.DeliveryDate = date;
            recipient.OrderItemRecipients.Clear();
            recipient.OrderItemRecipients.Add(orderItemRecipient);

            order.OrderRecipients.Clear();
            order.OrderRecipients = new List<OrderRecipient>() { recipient };

            order.OrderItems.Clear();
            order.OrderItems.Add(solution);

            var model = new ReviewModel(new OrderWrapper(order));

            var result = model.OrderItemDates(solution.CatalogueItemId);
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(List<DateTime?>));
            result.Count.Should().Be(1);
        }
    }
}

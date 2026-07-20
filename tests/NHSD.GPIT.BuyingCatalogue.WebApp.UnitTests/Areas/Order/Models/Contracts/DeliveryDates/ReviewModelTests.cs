using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
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
            OrderItem associatedServiceForAdditionalService,
            OrderItem solution,
            DateTime date)
        {
            additionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            associatedServiceForAdditionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            associatedService.ParentId = solution.Id;
            associatedService.Parent = solution;

            associatedServiceForAdditionalService.ParentId = additionalService.Id;
            associatedServiceForAdditionalService.Parent = additionalService;

            additionalService.Services.Clear();
            additionalService.Services.Add(associatedServiceForAdditionalService);

            order.OrderItems.Clear();
            order.OrderItems.Add(additionalService);
            order.OrderItems.Add(associatedService);
            order.OrderItems.Add(associatedServiceForAdditionalService);
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
            model.AssociatedServiceIdsForAdditionalServices.Should().BeEquivalentTo(
                new Dictionary<CatalogueItemId, List<int>>
                {
                    { additionalService.CatalogueItemId, new List<int> { associatedServiceForAdditionalService.Id } },
                });

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

            var result = model.OrderItemRecipients(solution.Id);
            result.Should().NotBeNull();
            result.Should().BeOfType<List<(string OdsCode, string Name, DateTime? DeliveryDate)>>();
            result.Count.Should().Be(1);
            result[0].DeliveryDate.Should().Be(date);
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
            orderItemSublocationRecipient.OrderItem = solution;
            orderItemSublocationRecipient.OrderItemId = solution.Id;

            recipient.OrderItemSublocationRecipients.Add(orderItemSublocationRecipient);
            sublocation.SublocationRecipients.Add(recipient);
            order.OrderSublocations.Add(sublocation);
            order.OrderItems.Add(solution);

            var model = new ReviewModel(new OrderWrapper(order));

            var result = model.OrderItemDates(solution.Id);
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(List<DateTime?>));
            result.Count.Should().Be(1);
            result[0].Should().Be(date);
        }

        [Theory]
        [MockAutoData]
        public static void OrderItemSublocationRecipients_ReturnsAllRecipientsWithDeliveryDates(
            EntityFramework.Ordering.Models.Order order,
            OrderItem solution,
            OrderSublocation sublocation,
            OrderSublocationRecipient firstRecipient,
            OrderSublocationRecipient secondRecipient,
            DateTime firstDate,
            DateTime secondDate,
            string firstOds,
            string secondOds)
        {
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            sublocation.SublocationRecipients.Clear();
            order.OrderSublocations.Clear();
            order.OrderItems.Clear();

            var firstOrganisation = "firstOrganisation";
            var secondOrganisation = "secondOrganisation";

            AddRecipientWithDeliveryDate(sublocation, firstRecipient, solution, firstDate, firstOds, firstOrganisation);
            AddRecipientWithDeliveryDate(sublocation, secondRecipient, solution, secondDate, secondOds, secondOrganisation);

            order.OrderSublocations.Add(sublocation);
            order.OrderItems.Add(solution);

            var model = new ReviewModel(new OrderWrapper(order));

            var result = model.OrderItemRecipients(solution.Id);
            result.Should().BeEquivalentTo(
                new List<(string OdsCode, string Name, DateTime? DeliveryDate)>
                {
                    (firstOds, firstOrganisation, firstDate),
                    (secondOds, secondOrganisation, secondDate),
                },
                opt => opt.WithStrictOrdering());
        }

        private static void AddRecipientWithDeliveryDate(
            OrderSublocation sublocation,
            OrderSublocationRecipient recipient,
            OrderItem orderItem,
            DateTime date,
            string odsCode,
            string organisationName)
        {
            recipient.RecipientOdsCode = odsCode;
            recipient.RecipientOdsOrganisation = new OdsOrganisation { Id = odsCode, Name = organisationName };
            recipient.OrderItemSublocationRecipients.Clear();
            recipient.OrderItemSublocationRecipients.Add(
                new OrderItemSublocationRecipient
                {
                    DeliveryDate = date,
                    OrderItem = orderItem,
                    OrderItemId = orderItem.Id,
                    Recipient = recipient,
                    RecipientOdsCode = odsCode,
                });

            sublocation.SublocationRecipients.Add(recipient);
        }
    }
}

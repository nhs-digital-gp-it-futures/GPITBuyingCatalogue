using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Contracts;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Contracts
{
    public class DeliveryDateServiceTests
    {
        [Theory]
        [MockInMemoryDbAutoData]
        public static void CreateDeliveryDate_NullOrderService_ThrowsException(
            [Frozen] BuyingCatalogueDbContext context)
        {
            Assert.Throws<ArgumentNullException>(() => new DeliveryDateService(null, context));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static void CreateDeliveryDate_NullDBContext_ThrowsException(
            [Frozen] IOrderService orderService)
        {
            Assert.Throws<ArgumentNullException>(() => new DeliveryDateService(orderService, null));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SetAllDeliveryDates_UpdatesDatabase(
            Order order,
            DateTime initialDate,
            DateTime deliveryDate,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            DeliveryDateService service)
        {
            order.DeliveryDate = null;
            order.OrderItems.ForEach(x =>
                order.FlattenedRecipients.ForEach(r => r.SetDeliveryDateForItem(x, initialDate)));
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            mockOrderService.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier).Returns(new OrderWrapper(order));

            await service.SetAllDeliveryDates(order.OrderingParty.InternalIdentifier, order.CallOffId, deliveryDate);
            context.ChangeTracker.Clear();

            Order dbOrder = await context.Orders.Include(x => x.OrderItems)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(x => x.OrderItemSublocationRecipients)
                .FirstAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().Be(deliveryDate);
            dbOrder.OrderItems.ForEach(x =>
                order.FlattenedRecipients.ForEach(r =>
                    r.GetDeliveryDateForItem(x.Id).Should().Be(deliveryDate)));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSelectedRecipientsAsync_ReturnsSelectedRecipientsForOrder(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var selectedRecipientId = order.FlattenedRecipients.First().RecipientOdsCode;
            var selectedOrganisationId = order.FlattenedRecipients.First().RecipientOdsOrganisation.Id;

            var result = await service.GetSelectedRecipientsAsync(
                order.CallOffId,
                [selectedOrganisationId]);

            result.Should().ContainSingle()
                .Which.RecipientOdsCode.Should().Be(selectedRecipientId);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSelectedRecipientsAsync_WithDifferentCallOffId_ReturnsNoRecipients(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var selectedOrganisationId = order.FlattenedRecipients.First().RecipientOdsOrganisation.Id;

            var differentCallOffId = new CallOffId(order.CallOffId.OrderNumber + 1, order.CallOffId.Revision);

            var result = await service.GetSelectedRecipientsAsync(
                differentCallOffId,
                [selectedOrganisationId]);

            result.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetRecipientOrderItemsDeliveryDatesAsync_ReturnsRecipientOrderItems(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var recipientOdsCode = order.FlattenedRecipients.First().RecipientOdsCode;
            var expectedIds = order.FlattenedRecipients.First()
                .OrderItemSublocationRecipients
                .Select(x => x.Id)
                .ToList();

            var result = await service.GetRecipientOrderItemsDeliveryDatesAsync(
                order.CallOffId,
                recipientOdsCode);

            result.Select(x => x.Id).Should().BeEquivalentTo(expectedIds);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetMostCommonRecipientDeliveryDateAsync_ReturnsMostCommonDeliveryDate(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            var mostCommonDeliveryDate = DateTime.Today.AddDays(1);
            var otherDeliveryDate = DateTime.Today;
            var recipientOrderItems = order.FlattenedRecipients
                .SelectMany(x => x.OrderItemSublocationRecipients)
                .ToList();
            recipientOrderItems.Should().HaveCountGreaterThan(1);

            recipientOrderItems.ForEach(x => x.DeliveryDate = mostCommonDeliveryDate);
            recipientOrderItems.First().DeliveryDate = otherDeliveryDate;
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var result = await service.GetMostCommonRecipientDeliveryDateAsync(order.CallOffId);

            result.Should().Be(mostCommonDeliveryDate);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetMostCommonRecipientDeliveryDateAsync_WithNoMatchingCallOffId_ReturnsNull(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var differentCallOffId = new CallOffId(order.CallOffId.OrderNumber + 1, order.CallOffId.Revision);

            var result = await service.GetMostCommonRecipientDeliveryDateAsync(differentCallOffId);

            result.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SetRecipientOrderItemDeliveryDatesAsync_WithNullDeliveryDates_ThrowsArgumentNullException(
            DeliveryDateService service)
        {
            var action = async () => await service.SetRecipientOrderItemDeliveryDatesAsync(null);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ResetDeliveryDates_AllRecipientsAffected_UpdatesDatabase(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            order.DeliveryDate = DateTime.Today;
            order.OrderItems.ForEach(x =>
                order.FlattenedRecipients.ForEach(r => r.SetDeliveryDateForItem(x, DateTime.Today)));
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            await service.ResetDeliveryDates(order.Id, DateTime.Today.AddDays(1));
            context.ChangeTracker.Clear();

            Order dbOrder = await context.Orders.Include(x => x.OrderItems)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(x => x.OrderItemSublocationRecipients)
                .FirstAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().BeNull();
            dbOrder.OrderItems.ForEach(x =>
                order.FlattenedRecipients.ForEach(r => r.GetDeliveryDateForItem(x.Id).Should().BeNull()));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ResetDeliveryDates_SomeRecipientsAffected_UpdatesDatabase(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            order.DeliveryDate = DateTime.Today;
            order.OrderItems.ForEach(x =>
                order.FlattenedRecipients.ForEach(r => r.SetDeliveryDateForItem(x, DateTime.Today)));
            var orderItem = order.OrderItems.First();
            order.FlattenedRecipients.ForEach(r => r.SetDeliveryDateForItem(
                orderItem,
                DateTime.Today.AddDays(1)));
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            await service.ResetDeliveryDates(order.Id, DateTime.Today.AddDays(1));
            context.ChangeTracker.Clear();

            Order dbOrder = await context.Orders.Include(x => x.OrderItems)
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(x => x.OrderItemSublocationRecipients)
                .FirstAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().BeNull();
            dbOrder.OrderItems
                .Where(o => o.CatalogueItemId == orderItem.CatalogueItemId)
                .ForEach(x => order.FlattenedRecipients.ForEach(r =>
                    r.GetDeliveryDateForItem(x.Id).Should().Be(DateTime.Today.AddDays(1))));

            dbOrder.OrderItems
                .Where(o => o.CatalogueItemId != orderItem.CatalogueItemId)
                .ForEach(x =>
                    order.FlattenedRecipients.ForEach(r =>
                        r.GetDeliveryDateForItem(x.Id).Should().BeNull()));
        }
    }
}

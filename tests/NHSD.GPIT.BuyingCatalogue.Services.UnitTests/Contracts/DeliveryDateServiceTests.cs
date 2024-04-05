using System;
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
        public static async Task SetDeliveryDate_UpdatesDatabase(
            Order order,
            DateTime deliveryDate,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            DeliveryDateService service)
        {
            order.DeliveryDate = null;
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            mockOrderService.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier).Returns(new OrderWrapper(order));

            await service.SetDeliveryDate(order.OrderingParty.InternalIdentifier, order.CallOffId, deliveryDate);
            context.ChangeTracker.Clear();

            var dbOrder = await context.Orders
                .FirstAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().Be(deliveryDate);
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
            order.OrderItems.ForEach(x => order.OrderRecipients.ForEach(r => r.SetDeliveryDateForItem(x.CatalogueItemId, initialDate)));
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            mockOrderService.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier).Returns(new OrderWrapper(order));

            await service.SetAllDeliveryDates(order.OrderingParty.InternalIdentifier, order.CallOffId, deliveryDate);
            context.ChangeTracker.Clear();

            var dbOrder = await context.Orders
                .Include(x => x.OrderRecipients)
                .ThenInclude(x => x.OrderItemRecipients)
                .FirstAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().Be(deliveryDate);
            dbOrder.OrderItems.ForEach(x => order.OrderRecipients.ForEach(r => r.GetDeliveryDateForItem(x.CatalogueItemId).Should().Be(deliveryDate)));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ResetRecipientDeliveryDates_UpdatesDatabase(
            Order order,
            DateTime initialDate,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            order.DeliveryDate = null;
            order.OrderItems.ForEach(x => order.OrderRecipients.ForEach(r => r.SetDeliveryDateForItem(x.CatalogueItemId, initialDate)));
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            await service.ResetRecipientDeliveryDates(order.Id);
            context.ChangeTracker.Clear();

            var dbOrder = await context.Orders
                .Include(x => x.OrderRecipients)
                .ThenInclude(x => x.OrderItemRecipients)
                .FirstAsync(x => x.Id == order.Id);

            dbOrder.OrderItems.ForEach(x => order.OrderRecipients.ForEach(r => r.GetDeliveryDateForItem(x.CatalogueItemId).Should().Be(null)));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SetDeliveryDates_UpdatesDatabase(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            var initialDate = DateTime.Today;
            var newDate = initialDate.AddDays(1);

            order.DeliveryDate = initialDate;
            order.OrderItems.ForEach(x => order.OrderRecipients.ForEach(r => r.SetDeliveryDateForItem(x.CatalogueItemId, initialDate)));
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var orderItem = order.OrderItems.First();
            var catalogueItemId = orderItem.CatalogueItemId;

            var deliveryDates = order.OrderRecipients
                .Select(x => new RecipientDeliveryDateDto(x.OdsCode, newDate))
                .ToList();

            await service.SetDeliveryDates(order.Id, catalogueItemId, deliveryDates);
            context.ChangeTracker.Clear();

            var dbOrder = await context.Orders
                .Include(x => x.OrderRecipients)
                    .ThenInclude(x => x.OrderItemRecipients)
                .FirstAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().Be(initialDate);

            dbOrder.OrderItems
                .Where(o => o.CatalogueItemId == orderItem.CatalogueItemId)
                .ForEach(x => order.OrderRecipients.ForEach(r => r.GetDeliveryDateForItem(x.CatalogueItemId).Should().Be(newDate)));

            dbOrder.OrderItems
                .Where(o => o.CatalogueItemId != orderItem.CatalogueItemId)
                .ForEach(x => order.OrderRecipients.ForEach(r => r.GetDeliveryDateForItem(x.CatalogueItemId).Should().Be(initialDate)));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ResetDeliveryDates_AllRecipientsAffected_UpdatesDatabase(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            order.DeliveryDate = DateTime.Today;
            order.OrderItems.ForEach(x => order.OrderRecipients.ForEach(r => r.SetDeliveryDateForItem(x.CatalogueItemId, DateTime.Today)));
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            await service.ResetDeliveryDates(order.Id, DateTime.Today.AddDays(1));
            context.ChangeTracker.Clear();

            var dbOrder = await context.Orders
                .Include(x => x.OrderRecipients)
                    .ThenInclude(x => x.OrderItemRecipients)
                .FirstAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().BeNull();
            dbOrder.OrderItems.ForEach(x => order.OrderRecipients.ForEach(r => r.GetDeliveryDateForItem(x.CatalogueItemId).Should().BeNull()));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ResetDeliveryDates_SomeRecipientsAffected_UpdatesDatabase(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            order.DeliveryDate = DateTime.Today;
            order.OrderItems.ForEach(x => order.OrderRecipients.ForEach(r => r.SetDeliveryDateForItem(x.CatalogueItemId, DateTime.Today)));
            var orderItem = order.OrderItems.First();
            order.OrderRecipients.ForEach(r => r.SetDeliveryDateForItem(orderItem.CatalogueItemId, DateTime.Today.AddDays(1)));
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            await service.ResetDeliveryDates(order.Id, DateTime.Today.AddDays(1));
            context.ChangeTracker.Clear();

            var dbOrder = await context.Orders
                .Include(x => x.OrderRecipients)
                    .ThenInclude(x => x.OrderItemRecipients)
                .FirstAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().BeNull();
            dbOrder.OrderItems
                .Where(o => o.CatalogueItemId == orderItem.CatalogueItemId)
                .ForEach(x => order.OrderRecipients.ForEach(r => r.GetDeliveryDateForItem(x.CatalogueItemId).Should().Be(DateTime.Today.AddDays(1))));

            dbOrder.OrderItems
                .Where(o => o.CatalogueItemId != orderItem.CatalogueItemId)
                .ForEach(x => order.OrderRecipients.ForEach(r => r.GetDeliveryDateForItem(x.CatalogueItemId).Should().BeNull()));
        }
    }
}

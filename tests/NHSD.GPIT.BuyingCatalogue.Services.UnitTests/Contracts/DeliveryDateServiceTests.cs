using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Contracts;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Contracts
{
    public class DeliveryDateServiceTests
    {
        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetDeliveryDate_UpdatesDatabase(
            Order order,
            DateTime deliveryDate,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            order.DeliveryDate = null;
            order.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate = null));

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var dbOrder = await context.Orders.SingleAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().BeNull();
            dbOrder.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate.Should().BeNull()));

            await service.SetDeliveryDate(order.OrderingParty.InternalIdentifier, order.CallOffId, deliveryDate);

            dbOrder.DeliveryDate.Should().Be(deliveryDate);
            dbOrder.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate.Should().Be(deliveryDate)));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetDeliveryDates_UpdatesDatabase(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            var initialDate = DateTime.Today;
            var newDate = initialDate.AddDays(1);

            order.DeliveryDate = initialDate;
            order.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate = initialDate));

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var dbOrder = await context.Orders.SingleAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().Be(initialDate);
            dbOrder.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate.Should().Be(initialDate)));

            var orderItem = order.OrderItems.First();
            var catalogueItemId = orderItem.CatalogueItemId;

            var deliveryDates = orderItem.OrderItemRecipients
                .Select(x => new RecipientDeliveryDateDto(x.OdsCode, newDate))
                .ToList();

            await service.SetDeliveryDates(order.Id, catalogueItemId, deliveryDates);

            orderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate.Should().Be(newDate));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task MatchDeliveryDates_WithMatchingRecipients_UpdatesDatabase(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            var initialDate = DateTime.Today;
            var newDate = initialDate.AddDays(1);

            order.DeliveryDate = initialDate;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate = initialDate));

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.OrderItemRecipients.ForEach(x => x.DeliveryDate = newDate);

            foreach (var additionalService in order.GetAdditionalServices())
            {
                foreach (var (x, i) in solution.OrderItemRecipients.Select((x, i) => (x, i)))
                {
                    additionalService.OrderItemRecipients.ElementAt(i).OdsCode = x.OdsCode;
                }
            }

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var serviceToTest = order.GetAdditionalServices().First();
            var orderItem = await context.OrderItems.SingleAsync(x => x.OrderId == order.Id && x.CatalogueItemId == serviceToTest.CatalogueItemId);

            orderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate.Should().Be(initialDate));

            await service.MatchDeliveryDates(order.Id, solution.CatalogueItemId, serviceToTest.CatalogueItemId);

            orderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate.Should().Be(newDate));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task MatchDeliveryDates_NoMatchingRecipients_NoChangesMade(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            DeliveryDateService service)
        {
            var initialDate = DateTime.Today;
            var newDate = initialDate.AddDays(1);

            order.DeliveryDate = initialDate;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate = initialDate));

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.OrderItemRecipients.ForEach(x => x.DeliveryDate = newDate);

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var serviceToTest = order.GetAdditionalServices().First();
            var orderItem = await context.OrderItems.SingleAsync(x => x.OrderId == order.Id && x.CatalogueItemId == serviceToTest.CatalogueItemId);

            orderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate.Should().Be(initialDate));

            await service.MatchDeliveryDates(order.Id, solution.CatalogueItemId, serviceToTest.CatalogueItemId);

            orderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate.Should().Be(initialDate));
        }
    }
}

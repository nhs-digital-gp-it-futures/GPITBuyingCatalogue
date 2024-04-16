using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using Azure.Storage.Queues;
using BuyingCatalogueFunction.Notifications.ContractExpiry.Services;
using BuyingCatalogueFunction.Notifications.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace BuyingCatalogueFunctionTests.Notifications.ContractExpiry.Services
{
    public static class ContractExpiryServiceTests
    {
        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DoesntReturnTerminatedOrders(
            ContractExpiryService service,
            [Frozen] BuyingCatalogueDbContext dbContext,
            Order order)
        {
            Initialise(order);
            order.IsTerminated = true;
            Save(dbContext, [order]);

            var orders = await service.GetOrdersNearingExpiry(DateTime.UtcNow.Date);

            orders.Count.Should().Be(0);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DoesntReturnDeletedOrders(
            ContractExpiryService service,
            [Frozen] BuyingCatalogueDbContext dbContext,
            Order order)
        {
            Initialise(order);
            order.IsDeleted = true;
            Save(dbContext, [order]);

            var orders = await service.GetOrdersNearingExpiry(DateTime.UtcNow.Date);

            orders.Count.Should().Be(0);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DoesntReturnOrdersThatArentCompleted(
            ContractExpiryService service,
            [Frozen] BuyingCatalogueDbContext dbContext,
            Order order)
        {
            Initialise(order);
            order.Completed = null;
            Save(dbContext, [order]);

            var orders = await service.GetOrdersNearingExpiry(DateTime.UtcNow.Date);

            orders.Count.Should().Be(0);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DoesntReturnOrdersThatHaveBothEvents(
            ContractExpiryService service,
            [Frozen] BuyingCatalogueDbContext dbContext,
            Order order)
        {
            Initialise(order);
            order.ContractOrderNumber.Id = order.OrderNumber;
            order.ContractOrderNumber.OrderEvents.Add(new OrderEvent() { EventTypeId = (int)EventTypeEnum.OrderEnteredFirstExpiryThreshold });
            order.ContractOrderNumber.OrderEvents.Add(new OrderEvent() { EventTypeId = (int)EventTypeEnum.OrderEnteredSecondExpiryThreshold });
            Save(dbContext, [order]);

            var orders = await service.GetOrdersNearingExpiry(DateTime.UtcNow.Date);

            orders.Count.Should().Be(0);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DoesntReturnOrdersThatHaveBothEvents_MultipleRevisions(
            ContractExpiryService service,
            [Frozen] BuyingCatalogueDbContext dbContext,
            Order order)
        {
            Initialise(order);
            order.Revision = 1;
            order.ContractOrderNumber.Id = order.OrderNumber;
            var amendment = order.BuildAmendment(2);
            amendment.Completed = DateTime.UtcNow.Date;
            amendment.ContractOrderNumber = order.ContractOrderNumber;
            amendment.ContractOrderNumber.OrderEvents.Add(new OrderEvent() { EventTypeId = (int)EventTypeEnum.OrderEnteredFirstExpiryThreshold });
            amendment.ContractOrderNumber.OrderEvents.Add(new OrderEvent() { EventTypeId = (int)EventTypeEnum.OrderEnteredSecondExpiryThreshold });
            Save(dbContext, [order, amendment]);

            var orders = await service.GetOrdersNearingExpiry(DateTime.UtcNow.Date);

            orders.Count.Should().Be(0);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ReturnsOrder(
            ContractExpiryService service,
            [Frozen] BuyingCatalogueDbContext dbContext,
            Order order)
        {
            Initialise(order);
            Save(dbContext, [order]);

            var orders = await service.GetOrdersNearingExpiry(DateTime.UtcNow.Date);

            orders.Count.Should().Be(1);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ReturnsOrder_MultipleRevisions(
            ContractExpiryService service,
            [Frozen] BuyingCatalogueDbContext dbContext,
            Order order)
        {
            Initialise(order);
            order.Revision = 1;
            order.ContractOrderNumber.Id = order.OrderNumber;
            var amendment = order.BuildAmendment(2);
            amendment.Completed = DateTime.UtcNow.Date;
            amendment.ContractOrderNumber = order.ContractOrderNumber;
            Save(dbContext, [order, amendment]);

            var orders = await service.GetOrdersNearingExpiry(DateTime.UtcNow.Date);

            orders.Count.Should().Be(1);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task RaiseExpiry_Adds_OrderEvent(
            DateTime date,
            EventTypeEnum eventType,
            EmailPreferenceType emailPreferenceType,
            Order order,
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] IOptions<QueueOptions> options,
            IFixture fixture)
        {
            options.Value.Returns(new QueueOptions());
            var service = fixture.Create<ContractExpiryService>();

            order.ContractOrderNumber.Id = order.OrderNumber;
            order.ContractOrderNumber.OrderEvents.Clear();
            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();

            await service.RaiseExpiry(date, order, eventType, emailPreferenceType);

            var orderNumber = dbContext.OrderNumbers
                .Include(o => o.OrderEvents)
                .First(o => o.Id == order.OrderNumber);

            orderNumber.OrderEvents.Count().Should().Be(1);
            orderNumber.OrderEvents.First().EventTypeId.Should().Be((int)eventType);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task RaiseExpiry_Throws(
            DateTime date,
            EventTypeEnum eventType,
            EmailPreferenceType emailPreferenceType,
            AspNetUser user,
            Order order,
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] IOptions<QueueOptions> options,
            [Frozen] QueueServiceClient queueServiceClient,
            [Frozen] QueueClient queueClient,
            [Frozen] IEmailPreferenceService emailPreferenceService,
            IFixture fixture)
        {
            options.Value.Returns(new QueueOptions());
            queueClient.SendMessageAsync(Arg.Any<string>()).ThrowsAsync(new Exception());
            queueServiceClient.GetQueueClient(Arg.Any<string>()).Returns(queueClient);
            emailPreferenceService.ShouldTriggerForUser(emailPreferenceType, user.Id).Returns(true);

            var service = fixture.Create<ContractExpiryService>();

            order.OrderingPartyId = order.OrderingParty.Id;
            order.ContractOrderNumber.Id = order.OrderNumber;
            order.ContractOrderNumber.OrderEvents.Clear();
            user.PrimaryOrganisationId = order.OrderingPartyId;
            user.Disabled = false;
            dbContext.Users.Add(user);
            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();

            await Assert.ThrowsAsync<Exception>(() => service.RaiseExpiry(date, order, eventType, emailPreferenceType));
        }

        private static void Save(BuyingCatalogueDbContext dbContext, Order[] orders)
        {
            dbContext.Orders.AddRange(orders);
            dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();
        }

        private static void Initialise(Order order)
        {
            order.IsTerminated = false;
            order.IsDeleted = false;
            order.Completed = DateTime.UtcNow.Date;
            order.MaximumTerm = 32;
            order.CommencementDate = DateTime.UtcNow.Date;
            order.OrderEvents.Clear();
        }
    }
}

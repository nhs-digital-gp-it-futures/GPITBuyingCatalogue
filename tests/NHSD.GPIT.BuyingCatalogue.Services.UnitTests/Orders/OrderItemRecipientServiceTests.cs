using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderItemRecipientServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderItemRecipientService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void AddOrderItemRecipients_NullRecipients_ThrowsException(
            int orderId,
            CatalogueItemId catalogueItemId,
            OrderItemRecipientService service)
        {
            FluentActions
                .Awaiting(() => service.AddOrderItemRecipients(orderId, catalogueItemId, null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddOrderItemRecipients_AddsServiceRecipientsToDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            int orderId,
            CatalogueItemId catalogueItemId,
            List<ServiceRecipientDto> serviceRecipients,
            [Frozen] Mock<IServiceRecipientService> mockServiceRecipientService,
            OrderItemRecipientService service)
        {
            foreach (var recipient in serviceRecipients)
            {
                context.ServiceRecipients.Add(new ServiceRecipient { OdsCode = recipient.OdsCode });
            }

            await context.SaveChangesAsync();

            mockServiceRecipientService
                .Setup(x => x.AddServiceRecipient(It.IsAny<ServiceRecipientDto>()))
                .Returns(Task.CompletedTask);

            await service.AddOrderItemRecipients(orderId, catalogueItemId, serviceRecipients);

            mockServiceRecipientService.VerifyAll();

            foreach (var recipient in serviceRecipients)
            {
                var actual = context.OrderItemRecipients.FirstOrDefault(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId
                    && x.OdsCode == recipient.OdsCode);

                actual.Should().NotBeNull();
                actual!.Quantity.Should().Be(null);
            }
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void UpdateOrderItemRecipients_NullRecipients_ThrowsException(
            int orderId,
            CatalogueItemId catalogueItemId,
            OrderItemRecipientService service)
        {
            FluentActions
                .Awaiting(() => service.UpdateOrderItemRecipients(orderId, catalogueItemId, null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateOrderItemRecipients_UpdatesServiceRecipientsInDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            int orderId,
            CatalogueItemId catalogueItemId,
            string odsCode,
            List<ServiceRecipientDto> serviceRecipients,
            [Frozen] Mock<IServiceRecipientService> mockServiceRecipientService,
            OrderItemRecipientService service)
        {
            foreach (var recipient in serviceRecipients)
            {
                context.ServiceRecipients.Add(new ServiceRecipient { OdsCode = recipient.OdsCode });
            }

            context.ServiceRecipients.Add(new ServiceRecipient { OdsCode = odsCode });

            context.OrderItemRecipients.Add(new OrderItemRecipient
            {
                OrderId = orderId,
                CatalogueItemId = catalogueItemId,
                OdsCode = serviceRecipients.First().OdsCode,
            });

            context.OrderItemRecipients.Add(new OrderItemRecipient
            {
                OrderId = orderId,
                CatalogueItemId = catalogueItemId,
                OdsCode = odsCode,
            });

            await context.SaveChangesAsync();

            mockServiceRecipientService
                .Setup(x => x.AddServiceRecipient(It.IsAny<ServiceRecipientDto>()))
                .Returns(Task.CompletedTask);

            await service.UpdateOrderItemRecipients(orderId, catalogueItemId, serviceRecipients);

            mockServiceRecipientService.VerifyAll();

            var recipients = context.OrderItemRecipients
                .Where(x => x.OrderId == orderId && x.CatalogueItemId == catalogueItemId)
                .ToList();

            recipients.Count.Should().Be(serviceRecipients.Count);

            foreach (var recipient in serviceRecipients)
            {
                recipients.Should().ContainSingle(x => x.OdsCode == recipient.OdsCode);
            }
        }
    }
}

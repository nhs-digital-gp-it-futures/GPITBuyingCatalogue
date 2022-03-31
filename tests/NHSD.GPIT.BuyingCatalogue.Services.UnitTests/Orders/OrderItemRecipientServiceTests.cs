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
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
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
        public static void AddServiceRecipient_NullRecipients_ThrowsException(
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
        public static async Task AddServiceRecipient_AddsServiceRecipientsToDatabase(
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
                actual!.Quantity.Should().Be(1);
            }
        }
    }
}

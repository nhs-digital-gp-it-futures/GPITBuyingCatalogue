using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderRecipientServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderRecipientService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetOrderRecipients_ReturnsExpectedResults(
           Order order,
           string odsCode,
           [Frozen] BuyingCatalogueDbContext context,
           [Frozen] Mock<IOrderService> mockOrderService,
           OrderRecipientService service)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            mockOrderService
                .Setup(s => s.GetOrderWithOrderItems(It.IsAny<CallOffId>(), It.IsAny<string>()))
                .ReturnsAsync(new OrderWrapper(order));

            await service.SetOrderRecipients(order.OrderingParty.InternalIdentifier, order.CallOffId, new[] { odsCode });
            context.ChangeTracker.Clear();

            var dbOrder = await context.Orders
                .Include(o => o.OrderRecipients)
                .Where(o => o.Id == order.Id)
                .FirstOrDefaultAsync();

            dbOrder.OrderRecipients.Count.Should().Be(1);
            dbOrder.OrderRecipients.Exists(odsCode);
        }
    }
}

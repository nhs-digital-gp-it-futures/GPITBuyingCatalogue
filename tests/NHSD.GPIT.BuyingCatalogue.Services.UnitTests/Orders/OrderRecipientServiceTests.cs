using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderRecipientServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderRecipientService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [MockInMemoryDbInlineAutoData(OrderTypeEnum.Solution)]
        public static async Task SetOrderRecipients_WhenOrderDeliveryDate_Null_AddsRecipient(
           OrderTypeEnum orderType,
           Order order,
           string odsCode,
           [Frozen] BuyingCatalogueDbContext context,
           [Frozen] IOrderService mockOrderService,
           OrderRecipientService service)
        {
            order.DeliveryDate = null;
            order.OrderType = orderType;
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            mockOrderService.GetOrderWithOrderItems(Arg.Any<CallOffId>(), Arg.Any<string>()).Returns(new OrderWrapper(order));

            await service.SetOrderRecipients(order.OrderingParty.InternalIdentifier, order.CallOffId, new[] { odsCode });
            context.ChangeTracker.Clear();

            var dbOrder = await context.Orders
                .Include(o => o.OrderRecipients)
                    .ThenInclude(x => x.OrderItemRecipients)
                .Where(o => o.Id == order.Id)
                .FirstOrDefaultAsync();

            dbOrder.OrderRecipients.Count.Should().Be(1);
            var recipient = dbOrder.OrderRecipients.Single();
            recipient.OdsCode.Should().Be(odsCode);
            recipient.OrderItemRecipients.Should().BeEmpty();
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task SetOrderRecipients_WhenOrderMergerSplit_AddsRecipientWithQuantity1(
           OrderTypeEnum orderType,
           Order order,
           string odsCode,
           [Frozen] BuyingCatalogueDbContext context,
           [Frozen] IOrderService mockOrderService,
           OrderRecipientService service)
        {
            order.DeliveryDate = null;
            order.OrderType = orderType;
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            mockOrderService.GetOrderWithOrderItems(Arg.Any<CallOffId>(), Arg.Any<string>()).Returns(new OrderWrapper(order));

            await service.SetOrderRecipients(order.OrderingParty.InternalIdentifier, order.CallOffId, new[] { odsCode });
            context.ChangeTracker.Clear();

            var dbOrder = await context.Orders
                .Include(o => o.OrderRecipients)
                    .ThenInclude(x => x.OrderItemRecipients)
                .Where(o => o.Id == order.Id)
                .FirstOrDefaultAsync();

            dbOrder.OrderRecipients.Count.Should().Be(1);
            var recipient = dbOrder.OrderRecipients.Single();
            recipient.OdsCode.Should().Be(odsCode);
            recipient.OrderItemRecipients.Should().NotBeEmpty();
            recipient.OrderItemRecipients.All(oir => oir.Quantity == 1)
                .Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceOther, null)]
        [MockInMemoryDbInlineAutoData(OrderTypeEnum.Solution, null)]
        [MockInMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, 1)]
        [MockInMemoryDbInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, 1)]
        public static async Task SetOrderRecipients_WhenOrderHasDelvieryDate_AddsRecipientWithDates(
           OrderTypeEnum orderType,
           int? expectedQty,
           Order order,
           string odsCode,
           [Frozen] BuyingCatalogueDbContext context,
           [Frozen] IOrderService mockOrderService,
           OrderRecipientService service)
        {
            order.OrderType = orderType;

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            mockOrderService.GetOrderWithOrderItems(Arg.Any<CallOffId>(), Arg.Any<string>()).Returns(new OrderWrapper(order));

            await service.SetOrderRecipients(order.OrderingParty.InternalIdentifier, order.CallOffId, new[] { odsCode });
            context.ChangeTracker.Clear();

            var dbOrder = await context.Orders
                .Include(o => o.OrderRecipients)
                    .ThenInclude(x => x.OrderItemRecipients)
                .Where(o => o.Id == order.Id)
                .FirstOrDefaultAsync();

            dbOrder.OrderRecipients.Count.Should().Be(1);
            var recipient = dbOrder.OrderRecipients.Single();
            recipient.OdsCode.Should().Be(odsCode);
            recipient.OrderItemRecipients.Should().NotBeEmpty();
            recipient.OrderItemRecipients.All(oir => oir.DeliveryDate == order.DeliveryDate)
                .Should().BeTrue();
            recipient.OrderItemRecipients.All(oir => oir.Quantity == expectedQty)
                .Should().BeTrue();
        }
    }
}

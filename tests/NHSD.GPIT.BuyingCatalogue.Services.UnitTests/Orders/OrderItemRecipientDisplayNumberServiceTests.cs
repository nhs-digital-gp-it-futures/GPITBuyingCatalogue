using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders;

public static class OrderItemRecipientDisplayNumberServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);

        assertion.Verify(typeof(OrderItemRecipientDisplayNumberService).GetConstructors());
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetOrderItemRecipientDisplayNumbers_NoPreviousRevision_AssignsSequentialNumbersByItemType(
        string internalOrgId,
        Order order,
        OrderSublocation sublocation,
        CatalogueItem associatedService,
        CatalogueItem solution,
        CatalogueItem additionalService,
        [Frozen] BuyingCatalogueDbContext context,
        OrderItemRecipientDisplayNumberService service)
    {
        UpdateOrder(order, 1, 100001, internalOrgId, 1);
        AddItemRecipients(order, sublocation, associatedService, CatalogueItemType.AssociatedService, 2);
        AddItemRecipients(order, sublocation, solution, CatalogueItemType.Solution, 2);
        AddItemRecipients(order, sublocation, additionalService, CatalogueItemType.AdditionalService, 2);
        context.Add(order);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetOrderItemRecipientDisplayNumbers(order.CallOffId, internalOrgId);

        var recipients = await context.OrderItemSublocationRecipients
            .Include(x => x.OrderItem)
            .ThenInclude(x => x.CatalogueItem)
            .Where(x => x.OrderId == order.Id)
            .ToListAsync();

        recipients.Select(x => x.DisplayNumber).Should().BeEquivalentTo([1, 2, 3, 4, 5, 6]);
        recipients.Where(x => x.OrderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution)
            .Select(x => x.DisplayNumber).Should().BeEquivalentTo([1, 2]);
        recipients.Where(x => x.OrderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService)
            .Select(x => x.DisplayNumber).Should().BeEquivalentTo([3, 4]);
        recipients.Where(x => x.OrderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService)
            .Select(x => x.DisplayNumber).Should().BeEquivalentTo([5, 6]);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetOrderItemRecipientDisplayNumbers_PreviousRevision_StartsAfterPreviousMaximum(
        string internalOrgId,
        Order previousOrder,
        Order currentOrder,
        OrderSublocation previousSublocation,
        OrderSublocation currentSublocation,
        CatalogueItem catalogueItem,
        [Frozen] BuyingCatalogueDbContext context,
        OrderItemRecipientDisplayNumberService service)
    {
        UpdateOrder(previousOrder, 1, 100002, internalOrgId, 1);
        AddItemRecipients(previousOrder, previousSublocation, catalogueItem, CatalogueItemType.Solution, 1, 7);
        UpdateOrder(currentOrder, 2, 100002, internalOrgId, 2);
        AddItemRecipients(currentOrder, currentSublocation, catalogueItem, CatalogueItemType.Solution, 2);
        context.AddRange(previousOrder, currentOrder);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetOrderItemRecipientDisplayNumbers(currentOrder.CallOffId, internalOrgId);

        var displayNumbers = await context.OrderItemSublocationRecipients
            .Where(x => x.OrderId == currentOrder.Id)
            .OrderBy(x => x.DisplayNumber)
            .Select(x => x.DisplayNumber)
            .ToListAsync();
        displayNumbers.Should().BeEquivalentTo([8, 9]);
    }

    [Theory]
    [MockInMemoryDbInlineAutoData(null, true)]
    [MockInMemoryDbInlineAutoData(1, false)]
    public static async Task HasItemRecipientCountDiscrepancy_ReturnsExpectedResult(
        int? displayNumber,
        bool expected,
        string internalOrgId,
        Order order,
        OrderSublocation sublocation,
        CatalogueItem catalogueItem,
        [Frozen] BuyingCatalogueDbContext context,
        OrderItemRecipientDisplayNumberService service)
    {
        UpdateOrder(order, 1, 100003, internalOrgId, 1);
        AddItemRecipients(order, sublocation, catalogueItem, CatalogueItemType.Solution, 1, displayNumber);
        context.Add(order);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.HasItemRecipientCountDiscrepancy(order.CallOffId, internalOrgId);

        result.Should().Be(expected);
    }

    private static void UpdateOrder(Order order, int id, int orderNumber, string internalOrgId, int revision)
    {
        order.Id = id;
        order.OrderNumber = orderNumber;
        order.Revision = revision;
        order.OrderingParty.InternalIdentifier = internalOrgId;
        order.ContractOrderNumber = null;
        order.OrderItems.Clear();
        order.OrderSublocations.Clear();
    }

    private static void AddItemRecipients(
        Order order,
        OrderSublocation sublocation,
        CatalogueItem catalogueItem,
        CatalogueItemType itemType,
        int count,
        int? displayNumber = null)
    {
        ConfigureSublocation(order, sublocation);

        var firstSequence = order.OrderItems.Count + 1;
        var itemRecipients = Enumerable.Range(firstSequence, count)
            .Select(sequence => BuildItemRecipient(order, sublocation, catalogueItem, itemType, sequence, displayNumber))
            .ToList();

        order.OrderItems = order.OrderItems
            .Concat(itemRecipients.Select(x => x.OrderItem))
            .ToHashSet();
        sublocation.SublocationRecipients = sublocation.SublocationRecipients
            .Concat(itemRecipients.Select(x => x.Recipient))
            .ToList();
    }

    private static void ConfigureSublocation(Order order, OrderSublocation sublocation)
    {
        if (order.OrderSublocations.Contains(sublocation))
        {
            return;
        }

        sublocation.OrderId = order.Id;
        sublocation.Order = order;
        sublocation.SublocationRecipients.Clear();
        order.OrderSublocations.Add(sublocation);
    }

    private static (OrderItem OrderItem, OrderSublocationRecipient Recipient) BuildItemRecipient(
        Order order,
        OrderSublocation sublocation,
        CatalogueItem catalogueItem,
        CatalogueItemType itemType,
        int sequence,
        int? displayNumber)
    {
        var itemId = (order.Id * 100) + sequence;
        var recipientOdsCode = $"R{itemId}";
        var orderItem = BuildOrderItem(order, itemId, catalogueItem, itemType);
        var recipient = BuildRecipient(order, orderItem, sublocation.SublocationOdsCode, recipientOdsCode, displayNumber);
        recipient.ParentSublocation = sublocation;

        return (orderItem, recipient);
    }

    private static OrderItem BuildOrderItem(
        Order order,
        int itemId,
        CatalogueItem catalogueItem,
        CatalogueItemType itemType)
    {
        catalogueItem.CatalogueItemType = itemType;
        return new OrderItem(catalogueItem.Id)
        {
            Id = itemId,
            OrderId = order.Id,
            Order = order,
            CatalogueItem = catalogueItem,
        };
    }

    private static OrderSublocationRecipient BuildRecipient(
        Order order,
        OrderItem orderItem,
        string parentOdsCode,
        string recipientOdsCode,
        int? displayNumber)
    {
        var recipient = new OrderSublocationRecipient
        {
            OrderId = order.Id,
            ParentSublocationOdsCode = parentOdsCode,
            RecipientOdsCode = recipientOdsCode,
            Order = order,
        };

        recipient.OrderItemSublocationRecipients.Add(new OrderItemSublocationRecipient
        {
            OrderId = order.Id,
            ParentSublocationOdsCode = parentOdsCode,
            RecipientOdsCode = recipientOdsCode,
            OrderItemId = orderItem.Id,
            OrderItem = orderItem,
            Recipient = recipient,
            DisplayNumber = displayNumber,
        });

        return recipient;
    }
}

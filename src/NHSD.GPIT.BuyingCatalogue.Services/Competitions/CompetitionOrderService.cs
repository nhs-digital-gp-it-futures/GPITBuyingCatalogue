using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Competitions;

public class CompetitionOrderService : ICompetitionOrderService
{
    private readonly BuyingCatalogueDbContext dbContext;

    public CompetitionOrderService(
        BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<CallOffId> CreateDirectAwardOrder(string internalOrgId, int competitionId, CatalogueItemId solutionId)
    {
        Competition competition = await dbContext.Competitions
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.CatalogueItem)
            .ThenInclude(x => x.Supplier)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Services)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .Include(x => x.CompetitionSublocations)
            .ThenInclude(y => y.SublocationRecipients)
            .ThenInclude(z => z.RecipientOrganisation)
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        if (competition?.Completed is null)
        {
            throw new ArgumentException(
                @"Competition either does not exist or is not yet completed",
                nameof(competitionId));
        }

        var directAwardSolution = competition.CompetitionSolutions.FirstOrDefault(x => x.CatalogueItemId == solutionId);

        if (directAwardSolution is null)
        {
            throw new ArgumentException(
                @"Solution does not exist on competition",
                nameof(solutionId));
        }

        var orderItems = CreateDirectAwardOrderItems(directAwardSolution);
        var nextOrderNumber = await dbContext.NextOrderNumber();

        var order = CreateOrder(nextOrderNumber, competition, directAwardSolution, orderItems);

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        return order.CallOffId;
    }

    public async Task<CallOffId> CreateOrder(string internalOrgId, int competitionId, CatalogueItemId solutionId)
    {
        Competition competition = await dbContext.Competitions
            .Include(x => x.CompetitionSublocations)
            .ThenInclude(y => y.SublocationRecipients)
            .ThenInclude(z => z.RecipientOrganisation)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.CatalogueItem)
            .ThenInclude(x => x.Supplier)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Quantities)
            .ThenInclude(x => x.Recipient)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Services)
            .ThenInclude(x => x.Quantities)
            .ThenInclude(x => x.Recipient)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Services)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        if (competition?.Completed is null)
        {
            throw new ArgumentException(
                @"Competition either does not exist or is not yet completed",
                nameof(competitionId));
        }

        var winningSolution = competition.CompetitionSolutions.FirstOrDefault(x => x.CatalogueItemId == solutionId);
        if (winningSolution is null || !winningSolution.IsWinningSolution)
        {
            throw new ArgumentException(
                @"Solution either does not exist or is not a winning Solution",
                nameof(solutionId));
        }

        var orderItems = CreateOrderItems(winningSolution).ToList();
        var nextOrderNumber = await dbContext.NextOrderNumber();

        var order = CreateOrder(nextOrderNumber, competition, winningSolution, orderItems);
        dbContext.Orders.Add(order);

        AssignRecipientQuantities(order, winningSolution, orderItems);

        await dbContext.SaveChangesAsync();

        return order.CallOffId;
    }

    private static IEnumerable<OrderItem> CreateDirectAwardOrderItems(CompetitionSolution directAwardSolution)
    {
        var solutionOrderItem = new OrderItem(directAwardSolution.CatalogueItemId) { Created = DateTime.UtcNow };

        var services = directAwardSolution.Services
            .Select(x => CreateOrderItem(x.CatalogueItemId, x.Price, x.Price.Tiers, solutionOrderItem))
            .ToList();
        var orderItems = new List<OrderItem>() { solutionOrderItem };

        orderItems.AddRange(services);

        return orderItems;
    }

    private static IEnumerable<OrderItem> CreateOrderItems(CompetitionSolution winningSolution)
    {
        var winningSolutionOrderItem = CreateOrderItem(
            winningSolution.CatalogueItemId,
            winningSolution.Price,
            winningSolution.Price.Tiers,
            null);
        var services = winningSolution.Services
            .Select(x => CreateOrderItem(x.CatalogueItemId, x.Price, x.Price.Tiers, winningSolutionOrderItem))
            .ToList();

        var orderItems = new List<OrderItem>() { winningSolutionOrderItem };
        orderItems.AddRange(services);

        return orderItems;
    }

    private static OrderItem CreateOrderItem(
        CatalogueItemId catalogueItemId,
        IPrice price,
        IEnumerable<IOrderablePriceTier> priceTiers,
        OrderItem parent)
        => new(catalogueItemId)
        {
            Created = DateTime.UtcNow,
            OrderItemPrice = new OrderItemPrice(price)
            {
                OrderItemPriceTiers = priceTiers.Select(
                        y => new OrderItemPriceTier(y))
                    .ToList(),
            },
            Parent = parent,
        };

    private static Order CreateOrder(
        int orderNumber,
        Competition competition,
        CompetitionSolution competitionSolution,
        IEnumerable<OrderItem> orderItems)
    {
        return new Order
        {
            OrderNumber = orderNumber,
            OrderType = OrderTypeEnum.Solution,
            Revision = 1,
            Description = $"Order created from competition: {competition.Id}",
            Created = DateTime.UtcNow,
            MaximumTerm = competition.ContractLength,
            OrderSublocations = competition.CompetitionSublocations.Select(x => new OrderSublocation(x)).ToList(),
            OrderItems = orderItems.ToList(),
            OrderingPartyId = competition.OrganisationId,
            SupplierId = competitionSolution.CatalogueItem.SupplierId,
            CompetitionId = competition.Id,
            SelectedFrameworkId = competition.FrameworkId,
        };
    }

    private static void AssignRecipientQuantities(Order order, CompetitionSolution winningSolution, List<OrderItem> orderItems)
    {
        var competitionItemQuantities = winningSolution.Quantities
            .Select(x => new { ItemId = winningSolution.CatalogueItemId, x.Quantity, OdsCode = x.RecipientOdsCode })
            .Concat(
                winningSolution.Services.SelectMany(
                    x => x.Quantities.Select(
                        y => new
                        {
                            ItemId = x.CatalogueItemId, y.Quantity, OdsCode = y.RecipientOdsCode,
                        })));

        foreach (var itemQuantity in competitionItemQuantities)
        {
            OrderSublocationRecipient orderRecipient =
                order.FlattenedRecipients.FirstOrDefault(x => x.RecipientOdsCode == itemQuantity.OdsCode);

            var orderItem = orderItems.FirstOrDefault(item => item.CatalogueItemId == itemQuantity.ItemId);
            orderRecipient?.SetQuantityForItem(orderItem, itemQuantity.Quantity);
        }
    }
}

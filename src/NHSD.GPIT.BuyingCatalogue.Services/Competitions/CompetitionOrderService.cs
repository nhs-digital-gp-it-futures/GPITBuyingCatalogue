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

    public async Task<CallOffId> CreateOrder(string internalOrgId, int competitionId, CatalogueItemId solutionId)
    {
        var competition = await dbContext.Competitions.Include(x => x.Recipients)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Solution)
            .ThenInclude(x => x.CatalogueItem)
            .ThenInclude(x => x.Supplier)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Quantities)
            .ThenInclude(x => x.CompetitionRecipient)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Quantities)
            .ThenInclude(x => x.CompetitionRecipient)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
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

        var winningSolution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);
        if (winningSolution is null || !winningSolution.IsWinningSolution)
        {
            throw new ArgumentException(
                @"Solution either does not exist or is not a winning Solution",
                nameof(solutionId));
        }

        var orderItems = CreateOrderItems(winningSolution);
        var nextOrderNumber = await dbContext.NextOrderNumber();

        var order = CreateOrder(nextOrderNumber, competition, winningSolution, orderItems);
        AssignRecipientQuantities(order, winningSolution);

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        return order.CallOffId;
    }

    private static IEnumerable<OrderItem> CreateOrderItems(CompetitionSolution winningSolution)
    {
        var orderItems = winningSolution.SolutionServices.Select(
                x => CreateOrderItem(x.ServiceId, x.Quantity, x.Price, x.Price.Tiers))
            .ToList();

        orderItems.Add(
            CreateOrderItem(
                winningSolution.SolutionId,
                winningSolution.Quantity,
                winningSolution.Price,
                winningSolution.Price.Tiers));

        return orderItems.ToList();
    }

    private static OrderItem CreateOrderItem(
        CatalogueItemId catalogueItemId,
        int? globalQuantity,
        IPrice price,
        IEnumerable<IOrderablePriceTier> priceTiers)
        => new(catalogueItemId)
        {
            Created = DateTime.UtcNow,
            Quantity = globalQuantity,
            OrderItemPrice = new OrderItemPrice(price)
            {
                OrderItemPriceTiers = priceTiers.Select(
                        y => new OrderItemPriceTier(y) { CatalogueItemId = catalogueItemId })
                    .ToList(),
            },
        };

    private static Order CreateOrder(int orderNumber, Competition competition, CompetitionSolution winningSolution, IEnumerable<OrderItem> orderItems) => new Order
    {
        OrderNumber = orderNumber,
        OrderType = OrderTypeEnum.Solution,
        Revision = 1,
        Description = $"Order created from competition: {competition.Id}",
        Created = DateTime.UtcNow,
        MaximumTerm = competition.ContractLength,
        OrderRecipients = competition.Recipients.Select(x => new OrderRecipient(x.Id)).ToList(),
        OrderItems = orderItems.ToList(),
        OrderingPartyId = competition.OrganisationId,
        SupplierId = winningSolution.Solution.CatalogueItem.SupplierId,
        CompetitionId = competition.Id,
        SelectedFrameworkId = competition.FrameworkId,
    };

    private static void AssignRecipientQuantities(Order order, CompetitionSolution winningSolution)
    {
        var competitionItemQuantities = winningSolution.Quantities
            .Select(x => new { ItemId = x.SolutionId, x.Quantity, x.CompetitionRecipient.OdsCode })
            .Concat(
                winningSolution.SolutionServices.SelectMany(
                    x => x.Quantities.Select(
                        y => new { ItemId = y.ServiceId, y.Quantity, y.CompetitionRecipient.OdsCode })));

        foreach (var itemQuantity in competitionItemQuantities)
        {
            var orderRecipient = order.OrderRecipients.FirstOrDefault(x => x.OdsCode == itemQuantity.OdsCode);

            orderRecipient?.SetQuantityForItem(itemQuantity.ItemId, itemQuantity.Quantity);
        }
    }
}

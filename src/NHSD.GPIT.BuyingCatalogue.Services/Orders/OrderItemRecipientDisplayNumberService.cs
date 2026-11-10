using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders;

public class OrderItemRecipientDisplayNumberService(BuyingCatalogueDbContext dbContext)
    : IOrderItemRecipientDisplayNumberService
{
    private readonly BuyingCatalogueDbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task SetOrderItemRecipientDisplayNumbers(CallOffId callOffId, string internalOrgId)
    {
        var orderId = await dbContext.OrderId(internalOrgId, callOffId);

        var offset = await GetDisplayNumberOffset(callOffId, internalOrgId);

        var orderItemRecipients = await dbContext.OrderItemSublocationRecipients
            .Include(oisr => oisr.OrderItem.CatalogueItem)
            .Where(oisr => oisr.OrderId == orderId)
            .ToListAsync();

        var orderedRecipients = orderItemRecipients
            .GroupBy(oisr => oisr.OrderItem)
            .OrderBy(group => group.Key.CatalogueItem.CatalogueItemType)
            .SelectMany(group => group)
            .ToList();

        if (orderedRecipients.Count > 0)
        {
            for (int i = 0; i < orderedRecipients.Count; i++)
            {
                orderedRecipients[i].DisplayNumber = i + 1 + offset;
            }

            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<bool> HasItemRecipientCountDiscrepancy(CallOffId callOffId, string internalOrgId)
    {
        var offset = await GetDisplayNumberOffset(callOffId, internalOrgId);
        var orderId = await dbContext.OrderId(internalOrgId, callOffId);

        var recipientItemQuery = dbContext.OrderItemSublocationRecipients.Where(oisr => oisr.OrderId == orderId);

        var recipientItemCount = await recipientItemQuery.CountAsync();
        var recipientDisplayNumberMax = await recipientItemQuery.MaxAsync(oi => oi.DisplayNumber);

        return recipientDisplayNumberMax != recipientItemCount + offset;
    }

    private async Task<int> GetDisplayNumberOffset(CallOffId callOffId, string internalOrgId)
    {
        var current = await dbContext.Order(internalOrgId, callOffId);
        var orderNumber = current.OrderNumber;

        var previous = await dbContext.Orders
            .Where(o => o.OrderNumber == orderNumber && o.Id != current.Id)
            .OrderBy(o => o.Revision)
            .LastOrDefaultAsync();

        var offset = previous != null ? await dbContext.OrderItemSublocationRecipients
            .Where(oisr => oisr.OrderId == previous.Id)
            .Select(oisr => oisr.DisplayNumber)
            .MaxAsync() : 0;

        return offset.GetValueOrDefault();
    }
}

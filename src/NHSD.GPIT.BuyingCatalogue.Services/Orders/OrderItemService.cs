using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderItemService : IOrderItemService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public OrderItemService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<OrderItem> GetOrderItem(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId) =>
            dbContext.OrderItems.AsNoTracking()
                .Include(oi => oi.OrderItemFunding)
                .Include(oi => oi.CatalogueItem)
                .Include(oi => oi.OrderItemPrice).ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(oi => oi.OrderItemRecipients).ThenInclude(ir => ir.Recipient)
                .SingleOrDefaultAsync(oi =>
                    oi.OrderId == callOffId.Id
                    && oi.CatalogueItemId == catalogueItemId
                    && oi.Order.OrderingParty.InternalIdentifier == internalOrgId);

        public async Task<OrderItem> SaveOrUpdateOrderItemFunding(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId, OrderItemFundingType selectedFundingType, decimal? centrallyAllocatedAmount = null)
        {
            var centrallyAllocated = 0M;
            var locallyAllocated = 0M;

            var item = await dbContext.OrderItems
                .Include(oi => oi.OrderItemFunding)
                .Include(oi => oi.CatalogueItem)
                .Include(oi => oi.OrderItemPrice).ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(oi => oi.OrderItemRecipients)
                .SingleOrDefaultAsync(oi =>
                    oi.OrderId == callOffId.Id
                    && oi.CatalogueItemId == catalogueItemId
                    && oi.Order.OrderingParty.InternalIdentifier == internalOrgId);

            decimal totalCost = item.CalculateTotalCost();

            switch (selectedFundingType)
            {
                case OrderItemFundingType.CentralFunding:
                    centrallyAllocated = totalCost;
                    break;
                case OrderItemFundingType.LocalFunding:
                    locallyAllocated = totalCost;
                    break;
                case OrderItemFundingType.MixedFunding:
                    centrallyAllocated = centrallyAllocatedAmount.Value;
                    locallyAllocated = totalCost - centrallyAllocatedAmount.Value;
                    break;
                case OrderItemFundingType.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(selectedFundingType));
            }

            if (item.OrderItemFunding is null)
            {
                item.OrderItemFunding = new OrderItemFunding
                {
                    OrderId = callOffId.Id,
                    CatalogueItemId = catalogueItemId,
                    TotalPrice = totalCost,
                    CentralAllocation = centrallyAllocated,
                    LocalAllocation = locallyAllocated,
                };
            }
            else
            {
                item.OrderItemFunding.TotalPrice = totalCost;
                item.OrderItemFunding.CentralAllocation = centrallyAllocated;
                item.OrderItemFunding.LocalAllocation = locallyAllocated;
            }

            await dbContext.SaveChangesAsync();

            return item;
        }
    }
}

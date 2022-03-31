using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public class OrderItemService : IOrderItemService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        private readonly IOrderService orderService;

        public OrderItemService(BuyingCatalogueDbContext dbContext, IOrderService orderService)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public async Task AddOrderItems(string internalOrgId, CallOffId callOffId, IEnumerable<CatalogueItemId> itemIds)
        {
            if (itemIds == null)
            {
                throw new ArgumentNullException(nameof(itemIds));
            }

            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            if (order == null)
            {
                return;
            }

            foreach (var id in itemIds)
            {
                var catalogueItem = dbContext.CatalogueItems.Single(x => x.Id == id);

                dbContext.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    CatalogueItemId = id,
                    CatalogueItem = catalogueItem,
                    Created = DateTime.UtcNow,
                });
            }

            await dbContext.SaveChangesAsync();
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

            var (localAllocation, centralAllocation) = CalculateFundingSplit(totalCost, selectedFundingType, centrallyAllocatedAmount);

            if (item.OrderItemFunding is null)
            {
                item.OrderItemFunding = new OrderItemFunding
                {
                    OrderId = callOffId.Id,
                    CatalogueItemId = catalogueItemId,
                    TotalPrice = totalCost,
                    CentralAllocation = centralAllocation,
                    LocalAllocation = localAllocation,
                };
            }
            else
            {
                item.OrderItemFunding.TotalPrice = totalCost;
                item.OrderItemFunding.CentralAllocation = centralAllocation;
                item.OrderItemFunding.LocalAllocation = localAllocation;
            }

            await dbContext.SaveChangesAsync();

            return item;
        }

        private static (decimal LocalAllocation, decimal CentralAllocation) CalculateFundingSplit(decimal totalCost, OrderItemFundingType fundingType, decimal? centralAllocation) =>
            fundingType switch
            {
                OrderItemFundingType.CentralFunding => (0M, totalCost),
                OrderItemFundingType.LocalFunding => (totalCost, 0M),
                OrderItemFundingType.MixedFunding => (totalCost - centralAllocation.Value, centralAllocation.Value),
                _ or OrderItemFundingType.None => throw new ArgumentOutOfRangeException(nameof(fundingType)),
            };
    }
}

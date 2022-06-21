using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public class OrderItemService : IOrderItemService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        private readonly IOrderService orderService;
        private readonly IOrderItemFundingService orderItemFundingService;

        public OrderItemService(BuyingCatalogueDbContext dbContext, IOrderService orderService, IOrderItemFundingService orderItemFundingService)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderItemFundingService = orderItemFundingService ?? throw new ArgumentNullException(nameof(orderItemFundingService));
        }

        public async Task AddOrderItems(string internalOrgId, CallOffId callOffId, IEnumerable<CatalogueItemId> itemIds)
        {
            if (itemIds == null)
            {
                throw new ArgumentNullException(nameof(itemIds));
            }

            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            if (order == null)
            {
                return;
            }

            foreach (var id in itemIds)
            {
                if (order.OrderItem(id) != null)
                {
                    continue;
                }

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

        public async Task DeleteOrderItems(string internalOrgId, CallOffId callOffId, IEnumerable<CatalogueItemId> itemIds)
        {
            if (itemIds == null)
            {
                throw new ArgumentNullException(nameof(itemIds));
            }

            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            if (order == null)
            {
                return;
            }

            foreach (var id in itemIds)
            {
                var orderItem = order.OrderItem(id);

                if (orderItem == null)
                {
                    continue;
                }

                dbContext.OrderItems.Remove(orderItem);
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

        public async Task SetOrderItemFunding(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId)
        {
            var item = await GetOrderItemTracked(callOffId, internalOrgId, catalogueItemId);
            var fundingType = await orderItemFundingService.GetFundingType(item);

            switch (fundingType.IsForcedFunding())
            {
                case true when item.FundingType != fundingType:
                    await SaveOrUpdateOrderItemFunding(item, callOffId, catalogueItemId, fundingType);
                    break;

                case false when item.IsForcedFunding:
                    item.OrderItemFunding = null;
                    await dbContext.SaveChangesAsync();
                    break;
            }
        }

        public async Task UpdateOrderItemFunding(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId, OrderItemFundingType selectedFundingType)
        {
            var item = await GetOrderItemTracked(callOffId, internalOrgId, catalogueItemId);

            await SaveOrUpdateOrderItemFunding(item, callOffId, catalogueItemId, selectedFundingType);
        }

        private Task<OrderItem> GetOrderItemTracked(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId) =>
            dbContext.OrderItems
                .Include(oi => oi.OrderItemFunding)
                .Include(oi => oi.CatalogueItem)
                .Include(oi => oi.OrderItemPrice).ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(oi => oi.OrderItemRecipients).ThenInclude(ir => ir.Recipient)
                .SingleOrDefaultAsync(oi =>
                    oi.OrderId == callOffId.Id
                    && oi.CatalogueItemId == catalogueItemId
                    && oi.Order.OrderingParty.InternalIdentifier == internalOrgId);

        private async Task SaveOrUpdateOrderItemFunding(
            OrderItem item,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            OrderItemFundingType selectedFundingType)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (item.FundingType == selectedFundingType)
                return;

            if (item.OrderItemFunding is null)
            {
                item.OrderItemFunding = new OrderItemFunding
                {
                    OrderId = callOffId.Id,
                    CatalogueItemId = catalogueItemId,
                    OrderItemFundingType = selectedFundingType,
                };
            }
            else
            {
                item.OrderItemFunding.OrderItemFundingType = selectedFundingType;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}

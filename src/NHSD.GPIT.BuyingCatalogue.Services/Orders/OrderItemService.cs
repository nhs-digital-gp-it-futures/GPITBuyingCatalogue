using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
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

        public async Task<OrderItem> SaveOrUpdateFundingIfItemIsLocalOrNoFunding(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId)
        {
            var item = await GetOrderItemTracked(callOffId, internalOrgId, catalogueItemId);

            var (isForcedFunding, fundingType) = await OrderItemShouldHaveForcedFundingType(item);

            if (isForcedFunding && !item.IsCurrentlyForcedFunding())
            {
                return await SaveOrUpdateOrderItemFunding(item, callOffId, catalogueItemId, fundingType);
            }
            else if (!isForcedFunding && item.IsCurrentlyForcedFunding())
            {
                item.OrderItemFunding = null;

                _ = await dbContext.SaveChangesAsync();
            }

            return item;
        }

        public async Task<OrderItem> SaveOrUpdateOrderItemFunding(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId, OrderItemFundingType selectedFundingType)
        {
            var item = await GetOrderItemTracked(callOffId, internalOrgId, catalogueItemId);

            return await SaveOrUpdateOrderItemFunding(item, callOffId, catalogueItemId, selectedFundingType);
        }

        public async Task<(bool IsForcedFunding, OrderItemFundingType FundingType)> OrderItemShouldHaveForcedFundingType(OrderItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (item.OrderItemPrice.CalculateTotalCost(item.GetQuantity()) == 0)
                return (true, OrderItemFundingType.NoFundingRequired);

            if (item.CatalogueItem.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.AssociatedService)
                return (false, OrderItemFundingType.None);

            if (item.CatalogueItem.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.Solution
                && await dbContext.CatalogueItems.Where(ci => ci.Id == item.CatalogueItemId && ci.Solution.FrameworkSolutions.All(fs => fs.Framework.LocalFundingOnly)).AnyAsync())
                return (true, OrderItemFundingType.LocalFundingOnly);

            if (await dbContext.CatalogueItems.Where(ci =>
            ci.Id == item.CatalogueItemId
            && ci.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.AdditionalService
            && ci.AdditionalService.Solution.FrameworkSolutions.All(fs => fs.Framework.LocalFundingOnly)).AnyAsync())
                return (true, OrderItemFundingType.LocalFundingOnly);

            return (false, OrderItemFundingType.None);
        }

        public Task<OrderItem> GetOrderItemTracked(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId) =>
            dbContext.OrderItems
                .Include(oi => oi.OrderItemFunding)
                .Include(oi => oi.CatalogueItem)
                .Include(oi => oi.OrderItemPrice).ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(oi => oi.OrderItemRecipients).ThenInclude(ir => ir.Recipient)
                .SingleOrDefaultAsync(oi =>
                    oi.OrderId == callOffId.Id
                    && oi.CatalogueItemId == catalogueItemId
                    && oi.Order.OrderingParty.InternalIdentifier == internalOrgId);

        public async Task<OrderItem> SaveOrUpdateOrderItemFunding(
            OrderItem item,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            OrderItemFundingType selectedFundingType)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (item.CurrentFundingType() == selectedFundingType)
                return item;

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

            return item;
        }
    }
}

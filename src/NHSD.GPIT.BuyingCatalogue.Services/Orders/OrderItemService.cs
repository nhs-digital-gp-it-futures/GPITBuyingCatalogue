using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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

        public async Task AddOrderItems(string internalOrgId, CallOffId callOffId, IEnumerable<CatalogueItemId> itemIds, int? parentId = null)
        {
            ArgumentNullException.ThrowIfNull(itemIds);

            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;

            if (order == null)
            {
                return;
            }

            var solutionItem = order.GetSolutionOrderItem();

            var catalogueItems = await dbContext.CatalogueItems.Where(catalogueItem => itemIds.Contains(catalogueItem.Id)).ToListAsync();
            var solutionCatalogueItem = catalogueItems.FirstOrDefault(catalogueItem => catalogueItem.CatalogueItemType == CatalogueItemType.Solution);
            if (solutionCatalogueItem != null && solutionItem == null)
            {
                solutionItem = order.InitialiseOrderItem(solutionCatalogueItem.Id);
                dbContext.OrderItems.Add(solutionItem);
                await dbContext.SaveChangesAsync();
            }

            catalogueItems.Where(ci => ci.CatalogueItemType != CatalogueItemType.Solution)
                .ForEach(catalogueItem => dbContext.OrderItems.Add(order.InitialiseOrderItem(catalogueItem.Id, parentId ?? solutionItem?.Id)));

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteOrderItems(string internalOrgId, CallOffId callOffId, IEnumerable<int> itemIds)
        {
            if (itemIds == null)
            {
                throw new ArgumentNullException(nameof(itemIds));
            }

            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;

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

        public async Task<OrderItem> GetOrderItem(CallOffId callOffId, string internalOrgId, int orderItemId)
        {
            var orderId = await dbContext.OrderId(internalOrgId, callOffId);

            return await dbContext.OrderItems
                .AsNoTracking()
                .Include(oi => oi.Parent)
                    .ThenInclude(parent => parent.CatalogueItem)
                .Include(oi => oi.Services)
                .Include(oi => oi.OrderItemFunding)
                .Include(oi => oi.CatalogueItem)
                .Include(oi => oi.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers)
                .AsSplitQuery()
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId
                    && oi.Order.Id == orderId
                    && oi.Order.OrderingParty.InternalIdentifier == internalOrgId);
        }

        public async Task UpdateOrderItemFunding(CallOffId callOffId, string internalOrgId, int orderItemId, OrderItemFundingType selectedFundingType)
        {
            var item = await GetOrderItemTracked(callOffId, internalOrgId, orderItemId);

            await SaveOrUpdateOrderItemFunding(item, selectedFundingType);
        }

        public async Task DetectChangesInFundingAndDelete(CallOffId callOffId, string internalOrgId, int orderItemId)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;
            var item = order.OrderItems.FirstOrDefault(oi => oi.Id == orderItemId);

            if (item?.OrderItemFunding is null || !item.IsReadyForReview(callOffId.IsAmendment, orderWrapper.DetermineOrderRecipients(item)))
                return;

            var newFundingType = item.FundingType;

            if (item.TotalCost(order.FlattenedRecipients.ToList()) == 0)
                newFundingType = OrderItemFundingType.NoFundingRequired;
            else if (item.Order.OrderingParty.OrganisationType == OrganisationType.GP)
                newFundingType = OrderItemFundingType.LocalFundingOnly;
            else if (item.Order.HasSingleFundingType)
                newFundingType = item.Order.SelectedFramework.FundingTypes.First().AsOrderItemFundingType();

            if (item.FundingType == newFundingType)
                return;

            item.OrderItemFunding = null;

            await dbContext.SaveChangesAsync();
        }

        public async Task SetOrderItemEstimationPeriod(CallOffId callOffId, string internalOrgId, int orderItemId, CataloguePrice price)
        {
            if (price is null)
                throw new ArgumentNullException(nameof(price));

            var orderId = await dbContext.OrderId(internalOrgId, callOffId);

            var orderItem = await dbContext.OrderItems
                .FirstAsync(oi =>
                    oi.Id == orderItemId
                    && oi.Order.Id == orderId
                    && oi.Order.OrderingParty.InternalIdentifier == internalOrgId);

            orderItem.EstimationPeriod = price.ProvisioningType switch
            {
                ProvisioningType.Patient => TimeUnit.PerMonth,
                ProvisioningType.OnDemand => price.BillingPeriod,
                ProvisioningType.Declarative or _ => TimeUnit.PerYear,
            };

            await dbContext.SaveChangesAsync();
        }

        private async Task<OrderItem> GetOrderItemTracked(CallOffId callOffId, string internalOrgId, int orderItemId)
        {
            var orderId = await dbContext.OrderId(internalOrgId, callOffId);

            return await dbContext.OrderItems
                .Include(oi => oi.OrderItemFunding)
                .Include(oi => oi.CatalogueItem)
                .Include(oi => oi.Parent)
                    .ThenInclude(p => p.CatalogueItem)
                .Include(oi => oi.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.SelectedFramework)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.OrderingParty)
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId
                    && oi.Id == orderItemId
                    && oi.Order.OrderingParty.InternalIdentifier == internalOrgId);
        }

        private async Task SaveOrUpdateOrderItemFunding(
            OrderItem item,
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
                    OrderItemId = item.Id,
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

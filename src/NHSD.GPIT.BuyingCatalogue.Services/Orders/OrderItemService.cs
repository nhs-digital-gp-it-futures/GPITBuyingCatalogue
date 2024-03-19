using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task AddOrderItems(string internalOrgId, CallOffId callOffId, IEnumerable<CatalogueItemId> itemIds)
        {
            ArgumentNullException.ThrowIfNull(itemIds);

            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;

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

                var catalogueItem = dbContext.CatalogueItems.First(x => x.Id == id);
                dbContext.OrderItems.Add(order.InitialiseOrderItem(catalogueItem.Id));
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteOrderItems(string internalOrgId, CallOffId callOffId, IEnumerable<CatalogueItemId> itemIds)
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

        public async Task<OrderItem> GetOrderItem(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId)
        {
            var orderId = await dbContext.OrderId(internalOrgId, callOffId);

            return await dbContext.OrderItems
                .AsNoTracking()
                .Include(oi => oi.OrderItemFunding)
                .Include(oi => oi.CatalogueItem)
                .Include(oi => oi.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers)
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId
                    && oi.CatalogueItemId == catalogueItemId
                    && oi.Order.OrderingParty.InternalIdentifier == internalOrgId);
        }

        public async Task UpdateOrderItemFunding(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId, OrderItemFundingType selectedFundingType)
        {
            var item = await GetOrderItemTracked(callOffId, internalOrgId, catalogueItemId);

            await SaveOrUpdateOrderItemFunding(item, callOffId, catalogueItemId, selectedFundingType);
        }

        public async Task DetectChangesInFundingAndDelete(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;
            var item = order.OrderItems.FirstOrDefault(oi => oi.CatalogueItemId == catalogueItemId);

            if (item.OrderItemFunding is null || !item.IsReadyForReview(callOffId.IsAmendment, orderWrapper.DetermineOrderRecipients(item.CatalogueItemId)))
                return;

            var newFundingType = item.FundingType;

            if (item.TotalCost(order.OrderRecipients) == 0)
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

        public async Task SetOrderItemEstimationPeriod(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId, CataloguePrice price)
        {
            if (price is null)
                throw new ArgumentNullException(nameof(price));

            var orderId = await dbContext.OrderId(internalOrgId, callOffId);

            var orderItem = await dbContext.OrderItems
                .FirstAsync(oi =>
                    oi.OrderId == orderId
                    && oi.CatalogueItemId == catalogueItemId
                    && oi.Order.OrderingParty.InternalIdentifier == internalOrgId);

            orderItem.EstimationPeriod = price.ProvisioningType switch
            {
                ProvisioningType.Patient => TimeUnit.PerMonth,
                ProvisioningType.OnDemand => price.BillingPeriod,
                ProvisioningType.Declarative or _ => TimeUnit.PerYear,
            };

            await dbContext.SaveChangesAsync();
        }

        private async Task<OrderItem> GetOrderItemTracked(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId)
        {
            var orderId = await dbContext.OrderId(internalOrgId, callOffId);

            return await dbContext.OrderItems
                .Include(oi => oi.OrderItemFunding)
                .Include(oi => oi.CatalogueItem)
                .Include(oi => oi.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.SelectedFramework)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.OrderingParty)
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId
                    && oi.CatalogueItemId == catalogueItemId
                    && oi.Order.OrderingParty.InternalIdentifier == internalOrgId);
        }

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
                var orderId = await dbContext.OrderId(callOffId);

                item.OrderItemFunding = new OrderItemFunding
                {
                    OrderId = orderId,
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

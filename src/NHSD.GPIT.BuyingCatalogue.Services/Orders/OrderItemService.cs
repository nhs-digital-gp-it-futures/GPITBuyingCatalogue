using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderItemService : IOrderItemService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IServiceRecipientService serviceRecipientService;
        private readonly IOrderService orderService;

        public OrderItemService(
            BuyingCatalogueDbContext dbContext,
            IServiceRecipientService serviceRecipientService,
            IOrderService orderService)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.serviceRecipientService = serviceRecipientService ?? throw new ArgumentNullException(nameof(serviceRecipientService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public async Task Create(CallOffId callOffId, string odsCode, CreateOrderItemModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var order = await orderService.GetOrderWithDefaultDeliveryDatesAndOrderItems(callOffId, odsCode);

            var catalogueItemId = model.CatalogueItemId;
            var catalogueItem = await dbContext.FindAsync<CatalogueItem>(catalogueItemId);
            var serviceRecipients = await AddOrUpdateServiceRecipients(model);

            var defaultDeliveryDate = order.DefaultDeliveryDates.SingleOrDefault(d => d.CatalogueItemId == catalogueItemId);
            var estimationPeriod = catalogueItem.CatalogueItemType.InferEstimationPeriod(model.CataloguePrice.ProvisioningType, model.EstimationPeriod);

            var item = order.AddOrUpdateOrderItem(new OrderItem
            {
                CatalogueItem = catalogueItem,
                DefaultDeliveryDate = defaultDeliveryDate?.DeliveryDate,
                EstimationPeriod = estimationPeriod,
                OrderId = order.Id,
                PriceId = model.CataloguePrice.CataloguePriceId,
                Price = model.AgreedPrice,
                Created = DateTime.UtcNow,
            });

            item.SetRecipients(model.ServiceRecipients.Select(r => new OrderItemRecipient
            {
                DeliveryDate = r.DeliveryDate,
                Quantity = r.Quantity.GetValueOrDefault(),
                Recipient = serviceRecipients[r.OdsCode],
            }));

            if (defaultDeliveryDate is not null)
                dbContext.DefaultDeliveryDates.Remove(defaultDeliveryDate);

            await dbContext.SaveChangesAsync();
        }

        public async Task<List<OrderItem>> GetOrderItems(CallOffId callOffId,  string odsCode, CatalogueItemType? catalogueItemType)
        {
            Expression<Func<Order, IEnumerable<OrderItem>>> orderItems = catalogueItemType is null
                ? o => o.OrderItems
                : o => o.OrderItems.Where(i => i.CatalogueItem.CatalogueItemType == catalogueItemType);

            if (!await dbContext.Orders.AnyAsync(o => o.Id == callOffId.Id))
                return null;

            return await dbContext.Orders
                .Where(o => o.Id == callOffId.Id && o.OrderingParty.OdsCode == odsCode)
                .Include(orderItems).ThenInclude(i => i.CatalogueItem)
                .Include(orderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(orderItems).ThenInclude(i => i.CataloguePrice).ThenInclude(p => p.PricingUnit)
                .SelectMany(orderItems)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<OrderItem> GetOrderItem(CallOffId callOffId, string odsCode, CatalogueItemId catalogueItemId)
        {
            Expression<Func<Order, IEnumerable<OrderItem>>> orderItems = o =>
                o.OrderItems.Where(i => i.CatalogueItem.Id == catalogueItemId);

            return dbContext.Orders
                .Where(o => o.Id == callOffId.Id && o.OrderingParty.OdsCode == odsCode)
                .Include(orderItems).ThenInclude(i => i.CatalogueItem)
                .Include(orderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(orderItems).ThenInclude(i => i.CataloguePrice).ThenInclude(p => p.PricingUnit)
                .SelectMany(orderItems)
                .SingleOrDefaultAsync();
        }

        public async Task DeleteOrderItem(CallOffId callOffId,  string odsCode, CatalogueItemId catalogueItemId)
        {
            var order = await dbContext.Orders
                .Where(o => o.Id == callOffId.Id && o.OrderingParty.OdsCode == odsCode)
                .Include(o => o.OrderItems)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem).ThenInclude(a => a.AdditionalService)
                .SingleAsync();

            order.DeleteOrderItemAndUpdateProgress(catalogueItemId);

            await dbContext.SaveChangesAsync();
        }

        private Task<IReadOnlyDictionary<string, ServiceRecipient>> AddOrUpdateServiceRecipients(CreateOrderItemModel model)
        {
            var serviceRecipients = model.ServiceRecipients.Select(s => new ServiceRecipient { OdsCode = s.OdsCode, Name = s.Name });

            return serviceRecipientService.AddOrUpdateServiceRecipients(serviceRecipients);
        }
    }
}

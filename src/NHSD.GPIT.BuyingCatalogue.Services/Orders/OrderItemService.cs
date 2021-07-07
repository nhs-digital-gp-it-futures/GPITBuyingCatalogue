using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderItemService : IOrderItemService
    {
        private readonly GPITBuyingCatalogueDbContext dbContext;
        private readonly ICreateOrderItemValidator orderItemValidator;
        private readonly IServiceRecipientService serviceRecipientService;
        private readonly IOrderService orderService;

        public OrderItemService(
            GPITBuyingCatalogueDbContext dbContext,
            ICreateOrderItemValidator orderItemValidator,
            IServiceRecipientService serviceRecipientService,
            IOrderService orderService)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.orderItemValidator = orderItemValidator ?? throw new ArgumentNullException(nameof(orderItemValidator));
            this.serviceRecipientService = serviceRecipientService ?? throw new ArgumentNullException(nameof(serviceRecipientService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public async Task<AggregateValidationResult> Create(CallOffId callOffId, CreateOrderItemModel model)
        {
            var order = await orderService.GetOrder(callOffId);

            var aggregateValidationResult = orderItemValidator.Validate(order, model, model.CatalogueItemType);
            if (!aggregateValidationResult.Success)
                return aggregateValidationResult;

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

            return aggregateValidationResult;
        }

        public async Task<List<OrderItem>> GetOrderItems(CallOffId callOffId, CatalogueItemType? catalogueItemType)
        {
            Expression<Func<Order, IEnumerable<OrderItem>>> orderItems = catalogueItemType is null
                ? o => o.OrderItems
                : o => o.OrderItems.Where(i => i.CatalogueItem.CatalogueItemType == catalogueItemType);

            if (!await dbContext.Orders.AnyAsync(o => o.Id == callOffId.Id))
                return null;

            return await dbContext.Orders
                .Where(o => o.Id == callOffId.Id)
                .Include(orderItems).ThenInclude(i => i.CatalogueItem)
                .Include(orderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(orderItems).ThenInclude(i => i.CataloguePrice).ThenInclude(p => p.PricingUnit)
                .SelectMany(orderItems)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<OrderItem> GetOrderItem(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            Expression<Func<Order, IEnumerable<OrderItem>>> orderItems = o =>
                o.OrderItems.Where(i => i.CatalogueItem.CatalogueItemId == catalogueItemId);

            return dbContext.Orders
                .Where(o => o.Id == callOffId.Id)
                .Include(orderItems).ThenInclude(i => i.CatalogueItem)
                .Include(orderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(orderItems).ThenInclude(i => i.CataloguePrice).ThenInclude(p => p.PricingUnit)
                .SelectMany(orderItems)
                .SingleOrDefaultAsync();
        }

        public async Task DeleteOrderItem(CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = await orderService.GetOrder(callOffId);

            if (order is null)
                throw new ArgumentNullException(nameof(catalogueItemId));

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

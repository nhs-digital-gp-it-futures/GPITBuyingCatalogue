using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public class OrderItemService : IOrderItemService
    {
        private readonly ILogWrapper<OrderItemService> logger;
        private readonly OrderingDbContext dbContext;
        private readonly ICreateOrderItemValidator orderItemValidator;
        private readonly IServiceRecipientService serviceRecipientService;
        private readonly IOrderService orderService;

        public OrderItemService(
            ILogWrapper<OrderItemService> logger,
            OrderingDbContext dbContext,
            ICreateOrderItemValidator orderItemValidator,
            IServiceRecipientService serviceRecipientService,
            IOrderService orderService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.orderItemValidator = orderItemValidator ?? throw new ArgumentNullException(nameof(orderItemValidator));
            this.serviceRecipientService = serviceRecipientService ?? throw new ArgumentNullException(nameof(serviceRecipientService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public async Task<AggregateValidationResult> Create(string callOffId, CreateOrderItemModel model)
        {
            var order = await orderService.GetOrder(callOffId);

            var aggregateValidationResult = orderItemValidator.Validate(order, model, model.CatalogueItemType);
            if (!aggregateValidationResult.Success)
                return aggregateValidationResult;

            //// TODO - handle case of non-success
            (var success, var catalogueItemId) = CatalogueItemId.Parse(model.CatalogueItemId);

            var catalogueItem = await AddOrUpdateCatalogueItem(catalogueItemId, model, model.CatalogueItemType);
            var serviceRecipients = await AddOrUpdateServiceRecipients(model);
            var pricingUnit = await AddOrUpdatePricingUnit(model);

            var defaultDeliveryDate = order.DefaultDeliveryDates.SingleOrDefault(d => d.CatalogueItemId == catalogueItemId);
            var estimationPeriod = model.CatalogueItemType.InferEstimationPeriod(model.ProvisioningType, model.EstimationPeriod);

            var item = order.AddOrUpdateOrderItem(new OrderItem
            {
                CatalogueItem = catalogueItem,
                CataloguePriceType = model.Type,
                CurrencyCode = model.CurrencyCode,
                DefaultDeliveryDate = defaultDeliveryDate?.DeliveryDate,
                EstimationPeriod = estimationPeriod,
                OrderId = order.Id,
                PriceId = model.PriceId,
                Price = model.Price,
                PricingUnit = pricingUnit,
                PriceTimeUnit = model.TimeUnit,
                ProvisioningType = model.ProvisioningType,
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

        public async Task<List<OrderItem>> GetOrderItems(string callOffId, CatalogueItemType catalogueItemType)
        {
            Expression<Func<Order, IEnumerable<OrderItem>>> orderItems = catalogueItemType is null
                ? o => o.OrderItems
                : o => o.OrderItems.Where(i => i.CatalogueItem.CatalogueItemType == catalogueItemType);

            var callOffIdStruct = CallOffId.Parse(callOffId);

            if (!await dbContext.Orders.AnyAsync(o => o.Id == callOffIdStruct.Id))
                return null;

            return await dbContext.Orders
                .Where(o => o.Id == callOffIdStruct.Id)
                .Include(orderItems).ThenInclude(i => i.CatalogueItem)
                .Include(orderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(orderItems).ThenInclude(i => i.PricingUnit)
                .SelectMany(orderItems)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OrderItem> GetOrderItem(string callOffId, string catalogueItemId)
        {
            // TODO - handle non-success
            (bool success, CatalogueItemId itemId) = CatalogueItemId.Parse(catalogueItemId);

            Expression<Func<Order, IEnumerable<OrderItem>>> orderItems = o =>
                o.OrderItems.Where(i => i.CatalogueItem.Id == itemId);

            var callOffIdStruct = CallOffId.Parse(callOffId);

            return await dbContext.Orders
                .Where(o => o.Id == callOffIdStruct.Id)
                .Include(orderItems).ThenInclude(i => i.CatalogueItem)
                .Include(orderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(orderItems).ThenInclude(i => i.PricingUnit)
                .SelectMany(orderItems)
                .SingleOrDefaultAsync();
        }

        public async Task<int> DeleteOrderItem(string callOffId, string catalogueItemId)
        {
            // TODO - handle non-success
            (bool success, CatalogueItemId itemId) = CatalogueItemId.Parse(catalogueItemId);

            var order = await orderService.GetOrder(callOffId);

            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var result = order.DeleteOrderItemAndUpdateProgress(itemId);

            await dbContext.SaveChangesAsync();

            return result;
        }

        private async Task<CatalogueItem> AddOrUpdateCatalogueItem(
            CatalogueItemId catalogueItemId,
            CreateOrderItemModel model,
            CatalogueItemType catalogueItemType)
        {
            CatalogueItem parentCatalogueItem = null;
            var catalogueSolutionId = model.CatalogueSolutionId;

            if (catalogueSolutionId is not null)
                parentCatalogueItem = await dbContext.FindAsync<CatalogueItem>(CatalogueItemId.Parse(catalogueSolutionId).Id);

            var catalogueItem = await dbContext.FindAsync<CatalogueItem>(catalogueItemId) ?? new CatalogueItem
            {
                Id = catalogueItemId,
                CatalogueItemType = catalogueItemType,
            };

            catalogueItem.Name = model.CatalogueItemName;
            catalogueItem.ParentCatalogueItemId = parentCatalogueItem?.Id;

            return catalogueItem;
        }

        private async Task<IReadOnlyDictionary<string, EntityFramework.Models.Ordering.ServiceRecipient>> AddOrUpdateServiceRecipients(CreateOrderItemModel model)
        {
            var serviceRecipients = model.ServiceRecipients.Select(s => new EntityFramework.Models.Ordering.ServiceRecipient { OdsCode = s.OdsCode, Name = s.Name });

            return await serviceRecipientService.AddOrUpdateServiceRecipients(serviceRecipients);
        }

        private async Task<PricingUnit> AddOrUpdatePricingUnit(CreateOrderItemModel model)
        {
            var pricingUnit = await dbContext.FindAsync<PricingUnit>(model.ItemUnit.Name) ?? new PricingUnit
            {
                Name = model.ItemUnit.Name,
            };

            pricingUnit.Description = model.ItemUnit.Description;

            return pricingUnit;
        }
    }
}

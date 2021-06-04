using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public OrderItemService(
            ILogWrapper<OrderItemService> logger,
            OrderingDbContext dbContext,
            ICreateOrderItemValidator orderItemValidator,
            IServiceRecipientService serviceRecipientService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.orderItemValidator = orderItemValidator ?? throw new ArgumentNullException(nameof(orderItemValidator));
            this.serviceRecipientService = serviceRecipientService ?? throw new ArgumentNullException(nameof(serviceRecipientService));
        }

        public async Task<AggregateValidationResult> Create(Order order, CatalogueItemId catalogueItemId, CreateOrderItemModel model)
        {
            var catalogueItemType = CatalogueItemType.Parse(model.CatalogueItemType);

            var aggregateValidationResult = orderItemValidator.Validate(order, model, catalogueItemType);
            if (!aggregateValidationResult.Success)
                return aggregateValidationResult;

            var catalogueItem = await AddOrUpdateCatalogueItem(catalogueItemId, model, catalogueItemType);
            var serviceRecipients = await AddOrUpdateServiceRecipients(model);
            var pricingUnit = await AddOrUpdatePricingUnit(model);

            var defaultDeliveryDate = order.DefaultDeliveryDates.SingleOrDefault(d => d.CatalogueItemId == catalogueItemId);
            var provisioningType = ProvisioningType.Parse(model.ProvisioningType);
            var estimationPeriod = catalogueItemType.InferEstimationPeriod(
                provisioningType,
                TimeUnit.Parse(model.EstimationPeriod));

            var item = order.AddOrUpdateOrderItem(new OrderItem
            {
                CatalogueItem = catalogueItem,
                CataloguePriceType = CataloguePriceType.Parse(model.Type),
                CurrencyCode = model.CurrencyCode,
                DefaultDeliveryDate = defaultDeliveryDate?.DeliveryDate,
                EstimationPeriod = estimationPeriod,
                OrderId = order.Id,
                PriceId = model.PriceId,
                Price = model.Price,
                PricingUnitNameNavigation = pricingUnit,
                TimeUnit = model.TimeUnit?.ToTimeUnit(),
                ProvisioningType = ProvisioningType.Parse(model.ProvisioningType),
            });

            item.SetRecipients(model.ServiceRecipients.Select(r => new OrderItemRecipient
            {
                DeliveryDate = r.DeliveryDate,
                Quantity = r.Quantity.GetValueOrDefault(),
                OdsCodeNavigation = serviceRecipients[r.OdsCode],
            }));

            if (defaultDeliveryDate is not null)
                dbContext.DefaultDeliveryDate.Remove(defaultDeliveryDate);

            await dbContext.SaveChangesAsync();

            return aggregateValidationResult;
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

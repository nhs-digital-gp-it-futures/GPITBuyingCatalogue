using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.FundingTypes;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    [ExcludeFromCodeCoverage]
    public class CsvService : CsvServiceBase, ICsvService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IFundingTypeService fundingTypeService;

        public CsvService(BuyingCatalogueDbContext dbContext, IFundingTypeService fundingTypeService)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.fundingTypeService = fundingTypeService ?? throw new ArgumentNullException(nameof(fundingTypeService));
        }

        public async Task CreateFullOrderCsvAsync(int orderId, MemoryStream stream, bool showRevisions = false)
        {
            var items = await CreateFullOrderCsv(orderId);

            if (showRevisions)
            {
                var order = await dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == orderId);

                var revisions = await dbContext.Orders
                    .AsNoTracking()
                    .Where(x => x.OrderNumber == order.OrderNumber && x.Revision < order.Revision)
                    .OrderByDescending(x => x.Revision)
                    .Select(y => y.Id).ToListAsync();

                foreach (var id in revisions)
                {
                    items.AddRange(await CreateFullOrderCsv(id));
                }
            }

            await WriteRecordsAsync<FullOrderCsvModel, FullOrderCsvModelMap>(stream, items);
        }

        public async Task<int> CreatePatientNumberCsvAsync(int orderId, MemoryStream stream)
        {
            var fundingTypes = await GetFundingTypes(orderId);
            var prices = await GetPrices(orderId);
            var (supplierId, supplierName) = await GetSupplierDetails(orderId);

            var items = await dbContext.OrderItemRecipients
                .Include(x => x.OrderItem).ThenInclude(x => x.OrderItemFunding)
                .AsNoTracking()
                .Where(oir => oir.OrderId == orderId)
                .Select(oir => new PatientOrderCsvModel
                {
                    CallOffId = oir.OrderItem.Order.CallOffId,
                    OdsCode = oir.OrderItem.Order.OrderingParty.ExternalIdentifier,
                    OrganisationName = oir.OrderItem.Order.OrderingParty.Name,
                    CommencementDate = oir.OrderItem.Order.CommencementDate,
                    ServiceRecipientId = oir.Recipient.OdsCode,
                    ServiceRecipientName = oir.Recipient.Name,
                    SupplierId = supplierId,
                    SupplierName = supplierName,
                    ProductId = oir.OrderItem.CatalogueItemId.ToString(),
                    ProductName = oir.OrderItem.CatalogueItem.Name,
                    ProductType = oir.OrderItem.CatalogueItem.CatalogueItemType.DisplayName(),
                    ProductTypeId = (int)oir.OrderItem.CatalogueItem.CatalogueItemType,

                    // TODO: Stop this reporting incorrectly when quantity is (erroneously) defined at both order item & recipient level
                    QuantityOrdered = oir.Quantity ?? oir.OrderItem.Quantity ?? 0,
                    UnitOfOrder = oir.OrderItem.OrderItemPrice.Description,
                    Price = prices[oir.OrderItem.CatalogueItemId],
                    FundingType = fundingTypeService.GetFundingType(fundingTypes, oir.OrderItem.FundingType).Description(),
                    M1Planned = oir.DeliveryDate,
                    Framework = oir.OrderItem.Order.SelectedFrameworkId,
                    InitialTerm = oir.OrderItem.Order.InitialPeriod,
                    MaximumTerm = oir.OrderItem.Order.MaximumTerm,
                }).ToListAsync();

            if (items.Count == 0)
                return 0;

            for (int i = 0; i < items.Count; i++)
                items[i].ServiceRecipientItemId = $"{items[i].CallOffId}-{items[i].ServiceRecipientId}-{i + 1}";

            await WriteRecordsAsync<PatientOrderCsvModel, PatientOrderCsvModelMap>(stream, items);

            return items.Count;
        }

        private static string TimeUnitDescription(TimeUnit? timeUnit) => timeUnit?.Description() ?? string.Empty;

        private async Task<Dictionary<CatalogueItemId, TimeUnit?>> GetBillingPeriods(int orderId)
        {
            return await dbContext.OrderItems
                .Include(x => x.OrderItemPrice)
                .AsNoTracking()
                .Where(x => x.OrderId == orderId)
                .ToDictionaryAsync(
                    x => x.CatalogueItemId,
                    x => x.OrderItemPrice?.BillingPeriod);
        }

        private async Task<List<OrderItemFundingType>> GetFundingTypes(int orderId)
        {
            return await dbContext.OrderItems
                .Include(x => x.OrderItemFunding)
                .AsNoTracking()
                .Where(x => x.OrderId == orderId)
                .Select(x => x.OrderItemFunding.OrderItemFundingType)
                .Distinct()
                .ToListAsync();
        }

        private async Task<Dictionary<CatalogueItemId, decimal>> GetPrices(int orderId)
        {
            return await dbContext.OrderItems
                .Include(x => x.OrderItemPrice)
                .ThenInclude(x => x.OrderItemPriceTiers)
                .AsNoTracking()
                .Where(x => x.OrderId == orderId)
                .ToDictionaryAsync(
                    x => x.CatalogueItemId,
                    x => x.OrderItemPrice?.OrderItemPriceTiers?.FirstOrDefault()?.Price ?? decimal.Zero);
        }

        private async Task<(int SupplierId, string SupplierName)> GetSupplierDetails(int orderId)
        {
            var order = await dbContext.Orders
                .Include(x => x.Supplier)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
            {
                return (0, string.Empty);
            }

            var output = order.Completed.HasValue
                ? await dbContext.Suppliers.TemporalAsOf(order.Completed.Value).FirstOrDefaultAsync(x => x.Id == order.SupplierId)
                : order.Supplier;

            output ??= order.Supplier;

            var name = output?.Name ?? string.Empty;
            var legalName = output?.LegalName ?? string.Empty;

            return (output?.Id ?? 0, string.Equals(name, legalName, StringComparison.OrdinalIgnoreCase) ? name : legalName);
        }

        private async Task<List<FullOrderCsvModel>> CreateFullOrderCsv(int orderId)
        {
            var billingPeriods = await GetBillingPeriods(orderId);
            var fundingTypes = await GetFundingTypes(orderId);
            var prices = await GetPrices(orderId);
            var (supplierId, supplierName) = await GetSupplierDetails(orderId);

            var items = await dbContext.OrderItemRecipients
                .Include(x => x.OrderItem).ThenInclude(x => x.OrderItemFunding)
                .AsNoTracking()
                .Where(oir => oir.OrderId == orderId)
                .Select(oir => new FullOrderCsvModel
                {
                    CallOffId = oir.OrderItem.Order.CallOffId,
                    OdsCode = oir.OrderItem.Order.OrderingParty.ExternalIdentifier,
                    OrganisationName = oir.OrderItem.Order.OrderingParty.Name,
                    CommencementDate = oir.OrderItem.Order.CommencementDate,
                    ServiceRecipientId = oir.Recipient.OdsCode,
                    ServiceRecipientName = oir.Recipient.Name,
                    SupplierId = $"{supplierId}",
                    SupplierName = supplierName,
                    ProductId = oir.OrderItem.CatalogueItemId.ToString(),
                    ProductName = oir.OrderItem.CatalogueItem.Name,
                    ProductType = oir.OrderItem.CatalogueItem.CatalogueItemType.DisplayName(),
                    ProductTypeId = (int)oir.OrderItem.CatalogueItem.CatalogueItemType,

                    // TODO: Stop this reporting incorrectly when quantity is (erroneously) defined at both order item & recipient level
                    QuantityOrdered = oir.Quantity ?? oir.OrderItem.Quantity ?? 0,
                    UnitOfOrder = oir.OrderItem.OrderItemPrice.Description,
                    UnitTime = TimeUnitDescription(billingPeriods[oir.OrderItem.CatalogueItemId]),
                    EstimationPeriod = TimeUnitDescription(oir.OrderItem.EstimationPeriod),
                    Price = prices[oir.OrderItem.CatalogueItemId],
                    OrderType = (int)oir.OrderItem.OrderItemPrice.ProvisioningType,
                    M1Planned = oir.DeliveryDate,
                    FundingType = fundingTypeService.GetFundingType(fundingTypes, oir.OrderItem.FundingType).Description(),
                    Framework = oir.OrderItem.Order.SelectedFrameworkId,
                    InitialTerm = oir.OrderItem.Order.InitialPeriod,
                    MaximumTerm = oir.OrderItem.Order.MaximumTerm,
                    CeaseDate = oir.OrderItem.Order.IsTerminated ? oir.OrderItem.Order.OrderTermination.DateOfTermination : null,
                })
                .OrderBy(o => o.ProductTypeId)
                .ThenBy(o => o.ProductName)
                .ThenBy(o => o.ServiceRecipientName)
                .ToListAsync();

            for (int i = 0; i < items.Count; i++)
                items[i].ServiceRecipientItemId = $"{items[i].CallOffId}-{items[i].ServiceRecipientId}-{i}";

            return items;
        }
    }
}

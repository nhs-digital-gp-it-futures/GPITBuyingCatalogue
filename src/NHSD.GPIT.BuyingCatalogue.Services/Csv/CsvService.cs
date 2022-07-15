using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    public class CsvService : ICsvService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IFrameworkService frameworkService;

        public CsvService(BuyingCatalogueDbContext dbContext, IFrameworkService frameworkService)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
        }

        public async Task CreateFullOrderCsvAsync(int orderId, MemoryStream stream)
        {
            var billingPeriods = await GetBillingPeriods(orderId);
            var framework = await frameworkService.GetFramework(orderId);
            var frameworkId = framework?.Id ?? string.Empty;
            var fundingType = await GetFundingType(orderId);
            var prices = await GetPrices(orderId);

            var items = await dbContext.OrderItemRecipients
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
                    SupplierId = $"{oir.OrderItem.Order.SupplierId ?? 0}",
                    SupplierName = oir.OrderItem.Order.Supplier.Name,
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
                    M1Planned = oir.OrderItem.Order.CommencementDate,
                    FundingType = fundingType,
                    Framework = frameworkId,
                    InitialTerm = oir.OrderItem.Order.InitialPeriod,
                    MaximumTerm = oir.OrderItem.Order.MaximumTerm,
                })
                .OrderBy(o => o.ProductTypeId).ThenBy(o => o.ServiceRecipientName)
                .ToListAsync();

            for (int i = 0; i < items.Count; i++)
                items[i].ServiceRecipientItemId = $"{items[i].CallOffId}-{items[i].ServiceRecipientId}-{i}";

            await WriteRecordsAsync<FullOrderCsvModel, FullOrderCsvModelMap>(stream, items);
        }

        public async Task<int> CreatePatientNumberCsvAsync(int orderId, MemoryStream stream)
        {
            var framework = await frameworkService.GetFramework(orderId);
            var frameworkId = framework?.Id ?? string.Empty;
            var fundingType = await GetFundingType(orderId);
            var prices = await GetPrices(orderId);

            var items = await dbContext.OrderItemRecipients
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
                    SupplierId = oir.OrderItem.Order.SupplierId.Value,
                    SupplierName = oir.OrderItem.Order.Supplier.Name,
                    ProductId = oir.OrderItem.CatalogueItemId.ToString(),
                    ProductName = oir.OrderItem.CatalogueItem.Name,
                    ProductType = oir.OrderItem.CatalogueItem.CatalogueItemType.DisplayName(),
                    ProductTypeId = (int)oir.OrderItem.CatalogueItem.CatalogueItemType,

                    // TODO: Stop this reporting incorrectly when quantity is (erroneously) defined at both order item & recipient level
                    QuantityOrdered = oir.Quantity ?? oir.OrderItem.Quantity ?? 0,
                    UnitOfOrder = oir.OrderItem.OrderItemPrice.Description,
                    Price = prices[oir.OrderItem.CatalogueItemId],
                    FundingType = fundingType,
                    M1Planned = oir.OrderItem.Order.CommencementDate,
                    Framework = frameworkId,
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

        private static async Task WriteRecordsAsync<TEntity, TClassMap>(MemoryStream stream, IEnumerable<TEntity> items)
            where TClassMap : ClassMap<TEntity>
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
            using var csvWriter = new CsvWriter(writer, CultureInfo.CurrentCulture);

            csvWriter.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(new TypeConverterOptions { Formats = new[] { "dd/MM/yyyy" } });

            csvWriter.Context.RegisterClassMap<TClassMap>();

            await csvWriter.WriteRecordsAsync(items);
        }

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

        private async Task<string> GetFundingType(int orderId)
        {
            var order = await dbContext.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.OrderItemFunding)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == orderId);

            return order?.ApproximateFundingType;
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
    }
}

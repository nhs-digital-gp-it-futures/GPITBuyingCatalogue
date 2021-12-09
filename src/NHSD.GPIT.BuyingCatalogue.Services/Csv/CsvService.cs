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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    public class CsvService : ICsvService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public CsvService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task CreateFullOrderCsvAsync(int orderId, MemoryStream stream)
        {
            var items = await dbContext.OrderItemRecipients.AsNoTracking()
                .Where(oir => oir.OrderId == orderId)
                .Select(oir => new FullOrderCsvModel
                {
                    CallOffId = oir.OrderItem.Order.CallOffId,
                    OdsCode = oir.OrderItem.Order.OrderingParty.OdsCode,
                    OrganisationName = oir.OrderItem.Order.OrderingParty.Name,
                    CommencementDate = oir.OrderItem.Order.CommencementDate,
                    ServiceRecipientId = oir.Recipient.OdsCode,
                    ServiceRecipientName = oir.Recipient.Name,
                    SupplierId = oir.OrderItem.Order.SupplierId.Value.ToString(CultureInfo.InvariantCulture),
                    SupplierName = oir.OrderItem.Order.Supplier.Name,
                    ProductId = oir.OrderItem.CatalogueItemId.ToString(),
                    ProductName = oir.OrderItem.CatalogueItem.Name,
                    ProductType = oir.OrderItem.CatalogueItem.CatalogueItemType.DisplayName(),
                    QuantityOrdered = oir.Quantity,
                    UnitOfOrder = oir.OrderItem.CataloguePrice.PricingUnit.Description,
                    UnitTime = !oir.OrderItem.CataloguePrice.TimeUnit.HasValue ? string.Empty : oir.OrderItem.CataloguePrice.TimeUnit.Value.Description(),
                    EstimationPeriod = !oir.OrderItem.EstimationPeriod.HasValue ? string.Empty : oir.OrderItem.EstimationPeriod.Value.Description(),
                    Price = oir.OrderItem.Price.GetValueOrDefault(),
                    OrderType = (int)oir.OrderItem.CataloguePrice.ProvisioningType,
                    M1Planned = oir.DeliveryDate,
                    Framework =
                    oir.OrderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution
                    ? oir.OrderItem.CatalogueItem.Solution.FrameworkSolutions.FirstOrDefault().FrameworkId
                    : oir.OrderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                        ? oir.OrderItem.CatalogueItem.AdditionalService.Solution.FrameworkSolutions.FirstOrDefault().FrameworkId
                        : oir.OrderItem.CatalogueItem.AssociatedService.CatalogueItem.Solution.FrameworkSolutions.FirstOrDefault().FrameworkId,
                    FundingType = oir.OrderItem.Order.FundingSourceOnlyGms.Value ? "Central" : "Local",
                }).ToListAsync();

            for (int i = 0; i < items.Count; i++)
                items[i].ServiceRecipientItemId = $"{items[i].CallOffId}-{items[i].ServiceRecipientId}-{i + 1}";

            await WriteRecordsAsync<FullOrderCsvModel, FullOrderCsvModelMap>(stream, items);
        }

        public async Task<int> CreatePatientNumberCsvAsync(int orderId, MemoryStream stream)
        {
            var items = await dbContext.OrderItemRecipients.AsNoTracking()
                .Where(oir => oir.OrderId == orderId && oir.OrderItem.CataloguePrice.ProvisioningType == ProvisioningType.Patient)
                .Select(oir => new PatientOrderCsvModel
                {
                    CallOffId = oir.OrderItem.Order.CallOffId,
                    OdsCode = oir.OrderItem.Order.OrderingParty.OdsCode,
                    OrganisationName = oir.OrderItem.Order.OrderingParty.Name,
                    CommencementDate = oir.OrderItem.Order.CommencementDate,
                    ServiceRecipientId = oir.Recipient.OdsCode,
                    ServiceRecipientName = oir.Recipient.Name,
                    SupplierId = oir.OrderItem.Order.SupplierId.Value,
                    SupplierName = oir.OrderItem.Order.Supplier.Name,
                    ProductId = oir.OrderItem.CatalogueItemId.ToString(),
                    ProductName = oir.OrderItem.CatalogueItem.Name,
                    ProductType = oir.OrderItem.CatalogueItem.CatalogueItemType.DisplayName(),
                    QuantityOrdered = oir.Quantity,
                    UnitOfOrder = oir.OrderItem.CataloguePrice.PricingUnit.Description,
                    Price = oir.OrderItem.Price.GetValueOrDefault(),
                    M1Planned = oir.DeliveryDate,
                    Framework =
                    oir.OrderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution
                    ? oir.OrderItem.CatalogueItem.Solution.FrameworkSolutions.FirstOrDefault().FrameworkId
                    : oir.OrderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                        ? oir.OrderItem.CatalogueItem.AdditionalService.Solution.FrameworkSolutions.FirstOrDefault().FrameworkId
                        : oir.OrderItem.CatalogueItem.AssociatedService.CatalogueItem.Solution.FrameworkSolutions.FirstOrDefault().FrameworkId,
                    FundingType = oir.OrderItem.Order.FundingSourceOnlyGms.Value ? "Central" : "Local",
                }).ToListAsync();

            if (items.Count == 0)
                return 0;

            for (int i = 0; i < items.Count; i++)
                items[i].ServiceRecipientItemId = $"{items[i].CallOffId}-{items[i].ServiceRecipientId}-{i + 1}";

            await WriteRecordsAsync<PatientOrderCsvModel, PatientOrderCsvModelMap>(stream, items);

            return items.Count;
        }

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
    }
}

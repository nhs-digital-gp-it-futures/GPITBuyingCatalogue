using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    public class CsvService : ICsvService
    {
        public async Task CreateFullOrderCsvAsync(Order order, MemoryStream stream)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var items = new List<FullOrderCsvModel>();

            int itemId = 1;

            foreach (var orderItem in order.OrderItems)
            {
                foreach (var recipient in orderItem.OrderItemRecipients)
                {
                    var orderCsvModel = new FullOrderCsvModel
                    {
                        CallOffId = order.CallOffId,
                        OdsCode = order.OrderingParty.OdsCode,
                        OrganisationName = order.OrderingParty.Name,
                        CommencementDate = order.CommencementDate,
                        ServiceRecipientId = recipient.Recipient.OdsCode,
                        ServiceRecipientName = recipient.Recipient.Name,
                        ServiceRecipientItemId = $"{order.CallOffId}-{recipient.Recipient.OdsCode}-{itemId}",
                        SupplierId = order.Supplier.Id,
                        SupplierName = order.Supplier.Name,
                        ProductId = orderItem.CatalogueItemId.ToString(),
                        ProductName = orderItem.CatalogueItem.Name,
                        ProductType = orderItem.CatalogueItem.CatalogueItemType.DisplayName(),
                        QuantityOrdered = recipient.Quantity,
                        UnitOfOrder = orderItem.CataloguePrice.PricingUnit.Description,
                        UnitTime = orderItem.CataloguePrice.ProvisioningType == ProvisioningType.OnDemand
                        ? null
                        : orderItem.CataloguePrice.TimeUnit?.Description(),
                        EstimationPeriod = orderItem.CataloguePrice.ProvisioningType == ProvisioningType.Declarative
                        ? null
                        : orderItem.EstimationPeriod?.Description(),
                        Price = orderItem.Price.GetValueOrDefault(),
                        OrderType = (int)orderItem.CataloguePrice.ProvisioningType,
                        M1Planned = recipient.DeliveryDate,
                    };

                    items.Add(orderCsvModel);
                    itemId++;
                }
            }

            await WriteRecordsAsync<FullOrderCsvModel, FullOrderCsvModelMap>(stream, items);
        }

        public async Task CreatePatientNumberCsvAsync(Order order, MemoryStream stream)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var items = new List<PatientOrderCsvModel>();

            int itemId = 1;

            foreach (var orderItem in order.OrderItems)
            {
                foreach (var recipient in orderItem.OrderItemRecipients)
                {
                    var orderCsvModel = new PatientOrderCsvModel
                    {
                        CallOffId = order.CallOffId,
                        OdsCode = order.OrderingParty.OdsCode,
                        OrganisationName = order.OrderingParty.Name,
                        CommencementDate = order.CommencementDate.Value,
                        ServiceRecipientId = recipient.Recipient.OdsCode,
                        ServiceRecipientName = recipient.Recipient.Name,
                        ServiceRecipientItemId = $"{order.CallOffId}-{recipient.Recipient.OdsCode}-{itemId}",
                        SupplierId = order.Supplier.Id,
                        SupplierName = order.Supplier.Name,
                        ProductId = orderItem.CatalogueItemId.ToString(),
                        ProductName = orderItem.CatalogueItem.Name,
                        ProductType = orderItem.CatalogueItem.CatalogueItemType.DisplayName(),
                        QuantityOrdered = recipient.Quantity,
                        UnitOfOrder = orderItem.CataloguePrice.PricingUnit.Description,
                        Price = orderItem.Price.GetValueOrDefault(),
                        M1Planned = recipient.DeliveryDate.Value,
                    };

                    items.Add(orderCsvModel);
                    itemId++;
                }
            }

            await WriteRecordsAsync<PatientOrderCsvModel, PatientOrderCsvModelMap>(stream, items);
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

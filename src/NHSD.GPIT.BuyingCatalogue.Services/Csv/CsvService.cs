﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.FundingTypes;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv
{
    public class CsvService : CsvServiceBase, ICsvService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IFundingTypeService fundingTypeService;
        private readonly ISupplierTemporalService supplierService;

        public CsvService(
            BuyingCatalogueDbContext dbContext,
            IFundingTypeService fundingTypeService,
            ISupplierTemporalService supplierService)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.fundingTypeService = fundingTypeService ?? throw new ArgumentNullException(nameof(fundingTypeService));
            this.supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
        }

        public async Task CreateFullOrderCsvAsync(
            int orderId,
            OrderType orderType,
            MemoryStream stream,
            bool showRevisions = false)
        {
            ArgumentNullException.ThrowIfNull(orderType);

            switch (orderType.Value)
            {
                case OrderTypeEnum.AssociatedServiceMerger:
                {
                    await WriteMergerCsv(orderId, stream);
                    break;
                }

                case OrderTypeEnum.AssociatedServiceSplit:
                {
                    await WriteSplitCsv(orderId, stream);
                    break;
                }

                default:
                {
                    await WriteDefaultOrderCsv(orderId, stream, showRevisions);
                    break;
                }
            }
        }

        private static string TimeUnitDescription(TimeUnit? timeUnit) => timeUnit?.Description() ?? string.Empty;

        private static string GetTieredArray(ICollection<OrderItemPriceTier> orderItemPriceTiers)
        {
            return $"[{string.Join(";", orderItemPriceTiers.Select(item => $"[{item.LowerRange}:{item.Price}]"))}]";
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

        private async Task<List<OrderItemFundingType>> GetFundingTypes(int orderId)
        {
            return await dbContext.OrderItems
                .Include(x => x.OrderItemFunding)
                .AsNoTracking()
                .Where(x => x.OrderId == orderId && x.OrderItemFunding != null)
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

            var output = order.Completed.HasValue && order.SupplierId.HasValue
                ? await supplierService.GetSupplierByDate(order.SupplierId.Value, order.Completed.Value)
                : order.Supplier;

            output ??= order.Supplier;

            var name = output?.Name ?? string.Empty;
            var legalName = output?.LegalName ?? string.Empty;

            return (output?.Id ?? 0,
                string.Equals(name, legalName, StringComparison.OrdinalIgnoreCase) ? name : legalName);
        }

        private async Task WriteMergerCsv(int orderId, MemoryStream stream)
        {
            var items = await GetModelListForMergerCsv(orderId);
            var map = new MergerOrderCsvModelMap(FullOrderCsvModelMap.Names);
            await WriteRecordsAsync(map, stream, items);
        }

        private async Task WriteSplitCsv(int orderId, MemoryStream stream)
        {
            var items = await GetModelListFormSplitCsv(orderId);
            var map = new SplitOrderCsvModelMap(FullOrderCsvModelMap.Names);
            await WriteRecordsAsync(map, stream, items);
        }

        private async Task WriteDefaultOrderCsv(int orderId, MemoryStream stream, bool showRevisions)
        {
            var items = await GetModelListForCsv(orderId);

            if (showRevisions)
            {
                var order = await dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == orderId);

                var revisions = await dbContext.Orders
                    .AsNoTracking()
                    .Where(x => x.OrderNumber == order.OrderNumber && x.Revision < order.Revision)
                    .OrderByDescending(x => x.Revision)
                    .Select(y => y.Id)
                    .ToListAsync();

                foreach (var id in revisions)
                    items.AddRange(await GetModelListForCsv(id));
            }

            await WriteRecordsAsync<FullOrderCsvModel, FullOrderCsvModelMap>(stream, items);
        }

        private async Task<List<FullOrderCsvModel>> GetModelListForCsv(int orderId)
        {
            var billingPeriods = await GetBillingPeriods(orderId);
            var fundingTypes = await GetFundingTypes(orderId);
            var prices = await GetPrices(orderId);
            var (supplierId, supplierName) = await GetSupplierDetails(orderId);

            var items = await dbContext.OrderRecipients
                .Include(x => x.OrderItemRecipients)
                .ThenInclude(x => x.OrderItem)
                .ThenInclude(x => x.OrderItemFunding)
                .AsNoTracking()
                .Where(or => or.OrderId == orderId)
                .SelectMany(
                    or => or.OrderItemRecipients,
                    (or, oir) => new FullOrderCsvModel
                    {
                        CallOffId = or.Order.CallOffId,
                        OdsCode = or.Order.OrderingParty.ExternalIdentifier,
                        OrganisationName = or.Order.OrderingParty.Name,
                        CommencementDate = or.Order.CommencementDate,
                        ServiceRecipientId =
                            !(oir.OrderItem.OrderItemPrice as IPrice).IsPerServiceRecipient()
                                ? or.Order.OrderingParty.ExternalIdentifier
                                : or.OdsCode,
                        ServiceRecipientName =
                            !(oir.OrderItem.OrderItemPrice as IPrice).IsPerServiceRecipient()
                                ? or.Order.OrderingParty.Name
                                : or.OdsOrganisation.Name,
                        SupplierId = $"{supplierId}",
                        SupplierName = supplierName,
                        ProductId = oir.OrderItem.CatalogueItemId.ToString(),
                        ProductName = oir.OrderItem.CatalogueItem.Name,
                        ProductType = oir.OrderItem.CatalogueItem.CatalogueItemType.DisplayName(),
                        ProductTypeId = (int)oir.OrderItem.CatalogueItem.CatalogueItemType,
                        QuantityOrdered =
                            or.OrderItemRecipients.FirstOrDefault(
                                x => x.CatalogueItemId == oir.OrderItem.CatalogueItemId) == null
                                ? (oir.OrderItem.Quantity ?? 0)
                                : or.OrderItemRecipients
                                    .FirstOrDefault(x => x.CatalogueItemId == oir.OrderItem.CatalogueItemId)
                                    .Quantity ?? oir.OrderItem.Quantity ?? 0,
                        UnitOfOrder = oir.OrderItem.OrderItemPrice.Description,
                        UnitTime = TimeUnitDescription(billingPeriods[oir.OrderItem.CatalogueItemId]),
                        EstimationPeriod = TimeUnitDescription(oir.OrderItem.EstimationPeriod),
                        Price = prices[oir.OrderItem.CatalogueItemId],
                        OrderType = (int)oir.OrderItem.OrderItemPrice.ProvisioningType,
                        M1Planned = or.OrderItemRecipients.FirstOrDefault(
                            x => x.CatalogueItemId == oir.OrderItem.CatalogueItemId) == null
                            ? null
                            : or.OrderItemRecipients
                                .FirstOrDefault(x => x.CatalogueItemId == oir.OrderItem.CatalogueItemId)
                                .DeliveryDate,
                        FundingType =
                            fundingTypeService.GetFundingType(fundingTypes, oir.OrderItem.FundingType).Description(),
                        Framework = or.Order.SelectedFrameworkId,
                        InitialTerm = or.Order.InitialPeriod,
                        MaximumTerm = or.Order.MaximumTerm,
                        CeaseDate = or.Order.IsTerminated ? or.Order.OrderTermination.DateOfTermination : null,
                        PricingType =
                            oir.OrderItem.OrderItemPrice.CataloguePriceType == CataloguePriceType.Tiered
                                ? $"{oir.OrderItem.OrderItemPrice.CataloguePriceType} {oir.OrderItem.OrderItemPrice.CataloguePriceCalculationType}"
                                : $"{oir.OrderItem.OrderItemPrice.CataloguePriceType}",
                        TieredArray =
                            oir.OrderItem.OrderItemPrice.CataloguePriceType == CataloguePriceType.Tiered
                            && oir.OrderItem.OrderItemPrice.CataloguePriceCalculationType
                            == CataloguePriceCalculationType.Cumulative
                                ? GetTieredArray(oir.OrderItem.OrderItemPrice.OrderItemPriceTiers)
                                : string.Empty,
                    })
                .ToListAsync();

            var distinctItems = items.DistinctBy(item => new { item.ServiceRecipientId, item.ProductId, })
                .OrderBy(o => o.ProductTypeId)
                .ThenBy(o => o.ProductName)
                .ThenBy(o => o.ServiceRecipientName)
                .ToList();

            for (int i = 0; i < distinctItems.Count; i++)
            {
                distinctItems[i].ServiceRecipientItemId =
                    $"{distinctItems[i].CallOffId}-{distinctItems[i].ServiceRecipientId}-{i}";
            }

            return distinctItems;
        }

        private async Task<List<MergerOrderCsvModel>> GetModelListForMergerCsv(int orderId)
        {
            var billingPeriods = await GetBillingPeriods(orderId);
            var fundingTypes = await GetFundingTypes(orderId);
            var prices = await GetPrices(orderId);
            var (supplierId, supplierName) = await GetSupplierDetails(orderId);

            var items = await dbContext.OrderRecipients
                .Include(x => x.OrderItemRecipients)
                .ThenInclude(x => x.OrderItem)
                .ThenInclude(x => x.OrderItemFunding)
                .AsNoTracking()
                .Where(or => or.OrderId == orderId)
                .SelectMany(
                    or => or.OrderItemRecipients,
                    (or, oir) => new MergerOrderCsvModel
                    {
                        CallOffId = or.Order.CallOffId,
                        OdsCode = or.Order.OrderingParty.ExternalIdentifier,
                        OrganisationName = or.Order.OrderingParty.Name,
                        CommencementDate = or.Order.CommencementDate,
                        ServiceRecipientId = or.OdsCode,
                        ServiceRecipientName = or.OdsOrganisation.Name,
                        ServiceRecipientToClose = $"{or.OdsOrganisation.Name} ({or.OdsCode})",
                        ServiceRecipientToRetain =
                            $"{or.Order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Name} ({or.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode})",
                        SupplierId = $"{supplierId}",
                        SupplierName = supplierName,
                        ProductId = oir.OrderItem.CatalogueItemId.ToString(),
                        ProductName = oir.OrderItem.CatalogueItem.Name,
                        ProductType = oir.OrderItem.CatalogueItem.CatalogueItemType.DisplayName(),
                        ProductTypeId = (int)oir.OrderItem.CatalogueItem.CatalogueItemType,
                        QuantityOrdered =
                            or.OrderItemRecipients.FirstOrDefault(
                                x => x.CatalogueItemId == oir.OrderItem.CatalogueItemId) == null
                                ? (oir.OrderItem.Quantity ?? 0)
                                : or.OrderItemRecipients
                                    .FirstOrDefault(x => x.CatalogueItemId == oir.OrderItem.CatalogueItemId)
                                    .Quantity ?? oir.OrderItem.Quantity ?? 0,
                        UnitOfOrder = oir.OrderItem.OrderItemPrice.Description,
                        UnitTime = TimeUnitDescription(billingPeriods[oir.OrderItem.CatalogueItemId]),
                        EstimationPeriod = TimeUnitDescription(oir.OrderItem.EstimationPeriod),
                        Price = prices[oir.OrderItem.CatalogueItemId],
                        OrderType = (int)oir.OrderItem.OrderItemPrice.ProvisioningType,
                        M1Planned = or.OrderItemRecipients.FirstOrDefault(
                            x => x.CatalogueItemId == oir.OrderItem.CatalogueItemId) == null
                            ? null
                            : or.OrderItemRecipients
                                .FirstOrDefault(x => x.CatalogueItemId == oir.OrderItem.CatalogueItemId)
                                .DeliveryDate,
                        FundingType =
                            fundingTypeService.GetFundingType(fundingTypes, oir.OrderItem.FundingType).Description(),
                        Framework = or.Order.SelectedFrameworkId,
                        InitialTerm = or.Order.InitialPeriod,
                        MaximumTerm = or.Order.MaximumTerm,
                        CeaseDate = or.Order.IsTerminated ? or.Order.OrderTermination.DateOfTermination : null,
                        PricingType =
                            oir.OrderItem.OrderItemPrice.CataloguePriceType == CataloguePriceType.Tiered
                                ? $"{oir.OrderItem.OrderItemPrice.CataloguePriceType} {oir.OrderItem.OrderItemPrice.CataloguePriceCalculationType}"
                                : $"{oir.OrderItem.OrderItemPrice.CataloguePriceType}",
                        TieredArray =
                            oir.OrderItem.OrderItemPrice.CataloguePriceType == CataloguePriceType.Tiered
                            && oir.OrderItem.OrderItemPrice.CataloguePriceCalculationType
                            == CataloguePriceCalculationType.Cumulative
                                ? GetTieredArray(oir.OrderItem.OrderItemPrice.OrderItemPriceTiers)
                                : string.Empty,
                    })
                .ToListAsync();

            var ordered = items.OrderBy(o => o.ProductTypeId)
                .ThenBy(o => o.ProductName)
                .ThenBy(o => o.ServiceRecipientName)
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
                ordered[i].ServiceRecipientItemId = $"{ordered[i].CallOffId}-{ordered[i].ServiceRecipientId}-{i}";

            return ordered;
        }

        private async Task<List<SplitOrderCsvModel>> GetModelListFormSplitCsv(int orderId)
        {
            var billingPeriods = await GetBillingPeriods(orderId);
            var fundingTypes = await GetFundingTypes(orderId);
            var prices = await GetPrices(orderId);
            var (supplierId, supplierName) = await GetSupplierDetails(orderId);

            var items = await dbContext.OrderRecipients
                .Include(x => x.OrderItemRecipients)
                .ThenInclude(x => x.OrderItem)
                .ThenInclude(x => x.OrderItemFunding)
                .AsNoTracking()
                .Where(or => or.OrderId == orderId)
                .SelectMany(
                    or => or.OrderItemRecipients,
                    (or, oir) => new SplitOrderCsvModel
                    {
                        CallOffId = or.Order.CallOffId,
                        OdsCode = or.Order.OrderingParty.ExternalIdentifier,
                        OrganisationName = or.Order.OrderingParty.Name,
                        CommencementDate = or.Order.CommencementDate,
                        ServiceRecipientId = or.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode,
                        ServiceRecipientName = or.OdsOrganisation.Name,
                        ServiceRecipientToRetain = $"{or.OdsOrganisation.Name} ({or.OdsCode})",
                        ServiceRecipientToSplit =
                            $"{or.Order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient.Name} ({or.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode})",
                        SupplierId = $"{supplierId}",
                        SupplierName = supplierName,
                        ProductId = oir.OrderItem.CatalogueItemId.ToString(),
                        ProductName = oir.OrderItem.CatalogueItem.Name,
                        ProductType = oir.OrderItem.CatalogueItem.CatalogueItemType.DisplayName(),
                        ProductTypeId = (int)oir.OrderItem.CatalogueItem.CatalogueItemType,
                        QuantityOrdered =
                            or.OrderItemRecipients.FirstOrDefault(
                                x => x.CatalogueItemId == oir.OrderItem.CatalogueItemId) == null
                                ? (oir.OrderItem.Quantity ?? 0)
                                : or.OrderItemRecipients
                                    .FirstOrDefault(x => x.CatalogueItemId == oir.OrderItem.CatalogueItemId)
                                    .Quantity ?? oir.OrderItem.Quantity ?? 0,
                        UnitOfOrder = oir.OrderItem.OrderItemPrice.Description,
                        UnitTime = TimeUnitDescription(billingPeriods[oir.OrderItem.CatalogueItemId]),
                        EstimationPeriod = TimeUnitDescription(oir.OrderItem.EstimationPeriod),
                        Price = prices[oir.OrderItem.CatalogueItemId],
                        OrderType = (int)oir.OrderItem.OrderItemPrice.ProvisioningType,
                        M1Planned = or.OrderItemRecipients.FirstOrDefault(
                            x => x.CatalogueItemId == oir.OrderItem.CatalogueItemId) == null
                            ? null
                            : or.OrderItemRecipients
                                .FirstOrDefault(x => x.CatalogueItemId == oir.OrderItem.CatalogueItemId)
                                .DeliveryDate,
                        FundingType =
                            fundingTypeService.GetFundingType(fundingTypes, oir.OrderItem.FundingType).Description(),
                        Framework = or.Order.SelectedFrameworkId,
                        InitialTerm = or.Order.InitialPeriod,
                        MaximumTerm = or.Order.MaximumTerm,
                        CeaseDate = or.Order.IsTerminated ? or.Order.OrderTermination.DateOfTermination : null,
                        PricingType =
                            oir.OrderItem.OrderItemPrice.CataloguePriceType == CataloguePriceType.Tiered
                                ? $"{oir.OrderItem.OrderItemPrice.CataloguePriceType} {oir.OrderItem.OrderItemPrice.CataloguePriceCalculationType}"
                                : $"{oir.OrderItem.OrderItemPrice.CataloguePriceType}",
                        TieredArray =
                            oir.OrderItem.OrderItemPrice.CataloguePriceType == CataloguePriceType.Tiered
                            && oir.OrderItem.OrderItemPrice.CataloguePriceCalculationType
                            == CataloguePriceCalculationType.Cumulative
                                ? GetTieredArray(oir.OrderItem.OrderItemPrice.OrderItemPriceTiers)
                                : string.Empty,
                    })
                .ToListAsync();

            var ordered = items
                .OrderBy(o => o.ServiceRecipientName)
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
                ordered[i].ServiceRecipientItemId = $"{ordered[i].CallOffId}-{ordered[i].ServiceRecipientId}-{i}";

            return ordered;
        }
    }
}

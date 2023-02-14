﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using Notify.Client;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderService : IOrderService
    {
        public const string OrganisationNameToken = "organisation_name";
        public const string FullOrderCsvToken = "full_order_csv";
        public const string OrderIdToken = "order_id";
        public const string OrderSummaryLinkToken = "order_summary_link";
        public const string OrderSummaryCsv = "order_summary_csv";
        public const string PatientOrderCsvToken = "patient_order_csv";

        private readonly BuyingCatalogueDbContext dbContext;
        private readonly ICsvService csvService;
        private readonly IGovNotifyEmailService emailService;
        private readonly IOrderPdfService pdfService;
        private readonly OrderMessageSettings orderMessageSettings;

        public OrderService(
            BuyingCatalogueDbContext dbContext,
            ICsvService csvService,
            IGovNotifyEmailService emailService,
            IOrderPdfService pdfService,
            OrderMessageSettings orderMessageSettings)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            this.orderMessageSettings = orderMessageSettings ?? throw new ArgumentNullException(nameof(orderMessageSettings));
        }

        public async Task<int> GetOrderId(CallOffId callOffId)
        {
            return await dbContext.OrderId(callOffId);
        }

        public async Task<int> GetOrderId(string internalOrgId, CallOffId callOffId)
        {
            return await dbContext.OrderId(internalOrgId, callOffId);
        }

        public async Task<bool> HasSubsequentRevisions(CallOffId callOffId)
        {
            return await dbContext.Orders
                .AnyAsync(x => x.OrderNumber == callOffId.OrderNumber && x.Revision > callOffId.Revision);
        }

        public async Task<OrderWrapper> GetOrderThin(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Supplier)
                .Include(o => o.LastUpdatedByUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                .Include(o => o.SelectedFramework)
                .AsSplitQuery()
                .AsNoTracking()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            return new OrderWrapper(orders);
        }

        public async Task<OrderWrapper> GetOrderWithCatalogueItemAndPrices(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(x => x.Solution)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                    .ThenInclude(x => x.CataloguePrices.Where(p => p.PublishedStatus == PublicationStatus.Published))
                    .ThenInclude(x => x.CataloguePriceTiers)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemRecipients)
                    .ThenInclude(r => r.Recipient)
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            return new OrderWrapper(orders);
        }

        public async Task<OrderWrapper> GetOrderWithOrderItems(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(x => x.OrderingParty)
                .Include(x => x.Solution)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemFunding)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers.OrderBy(t => t.LowerRange))
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemRecipients.OrderBy(oir => oir.Recipient.Name))
                    .ThenInclude(r => r.Recipient)
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            return new OrderWrapper(orders);
        }

        public async Task<OrderWrapper> GetOrderWithOrderItemsForFunding(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemFunding)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemRecipients)
                    .ThenInclude(r => r.Recipient)
                .Include(o => o.SelectedFramework)
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            return new OrderWrapper(orders);
        }

        public async Task<OrderWrapper> GetOrderWithSupplier(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            return new OrderWrapper(orders);
        }

        public async Task<OrderWrapper> GetOrderForSummary(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(x => x.ContractFlags)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Solution)
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .Include(o => o.LastUpdatedByUser)
                .Include(o => o.SelectedFramework)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                    .ThenInclude(ci => ci.Solution)
                    .ThenInclude(s => s.FrameworkSolutions)
                    .ThenInclude(fs => fs.Framework)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemFunding)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers.OrderBy(pt => pt.LowerRange))
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemRecipients.OrderBy(r => r.Recipient.Name))
                    .ThenInclude(r => r.Recipient)
                .AsNoTracking()
                .AsSplitQuery()
                .Where(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision <= callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            foreach (var order in orders)
            {
                var supplier = order.Completed.HasValue
                    ? await dbContext.Suppliers.TemporalAsOf(order.Completed.Value).FirstOrDefaultAsync(x => x.Id == order.SupplierId)
                    : null;

                if (supplier != null)
                {
                    order.Supplier = supplier;
                }
            }

            return new OrderWrapper(orders);
        }

        public async Task<OrderWrapper> GetOrderForTaskListStatuses(CallOffId callOffId, string internalOrgId)
        {
            var orders = await dbContext.Orders
                .Include(x => x.ContractFlags)
                .Include(x => x.LastUpdatedByUser)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.CatalogueItem)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.OrderItemFunding)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.OrderItemPrice)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.OrderItemRecipients)
                .Include(x => x.OrderingPartyContact)
                .Include(x => x.OrderingParty)
                .Include(x => x.Supplier)
                .Include(x => x.SupplierContact)
                .Include(x => x.SelectedFramework)
                .AsSplitQuery()
                .AsNoTracking()
                .Where(x => x.OrderNumber == callOffId.OrderNumber
                    && x.Revision <= callOffId.Revision
                    && x.OrderingParty.InternalIdentifier == internalOrgId)
                .ToListAsync();

            return new OrderWrapper(orders);
        }

        public async Task<(PagedList<Order> Orders, IEnumerable<CallOffId> OrderIds)> GetPagedOrders(int organisationId, PageOptions options, string search = null)
        {
            options ??= new PageOptions();

            var query = await dbContext.Orders
                .Include(o => o.LastUpdatedByUser)
                .Where(o => o.OrderingPartyId == organisationId)
                .OrderByDescending(o => o.LastUpdated)
                .AsNoTracking()
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o =>
                    o.CallOffId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                    || o.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            options.TotalNumberOfItems = query.Count;
            var orderIds = query.Select(x => x.CallOffId);

            if (options.PageNumber != 0)
                query = query.Skip((options.PageNumber - 1) * options.PageSize).ToList();

            var results = query.Take(options.PageSize).ToList();

            return (new PagedList<Order>(results, options), orderIds);
        }

        public async Task<IList<SearchFilterModel>> GetOrdersBySearchTerm(int organisationId, string searchTerm)
        {
            var baseData = await dbContext
                .Orders
                .Where(o => o.OrderingPartyId == organisationId)
                .AsNoTracking()
                .ToListAsync();

            var matches = baseData
                .Where(o => o.CallOffId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                         || o.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(o => new SearchFilterModel
                {
                    Title = o.Description,
                    Category = o.CallOffId.ToString(),
                });

            return matches.ToList();
        }

        public async Task<Order> CreateOrder(string description, string internalOrgId, OrderTriageValue? orderTriageValue, bool isAssociatedServiceOnly)
        {
            var orderingParty = await dbContext.Organisations.FirstAsync(o => o.InternalIdentifier == internalOrgId);

            var order = new Order
            {
                OrderNumber = await dbContext.NextOrderNumber(),
                Revision = 1,
                Description = description,
                OrderingParty = orderingParty,
                AssociatedServicesOnly = isAssociatedServiceOnly,
                OrderTriageValue = orderTriageValue,
            };

            dbContext.Add(order);

            await dbContext.SaveChangesAsync();

            return order;
        }

        public async Task<Order> AmendOrder(string internalOrgId, CallOffId callOffId)
        {
            var order = (await GetOrderForSummary(callOffId, internalOrgId)).Order;

            var amendment = new Order
            {
                OrderNumber = order.OrderNumber,
                Revision = await dbContext.NextRevision(order.OrderNumber),
                AssociatedServicesOnly = order.AssociatedServicesOnly,
                CommencementDate = order.CommencementDate,
                Description = order.Description,
                InitialPeriod = order.InitialPeriod,
                MaximumTerm = order.MaximumTerm,
                OrderingPartyId = order.OrderingPartyId,
                OrderingPartyContact = order.OrderingPartyContact.Clone(),
                OrderTriageValue = order.OrderTriageValue,
                SelectedFrameworkId = order.SelectedFrameworkId,
                SupplierId = order.SupplierId,
                SupplierContact = order.SupplierContact.Clone(),
            };

            dbContext.Add(amendment);

            await dbContext.SaveChangesAsync();

            return amendment;
        }

        public async Task SoftDeleteOrder(CallOffId callOffId, string internalOrgId)
        {
            var order = await dbContext.Order(internalOrgId, callOffId);

            order.IsDeleted = true;

            await dbContext.SaveChangesAsync();
        }

        public async Task HardDeleteOrder(CallOffId callOffId, string internalOrgId)
        {
            var order = await dbContext.Order(internalOrgId, callOffId);

            dbContext.ContractFlags.RemoveRange(dbContext.ContractFlags.Where(x => x.OrderId == order.Id));
            dbContext.OrderDeletionApprovals.RemoveRange(dbContext.OrderDeletionApprovals.Where(x => x.OrderId == order.Id));
            dbContext.OrderItems.RemoveRange(dbContext.OrderItems.Where(x => x.OrderId == order.Id));
            dbContext.OrderItemFunding.RemoveRange(dbContext.OrderItemFunding.Where(x => x.OrderId == order.Id));
            dbContext.OrderItemPriceTiers.RemoveRange(dbContext.OrderItemPriceTiers.Where(x => x.OrderId == order.Id));
            dbContext.OrderItemPrices.RemoveRange(dbContext.OrderItemPrices.Where(x => x.OrderId == order.Id));
            dbContext.OrderItemRecipients.RemoveRange(dbContext.OrderItemRecipients.Where(x => x.OrderId == order.Id));
            dbContext.Orders.Remove(order);

            await dbContext.SaveChangesAsync();
        }

        public async Task CompleteOrder(CallOffId callOffId, string internalOrgId, int userId)
        {
            var order = (await GetOrderThin(callOffId, internalOrgId)).Order;
            order.Complete();

            await using var fullOrderStream = new MemoryStream();
            await using var patientOrderStream = new MemoryStream();

            await csvService.CreateFullOrderCsvAsync(order.Id, fullOrderStream);

            fullOrderStream.Position = 0;

            var adminTokens = new Dictionary<string, dynamic>
            {
                { OrganisationNameToken, order.OrderingParty.Name },
                { FullOrderCsvToken, NotificationClient.PrepareUpload(fullOrderStream.ToArray(), true) },
            };

            var templateId = orderMessageSettings.SingleCsvTemplateId;

            if (await csvService.CreatePatientNumberCsvAsync(order.Id, patientOrderStream) > 0)
            {
                patientOrderStream.Position = 0;
                adminTokens.Add(PatientOrderCsvToken, NotificationClient.PrepareUpload(patientOrderStream.ToArray(), true));
                templateId = orderMessageSettings.DualCsvTemplateId;
            }

            var userEmail = dbContext.Users.First(x => x.Id == userId).Email;
            using var pdfData = await pdfService.CreateOrderSummaryPdf(order);

            fullOrderStream.Position = 0;
            var userTokens = new Dictionary<string, dynamic>
            {
                { OrderIdToken, $"{callOffId}" },
                { OrderSummaryLinkToken, NotificationClient.PrepareUpload(pdfData.ToArray()) },
                { OrderSummaryCsv, NotificationClient.PrepareUpload(fullOrderStream.ToArray(), true) },
            };

            dbContext.Entry(order).State = EntityState.Modified;

            var userTemplateId = order.AssociatedServicesOnly
                ? orderMessageSettings.UserAssociatedServiceTemplateId
                : orderMessageSettings.UserTemplateId;

            await Task.WhenAll(
                emailService.SendEmailAsync(orderMessageSettings.Recipient.Address, templateId, adminTokens),
                emailService.SendEmailAsync(userEmail, userTemplateId, userTokens),
                dbContext.SaveChangesAsync());
        }

        public async Task<List<Order>> GetUserOrders(int userId)
        {
            return await dbContext.Orders
                .Where(o => o.LastUpdatedBy == userId)
                .OrderByDescending(o => o.LastUpdated)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task SetSolutionId(string internalOrgId, CallOffId callOffId, CatalogueItemId solutionId)
        {
            var order = await dbContext.Order(internalOrgId, callOffId);

            order.SolutionId = solutionId;

            await dbContext.SaveChangesAsync();
        }

        public async Task SetFundingSourceForForceFundedItems(string internalOrgId, CallOffId callOffId)
        {
            var order = await dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemPrice)
                    .ThenInclude(oip => oip.OrderItemPriceTiers)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemRecipients)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemFunding)
                .Include(o => o.SelectedFramework)
                .AsSplitQuery()
                .FirstAsync(o => o.OrderNumber == callOffId.OrderNumber
                    && o.Revision == callOffId.Revision
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            foreach (var orderItem in order.OrderItems.Where(oi => oi.OrderItemFunding is null))
            {
                var selectedFundingType = OrderItemFundingType.None;

                if (orderItem.TotalCost() == 0)
                    selectedFundingType = OrderItemFundingType.NoFundingRequired;
                else if (order.SelectedFramework.LocalFundingOnly)
                    selectedFundingType = OrderItemFundingType.LocalFundingOnly;

                if (selectedFundingType != OrderItemFundingType.None)
                {
                    var orderId = await dbContext.OrderId(internalOrgId, callOffId);

                    orderItem.OrderItemFunding = new OrderItemFunding
                    {
                        OrderId = orderId,
                        CatalogueItemId = orderItem.CatalogueItemId,
                        OrderItemFundingType = selectedFundingType,
                    };
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteSelectedFramework(string internalOrgId, CallOffId callOffId)
        {
            var order = await dbContext.Order(internalOrgId, callOffId);

            order.SelectedFrameworkId = null;

            await dbContext.SaveChangesAsync();
        }
    }
}

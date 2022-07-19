using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using Notify.Client;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderService : IOrderService
    {
        public const string OrderIdToken = "order_id";
        public const string OrderSummaryLinkToken = "order_summary_link";

        private readonly BuyingCatalogueDbContext dbContext;
        private readonly ICsvService csvService;
        private readonly IGovNotifyEmailService emailService;
        private readonly IPdfService pdfService;
        private readonly OrderMessageSettings orderMessageSettings;

        public OrderService(
            BuyingCatalogueDbContext dbContext,
            ICsvService csvService,
            IGovNotifyEmailService emailService,
            IPdfService pdfService,
            OrderMessageSettings orderMessageSettings)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            this.orderMessageSettings = orderMessageSettings ?? throw new ArgumentNullException(nameof(orderMessageSettings));
        }

        public Task<Order> GetOrderThin(CallOffId callOffId, string internalOrgId)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Supplier)
                .Include(o => o.LastUpdatedByUser)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .SingleOrDefaultAsync();
        }

        public Task<Order> GetOrderWithCatalogueItemAndPrices(CallOffId callOffId, string internalOrgId)
        {
            return dbContext.Orders
                .Where(o => o.Id == callOffId.Id
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
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
                .SingleOrDefaultAsync();
        }

        public Task<Order> GetOrderWithOrderItems(CallOffId callOffId, string internalOrgId)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .Include(x => x.Solution)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemFunding)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers.OrderBy(ip => ip.LowerRange))
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemRecipients.OrderBy(i => i.Recipient.Name))
                    .ThenInclude(r => r.Recipient)
                .AsSplitQuery()
                .SingleOrDefaultAsync();
        }

        public Task<Order> GetOrderWithOrderItemsForFunding(CallOffId callOffId, string internalOrgId)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.CatalogueItem)
                    .ThenInclude(ci => ci.Solution)
                    .ThenInclude(s => s.FrameworkSolutions)
                    .ThenInclude(fs => fs.Framework)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemFunding)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemPrice)
                    .ThenInclude(ip => ip.OrderItemPriceTiers)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.OrderItemRecipients)
                    .ThenInclude(r => r.Recipient)
                .AsSplitQuery()
                .SingleOrDefaultAsync();
        }

        public Task<Order> GetOrderWithSupplier(CallOffId callOffId, string internalOrgId)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .SingleOrDefaultAsync();
        }

        public async Task<Order> GetOrderForSummary(CallOffId callOffId, string internalOrgId)
        {
            var output = await dbContext.Orders
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Solution)
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .Include(o => o.ServiceInstanceItems)
                .Include(o => o.LastUpdatedByUser)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem).ThenInclude(ci => ci.Solution).ThenInclude(s => s.FrameworkSolutions).ThenInclude(fs => fs.Framework)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemFunding)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemPrice).ThenInclude(ip => ip.OrderItemPriceTiers.OrderBy(pt => pt.LowerRange))
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients.OrderBy(i => i.Recipient.Name)).ThenInclude(r => r.Recipient)
                .AsNoTracking()
                .AsSplitQuery()
                .SingleOrDefaultAsync(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.InternalIdentifier == internalOrgId);

            var supplier = output?.Completed.HasValue ?? false
                ? await dbContext.Suppliers.TemporalAsOf(output.Completed.Value).SingleOrDefaultAsync(x => x.Id == output.SupplierId)
                : null;

            if (supplier != null)
            {
                output.Supplier = supplier;
            }

            return output;
        }

        public async Task<PagedList<Order>> GetPagedOrders(int organisationId, PageOptions options, string search = null)
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

            if (options.PageNumber != 0)
                query = query.Skip((options.PageNumber - 1) * options.PageSize).ToList();

            var results = query.Take(options.PageSize).ToList();

            return new PagedList<Order>(results, options);
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

        public Task<Order> GetOrderSummary(CallOffId callOffId, string internalOrgId)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.SupplierContact)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients)
                .AsSplitQuery()
                .SingleOrDefaultAsync();
        }

        public Task<Order> GetOrderForStatusUpdate(CallOffId callOffId, string internalOrgId)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .Include(o => o.OrderingParty)
                .Include(o => o.Supplier)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                /*.Include(o => o.OrderItems).ThenInclude(i => i.CataloguePrice).ThenInclude(p => p.PricingUnit)*/
                .AsSplitQuery()
                .SingleOrDefaultAsync();
        }

        public async Task<Order> CreateOrder(string description, string internalOrgId, OrderTriageValue? orderTriageValue, bool isAssociatedServiceOnly)
        {
            var orderingParty = await dbContext.Organisations.SingleAsync(o => o.InternalIdentifier == internalOrgId);

            var order = new Order
            {
                Description = description,
                OrderingParty = orderingParty,
                AssociatedServicesOnly = isAssociatedServiceOnly,
                OrderTriageValue = orderTriageValue,
            };

            dbContext.Add(order);
            await dbContext.SaveChangesAsync();

            return order;
        }

        public async Task DeleteOrder(CallOffId callOffId, string internalOrgId)
        {
            var order = await dbContext.Orders
                .Where(o => o.Id == callOffId.Id
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .SingleAsync();

            order.IsDeleted = true;

            await dbContext.SaveChangesAsync();
        }

        public async Task CompleteOrder(CallOffId callOffId, string internalOrgId, int userId, Uri orderSummaryUri)
        {
            var order = await GetOrderThin(callOffId, internalOrgId);

            order.Complete();

            await using var fullOrderStream = new MemoryStream();
            await using var patientOrderStream = new MemoryStream();

            await csvService.CreateFullOrderCsvAsync(order.Id, fullOrderStream);

            fullOrderStream.Position = 0;

            var adminTokens = new Dictionary<string, dynamic>
            {
                { "organisation_name", order.OrderingParty.Name },
                { "full_order_csv", NotificationClient.PrepareUpload(fullOrderStream.ToArray(), true) },
            };

            var templateId = orderMessageSettings.SingleCsvTemplateId;

            if (await csvService.CreatePatientNumberCsvAsync(order.Id, patientOrderStream) > 0)
            {
                patientOrderStream.Position = 0;
                adminTokens.Add("patient_order_csv", NotificationClient.PrepareUpload(patientOrderStream.ToArray(), true));
                templateId = orderMessageSettings.DualCsvTemplateId;
            }

            var userEmail = dbContext.Users.Single(x => x.Id == userId).Email;
            var pdfData = pdfService.Convert(orderSummaryUri);

            var userTokens = new Dictionary<string, dynamic>
            {
                { OrderIdToken, $"{callOffId}" },
                { OrderSummaryLinkToken, NotificationClient.PrepareUpload(pdfData) },
            };

            await Task.WhenAll(
                emailService.SendEmailAsync(orderMessageSettings.Recipient.Address, templateId, adminTokens),
                emailService.SendEmailAsync(userEmail, orderMessageSettings.UserTemplateId, userTokens),
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
            var order = await dbContext.Orders
                .Where(o => o.Id == callOffId.Id
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .SingleAsync();

            order.SolutionId = solutionId;

            await dbContext.SaveChangesAsync();
        }
    }
}

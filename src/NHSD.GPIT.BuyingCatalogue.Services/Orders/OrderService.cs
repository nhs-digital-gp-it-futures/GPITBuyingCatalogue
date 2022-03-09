using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
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

        public Task<Order> GetOrderWithDefaultDeliveryDatesAndOrderItems(CallOffId callOffId, string internalOrgId)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(o => o.DefaultDeliveryDates)
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

        // TODO - Tiered Pricing - Reintroduct Pricing Information
        public Task<Order> GetOrderForSummary(CallOffId callOffId, string internalOrgId)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.InternalIdentifier == internalOrgId)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .Include(o => o.ServiceInstanceItems)
                .Include(o => o.LastUpdatedByUser)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                /*.Include(o => o.OrderItems).ThenInclude(i => i.CataloguePrice).ThenInclude(p => p.PricingUnit)*/
                .SingleOrDefaultAsync();
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
                .AsQueryable()
                .SingleOrDefaultAsync();
        }

        // TODO - Tiered Pricing - Reintroduct Pricing Data
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
                .SingleOrDefaultAsync();
        }

        public async Task<Order> CreateOrder(string description, string internalOrgId)
        {
            var orderingParty = await dbContext.Organisations.SingleAsync(o => o.InternalIdentifier == internalOrgId);

            var order = new Order
            {
                Description = description,
                OrderingParty = orderingParty,
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
    }
}

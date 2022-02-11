using System;
using System.Collections.Generic;
using System.Globalization;
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
using Notify.Client;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class OrderService : IOrderService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly ICsvService csvService;
        private readonly IGovNotifyEmailService emailService;
        private readonly OrderMessageSettings orderMessageSettings;

        public OrderService(
            BuyingCatalogueDbContext dbContext,
            ICsvService csvService,
            IGovNotifyEmailService emailService,
            OrderMessageSettings orderMessageSettings)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.orderMessageSettings = orderMessageSettings ?? throw new ArgumentNullException(nameof(orderMessageSettings));
        }

        public Task<Order> GetOrderThin(CallOffId callOffId, string odsCode)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.OdsCode == odsCode)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Supplier)
                .Include(o => o.LastUpdatedByUser)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .SingleOrDefaultAsync();
        }

        public Task<Order> GetOrderWithDefaultDeliveryDatesAndOrderItems(CallOffId callOffId, string odsCode)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.OdsCode == odsCode)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(o => o.DefaultDeliveryDates)
                .SingleOrDefaultAsync();
        }

        public Task<Order> GetOrderWithSupplier(CallOffId callOffId, string odsCode)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.OdsCode == odsCode)
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .SingleOrDefaultAsync();
        }

        public Task<Order> GetOrderForSummary(CallOffId callOffId, string odsCode)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.OdsCode == odsCode)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.Supplier)
                .Include(o => o.SupplierContact)
                .Include(o => o.ServiceInstanceItems)
                .Include(o => o.LastUpdatedByUser)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(o => o.OrderItems).ThenInclude(i => i.CataloguePrice).ThenInclude(p => p.PricingUnit)
                .SingleOrDefaultAsync();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "Formatting used in LINQ-to-SQL Queries which does not support format providers")]
        public async Task<PagedList<Order>> GetPagedOrders(int organisationId, PageOptions options, string search = null)
        {
            options ??= new PageOptions();

            var query = await dbContext.Organisations
                .Where(o => o.Id == organisationId)
                .Include(o => o.Orders).ThenInclude(o => o.LastUpdatedByUser)
                .SelectMany(o => o.Orders)
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
              .Organisations
              .Include(o => o.Orders)
              .Where(o => o.Id == organisationId)
              .SelectMany(o => o.Orders)
              .AsNoTracking()
              .ToListAsync();

            var callOffIdMatches = baseData
                .Where(o => o.CallOffId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(o => new SearchFilterModel
                {
                    Title = o.CallOffId.ToString(),
                    Category = "Call-off ID",
                });

            var descriptionMatches = baseData
                .Where(o => o.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Select(o => new SearchFilterModel
                {
                    Title = o.Description,
                    Category = "Description",
                });

            return callOffIdMatches.Union(descriptionMatches).ToList();
        }

        public Task<Order> GetOrderSummary(CallOffId callOffId, string odsCode)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.OdsCode == odsCode)
                .Include(o => o.OrderingParty)
                .Include(o => o.OrderingPartyContact)
                .Include(o => o.SupplierContact)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients)
                .AsQueryable()
                .SingleOrDefaultAsync();
        }

        public Task<Order> GetOrderForStatusUpdate(CallOffId callOffId, string odsCode)
        {
            return dbContext.Orders
                .Where(o =>
                    o.Id == callOffId.Id
                    && o.OrderingParty.OdsCode == odsCode)
                .Include(o => o.OrderingParty)
                .Include(o => o.Supplier)
                .Include(o => o.OrderItems).ThenInclude(i => i.CatalogueItem)
                .Include(o => o.OrderItems).ThenInclude(i => i.OrderItemRecipients).ThenInclude(r => r.Recipient)
                .Include(o => o.OrderItems).ThenInclude(i => i.CataloguePrice).ThenInclude(p => p.PricingUnit)
                .SingleOrDefaultAsync();
        }

        public async Task<Order> CreateOrder(string description, string odsCode)
        {
            var orderingParty = await dbContext.Organisations.SingleAsync(o => o.OdsCode == odsCode);

            var order = new Order
            {
                Description = description,
                OrderingParty = orderingParty,
            };

            dbContext.Add(order);
            await dbContext.SaveChangesAsync();

            return order;
        }

        public async Task DeleteOrder(CallOffId callOffId, string odsCode)
        {
            var order = await dbContext.Orders.Where(o => o.Id == callOffId.Id && o.OrderingParty.OdsCode == odsCode).SingleAsync();

            if (order != null)
            {
                order.IsDeleted = true;

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task CompleteOrder(CallOffId callOffId, string odsCode)
        {
            var order = await GetOrderThin(callOffId, odsCode);

            order.Complete();

            await using var fullOrderStream = new MemoryStream();
            await using var patientOrderStream = new MemoryStream();

            await csvService.CreateFullOrderCsvAsync(order.Id, fullOrderStream);

            fullOrderStream.Position = 0;

            var personalisation = new Dictionary<string, dynamic>
            {
                { "organisation_name", order.OrderingParty.Name },
                { "full_order_csv", NotificationClient.PrepareUpload(fullOrderStream.ToArray(), true) },
            };

            var templateId = orderMessageSettings.SingleCsvTemplateId;
            if (await csvService.CreatePatientNumberCsvAsync(order.Id, patientOrderStream) > 0)
            {
                patientOrderStream.Position = 0;
                personalisation.Add("patient_order_csv", NotificationClient.PrepareUpload(patientOrderStream.ToArray(), true));
                templateId = orderMessageSettings.DualCsvTemplateId;
            }

            await emailService.SendEmailAsync(
                orderMessageSettings.Recipient.Address,
                templateId,
                personalisation);

            await dbContext.SaveChangesAsync();
        }
    }
}
